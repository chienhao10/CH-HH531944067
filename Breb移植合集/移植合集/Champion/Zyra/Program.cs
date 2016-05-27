using System;
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

namespace D_Zyra
{
    internal class Program
    {
        private const string ChampionName = "Zyra";

        private static Spell _q, _w, _e, _r, _passive;

        private static Menu _config;

        private static AIHeroClient _player;

        public static Menu comboMenu, harassMenu, farmMenu, miscMenu, drawMenu;

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

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.ChampionName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 800);
            _w = new Spell(SpellSlot.W, 825);
            _e = new Spell(SpellSlot.E, 1100);
            _r = new Spell(SpellSlot.R, 700);
            _passive = new Spell(SpellSlot.Q, 1470);

            _q.SetSkillshot(1f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            _e.SetSkillshot(0.5f, 100f, 1150f, false, SkillshotType.SkillshotLine);
            _r.SetSkillshot(0.5f, 500f, 20f, false, SkillshotType.SkillshotCircle);
            _passive.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);

            //D Zyra
            _config = MainMenu.AddMenu("D-Zyra", "D-Zyra");

            //Combo usedfg, useignite
            comboMenu = _config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("useQC", new CheckBox("Use Q"));
            comboMenu.Add("useW_Passive", new CheckBox("Plant on Q"));
            comboMenu.Add("useEC", new CheckBox("Use E"));
            comboMenu.Add("useWE_Passive", new CheckBox("Plant on E"));
            comboMenu.Add("use_ulti", new CheckBox("Use R If Killable"));
            comboMenu.Add("UseRE", new CheckBox("Use AutoR"));
            comboMenu.Add("MinTargets", new Slider("AutoR if Min Targets >=", 2, 1, 5));
            comboMenu.Add("useRaim", new KeyBind("Use R(Semi-Manual)", false, KeyBind.BindTypes.HoldActive, 'T'));

