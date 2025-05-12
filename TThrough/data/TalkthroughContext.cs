using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using TThrough.mvvm.Models;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TThrough.data
{
    public class TalkthroughContext(DbContextOptions<TalkthroughContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Mensaje> Mensajes { get; set; }

        public DbSet<Llamada> Llamadas { get; set; }

        public DbSet<LlamadaUsuario> LlamadasUsuarios { get; set; }

        public DbSet<MensajeUsuario> MensajesUsuarios { get; set; }

        public DbSet<Amigos> Amigos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Amigos>()
            .HasKey(a => new { a.IdUsuarioEnvio, a.IdUsuarioRemitente });

            modelBuilder.Entity<Amigos>()
                .HasOne(a => a.UsuarioPeticion)
                .WithMany() // o .WithMany(u => u.PeticionesEnviadas) si defines colección
                .HasForeignKey(a => a.IdUsuarioEnvio)
                .OnDelete(DeleteBehavior.Restrict); // o Cascade, SetNull

            modelBuilder.Entity<Amigos>()
                .HasOne(a => a.UsuarioRemitente)
                .WithMany() // o .WithMany(u => u.PeticionesRecibidas)
                .HasForeignKey(a => a.IdUsuarioRemitente)
                .OnDelete(DeleteBehavior.Restrict); // Esto evita eliminación en cascada circular
        }

    }
}
