using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp.Common;
using Leblanc.Champion;
using Leblanc.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using CommonGeometry = Leblanc.Common.CommonGeometry;
using Font = SharpDX.Direct3D9.Font;

namespace Leblanc.Modes
{
    using System.Linq;
    using LeagueSharp;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    internal class LeblancQ
    {
        public GameObject Object { get; set; }
        public float NetworkId { get; set; }
        public Vector3 QPos { get; set; }
        public double ExpireTime { get; set; }
    }

    internal class LeblancViciousStrikes
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }
    }


    internal class LeblancRagnarok
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }

    }

    enum PcMode
    {
        NewComputer,
        NormalComputer,
        OldComputer
    }

    internal class ModeDraw
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => PlayerSpells.R;
        private static LeagueSharp.Common.Spell Q2 => PlayerSpells.Q2;
        private static LeagueSharp.Common.Spell W2 => PlayerSpells.W2;
        private static LeagueSharp.Common.Spell E2 => PlayerSpells.E2;

        private static readonly List<LeblancQ> LeblancQ = new List<LeblancQ>();
        public void Init()
        {
            MenuLocal = ModeConfig.MenuConfig.AddSubMenu("Drawings", "Drawings");
            {
                MenuLocal.Add("Draw.Enable", new CheckBox("Enable/Disable Drawings"));
                MenuLocal.Add("DrawKillableEnemy", new CheckBox("Killable Enemy Notification"));
                MenuLocal.Add("DrawKillableEnemyMini", new CheckBox("Killable Enemy [Mini Map]"));//.SetValue(new Circle(true, Color.GreenYellow)));
                MenuLocal.Add("Draw.MinionLastHit", new ComboBox("Draw Minion Last Hit", 2, "Off", "Auto Attack", "Q Damage"));

                MenuLocal.AddGroupLabel("Mana Bar Combo Indicator");
                {
                    MenuLocal.Add("DrawManaBar.Q", new CheckBox("Q"));
                    MenuLocal.Add("DrawManaBar.W", new CheckBox("W"));
                    MenuLocal.Add("DrawManaBar.E", new CheckBox("E"));
                }


                MenuLocal.AddGroupLabel("Spell Ranges");
                {
                    MenuLocal.Add("Draw.Q", new CheckBox("Q"));//.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                    MenuLocal.Add("Draw.W", new CheckBox("W"));//.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))).SetFontStyle(FontStyle.Regular, W.MenuColor()));
                    MenuLocal.Add("Draw.E", new CheckBox("E"));//.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))).SetFontStyle(FontStyle.Regular, E.MenuColor()));
                    MenuLocal.Add("Draw.R", new CheckBox("R"));//.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))).SetFontStyle(FontStyle.Regular, R.MenuColor()));
                }
                CommonManaBar.Init(MenuLocal);
            }
            Game.OnUpdate += GameOnOnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += DrawingOnOnEndScene;
        }

        private void GameOnOnUpdate(EventArgs args)
        {
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!MenuLocal["Draw.Enable"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }


            DrawSpells();
            DrawMinionLastHit();
            //KillableEnemy();
            //DrawBuffs();

            //Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Red);

            return;
            /*
            var t = TargetSelector.GetTarget(W.Range * 3, DamageType.Magical);

            if (t == null)
            {
                return;
            }

            if (t.LSIsValidTarget(W.Range))
            {
                return;
            }


            List<Vector2> xList = new List<Vector2>();

            var nLocation = ObjectManager.Player.Position.LSTo2D() + Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D()) * W.Range;


            //if (CommonGeometry.IsWallBetween(nEvadePoint.To3D(), location.To3D()))
            //{
            //    Game.PrintChat("Wall");
            //}
            //else
            //{
            //    Game.PrintChat("Not Wall");
            //}



            Vector2 wCastPosition = nLocation;

            //Render.Circle.DrawCircle(wCastPosition.To3D(), 105f, System.Drawing.Color.Red);


            if (!wCastPosition.LSIsWall())
            {
                xList.Add(wCastPosition);
            }

            if (wCastPosition.LSIsWall())
            {
                for (int j = 20; j < 80; j += 20)
                {
                    Vector2 wcPositive = ObjectManager.Player.Position.LSTo2D() + Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D()).LSRotated(j * (float)Math.PI / 180) * W.Range;
                    if (!wcPositive.LSIsWall())
                    {
                        xList.Add(wcPositive);
                    }

                    Vector2 wcNegative = ObjectManager.Player.Position.LSTo2D() + Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D()).LSRotated(-j * (float)Math.PI / 180) * W.Range;
                    if (!wcNegative.LSIsWall())
                    {
                        xList.Add(wcNegative);
                    }
                }

                float xDiff = ObjectManager.Player.Position.X - t.Position.X;
                float yDiff = ObjectManager.Player.Position.Y - t.Position.Y;
                int angle = (int)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            }

            //foreach (var aa in xList)
            //{
            //    Render.Circle.DrawCircle(aa.To3D2(), 105f, System.Drawing.Color.White);
            //}
            var nJumpPoint = xList.OrderBy(al => al.LSDistance(t.Position)).First();

            var color = System.Drawing.Color.DarkRed;
            var width = 4;

            var startpos = ObjectManager.Player.Position;
            var endpos = nJumpPoint.To3D();

            if (startpos.LSDistance(endpos) > 100)
            {
                var endpos1 = nJumpPoint.To3D() + (startpos - endpos).LSTo2D().LSNormalized().LSRotated(25 * (float)Math.PI / 180).To3D() * 75;
                var endpos2 = nJumpPoint.To3D() + (startpos - endpos).LSTo2D().LSNormalized().LSRotated(-25 * (float)Math.PI / 180).To3D() * 75;

                var x1 = new LeagueSharp.Common.Geometry.Polygon.Line(startpos, endpos);
                x1.Draw(color, width - 2);
                var y1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos, endpos1);
                y1.Draw(color, width - 2);
                var z1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos, endpos2);
                z1.Draw(color, width - 2);

                LeagueSharp.Common.Geometry.Polygon.Circle x2 = new LeagueSharp.Common.Geometry.Polygon.Circle(endpos, W.Width / 2);

                if (CommonGeometry.IsWallBetween(ObjectManager.Player.Position, endpos))
                {
                    x2.Draw(Color.Red, width - 2);
                }
                else
                {
                    x2.Draw(Color.Wheat, width - 2);
                }
            }

            if (!t.LSIsValidTarget(W.Range + Q.Range - 60))
            {
                return;
            }

            if (t.LSIsValidTarget(W.Range))
            {
                return;
            }

            var canJump = false;
            if (Modes.ModeCombo.ComboMode == ComboMode.Mode2xQ)
            {
                if ((t.Health < ModeCombo.GetComboDamage(t) - W.GetDamage(t) && Q.IsReady() && R.IsReady()) || (t.Health < Q.GetDamage(t) && Q.IsReady()))
                {
                    canJump = true;
                }
            }

            var nPoint = nJumpPoint.LSExtend(ObjectManager.Player.Position.LSTo2D(), +ObjectManager.Player.BoundingRadius * 3);
            Render.Circle.DrawCircle(nPoint.To3D(), 50f, Color.GreenYellow);

            if (CommonGeometry.IsWallBetween(nPoint.To3D(), nJumpPoint.To3D()))
            {
                canJump = false;
            }

            if (canJump && W.IsReady() && !W.StillJumped())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    W.Cast(nJumpPoint);
                }
                return;
            }
            */
        }


        private static void DrawBuffs()
        {
            if (MenuLocal["DrawBuffs"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var hero in HeroManager.AllHeroes)
                {
                    var jungleBuffs =
                        (from b in hero.Buffs
                         join b1 in CommonBuffManager.JungleBuffs on b.DisplayName equals b1.BuffName
                         select new { b, b1 }).Distinct();

                    foreach (var buffName in jungleBuffs.ToList())
                    {
                        var circle1 =
                            new CommonGeometry.Circle2(new Vector2(hero.Position.X + 3, hero.Position.Y - 3),
                                140 + (buffName.b1.Number * 20),
                                Game.Time - buffName.b.StartTime, buffName.b.EndTime - buffName.b.StartTime).ToPolygon();
                        circle1.Draw(Color.Black, 3);

                        var circle =
                            new CommonGeometry.Circle2(hero.Position.LSTo2D(), 140 + (buffName.b1.Number * 20),
                                Game.Time - buffName.b.StartTime, buffName.b.EndTime - buffName.b.StartTime).ToPolygon();
                        circle.Draw(buffName.b1.Color, 3);
                    }
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            var drawKillableEnemyMini = MenuLocal["DrawKillableEnemyMini"].Cast<CheckBox>().CurrentValue;
            if (drawKillableEnemyMini)
            {
                foreach (
                    var e in
                        HeroManager.Enemies.Where(
                            e => e.IsVisible && !e.IsDead && !e.IsZombie && e.Health < Common.CommonMath.GetComboDamage(e)))
                {
                    if ((int)Game.Time % 2 == 1)
                    {
#pragma warning disable 618
                        LeagueSharp.Common.Utility.DrawCircle(e.Position, 850, Color.GreenYellow, 2, 30, true);
#pragma warning restore 618
                    }
                }
            }
        }

        private static void DrawSpells()
        {
            var t = TargetSelector.GetTarget(Q.Range + 500, DamageType.Physical);

            var drawQ = MenuLocal["Draw.Q"].Cast<CheckBox>().CurrentValue;
            if (drawQ && Q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, Q.IsReady() ? 5 : 1);
            }

            var drawW = MenuLocal["Draw.W"].Cast<CheckBox>().CurrentValue;
            if (drawW && W.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, W.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, W.IsReady() ? 5 : 1);
            }

            var drawE = MenuLocal["Draw.E"].Cast<CheckBox>().CurrentValue;
            if (drawE && E.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Modes.ModeSettings.MaxERange, E.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, E.IsReady() ? 5 : 1);
            }

            var drawR = MenuLocal["Draw.R"].Cast<CheckBox>().CurrentValue;
            if (drawR && R.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, E.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, E.IsReady() ? 5 : 1);
            }
        }

        public static AIHeroClient GetKillableEnemy
        {
            get
            {
                if (MenuLocal["DrawKillableEnemy"].Cast<CheckBox>().CurrentValue)
                {
                    return HeroManager.Enemies.FirstOrDefault(e => e.IsVisible && !e.IsDead && !e.IsZombie && e.Health < Common.CommonMath.GetComboDamage(e));
                }
                return null;
            }
        }

        private static void KillableEnemy()
        {
            if (MenuLocal["DrawKillableEnemy"].Cast<CheckBox>().CurrentValue)
            {
                var t = KillableEnemyAa;
                if (t.Item1 != null && t.Item1.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 800) && t.Item2 > 0)
                {
                    CommonHelper.DrawText(CommonHelper.Text, $"{t.Item1.ChampionName}: {t.Item2} Combo = Kill", (int)t.Item1.HPBarPosition.X + 85, (int)t.Item1.HPBarPosition.Y + 5, SharpDX.Color.GreenYellow);
                    //CommonHelper.DrawText(CommonHelper.Text, $"{t.Item1.ChampionName}: {t.Item2} Combo = Kill", (int)t.Item1.HPBarPosition.X + 7, (int)t.Item1.HPBarPosition.Y + 36, SharpDX.Color.GreenYellow);

                }
            }
        }
        private static void DrawMinionLastHit()
        {

            var drawMinionLastHit = MenuLocal["Draw.MinionLastHit"].Cast<ComboBox>().CurrentValue;
            if (drawMinionLastHit != 0)
            {

                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, (float)(Q.Range * 1.5), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

                switch (drawMinionLastHit)
                {
                    case 1:
                        {
                            foreach (var m in minions.ToList().Where(m => m.Health < ObjectManager.Player.TotalAttackDamage)
                                )
                            {
                                Render.Circle.DrawCircle(m.Position, m.BoundingRadius, Color.Wheat);
                            }
                            break;
                        }
                    case 2:
                        {
                            foreach (var m in minions.ToList().Where(m => m.Health < Q.GetDamage(m)))
                            {
                                Render.Circle.DrawCircle(m.Position, m.BoundingRadius, Color.Wheat);
                            }
                            break;
                        }
                }
            }
        }

        private static Tuple<AIHeroClient, int> KillableEnemyAa
        {
            get
            {
                var x = 0;
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                {
                    if (t.LSIsValidTarget())
                    {
                        if (t.Health <= Common.CommonMath.GetComboDamage(t))
                        {
                            x = (int)Math.Ceiling(t.Health / ObjectManager.Player.TotalAttackDamage);
                        }
                        return new Tuple<AIHeroClient, int>(t, x);
                    }

                }
                return new Tuple<AIHeroClient, int>(t, x);
            }
        }

        public static void DrawText(Font aFont, String aText, int aPosX, int aPosY, SharpDX.Color aColor)
        {
            aFont.DrawText(null, aText, aPosX + 2, aPosY + 2, aColor != SharpDX.Color.Black ? SharpDX.Color.Black : SharpDX.Color.White);
            aFont.DrawText(null, aText, aPosX, aPosY, aColor);
        }
    }

    internal class Sprite
    {
        private static LeagueSharp.Common.Spell Q => PlayerSpells.Q;

        private static AIHeroClient KillableEnemy
        {
            get
            {
                var t = ModeDraw.GetKillableEnemy;

                if (t.LSIsValidTarget())
                    return t;

                return null;
            }
        }
    }
}
