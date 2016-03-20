using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc.Modes
{
    internal class JungleClear
    {
        public static bool UseQ
        {
            get { return Config.JungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseW
        {
            get { return Config.JungleClearMenu["useW"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseE
        {
            get { return Config.JungleClearMenu["useE"].Cast<CheckBox>().CurrentValue; }
        }

        public static int SliderW
        {
            get { return Config.JungleClearMenu["sliderW"].Cast<Slider>().CurrentValue; }
        }

        public static bool UseQr
        {
            get { return Config.JungleClearMenu["useQR"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseWr
        {
            get { return Config.JungleClearMenu["useWR"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEr
        {
            get { return Config.JungleClearMenu["useER"].Cast<CheckBox>().CurrentValue; }
        }

        public static int SliderWr
        {
            get { return Config.JungleClearMenu["sliderWR"].Cast<Slider>().CurrentValue; }
        }

        public static void Execute()
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level < 1)
            {
                Pre6JungleClear();
            }
            else
            {
                Post6JungleClear();
            }
        }

        public static void Pre6JungleClear()
        {
            if (UseQ && Program.Q.IsReady())
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, Program.Q.Range)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (minion != null)
                {
                    Program.Q.Cast(minion);
                }
            }
            if (UseW && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.W.Range);
                var wAoe = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.W.Width,
                    (int) Program.W.Range);

                if (wAoe.HitNumber >= SliderW)
                {
                    Program.W.Cast(wAoe.CastPosition);
                }
            }
            if (UseW && Program.WReturn.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }

            if (UseE && Program.E.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.E.Range).OrderByDescending(t => t.Health).FirstOrDefault(Extension.IsMarked);

                if (minion != null)
                {
                    Program.E.Cast(minion);
                }
            }
        }

        public static void Post6JungleClear()
        {
            if (UseQ && Program.Q.IsReady())
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, Program.Q.Range)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (minion != null)
                {
                    Program.Q.Cast(minion);
                }
            }
            if (UseW && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.W.Range);
                var wAoe = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.W.Width,
                    (int) Program.W.Range);

                if (wAoe.HitNumber >= SliderW)
                {
                    Program.W.Cast(wAoe.CastPosition);
                }
            }

            if (UseW && Program.WReturn.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }

            if (UseWr && Program.WUltimate.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.WUltimate.Range);
                var wAoe = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.W.Width,
                    (int) Program.W.Range);

                if (wAoe.HitNumber >= SliderW)
                {
                    Program.WUltimate.Cast(wAoe.CastPosition);
                }
            }

            if (UseWr && Program.RReturn.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
            {
                Program.RReturn.Cast();
            }

            if (UseE && Program.E.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.E.Range).OrderByDescending(t => t.Health).FirstOrDefault(Extension.IsMarked);

                if (minion != null)
                {
                    Program.E.Cast(minion);
                }
            }

            if (UseEr && Program.EUltimate.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.EUltimate.Range).OrderByDescending(t => t.Health).FirstOrDefault(Extension.IsMarked);

                if (minion != null)
                {
                    Program.EUltimate.Cast(minion);
                }
            }
        }
    }
}