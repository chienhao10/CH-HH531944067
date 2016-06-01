using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using Utility = LeagueSharp.Common.Utility;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu.Values;

namespace EkkoGod
{
    class Program
    {
        private static AIHeroClient Player;
        private static Menu Menu;
        public static Menu comboMenu, rMenu, miscMenu, drawMenu, harassMenu, fleeMenu;
        private static Spell Q, W, E, R;
        private static SpellSlot ignite;
        private static Obj_AI_Minion jumpfar;

        public static void GameOnOnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != "Ekko")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(0.25f, 60f, 1650, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1650);
            W.SetSkillshot(3f, 500f, int.MaxValue, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 450);

            R = new Spell(SpellSlot.R, 375);
            R.SetSkillshot(.3f, 375, int.MaxValue, false, SkillshotType.SkillshotCircle);

            ignite = Player.GetSpellSlot("summonerdot");

            Menu = MainMenu.AddMenu("Ekko God", "EkkoGod");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("QMode", new ComboBox("QMode :", 0, "QE", "EQ", "EQ Hyper Speed (test)"));
            comboMenu.AddSeparator();
            comboMenu.Add("UseQCombo", new CheckBox("Use Q in combo", true));
            comboMenu.Add("UseWCombo", new CheckBox("Cast W before R in AoE", true));
            comboMenu.Add("UseWCombo2", new CheckBox("Cast W before R in combo killable", true));
            comboMenu.Add("UseECombo", new CheckBox("Use E in combo", true));

            rMenu = Menu.AddSubMenu("R Options", "RMenu");
            rMenu.Add("UseRKillable", new CheckBox("Use R if combo killable", true));
            rMenu.Add("UseRatHP", new CheckBox("Use R at % HP", true));
            rMenu.Add("HP", new Slider("HP", 30, 0, 100));
            rMenu.Add("UseRAoE", new CheckBox("Use R AoE", true));
            rMenu.Add("AoECount", new Slider("Minimum targets to R", 3, 1, 5));
            rMenu.Add("UseRifDie", new CheckBox("Use R if ability will kill me", true));
            rMenu.Add("UseRDangerous", new CheckBox("Use R on ZedR, ViR, etc", true));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q in harass", true));
            harassMenu.Add("UseEHarass", new CheckBox("Use E in harass", true));
            harassMenu.Add("HarassMana", new Slider("Mana manager (%)", 40, 1, 100));

            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("drawQ", new CheckBox("Q range (also is dash+leap range)", false));
            drawMenu.Add("drawW", new CheckBox("W range", false));
            drawMenu.Add("drawE", new CheckBox("E (leap) range", false));
            drawMenu.Add("drawGhost", new CheckBox("R range (around ghost)", true));
            drawMenu.Add("drawPassiveStacks", new CheckBox("Passive stacks", true));
            drawMenu.Add("DamageAfterCombo", new CheckBox("Draw damage after combo", true));
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;

            fleeMenu = Menu.AddSubMenu("Flee", "Flee");
            fleeMenu.Add("QFlee", new CheckBox("Q enemy while fleeing)", true));
            fleeMenu.Add("EFlee", new CheckBox("Jump to furthest minion w/ E", true));


            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("Killsteal", new CheckBox("KS with Q", true));
            miscMenu.Add("WSelf", new CheckBox("W Self on Gapclose", true));
            miscMenu.Add("WCC", new CheckBox("Cast W on Immobile", true));
            miscMenu.Add("UseIgnite", new CheckBox("Ignite if Combo Killable", true));


            //Config.AddItem(new MenuItem("eToMinion", "E Minion After Manual E if Target Far").SetValue(true));


            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
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

        #region damagecalcscreditto1337/worstping
        // WorstPing
        public static double GetDamageQ(Obj_AI_Base target) // fixed
        {
            return Q.IsReady()
                       ? Player.CalcDamage(
                           target,
                           DamageType.Magical,
                           new double[] { 120, 160, 200, 240, 280 }[Q.Level - 1]
                           + Player.TotalMagicalDamage * .8f)
                       : 0d;
        }

        // 1337
        public static double GetDamageE(Obj_AI_Base target)
        {
            return E.IsReady()
                       ? Player.CalcDamage(
                           target,
                           DamageType.Magical,
                           new double[] { 50, 80, 110, 140, 170 }[E.Level - 1]
                           + Player.TotalMagicalDamage * .2f)
                       : 0d;
        }


        // WorstPing
        public static double GetDamageR(Obj_AI_Base target)
        {
            return R.IsReady()
                       ? Player.CalcDamage(
                           target,
                           DamageType.Magical,
                           new double[] { 200, 350, 500 }[R.Level - 1]
                           + Player.TotalMagicalDamage * 1.3f)
                       : 0d;
        }

        #endregion


        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            var WSelf = getCheckBoxItem(miscMenu, "WSelf");

