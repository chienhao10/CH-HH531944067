using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using System.Collections.Generic;
using LeagueSharp.Common;
using Color = SharpDX.Color;
using System.Windows.Input;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

namespace OlafxQx.Common
{
    internal class CommonAutoLevel
    {
        public static Menu MenuLocal;

        public static int[] SpellLevels;

        public static void Init(Menu parentMenu)
        {
            MenuLocal = parentMenu.AddSubMenu("Auto Level", "Auto Level");
            MenuLocal.Add("AutoLevel.Set", new ComboBox("at Start:", 2, "Always Off", "Always On", "Remember Last Settings"));
            MenuLocal.Add("AutoLevel.Active", new KeyBind("Auto Level Active!", false, KeyBind.BindTypes.PressToggle, 'L'));

            SpellLevels = new[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };

            switch (MenuLocal["AutoLevel.Set"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    MenuLocal["AutoLevel.Active"].Cast<KeyBind>().CurrentValue = false;
                    break;

                case 1:
                    MenuLocal["AutoLevel.Active"].Cast<KeyBind>().CurrentValue = true;
                    break;
            }

            Game.OnUpdate += Game_OnUpdate;
        }

        private static string GetLevelList(int[] spellLevels)
        {
            var a = new[] { "Q", "W", "E", "R" };
            var b = spellLevels.Aggregate("", (c, i) => c + (a[i - 1] + " - "));
            return b != "" ? b.Substring(0, b.Length - (17 * 3)) : "";
        }

        private static int GetRandomDelay
        {
            get
            {
                var rnd = new Random(DateTime.Now.Millisecond);
                return rnd.Next(750, 1000);
            }

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!MenuLocal["AutoLevel.Active"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            var qLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            var wLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
            var eLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level;
            var rLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;

            if (qLevel + wLevel + eLevel + rLevel >= ObjectManager.Player.Level)
            {
                return;
            }

            var level = new[] { 0, 0, 0, 0 };
            for (var i = 0; i < ObjectManager.Player.Level; i++)
            {
                level[SpellLevels[i] - 1] = level[SpellLevels[i] - 1] + 1;
            }

            if (qLevel < level[0])
            {

                LeagueSharp.Common.Utility.DelayAction.Add(GetRandomDelay, () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q));
            }

            if (wLevel < level[1])
            {
                LeagueSharp.Common.Utility.DelayAction.Add(GetRandomDelay, () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W));
            }

            if (eLevel < level[2])
            {
                LeagueSharp.Common.Utility.DelayAction.Add(GetRandomDelay, () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E));
            }

            if (rLevel < level[3])
            {
                LeagueSharp.Common.Utility.DelayAction.Add(GetRandomDelay, () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R));
            }
        }
    }
}