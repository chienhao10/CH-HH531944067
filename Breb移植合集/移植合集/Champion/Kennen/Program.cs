using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Damage = LeagueSharp.Common.Damage;
using Environment = UnderratedAIO.Helpers.Environment;
using Spell = LeagueSharp.Common.Spell;

namespace UnderratedAIO.Champions
{
    internal class Kennen
    {
        public static Menu config;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static Obj_AI_Minion LastAttackedminiMinion;
        public static float LastAttackedminiMinionTime;

        public static Menu drawMenu, comboMenu, harassMenu, clearMenu, miscMenu, autoHarassMenu;

        public Kennen()
        {
            InitKennen();
            InitMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Orbwalker.OnAttack += Orbwalker_OnAttack;
        }

        private void Orbwalker_OnAttack(AttackableUnit target, EventArgs args)
        {
            if (target is Obj_AI_Minion)
            {
                LastAttackedminiMinion = (Obj_AI_Minion) target;
                LastAttackedminiMinionTime = Utils.GameTimeTickCount;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            Orbwalker.DisableMovement = false;
            Orbwalker.DisableAttacking = player.HasBuff("KennenLightningRush");

            var target = getTarget();


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

            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(miscMenu, "autoq"))
            {
                if (Q.CanCast(target) && !target.IsDashing() &&
                    (MarkOfStorm(target) > 1 || (MarkOfStorm(target) > 0 && player.Distance(target) < W.Range)))
                {
                    Q.Cast(target, getCheckBoxItem(config, "packets"));
                }
            }
            if (getCheckBoxItem(miscMenu, "autow") && W.IsReady() && MarkOfStorm(target) > 1 &&
                !player.HasBuff("KennenShurikenStorm"))
            {
                if (player.Distance(target) < W.Range)
                {
                    W.Cast(getCheckBoxItem(config, "packets"));
                }
            }
            if (getKeyBindItem(autoHarassMenu, "KenAutoQ") && Q.IsReady() &&
                getSliderItem(autoHarassMenu, "KenminmanaaQ") < player.ManaPercent &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                !player.UnderTurret(true))
            {
                if (target != null && Q.CanCast(target) && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(
                        target, CombatHelper.GetHitChance(getSliderItem(autoHarassMenu, "qHit")),
                        getCheckBoxItem(config, "packets"));
                }
            }
        }

        private void Clear()
        {
            var targetW =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(m => m.IsEnemy && player.Distance(m) < W.Range && m.HasBuff("kennenmarkofstorm"));
            var targetE =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        m =>
                            m.Health > 5 && m.IsEnemy && player.Distance(m) < W.Range &&
                            Environment.Hero.countChampsAtrange(m.Position, 1000f) < 1 && !m.IsDead &&
                            !(m is Obj_AI_Turret) && !m.HasBuff("kennenmarkofstorm") && !m.UnderTurret(true))
                    .OrderBy(m => player.Distance(m));
            if (getCheckBoxItem(clearMenu, "useeClear") && E.IsReady() &&
                ((targetE.FirstOrDefault() != null && Environment.Hero.countChampsAtrange(player.Position, 1200f) < 1 &&
                  !player.HasBuff("KennenLightningRush") && targetE.Count() > 1) ||
                 (player.HasBuff("KennenLightningRush") && targetE.FirstOrDefault() == null)))
            {
                E.Cast(getCheckBoxItem(config, "packets"));
                return;
            }
            if (W.IsReady() && targetW.Count() >= getSliderItem(clearMenu, "minw") &&
                !player.HasBuff("KennenLightningRush"))
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
            var moveTo = targetE.FirstOrDefault();

            if (player.HasBuff("KennenLightningRush"))
            {
                if (moveTo == null)
                {
                    Orbwalker.DisableMovement = true;
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                else
                {
                    Orbwalker.DisableMovement = true;
                    Player.IssueOrder(GameObjectOrder.MoveTo, moveTo);
                }
            }
        }


        private void Harass()
        {
            var target = getTarget();
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "useqLC") && Q.CanCast(target) &&
                !target.IsDashing())
            {
                Q.Cast(target, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(harassMenu, "usewLC") && W.IsReady() && W.Range < player.Distance(target) &&
                target.HasBuff("kennenmarkofstorm"))
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Combo()
        {
            var target = getTarget();
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "usee") && player.HasBuff("KennenLightningRush") &&
                player.Health > target.Health && !target.UnderTurret(true) && target.Distance(Game.CursorPos) < 250f)
            {
                Orbwalker.DisableMovement = true;
                Player.IssueOrder(GameObjectOrder.MoveTo, target);
            }

