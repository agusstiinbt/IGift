using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregamosReferenciasDeGIftCardsAUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalAdherido",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "GiftCards",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdLocalAdherido",
                table: "GiftCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GiftCards_ApplicationUserId",
                table: "GiftCards",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiftCards_AspNetUsers_ApplicationUserId",
                table: "GiftCards",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiftCards_AspNetUsers_ApplicationUserId",
                table: "GiftCards");

            migrationBuilder.DropIndex(
                name: "IX_GiftCards_ApplicationUserId",
                table: "GiftCards");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "GiftCards");

            migrationBuilder.DropColumn(
                name: "IdLocalAdherido",
                table: "GiftCards");

            migrationBuilder.AddColumn<int>(
                name: "LocalAdherido",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Telefono",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
