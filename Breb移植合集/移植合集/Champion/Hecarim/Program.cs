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

namespace JustHecarim
{
    internal class Program
    {
        public const string ChampName = "Hecarim";
        public const string Menuname = "JustHecarim";
        public static Menu Config, comboMenu, harassMenu, laneClear, drawMenu, miscMenu;
        public static Spell Q, W, E, R;
        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;
        private static readonly AIHeroClient player = ObjectManager.Player;


        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range) && getCheckBoxItem(miscMenu, "interrupte"))
            {
                E.Cast();
            }

            if (R.IsReady() && sender.IsValidTarget(R.Range) && getCheckBoxItem(miscMenu, "interruptr"))
            {
                var pred = R.GetPrediction(sender).Hitchance;
                if (pred >= HitChance.High)
                    R.Cast(sender);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(player.AttackRange) &&
                getCheckBoxItem(miscMenu, "antigap"))
                E.Cast();
        }

        private static void AutoR()
        {
            if (!R.IsReady() || !getCheckBoxItem(comboMenu, "AutoR"))
                return;

            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            var enemys = target.CountEnemiesInRange(R.Range);
            if (R.IsReady() && getCheckBoxItem(comboMenu, "UseR") && target.IsValidTarget(R.Range))
            {
                var pred = R.GetPrediction(target).Hitchance;
                if (pred >= HitChance.High)
                    R.CastIfWillHit(target, enemys);
            }
        }

        private static void combo()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            var enemys = target.CountEnemiesInRange(R.Range);

            if (E.IsReady() && target.IsValidTarget(2000) && getCheckBoxItem(comboMenu, "UseE"))
                E.Cast();

            if (W.IsReady() && target.IsValidTarget(W.Range) && getCheckBoxItem(comboMenu, "UseW"))
                W.Cast();

            if (Q.IsReady() && getCheckBoxItem(comboMenu, "UseQ") && target.IsValidTarget(Q.Range))
            {
                Q.Cast();
            }

            if (R.IsReady() && getCheckBoxItem(comboMenu, "UseR") && target.IsValidTarget(R.Range))
                if (getSliderItem(comboMenu, "Rene") <= enemys)
                    R.CastIfHitchanceEquals(target, HitChance.High);
        }

        private static float GetComboDamage(AIHeroClient Target)
        {
            if (Target != null)
            {
                var ComboDamage = new float();

                ComboDamage = Q.IsReady() ? Q.GetDamage(Target) : 0;
                ComboDamage += W.IsReady() ? W.GetDamage(Target) : 0;
                ComboDamage += R.IsReady() ? R.GetDamage(Target) : 0;
                ComboDamage += player.TotalAttackDamage;
                return ComboDamage;
            }
            return 0;
        }

        private static float[] GetLength()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target != null)
            {
                float[] Length =
                {
                    GetComboDamage(Target) > Target.Health
                        ? 0
                        : (Target.Health - GetComboDamage(Target))/Target.MaxHealth,
                    Target.Health/Target.MaxHealth
                };
                return Length;
            }
            return new float[] {0, 0};
        }

        private static void Killsteal()
        {
            if (getCheckBoxItem(miscMenu, "ksQ") && Q.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(Q.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.Q));
                if (target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
            }

            if (getCheckBoxItem(miscMenu, "ksR") && R.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(R.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.R));
                if (target.IsValidTarget(R.Range))
                {
                    R.CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Lasthit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            Killsteal();
            AutoR();
            AutoHarass();
        }

        private static void Lasthit()
        {
            var minions = MinionManager.GetMinions(player.ServerPosition, Q.Range);
            if (minions.Count <= 0)
                return;

            if (Q.IsReady() && getCheckBoxItem(laneClear, "fQ"))
            {
                var qtarget =
                    minions.Where(
                        x =>
                            x.Distance(player) < Q.Range && x.Health < player.GetSpellDamage(x, SpellSlot.Q) &&
                            !(x.Health < player.GetAutoAttackDamage(x)))
                        .OrderByDescending(x => x.Health)
                        .FirstOrDefault();
                if (HealthPrediction.GetHealthPrediction(qtarget, (int) 0.5) <=
                    player.GetSpellDamage(qtarget, SpellSlot.Q))
                    Q.Cast();
            }
        }

        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!Q.IsReady() || !getCheckBoxItem(harassMenu, "hQA") || player.IsRecalling() || target == null ||
                !target.IsValidTarget())
                return;

            if (Q.IsReady() && getCheckBoxItem(harassMenu, "hQA") && target.IsValidTarget(Q.Range - 10))
            {
                Q.Cast();
            }
        }


        private static void harass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var harassmana = getSliderItem(harassMenu, "harassmana");
            if (target == null || !target.IsValidTarget())
                return;

            if (E.IsReady() && player.ManaPercent >= harassmana &&
                getCheckBoxItem(harassMenu, "hE"))
                E.Cast();

            if (W.IsReady() && target.IsValidTarget(W.Range) && player.ManaPercent >= harassmana &&
                getCheckBoxItem(harassMenu, "hW"))
                W.Cast();

            if (getCheckBoxItem(harassMenu, "hQ") && target.IsValidTarget(Q.Range) &&
                player.ManaPercent >= harassmana)
                Q.Cast();
        }

        private static void Clear()
        {
            var minionObj = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);
            var lanemana = getSliderItem(laneClear, "lanemana");

            if (!minionObj.Any())
            {
                return;
            }

            if (player.ManaPercent >= lanemana)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                    getCheckBoxItem(laneClear, "laneQ"))
                {
                    Q.Cast();
                }
            }

            if (player.ManaPercent >= lanemana)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                    getCheckBoxItem(laneClear, "laneE"))
                {
                    E.Cast();
                }
            }

            if (minionObj.Count > getSliderItem(laneClear, "wmin") && player.ManaPercent >= lanemana)
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                    getCheckBoxItem(laneClear, "laneW"))
                {
                    {
                        W.Cast();
                    }
                }
        }

        private static void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (getCheckBoxItem(drawMenu, "Draw_Disabled"))
                return;

            if (getCheckBoxItem(drawMenu, "Qdraw"))
                Render.Circle.DrawCircle(player.Position, Q.Range, Color.White, 3);
            if (getCheckBoxItem(drawMenu, "Wdraw"))
                Render.Circle.DrawCircle(player.Position, W.Range, Color.White, 3);
            if (getCheckBoxItem(drawMenu, "Rdraw"))
                Render.Circle.DrawCircle(player.Position, R.Range, Color.White, 3);
            if (Target == null)
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "combodamage") && Q.IsInRange(Target))
            {
                var Positions = GetLength();
                Drawing.DrawLine
                    (
                        new Vector2(Target.HPBarPosition.X + 10 + Positions[0]*104, Target.HPBarPosition.Y + 20),
                        new Vector2(Target.HPBarPosition.X + 10 + Positions[1]*104, Target.HPBarPosition.Y + 20),
                        9,
                        Color.Orange
                    );
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

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void OnLoad()
        {
            if (player.ChampionName != ChampName)
                return;

            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q, 350);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);


            abilitySequence = new[] {1, 3, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 3, 2, 3, 4, 3, 3};

            Config = MainMenu.AddMenu(Menuname, Menuname);

            //Combo
            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQ", new CheckBox("Use Q"));
            comboMenu.Add("UseW", new CheckBox("Use W"));
            comboMenu.Add("UseE", new CheckBox("Use E"));
            comboMenu.Add("UseR", new CheckBox("Use R"));
            comboMenu.Add("Rene", new Slider("Min Enemies for R", 2, 1, 5));
            comboMenu.Add("AutoR", new CheckBox("Auto R", false));
            comboMenu.Add("Renem", new Slider("Min Enemies for Auto R", 3, 1, 5));

            //Harass
            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("hQ", new CheckBox("Use Q"));
            harassMenu.Add("hQA", new CheckBox("Use Auto Q Harass"));
            harassMenu.Add("hW", new CheckBox("Use W", false));
            harassMenu.Add("hE", new CheckBox("Use E"));
            harassMenu.Add("harassmana", new Slider("Mana Percentage", 30));

            //Laneclear
            laneClear = Config.AddSubMenu("Clear", "Clear");
            laneClear.Add("laneQ", new CheckBox("Use Q"));
            laneClear.Add("fQ", new CheckBox("Farm with Q ( While pressing last hit )"));
            laneClear.Add("laneE", new CheckBox("Use E"));
            laneClear.Add("laneW", new CheckBox("Use W"));
            laneClear.Add("wmin", new Slider("Min Minion for W", 3, 1, 5));
            laneClear.Add("lanemana", new Slider("Mana Percentage", 30));

            //Draw
            drawMenu = Config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("Draw_Disabled", new CheckBox("Disable All Spell Drawings", false));
            drawMenu.Add("Qdraw", new CheckBox("Draw Q Range"));
            drawMenu.Add("Wdraw", new CheckBox("Draw W Range"));
            drawMenu.Add("Rdraw", new CheckBox("Draw R Range"));
            drawMenu.Add("combodamage", new CheckBox("Damage on HPBar"));

            //Misc
            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ksQ", new CheckBox("Killsteal with Q"));
            miscMenu.Add("ksR", new CheckBox("Killsteal with R", false));
            miscMenu.Add("antigap", new CheckBox("AntiGapCloser with E", false));
            miscMenu.Add("interrupte", new CheckBox("Interrupt with E"));
            miscMenu.Add("interruptr", new CheckBox("Interrupt with R"));

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }
    }
}