using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IDataAccessService
    {
        Task<IEnumerable<EntryModel>> GetRandomEntries();
        Task FindAsync(string inputText);
    }
}
