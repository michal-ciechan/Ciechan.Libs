using System.Threading;
using Ciechan.Libs.Testing;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Testing
{
    public class CheckpointTests
    {
        private enum CheckpointFlags
        {
            Start,
            Middle,
            End
        }

        [Fact]
        public void Ctor_ShouldNotHaveAnyFlagSet()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.IsSet(CheckpointFlags.Start).Should().BeFalse();
            checkpoint.IsSet(CheckpointFlags.Middle).Should().BeFalse();
            checkpoint.IsSet(CheckpointFlags.End).Should().BeFalse();
        }

        [Fact]
        public void Set_All_ShouldHaveAllFlagsSet()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Start);
            checkpoint.Set(CheckpointFlags.Middle);
            checkpoint.Set(CheckpointFlags.End);

            checkpoint.IsSet(CheckpointFlags.Start).Should().BeTrue();
            checkpoint.IsSet(CheckpointFlags.Middle).Should().BeTrue();
            checkpoint.IsSet(CheckpointFlags.End).Should().BeTrue();
        }

        [Fact]
        public void Set_Middle_ShouldOnlyHaveMiddleFlagSet()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Middle);

            checkpoint.IsSet(CheckpointFlags.Start).Should().BeFalse();
            checkpoint.IsSet(CheckpointFlags.Middle).Should().BeTrue();
            checkpoint.IsSet(CheckpointFlags.End).Should().BeFalse();
        }

        [Fact]
        public void Set_Start_ShouldOnlyHaveStartFlagSet()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Start);

            checkpoint.IsSet(CheckpointFlags.Start).Should().BeTrue();
            checkpoint.IsSet(CheckpointFlags.Middle).Should().BeFalse();
            checkpoint.IsSet(CheckpointFlags.End).Should().BeFalse();
        }

        [Fact]
        public void Set_WhenStartFlagsSet_ShouldNotHaveStartContinueTaskCompleted()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Start);

            checkpoint.WaitForContinuation(CheckpointFlags.Start).IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void SetAndWaitForContinuation_WhenStartFlagsSet_ShouldReturnUncompletedTaskUntilContinueIsCalled()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            var task = checkpoint.SetAndWaitForContinuation(CheckpointFlags.Start);

            task.IsCompleted.Should().BeFalse();

            checkpoint.Continue(CheckpointFlags.Start);

            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void WaitFor_WhenNoFlagsSet_ShouldReturnNonCompletedTask()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.WaitFor(CheckpointFlags.Start).IsCompleted.Should().BeFalse();
            checkpoint.WaitFor(CheckpointFlags.Middle).IsCompleted.Should().BeFalse();
            checkpoint.WaitFor(CheckpointFlags.End).IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void WaitFor_WhenStartFlagsSet_ShouldReturnCompletedTaskForStartFlag()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Start);

            checkpoint.WaitFor(CheckpointFlags.Start).IsCompleted.Should().BeTrue();
            checkpoint.WaitFor(CheckpointFlags.Middle).IsCompleted.Should().BeFalse();
            checkpoint.WaitFor(CheckpointFlags.End).IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void WaitForAndContinue_WhenStartFlagsSet_ShouldReturnCompletedTaskForStartFlag()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            var waitTask = checkpoint.WaitForAndContinue(CheckpointFlags.Middle);
            var continueTask = checkpoint.WaitForContinuation(CheckpointFlags.Middle);

            waitTask.IsCompleted.Should().BeFalse();
            continueTask.IsCompleted.Should().BeFalse();

            checkpoint.Set(CheckpointFlags.Middle);

            Thread.Sleep(5); // Let continuation happen

            waitTask.IsCompleted.Should().BeTrue();
            continueTask.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void WaitForContinuation_ShouldBeCompletedWhenContinueCalled()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            var continueTask = checkpoint.WaitForContinuation(CheckpointFlags.Middle);

            continueTask.IsCompleted.Should().BeFalse();

            checkpoint.Continue(CheckpointFlags.Middle);

            continueTask.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void WaitForContinuation_ShouldBeCompletedWhenSetAndContinueCalled()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            var continueTask = checkpoint.WaitForContinuation(CheckpointFlags.Middle);

            continueTask.IsCompleted.Should().BeFalse();

            checkpoint.SetAndContinue(CheckpointFlags.Middle);

            continueTask.IsCompleted.Should().BeTrue();
        }
    }
}