using System.Collections.Generic;
using System.Linq;
using Ciechan.Libs.Timing;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Timing
{
    public class TickTimingsTests
    {
        [Fact]
        public void AddSingleTimings()
        {
            // Arrange
            var timings = new TickTimings<string>();

            // Act
            timings.Add("Test", Ticks.TicksPerMillisecond);

            // Assert
            var results = GetResults(timings);

            results.Should().BeEquivalentTo(new[]
            {
                ("Test", 1)
            });
        }

        private static List<(string key, double ms)> GetResults(TickTimings<string> timings)
        {
            return timings.Entries
                .Select(pair => (key: pair.Key, ms: pair.Value.ElapsedMilliseconds))
                .ToList();
        }

        [Fact]
        public void AddSameKeyTwice_ShouldIncrementTimerForKey()
        {
            // Act
            var timings = new TickTimings<string>();

            // Act
            timings.Add("Test", Ticks.TicksPerMillisecond);
            timings.Add("Test", Ticks.TicksPerMillisecond);

            // Assert
            var results = GetResults(timings);

            results.Should().BeEquivalentTo(new[]
            {
                ("Test", 2)
            });
        }

        [Fact]
        public void AddThreeKeysTwice_ShouldIncrementTimerForKey()
        {
            // Act
            var timings = new TickTimings<string>();

            // Act
            timings.Add("Test1", Ticks.TicksPerMillisecond);
            timings.Add("Test1", Ticks.TicksPerMillisecond);
            timings.Add("Test2", Ticks.TicksPerMillisecond);
            timings.Add("Test2", Ticks.TicksPerMillisecond);
            timings.Add("Test3", Ticks.TicksPerMillisecond);
            timings.Add("Test3", Ticks.TicksPerMillisecond);

            // Assert
            var results = GetResults(timings);

            results.Should().BeEquivalentTo(new[]
            {
                ("Test1", 2),
                ("Test2", 2),
                ("Test3", 2),
            });
        }

        [Fact]
        public void CustomNeverEqualComparer_AddSameString_ShouldHave2Entries()
        {
            // Act
            var timings = new TickTimings<string>(new StringNeverEqualEqualityComparer());

            // Act
            timings.Add("Test1", Ticks.TicksPerMillisecond);
            timings.Add("Test1", Ticks.TicksPerMillisecond);

            // Assert
            var results = GetResults(timings);

            results.Should().BeEquivalentTo(new[]
            {
                ("Test1", 1),
                ("Test1", 1),
            });
        }
    }
}