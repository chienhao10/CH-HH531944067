namespace ElRumble
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using LeagueSharp.Common;
    using Spell = LeagueSharp.Common.Spell;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal enum Spells
    {
        Q,

        W,

        E,

        R,

        R1
    }

    internal static class Rumble
    {
        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 500) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 0) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 950) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 1700) },
                                                                 { Spells.R1, new Spell(SpellSlot.R, 800) }
                                                             };

        private static SpellSlot _ignite;

        #endregion

        #region Properties

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

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

        #region Public Methods and Operators

        public static int CountEnemiesNearPosition(Vector3 pos, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(hero => hero.IsEnemy && !hero.IsDead && hero.IsValid && hero.Distance(pos) <= range);
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        public static void OnDraw(EventArgs args)
        {
            var drawQ = getCheckBoxItem(ElRumbleMenu.miscMenu, "ElRumble.Draw.Q");
            var drawE = getCheckBoxItem(ElRumbleMenu.miscMenu, "ElRumble.Draw.E");
            var drawR = getCheckBoxItem(ElRumbleMenu.miscMenu, "ElRumble.Draw.R");


            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }

                if (CountEnemiesNearPosition(Player.ServerPosition, spells[Spells.R].Range + 500) < 2)
                {
                    var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);

                    if (target == null)
                    {
                        return;
                    }

                    var vector1 = target.ServerPosition
                                  - Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 300;

                    spells[Spells.R1].UpdateSourcePosition(vector1, vector1);

                    var pred = spells[Spells.R1].GetPrediction(target, true);

                    var midpoint = (Player.ServerPosition + pred.UnitPosition) / 2;
                    var vector2 = midpoint - Vector3.Normalize(pred.UnitPosition - Player.ServerPosition) * 300;

                    if (Player.Distance(target.Position) < 400)
                    {
                        vector1 = midpoint + Vector3.Normalize(pred.UnitPosition - Player.ServerPosition) * 800;
                        if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, vector2))
                        {
                            var wts = Drawing.WorldToScreen(Player.Position);
                            Drawing.DrawText(wts[0], wts[1], Color.Wheat, "Hit: " + 1);

                            var wtsPlayer = Drawing.WorldToScreen(vector1);
                            var wtsPred = Drawing.WorldToScreen(vector2);

                            Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.Wheat);
                            Render.Circle.DrawCircle(vector1, 50, Color.Aqua);
                            Render.Circle.DrawCircle(vector2, 50, Color.Yellow);
                            Render.Circle.DrawCircle(pred.UnitPosition, 50, Color.Red);
                        }
                    }
                    else if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, pred.CastPosition))
                    {
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            var wts = Drawing.WorldToScreen(Player.Position);
                            Drawing.DrawText(wts[0], wts[1], Color.Wheat, "Hit: " + 1);

                            var wtsPlayer = Drawing.WorldToScreen(vector1);
                            var wtsPred = Drawing.WorldToScreen(pred.CastPosition);

                            Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.Wheat);
                            Render.Circle.DrawCircle(vector1, 50, Color.Aqua);
                            Render.Circle.DrawCircle(pred.CastPosition, 50, Color.Yellow);
                        }
                    }
                }
            }
        }

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Rumble")
            {
                return;
            }

            _ignite = Player.GetSpellSlot("summonerdot");

            spells[Spells.R].SetSkillshot(0.25f, 110, 2500, false, SkillshotType.SkillshotLine);
            spells[Spells.R1].SetSkillshot(0.25f, 110, 2600, false, SkillshotType.SkillshotLine);
            spells[Spells.E].SetSkillshot(0.45f, 90, 1200, true, SkillshotType.SkillshotLine);

            ElRumbleMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        #endregion

        #region Methods

        //CREDITS TO XSALICE - Made a few changes to it
        private static void CastR()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var vector1 = target.ServerPosition - Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 300;

            spells[Spells.R1].UpdateSourcePosition(vector1, vector1);

            var pred = spells[Spells.R1].GetPrediction(target, true);

            if (Player.Distance(target.Position) < 400)
            {
                var midpoint = (Player.ServerPosition + pred.UnitPosition) / 2;

                vector1 = midpoint + Vector3.Normalize(pred.UnitPosition - Player.ServerPosition) * 800;
                var vector2 = midpoint - Vector3.Normalize(pred.UnitPosition - Player.ServerPosition) * 300;

                if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, vector2))
                {
                    CastR2(vector1, vector2);
                }
            }
            else if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, pred.CastPosition))
            {
                if (pred.Hitchance >= HitChance.High)
                {
                    CastR2(vector1, pred.CastPosition);
                }
            }
        }

        private static void CastR2(Vector3 start, Vector3 end)
        {
            if (!spells[Spells.R].IsReady())
            {
                return;
            }

            spells[Spells.R].Cast(start, end);
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        private static bool IsPassWall(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);
            for (uint i = 0; i <= count; i += 25)
            {
                var pos = start.To2D().Extend(Player.ServerPosition.To2D(), -i);
                if (IsWall(pos))
                {
                    return true;
                }
            }
            return false;
        }

        //CREDITS TO XSALICE
        private static bool IsWall(Vector2 pos)
        {
            return (NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Wall
                    || NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Building);
        }

        private static void KeepHeat()
        {
            var useQ = getCheckBoxItem(ElRumbleMenu.heatMenu, "ElRumble.Heat.Q"); 
            var useW = getCheckBoxItem(ElRumbleMenu.heatMenu, "ElRumble.Heat.W");

            if (Player.Mana < 50)
            {
                if (useQ && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(Game.CursorPos);
                }

                if (useW && spells[Spells.W].IsReady())
                {
                    spells[Spells.W].Cast();
                }
            }
        }

        private static void OnClear()
        {
            var useQ = getCheckBoxItem(ElRumbleMenu.clearMenu, "ElRumble.LaneClear.Q");
            var useE = getCheckBoxItem(ElRumbleMenu.clearMenu, "ElRumble.LaneClear.E");

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (minions.Count > 1)
                {
                    var farmLocation = spells[Spells.Q].GetCircularFarmLocation(minions);
                    spells[Spells.Q].Cast(farmLocation.Position);
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(minions.FirstOrDefault());
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);

            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.Q");
            var useW = getCheckBoxItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.W"); 
            var useE = getCheckBoxItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.E"); 
            var useR = getCheckBoxItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.R"); 
            var useI = getCheckBoxItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.Ignite");
            var countEnemies = getSliderItem(ElRumbleMenu.comboMenu, "ElRumble.Combo.Count.Enemies");

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                var pred = spells[Spells.E].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.E].Cast(pred.CastPosition);
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }

            if (useR && spells[Spells.R].IsReady() && Player.CountEnemiesInRange(spells[Spells.R].Range) >= countEnemies)
            {
                CastR();
            }

            if (useR && spells[Spells.R].IsReady())
            {
                if (target.Health < spells[Spells.R].GetDamage(target))
                {
                    CastR();
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(ElRumbleMenu.harassMenu, "ElRumble.Harass.Q"); 
            var useE = getCheckBoxItem(ElRumbleMenu.harassMenu, "ElRumble.Harass.E");

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                var pred = spells[Spells.E].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.E].Cast(target);
                }
            }
        }

        private static void OnJungleClear()
        {
            var useQ = getCheckBoxItem(ElRumbleMenu.clearMenu, "ElRumble.JungleClear.Q");  
            var useE = getCheckBoxItem(ElRumbleMenu.clearMenu, "ElRumble.JungleClear.E"); 

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (minions.Count > 1)
                {
                    var farmLocation = spells[Spells.Q].GetCircularFarmLocation(minions);
                    spells[Spells.Q].Cast(farmLocation.Position);
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(minions.FirstOrDefault());
            }
        }

        private static void OnLastHit()
        {
            var useE = getCheckBoxItem(ElRumbleMenu.clearMenu, "ElRumble.LastHit.E");
            if (useE && spells[Spells.E].IsReady())
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.E].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.E].Cast(minion);
                            return;
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnJungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                OnLastHit();
            }           

            var keepHeat = getKeyBindItem(ElRumbleMenu.heatMenu, "ElRumble.KeepHeat.Activated");
            if (keepHeat)
            {
                KeepHeat();
            }

            if (getKeyBindItem(ElRumbleMenu.miscMenu, "ElRumble.Misc.R") && spells[Spells.R].IsReady())
            {
                CastR();
            }
        }

        #endregion
    }
}