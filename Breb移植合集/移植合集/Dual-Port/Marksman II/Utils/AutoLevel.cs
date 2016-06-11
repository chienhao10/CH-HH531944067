using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

namespace Marksman.Utils
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using System.Linq;
    using System.Security.AccessControl;

    public class AutoLevel
    {
        public static Menu LocalMenu;

        public static int[] SpellLevels;

        public AutoLevel()
        {
            LocalMenu = Program.MenuActivator;
            LocalMenu.AddGroupLabel("Auto Level");
            LocalMenu.Add("AutoLevel.Set", new ComboBox("at Start:", 2, "Allways Off", "Allways On", "Remember Last Settings"));
            LocalMenu.Add("AutoLevel.Active", new CheckBox("Auto Level Active!"));
            
            var championName = ObjectManager.Player.ChampionName.ToLowerInvariant();
            
            switch (championName)
            {
                case "ashe":
                    SpellLevels = new int[] { 2, 1, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "caitlyn":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "corki":
                    SpellLevels = ObjectManager.Player.PercentMagicDamageMod
                                  > ObjectManager.Player.PercentPhysicalDamageMod
                                      ? new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 }
                                      : new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "draven":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    break;

                case "ezreal":
                    SpellLevels = ObjectManager.Player.PercentMagicDamageMod
                                  > ObjectManager.Player.PercentPhysicalDamageMod
                                      ? new int[] { 2, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 }
                                      : new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "graves":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "gnar":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "jinx":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "kalista":
                    SpellLevels = new int[] { 2, 3, 1, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "kindred":
                    SpellLevels = new int[] { 2, 1, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3 , 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "kogmaw":
                    SpellLevels = ObjectManager.Player.PercentMagicDamageMod
                                  > ObjectManager.Player.PercentPhysicalDamageMod
                                      ? new int[] { 2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 }
                                      : new int[] { 3, 2, 1, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "lucian":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "missfortune":
                    SpellLevels = new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "quinn":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "sivir":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "teemo":
                    SpellLevels = new int[] { 3, 1, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;
                    
                case "tristana":
                    SpellLevels = new int[] { 3, 2, 3, 1, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "twitch":
                    SpellLevels = new int[] { 3, 2, 3, 1, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "urgot":
                    SpellLevels = new int[] { 3, 1, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "vayne":
                    SpellLevels = new int[] { 1, 3, 2, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "varus":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //LocalMenu.Add("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;
            }

            switch (LocalMenu["AutoLevel.Set"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    LocalMenu["AutoLevel.Active"].Cast<CheckBox>().CurrentValue = false;
                    break;

                case 1:
                    LocalMenu["AutoLevel.Active"].Cast<CheckBox>().CurrentValue = true;
                    break;
            }

            Game.OnUpdate += Game_OnUpdate;
        }

        private static string GetLevelList(int[] spellLevels)
        {
            var a = new[] { "Q", "W", "E", "R" };
            var b = spellLevels.Aggregate("", (c, i) => c + (a[i - 1] + " - "));
            return b != "" ? b.Substring(0, b.Length - 2) : "";
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
            if (!LocalMenu["AutoLevel.Active"].Cast<CheckBox>().CurrentValue)
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

            var level = new int[] { 0, 0, 0, 0 };
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
