﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using chldr_utils;
using GraphQL;
using Realms;
using Microsoft.EntityFrameworkCore.Update;
using chldr_utils.Services;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        public RealmEntriesRepository(Realm context, ExceptionHandler exceptionHandler) : base(context, exceptionHandler) { }
        protected override RecordType RecordType => RecordType.Entry;
        protected override EntryModel FromEntityShortcut(RealmEntry entry)
        {
            return EntryModel.FromEntity(
                                    entry,
                                    entry.Source,
                                    entry.Translations,
                                    entry.Sounds);
        }

        public override void Add(EntryDto newEntryDto)
        {
            RealmEntry? newEntry = null;
            
            _dbContext.Write(() =>
            {
                newEntry = RealmEntry.FromDto(newEntryDto, _dbContext);
                _dbContext.Add(newEntry);
            });

            if (newEntry == null)
            {
                throw new NullReferenceException();
            }

            foreach (var sound in newEntry.Sounds)
            {
                var soundDto = newEntryDto.Sounds.FirstOrDefault(s => s.SoundId == sound.SoundId && !string.IsNullOrEmpty(s.RecordingB64));
                if (soundDto == null)
                {
                    continue;
                }

                var filePath = Path.Combine(FileService.EntrySoundsDirectory, soundDto.FileName);
                File.WriteAllText(filePath, soundDto.RecordingB64);
            }
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            RealmEntry? updatedEntry = null;
            _dbContext.Write(() =>
            {
                updatedEntry = RealmEntry.FromDto(updatedEntryDto, _dbContext);
            });

            if (updatedEntry == null)
            {
                throw new NullReferenceException();
            }

            foreach (var sound in updatedEntry.Sounds)
            {
                var soundDto = updatedEntryDto.Sounds.FirstOrDefault(s => s.SoundId == sound.SoundId && !string.IsNullOrEmpty(s.RecordingB64));
                if (soundDto == null)
                {
                    continue;
                }

                var filePath = Path.Combine(FileService.EntrySoundsDirectory, soundDto.FileName);
                File.WriteAllText(filePath, soundDto.RecordingB64);
            }
        }
    }
}