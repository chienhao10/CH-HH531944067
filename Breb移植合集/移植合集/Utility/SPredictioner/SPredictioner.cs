using System;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace SPredictioner
{
    public class SPredictioner
    {
        public static Spell[] Spells = { null, null, null, null };
        public static Menu Config;

        public static void Initialize()
        {
            #region Initialize Menu
            Config = MainMenu.AddMenu("SDK预判者", "sdkpaksldjaskdjlkasjdk");
            Config.Add("ENABLED", new CheckBox("开启"));
            Config.Add("SPREDHITC", new ComboBox("命中率", 1, "非常高", "高", "中"));
            Config.AddSeparator();
            #region Initialize Spells
            Config.AddGroupLabel("技能");
            foreach (var spell in SpellDatabase.Spells)
            {
                if (spell.ChampionName == ObjectManager.Player.CharData.BaseSkinName && Config[String.Format("{0}{1}", ObjectManager.Player.ChampionName, spell.Slot)] == null)
                {
                    Spells[(int)spell.Slot] = new Spell(spell.Slot, spell.Range);
                    Spells[(int)spell.Slot].SetSkillshot(spell.Delay / 1000f, spell.Radius, spell.MissileSpeed, spell.Collisionable, spell.Type);
                    Config.Add(String.Format("{0}{1}", ObjectManager.Player.ChampionName, spell.Slot), new CheckBox("Convert Spell " + spell.Slot.ToString()));
                }
            }
            #endregion
            #endregion

            #region Initialize Events
            Spellbook.OnCastSpell += EventHandlers.Spellbook_OnCastSpell;
            AIHeroClient.OnProcessSpellCast += EventHandlers.Obj_AI_Hero_OnProcessSpellCast;
            #endregion
        }
    }
}
