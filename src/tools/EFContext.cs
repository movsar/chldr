using core;
using chldr_maintenance.Entities;
using Microsoft.EntityFrameworkCore;

namespace chldr_maintenance
{
    internal class EfContext : DbContext
    {
        public DbSet<LegacyKarasaevUserphrase> LegacyEntries { get; set; }
        public DbSet<LegacyPhraseEntry> LegacyPhraseEntries { get; set; }
        public DbSet<LegacyTranslationEntry> LegacyTranslationEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var legacyConnectionString = "server=localhost;port=3306;database=dosham_legacy;user=root;password=password";
            optionsBuilder.UseMySQL(legacyConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LegacyKarasaevUserphrase>(x => x
             .ToTable("userphrases")
         );
        }
    }

}
