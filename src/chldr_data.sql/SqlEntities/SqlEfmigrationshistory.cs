using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.sql.SqlEntities;

[Table("Efmigrationshistory")]
public class SqlEfmigrationshistory
{
    [Key]
    public string MigrationId { get; set; } = null!;
    public string ProductVersion { get; set; } = null!;
}
