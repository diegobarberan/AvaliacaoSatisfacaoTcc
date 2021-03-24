using Microsoft.EntityFrameworkCore.Migrations;

namespace AvaliacaoDesempenho.Data.Migrations
{
    public partial class AddNivelCurso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nivel",
                table: "Cursos",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Cursos");
        }
    }
}
