using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace chldr_data.Migrations
{
    /// <inheritdoc />
    public partial class CreateChangesetsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "__efmigrationshistory",
                columns: table => new
                {
                    MigrationId = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    ProductVersion = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.MigrationId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    password = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    rate = table.Column<int>(type: "int", nullable: false),
                    image_path = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    first_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    patronymic = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_moderator = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    user_status = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.user_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "activity",
                columns: table => new
                {
                    activity_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    object_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    object_class = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    object_property = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    old_value = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    new_value = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.activity_id);
                    table.ForeignKey(
                        name: "fk_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "changesets",
                columns: table => new
                {
                    changeset_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    sequence_number = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "bigint"),
                    record_id = table.Column<string>(type: "varchar(40)", nullable: false),
                    record_type = table.Column<int>(type: "int", nullable: false),
                    operation = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.changeset_id);
                    table.ForeignKey(
                        name: "fk_changesets_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "language",
                columns: table => new
                {
                    language_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    code = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.language_id);
                    table.ForeignKey(
                        name: "fk_language_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "query",
                columns: table => new
                {
                    query_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    content = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.query_id);
                    table.ForeignKey(
                        name: "fk_query_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "source",
                columns: table => new
                {
                    source_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.source_id);
                    table.ForeignKey(
                        name: "fk_source_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    token_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    type = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'0'"),
                    value = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    expires_in = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.token_id);
                    table.ForeignKey(
                        name: "fk_tokens_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "entry",
                columns: table => new
                {
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    source_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    raw_contents = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.entry_id);
                    table.ForeignKey(
                        name: "fk_entry_source_id",
                        column: x => x.source_id,
                        principalTable: "source",
                        principalColumn: "source_id");
                    table.ForeignKey(
                        name: "fk_entry_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "image",
                columns: table => new
                {
                    image_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    file_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    rate = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.image_id);
                    table.ForeignKey(
                        name: "fk_image_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id");
                    table.ForeignKey(
                        name: "fk_image_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "phrase",
                columns: table => new
                {
                    phrase_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    content = table.Column<string>(type: "varchar(20000)", nullable: false),
                    notes = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.phrase_id);
                    table.ForeignKey(
                        name: "fk_phrase_user_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sound",
                columns: table => new
                {
                    sound_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    file_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.sound_id);
                    table.ForeignKey(
                        name: "fk_sound_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id");
                    table.ForeignKey(
                        name: "fk_sound_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "text",
                columns: table => new
                {
                    text_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    content = table.Column<string>(type: "varchar(20000)", nullable: false),
                    notes = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.text_id);
                    table.ForeignKey(
                        name: "fk_text_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "translation",
                columns: table => new
                {
                    translation_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    language_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    content = table.Column<string>(type: "varchar(10000)", maxLength: 10000, nullable: false),
                    raw_contents = table.Column<string>(type: "varchar(10000)", maxLength: 10000, nullable: false),
                    notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    rate = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.translation_id);
                    table.ForeignKey(
                        name: "fk_translation_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id");
                    table.ForeignKey(
                        name: "fk_translation_language_id",
                        column: x => x.language_id,
                        principalTable: "language",
                        principalColumn: "language_id");
                    table.ForeignKey(
                        name: "fk_translation_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "word",
                columns: table => new
                {
                    word_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    content = table.Column<string>(type: "varchar(10000)", maxLength: 10000, nullable: false),
                    notes = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: true),
                    part_of_speech = table.Column<int>(type: "int", nullable: true),
                    additional_details = table.Column<string>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.word_id);
                    table.ForeignKey(
                        name: "fk_word_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "fk_user_id_idx",
                table: "activity",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_changesets_user_id_idx",
                table: "changesets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_entry_source_id",
                table: "entry",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "fk_entry_user_id",
                table: "entry",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_image_entry_id",
                table: "image",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "fk_image_user_id",
                table: "image",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_language_user_id",
                table: "language",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "entry_id_UNIQUE",
                table: "phrase",
                column: "entry_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_query_user_id",
                table: "query",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_sound_entry_id",
                table: "sound",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "fk_sound_user_id",
                table: "sound",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_source_user_id",
                table: "source",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "entry_id_UNIQUE1",
                table: "text",
                column: "entry_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_tokens_user_id_idx",
                table: "tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_translation_entry_id",
                table: "translation",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "fk_translation_language_id",
                table: "translation",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "fk_translation_user_id",
                table: "translation",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "entry_id_UNIQUE2",
                table: "word",
                column: "entry_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__efmigrationshistory");

            migrationBuilder.DropTable(
                name: "activity");

            migrationBuilder.DropTable(
                name: "changesets");

            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.DropTable(
                name: "phrase");

            migrationBuilder.DropTable(
                name: "query");

            migrationBuilder.DropTable(
                name: "sound");

            migrationBuilder.DropTable(
                name: "text");

            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.DropTable(
                name: "translation");

            migrationBuilder.DropTable(
                name: "word");

            migrationBuilder.DropTable(
                name: "language");

            migrationBuilder.DropTable(
                name: "entry");

            migrationBuilder.DropTable(
                name: "source");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
