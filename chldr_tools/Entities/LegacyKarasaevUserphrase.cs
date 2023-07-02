using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace chldr_maintenance.Entities
{
    public class LegacyKarasaevUserphrase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("phrase")]
        public string? Word { get; set; }

        [Column("translation")]
        public string? Translation { get; set; }
    }
}
