using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Ciechan.Libs.Testing
{
    internal class Entry
    {
        public int HitCount { get; set; }

        public TaskCompletionSource<bool> IsHitTaskCompletionSource { get; } = new TaskCompletionSource<bool>();

        public Task IsHitTask => IsHitTaskCompletionSource.Task;

        public TaskCompletionSource<bool> ContinuationTaskCompletionSource { get; } = new TaskCompletionSource<bool>();

        public Task ContinuationTask => ContinuationTaskCompletionSource.Task;

        public bool IsHit => HitCount > 0;

        public Entry Continue()
        {
            ContinuationTaskCompletionSource.SetResult(true);

            return this;
        }

        public Entry Set()
        {
            HitCount++;

            if (HitCount == 1)
                IsHitTaskCompletionSource.SetResult(true);

            return this;
        }
    }

    public class Checkpoint<T>
    {
        private readonly ConcurrentDictionary<T, Entry> _points = new ConcurrentDictionary<T, Entry>();

        public bool IsSet(T point)
        {
            return _points.GetOrAdd(point, arg => new Entry()).IsHit;
        }

        public Task WaitFor(T point)
        {
            return _points.GetOrAdd(point, arg => new Entry()).IsHitTask;
        }

        public void Set(T point)
        {
            SetInternal(point);
        }

        private Entry SetInternal(T point)
        {
            return _points.AddOrUpdate(point
                , arg => new Entry().Set()
                , (arg1, i) => i.Set());
        }

        public Task SetAndWaitForContinuation(T point)
        {
            return SetInternal(point).ContinuationTask;
        }

        public void SetAndContinue(T point)
        {
            _points.AddOrUpdate(point
                , arg => new Entry().Set().Continue()
                , (arg1, i) => i.Set().Continue()
            );
        }

        public void Continue(T point)
        {
            _points.AddOrUpdate(point
                , arg => new Entry().Continue()
                , (arg1, entry) => entry.Continue());
        }

        public Task WaitForContinuation(T point)
        {
            return _points.GetOrAdd(point, arg => new Entry()).ContinuationTask;
        }

        public Task WaitForAndContinue(T point)
        {
            var entry = _points.GetOrAdd(point, arg => new Entry());

            return entry.IsHitTask.ContinueWith(x => Continue(point));
        }
    }
}