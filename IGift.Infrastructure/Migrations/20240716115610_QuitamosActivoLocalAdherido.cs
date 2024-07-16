﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class QuitamosActivoLocalAdherido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "LocalesAdheridos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "LocalesAdheridos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
