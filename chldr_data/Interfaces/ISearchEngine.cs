using chldr_utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public interface ISearchEngine
    {
        Task FindAsync(string inputText, FiltrationFlags filtrationFlags);

    }
}
