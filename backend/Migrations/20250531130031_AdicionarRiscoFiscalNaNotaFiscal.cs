using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TributoJustoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRiscoFiscalNaNotaFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RiscoFiscal",
                table: "NotasFiscais",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RiscoFiscal",
                table: "NotasFiscais");
        }
    }
}
