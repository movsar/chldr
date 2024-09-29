using domain.DatabaseObjects.Dtos;
using domain;
using realm_dl.RealmEntities;
using domain.Models;
using Newtonsoft.Json;
using Realms;
using System.Diagnostics;
using domain.DatabaseObjects.Models;
using realm_dl.Interfaces;
using domain.Interfaces;
using domain;
using domain.Enums;

namespace realm_dl.Services
{
    public class SyncService : ISyncService
    {
        private readonly IRequestService _requestService;
        private readonly IFileService _fileService;
        private Timer _timer;

        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1);
        public SyncService(IRequestService requestService, IFileService fileService)
        {
            _requestService = requestService;
            _fileService = fileService;

            BeginListening();
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


        static bool _isRunning = false;

        private async Task<List<T>> RetrieveAll<T>(RecordType recordType) where T : class, new()
        {
            // Best number for requests according to the measurements
            var limit = 100;
            int offset = 0;

            var combinedResults = new List<T>();
            IEnumerable<T> newResults;
            do
            {
                var response = await _requestService.TakeAsync(recordType, offset, limit);
                if (!response.Success)
                {
                    throw new Exception(response.ErrorMessage);
                }

                newResults = RequestResult.GetData<IEnumerable<T>>(response);
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
            var users = await RetrieveAll<UserModel>(RecordType.User);
            var sources = await RetrieveAll<SourceModel>(RecordType.Source);
            var entries = await RetrieveAll<EntryModel>(RecordType.Entry);
            var changeSets = await RetrieveAll<ChangeSetModel>(RecordType.ChangeSet);

            var downloadedIn = sw.ElapsedMilliseconds;
            sw.Restart();

            _dbContext.Write(() =>
            {
                foreach (var user in users)
                {
                    _dbContext.Add(RealmUser.FromModel(user));
                }

                foreach (var source in sources)
                {
                    _dbContext.Add(RealmSource.FromModel(source));
                }

                foreach (var entry in entries)
                {
                    var user = _dbContext.Find<RealmUser>(entry.UserId);

                    if (_dbContext.Find<RealmEntry>(entry.EntryId) != null)
                    {
                        continue;
                    }

                    _dbContext.Add(RealmEntry.FromModel(entry, _dbContext));
                }

                foreach (var changeSet in changeSets)
                {
                    _dbContext.Add(RealmChangeSet.FromModel(changeSet));
                }
            });

            var entry = _dbContext.All<RealmEntry>().ToList()[0];
            var entryModels = EntryModel.FromEntity(entry, entry.Translations, entry.Sounds);

            Realm.Compact(RealmDataProvider.OfflineDatabaseConfiguration);
            _dbContext.WriteCopy(new RealmConfiguration(_fileService.DatabaseFilePath + ".new"));

            sw.Stop();
            var savedIn = sw.ElapsedMilliseconds;
        }

        public  async Task Sync()
        {
            if (_isRunning || !_requestService.IsNetworUp)
            {
                return;
            }

            try
            {
                _isRunning = true;
                await _syncLock.WaitAsync();

                // Return if there are no changesets available on remote server
                var response = await _requestService.TakeLastAsync(RecordType.ChangeSet, Constants.ChangeSetsToApply);
                if (!response.Success)
                {
                    throw new Exception(response.ErrorMessage);
                }

                var latestlocalChangeSet = _dbContext.All<RealmChangeSet>().LastOrDefault();
                var latestlocalChangeSetIndex = latestlocalChangeSet == null ? 0 : latestlocalChangeSet.ChangeSetIndex;

                // If there are no changesets available in local database - hardreset
                if (latestlocalChangeSetIndex == 0)
                {
                    //await PullRemoteDatabase();
                    return;
                }

                var latestRemoteChangeSets = RequestResult.GetData<IEnumerable<ChangeSetDto>>(response);
                if (!latestRemoteChangeSets.Any())
                {
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
                else
                {
                    // Update database
                    // ApplyChangeSets(latestRemoteChangeSets);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Sync() Error occurred");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
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
                            var changes = JsonConvert.DeserializeObject<List<Change>>(changeSet.RecordChanges);
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
        }
    }
}