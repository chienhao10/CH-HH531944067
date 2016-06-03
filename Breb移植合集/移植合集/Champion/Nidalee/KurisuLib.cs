using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using CM = KurisuNidalee.CastManager;
using KN = KurisuNidalee.KurisuNidalee;
using Spell = LeagueSharp.Common.Spell;

namespace KurisuNidalee
{
    internal static class KurisuLib
    {
        internal static int Counter;
        internal static int MissileCount;
        internal static int LastAttack;
        internal static int LastBite, LastR;
        internal static AttackableUnit LastUnit;
        internal static SpellSlot Smite;
        internal static bool SmiteInGame;
        internal static AIHeroClient Player = ObjectManager.Player;

        public static Menu qHMenu = KN.qHMenu,
            wHMenu = KN.wHMenu,
            eHMenu = KN.eHMenu,
            rHMenu = KN.rHMenu,
            qCMenu = KN.qCMenu,
            wCMenu = KN.wCMenu,
            eCMenu = KN.eCMenu,
            rCMenu = KN.rCMenu,
            drawMenu = KN.drawMenu,
            jungleMenu = KN.jungleMenu,
            autoMenu = KN.autoMenu;

        /// <summary>
        ///     Stores the nidalee spells.
        /// </summary>
        internal static Dictionary<string, Spell> Spells = new Dictionary<string, Spell>
        {
            {"Takedown", new Spell(SpellSlot.Q, 400f)},
            {"Pounce", new Spell(SpellSlot.W, 375f)},
            {"ExPounce", new Spell(SpellSlot.W, 740f)},
            {"Swipe", new Spell(SpellSlot.E, 300f)},
            {"Javelin", new Spell(SpellSlot.Q, 1500f)},
            {"Bushwhack", new Spell(SpellSlot.W, 875f)},
            {"Primalsurge", new Spell(SpellSlot.E, 600f)},
            {"Aspect", new Spell(SpellSlot.R)}
        };


        /// <summary>
        ///     Stores when the last spell was used.
        /// </summary>
        internal static Dictionary<string, float> TimeStamp = new Dictionary<string, float>
        {
            {"Takedown", 0f},
            {"Pounce", 0f},
            {"Swipe", 0f},
            {"Javelin", 0f},
            {"Bushwhack", 0f},
            {"Primalsurge", 0f}
        };

        /// <summary>
        ///     Stores the current tickcount of the spell.
        /// </summary>
        internal static Dictionary<string, float> SpellTimer = new Dictionary<string, float>
        {
            {"Takedown", 0f},
            {"Pounce", 0f},
            {"ExPounce", 0f},
            {"Swipe", 0f},
            {"Javelin", 0f},
            {"Bushwhack", 0f},
            {"Primalsurge", 0f},
            {"Aspect", 0f}
        };

        /// <summary>
        ///     Entire neutral list.
        /// </summary>
        internal static readonly string[] MinionList =
        {
            // summoners rift
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", "SRU_RiftHerald",

            // twisted treeline
            "TT_NGolem5.1", "TT_NGolem2.1", "TT_NWolf6.1", "TT_NWolf3.1",
            "TT_NWraith4.1", "TT_NWraith1.1", "TT_Spiderboss"
        };

        /// <summary>
        ///     Epic minion list.
        /// </summary>
        internal static readonly string[] EpicList =
        {
            "TT_Spiderboss", "SRU_Baron", "SRU_Dragon", "SRU_RiftHerald"
        };

        /// <summary>
        ///     Small minion list.
        /// </summary>
        internal static readonly string[] SmallList =
        {
            "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "SRU_Gromp"
        };

        internal static readonly string[] LargeList =
        {
            "SRU_Blue", "SRU_Red", "TT_NWraith4.1", "TT_NWraith1.1", "TT_NGolem2.1", "TT_NGolem5.1", "TT_NWolf3.1",
            "TT_NWolf6.1"
        };

        static KurisuLib()
        {
            SetSpells();
            GetSmiteSlot();

            // Core
            Game.OnUpdate += SpellsOnUpdate;

            // Orbwalk shit
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;

            // Missile Handler
            GameObject.OnCreate += MissileClient_OnCreate;

            // Cast Handler
            Obj_AI_Base.OnProcessSpellCast += HeroOnCast;

            // Anti-Gapclosing
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            // wtf
            if (MissileCount > 0)
            {
                Counter += 1;
                MissileCount = 0;
                LastUnit = target;
                LastAttack = Utils.GameTimeTickCount;
            }
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (args.Target.Name.Contains("Mini") && KN.m)
                    args.Process = false;
            }
        }

