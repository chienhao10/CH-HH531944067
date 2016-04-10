using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace Ex1L_Riven.Base
{
    internal class Logic
    {
        public static AIHeroClient Qtarget;

        public static void CastQaa(AIHeroClient target)
        {
            if (!target.IsValidTarget()) return;

            Qtarget = target;
            if (Player.Instance.Distance(target) > 270)
            {
                Riven.PerformQaa = true;
            }
        }

        public static void CastQaa2(AIHeroClient target)
        {
            if (!target.IsValidTarget()) return;

            if (Player.Instance.Distance(target) < 270)
            {
                Riven.PerformQaa = true;
            }
            else
            {
                CastQaa(target);
            }
        }

        public static void CastQ(AIHeroClient target)
        {
            if (Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(target.ServerPosition);
            }
        }

        public static void CastW(AIHeroClient target)
        {
            if (Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast();
            }
        }

        public static void CastE(AIHeroClient target)
        {
            if (Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range + Player.Instance.BoundingRadius))
            {
                Spells.E.Cast(target.ServerPosition);
            }
        }

        public static void CastR(AIHeroClient target)
        {
            if (Spells.R1.IsReady() && Variables.UseRCombo && Riven.R1Activated == false && target.IsValidTarget(500))
            {
                Spells.R1.Cast();
            }
        }

        public static void CastTiaHydra(AIHeroClient target)
        {
            if (target.IsValidTarget(400))
            {
                if (Items.Tiamat.IsReady())
                {
                    Items.Tiamat.Cast();
                }
                if (Items.Hydra.IsReady())
                {
                    Items.Hydra.Cast();
                }
            }
        }

        public static void CastQss()
        {
            if (Player.Instance.CountEnemiesInRange(1000) > 0)
            {
                if (Items.Qss.IsReady())
                {
                    Items.Qss.Cast();
                }
                if (Items.Mercurial.IsReady())
                {
                    Items.Mercurial.Cast();
                }
            }
        }

        public static void DoEmote()
        {
            switch (Variables.SelectedEmote)
            {
                case "Laugh":
                    Player.DoEmote(Emote.Laugh);
                    break;
                case "Dance":
                    Player.DoEmote(Emote.Dance);
                    break;
                case "Joke":
                    Player.DoEmote(Emote.Joke);
                    break;
                case "Taunt":
                    Player.DoEmote(Emote.Taunt);
                    break;
            }
        }

        public static void ResetAa()
        {
            Orbwalker.ResetAutoAttack();
        }

        /// <summary>
        ///     This function will list all the closest Jungle monsters
        /// </summary>
        /// <param name="range">Limit the list to only range value</param>
        /// <param name="from">Limit the list according to the Vector3 coordinations</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> Monsters(float range, Vector3 from = default(Vector3))
        {
            return EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(x => x.IsInRange(from, range)).ToList();
        }

        public static void IniInitialize()
        {
        }
    }
}