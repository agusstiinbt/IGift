using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregamosFinalizadoAPeticiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                schema: "dbo",
                table: "Pedidos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finalizado",
                schema: "dbo",
                table: "Pedidos");
        }
    }
}
