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
            migrationBuilder.DropIndex(
                name: "translation_rawcontents_idx",
                table: "translation");

            migrationBuilder.DropIndex(
                name: "entry_rawcontents_idx",
                table: "entry");

            migrationBuilder.AlterColumn<string>(
                name: "raw_contents",
                table: "translation",
                type: "text",
                maxLength: 1500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10000)",
                oldMaxLength: 10000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "translation",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10000)",
                oldMaxLength: 10000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "raw_contents",
                table: "translation",
                type: "varchar(10000)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 1500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "translation",
                type: "varchar(10000)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "translation_rawcontents_idx",
                table: "translation",
                column: "raw_contents");

            migrationBuilder.CreateIndex(
                name: "entry_rawcontents_idx",
                table: "entry",
                column: "raw_contents");
        }
    }
}
