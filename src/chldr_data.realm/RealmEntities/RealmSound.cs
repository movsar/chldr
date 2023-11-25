using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using Realms;

namespace chldr_data.realm.RealmEntities;

[MapTo("Sound")]
public class RealmSound : RealmObject, ISoundEntity
{
    [PrimaryKey] public string SoundId { get; set; }
    public RealmUser User { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    public string FileName { get; set; } = null!;

    [Ignored] public string EntryId => Entry.EntryId;
    [Ignored] public string UserId => User.Id;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public int Rate { get; set; }

    internal static RealmSound FromDto(PronunciationDto soundDto, Realm context, RealmEntry? entry = null)
    {
        var user = context.Find<RealmUser>(soundDto.UserId);

        if (string.IsNullOrEmpty(soundDto.EntryId) || user == null)
        {
            throw new NullReferenceException();
        }

        if (entry == null)
        {
            entry = context.Find<RealmEntry>(soundDto.EntryId);
        }

        if (entry == null)
        {
            throw new NullReferenceException();
        }

        // Sound
        var soundEntity = context.Find<RealmSound>(soundDto.SoundId);
        if (soundEntity == null)
        {
            soundEntity = new RealmSound();
        }

        soundEntity.Entry = entry;
        soundEntity.User = user;
        soundEntity.SoundId = soundDto.SoundId;
        soundEntity.FileName = soundDto.FileName;
        soundEntity.Rate = soundDto.Rate;

        return soundEntity;
    }

    internal static RealmSound FromModel(PronunciationModel soundModel, RealmUser user, RealmEntry entry)
    {
        if (string.IsNullOrEmpty(soundModel.EntryId) || user == null || entry == null)
        {
            throw new NullReferenceException();
        }

        // Sound
        var soundEntity = new RealmSound();

        soundEntity.Entry = entry;
        soundEntity.User = user;
        soundEntity.SoundId = soundModel.SoundId;
        soundEntity.FileName = soundModel.FileName;
        soundEntity.Rate = soundModel.Rate;

        return soundEntity;
    }
}
