using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp.Common;
using Irelia.Champion;
using Irelia.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using CommonGeometry = Irelia.Common.CommonGeometry;
using Font = SharpDX.Direct3D9.Font;

namespace Irelia.Modes
{
    using System.Linq;
    using LeagueSharp;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    internal class IreliaQ
    {
        public GameObject Object { get; set; }
        public float NetworkId { get; set; }
        public Vector3 QPos { get; set; }
        public double ExpireTime { get; set; }
    }

    internal class IreliaViciousStrikes
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }
    }


    internal class IreliaRagnarok
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

    internal class ModeDraw
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => PlayerSpells.R;

        //private static readonly List<MenuItem> MenuLocalSubMenuItems = new List<MenuItem>();

        private static readonly List<IreliaQ> IreliaQ = new List<IreliaQ>();
        public void Init()
        {
            MenuLocal = ModeConfig.MenuConfig.AddSubMenu("Drawings", "Drawings");
            {
                MenuLocal.Add("Draw.Enable", new CheckBox("Enable/Disable Drawings:"));

                MenuLocal.AddGroupLabel("Mana Bar Combo Indicator");
                {
                    MenuLocal.Add("DrawManaBar.Q", new CheckBox("Q:"));//;.SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                    MenuLocal.Add("DrawManaBar.W", new CheckBox("W:"));//;.SetFontStyle(FontStyle.Regular, W.MenuColor()));
                    MenuLocal.Add("DrawManaBar.E", new CheckBox("E:"));//;.SetFontStyle(FontStyle.Regular, E.MenuColor()));
                    MenuLocal.Add("DrawManaBar.R", new CheckBox("R:"));//;.SetFontStyle(FontStyle.Regular, R.MenuColor()));
                }

                MenuLocal.AddGroupLabel("Spell Ranges");
                MenuLocal.Add("Draw.Q", new CheckBox("Q:", false));//.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                MenuLocal.Add("Draw.E", new CheckBox("E:", false));//.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))).SetFontStyle(FontStyle.Regular, E.MenuColor()));
                MenuLocal.Add("Draw.R", new CheckBox("R:", false));//.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))).SetFontStyle(FontStyle.Regular, R.MenuColor());

                MenuLocal.AddGroupLabel("Buff Times");
                MenuLocal.Add("DrawBuffs", new ComboBox("Show Red/Blue Time Circle", 3, "Off", "Blue Buff", "Red Buff", "Both"));

                MenuLocal.AddGroupLabel("Spell Times");
                {
                    MenuLocal.Add("Draw.W.BuffTime", new ComboBox("W: Show Time Circle", 1, "Off", "On"));//.SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, R.MenuColor()));
                    MenuLocal.Add("Draw.R.BuffTime", new ComboBox("R: Show Time Circle", 1, "Off", "On"));//.SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, E.MenuColor()));
                }

                MenuLocal.Add("DrawKillableEnemy", new CheckBox("Killable Enemy Notification"));
                MenuLocal.Add("DrawKillableEnemyMini", new CheckBox("Killable Enemy [Mini Map]"));//.SetValue(new Circle(true, Color.GreenYellow)));

                MenuLocal.Add("Draw.MinionLastHit", new ComboBox("Draw Minion Last Hit", 2, "Off", "Auto Attack", "Q Damage"));

                CommonManaBar.Init(MenuLocal);
            }

            Game.OnUpdate += GameOnOnUpdate;

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += DrawingOnOnEndScene;
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

        private void GameOnOnUpdate(EventArgs args)
        {
            if (getBoxItem(MenuLocal, "Draw.W.BuffTime") == 1 && CommonBuffs.IreliaHaveFrenziedStrikes)
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "IreliaFrenziedStrikes");
                if (IreliaViciousStrikes.EndTime < Game.Time || b.EndTime > IreliaViciousStrikes.EndTime)
                {
                    IreliaViciousStrikes.StartTime = b.StartTime;
                    IreliaViciousStrikes.EndTime = b.EndTime;
                }
            }

            if (getBoxItem(MenuLocal, "Draw.R.BuffTime") == 1 & CommonBuffs.IreliaHaveRagnarok)
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "IreliaRagnarok");
                if (IreliaRagnarok.EndTime < Game.Time || b.EndTime > IreliaRagnarok.EndTime)
                {
                    IreliaRagnarok.StartTime = b.StartTime;
                    IreliaRagnarok.EndTime = b.EndTime;
                }
            }

            var drawBuffs = getBoxItem(MenuLocal, "DrawBuffs");
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
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!getCheckBoxItem(MenuLocal, "Draw.Enable"))
            {
                return;
            }

            foreach (var t in HeroManager.Enemies.Where(e => !e.IsDead && e.LSIsValidTarget(Q.Range * 2)).Where(t => t.CanStun()))
            {
                //Render.Circle.DrawCircle(t.Position, 105, Color.White);
                CommonHelper.DrawText(CommonHelper.TextStatus, "Can Stun!", (int)t.HPBarPosition.X + 145, (int)t.HPBarPosition.Y + 5, SharpDX.Color.White);
            }

            DrawSpells();
            DrawMinionLastHit();
            KillableEnemy();
            DrawBuffs();
        }


        private static void DrawBuffs()
        {
            var drawBuffs = getBoxItem(MenuLocal, "DrawBuffs");

            if ((drawBuffs == 1 | drawBuffs == 3) && ObjectManager.Player.HasBlueBuff())
            {
                if (BlueBuff.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 170f, Game.Time - BlueBuff.StartTime, BlueBuff.EndTime - BlueBuff.StartTime).ToPolygon();
                    circle1.Draw(Color.Black, 4);

                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.To2D(), 170f, Game.Time - BlueBuff.StartTime, BlueBuff.EndTime - BlueBuff.StartTime).ToPolygon();
                    circle.Draw(Color.Blue, 4);
                }
            }

            if ((drawBuffs == 2 || drawBuffs == 3) && ObjectManager.Player.HasRedBuff())
            {
                if (RedBuff.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 150f, Game.Time - RedBuff.StartTime, RedBuff.EndTime - RedBuff.StartTime).ToPolygon();
                    circle1.Draw(Color.Black, 4);

                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.To2D(), 150f, Game.Time - RedBuff.StartTime, RedBuff.EndTime - RedBuff.StartTime).ToPolygon();
                    circle.Draw(Color.Red, 4);
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            var drawKillableEnemyMini = getCheckBoxItem(MenuLocal, "DrawKillableEnemyMini");
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
            if (t.LSIsValidTarget())
            {
                var targetBehind = t.Position + Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position) * 80;
                Render.Circle.DrawCircle(targetBehind, 75f, Color.Red, 2);
            }

            var drawQ = getCheckBoxItem(MenuLocal, "Draw.Q");
            if (drawQ && Q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, Q.IsReady() ? 5 : 1);
            }

            var drawE = getCheckBoxItem(MenuLocal, "Draw.E");
            if (drawE && E.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, E.IsReady() ? 5 : 1);
            }

            var drawR = getCheckBoxItem(MenuLocal, "Draw.R");
            if (drawR && R.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, E.IsReady() ? Color.FromArgb(255, 255, 255, 255) : Color.LightGray, E.IsReady() ? 5 : 1);
            }
        }

        public static AIHeroClient GetKillableEnemy
        {
            get
            {
                if (getCheckBoxItem(MenuLocal, "DrawKillableEnemy"))
                {
                    return HeroManager.Enemies.FirstOrDefault(e => e.IsVisible && !e.IsDead && !e.IsZombie && e.Health < Common.CommonMath.GetComboDamage(e));
                }
                return null;
            }
        }

        private static void KillableEnemy()
        {
            if (getCheckBoxItem(MenuLocal, "DrawKillableEnemy"))
            {
                var t = KillableEnemyAa;
                if (t.Item1 != null && t.Item1.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 800) && t.Item2 > 0)
                {
                    CommonHelper.DrawText(CommonHelper.Text, $"{t.Item1.ChampionName}: {t.Item2} Combo = Kill", (int)t.Item1.HPBarPosition.X + 85, (int)t.Item1.HPBarPosition.Y + 5, SharpDX.Color.GreenYellow);
                }
            }
        }
        private static void DrawMinionLastHit()
        {

            var drawMinionLastHit = getBoxItem(MenuLocal, "Draw.MinionLastHit");
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
        //private static Spell Q => Champion.PlayerSpells.Q;

        private static Vector2 DrawPosition
        {
            get
            {
                return Vector2.Zero;
                //var drawStatus = CommonTargetSelector.MenuLocal.Item("Draw.Status");
                //if (KillableEnemy == null || (drawStatus != 2 && drawStatus != 3))
                //return new Vector2(0f, 0f);

                //return new Vector2(KillableEnemy.HPBarPosition.X + KillableEnemy.BoundingRadius / 2f,
                //KillableEnemy.HPBarPosition.Y - 70);
            }
        }

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
