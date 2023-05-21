namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IChangeSetEntity : IChangeSet, IEntity
    {
        public int RecordType { get; set; }
        public int Operation { get; set; }
    }
}
