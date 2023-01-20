using chldr_shared.ViewModels;
using chldr_dataaccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Factories
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
