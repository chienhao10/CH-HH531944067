using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Plugins
{
    public class Varus : PluginBase
    {
        public Varus()
        {

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1600f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 925f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1200f);

            Q.SetSkillshot(.25f, 70f, 1650f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(.25f, 100f, 1950f, false, SkillshotType.SkillshotLine);
        }
        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                CastQEnemy(Target);
                if (E.IsReady() && Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
                if (R.IsReady() && Target.LSCountEnemiesInRange(1000) >=2)
                {
                        Vector3 searchPos;

                        if (ObjectManager.Player.LSDistance(Game.CursorPos) < R.Range - 300f)
                            searchPos = Game.CursorPos;
                        else
                            searchPos = ObjectManager.Player.Position +
                                        Vector3.Normalize(Game.CursorPos - ObjectManager.Player.Position) * (R.Range - 300f);

                        var rTarget =
                            ObjectManager.Get<AIHeroClient>()
                                .Where(hero => hero.LSIsValidTarget(R.Range) && hero.LSDistance(searchPos) < 300f)
                                .OrderByDescending(TargetSelector.GetPriority)
                                .First();

                        if (rTarget != null && R.IsReady())
                            R.Cast(rTarget);
                }

            }
        }

        private void CastQEnemy(AIHeroClient vTarget)
        {
            if (vTarget == null)
                return;
            if (!Q.IsReady())
                return;

            Random rnd = new Random();
            var randQ = rnd.Next(1000, 1400); //Rand minQRange

            if (Q.IsCharging)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (Q.Range >= randQ)
                {
                    Q.Cast(vTarget, false, true);
                }
            }
            else
            {
                Q.StartCharging();
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

    }
}
