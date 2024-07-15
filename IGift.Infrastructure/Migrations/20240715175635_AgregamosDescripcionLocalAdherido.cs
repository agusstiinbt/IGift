using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregamosDescripcionLocalAdherido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "LocalesAdheridos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "LocalesAdheridos");
        }
    }
}
