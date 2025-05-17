using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TThrough.Migrations
{
    /// <inheritdoc />
    public partial class AñadirClaveChatAMensaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdChat",
                table: "Mensajes",
                type: "varchar(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdChat",
                table: "Mensajes",
                column: "IdChat");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Chats_IdChat",
                table: "Mensajes",
                column: "IdChat",
                principalTable: "Chats",
                principalColumn: "IdChat",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Chats_IdChat",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_IdChat",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "IdChat",
                table: "Mensajes");
        }
    }
}
