using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApp.Model.Order;
using MyApp.Model.OrderItem;
using MyApp.Model.Pet;
using MyApp.Model.PetBreed;
using MyApp.Model.User;

namespace MyApp.Data;

public class ApplicationDbContext: IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        DataSeeder.DataSeeder.PetBreedSeeder(builder);
    }
    
    public DbSet<Pet> Pets { get; set; }
    public DbSet<PetBreed> PetBreeds { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems  { get; set; }
}