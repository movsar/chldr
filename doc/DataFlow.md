### Entry addition
	ContentStore
        EntryService
            Add
                AddRemote
                    EntryResolver
                        SqlEntryRepository
                            Add
                AddLocal
                    RealmEntryRepository
                        Add

                Invoke EntryInserted
        
        OnEntryInserted
            UpdateUI