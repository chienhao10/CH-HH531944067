using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy;
using Auto_Carry_Vayne.Logic;


namespace Auto_Carry_Vayne.Features.Modes
{
    class LaneClear
    {
        #region FarmQ
        public static void SpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                 Manager.MenuManager.UseQLC && Variables._Player.ManaPercent >= Manager.MenuManager.UseQLCMana) &&
                Manager.SpellManager.Q.IsReady())
            {
                if (Orbwalker.CanAutoAttack)
                {
                    return;
                }
                foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Variables._Player.ServerPosition, Variables._Player.GetAutoAttackRange()))
                {
                    if (minion == null) return;
                    var dmg = Variables._Player.GetSpellDamage(minion, SpellSlot.Q) +
                              Variables._Player.GetAutoAttackDamage(minion);
                    if (Prediction.Health.GetPrediction(minion, (int)(Variables._Player.AttackDelay * 1000)) <= dmg / 2 &&
                        (Orbwalker.LastTarget == null || Orbwalker.LastTarget.NetworkId != minion.NetworkId))
                    {
                        var Farmtumblepos = Variables._Player.Position.Extend(minion, 300f);
                        if (Farmtumblepos.IsSafeEx())
                        {
                            Player.CastSpell(SpellSlot.Q, Farmtumblepos.To3D());
                        }
                    }
                }
            }
            var LastHitE = Variables._Player;

            foreach (
                var Etarget in
                    EntityManager.Heroes.Enemies.Where(
                        Etarget => Etarget.IsValidTarget(Manager.SpellManager.E.Range) && Etarget.Path.Count() < 2))
            {
                if (Manager.MenuManager.UseEKill && Manager.SpellManager.E.IsReady() &&
                    Variables._Player.CountEnemiesInRange(600) <= 1)
                {
                    var dmgE = Variables._Player.GetSpellDamage(Etarget, SpellSlot.E);
                    if (dmgE > Etarget.Health ||
                        (Etarget.GetBuffCount("vaynesilvereddebuff") == 2 && dmgE + Manager.DamageManager.Wdmg(Etarget) > Etarget.Health))
                    {
                        LastHitE = Etarget;

                    }
                }

                if (LastHitE != Variables._Player)
                {
                    Manager.SpellManager.E.Cast(LastHitE);
                }
            }
        }
        #endregion FarmQ
    }
}
