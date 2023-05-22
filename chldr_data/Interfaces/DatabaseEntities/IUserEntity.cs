namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IUserEntity : IUser, IEntity
    {
        public string? Password { get; set; }
        public string? ImagePath { get; set; }
        public byte? IsModerator { get; set; }
        public byte? UserStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

    }
}
