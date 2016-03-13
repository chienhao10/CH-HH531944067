using System;
using System.Collections.Generic;
using System.Linq;
using AdEvade.Config;
using AdEvade.Config.Controls;
using AdEvade.Data.Spells;
using AdEvade.Helpers;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using Spell = AdEvade.Data.Spells.Spell;

namespace AdEvade.Data.EvadeSpells
{
    class EvadeSpell
    {
        private static AIHeroClient MyHero { get { return ObjectManager.Player; } }

        public delegate void Callback();

        public static List<EvadeSpellData> EvadeSpells = new List<EvadeSpellData>();
        public static List<EvadeSpellData> ItemSpells = new List<EvadeSpellData>();
        public static EvadeCommand LastSpellEvadeCommand = new EvadeCommand { IsProcessed = true, Timestamp = EvadeUtils.TickCount };

        public static Menu Menu;
        public static Menu EvadeSpellMenu;

        public EvadeSpell(Menu mainMenu)
        {
            Menu = mainMenu;

            Game.OnUpdate += Game_OnGameUpdate;

            EvadeSpellMenu = Menu.AddSubMenu("Evade Spells", "EvadeSpells");

            LoadEvadeSpellList();
            Shop.OnBuyItem += delegate { CheckForItems(); };
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            CheckDashing();
        }

        public static void CheckDashing()
        {
            if (EvadeUtils.TickCount - LastSpellEvadeCommand.Timestamp < 250 && MyHero.IsDashing()
                && LastSpellEvadeCommand.EvadeSpellData.EvadeType == EvadeType.Dash)
            {
                LastSpellEvadeCommand.TargetPosition = Player.Instance.GetDashInfo().EndPos.To2D();
            }
        }

        private static void CheckForItems()
        {
            foreach (var spell in ItemSpells)
            {
                var hasItem = Items.HasItem((int)spell.ItemId);

                if (hasItem && !EvadeSpells.Exists(s => s.SpellName == spell.SpellName))
                {
                    EvadeSpells.Add(spell);

                    CreateEvadeSpellMenu(spell);
                }
            }
        }

        private static Menu CreateEvadeSpellMenu(EvadeSpellData spell)
        {

            string menuName = spell.Name + " (" + spell.SpellKey + ") Settings";

            if (spell.IsItem)
            {
                menuName = spell.Name + " Settings";
            }
            var evadeSpellConfig = new EvadeSpellConfigControl(EvadeSpellMenu, menuName, spell);

            return evadeSpellConfig.GetMenu();
        }

        public static SpellModes GetDefaultSpellMode(EvadeSpellData spell)
        {
            if ((int)spell.Dangerlevel > (int)SpellDangerLevel.High)
            {
                return SpellModes.Undodgeable;
            }

            return SpellModes.ActivationTime;
        }

        public static bool PreferEvadeSpell()
        {
            if (!Situation.ShouldUseEvadeSpell())
                return false;

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                if (!GameData.HeroInfo.ServerPos2D.InSkillShot(spell, GameData.HeroInfo.BoundingRadius))
                    continue;

                if (ActivateEvadeSpell(spell, true))
                {
                    return true;
                }
            }

            return false;
        }

