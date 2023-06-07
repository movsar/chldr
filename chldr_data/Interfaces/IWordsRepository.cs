using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public interface IWordsRepository
    {
        IEnumerable<ChangeSetModel> Add(string userId, WordDto dto);
        WordModel Get(string entityId);
        IEnumerable<ChangeSetModel> Update(string userId, WordDto updatedWordDto);
    }
}
