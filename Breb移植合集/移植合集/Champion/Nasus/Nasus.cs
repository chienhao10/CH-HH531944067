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
    internal class Nasus
    {
        public static Menu config;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static Menu menuD, menuC, menuH, menuLC, menuM;

        public Nasus()
        {
            InitNocturne();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Orbwalker.OnPostAttack += Orbwalking_OnAttack;
            Orbwalker.OnPreAttack += Orbwalking_AfterAttack;
        }

        private static bool NasusQ
        {
            get { return player.Buffs.Any(buff => buff.Name == "NasusQ"); }
        }

        private void Orbwalking_AfterAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var tar = target as Obj_AI_Base;
            if (Q.IsReady() && tar is AIHeroClient &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && tar.HasBuffOfType(BuffType.Slow) &&
                target.Health > Q.GetDamage(tar) + player.GetAutoAttackDamage(tar) + 50)
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
                Orbwalker.ResetAutoAttack();
            }
        }

        private void Orbwalking_OnAttack(AttackableUnit target, EventArgs args)
        {
            if (Q.IsReady() &&
                ((getCheckBoxItem(menuM, "autoQ") &&
                  target.Health < Q.GetDamage((Obj_AI_Base) target) + player.GetAutoAttackDamage((Obj_AI_Base) target)) ||
                 (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && target.Health > 1000)))
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var targeta = args.Target as Obj_AI_Base;
            if (Q.IsReady() && targeta != null &&
                ((targeta is AIHeroClient && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                  !targeta.HasBuffOfType(BuffType.Slow)) ||
                 targeta.Health < Q.GetDamage(targeta) + player.GetAutoAttackDamage(targeta)))
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(950, DamageType.Physical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo(target);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass(target);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                if (Q.IsReady())
                {
                    useQ();
                }
            }
        }

        private void Combo(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            var cmbdmg = ComboDamage(target);
            if (!getCheckBoxItem(menuM, "Rdamage"))
            {
                cmbdmg += R.GetDamage(target)*15;
            }
            var bonusDmg = Environment.Hero.GetAdOverTime(player, target, 5);
            if ((getCheckBoxItem(menuC, "user") && player.Distance(target) < player.AttackRange + 50 &&
                 cmbdmg + bonusDmg > target.Health && target.Health > bonusDmg + 200 && player.HealthPercent < 50) ||
                (getSliderItem(menuC, "usertf") <= player.CountEnemiesInRange(600) &&
                 player.HealthPercent < 80))
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (getCheckBoxItem(menuC, "usew") && W.CanCast(target))
            {
                if (((getCheckBoxItem(menuC, "keepManaForR") && R.IsReady()) || !R.IsReady()) &&
                    player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana)
                {
                    W.Cast(target, getCheckBoxItem(config, "packets"));
                }
            }
            if (((getCheckBoxItem(menuC, "keepManaForR") && R.IsReady()) || !R.IsReady()) &&
                (player.Mana > R.Instance.SData.Mana + E.Instance.SData.Mana ||
                 (E.IsReady() && E.GetDamage(target) > target.Health)))
            {
                if (getCheckBoxItem(menuC, "usee") && E.IsReady() &&
                    ((getCheckBoxItem(menuC, "useeslow") && NasusW(target)) ||
                     !getCheckBoxItem(menuC, "useeslow")))
                {
                    var ePred = E.GetPrediction(target);
                    if (E.Range > ePred.CastPosition.Distance(player.Position) &&
                        target.Distance(ePred.CastPosition) < 400)
                    {
                        E.Cast(ePred.CastPosition, getCheckBoxItem(config, "packets"));
                    }
                    else
                    {
                        if (ePred.CastPosition.Distance(player.Position) < 925 &&
                            target.Distance(ePred.CastPosition) < 400)
                        {
                            E.Cast(
                                player.Position.Extend(target.Position, E.Range),
                                getCheckBoxItem(config, "packets"));
                        }
                    }
                }
            }
        }

        private void Clear()
        {
            if (Q.IsReady())
            {
                useQ();
            }
            if (NasusQ && player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(player)) == 0)
            {
                var minion =
                    MinionManager.GetMinions(
                        Orbwalking.GetRealAutoAttackRange(player), MinionTypes.All, MinionTeam.NotAlly)
                        .FirstOrDefault(m => m.Health > 5 && m.Health < Q.GetDamage(m) + player.GetAutoAttackDamage(m));
                Orbwalker.ForcedTarget = minion;
            }
            var perc = getSliderItem(menuLC, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var bestPositionE =
                E.GetCircularFarmLocation(MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly), 400f);
            if (getCheckBoxItem(menuLC, "useeLC") && Q.IsReady() &&
                bestPositionE.MinionsHit >= getSliderItem(menuLC, "ehitLC"))
            {
                E.Cast(bestPositionE.Position, getCheckBoxItem(config, "packets"));
            }
        }

        private void useQ()
        {
            var minions =
                MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(player), MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(m => m.Health > 5 && m.Health < Q.GetDamage(m) + player.GetAutoAttackDamage(m));
            if (minions != null)
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Harass(AIHeroClient target)
        {
            if (Q.IsReady())
            {
                useQ();
            }
            var perc = getSliderItem(menuH, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuH, "useeH") && E.IsReady())
            {
                var ePred = E.GetPrediction(target);
                if (E.Range > ePred.CastPosition.Distance(player.Position) && target.Distance(ePred.CastPosition) < 400)
                {
                    E.Cast(ePred.CastPosition, getCheckBoxItem(config, "packets"));
                }
                else
                {
                    if (ePred.CastPosition.Distance(player.Position) < 925 && target.Distance(ePred.CastPosition) < 400)
                    {
                        E.Cast(
                            player.Position.Extend(target.Position, E.Range), getCheckBoxItem(config, "packets"));
                    }
                }
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (E.IsReady() && E.Instance.SData.Mana < player.Mana)
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.E);
            }
            if (R.IsReady() && getCheckBoxItem(menuM, "Rdamage"))
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.R)*15;
            }
            if (Q.IsReady() && getCheckBoxItem(menuM, "Qdamage"))
            {
                damage += Q.GetDamage(hero) + player.GetAutoAttackDamage(hero);
            }
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawww"), W.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawrr"), R.Range, Color.FromArgb(180, 100, 146, 166));
        }

        private void InitNocturne()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(
                E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false,
                SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 350f);
        }

        private static bool NasusW(AIHeroClient target)
        {
            return target.Buffs.Any(buff => buff.Name == "NasusW");
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
            config = MainMenu.AddMenu("Nasus ", "Nasus");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawww", new CheckBox("Draw W range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawrr", new CheckBox("Draw R range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));

            // Combo Settings
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usew", new CheckBox("Use W"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("keepManaForR", new CheckBox("Keep mana for R"));
            menuC.Add("useeslow", new CheckBox("Only for slowed enemy", false));
            menuC.Add("user", new CheckBox("Use R in 1v1"));
            menuC.Add("usertf", new Slider("R min enemy in teamfight", 2, 1, 5));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useeH", new CheckBox("Use E"));
            menuH.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useeLC", new CheckBox("Use E"));
            menuLC.Add("ehitLC", new Slider("E : Min hit", 4, 1, 10));
            menuLC.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Menu
            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("autoQ", new CheckBox("Auto Q"));
            menuM.Add("Rdamage", new CheckBox("Combo damage with R"));
            menuM.Add("Qdamage", new CheckBox("Combo damage with Q"));

            // Config
            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}