            if (WSelf && W.IsReady() && Player.LSDistance(gapcloser.Sender.ServerPosition) < E.Range)
            {
                W.Cast(Player.Position);
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {

            var dmg = 0d;

            dmg += GetDamageQ(hero);
            dmg += GetDamageE(hero);
            dmg += GetDamageR(hero);
            if (!hero.HasBuff("EkkoStunMarker"))
            {
                dmg += 15 + (12 * Player.Level) + Player.TotalMagicalDamage * .7f; // passive damage
            }
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                dmg += Player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            }
            return (float)dmg;
        }

        private static void OnUpdate(EventArgs args)
        {
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawMenu, "DamageAfterCombo");

            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Escape();
            }
         
            WCC();
            Killsteal();
            RSafe();
        }

        private static void Escape() // some credits to 1337 :v) (also not as good, me suck)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var enemies = HeroManager.Enemies.Where(t => t.LSIsValidTarget() && t.LSDistance(Player.Position) <= W.Range);

            if (Q.IsReady() && target.LSIsValidTarget() && getCheckBoxItem(fleeMenu, "QFlee"))
            {
                Q.Cast(target);
            }

            if (E.IsReady() && getCheckBoxItem(fleeMenu, "EFlee"))
            {
                E.Cast(Game.CursorPos);
                var enemy = enemies.OrderBy(t => t.LSDistance(Player.Position)).FirstOrDefault();
                if (enemy != null)
                {
                    var minion = ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.LSDistance(enemy.Position) <= Q.Range)
                        .OrderByDescending(m => m.LSDistance(Player.Position)).FirstOrDefault();

                    if (minion.LSIsValidTarget() && minion != null)
                    {
                        jumpfar = minion;
                    }
                }

                else if (enemy == null)
                {
                    var minion = ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.LSDistance(Player.Position) <= Q.Range)
                        .OrderByDescending(m => m.LSDistance(Player.Position)).FirstOrDefault();

                    if (minion.LSIsValidTarget() && minion != null)
                    {
                        jumpfar = minion;
                    }
                }
            }

            if (Player.AttackRange == 425 && jumpfar.LSIsValidTarget())
            {
              EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, jumpfar);
            }
            Orbwalker.MoveTo(Game.CursorPos);
        }

        private static void WCC()
        {
            var WCC = getCheckBoxItem(miscMenu, "WCC");
            if (WCC)
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsVisible && !enemy.IsDead && Player.LSDistance(enemy.Position) <= W.Range && W.IsReady() && Player.LSDistance(enemy.Position) < W.Range))
                {
                    if (target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Stun))
                    {
                        W.Cast(target.ServerPosition);
                    }
                }
            }
        }

        private static void Killsteal()
        {
            var KS = getCheckBoxItem(miscMenu, "Killsteal");
            if (KS)
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsVisible && enemy.LSIsValidTarget() && GetDamageQ(enemy) > enemy.Health && Player.LSDistance(enemy.Position) <= Q.Range && Q.IsReady()))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void RSafe()
        {
            var danger = getCheckBoxItem(rMenu, "UseRDangerous");
            if (R.IsReady() && Player.HasBuff("zedulttargetmark") && danger) //stupid idea idk what to do tho
            {
                Utility.DelayAction.Add(3500, () => R.Cast());
            }
        }

        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "UseQCombo");
            var useW = getCheckBoxItem(comboMenu, "UseWCombo");
            var useE = getCheckBoxItem(comboMenu, "UseECombo");
            var useRKillable = getCheckBoxItem(rMenu, "UseRKillable");
            var useRatHP = getCheckBoxItem(rMenu, "UseRatHP");
            var HP = getSliderItem(rMenu, "HP");
            var useRAoE = getCheckBoxItem(rMenu, "UseRAoE");
            var AoECount = getSliderItem(rMenu, "AoECount");
            var alone = HeroManager.Enemies.Count(scared => scared.LSDistance(Player.Position) <= 1000);
            var enemyCount = 0;
            var useW2 = getCheckBoxItem(comboMenu, "UseWCombo2");
            var UseIgnite = getCheckBoxItem(miscMenu, "UseIgnite");
            if (ghost != null)
            {
                enemyCount += HeroManager.Enemies.Count(enemy => enemy.LSDistance(ghost.Position) <= 375);
            }

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;

            var rdelay = R.GetPrediction(target).UnitPosition;

            if (getBoxItem(comboMenu, "QMode") == 0)
            {
                if (useQ && Q.IsReady())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }

                if (useE && E.IsReady())
                {
                    E.Cast(Game.CursorPos);
                }
            }

            else if (getBoxItem(comboMenu, "QMode") == 1)
            {
                if (useE && E.IsReady())
                {
                    E.Cast(Game.CursorPos);
                }

                if (useQ && Q.IsReady() && !E.IsReady() && !Player.HasBuff("ekkoeattackbuff"))
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
            }

            else if (getBoxItem(comboMenu, "QMode") == 2)
            {
                if (useE && E.IsReady())
                {
                    E.Cast(Game.CursorPos);
                }

                if (useQ && Q.IsReady() && !E.IsReady() && Player.HasBuff("ekkoeattackbuff"))
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }

                else if (useQ && Q.IsReady() && !E.IsReady())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
            }


            if (useRKillable && R.IsReady() && useW2)
            {
                if (rdelay.LSDistance(ghost.Position) <= R.Range)
                {
                    if (ComboDamage(target) >= target.Health && Player.LSDistance(ghost.Position) < W.Range)
                    {
                        W.Cast(ghost.Position);
                        R.Cast();
                    }
                }
            }

            else if (useRKillable && R.IsReady())
            {
                if (rdelay.LSDistance(ghost.Position) <= R.Range)
                {
                    if (ComboDamage(target) >= target.Health)
                    {
                        R.Cast();
                    }
                }
            }

            if (useRatHP && R.IsReady())
            {
                if (Player.HealthPercent <= HP && alone >= 1)
                {
                    R.Cast();
                }
            }

            if (useRAoE && R.IsReady() && enemyCount >= AoECount && useW && W.IsReady() && Player.LSDistance(ghost.Position) < W.Range)
            {
                W.Cast(ghost.Position);
                R.Cast();
            }

            else if (useRAoE && R.IsReady() && enemyCount >= AoECount)
            {
                R.Cast();
            }

            if (Player.LSDistance(target.ServerPosition) <= 600 && ComboDamage(target) >= target.Health && UseIgnite)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Harass()
        {

            var mana = getSliderItem(harassMenu, "HarassMana");
            var useQ = getCheckBoxItem(harassMenu, "UseQHarass");
            var useE = getCheckBoxItem(harassMenu, "UseEHarass");

            if (Player.ManaPercent < mana)
                return;

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;

            if (useQ && Q.IsReady())
            {
                Q.Cast(target);
            }

            if (useE && E.IsReady())
            {
                E.Cast(Game.CursorPos);
            }
        }

        private static Obj_AI_Base ghost
        {
            get
            {
                return
                ObjectManager.Get<Obj_AI_Base>()
                                .FirstOrDefault(ghost => !ghost.IsEnemy && ghost.Name.Contains("Ekko"));
            }
        }


        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            var enemy = TargetSelector.GetTarget(E.Range + 425, DamageType.Magical);
            var userdie = getCheckBoxItem(rMenu, "UseRifDie");
            var danger = getCheckBoxItem(rMenu, "UseRDangerous");

            if (sender.IsMe && args.SData.Name == "EkkoE")
            {
                // make sure orbwalker doesnt mess up after casting E

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (enemy == null)
                        return;

                    Utility.DelayAction.Add((int)(Math.Ceiling(Game.Ping / 2f) + 350), () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy));
                }
            }

            if (args.Target == null)
                return;

            if (R.IsReady() && args.End.LSDistance(Player.Position) < 150 && args.SData.Name == "ViR" && danger)
            {
                Utility.DelayAction.Add(250, () => R.Cast());
            }

            if (R.IsReady() && sender.IsEnemy && args.Target.IsMe && userdie)
            {
                var dmg = sender.GetDamageSpell(Player, args.SData.Name);
                if (dmg.CalculatedDamage >= Player.Health - 50)
                {
                    Utility.DelayAction.Add(0, () => R.Cast());
                }
            }
        }



        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var drawQ = getCheckBoxItem(drawMenu, "drawQ");
            var drawW = getCheckBoxItem(drawMenu, "drawW");
            var drawE = getCheckBoxItem(drawMenu, "drawE");
            var drawPassive = getCheckBoxItem(drawMenu, "drawPassiveStacks");
            var drawGhost = getCheckBoxItem(drawMenu, "drawGhost");


            if (drawQ && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.GreenYellow);
            }
            else if (drawQ && !Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Maroon);
            }

            if (drawW && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.GreenYellow);
            }

            if (drawE && E.IsReady() || Player.Spellbook.GetSpell(SpellSlot.E).State == SpellState.Surpressed)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.GreenYellow);
            }

            else if (drawE && !E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Maroon);
            }

            if (drawPassive)
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.Buffs.Any(buff1 => buff1.Name == "EkkoStacks" && buff1.Count == 2) && enemy.IsVisible)
                    {
                        var enemypos = Drawing.WorldToScreen(enemy.Position);
                        Render.Circle.DrawCircle(enemy.Position, 150, Color.Red);
                        Drawing.DrawText(enemypos.X, enemypos.Y + 15, Color.Red, "2 Stacks");
                    }

                    else if (enemy.Buffs.Any(buff1 => buff1.Name == "EkkoStacks" && buff1.Count == 1) && enemy.IsVisible)
                    {
                        var enemypos = Drawing.WorldToScreen(enemy.Position);
                        Render.Circle.DrawCircle(enemy.Position, 150, Color.White);
                        Drawing.DrawText(enemypos.X, enemypos.Y + 15, Color.White, "1 Stack");
                    }
                }
            }

            if (drawGhost && ghost != null)
            {
                if (R.IsReady())
                {
                    Render.Circle.DrawCircle(ghost.Position, R.Range, Color.GreenYellow);
                }
            }
        }
    }
}