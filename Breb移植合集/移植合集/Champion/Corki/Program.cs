using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Properties;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace ElCorki
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R1,

        R2
    }

    internal static class Corki
    {
        #region Public Properties

        public static string ScriptVersion
        {
            get { return typeof (Corki).Assembly.GetName().Version.ToString(); }
        }

        #endregion

        #region Static Fields

        public static Menu _menu, comboMenu, harassMenu, laneclearMenu, miscMenu;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 825)},
            {Spells.W, new Spell(SpellSlot.W, 800)},
            {Spells.E, new Spell(SpellSlot.E, 600)},
            {Spells.R1, new Spell(SpellSlot.R, 1300)},
            {Spells.R2, new Spell(SpellSlot.R, 1500)}
        };

        private static SpellSlot _ignite;

        #endregion

        #region Properties

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        public static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(miscMenu, "ElCorki.Draw.off");
            var drawQ = getCheckBoxItem(miscMenu, "ElCorki.Draw.Q");
            var drawW = getCheckBoxItem(miscMenu, "ElCorki.Draw.W");
            var drawE = getCheckBoxItem(miscMenu, "ElCorki.Draw.E");
            var drawR = getCheckBoxItem(miscMenu, "ElCorki.Draw.R");

            if (drawOff)
            {
                return;
            }

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

            if (drawW)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R1].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R1].Range, Color.White);
                }
            }
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

        public static void Initialize()
        {
            _menu = MainMenu.AddMenu("ElCorki", "menu");

            comboMenu = _menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElCorki.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElCorki.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElCorki.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElCorki.Combo.Ignite", new CheckBox("Use Ignite"));
            comboMenu.Add("ElCorki.Combo.RStacks", new Slider("Keep R Stacks", 0, 0, 7));
            comboMenu.Add("ElCorki.hitChance", new Slider("Hitchance Q (Lowest Hitchance to Highest) : ", 0, 0, 3));

            harassMenu = _menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElCorki.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElCorki.Harass.E", new CheckBox("Use E", false));
            harassMenu.Add("ElCorki.Harass.R", new CheckBox("Use R"));
            harassMenu.Add("ElCorki.Harass.RStacks", new Slider("Keep R Stacks", 0, 0, 7));
            harassMenu.Add("ElCorki.harass.mana2", new Slider("Harass mana", 55));
            harassMenu.AddSeparator();

            harassMenu.AddGroupLabel("Auto Harass");
            harassMenu.Add("ElCorki.AutoHarass",
                new KeyBind("[Toggle] Auto harass", false, KeyBind.BindTypes.PressToggle, 'U'));
            harassMenu.Add("ElCorki.UseQAutoHarass", new CheckBox("Use Q"));
            harassMenu.Add("ElCorki.UseRAutoHarass", new CheckBox("Use R"));
            harassMenu.Add("ElCorki.harass.mana", new Slider("Auto harass mana", 55));

            laneclearMenu = _menu.AddSubMenu("Clear", "Clear");
            laneclearMenu.AddGroupLabel("Lane Clear");
            laneclearMenu.Add("useQFarm", new CheckBox("Use Q"));
            laneclearMenu.Add("ElCorki.Count.Minions", new Slider("Killable minions with Q >=", 2, 1, 5));
            laneclearMenu.Add("useEFarm", new CheckBox("Use E"));
            laneclearMenu.Add("ElCorki.Count.Minions.E", new Slider("Killable minions with E >=", 2, 1, 5));
            laneclearMenu.Add("useRFarm", new CheckBox("Use R"));
            laneclearMenu.Add("ElCorki.Count.Minions.R", new Slider("Killable minions with R >=", 2, 1, 5));
            laneclearMenu.AddSeparator();

            laneclearMenu.AddGroupLabel("Jungle Clear");
            laneclearMenu.Add("useQFarmJungle", new CheckBox("Use Q in jungle"));
            laneclearMenu.Add("useEFarmJungle", new CheckBox("Use E in jungle"));
            laneclearMenu.Add("useRFarmJungle", new CheckBox("Use R in jungle"));
            laneclearMenu.AddSeparator();

            laneclearMenu.AddGroupLabel("Both");
            laneclearMenu.Add("minmanaclear", new Slider("Mana needed to clear ", 55));

            //ElCorki.Misc
            miscMenu = _menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElCorki.Draw.off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("ElCorki.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElCorki.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("ElCorki.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElCorki.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElCorki.misc.ks", new CheckBox("Killsteal mode"));
            miscMenu.Add("ElCorki.misc.junglesteal", new CheckBox("Jungle steal mode"));

            Console.WriteLine(Resources.Corki_Initialize_Menu_Loaded);
        }

        public static void Game_OnGameLoad()
        {
            Console.WriteLine(Resources.Corki_Game_OnGameLoad_Injected);

            spells[Spells.Q].SetSkillshot(0.35f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            spells[Spells.E].SetSkillshot(0f, (float) (45*Math.PI/180), 1500, false, SkillshotType.SkillshotCone);
            spells[Spells.R1].SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            spells[Spells.R2].SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            _ignite = Player.GetSpellSlot("summonerdot");

            Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion

        #region Methods

        private static void AutoHarassMode()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Physical);
            var rTarget = TargetSelector.GetTarget(spells[Spells.R1].Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget() || rTarget == null || !rTarget.IsValidTarget())
            {
                return;
            }

            if (getKeyBindItem(harassMenu, "ElCorki.AutoHarass"))
            {
                var q = getCheckBoxItem(harassMenu, "ElCorki.UseQAutoHarass");
                var r = getCheckBoxItem(harassMenu, "ElCorki.UseRAutoHarass");
                var mana = getSliderItem(harassMenu, "ElCorki.harass.mana");

                if (Player.ManaPercent < mana)
                {
                    return;
                }

                if (r && spells[Spells.R1].IsReady() && spells[Spells.R1].IsInRange(rTarget)
                    || spells[Spells.R2].IsInRange(rTarget))
                {
                    var bigR = HasBigRocket();

                    var _target = TargetSelector.GetTarget(bigR ? spells[Spells.R2].Range : spells[Spells.R1].Range,
                        DamageType.Magical);
                    if (_target != null)
                    {
                        if (bigR)
                        {
                            spells[Spells.R2].Cast(_target);
                        }
                        else
                        {
                            spells[Spells.R1].Cast(_target);
                        }
                    }
                }

                if (q && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast(target);
                }
            }
        }

        private static bool HasBigRocket()
        {
            return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName.ToLower() == "corkimissilebarragecounterbig");
        }

        private static void Combo(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var comboQ = getCheckBoxItem(comboMenu, "ElCorki.Combo.Q");
            var comboE = getCheckBoxItem(comboMenu, "ElCorki.Combo.E");
            var comboR = getCheckBoxItem(comboMenu, "ElCorki.Combo.R");
            var useIgnite = getCheckBoxItem(comboMenu, "ElCorki.Combo.Ignite");
            var rStacks = getSliderItem(comboMenu, "ElCorki.Combo.RStacks");

            var bigR = HasBigRocket();

            var _target = TargetSelector.GetTarget(
                bigR ? spells[Spells.R2].Range : spells[Spells.R1].Range,
                DamageType.Magical);

            if (comboQ && spells[Spells.Q].IsReady())
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (comboE && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(target);
            }

            if (comboR && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo > rStacks && _target != null)
            {
                if (!bigR)
                {
                    if (target.IsValidTarget(spells[Spells.R1].Range))
                    {
                        spells[Spells.R1].Cast(target);
                    }
                }
                else
                {
                    if (target.IsValidTarget(spells[Spells.R2].Range))
                    {
                        spells[Spells.R2].Cast(target);
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }

        private static HitChance GetHitchance()
        {
            switch (getSliderItem(comboMenu, "ElCorki.hitChance"))
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
                    return HitChance.Medium;
            }
        }

        private static void Harass(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var harassQ = getCheckBoxItem(harassMenu, "ElCorki.Harass.Q");
            var harassE = getCheckBoxItem(harassMenu, "ElCorki.Harass.E");
            var harassR = getCheckBoxItem(harassMenu, "ElCorki.Harass.R");
            var minmana = getSliderItem(harassMenu, "ElCorki.harass.mana2");
            var rStacks = getSliderItem(harassMenu, "ElCorki.Harass.RStacks");

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            if (harassQ && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast(target);
            }

            if (harassE && spells[Spells.E].IsReady())
            {
                spells[Spells.E].CastOnBestTarget();
            }

            if (harassR && spells[Spells.R1].IsReady()
                && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo > rStacks)
            {
                spells[Spells.R1].CastIfHitchanceEquals(target, CustomHitChance, true);
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void JungleClear()
        {
            var useQ = getCheckBoxItem(laneclearMenu, "useQFarmJungle");
            var useE = getCheckBoxItem(laneclearMenu, "useEFarmJungle");
            var useR = getCheckBoxItem(laneclearMenu, "useRFarmJungle");
            var minmana = getSliderItem(laneclearMenu, "minmanaclear");

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 700, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].IsReady() && useQ)
                {
                    spells[Spells.Q].Cast(minion);
                }

                if (spells[Spells.E].IsReady() && useE)
                {
                    spells[Spells.E].Cast(minion);
                }

                if (spells[Spells.R1].IsReady() && useR)
                {
                    spells[Spells.R1].Cast(minion);
                }
            }
        }

        private static void JungleStealMode()
        {
            var useJsm = getCheckBoxItem(miscMenu, "ElCorki.misc.junglesteal");

            if (!useJsm)
            {
                return;
            }

            var jMob =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.R1].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.Health + x.HPRegenRate/2 <= spells[Spells.R1].GetDamage(x));

            if (spells[Spells.R1].CanCast(jMob))
            {
                spells[Spells.R1].Cast(jMob);
            }

            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.R1].Range,
                    MinionTypes.All,
                    MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(
                        x =>
                            x.Health <= spells[Spells.E].GetDamage(x)
                            &&
                            (x.BaseSkinName.ToLower().Contains("siege") || x.BaseSkinName.ToLower().Contains("super")));

            if (spells[Spells.R1].IsReady() && spells[Spells.R1].CanCast(minion))
            {
                spells[Spells.R1].Cast(minion);
            }
        }

        private static void KsMode()
        {
            var useKs = getCheckBoxItem(miscMenu, "ElCorki.misc.ks");
            if (!useKs)
            {
                return;
            }

            var target =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                        !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield)
                        && spells[Spells.R1].CanCast(x)
                        && x.Health + x.HPRegenRate/2 <= spells[Spells.R1].GetDamage(x));

            if (spells[Spells.R1].IsReady() && spells[Spells.R1].CanCast(target))
            {
                spells[Spells.R1].Cast(target);
            }
        }

        private static void LaneClear()
        {
            var useQ = getCheckBoxItem(laneclearMenu, "useQFarm");
            var useE = getCheckBoxItem(laneclearMenu, "useEFarm");
            var useR = getCheckBoxItem(laneclearMenu, "useRFarm");
            var countMinions = getSliderItem(laneclearMenu, "ElCorki.Count.Minions");
            var countMinionsE = getSliderItem(laneclearMenu, "ElCorki.Count.Minions.E");
            var countMinionsR = getSliderItem(laneclearMenu, "ElCorki.Count.Minions.R");
            var minmana = getSliderItem(laneclearMenu, "minmanaclear");

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);

            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.Q].GetDamage(x)))
                {
                    var killcount = 0;

                    foreach (var cminion in minions)
                    {
                        if (cminion.Health <= spells[Spells.Q].GetDamage(cminion))
                        {
                            killcount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (killcount >= countMinions)
                    {
                        spells[Spells.Q].Cast(minion);
                    }
                }
            }

            if (!useE || !spells[Spells.E].IsReady())
            {
                return;
            }

            var minionkillcount =
                minions.Count(x => spells[Spells.E].CanCast(x) && x.Health <= spells[Spells.E].GetDamage(x));

            if (minionkillcount >= countMinionsE)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.E].GetDamage(x)))
                {
                    spells[Spells.E].Cast();
                }
            }

            if (!useR || !spells[Spells.R1].IsReady())
            {
                return;
            }

            var rMinionkillcount =
                minions.Count(x => spells[Spells.R1].CanCast(x) && x.Health <= spells[Spells.R1].GetDamage(x));

            if (rMinionkillcount >= countMinionsR)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.R1].GetDamage(x)))
                {
                    spells[Spells.R1].Cast(minion);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Physical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo(target);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass(target);
            }

            KsMode();
            AutoHarassMode();
            JungleStealMode();

            spells[Spells.R1].Range = ObjectManager.Player.HasBuff("corkimissilebarragecounterbig")
                ? spells[Spells.R2].Range
                : spells[Spells.R1].Range;
        }

        #endregion
    }
}