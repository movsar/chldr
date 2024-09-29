using sql_dl.Interfaces;
using sql_dl.SqlEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace sql_dl;

public class SqlContext : IdentityUserContext<SqlUser>
{
    public SqlContext() { }
    public SqlContext(DbContextOptions<SqlContext> options)
        : base(options) { }

    public virtual DbSet<SqlEfmigrationshistory> Efmigrationshistories { get; set; }
    public virtual DbSet<SqlChangeSet> ChangeSets { get; set; }
    public virtual DbSet<SqlEntry> Entries { get; set; }
    public virtual DbSet<SqlQuery> Queries { get; set; }
    public virtual DbSet<SqlSound> Pronunciations { get; set; }
    public virtual DbSet<SqlSource> Sources { get; set; }
    public virtual DbSet<SqlTranslation> Translations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SqlEfmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<SqlEntry>(entity =>
        {
            entity.HasKey(e => e.EntryId).HasName("PRIMARY");

            entity.ToTable("entry");

            entity.HasIndex(e => e.UserId, "fk_entry_user_id");
            entity.HasIndex(e => e.ParentEntryId, "entry_parent_id_idx");
            entity.HasIndex(e => e.Rate, "entry_rate_idx");
            entity.HasIndex(e => e.Type, "entry_type_idx");

            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");

            entity.Property(e => e.ParentEntryId)
               .HasMaxLength(40)
               .HasColumnName("parent_id");

            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.Property(e => e.Type)
                .HasColumnName("type");

            entity.Property(e => e.Rate)
                .HasColumnName("rate");

            entity.Property(e => e.Subtype)
                .HasColumnName("subtype");

            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");

            entity.Property(e => e.RawContents)
                .HasMaxLength(1500)
                .HasColumnName("raw_contents");

            entity.Property(e => e.Details)
                .HasColumnType("text")
                .HasColumnName("details");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.User).WithMany(p => p.Entries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_entry_user_id");
        });

        modelBuilder.Entity<SqlQuery>(entity =>
        {
            entity.HasKey(e => e.QueryId).HasName("PRIMARY");

            entity.ToTable("query");

            entity.HasIndex(e => e.UserId, "fk_query_user_id");

            entity.Property(e => e.QueryId)
                .HasMaxLength(40)
                .HasColumnName("query_id");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Queries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_query_user_id");
        });

        modelBuilder.Entity<SqlSound>(entity =>
        {
            entity.HasKey(e => e.SoundId).HasName("PRIMARY");

            entity.ToTable("sound");

            entity.HasIndex(e => e.EntryId, "fk_sound_entry_id");

            entity.HasIndex(e => e.UserId, "fk_sound_user_id");

            entity.Property(e => e.SoundId)
                .HasMaxLength(40)
                .HasColumnName("sound_id");

            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");

            entity.Property(e => e.Rate)
              .HasColumnName("rate");

            entity.Property(e => e.FileName)
                .HasMaxLength(250)
                .HasColumnName("file_name");

            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("CURRENT_TIMESTAMP")
             .HasColumnType("datetime")
             .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Entry).WithMany(p => p.Sounds)
                .HasForeignKey(d => d.EntryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_sound_entry_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sounds)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sound_user_id");
        });

        modelBuilder.Entity<SqlSource>(entity =>
        {
            entity.HasKey(e => e.SourceId).HasName("PRIMARY");

            entity.ToTable("source");

            entity.HasIndex(e => e.UserId, "fk_source_user_id");

            entity.Property(e => e.SourceId)
                .HasMaxLength(40)
                .HasColumnName("source_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sources)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_source_user_id");
        });

        modelBuilder.Entity<SqlTranslation>(entity =>
        {
            entity.HasKey(e => e.TranslationId).HasName("PRIMARY");

            entity.ToTable("translation");

            entity.Property(e => e.TranslationId)
                .HasMaxLength(40)
                .HasColumnName("translation_id");

            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");

            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");
            
            entity.Property(e => e.SourceId)
                .HasMaxLength(40)
                .HasColumnName("source_id");
            
            entity.Property(e => e.Content)
                .HasColumnName("content")
                .HasColumnType("longtext");

            entity.Property(e => e.RawContents)
                .HasColumnName("raw_contents")
                .HasColumnType("text")
                .HasMaxLength(1500);

            entity.Property(e => e.LanguageCode)
                .HasMaxLength(40)
                .HasColumnName("language_code");

            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");

            entity.Property(e => e.Rate)
                .HasColumnName("rate");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");


            entity.HasIndex(e => e.EntryId, "fk_translation_entry_id");
            entity.HasIndex(e => e.SourceId, "fk_translation_source_id");
            entity.HasIndex(e => e.LanguageCode, "fk_translation_language_id");
            entity.HasIndex(e => e.UserId, "fk_translation_user_id");
            entity.HasIndex(e => e.Rate, "translation_rate_idx");

            entity.HasOne(d => d.Source).WithMany(p => p.Translations)
                .HasForeignKey(d => d.SourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_translation_source_id");

            entity.HasOne(d => d.Entry).WithMany(p => p.Translations)
                .HasForeignKey(d => d.EntryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_translation_entry_id");

            entity.HasOne(d => d.User).WithMany(p => p.Translations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_translation_user_id");
        });

        modelBuilder.Entity<SqlUser>(entity =>
        {
            entity.Property(e => e.Status)
                    .HasColumnName("Status");

            entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedAt");

            entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedAt");

            entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("FirstName");

            entity.Property(e => e.ImagePath)
                    .HasMaxLength(250)
                    .HasColumnName("ImagePath");

            entity.Property(e => e.Type)
                    .HasColumnName("Type");

            entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("LastName");

            entity.Property(e => e.Patronymic)
                    .HasMaxLength(100)
                    .HasColumnName("Patronymic");

            entity.Property(e => e.Rate)
                    .HasColumnName("Rate");
        });

        modelBuilder.Entity<SqlChangeSet>(entity =>
        {
            entity.HasKey(e => e.ChangeSetIndex).HasName("PRIMARY");

            entity.ToTable("changesets");

            entity.HasIndex(e => e.UserId, "fk_changesets_user_id_idx");
            entity.HasIndex(e => e.ChangeSetId, "fk_changesets_id_idx");

            entity.Property(e => e.ChangeSetIndex)
                .HasColumnName("changeset_index");

            entity.Property(e => e.ChangeSetId)
                .HasMaxLength(40)
                .HasColumnName("changeset_id");

            entity.Property(e => e.UserId)
              .HasMaxLength(40)
              .HasColumnName("user_id");

            entity.Property(e => e.RecordId)
                .HasMaxLength(40)
                .HasColumnName("record_id");

            entity.Property(e => e.RecordType)
                .HasColumnName("record_type");

            entity.Property(e => e.RecordChanges)
                .HasColumnType("text")
                .HasColumnName("record_changes");

            entity.Property(e => e.Operation)
                .HasColumnName("operation");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");


            entity.HasOne(d => d.User).WithMany(p => p.ChangeSets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_changesets_user_id");
        });
    }

    public static readonly LoggerFactory _myLoggerFactory =
    new LoggerFactory(new[] {
        new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
    });

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(_myLoggerFactory);
    }
}
