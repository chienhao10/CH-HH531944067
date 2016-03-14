using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

using Settings = Kitelyn.Config.Modes.Harass;

namespace Kitelyn.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Settings.Mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target != null && Q.GetPrediction(target).HitChance >= HitChance.High && DamageLibrary.GetSpellDamage(Player.Instance, target, SpellSlot.Q) > Player.Instance.GetAutoAttackDamage(target))
                {
                    if (target.HasBuff("caitlynyordletrapsight") || (
                        (target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Flee) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression))
                        ) && Q.GetPrediction(target).GetCollisionObjects<Obj_AI_Base>().Count() > 0 && Q.GetPrediction(target).GetCollisionObjects<Obj_AI_Base>()[0].NetworkId == target.NetworkId)
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }
                    else if (Q.GetPrediction(target).GetCollisionObjects<Obj_AI_Base>().Count() > 0 && Q.GetPrediction(target).GetCollisionObjects<Obj_AI_Base>()[0].NetworkId == target.NetworkId && Q.GetPrediction(target).HitChance >= HitChance.Medium)
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }
                    else if ((target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Flee) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression)
                        ) && Settings.UseQStunned && Player.Instance.ManaPercent > Settings.ManaQAlways)
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }
                    else if (Settings.UseQNotStunned && Player.Instance.ManaPercent > Settings.ManaQAlways)
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }
                    /**PRESEASON
                     * else if(target has Buff Snap Trap)
                     **/
                    //caitlynyordletrapsight
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Ammo > Settings.StockW)
                {
                    var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    if (target != null)
                        W.Cast(W.GetPrediction(target).CastPosition);
                }
            }
            if (Settings.UseR && R.IsReady())
            {
                var target = TargetSelector.GetTarget(SpellManager.RRange, DamageType.Physical);
                if (target != null && target.Distance(Player.Instance) > 700 && Player.Instance.CountEnemiesInRange(650) == 0)
                {
                    R.Cast(target);
                }
            }
        }
    }
}
