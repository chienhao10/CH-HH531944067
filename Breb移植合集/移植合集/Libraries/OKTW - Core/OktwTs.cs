using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Orbwalking = SebbyLib.Orbwalking;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Core
{
    internal class OktwTs
    {
        private static Menu Sub;
        private readonly Menu Config = Program.Config;

        private AIHeroClient FocusTarget, DrawInfo;
        private float LatFocusTime = Game.Time;

        private AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool getCheckBoxItem(string item)
        {
            return Sub[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Sub[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Sub[item].Cast<KeyBind>().CurrentValue;
        }

        public void LoadOKTW()
        {
            Sub = Config.AddSubMenu("Target Selector OKTW©");
            Sub.Add("TsAa", new Slider("Auto-attack MODE (0 : Fast Kill | 1 : Priority | 2 : Normal EB TS) :", 2, 0, 2));

            // FAST KILL //////////////////////////
            Sub.Add("extraFocus", new CheckBox("One Focus To Win", Player.IsMelee));
            Sub.Add("extraRang", new Slider("Extra Focus Range", 300, 0, 600));
            Sub.Add("extraTime", new Slider("Time out focus time (ms)", 2000, 0, 4000));
            Sub.Add("drawFocus", new CheckBox("Draw notification", Player.IsMelee));

            var i = 5;

            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(enemy => enemy.IsEnemy)
                        .OrderBy(enemy => enemy.MaxHealth/Player.GetAutoAttackDamage(enemy)))
            {
                Sub.Add("TsAaPriority" + enemy.ChampionName, new Slider(enemy.ChampionName, i, 0, 5));
                i--;
            }
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            if (target is AIHeroClient)
            {
                FocusTarget = (AIHeroClient) target;
                LatFocusTime = Game.Time;
            }
        }

        private void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (getSliderItem("TsAa") != 0 || !getCheckBoxItem("extraFocus") || !Program.Combo)
            {
                DrawInfo = null;
                return;
            }

            if (args.Target is AIHeroClient)
            {
                var newTarget = (AIHeroClient) args.Target;
                var forceFocusEnemy = newTarget;
                // if (newTarget.Health / Player.GetAutoAttackDamage(newTarget) > FocusTarget.Health / Player.GetAutoAttackDamage(FocusTarget))
                {
                }
                // else
                {
                    var aaRange = Player.AttackRange + Player.BoundingRadius + getSliderItem("extraRang");

                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(aaRange)))
                    {
                        if (enemy.Health/Player.GetAutoAttackDamage(enemy) + 1 <
                            forceFocusEnemy.Health/Player.GetAutoAttackDamage(forceFocusEnemy))
                        {
                            forceFocusEnemy = enemy;
                        }
                    }
                }
                if (forceFocusEnemy.NetworkId != newTarget.NetworkId &&
                    Game.Time - LatFocusTime < getSliderItem("extraTime")/1000)
                {
                    args.Process = false;
                    Program.debug("Focus: " + forceFocusEnemy.ChampionName);
                    DrawInfo = forceFocusEnemy;
                    return;
                }
            }
            DrawInfo = null;
        }

        private void OnDraw(EventArgs args)
        {
            if (getSliderItem("TsAa") == 2)
            {
                return;
            }
            if (DrawInfo.IsValidTarget() && (int) (Game.Time*10)%2 == 0 && getCheckBoxItem("drawFocus"))
            {
                Utility.DrawCircle(Player.Position,
                    Player.AttackRange + Player.BoundingRadius + getSliderItem("extraRang"), Color.Gray, 1, 1);

                drawText("FORCE FOCUS", DrawInfo.Position, Color.Orange);
            }
        }

        public static void drawText(string msg, Vector3 Hero, Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1] + weight, color, msg);
        }

        private void OnUpdate(EventArgs args)
        {
            if (getSliderItem("TsAa") == 2 || !Orbwalking.CanAttack() || !Program.Combo)
            {
                return;
            }

            var orbT = Orbwalker.LastTarget;

            if (orbT != null)
            {
                var bestTarget = (AIHeroClient) orbT;
                var hitToBestTarget = bestTarget.Health/Player.GetAutoAttackDamage(bestTarget);

                if (getSliderItem("TsAa") == 0)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy => enemy.IsValidTarget() && LeagueSharp.Common.Orbwalking.InAutoAttackRange(enemy))
                        )
                    {
                        if (enemy.Health/Player.GetAutoAttackDamage(enemy) < hitToBestTarget)
                        {
                            bestTarget = enemy;
                        }
                    }
                }
                else
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy => enemy.IsValidTarget() && LeagueSharp.Common.Orbwalking.InAutoAttackRange(enemy))
                        )
                    {
                        if (enemy.Health/Player.GetAutoAttackDamage(enemy) < 3)
                        {
                            bestTarget = enemy;
                            break;
                        }
                        if (getSliderItem("TsAaPriority" + enemy.ChampionName) >
                            getSliderItem("TsAaPriority" + bestTarget.ChampionName))
                        {
                            bestTarget = enemy;
                        }
                    }
                }
                if (bestTarget.NetworkId != orbT.NetworkId)
                {
                    Program.debug("force " + bestTarget.ChampionName);
                    Orbwalker.ForcedTarget = bestTarget;
                }
            }
        }
    }
}