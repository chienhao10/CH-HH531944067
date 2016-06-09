using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace Leblanc.Modes
{
    internal static class ModeLane
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;

        public static void Init(Menu mainMenu)
        {
            MenuLocal = mainMenu.AddSubMenu("Lane", "Lane");
            {
                MenuLocal.Add("Lane.UseQ", new ComboBox("Q Last Hit:", 2, "Off", "On: Last Hit", "On: Unkillable Minions", "On: Both"));

                string[] strW = new string[6];
                {
                    strW[0] = "Off";
                    for (var i = 1; i < 6; i++)
                    {
                        strW[i] = "Killable Minion Count >= " + (i + 3);
                    }
                    MenuLocal.Add("Lane.UseW", new ComboBox("W:", 4, strW));
                }

                string[] strWR = new string[8];
                {
                    strWR[0] = "Off";
                    for (var i = 1; i < 8; i++)
                    {
                        strWR[i] = "Killable Minion Count >= " + (i + 3);
                    }
                    MenuLocal.Add("Lane.UseR", new ComboBox("R [Mega W]:", 6, strWR));
                }
                MenuLocal.Add("Lane.MinMana.Alone", new Slider("Min. Mana: I'm Alone %", 30, 0, 100));
                MenuLocal.Add("Lane.MinMana.Enemy", new Slider("Min. Mana: I'm NOT Alone (Enemy Close) %", 60, 0, 100));
                MenuLocal.Add("MinMana.Jungle.Default", new CheckBox("Load Recommended Settings")).OnValueChange += (sender, args) =>
                    {
                        if (args.NewValue)
                        {
                            LoadDefaultSettings();
                        }
                    };
            }

            Game.OnUpdate += OnUpdate;
        }

        public static void LoadDefaultSettings()
        {
            MenuLocal["Lane.UseQ"].Cast<ComboBox>().CurrentValue = 2;
            string[] strW = new string[6];
            {
                strW[0] = "Off";
                for (var i = 1; i < 6; i++)
                {
                    strW[i] = "Killable Minion Count >= " + (i + 3);
                }
                MenuLocal["Lane.UseW"].Cast<ComboBox>().CurrentValue = 4;
            }
            string[] strWR = new string[8];
            {
                strWR[0] = "Off";
                for (var i = 1; i < 8; i++)
                {
                    strWR[i] = "Killable Minion Count >= " + (i + 3);
                }
                MenuLocal["Lane.UseR"].Cast<ComboBox>().CurrentValue = 6;
            }
            MenuLocal["Lane.MinMana.Alone"].Cast<Slider>().CurrentValue = 30;
            MenuLocal["Lane.MinMana.Enemy"].Cast<Slider>().CurrentValue = 60;
        }

        public static float LaneMinManaPercent
        {
            get
            {
                if (ModeConfig.MenuFarm["Farm.MinMana.Enable"].Cast<KeyBind>().CurrentValue)
                {
                    return HeroManager.Enemies.Find(e => e.LSIsValidTarget(2000) && !e.IsZombie) == null
                        ? MenuLocal["Lane.MinMana.Alone"].Cast<Slider>().CurrentValue
                        : MenuLocal["Lane.MinMana.Enemy"].Cast<Slider>().CurrentValue;
                }

                return 0f;
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                return;
            }

            if (!ModeConfig.MenuFarm["Farm.Enable"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < LaneMinManaPercent)
            {
                return;
            }

            ExecuteQ();
        }

        private static void ExecuteQ()
        {
            var xUseQ = MenuLocal["Lane.UseQ"].Cast<ComboBox>().CurrentValue;
            if (Q.IsReady() && xUseQ != 0)
            {
                var minionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                if (xUseQ == 1 || xUseQ == 3)
                {
                    foreach (Obj_AI_Base vMinion in
                        from vMinion in minionsQ
                        let vMinionQDamage = ObjectManager.Player.LSGetSpellDamage(vMinion, SpellSlot.Q)
                        where
                            vMinion.Health <= vMinionQDamage &&
                            vMinion.Health > ObjectManager.Player.LSGetAutoAttackDamage(vMinion)
                        select vMinion)
                    {
                        Q.CastOnUnit(vMinion);
                    }

                }

                if (xUseQ == 2 || xUseQ == 3)
                {

                    foreach (
                        var minion in
                            minionsQ.Where(
                                m =>
                                    HealthPrediction.GetHealthPrediction(m,
                                        (int)(ObjectManager.Player.AttackCastDelay * 1000), Game.Ping / 2 + (int) W.Delay + 30) < 0)
                                .Where(m => m.Health <= Q.GetDamage(m)))
                    {
                        Q.CastOnUnit(minion);
                    }
                }
            }
        }

        private static void ExecuteQuickLaneClear()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (!MenuLocal["Lane.Enable"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            if (Q.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

                foreach (
                    var minion in
                        MinionManager.GetMinions(Q.Range)
                            .Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                {
                   // Champion.PlayerSpells.CastQObjects(minion);
                }
            }

            if (!Q.IsReady() && E.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                foreach (
                    var minion in
                        minions.Where(
                            m =>
                                HealthPrediction.GetHealthPrediction(m,
                                    (int) (ObjectManager.Player.AttackCastDelay*1000), Game.Ping/2 - 100) < 0)
                            .Where(m => m.CanKillableWith(E) && E.CanCast(m)))
                {
                    //Champion.PlayerSpells.CastQObjects(minion);
                }
            }
        }
    }
}