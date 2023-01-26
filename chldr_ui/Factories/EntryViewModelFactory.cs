﻿using chldr_ui.ViewModels;
using chldr_data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.Factories
{
    internal class EntryViewModelFactory
    {
        public static EntryViewModelBase CreateViewModel(EntryModel entry)
        {
            switch (entry)
            {
                case WordModel:
                    return new WordViewModel(entry as WordModel);
                case PhraseModel:
                    return new PhraseViewModel(entry as PhraseModel);
                default:
                    return null;
            }
        }
    }
}