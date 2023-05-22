namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface IPhrase
    {
        string PhraseId { get; set; }
        string Content { get; set; }
        string? Notes { get; set; }
    }
}
