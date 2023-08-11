using chldr_data.Enums;

namespace chldr_data.Models
{
    public class FiltrationFlagsDto
    {
        public bool? OnModeration {get;set;}
        public string? StartsWith {get;set;}
        public int[]? EntryTypes { get; set; }
    }

    public class FiltrationFlags
    {
        public bool IncludeOnModeration { get; set; } = false;
        public string? StartsWith { get; set; } = null;
        public EntryType[] EntryTypes { get; set; } = new EntryType[] {
            EntryType.Text,
            EntryType.Word,
            EntryType.Phrase
        };

        public static FiltrationFlags FromDto(FiltrationFlagsDto dto)
        {
            var flags = new FiltrationFlags();

            if (dto != null)
            {
                if (dto.OnModeration.HasValue)
                {
                    flags.IncludeOnModeration = dto.OnModeration.Value;
                }

                if (dto.StartsWith != null)
                {
                    flags.StartsWith = dto.StartsWith;
                }

                if (dto.EntryTypes != null)
                {
                    flags.EntryTypes = new EntryType[dto.EntryTypes.Length];
                    for (int i = 0; i < dto.EntryTypes.Length; i++)
                    {
                        flags.EntryTypes[i] = (EntryType)dto.EntryTypes[i];
                    }
                }
            }

            return flags;
        }
    }
}
