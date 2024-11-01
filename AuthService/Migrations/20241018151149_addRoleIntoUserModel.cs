using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class addRoleIntoUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adding the Roles column with a default empty array to prevent null values
            migrationBuilder.AddColumn<string[]>(
                name: "Roles",
                table: "Users",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]); // Set default value as an empty array
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Users");
        }
    }
}
