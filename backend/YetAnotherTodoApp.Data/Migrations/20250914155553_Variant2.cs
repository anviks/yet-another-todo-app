using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YetAnotherTodoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Variant2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Tasks",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Tasks",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
