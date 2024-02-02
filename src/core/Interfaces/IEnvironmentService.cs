using core.Enums;

namespace core.Interfaces
{
    public interface IEnvironmentService
    {
        Platforms CurrentPlatform { get; set; }
        bool IsDevelopment { get; }
    }
}
