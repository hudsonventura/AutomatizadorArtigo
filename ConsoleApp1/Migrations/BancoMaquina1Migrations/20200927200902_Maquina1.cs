using Microsoft.EntityFrameworkCore.Migrations;

namespace Aumatizador.Migrations.BancoMaquina1Migrations
{
    public partial class Maquina1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "imdb",
                columns: table => new
                {
                    nconst = table.Column<string>(nullable: false),
                    primaryName = table.Column<string>(nullable: true),
                    birthYear = table.Column<int>(nullable: false),
                    deathYear = table.Column<int>(nullable: false),
                    primaryProfession = table.Column<string>(nullable: true),
                    knownForTitles = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imdb", x => x.nconst);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imdb");
        }
    }
}
