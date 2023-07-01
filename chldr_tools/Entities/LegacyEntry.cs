using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_maintenance.Entities
{
    public class LegacyEntry
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
