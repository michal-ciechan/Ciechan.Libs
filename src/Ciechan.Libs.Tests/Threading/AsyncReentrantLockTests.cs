using Ciechan.Libs.Testing;
using System;
using System.Threading.Tasks;
using Ciechan.Libs.Threading;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Threading
{
    public class AsyncReentrantLockTests
    {
        private AsyncReentrantLock _lock;

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

            _lock = await Task.Run(() => new AsyncReentrantLock());

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
                    checkpoint.SetAndWaitForContinuation(Checkpoints.Start);

                    var outerLock = _lock.DoWithLock(() =>
                    {
                        checkpoint.SetAndWaitForContinuation(Checkpoints.InsideOuterLockStart);

                        var innerLock = _lock.DoWithLock(() =>
                        {
                            checkpoint.SetAndWaitForContinuation(Checkpoints.InsideInnerLockStart);

                            return Task.CompletedTask;
                        });

                        checkpoint.SetAndWaitForContinuation(Checkpoints.InsideOuterLockInnerLockCalled);

                        innerLock.GetAwaiter().GetResult();

                        checkpoint.SetAndWaitForContinuation(Checkpoints.InsideOuterLockEnd);

                        return Task.CompletedTask;
                    });

                    checkpoint.SetAndWaitForContinuation(Checkpoints.OuterLockCalled);

                    outerLock.GetAwaiter().GetResult();

                    checkpoint.SetAndWaitForContinuation(Checkpoints.End);
                });
            }
        }
    }

    public class TestableTaskCompletionSource
    {
        private TestableTaskCompletionSource<object> _tcs = new TestableTaskCompletionSource<object>();


    }
    public class TestableTaskCompletionSource<T>
    {

    }
}
