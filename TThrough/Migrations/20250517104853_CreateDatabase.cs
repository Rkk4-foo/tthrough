using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TThrough.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    IdChat = table.Column<string>(type: "varchar(40)", nullable: false),
                    NombreChat = table.Column<string>(type: "varchar(40)", nullable: false),
                    FechaInicioChat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.IdChat);
                });

            migrationBuilder.CreateTable(
                name: "Llamadas",
                columns: table => new
                {
                    IdLlamada = table.Column<string>(type: "varchar(40)", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Llamadas", x => x.IdLlamada);
                });

            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    IdMensaje = table.Column<string>(type: "varchar(40)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.IdMensaje);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<string>(type: "varchar(40)", nullable: false),
                    FotoPerfil = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    NombreUsuario = table.Column<string>(type: "varchar(25)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombrePublico = table.Column<string>(type: "varchar(25)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Amigos",
                columns: table => new
                {
                    IdUsuarioEnvio = table.Column<string>(type: "varchar(40)", nullable: false),
                    IdUsuarioRemitente = table.Column<string>(type: "varchar(40)", nullable: false),
                    SolicitudAceptada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amigos", x => new { x.IdUsuarioEnvio, x.IdUsuarioRemitente });
                    table.ForeignKey(
                        name: "FK_Amigos_Usuarios_IdUsuarioEnvio",
                        column: x => x.IdUsuarioEnvio,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amigos_Usuarios_IdUsuarioRemitente",
                        column: x => x.IdUsuarioRemitente,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatsUsuarios",
                columns: table => new
                {
                    IdChat = table.Column<string>(type: "varchar(40)", nullable: false),
                    IdUsuario = table.Column<string>(type: "varchar(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsUsuarios", x => new { x.IdChat, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_ChatsUsuarios_Chats_IdChat",
                        column: x => x.IdChat,
                        principalTable: "Chats",
                        principalColumn: "IdChat",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsUsuarios_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LlamadasUsuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<string>(type: "varchar(40)", nullable: false),
                    IdLlamada = table.Column<string>(type: "varchar(40)", nullable: false),
                    FechaLlamada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LlamadasUsuarios", x => new { x.IdUsuario, x.IdLlamada });
                    table.ForeignKey(
                        name: "FK_LlamadasUsuarios_Llamadas_IdLlamada",
                        column: x => x.IdLlamada,
                        principalTable: "Llamadas",
                        principalColumn: "IdLlamada",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LlamadasUsuarios_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensajesUsuarios",
                columns: table => new
                {
                    IdMensaje = table.Column<string>(type: "varchar(40)", nullable: false),
                    IdUsuario = table.Column<string>(type: "varchar(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesUsuarios", x => new { x.IdMensaje, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_MensajesUsuarios_Mensajes_IdMensaje",
                        column: x => x.IdMensaje,
                        principalTable: "Mensajes",
                        principalColumn: "IdMensaje",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MensajesUsuarios_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amigos_IdUsuarioRemitente",
                table: "Amigos",
                column: "IdUsuarioRemitente");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsUsuarios_IdUsuario",
                table: "ChatsUsuarios",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_LlamadasUsuarios_IdLlamada",
                table: "LlamadasUsuarios",
                column: "IdLlamada");

            migrationBuilder.CreateIndex(
                name: "IX_MensajesUsuarios_IdUsuario",
                table: "MensajesUsuarios",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amigos");

            migrationBuilder.DropTable(
                name: "ChatsUsuarios");

            migrationBuilder.DropTable(
                name: "LlamadasUsuarios");

            migrationBuilder.DropTable(
                name: "MensajesUsuarios");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Llamadas");

            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
