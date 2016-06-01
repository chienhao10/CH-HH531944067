using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using hVayne.Extensions;
using LeagueSharp.SDK.Core.Utils;
using Color = System.Drawing.Color;
using EloBuddy.SDK;
using EloBuddy.SDK.Notifications;

namespace hVayne.Champions
{
    public class Vayne
    {
        public Vayne()
        {
            VayneOnLoad();
        }


        private static void VayneOnLoad()
        {
            EloBuddy.SDK.Notifications.Notifications.Show(new SimpleNotification("hVayne - (click and read)", "Vayne is well syncronized with scripting mechanisms. I developed this assembly to increase your ingame performance with Vayne. With this assembly taking a control of your game is inevitable. Take a step in enjoy the smooth work."));
            
            Spells.ExecuteSpells();
            Config.ExecuteMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPostAttack += OnAction;
        }

        private static void OnAction(AttackableUnit dgfg, EventArgs args)
        {
            if (dgfg is AIHeroClient)
            {
                var Target = dgfg as AIHeroClient;

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.getCheckBoxItem(Config.comboMenu, "combo.q") && Spells.Q.IsReady()
                    && Target.LSIsValidTarget(777) && Config.ComboMethod == 0)
                {
                    SpellManager.ExecuteQ(((AIHeroClient)Target));
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.getCheckBoxItem(Config.comboMenu, "combo.q") && Spells.Q.IsReady()
                    && Target.LSIsValidTarget(777) && Config.ComboMethod == 1 &&
                    ((AIHeroClient)Target).GetBuffCount("vaynesilvereddebuff") >= 1)
                {
                    SpellManager.ExecuteQ(((AIHeroClient)Target));
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.getCheckBoxItem(Config.itemMenu, "use.youmuu") && Items.HasItem(3142)
                    && Items.CanUseItem(3142) && Target.LSIsValidTarget(ObjectManager.Player.AttackRange))
                {
                    Items.UseItem(3142);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.getCheckBoxItem(Config.itemMenu, "use.botrk") && Items.HasItem(3153)
                    && Items.CanUseItem(3153) && Target.LSIsValidTarget(550))
                {
                    if ((((AIHeroClient)Target).Health / ((AIHeroClient)Target).MaxHealth) < Config.getSliderItem(Config.itemMenu, "botrk.enemy.hp") && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < Config.getSliderItem(Config.itemMenu, "botrk.vayne.hp")))
                    {
                        Items.UseItem(3153, ((AIHeroClient)Target));
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Config.getCheckBoxItem(Config.harassMenu, "harass.q") && Spells.Q.IsReady()
                    && ObjectManager.Player.ManaPercent >= Config.getSliderItem(Config.harassMenu, "harass.mana") && Target.LSIsValidTarget(777)
                    && ((AIHeroClient)Target).GetBuffCount("vaynesilvereddebuff") >= 1 && Config.HarassMethod == 0)
                {
                    SpellManager.ExecuteQ(((AIHeroClient)Target));
                }

                
            }

            if (Target.Type == GameObjectType.obj_AI_Minion && 
                Target.Team == GameObjectTeam.Neutral && ObjectManager.Player.ManaPercent >= Config.getSliderItem(Config.jungleMenu, "jungle.mana")
                && Spells.Q.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Spells.Q.Cast(Game.CursorPos);
            }
        }

      }
        private static void OnUpdate(EventArgs args)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnJungle();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnHybrid();
            }

            if (Config.getCheckBoxItem(Config.itemMenu, "use.qss") && (Items.HasItem((int)ItemId.Quicksilver_Sash) && Items.CanUseItem((int)ItemId.Quicksilver_Sash) || 
                Items.CanUseItem(3139) && Items.HasItem(3137)))
            {
                ExecuteQss();
            }
            if (Config.getCheckBoxItem(Config.miscMenu, "auto.orb.buy") && ObjectManager.Player.Level >= Config.getSliderItem(Config.miscMenu, "orb.level")
                && !Items.HasItem((int)ItemId.Farsight_Alteration))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }

        private static void ExecuteQss()
        {
            if (Config.getCheckBoxItem(Config.itemMenu, "qss.charm") && ObjectManager.Player.HasBuffOfType(BuffType.Charm))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.getCheckBoxItem(Config.itemMenu, "qss.snare") && ObjectManager.Player.HasBuffOfType(BuffType.Snare))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.getCheckBoxItem(Config.itemMenu, "qss.polymorph") && ObjectManager.Player.HasBuffOfType(BuffType.Polymorph))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.getCheckBoxItem(Config.itemMenu, "qss.stun") && ObjectManager.Player.HasBuffOfType(BuffType.Stun))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.getCheckBoxItem(Config.itemMenu, "qss.suppression") && ObjectManager.Player.HasBuffOfType(BuffType.Suppression))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.getCheckBoxItem(Config.itemMenu, "qss.taunt") && ObjectManager.Player.HasBuffOfType(BuffType.Taunt))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }

        }

        private static void OnCombo()
        {
            if (Config.getCheckBoxItem(Config.comboMenu, "combo.e") && Spells.E.IsReady())
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(ObjectManager.Player.AttackRange)))
                {
                    if (Config.getCheckBoxItem(Config.condemMenu, "condemn." + enemy.NetworkId))
                    {
                        SpellManager.ExecuteE(enemy);
                    }
                }
            }
            if (Config.getCheckBoxItem(Config.comboMenu, "combo.r") && Spells.R.IsReady() &&
                ObjectManager.Player.CountEnemyHeroesInRange(ObjectManager.Player.AttackRange) >= Config.getSliderItem(Config.comboMenu, "combo.r.count")) // edit this part
            {
                Spells.R.Cast();
            }
        }
        private static void OnHybrid()
        {
            if ( ObjectManager.Player.ManaPercent < Config.getSliderItem(Config.harassMenu, "harass.mana"))
            {
                return;
            }

            if (Config.getCheckBoxItem(Config.harassMenu, "harass.e") && Spells.E.IsReady() && Config.HarassMethod == 1)
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(Spells.E.Range) && x.GetBuffCount("vaynesilvereddebuff") >= 2))
                {
                    Spells.E.Cast(enemy);
                }
            }
            
        }

        private static void OnJungle()
        {
            if (Config.getSliderItem(Config.jungleMenu, "jungle.mana") < ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Config.getCheckBoxItem(Config.jungleMenu, "jungle.e") && Spells.E.IsReady())
            {
                foreach (var mob in GameObjects.JungleLarge.Where(x=> x.LSIsValidTarget(Spells.E.Range)))
                {
                    Condemn.JungleCondemn(mob, Config.PushDistance);
                }
            }
        }
    }
}
