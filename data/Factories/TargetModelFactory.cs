using Data.Entities;
using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Factories
{
    internal static class TargetModelFactory
    {
        public static TargetModel CreateTarget(Entities.Entry entryEntity)
        {
            switch (entryEntity.Type)
            {
                case EntryType.Word:
                    return new WordModel(entryEntity.Word);
                case EntryType.Phrase:
                    return new PhraseModel(entryEntity.Phrase);
                case EntryType.Text:
                    return new TextModel(entryEntity.Text);
                default:
                    return null;
            }
        }
    }
}
