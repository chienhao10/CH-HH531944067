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
    internal class Skarner
    {
        public static Menu config;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static Menu menuD, menuC, menuH, menuLC, menuM;

        public Skarner()
        {
            InitSkarner();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static bool SkarnerR
        {
            get { return player.Buffs.Any(buff => buff.Name == "skarnerimpalevo"); }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(menuM, "Interrupt") && R.CanCast(sender))
            {
                R.CastOnUnit(sender);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (SkarnerR)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            else
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

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
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }

            var dist = player.LSDistance(target);
            if (getCheckBoxItem(menuC, "useq") && player.CountEnemiesInRange(Q.Range) > 0)
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }

            if (getCheckBoxItem(menuC, "usew") || player.LSDistance(target) < 600)
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }

            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") && ignitedmg > target.Health && hasIgnite && !E.CanCast(target) && (target.LSDistance(player) >= Q.Range || (target.LSDistance(player) <= Q.Range && player.HealthPercent < 30)))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }

            if (SkarnerR)
            {
                return;
            }

            if (getCheckBoxItem(menuC, "usee") && E.CanCast(target) && ((dist < getSliderItem(menuC, "useeMaxRange") && dist > getSliderItem(menuC, "useeMinRange")) || target.Health < ComboDamage(target)))
            {
                E.Cast(target, getCheckBoxItem(config, "packets"));
            }

            if (getCheckBoxItem(menuC, "user") && R.CanCast(target) && (!getCheckBoxItem(menuC, "ult" + target.BaseSkinName) || player.CountEnemiesInRange(1500) == 1) && !target.HasBuffOfType(BuffType.Stun) && !target.HasBuffOfType(BuffType.Snare) && !E.IsReady() && ((player.HealthPercent < 50 && target.HealthPercent < 50) || player.CountAlliesInRange(1000) > 1))
            {
                R.Cast(target, getCheckBoxItem(config, "packets"));
            }
        }

        private void Clear()
        {
            var perc = getSliderItem(menuLC, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var bestPositionE =
                E.GetLineFarmLocation(MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly));
            var qMinions = Environment.Minion.countMinionsInrange(player.Position, Q.Range);
            if (getCheckBoxItem(menuLC, "useeLC") && E.IsReady() &&
                bestPositionE.MinionsHit > getSliderItem(menuLC, "ehitLC"))
            {
                E.Cast(bestPositionE.Position, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuLC, "useqLC") && Q.IsReady() &&
                qMinions >= getSliderItem(menuLC, "qhitLC"))
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Harass()
        {
            var perc = getSliderItem(menuH, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuH, "useqH") && Q.CanCast(target))
            {
                Q.Cast(getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuH, "useeH") && E.CanCast(target))
            {
                E.Cast(target, getCheckBoxItem(config, "packets"));
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.R)*2;
            }
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (getCheckBoxItem(menuC, "useragainstpush") &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var spellName = args.SData.Name;
                if (spellName == "TristanaR" || spellName == "BlindMonkRKick" || spellName == "AlZaharNetherGrasp" ||
                    spellName == "GalioIdolOfDurand" || spellName == "VayneCondemn" ||
                    spellName == "JayceThunderingBlow" || spellName == "Headbutt")
                {
                    if (args.Target.IsMe && R.CanCast(sender) &&
                        (!getCheckBoxItem(menuC, "ult" + sender.BaseSkinName) || player.CountEnemiesInRange(1500) == 1))
                    {
                        R.Cast(sender, getCheckBoxItem(config, "packets"));
                    }
                }
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawrr"), R.Range, Color.FromArgb(180, 100, 146, 166));
        }

        private void InitSkarner()
        {
            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 985);
            E.SetSkillshot(0.25f, 70, 1500, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 325);
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
            config = MainMenu.AddMenu("Skarner", "Skarner");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawrr", new CheckBox("Draw R range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));

            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usew", new CheckBox("Use W"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("useeMinRange", new Slider("   E min Range", 150, 0, (int) E.Range));
            menuC.Add("useeMaxRange", new Slider("   E max Range", 800, 0, (int) E.Range));
            menuC.Add("user", new CheckBox("Use R"));
            menuC.Add("useragainstpush", new CheckBox("Use R to counter spells"));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));
            menuC.AddGroupLabel("Team Fight Ult Block :");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                menuC.Add("ult" + hero.BaseSkinName, new CheckBox(hero.BaseSkinName, false));
            }

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q"));
            menuH.Add("useeH", new CheckBox("Use E"));
            menuH.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));
            menuLC.Add("qhitLC", new Slider("   Min hit", 2, 1, 10));
            menuLC.Add("useeLC", new CheckBox("Use E"));
            menuLC.Add("ehitLC", new Slider("   Min hit", 2, 1, 10));
            menuLC.Add("minmana", new Slider("Keep X% mana", 1, 1));

            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("Interrupt", new CheckBox("Use R interrupt"));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}