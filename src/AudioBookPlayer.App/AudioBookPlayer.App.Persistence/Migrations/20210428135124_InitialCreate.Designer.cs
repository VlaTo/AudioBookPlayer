﻿// <auto-generated />

using System;
using AudioBookPlayer.App.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AudioBookPlayer.App.Data.Migrations
{
    [DbContext(typeof(SqLiteDbContext))]
    [Migration("20210428135124_InitialCreate")]
    public partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            /*modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .Entity("AudioBookPlayer.App.Data.Models.Book", b =>
                {
                    b.Property<int>(nameof(Book.Id)).ValueGeneratedOnAdd();
                    b.Property<string>(nameof(Book.Title)).IsRequired();
                    b.Property<string>(nameof(Book.Description));
                    b.Property<bool>(nameof(Book.IsExcluded));
                    b.Property<DateTime>(nameof(Book.AddedToLibrary));

                    b.HasKey(nameof(Book.Id));
                    b.ToTable("books");

                    b.HasOne("AudioBookPlayer.App.Data.Models.SourceFile", "Author")
                        .WithMany("Addresses")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);
                })
                .Entity("AudioBookPlayer.App.Data.Models.SourceFile", b =>
                {
                    b.Property<int>(nameof(SourceFile.Id)).ValueGeneratedOnAdd();
                    b.Property<int>(nameof(SourceFile.BookId)).IsRequired();
                    b.Property<string>(nameof(SourceFile.Source));
                    b.Property<DateTime>(nameof(SourceFile.Created));
                    b.Property<DateTime>(nameof(SourceFile.Modified));
                    b.Property<long>(nameof(SourceFile.Length));

                    b.HasKey(nameof(SourceFile.Id));
                    b.HasIndex(nameof(SourceFile.BookId));
                    b.ToTable("sources");

                    b.HasOne("AudioBookPlayer.App.Data.Models.Book", nameof(SourceFile.Book))
                        .WithMany(nameof(Book.SourceFiles))
                        .HasForeignKey(nameof(SourceFile.BookId))
                        .OnDelete(DeleteBehavior.Cascade);
                });*/
#pragma warning restore 612, 618
        }
    }
}