        /// <summary>
        ///     Swipe/Javelin on gapclosers
        /// </summary>
        /// <param name="gapcloser"></param>
        internal static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var attacker = gapcloser.Sender;
            if (attacker.IsValidTarget(275f) && !Player.UnderTurret(true))
            {
                if (CatForm())
                {
                    CM.CastSwipe(attacker, "gap");
                    CM.SwitchForm(attacker, "gap");
                }
                else
                {
                    CM.CastJavelin(attacker, "gap");
                    CM.SwitchForm(attacker, "gap");
                }
            }
        }

        /// <summary>
        ///     Checks if a auto attack missile has been created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal static void MissileClient_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null && missile.SpellCaster.IsMe)
            {
                if (missile.SData.IsAutoAttack())
                    MissileCount += 1;
            }
        }

        /// <summary>
        ///     Returns true if the spell is ready via game time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        internal static bool IsReady(this float time, float extra = 0f)
        {
            return time < 1 + extra;
        }

        /// <summary>
        ///     Returns true if the unit is Q marked.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        internal static bool IsHunted(this Obj_AI_Base unit)
        {
            return unit.HasBuff("nidaleepassivehunted");
        }

        /// <summary>
        ///     Returns true if the unit is rooted (Prowl passive)
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        internal static bool PassiveRooted(this Obj_AI_Base unit)
        {
            return unit.IsValid<Obj_AI_Minion>() && unit.HasBuff("nidaleepassivemonsterroot");
        }

        /// <summary>
        ///     Returns when Nidalee can properly use a spell.
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="human"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static bool CanUse(Spell spell, bool human, string mode)
        {
            var menua = KN.Root;

            #region Human

            if (human && spell.Slot == SpellSlot.Q)
            {
                menua = KN.qHMenu;
            }

            if (human && spell.Slot == SpellSlot.W)
            {
                menua = KN.wHMenu;
            }

            if (human && spell.Slot == SpellSlot.E)
            {
                menua = KN.eHMenu;
            }

            if (human && spell.Slot == SpellSlot.R)
            {
                menua = KN.rHMenu;
            }

            #endregion

            #region Cougar

            if (!human && spell.Slot == SpellSlot.Q)
            {
                menua = KN.qCMenu;
            }

            if (!human && spell.Slot == SpellSlot.W)
            {
                menua = KN.wCMenu;
            }

            if (!human && spell.Slot == SpellSlot.E)
            {
                menua = KN.eCMenu;
            }

            if (!human && spell.Slot == SpellSlot.R)
            {
                menua = KN.rCMenu;
            }

            if (mode == "gap" || mode == "imm")
            {
                menua = KN.autoMenu;
            }

            #endregion

            var p = human ? "h" : "c";

            var enabled = menua["nd" + p + spell.Slot.ToString().ToLower() + mode.ToLower()].Cast<CheckBox>().CurrentValue;

            var learned = !(spell.Level <= 0 || Player.Spellbook.GetSpell(spell.Slot).State == SpellState.NotLearned);

            var n = human && spell.Slot == SpellSlot.Q
                ? "Javelin"
                : (human && spell.Slot == SpellSlot.W
                    ? "Bushwhack"
                    : human && spell.Slot == SpellSlot.E
                        ? "Primalsurge"
                        : (!human && spell.Slot == SpellSlot.Q
                            ? "Takedown"
                            : (!human && spell.Slot == SpellSlot.W ? "Pounce" : (!human && spell.Slot == SpellSlot.E ? "Swipe" : "Aspect"))));

            return enabled && learned && SpellTimer[n].IsReady();
        }

        /// <summary>
        ///     Checks if Nidalee is in Cougar form or not.
        /// </summary>
        /// <returns></returns>
        internal static bool CatForm()
        {
            return Player.CharData.BaseSkinName != "Nidalee";
        }

        /// <summary>
        ///     Returns the available cougar damage dealt to the target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static float CatDamage(Obj_AI_Base target)
        {
            var damage = 0d;

            if (SpellTimer["Takedown"].IsReady())
                damage += Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
            if (SpellTimer["Pounce"].IsReady())
                damage += Player.LSGetSpellDamage(target, SpellSlot.W, 1);
            if (SpellTimer["Swipe"].IsReady())
                damage += Player.LSGetSpellDamage(target, SpellSlot.E, 1);

            return (float) (damage + Player.GetAutoAttackDamage(target)*2);
        }

        /// <summary>
        ///     Returns if the minion is all three types (epic, large, or small jungle creep)
        /// </summary>
        /// <param name="minion"></param>
        /// <returns></returns>
        public static bool IsJungleMinion(Obj_AI_Base minion)
        {
            return IsEpicMinion(minion) || IsLargeMinion(minion) || IsEpicMinion(minion);
        }

        /// <summary>
        ///     Returns if the minion is an "Epic" minion (baron, dragon, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <returns></returns>
        public static bool IsEpicMinion(Obj_AI_Base minion)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion &&
                   (name.StartsWith("SRU_Baron") || name.StartsWith("SRU_Dragon") ||
                    name.StartsWith("SRU_RiftHerald") || name.StartsWith("TT_Spiderboss"));
        }

        /// <summary>
        ///     Returns if the minion is a "Large" minion (Red Buff, Blue Buff, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <param name="notMini">Check if is mini. </param>
        /// <returns></returns>
        public static bool IsLargeMinion(Obj_AI_Base minion, bool notMini = true)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion && notMini && !minion.Name.Contains("Mini") &&
                   (name.StartsWith("SRU_Blue") || name.StartsWith("SRU_Red") || name.StartsWith("TT_NWraith1.1") ||
                    name.StartsWith("TT_NWraith4.1") || name.StartsWith("TT_NGolem2.1") ||
                    name.StartsWith("TT_NGolem5.1") ||
                    name.StartsWith("TT_NWolf3.1") || name.StartsWith("TT_NWolf6.1"));
        }

        /// <summary>
        ///     Returns if the minion is a "Small" minion (Razorbeak, Krug, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <param name="notMini">Check if is mini. </param>
        /// <returns></returns>
        public static bool IsSmallMinion(Obj_AI_Base minion, bool notMini = true)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion && notMini && !minion.Name.Contains("Mini") &&
                   (name.StartsWith("SRU_Murkwolf") || name.StartsWith("SRU_Razorbeak") ||
                    name.StartsWith("SRU_Gromp") || name.StartsWith("SRU_Krug"));
        }

        /// <summary>
        ///     Casts swipe to the best location (aoe)
        /// </summary>
        internal static void CastSmartSwipe()
        {
            var minionpositions =
                MinionManager.GetMinions(Player.ServerPosition, 500f, MinionTypes.All, MinionTeam.NotAllyForEnemy)
                    .Select(x => x.Position.To2D())
                    .ToList();

            var swipelocation = MinionManager.GetBestCircularFarmLocation(minionpositions, 275f, 375f);

            if (swipelocation.MinionsHit >= getSliderItem(eCMenu, "ndcenum"))
                if (SpellTimer["Swipe"].IsReady())
                    Spells["Swipe"].Cast(swipelocation.Position);
        }

        /// <summary>
        ///     Sets the nidalee spell data on load.
        /// </summary>
        internal static void SetSpells()
        {
            try
            {
                Spells["Javelin"].SetSkillshot(0.25f, 40f, 1300f, true, SkillshotType.SkillshotLine);
                Spells["Bushwhack"].SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                Spells["Swipe"].SetSkillshot(0.25f, (float) (15*Math.PI/180), float.MaxValue, false,
                    SkillshotType.SkillshotCone);
                Spells["Pounce"].SetSkillshot(0.50f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            }

            catch (Exception e)
            {
                Console.WriteLine("KL: SetSpellsException (" + e.Message + ")");
            }
        }


        /// <summary>
        ///     Updates the spell timers on update.
        /// </summary>
        /// <param name="args"></param>
        internal static void SpellsOnUpdate(EventArgs args)
        {
            SpellTimer["Takedown"] = TimeStamp["Takedown"] - Game.Time > 0
                ? TimeStamp["Takedown"] - Game.Time
                : 0;

            SpellTimer["Pounce"] = TimeStamp["Pounce"] - Game.Time > 0
                ? TimeStamp["Pounce"] - Game.Time
                : 0;

            SpellTimer["Swipe"] = TimeStamp["Swipe"] - Game.Time > 0
                ? TimeStamp["Swipe"] - Game.Time
                : 0;

            SpellTimer["Javelin"] = TimeStamp["Javelin"] - Game.Time > 0
                ? TimeStamp["Javelin"] - Game.Time
                : 0;

            SpellTimer["Bushwhack"] = TimeStamp["Bushwhack"] - Game.Time > 0
                ? TimeStamp["Bushwhack"] - Game.Time
                : 0;

            SpellTimer["Primalsurge"] = TimeStamp["Primalsurge"] - Game.Time > 0
                ? TimeStamp["Primalsurge"] - Game.Time
                : 0;
        }

        /// <summary>
        ///     Tracks the spells nidalee casts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void HeroOnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (getCheckBoxItem(wHMenu, "ndhwsp"))
                {
                    if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient && SpellTimer["Javelin"].IsReady())
                    {
                        if (args.End.IsValid() &&
                            args.End.LSDistance(KN.Player.ServerPosition) <= KN.Player.BoundingRadius*2)
                        {
                            var hero = sender as AIHeroClient;
                            if (hero == null || !hero.IsValidTarget(Spells["Bushwhack"].Range))
                            {
                                return;
                            }

                            CM.CastBushwhack(hero, "co");
                        }
                    }
                }

                if (sender.IsMe && args.SData.Name.ToLower() == "pounce")
                {
                    var unit = args.Target as Obj_AI_Base;
                    if (unit.IsValid<Obj_AI_Base>() && unit.IsHunted())
                        TimeStamp["Pounce"] = Game.Time + 1.5f;
                    else
                        TimeStamp["Pounce"] = Game.Time + (5 + 5*Player.PercentCooldownMod);
                }

                if (sender.IsMe && args.SData.Name.ToLower() == "swipe")
                    TimeStamp["Swipe"] = Game.Time + (5 + 5*Player.PercentCooldownMod);

                if (sender.IsMe && args.SData.Name.ToLower() == "primalsurge")
                    TimeStamp["Primalsurge"] = Game.Time + (12 + 12*Player.PercentCooldownMod);

                if (sender.IsMe && args.SData.Name.ToLower() == "bushwhack")
                {
                    var wperlevel = new[] {13, 12, 11, 10, 9}[Spells["Bushwhack"].Level - 1];
                    TimeStamp["Bushwhack"] = Game.Time + (wperlevel + wperlevel*Player.PercentCooldownMod);
                }

                if (sender.IsMe && args.SData.Name.ToLower() == "javelintoss")
                {
                    Counter = 0;
                    TimeStamp["Javelin"] = Game.Time + (6 + 6*Player.PercentCooldownMod);
                }

                if (sender.IsMe && args.SData.Name.ToLower() == "aspectofthecougar")
                {
                    LastR = Utils.GameTimeTickCount;
                }

                if (sender.IsMe && args.SData.Name.ToLower() == "aspectofthecougar" && CatForm())
                {
                    Counter = 0;
                    Orbwalker.ResetAutoAttack();
                }

                if (sender.IsMe && args.SData.Name.ToLower() == "aspectofthecougar" && !CatForm())
                {
                    Orbwalker.ResetAutoAttack();
                }

                if (sender.IsMe && args.SData.IsAutoAttack() && Player.HasBuff("Takedown"))
                {
                    LastBite = Utils.GameTimeTickCount;
                    TimeStamp["Takedown"] = Game.Time + (5 + 5*Player.PercentCooldownMod);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("KL: OnCastException (" + e.Message + ")");
            }
        }

        /// <summary>
        ///     Gets the correct smite slot.
        /// </summary>
        internal static void GetSmiteSlot()
        {
            if (Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner1;
            }

            if (Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner2;
            }
        }

        internal static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            return GetFirstWallPoint(from.To2D(), to.To2D(), step);
        }

        internal static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.LSDistance(to); d = d + step)
            {
                var testPoint = from + d*direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step)*direction;
                }
            }

            return null;
        }
    }
}