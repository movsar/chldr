﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using chldr_data.remote.Services;

#nullable disable

namespace chldr_api.Migrations
{
    [DbContext(typeof(SqlContext))]
    [Migration("20230924065718_identity")]
    partial class identity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlChangeSet", b =>
                {
                    b.Property<long>("ChangeSetIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("changeset_index");

                    b.Property<string>("ChangeSetId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("changeset_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("Operation")
                        .HasColumnType("int")
                        .HasColumnName("operation");

                    b.Property<string>("RecordChanges")
                        .HasColumnType("text")
                        .HasColumnName("record_changes");

                    b.Property<string>("RecordId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("record_id");

                    b.Property<int>("RecordType")
                        .HasColumnType("int")
                        .HasColumnName("record_type");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("user_id");

                    b.HasKey("ChangeSetIndex")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "ChangeSetId" }, "fk_changesets_id_idx");

                    b.HasIndex(new[] { "UserId" }, "fk_changesets_user_id_idx");

                    b.ToTable("changesets", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlEfmigrationshistory", b =>
                {
                    b.Property<string>("MigrationId")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<string>("ProductVersion")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.HasKey("MigrationId")
                        .HasName("PRIMARY");

                    b.ToTable("__efmigrationshistory", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlEntry", b =>
                {
                    b.Property<string>("EntryId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("entry_id");

                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Details")
                        .HasColumnType("text")
                        .HasColumnName("details");

                    b.Property<string>("ParentEntryId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("parent_id");

                    b.Property<int>("Rate")
                        .HasColumnType("int")
                        .HasColumnName("rate");

                    b.Property<string>("RawContents")
                        .IsRequired()
                        .HasMaxLength(1500)
                        .HasColumnType("varchar(1500)")
                        .HasColumnName("raw_contents");

                    b.Property<string>("SourceId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("source_id");

                    b.Property<int>("Subtype")
                        .HasColumnType("int")
                        .HasColumnName("subtype");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasColumnName("type");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("user_id");

                    b.HasKey("EntryId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "ParentEntryId" }, "entry_parent_id_idx");

                    b.HasIndex(new[] { "Rate" }, "entry_rate_idx");

                    b.HasIndex(new[] { "RawContents" }, "entry_rawcontents_idx");

                    b.HasIndex(new[] { "Type" }, "entry_type_idx");

                    b.HasIndex(new[] { "SourceId" }, "fk_entry_source_id");

                    b.HasIndex(new[] { "UserId" }, "fk_entry_user_id");

                    b.ToTable("entry", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlQuery", b =>
                {
                    b.Property<string>("QueryId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("query_id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("content");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("user_id");

                    b.HasKey("QueryId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "UserId" }, "fk_query_user_id");

                    b.ToTable("query", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlSound", b =>
                {
                    b.Property<string>("SoundId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("sound_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("EntryId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("entry_id");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)")
                        .HasColumnName("file_name");

                    b.Property<int>("Rate")
                        .HasColumnType("int")
                        .HasColumnName("rate");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("user_id");

                    b.HasKey("SoundId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "EntryId" }, "fk_sound_entry_id");

                    b.HasIndex(new[] { "UserId" }, "fk_sound_user_id");

                    b.ToTable("sound", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlSource", b =>
                {
                    b.Property<string>("SourceId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("source_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("name");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("notes");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("UserId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("user_id");

                    b.HasKey("SourceId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "UserId" }, "fk_source_user_id");

                    b.ToTable("source", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlToken", b =>
                {
                    b.Property<string>("TokenId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("token_id");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("datetime")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("ExpiresIn")
                        .HasColumnType("datetime")
                        .HasColumnName("expires_in");

                    b.Property<int?>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("type")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("user_id");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)")
                        .HasColumnName("value");

                    b.HasKey("TokenId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "UserId" }, "fk_tokens_user_id_idx");

                    b.HasIndex(new[] { "Value" }, "token_value_idx");

                    b.ToTable("tokens", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlTranslation", b =>
                {
                    b.Property<string>("TranslationId")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("translation_id");

                    b.Property<string>("Content")
                        .HasMaxLength(10000)
                        .HasColumnType("varchar(10000)")
                        .HasColumnName("content");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("EntryId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("entry_id");

                    b.Property<string>("LanguageCode")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("language_code");

                    b.Property<string>("Notes")
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("notes");

                    b.Property<int>("Rate")
                        .HasColumnType("int")
                        .HasColumnName("rate");

                    b.Property<string>("RawContents")
                        .HasMaxLength(10000)
                        .HasColumnType("varchar(10000)")
                        .HasColumnName("raw_contents");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("user_id");

                    b.HasKey("TranslationId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "EntryId" }, "fk_translation_entry_id");

                    b.HasIndex(new[] { "LanguageCode" }, "fk_translation_language_id");

                    b.HasIndex(new[] { "UserId" }, "fk_translation_user_id");

                    b.HasIndex(new[] { "Rate" }, "translation_rate_idx");

                    b.HasIndex(new[] { "RawContents" }, "translation_rawcontents_idx");

                    b.ToTable("translation", (string)null);
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("ImagePath")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .HasColumnType("longtext");

                    b.Property<string>("Patronymic")
                        .HasColumnType("longtext");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlChangeSet", b =>
                {
                    b.HasOne("chldr_data.remote.SqlEntities.SqlUser", "User")
                        .WithMany("ChangeSets")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_changesets_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlEntry", b =>
                {
                    b.HasOne("chldr_data.remote.SqlEntities.SqlSource", "Source")
                        .WithMany("Entries")
                        .HasForeignKey("SourceId")
                        .IsRequired()
                        .HasConstraintName("fk_entry_source_id");

                    b.HasOne("chldr_data.remote.SqlEntities.SqlUser", "User")
                        .WithMany("Entries")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_entry_user_id");

                    b.Navigation("Source");

                    b.Navigation("User");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlQuery", b =>
                {
                    b.HasOne("chldr_data.remote.SqlEntities.SqlUser", "User")
                        .WithMany("Queries")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_query_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlSound", b =>
                {
                    b.HasOne("chldr_data.remote.SqlEntities.SqlEntry", "Entry")
                        .WithMany("Sounds")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_sound_entry_id");

                    b.HasOne("chldr_data.remote.SqlEntities.SqlUser", "User")
                        .WithMany("Sounds")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_sound_user_id");

                    b.Navigation("Entry");

                    b.Navigation("User");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlSource", b =>
                {
                    b.HasOne("chldr_data.remote.SqlEntities.SqlUser", "User")
                        .WithMany("Sources")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_source_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlToken", b =>
                {
                    b.HasOne("chldr_data.remote.SqlEntities.SqlUser", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_tokens_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlTranslation", b =>
                {
                    b.HasOne("chldr_data.remote.SqlEntities.SqlEntry", "Entry")
                        .WithMany("Translations")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_translation_entry_id");

                    b.HasOne("chldr_data.remote.SqlEntities.SqlUser", "User")
                        .WithMany("Translations")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_translation_user_id");

                    b.Navigation("Entry");

                    b.Navigation("User");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlUser", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithOne()
                        .HasForeignKey("chldr_data.remote.SqlEntities.SqlUser", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlEntry", b =>
                {
                    b.Navigation("Sounds");

                    b.Navigation("Translations");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlSource", b =>
                {
                    b.Navigation("Entries");
                });

            modelBuilder.Entity("chldr_data.remote.SqlEntities.SqlUser", b =>
                {
                    b.Navigation("ChangeSets");

                    b.Navigation("Entries");

                    b.Navigation("Queries");

                    b.Navigation("Sounds");

                    b.Navigation("Sources");

                    b.Navigation("Tokens");

                    b.Navigation("Translations");
                });
#pragma warning restore 612, 618
        }
    }
}
