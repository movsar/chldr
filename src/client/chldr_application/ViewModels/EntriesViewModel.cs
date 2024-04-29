﻿using core.DatabaseObjects.Models;
using ReactiveUI;

namespace chldr_application.ViewModels
{
    public class EntriesViewModel : ViewModelBase
    {
        private IEnumerable<EntryModel> _entries;
        public IEnumerable<EntryModel> Entries
        {
            get => _entries;
            set => this.RaiseAndSetIfChanged(ref _entries, value);
        }
        
        public EntriesViewModel()
        {
         
        }
    }
}