using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace D_RekSai
{
    internal static class Program
    {
        private const string ChampionName = "RekSai";

        private const string Activeq = "RekSaiQ";

        private static Spell _q, _bq, _w, _bw, _e, _be, _r;

        private static Menu _config;

        private static AIHeroClient _player;

        private static readonly List<Spell> SpellList = new List<Spell>();

        public static Menu comboMenu, harassMenu, farmMenu, jungleMenu, extraMenu, ksMenu, drawMenu;

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.ChampionName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 300);
            _bq = new Spell(SpellSlot.Q, 1450);
            _w = new Spell(SpellSlot.W, 200f);
            _bw = new Spell(SpellSlot.W, 200f);
            _e = new Spell(SpellSlot.E, 250f);
            _be = new Spell(SpellSlot.E, 700);
            _r = new Spell(SpellSlot.R);

            _bq.SetSkillshot(0.5f, 60, 1950, true, SkillshotType.SkillshotLine);
            _be.SetSkillshot(0, 60, 1600, false, SkillshotType.SkillshotLine);

            SpellList.Add(_q);
            SpellList.Add(_bq);
            SpellList.Add(_w);
            SpellList.Add(_bw);
            SpellList.Add(_e);
            SpellList.Add(_be);

            //D Rek'Sai
            _config = MainMenu.AddMenu("D-RekSai", "D-RekSai");

            //Combo
            comboMenu = _config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q Unburrowed"));
            comboMenu.Add("UseQBCombo", new CheckBox("Use Q burrowed"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E Unburrowed"));
            comboMenu.Add("UseEBCombo", new CheckBox("Use E burrowed"));

            // harass
            harassMenu = _config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            harassMenu.Add("UseEHarass", new CheckBox("Use E"));
            harassMenu.Add("harasstoggle", new KeyBind("Harass(toggle)", false, KeyBind.BindTypes.PressToggle, 'G'));

            // lane
            farmMenu = _config.AddSubMenu("Farm", "Farm");
            farmMenu.Add("UseQLane", new CheckBox("Use Q"));
            farmMenu.Add("UseWLane", new CheckBox("Use W"));
            farmMenu.Add("UseELane", new CheckBox("Use E"));

            //jungle
            jungleMenu = _config.AddSubMenu("Jungle", "Jungle");
            jungleMenu.Add("UseQJungle", new CheckBox("Use Q"));
            jungleMenu.Add("UseWJungle", new CheckBox("Use W"));
            jungleMenu.Add("UseEJungle", new CheckBox("Use E"));

            //Extra
            extraMenu = _config.AddSubMenu("Misc", "Misc");
            extraMenu.Add("AutoW", new CheckBox("Auto W"));
            extraMenu.Add("AutoWHP", new Slider("Use W if HP is <= ", 25, 1));
            extraMenu.Add("AutoWMP", new Slider("Use W if Fury is >= ", 100, 1));
            extraMenu.Add("Inter_W", new CheckBox("Use W to Interrupter"));
            extraMenu.Add("turnburrowed", new CheckBox("Turn Burrowed if do nothing"));
            extraMenu.Add("escapeterino", new KeyBind("Escape!!!", false, KeyBind.BindTypes.HoldActive, 'T'));

            //Kill Steal
            ksMenu = _config.AddSubMenu("KillSteal", "Ks");
            ksMenu.Add("ActiveKs", new CheckBox("Use KillSteal"));
            ksMenu.Add("UseQKs", new CheckBox("Use Q"));
            ksMenu.Add("UseEKs", new CheckBox("Use E"));

            //Drawings
            drawMenu = _config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("Drawharass", new CheckBox("Draw AutoHarass"));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
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

        private static void Game_OnGameUpdate(EventArgs args)
        {
            _player = ObjectManager.Player;

            Orbwalker.DisableAttacking = false;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                 getKeyBindItem(harassMenu, "harasstoggle")) &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Farm();
                JungleClear();
            }

            if (getCheckBoxItem(ksMenu, "ActiveKs"))
            {
                KillSteal();
            }

            if (getCheckBoxItem(extraMenu, "AutoW") &&
                (getCheckBoxItem(extraMenu, "turnburrowed") &&
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                 !getKeyBindItem(harassMenu, "harasstoggle") ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                 !getKeyBindItem(extraMenu, "escapeterino") ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)))
            {
                AutoW();
            }

            if (getKeyBindItem(extraMenu, "escapeterino"))
            {
                Escapeterino();
            }

            if ((!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                 !getKeyBindItem(harassMenu, "harasstoggle") ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                 !getKeyBindItem(extraMenu, "escapeterino") ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) &&
                getCheckBoxItem(extraMenu, "turnburrowed") && !IsBurrowed())
            {
                autoburrowed();
            }
        }

        public static bool IsBurrowed()
        {
            return ObjectManager.Player.HasBuff("RekSaiW");
        }

        public static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            return GetFirstWallPoint(from.LSTo2D(), to.LSTo2D(), step);
        }

        public static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
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

        public static void autoburrowed()
        {
            if (IsBurrowed() || _player.HasBuff("recall") || _player.InFountain()) return;
            if (!IsBurrowed() && _w.IsReady())
            {
                _w.Cast();
            }
        }

        private static void Escapeterino()
        {
            // Walljumper credits to Hellsing

            if (!IsBurrowed() && _w.IsReady() && _be.IsReady()) _w.Cast();

            // We need to define a new move position since jumping over walls
            // requires you to be close to the specified wall. Therefore we set the move
            // point to be that specific piont. People will need to get used to it,
            // but this is how it works.
            var wallCheck = GetFirstWallPoint(_player.Position, Game.CursorPos);

            // Be more precise
            if (wallCheck != null) wallCheck = GetFirstWallPoint((Vector3) wallCheck, Game.CursorPos, 5);

            // Define more position point
            var movePosition = wallCheck != null ? (Vector3) wallCheck : Game.CursorPos;

            // Update fleeTargetPosition
            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);

            // Only calculate stuff when our Q is up and there is a wall inbetween
            if (IsBurrowed() && _be.IsReady() && wallCheck != null)
            {
                // Get our wall position to calculate from
                var wallPosition = movePosition;

                // Check 300 units to the cursor position in a 160 degree cone for a valid non-wall spot
                var direction = (Game.CursorPos.LSTo2D() - wallPosition.LSTo2D()).Normalized();
                float maxAngle = 80;
                var step = maxAngle/20;
                float currentAngle = 0;
                float currentStep = 0;
                var jumpTriggered = false;
                while (true)
                {
                    // Validate the counter, break if no valid spot was found in previous loops
                    if (currentStep > maxAngle && currentAngle < 0) break;

                    // Check next angle
                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                    {
                        currentAngle = currentStep*(float) Math.PI/180;
                        currentStep += step;
                    }

                    else if (currentAngle > 0) currentAngle = -currentAngle;

                    Vector3 checkPoint;

                    // One time only check for direct line of sight without rotating
                    if (currentStep == 0)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + _be.Range*direction.To3D();
                    }
                    // Rotated check
                    else checkPoint = wallPosition + _be.Range*direction.Rotated(currentAngle).To3D();

                    // Check if the point is not a wall
                    if (!checkPoint.IsWall())
                    {
                        // Check if there is a wall between the checkPoint and wallPosition
                        wallCheck = GetFirstWallPoint(checkPoint, wallPosition);
                        if (wallCheck != null)
                        {
                            // There is a wall inbetween, get the closes point to the wall, as precise as possible
                            var wallPositionOpposite =
                                (Vector3) GetFirstWallPoint((Vector3) wallCheck, wallPosition, 5);

                            // Check if it's worth to jump considering the path length
                            if (_player.GetPath(wallPositionOpposite).ToList().LSTo2D().PathLength()
                                - _player.LSDistance(wallPositionOpposite) > 200) //200
                            {
                                // Check the distance to the opposite side of the wall
                                if (_player.LSDistance(wallPositionOpposite, true)
                                    < Math.Pow(_be.Range + 200 - _player.BoundingRadius/2, 2))
                                {
                                    // Make the jump happen
                                    _be.Cast(wallPositionOpposite);

                                    // Update jumpTriggered value to not orbwalk now since we want to jump
                                    jumpTriggered = true;

                                    break;
                                }
                            }

                            else
                            {
                                // yolo
                                Render.Circle.DrawCircle(Game.CursorPos, 35, Color.Red, 2);
                            }
                        }
                    }
                }

                // Check if the loop triggered the jump, if not just orbwalk
                if (!jumpTriggered)
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }
            }

            // Either no wall or W on cooldown, just move towards to wall then
            else
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
                if (IsBurrowed() && _be.IsReady()) _be.Cast(Game.CursorPos);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (IsBurrowed() && _bw.IsReady() && unit.LSIsValidTarget(_q.Range) && getCheckBoxItem(extraMenu, "Inter_W"))
                _bw.Cast(unit);
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }

            if (spell.Name.ToLower().Contains("reksaiq"))
            {
                Utility.DelayAction.Add(450, Orbwalker.ResetAutoAttack);
            }

            /*if (sender.IsMe)
            {
                 Game.PrintChat("Spell name: " + args.SData.Name.ToString());
            }*/
        }

        private static bool Qactive(this AIHeroClient target)
        {
            return
                target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == Activeq);
        }

        private static void Combo()
        {
            var t = TargetSelector.GetTarget(_bq.Range, DamageType.Physical);
            var reksaifury = Equals(_player.Mana, _player.MaxMana);

            if (IsBurrowed())
            {
                if (getCheckBoxItem(comboMenu, "UseEBCombo"))
                {
                    var te = TargetSelector.GetTarget(_be.Range + _bw.Range, DamageType.Physical);
                    if (_be.IsReady() && te.LSIsValidTarget(_be.Range + _bw.Range) && _player.LSDistance(te) > _q.Range)
                    {
                        var predE = _be.GetPrediction(te, true);
                        if (predE.Hitchance >= HitChance.High)
                            _be.Cast(predE.CastPosition.LSExtend(_player.ServerPosition, -50));
                    }
                }

                if (getCheckBoxItem(comboMenu, "UseQBCombo"))
                {
                    var tbq = TargetSelector.GetTarget(_bq.Range, DamageType.Magical);
                    if (_bq.IsReady() && t.LSIsValidTarget(_bq.Range)) _bq.CastIfHitchanceEquals(tbq, HitChance.High);
                }

                if (getCheckBoxItem(comboMenu, "UseWCombo"))
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
                    if (_w.IsReady() && tw.LSIsValidTarget(_w.Range) && !_bq.IsReady())
                    {
                        _bw.Cast(t);
                    }
                }
            }

            if (!IsBurrowed())
            {
                if (getCheckBoxItem(comboMenu, "UseQCombo"))
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (_q.IsReady() && tq.LSIsValidTarget(_q.Range)) _q.Cast(t);
                }

                if (getCheckBoxItem(comboMenu, "UseECombo"))
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    if (te.LSIsValidTarget(_e.Range) && _e.IsReady())
                    {
                        if (reksaifury && !Qactive(_player))
                        {
                            _e.Cast(te);
                        }
                        else if (_player.Mana < 100 && t.Health <= EDamage(t))
                        {
                            _e.Cast(te);
                        }
                        else if (_player.Mana == 100 && t.Health <= EDamagetrue(t))
                        {
                            _e.Cast(te);
                        }
                        else if (t.Health <= ComboDamage(t))
                        {
                            _e.Cast(te);
                        }
                    }
                }

                if (getCheckBoxItem(comboMenu, "UseWCombo") && _w.IsReady())
                {
                    var tw = TargetSelector.GetTarget(_bq.Range, DamageType.Physical);
                    if (!_q.IsReady() && !tw.LSIsValidTarget(_e.Range) && tw.LSIsValidTarget(_bq.Range) && !Qactive(_player))
                        _w.Cast();
                }
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            var dmg = 0d;

            if (_q.IsReady() && !IsBurrowed()) dmg += QDamage(hero);

            if (IsBurrowed()) dmg += BqDamage(hero);
            if (_w.IsReady() && IsBurrowed()) dmg += WDamage(hero);
            if (_e.IsReady())
                if (_player.Mana < 100)
                {
                    dmg += EDamage(hero);
                }

            if (_player.Mana == 100)
            {
                dmg += EDamagetrue(hero);
            }

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += _player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            dmg += _player.GetAutoAttackDamage(hero, true)*2;
            return (float) dmg;
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_bq.Range, DamageType.Magical);
            var targetq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            var targete = TargetSelector.GetTarget(_e.Range, DamageType.True);
            var reksaifury = Equals(_player.Mana, _player.MaxMana);
            if (getCheckBoxItem(harassMenu, "UseQHarass"))
            {
                if (target.LSIsValidTarget(_bq.Range) && _bq.IsReady() && IsBurrowed())
                {
                    _bq.CastIfHitchanceEquals(target, HitChance.High);
                }

                if (targetq.LSIsValidTarget(_q.Range) && _q.IsReady() && !IsBurrowed())
                {
                    _q.Cast();
                }
            }

            if (targete.LSIsValidTarget(_e.Range) && getCheckBoxItem(harassMenu, "UseEHarass") && _e.IsReady() &&
                !IsBurrowed() && reksaifury)
            {
                _e.Cast(targete);
            }
        }

        private static double EDamage(Obj_AI_Base unit)
        {
            return _e.IsReady()
                ? _player.CalcDamage(unit, DamageType.Physical,
                    new[] {0.8, 0.9, 1, 1.1, 1.2}[_e.Level - 1]*_player.TotalAttackDamage*
                    (1 + _player.Mana/_player.MaxMana))
                : 0d;
        }

        private static double EDamagetrue(Obj_AI_Base unit)
        {
            return _e.IsReady()
                ? _player.CalcDamage(unit, DamageType.True,
                    new[] {1.6, 1.8, 2, 2.2, 2.4}[_e.Level - 1]*_player.TotalAttackDamage)
                : 0d;
        }

        private static double QDamage(Obj_AI_Base unit)
        {
            return _q.IsReady()
                ? _player.CalcDamage(unit, DamageType.Physical,
                    new double[] {45, 75, 105, 135, 165}[_q.Level - 1] + _player.TotalAttackDamage*0.6)
                : 0d;
        }

        private static double BqDamage(Obj_AI_Base unit)
        {
            return _bq.IsReady()
                ? _player.CalcDamage(
                    unit,
                    DamageType.Magical,
                    new double[] {60, 90, 120, 150, 180}[_bq.Level - 1] + 0.7*_player.FlatMagicDamageMod)
                : 0d;
        }

        private static double WDamage(Obj_AI_Base unit)
        {
            return _bq.IsReady()
                ? _player.CalcDamage(
                    unit,
                    DamageType.Physical,
                    new double[] {40, 80, 120, 160, 200}[_bq.Level - 1] + 0.4*_player.TotalAttackDamage)
                : 0d;
        }

        private static void Farm()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _bq.Range);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range);
            var useQ = getCheckBoxItem(farmMenu, "UseQLane");
            var useW = getCheckBoxItem(farmMenu, "UseWLane");
            var useE = getCheckBoxItem(farmMenu, "UseELane");
            if (_q.IsReady() && useQ && !IsBurrowed())
            {
                if (
                    allMinionsQ.Where(m => m.LSDistance(_player.Position) <= _q.Range)
                        .Count(x => _q.GetDamage(x) > x.Health) >= 0)
                {
                    _q.Cast();
                }
            }

            if (_bq.IsReady() && useQ && IsBurrowed())
            {
                if (allMinions.Where(m => m.LSDistance(_player.Position) <= _bq.Range).Count(x => BqDamage(x) > x.Health)
                    >= 0)
                {
                    _bq.Cast(allMinions.FirstOrDefault());
                }
                else
                    foreach (var minion in allMinions)
                        if (minion.Health < 0.75*_player.LSGetSpellDamage(minion, SpellSlot.Q)
                            && minion.LSIsValidTarget(_bq.Range)) _bq.Cast(minion);
            }

            if (_e.IsReady() && useE && !IsBurrowed())
            {
                foreach (var minione in allMinions)
                    if (minione.Health < EDamage(minione) && !Qactive(_player)) _e.Cast(minione);
            }

            foreach (var minion in allMinions)
            {
                if (useW && !IsBurrowed() && !_q.IsReady() && !_e.IsReady() && Orbwalking.InAutoAttackRange(minion)
                    && !minion.HasBuff("RekSaiKnockupImmune") && !Qactive(_player))
                {
                    _w.Cast();
                }

                if (IsBurrowed() && _bw.IsReady() && useW &&
                    minion.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)))
                {
                    _bw.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var mob = MinionManager.GetMinions(_player.ServerPosition, _bq.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            var useQ = getCheckBoxItem(jungleMenu, "UseQJungle");
            var useW = getCheckBoxItem(jungleMenu, "UseWJungle");
            var useE = getCheckBoxItem(jungleMenu, "UseEJungle");
            var reksaifury = Equals(_player.Mana, _player.MaxMana);

            if (mob == null) return;

            if (!IsBurrowed())
            {
                if (useQ && _q.IsReady() && Orbwalking.InAutoAttackRange(mob))
                {
                    _q.Cast();
                }

                if (_e.IsReady() && useE && _player.LSDistance(mob) < _e.Range && !mob.Name.Contains("Mini"))
                {
                    if (reksaifury && !Qactive(_player))
                    {
                        _e.Cast(mob);
                    }
                    else if (mob.Health <= EDamage(mob))
                    {
                        _e.Cast(mob);
                    }
                    // RekSaiKnockupImmune , reksaiknockupimmune
                }

                if (useW && !mob.HasBuff("RekSaiKnockupImmune") && _w.IsReady() && !_q.IsReady() && !_e.IsReady()
                    && mob.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)) && !Qactive(_player))
                {
                    _w.Cast();
                }
            }

            if (IsBurrowed() && _bq.IsReady() && useQ && _player.LSDistance(mob) <= _bq.Range)
            {
                _bq.Cast(mob);
            }

            if (IsBurrowed() && _bw.IsReady() && useW && mob.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(_player)))
            {
                _bw.Cast();
            }
        }


        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                if (getCheckBoxItem(ksMenu, "UseQKs"))
                {
                    if (_bq.IsReady() && hero.LSIsValidTarget(_bq.Range) && IsBurrowed())
                    {
                        if (hero.Health <= BqDamage(hero)) _bq.CastIfHitchanceEquals(hero, HitChance.High);
                    }

                    if (_bq.IsReady() && _w.IsReady() && !hero.LSIsValidTarget(_q.Range) && hero.LSIsValidTarget(_bq.Range)
                        && hero.Health <= BqDamage(hero))
                    {
                        _w.Cast();
                        _bq.CastIfHitchanceEquals(hero, HitChance.High);
                    }

                    if (_q.IsReady() && hero.LSIsValidTarget(_q.Range) && !IsBurrowed())
                    {
                        if (hero.Health <= QDamage(hero)) _q.Cast();
                    }
                }

                if (_e.IsReady() && hero.LSIsValidTarget(_e.Range) && getCheckBoxItem(ksMenu, "UseEKs")
                    && !IsBurrowed())
                {
                    if (_player.Mana <= 100 && hero.Health <= EDamage(hero))
                    {
                        _e.Cast(hero);
                    }

                    if (_player.Mana == 100 && hero.Health <= EDamagetrue(hero))
                    {
                        _e.Cast(hero);
                    }
                }
            }
        }

        private static void AutoW()
        {
            var reksaiHp = _player.MaxHealth*getSliderItem(extraMenu, "AutoWHP")/100;
            var reksaiMp = _player.MaxMana*getSliderItem(extraMenu, "AutoWMP")/100;
            if (_player.HasBuff("Recall") || _player.InFountain()) return;
            if (_w.IsReady() && _player.Health <= reksaiHp && !IsBurrowed() && _player.Mana >= reksaiMp)
            {
                _w.Cast();
            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = getKeyBindItem(harassMenu, "harasstoggle");
            if (getCheckBoxItem(drawMenu, "Drawharass"))
            {
                if (harass)
                {
                    Drawing.DrawText(
                        Drawing.Width*0.02f,
                        Drawing.Height*0.92f,
                        Color.GreenYellow,
                        "Auto harass Enabled");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width*0.02f,
                        Drawing.Height*0.92f,
                        Color.OrangeRed,
                        "Auto harass Disabled");
            }

            if (getCheckBoxItem(drawMenu, "DrawQ") && IsBurrowed() && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _bq.Range, Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawE") && _e.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, IsBurrowed() ? _be.Range : _e.Range,
                    Color.GreenYellow);
            }
        }
    }
}