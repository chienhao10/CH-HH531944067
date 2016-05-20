using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
namespace AhriSharp
{
    internal static class Ahri
    {

        private static Spell _spellQ, _spellW, _spellE, _spellR;
        const float _spellQSpeed = 2600;
        const float _spellQSpeedMin = 400;
        const float _spellQFarmSpeed = 1600;
        const float _spellQAcceleration = -3200;
        private static Menu Main,ComboM, HarassM, FarmM, DrawM, Misc;
        public static Helper Helper;

        public static void Ahri_Load()
        {
            if (EloBuddy.Player.Instance.ChampionName != "Ahri")
                return;

            Main = MainMenu.AddMenu("AhriSharp","AhriSharp");

            ComboM = Main.AddSubMenu("Combo", "Combo");
            ComboM.Add("comboQ", new CheckBox("Use Q"));
            ComboM.Add("comboW", new CheckBox("Use W"));
            ComboM.Add("comboE", new CheckBox("Use E"));
            ComboM.Add("comboR", new CheckBox("Use R"));
            ComboM.Add("comboROnlyUserInitiate", new CheckBox("Use R only if user initiated", false));
            HarassM =  Main.AddSubMenu("Harass","Harass");
            HarassM.Add("harassQ", new CheckBox("Use Q"));
            HarassM.Add("harassE", new CheckBox("Use E"));
            HarassM.Add("harassPercent", new Slider("Skills until Mana %",20,0,100));

            FarmM = Main.AddSubMenu("LaneClear", "Lane Clear");
            FarmM.Add("farmQ", new CheckBox("use  Q"));
            FarmM.Add("farmW", new CheckBox("Use W",false));
            FarmM.Add("farmPercent", new Slider("Skills until Mana %",20,0,100));

            DrawM = Main.AddSubMenu("Draw","Draws");
            DrawM.Add("drawQE", new CheckBox("Draw Q, E range"));
            DrawM.Add("drawW", new CheckBox("Draw W range"));
            DrawM.Add("DamageAfterCombo", new CheckBox("Draw Combo Damage"));

            Misc = Main.AddSubMenu("Misc", "Misc");
            Misc.Add("autoE", new CheckBox("Auto E on gapclosing targets"));
            Misc.Add("autoEI", new CheckBox("Auto E to interrupt"));


            _spellQ = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.Q, 1000);
            _spellW = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.W, 795 - 95);
            _spellE = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.E, 1000);
            _spellR = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.R, 1000 - 100);

            _spellQ.SetSkillshot(0.25f, 50, 1600f, false, LeagueSharp.Common.SkillshotType.SkillshotLine);
            _spellW.SetSkillshot(0.70f, _spellW.Range, float.MaxValue, false, LeagueSharp.Common.SkillshotType.SkillshotCircle);
            _spellE.SetSkillshot(0.25f, 60, 1550f, true, LeagueSharp.Common.SkillshotType.SkillshotLine);

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            LeagueSharp.Common.AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            LeagueSharp.Common.Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            EloBuddy.Drawing.OnEndScene += Drawing_OnDraw;
            EloBuddy.Game.OnUpdate += Game_OnUpdate;
            Helper = new Helper();
        }

        static void AntiGapcloser_OnEnemyGapcloser(LeagueSharp.Common.ActiveGapcloser gapcloser)
        {
            if (!Misc["autoE"].Cast<CheckBox>().CurrentValue) return;
            if (EloBuddy.ObjectManager.Player.LSDistance(gapcloser.Sender, true) < _spellE.Range * _spellE.Range)
            {
                _spellE.Cast(gapcloser.Sender);
            }
        }

        static void Interrupter2_OnInterruptableTarget(EloBuddy.AIHeroClient sender, LeagueSharp.Common.Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Misc["autoEI"].Cast<CheckBox>().CurrentValue) return;

            if (EloBuddy.ObjectManager.Player.LSDistance(sender, true) < _spellE.Range * _spellE.Range)
            {
                _spellE.Cast(sender);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (EloBuddy.ObjectManager.Player.IsDead)
            {
                return;
            }

            if (EloBuddy.SDK.Orbwalker.ActiveModesFlags.HasFlag(EloBuddy.SDK.Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (EloBuddy.SDK.Orbwalker.ActiveModesFlags.HasFlag(EloBuddy.SDK.Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (EloBuddy.SDK.Orbwalker.ActiveModesFlags.HasFlag(EloBuddy.SDK.Orbwalker.ActiveModes.LaneClear) || EloBuddy.SDK.Orbwalker.ActiveModesFlags.HasFlag(EloBuddy.SDK.Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }
        }

        static void Harass()
        {
            if (HarassM["harassE"].Cast<CheckBox>().CurrentValue && EloBuddy.ObjectManager.Player.ManaPercent >=  HarassM["harassPercent"].Cast<Slider>().CurrentValue)
                CastE();

            if ( HarassM["harassQ"].Cast<CheckBox>().CurrentValue && EloBuddy.ObjectManager.Player.ManaPercent >= HarassM["harassPercent"].Cast<Slider>().CurrentValue)
                CastQ();
        }

        static void LaneClear()
        {
            _spellQ.Speed = _spellQFarmSpeed;
            var minions = LeagueSharp.Common.MinionManager.GetMinions(EloBuddy.ObjectManager.Player.ServerPosition, _spellQ.Range, LeagueSharp.Common.MinionTypes.All, LeagueSharp.Common.MinionTeam.NotAlly);

            bool jungleMobs = minions.Any(x => x.Team == EloBuddy.GameObjectTeam.Neutral);

            if (( FarmM["farmQ"].Cast<CheckBox>().CurrentValue && EloBuddy.ObjectManager.Player.ManaPercent >= FarmM["farmPercent"].Cast<Slider>().CurrentValue) || jungleMobs)
            {
                LeagueSharp.Common.MinionManager.FarmLocation farmLocation = _spellQ.GetLineFarmLocation(minions);

                if (LeagueSharp.Common.Geometry.IsValid(farmLocation.Position))
                    if (farmLocation.MinionsHit >= 2 || jungleMobs)
                        CastQ(farmLocation.Position);
            }

            minions = LeagueSharp.Common.MinionManager.GetMinions(EloBuddy.ObjectManager.Player.ServerPosition, _spellW.Range, LeagueSharp.Common.MinionTypes.All, LeagueSharp.Common.MinionTeam.NotAlly);

            if (minions.Count() > 0)
            {
                jungleMobs = minions.Any(x => x.Team == EloBuddy.GameObjectTeam.Neutral);

                if ((FarmM["farmW"].Cast<CheckBox>().CurrentValue  && EloBuddy.ObjectManager.Player.ManaPercent >=  FarmM["farmPercent"].Cast<Slider>().CurrentValue && EloBuddy.ObjectManager.Player.Level >= FarmM["farmStartAtLevel"].Cast<Slider>().CurrentValue) || jungleMobs)
                    CastW(true);
            }
        }

        static bool CastE()
        {
            if (!_spellE.IsReady())
            {
                return false;
            }

            var target = EloBuddy.SDK.TargetSelector.GetTarget(_spellE.Range, EloBuddy.DamageType.Magical);
            var predE = _spellQ.GetPrediction(target);
            if (target !=  null && !target.CanMove && predE.Hitchance >= HitChance.VeryHigh)
            {
                return _spellE.Cast(target) == Spell.CastStates.SuccessfullyCasted;
            }
            else if (target != null && predE.Hitchance >=  HitChance.VeryHigh && _spellE.WillHit(target,predE.CastPosition))
            {
                return _spellE.Cast(predE.CastPosition);
            }

            return false;
        }

        static void CastQ()
        {
            if (!_spellQ.IsReady())
            {
                return;
            }

            var target = EloBuddy.SDK.TargetSelector.GetTarget(_spellQ.Range, EloBuddy.DamageType.Magical);
            var predQ = _spellQ.GetPrediction(target);
            if (target !=  null && !target.CanMove && predQ.Hitchance >= HitChance.VeryHigh)
            {
                _spellQ.Cast(target);
            }
            else if (target != null && predQ.Hitchance >= HitChance.VeryHigh )
            {
                 _spellQ.Cast(predQ.CastPosition);
            }
        }

        static void CastQ(Vector2 pos)
        {
            if (!_spellQ.IsReady())
                return;

            _spellQ.Cast(pos);
        }

       static void CastW(bool ignoreTargetCheck = false)
        {
            if (!_spellW.IsReady())
            {
                return;
            }

            var target = EloBuddy.SDK.TargetSelector.GetTarget(_spellW.Range, EloBuddy.DamageType.Magical);

            if (target != null || ignoreTargetCheck)
            {
                _spellW.CastOnUnit(EloBuddy.ObjectManager.Player);
            }
        }

        static void Combo()
        {
            if (ComboM["comboE"].Cast<CheckBox>().CurrentValue)
            {
                if (CastE())
                {
                    return;
                }
            }

            if (ComboM["comboQ"].Cast<CheckBox>().CurrentValue)
            {
                CastQ();
            }


            if (ComboM["comboW"].Cast<CheckBox>().CurrentValue)
            {
                CastW();
            }


            if (ComboM["ComboR"].Cast<CheckBox>().CurrentValue && _spellR.IsReady())
            {
                if (OkToUlt())
                {
                    _spellR.Cast(EloBuddy.Game.CursorPos);
                }
            }
        }

        static List<EloBuddy.SpellSlot> GetSpellCombo()
        {
            var spellCombo = new List<EloBuddy.SpellSlot>();

            if (_spellQ.IsReady())
                spellCombo.Add(EloBuddy.SpellSlot.Q);
            if (_spellW.IsReady())
                spellCombo.Add(EloBuddy.SpellSlot.W);
            if (_spellE.IsReady())
                spellCombo.Add(EloBuddy.SpellSlot.E);
            if (_spellR.IsReady())
                spellCombo.Add(EloBuddy.SpellSlot.R);
            return spellCombo;
        }

        static float GetComboDamage(EloBuddy.Obj_AI_Base target)
        {
            double comboDamage = (float)EloBuddy.ObjectManager.Player.GetComboDamage(target, GetSpellCombo());

            return (float)(comboDamage + EloBuddy.ObjectManager.Player.LSGetAutoAttackDamage(target));
        }

        static bool OkToUlt()
        {
            if (Helper.EnemyTeam.Any(x => x.LSDistance(EloBuddy.ObjectManager.Player) < 500)) //any enemies around me?
                return true;

            Vector3 mousePos = EloBuddy.Game.CursorPos;

            var enemiesNearMouse = Helper.EnemyTeam.Where(x => x.LSDistance(EloBuddy.ObjectManager.Player) < _spellR.Range && x.LSDistance(mousePos) < 650);

            if (enemiesNearMouse.Count() > 0)
            {
                if (IsRActive()) //R already active
                    return true;

                bool enoughMana = EloBuddy.ObjectManager.Player.Mana > EloBuddy.ObjectManager.Player.Spellbook.GetSpell(EloBuddy.SpellSlot.Q).SData.Mana + EloBuddy.ObjectManager.Player.Spellbook.GetSpell(EloBuddy.SpellSlot.E).SData.Mana + EloBuddy.ObjectManager.Player.Spellbook.GetSpell(EloBuddy.SpellSlot.R).SData.Mana;

                if (ComboM["comboROnlyUserInitiate"].Cast<CheckBox>().CurrentValue || !(_spellQ.IsReady() && _spellE.IsReady()) || !enoughMana) //dont initiate if user doesnt want to, also dont initiate if Q and E isnt ready or not enough mana for QER combo
                    return false;

                var friendsNearMouse = Helper.OwnTeam.Where(x => x.IsMe || x.LSDistance(mousePos) < 650); //me and friends near mouse (already in fight)

                if (enemiesNearMouse.Count() == 1) //x vs 1 enemy
                {
                    EloBuddy.AIHeroClient enemy = enemiesNearMouse.FirstOrDefault();

                    bool underTower = Utility.UnderTurret(enemy);

                    return GetComboDamage(enemy) / enemy.Health >= (underTower ? 1.25f : 1); //if enemy under tower, only initiate if combo damage is >125% of enemy health
                }
                else //fight if enemies low health or 2 friends vs 3 enemies and 3 friends vs 3 enemies, but not 2vs4
                {
                    int lowHealthEnemies = enemiesNearMouse.Count(x => x.Health / x.MaxHealth <= 0.1); //dont count low health enemies

                    float totalEnemyHealth = enemiesNearMouse.Sum(x => x.Health);

                    return friendsNearMouse.Count() - (enemiesNearMouse.Count() - lowHealthEnemies) >= -1 || EloBuddy.ObjectManager.Player.Health / totalEnemyHealth >= 0.8;
                }
            }

            return false;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!EloBuddy.ObjectManager.Player.IsDead)
            {
                var drawQE = DrawM["drawQE"].Cast<CheckBox>().CurrentValue;
                var drawW = DrawM["drawW"].Cast<CheckBox>().CurrentValue;

                if (drawQE)
                    Render.Circle.DrawCircle(EloBuddy.ObjectManager.Player.Position, _spellQ.Range, System.Drawing.Color.FromArgb(125, 0, 255, 0));

                if (drawW)
                    Render.Circle.DrawCircle(EloBuddy.ObjectManager.Player.Position, _spellW.Range, System.Drawing.Color.FromArgb(125, 0, 0, 255));
            }
        }

        static float GetDynamicQSpeed(float distance)
        {
            var a = 0.5f * _spellQAcceleration;
            var b = _spellQSpeed;
            var c = -distance;

            if (b * b - 4 * a * c <= 0f)
            {
                return 0;
            }

            var t = (float)(-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            return distance / t;
        }

        static bool IsRActive()
        {
            return EloBuddy.ObjectManager.Player.HasBuff("AhriTumble");
        }

        static int GetRStacks()
        {
            return EloBuddy.ObjectManager.Player.GetBuffCount("AhriTumble");
        }
    }
}
