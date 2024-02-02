using chldr_data.DatabaseObjects.Models;

namespace chldr_app.Services
{
    public class EntryCacheService
    {
        public List<EntryModel> All => _cache.SelectMany(d => d.Value).ToList();

        private Dictionary<string, List<EntryModel>> _cache;
        private Queue<string> _usageQueue;
        private int Capacity { get; set; } = 50;

        public EntryCacheService()
        {
            _cache = new Dictionary<string, List<EntryModel>>(Capacity);
            _usageQueue = new Queue<string>(Capacity);
        }

        public void Add(string key, List<EntryModel> value)
        {
            if (!_cache.ContainsKey(key))
            {
                if (_cache.Count == Capacity)
                {
                    var oldestKey = _usageQueue.Dequeue();
                    _cache.Remove(oldestKey);
                }

                _cache[key] = value;
                _usageQueue.Enqueue(key);
            }
            else
            {
                // Update the value and refresh its position in the usage queue
                _cache[key] = value;
                RefreshUsageOrder(key);
            }
        }

        public List<EntryModel>? Get(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                RefreshUsageOrder(key);
                return value;
            }

            return null;
        }

        private void RefreshUsageOrder(string key)
        {
            // Check if the key exists in the usage queue
            bool existsInQueue = _usageQueue.Contains(key);

            // If the key exists in the queue, remove it
            if (existsInQueue)
            {
                List<string> temp = new List<string>(_usageQueue);
                temp.Remove(key);
                _usageQueue = new Queue<string>(temp);
            }

            // Add the key back to the end of the queue
            _usageQueue.Enqueue(key);
        }
        public void Update(EntryModel updatedEntry)
        {
            foreach (var key in _cache.Keys.ToList())
            {
                var entryList = _cache[key];
                var index = entryList.FindIndex(entry => entry.EntryId == updatedEntry.EntryId);

                if (index != -1)
                {
                    entryList[index] = updatedEntry;
                    RefreshUsageOrder(key);
                    break;
                }
            }
        }

        public void Remove(EntryModel entryToRemove)
        {
            foreach (var key in _cache.Keys.ToList())
            {
                var entryList = _cache[key];
                var index = entryList.FindIndex(entry => entry.EntryId == entryToRemove.EntryId);

                if (index != -1)
                {
                    entryList.RemoveAt(index);
                    if (!entryList.Any())
                    {
                        _cache.Remove(key);
                        RemoveFromUsageQueue(key);
                    }
                    break;
                }
            }
        }
        private void RemoveFromUsageQueue(string key)
        {
            var temp = new List<string>(_usageQueue);
            temp.Remove(key);
            _usageQueue = new Queue<string>(temp);
        }
    }
}
