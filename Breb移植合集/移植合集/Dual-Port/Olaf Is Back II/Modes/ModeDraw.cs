using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using LeagueSharp.Common;
using OlafxQx.Champion;
using OlafxQx.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using CommonGeometry = OlafxQx.Common.CommonGeometry;
using Font = SharpDX.Direct3D9.Font;

namespace OlafxQx.Modes
{
    using System.Linq;
    using LeagueSharp;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    internal class OlafQ
    {
        public GameObject Object { get; set; }
        public float NetworkId { get; set; }
        public Vector3 QPos { get; set; }
        public double ExpireTime { get; set; }
    }

    internal class OlafViciousStrikes
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }
    }


    internal class OlafRagnarok
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }

    }
    internal class BlueBuff
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }
    }

    internal class RedBuff
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

    internal static class ModeDraw
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => PlayerSpells.R;

        public static Font AxeDisplayText;

        private static readonly List<OlafQ> OlafQ = new List<OlafQ>();
        public static void Init()
        {
            AxeDisplayText = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 38,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural,
                });

            PlayerObjects.Init();

            MenuLocal = ModeConfig.MenuConfig.AddSubMenu("Drawings", "Drawings");
            {
                MenuLocal.Add("Draw.Enable", new CheckBox("Enable/Disable Drawings:"));
                MenuLocal.Add("DrawKillableEnemy", new CheckBox("Killable Enemy Notification"));
                //MenuLocal.Add("DrawKillableEnemyMini", new CheckBox("Killable Enemy [Mini Map]"));
                MenuLocal.Add("DrawMinionLastHist", new CheckBox("Draw Minion Last Hit"));

                MenuLocal.AddGroupLabel("Mana Bar Combo Indicator");
                {
                    MenuLocal.Add("DrawManaBar.Q", new CheckBox("Q"));
                    MenuLocal.Add("DrawManaBar.W", new CheckBox("W"));
                    MenuLocal.Add("DrawManaBar.E", new CheckBox("E"));
                    MenuLocal.Add("DrawManaBar.R", new CheckBox("R"));
                }

                MenuLocal.AddGroupLabel("Spell Ranges");
                {
                    MenuLocal.Add("Draw.Q", new ComboBox("Q:", 3, "Off", "On: Small", "On: Large", "On: Both"));
                    MenuLocal.Add("Draw.E", new CheckBox("E:", false));
                }

                /*

                MenuLocal.AddGroupLabel("Buff Times");
                {
                    MenuLocal.Add("DrawBuffs", new ComboBox("Show Red/Blue Time Circle", 3, "Off", "Blue Buff", "Red Buff", "Both"));
                }

                MenuLocal.AddGroupLabel("Spell Times");
                {
                    MenuLocal.Add("Draw.W.BuffTime", new ComboBox("E: Show Time Circle", 1, "Off", "On"));
                    MenuLocal.Add("Draw.R.BuffTime", new ComboBox("R: Show Time Circle", 1, "Off", "On"));
                }

                MenuLocal.AddGroupLabel("Axe Times");
                {
                    MenuLocal.Add("Draw.AxePosition", new ComboBox("OlafAxe Position", 3, "Off", "Circle", "Line", "Both"));
                    MenuLocal.Add("Draw.AxeTime", new CheckBox("OlafAxe Time Remaining"));
                }

                */
                CommonManaBar.Init(MenuLocal);
            }

            Game.OnUpdate += GameOnOnUpdate;

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += DrawingOnOnEndScene;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            /*
            if (MenuLocal["Draw.W.BuffTime"].Cast<ComboBox>().CurrentValue == 1 && CommonHelper.OlafHaveFrenziedStrikes)
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "OlafFrenziedStrikes");
                if (OlafViciousStrikes.EndTime < Game.Time || b.EndTime > OlafViciousStrikes.EndTime)
                {
                    OlafViciousStrikes.StartTime = b.StartTime;
                    OlafViciousStrikes.EndTime = b.EndTime;
                }
            }

            if (MenuLocal["Draw.R.BuffTime"].Cast<ComboBox>().CurrentValue == 1 & CommonHelper.OlafHaveRagnarok)
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "OlafRagnarok");
                if (OlafRagnarok.EndTime < Game.Time || b.EndTime > OlafRagnarok.EndTime)
                {
                    OlafRagnarok.StartTime = b.StartTime;
                    OlafRagnarok.EndTime = b.EndTime;
                }
            }

            var drawBuffs = MenuLocal["DrawBuffs"].Cast<ComboBox>().CurrentValue;
            if ((drawBuffs == 1 | drawBuffs == 3) && ObjectManager.Player.HasBlueBuff())
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "CrestoftheAncientGolem");
                if (BlueBuff.EndTime < Game.Time || b.EndTime > BlueBuff.EndTime)
                {
                    BlueBuff.StartTime = b.StartTime;
                    BlueBuff.EndTime = b.EndTime;
                }
            }

            if ((drawBuffs == 2 | drawBuffs == 3) && ObjectManager.Player.HasRedBuff())
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "BlessingoftheLizardElder");
                if (RedBuff.EndTime < Game.Time || b.EndTime > RedBuff.EndTime)
                {
                    RedBuff.StartTime = b.StartTime;
                    RedBuff.EndTime = b.EndTime;
                }
            }
            */
        }
        

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!MenuLocal["Draw.Enable"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            DrawSpells();
            DrawMinionLastHit();
            KillableEnemy();
            //DrawBuffs();
            //DrawViciousStrikesBuffTime();
            //DrawRagnarokBuffTime();
            //DrawAxeTimes();
        }

        private static void DrawViciousStrikesBuffTime()
        {
            if (MenuLocal["Draw.W.BuffTime"].Cast<ComboBox>().CurrentValue == 1 && CommonHelper.OlafHaveFrenziedStrikes)
            {
                if (OlafViciousStrikes.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 190f, Game.Time * 100 - OlafViciousStrikes.StartTime * 100, OlafViciousStrikes.EndTime * 100 - OlafViciousStrikes.StartTime * 100).ToPolygon();
                    circle1.Draw(Color.Black, 4);
                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.LSTo2D(), 190f, Game.Time * 100 - OlafViciousStrikes.StartTime * 100, OlafViciousStrikes.EndTime * 100 - OlafViciousStrikes.StartTime * 100).ToPolygon();
                    circle.Draw(Color.GreenYellow, 4);

                }
            }
        }

        private static void DrawRagnarokBuffTime()
        {
            if (MenuLocal["Draw.R.BuffTime"].Cast<ComboBox>().CurrentValue == 1 && CommonHelper.OlafHaveRagnarok)
            {
                if (OlafRagnarok.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 220f, Game.Time * 100 - OlafRagnarok.StartTime * 100, OlafRagnarok.EndTime * 100 - OlafRagnarok.StartTime * 100).ToPolygon();
                    circle1.Draw(Color.Black, 4);

                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.LSTo2D(), 220f, Game.Time * 100 - OlafRagnarok.StartTime * 100, OlafRagnarok.EndTime * 100 - OlafRagnarok.StartTime * 100).ToPolygon();
                    circle.Draw(Color.DarkRed, 4);
                }
            }
        }

        private static void DrawAxeTimes()
        {
            if (PlayerObjects.AxeObject == null)
            {
                return;
            }

            var drawAxePosition = MenuLocal["Draw.AxePosition"].Cast<ComboBox>().CurrentValue;

            var exTime = TimeSpan.FromSeconds(PlayerObjects.EndTime - Game.Time).TotalSeconds;
            var color = exTime > 4 ? Color.White : Color.Red;
            switch (drawAxePosition)
            {
                case 1:
                    {
                        var circle1 = new CommonGeometry.Circle2(new Vector2(PlayerObjects.AxeObject.Position.X + 3, PlayerObjects.AxeObject.Position.Y - 3), 150f, Game.Time * 100 - PlayerObjects.StartTime * 100, PlayerObjects.EndTime * 100 - PlayerObjects.StartTime * 100).ToPolygon();
                        circle1.Draw(Color.Black, 4);

                        var circle = new CommonGeometry.Circle2(PlayerObjects.AxeObject.Position.LSTo2D(), 150f, Game.Time * 100 - PlayerObjects.StartTime * 100, PlayerObjects.EndTime * 100 - PlayerObjects.StartTime * 100).ToPolygon();
                        circle.Draw(TimeSpan.FromSeconds(PlayerObjects.EndTime - Game.Time).TotalSeconds > 4 ? Color.White : Color.Red, 4);
                        break;
                    }
                case 2:
                    {
                        var startpos = ObjectManager.Player.Position.LSTo2D();
                        var endpos = PlayerObjects.AxeObject.Position.LSTo2D();
                        if (startpos.LSDistance(endpos) > 100)
                        {
                            var endpos1 = PlayerObjects.AxeObject.Position + (startpos - endpos).LSNormalized().LSRotated(25 * (float)Math.PI / 180).To3D() * 75;
                            var endpos2 = PlayerObjects.AxeObject.Position + (startpos - endpos).LSNormalized().LSRotated(-25 * (float)Math.PI / 180).To3D() * 75;

                            var x1 = new LeagueSharp.Common.Geometry.Polygon.Line(startpos, endpos);
                            x1.Draw(color, 1);
                            var y1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos.To3D(), endpos1);
                            y1.Draw(color, 2);
                            var z1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos.To3D(), endpos2);
                            z1.Draw(color, 2);
                        }
                        break;
                    }

                case 3:
                    {
                        var circle1 = new CommonGeometry.Circle2(new Vector2(PlayerObjects.AxeObject.Position.X + 3, PlayerObjects.AxeObject.Position.Y - 3), 150f, Game.Time * 100 - PlayerObjects.StartTime * 100, PlayerObjects.EndTime * 100 - PlayerObjects.StartTime * 100).ToPolygon();
                        circle1.Draw(Color.Black, 4);

                        var circle = new CommonGeometry.Circle2(PlayerObjects.AxeObject.Position.LSTo2D(), 150f, Game.Time * 100 - PlayerObjects.StartTime * 100, PlayerObjects.EndTime * 100 - PlayerObjects.StartTime * 100).ToPolygon();
                        circle.Draw(TimeSpan.FromSeconds(PlayerObjects.EndTime - Game.Time).TotalSeconds > 4 ? Color.White : Color.Red, 4);

                        var startpos = ObjectManager.Player.Position.LSTo2D();
                        var endpos = PlayerObjects.AxeObject.Position.LSTo2D();
                        if (startpos.LSDistance(endpos) > 100)
                        {
                            var endpos1 = PlayerObjects.AxeObject.Position + (startpos - endpos).LSNormalized().LSRotated(25 * (float)Math.PI / 180).To3D() * 75;
                            var endpos2 = PlayerObjects.AxeObject.Position + (startpos - endpos).LSNormalized().LSRotated(-25 * (float)Math.PI / 180).To3D() * 75;

                            var x1 = new LeagueSharp.Common.Geometry.Polygon.Line(startpos, endpos);
                            x1.Draw(color, 1);
                            var y1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos.To3D(), endpos1);
                            y1.Draw(color, 2);
                            var z1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos.To3D(), endpos2);
                            z1.Draw(color, 2);
                        }

                        var line = new Geometry.Polygon.Line(ObjectManager.Player.Position, PlayerObjects.AxeObject.Position, ObjectManager.Player.LSDistance(PlayerObjects.AxeObject.Position));
                        line.Draw(color, 2);
                        break;
                    }
            }

            if (MenuLocal["Draw.AxeTime"].Cast<CheckBox>().CurrentValue)
            {
                var time = TimeSpan.FromSeconds(PlayerObjects.EndTime - Game.Time);
                var pos = Drawing.WorldToScreen(PlayerObjects.AxeObject.Position);
                var display = $"{time.Seconds:D1}";

                SharpDX.Color vTimeColor = time.TotalSeconds > 4 ? SharpDX.Color.White : SharpDX.Color.Red;
                DrawText(AxeDisplayText, display, (int)pos.X - display.Length, (int)pos.Y - 105, vTimeColor);
            }
        }


        private static void DrawBuffs()
        {
            var drawBuffs = MenuLocal["DrawBuffs"].Cast<ComboBox>().CurrentValue;

            if ((drawBuffs == 1 | drawBuffs == 3) && ObjectManager.Player.HasBlueBuff())
            {
                if (BlueBuff.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 170f, Game.Time - BlueBuff.StartTime, BlueBuff.EndTime - BlueBuff.StartTime).ToPolygon();
                    circle1.Draw(Color.Black, 4);

                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.LSTo2D(), 170f, Game.Time - BlueBuff.StartTime, BlueBuff.EndTime - BlueBuff.StartTime).ToPolygon();
                    circle.Draw(Color.Blue, 4);
                }
            }

            if ((drawBuffs == 2 || drawBuffs == 3) && ObjectManager.Player.HasRedBuff())
            {
                if (RedBuff.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 150f, Game.Time - RedBuff.StartTime, RedBuff.EndTime - RedBuff.StartTime).ToPolygon();
                    circle1.Draw(Color.Black, 4);

                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.LSTo2D(), 150f, Game.Time - RedBuff.StartTime, RedBuff.EndTime - RedBuff.StartTime).ToPolygon();
                    circle.Draw(Color.Red, 4);
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            return;
            /*
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
                        //LeagueSharp.Common.Utility.DrawCircle(e.Position, 850, Color.GreenYellow, 2, 30, true);
#pragma warning disable 618
                    }
                }
            }
            */
        }

        private static void DrawSpells()
        {
            var t = TargetSelector.GetTarget(Q.Range + 500, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                var targetBehind = t.Position + Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position) * 80;
                Render.Circle.DrawCircle(targetBehind, 75f, Color.Red, 2);
            }

            var drawQ = MenuLocal["Draw.Q"].Cast<ComboBox>().CurrentValue;
            if (drawQ != 0 && Q.Level > 0)
            {
                switch (drawQ)
                {
                    case 1:
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, Modes.ModeHarass.MenuLocal["Harass.Q.SmallRange"].Cast<Slider>().CurrentValue, Q.IsReady() ? Color.Coral : Color.LightGray, Q.IsReady() ? 5 : 1);
                            break;
                        }
                    case 2:
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Color.Coral : Color.LightGray, Q.IsReady() ? 5 : 1);
                            break;
                        }
                    case 3:
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, Modes.ModeHarass.MenuLocal["Harass.Q.SmallRange"].Cast<Slider>().CurrentValue, Q.IsReady() ? Color.Coral : Color.LightGray, Q.IsReady() ? 5 : 1);
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Color.Coral : Color.LightGray, Q.IsReady() ? 5 : 1);
                            break;
                        }
                }
            }

            var drawE = MenuLocal["Draw.E"].Cast<CheckBox>().CurrentValue;
            if (drawE && E.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, E.IsReady() ? 5 : 1);
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
                    //CommonHelper.DrawText(CommonHelper.Text, $"{t.Item1.ChampionName}: {t.Item2} x AA Damage = Kill", (int)t.Item1.HPBarPosition.X + 65, (int)t.Item1.HPBarPosition.Y + 5, SharpDX.Color.White);
                    CommonHelper.DrawText(CommonHelper.Text, $"{t.Item1.ChampionName}: {t.Item2} Combo = Kill", (int)t.Item1.HPBarPosition.X + 7, (int)t.Item1.HPBarPosition.Y + 36, SharpDX.Color.GreenYellow);
                    //                    Common.CommonGeometry.DrawText(CommonGeometry.Text, ">>> Combo Kill <<<", t.Item1.HPBarPosition.X + 7, t.Item1.HPBarPosition.Y + 36, SharpDX.Color.White);
                }
            }
        }
        private static void DrawMinionLastHit()
        {
            var drawMinionLastHit = MenuLocal["DrawMinionLastHist"].Cast<CheckBox>().CurrentValue;
            if (drawMinionLastHit)
            {
                foreach (
                    var xMinion in
                        MinionManager.GetMinions(
                            ObjectManager.Player.Position,
                            ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius + 300,
                            MinionTypes.All,
                            MinionTeam.Enemy,
                            MinionOrderTypes.MaxHealth)
                            .Where(xMinion => ObjectManager.Player.LSGetAutoAttackDamage(xMinion, true) >= xMinion.Health))
                {
                    Render.Circle.DrawCircle(xMinion.Position, xMinion.BoundingRadius, Color.GreenYellow);
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
                        //if (t.Health < ObjectManager.Player.TotalAttackDamage * (1 / ObjectManager.Player.AttackCastDelay > 1500 ? 12 : 8))
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

        private static bool DrawSprite => true;

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
