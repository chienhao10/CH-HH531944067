// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kalista.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//             This program is free software: you can redistribute it and/or modify
//             it under the terms of the GNU General Public License as published by
//             the Free Software Foundation, either version 3 of the License, or
//             (at your option) any later version.
//   
//             This program is distributed in the hope that it will be useful,
//             but WITHOUT ANY WARRANTY; without even the implied warranty of
//             MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//             GNU General Public License for more details.
//   
//             You should have received a copy of the GNU General Public License
//             along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   An Assembly for <see cref="Kalista" /> okay
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace IKalista
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;
    using Color = System.Drawing.Color;
    using LeagueSharp.Common;

    using SPrediction;

    using Collision = LeagueSharp.Common.Collision;

    /// <summary>
    ///     An Assembly for <see cref="Kalista" /> okay
    /// </summary>
    public class Kalista
    {
        /// <summary>
        ///     The dictionary to call the Spell slot and the Spell Class
        /// </summary>
        public static readonly Dictionary<SpellSlot, LeagueSharp.Common.Spell> spells = new Dictionary<SpellSlot, LeagueSharp.Common.Spell>
                                                                   {
                                                                       { SpellSlot.Q, new LeagueSharp.Common.Spell(SpellSlot.Q, 1150) }, 
                                                                       { SpellSlot.W, new LeagueSharp.Common.Spell(SpellSlot.W, 5200) }, 
                                                                       { SpellSlot.E, new LeagueSharp.Common.Spell(SpellSlot.E, 950) }, 
                                                                       { SpellSlot.R, new LeagueSharp.Common.Spell(SpellSlot.R, 1200) }
                                                                   };

        public static float getEDamage(Obj_AI_Base target)
        {
            return spells[SpellSlot.E].GetDamage(target);
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Kalista" /> class
        /// </summary>
        public Kalista()
        {

            this.InitMenu();
            this.InitSpells();
            this.InitEvents();

            // SpriteHandler.LoadSprite();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The delegate for the On orb walking Event
        /// </summary>
        private delegate void OnOrbwalkingMode();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     TODO The has undying buff.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasUndyingBuff(AIHeroClient target)
        {
            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            //// Kindred's Lamb's Respite(R)
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "kindredrnodeathbuff"))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This is where the magic happens, we like to steal other peoples stuff.
        /// </summary>
        private void DoMobSteal()
        {

            var junglelMinions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[SpellSlot.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(
                        x =>
                        Extensions.IsRendKillable(x) && !x.Name.Contains("Mini")
                        && !x.Name.Contains("Dragon") && !x.Name.Contains("Baron"));

            var bigMinions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[SpellSlot.E].Range,
                    MinionTypes.All,
                    MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(
                        x =>
                        Extensions.IsRendKillable(x)
                        && (x.BaseSkinName.ToLower().Contains("siege") || x.BaseSkinName.ToLower().Contains("super")));

            var baron =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[SpellSlot.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValid && Extensions.IsRendKillable(x) && x.Name.Contains("Baron"));

            var dragon =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[SpellSlot.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValid && Extensions.IsRendKillable(x) && x.Name.Contains("Dragon"));

            switch (getBoxItem(miscMenu, "jungStealMode"))
            {
                case 0: // jungle mobs
                    if ((junglelMinions != null && spells[SpellSlot.E].CanCast(junglelMinions) && Extensions.IsRendKillable(junglelMinions)) || (baron != null && spells[SpellSlot.E].CanCast(baron) && Extensions.IsRendKillable(baron)) || (dragon != null && spells[SpellSlot.E].CanCast(dragon) && Extensions.IsRendKillable(dragon)))
                    {
                        spells[SpellSlot.E].Cast();
                    }

                    break;

                case 1: // siege and super
                    if (bigMinions != null && Extensions.IsRendKillable(bigMinions))
                    {
                        spells[SpellSlot.E].Cast();
                    }

                    break;

                case 2: // both
                    if ((junglelMinions != null && spells[SpellSlot.E].CanCast(junglelMinions) && Extensions.IsRendKillable(bigMinions))
                        || (baron != null && spells[SpellSlot.E].CanCast(baron) && Extensions.IsRendKillable(baron))
                        || (dragon != null && spells[SpellSlot.E].CanCast(dragon) && Extensions.IsRendKillable(dragon))
                        || (bigMinions != null && spells[SpellSlot.E].CanCast(bigMinions) && Extensions.IsRendKillable(bigMinions)))
                    {
                        spells[SpellSlot.E].Cast();
                    }

                    break;
            }
        }

        /// <summary>
        ///     Do Wall Flee
        /// </summary>
        private void DoWallFlee()
        {
            if (!spells[SpellSlot.Q].IsReady() || !getKeyBindItem(miscMenu, "fleeKey") || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                return;
            }

            const float JumpRange = 250f;
            var extendedPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, JumpRange);
            if (this.IsOverWall(ObjectManager.Player.ServerPosition, extendedPosition) && !extendedPosition.IsWall())
            {
                spells[SpellSlot.Q].Cast(extendedPosition);
            }
        }

        /// <summary>
        ///     Gets the collision minions
        /// </summary>
        /// <param name="source">
        ///     the source
        /// </param>
        /// <param name="targetPosition">
        ///     the target position
        /// </param>
        /// <returns>
        ///     The list of minions
        /// </returns>
        private IEnumerable<Obj_AI_Base> GetCollisionMinions(Obj_AI_Base source, Vector3 targetPosition)
        {
            var input = new PredictionInput
                            {
                                Unit = source,
                                Radius = spells[SpellSlot.Q].Width,
                                Delay = spells[SpellSlot.Q].Delay,
                                Speed = spells[SpellSlot.Q].Speed
                            };

            input.CollisionObjects[0] = CollisionableObjects.Minions;

            return
                Collision.GetCollision(new List<Vector3> { targetPosition }, input)
                    .OrderBy(obj => obj.LSDistance(source))
                    .ToList();
        }


        /// <summary>
        ///     Handles the grab
        /// </summary>
        private void HandleBalista()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            var blitzcrank = HeroManager.Allies.FirstOrDefault(x => x.IsAlly && x.ChampionName == "Blitzcrank" && ObjectManager.Player.LSDistance(x.ServerPosition) < getSliderItem(balistaMenu, "maxRange") && ObjectManager.Player.LSDistance(x.ServerPosition) >= getSliderItem(balistaMenu, "minRange"));

            if (blitzcrank != null)
            {
                foreach (var target in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(enem => enem.IsValid && enem.IsEnemy && enem.LSDistance(ObjectManager.Player) <= 2450f))
                {
                    if (getCheckBoxItem(balistaMenu, "disable" + target.ChampionName) || !spells[SpellSlot.R].IsReady()
                        || !getCheckBoxItem(balistaMenu, "useBalista"))
                    {
                        return;
                    }

                    if (target.Buffs != null && target.GetTotalHealth() > 200 && blitzcrank.LSDistance(target) > 450f)
                    {
                        for (var i = 0; i < target.Buffs.Count(); i++)
                        {
                            if (target.Buffs[i].Name == "rocketgrab2" && target.Buffs[i].IsActive)
                            {
                                spells[SpellSlot.R].Cast();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the Sentinel trick
        /// </summary>
        private void HandleSentinels()
        {
            var baronPosition = new Vector3(4944, 10388, -712406f);
            var dragonPosition = new Vector3(9918f, 4474f, -71.2406f);

            if (!spells[SpellSlot.W].IsReady())
            {
                return;
            }

            if (getKeyBindItem(miscMenu, "sentBaron")
                && ObjectManager.Player.LSDistance(baronPosition) <= spells[SpellSlot.W].Range)
            {
                spells[SpellSlot.W].Cast(baronPosition);
            }
            else if (getKeyBindItem(miscMenu, "sentDragon")
                     && ObjectManager.Player.LSDistance(dragonPosition) <= spells[SpellSlot.W].Range)
            {
                spells[SpellSlot.W].Cast(dragonPosition);
            }
        }

        /// <summary>
        ///     Initialize all the events
        /// </summary>
        private void InitEvents()
        {
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = Extensions.GetRendDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;

            CustomDamageIndicator.Initialize(Extensions.GetRendDamage);

            Game.OnUpdate += this.OnUpdate;

            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpell;

            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;

            Drawing.OnDraw += args =>
                {
                    if (getCheckBoxItem(drawMenu, "drawQ"))
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[SpellSlot.Q].Range, Color.FromArgb(150, Color.Red));
                    }

                    if (getCheckBoxItem(drawMenu, "drawE"))
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[SpellSlot.E].Range, Color.FromArgb(150, Color.Red));
                    }

                    CustomDamageIndicator.DrawingColor = Color.FromArgb(150, Color.LawnGreen);

                    if (getCheckBoxItem(drawMenu, "drawPercentage"))
                    {
                        foreach (var source in HeroManager.Enemies.Where(x => ObjectManager.Player.LSDistance(x) <= 2000f && !x.IsDead))
                        {
                            var currentPercentage = Extensions.GetRendDamage(source) * 100 / source.GetTotalHealth();
                            var updatedCurrentPercentage = (int)Math.Ceiling(currentPercentage);

                            Drawing.DrawText(
                                Drawing.WorldToScreen(source.Position)[0],
                                Drawing.WorldToScreen(source.Position)[1],
                                currentPercentage >= 100 ? Color.Gold : Color.White,
                                currentPercentage >= 100
                                    ? "Killable With E"
                                    : "Current Damage: " + updatedCurrentPercentage + "%");
                        }
                    }

                    if (getCheckBoxItem(drawMenu, "drawJunglePercentage"))
                    {
                        foreach (var jungleMobs in
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => ObjectManager.Player.LSDistance(x) <= spells[SpellSlot.E].Range && !x.IsDead
                                && x.Team == GameObjectTeam.Neutral))
                        {
                            var currentPercentage = Extensions.GetRendDamage(jungleMobs) * 100 / jungleMobs.GetTotalHealth();
                            var updatedCurrentPercentage = (int)Math.Ceiling(currentPercentage);

                            var changeby = 40;

                            if (updatedCurrentPercentage >= 0)
                            {
                                switch (jungleMobs.CharData.BaseSkinName)
                                {
                                    case "SRU_Razorbeak":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X + 50, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "SRU_Red":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "SRU_Blue":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "SRU_Dragon_Air":
                                    case "SRU_Dragon_Water":
                                    case "SRU_Dragon_Elder":
                                    case "SRU_Dragon_Fire":
                                    case "SRU_Dragon_Earth":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "SRU_Baron":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "SRU_Gromp":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "SRU_Krug":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X + 53, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "SRU_Murkwolf":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X + 50, jungleMobs.HPBarPosition.Y + changeby, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                    case "Sru_Crab":
                                        Drawing.DrawText(jungleMobs.HPBarPosition.X + 50, jungleMobs.HPBarPosition.Y + 20, Color.GreenYellow,
                                            string.Format("{0}%", updatedCurrentPercentage));
                                        break;
                                }
                            }
                        }
                    }

                };
        }

        void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            var killableMinion = target as Obj_AI_Base;
            if (killableMinion == null || !spells[SpellSlot.E].IsReady())
            {
                return;
            }

            if (getCheckBoxItem(laneClearMenu, "eUnkillable") && Extensions.GetRendDamage(killableMinion) > killableMinion.GetTotalHealth() + 10 && spells[SpellSlot.E].CanCast(killableMinion) && killableMinion.HasBuff("KalistaExpungeMarker"))
            {
                spells[SpellSlot.E].Cast();
            }
        }

        /// <summary>
        ///     The Initialization
        /// </summary>
        private void InitializeBalista()
        {
            if (!HeroManager.Allies.Any(x => x.IsAlly && !x.IsMe && x.ChampionName == "Blitzcrank"))
            {
                return;
            }

            balistaMenu = menu.AddSubMenu("Balista");
            balistaMenu.AddGroupLabel("Disabled Targets");
            foreach (var hero in HeroManager.Enemies.Where(x => x.IsValid))
            {
                balistaMenu.Add("disable" + hero.ChampionName, new CheckBox("Disable " + hero.ChampionName, false));
            }

            balistaMenu.Add("minRange", new Slider("Min Range", 700, 100, 1450));
            balistaMenu.Add("maxRange", new Slider("Max Range", 1500, 100, 1500));
            balistaMenu.Add("useBalista", new CheckBox("Use Balista"));
        }

        /// <summary>
        ///     Calling the menu wrapper
        /// </summary>
        public static Menu menu, harassMenu, laneClearMenu, balistaMenu, miscMenu, drawMenu;
        public static Menu comboMenu;

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

        /// <summary>
        ///     Initialize the menu
        /// </summary>
        private void InitMenu()
        {
            menu = MainMenu.AddMenu("iKalista:Rework", "Kalista");

            comboMenu = menu.AddSubMenu("Combo Options");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useQMin", new CheckBox("Q > Minon Combo"));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("eLeaving", new CheckBox("Auto E Leaving"));
            comboMenu.Add("ePercent", new Slider("Min Percent to E Leaving", 50, 10, 100));
            comboMenu.Add("minStacks", new Slider("Min Stacks E", 10, 5, 20));
            comboMenu.Add("eDeath", new CheckBox("E Before Death"));
            comboMenu.Add("eDeathC", new Slider("E Death Damage %", 20, 10, 100));
            comboMenu.Add("eHealth", new Slider("Health To E Death %", 15, 5, 50));
            comboMenu.Add("saveAllyR", new CheckBox("Save Ally with R"));
            comboMenu.Add("allyPercent", new Slider("Save Ally Percentage", 20, 0, 100));
            comboMenu.Add("saveManaR", new CheckBox("Save Mana For Ultimate"));

            harassMenu = menu.AddSubMenu("Harass Options");
            harassMenu.Add("useQH", new CheckBox("Use Q"));
            harassMenu.Add("useEH", new CheckBox("Use E"));
            harassMenu.Add("harassStacks", new Slider("Min Stacks for E", 6, 2, 15));
            harassMenu.Add("useEMin", new CheckBox("Use Minion Harass"));

            laneClearMenu = menu.AddSubMenu("Laneclear Options");
            laneClearMenu.Add("useQLC", new CheckBox("Use Q"));
            laneClearMenu.Add("minHitQ", new Slider("Q Minions Killed", 3, 1, 7));
            laneClearMenu.Add("useELC", new CheckBox("Use E"));
            laneClearMenu.Add("minLC", new CheckBox("Minion Harass"));
            laneClearMenu.Add("eUnkillable", new CheckBox("E Unkillable Minions"));
            laneClearMenu.Add("qKillable", new CheckBox("Q Unkillable if no buff"));
            laneClearMenu.Add("eHit", new Slider("Min Minions E", 4, 2, 10));

            this.InitializeBalista();

            miscMenu = menu.AddSubMenu("Misc Options");
            miscMenu.Add("reduceE", new Slider("Reduce Rend Damage", 50, 0, 300));
            miscMenu.Add("fleeKey", new KeyBind("Flee Key", false, KeyBind.BindTypes.HoldActive, 'G'));
            miscMenu.Add("useJungleSteal", new CheckBox("Enabled Jungle Steal"));
            miscMenu.Add("jungStealMode", new ComboBox("Steal Mode", 0, "Jungle Mobs", "Siege Minions | Super Minions", "Both"));
            miscMenu.Add("qMana", new CheckBox("Save Mana For E"));
            miscMenu.Add("sentBaron", new KeyBind("Sentinel Baron", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("sentDragon", new KeyBind("Sentinel Dragon", false, KeyBind.BindTypes.HoldActive, 'Y'));
            miscMenu.Add("autoTrinket", new CheckBox("Auto Blue Trinket"));
            miscMenu.Add("exploit", new CheckBox("Exploit ? ", false));

            drawMenu = menu.AddSubMenu("Drawing Options");
            drawMenu.Add("drawSprite", new CheckBox("Draw Sprite for W"));
            drawMenu.Add("drawPercentage", new CheckBox("Draw Percentage Damage"));
            drawMenu.Add("drawJunglePercentage", new CheckBox("Draw Jungle Percentage Damage"));
            drawMenu.Add("drawQ", new CheckBox("Draw Q Range", true));
            drawMenu.Add("drawE", new CheckBox("Draw E Range", true));

            SPrediction.Prediction.Initialize();
        }

        /// <summary>
        ///     Initialize the spells
        /// </summary>
        private void InitSpells()
        {
            spells[SpellSlot.Q].SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);
            spells[SpellSlot.R].SetSkillshot(0.50f, 1500f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        /// <summary>
        ///     Determines if the end point is over a wall
        /// </summary>
        /// <param name="start">
        ///     Start point
        /// </param>
        /// <param name="end">
        ///     End Point
        /// </param>
        /// <returns>
        ///     If the End point is over a wall
        /// </returns>
        private bool IsOverWall(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i).To2D();
                if (tempPosition.IsWall())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Kill steal
        /// </summary>
        private void KillstealQ()
        {
            foreach (var source in EntityManager.Heroes.Enemies.Where(x => spells[SpellSlot.E].IsInRange(x) && Extensions.IsRendKillable(x)))
            {
                if (source.IsValidTarget(spells[SpellSlot.E].Range) && !HasUndyingBuff(source) && Extensions.IsRendKillable(source))
                {
                    spells[SpellSlot.E].Cast();
                    spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
                }
            }
        }

        /// <summary>
        ///     Perform the combo
        /// </summary>
        private void OnCombo()
        {
            if (getCheckBoxItem(miscMenu, "exploit") && ObjectManager.Player.AttackDelay / 1 > 1.70)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)))
                {
                    if (Game.Time * 1000 >= Orbwalker.LastAutoAttack + 1)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                    if (Game.Time * 1000 > Orbwalker.LastAutoAttack + ObjectManager.Player.AttackDelay * 1000 - 150)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                    }
                }
            }

            if (spells[SpellSlot.Q].IsReady() && getCheckBoxItem(comboMenu, "useQ") && !ObjectManager.Player.IsDashing()
                && !Orbwalker.IsAutoAttacking)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[SpellSlot.Q].Range)))
                {
                    var prediction = spells[SpellSlot.Q].GetSPrediction(enemy);
                    if (prediction.HitChance >= LeagueSharp.Common.HitChance.High)
                    {
                        spells[SpellSlot.Q].Cast(enemy);
                    }
                    else if (prediction.HitChance == LeagueSharp.Common.HitChance.Collision)
                    {
                        this.QCollisionCheck(enemy);
                    }
                }
            }
            if (spells[SpellSlot.E].IsReady() && getCheckBoxItem(comboMenu, "useE"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[SpellSlot.E].Range) && x.HasBuff("KalistaExpungeMarker")))
                {
                    var stacks = enemy.GetBuffCount("kalistaexpungemarker");
                    var damage = Math.Ceiling(Extensions.GetRendDamage(enemy) * 100 / enemy.GetTotalHealth());

                    if (getCheckBoxItem(comboMenu, "eLeaving") && damage >= getSliderItem(comboMenu, "ePercent") && enemy.HealthPercent > 20 && enemy.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition, true) > Math.Pow(spells[SpellSlot.E].Range * 0.8, 2) && Environment.TickCount - spells[SpellSlot.E].LastCastAttemptT > 500)
                    {
                        spells[SpellSlot.E].Cast();
                        spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
                    }
                }
            }
        }

        /// <summary>
        ///     TODO The on flee.
        /// </summary>
        private void OnFlee()
        {
            var bestTarget =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(x => x.IsEnemy && ObjectManager.Player.LSDistance(x) <= Orbwalking.GetRealAutoAttackRange(x))
                    .OrderBy(x => ObjectManager.Player.LSDistance(x))
                    .FirstOrDefault();

            // ReSharper disable once ConstantNullCoalescingCondition
            Orbwalker.OrbwalkTo(Game.CursorPos);
            this.DoWallFlee();
        }

        /// <summary>
        ///     Perform the harass function
        /// </summary>
        private void OnHarass()
        {
            var spearTarget = TargetSelector.GetTarget(
                spells[SpellSlot.Q].Range,
                DamageType.Physical);
            if (getCheckBoxItem(harassMenu, "useQH") && spells[SpellSlot.Q].IsReady() && !Orbwalker.IsAutoAttacking && !ObjectManager.Player.IsDashing())
            {
                if (getCheckBoxItem(miscMenu, "qMana") && ObjectManager.Player.Mana < spells[SpellSlot.Q].Instance.SData.Mana + spells[SpellSlot.E].Instance.SData.Mana && spells[SpellSlot.Q].GetDamage(spearTarget) < spearTarget.GetTotalHealth())
                {
                    return;
                }

                if (getCheckBoxItem(comboMenu, "saveManaR") && spells[SpellSlot.R].IsReady()
                    && ObjectManager.Player.Mana
                    < spells[SpellSlot.Q].Instance.SData.Mana + spells[SpellSlot.R].Instance.SData.Mana)
                {
                    return;
                }

                foreach (var unit in
                    HeroManager.Enemies.Where(x => x.IsValidTarget(spells[SpellSlot.Q].Range))
                        .Where(unit => spells[SpellSlot.Q].GetSPrediction(unit).HitChance == LeagueSharp.Common.HitChance.Immobile))
                {
                    spells[SpellSlot.Q].Cast(unit);
                }

                var prediction = spells[SpellSlot.Q].GetSPrediction(spearTarget);
                if (!Orbwalker.IsAutoAttacking && !ObjectManager.Player.IsDashing())
                {
                    switch (prediction.HitChance)
                    {
                        case LeagueSharp.Common.HitChance.Collision:
                            this.QCollisionCheck(spearTarget);
                            break;
                        case LeagueSharp.Common.HitChance.High:
                        case LeagueSharp.Common.HitChance.VeryHigh:
                            spells[SpellSlot.Q].Cast(spearTarget);
                            break;
                    }
                }
            }

            if (getCheckBoxItem(harassMenu, "useEH"))
            {
                var rendTarget =
                    HeroManager.Enemies.Where(
                        x =>
                        x.IsValidTarget(spells[SpellSlot.E].Range) && Extensions.GetRendDamage(x) >= 1
                        && !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield))
                        .OrderByDescending(x => Extensions.GetRendDamage(x))
                        .FirstOrDefault();

                if (rendTarget != null)
                {
                    var stackCount = rendTarget.GetBuffCount("kalistaexpungemarker");
                    if (Extensions.IsRendKillable(rendTarget) || stackCount >= getSliderItem(comboMenu, "minStacks"))
                    {
                        if (Environment.TickCount - spells[SpellSlot.E].LastCastAttemptT < 500)
                        {
                            return;
                        }

                        spells[SpellSlot.E].Cast();
                        spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
                    }
                }
            }

            if (getCheckBoxItem(harassMenu, "useEMin"))
            {
                var minion =
                    MinionManager.GetMinions(spells[SpellSlot.E].Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(x => Extensions.IsRendKillable(x))
                        .OrderBy(x => x.GetTotalHealth())
                        .FirstOrDefault();
                var target =
                    HeroManager.Enemies.Where(
                        x =>
                        spells[SpellSlot.E].CanCast(x) && Extensions.GetRendDamage(x) >= 1
                        && !x.HasBuffOfType(BuffType.SpellShield))
                        .OrderByDescending(x => Extensions.GetRendDamage(x))
                        .FirstOrDefault();

                if (minion != null && target != null && spells[SpellSlot.E].CanCast(minion) && spells[SpellSlot.E].CanCast(target) && Environment.TickCount - spells[SpellSlot.E].LastCastAttemptT > 500)
                {
                    spells[SpellSlot.E].Cast();
                    spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
                }
            }
        }

        /// <summary>
        ///     Perform the lane clear function
        /// </summary>
        private void OnLaneClear()
        {
            var minions = MinionManager.GetMinions(spells[SpellSlot.E].Range);

            if (getCheckBoxItem(laneClearMenu, "useQLC") && spells[SpellSlot.Q].IsReady())
            {
                if (getCheckBoxItem(comboMenu, "saveManaR") && spells[SpellSlot.R].IsReady()
                    && ObjectManager.Player.Mana
                    < spells[SpellSlot.Q].Instance.SData.Mana + spells[SpellSlot.R].Instance.SData.Mana)
                {
                    return;
                }

                foreach (var selectedMinion in
                    from selectedMinion in minions
                    let killcount =
                        this.GetCollisionMinions(
                            ObjectManager.Player,
                            ObjectManager.Player.ServerPosition.LSExtend(
                                selectedMinion.ServerPosition,
                                spells[SpellSlot.Q].Range))
                        .Count(
                            collisionMinion =>
                            collisionMinion.GetTotalHealth() < spells[SpellSlot.Q].GetDamage(collisionMinion))
                    where killcount >= getSliderItem(laneClearMenu, "minHitQ")
                    where !Orbwalker.IsAutoAttacking && !ObjectManager.Player.IsDashing()
                    select selectedMinion)
                {
                    spells[SpellSlot.Q].Cast(selectedMinion.ServerPosition);
                }
            }

            var harassableMinion =
                MinionManager.GetMinions(spells[SpellSlot.E].Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(x => Extensions.IsRendKillable(x))
                    .OrderBy(x => x.GetTotalHealth())
                    .FirstOrDefault();

            var rendTarget =
                HeroManager.Enemies.Where(
                    x =>
                    spells[SpellSlot.E].IsInRange(x) && Extensions.GetRendDamage(x) >= 1 && !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield)).OrderByDescending(x => Extensions.GetRendDamage(x)).FirstOrDefault();

            if (getCheckBoxItem(laneClearMenu, "minLC") && harassableMinion != null && rendTarget != null && spells[SpellSlot.E].CanCast(harassableMinion) && spells[SpellSlot.E].CanCast(rendTarget) && Environment.TickCount - spells[SpellSlot.E].LastCastAttemptT > 500)
            {
                spells[SpellSlot.E].Cast();
                spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
            }

            if (spells[SpellSlot.E].IsReady() && getCheckBoxItem(laneClearMenu, "useELC"))
            {
                var count = minions.Count(x => spells[SpellSlot.E].CanCast(x) && Extensions.IsRendKillable(x));

                if (count >= getSliderItem(laneClearMenu, "eHit") && Environment.TickCount - spells[SpellSlot.E].LastCastAttemptT > 500)
                {
                    spells[SpellSlot.E].Cast();
                    spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
                }
            }
        }

        /// <summary>
        ///     Perform the last hit function
        /// </summary>
        private void OnLastHit()
        {
        }

        /// <summary>
        ///     The on process spell function
        /// </summary>
        /// <param name="sender">
        ///     The Spell Sender
        /// </param>
        /// <param name="args">
        ///     The Arguments
        /// </param>
        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper")
            {
                Orbwalker.ResetAutoAttack();
            }

            if (sender.Type == GameObjectType.AIHeroClient && sender.IsEnemy && args.Target != null && getCheckBoxItem(comboMenu, "saveAllyR"))
            {
                var soulboundhero = HeroManager.Allies.FirstOrDefault(hero => hero.HasBuff("kalistacoopstrikeally") && args.Target.NetworkId == hero.NetworkId && hero.HealthPercent <= 15);

                if (soulboundhero != null && soulboundhero.HealthPercent < getSliderItem(comboMenu, "allyPercent"))
                {
                    spells[SpellSlot.R].Cast();
                }
            }
        }

        /// <summary>
        ///     TODO The on update.
        /// </summary>
        /// <param name="args">
        ///     TODO The args.
        /// </param>
        private void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) 
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) 
            {
                OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) 
            {
                OnLastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) 
            {
                OnLaneClear();
            }

            this.HandleSentinels();
            this.KillstealQ();

            var enemies = HeroManager.Enemies.Count(x => ObjectManager.Player.LSDistance(x) <= spells[SpellSlot.E].Range);

            if (getCheckBoxItem(comboMenu, "eDeath") && enemies > 2 && ObjectManager.Player.HealthPercent <= getSliderItem(comboMenu, "eHealth") && spells[SpellSlot.E].IsReady())
            {
                var target = HeroManager.Enemies.Where(x => spells[SpellSlot.E].IsInRange(x) && x.HasBuff("KalistaExpungeMarker")).OrderBy(x => Extensions.GetRendDamage(x)).FirstOrDefault();
                if (target != null)
                {
                    var stacks = Extensions.GetRendDamage(target);
                    var damage = Math.Ceiling(stacks * 100 / target.GetTotalHealth());
                    if (damage >= getSliderItem(comboMenu, "eDeathC") && Environment.TickCount - spells[SpellSlot.E].LastCastAttemptT > 500)
                    {
                        spells[SpellSlot.E].Cast();
                        spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
                    }
                }
            }

            if (getCheckBoxItem(miscMenu, "useJungleSteal"))
            {
                this.DoMobSteal();
            }

            if (getKeyBindItem(miscMenu, "fleeKey") || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                this.OnFlee();
            }

            var item = new Item(ItemId.Farsight_Alteration);
            if (getCheckBoxItem(miscMenu, "autoTrinket") && ObjectManager.Player.Level >= 9 && ObjectManager.Player.InShop() && !Items.HasItem((int)ItemId.Farsight_Alteration))
            {
                item.Buy();
            }

            this.HandleBalista();
        }

        /// <summary>
        ///     The target to check the collision for.
        /// </summary>
        /// <param name="target">
        ///     The target
        /// </param>
        private void QCollisionCheck(AIHeroClient target)
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, spells[SpellSlot.Q].Range);

            if (minions.Count < 1 || !getCheckBoxItem(comboMenu, "useQMin") || Orbwalker.IsAutoAttacking || ObjectManager.Player.IsDashing())
            {
                return;
            }

            foreach (var minion in minions.Where(x => x.IsValidTarget(spells[SpellSlot.Q].Range)))
            {
                var difference = ObjectManager.Player.LSDistance(target) - ObjectManager.Player.LSDistance(minion);

                for (var i = 0; i < difference; i += (int)target.BoundingRadius)
                {
                    var point =
                        minion.ServerPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -i).To3D();
                    var time = spells[SpellSlot.Q].Delay
                               + (ObjectManager.Player.LSDistance(point) / spells[SpellSlot.Q].Speed * 1000f);

                    var prediction = LeagueSharp.Common.Prediction.GetPrediction(target, time);

                    var collision = spells[SpellSlot.Q].GetCollision(
                        point.To2D(),
                        new List<Vector2> { prediction.UnitPosition.To2D() });

                    if (collision.Any(x => x.GetTotalHealth() > spells[SpellSlot.Q].GetDamage(x)))
                    {
                        return;
                    }

                    if (prediction.UnitPosition.LSDistance(point) <= spells[SpellSlot.Q].Width
                        && !minions.Any(m => m.LSDistance(point) <= spells[SpellSlot.Q].Width))
                    {
                        spells[SpellSlot.Q].Cast(minion);
                    }
                }
            }
        }

        #endregion
    }
}
