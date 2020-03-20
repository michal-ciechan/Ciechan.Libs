using System.Diagnostics;
using System.Threading;
using Ciechan.Libs.Timing;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Timing
{
    public class StopwatchWrapperTests
    {
        private readonly Stopwatch _stopwatch;
        private readonly StopwatchWrapper _subject;

        public StopwatchWrapperTests()
        {
            _stopwatch = Stopwatch.StartNew();
            _subject = new StopwatchWrapper(_stopwatch);
        }

        [Fact]
        public void ElapsedTicks_FrozenStopwatch_ShouldReturnSameElapsedTicks()
        {
            // Arrange
            _stopwatch.Stop();

            // Act
            var ticks = _subject.ElapsedTicks;

            Thread.Sleep(10);

            // Assert
            ticks.Should().Be(_subject.ElapsedTicks);
        }


        [Fact]
        public void ElapsedTicks_RunningStopwatch_ShouldReturnElapsedTicks()
        {
            // Act
            var ticks = _subject.ElapsedTicks;

            Thread.Sleep(10);

            // Assert
            ticks.Should().BeLessThan(_subject.ElapsedTicks);
        }


        [Fact]
        public void Frequency_ShouldReturnStopwatchFrequency()
        {
            // Assert
            _subject.Frequency.Should().Be(Stopwatch.Frequency);
        }

        [Fact]
        public void Reset_ShouldReturnStopwatchFrequency()
        {
            // Arrange
            _subject.ElapsedTicks.Should().BeGreaterThan(0);

            // Act
            _subject.Reset();

            // Assert
            _stopwatch.IsRunning.Should().BeFalse();
            _subject.ElapsedTicks.Should().Be(0);
        }

        [Fact]
        public void Restart_ShouldReturnStopwatchFrequency()
        {
            // Arrange
            Thread.Sleep(10);

            var initialTicks = _subject.ElapsedTicks;

            initialTicks.Should().BeGreaterThan(0);

            // Act

            _subject.Restart();

            // Assert
            _stopwatch.IsRunning.Should().BeTrue();
            _subject.ElapsedTicks.Should().BeLessThan(initialTicks);
            _subject.ElapsedTicks.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ElapsedMilliseconds_FrozenStopwatch_ShouldReturnSameElapsedTicks()
        {
            // Arrange
            _stopwatch.Stop();

            // Act
            var ms = _subject.ElapsedMilliseconds;

            Thread.Sleep(10);

            // Assert
            ms.Should().Be(_subject.ElapsedMilliseconds);
        }

        [Fact]
        public void ElapsedMilliseconds_RunningStopwatch_ShouldReturnElapsedTicks()
        {
            // Act
            var ms = _subject.ElapsedMilliseconds;

            Thread.Sleep(10);

            // Assert
            ms.Should().BeLessThan(_subject.ElapsedMilliseconds);
        }

        [Fact]
        public void Elapsed_FrozenStopwatch_ShouldReturnSameElapsedTicks()
        {
            // Arrange
            _stopwatch.Stop();

            // Act
            var elapsed = _subject.Elapsed;

            Thread.Sleep(10);

            // Assert
            elapsed.Should().Be(_subject.Elapsed);
        }

        [Fact]
        public void Elapsed_RunningStopwatch_ShouldReturnElapsedTicks()
        {
            // Act
            var elapsed = _subject.Elapsed;

            Thread.Sleep(10);

            // Assert
            elapsed.Should().BeLessThan(_subject.Elapsed);
        }

        [Fact]
        public void IsRunning_Default_StopwatchShouldBeRunning()
        {
            // Assert
            _subject.IsRunning.Should().Be(true);
        }

        [Fact]
        public void IsRunning_Stop_StopwatchShouldNotBeRunning()
        {
            // Act
            _stopwatch.Stop();
            
            // Assert
            _subject.IsRunning.Should().Be(false);
        }

        [Fact]
        public void IsRunning_Reset_StopwatchShouldNotBeRunning()
        {
            // Act
            _stopwatch.Reset();

            // Assert
            _subject.IsRunning.Should().Be(false);
        }

        [Fact]
        public void IsRunning_Reset_Start_StopwatchShouldBeRunning()
        {
            // Act
            _stopwatch.Reset();
            _stopwatch.Start();

            // Assert
            _subject.IsRunning.Should().Be(true);
        }

        [Fact]
        public void Stop_StopwatchShouldNotBeRunning()
        {
            // Act
            _subject.Stop();

            // Assert
            _stopwatch.IsRunning.Should().Be(false);
        }

        [Fact]
        public void Stop_Start_StopwatchShouldBeRunning()
        {
            // Act
            _subject.Stop();
            _subject.Start();

            // Assert
            _stopwatch.IsRunning.Should().Be(true);
        }
    }
}