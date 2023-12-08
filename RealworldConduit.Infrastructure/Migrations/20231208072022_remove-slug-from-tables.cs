using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealworldConduit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeslugfromtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Slug",
                schema: "user",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Blog_Slug",
                schema: "blog",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "user",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "blog",
                table: "Blog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "user",
                table: "User",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "blog",
                table: "Blog",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_User_Slug",
                schema: "user",
                table: "User",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Slug",
                schema: "blog",
                table: "Blog",
                column: "Slug",
                unique: true);
        }
    }
}
