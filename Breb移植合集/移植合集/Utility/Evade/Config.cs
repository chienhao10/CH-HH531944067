// Copyright 2014 - 2014 Esk0r
// Config.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace EvadeSharp
{
    internal static class Config
    {
        public const bool PrintSpellData = false;
        public const bool TestOnAllies = false;
        public const int SkillShotsExtraRadius = 9;
        public const int SkillShotsExtraRange = 20;
        public const int GridSize = 10;
        public const int ExtraEvadeDistance = 15;
        public const int PathFindingDistance = 60;
        public const int PathFindingDistance2 = 35;

        public const int DiagonalEvadePointsCount = 7;
        public const int DiagonalEvadePointsStep = 20;

        public const int CrossingTimeOffset = 250;

        public const int EvadingFirstTimeOffset = 250;
        public const int EvadingSecondTimeOffset = 80;

        public const int EvadingRouteChangeTimeOffset = 250;

        public const int EvadePointChangeInterval = 300;
        public static int LastEvadePointChangeT = 0;

        public static Menu Menu, evadeSpells, skillShots, shielding, collision, drawings, misc;

        public static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("躲避", "Evade");
            Menu.Add("Enabled", new KeyBind("开启", true, KeyBind.BindTypes.PressToggle, 'K'));
            Menu.Add("OnlyDangerous", new KeyBind("只躲避危险的", false, KeyBind.BindTypes.HoldActive, 32));

            evadeSpells = Menu.AddSubMenu("技能躲避", "evadeSpells");
            foreach (var spell in EvadeSpellDatabase.Spells)
            {
                evadeSpells.AddGroupLabel(spell.Name);
                evadeSpells.Add("DangerLevel" + spell.Name, new Slider("危险等级", spell.DangerLevel, 5, 1));
                if (spell.IsTargetted && spell.ValidTargets.Contains(SpellValidTargets.AllyWards))
                {
                    evadeSpells.Add("WardJump" + spell.Name, new CheckBox("跳眼"));
                }
                evadeSpells.Add("Enabled" + spell.Name, new CheckBox("开启"));
                evadeSpells.AddSeparator();
            }

            skillShots = Menu.AddSubMenu("指向性技能", "Skillshots");
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team || Config.TestOnAllies)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (String.Equals(spell.ChampionName, hero.ChampionName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            skillShots.AddGroupLabel(spell.MenuItemName);
                            skillShots.Add("DangerLevel" + spell.MenuItemName, new Slider("危险等级", spell.DangerValue, 5, 1));
                            skillShots.Add("IsDangerous" + spell.MenuItemName, new CheckBox("为危险", spell.IsDangerous));
                            skillShots.Add("Draw" + spell.MenuItemName, new CheckBox("显示"));
                            skillShots.Add("Enabled" + spell.MenuItemName, new CheckBox("开启", !spell.DisabledByDefault));
                            skillShots.AddSeparator();
                        }
                    }
                }
            }

            shielding = Menu.AddSubMenu("队友护盾", "Shielding");
            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                if (ally.IsAlly && !ally.IsMe)
                {
                    shielding.Add("shield" + ally.ChampionName, new CheckBox("护盾 " + ally.ChampionName));
                }
            }

            collision = Menu.AddSubMenu("体碰碰撞", "Collision");
            collision.Add("MinionCollision", new CheckBox("小兵", false));
            collision.Add("HeroCollision", new CheckBox("英雄", false));
            collision.Add("YasuoCollision", new CheckBox("亚索风墙"));
            collision.Add("EnableCollision", new CheckBox("开启"));

            drawings = Menu.AddSubMenu("线圈", "Drawings");
            drawings.AddLabel("开启技能颜色 = 白");//.SetValue(Color.White));
            drawings.AddLabel("关闭技能颜色 = 红");//.SetValue(Color.Red));
            drawings.AddLabel("弹道颜色 = 青绿");//.SetValue(Color.LimeGreen));
            drawings.Add("Border", new Slider("线宽", 1, 1, 5));
            drawings.Add("EnableDrawings", new CheckBox("开启"));

            misc = Menu.AddSubMenu("杂项", "Misc");
            misc.Add("BlockSpells", new ComboBox("躲避时屏蔽技能", 1, "不", "只躲避危险的", "一直"));
            misc.Add("DisableFow", new CheckBox("屏蔽躲避战争迷雾中的", false));
            misc.Add("ShowEvadeStatus", new CheckBox("显示躲避状态", false));
            if (ObjectManager.Player.CharData.BaseSkinName == "Olaf")
            {
                misc.Add("DisableEvadeForOlafR", new CheckBox("奥拉弗R时自动屏蔽躲避"));
            }
        }
    }
}
