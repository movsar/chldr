using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public interface IChangeSetsRepository
    {
        IEnumerable<ChangeSetModel> Add(string userId, ChangeSetDto dto);
        void AddRange(IEnumerable<ChangeSetDto> dtos);
        ChangeSetModel Get(string entityId);
        IEnumerable<ChangeSetModel> Get(string[] changeSetIds);
        IEnumerable<ChangeSetModel> GetLatest(int limit);
        IEnumerable<ChangeSetModel> Update(string userId, ChangeSetDto dto);
    }
}
