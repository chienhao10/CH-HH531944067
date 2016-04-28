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
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Maokai
    {
        public static Menu config;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, Qint, W, E, R;
        public static bool turnOff;

        public static Menu drawMenu, comboMenu, harassMenu, laneClearMenu, miscMenu;

        public Maokai()
        {
            InitMao();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
        }

        private static bool maoR
        {
            get { return player.Buffs.Any(buff => buff.Name == "MaokaiDrain3"); }
        }

        private static int maoRStack
        {
            get { return R.Instance.Ammo; }
        }

        private void OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(miscMenu, "useQint"))
            {
                if (Qint.CanCast(sender))
                {
                    Q.Cast(sender, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(miscMenu, "useQgc"))
            {
                if (gapcloser.Sender.IsValidTarget(Qint.Range) && Q.IsReady())
                {
                    Q.Cast(gapcloser.End, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            AutoE();
        }

        private void AutoE()
        {
            if (getCheckBoxItem(miscMenu, "autoe") && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (E.CanCast(target) &&
                    (target.HasBuff("zhonyasringshield") || target.HasBuffOfType(BuffType.Snare) ||
                     target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Stun) ||
                     target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Fear)))
                {
                    E.Cast(target);
                }
            }
        }

        private void Clear()
        {
            var perc = getSliderItem(laneClearMenu, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var bestPositionE =
                E.GetCircularFarmLocation(MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly));
            var bestPositionQ =
                Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (getCheckBoxItem(laneClearMenu, "useeLC") && E.IsReady() &&
                bestPositionE.MinionsHit > getSliderItem(laneClearMenu, "ehitLC"))
            {
                E.Cast(bestPositionE.Position, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(laneClearMenu, "useqLC") && Q.IsReady() &&
                bestPositionQ.MinionsHit > getSliderItem(laneClearMenu, "qhitLC"))
            {
                Q.Cast(bestPositionQ.Position, getCheckBoxItem(config, "packets"));
            }
        }

        private void Harass()
        {
            var perc = getSliderItem(harassMenu, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "useqH") && Q.CanCast(target))
            {
                Q.Cast(target, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(harassMenu, "useeH") && E.CanCast(target))
            {
                E.Cast(target, getCheckBoxItem(config, "packets"));
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
            {
                if (maoR)
                {
                    if (!turnOff)
                    {
                        turnOff = true;
                        Utility.DelayAction.Add(2600, turnOffUlt);
                    }
                }
                return;
            }
            if (getCheckBoxItem(comboMenu, "selected"))
            {
                target = CombatHelper.SetTarget(target, TargetSelector.SelectedTarget);
                Orbwalker.ForcedTarget = target;
            }
            var manaperc = player.Mana/player.MaxMana*100;
            if (player.HasBuff("MaokaiSapMagicMelee") &&
                player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) + 75)
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "useq") && Q.CanCast(target) &&
                getCheckBoxItem(comboMenu, "usee") &&
                player.Distance(target) <= getSliderItem(comboMenu, "useqrange") &&
                ((getCheckBoxItem(comboMenu, "useqroot") && !target.HasBuffOfType(BuffType.Snare) &&
                  !target.HasBuffOfType(BuffType.Slow) && !target.HasBuffOfType(BuffType.Stun) &&
                  !target.HasBuffOfType(BuffType.Suppression)) ||
                 !getCheckBoxItem(comboMenu, "useqroot")))
            {
                Q.Cast(target, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(comboMenu, "usew"))
            {
                if (getCheckBoxItem(comboMenu, "blocke") && player.Distance(target) < W.Range && W.IsReady() &&
                    E.CanCast(target))
                {
                    E.Cast(target, getCheckBoxItem(config, "packets"));
                    CastR(target);
                    Utility.DelayAction.Add(100, () => W.Cast(target, getCheckBoxItem(config, "packets")));
                }
                else if (W.CanCast(target))
                {
                    CastR(target);
                    W.Cast(target, getCheckBoxItem(config, "packets"));
                }
            }
            if (getCheckBoxItem(comboMenu, "usee") && E.CanCast(target))
            {
                if (!getCheckBoxItem(comboMenu, "blocke") ||
                    getCheckBoxItem(comboMenu, "blocke") && !W.IsReady())
                {
                    E.Cast(target, getCheckBoxItem(config, "packets"));
                }
            }

            if (R.IsReady())
            {
                var enoughEnemies = getSliderItem(comboMenu, "user") <=
                                    player.CountEnemiesInRange(R.Range - 50);
                var targetR = TargetSelector.GetTarget(R.Range, DamageType.Magical);

                if (maoR && targetR != null &&
                    ((getCheckBoxItem(comboMenu, "rks") &&
                      player.LSGetSpellDamage(targetR, SpellSlot.R) +
                      player.CalcDamage(target, DamageType.Magical, maoRStack) > targetR.Health) ||
                     manaperc < getSliderItem(comboMenu, "rmana") ||
                     (!enoughEnemies && player.Distance(targetR) > R.Range - 50)))
                {
                    R.Cast(getCheckBoxItem(config, "packets"));
                }

                if (targetR != null && !maoR && manaperc > getSliderItem(comboMenu, "rmana") &&
                    (enoughEnemies || R.IsInRange(targetR)))
                {
                    R.Cast(getCheckBoxItem(config, "packets"));
                }
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(comboMenu, "useIgnite") && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private void turnOffUlt()
        {
            turnOff = false;
            if (maoR && getSliderItem(comboMenu, "user") > player.CountEnemiesInRange(R.Range - 50))
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void CastR(AIHeroClient target)
        {
            if (R.IsReady() && !maoR &&
                player.Mana/player.MaxMana*100 > getSliderItem(comboMenu, "rmana") &&
                getSliderItem(comboMenu, "user") <= target.CountEnemiesInRange(R.Range - 50))
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawqq"), Q.Range, Color.FromArgb(180, 200, 46, 66));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawww"), W.Range, Color.FromArgb(180, 200, 46, 66));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawee"), E.Range, Color.FromArgb(180, 200, 46, 66));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawrr"), R.Range, Color.FromArgb(180, 200, 46, 66));
        }

        private void InitMao()
        {
            Q = new Spell(SpellSlot.Q, 600);
            Q.SetSkillshot(0.50f, 110f, 1200f, false, SkillshotType.SkillshotLine);
            Qint = new Spell(SpellSlot.Q, 250f);
            W = new Spell(SpellSlot.W, 500);
            E = new Spell(SpellSlot.E, 1100);
            E.SetSkillshot(1f, 250f, 1500f, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 450);
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

        private void InitMenu()
        {
            config = MainMenu.AddMenu("Maokai", "Maokai");

            // Draw settings
            drawMenu = config.AddSubMenu("Drawings ", "dsettings");
            drawMenu.Add("drawqq", new CheckBox("Draw Q range"));
            drawMenu.Add("drawww", new CheckBox("Draw W range"));
            drawMenu.Add("drawee", new CheckBox("Draw E range"));
            drawMenu.Add("drawrr", new CheckBox("Draw R range"));

            // Combo settings
            comboMenu = config.AddSubMenu("Combo ", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("useqroot", new CheckBox("Q : Only Immobile/CC'd"));
            comboMenu.Add("useqrange", new Slider("Q max range", (int) Q.Range, 0, (int) Q.Range));
            comboMenu.Add("usew", new CheckBox("Use W"));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("blocke", new CheckBox("EW Combo if possible"));
            comboMenu.Add("user", new Slider("Use R min", 1, 1, 5));
            comboMenu.Add("rks", new CheckBox("R : Deactivate to KS target"));
            comboMenu.Add("rmana", new Slider("R : Deactivate min mana", 20));
            comboMenu.Add("selected", new CheckBox("Focus Selected target"));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            harassMenu = config.AddSubMenu("Harass ", "Hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q"));
            harassMenu.Add("useeH", new CheckBox("Use E"));
            harassMenu.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            laneClearMenu = config.AddSubMenu("LaneClear ", "Lcsettings");
            laneClearMenu.Add("useqLC", new CheckBox("Use Q"));
            laneClearMenu.Add("qhitLC", new Slider("Q : More than x minion", 2, 1, 10));
            laneClearMenu.Add("useeLC", new CheckBox("Use E"));
            laneClearMenu.Add("ehitLC", new Slider("E : More than x minion", 2, 1, 10));
            laneClearMenu.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Settings
            miscMenu = config.AddSubMenu("Misc ", "Msettings");
            miscMenu.Add("autoe", new CheckBox("Auto E target (Stun/snare...)"));
            miscMenu.Add("useQgc", new CheckBox("Use Q on gapclosers", false));
            miscMenu.Add("useQint", new CheckBox("Use W to interrupt"));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}