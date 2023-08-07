namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IUserEntity : IUser, IEntity
    {
        public int Type { get; set; }
        public int Status { get; set; }
    }
}
