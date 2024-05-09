using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class agregamosLocalAdherido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalAdherido",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalAdherido",
                table: "AspNetUsers");
        }
    }
}
