using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARAMDetFull
{
    public class Aggresivity
    {
        private static List<AgresiveMove> agresiveMoves = new List<AgresiveMove>();

        public static void addAgresiveMove(AgresiveMove move)
        {
            agresiveMoves.Add(move);
        }

        public static int getAgroBalance()
        {
            agresiveMoves.RemoveAll(mov => mov.endAt < LXOrbwalker.now);
            if (!agresiveMoves.Any())
                return 0;
            return agresiveMoves.Max(agr => agr.agroBalance);
        }

        public static bool getIgnoreMinions()
        {
            agresiveMoves.RemoveAll(mov => mov.endAt < LXOrbwalker.now);
            
            return agresiveMoves.Any(agr => agr.ignoreMinions);
        }

    }

    public class AgresiveMove
    {
        public int endAt;
        public int agroBalance;
        public bool ignoreMinions;

        public AgresiveMove(int agro = 10, int duration = 5000, bool ignoreMins = false)
        {
            agroBalance = 10;
            endAt = LXOrbwalker.now + duration;
            ignoreMinions = ignoreMins;
        }
    }
}
