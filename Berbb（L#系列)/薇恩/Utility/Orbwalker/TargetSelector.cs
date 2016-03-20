#region LICENSE

/*
 Copyright 2014 - 2014 LeagueSharp
 TargetSelector.cs is part of LeagueSharp.Common.
 
 LeagueSharp.Common is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 LeagueSharp.Common is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with LeagueSharp.Common. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

#region

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

#endregion

namespace LeagueSharp.Common
{
    public class TargetSelector
    {
        #region Enum

        public enum DamageType
        {
            Magical,
            Physical,
            True
        }

        public enum TargetingMode
        {
            AutoPriority,
            LowHP,
            MostAD,
            MostAP,
            Closest,
            NearMouse,
            LessAttack,
            LessCast,
            MostStack
        }

        #endregion

        #region Vars

        public static TargetingMode Mode = TargetingMode.AutoPriority;
        private static AIHeroClient _selectedTargetObjAiHero;

        private static bool UsingCustom;

        public static bool CustomTS
        {
            get { return UsingCustom; }
            set
            {
                UsingCustom = value;
                if (value)
                {
                    Drawing.OnDraw -= DrawingOnOnDraw;
                }
                else
                {
                    Drawing.OnDraw += DrawingOnOnDraw;
                }
            }
        }

        #endregion

        #region EventArgs

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (_selectedTargetObjAiHero.IsValidTarget() && focusMenu != null && FocusSelected)
            {
                Render.Circle.DrawCircle(_selectedTargetObjAiHero.Position, 150, Color.Orange, 7, true);
            }

            var a = (ForceFocusSelectedK || ForceFocusSelectedK2) && ForceFocusSelectedKeys;

            //focusMenu.Item("ForceFocusSelectedKeys").Permashow(SelectedTarget != null && a);
            //focusMenu.Item("ForceFocusSelected").Permashow(focusMenu.Item("ForceFocusSelected").GetValue<bool>());
        }

        private static void GameOnOnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            _selectedTargetObjAiHero =
                HeroManager.Enemies
                    .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
        }

        #endregion

        #region Functions

        public static AIHeroClient SelectedTarget
        {
            get
            {
                return (focusMenu != null && FocusSelected ? _selectedTargetObjAiHero : null);
            }
        }

        /// <summary>
        ///     Sets the priority of the hero
        /// </summary>
        public static void SetPriority(AIHeroClient hero, int newPriority)
        {
        }

        /// <summary>
        ///     Returns the priority of the hero
        /// </summary>
        public static float GetPriority(AIHeroClient hero)
        {
            /*
            var p = 1;
            if (focusMenu != null && focusMenu.Item("TargetSelector" + hero.ChampionName + "Priority") != null)
            {
                p = focusMenu.Item("TargetSelector" + hero.ChampionName + "Priority").GetValue<Slider>().Value;
            }

            switch (p)
            {
                case 2:
                    return 1.5f;
                case 3:
                    return 1.75f;
                case 4:
                    return 2f;
                case 5:
                    return 2.5f;
                default:
                    return 1f;
            }
            */
            return 2.5f;
        }

        private static int GetPriorityFromDb(string championName)
        {
            string[] p1 =
            {
                "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Taric", "TahmKench", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

            string[] p2 =
            {
                "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", "Gragas", "Irelia", "Jax",
                "Lee Sin", "Maokai", "Morgana", "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze", "Swain",
                "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao", "RekSai"
            };

            string[] p3 =
            {
                "Akali", "Diana", "Ekko", "Fiddlesticks", "Fiora", "Fizz", "Heimerdinger", "Jayce", "Kassadin",
                "Kayle", "Kha'Zix", "Lissandra", "Mordekaiser", "Nidalee", "Riven", "Shaco", "Vladimir", "Yasuo",
                "Zilean"
            };

            string[] p4 =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Kindred",
                "Leblanc", "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra",
                "Talon", "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "Velkoz", "Viktor",
                "Xerath", "Zed", "Ziggs", "Jhin", "Soraka"
            };

            if (p1.Contains(championName))
            {
                return 1;
            }
            if (p2.Contains(championName))
            {
                return 2;
            }
            if (p3.Contains(championName))
            {
                return 3;
            }
            return p4.Contains(championName) ? 4 : 1;
        }

        private static Menu Menu, focusMenu;

        public static bool FocusSelected { get { return focusMenu["FocusSelected"].Cast<CheckBox>().CurrentValue; } }
        public static bool ForceFocusSelected { get { return focusMenu["ForceFocusSelected"].Cast<CheckBox>().CurrentValue; } }
        public static bool ForceFocusSelectedKeys { get { return focusMenu["ForceFocusSelectedKeys"].Cast<CheckBox>().CurrentValue; } }

        public static bool ForceFocusSelectedK { get { return focusMenu["ForceFocusSelectedK"].Cast<KeyBind>().CurrentValue; } }
        public static bool ForceFocusSelectedK2 { get { return focusMenu["ForceFocusSelectedK2"].Cast<KeyBind>().CurrentValue; } }

        public static int getMode { get { return focusMenu["getMode"].Cast<Slider>().CurrentValue; } }

        internal static void Initialize()
        {
            CustomEvents.Game.OnGameLoad += args =>
            {
                Menu = MainMenu.AddMenu("Target Selector", "TargetSelector");

                focusMenu = Menu.AddSubMenu("Focus Target Settings", "FocusTargetSettings");

                focusMenu.Add("FocusSelected", new CheckBox("Focus selected target"));
                focusMenu.Add("ForceFocusSelected", new CheckBox("Only attack selected target", false));
                focusMenu.AddSeparator();
                focusMenu.Add("ForceFocusSelectedKeys", new CheckBox("Enable only attack selected Keys", false));

                focusMenu.Add("ForceFocusSelectedK", new KeyBind("Only attack selected Key", false, KeyBind.BindTypes.HoldActive, 32));
                focusMenu.Add("ForceFocusSelectedK2", new KeyBind("Only attack selected Key 2", false, KeyBind.BindTypes.HoldActive, 32));

                focusMenu.Add("getMode", new Slider("Targetting Mode", 8, 0, 9));

                //LowHP 1
                //MostAD 2
                //MostAP 3
                //Closest 4
                //NearMouse 5
                //AutoPriority 6
                //LessAttack 7
                //LessCast 8
                //MostStack 9

                Game.OnWndProc += GameOnOnWndProc;

                if (!CustomTS)
                {
                    Drawing.OnDraw += DrawingOnOnDraw;
                }
            };
        }

        public static void AddToMenu(Menu config)
        {
            config.AddLabel("----Use TS in Common Menu----");
        }

        public static bool IsInvulnerable(Obj_AI_Base target, DamageType damageType, bool ignoreShields = true)
        {
            //Kindred's Lamb's Respite(R)

            if (target.HasBuff("kindredrnodeathbuff") && target.HealthPercent <= 10)
            {
                return true;
            }

            // Tryndamere's Undying Rage (R)
            if (target.HasBuff("Undying Rage") && target.Health <= target.MaxHealth * 0.10f)
            {
                return true;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }

            if (ignoreShields)
            {
                return false;
            }

            // Morgana's Black Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BlackShield"))
            {
                return true;
            }

            // Banshee's Veil (PASSIVE)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return true;
            }

            // Sivir's Spell Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return true;
            }

            // Nocturne's Shroud of Darkness (W)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("ShroudofDarkness"))
            {
                // TODO: Get exact Nocturne's Shourd of Darkness buff name
                return true;
            }

            return false;
        }


        public static void SetTarget(AIHeroClient hero)
        {
            if (hero.IsValidTarget())
            {
                _selectedTargetObjAiHero = hero;
            }
        }

        public static AIHeroClient GetSelectedTarget()
        {
            return SelectedTarget;
        }

        public static AIHeroClient GetTarget(float range, DamageType damageType, bool ignoreShield = true, IEnumerable<AIHeroClient> ignoredChamps = null, Vector3? rangeCheckFrom = null)
        {
            return GetTarget(ObjectManager.Player, range, damageType, ignoreShield, ignoredChamps, rangeCheckFrom);
        }

        public static AIHeroClient GetTargetNoCollision(Spell spell, bool ignoreShield = true, IEnumerable<AIHeroClient> ignoredChamps = null, Vector3? rangeCheckFrom = null)
        {
            //var t = GetTarget(ObjectManager.Player, spell.Range, spell.DamageType, ignoreShield, ignoredChamps, rangeCheckFrom);

            //if (spell.Collision && spell.GetPrediction(t).Hitchance != HitChance.Collision)
            //{
                //return t;
            //}

            return null;
        }

        private static bool IsValidTarget(Obj_AI_Base target,
            float range,
            DamageType damageType,
            bool ignoreShieldSpells = true,
            Vector3? rangeCheckFrom = null)
        {
            return target.IsValidTarget() &&
                   target.Distance(rangeCheckFrom ?? ObjectManager.Player.ServerPosition, true) <
                   Math.Pow(range <= 0 ? Orbwalking.GetRealAutoAttackRange(target) : range, 2) &&
                   !IsInvulnerable(target, damageType, ignoreShieldSpells);
        }

        private static string[] StackNames =
            {
                "kalistaexpungemarker",
                "vaynesilvereddebuff",
                "twitchdeadlyvenom",
                "ekkostacks",
                "dariushemo",
                "gnarwproc",
                "tahmkenchpdebuffcounter",
                "varuswdebuff",
            };

        public static AIHeroClient GetTarget(Obj_AI_Base champion, float range, DamageType type, bool ignoreShieldSpells = true, IEnumerable<AIHeroClient> ignoredChamps = null, Vector3? rangeCheckFrom = null)
        {
            try
            {
                if (ignoredChamps == null)
                {
                    ignoredChamps = new List<AIHeroClient>();
                }

                var damageType = (Damage.DamageType)Enum.Parse(typeof(Damage.DamageType), type.ToString());

                if (focusMenu != null && IsValidTarget(SelectedTarget, ForceFocusSelected ? float.MaxValue : range, type, ignoreShieldSpells, rangeCheckFrom))
                {
                    return SelectedTarget;
                }

                if (focusMenu != null && IsValidTarget(SelectedTarget, ForceFocusSelectedKeys ? float.MaxValue : range, type, ignoreShieldSpells, rangeCheckFrom))
                {
                    if (ForceFocusSelectedK || ForceFocusSelectedK2)
                    {
                        return SelectedTarget;
                    }
                }

                var targets = HeroManager.Enemies.FindAll(hero => ignoredChamps.All(ignored => ignored.NetworkId != hero.NetworkId) && IsValidTarget(hero, range, type, ignoreShieldSpells, rangeCheckFrom));

                switch (getMode)
                {
                    case 1:
                        return targets.MinOrDefault(hero => hero.Health);

                    case 2:
                        return targets.MaxOrDefault(hero => hero.BaseAttackDamage + hero.FlatPhysicalDamageMod);

                    case 3:
                        return targets.MaxOrDefault(hero => hero.BaseAbilityDamage + hero.FlatMagicDamageMod);

                    case 4:
                        return targets.MinOrDefault(hero => (rangeCheckFrom.HasValue ? rangeCheckFrom.Value : champion.ServerPosition).Distance(hero.ServerPosition, true));

                    case 5:
                        return targets.MinOrDefault(hero => hero.Distance(Game.CursorPos, true));

                    case 6:
                        return targets.MaxOrDefault(hero => champion.CalcDamage(hero, damageType, 100) / (1 + hero.Health) * GetPriority(hero));

                    case 7:
                        return targets.MaxOrDefault(hero => champion.CalcDamage(hero, Damage.DamageType.Physical, 100) / (1 + hero.Health) * GetPriority(hero));

                    case 8:
                        return targets.MaxOrDefault(hero => champion.CalcDamage(hero, Damage.DamageType.Magical, 100) / (1 + hero.Health) * GetPriority(hero));

                    case 9:
                        return targets.MaxOrDefault(hero => champion.CalcDamage(hero, damageType, 100) / (1 + hero.Health) * GetPriority(hero) + (1 + hero.Buffs.Where(b => StackNames.Contains(b.Name.ToLower())).Sum(t => t.Count)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    ///     This TS attempts to always lock the same target, useful for people getting targets for each spell, or for champions
    ///     that have to burst 1 target.
    /// </summary>
    public class LockedTargetSelector
    {
        public static AIHeroClient _lastTarget;
        private static TargetSelector.DamageType _lastDamageType;

        public static AIHeroClient GetTarget(float range,
            TargetSelector.DamageType damageType,
            bool ignoreShield = true,
            IEnumerable<AIHeroClient> ignoredChamps = null,
            Vector3? rangeCheckFrom = null)
        {
            if (_lastTarget == null || !_lastTarget.IsValidTarget() || _lastDamageType != damageType)
            {
                var newTarget = TargetSelector.GetTarget(range, damageType, ignoreShield, ignoredChamps, rangeCheckFrom);

                _lastTarget = newTarget;
                _lastDamageType = damageType;

                return newTarget;
            }

            if (_lastTarget.IsValidTarget(range) && damageType == _lastDamageType)
            {
                return _lastTarget;
            }

            var newTarget2 = TargetSelector.GetTarget(range, damageType, ignoreShield, ignoredChamps, rangeCheckFrom);

            _lastTarget = newTarget2;
            _lastDamageType = damageType;

            return newTarget2;
        }

        /// <summary>
        ///     Unlocks the currently locked target.
        /// </summary>
        public static void UnlockTarget()
        {
            _lastTarget = null;
        }

        public static void AddToMenu(Menu menu)
        {
            TargetSelector.AddToMenu(menu);
        }
    }
}