            //harass
            harassMenu = _config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("useQH", new CheckBox("Use Q"));
            harassMenu.Add("useW_Passiveh", new CheckBox("Plant on Q"));
            harassMenu.Add("useEH", new CheckBox("Use E"));
            harassMenu.Add("useWE_Passiveh", new CheckBox("Plant on E"));
            harassMenu.Add("harassmana", new Slider("Minimum Mana% >", 35, 1));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.PressToggle, 'G'));

            //Farm
            farmMenu = _config.AddSubMenu("Farm", "Farm");
            farmMenu.AddGroupLabel("Lane");
            farmMenu.Add("useQL", new CheckBox("Use Q"));
            farmMenu.Add("useW_Passivel", new CheckBox("Plant on Q"));
            farmMenu.Add("useEL", new CheckBox("Use E"));
            farmMenu.Add("useWE_Passivel", new CheckBox("Plant on E"));
            farmMenu.Add("lanemana", new Slider("Minimum Mana% >", 35, 1));

            farmMenu.AddGroupLabel("Jungle");
            farmMenu.Add("useQJ", new CheckBox("Use Q"));
            farmMenu.Add("useW_Passivej", new CheckBox("Plant on Q"));
            farmMenu.Add("useEJ", new CheckBox("Use E"));
            farmMenu.Add("useWE_Passivej", new CheckBox("Plant on E"));
            farmMenu.Add("junglemana", new Slider("Minimum Mana% >", 35, 1));

            //Misc
            miscMenu = _config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("useQkill", new CheckBox("Q to Killsteal"));
            miscMenu.Add("useEkill", new CheckBox("E to Killsteal"));
            miscMenu.Add("Inter_E", new CheckBox("Interrupter E"));
            miscMenu.Add("Gap_E", new CheckBox("GapClosers E"));
            miscMenu.Add("usefrostq", new CheckBox("Frost Queen to GapClosers"));
            miscMenu.Add("support", new CheckBox("Support Mode", false));
            miscMenu.AddGroupLabel("E Hit Change");
            miscMenu.Add("Echange", new ComboBox("E Hit Change", 3, "Low", "Medium", "High", "Very High"));

            //Draw
            drawMenu = _config.AddSubMenu("Drawing", "Drawing");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("DrawR", new CheckBox("Draw R", false));
            drawMenu.Add("damagetest", new CheckBox("Damage Text", false));
            drawMenu.Add("Drawharass", new CheckBox("Draw AutoHarass"));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (ZyraisZombie())
            {
                CastPassive();
                return;
            }
            if (getKeyBindItem(comboMenu, "useRaim") && _r.IsReady())
            {
                var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                if (t.IsValidTarget(_r.Range)) _r.Cast(t.Position);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                 getKeyBindItem(harassMenu, "harasstoggle")) &&
                100*(_player.Mana/_player.MaxMana) > getSliderItem(harassMenu, "harassmana") &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var lc = 100*(_player.Mana/_player.MaxMana) > getSliderItem(farmMenu, "lanemana");
                if (lc)
                {
                    Laneclear();
                }

                var jg = 100*(_player.Mana/_player.MaxMana) > getSliderItem(farmMenu, "junglemana");
                if (jg)
                {
                    JungleClear();
                }
            }

            _player = ObjectManager.Player;
            Orbwalker.DisableAttacking = false;
            KillSteal();
        }

        private static HitChance Echange()
        {
            switch (getBoxItem(miscMenu, "Echange"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        // princer007  Code
        private static int Getallies(float range)
        {
            return ObjectManager.Get<AIHeroClient>().Count(hero => hero.IsAlly && !hero.IsMe && _player.LSDistance(hero) <= range);
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (getCheckBoxItem(miscMenu, "support") && Getallies(1000) > 0)
            {
                if (args.Target is Obj_AI_Minion)
                {
                    args.Process = false;
                }
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            var dmg = 0d;

            if (_q.IsReady())
            {
                if (_w.IsReady())
                {
                    dmg += _player.GetSpellDamage(hero, SpellSlot.Q) + (23 + 6.5*ObjectManager.Player.Level)
                           + 1.2*_player.FlatMagicDamageMod;
                }
                else dmg += _player.GetSpellDamage(hero, SpellSlot.Q);
            }

            if (_e.IsReady())
            {
                if (_w.IsReady())
                {
                    dmg += _player.GetSpellDamage(hero, SpellSlot.E) + (23 + 6.5*ObjectManager.Player.Level)
                           + 1.2*_player.FlatMagicDamageMod;
                }
                else dmg += _player.GetSpellDamage(hero, SpellSlot.E);
            }
            if (_r.IsReady()) dmg += _player.GetSpellDamage(hero, SpellSlot.R);
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                dmg += _player.GetItemDamage(hero, Damage.DamageItems.Botrk);
            if (Items.HasItem(3146) && Items.CanUseItem(3146))
                dmg += _player.GetItemDamage(hero, Damage.DamageItems.Hexgun);

            if (ObjectManager.Player.HasBuff("LichBane"))
            {
                dmg += _player.BaseAttackDamage*0.75 + _player.FlatMagicDamageMod*0.5;
            }
            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += _player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            dmg += _player.GetAutoAttackDamage(hero, true);
            return (float) dmg;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var pos = _e.GetPrediction(gapcloser.Sender).CastPosition;
            if (getCheckBoxItem(miscMenu, "Gap_E"))
            {
                if (_e.IsReady() && gapcloser.Sender.IsValidTarget(_e.Range) && _w.IsReady())
                {
                    _e.CastIfHitchanceEquals(gapcloser.Sender, Echange());
                    Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 2, pos.Y - 2, pos.Z)));
                    Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 2, pos.Y + 2, pos.Z)));
                }
                else if (_e.IsReady() && gapcloser.Sender.IsValidTarget(_e.Range))
                {
                    _e.CastIfHitchanceEquals(gapcloser.Sender, HitChance.High);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var pos = _e.GetPrediction(unit).CastPosition;
            if (!getCheckBoxItem(miscMenu, "Inter_E")) return;
            if (_e.IsReady() && unit.IsValidTarget(_e.Range) && _w.IsReady())
            {
                _e.CastIfHitchanceEquals(unit, Echange());
                Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 2, pos.Y - 2, pos.Z)));
                Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 2, pos.Y + 2, pos.Z)));
            }
            else if (_e.IsReady() && unit.IsValidTarget(_e.Range))
            {
                _e.CastIfHitchanceEquals(unit, HitChance.High);
            }
        }

        private static void Combo()
        {
            if (getCheckBoxItem(comboMenu, "UseRE") || getCheckBoxItem(comboMenu, "use_ulti")) CastREnemy();
            if (getCheckBoxItem(comboMenu, "useQC")) CastQEnemy();
            if (getCheckBoxItem(comboMenu, "useEC")) CastEEnemy();
        }

        private static void Harass()
        {
            if (getCheckBoxItem(harassMenu, "useQH")) CastQEnemyharass();
            if (getCheckBoxItem(harassMenu, "useEH")) CastEEnemyharass();
        }

        private static void Laneclear()
        {
            if (getCheckBoxItem(farmMenu, "useQL")) CastQMinion();
            if (getCheckBoxItem(farmMenu, "useEL")) CastEMinion();
        }

        private static void JungleClear()
        {
            if (getCheckBoxItem(farmMenu, "useQJ")) CastQjungleMinion();
            if (getCheckBoxItem(farmMenu, "useEJ")) CastEjungleMinion();
        }

        private static bool ZyraisZombie()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name
                   == ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Name
                   || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name
                   == ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name;
        }

        private static void CastEjungleMinion()
        {
            if (!_e.IsReady()) return;
            var minions = MinionManager.GetMinions(
                _player.ServerPosition,
                _e.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (minions.Count == 0) return;
            var castPostion =
                MinionManager.GetBestLineFarmLocation(
                    minions.Select(minion => minion.ServerPosition.To2D()).ToList(),
                    _e.Width,
                    _e.Range);
            _e.Cast(castPostion.Position);
            if (getCheckBoxItem(farmMenu, "useWE_Passivej") && _w.IsReady())
            {
                var pos = _e.GetCircularFarmLocation(minions);
                Utility.DelayAction.Add(50, () => _w.Cast(pos.Position.To3D()));
                Utility.DelayAction.Add(150, () => _w.Cast(pos.Position.To3D()));
            }
        }

        private static void CastEMinion()
        {
            if (!_e.IsReady()) return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range);
            if (minions.Count == 0) return;
            var castPostion =
                MinionManager.GetBestLineFarmLocation(
                    minions.Select(minion => minion.ServerPosition.To2D()).ToList(),
                    _e.Width,
                    _e.Range);
            _e.Cast(castPostion.Position);
            if (getCheckBoxItem(farmMenu, "useWE_Passivel") && _w.IsReady())
            {
                var pos = _e.GetCircularFarmLocation(minions);
                Utility.DelayAction.Add(50, () => _w.Cast(pos.Position.To3D()));
                Utility.DelayAction.Add(150, () => _w.Cast(pos.Position.To3D()));
            }
        }


        private static void CastQjungleMinion()
        {
            if (!_q.IsReady()) return;
            var minions = MinionManager.GetMinions(
                _player.ServerPosition,
                _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (minions.Count == 0) return;
            var castPostion =
                MinionManager.GetBestCircularFarmLocation(
                    minions.Select(minion => minion.ServerPosition.To2D()).ToList(),
                    _q.Width,
                    _q.Range);
            _q.Cast(castPostion.Position);
            if (getCheckBoxItem(farmMenu, "useW_Passivej") && _w.IsReady())
            {
                var pos = castPostion.Position.To3D();
                Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z)));
                Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z)));
            }
        }

        private static void CastQMinion()
        {
            if (!_q.IsReady()) return;
            var minions = MinionManager.GetMinions(
                ObjectManager.Player.Position,
                _q.Range + _q.Width/2);
            if (minions.Count == 0) return;
            var castPostion =
                MinionManager.GetBestCircularFarmLocation(
                    minions.Select(minion => minion.ServerPosition.To2D()).ToList(),
                    _q.Width,
                    _q.Range);
            _q.Cast(castPostion.Position);
            if (getCheckBoxItem(farmMenu, "useW_Passivel") && _w.IsReady())
            {
                var pos = castPostion.Position.To3D();
                Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z)));
                Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z)));
            }
        }

        private static void CastREnemy()
        {
            var target = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            var rpred = _r.GetPrediction(target, true);
            if (!target.IsValidTarget(_r.Range) || !_r.IsReady()) return;

            if (ComboDamage(target)*1.3 > target.Health && getCheckBoxItem(comboMenu, "use_ulti")
                && _r.GetPrediction(target).Hitchance >= HitChance.High)
            {
                _r.Cast(rpred.CastPosition);
            }
            if (ObjectManager.Get<AIHeroClient>().Count(hero => hero.IsValidTarget(_r.Range))
                >= getSliderItem(comboMenu, "MinTargets")
                && _r.GetPrediction(target).Hitchance >= HitChance.High)
            {
                _r.Cast(target);
            }
        }

        private static void CastQEnemy()
        {
            if (!_q.IsReady()) return;
            var target = TargetSelector.GetTarget(_q.Range + _q.Width/2, DamageType.Magical);
            if (!target.IsValidTarget(_q.Range)) return;
            _q.CastIfHitchanceEquals(target, HitChance.High);
            if (_w.IsReady() && getCheckBoxItem(comboMenu, "useW_Passive"))
            {
                var pos = _q.GetPrediction(target).CastPosition;
                Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 2, pos.Y - 2, pos.Z)));
                Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 2, pos.Y + 2, pos.Z)));
            }
        }

        private static void CastQEnemyharass()
        {
            if (!_q.IsReady()) return;
            var target = TargetSelector.GetTarget(_q.Range + _q.Width/2, DamageType.Magical);
            if (!target.IsValidTarget(_q.Range)) return;
            _q.CastIfHitchanceEquals(target, HitChance.High);
            if (_w.IsReady() && getCheckBoxItem(harassMenu, "useW_Passiveh"))
            {
                var pos = _q.GetPrediction(target).CastPosition;
                Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 2, pos.Y - 2, pos.Z)));
                Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 2, pos.Y + 2, pos.Z)));
            }
        }

        private static void CastEEnemy()
        {
            if (!_e.IsReady()) return;
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
            if (!target.IsValidTarget(_e.Range)) return;
            _e.CastIfHitchanceEquals(target, Echange());
            if (_w.IsReady() && getCheckBoxItem(comboMenu, "useWE_Passive"))
            {
                var pos = _e.GetPrediction(target).CastPosition;
                Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z)));
                Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z)));
            }
        }

        private static void CastEEnemyharass()
        {
            if (!_e.IsReady()) return;
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
            if (!target.IsValidTarget(_e.Range)) return;
            _e.CastIfHitchanceEquals(target, Echange());
            if (_w.IsReady() && getCheckBoxItem(harassMenu, "useWE_Passiveh"))
            {
                var pos = _e.GetPrediction(target).CastPosition;
                Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z)));
                Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z)));
            }
        }

        private static void CastPassive()
        {
            if (!_passive.IsReady()) return;
            var target = TargetSelector.GetTarget(_passive.Range, DamageType.Magical);
            if (!target.IsValidTarget(_e.Range)) return;
            _passive.CastIfHitchanceEquals(target, HitChance.High);
        }

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var useq = getCheckBoxItem(miscMenu, "useQkill");
                var usee = getCheckBoxItem(miscMenu, "useEkill");
                var whDmg = _player.GetSpellDamage(hero, SpellSlot.W);
                var qhDmg = _player.GetSpellDamage(hero, SpellSlot.Q);
                var ehDmg = _player.GetSpellDamage(hero, SpellSlot.E);
                var emana = _player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
                var qmana = _player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
                if (useq && hero.IsValidTarget(_q.Range) && _q.IsReady())
                {
                    if (qhDmg >= hero.Health && qmana < _player.Mana)
                    {
                        _q.CastIfHitchanceEquals(hero, HitChance.High);
                    }
                    else if (qhDmg + whDmg > hero.Health && _player.Mana >= qmana && _w.IsReady())
                    {
                        _q.CastIfHitchanceEquals(hero, HitChance.High);
                        var pos = _e.GetPrediction(hero).CastPosition;
                        Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z)));
                        Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z)));
                    }
                }
                if (usee && hero.IsValidTarget(_e.Range) && _e.IsReady())
                {
                    if (ehDmg >= hero.Health && emana < _player.Mana)
                    {
                        _e.CastIfHitchanceEquals(hero, HitChance.High);
                    }
                    else if (ehDmg + whDmg > hero.Health && _player.Mana >= emana && _w.IsReady())
                    {
                        _e.CastIfHitchanceEquals(hero, HitChance.High);
                        var pos = _e.GetPrediction(hero).CastPosition;
                        Utility.DelayAction.Add(50, () => _w.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z)));
                        Utility.DelayAction.Add(150, () => _w.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z)));
                    }
                }
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
            if (getCheckBoxItem(drawMenu, "damagetest"))
            {
                foreach (var enemyVisible in
                    ObjectManager.Get<AIHeroClient>().Where(enemyVisible => enemyVisible.IsValidTarget()))
                {
                    if (ComboDamage(enemyVisible) > enemyVisible.Health)
                    {
                        Drawing.DrawText(
                            Drawing.WorldToScreen(enemyVisible.Position)[0] + 50,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] - 40,
                            Color.Red,
                            "Combo=Rekt");
                    }
                    else if (ComboDamage(enemyVisible) + _player.GetAutoAttackDamage(enemyVisible, true)*2 >
                             enemyVisible.Health)
                    {
                        Drawing.DrawText(
                            Drawing.WorldToScreen(enemyVisible.Position)[0] + 50,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] - 40,
                            Color.Orange,
                            "Combo+AA=Rekt");
                    }
                    else
                        Drawing.DrawText(
                            Drawing.WorldToScreen(enemyVisible.Position)[0] + 50,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] - 40,
                            Color.Green,
                            "Unkillable");
                }
            }

            if (getCheckBoxItem(drawMenu, "DrawQ") && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, Color.GreenYellow);
            }
            if (getCheckBoxItem(drawMenu, "DrawW") && _w.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, Color.GreenYellow);
            }
            if (getCheckBoxItem(drawMenu, "DrawE") && _e.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawR") && _r.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, Color.GreenYellow);
            }
        }
    }
}