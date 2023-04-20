﻿using chldr_data.Enums;
using chldr_data.Models.Words;

namespace chldr_data.Models
{
    internal class EntryModelFactory
    {
        public static EntryModel CreateEntryModel(Entities.SqlEntry entryEntity)
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
