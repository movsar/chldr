using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Factories
{
    internal class EntryModelFactory
    {
        public static EntryModel CreateEntryModel(Entities.Entry entryEntity)
        {
            switch ((EntryType)entryEntity?.Type)
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
