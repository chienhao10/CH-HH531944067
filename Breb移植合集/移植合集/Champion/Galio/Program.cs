using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Galio
    {
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justR, justQ, justE;
        public static IncomingDamage IncDamages = new IncomingDamage();

        public static Menu config, drawMenu, comboMenu, harassMenu, laneClearMenu, miscMenu;

        private static bool rActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "GalioIdolOfDurand"); }
        }

        public static void OnLoad()
        {
            InitGalio();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R && W.IsReady())
            {
                if (player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana)
                {
                    W.Cast(player, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && getCheckBoxItem(miscMenu, "Interrupt") && sender.Distance(player) < R.Range)
            {
                CastR();
            }
        }

        private static void InitGalio()
        {
            Q = new Spell(SpellSlot.Q, 940);
            Q.SetSkillshot(0.25f, 125, 1300, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 1180);
            E.SetSkillshot(0.25f, 140, 1200, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 575);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getCheckBoxItem(miscMenu, "AutoW") && W.IsReady())
            {
                CastW(false);
            }
            if (rActive || justR)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                return;
            }
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;

            if (getKeyBindItem(comboMenu, "manualRflash"))
            {
                FlashCombo();
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

        private static bool CheckAutoW()
        {
            return getSliderItem(miscMenu, "AutoWmana") < player.ManaPercent &&
                   getSliderItem(miscMenu, "AutoWhealth") > player.HealthPercent;
        }

        private static void FlashCombo()
        {
            if (R.IsReady() && player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready)
            {
                var points = CombatHelper.PointsAroundTheTarget(player.Position, 425);
                var best =
                    points.Where(
                        p =>
                            !p.IsWall() && p.Distance(player.Position) > 200 && p.Distance(player.Position) < 425 &&
                            p.IsValid() && p.CountEnemiesInRange(R.Range) > 0 &&
                            getSliderItem(comboMenu, "Rminflash") <= p.CountEnemiesInRange(R.Range - 150))
                        .OrderByDescending(p => p.CountEnemiesInRange(R.Range - 100))
                        .FirstOrDefault();
                if (best.CountEnemiesInRange(R.Range - 150) > player.CountEnemiesInRange(R.Range) &&
                    CombatHelper.CheckInterrupt(best, R.Range))
                {
                    player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"), best);
                    Utility.DelayAction.Add(50, () => { R.Cast(getCheckBoxItem(config, "packets")); });
                    justR = true;
                    Utility.DelayAction.Add(200, () => justR = false);
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                    return;
                }
            }
            if (!rActive)
            {
                if (!justR)
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                    Combo();
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var perc = getSliderItem(harassMenu, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc || target == null)
            {
                return;
            }
            var hitC = HitChance.High;
            if (getCheckBoxItem(miscMenu, "useHigherHit"))
            {
                hitC = HitChance.VeryHigh;
            }
            if (getCheckBoxItem(harassMenu, "useqH") && Q.CanCast(target))
            {
                Q.CastIfHitchanceEquals(target, hitC, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(harassMenu, "useeH") && E.CanCast(target))
            {
                E.CastIfHitchanceEquals(target, hitC, getCheckBoxItem(config, "packets"));
            }
        }

        private static void Clear()
        {
            var perc = getSliderItem(laneClearMenu, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            if (getCheckBoxItem(laneClearMenu, "useqLC") && Q.IsReady())
            {
                var bestPositionQ =
                    Q.GetCircularFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));

                if (bestPositionQ.MinionsHit >= getSliderItem(laneClearMenu, "qMinHit"))
                {
                    Q.Cast(bestPositionQ.Position, getCheckBoxItem(config, "packets"));
                }
            }
            if (getCheckBoxItem(laneClearMenu, "useeLC") && E.IsReady())
            {
                var bestPositionE =
                    E.GetLineFarmLocation(
                        MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly));

                if (bestPositionE.MinionsHit >= getSliderItem(laneClearMenu, "eMinHit"))
                {
                    E.Cast(bestPositionE.Position, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(comboMenu, "useIgnite") && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !Q.IsReady() && !justQ && !justE && !rActive)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (getCheckBoxItem(comboMenu, "usew") && W.IsReady())
            {
                CastW(true);
            }
            if (rActive || justR)
            {
                return;
            }
            if (R.IsReady() && getCheckBoxItem(comboMenu, "user") &&
                getSliderItem(comboMenu, "Rmin") <= player.CountEnemiesInRange(R.Range))
            {
                CastR();
                justR = true;
                Utility.DelayAction.Add(200, () => justR = false);
                return;
            }
            var hitC = HitChance.High;
            if (getCheckBoxItem(miscMenu, "useHigherHit"))
            {
                hitC = HitChance.VeryHigh;
            }
            if (getCheckBoxItem(comboMenu, "useq") && Q.CanCast(target) &&
                player.Distance(target) < getSliderItem(comboMenu, "useqRange"))
            {
                Q.CastIfHitchanceEquals(target, hitC, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(comboMenu, "usee") && E.CanCast(target))
            {
                E.CastIfHitchanceEquals(target, hitC, getCheckBoxItem(config, "packets"));
            }
        }

        private static void CastW(bool combo)
        {
            foreach (
                var h in
                    HeroManager.Allies.Where(i => i.IsValid && i.Distance(player) < W.Range)
                        .OrderByDescending(TargetSelector.GetPriority))
            {
                var incDamage = IncDamages.GetAllyData(h.NetworkId);
                if (incDamage != null &&
                    (incDamage.DamageCount >= getSliderItem(miscMenu, "Wmin") ||
                     CheckDamageToW(incDamage)) && (combo || (!combo && CheckAutoW())))
                {
                    W.Cast(incDamage.Hero, getCheckBoxItem(config, "packets"));
                    return;
                }
            }
        }

        private static bool CheckDamageToW(IncData incDamage)
        {
            switch (getBoxItem(miscMenu, "Wdam"))
            {
                case 0:
                    if (incDamage.DamageTaken > player.TotalAttackDamage/2)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (incDamage.DamageTaken > player.TotalAttackDamage)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (incDamage.DamageTaken > player.TotalAttackDamage*2)
                    {
                        return true;
                    }
                    break;
                case 3:
                    return false;
            }
            return false;
        }

        private static void CastR()
        {
            if (CombatHelper.CheckInterrupt(player.Position, R.Range))
            {
                R.Cast(getCheckBoxItem(config, "packets"));
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

        private static void Game_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawqq"))
            {
                Render.Circle.DrawCircle(player.Position, Q.Range, Color.FromArgb(180, 100, 146, 166));
            }

            if (getCheckBoxItem(drawMenu, "drawww"))
            {
                Render.Circle.DrawCircle(player.Position, W.Range, Color.FromArgb(180, 100, 146, 166));
            }

            if (getCheckBoxItem(drawMenu, "drawee"))
            {
                Render.Circle.DrawCircle(player.Position, E.Range, Color.FromArgb(180, 100, 146, 166));
            }

            if (getCheckBoxItem(drawMenu, "drawrr"))
            {
                Render.Circle.DrawCircle(player.Position, R.Range, Color.FromArgb(180, 100, 146, 166));
            }
        }

        private static void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Base))
            {
                return;
            }
            if (sender.IsMe && args.SData.Name == "GalioIdolOfDurand" && !justR)
            {
                justR = true;
                Utility.DelayAction.Add(200, () => justR = false);
            }
            if (sender.IsMe && args.SData.Name == "GalioResoluteSmite")
            {
                justQ = true;
                Utility.DelayAction.Add(getDelay(Q, args.End), () => justQ = false);
            }
            if (sender.IsMe && args.SData.Name == "GalioRighteousGust")
            {
                justE = true;
                Utility.DelayAction.Add(getDelay(E, args.End), () => justE = false);
            }
        }

        private static int getDelay(Spell spell, Vector3 pos)
        {
            return (int) (spell.Delay*1000 + player.Distance(pos)/spell.Speed);
        }

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Galio ", "Galio");

            // Draw settings
            drawMenu = config.AddSubMenu("Drawings ", "dsettings");
            drawMenu.Add("drawqq", new CheckBox("Draw Q range"));
            drawMenu.Add("drawww", new CheckBox("Draw W range"));
            drawMenu.Add("drawee", new CheckBox("Draw E range"));
            drawMenu.Add("drawrr", new CheckBox("Draw R range"));

            // Combo Settings
            comboMenu = config.AddSubMenu("Combo ", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("useqRange", new Slider("Max range", (int) Q.Range, 0, (int) Q.Range));
            comboMenu.Add("usew", new CheckBox("Use W", false));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("user", new CheckBox("Use R"));
            comboMenu.Add("Rmin", new Slider("R min", 2, 1, 5));
            comboMenu.Add("manualRflash", new KeyBind("Flash R", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("Rminflash", new Slider("R min", 3, 1, 5));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            harassMenu = config.AddSubMenu("Harass ", "Hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q"));
            harassMenu.Add("useeH", new CheckBox("Use E"));
            harassMenu.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            laneClearMenu = config.AddSubMenu("LaneClear ", "Lcsettings");
            laneClearMenu.Add("useqLC", new CheckBox("Use Q"));
            laneClearMenu.Add("qMinHit", new Slider("Q min hit", 3, 1, 6));
            laneClearMenu.Add("useeLC", new CheckBox("Use E"));
            laneClearMenu.Add("eMinHit", new Slider("E min hit", 3, 1, 6));
            laneClearMenu.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Settings
            miscMenu = config.AddSubMenu("Misc ", "Msettings");
            miscMenu.Add("Interrupt", new CheckBox("Cast R to interrupt spells", false));
            miscMenu.Add("useHigherHit", new CheckBox("Higher HitChance(Q-E)"));
            miscMenu.Add("AutoW", new CheckBox("Auto cast W"));
            miscMenu.Add("Wmin", new Slider("W min hits", 3, 1, 10));
            miscMenu.Add("Wdam", new ComboBox("W to damage", 1, "Low", "Mid", "High", "Off"));
            miscMenu.Add("AutoWmana", new Slider("Min mana", 50, 1));
            miscMenu.Add("AutoWhealth", new Slider("Under health", 70, 1));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}