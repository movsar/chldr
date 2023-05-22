namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IWordEntity : IWord, IEntity
    {
        string? AdditionalDetails { get; set; }
        int? PartOfSpeech { get; set; }
    }
}
