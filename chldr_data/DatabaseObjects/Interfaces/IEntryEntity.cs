namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IEntryEntity : IEntry, IEntity
    {
        int Type { get; }
        int Subtype { get; }

        //RealmPhrase? Phrase { get; set; }
        //RealmSource Source { get; set; }
        //RealmText? Text { get; set; }
        //RealmUser User { get; set; }
        //RealmWord? Word { get; set; }
        //IList<RealmImage> Images { get; }
        //IList<RealmSound> Sounds { get; }
        //IList<RealmTranslation> Translations { get; }
    }
}
