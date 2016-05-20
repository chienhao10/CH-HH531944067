using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

namespace NabbTracker
{
    using System.Linq;
    using SharpDX;

    /// <summary>
    ///     The drawings class.
    /// </summary>
    class Drawings
    {
        /// <summary>
        ///     Loads the range drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                foreach (var pg in HeroManager.AllHeroes.Where(e => e.IsHPBarRendered && (e.IsAlly && Variables.getCheckBoxItem(Variables.Menu, "allies") || e.IsEnemy && Variables.getCheckBoxItem(Variables.Menu, "enemies"))))
                {
                    for (int Spell = 0; Spell < Variables.SpellSlots.Count(); Spell++)
                    {
                        Variables.SpellX = (int)pg.HPBarPosition.X + (pg.ChampionName.Equals("Jhin") ? 15 : 10) + (Spell * 25);
                        Variables.SpellY = (int)pg.HPBarPosition.Y + (pg.ChampionName.Equals("Jhin") ? 25 : 35) + (pg.ChampionName.ToLower().Equals("velkoz") ? 20 : 0);

                        Variables.DisplayTextFont.DrawText(null, pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).CooldownExpires - Game.Time > 0 ? string.Format("{0:0}", pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).CooldownExpires - Game.Time) : Variables.SpellSlots[Spell].ToString(), Variables.SpellX, Variables.SpellY, pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).Level < 1 ? Color.Gray : pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).SData.ManaCostArray.MaxOrDefault((value) => value) > pg.Mana ? Color.Cyan : pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).CooldownExpires - Game.Time > 0 && pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).CooldownExpires - Game.Time <= 4 ? Color.Red : pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).CooldownExpires - Game.Time > 4 ? Color.Yellow : Color.LightGreen);

                        for (int DrawSpellLevel = 0; DrawSpellLevel <= pg.Spellbook.GetSpell(Variables.SpellSlots[Spell]).Level - 1; DrawSpellLevel++)
                        {
                            Variables.SpellLevelX = Variables.SpellX + (DrawSpellLevel * 3) - 4;
                            Variables.SpellLevelY = Variables.SpellY + 4;
                            Variables.DisplayTextFont.DrawText(null, ".", Variables.SpellLevelX, Variables.SpellLevelY, Color.White
                            );
                        }
                    }
                    
                    for (int SummonerSpell = 0; SummonerSpell < Variables.SummonerSpellSlots.Count(); SummonerSpell++)
                    {
                        Variables.SummonerSpellX = (int)pg.HPBarPosition.X + 10 + (SummonerSpell * 88);
                        Variables.SummonerSpellY = (int)pg.HPBarPosition.Y + (pg.ChampionName.Equals("Jhin") ? -6 : 4);

                        switch (pg.Spellbook.GetSpell(Variables.SummonerSpellSlots[SummonerSpell]).Name.ToLower())
                        {
                            case "summonerflash":        Variables.GetSummonerSpellName = "Flash";        break;
                            case "summonerdot":          Variables.GetSummonerSpellName = "Ignite";       break;
                            case "summonerheal":         Variables.GetSummonerSpellName = "Heal";         break;
                            case "summonerteleport":     Variables.GetSummonerSpellName = "Teleport";     break;
                            case "summonerexhaust":      Variables.GetSummonerSpellName = "Exhaust";      break;
                            case "summonerhaste":        Variables.GetSummonerSpellName = "Ghost";        break;
                            case "summonerbarrier":      Variables.GetSummonerSpellName = "Barrier";      break;
                            case "summonerboost":        Variables.GetSummonerSpellName = "Cleanse";      break;
                            case "summonermana":         Variables.GetSummonerSpellName = "Clarity";      break;
                            case "summonerclairvoyance": Variables.GetSummonerSpellName = "Clairvoyance"; break;
                            case "summonerodingarrison": Variables.GetSummonerSpellName = "Garrison";     break;
                            case "summonersnowball":     Variables.GetSummonerSpellName = "Mark";         break;
                            default:
                                Variables.GetSummonerSpellName = "Smite";
                                break;
                        }
                        
                        Variables.DisplayTextFont.DrawText(null, pg.Spellbook.GetSpell(Variables.SummonerSpellSlots[SummonerSpell]).CooldownExpires - Game.Time > 0 ? Variables.GetSummonerSpellName + ":" + string.Format("{0:0}", pg.Spellbook.GetSpell(Variables.SummonerSpellSlots[SummonerSpell]).CooldownExpires - Game.Time)  : Variables.GetSummonerSpellName + ": UP ", Variables.SummonerSpellX + (150 * ((SummonerSpell * (int)0.4) + 1)), Variables.SummonerSpellY + (SummonerSpell), pg.Spellbook.GetSpell(Variables.SummonerSpellSlots[SummonerSpell]).CooldownExpires - Game.Time > 0  ? Color.Red : Color.Yellow);
                    }
                }
            };
        }
    }
}
