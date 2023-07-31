using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_maintenance.Entities
{
    [Table("phrases")]
    public class LegacyPhraseEntry
    {
        [Key]
        [Column("pid")]
        public int Id { get; set; }

        [Column("uid")]
        public int UserId { get; set; }

        [Column("phrase")]
        public string? Phrase { get; set; }

        [Column("forms")]
        public string? Forms { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("cid")]
        public int? CategoryId { get; set; }

        [Column("source")]
        public string? Source { get; set; }
        [Column("notes")]
        public string? Notes { get; set; }

        [Column("rate")]
        public int? Rate { get; set; }

        [Column("date")]
        public string? Date { get; set; }
    }
}