namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface IUserEntity : IUser, IEntity
    {
        public string? Password { get; set; }
        public byte? IsModerator { get; set; }
        public byte? UserStatus { get; set; }
    }
}
