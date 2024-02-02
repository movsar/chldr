using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_domain.SqlEntities;

[Table("Efmigrationshistory")]
public class SqlEfmigrationshistory
{
    [Key]
    public string MigrationId { get; set; } = null!;
    public string ProductVersion { get; set; } = null!;
}
