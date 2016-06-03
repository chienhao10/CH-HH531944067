#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using Spirit_Karma.Core;
using Spirit_Karma.Menus;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

namespace Spirit_Karma.Event
{
    internal class Mode : Core.Core
    {
        private static int _lastTick;

        public static void OnUpdate(EventArgs args)
        {
            ChangeMantra();
            OrbHandler();
        }

        private static void ChangeMantra()
        {
            var changetime = Environment.TickCount - _lastTick;


            if (MenuConfig.getKeyBindItem(MenuConfig.comboMenu, "Mantra"))
            {
                if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 0 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["MantraMode"].Cast<ComboBox>().CurrentValue = 1;
                }

                if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 1 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["MantraMode"].Cast<ComboBox>().CurrentValue = 2;
                }
                if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 2 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["MantraMode"].Cast<ComboBox>().CurrentValue = 3;
                }
                if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 3 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    MenuConfig.comboMenu["MantraMode"].Cast<ComboBox>().CurrentValue = 0;
                }

            }
        }
        private static void OrbHandler()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mixed();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Lane();
                Jungle();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
        }

        private static void Combo()
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().OrderBy(hp => hp.Health))
            {
                if (!enemy.LSIsValidTarget(Spells.Q.Range) || enemy.IsDead)
                    return;

                Usables.Locket();
                //    Usables.Seraph();
                switch (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode"))
                {
                    case 0:
                        if (Spells.R.IsReady() && Spells.Q.IsReady())
                        {
                            Spells.R.Cast();
                            Spells.Q.Cast(enemy);
                        }
                        else if (Spells.Q.IsReady())
                        {
                            Usables.FrostQueen();
                            Usables.ProtoBelt();
                            Spells.Q.Cast(enemy);
                        }
                        else if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(enemy);
                        }
                        else if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(Player);
                        }
                        break;
                    case 1:
                        if (Spells.R.IsReady() && Spells.W.IsReady())
                        {
                            Spells.R.Cast();
                        }
                        else if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(enemy);
                        }
                        else if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(Player);
                        }
                        else if (Spells.Q.IsReady())
                        {
                            Usables.FrostQueen();
                            Usables.ProtoBelt();
                            Spells.Q.Cast(enemy);
                        }
                        break;
                    case 2:
                        if (Spells.R.IsReady() && Spells.E.IsReady())
                        {
                            Spells.R.Cast();

                        }
                        else if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(Player);
                        }
                        else if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(enemy);
                        }
                        else if (Spells.Q.IsReady())
                        {
                            Usables.FrostQueen();
                            Usables.ProtoBelt();
                            Spells.Q.Cast(enemy);
                        }
                        break;
                    // Auto
                    case 3:
                        if (Player.HealthPercent <= 30 && enemy.HealthPercent >= 50)
                        {
                            goto case 2;
                        }
                        if (!enemy.LSIsFacing(Player))
                        {
                            goto case 1;
                        }
                        goto case 0;
                }
            }
        }

        private static void Mixed()
        {
            if (MenuConfig.getCheckBoxItem(MenuConfig.harassMenu, "HarassR") && Spells.R.IsReady())
            {
                if (Spells.Q.IsReady() || Spells.E.IsReady())
                {
                    Spells.R.Cast();
                }
            }
            if (MenuConfig.getCheckBoxItem(MenuConfig.harassMenu, "HarassQ1") && Spells.Q.IsReady())
            {
                if (!(Player.ManaPercent >= MenuConfig.getSliderItem(MenuConfig.harassMenu, "HarassQ"))) return;
                {
                    Spells.Q.Cast(Target);
                }
            }
            if (MenuConfig.getCheckBoxItem(MenuConfig.harassMenu, "HarassW1") && Spells.W.IsReady())
            {
                if (!(Player.ManaPercent >= MenuConfig.getSliderItem(MenuConfig.harassMenu, "HarassW"))) return;
                {
                    Spells.W.Cast(Target);
                }
            }
            if (MenuConfig.getCheckBoxItem(MenuConfig.harassMenu, "HarassE1") && Spells.E.IsReady())
            {
                if (!(Player.ManaPercent >= MenuConfig.getSliderItem(MenuConfig.harassMenu, "HarassE"))) return;
                {
                    Spells.E.Cast(Player);
                }
            }
        }

        private static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(1050));

            foreach (var m in minions)
            {
                if (MenuConfig.getCheckBoxItem(MenuConfig.laneclearMenu, "LaneR") && Spells.R.IsReady())
                {
                    if (Spells.Q.IsReady() && Player.ManaPercent >= MenuConfig.getSliderItem(MenuConfig.laneclearMenu, "LaneQ"))
                    {
                        Spells.R.Cast();
                    }
                }
                if (MenuConfig.getCheckBoxItem(MenuConfig.laneclearMenu, "LaneQ1") && Spells.Q.IsReady() && m.Health > Player.GetAutoAttackDamage(m))
                {
                    if (!(Player.ManaPercent >= MenuConfig.getSliderItem(MenuConfig.laneclearMenu, "LaneQ"))) return;
                    {
                        Spells.Q.Cast(m.ServerPosition);
                    }
                }
                if (MenuConfig.getCheckBoxItem(MenuConfig.laneclearMenu, "LaneE1") && Player.ManaPercent >= MenuConfig.getSliderItem(MenuConfig.laneclearMenu, "LaneE") && Spells.E.IsReady())
                {
                    if (!(Player.ManaPercent >= MenuConfig.getSliderItem(MenuConfig.laneclearMenu, "LaneE"))) return;
                    {
                        Spells.E.Cast(Player);
                    }
                }
            }
        }

        private static void Jungle()
        {
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget(Spells.Q.Range));
            foreach (var m in mob)
            {
                if (Spells.R.IsReady())
                {
                    if (Spells.Q.IsReady())
                    {
                        Spells.R.Cast();
                        Spells.Q.Cast(m.ServerPosition);
                    }
                    else if (Spells.E.IsReady() && Player.HealthPercent <= 80)
                    {
                        Spells.R.Cast();
                        Spells.E.Cast(Player);
                    }
                }
               else if (Spells.Q.IsReady())
                {
                    Spells.Q.Cast(m.ServerPosition);
                }
               else if (Spells.E.IsReady())
               {
                   Spells.E.Cast(Player);
               }
                else if (Spells.W.IsReady() && Player.ManaPercent >= 35)
                {
                    Spells.W.Cast(m);
                }
            }
        }

        private static void LastHit()
        {
            
        }

        private static void Flee()
        {
           EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                return;
            }

            if (Spells.R.IsReady() && Spells.E.IsReady())
            {
                Spells.R.Cast();
                Spells.E.Cast(Player);
            }
           else if (Spells.E.IsReady())
            {
                Spells.E.Cast(Player);
            }
        }
    }
}
