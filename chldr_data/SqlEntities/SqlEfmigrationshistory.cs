using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools;

[Table("Efmigrationshistory")]
public  class SqlEfmigrationshistory
{
    public string MigrationId { get; set; } = null!;
    public string ProductVersion { get; set; } = null!;
}
