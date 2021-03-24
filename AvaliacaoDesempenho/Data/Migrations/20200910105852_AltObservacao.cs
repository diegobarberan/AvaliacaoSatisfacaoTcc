using Microsoft.EntityFrameworkCore.Migrations;

namespace AvaliacaoDesempenho.Data.Migrations
{
    public partial class AltObservacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Avaliacoes");

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Avaliacoes",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Avaliacoes");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Avaliacoes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
