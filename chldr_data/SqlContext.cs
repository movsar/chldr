using System;
using System.Collections.Generic;
using chldr_data.Entities;
using Microsoft.EntityFrameworkCore;

namespace chldr_tools;

public partial class SqlContext : DbContext
{
    public SqlContext()
    {
    }

    public SqlContext(DbContextOptions<SqlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SqlActivity> Activities { get; set; }
    public virtual DbSet<SqlEfmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<SqlEntry> Entries { get; set; }

    public virtual DbSet<SqlImage> Images { get; set; }

    public virtual DbSet<SqlLanguage> Languages { get; set; }

    public virtual DbSet<SqlPhrase> Phrases { get; set; }

    public virtual DbSet<SqlQuery> Queries { get; set; }

    public virtual DbSet<SqlSound> Sounds { get; set; }

    public virtual DbSet<SqlSource> Sources { get; set; }

    public virtual DbSet<SqlText> Texts { get; set; }

    public virtual DbSet<SqlTranslation> Translations { get; set; }

    public virtual DbSet<SqlUser> Users { get; set; }

    public virtual DbSet<SqlWord> Words { get; set; }
    public virtual DbSet<SqlToken> Tokens { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;database=chldr;user=root;password=io34j0f934j9g034!#Aa-");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SqlActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PRIMARY");

            entity.ToTable("activity");

            entity.HasIndex(e => e.UserId, "fk_user_id_idx");

            entity.Property(e => e.ActivityId)
                .HasMaxLength(40)
                .HasColumnName("activity_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.NewValue)
                .HasMaxLength(255)
                .HasColumnName("new_value");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.ObjectClass)
                .HasMaxLength(255)
                .HasColumnName("object_class");
            entity.Property(e => e.ObjectId)
                .HasMaxLength(40)
                .HasColumnName("object_id");
            entity.Property(e => e.ObjectProperty)
                .HasMaxLength(255)
                .HasColumnName("object_property");
            entity.Property(e => e.OldValue)
                .HasMaxLength(255)
                .HasColumnName("old_value");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Activities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_id");
        });

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

            entity.HasIndex(e => e.SourceId, "fk_entry_source_id");

