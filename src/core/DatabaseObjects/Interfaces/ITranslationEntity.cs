namespace core.DatabaseObjects.Interfaces
{
    public interface ITranslationEntity : ITranslation
    {
        string RawContents { get; }
    }
}
