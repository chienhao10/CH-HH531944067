using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.UI;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using System;
using System.Linq;
using PrideStalker_Rengar.Main;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace PrideStalker_Rengar.Handlers
{
    class Mode : Core
    {
        private static int _lastTick;
        public static bool hasPassive => Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarpassivebuff"));
        #region Combo
        public static void Combo()
        {   
            var Target = TargetSelector.GetTarget(1000, DamageType.Physical);
            

            if (Target != null && Target.LSIsValidTarget() && !Target.IsZombie)
            {
                if(Player.Mana == 5)
                {
                    if(MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && hasPassive)
                    {
                        ITEM.CastYomu();
                    }
                    if(Spells.E.IsReady() && Target.Distance(Player) <= Spells.E.Range)
                    {
                       Spells.E.Cast(Target);
                    }
                    if (Spells.Q.IsReady() && Target.Distance(Player) <= Player.AttackRange && !Spells.E.IsReady())
                    {
                        Spells.Q.Cast(Target);
                    }
                }
                if(Player.Mana < 5)
                {
                    if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && hasPassive)
                    {
                        ITEM.CastYomu();
                    }
                    if (Spells.E.IsReady() && !hasPassive && Target.Distance(Player) <= Spells.E.Range)
                    {
                        Spells.E.Cast(Target);
                    }
                    if(Target.Distance(Player) <= Spells.W.Range)
                    {
                        if (Spells.Q.IsReady())
                        {
                            Spells.Q.Cast(Target);
                        }
                        if (MenuConfig.UseItem)
                        {
                            ITEM.CastHydra();
                        }
                        if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(Target);
                        }
                    }
                }
            }
        }
        #endregion
        #region ApCombo
        public static void ApCombo()
        {
            var Target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Magical);

            if (Target != null && Target.LSIsValidTarget() && !Target.IsZombie)
            {
                if (Player.Mana == 5)
                {
                    if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && hasPassive)
                    {
                        ITEM.CastYomu();
                    }
                    if(Target.Distance(Player) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem && Spells.W.IsReady())
                        {
                            ITEM.CastHydra();
                        }
                        if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(Target);
                        }
                    }
                }
                if (Player.Mana < 5)
                {
                     if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && hasPassive)
                    {
                        ITEM.CastYomu();
                    }
                     if(MenuConfig.UseItem && !hasPassive)
                    {
                        ITEM.CastProtobelt();
                    }
                    if(Target.Distance(Player) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem && Spells.W.IsReady())
                        {
                            ITEM.CastHydra();
                        }
                        if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(Target);
                            Spells.W.Cast(Target);
                        }
                        else if (Spells.Q.IsReady())
                        {
                            Spells.Q.Cast(Target);
                        }
                        else if (Spells.E.IsReady() && Target.Distance(Player) <= Spells.W.Range + 225)
                        {
                            if (MenuConfig.IgnoreE && hasPassive)
                            {
                                Spells.E.Cast(Game.CursorPos);
                            }
                            else
                            {
                                Spells.E.Cast(Target);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region TripleQ
        public static void TripleQ()
        {
            var Target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);

            if (Target != null && Target.LSIsValidTarget() && !Target.IsZombie)
            {
                if (Player.Mana == 5)
                {
                    if (MenuConfig.UseItem && Spells.Q.IsReady() && hasPassive)
                    {
                        ITEM.CastYomu();
                    }
                    
                    if (Spells.Q.IsReady() && Target.Distance(Player) <= Spells.W.Range)
                    {
                        if(!MenuConfig.TripleQAAReset)
                        {
                            Spells.Q.Cast();
                        }
                    }
                }
                if (Player.Mana < 5)
                {
                    if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady())
                    {
                        ITEM.CastYomu();
                    }
                    if (Spells.Q.IsReady() && Target.Distance(Player) <= Spells.W.Range)
                    {
                        if (!MenuConfig.TripleQAAReset)
                        {
                            Spells.Q.Cast();
                        }
                    }
                    if (Spells.E.IsReady() && !Spells.Q.IsReady() && Target.Distance(Player) <= Spells.W.Range)
                    {
                        if (MenuConfig.IgnoreE && hasPassive)
                        {
                            Spells.E.Cast(Game.CursorPos);
                        }
                        else
                        {
                            Spells.E.Cast(Target);
                        }
                    }
                   if (Spells.W.IsReady() && !Spells.Q.IsReady() && Player.Distance(Target) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEM.CastHydra();
                        }
                        Spells.W.Cast(Target);
                    }
                }
            }
        }
        #endregion
        #region OneShot
        public static void OneShot()
        {
            var Target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Spells.W.Range)).ToList();

            if (Target != null && Target.LSIsValidTarget() && !Target.IsZombie)
            {
                if (Player.Mana == 5)
                {
                    if (MenuConfig.UseItem && Spells.Q.IsReady() && hasPassive)
                    {
                        ITEM.CastYomu();
                    }
                    if (Spells.Q.IsReady() && Target.Distance(Player) <= Spells.E.Range)
                    {
                        Spells.Q.Cast();   
                    }
                }
                if (Player.Mana < 5)
                {
                    if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady())
                    {
                        ITEM.CastYomu();
                    }
                    if (Spells.E.IsReady() && Target.Distance(Player) <= Spells.W.Range + 225)
                    {
                        if (MenuConfig.IgnoreE && hasPassive)
                        {
                            Spells.E.Cast(Game.CursorPos);
                        }
                        else
                        {
                            Spells.E.Cast(Target);
                        }
                    }
                    if (Spells.Q.IsReady() && Target.Distance(Player) <= Spells.W.Range)
                    {
                        Spells.Q.Cast();
                    }
                    if (Spells.W.IsReady() && Player.Distance(Target) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEM.CastHydra();
                        }
                        Spells.W.Cast(Target);
                    }
                }
            }
         }
        
        #endregion

        #region Lane
        public static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Spells.W.Range)).ToList();

            if (minions == null || Player.Mana == 5 && MenuConfig.Passive)
            {
                return;
            }

            foreach(var m in minions)
            {
                if (Player.Mana == 5)
                {
                    if (Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode") == 2)
                    {
                        if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                        {
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                    else
                    {
                        if (Spells.Q.IsReady() && m.Distance(Player) <= Player.AttackRange)
                        {
                            if (MenuConfig.UseItem)
                            {
                                ITEM.CastHydra();
                            }
                            Spells.Q.Cast(m);
                        }
                    }
                }
                if (Player.Mana < 5)
                {
                    if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(m);
                    }
                    if (Spells.E.IsReady() && !hasPassive)
                    {
                        Spells.E.Cast(m);
                    }
                    if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEM.CastHydra();
                        }
                        Spells.W.Cast(m);
                    }
                }
            }
              
        }
        #endregion
        #region Jungle
        public static void Jungle()
        {
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.IsValidTarget(Spells.W.Range)).ToList();

            if (mob == null || Player.Mana == 5 && MenuConfig.Passive)
            {
                return;
            }

            foreach (var m in mob)
            {
                if (Player.Mana == 5)
                {
                    if(Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode") == 2)
                    {
                        if(Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                        {
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                    else
                    {
                        if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range && Player.HealthPercent < 80)
                        {
                            if (MenuConfig.UseItem)
                            {
                                ITEM.CastHydra();
                            }
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                }
                if (Player.Mana < 5)
                {
                   
                    if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEM.CastHydra();
                        }
                        Spells.W.Cast(m.ServerPosition);
                    }
                   if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(m.ServerPosition);
                    }
                }
            }
        }
        #endregion
        #region LastHit
        public static void LastHit()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Player.AttackRange)).ToList();
            

            if (minions == null || Player.Mana == 5 && MenuConfig.Passive)
            {
                return;
            }
            if(MenuConfig.StackLastHit)
            {
                foreach (var m in minions)
                {
                    if (m.Health < Spells.Q.GetDamage(m) + (float)Player.GetAutoAttackDamage(m))
                    {
                        Spells.Q.Cast();
                    }
                }
            }
        }
        #endregion

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        #region ComboMode


        public static void ChangeComboMode()
        {
            var changetime = Environment.TickCount - _lastTick;


            if (MenuConfig.ChangeComboMode)
            {
                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 0 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 1;
                }

                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 1 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 2;
                }
                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 2 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 3;
                }
                if (getBoxItem(MenuConfig.comboMenu, "ComboMode") == 3 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = 0;
                }
            }

        }

        #endregion
    }
}