        public static void UseEvadeSpell()
        {
            if (!Situation.ShouldUseEvadeSpell())
            {
                return;
            }

            //int posDangerlevel = EvadeHelper.CheckPosDangerLevel(ObjectCache.myHeroCache.serverPos2D, 0);

            if (EvadeUtils.TickCount - LastSpellEvadeCommand.Timestamp < 1000)
            {
                return;
            }
            //ConsoleDebug.WriteLineColor("Dodging Skill Shots by Evade Spell", ConsoleColor.Green);
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                if (ShouldActivateEvadeSpell(spell))
                {
                    if (ActivateEvadeSpell(spell))
                    {
                        AdEvade.SetAllUndodgeable();
                        return;
                    }
                }
            }

        }

        public static bool ActivateEvadeSpell(Spell spell, bool checkSpell = false)
        {
            var sortedEvadeSpells = EvadeSpells.OrderBy(s => s.Dangerlevel);

            var extraDelayBuffer = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
            float spellActivationTime = ConfigValue.SpellActivationTime.GetInt() + Game.Ping + extraDelayBuffer;

            if (ConfigValue.CalculateWindupDelay.GetBool())
            {
                var extraWindupDelay = AdEvade.LastWindupTime - EvadeUtils.TickCount;
                if (extraWindupDelay > 0)
                {
                    return false;
                }
            }

            foreach (var evadeSpell in sortedEvadeSpells)
            {
                var processSpell = true;
                if (!Config.Properties.GetEvadeSpell(evadeSpell.Name).Use ||
                    ((int) GetSpellDangerLevel(evadeSpell) > (int) spell.GetSpellDangerLevel()) ||
                    (!evadeSpell.IsItem && MyHero.Spellbook.CanUseSpell(evadeSpell.SpellKey) != SpellState.Ready)||
                    (evadeSpell.IsItem && !(Items.CanUseItem((int) evadeSpell.ItemId))) ||
                    (evadeSpell.CheckSpellName &&
                     MyHero.Spellbook.GetSpell(evadeSpell.SpellKey).Name != evadeSpell.SpellName))
                {
                    continue; //can't use spell right now               
                }


                float evadeTime, spellHitTime;
                spell.CanHeroEvade(MyHero, out evadeTime, out spellHitTime);

                float finalEvadeTime = (spellHitTime - evadeTime);

                if (checkSpell)
                {
                    var mode = Config.Properties.GetEvadeSpell(evadeSpell.Name).SpellMode;

                    switch (mode)
                    {
                        case SpellModes.Undodgeable:
                            continue;
                        case SpellModes.ActivationTime:
                            if (spellActivationTime < finalEvadeTime)
                            {
                                continue;
                            }
                            break;
                    }
                }
                else
                {
                    //if (ObjectCache.menuCache.cache[evadeSpell.name + "LastResort"].Cast<CheckBox>().CurrentValue)
                    if (evadeSpell.SpellDelay <= 50 && evadeSpell.EvadeType != EvadeType.Dash)
                    {
                        var path = MyHero.Path;
                        if (path.Length > 0)
                        {
                            var movePos = path[path.Length - 1].To2D();
                            var posInfo = EvadeHelper.CanHeroWalkToPos(movePos, GameData.HeroInfo.MoveSpeed, 0, 0);

                            if ((int) GetSpellDangerLevel(evadeSpell) > (int) posInfo.PosDangerLevel)
                            {
                                continue;
                            }
                        }
                    }
                }

                if (evadeSpell.EvadeType != EvadeType.Dash && spellHitTime > evadeSpell.SpellDelay + 100 + Game.Ping +
                    Config.Properties.GetInt(ConfigValue.ExtraPingBuffer))
                {
                    processSpell = false;

                    if (checkSpell == false)
                    {
                        continue;
                    }
                }

                if (evadeSpell.IsSpecial)
                {
                    if (evadeSpell.UseSpellFunc != null)
                        if (evadeSpell.UseSpellFunc(evadeSpell, processSpell))
                            return true;
                    continue;
                }

                if (evadeSpell.EvadeType == EvadeType.Blink)
                {
                    if (evadeSpell.CastType == CastType.Position)
                    {
                        var posInfo = EvadeHelper.GetBestPositionBlink();
                        if (posInfo != null)
                        {
                            CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell, posInfo.Position), processSpell);
                            //DelayAction.Add(50, () => myHero.IssueOrder(GameObjectOrder.MoveTo, posInfo.position.To3D()));
                            return true;
                        }
                    }
                    else if (evadeSpell.CastType == CastType.Target)
                    {
                        var posInfo = EvadeHelper.GetBestPositionTargetedDash(evadeSpell);
                        if (posInfo != null && posInfo.Target != null && posInfo.PosDangerLevel == 0)
                        {
                            CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell, posInfo.Target), processSpell);
                            //DelayAction.Add(50, () => myHero.IssueOrder(GameObjectOrder.MoveTo, posInfo.position.To3D()));
                            return true;
                        }
                    }
                }
                else if (evadeSpell.EvadeType == EvadeType.Dash)
                {
                    if (evadeSpell.CastType == CastType.Position)
                    {
                        var posInfo = EvadeHelper.GetBestPositionDash(evadeSpell);
                        if (posInfo != null && CompareEvadeOption(posInfo, checkSpell))
                        {
                            if (evadeSpell.IsReversed)
                            {
                                var dir = (posInfo.Position - GameData.HeroInfo.ServerPos2D).Normalized();
                                var range = GameData.HeroInfo.ServerPos2D.Distance(posInfo.Position);
                                var pos = GameData.HeroInfo.ServerPos2D - dir * range;

                                posInfo.Position = pos;
                            }

                            CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell, posInfo.Position), processSpell);
                            //DelayAction.Add(50, () => myHero.IssueOrder(GameObjectOrder.MoveTo, posInfo.position.To3D()));
                            return true;
                        }
                    }
                    else if (evadeSpell.CastType == CastType.Target)
                    {
                        var posInfo = EvadeHelper.GetBestPositionTargetedDash(evadeSpell);
                        if (posInfo != null && posInfo.Target != null && posInfo.PosDangerLevel == 0)
                        {
                            CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell, posInfo.Target), processSpell);
                            //DelayAction.Add(50, () => myHero.IssueOrder(GameObjectOrder.MoveTo, posInfo.position.To3D()));
                            return true;
                        }
                    }
                }
                else if (evadeSpell.EvadeType == EvadeType.WindWall)
                {
                    if (spell.HasProjectile() || evadeSpell.SpellName == "FioraW") //TODO: temp fix, don't have fiora :'(
                    {
                        var dir = (spell.StartPos - GameData.HeroInfo.ServerPos2D).Normalized();
                        var pos = GameData.HeroInfo.ServerPos2D + dir * 100;

                        CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell, pos), processSpell);
                        return true;
                    }
                }
                else if (evadeSpell.EvadeType == EvadeType.SpellShield)
                {
                    if (evadeSpell.IsItem)
                    {
                        CastEvadeSpell(() => Items.UseItem((int)evadeSpell.ItemId), processSpell);
                        return true;
                    }
                    else
                    {
                        if (evadeSpell.CastType == CastType.Target)
                        {
                            CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell, MyHero), processSpell);
                            return true;
                        }
                        else if (evadeSpell.CastType == CastType.Self)
                        {
                            CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell), processSpell);
                            return true;
                        }
                    }
                }
                else if (evadeSpell.EvadeType == EvadeType.MovementSpeedBuff)
                {
                    
                }
            }

            return false;
        }

        public static void CastEvadeSpell(Callback func, bool process = true)
        {
            if (process)
            {
                func();
            }
        }

        public static bool CompareEvadeOption(PositionInfo posInfo, bool checkSpell = false)
        {
            if (checkSpell)
            {
                if (posInfo.PosDangerLevel == 0)
                {
                    return true;
                }
            }

            return posInfo.IsBetterMovePos();
        }

        private static bool ShouldActivateEvadeSpell(Spell spell)
        {
            if (AdEvade.LastPosInfo == null)
                return false;

            if (ConfigValue.DodgeSkillShots.GetBool())
            {
                if (AdEvade.LastPosInfo.UndodgeableSpells.Contains(spell.SpellId)
                && GameData.HeroInfo.ServerPos2D.InSkillShot(spell, GameData.HeroInfo.BoundingRadius))
                {
                    return true;
                }
            }
            else
            {
                if (GameData.HeroInfo.ServerPos2D.InSkillShot(spell, GameData.HeroInfo.BoundingRadius))
                {
                    return true;
                }
            }


            /*float activationTime = Evade.Menu.SubMenu("MiscSettings").SubMenu("EvadeSpellMisc").Item("EvadeSpellActivationTime")
                .Cast<Slider>().CurrentValue + ObjectCache.gamePing;

            if (spell.spellHitTime != float.MinValue && activationTime > spell.spellHitTime - spell.evadeTime)
            {
                return true;
            }*/

            return false;
        }

        public static SpellDangerLevel GetSpellDangerLevel(EvadeSpellData spell)
        {
            return Config.Properties.GetEvadeSpell(spell.Name).DangerLevel;
        }

        private SpellSlot GetSummonerSlot(string spellName)
        {
            if (MyHero.Spellbook.GetSpell(SpellSlot.Summoner1).SData.Name == spellName)
            {
                return SpellSlot.Summoner1;
            }
            else if (MyHero.Spellbook.GetSpell(SpellSlot.Summoner2).SData.Name == spellName)
            {
                return SpellSlot.Summoner2;
            }

            return SpellSlot.Unknown;
        }

        private void LoadEvadeSpellList()
        {

            foreach (var spell in EvadeSpellDatabase.Spells.Where(
                s => (s.CharName == MyHero.ChampionName || s.CharName == Constants.AllChampions)))
            {

                if (spell.IsSummonerSpell)
                {
                    SpellSlot spellKey = GetSummonerSlot(spell.SpellName);
                    if (spellKey == SpellSlot.Unknown)
                    {
                        continue;
                    }
                    else
                    {
                        spell.SpellKey = spellKey;
                    }
                }

                if (spell.IsItem)
                {
                    ItemSpells.Add(spell);
                    continue;
                }

                if (spell.IsSpecial)
                {
                    SpecialEvadeSpell.LoadSpecialSpell(spell);
                }

                EvadeSpells.Add(spell);

                var newSpellMenu = CreateEvadeSpellMenu(spell);
            }

            EvadeSpells.Sort((a, b) => a.Dangerlevel.CompareTo(b.Dangerlevel));
        }
    }
}
