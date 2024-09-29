using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;

namespace domain.Interfaces.Repositories
{
    public interface ISourcesRepository :IRepository<SourceModel, SourceDto>
    {
    }
}
