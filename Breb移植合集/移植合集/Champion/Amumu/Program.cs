using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Utility.AmumuSharp;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Amumu
{
    public class Program
    {
        public static Menu Menu, comboMenu, farmMenu, drawMenu, miscMenu;

        private static Spell _spellQ;
        private static Spell _spellW;
        private static Spell _spellE;
        private static Spell _spellR;

        private static bool _comboW;

        private static Helper Helper;

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

        public static void OnLoad()
        {
            Helper = new Helper();

            if (ObjectManager.Player.ChampionName != "Amumu")
                return;

            Menu = MainMenu.AddMenu("木木Sharp", "AmumuSharp");

            comboMenu = Menu.AddSubMenu("连招", "Combo");
            comboMenu.Add("comboQ", new Slider("使用 Q (1 : 从不 | 2 : 移植 | 3 : 超出范围时)", 2, 1, 3));
            comboMenu.Add("comboW", new CheckBox("使用 W"));
            comboMenu.Add("comboE", new CheckBox("使用 E"));
            comboMenu.Add("comboR", new Slider("自动 R，敌人数量", 3, 0, 5));
            comboMenu.Add("comboWPercent", new Slider("持续开启W，直到蓝量 %", 10));

            farmMenu = Menu.AddSubMenu("农兵", "Farming");
            farmMenu.Add("farmQ", new Slider("使用 Q (1 : 从不 | 2 : 移植 | 3 : 超出范围时)", 2, 1, 3));
            farmMenu.Add("farmW", new CheckBox("使用 W"));
            farmMenu.Add("farmE", new CheckBox("使用 E"));
            farmMenu.Add("farmWPercent", new Slider("持续开启W，直到蓝量 %", 20));

            drawMenu = Menu.AddSubMenu("线圈", "Drawing");
            drawMenu.Add("drawQ", new CheckBox("显示 Q"));
            drawMenu.Add("drawW", new CheckBox("显示 W"));
            drawMenu.Add("drawE", new CheckBox("显示 E"));
            drawMenu.Add("drawR", new CheckBox("显示 R"));

            miscMenu = Menu.AddSubMenu("杂项", "Misc");
            miscMenu.Add("aimQ", new KeyBind("Q 至鼠标附近", false, KeyBind.BindTypes.HoldActive, 'T'));

            _spellQ = new Spell(SpellSlot.Q, 1080);
            _spellW = new Spell(SpellSlot.W, 300);
            _spellE = new Spell(SpellSlot.E, 350);
            _spellR = new Spell(SpellSlot.R, 550);

            _spellQ.SetSkillshot(.25f, 90, 2000, true, SkillshotType.SkillshotLine); //check delay
            _spellW.SetSkillshot(0f, _spellW.Range, float.MaxValue, false, SkillshotType.SkillshotCircle); //correct
            _spellE.SetSkillshot(.5f, _spellE.Range, float.MaxValue, false, SkillshotType.SkillshotCircle);
                //check delay
            _spellR.SetSkillshot(.25f, _spellR.Range, float.MaxValue, false, SkillshotType.SkillshotCircle);
                //check delay

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            AutoUlt();

            if (getKeyBindItem(miscMenu, "aimQ"))
                CastQ(
                    Helper.EnemyTeam.Where(x => x.LSIsValidTarget(_spellQ.Range) && x.LSDistance(Game.CursorPos) < 400)
                        .OrderBy(x => x.LSDistance(Game.CursorPos))
                        .FirstOrDefault());

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }

            RegulateWState();
        }

        private static void AutoUlt()
        {
            var comboR = getSliderItem(comboMenu, "comboR");

            if (comboR > 0 && _spellR.IsReady())
            {
                var enemiesHit = 0;
                var killableHits = 0;

                foreach (var enemy in Helper.EnemyTeam.Where(x => x.LSIsValidTarget(_spellR.Range)))
                {
                    var prediction = Prediction.GetPrediction(enemy, _spellR.Delay);

                    if (prediction != null &&
                        prediction.UnitPosition.LSDistance(ObjectManager.Player.ServerPosition) <= _spellR.Range)
                    {
                        enemiesHit++;

                        if (ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.W) >= enemy.Health)
                            killableHits++;
                    }
                }

                if (enemiesHit >= comboR ||
                    (killableHits >= 1 && ObjectManager.Player.Health/ObjectManager.Player.MaxHealth <= 0.1))
                    CastR();
            }
        }

        private static void CastE(Obj_AI_Base target)
        {
            if (!_spellE.IsReady() || target == null || !target.LSIsValidTarget())
                return;

            if (_spellE.GetPrediction(target).UnitPosition.LSDistance(ObjectManager.Player.ServerPosition) <=
                _spellE.Range)
                _spellE.CastOnUnit(ObjectManager.Player);
        }

        public static float GetManaPercent()
        {
            return ObjectManager.Player.Mana/ObjectManager.Player.MaxMana*100f;
        }

        private static void Combo()
        {
            var comboQ = getSliderItem(comboMenu, "comboQ");
            var comboW = getCheckBoxItem(comboMenu, "comboW");
            var comboE = getCheckBoxItem(comboMenu, "comboE");
            var comboR = getSliderItem(comboMenu, "comboR");

            if (comboQ > 1 && _spellQ.IsReady())
            {
                if (_spellR.IsReady() && comboR > 0)
                    //search unit that provides most targets hit by ult. prioritize hero target unit
                {
                    var maxTargetsHit = 0;
                    Obj_AI_Base unitMostTargetsHit = null;

                    foreach (
                        var unit in
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(
                                    x =>
                                        x.LSIsValidTarget(_spellQ.Range) &&
                                        _spellQ.GetPrediction(x).Hitchance >= HitChance.High)) //causes troubles?
                    {
                        var targetsHit = unit.LSCountEnemiesInRange((int) _spellR.Range);
                            //unitposition might not reflect where you land with Q

                        if (targetsHit > maxTargetsHit ||
                            (unitMostTargetsHit != null && targetsHit >= maxTargetsHit &&
                             unit.Type == GameObjectType.AIHeroClient))
                        {
                            maxTargetsHit = targetsHit;
                            unitMostTargetsHit = unit;
                        }
                    }

                    if (maxTargetsHit >= comboR)
                        CastQ(unitMostTargetsHit);
                }

                Obj_AI_Base target = TargetSelector.GetTarget(_spellQ.Range, DamageType.Magical);
                if (target != null)
                {
                    var pred = _spellQ.GetPrediction(target);
                    if (comboQ == 2 || (comboQ == 3 && !Orbwalking.InAutoAttackRange(target)) && _spellQ.IsReady() && target.LSIsValidTarget() && pred.Hitchance >= HitChance.High)
                        _spellQ.Cast(pred.CastPosition);
                    else if (!target.CanMove && comboQ == 2 || comboQ == 3)
                    {
                        _spellQ.Cast(target);
                    }
                }   
            }

            if (comboW && _spellW.IsReady())
            {
                var target = TargetSelector.GetTarget(_spellW.Range + 200, DamageType.Magical);

                if (target != null)
                {
                    var enoughMana = GetManaPercent() >= getSliderItem(comboMenu, "comboWPercent");

                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    {
                        if (ObjectManager.Player.LSDistance(target.ServerPosition) <= _spellW.Range && enoughMana)
                        {
                            _comboW = true;
                            _spellW.Cast();
                        }
                    }
                    else if (!enoughMana)
                        RegulateWState(true);
                }
                else
                    RegulateWState();
            }

            if (comboE && _spellE.IsReady())
                CastE(Helper.EnemyTeam.OrderBy(x => x.LSDistance(ObjectManager.Player)).FirstOrDefault(x => _spellE.IsInRange(x)));
        }

        private static void LaneClear()
        {
            var farmQ = getSliderItem(farmMenu, "farmQ");
            var farmW = getCheckBoxItem(farmMenu, "farmW");
            var farmE = getCheckBoxItem(farmMenu, "farmW");

            List<Obj_AI_Base> minions;

            if (farmQ > 0 && _spellQ.IsReady())
            {
                var minion =
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellQ.Range, MinionTypes.All,
                        MinionTeam.NotAlly, MinionOrderTypes.MaxHealth)
                        .FirstOrDefault(x => _spellQ.GetPrediction(x).Hitchance >= HitChance.Medium);

                if (minion != null)
                    if (farmQ == 1 || (farmQ == 2 && !Orbwalking.InAutoAttackRange(minion)))
                        CastQ(minion, HitChance.Medium);
            }

            if (farmE && _spellE.IsReady())
            {
                minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellE.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
                CastE(minions.OrderBy(x => x.LSDistance(ObjectManager.Player)).FirstOrDefault());
            }

            if (!farmW || !_spellW.IsReady())
                return;
            _comboW = false;

            minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellW.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            var anyJungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

            var enoughMana = GetManaPercent() > getSliderItem(farmMenu, "farmWPercent");

            if (enoughMana && (minions.Count >= 3 || anyJungleMobs) &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                _spellW.Cast();
            else if (!enoughMana ||
                     (minions.Count <= 2 && !anyJungleMobs &&
                      ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2))
                RegulateWState(!enoughMana);
        }

        private static void RegulateWState(bool ignoreTargetChecks = false)
        {
            if (!_spellW.IsReady() || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 2)
                return;

            var target = TargetSelector.GetTarget(_spellW.Range, DamageType.Magical);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellW.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            if (!ignoreTargetChecks && (target != null || (!_comboW && minions.Count != 0)))
                return;

            _spellW.Cast();
            _comboW = false;
        }

        private static void CastQ(Obj_AI_Base target, HitChance hitChance = HitChance.High)
        {
            if (!_spellQ.IsReady())
                return;
            if (target == null || !target.LSIsValidTarget())
                return;

            _spellQ.CastIfHitchanceEquals(target, hitChance);
        }

        private static void CastR()
        {
            if (!_spellR.IsReady())
                return;
            _spellR.Cast();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                var drawQ = getCheckBoxItem(drawMenu, "drawQ");
                var drawW = getCheckBoxItem(drawMenu, "drawW");
                var drawE = getCheckBoxItem(drawMenu, "drawE");
                var drawR = getCheckBoxItem(drawMenu, "drawR");

                if (drawQ)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _spellQ.Range,
                        Color.FromArgb(125, 0, 255, 0));

                if (drawW)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _spellW.Range,
                        Color.FromArgb(125, 0, 255, 0));

                if (drawE)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _spellE.Range,
                        Color.FromArgb(125, 0, 255, 0));

                if (drawR)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _spellR.Range,
                        Color.FromArgb(125, 0, 255, 0));
            }
        }
    }
}
