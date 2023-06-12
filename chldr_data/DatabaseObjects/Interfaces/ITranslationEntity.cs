namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ITranslationEntity : ITranslation
    {
        string RawContents { get; }
    }
}
