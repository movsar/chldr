using chldr_data.Enums.WordDetails;
using chldr_data.Models.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Dto.WordDtos
{
    public class VerbDto : WordDto
    {
        public VerbDto(VerbModel word) : base(word)
        {
            PartOfSpeech = PartOfSpeech.Verb;
            Tense = word.Tense;
            Conjugation = word.Conjugation;
            Mood = word.Mood;
            NumericalType = word.NumericalType;
            Transitiveness = word.Transitiveness;
            Class = word.Class;
        }

        public VerbTense Tense { get; set; }
        public VerbConjugation Conjugation { get; set; }
        public VerbMood Mood { get; set; } = new();
        public NumericalType NumericalType { get; set; }
        public Transitiveness Transitiveness { get; set; }
        // Should be available only for some verbs, so don't allow to set it for all verbs
        public int Class { get; set; } = 0;
    }
}
