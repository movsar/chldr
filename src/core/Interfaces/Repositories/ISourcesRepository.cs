using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Models;

namespace core.Interfaces.Repositories
{
    public interface ISourcesRepository :IRepository<SourceModel, SourceDto>
    {
    }
}
