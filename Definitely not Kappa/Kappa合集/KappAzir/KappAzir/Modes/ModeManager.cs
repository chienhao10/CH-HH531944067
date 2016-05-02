namespace KappAzir.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;

    using Mario_s_Lib;
    using static Menus;

    internal class ModeManager
    {
        public static List<AIHeroClient> Enemies = EntityManager.Heroes.Enemies;

        public static Obj_AI_Turret tower = EntityManager.Turrets.Allies.FirstOrDefault(t => t.IsValidTarget(1250));

        public static AIHeroClient ally =
            EntityManager.Heroes.Allies.OrderByDescending(a => a.CountEnemiesInRange(SpellsManager.R.Range)).FirstOrDefault();

        public static AIHeroClient Azir = Player.Instance;

        internal static bool Ehit(AIHeroClient target)
        {
            return
                Orbwalker.AzirSoldiers.Select(soldier => new Geometry.Polygon.Rectangle(Azir.Position, soldier.Position, target.BoundingRadius + 35))
                    .Any(rectangle => rectangle.IsInside(target));
        }

        internal static void SoldierAttack(Obj_AI_Base target)
        {
            if (Orbwalker.AzirSoldiers.Any(x => x.IsInRange(target, Orbwalker.AzirSoldierAutoAttackRange - 5) && x.IsAlly))
            {
                if (Azir.Distance(target.Position) < 850)
                {
                    Core.DelayAction((() => Player.IssueOrder(GameObjectOrder.AttackUnit, target)), 250);
                }
            }
        }

        public static int ManaCheck(Obj_AI_Base target)
        {
            var mana = (float)0;

            if (Azir.Spellbook.GetSpell(SpellSlot.Q).IsReady)
            {
                // Q mana
                mana += SpellsManager.Q.Handle.SData.Mana;
            }

            if (Azir.Spellbook.GetSpell(SpellSlot.W).IsReady && Orbwalker.AzirSoldiers.Count == 0)
            {
                // W mana
                mana += SpellsManager.W.Handle.SData.Mana;
            }

            if (Azir.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                // E mana
                mana += SpellsManager.E.Handle.SData.Mana;
            }
            if (Azir.Spellbook.GetSpell(SpellSlot.R).IsReady)
            {
                // E mana
                mana += SpellsManager.R.Handle.SData.Mana;
            }

            return (int)mana;
        }

        /// <summary>
        /// Create the event on tick
        /// </summary>
        public static HitChance hitchance;

        public static DangerLevel Intdanger;

        public static void InitializeModes()
        {
            switch (SpellsMenu.GetComboBoxValue("chance"))
            {
                case 0:
                    {
                        hitchance = HitChance.High;
                    }
                    break;

                case 1:
                    {
                        hitchance = HitChance.Medium;
                    }
                    break;

                case 2:
                    {
                        hitchance = HitChance.Low;
                    }
                    break;
            }

            switch (SpellsMenu.GetComboBoxValue("Intdanger"))
            {
                case 0:
                    {
                        Intdanger = DangerLevel.High;
                    }
                    break;

                case 1:
                    {
                        Intdanger = DangerLevel.Medium;
                    }
                    break;

                case 2:
                    {
                        Intdanger = DangerLevel.Low;
                    }
                    break;
            }
            Game.OnUpdate += Game_OnTick;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !SpellsMenu.GetCheckBoxValue("rUseInt"))
            {
                return;
            }
            if (sender.IsValidTarget(SpellsManager.R.Range) && e.DangerLevel >= Intdanger)
            {
                SpellsManager.R.Cast(sender.ServerPosition);
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !SpellsMenu.GetCheckBoxValue("rUseGap"))
            {
                return;
            }
            if (sender.IsValidTarget(SpellsManager.R.Range))
            {
                Core.DelayAction(() => SpellsManager.R.Cast(sender.ServerPosition), (int)(sender.Spellbook.CastEndTime - Game.Time));
            }

            if (Azir.IsInRange(e.End, SpellsManager.R.Range) && e.End.IsInRange(Azir.ServerPosition, SpellsManager.R.Range))
            {
                Core.DelayAction(
                    () => SpellsManager.R.Cast(Azir.ServerPosition.Extend(sender.ServerPosition, SpellsManager.R.Range).To3D()),
                    (int)(sender.Spellbook.CastEndTime - Game.Time));
            }
        }

        /// <summary>
        /// This event is triggered every tick of the game
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnTick(EventArgs args)
        {
            var orbMode = Orbwalker.ActiveModesFlags;
            var playerMana = Azir.ManaPercent;

            Active.Execute();

            if (orbMode.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.Execute();
            }

            if (orbMode.HasFlag(Orbwalker.ActiveModes.Harass) && playerMana > HarassMenu.GetSliderValue("manaSlider"))
            {
                Harass.Execute();
            }

            if (orbMode.HasFlag(Orbwalker.ActiveModes.LastHit) && playerMana > LasthitMenu.GetSliderValue("manaSlider"))
            {
                LastHit.Execute();
            }

            if (orbMode.HasFlag(Orbwalker.ActiveModes.LaneClear) && playerMana > LaneClearMenu.GetSliderValue("manaSlider"))
            {
                LaneClear.Execute();
            }

            if (orbMode.HasFlag(Orbwalker.ActiveModes.JungleClear) && playerMana > JungleClearMenu.GetSliderValue("manaSlider"))
            {
                JungleClear.Execute();
            }

            if (AutoHarassMenu.GetKeyBindValue("autoHarassKey"))
            {
                if (Orbwalker.AzirSoldiers.Count >= 1 && AutoHarassMenu.GetCheckBoxValue("attack"))
                {
                    var target = TargetSelector.GetTarget(1250, DamageType.Magical);
                    if (target == null)
                    {
                        return;
                    }
                    SoldierAttack(target);
                }
                if (playerMana > AutoHarassMenu.GetSliderValue("manaSlider"))
                {
                    AutoHarass.Execute();
                }
            }
            if (FleeMenu.GetKeyBindValue("insect"))
            {
                InSec.Normal();
            }

            if (FleeMenu.GetKeyBindValue("insected"))
            {
                InSec.New();
            }

            if (FleeMenu.GetKeyBindValue("flee"))
            {
                Flee.Execute();
            }
            KillSteal.Execute();
        }
    }
}