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

using static Realms.ThreadSafeReference;

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

        private async Task<System.Collections.Generic.List<T>> RetrieveAll<T>(RecordType recordType) where T : class, new()
        {
            // Best number for requests according to the measurements
            var limit = 13000;
            int offset = 0;

            var combinedResults = new System.Collections.Generic.List<T>();
            IEnumerable<T> newResults;
            do
            {
                var response = await _requestService.Take<T>(recordType, offset, limit);
                if (!response.Success)
                {
                    throw new Exception(response.ErrorMessage);
                }

                newResults = response.Data<IEnumerable<T>>();
                combinedResults.AddRange(newResults);

                offset += limit;
            } while (newResults.Any());

            return combinedResults;
        }

        private async Task PullRemoteDatabase()
        {
            // Remove existing database
            _dbContext.Write(() =>
            {
                _dbContext.RemoveAll();
            });

            var sw = Stopwatch.StartNew();

            // Get users
            var userDtos = await RetrieveAll<UserDto>(RecordType.User);
            var sourceDtos = await RetrieveAll<SourceDto>(RecordType.Source);
            var entryDtos = await RetrieveAll<EntryDto>(RecordType.Entry);
            var changeSetDtos = await RetrieveAll<ChangeSetDto>(RecordType.ChangeSet);

            var downloadedIn = sw.ElapsedMilliseconds;
            sw.Restart();

            _dbContext.Write(() =>
            {
                foreach (var dto in userDtos)
                {
                    _dbContext.Add(RealmUser.FromDto(dto, _dbContext));
                }

                foreach (var dto in sourceDtos)
                {
                    _dbContext.Add(RealmSource.FromDto(dto, _dbContext));
                }

                foreach (var dto in entryDtos)
                {
                    _dbContext.Add(RealmEntry.FromDto(dto, _dbContext));
                }

                foreach (var dto in changeSetDtos)
                {
                    _dbContext.Add(RealmChangeSet.FromDto(dto, _dbContext));
                }
            });

            sw.Stop();
            var savedIn = sw.ElapsedMilliseconds;
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

                // Return if there are no changesets available on remote server
                var response = await _requestService.TakeLast<ChangeSetDto>(RecordType.ChangeSet, Constants.ChangeSetsToApply);
                var latestRemoteChangeSets = response.Data<IEnumerable<ChangeSetDto>>();

                if (!latestRemoteChangeSets.Any())
                {
                    return;
                }

                var latestlocalChangeSet = _dbContext.All<RealmChangeSet>().LastOrDefault();
                var latestlocalChangeSetIndex = latestlocalChangeSet == null ? 0 : latestlocalChangeSet.ChangeSetIndex;

                // If there are no changesets available in local database - hardreset
                if (latestlocalChangeSetIndex == 0)
                {
                    await PullRemoteDatabase();
                    return;
                }

                var latestRemoteChangeSet = latestRemoteChangeSets.Last();
                if (latestRemoteChangeSet.ChangeSetIndex <= latestlocalChangeSetIndex)
                {
                    return;
                }

                // If more than 1000 changes have been made - just perform hard reset
                if ((latestRemoteChangeSet.ChangeSetIndex - latestlocalChangeSetIndex) > Constants.ChangeSetsToApply)
                {
                    await PullRemoteDatabase();
                    return;
                }

                ApplyChangeSets(latestRemoteChangeSets);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _syncLock.Release();
                _isRunning = false;
            }

        }

        private void ApplyChangeSets(IEnumerable<ChangeSetDto> changeSets)
        {
            _dbContext.Write(() =>
            {
                foreach (var changeSet in changeSets)
                {
                    switch (changeSet.Operation)
                    {
                        case Operation.Insert:
                            break;
                        case Operation.Delete:
                            break;
                        case Operation.Update:
                            var changes = JsonConvert.DeserializeObject<System.Collections.Generic.List<Change>>(changeSet.RecordChanges);
                            if (changes == null || changes.Count == 0)
                            {
                                continue;
                            }

                            //ApplyChanges(changeSet.RecordType, changeSet.RecordId, changes);
                            break;
                        default:
                            break;
                    }

                }

            });
        }
        private void ApplyChanges<T>(RecordType recordType, string entityId, IEnumerable<Change> changes)
        {
            switch (recordType)
            {
                case RecordType.Entry:
                    ApplyChangesForType<RealmEntry>(entityId, changes);
                    break;
                case RecordType.User:
                    ApplyChangesForType<RealmUser>(entityId, changes);
                    break;
                case RecordType.Source:
                    ApplyChangesForType<RealmSource>(entityId, changes);
                    break;
                case RecordType.Sound:
                    ApplyChangesForType<RealmSound>(entityId, changes);
                    break;
                case RecordType.Translation:
                    ApplyChangesForType<RealmTranslation>(entityId, changes);
                    break;
                case RecordType.ChangeSet:
                    ApplyChangesForType<RealmChangeSet>(entityId, changes);
                    break;
                default:
                    break;
            }
        }

        private void ApplyChangesForType<T>(string entityId, IEnumerable<Change> changes) where T : RealmObject
        {
            var realmWord = _dbContext.Find<T>(entityId);
            foreach (var change in changes)
            {
                SetPropertyValue(realmWord, change.Property, change.NewValue);
            }
        }

        internal void BeginListening()
        {
            _timer = new Timer(async state => await Sync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            //Task.Run(Sync);
        }
    }
}