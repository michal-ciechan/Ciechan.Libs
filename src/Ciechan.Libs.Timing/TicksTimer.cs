namespace Ciechan.Libs.Timing
{
    public struct TicksTimer
    {
        public long Ticks { get; }

        public TicksTimer(long ticks)
        {
            Ticks = ticks;
        }

        public double ElapsedMilliseconds => Timing.Ticks.MillisecondsSince(Ticks);
        public long Elapsed => Timing.Ticks.Elapsed - Ticks;

        public static TicksTimer Create()
        {
            return new TicksTimer(Timing.Ticks.Elapsed);
        }
    }
}