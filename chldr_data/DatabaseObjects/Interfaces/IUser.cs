namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface IUser
    {
        string? UserId { get; set; }
        string? Email { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
        string? Patronymic { get; set; }
        int Rate { get; set; }
    }
}
