namespace chldr_data.Interfaces
{
    public interface INetworkService
    {
        bool IsNetworUp { get; }

        event Action NetworkStateHasChanged;
    }
}
