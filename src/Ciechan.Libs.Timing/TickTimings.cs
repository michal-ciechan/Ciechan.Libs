using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ciechan.Libs.Timing
{
    public class TickTimings<TKey>
    {
        private readonly ConcurrentDictionary<TKey, TickTimingsEntry> _entries;

        public TickTimings()
        {
            _entries = new ConcurrentDictionary<TKey, TickTimingsEntry>();
        }

        public TickTimings(IEqualityComparer<TKey> equalityComparer)
        {
            _entries = new ConcurrentDictionary<TKey, TickTimingsEntry>(equalityComparer);
        }

        public IReadOnlyDictionary<TKey, TickTimingsEntry> Entries => _entries;

        public void Add(TKey key, long ticks)
        {
            _entries.AddOrUpdate(key, new TickTimingsEntry(ticks),
                (k, entry) => new TickTimingsEntry(entry.ElapsedTicks + ticks));
        }
    }
}