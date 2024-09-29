namespace domain.DatabaseObjects.Interfaces
{
    public interface IChangeSet
    {
        public long ChangeSetIndex { get; }
        public string ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
