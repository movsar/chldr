using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ITokenEntity : IToken, IEntity
    {
        public int? Type { get; set; }
    }
}
