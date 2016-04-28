/*
 

                                                    __                    .-'''-.                                                
       .-''-.                    .---.         ...-'  |`. _______        '   _    \                                              
     .' .-.  )         .--.      |   |         |      |  |\  ___ `'.   /   /` '.   \   _..._               __.....__             
    / .'  / /    .--./)|__|      |   |         ....   |  | ' |--.\  \ .   |     \  ' .'     '.  .--./) .-''         '.           
   (_/   / /    /.''\\ .--.-,.--.|   |           -|   |  | | |    \  '|   '      |  .   .-.   ./.''\\ /     .-''"'-.  `..-,.--.  
        / /    | |  | ||  |  .-. |   |            |   |  | | |     |  \    \     / /|  '   '  | |  | /     /________\   |  .-. | 
       / /      \`-' / |  | |  | |   |      _  ...'   `--' | |     |  |`.   ` ..' / |  |   |  |\`-' /|                  | |  | | 
      . '       /("'`  |  | |  | |   |    .' | |         |`| |     ' .'   '-...-'`  |  |   |  |/("'` \    .-------------| |  | | 
     / /    _.-'\ '---.|  | |  '-|   |   .   | ` --------\ | |___.' /'              |  |   |  |\ '---.\    '-.____...---| |  '-  
   .' '  _.'.-'' /'""'.|__| |    |   | .'.'| |//`---------/_______.'/               |  |   |  | /'""'.\`.             .'| |      
  /  /.-'_.'    ||     || | |    '---.'.'.-'  /           \_______|/                |  |   |  |||     || `''-...... -'  | |      
 /    _.'       \'. __//  |_|        .'   \_.'                                      |  |   |  |\'. __//                 |_|      
( _.-'           `'---'                                                             '--'   '--' `'---'                           

*/

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace Two_Girls_One_Donger
{
    internal class Program
    {
        private const string Champion = "Heimerdinger";
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell W1;
        private static Spell E1;
        public static Spell E2;
        public static Spell E3;
        public static SpellSlot Ignite = 0;
        private static Spell R;
        private static Menu Config, comboMenu, laneClearMenu, miscMenu;
        private static Items.Item ZHO;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
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

        public static void Game_OnGameLoad()
        {
            if (Player.CharData.BaseSkinName != Champion) return;

            #region spells

            ZHO = new Items.Item(3157, 1f);
            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 100);

            W1 = new Spell(SpellSlot.W, 1100);
            E1 = new Spell(SpellSlot.E, 925);
            E2 = new Spell(SpellSlot.E, 1125);
            E3 = new Spell(SpellSlot.E, 1325);

            Q.SetSkillshot(0.5f, 40f, 1100f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            W1.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            E1.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            E2.SetSkillshot(0.25f + E1.Delay, 120f, 1200f, false, SkillshotType.SkillshotLine);
            E3.SetSkillshot(0.3f + E2.Delay, 120f, 1200f, false, SkillshotType.SkillshotLine);

            #endregion

            #region Menu

            //Menu
            Config = MainMenu.AddMenu(Champion, "2Girls1Donger");

            //LaneClear
            laneClearMenu = Config.AddSubMenu("Laneclear", "Laneclear");
            laneClearMenu.Add("LaneclearW", new CheckBox("Use W"));
            laneClearMenu.Add("LaneclearE", new CheckBox("Use E", false));
            laneClearMenu.Add("LaneMana", new Slider("Minimum Mana for clear", 30));

            //C-C-C-Combo
            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseQRCombo", new CheckBox("Use Q Upgrade"));
            comboMenu.Add("QRcount", new Slider("Minimum Enemies for Q upgrade", 2, 1, 5));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseWRCombo", new CheckBox("Use W Upgrade"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));
            comboMenu.Add("UseERCombo", new CheckBox("Use E Upgrade"));
            comboMenu.Add("ERcount", new Slider("Minimum Enemies to Stun", 3, 1, 5));
            comboMenu.Add("KS", new CheckBox("Killsteal"));
            comboMenu.Add("ZhoUlt", new CheckBox("Ult + Q > Zhonyas"));

            //MISCMENU
            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("AntiGap", new CheckBox("Anti Gapcloser - E", false));
            miscMenu.Add("Interrupt", new CheckBox("Interrupt Spells - E", false));
            miscMenu.Add("AutoHarras", new KeyBind("Auto Harass W", false, KeyBind.BindTypes.PressToggle, 'J'));
            miscMenu.Add("ManaW", new Slider("Auto Harass if % MP >", 30, 1));

            // Interruption
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapCloser_OnEnemyGapcloser;
            Game.OnUpdate += OnGameUpdate;
        }

        #endregion

        #region wip

        private static void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            var lanemana = getSliderItem(laneClearMenu, "LaneMana");
            var MinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width);
            var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width);
            var Wfarmpos = W.GetLineFarmLocation(MinionsW, W.Width);
            var Efarmpos = E.GetCircularFarmLocation(MinionsE, E.Width);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Wfarmpos.MinionsHit >= 3 &&
                getCheckBoxItem(laneClearMenu, "LaneclearW")
                && Player.ManaPercent >= lanemana)
            {
                W.Cast(Wfarmpos.Position);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Efarmpos.MinionsHit >= 3 &&
                MinionsE.Count >= 1 && getCheckBoxItem(laneClearMenu, "LaneclearE")
                && Player.ManaPercent >= lanemana)
            {
                E.Cast(Efarmpos.Position);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (getCheckBoxItem(comboMenu, "KS"))
            {
                KS();
            }
            if (getCheckBoxItem(comboMenu, "ZhoUlt"))
            {
                ZhoUlt();
            }
            if (getKeyBindItem(miscMenu, "AutoHarras"))
            {
                AutoHarras();
            }
        }


        private static void CastER(Obj_AI_Base target) // copied from ScienceARK
        {
            PredictionOutput prediction;

            if (ObjectManager.Player.Distance(target) < E1.Range)
            {
                var oldrange = E1.Range;
                E1.Range = E2.Range;
                prediction = E1.GetPrediction(target, true);
                E1.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < E2.Range)
            {
                var oldrange = E2.Range;
                E2.Range = E3.Range;
                prediction = E2.GetPrediction(target, true);
                E2.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < E3.Range)
            {
                prediction = E3.GetPrediction(target, true);
            }
            else
            {
                return;
            }

            if (prediction.Hitchance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <= E1.Range + E1.Width)
                {
                    Vector3 p;
                    if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) > 300)
                    {
                        p = prediction.CastPosition -
                            100*
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized()
                                .To3D();
                    }
                    else
                    {
                        p = prediction.CastPosition;
                    }
                    R.Cast();
                    E1.Cast(p);
                }
                else if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <=
                         (E1.Range + E1.Range)/2)
                {
                    var p = ObjectManager.Player.ServerPosition.To2D()
                        .Extend(prediction.CastPosition.To2D(), E1.Range - 100);
                    {
                        R.Cast();
                        E1.Cast(p.To3D());
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.To2D() +
                            E1.Range*
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized
                                ();

                    {
                        R.Cast();
                        E1.Cast(p.To3D());
                    }
                }
            }
        }

        private static void ZhoUlt()
        {
            var fullHP = Player.MaxHealth;
            var HP = Player.Health;
            var critHP = fullHP/4;
            if (HP <= critHP)
            {
                var target = TargetSelector.GetTarget(1000, DamageType.Magical);
                if (target == null) return;
                R.Cast();
                Utility.DelayAction.Add(1010, () => Q.Cast(Player.Position));
                Utility.DelayAction.Add(500, () => Q.Cast(Player.Position));
                Utility.DelayAction.Add(1200, () => ZHO.Cast());
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (target == null)
                return;
            var qtarget = TargetSelector.GetTarget(600, DamageType.Magical);
            if (qtarget == null)
                return;
            var wpred = W.GetPrediction(target);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Q.IsReady() && R.IsReady() && getCheckBoxItem(comboMenu, "UseQRCombo") &&
                    getCheckBoxItem(comboMenu, "UseQCombo") && qtarget.IsValidTarget(650) &&
                    Player.Position.CountEnemiesInRange(650) >=
                    getSliderItem(comboMenu, "QRcount"))
                {
                    R.Cast();
                    Q.Cast(Player.Position.Extend(target.Position, +300));
                }
                else
                {
                    if (Q.IsReady() && getCheckBoxItem(comboMenu, "UseQCombo") && qtarget.IsValidTarget(650) &&
                        Player.Position.CountEnemiesInRange(650) >= 1)
                    {
                        Q.Cast(Player.Position.Extend(target.Position, +300));
                    }
                }
                if (E3.IsReady() && R.IsReady() && getCheckBoxItem(comboMenu, "UseERCombo") &&
                    getCheckBoxItem(comboMenu, "UseRCombo") &&
                    target.Position.CountEnemiesInRange(450 - 250) >=
                    getSliderItem(comboMenu, "ERcount"))
                {
                    CastER(target);
                }
                else
                {
                    if (E.IsReady() && getCheckBoxItem(comboMenu, "UseECombo") && target.IsValidTarget(E.Range))
                    {
                        E.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                    if (W.IsReady() && getCheckBoxItem(comboMenu, "UseWRCombo") &&
                        getCheckBoxItem(comboMenu, "UseRCombo") &&
                        R.IsReady() && target.IsValidTarget(W.Range) &&
                        wpred.Hitchance >= HitChance.High && CalcDamage(target) >= target.Health)
                    {
                        R.Cast();

                        Utility.DelayAction.Add(1010,
                            () => W.CastIfHitchanceEquals(target, HitChance.High, true));
                    }
                    else
                    {
                        if (W.IsReady() && getCheckBoxItem(comboMenu, "UseWCombo") && target.IsValidTarget(W.Range))
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                        }
                    }
                }
            }
        }

        private static void AutoHarras()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (target == null || !target.IsValid)
                return;
            var harassmana = getSliderItem(miscMenu, "ManaW");
            var useW = getKeyBindItem(miscMenu, "AutoHarras");
            if (W.IsReady() && target.IsValidTarget() && useW && Player.Mana/Player.MaxMana*100 > harassmana)
            {
                W.CastIfHitchanceEquals(target, HitChance.High, true);
            }
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            //Calculate Combo Damage

            var aa = Player.GetAutoAttackDamage(target, true);
            var damage = aa;

            if (Ignite != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(Ignite) == SpellState.Ready)
                damage += (float) ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);


            if (getCheckBoxItem(comboMenu, "UseE")) // edamage
            {
                if (E.IsReady())
                {
                    damage += E.GetDamage(target);
                }
            }

            if (E.IsReady() && getCheckBoxItem(comboMenu, "UseE")) // rdamage
            {
                damage += E.GetDamage(target);
            }

            if (W.IsReady() && getCheckBoxItem(comboMenu, "UseW"))
            {
                damage += W.GetDamage(target);
            }
            if (W.IsReady() && getCheckBoxItem(comboMenu, "UseW"))
            {
                if (R.IsReady() && getCheckBoxItem(comboMenu, "UseW") && getCheckBoxItem(comboMenu, "UseR"))
                    damage += (float) (W.GetDamage(target)*2.2);
            }
            return (int) damage;
        }

        private static void KS()
        {
            var target = TargetSelector.GetTarget(E.Range + 200, DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetEDamage())
            {
                E.CastIfHitchanceEquals(target, HitChance.Medium, true);
                E.CastIfHitchanceEquals(target, HitChance.High, true);
                return;
            }


            target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetWDamage())
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High &&
                    prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    W.Cast(prediction.CastPosition);
                    return;
                }
            }

            target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetW1Damage() && R.IsReady())
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High &&
                    prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    R.Cast();
                    W.Cast(prediction.CastPosition);
                    W.Cast(prediction.CastPosition);
                }
            }
        }

        private static float GetWDamage()
        {
            var target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return 0;
            var damage = 0d;

            if (W.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.W);

            return (float) damage*2;
        }

        private static float GetW1Damage()
        {
            var target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return 0;
            var damage = 0d;

            if (W1.IsReady() && R.IsReady())
                damage += Player.LSGetSpellDamage(target, SpellSlot.W, 1);

            return (float) damage*2;
        }

        private static float GetEDamage()
        {
            var target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return 0;
            var damage = 0d;

            if (E.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.E);

            return (float) damage*2;
        }

        #endregion

        #region Misc

        private static void AntiGapCloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range) && getCheckBoxItem(miscMenu, "AntiGap"))
                E.Cast(gapcloser.End);
        }


        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range) && getCheckBoxItem(miscMenu, "Interrupt"))
                E.Cast(sender.Position);
        }

        #endregion
    }
}