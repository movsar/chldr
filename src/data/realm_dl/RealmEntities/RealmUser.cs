using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using Realms;

namespace realm_dl.RealmEntities;
[MapTo("User")]
public class RealmUser : RealmObject, IUserEntity
{
    [PrimaryKey] public string Id { get; set; }
    public string? Email { get; set; }
    public int Rate { get; set; } = 0;
    public string? ImagePath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public int Type { get; set; }
    public int Status { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public IList<RealmEntry> Entries { get; }
    public IList<RealmSound> Sounds { get; }
    public IList<RealmSource> Sources { get; }
    public IList<RealmTranslation> Translations { get; }

    internal static RealmUser FromDto(UserDto userDto, Realm context)
    {
        if (string.IsNullOrEmpty(userDto.Id) || context == null)
        {
            throw new NullReferenceException();
        }

        // Translation
        var user = context.Find<RealmUser>(userDto.Id);
        if (user == null)
        {
            user = new RealmUser();
        }

        user.Id = userDto.Id;
        user.Rate = userDto.Rate;
        user.Email = userDto.Email;
        user.Status = (int)userDto.Status;
        user.Type = (int)userDto.Type;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Patronymic = userDto.Patronymic;
        user.CreatedAt = userDto.CreatedAt;
        user.UpdatedAt = userDto.UpdatedAt;

        return user;
    }

    internal static RealmUser FromModel(UserModel userModel)
    {
        if (string.IsNullOrEmpty(userModel.Id))
        {
            throw new NullReferenceException();
        }

        var user = new RealmUser();

        user.Id = userModel.Id;
        user.Rate = userModel.Rate;
        user.Email = userModel.Email;
        user.Status = (int)userModel.Status;
        user.Type = (int)userModel.Type;
        user.FirstName = userModel.FirstName;
        user.LastName = userModel.LastName;
        user.Patronymic = userModel.Patronymic;
        user.CreatedAt = userModel.CreatedAt;
        user.UpdatedAt = userModel.UpdatedAt;

        return user;
    }
}
