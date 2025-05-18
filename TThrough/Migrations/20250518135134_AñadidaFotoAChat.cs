using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TThrough.Migrations
{
    /// <inheritdoc />
    public partial class AñadidaFotoAChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FotoChat",
                table: "Chats",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoChat",
                table: "Chats");
        }
    }
}