            entity.HasIndex(e => e.UserId, "fk_entry_user_id");

            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.RawContents)
                .HasMaxLength(1500)
                .HasColumnName("raw_contents");
            entity.Property(e => e.SourceId)
                .HasMaxLength(40)
                .HasColumnName("source_id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Source).WithMany(p => p.Entries)
                .HasForeignKey(d => d.SourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_entry_source_id");

            entity.HasOne(d => d.User).WithMany(p => p.Entries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_entry_user_id");
        });

        modelBuilder.Entity<SqlImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");

            entity.ToTable("image");

            entity.HasIndex(e => e.EntryId, "fk_image_entry_id");

            entity.HasIndex(e => e.UserId, "fk_image_user_id");

            entity.Property(e => e.ImageId)
                .HasMaxLength(40)
                .HasColumnName("image_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(250)
                .HasColumnName("file_name");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Entry).WithMany(p => p.Images)
                .HasForeignKey(d => d.EntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_image_entry_id");

            entity.HasOne(d => d.User).WithMany(p => p.Images)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_image_user_id");
        });

        modelBuilder.Entity<SqlLanguage>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PRIMARY");

            entity.ToTable("language");

            entity.HasIndex(e => e.UserId, "fk_language_user_id");

            entity.Property(e => e.LanguageId)
                .HasMaxLength(40)
                .HasColumnName("language_id");
            entity.Property(e => e.Code)
                .HasMaxLength(40)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Languages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_language_user_id");
        });

        modelBuilder.Entity<SqlPhrase>(entity =>
        {
            entity.HasKey(e => e.PhraseId).HasName("PRIMARY");

            entity.ToTable("phrase");

            entity.HasIndex(e => e.EntryId, "entry_id_UNIQUE").IsUnique();

            entity.Property(e => e.PhraseId)
                .HasMaxLength(40)
                .HasColumnName("phrase_id");
            entity.Property(e => e.Content)
                .HasColumnType("varchar(20000)")
                .HasColumnName("content");
            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1500)
                .HasColumnName("notes");

            entity.HasOne(d => d.Entry).WithOne(p => p.Phrase)
                .HasForeignKey<SqlPhrase>(d => d.EntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_phrase_user_id");
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
                .ValueGeneratedOnAddOrUpdate()
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
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(250)
                .HasColumnName("file_name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Entry).WithMany(p => p.Sounds)
                .HasForeignKey(d => d.EntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
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
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sources)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_source_user_id");
        });

        modelBuilder.Entity<SqlText>(entity =>
        {
            entity.HasKey(e => e.TextId).HasName("PRIMARY");

            entity.ToTable("text");

            entity.HasIndex(e => e.EntryId, "entry_id_UNIQUE").IsUnique();

            entity.Property(e => e.TextId)
                .HasMaxLength(40)
                .HasColumnName("text_id");
            entity.Property(e => e.Content)
                .HasColumnType("varchar(20000)")
                .HasColumnName("content");
            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1500)
                .HasColumnName("notes");

            entity.HasOne(d => d.Entry).WithOne(p => p.Text)
                .HasForeignKey<SqlText>(d => d.EntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_text_entry_id");
        });

        modelBuilder.Entity<SqlTranslation>(entity =>
        {
            entity.HasKey(e => e.TranslationId).HasName("PRIMARY");

            entity.ToTable("translation");

            entity.HasIndex(e => e.EntryId, "fk_translation_entry_id");

            entity.HasIndex(e => e.LanguageId, "fk_translation_language_id");

            entity.HasIndex(e => e.UserId, "fk_translation_user_id");

            entity.Property(e => e.TranslationId)
                .HasMaxLength(40)
                .HasColumnName("translation_id");
            entity.Property(e => e.Content)
                .HasMaxLength(10000)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");
            entity.Property(e => e.LanguageId)
                .HasMaxLength(40)
                .HasColumnName("language_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.RawContents)
                .HasMaxLength(10000)
                .HasColumnName("raw_contents");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Entry).WithMany(p => p.Translations)
                .HasForeignKey(d => d.EntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_translation_entry_id");

            entity.HasOne(d => d.Language).WithMany(p => p.Translations)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_translation_language_id");

            entity.HasOne(d => d.User).WithMany(p => p.Translations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_translation_user_id");
        });

        modelBuilder.Entity<SqlUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .HasMaxLength(40)
                .HasColumnName("user_id");
            entity.Property(e => e.AccountStatus).HasColumnName("account_status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(250)
                .HasColumnName("image_path");
            entity.Property(e => e.IsModerator).HasColumnName("is_moderator");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(100)
                .HasColumnName("patronymic");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<SqlWord>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PRIMARY");

            entity.ToTable("word");

            entity.HasIndex(e => e.EntryId, "entry_id_UNIQUE").IsUnique();

            entity.Property(e => e.WordId)
                .HasMaxLength(40)
                .HasColumnName("word_id");
            entity.Property(e => e.AdditionalDetails)
                .HasColumnType("json")
                .HasColumnName("additional_details");
            entity.Property(e => e.Content)
                .HasMaxLength(10000)
                .HasColumnName("content");
            entity.Property(e => e.EntryId)
                .HasMaxLength(40)
                .HasColumnName("entry_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1500)
                .HasColumnName("notes");
            entity.Property(e => e.PartOfSpeech).HasColumnName("part_of_speech");

            entity.HasOne(d => d.Entry).WithOne(p => p.Word)
                .HasForeignKey<SqlWord>(d => d.EntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_word_entry_id");
        });

        modelBuilder.Entity<SqlToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PRIMARY");
            entity.ToTable("tokens");

            entity.Property(e => e.UserId)
                          .HasMaxLength(40)
                          .HasColumnName("user_id");

            entity.Property(e => e.Type)
                .HasColumnName("type");
            
            entity.Property(e => e.Value)
                .HasMaxLength(300)
                .HasColumnName("value");

            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_in");

            entity.Property(e => e.CreatedAt)
              .HasDefaultValueSql("CURRENT_TIMESTAMP")
              .HasColumnType("datetime")
              .HasColumnName("created_at");

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_tokens_user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
