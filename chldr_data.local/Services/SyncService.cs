using chldr_data.DatabaseObjects.Dtos;
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
using System.Collections.Generic;
using System.Diagnostics;

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
            var offset = 0;
            // Best number for requests according to the measurements
            var limit = 13000;

            // Remove existing database
            _dbContext.Write(() =>
            {
                _dbContext.RemoveAll();
            });

            var sw = Stopwatch.StartNew();

            // Get users
            IEnumerable<UserDto> userDtos;
            do
            {
                userDtos = await _requestService.Take<UserDto>(RecordType.User, offset, limit);
                _dbContext.Write(() =>
                {
                    foreach (var dto in userDtos)
                    {
                        _dbContext.Add(RealmUser.FromDto(dto, _dbContext));
                    }
                });
                offset += limit;
            } while (userDtos.Any());

            // Get all sources
            offset = 0;
            IEnumerable<SourceDto> sourceDtos;
            do
            {
                sourceDtos = await _requestService.Take<SourceDto>(RecordType.Source, offset, limit);
                _dbContext.Write(() =>
                {
                    foreach (var dto in sourceDtos)
                    {
                        _dbContext.Add(RealmSource.FromDto(dto, _dbContext));
                    }
                });
                offset += limit;
            } while (sourceDtos.Any());

            // Get all entries with translations and sounds
            var entriesStopWatch = Stopwatch.StartNew();

            offset = 0;
            IEnumerable<EntryDto> entryDtos;
            do
            {
                entryDtos = await _requestService.Take<EntryDto>(RecordType.Entry, offset, limit);

                _dbContext.Write(() =>
                        {
                            foreach (var dto in entryDtos)
                            {
                                _dbContext.Add(RealmEntry.FromDto(dto, _dbContext));
                            }
                        });
                offset += limit;
            } while (entryDtos.Any());
            var entriesPerformance = entriesStopWatch.ElapsedMilliseconds;
            // 62500, 62000

            // Get all changeSets
            offset = 0;
            IEnumerable<ChangeSetDto> changeSetDtos;
            do
            {
                changeSetDtos = await _requestService.Take<ChangeSetDto>(RecordType.ChangeSet, offset, limit);
                _dbContext.Write(() =>
                {
                    foreach (var dto in changeSetDtos)
                    {
                        _dbContext.Add(RealmChangeSet.FromDto(dto, _dbContext));
                    }
                });
                offset += limit;
            } while (entryDtos.Any());

            sw.Stop();
            var ms = sw.ElapsedMilliseconds;
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
            _timer = new Timer(async state => await Sync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            //Task.Run(Sync);
        }
    }
}