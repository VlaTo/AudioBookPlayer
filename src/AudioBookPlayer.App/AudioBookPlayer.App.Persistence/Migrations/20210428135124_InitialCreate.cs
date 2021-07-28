using System;
using AudioBookPlayer.App.Persistence.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AudioBookPlayer.App.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "authors",
                table => new
                {
                    Id = table
                        .Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                "books",
                table => new
                {
                    Id = table
                        .Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: false),
                    Synopsis = table.Column<string>(nullable: true),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    DoNotShow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        name: "PK_books",
                        columns: x => x.Id
                    );
                }
            );

            migrationBuilder.CreateTable(
                "sources",
                table => new
                {
                    Id = table
                        .Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<long>(nullable: false),
                    Filename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        name: "PK_sources",
                        columns: x => x.Id
                    );
                    table.ForeignKey(
                        name: "FK_sources_books_Id",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: nameof(Book.Id),
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                "chapters",
                table => new
                {
                    Id = table
                        .Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Position = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    BookId = table.Column<long>(nullable: false),
                    SourceFileId = table.Column<long>(nullable: false),
                    Offset = table.Column<TimeSpan>(nullable: false),
                    Length = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        name: "PK_chapters",
                        columns: x => x.Id
                    );
                    table.ForeignKey(
                        name: "FK_chapters_book_Id",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: nameof(Book.Id),
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_chapters_sourcefile_Id",
                        column: x => x.SourceFileId,
                        principalTable: "sources",
                        principalColumn: nameof(SourceFile.Id),
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "author-books",
                columns: table => new
                {
                    AuthorId = table.Column<long>(nullable: false),
                    BookId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_author_books_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_author_books_BookId",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "book-images",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    BookId = table.Column<long>(nullable: false),
                    Blob = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        name: "PK_Book-images",
                        columns: x => x.Id
                    );
                    table.ForeignKey(
                        name: "FK_Book-images_BookId",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            /*migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    City = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: false),
                    AddressTypes = table.Column<int>(nullable: false),
                    AuthorId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    AuthorId = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stories_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(nullable: false),
                    AuthorId = table.Column<long>(nullable: false),
                    StoryId = table.Column<long>(nullable: false),
                    ParentId = table.Column<long>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Featured",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StoryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Featured", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Featured_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });*/

            migrationBuilder.CreateIndex(
                name: "IX_sources_BooksId",
                table: "sources",
                columns: new[] {"Id", "BookId"}
            );

            migrationBuilder.CreateIndex(
                name: "IX_Book-images_BookId",
                table: "book-images",
                columns: new[] {"Id", "BookId"}
            );

            migrationBuilder.CreateIndex(
                name: "IX_chapters_Position",
                table: "chapters",
                columns: new[] {"Position"}
            );

            /*migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_StoryId",
                table: "Comments",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Featured_StoryId",
                table: "Featured",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Name",
                table: "Settings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_AuthorId",
                table: "Stories",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_Slug",
                table: "Stories",
                column: "Slug",
                unique: true);*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "sources");
            migrationBuilder.DropTable(name: "books");
        }
    }
}
