using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace Challenger_Series
{
    public class Irelia
    {
        public static Spell Q, W, E, R;

        private static int UseQComboStringList;

        private static bool UseWComboBool;

        private static int UseEComboStringList;

        private static bool UseEKSBool;

        private static bool UseRComboBool;

        private static int QGapcloseModeStringList;

        private static int MinDistForQGapcloser;

        private static int QFarmModeStringList;

        private static bool UseRComboKeybind;

        public static Menu config;

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

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(100, 50, 1600, false, SkillshotType.SkillshotLine);

            InitMenu();
            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPreAttack += OnOrbwalkerAction;
            Spellbook.OnCastSpell += OnCastSpell;
        }

        private static bool pressedR = false;

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R)
            {
                if (!pressedR)
                {
                    if (!ObjectManager.Player.HasBuff("ireliatranscendentbladesspell"))
                    {
                        args.Process = false;
                    }
                }
                else
                {
                    args.Process = true;
                    pressedR = false;
                }
            }
        }

        private static void OnOrbwalkerAction(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target is AIHeroClient && UseWComboBool)
            {
                W.Cast();
            }
        }

        public static void OnUpdate(EventArgs args)
        {
            UseQComboStringList = getBoxItem(config, "useqcombo");

            UseWComboBool = getCheckBoxItem(config, "usewcombo");

            UseEComboStringList = getBoxItem(config, "useecombo");

            UseEKSBool = getCheckBoxItem(config, "useeks");

            UseRComboBool = getCheckBoxItem(config, "usercombo");

            QGapcloseModeStringList = getBoxItem(config, "qgc");

            MinDistForQGapcloser = getSliderItem(config, "mindistqgapcloser");

            QFarmModeStringList = getBoxItem(config, "useqfarm");

            UseRComboKeybind = getKeyBindItem(config, "usercombo");

            var target = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (R.IsReady() && target != null && target.IsHPBarRendered)
            {
                if (UseRComboKeybind)
                {
                    pressedR = true;
                    R.Cast(R.GetPrediction(target).UnitPosition);
                }
                if (target != null)
                {
                    if (ObjectManager.Player.HasBuff("ireliatranscendentbladesspell"))
                    {
                        R.Cast(R.GetPrediction(target).UnitPosition);
                    }
                }
                if (ObjectManager.Player.HealthPercent < 15 || target.Health < R.GetDamage(target))
                {
                    R.Cast(R.GetPrediction(target).UnitPosition);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Q.IsReady())
                {
                    var killableEnemy =
                        ObjectManager.Get<AIHeroClient>()
                            .FirstOrDefault(
                                hero =>
                                    hero.IsEnemy && hero.IsValidTarget() && hero.Health < Q.GetDamage(hero) &&
                                    hero.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 650);
                    if (killableEnemy != null && killableEnemy.IsValidTarget())
                    {
                        Q.Cast(killableEnemy);
                    }

                    var qMode = UseQComboStringList;
                    if (qMode == 0)
                    {
                        var distBetweenMeAndTarget =
                            ObjectManager.Player.ServerPosition.Distance(target.ServerPosition);
                        if (distBetweenMeAndTarget > MinDistForQGapcloser)
                        {
                            if (distBetweenMeAndTarget < 650)
                            {
                                Q.Cast(target);
                            }
                            else
                            {
                                var minionGapclosingMode = QGapcloseModeStringList;
                                if (minionGapclosingMode == 0)
                                {
                                    var gapclosingMinion =
                                        ObjectManager.Get<Obj_AI_Minion>()
                                            .Where(
                                                m =>
                                                    m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <
                                                    650 &&
                                                    m.IsEnemy &&
                                                    m.ServerPosition.Distance(target.ServerPosition) <
                                                    distBetweenMeAndTarget && m.IsValidTarget() &&
                                                    m.Health < Q.GetDamage(m))
                                            .OrderBy(m => m.Position.Distance(target.ServerPosition))
                                            .FirstOrDefault();
                                    if (gapclosingMinion != null)
                                    {
                                        Q.Cast(gapclosingMinion);
                                    }
                                }
                                else
                                {
                                    var firstGapclosingMinion =
                                        ObjectManager.Get<Obj_AI_Minion>()
                                            .Where(
                                                m =>
                                                    m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <
                                                    650 && m.IsEnemy &&
                                                    m.ServerPosition.Distance(target.ServerPosition) <
                                                    distBetweenMeAndTarget &&
                                                    m.IsValidTarget() && m.Health < Q.GetDamage(m))
                                            .OrderByDescending(m => m.Position.Distance(target.ServerPosition))
                                            .FirstOrDefault();
                                    if (firstGapclosingMinion != null)
                                    {
                                        Q.Cast(firstGapclosingMinion);
                                    }
                                }
                            }
                        }
                    }
                    if (qMode == 1)
                    {
                        var distBetweenMeAndTarget =
                            ObjectManager.Player.ServerPosition.Distance(target.ServerPosition);
                        if (distBetweenMeAndTarget < 650)
                        {
                            Q.Cast(target);
                        }
                        else
                        {
                            var firstGapclosingMinion =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        m =>
                                            m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <
                                            650 && m.IsEnemy &&
                                            m.ServerPosition.Distance(target.ServerPosition) <
                                            distBetweenMeAndTarget &&
                                            m.IsValidTarget() && m.Health < Q.GetDamage(m))
                                    .OrderByDescending(m => m.Position.Distance(target.ServerPosition))
                                    .FirstOrDefault();
                            if (firstGapclosingMinion != null)
                            {
                                Q.Cast(firstGapclosingMinion);
                            }
                        }
                    }
                }
                if (E.IsReady())
                {
                    var killableEnemy =
                        ObjectManager.Get<AIHeroClient>()
                            .FirstOrDefault(
                                hero =>
                                    hero.IsEnemy && !hero.IsDead && hero.Health < E.GetDamage(hero) &&
                                    hero.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 425 &&
                                    hero.ServerPosition.Distance(ObjectManager.Player.ServerPosition) >
                                    ObjectManager.Player.GetAutoAttackRange());
                    if (!Q.IsReady() && UseEKSBool)
                    {
                        E.Cast(killableEnemy);
                    }

                    var eMode = UseEComboStringList;
                    if (eMode == 0)
                    {
                        if (ObjectManager.Player.HealthPercent <= target.HealthPercent)
                        {
                            E.Cast(target);
                        }
                        if (target.HealthPercent < ObjectManager.Player.HealthPercent &&
                            target.MoveSpeed > ObjectManager.Player.MoveSpeed - 5 &&
                            ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) > 300)
                        {
                            E.Cast(target);
                        }
                    }
                    if (eMode == 1)
                    {
                        E.Cast(target);
                    }
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var farmMode = QFarmModeStringList;
                switch (farmMode)
                {
                    case 0:
                    {
                        var unkillableMinion =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .FirstOrDefault(
                                    m =>
                                        m.IsEnemy && m.Position.Distance(ObjectManager.Player.ServerPosition) < 650 &&
                                        m.Position.Distance(ObjectManager.Player.Position) >
                                        ObjectManager.Player.AttackRange && m.IsValidTarget() &&
                                        m.Health < 25);
                        if (unkillableMinion != null)
                        {
                            Q.Cast(unkillableMinion);
                        }
                        break;
                    }
                    case 1:
                    {
                        var killableMinion =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .FirstOrDefault(
                                    m =>
                                        m.IsEnemy && m.Position.Distance(ObjectManager.Player.ServerPosition) < 650 &&
                                        m.IsValidTarget() && m.Health < Q.GetDamage(m));
                        if (killableMinion != null)
                        {
                            Q.Cast(killableMinion);
                        }
                        break;
                    }
                    case 2:
                    {
                        break;
                    }
                }
            }
        }

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Irelia", "Irelia");
            config.Add("useqcombo", new ComboBox("Q Combo MODE : ", 0, "CHALLENGER", "BRONZE", "NEVER"));
            config.Add("usewcombo", new CheckBox("Use W Combo"));
            config.Add("useecombo", new ComboBox("Use E Combo", 0, "CHALLENGER", "BRONZE", "NEVER"));
            config.Add("usercombo", new KeyBind("Use R Combo", false, KeyBind.BindTypes.HoldActive, 'R'));
            config.Add("useeks", new CheckBox("Use E KS if Q on CD"));
            config.Add("usercombo", new CheckBox("Use R Combo"));
            config.Add("qgc", new ComboBox("Q Gapcloser Mode : ", 0, "ONLY-CLOSEST-TO-TARGET", "ALL-KILLABLE-MINIONS"));
            config.Add("mindistqgapcloser", new Slider("Min Distance for Q Gapclose", 350, 325, 625));
            config.Add("useqfarm", new ComboBox("Q Farm Mode: ", 0, "ONLY-UNKILLABLE", "ALWAYS", "NEVER"));
        }
    }
}