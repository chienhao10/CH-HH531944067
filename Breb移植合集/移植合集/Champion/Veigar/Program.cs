using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SCommon.Database;
using Champion = SCommon.PluginBase.Champion;
using Spell = LeagueSharp.Common.Spell;
using TargetSelector = SCommon.TS.TargetSelector;

namespace SAutoCarry.Champions
{
    public class Veigar : Champion
    {
        public static Menu combo, harass, laneclear, misc, autoUlt;

        public Veigar()
            : base("Veigar", "SAutoCarry - Veigar")
        {
            CreateConfigMenu();
            OnUpdate += BeforeOrbwalk;
        }

        public void CreateConfigMenu()
        {
            ConfigMenu = MainMenu.AddMenu("SAutoCarry - Veigar", "Veigar");
            ConfigMenu.AddLabel("This is a very complicated addon that conflicts with the target selector of EB.");
            ConfigMenu.AddLabel("1. Just ignore EB's Target Selector.");

            combo = ConfigMenu.AddSubMenu("Combo", "SAutoCarry.Veigar.Combo");
            combo.Add("SAutoCarry.Veigar.Combo.UseQ", new CheckBox("Use Q"));
            combo.Add("SAutoCarry.Veigar.Combo.UseW", new CheckBox("Use W"));
            combo.Add("SAutoCarry.Veigar.Combo.UseE", new CheckBox("Use E"));
            combo.Add("SAutoCarry.Veigar.Combo.UseR", new CheckBox("Use R"));

            harass = ConfigMenu.AddSubMenu("Harass", "SAutoCarry.Veigar.Harass");
            harass.Add("SAutoCarry.Veigar.Harass.UseQ", new CheckBox("Use Q"));
            harass.Add("SAutoCarry.Veigar.Harass.UseW", new CheckBox("Use W"));
            harass.Add("SAutoCarry.Veigar.Harass.UseE", new CheckBox("Use E"));
            harass.Add("SAutoCarry.Veigar.Harass.Mana", new Slider("Min. Mana Percent", 60));

            laneclear = ConfigMenu.AddSubMenu("Lane/Jungle Clear", "SAutoCarry.Veigar.LaneClear");
            laneclear.Add("SAutoCarry.Veigar.LaneClear.UseQ", new CheckBox("Use Q"));
            laneclear.Add("SAutoCarry.Veigar.LaneClear.UseW", new CheckBox("Use W"));
            laneclear.Add("SAutoCarry.Veigar.LaneClear.MinW", new Slider("Min. Minions To W In Range", 4, 1, 12));
            laneclear.Add("SAutoCarry.Veigar.LaneClear.Mana", new Slider("Min. Mana Percent", 10));

            misc = ConfigMenu.AddSubMenu("Misc", "SAutoCarry.Veigar.Misc");
            misc.Add("SAutoCarry.Veigar.Misc.AutoQLastHit",
                new KeyBind("Auto Q Last Hit", false, KeyBind.BindTypes.PressToggle, 'T'));
            misc.Add("SAutoCarry.Veigar.Misc.AntiGapcloseE", new CheckBox("Anti Gap Closer With E"));
            misc.Add("SAutoCarry.Veigar.Misc.AutoWImmobile", new CheckBox("Auto W Immobile Target"));

            autoUlt = ConfigMenu.AddSubMenu("Auto Ult Settings (Killable)", "SAutoCarry.Veigar.AutoR");
            foreach (var enemy in HeroManager.Enemies)
            {
                autoUlt.Add("SAutoCarry.Veigar.AutoR.DontUlt" + enemy.ChampionName,
                    new CheckBox(string.Format("Dont Auto Ult {0}", enemy.ChampionName), false));
            }
            autoUlt.Add("SAutoCarry.Veigar.AutoR.Enabled", new CheckBox("Enabled"));
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 650 + 150);
            Spells[Q].SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 900 + 150);
            Spells[W].SetSkillshot(1.25f, 200f, 0, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 700);
            Spells[E].SetSkillshot(0.5f, 350f, 0, false, SkillshotType.SkillshotCircle);

