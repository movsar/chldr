namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IPhrase
    {
        string PhraseId { get; set; }
        string Content { get; set; }
        string? Notes { get; set; }
    }
}
