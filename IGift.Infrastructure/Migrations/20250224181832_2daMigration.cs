using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _2daMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRoom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false),
                    FromWhoIsLastMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUser1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUser2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoom", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRoom");
        }
    }
}
