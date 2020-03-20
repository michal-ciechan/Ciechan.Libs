using System.Diagnostics;
using System.Threading;
using Ciechan.Libs.Timing;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ciechan.Libs.Tests.Timing
{
    public class TicksTests
    {
        public TicksTests()
        {
            Ticks.Stopwatch = StaticStopwatch.ReadOnly;
        }

        private static Mock<IReadOnlyStopwatch> MockStopwatch()
        {
            var stopwatch = new Mock<IReadOnlyStopwatch>();

            Ticks.Stopwatch = stopwatch.Object;
            return stopwatch;
        }

        [Fact]
        public void Elapsed()
        {
            // Arrange
            var stopwatch = MockStopwatch();

            stopwatch.Setup(x => x.ElapsedTicks)
                .Returns(69L);

            // Act
            var elapsed = Ticks.Elapsed;

            // Assert
            elapsed.Should().Be(69L);
        }

        [Fact]
        public void Milliseconds_From0_ToStopwatchFrequency_ShouldEqual1Second()
        {
            // Arrange
            var from = 0L;
            var to = Stopwatch.Frequency;

            // Act
            var ms = Ticks.Milliseconds(from, to);

            // Assert
            ms.Should().Be(1000);
        }

        [Fact]
        public void MillisecondsSince_From0_ToStopwatchFrequency_ShouldEqual1Second()
        {
            // Arrange
            var from = Ticks.Elapsed;

            // Act
            Thread.Sleep(100);

            var ms = Ticks.MillisecondsSince(from);

            // Assert
            ms.Should().BeApproximately(100, 5);
        }
    }
}