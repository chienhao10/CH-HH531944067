using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Ezreal : PluginBase
    {
        public Ezreal()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 850);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);

            R = new LeagueSharp.Common.Spell(SpellSlot.R, 2500);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            KS();
            if (ComboMode)
            {
                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast(Target);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.CastIfHitchanceEquals(Target, HitChance.Medium);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public void KS()
        {
            foreach (
                var target in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            x =>
                                Player.LSDistance(x) < 2000 && Player.LSDistance(x) > 400 && x.LSIsValidTarget() && x.IsEnemy &&
                                !x.IsDead))
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (target != null)
                {
                    //R
                    if (Player.LSDistance(target.ServerPosition) <= R.Range &&
                        (Player.LSGetSpellDamage(target, SpellSlot.R)) > target.Health + 50)
                    {
                        if (R.CastCheck(Target, "ComboRKS"))
                        {
                            R.Cast(target);
                            return;
                        }
                    }
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboRKS", "Use R KS", true);
        }
    }
}