using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Darius : PluginBase
    {
        public Darius()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 425);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 145);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 550);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 460);
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null)
            {
                if (W.IsReady())
                {
                    W.Cast();
                    Orbwalker.ResetAutoAttack();
                }
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            ExecuteKillsteal();
            if (ComboMode)
            {
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast();
                }
                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast();
                }
            }
        }

        public void ExecuteKillsteal()
        {
            foreach (
                var target in
                    ObjectManager.Get<AIHeroClient>().Where(x => Player.LSDistance(x) < R.Range && x.IsEnemy && !x.IsDead))
            {
                if (R.IsReady() && Player.LSDistance(target) <= R.Range && R.IsKillable(target))
                {
                    CastR(target);
                }
            }
        }

        // R Calculate Credit TC-Crew
        public void CastR(Obj_AI_Base target)
        {
            if (!target.LSIsValidTarget(R.Range) || !R.IsReady())
            {
                return;
            }

            if (!(ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q, 1) > target.Health))
            {
                foreach (var buff in target.Buffs)
                {
                    if (buff.Name == "dariushemo")
                    {
                        if (ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R, 1) * (1 + buff.Count / 5) - 1 >
                            (target.Health))
                        {
                            R.Cast(target, true);
                        }
                    }
                }
            }
            else if (ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R, 1) - 15 > (target.Health))
            {
                R.Cast(target, true);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
        }
    }
}