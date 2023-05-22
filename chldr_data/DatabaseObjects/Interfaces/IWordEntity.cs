namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface IWordEntity : IWord, IEntity
    {
        string? AdditionalDetails { get; set; }
        int? PartOfSpeech { get; set; }
    }
}
