using chldr_data.Entities;
using chldr_data.Models.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models.Entries
{
    public class InterjectionModel : WordModel
    {
        public InterjectionModel(SqlEntry entry) : base(entry)
        {
        }
    }
}
