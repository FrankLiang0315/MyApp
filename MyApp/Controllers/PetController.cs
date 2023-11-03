using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Model;
using MyApp.Model.Auth;
using MyApp.Model.Pet;
using MyApp.Model.Pet.View;
using MyApp.Model.PetBreed;
using NuGet.Versioning;

namespace MyApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : InfoController
    {
        private readonly ApplicationDbContext _context;

        public PetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pet/list
        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<PetListVM>>> GetPets()
        {
            var userInfo = getUserInfo();
            var pet = await _context.Pets.Where((p) => p.UserId == userInfo.UserId)
                .Select((p) => new PetListVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    DailyGrams = p.DailyGrams
                }).ToListAsync();


            return Ok(new Response<List<PetListVM>> { Status = "Success", Data = pet });
        }

        [HttpGet]
        [Route("breed-list")]
        public async Task<ActionResult<Response<List<PetBreed>>>> GetPetBreeds()
        {
            var data = await _context.PetBreeds.ToListAsync();
            return Ok(new Response<List<PetBreed>> { Status = "Success", Data = data });
        }


        // GET: Pet/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPet(int id)
        {
            var userInfo = getUserInfo();
            var pet = await _context.Pets.Where((p) => p.Id == id && p.UserId == userInfo.UserId)
                .Select((p) =>
                    new PetVM
                    {
                        Id = p.Id,
                        PetBreadId = p.PetBreedId,
                        Breed = p.PetBreed.Name,
                        Name = p.Name,
                        Birthday = p.Birthday,
                        Gender = p.Gender,
                        Weight = p.Weight,
                        IsNeutered = p.IsNeutered,
                        Activity = p.Activity
                    }
                ).FirstOrDefaultAsync();

            if (pet == null)
            {
                return NotFound();
            }

            return Ok(new Response<PetVM> { Status = "Success", Data = pet });
        }


        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Response<PetVM>>> CreatePet(Pet pet)
        {
            var userInfo = getUserInfo();


            pet.UserId = userInfo?.UserId;
            pet.DailyGrams = await calDailyGrams(pet);

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return Ok(new Response<PetVM>
            {
                Status = "Success", Data = new PetVM
                {
                    Breed = pet.PetBreed.Name,
                    Name = pet.Name,
                    Birthday = pet.Birthday,
                    Gender = pet.Gender,
                    Weight = pet.Weight,
                    IsNeutered = pet.IsNeutered,
                    Activity = pet.Activity
                }
            });
        }


        // PUT: Pet/5
        [HttpPost]
        [Route("update")]
        public async Task<ActionResult<Pet>> PutPet(Pet pet)
        {
            _context.Entry(pet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(pet.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: Pet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            if (_context.Pets == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PetExists(int id)
        {
            return (_context.Pets?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private UserInfo? getUserInfo()
        {
            if (HttpContext.Items.TryGetValue("UserInfo", out var userInfo) && userInfo is UserInfo)
            {
                return ((UserInfo)userInfo);
            }

            return null;
        }

        // [HttpGet]
        // [Route("test")]
        // public async Task<ActionResult<Object>> Test()
        // {
        //     var pets = await _context.Pets.Include(p => p.PetBreed).ToListAsync();
        //     if (pets == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     foreach (var pet in pets)
        //     {
        //         pet.DailyGrams = calDailyGrams(pet);
        //         _context.Update(pet);
        //     }
        //
        //
        //     await _context.SaveChangesAsync();
        //
        //     return Ok(new Response<Object>
        //     {
        //         Status = "Success", Data = pets
        //     });
        // }

        private async Task<int> calDailyGrams(Pet pet)
        {
            int sizeEffect = 0;
            double activityEffect = 0;
            double neuteredEffect = (bool)pet.IsNeutered ? 0.9 : 1.0;

            PetBreed? breed = await _context.PetBreeds.FindAsync(pet.PetBreedId);
            // 基於犬的大小，分配不同的 sizeEffect
            switch (breed?.Size)
            {
                case PetBreed.SizeType.Small: // 小型犬
                    sizeEffect = 70;
                    break;
                case PetBreed.SizeType.Medium: // 中型犬
                    sizeEffect = 95;
                    break;
                case PetBreed.SizeType.Big: // 大型犬
                    sizeEffect = 110;
                    break;
                default:
                    sizeEffect = 70;
                    break;
            }

            // 基於活動水平，分配不同的 activityEffect
            switch (pet.Activity)
            {
                case Pet.ActivityType.Level1: // 在家運動
                    activityEffect = 1.2;
                    break;
                case Pet.ActivityType.Level2: // 每天戶外1小時以下
                    activityEffect = 1.4;
                    break;
                case Pet.ActivityType.Level3: // 每天戶外1小時到3小時
                    activityEffect = 1.6;
                    break;
                case Pet.ActivityType.Level4: // 每天戶外3小時
                    activityEffect = 2.0;
                    break;
            }

            // 每日食量公式(g)
            double weight = double.Parse(pet.Weight);
            double bmr = 70 * Math.Pow(weight, 0.75);
            double dailyGrams = Math.Round((bmr * sizeEffect * activityEffect * neuteredEffect) / 30, 0);

            return Convert.ToInt32(dailyGrams);
        }
    }
}