using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chldr_api.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    token_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    expires_in = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    type = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'0'"),
                    value = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.token_id);
                    table.ForeignKey(
                        name: "fk_tokens_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "fk_tokens_user_id_idx",
                table: "tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "token_value_idx",
                table: "tokens",
                column: "value");
        }
    }
}