            if (getCheckBoxItem(comboMenu, "useq") && Q.CanCast(target) &&
                !target.IsDashing())
            {
                Q.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(comboMenu, "usew") && W.IsReady())
            {
                if (player.HasBuff("KennenShurikenStorm"))
                {
                    if (HeroManager.Enemies.Count(e => e.Distance(player) < R.Range && MarkOfStorm(e) > 0) ==
                        player.CountEnemiesInRange(R.Range))
                    {
                        W.Cast(getCheckBoxItem(config, "packets"));
                    }
                }
                else if (W.Range > player.Distance(target) && MarkOfStorm(target) > 0)
                {
                    W.Cast(getCheckBoxItem(config, "packets"));
                }
            }
            if (getCheckBoxItem(comboMenu, "usee") && !target.UnderTurret(true) && E.IsReady() &&
                (player.Distance(target) < 80 ||
                 (!player.HasBuff("KennenLightningRush") && !Q.CanCast(target) &&
                  getSliderItem(comboMenu, "useemin") < player.Health/player.MaxHealth*100 &&
                  MarkOfStorm(target) > 0 &&
                  CombatHelper.IsPossibleToReachHim(target, 1f, new float[5] {2f, 2f, 2f, 2f, 2f}[Q.Level - 1]))))
            {
                E.Cast(getCheckBoxItem(config, "packets"));
            }
            var combodamage = ComboDamage(target);
            if (R.IsReady() && !player.HasBuffOfType(BuffType.Snare) &&
                (getSliderItem(comboMenu, "user") <=
                 player.CountEnemiesInRange(getSliderItem(comboMenu, "userrange")) ||
                 (getCheckBoxItem(comboMenu, "usertarget") &&
                  player.CountEnemiesInRange(getSliderItem(comboMenu, "userrange")) == 1 &&
                  combodamage + player.GetAutoAttackDamage(target)*3 > target.Health && !Q.CanCast(target) &&
                  player.Distance(target) < getSliderItem(comboMenu, "userrange"))) ||
                (getSliderItem(comboMenu, "userLow") <=
                 HeroManager.Enemies.Count(
                     e => e.IsValidTarget(getSliderItem(comboMenu, "userrange")) && e.HealthPercent < 75)))
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawqq"))
            {
                Render.Circle.DrawCircle(player.Position, Q.Range, Color.FromArgb(180, 109, 111, 126));
            }

            if (getCheckBoxItem(drawMenu, "drawww"))
            {
                Render.Circle.DrawCircle(player.Position, W.Range, Color.FromArgb(180, 109, 111, 126));
            }
            if (getCheckBoxItem(drawMenu, "drawrr"))
            {
                Render.Circle.DrawCircle(player.Position, R.Range, Color.FromArgb(180, 109, 111, 126));
            }
            if (getCheckBoxItem(drawMenu, "drawrrr"))
            {
                Render.Circle.DrawCircle(player.Position, getSliderItem(comboMenu, "userrange"),
                    Color.FromArgb(180, 109, 111, 126));
            }
        }

        private float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (R.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.R)*2;
            }
            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.W, 1);
            }
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private int MarkOfStorm(Obj_AI_Base target)
        {
            var buff = target.GetBuff("kennenmarkofstorm");
            if (buff != null)
            {
                return buff.Count;
            }
            return 0;
        }

        private void InitKennen()
        {
            Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(0.5f, 50, 1700, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 500);
        }

        private AIHeroClient getTarget()
        {
            switch (getBoxItem(miscMenu, "DmgType"))
            {
                case 0:
                    return TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                case 1:
                    return TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                default:
                    return TargetSelector.GetTarget(Q.Range, DamageType.Magical);
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

        private void InitMenu()
        {
            config = MainMenu.AddMenu("凯南", "Kennen");

            // Draw settings
            drawMenu = config.AddSubMenu("线圈 ", "dsettings");
            drawMenu.Add("drawqq", new CheckBox("显示 Q 范围"));
            drawMenu.Add("drawww", new CheckBox("显示 W 范围"));
            drawMenu.Add("drawrr", new CheckBox("显示 R 范围"));
            drawMenu.Add("drawrrr", new CheckBox("显示 R 开启范围"));
            drawMenu.Add("drawcombo", new CheckBox("显示 连招伤害"));

            // Combo Settings
            comboMenu = config.AddSubMenu("连招 ", "csettings");
            comboMenu.Add("useq", new CheckBox("使用 Q"));
            comboMenu.Add("usew", new CheckBox("使用 W"));
            comboMenu.Add("usee", new CheckBox("使用 E"));
            comboMenu.Add("useemin", new Slider("最低血量使用 E", 50));
            comboMenu.Add("user", new Slider("R 最低数量", 4, 1, 5));
            comboMenu.Add("userLow", new Slider("或者附近敌人血量低于 75%", 3, 1, 5));
            comboMenu.Add("usertarget", new CheckBox("使用 R 单挑"));
            comboMenu.Add("userrange", new Slider("R 激活距离", 350, 0, 550));

            // Harass Settings
            harassMenu = config.AddSubMenu("骚扰 ", "Hcsettings");
            harassMenu.Add("useqLC", new CheckBox("使用 Q"));
            harassMenu.Add("usewLC", new CheckBox("使用 W"));

            // Clear Settings
            clearMenu = config.AddSubMenu("清线 ", "Clearsettings");
            clearMenu.Add("useqClear", new CheckBox("使用 Q"));
            clearMenu.Add("minw", new Slider("最低数量适使用 W", 3, 1, 8));
            clearMenu.Add("useeClear", new CheckBox("使用 E"));

            // Misc Settings
            miscMenu = config.AddSubMenu("杂项 ", "Msettings");
            miscMenu.Add("Minhelath", new Slider("低于 X 血量使用中亚", 35));
            miscMenu.Add("autoq", new CheckBox("准备击晕时，自动Q"));
            miscMenu.Add("autow", new CheckBox("自动 W 晕眩"));
            miscMenu.Add("DmgType", new ComboBox("伤害类型", 0, "AP", "AD"));

            autoHarassMenu = config.AddSubMenu("自动骚扰", "autoQ");
            autoHarassMenu.Add("KenAutoQ", new KeyBind("自动 Q 开关", false, KeyBind.BindTypes.PressToggle, 'H'));
            autoHarassMenu.Add("KenminmanaaQ", new Slider("保留 X% 能量", 40, 1));
            autoHarassMenu.Add("qHit", new Slider("Q 命中率", 4, 1, 4));

            config.Add("packets", new CheckBox("使用封包", false));
        }
    }
}