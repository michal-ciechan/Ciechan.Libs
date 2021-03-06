using System;
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

        [Fact]
        public void IsExactlySet_SingleSet_SingleCheck_ShouldReturnTrue()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Middle);

            checkpoint.IsExactlySet(CheckpointFlags.Start).Should().BeFalse();
            checkpoint.IsExactlySet(CheckpointFlags.Middle).Should().BeTrue();
            checkpoint.IsExactlySet(CheckpointFlags.End).Should().BeFalse();

            checkpoint.IsExactlySet(CheckpointFlags.Middle, CheckpointFlags.Start).Should().BeFalse();
            checkpoint.IsExactlySet(CheckpointFlags.Middle, CheckpointFlags.End).Should().BeFalse();
            checkpoint.IsExactlySet(CheckpointFlags.Start, CheckpointFlags.End).Should().BeFalse();

            checkpoint.IsExactlySet(CheckpointFlags.Start, CheckpointFlags.Middle, CheckpointFlags.End)
                .Should().BeFalse();
        }

        [Fact]
        public void SetPoints_SingleSet_ShouldReturnArrayOfOne()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Middle);

            checkpoint.SetPoints().Should().BeEquivalentTo(CheckpointFlags.Middle);
        }

        [Fact]
        public void SetPoints_SingleSet_CheckMultiplePoints_ShouldThrowHelpfulError()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Middle);

            Action act = () => checkpoint.SetPoints().Should().BeEquivalentTo(CheckpointFlags.Middle, CheckpointFlags.Start);

            act.Should().Throw<Exception>()
                .WithMessage(
                    "Expected collection {Middle} to be equivalent to {Middle, Start}, but it misses {Start}.");
        }

        [Fact]
        public void SetPoints_DoubleSet_CheckSinglePoint_ShouldThrowHelpfulError()
        {
            var checkpoint = new Checkpoint<CheckpointFlags>();

            checkpoint.Set(CheckpointFlags.Start);
            checkpoint.Set(CheckpointFlags.Middle);

            Action act = () => checkpoint.SetPoints().Should().BeEquivalentTo(CheckpointFlags.Start);

            act.Should().Throw<Exception>()
                .WithMessage(
                    "Expected collection {Start, Middle} to be equivalent to {Start}, but it contains too many items.");
        }
    }
}