using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Draven : PluginBase
    {
        private bool _blockR2;

        public Draven()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1100);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 20000);
            E.SetSkillshot(250f, 130f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(400f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (E.IsReady() && unit.LSIsValidTarget(E.Range))
            {
                E.Cast(unit);
            }
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null)
            {
                W.Cast();
                Q.Cast();
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (R.Instance.Cooldown > 0)
            {
                _blockR2 = false;
            }
            KS();

            if (ComboMode)
            {
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
                if (R.IsReady() && R.IsKillable(Target))
                {
                    R.Cast(Target);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public void KS()
        {
            foreach (
                var target in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => Player.LSDistance(x) < 2000 && x.LSIsValidTarget() && x.IsEnemy && !x.IsDead))
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (target != null)
                {
                    //R
                    if (Player.LSDistance(target.ServerPosition) <= R.Range &&
                        (Player.LSGetSpellDamage(target, SpellSlot.R)) > target.Health + 100)
                    {
                        if (R.IsReady() && !_blockR2)
                        {
                            R.Cast(target);
                            _blockR2 = true;
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
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}