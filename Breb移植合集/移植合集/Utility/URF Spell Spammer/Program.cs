using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace URF_Spell_Spammer
{
    internal class Program
    {
        public static Menu Menu;
        public static AIHeroClient Player = ObjectManager.Player;

        public static void Game_OnGameLoad()
        {
            Menu = MainMenu.AddMenu("技能狂放(阿福快打)", "URF Spell Spammer");
            Menu.Add("Q", new KeyBind("Q", false, KeyBind.BindTypes.PressToggle, 'A'));
            Menu.Add("W", new KeyBind("W", false, KeyBind.BindTypes.PressToggle, 'S'));
            Menu.Add("E", new KeyBind("E", false, KeyBind.BindTypes.PressToggle, 'G'));

            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Menu["Q"].Cast<KeyBind>().CurrentValue)
                {
                    if (Player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready &&
                        Player.Spellbook.GetSpell(SpellSlot.Q).Level > 0 && !Player.LSIsRecalling() && !Player.IsChannelingImportantSpell())
                    {
                        Player.Spellbook.CastSpell(SpellSlot.Q);
                    }
                }
                if (Menu["W"].Cast<KeyBind>().CurrentValue)
                {
                    if (Player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready &&
                        Player.Spellbook.GetSpell(SpellSlot.W).Level > 0 && !Player.LSIsRecalling() && !Player.IsChannelingImportantSpell())
                    {
                        Player.Spellbook.CastSpell(SpellSlot.W);
                    }
                }
                if (Menu["E"].Cast<KeyBind>().CurrentValue)
                {
                    if (Player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready &&
                        Player.Spellbook.GetSpell(SpellSlot.E).Level > 0 && !Player.LSIsRecalling() && !Player.IsChannelingImportantSpell())
                    {
                        Player.Spellbook.CastSpell(SpellSlot.E);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}