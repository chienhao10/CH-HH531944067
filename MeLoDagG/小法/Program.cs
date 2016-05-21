using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Veigar_The_Troll
{
    internal class Program
    {
        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;
        public static Item ZhonyaHourglass;

        private static Menu _menu,
            _comboMenu,
            _jungleLaneMenu,
            _miscMenu,
            _drawMenu,
            _skinMenu,
            _autoPotHealMenu,
            _autostuckEnemy;

        public static AIHeroClient _target;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }


        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Veigar)
            {
                return;
            }
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);
            ZhonyaHourglass = new Item(ItemId.Zhonyas_Hourglass);


            _menu = MainMenu.AddMenu("Troll小法", "VeigarTheTroll");
            _comboMenu = _menu.AddSubMenu("连招", "Combo");
            _comboMenu.Add("useQCombo", new CheckBox("使用 Q", true));
            _comboMenu.Add("useWCombo", new CheckBox("使用 W", true));
            _comboMenu.Add("useECombo", new CheckBox("使用 E", true));
            _comboMenu.AddLabel("E 逻辑 | 命中率");
            _comboMenu.Add("useECombopred", new CheckBox("使用 E [% 命中率调制器]", false));
            _comboMenu.Add("predE", new Slider("使用 E 预判 %"));
            _comboMenu.AddLabel("R 逻辑");
            _comboMenu.Add("useRburst", new CheckBox("使用 R 爆发", true));
            _comboMenu.Add("useRCombo", new CheckBox("使用 R 抢头[旧]", true));
            _comboMenu.Add("useRCombobeta", new CheckBox("使用 R 新计算[Beta]", false));
            _comboMenu.AddLabel("点燃设置");
            _comboMenu.Add("UseIgnite", new CheckBox("如果连招可击杀 使用点燃", true));

            _autostuckEnemy = _menu.AddSubMenu("骚扰设置", "Harass");
            _autostuckEnemy.Add("UseQstuck", new CheckBox("使用 Q", true));
            _autostuckEnemy.Add("UseWstuck", new CheckBox("使用 W", true));
            _autostuckEnemy.Add("StuckEnemyMana", new Slider("使用 Q W 蓝量限制", 70, 0, 100));

            _jungleLaneMenu = _menu.AddSubMenu("清线设置", "FarmSettings");
            _jungleLaneMenu.AddLabel("清线");
            _jungleLaneMenu.Add("qFarm", new CheckBox("Q 尾兵[全模式]", true));
            _jungleLaneMenu.Add("wwFarm", new CheckBox("使用 W",false));
            _jungleLaneMenu.Add("wFarm", new Slider("使用 W 如果可命中小兵数量 >=", 4, 1, 15));
            _jungleLaneMenu.AddLabel("自动使用 Q");
            _jungleLaneMenu.Add("qFarmAuto", new CheckBox("使用 Q 尾兵[自动]", true));
            _jungleLaneMenu.Add("qFarmAutoMana", new Slider("蓝量使用Q", 60, 0, 100));
            _jungleLaneMenu.AddLabel("清野");
            _jungleLaneMenu.Add("useQJungle", new CheckBox("使用 Q"));
            _jungleLaneMenu.Add("useWJungle", new CheckBox("使用 W"));

            _miscMenu = _menu.AddSubMenu("杂项", "MiscSettings");
            _miscMenu.AddGroupLabel("自动设置");
            _miscMenu.Add("CCQ", new CheckBox("自动 Q 当敌方被强控"));
            _miscMenu.Add("CCW", new CheckBox("自动 W 当敌方被强控"));
            _miscMenu.AddGroupLabel("抢头 设置");
            _miscMenu.Add("ksQ", new CheckBox("抢头 Q"));
            _miscMenu.Add("ksIgnite", new CheckBox("点燃 抢头"));
            _miscMenu.AddLabel("自动中亚");
            _miscMenu.Add("Zhonyas", new CheckBox("使用中亚"));
            _miscMenu.Add("ZhonyasHp", new Slider("使用中亚 当血量低于 X%", 20, 0, 100));
            _miscMenu.AddLabel("技能打断设置");
            _miscMenu.Add("interuptE", new CheckBox("自动 E 技能打断"));
            _miscMenu.AddLabel("防突进");
            _miscMenu.Add("gapcloser", new CheckBox("自动 E 防突进"));


            _autoPotHealMenu = _menu.AddSubMenu("药水", "Potion");
            _autoPotHealMenu.AddGroupLabel("自动使用药水");
            _autoPotHealMenu.Add("potion", new CheckBox("使用 药水"));
            _autoPotHealMenu.Add("potionminHP", new Slider("最低血量 % 使用药水", 40));
            _autoPotHealMenu.Add("potionMinMP", new Slider("最低蓝量 % 使用药水", 20));

            _skinMenu = _menu.AddSubMenu("换肤", "SkinChanger");
            _skinMenu.Add("checkSkin", new CheckBox("开启换肤", false));
            _skinMenu.Add("skin.Id", new Slider("皮肤", 1, 0, 8));

            _drawMenu = _menu.AddSubMenu("线圈");
            _drawMenu.AddGroupLabel("显示技能 kills");
            _drawMenu.Add("drawQ", new CheckBox("显示 Q 范围"));
            _drawMenu.Add("drawW", new CheckBox("显示 W 范围"));
            _drawMenu.Add("drawE", new CheckBox("显示 E 范围"));
            _drawMenu.Add("drawR", new CheckBox("显示 R 范围"));
            _drawMenu.AddLabel("伤害指示器");
            _drawMenu.Add("healthbar", new CheckBox("血量显示"));
            _drawMenu.Add("percent", new CheckBox("伤害百分比"));


            DamageIndicator.Initialize(ComboDamage);
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;

            Chat.Print(
                "<font color=\"#6909aa\" >MeLoDag Presents </font><font color=\"#fffffff\" >Veigar </font><font color=\"#6909aa\" >Kappa Kippo</font>");
        }

        public static
            void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (_miscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) <= 750)
            {
                SpellManager.E.Cast(e.End);
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var useE = _miscMenu["interuptE"].Cast<CheckBox>().CurrentValue;


            {
                if (useE)
                {
                    if (sender.IsEnemy && SpellManager.E.IsReady() && sender.Distance(_Player) <= SpellManager.E.Range)
                    {
                        SpellManager.E.Cast(sender);
                        Chat.Print("<font color=\"#fffffff\" > Use E Interrupt</font>");
                    }
                }
            }
        }

        private static
            void Game_OnTick(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                CastR();
                UseEpercent();
                UseRburst();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                AutoStuckEnemy();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                FarmQ();
                FarmW();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                FarmQ();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                FarmQ();
            }
            Auto();
            Killsteal();
            AutoPot();
            CastR();
            QFarmAuto();
            AutoHourglass();
        }

        private static void UseRburst()
        {
            var useRburst = _comboMenu["useRburst"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(SpellManager.R.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;
            {
                if (SpellManager.R.IsReady() && useRburst)
                {
                    SpellManager.R.Cast(target);
                }
            }
        }

        private static
            void AutoStuckEnemy()
        {
            var useQstuck = _autostuckEnemy["UseQstuck"].Cast<CheckBox>().CurrentValue;
            var useWstuck = _autostuckEnemy["UseWstuck"].Cast<CheckBox>().CurrentValue;
            var stuckEnemyMana = _autostuckEnemy["StuckEnemyMana"].Cast<Slider>().CurrentValue;

            {
                var targetq = TargetSelector.GetTarget(SpellManager.Q.Range, DamageType.Magical);
                if (targetq != null)
                {
                    if (SpellManager.Q.IsReady() && useQstuck && _Player.ManaPercent >= stuckEnemyMana)
                    {
                        var predQ = SpellManager.Q.GetPrediction(targetq);
                        if (predQ.HitChance >= HitChance.High)
                        {
                            SpellManager.Q.Cast(predQ.CastPosition);
                        }
                        else if (predQ.HitChance >= HitChance.Immobile)
                        {
                            SpellManager.Q.Cast(predQ.CastPosition);
                        }
                    }
                }
                var targetw = TargetSelector.GetTarget(SpellManager.W.Range, DamageType.Magical);
                if (targetw != null)
                {
                    if (SpellManager.W.IsReady() && useWstuck && _Player.ManaPercent >= stuckEnemyMana)
                    {
                        var predW = SpellManager.W.GetPrediction(targetw);
                        if (predW.HitChance >= HitChance.High)
                        {
                            SpellManager.W.Cast(predW.CastPosition);
                        }
                        else if (predW.HitChance >= HitChance.Immobile)
                        {
                            SpellManager.W.Cast(predW.CastPosition);
                        }
                    }
                }
            }
        }

        private static
            void AutoHourglass()
        {
            var zhonyas = _miscMenu["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = _miscMenu["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && _Player.HealthPercent <= zhonyasHp && ZhonyaHourglass.IsReady())
            {
                ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPot()
        {
            if (_autoPotHealMenu["potion"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange() &&
                Player.Instance.HealthPercent <= _autoPotHealMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") ||
                  Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                {
                    HealthPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
            }
            if (Player.Instance.ManaPercent <= _autoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                }
            }
        }

        private static void CastR()
        {
            var useR = _comboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var useRCombobeta = _comboMenu["useRCombobeta"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(SpellManager.R.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;
            {
                if (SpellManager.R.IsReady() && useR)
                {
                    if (Rdamageold(target) >= target.Health)
                    {
                        SpellManager.R.Cast(target);
                        Chat.Print("<font color=\"#fffffff\" > Use Ulty Free Kill</font>");
                    }
                    if (SpellManager.R.IsReady() && useRCombobeta)
                    {
                        if (Rdamage(target) >= target.Health)
                        {
                            SpellManager.R.Cast(target);
                            Chat.Print("<font color=\"#fffffff\" > Use new Ulty formula Free Kill</font>");
                        }
                    }
                }
            }
        }


        private static
            void Killsteal()
        {
            var ksQ = _miscMenu["ksQ"].Cast<CheckBox>().CurrentValue;
             var ksIgnite = _miscMenu["ksIgnite"].Cast<CheckBox>().CurrentValue;

            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= SpellManager.Q.Range && e.IsValidTarget() && !e.IsInvulnerable))

            {
                if (ksQ && SpellManager.Q.IsReady() &&
                    Qdamage(enemy) >= enemy.Health &&
                    enemy.Distance(_Player) <= SpellManager.Q.Range)
                {
                    SpellManager.Q.Cast(enemy);
                    Chat.Print("<font color=\"#fffffff\" > Use Q Free Kill</font>");
                }
                if (ksIgnite)
                {
                    var enemyignite =
                        EntityManager.Heroes.Enemies.Where(
                            t =>
                                t.IsEnemy && !t.IsZombie && !t.IsDead && t.IsValid && !t.IsInvulnerable &&
                                t.IsInRange(Player.Instance.Position, SpellManager.Ignite.Range) &&
                                DamageLibrary.GetSummonerSpellDamage(Player.Instance, t,
                                    DamageLibrary.SummonerSpells.Ignite) > t.Health).FirstOrDefault();
                    if (enemyignite != null)
                        if (SpellManager.Ignite.IsReady())
                        {
                            SpellManager.Ignite.Cast(enemyignite);
                        }
                }
            }
        }
    

    private static
            void UseEpercent()
        {
            var useECombopred = _comboMenu["useECombopred"].Cast<CheckBox>().CurrentValue;
            var predE = _comboMenu["predE"].Cast<Slider>().CurrentValue;
            var eTarget = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Magical);
            if (eTarget != null)

                if (SpellManager.E.IsReady() && useECombopred && eTarget.Distance(_Player) <= 750)
                {
                    var prede = SpellManager.E.GetPrediction(eTarget);
                    if (prede.HitChancePercent >= predE)
                    {
                        SpellManager.E.Cast(prede.CastPosition);
                    }
                }
        }

        private static
            void Combo()
        {
            var useQ = _comboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useE = _comboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            // var useIgnite = _comboMenu["UseIgnite"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(SpellManager.Q.Range) || target == null)
            {
                return;
            }
            var eTarget = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Magical);
            if (eTarget != null)
                if (SpellManager.E.IsReady() && useE && eTarget.Distance(_Player) <= 750)
                {
                    var ePosition = eTarget.ServerPosition.Extend(_Player.Position, 350);
                    SpellManager.E.Cast(ePosition.To3D());
                }
            if (useW && SpellManager.W.IsReady() && target.IsValidTarget(SpellManager.W.Range) && !target.IsInvulnerable)
            {
                var predW = SpellManager.W.GetPrediction(target);
                if (predW.HitChance >= HitChance.Medium)
                {
                    SpellManager.W.Cast(predW.CastPosition);
                }
                else if (predW.HitChance >= HitChance.Immobile)
                {
                    SpellManager.W.Cast(predW.CastPosition);
                }
                if (useQ && SpellManager.Q.IsReady() && target.IsValidTarget(SpellManager.Q.Range) &&
                    !target.IsInvulnerable)
                {
                    var predq = SpellManager.Q.GetPrediction(target);
                    if (predq.HitChance >= HitChance.Medium)
                    {
                        SpellManager.Q.Cast(predq.CastPosition);
                    }
                    else if (predq.HitChance >= HitChance.Immobile)
                    {
                        SpellManager.Q.Cast(predq.CastPosition);
                    }
                }
            }
        }

        public static
            float ComboDamage(Obj_AI_Base hero)
        {
            var result = 0d;

            if (SpellManager.Q.IsReady())
            {
                result += Qdamage(hero);
            }
            if (SpellManager.W.IsReady())
            {
                result += Wdamage(hero);
            }
            if (SpellManager.R.IsReady())
            {
                result += Rdamageold(hero);
            }

            return (float) result;
        }

        public static float Qdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] {0, 70, 110, 150, 190, 230}[SpellManager.Q.Level] + 0.6f*_Player.FlatMagicDamageMod);
        }

        public static float Wdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] {0, 100, 150, 200, 250, 300}[SpellManager.W.Level] + 0.99f*_Player.FlatMagicDamageMod);
        }

        public static float Rdamageold(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)
                (new[] {0, 250, 375, 500}[SpellManager.R.Level] +
                 0.99*target.FlatMagicDamageMod +
                 1.0*_Player.FlatMagicDamageMod));
        }

        public static double Rdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)
                (new[] {0, 175, 250, 325}[SpellManager.R.Level] + (new[] {0, 350, 500, 650}[SpellManager.R.Level] +
                                                                   0.88*target.FlatMagicDamageMod +
                                                                   1.5f*target.HealthPercent)));
        }


        private static void Auto()
        {
            var qonCc = _miscMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            var wonCc = _miscMenu["CCW"].Cast<CheckBox>().CurrentValue;
            if (qonCc)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(Player.Instance) < SpellManager.Q.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        SpellManager.Q.Cast(enemy);
                    }
                    if (wonCc)
                    {
                        if (enemy.Distance(Player.Instance) < SpellManager.W.Range &&
                            (enemy.HasBuffOfType(BuffType.Stun)
                             || enemy.HasBuffOfType(BuffType.Snare)
                             || enemy.HasBuffOfType(BuffType.Suppression)
                             || enemy.HasBuffOfType(BuffType.Fear)
                             || enemy.HasBuffOfType(BuffType.Knockup)))
                        {
                            SpellManager.W.Cast(enemy);
                        }
                    }
                }
            }
        }

        private static
            void JungleClear()
        {
            var useWJungle = _jungleLaneMenu["useWJungle"].Cast<CheckBox>().CurrentValue;
            var useQJungle = _jungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;

            if (useQJungle)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (SpellManager.Q.IsReady() && minion != null)
                {
                    SpellManager.Q.Cast(minion.Position);
                }

                if (SpellManager.W.IsReady() && useWJungle && minion != null)
                {
                    SpellManager.W.Cast(minion.Position);
                }
            }
        }

        private static void QFarmAuto()
        {
            var qFarmAuto = _jungleLaneMenu["qFarmAuto"].Cast<CheckBox>().CurrentValue;
            var qFarmAutoMana = _jungleLaneMenu["qFarmAutoMana"].Cast<Slider>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position,
                    SpellManager.Q.Range)
                    .FirstOrDefault(m =>
                        m.Distance(_Player) <= SpellManager.Q.Range &&
                        m.Health <= _Player.GetSpellDamage(m, SpellSlot.Q) - 20 &&
                        m.IsValidTarget());


            if (SpellManager.Q.IsReady() && qFarmAuto && _Player.ManaPercent >= qFarmAutoMana && qminion != null &&
                !Orbwalker.IsAutoAttacking)
            {
                SpellManager.Q.Cast(qminion);
            }
        }

        private static void FarmQ()
        {
            var useQ = _jungleLaneMenu["qFarm"].Cast<CheckBox>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position,
                    SpellManager.Q.Range)
                    .FirstOrDefault(m =>
                        m.Distance(_Player) <= SpellManager.Q.Range &&
                        m.Health <= _Player.GetSpellDamage(m, SpellSlot.Q) - 20 &&
                        m.IsValidTarget());


            if (SpellManager.Q.IsReady() && useQ && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                SpellManager.Q.Cast(qminion);
            }
        }

        private static
            void FarmW()
        {
            var useW = _jungleLaneMenu["wwFarm"].Cast<CheckBox>().CurrentValue;
            if (SpellManager.W.IsReady() && useW)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(_Player) <= SpellManager.W.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinionsInRange >= _jungleLaneMenu["wFarm"].Cast<Slider>().CurrentValue && useW)
                    {
                        SpellManager.W.Cast(enemyMinion);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_target != null && _target.IsValid)
            {
            }

            if (SpellManager.Q.IsReady() && _drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.Q.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager.Q.Range, Color.DarkOliveGreen);
            }

            if (SpellManager.W.IsReady() && _drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.W.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager.W.Range, Color.DarkOliveGreen);
            }

            if (SpellManager.E.IsReady() && _drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.E.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager.E.Range, Color.DarkOliveGreen);
            }

            if (SpellManager.R.IsReady() && _drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.R.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager.R.Range, Color.DarkOliveGreen);
            }

            DamageIndicator.HealthbarEnabled = _drawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
            DamageIndicator.PercentEnabled = _drawMenu["percent"].Cast<CheckBox>().CurrentValue;
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (CheckSkin())
            {
                Player.SetSkinId(SkinId());
            }
        }

        public static int SkinId()
        {
            return _skinMenu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return _skinMenu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }
    }
}