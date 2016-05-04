using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TreeLib.Extensions;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace jesuisFiora
{
    internal static class Dispeller
    {
        public static Menu Menu;

        static Dispeller()
        {
            new Dispel("Vladimir", "vladimirhemoplaguedebuff", SpellSlot.R).Add();
            new Dispel("Tristana", "tristanaechargesound", SpellSlot.E).Add();
            new Dispel("Karma", "karmaspiritbind", SpellSlot.W).Add();
            new Dispel("Karthus", "karthusfallenone", SpellSlot.R).Add();
            new Dispel("Leblanc", "leblancsoulshackle", SpellSlot.E).Add();
            new Dispel("Leblanc", "leblancsoulshacklem", SpellSlot.R).Add();
            new Dispel("Morgana", "soulshackles", SpellSlot.R).Add();
            new Dispel("Zed", "zedultexecute", SpellSlot.R).Add();
            new Dispel("Fizz", "fizzmarinerdoombomb", SpellSlot.R).Add();
        }

        public static List<Dispel> Dispells
        {
            get { return Dispel.GetDispelList(); }
        }

        public static void Initialize(Menu menu)
        {
            foreach (var dispel in Dispel.GetDispelList().Where(d => HeroManager.Enemies.Any(h => h.ChampionName.Equals(d.ChampionName))))
            {
                menu.Add("Dispel" + dispel.ChampionName + dispel.BuffName, new CheckBox("Dispel " + dispel.ChampionName + " " + dispel.Slot));
            }

            Menu = menu;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var w = SpellManager.W;

            if (!w.IsReady())
            {
                return;
            }

            foreach (var dispel in
                Dispells.Where(
                    d =>
                        ObjectManager.Player.HasBuff(d.BuffName) && Menu["Dispel" + d.ChampionName + d.BuffName] != null && Menu["Dispel" + d.ChampionName + d.BuffName].Cast<CheckBox>().CurrentValue))
            {
                var buff = ObjectManager.Player.GetBuff(dispel.BuffName);
                if (buff == null || !buff.IsValid || !buff.IsActive)
                {
                    Console.WriteLine("CONTINUE");
                    continue;
                }

                var t = (buff.EndTime - Game.Time) * 1000f + dispel.Offset + 250;
                var wT = w.Delay * 1000f + Game.Ping / 2f;
                Console.WriteLine("T: {0} WT: {1}", t, wT);
                if (t < wT)
                {
                    var target = TargetSelector.GetTarget(w.Range, DamageType.Physical);
                    Console.WriteLine("CAST DISPEL");
                    if (target != null && target.IsValidTarget(w.Range) && w.Cast(target).IsCasted())
                    {
                        return;
                    }

                    if (w.Cast(Game.CursorPos))
                    {
                        return;
                    }
                }
            }
        }
    }

    public class Dispel
    {
        private static readonly List<Dispel> DispelList = new List<Dispel>();
        public string BuffName;
        public string ChampionName;
        public int Offset;
        public SpellSlot Slot;

        public Dispel(string champName, string buff, SpellSlot slot, int offset = 0)
        {
            ChampionName = champName;
            BuffName = buff;
            Slot = slot;
            Offset = offset;
        }

        public void Add()
        {
            DispelList.Add(this);
        }

        public static List<Dispel> GetDispelList()
        {
            return DispelList;
        }
    }
}