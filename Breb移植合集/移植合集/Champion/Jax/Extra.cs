using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace JaxQx
{
    internal class Extra
    {
        private static readonly Menu menuExtra = Program.Config;
        public static Menu menu;

        public Extra()
        {
            menu = menuExtra.AddSubMenu("Extra", "Extra");
            menu.Add("Extra.DrawKillableEnemy", new CheckBox("Killable Enemy Notification"));
            menu.Add("Extra.DrawMinionLastHist", new CheckBox("Draw Minion Last Hit"));

            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static Tuple<AIHeroClient, int> KillableEnemyAA
        {
            get
            {
                var x = 0;
                var t = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 800, DamageType.Physical);
                {
                    if (t.IsValidTarget())
                    {
                        if (t.Health
                            < ObjectManager.Player.TotalAttackDamage
                            *(1/ObjectManager.Player.AttackCastDelay > 1500 ? 12 : 8))
                        {
                            x = (int) Math.Ceiling(t.Health/ObjectManager.Player.TotalAttackDamage);
                        }
                        return new Tuple<AIHeroClient, int>(t, x);
                    }
                }
                return new Tuple<AIHeroClient, int>(t, x);
            }
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

        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawMinionLastHit = getCheckBoxItem(menu, "Extra.DrawMinionLastHist");
            if (drawMinionLastHit)
            {
                foreach (
                    var xMinion in
                        MinionManager.GetMinions(
                            Program.Player.Position,
                            Program.Player.AttackRange + Program.Player.BoundingRadius + 300,
                            MinionTypes.All,
                            MinionTeam.Enemy,
                            MinionOrderTypes.MaxHealth)
                            .Where(xMinion => Program.Player.GetAutoAttackDamage(xMinion, true) >= xMinion.Health))
                {
                    Render.Circle.DrawCircle(xMinion.Position, xMinion.BoundingRadius, Color.GreenYellow);
                }
            }

            if (getCheckBoxItem(menu, "Extra.DrawKillableEnemy"))
            {
                var t = KillableEnemyAA;
                if (t.Item1 != null && t.Item1.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 800) &&
                    t.Item2 > 0)
                {
                    Utils.DrawText(
                        Utils.Text,
                        string.Format("{0}: {1} x AA Damage = Kill", t.Item1.ChampionName, t.Item2),
                        (int) t.Item1.HPBarPosition.X + 65,
                        (int) t.Item1.HPBarPosition.Y + 5,
                        SharpDX.Color.White);
                }
            }
        }
    }
}