namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IUserEntity : IUser, IEntity
    {
        public int? IsModerator { get; set; }
        public int Status { get; set; }
    }
}
