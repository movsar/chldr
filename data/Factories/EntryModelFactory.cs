using Data.Enums;
using Data.Interfaces;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Factories
{
    internal class EntryModelFactory
    {
        public static EntryModel CreateEntryModel(Entities.Entry entryEntity)
        {
            switch (entryEntity.Type)
            {
                case EntryType.Word:
                    return new WordModel(entryEntity);
                case EntryType.Phrase:
                    return new PhraseModel(entryEntity);
                case EntryType.Text:
                    return new TextModel(entryEntity);
                default:
                    return null;
            }
        }
    }
}
