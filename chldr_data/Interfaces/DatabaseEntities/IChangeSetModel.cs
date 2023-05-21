using chldr_data.Enums;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IChangeSetModel : IChangeSet
    {
        public RecordType RecordType { get; set; }
        public Operation Operation { get; set; }
    }
}
