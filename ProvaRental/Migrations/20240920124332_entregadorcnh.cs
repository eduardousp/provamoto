using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProvaRental.Migrations
{
    /// <inheritdoc />
    public partial class entregadorcnh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locacoes_Entregadores_EntregadorId",
                table: "Locacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Locacoes_Motos_MotoId",
                table: "Locacoes");

            migrationBuilder.DropIndex(
                name: "IX_Locacoes_EntregadorId",
                table: "Locacoes");

            migrationBuilder.DropIndex(
                name: "IX_Locacoes_MotoId",
                table: "Locacoes");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "Locacoes");

            migrationBuilder.RenameColumn(
                name: "DataTermino",
                table: "Locacoes",
                newName: "DataDevolucao");

            migrationBuilder.RenameColumn(
                name: "Cnpj",
                table: "Entregadores",
                newName: "CNPJ");

            migrationBuilder.RenameColumn(
                name: "TipoCNH",
                table: "Entregadores",
                newName: "CNHTipo");

            migrationBuilder.RenameColumn(
                name: "NumeroCNH",
                table: "Entregadores",
                newName: "CNHNumero");

            migrationBuilder.AddColumn<int>(
                name: "Plano",
                table: "Locacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plano",
                table: "Locacoes");

            migrationBuilder.RenameColumn(
                name: "DataDevolucao",
                table: "Locacoes",
                newName: "DataTermino");

            migrationBuilder.RenameColumn(
                name: "CNPJ",
                table: "Entregadores",
                newName: "Cnpj");

            migrationBuilder.RenameColumn(
                name: "CNHTipo",
                table: "Entregadores",
                newName: "TipoCNH");

            migrationBuilder.RenameColumn(
                name: "CNHNumero",
                table: "Entregadores",
                newName: "NumeroCNH");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "Locacoes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_EntregadorId",
                table: "Locacoes",
                column: "EntregadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_MotoId",
                table: "Locacoes",
                column: "MotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locacoes_Entregadores_EntregadorId",
                table: "Locacoes",
                column: "EntregadorId",
                principalTable: "Entregadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Locacoes_Motos_MotoId",
                table: "Locacoes",
                column: "MotoId",
                principalTable: "Motos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
