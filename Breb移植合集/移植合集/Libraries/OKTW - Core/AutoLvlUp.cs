using System;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Core
{
    internal class AutoLvlUp
    {
        public static Menu Sub;
        private readonly Menu Config = Program.Config;
        private int lvl1;
        private int lvl2;
        private int lvl3;
        private int lvl4;

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
            Sub = Config.AddSubMenu("AutoLvlUp OKTW©");
            Sub.Add("AutoLvl", new CheckBox("ENABLE"));

            Sub.AddLabel("0 : Q | 1 : W | 2 : E | 3 : R");
            Sub.Add("1", new Slider("1", 3, 0, 3));
            Sub.Add("2", new Slider("2", 1, 0, 3));
            Sub.Add("3", new Slider("3", 1, 0, 3));
            Sub.Add("4", new Slider("4", 1, 0, 3));

            Sub.Add("LvlStart", new Slider("Auto LVL start", 2, 1, 6));

            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!Program.LagFree(0) || !getCheckBoxItem("AutoLvl"))
                return;
            lvl1 = getSliderItem("1");
            lvl2 = getSliderItem("2");
            lvl3 = getSliderItem("3");
            lvl4 = getSliderItem("4");
        }

        private void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !getCheckBoxItem("AutoLvl") || ObjectManager.Player.Level < getSliderItem("LvlStart"))
                return;
            if (lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4)
                return;
            var delay = 700;
            Utility.DelayAction.Add(delay, () => Up(lvl1));
            Utility.DelayAction.Add(delay + 50, () => Up(lvl2));
            Utility.DelayAction.Add(delay + 100, () => Up(lvl3));
            Utility.DelayAction.Add(delay + 150, () => Up(lvl4));
        }


        private void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.Level == 1 && getCheckBoxItem("AutoLvl"))
            {
                if ((lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4) && (int) Game.Time%2 == 0)
                {
                    drawText("AutoLvlUp: PLEASE SET ABILITY SEQENCE", ObjectManager.Player.Position, Color.OrangeRed,
                        -200);
                }
            }
        }

        public static void drawText(string msg, Vector3 Hero, Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1] + weight, color, msg);
        }

        private void Up(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if (indx == 0 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }
    }
}