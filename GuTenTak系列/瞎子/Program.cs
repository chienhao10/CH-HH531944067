using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GuTenTak.LeeSin;
using SharpDX;
using EloBuddy.SDK.Constants;
using color = System.Drawing.Color;
using System.Collections.Generic;
using SharpDX.Direct3D9;

namespace GuTenTak.LeeSin
{
    internal class Program
    {
        public const string ChampionName = "LeeSin";
        public static Menu Menu, ModesMenu1, DrawMenu;
        public static int SkinBase;
        private static readonly string[] QSpellNames = { string.Empty, "BlindMonkQOne", "BlindMonkQTwo" };

        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }
        private static float HealthPercent()
        {
            return (PlayerInstance.Health / PlayerInstance.MaxHealth) * 100;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static Spell.Skillshot Q1;
        public static Spell.Active Q2;
        public static Spell.Targeted R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
        }

        static void Game_OnStart(EventArgs args)
        {
            Game.OnTick += OnTick;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            SkinBase = Player.Instance.SkinId;
            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Q1 = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1800, 60)
                {
                    AllowedCollisionCount = 0
                };
                Q2 = new Spell.Active(SpellSlot.Q, 1300);
                R = new Spell.Targeted(SpellSlot.R, 375);

          

                Bootstrap.Init(null);
                Chat.Print("GuTenTak Addon Loading Success", Color.Green);


                Menu = MainMenu.AddMenu("GuTenTak瞎子助手", "LeeSin");
                Menu.AddSeparator();
                Menu.AddLabel("CH汉化-GuTenTak脚本");

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("菜单", "Modes1LeeSin");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("连招设置");
                ModesMenu1.Add("ComboQ", new CheckBox("使用Q", true));
                ModesMenu1.Add("ComboQ2", new CheckBox("使用Q2", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.Add("MultiKickS", new Slider("最少踢中数量 ", 3, 2, 5));
                ModesMenu1.Add("MultiKick", new KeyBind("一脚多踢按键 ", false, KeyBind.BindTypes.PressToggle, 'T'));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("换肤");
                ModesMenu1.Add("skinhack", new CheckBox("开启换肤", false));
                ModesMenu1.Add("skinId", new ComboBox("皮肤模式", 0, "预设", "1", "2", "3", "4", "5", "6", "7", "8", "9"));

                DrawMenu = Menu.AddSubMenu("线圈", "DrawLeeSin");
                DrawMenu.Add("drawQ", new CheckBox(" 显示 Q", false));
                DrawMenu.Add("multikick", new CheckBox(" 显示一脚多踢中数量", true));
            }

            catch (Exception e)
            {

            }

        }

        private static void Game_OnDraw(EventArgs args)
        {
            try
            {
                var heroPos = Drawing.WorldToScreen(_Player.Position);
                var textDimension = Drawing.GetTextEntent("一脚多踢已开!", 9);
                const int OffsetValue = -25;
                const int OffsetValueInfo = -50;
                var offSet = new Vector2(heroPos.X, heroPos.Y - OffsetValue);
                var offSetInfo = new Vector2(heroPos.X, heroPos.Y - OffsetValueInfo);

                if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q1.IsReady() && Q1.IsLearned)
                    {
                        Circle.Draw(Color.White, Q1.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["Multikick"].Cast<CheckBox>().CurrentValue)
                {
                    if (ModesMenu1["MultiKick"].Cast<KeyBind>().CurrentValue)
                    {
                        if (ModesMenu1["MultiKickS"].Cast<Slider>().CurrentValue == 1)
                        Drawing.DrawText(offSet.X - textDimension.Width / 2f, offSet.Y, color.LimeGreen, "Multi Kick Active Min 1 !");
                        if (ModesMenu1["MultiKickS"].Cast<Slider>().CurrentValue == 2)
                            Drawing.DrawText(offSet.X - textDimension.Width / 2f, offSet.Y, color.LimeGreen, "Multi Kick Active Min 2 !");
                        if (ModesMenu1["MultiKickS"].Cast<Slider>().CurrentValue == 3)
                            Drawing.DrawText(offSet.X - textDimension.Width / 2f, offSet.Y, color.LimeGreen, "Multi Kick Active Min 3 !");
                        if (ModesMenu1["MultiKickS"].Cast<Slider>().CurrentValue == 4)
                            Drawing.DrawText(offSet.X - textDimension.Width / 2f, offSet.Y, color.LimeGreen, "Multi Kick Active Min 4 !");
                        if (ModesMenu1["MultiKickS"].Cast<Slider>().CurrentValue == 5)
                            Drawing.DrawText(offSet.X - textDimension.Width / 2f, offSet.Y, color.LimeGreen, "Multi Kick Active Min 5 !");
                    }
                }
            }
            catch (Exception e)
            {

            }
        }


        static void Game_OnUpdate(EventArgs args)
        {
            try
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Common.Combo();               
                }
            }
            catch (Exception e)
            {

            }
        }

        public static void OnTick(EventArgs args)
        {
            if (ModesMenu1["MultiKick"].Cast<KeyBind>().CurrentValue)
            {
                float leeSinRKickDistance = 700;
                float leeSinRKickWidth = 100;
                var minREnemies = ModesMenu1["MultiKickS"].Cast<Slider>().CurrentValue;
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    var startPos = enemy.ServerPosition;
                    var endPos = _Player.ServerPosition.Extend(startPos, _Player.Distance(enemy) + leeSinRKickDistance);
                    var rectangle = new Geometry.Polygon.Rectangle(startPos, (Vector3)endPos, leeSinRKickWidth);

                    if (EntityManager.Heroes.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies)
                    {
                        R.Cast(enemy);
                    }
                }
            }
            Common.Skinhack();

        }

        public static void Skinhack()
        {
            if (ModesMenu1["skinhack"].Cast<CheckBox>().CurrentValue)
            {
                Player.SetSkinId((int)ModesMenu1["skinId"].Cast<ComboBox>().CurrentValue);
            }
        }
    }
}