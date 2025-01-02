using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregamosTitulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TitulosConectados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitulosConectados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TitulosDesconectados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitulosDesconectados", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TitulosConectados");

            migrationBuilder.DropTable(
                name: "TitulosDesconectados");
        }
    }
}
