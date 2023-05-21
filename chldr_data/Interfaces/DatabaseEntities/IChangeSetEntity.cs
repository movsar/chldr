using chldr_data.Dto;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IChangeSetEntity : IChangeSet, IEntity
    {
        static abstract IChangeSetEntity FromDto(ChangeSetDto entity);
        public int RecordType { get; set; }
        public int Operation { get; set; }
    }
}
