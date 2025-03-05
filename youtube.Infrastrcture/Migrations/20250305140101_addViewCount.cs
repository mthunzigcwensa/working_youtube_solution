using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace youtube.Infrastrcture.Migrations
{
    /// <inheritdoc />
    public partial class addViewCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "viewCount",
                table: "Videos",
                type: "int",
                maxLength: 255,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "viewCount",
                table: "Videos");
        }
    }
}
