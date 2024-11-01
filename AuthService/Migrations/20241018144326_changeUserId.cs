using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class changeUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, add a temporary GUID column to hold the new Id values
            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.NewGuid());

            // Populate the new Id values with GUIDs for existing records
            migrationBuilder.Sql("UPDATE \"Users\" SET \"NewId\" = gen_random_uuid() WHERE \"NewId\" IS NULL");

            // Drop the old Id column
            migrationBuilder.DropColumn(name: "Id", table: "Users");

            // Rename the NewId column to Id
            migrationBuilder.RenameColumn(name: "NewId", table: "Users", newName: "Id");

            // Alter the column to ensure it's a non-nullable GUID
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false);

            // Update the Username column constraints
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Add new fields
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true); // Made nullable to avoid issues on existing data

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new columns added
            migrationBuilder.DropColumn(name: "CreatedAt", table: "Users");
            migrationBuilder.DropColumn(name: "Email", table: "Users");
            migrationBuilder.DropColumn(name: "IsActive", table: "Users");
            migrationBuilder.DropColumn(name: "PhoneNumber", table: "Users");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "Users");

            // Restore the Id column as integer
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // Restore Username column constraints
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
