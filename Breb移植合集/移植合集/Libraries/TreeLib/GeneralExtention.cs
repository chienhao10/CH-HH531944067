using LeagueSharp.Common;

namespace TreeLib.Extensions
{
    public static class GeneralExtensions
    {
        public static bool HasTimePassed(this int time, int duration)
        {
            return time.TimeSince() >= duration;
        }

        public static int TimeSince(this int time)
        {
            return Utils.TickCount - time;
        }
    }
}