﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_data.Services;
using chldr_utils.Interfaces;
using GraphQL;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

namespace chldr_data.local.Services
{
    public class SyncService
    {
        private readonly RequestService _requestService;
        private Timer _timer;

        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1);
        public SyncService(RequestService requestService)
        {
            _requestService = requestService;
        }
        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        private Realm _dbContext => Realm.GetInstance(RealmDataProvider.OfflineDatabaseConfiguration);


        bool _isRunning = false;
        private async Task PullRemoteDatabase()
        {
            // Remove existing database
            _dbContext.Write(() =>
            {
                _dbContext.RemoveAll();
            });

            // Get users
            var userDtos = await _requestService.Take<UserDto>(RecordType.User, 0, 100);
            _dbContext.Write(() =>
            {
                foreach (var dto in userDtos)
                {
                    _dbContext.Add(RealmUser.FromDto(dto, _dbContext));
                }
            });

            // Get sources
            var sourceDtos = await _requestService.Take<SourceDto>(RecordType.Source, 0, 100);
            _dbContext.Write(() =>
            {
                foreach (var dto in sourceDtos)
                {
                    _dbContext.Add(RealmSource.FromDto(dto, _dbContext));
                }
            });

            // Get entries with translations and sounds
            var entryDtos = await _requestService.Take<EntryDto>(RecordType.Entry, 0, 100);
            _dbContext.Write(() =>
            {
                foreach (var dto in entryDtos)
                {
                    _dbContext.Add(RealmEntry.FromDto(dto, _dbContext));
                }
            });

            // Get latest changesets
            var changeSetDtos = await _requestService.Take<ChangeSetDto>(RecordType.ChangeSet, 0, 100);
            _dbContext.Write(() =>
            {
                foreach (var dto in changeSetDtos)
                {
                    _dbContext.Add(RealmChangeSet.FromDto(dto, _dbContext));

                }
            });

            // Get by chunks until finished

        }
        internal async Task Sync()
        {
            if (_isRunning)
            {
                return;
            }

            try
            {
                _isRunning = true;
                await _syncLock.WaitAsync();

                try
                {
                    var latestChangeSet = _dbContext.All<RealmChangeSet>().LastOrDefault();
                    var latestChangeSetIndex = latestChangeSet == null ? 0 : latestChangeSet.ChangeSetIndex;

                    if (latestChangeSetIndex == 0)
                    {
                        await PullRemoteDatabase();
                    }
                    else
                    {
                        var latestRemoteChangeSets = await _requestService.TakeLast<ChangeSetDto>(RecordType.ChangeSet, 1);
                        if (latestRemoteChangeSets == null)
                        {
                            // TODO: Get latest changesets based on...?
                        }
                    }

                    _dbContext.Write(() =>
                    {


                        //foreach (var changeSet in latestRemoteChangeSets)
                        //{
                        //    var changes = JsonConvert.DeserializeObject<List<Change>>(changeSet.RecordChanges);
                        //    if (changes == null || changes.Count == 0)
                        //    {
                        //        continue;
                        //    }

                        //    // Apply changes to the local database
                        //    if (changeSet.RecordType == Enums.RecordType.Entry)
                        //    {
                        //        try
                        //        {
                        //            var realmWord = _dbContext.Find<RealmEntry>(changeSet.RecordId);
                        //            if (realmWord == null)
                        //            {
                        //                throw new NullReferenceException();
                        //            }

                        //            foreach (var change in changes)
                        //            {
                        //                SetPropertyValue(realmWord, change.Property, change.NewValue);
                        //            }
                        //        }
                        //        catch (Exception ex)
                        //        {

                        //        }
                        //    }
                        //}

                    });
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            finally
            {
                _syncLock.Release();
                _isRunning = false;
            }

        }

        internal void BeginListening()
        {
            _timer = new Timer(async state => await Sync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Task.Run(Sync);
        }
    }
}