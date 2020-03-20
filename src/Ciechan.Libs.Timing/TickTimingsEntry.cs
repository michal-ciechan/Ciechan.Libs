namespace Ciechan.Libs.Timing
{
    public struct TickTimingsEntry
    {
        public long ElapsedTicks { get; }

        public TickTimingsEntry(in long elapsedTicks)
        {
            ElapsedTicks = elapsedTicks;
        }

        public double ElapsedMilliseconds => Timing.Ticks.Milliseconds(0, ElapsedTicks);
    }
}