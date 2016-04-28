using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Damage = LeagueSharp.Common.Damage;
using Environment = UnderratedAIO.Helpers.Environment;
using Spell = LeagueSharp.Common.Spell;

namespace UnderratedAIO.Champions
{
    internal class Garen
    {
        public static Menu config, drawMenu, comboMenu, laneClearMenu, miscMenu;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static IncomingDamage IncDamages = new IncomingDamage();

        public static int[] spins = {5, 6, 7, 8, 9, 10};
        public static double[] baseEDamage = {15, 18.8, 22.5, 26.3, 30};
        public static double[] bonusEDamage = {34.5, 35.3, 36, 36.8, 37.5};

        private static bool GarenE
        {
            get { return player.Buffs.Any(buff => buff.Name == "GarenE"); }
        }

        private static bool GarenQ
        {
            get { return player.Buffs.Any(buff => buff.Name == "GarenQ"); }
        }

        public static void OnLoad()
        {
            InitGaren();
            InitMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPostAttack += AfterAttack;
            Drawing.OnDraw += Game_OnDraw;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (GarenE)
            {
                Orbwalker.DisableMovement = true;
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }
            else
            {
                Orbwalker.DisableMovement = false;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                Clear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                Combo();
            }
        }

        private static void Clear()
        {
            if (getCheckBoxItem(laneClearMenu, "useeLC") && E.IsReady() && !GarenE &&
                Environment.Minion.countMinionsInrange(player.Position, E.Range) > 2)
            {
                E.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Q.IsReady() && getCheckBoxItem(miscMenu, "useqAAA") && !GarenE && target.IsEnemy &&
                target is AIHeroClient)
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
                Player.IssueOrder(GameObjectOrder.AutoAttack, target);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(700, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (getCheckBoxItem(comboMenu, "useIgnite") && hasIgnite &&
                ((R.IsReady() && ignitedmg + R.GetDamage(target) > target.Health) || ignitedmg > target.Health) &&
                (target.Distance(player) > E.Range || player.HealthPercent < 20))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (getCheckBoxItem(comboMenu, "useq") && Q.IsReady() &&
                player.Distance(target) > player.AttackRange && !GarenE && !GarenQ &&
                player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target) &&
                CombatHelper.IsPossibleToReachHim(target, 0.30f, new float[5] {1.5f, 2f, 2.5f, 3f, 3.5f}[Q.Level - 1]))
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(comboMenu, "useq") && Q.IsReady() && !GarenQ &&
                (!GarenE || (Q.IsReady() && player.LSGetSpellDamage(target, SpellSlot.Q) > target.Health)))
            {
                if (GarenE)
                {
                    E.Cast(getCheckBoxItem(config, "packets"));
                }
                Q.Cast(getCheckBoxItem(config, "packets"));
                Player.IssueOrder(GameObjectOrder.AutoAttack, target);
            }
            if (getCheckBoxItem(comboMenu, "usee") && E.IsReady() && !Q.IsReady() && !GarenQ && !GarenE &&
                player.CountEnemiesInRange(E.Range) > 0)
            {
                E.Cast(getCheckBoxItem(config, "packets"));
            }
            var targHP = target.Health + 20 - CombatHelper.IgniteDamage(target);
            var rLogic = getCheckBoxItem(comboMenu, "user") && R.IsReady() && target.IsValidTarget() &&
                         (!getCheckBoxItem(miscMenu, "ult" + target.BaseSkinName) ||
                          player.CountEnemiesInRange(1500) == 1) && getRDamage(target) > targHP && targHP > 0;
            if (rLogic && target.Distance(player) < R.Range)
            {
                if (!(GarenE && target.Health < getEDamage(target, true) && target.Distance(player) < E.Range))
                {
                    if (GarenE)
                    {
                        E.Cast(getCheckBoxItem(config, "packets"));
                    }
                    else
                    {
                        R.Cast(target, getCheckBoxItem(config, "packets"));
                    }
                }
            }
            var data = IncDamages.GetAllyData(player.NetworkId);
            if (getCheckBoxItem(comboMenu, "usew") && W.IsReady() && target.IsFacing(player) &&
                data.DamageTaken > 40)
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
            var hasFlash = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready;
            if (getCheckBoxItem(comboMenu, "useFlash") && hasFlash && rLogic &&
                target.Distance(player) < R.Range + 425 && target.Distance(player) > R.Range + 250 && !Q.IsReady() &&
                !CombatHelper.IsFacing(target, player.Position) && !GarenQ)
            {
                if (target.Distance(player) < R.Range + 300 && player.MoveSpeed > target.MoveSpeed)
                {
                    return;
                }
                if (GarenE)
                {
                    E.Cast(getCheckBoxItem(config, "packets"));
                }
                else if (!player.Position.Extend(target.Position, 425f).IsWall())
                {
                }
                {
                    player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"),
                        player.Position.LSExtend(target.Position, 425f));
                }
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawaa"))
            {
                Render.Circle.DrawCircle(player.Position, player.AttackRange, Color.FromArgb(180, 109, 111, 126));
            }

