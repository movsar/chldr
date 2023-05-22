using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IChangeSetEntity : IChangeSet, IEntity
    {
        static abstract IChangeSetEntity FromDto(ChangeSetDto entity);
        public int RecordType { get; set; }
        public int Operation { get; set; }
    }
}
