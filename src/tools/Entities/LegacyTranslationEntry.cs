using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_maintenance.Entities
{
    [Table("translations")]
    public class LegacyTranslationEntry
    {
        [Key]
        [Column("tid")]
        public int Id { get; set; }
        [Column("pid")]
        public int PhraseId { get; set; }

        [Column("uid")]
        public int UserId { get; set; }

        [Column("translation")]
        public string? Translation { get; set; }

        [Column("lang")]
        public string? LanguageCode { get; set; }

        [Column("rate")]
        public int? Rate { get; set; }

        [Column("date")]
        public string? Date { get; set; }
    }
}