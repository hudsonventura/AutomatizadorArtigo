﻿// <auto-generated />
using Aumatizador;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Aumatizador.Migrations.BancoMaquina1Migrations
{
    [DbContext(typeof(BancoMaquina1))]
    [Migration("20200927200902_Maquina1")]
    partial class Maquina1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Aumatizador.IMDB", b =>
                {
                    b.Property<string>("nconst")
                        .HasColumnType("text");

                    b.Property<int>("birthYear")
                        .HasColumnType("integer");

                    b.Property<int>("deathYear")
                        .HasColumnType("integer");

                    b.Property<string>("knownForTitles")
                        .HasColumnType("text");

                    b.Property<string>("primaryName")
                        .HasColumnType("text");

                    b.Property<string>("primaryProfession")
                        .HasColumnType("text");

                    b.HasKey("nconst");

                    b.ToTable("imdb");
                });
#pragma warning restore 612, 618
        }
    }
}
