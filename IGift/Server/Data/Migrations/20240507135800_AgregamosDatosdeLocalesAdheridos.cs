using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregamosDatosdeLocalesAdheridos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoLocalesAdheridos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLocalAdherido = table.Column<int>(type: "int", nullable: false),
                    LocalAdheridoId = table.Column<int>(type: "int", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoLocalesAdheridos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfoLocalesAdheridos_LocalesAdheridos_LocalAdheridoId",
                        column: x => x.LocalAdheridoId,
                        principalTable: "LocalesAdheridos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfoLocalesAdheridos_LocalAdheridoId",
                table: "InfoLocalesAdheridos",
                column: "LocalAdheridoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoLocalesAdheridos");
        }
    }
}