            Spells[R] = new Spell(SpellSlot.R, 650);
        }

        public void BeforeOrbwalk()
        {
            var autoQ = getKeyBindItem(misc, "SAutoCarry.Veigar.Misc.AutoQLastHit");
            var autoW = getCheckBoxItem(misc, "SAutoCarry.Veigar.Misc.AutoWImmobile");
            var autoR = getCheckBoxItem(autoUlt, "SAutoCarry.Veigar.AutoR.Enabled");

            if (Spells[W].IsReady() || Spells[R].IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[W].Range)))
                {
                    if (Spells[W].IsInRange(enemy))
                    {
                        if (enemy.IsValidTarget(Spells[W].Range - 150) && enemy.IsImmobilized() && autoW)
                            Spells[W].Cast(enemy.ServerPosition);
                    }

                    if (Spells[R].IsInRange(enemy))
                    {
                        if (enemy.IsValidTarget(Spells[R].Range) && autoR &&
                            !getCheckBoxItem(autoUlt, "SAutoCarry.Veigar.AutoR.DontUlt" + enemy.ChampionName))
                        {
                            if (CalculateDamageR(enemy) >= enemy.Health)
                                Spells[R].CastOnUnit(enemy);
                        }
                    }
                }
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
                LaneClear();
            }

            if (autoQ && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                StackQ();
        }

        public void Combo()
        {
            var waitforE = false;
            if (Spells[E].IsReady() && getCheckBoxItem(combo, "SAutoCarry.Veigar.Combo.UseE"))
            {
                var t = TargetSelector.GetTarget(1000, DamageType.Magical);
                if (t != null)
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(t, 0.2f, t.BoundingRadius);
                    var pos = pred.UnitPosition;
                    var extendpos = pos.Extend(ObjectManager.Player.ServerPosition, 360);
                    Spells[E].Cast(extendpos); 
                    waitforE = true;
                }
            }

            if (Spells[W].IsReady() && getCheckBoxItem(combo, "SAutoCarry.Veigar.Combo.UseW") && !waitforE)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, DamageType.Magical);
                if (t != null)
                    Spells[W].CastIfHitchanceEquals(t, HitChance.High);
            }

            if (Spells[R].IsReady() && getCheckBoxItem(combo, "SAutoCarry.Veigar.Combo.UseR"))
            {
                var t = TargetSelector.GetTarget(Spells[R].Range, DamageType.Magical);
                if (t != null && CalculateComboDamage(t, 4) >= t.Health)
                    Spells[R].CastOnUnit(t);
            }

            if (Spells[Q].IsReady() && getCheckBoxItem(combo, "SAutoCarry.Veigar.Combo.UseQ"))
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, DamageType.Magical);
                if (t != null)
                    Spells[Q].CastIfHitchanceEquals(t, HitChance.High);
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(harass, "SAutoCarry.Veigar.Harass.Mana"))
                return;

            var waitforE = false;

            if (Spells[E].IsReady() && getCheckBoxItem(harass, "SAutoCarry.Veigar.Harass.UseE"))
            {
                var t = TargetSelector.GetTarget(1000, DamageType.Magical);
                if (t != null)
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(t, 0.2f, t.BoundingRadius);
                    var pos = pred.UnitPosition;
                    var extendpos = pos.Extend(ObjectManager.Player.ServerPosition, 360);
                    Spells[E].Cast(extendpos);
                    waitforE = true;
                }
            }

            if (Spells[W].IsReady() && getCheckBoxItem(harass, "SAutoCarry.Veigar.Harass.UseW") && !waitforE)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, DamageType.Magical);
                if (t != null)
                    Spells[W].CastIfHitchanceEquals(t, HitChance.High);
            }

            if (Spells[Q].IsReady() && getCheckBoxItem(harass, "SAutoCarry.Veigar.Harass.UseQ"))
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, DamageType.Magical);
                if (t != null && Spells[Q].GetPrediction(t).Hitchance != HitChance.Collision)
                {
                    Spells[Q].CastIfHitchanceEquals(t, HitChance.High);
                }
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(laneclear, "SAutoCarry.Veigar.LaneClear.Mana"))
                return;

            if (getCheckBoxItem(laneclear, "SAutoCarry.Veigar.LaneClear.UseQ"))
                StackQ();

            if (getCheckBoxItem(laneclear, "SAutoCarry.Veigar.LaneClear.UseW"))
            {
                var farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Spells[W].Range, MinionTypes.All, MinionTeam.NotAlly,
                            MinionOrderTypes.MaxHealth).Select(q => q.ServerPosition.To2D()).ToList(), Spells[W].Width,
                        Spells[W].Range);
                if (farmLocation.MinionsHit >= getSliderItem(laneclear, "SAutoCarry.Veigar.LaneClear.MinW"))
                    Spells[W].Cast(farmLocation.Position);
            }
        }

        private void StackQ()
        {
            if (Spells[Q].IsReady())
            {
                var farmLocation =
                    MinionManager.GetBestLineFarmLocation(
                        MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly,
                            MinionOrderTypes.MaxHealth)
                            .Where(p => ObjectManager.Player.GetSpellDamage(p, SpellSlot.Q) >= p.Health)
                            .Select(q => q.ServerPosition.To2D())
                            .ToList(), Spells[Q].Width, Spells[Q].Range);
                if (farmLocation.MinionsHit > 0)
                    Spells[Q].Cast(farmLocation.Position);
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(misc, "SAutoCarry.Veigar.Misc.AntiGapcloseE"))
            {
                if (gapcloser.End.Distance(ObjectManager.Player.ServerPosition) < 350)
                    Spells[E].Cast(ObjectManager.Player.ServerPosition);
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

        public override double CalculateDamageR(AIHeroClient target)
        {
            if (!Spells[R].IsReady())
                return 0;

            return ObjectManager.Player.CalcDamage(target, DamageType.Magical,
                new[] { 250, 375, 500 }[Spells[R].Level - 1] + ObjectManager.Player.FlatMagicDamageMod +
                target.FlatMagicDamageMod * 0.8);
        }
    }
}