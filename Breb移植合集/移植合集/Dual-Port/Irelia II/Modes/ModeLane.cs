using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;

namespace Irelia.Modes
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
                MenuLocal.Add("Lane.LaneQuick", new KeyBind("Fast Lane Clear Mode:", false, KeyBind.BindTypes.PressToggle, 'T'));
                MenuLocal.Add("Lane.UseQ", new ComboBox("Q Last Hit:", 1, "Off", "On"));

                string[] strW = new string[6];
                {
                    strW[0] = "Off";
                    for (var i = 1; i < 6; i++)
                    {
                        strW[i] = "If need to AA count >= " + (i + 3);
                    }
                    MenuLocal.Add("Lane.UseW", new ComboBox("W:", 4, strW));
                }

                MenuLocal.Add("Lane.UseE", new ComboBox("E:", 1, "Off", "On: Last hit"));
                MenuLocal.Add("Lane.Item", new ComboBox("Items:", 1, "Off", "On"));
                MenuLocal.Add("Lane.MinMana.Alone", new Slider("Min. Mana: I'm Alone %", 30, 0, 100));
                MenuLocal.Add("Lane.MinMana.Enemy", new Slider("Min. Mana: I'm NOT Alone (Enemy Close) %", 60));
            }

            Game.OnUpdate += OnUpdate;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static float LaneMinManaPercent
        {
            get
            {
                if (getKeyBindItem(ModeConfig.MenuFarm, "Farm.MinMana.Enable"))
                {
                    return HeroManager.Enemies.Find(e => e.LSIsValidTarget(2000) && !e.IsZombie) == null
                        ? getSliderItem(MenuLocal, "Lane.MinMana.Alone")
                        : getSliderItem(MenuLocal, "Lane.MinMana.Enemy");
                }

                return 0f;
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ExecuteLaneClear();
            }
        }

        private static void ExecuteLaneClear()
        {
            if (!getKeyBindItem(ModeConfig.MenuFarm, "Farm.Enable"))
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < LaneMinManaPercent)
            {
                return;
            }

            if (Q.IsReady() && getBoxItem(MenuLocal, "Lane.UseQ") != 0)
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range).Where(m => !m.UnderTurret(true));

                if (getKeyBindItem(MenuLocal, "Lane.LaneQuick"))
                {
                    foreach (
                        var minion in
                            minions.Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                    {
                        Champion.PlayerSpells.CastQObjects(minion);
                    }
                }
                else
                {

                    foreach (
                        var minion in
                            minions.Where(
                                m =>
                                    HealthPrediction.GetHealthPrediction(m,
                                        (int) (ObjectManager.Player.AttackCastDelay*1000), Game.Ping/2 - 100) < 0)
                                .Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                    {
                        Champion.PlayerSpells.CastQObjects(minion);
                    }
                }
            }

            if ((getBoxItem(MenuLocal, "Lane.UseQ") == 0 || !Q.IsReady()) && E.IsReady() &&
                getBoxItem(MenuLocal, "Lane.UseE") != 0)
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                foreach (
                    var minion in
                        minions.Where(
                            m =>
                                HealthPrediction.GetHealthPrediction(m,
                                    (int) (ObjectManager.Player.AttackCastDelay*1000), Game.Ping/2 - 100) < 0)
                            .Where(m => m.Health < E.GetDamage(m) - 10 && E.CanCast(m)))
                {
                    Champion.PlayerSpells.CastQObjects(minion);
                }
            }
        }

        private static void ExecuteQuickLaneClear()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            //if (!MenuLocal.Item("Lane.Enable").GetValue<KeyBind>().Active)
            //{
                //return;
            //}

            if (Q.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

                foreach (
                    var minion in
                        MinionManager.GetMinions(Q.Range)
                            .Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                {
                    Champion.PlayerSpells.CastQObjects(minion);
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
                    Champion.PlayerSpells.CastQObjects(minion);
                }
            }
        }
    }
}