using Ciechan.Libs.Testing;
using System;
using System.Threading.Tasks;
using Ciechan.Libs.Threading;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Threading
{
    public class AsyncReentrantLockExTests
    {
        private AsyncReentrantLockEx _lock;

        private enum Checkpoints
        {
            Start,
            OuterLockCalled,
            InsideOuterLockStart,
            InsideInnerLockStart,
            InsideOuterLockInnerLockCalled,
            InsideOuterLockEnd,
            End
        }

        [Fact]
        public async Task LockCreatedOnBackgroundTask_2ThreadsAccessWithReentrancy()
        {
            var tc1 = new Checkpoint<Checkpoints>();
            var tc2 = new Checkpoint<Checkpoints>();

            _lock = await Task.Run(() => new AsyncReentrantLockEx());

            var task1 = CreateAndStartTask(tc1);
            var task2 = CreateAndStartTask(tc2);

            task1.Should().NotBeNull();
            task2.Should().NotBeNull();

            await tc1.WaitFor(Checkpoints.Start);
            await tc2.WaitFor(Checkpoints.Start);

            Task CreateAndStartTask(Checkpoint<Checkpoints> checkpoint)
            {
                return Task.Run(() =>
                {
                    checkpoint.SetAndWaitForContinuationSync(Checkpoints.Start);

                    var outerLockTask = _lock.Lock();

                    checkpoint.SetAndWaitForContinuationSync(Checkpoints.OuterLockCalled);

                    using (outerLockTask.GetAwaiter().GetResult())
                    {
                        checkpoint.SetAndWaitForContinuationSync(Checkpoints.InsideOuterLockStart);

                        var innerLockTask = _lock.Lock();

                        checkpoint.SetAndWaitForContinuationSync(Checkpoints.InsideOuterLockInnerLockCalled);

                        using (innerLockTask.GetAwaiter().GetResult())
                        {
                            checkpoint.SetAndWaitForContinuationSync(Checkpoints.InsideInnerLockStart);
                        }

                        checkpoint.SetAndWaitForContinuationSync(Checkpoints.InsideOuterLockEnd);
                    }

                    checkpoint.SetAndWaitForContinuationSync(Checkpoints.End);
                });
            }
        }
    }
}
