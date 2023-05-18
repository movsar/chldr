namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IWordEntity : IWord, IEntity
    {
        string? AdditionalDetails { get; set; }
        int? PartOfSpeech { get; set; }
    }
}
