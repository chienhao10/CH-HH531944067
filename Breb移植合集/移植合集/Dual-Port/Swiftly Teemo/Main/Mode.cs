#region

using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using EloBuddy.SDK;

#endregion

namespace Swiftly_Teemo.Main
{
    internal class Mode : Core
    {
        public static void Combo()
        {
            if (Target == null || Target.IsZombie || Target.IsInvulnerable) return;

            var rPrediction = Spells.R.GetPrediction(Target).CastPosition;
            var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;

            if (Spells.R.IsReady())
            {
                if (!Target.HasBuffOfType(BuffType.Poison) && !Target.HasBuffOfType(BuffType.Slow) && !Target.HasBuffOfType(BuffType.NearSight))
                {
                    if (!MenuConfig.RCombo)
                    {
                        if (Target.LSIsValidTarget(Spells.R.Range))
                        {
                            if (Target.Distance(Player) <= Spells.R.Range)
                            {
                                Spells.R.Cast(rPrediction);
                            }
                        }

                    }
                    if (MenuConfig.RCombo)
                    {
                        if (Target.LSIsValidTarget(Spells.R.Range * 2))
                        {
                            if (Target.Distance(Player) <= Spells.R.Range * 2)
                            {
                                Spells.R.Cast(rPrediction);
                            }
                        }
                    }
                }
            }

            if (Spells.W.IsReady() && Player.ManaPercent >= 22.5)
            {
                Spells.W.Cast();
            }
        }
        public static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                if (sender.Owner == Player)
                {
                    Orbwalker.DisableAttacking = true;
                    DelayAction.Add(200, () => Orbwalker.DisableAttacking = false);
                }
            }
        }
        public static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(Spells.Q.Range)).ToList();
            var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            var rPred = Spells.R.GetCircularFarmLocation(minions);

            foreach (var m in minions)
            {
                if (m.Health < Spells.Q.GetDamage(m) && Player.ManaPercent >= 20 && MenuConfig.LaneQ)
                {
                    Spells.Q.Cast(m);
                }

                if (!(m.Health < Spells.R.GetDamage(m)) || !(Player.ManaPercent >= 40) || ammo < 3) continue;

                if (rPred.MinionsHit >= 3)
                {
                    Spells.R.Cast(rPred.Position);
                }
            }
        }
        public static void Jungle()
        {
            var mob = GameObjects.Jungle.Where(m => m.LSIsValidTarget(Spells.Q.Range) && !GameObjects.JungleSmall.Contains(m)).ToList();
            var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;

            foreach (var m in mob)
            {
                if (!Spells.R.IsReady() || !(m.Distance(Player) <= Spells.R.Range) || !(m.Health > Spells.R.GetDamage(m))) continue;
                if (m.BaseSkinName.Contains("Sru_Crab")) continue;

                if (ammo >= 3)
                {
                    Spells.R.Cast(m);
                }
            }
        }

        public static void Flee()
        {
            if (!MenuConfig.Flee)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Spells.W.IsReady())
            {
                Spells.W.Cast();
            }

            if (!(Target.Distance(Player) <= Spells.R.Range) || !Target.LSIsValidTarget() || Target == null) return;

            if (Spells.R.IsReady())
            {
                Spells.R.Cast(Player.Position);
            }
        }
    }
}
