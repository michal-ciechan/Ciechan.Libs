using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ciechan.Libs.Threading
{
    public class AsyncReentrantLock
    {
        private readonly SemaphoreSlim _topLevelLock = new SemaphoreSlim(1);
        private readonly AsyncLocal<SemaphoreSlim> _currentSemaphore = new AsyncLocal<SemaphoreSlim>();

        public Task DoWithLock(Func<Task> body, int millisecondsTimeout = -1) => DoWithLock(body, millisecondsTimeout, CancellationToken.None);
        public async Task DoWithLock(Func<Task> body, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            var currentSem = _currentSemaphore.Value;
            var isTopLevel = currentSem == null;

            // Top Level Entry for a logical call context, therefore wait for top level lock
            if (isTopLevel)
            {
                _currentSemaphore.Value = currentSem = new SemaphoreSlim(1);

                await _topLevelLock.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);
            }

            SemaphoreSlim nextSem = null;
            bool timedOutWaitingForNextSem = false;
            try
            {
                // Wait for any current Semaphore
                if (!await currentSem.WaitAsync(millisecondsTimeout, cancellationToken))
                    throw new TimeoutException(CreateTimeoutMessage(millisecondsTimeout));

                // Create new Semaphore for any re-entrant threads to be able to await
                nextSem = new SemaphoreSlim(1);

                _currentSemaphore.Value = nextSem;

                await body();
            }
            finally
            {
                if (nextSem != null)
                {
                    Debug.Assert(nextSem == _currentSemaphore.Value);

                    timedOutWaitingForNextSem = !await nextSem.WaitAsync(millisecondsTimeout, cancellationToken);
                }

                Debug.Assert(currentSem != null);

                _currentSemaphore.Value = currentSem;
                currentSem.Release();

                if (isTopLevel)
                    _topLevelLock.Release();
            }

            if(cancellationToken.IsCancellationRequested)
                return;
            
            if(timedOutWaitingForNextSem)
                throw new TimeoutException(CreateTimeoutMessage(millisecondsTimeout));
        }

        private string CreateTimeoutMessage(int millisecondsTimeout) =>
            $"Timeout happened waiting for an inner (re-entered) lock inside {nameof(AsyncReentrantLock)} after {millisecondsTimeout:N0}";
    }
}
