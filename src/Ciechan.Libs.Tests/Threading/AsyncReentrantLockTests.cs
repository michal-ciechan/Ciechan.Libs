using Ciechan.Libs.Testing;
using System;
using System.Threading;
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
            End,
            OuterSubTaskStart,
            OuterSubTaskInsideLock,
            OuterSubTaskLockCalled,
            OuterSubTaskEnd,
            InsideOuterLockSubTaskCalled
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

            tc1.SetPoints().Should().BeEquivalentTo(new[]
            {
                Checkpoints.Start
            });
            tc2.SetPoints().Should().BeEquivalentTo(new[]
            {
                Checkpoints.Start
            });

            {
                ContinueAndWait(tc1, Checkpoints.Start);

                tc1.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start,
                    Checkpoints.OuterLockCalled,
                    Checkpoints.InsideOuterLockStart
                });
                tc2.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start
                });
            }
            {
                ContinueAndWait(tc1, Checkpoints.OuterLockCalled);

                tc1.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start,
                    Checkpoints.OuterLockCalled,
                    Checkpoints.InsideOuterLockStart
                });
                tc2.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start
                });
            }
            {
                ContinueAndWait(tc1, Checkpoints.InsideOuterLockStart);

                tc1.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start,
                    Checkpoints.OuterLockCalled,
                    Checkpoints.InsideOuterLockStart,
                    Checkpoints.InsideOuterLockInnerLockCalled,
                    Checkpoints.InsideInnerLockStart
                });
                tc2.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start
                });
            }
            {
                ContinueAndWait(tc1, Checkpoints.InsideOuterLockInnerLockCalled);

                tc1.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start,
                    Checkpoints.OuterLockCalled,
                    Checkpoints.InsideOuterLockStart,
                    Checkpoints.InsideOuterLockInnerLockCalled,
                    Checkpoints.InsideInnerLockStart,
                    Checkpoints.OuterSubTaskStart,
                    Checkpoints.InsideOuterLockSubTaskCalled
                });
                tc2.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start
                });
            }
            {
                ContinueAndWait(tc1, Checkpoints.InsideInnerLockStart);

                tc1.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start,
                    Checkpoints.OuterLockCalled,
                    Checkpoints.InsideOuterLockStart,
                    Checkpoints.InsideOuterLockInnerLockCalled,
                    Checkpoints.InsideInnerLockStart,
                    Checkpoints.InsideOuterLockEnd
                });
                tc2.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start
                });
            }
            {
                ContinueAndWait(tc1, Checkpoints.InsideOuterLockEnd);

                tc1.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start,
                    Checkpoints.OuterLockCalled,
                    Checkpoints.InsideOuterLockStart,
                    Checkpoints.InsideOuterLockInnerLockCalled,
                    Checkpoints.InsideInnerLockStart,
                    Checkpoints.InsideOuterLockEnd,
                    Checkpoints.End
                });
                tc2.SetPoints().Should().BeEquivalentTo(new[]
                {
                    Checkpoints.Start
                });
            }


            Task CreateAndStartTask(Checkpoint<Checkpoints> checkpoint)
            {
                return Task.Run(async () =>
                {
                    await checkpoint.SetAndWaitForContinuation(Checkpoints.Start);

                    var outerLock = _lock.DoWithLock(async () =>
                    {
                        await checkpoint.SetAndWaitForContinuation(Checkpoints.InsideOuterLockStart);

                        var innerLock = _lock.DoWithLock(async () =>
                        {
                            await checkpoint.SetAndWaitForContinuation(Checkpoints.InsideInnerLockStart);
                        });

                        await checkpoint.SetAndWaitForContinuation(Checkpoints.InsideOuterLockInnerLockCalled);

                        Task.Run(async () =>
                        {
                            await checkpoint.SetAndWaitForContinuation(Checkpoints.OuterSubTaskStart);

                            var subTaskLock = _lock.DoWithLock(async () =>
                            {
                                await checkpoint.SetAndWaitForContinuation(Checkpoints.OuterSubTaskInsideLock);
                            });

                            await checkpoint.SetAndWaitForContinuation(Checkpoints.OuterSubTaskLockCalled);

                            subTaskLock.GetAwaiter().GetResult();

                            await checkpoint.SetAndWaitForContinuation(Checkpoints.OuterSubTaskEnd);
                        }).GetAwaiter();

                        await checkpoint.SetAndWaitForContinuation(Checkpoints.InsideOuterLockSubTaskCalled);

                        innerLock.GetAwaiter().GetResult();

                        await checkpoint.SetAndWaitForContinuation(Checkpoints.InsideOuterLockEnd);
                    });

                    await checkpoint.SetAndWaitForContinuation(Checkpoints.OuterLockCalled);

                    outerLock.GetAwaiter().GetResult();

                    await checkpoint.SetAndWaitForContinuation(Checkpoints.End);
                });
            }
        }

        private static void ContinueAndWait(Checkpoint<Checkpoints> tc1, Checkpoints point)
        {
            tc1.Continue(point);

            Thread.Sleep(100);
        }
    }
}
