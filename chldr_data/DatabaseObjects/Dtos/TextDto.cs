using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class TextDto : IText
    {
        public string TextId { get; set; }
        public string Content { get; set; }
    }
}
