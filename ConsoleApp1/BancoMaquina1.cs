using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Aumatizador
{
    class BancoMaquina1 : DbContext
    {
        public DbSet<IMDB> imdb { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql("Server=192.168.18.235;Port=5432;User Id=artigo;Password=12345#;Database=artigo;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //modelBuilder.Entity<AssocProdutoFornecedor>().HasKey(assoc => new { assoc.produtoId, assoc.fornecedorId});
        }
    }
}
