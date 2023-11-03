using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    public partial class AddPetBreed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Breed",
                table: "Pets");

            migrationBuilder.AddColumn<bool>(
                name: "IsNeutered",
                table: "Pets",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PetBreedId",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PetBreeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetBreeds", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PetBreeds",
                columns: new[] { "Id", "Name", "Size", "Type" },
                values: new object[,]
                {
                    { 1, "千島短尾貓", 0, 2 },
                    { 2, "土耳其安哥拉貓", 0, 2 },
                    { 3, "土耳其梵貓", 0, 2 },
                    { 4, "巴比諾貓", 0, 2 },
                    { 5, "巴西短毛貓", 0, 2 },
                    { 6, "日本短尾貓", 0, 2 },
                    { 7, "爪哇貓", 0, 2 },
                    { 8, "加州閃亮貓", 0, 2 },
                    { 9, "北美洲短毛貓", 0, 2 },
                    { 10, "卡特爾貓", 0, 2 },
                    { 11, "尼比龍貓", 0, 2 },
                    { 12, "尼亞貓", 0, 2 },
                    { 13, "布偶貓", 0, 2 },
                    { 14, "西伯利亞貓", 0, 2 },
                    { 15, "伯曼貓", 0, 2 },
                    { 16, "亞洲半長毛貓", 0, 2 },
                    { 17, "亞洲貓", 0, 2 },
                    { 18, "奇多貓", 0, 2 },
                    { 19, "孟加拉貓", 0, 2 },
                    { 20, "孟買貓", 0, 2 },
                    { 21, "彼得禿貓", 0, 2 },
                    { 22, "拉邦貓", 0, 2 },
                    { 23, "東方長毛貓", 0, 2 },
                    { 24, "東方短毛貓", 0, 2 },
                    { 25, "東方雙色貓", 0, 2 },
                    { 26, "東奇尼貓", 0, 2 },
                    { 27, "波米拉貓", 0, 2 },
                    { 28, "波斯貓", 0, 2 },
                    { 29, "玩具虎貓", 0, 2 },
                    { 30, "肯尼亞貓", 0, 2 },
                    { 31, "俄勒岡捲毛貓", 0, 2 },
                    { 32, "俄羅斯白貓、黑貓和虎斑貓", 0, 2 },
                    { 33, "俄羅斯藍貓", 0, 2 },
                    { 34, "哈瓦那棕貓", 0, 2 },
                    { 35, "威爾斯貓", 0, 2 },
                    { 36, "峇里貓", 0, 2 },
                    { 37, "查達利貓", 0, 2 },
                    { 38, "科拉特貓", 0, 2 },
                    { 39, "美國多趾貓", 0, 2 },
                    { 40, "美國短尾貓", 0, 2 },
                    { 41, "美國硬毛貓", 0, 2 },
                    { 42, "米克斯貓", 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "PetBreeds",
                columns: new[] { "Id", "Name", "Size", "Type" },
                values: new object[,]
                {
                    { 43, "三色貓", 0, 2 },
                    { 44, "明斯欽貓", 0, 2 },
                    { 45, "美國反耳貓", 0, 2 },
                    { 46, "美國短毛貓", 0, 2 },
                    { 47, "約克巧克力貓", 0, 2 },
                    { 48, "英國長毛貓", 0, 2 },
                    { 49, "英國短毛貓", 0, 2 },
                    { 50, "重點色短毛貓", 0, 2 },
                    { 51, "埃及貓", 0, 2 },
                    { 52, "拿破崙貓", 0, 2 },
                    { 53, "挪威森林貓", 0, 2 },
                    { 54, "烏克蘭勒夫科伊貓", 0, 2 },
                    { 55, "烏蘇里貓", 0, 2 },
                    { 56, "狸花貓", 0, 2 },
                    { 57, "索馬利貓", 0, 2 },
                    { 58, "馬恩島貓", 0, 2 },
                    { 59, "康沃耳帝王貓", 0, 2 },
                    { 60, "曼切堪貓", 0, 2 },
                    { 61, "異國短毛貓", 0, 2 },
                    { 62, "雪鞋貓", 0, 2 },
                    { 63, "喜馬拉雅貓", 0, 2 },
                    { 64, "斯芬克斯貓", 0, 2 },
                    { 65, "短毛家貓", 0, 2 },
                    { 66, "傳統暹羅貓", 0, 2 },
                    { 67, "塞倫蓋蒂貓", 0, 2 },
                    { 68, "塞浦路斯短毛貓", 0, 2 },
                    { 69, "塞爾凱克鬈毛貓", 0, 2 },
                    { 70, "愛琴海貓", 0, 2 },
                    { 71, "新加坡貓", 0, 2 },
                    { 72, "獅子貓", 0, 2 },
                    { 73, "頓斯科伊貓", 0, 2 },
                    { 74, "德文帝王貓", 0, 2 },
                    { 75, "德國捲毛貓", 0, 2 },
                    { 76, "歐西貓", 0, 2 },
                    { 77, "歐洲短毛貓", 0, 2 },
                    { 78, "歐斯亞史烈斯貓", 0, 2 },
                    { 79, "熱帶草原貓", 0, 2 },
                    { 80, "緬因貓", 0, 2 },
                    { 81, "緬甸貓", 0, 2 },
                    { 82, "暹羅貓", 0, 2 },
                    { 83, "澳大利亞霧貓", 0, 2 },
                    { 84, "蘇格蘭摺耳貓", 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "PetBreeds",
                columns: new[] { "Id", "Name", "Size", "Type" },
                values: new object[,]
                {
                    { 85, "襤褸貓", 0, 2 },
                    { 86, "哈士奇", 3, 1 },
                    { 87, "拉不拉多", 3, 1 },
                    { 88, "黃金獵犬", 3, 1 },
                    { 89, "鬆獅犬", 3, 1 },
                    { 90, "英國古代牧羊犬", 3, 1 },
                    { 91, "德國狼犬", 3, 1 },
                    { 92, "秋田犬", 3, 1 },
                    { 93, "聖伯納犬", 3, 1 },
                    { 94, "藏獒", 3, 1 },
                    { 95, "標準型貴賓", 3, 1 },
                    { 96, "柴犬", 2, 1 },
                    { 97, "米克斯", 2, 1 },
                    { 98, "米格魯", 2, 1 },
                    { 99, "柯基", 2, 1 },
                    { 100, "台灣犬", 2, 1 },
                    { 101, "薩摩耶犬", 2, 1 },
                    { 102, "可卡", 2, 1 },
                    { 103, "邊境牧羊犬", 2, 1 },
                    { 104, "沙皮狗", 2, 1 },
                    { 105, "英國鬥牛犬", 2, 1 },
                    { 106, "吉娃娃", 1, 1 },
                    { 107, "約克夏", 1, 1 },
                    { 108, "博美", 1, 1 },
                    { 109, "瑪爾濟斯", 1, 1 },
                    { 110, "迷你貴賓犬", 1, 1 },
                    { 111, "迷你臘腸犬", 1, 1 },
                    { 112, "迷你雪納瑞", 1, 1 },
                    { 113, "西施", 1, 1 },
                    { 114, "巴哥犬", 1, 1 },
                    { 115, "狐狸犬", 1, 1 },
                    { 116, "法國鬥牛犬", 1, 1 },
                    { 117, "蝴蝶犬", 1, 1 },
                    { 118, "迷你杜賓犬", 1, 1 },
                    { 119, "比熊犬", 1, 1 },
                    { 120, "北京犬", 1, 1 },
                    { 121, "傑克羅素㹴", 1, 1 },
                    { 122, "鋼毛獵狐㹴", 1, 1 },
                    { 123, "喜樂蒂牧羊犬", 1, 1 },
                    { 124, "標準臘腸犬", 1, 1 },
                    { 125, "牛頭㹴", 1, 1 },
                    { 126, "標準雪納瑞", 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "PetBreeds",
                columns: new[] { "Id", "Name", "Size", "Type" },
                values: new object[] { 127, "杜賓犬", 1, 1 });

            migrationBuilder.InsertData(
                table: "PetBreeds",
                columns: new[] { "Id", "Name", "Size", "Type" },
                values: new object[] { 128, "西高地白㹴", 1, 1 });

            migrationBuilder.InsertData(
                table: "PetBreeds",
                columns: new[] { "Id", "Name", "Size", "Type" },
                values: new object[] { 129, "其他", 0, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_PetBreedId",
                table: "Pets",
                column: "PetBreedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetBreeds_PetBreedId",
                table: "Pets",
                column: "PetBreedId",
                principalTable: "PetBreeds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetBreeds_PetBreedId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "PetBreeds");

            migrationBuilder.DropIndex(
                name: "IX_Pets_PetBreedId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "IsNeutered",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetBreedId",
                table: "Pets");

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
