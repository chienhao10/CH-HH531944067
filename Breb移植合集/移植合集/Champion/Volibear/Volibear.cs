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

namespace UnderratedAIO.Champions
{
    internal class Volibear
    {
        public static Menu config;
        public static Spell Q, W, E, R;
        public static float[] MsBuff = new float[5] {0.3f, 0.35f, 0.4f, 0.45f, 0.5f};
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static Menu menuD, menuC, menuH, menuLC, menuM;
        private bool passivecd;
        private float passivetime;

        public Volibear()
        {
            InitVolibear();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static bool QEnabled
        {
            get { return player.Buffs.Any(buff => buff.Name == "VolibearQ"); }
        }

        private static bool CanW
        {
            get { return player.Buffs.Any(buff => buff.Name == "volibearwparticle"); }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var hasbuff = player.HasBuff("volibearpassivecd");
            if (hasbuff && !passivecd)
            {
                passivecd = true;
                passivetime = Game.Time;
            }
            if (!hasbuff)
            {
                passivecd = false;
                passivetime = 0f;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }

            var enemyForKs = HeroManager.Enemies.FirstOrDefault(h => W.CanCast(h) && Wdmg(h) > h.Health);
            if (enemyForKs != null && W.IsReady() && getCheckBoxItem(menuM, "ksW"))
            {
                W.CastOnUnit(enemyForKs, getCheckBoxItem(config, "packets"));
            }
        }

        private void Harass()
        {
            var perc = getSliderItem(menuH, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuH, "usewH") && W.CanCast(target) && CanW &&
                getSliderItem(menuH, "maxHealthH")/100f*target.MaxHealth > target.Health)
            {
                W.Cast(target, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuH, "useeH") && E.CanCast(target))
            {
                E.Cast(target, getCheckBoxItem(config, "packets"));
            }
        }

        private void Clear()
        {
            var mob = Jungle.GetNearest(player.Position);
            if (mob != null && getCheckBoxItem(menuLC, "usewLCSteal") && CanW && W.CanCast(mob) &&
                player.CalcDamage(mob, DamageType.Physical, Wdmg(mob)) > mob.Health)
            {
                W.Cast(mob, getCheckBoxItem(config, "packets"));
            }
            var perc = getSliderItem(menuLC, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var minions = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (getCheckBoxItem(menuLC, "useeLC") && E.IsReady() &&
                getSliderItem(menuLC, "ehitLC") <= minions.Count)
            {
                E.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        public static float MsBonus(AIHeroClient target)
        {
            var msBonus = 1f;

            if (Q.IsReady() && !QEnabled)
            {
                if (
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(h => h.IsEnemy && player.LSDistance(h) < 2000 && player.IsFacing(h)) != null)
                {
                    msBonus += MsBuff[Q.Level - 1];
                }
                else
                {
                    msBonus += 0.15f;
                }
            }
            return msBonus;
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(1490, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuC, "selected"))
            {
                target = CombatHelper.SetTarget(target, (AIHeroClient) Orbwalker.LastTarget);
                Orbwalker.ForcedTarget = target;
            }
            if (getCheckBoxItem(menuC, "useq") && Q.IsReady() && !QEnabled &&
                player.LSDistance(target) >= getSliderItem(menuC, "useqmin") &&
                player.LSDistance(target) < player.MoveSpeed*MsBonus(target)*3.0f)
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuC, "usew") && CanW && W.CanCast(target) &&
                (player.CalcDamage(target, DamageType.Physical, Wdmg(target)) > target.Health ||
                 player.HealthPercent < 10))
            {
                W.Cast(target, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuC, "usee") && E.CanCast(target) &&
                ((getCheckBoxItem(menuC, "useenotccd") && !target.HasBuffOfType(BuffType.Snare) &&
                  !target.HasBuffOfType(BuffType.Slow) && !target.HasBuffOfType(BuffType.Stun) &&
                  !target.HasBuffOfType(BuffType.Suppression)) ||
                 !getCheckBoxItem(menuC, "useenotccd")))
            {
                E.Cast(getCheckBoxItem(config, "packets"));
            }
            if (R.IsReady() && player.HealthPercent > 20 &&
                ((getCheckBoxItem(menuC, "user") && player.LSDistance(target) < 200 &&
                  ComboDamage(target) + R.GetDamage(target)*10 > target.Health && ComboDamage(target) < target.Health) ||
                 (getSliderItem(menuC, "usertf") <= player.CountEnemiesInRange(300))))
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") && ignitedmg > target.Health && hasIgnite &&
                !W.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            var msBonus = 1f;
            if (Q.IsReady() && !QEnabled)
            {
                if (
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(h => h.IsEnemy && player.LSDistance(h) < 2000 && player.IsFacing(h)) != null)
                {
                    msBonus += MsBuff[Q.Level - 1];
                }
                else
                {
                    msBonus += 0.15f;
                }
            }
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), player.MoveSpeed*msBonus*4.0f,
                Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawww"), W.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(180, 100, 146, 166));
            if (getCheckBoxItem(menuD, "drawpass") && !player.IsDead)
            {
                DrawPassive();
            }
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawrr"), 300, Color.FromArgb(180, 100, 146, 166));
        }

