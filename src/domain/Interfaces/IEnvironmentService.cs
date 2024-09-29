using domain;
using domain.Enums;

namespace domain.Interfaces
{
    public interface IEnvironmentService
    {
        Platforms CurrentPlatform { get; set; }
        bool IsDevelopment { get; }
    }
}
