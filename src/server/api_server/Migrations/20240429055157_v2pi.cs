using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace api_server.Migrations
{
    /// <inheritdoc />
    public partial class v2pi : Migration
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
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    ImagePath = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Patronymic = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    EmailConfirmationToken = table.Column<string>(type: "longtext", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "changesets",
                columns: table => new
                {
                    changeset_index = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    changeset_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    record_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    record_type = table.Column<int>(type: "int", nullable: false),
                    record_changes = table.Column<string>(type: "text", nullable: true),
                    operation = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.changeset_index);
                    table.ForeignKey(
                        name: "fk_changesets_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "entry",
                columns: table => new
                {
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    parent_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    subtype = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    raw_contents = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: false),
                    details = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.entry_id);
                    table.ForeignKey(
                        name: "fk_entry_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
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
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.query_id);
                    table.ForeignKey(
                        name: "fk_query_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
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
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.source_id);
                    table.ForeignKey(
                        name: "fk_source_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sound",
                columns: table => new
                {
                    sound_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    file_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.sound_id);
                    table.ForeignKey(
                        name: "fk_sound_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sound_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "translation",
                columns: table => new
                {
                    translation_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    entry_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    content = table.Column<string>(type: "longtext", nullable: true),
                    raw_contents = table.Column<string>(type: "text", maxLength: 1500, nullable: true),
                    language_code = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    rate = table.Column<int>(type: "int", nullable: false),
                    source_id = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.translation_id);
                    table.ForeignKey(
                        name: "fk_translation_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entry",
                        principalColumn: "entry_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_translation_source_id",
                        column: x => x.source_id,
                        principalTable: "source",
                        principalColumn: "source_id");
                    table.ForeignKey(
                        name: "fk_translation_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_changesets_id_idx",
                table: "changesets",
                column: "changeset_id");

            migrationBuilder.CreateIndex(
                name: "fk_changesets_user_id_idx",
                table: "changesets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "entry_parent_id_idx",
                table: "entry",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "entry_rate_idx",
                table: "entry",
                column: "rate");

            migrationBuilder.CreateIndex(
                name: "entry_type_idx",
                table: "entry",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "fk_entry_user_id",
                table: "entry",
                column: "user_id");

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
                name: "fk_translation_entry_id",
                table: "translation",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "fk_translation_language_id",
                table: "translation",
                column: "language_code");

            migrationBuilder.CreateIndex(
                name: "fk_translation_source_id",
                table: "translation",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "fk_translation_user_id",
                table: "translation",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "translation_rate_idx",
                table: "translation",
                column: "rate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__efmigrationshistory");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "changesets");

            migrationBuilder.DropTable(
                name: "query");

            migrationBuilder.DropTable(
                name: "sound");

            migrationBuilder.DropTable(
                name: "translation");

            migrationBuilder.DropTable(
                name: "entry");

            migrationBuilder.DropTable(
                name: "source");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
