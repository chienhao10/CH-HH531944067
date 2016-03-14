using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using JokerFioraBuddy.Evade;

namespace JokerFioraBuddy
{
    public static class Dispeller
    {
        static Dispeller()
        {
            new Dispell("Vladimir", "vladimirhemoplaguedebuff", SpellSlot.R).Add();
            new Dispell("Tristana", "tristanaechargesound", SpellSlot.E).Add();
            new Dispell("Karma", "karmaspiritbind", SpellSlot.W).Add();
            new Dispell("Karthus", "karthusfallenone", SpellSlot.R).Add();
            new Dispell("Leblanc", "leblancsoulshackle", SpellSlot.E).Add();
            new Dispell("Leblanc", "leblancsoulshacklem", SpellSlot.R).Add();
            new Dispell("Morgana", "soulshackles", SpellSlot.R).Add();
            new Dispell("Zed", "zedultexecute", SpellSlot.R).Add();
            new Dispell("Fizz", "fizzmarinerdoombomb", SpellSlot.R).Add();

            foreach (var dispell in Dispell.GetDispellList().Where(d => EntityManager.Heroes.Enemies.Any(h => h.ChampionName.Equals(d.ChampionName))))
            {
                Config.Dispell.Menu.Add(dispell.ChampionName + dispell.BuffName, new CheckBox(dispell.ChampionName + " - " + dispell.Slot, true));
            }

            Game.OnUpdate += OnUpdate;
        }

        static void OnUpdate(EventArgs args)
        {
            if (!SpellManager.W.IsReady() || !Config.Dispell.DispellSpells)
                return;

            foreach (var dispell in Dispells.Where(
                d => 
                    Player.HasBuff(d.BuffName) && 
                    Config.Dispell.Menu[d.ChampionName + d.BuffName] != null &&
                    Config.Dispell.Menu[d.ChampionName + d.BuffName].Cast<CheckBox>().CurrentValue
                ))
            {
                var buff = Player.GetBuff(dispell.BuffName);
                if (buff == null || !buff.IsValid || !buff.IsActive)
                    continue;

                var t = (buff.EndTime - Game.Time) * 1000f + dispell.Offset + 250;
                var wT = SpellManager.W.CastDelay + Game.Ping / 2f;

                if (t < wT)
                {
                    var target = TargetSelector2.GetTarget(SpellManager.W.Range, DamageType.Mixed);

                    if (target != null && target.IsValidTarget(SpellManager.W.Range) && SpellManager.W.Cast(target))
                        return;

                    if (SpellManager.W.Cast(Game.CursorPos))
                        return;
                }
            }
        }

        public static List<Dispell> Dispells
        {
            get { return Dispell.GetDispellList(); }
        }

        public static void Initialize()
        { 

        }
    }

    public class Dispell
    {
        private static readonly List<Dispell> DispellList = new List<Dispell>();
        public string BuffName;
        public string ChampionName;
        public int Offset;
        public SpellSlot Slot;

        public Dispell(string champName, string buff, SpellSlot slot, int offset = 0)
        {
            ChampionName = champName;
            BuffName = buff;
            Slot = slot;
            Offset = offset;
        }

        public void Add()
        {
            DispellList.Add(this);
        }

        public static List<Dispell> GetDispellList()
        {
            return DispellList;
        }
    }
}
