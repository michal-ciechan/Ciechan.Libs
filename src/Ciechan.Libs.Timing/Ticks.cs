namespace Ciechan.Libs.Timing
{
    /// <summary>
    /// Static helper around Stopwatch.ElapsedTicks
    /// </summary>
    public static class Ticks
    {
        public static IReadOnlyStopwatch Stopwatch = StaticStopwatch.ReadOnly;

        public static long Elapsed => Stopwatch.ElapsedTicks;

        public static long TicksPerMillisecond => Stopwatch.TickPerMillisecond;

        public static double Milliseconds(in long fromTicks, in long toTicks)
        {
            var ticksDiff = toTicks - fromTicks;

            return ((double)ticksDiff) / TicksPerMillisecond;
        }

        public static double MillisecondsSince(in long fromTicks) => Milliseconds(fromTicks, Elapsed);
    }
}