using Microsoft.EntityFrameworkCore.Migrations;

namespace AvaliacaoDesempenho.Data.Migrations
{
    public partial class AddVisivelProf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VisivelProf",
                table: "Avaliacoes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisivelProf",
                table: "Avaliacoes");
        }
    }
}
