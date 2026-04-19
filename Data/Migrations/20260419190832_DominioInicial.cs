using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practica2_Teoria.Data.Migrations
{
    /// <inheritdoc />
    public partial class DominioInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matriculas_AspNetUsers_UsuarioId1",
                table: "Matriculas");

            migrationBuilder.DropIndex(
                name: "IX_Matriculas_UsuarioId1",
                table: "Matriculas");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Matriculas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId1",
                table: "Matriculas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_UsuarioId1",
                table: "Matriculas",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Matriculas_AspNetUsers_UsuarioId1",
                table: "Matriculas",
                column: "UsuarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
