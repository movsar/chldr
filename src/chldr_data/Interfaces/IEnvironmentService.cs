using dosham.Enums;

namespace chldr_data.Interfaces
{
    public interface IEnvironmentService
    {
        Platforms CurrentPlatform { get; set; }
        bool IsDevelopment { get; }
        bool IsNetworkUp();
    }
}
