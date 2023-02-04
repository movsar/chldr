namespace chldr_shared.Enums
{
    public enum SessionStatus
    {
        // Offline
        Disconnected = 0,

        // Anonumous user logged in
        Unauthorized = 1,

        LoggingIn = 2,
        LoggedIn = 3,
    }
}