            if (getCheckBoxItem(drawMenu, "drawee"))
            {
                Render.Circle.DrawCircle(player.Position, E.Range, Color.FromArgb(180, 109, 111, 126));
            }

            if (getCheckBoxItem(drawMenu, "drawrr"))
            {
                Render.Circle.DrawCircle(player.Position, R.Range, Color.FromArgb(180, 109, 111, 126));
            }

            if (R.IsReady() && getCheckBoxItem(drawMenu, "drawrkillable"))
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.IsValid && e.IsHPBarRendered))
                {
                    if (e.Health < getRDamage(e))
                    {
                        Render.Circle.DrawCircle(e.Position, 157, Color.Gold, 12);
                    }
                }
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (R.IsReady())
            {
                damage += getRDamage(hero);
            }

            if (Q.IsReady() && !GarenQ)
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (E.IsReady() && !GarenE)
            {
                damage += getEDamage(hero);
            }
            else if (GarenE)
            {
                damage += getEDamage(hero, true);
            }
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private static double getRDamage(AIHeroClient hero)
        {
            var dmg = new double[] {175, 350, 525}[R.Level - 1] +
                      new[] {28.57, 33.33, 40}[R.Level - 1]/100*(hero.MaxHealth - hero.Health);
            if (hero.HasBuff("garenpassiveenemytarget"))
            {
                return player.CalcDamage(hero, DamageType.True, dmg);
            }
            return player.CalcDamage(hero, DamageType.Magical, dmg);
        }

        private static double getEDamage(AIHeroClient target, bool bufftime = false)
        {
            var spins = 0d;
            if (bufftime)
            {
                spins = CombatHelper.GetBuffTime(player.GetBuff("GarenE"))*GetSpins()/3;
            }
            else
            {
                spins = GetSpins();
            }
            var dmg = (baseEDamage[E.Level - 1] + bonusEDamage[E.Level - 1]/100*player.TotalAttackDamage)*spins;
            var bonus = target.HasBuff("garenpassiveenemytarget") ? target.MaxHealth/100f*spins : 0;
            if (ObjectManager.Get<Obj_AI_Base>().Count(o => o.IsValidTarget() && o.Distance(target) < 650) == 0)
            {
                return player.CalcDamage(target, DamageType.Physical, dmg)*1.33 + bonus;
            }
            return player.CalcDamage(target, DamageType.Physical, dmg) + bonus;
        }

        private static double GetSpins()
        {
            if (player.Level < 4)
            {
                return 5;
            }
            if (player.Level < 7)
            {
                return 6;
            }
            if (player.Level < 10)
            {
                return 7;
            }
            if (player.Level < 13)
            {
                return 8;
            }
            if (player.Level < 16)
            {
                return 9;
            }
            if (player.Level < 18)
            {
                return 10;
            }
            return 5;
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

        private static void InitGaren()
        {
            Q = new Spell(SpellSlot.Q, player.AttackRange);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R, 400);
        }

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Garen", "Garen");

            // Draw settings
            drawMenu = config.AddSubMenu("Drawings ", "dsettings");
            drawMenu.Add("drawaa", new CheckBox("Draw AA range"));
            drawMenu.Add("drawee", new CheckBox("Draw E range"));
            drawMenu.Add("drawrr", new CheckBox("Draw R range"));
            drawMenu.Add("drawrkillable", new CheckBox("Show if killable with R"));

            // Combo Settings
            comboMenu = config.AddSubMenu("Combo ", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("usew", new CheckBox("Use W"));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("user", new CheckBox("Use R"));
            comboMenu.Add("useFlash", new CheckBox("Use Flash"));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            // LaneClear Settings
            laneClearMenu = config.AddSubMenu("LaneClear ", "Lcsettings");
            laneClearMenu.Add("useeLC", new CheckBox("Use E"));

            // Misc Settings
            miscMenu = config.AddSubMenu("Misc ", "Msettings");
            miscMenu.Add("useqAAA", new CheckBox("Use Q after AA"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("TeamFight Ult block");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                miscMenu.Add("ult" + hero.BaseSkinName, new CheckBox(hero.BaseSkinName));
            }

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}