using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Champion = SCommon.PluginBase.Champion;
using Spell = LeagueSharp.Common.Spell;
using TargetSelector = SCommon.TS.TargetSelector;

namespace SAutoCarry.Champions
{
    public class Cassiopeia : Champion
    {
        public static Menu rootMenu, comboMenu, harassMenu, laneClearMenu, miscMenu;

        public Cassiopeia() : base("Cassiopeia", "SAutoCarry - Cassiopeia")
        {
            Game.OnUpdate += BeforeOrbwalk;
            CreateConfigMenu();
        }


        public bool ComboUseQ
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Cassiopeia.Combo.UseQ"); }
        }

        public bool ComboUseE
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Cassiopeia.Combo.UseE"); }
        }

        public bool ComboUseW
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Cassiopeia.Combo.UseW"); }
        }

        public int ComboUseRMin
        {
            get { return getSliderItem(comboMenu, "SAutoCarry.Cassiopeia.Combo.UseR"); }
        }

        public bool HarassUseQ
        {
            get { return getCheckBoxItem(harassMenu, "SAutoCarry.Cassiopeia.Harass.UseQ"); }
        }

        public bool HarassUseW
        {
            get { return getCheckBoxItem(harassMenu, "SAutoCarry.Cassiopeia.Harass.UseW"); }
        }

        public bool HarassUseE
        {
            get { return getCheckBoxItem(harassMenu, "SAutoCarry.Cassiopeia.Harass.UseE"); }
        }

        public int HarassMinMana
        {
            get { return getSliderItem(harassMenu, "SAutoCarry.Cassiopeia.Harass.MinMana"); }
        }

        public bool LaneClearQ
        {
            get { return getCheckBoxItem(laneClearMenu, "SAutoCarry.Cassiopeia.LaneClear.UseQ"); }
        }

        public bool LaneClearW
        {
            get { return getCheckBoxItem(laneClearMenu, "SAutoCarry.Cassiopeia.LaneClear.UseW"); }
        }

        public bool LaneClearE
        {
            get { return getCheckBoxItem(laneClearMenu, "SAutoCarry.Cassiopeia.LaneClear.UseE"); }
        }

        public bool LaneClearToggle
        {
            get { return getKeyBindItem(laneClearMenu, "SAutoCarry.Cassiopeia.LaneClear.Toggle"); }
        }

        public int LaneClearMinMana
        {
            get { return getSliderItem(laneClearMenu, "SAutoCarry.Cassiopeia.LaneClear.MinMana"); }
        }

        public bool KillStealE
        {
            get { return getCheckBoxItem(miscMenu, "SAutoCarry.Cassiopeia.Misc.EKillSteal"); }
        }

        public bool KillStealOnlyPoison
        {
            get { return getCheckBoxItem(miscMenu, "SAutoCarry.Cassiopeia.Misc.KSOnlyPoision"); }
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

        public static void CreateConfigMenu()
        {
            rootMenu = MainMenu.AddMenu("SAutoCarry - Cassiopeia", "Cass");

            comboMenu = rootMenu.AddSubMenu("Combo", "SAutoCarry.Cassiopeia.Combo");
            comboMenu.Add("SAutoCarry.Cassiopeia.Combo.UseQ", new CheckBox("Use Q"));
            comboMenu.Add("SAutoCarry.Cassiopeia.Combo.UseW", new CheckBox("Use W"));
            comboMenu.Add("SAutoCarry.Cassiopeia.Combo.UseE", new CheckBox("Use E"));
            comboMenu.Add("SAutoCarry.Cassiopeia.Combo.UseR", new Slider("Use R Min", 1, 1, 5));

            harassMenu = rootMenu.AddSubMenu("Harass", "SAutoCarry.Cassiopeia.Harass");
            harassMenu.Add("SAutoCarry.Cassiopeia.Harass.UseQ", new CheckBox("Use Q"));
            harassMenu.Add("SAutoCarry.Cassiopeia.Harass.UseW", new CheckBox("Use W"));
            harassMenu.Add("SAutoCarry.Cassiopeia.Harass.UseE", new CheckBox("Use E"));
            harassMenu.Add("SAutoCarry.Cassiopeia.Harass.MinMana", new Slider("Min Mana Percent", 30));

            laneClearMenu = rootMenu.AddSubMenu("LaneClear/JungleClear", "SAutoCarry.Cassiopeia.LaneClear");
            laneClearMenu.Add("SAutoCarry.Cassiopeia.LaneClear.UseQ", new CheckBox("Use Q"));
            laneClearMenu.Add("SAutoCarry.Cassiopeia.LaneClear.UseW", new CheckBox("Use W"));
            laneClearMenu.Add("SAutoCarry.Cassiopeia.LaneClear.UseE", new CheckBox("Use E"));
            laneClearMenu.Add("SAutoCarry.Cassiopeia.LaneClear.Toggle",
                new KeyBind("Enabled Spell Farm", false, KeyBind.BindTypes.PressToggle, 'T'));
            laneClearMenu.Add("SAutoCarry.Cassiopeia.LaneClear.MinMana", new Slider("Min Mana Percent", 50));

            miscMenu = rootMenu.AddSubMenu("Misc", "SAutoCarry.Cassiopeia.Misc");
            miscMenu.Add("SAutoCarry.Cassiopeia.Misc.EKillSteal", new CheckBox("KS With E"));
            miscMenu.Add("SAutoCarry.Cassiopeia.Misc.KSOnlyPoision", new CheckBox("^ Only KS If Target Has poison"));
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 850f);
            Spells[Q].SetSkillshot(0.75f, Spells[Q].Instance.SData.CastRadius, float.MaxValue, false,
                SkillshotType.SkillshotCircle);

            Spells[W] = new Spell(SpellSlot.W, 850f);
            Spells[W].SetSkillshot(0.5f, Spells[W].Instance.SData.CastRadius, Spells[W].Instance.SData.MissileSpeed,
                false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 750f);
            Spells[E].SetTargetted(0.2f, float.MaxValue);

            Spells[R] = new Spell(SpellSlot.R, 460f);
            Spells[R].SetSkillshot(0.3f, (float) (80*Math.PI/180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        public void BeforeOrbwalk(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (KillStealE)
                KillSteal();
        }

        public void Combo()
        {
            if (Spells[Q].IsReady() && ComboUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, DamageType.Magical);
                if (t != null)
                {
                    Spells[Q].Cast(t);   
                }
            }

            if (!Spells[Q].IsReady() && Spells[W].IsReady() && ComboUseW)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, DamageType.Magical);
                if (t != null)
                    Spells[W].Cast(t);
            }

            if (Spells[E].IsReady() && ComboUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, DamageType.Magical);
                if (t != null && t.HasBuffOfType(BuffType.Poison))
                    Spells[E].CastOnUnit(t);
            }


            if (Spells[R].IsReady())
            {
                if (ComboUseRMin == 1)
                {
                    var t = TargetSelector.GetTarget(Spells[R].Range, DamageType.Magical);
                    if (t != null)
                    {
                        var pred = Spells[R].GetPrediction(t);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            if (t.IsFacing(ObjectManager.Player))
                                Spells[R].Cast(pred.CastPosition);
                        }
                    }
                }
                else
                {
                    var t = TargetSelector.GetTarget(Spells[R].Range, DamageType.Magical);
                    if (Spells[R].GetHitCount() >= ComboUseRMin)
                    {
                        Spells[R].Cast(Spells[R].GetPrediction(t).CastPosition);
                    }
                }
            }
        }


        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassMinMana)
                return;

            if (Spells[Q].IsReady() && HarassUseQ)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, DamageType.Magical);
                if (t != null)
                    Spells[Q].CastIfHitchanceEquals(t, HitChance.High);
            }

            if (!Spells[Q].IsReady() && Spells[W].IsReady() && HarassUseW)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, DamageType.Magical);
                if (t != null)
                    Spells[W].CastIfHitchanceEquals(t, HitChance.High);
            }

            if (Spells[E].IsReady() && HarassUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, DamageType.Magical);
                if (t != null && t.HasBuffOfType(BuffType.Poison))
                    Spells[E].Cast();
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana || !LaneClearToggle)
                return;

            if (Spells[Q].IsReady() && LaneClearQ)
            {
                var farm = Spells[Q].GetCircularFarmLocation(MinionManager.GetMinions(Spells[Q].Range + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth));
                if (farm.MinionsHit > 1)
                {
                    Spells[Q].Cast(farm.Position);   
                }
            }

            if (!Spells[Q].IsReady() && Spells[W].IsReady() && LaneClearW)
            {
                var farm = Spells[W].GetCircularFarmLocation(MinionManager.GetMinions(Spells[W].Range + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth));
                if (farm.MinionsHit > 2)
                {
                    Spells[W].Cast(farm.Position);   
                }
            }

            if (Spells[E].IsReady() && LaneClearE)
            {
                var minion = MinionManager.GetMinions(Spells[E].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault(p => p.HasBuffOfType(BuffType.Poison) && Spells[E].IsKillable(p));
                if (minion != null)
                {
                    Spells[E].Cast(minion);
                }
            }
        }

        public void KillSteal()
        {
            if (!Spells[E].IsReady())
                return;

            foreach (
                var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.IsValidTarget(Spells[E].Range) && !x.HasBuffOfType(BuffType.Invulnerability) &&
                            (!KillStealOnlyPoison || x.HasBuffOfType(BuffType.Poison))))
            {
                if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 20)
                    Spells[E].CastOnUnit(target);
            }
        }
    }
}