using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Aumatizador
{
    class BancoEstatistica : DbContext
    {
        public DbSet<Estatisica> estatisticas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;User Id=artigo;Password=12345#;Database=artigoEstatisticas;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //modelBuilder.Entity<AssocProdutoFornecedor>().HasKey(assoc => new { assoc.produtoId, assoc.fornecedorId});
        }
    }
}