        private void DrawPassive()
        {
            var baseTime = 0.3f;
            if (player.HasBuff("volibearpassivecd") && passivecd)
            {
                var time = Game.Time - passivetime;
                if (time <= 6f)
                {
                    baseTime = baseTime - time*0.05f;
                }
                else
                {
                    return;
                }
            }

            var percentHealth = Math.Max(0, player.MaxHealth - player.Health)/player.MaxHealth;
            var barPos = player.HPBarPosition;
            var xPos = barPos.X + 36 + 103*(1 - percentHealth);
            Drawing.DrawLine(xPos, barPos.Y + 9, xPos, barPos.Y + 17, -105f*baseTime, Color.FromArgb(140, 30, 197, 22));
        }

        public static double Wdmg(Obj_AI_Base target)
        {
            return (new double[] {80, 125, 170, 215, 260}[W.Level - 1] +
                    (player.MaxHealth - (498.48f + 86f*(player.Level - 1f)))*0.15f)*
                   ((target.MaxHealth - target.Health)/target.MaxHealth + 1);
        }

        private void InitVolibear()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R);
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (W.IsReady() || player.GetSpell(SpellSlot.W).State == SpellState.Surpressed)
            {
                damage += player.CalcDamage(hero, DamageType.Physical, Wdmg(hero));
            }
            if (E.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.E);
            }
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite))
            {
                damage += player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float) damage;
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
            config = MainMenu.AddMenu("Volibear", "Volibear");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawww", new CheckBox("Draw W range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawrr", new CheckBox("Draw R range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawpass", new CheckBox("Draw passive"));
                //.SetValue(new Circle(true, Color.FromArgb(140, 30, 197, 22)));

            // Combo settings
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("useqmin", new Slider("1 : Min distance", 200, 0, 1000));
            menuC.Add("usew", new CheckBox("Use W"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("useenotccd", new CheckBox("E : Wait if the target stunned, slowed..."));
            menuC.Add("user", new CheckBox("Use R (1v1)"));
            menuC.Add("usertf", new Slider("Use R min (teamfight)", 2, 1, 5));
            menuC.Add("selected", new CheckBox("Focus Selected target"));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("usewH", new CheckBox("Use W"));
            menuH.Add("maxHealthH", new Slider("Target health less than", 50, 1));
            menuH.Add("useeH", new CheckBox("Use E"));
            menuH.Add("minmanaH", new Slider("Keep X% mana"));

            // LaneClear Settings
            menuLC = config.AddSubMenu("Clear ", "Lcsettings");
            menuLC.Add("usewLCSteal", new CheckBox("Use W to steal in jungle"));
            menuLC.Add("useeLC", new CheckBox("Use E"));
            menuLC.Add("ehitLC", new Slider("E : More than x minion", 2, 1, 10));
            menuLC.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc settings
            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("ksW", new CheckBox("KS with W", false));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}