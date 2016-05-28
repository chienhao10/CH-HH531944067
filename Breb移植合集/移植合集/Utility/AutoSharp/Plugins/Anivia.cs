//Get Some Part From xSaliceReligionAIO Credit xSalice

using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Plugins
{
    public class Anivia : PluginBase
    {
        //R
        public Anivia()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 950);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 650);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 625);

            Q.SetSkillshot(.5f, 110f, 750f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(.25f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            SmartKs();
            if (ComboMode)
            {
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range && ShouldE(Target))
                {
                    E.Cast(Target);
                }

                //Q
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range && ShouldQ())
                {
                    Q.CastIfHitchanceEquals(Target, HitChance.Medium);
                }

                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    CastW(Target);
                }

                if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range && R.GetPrediction(Target).Hitchance >= HitChance.High)
                {
                    R.Cast(Target);
                }
            }
        }

        private void SmartKs()
        {
            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(x => x.LSIsValidTarget(1300)))
            {
                //ER
                if (Player.LSDistance(target.ServerPosition) <= R.Range && R.Instance.ToggleState == 1 &&
                    (Player.LSGetSpellDamage(target, SpellSlot.R) + Player.LSGetSpellDamage(target, SpellSlot.E) * 2) >
                    target.Health + 50)
                {
                    if (R.IsReady() && E.IsReady())
                    {
                        E.Cast(target);
                        R.Cast(target);
                        return;
                    }
                }

                //QR
                if (Player.LSDistance(target.ServerPosition) <= R.Range && ShouldQ() &&
                    (Player.LSGetSpellDamage(target, SpellSlot.Q) + Player.LSGetSpellDamage(target, SpellSlot.R)) >
                    target.Health + 30)
                {
                    if (W.IsReady() && R.IsReady())
                    {
                        W.Cast(target);
                        return;
                    }
                }

                //Q
                if (Player.LSDistance(target.ServerPosition) <= Q.Range && ShouldQ() &&
                    (Player.LSGetSpellDamage(target, SpellSlot.Q)) > target.Health + 30)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                //E
                if (Player.LSDistance(target.ServerPosition) <= E.Range &&
                    (Player.LSGetSpellDamage(target, SpellSlot.E)) > target.Health + 30)
                {
                    if (E.IsReady())
                    {
                        E.Cast(target);
                        return;
                    }
                }
            }
        }

        private void CastW(AIHeroClient target)
        {
            var pred = W.GetPrediction(target);
            var vec = new Vector3(
                pred.CastPosition.X - Player.ServerPosition.X, 0, pred.CastPosition.Z - Player.ServerPosition.Z);
            var castBehind = pred.CastPosition + Vector3.Normalize(vec) * 125;

            if (W.IsReady())
            {
                W.Cast(castBehind);
            }
        }

        private bool ShouldE(AIHeroClient target)
        {
            if (checkChilled(target))
            {
                return true;
            }

            if (Player.LSGetSpellDamage(target, SpellSlot.E) > target.Health)
            {
                return true;
            }

            if (R.IsReady() && Player.LSDistance(target) <= R.Range - 25 && Player.LSDistance(target.ServerPosition) > 250)
            {
                return true;
            }

            return false;
        }

        private bool checkChilled(AIHeroClient target)
        {
            return target.HasBuff("Chilled");
        }

        private bool ShouldQ()
        {
            if (Environment.TickCount - Q.LastCastAttemptT > 2000)
            {
                return true;
            }

            return false;
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