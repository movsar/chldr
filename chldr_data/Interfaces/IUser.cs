namespace chldr_data.Interfaces
{
    public interface IUser
    {
        string? Email { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
        string? Patronymic { get; set; }
        int Rate { get; set; }
    }
}
