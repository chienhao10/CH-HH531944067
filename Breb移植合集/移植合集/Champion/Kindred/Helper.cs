using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Kindred___YinYang.Spell_Database;
using LeagueSharp.Common;
using Geometry = LeagueSharp.Common.Geometry;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;

namespace Kindred___YinYang
{
    internal class Helper
    {
        private static readonly AIHeroClient Kindred = ObjectManager.Player;

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

        public static void AntiRengarOnCreate(GameObject sender, EventArgs args)
        {
            if (getCheckBoxItem(Language.miscMenu, "anti.rengar") && Program.R.IsReady() && sender.IsEnemy &&
                !sender.IsAlly && !sender.IsDead
                && sender.Name == "Rengar_LeapSound.troy" &&
                Kindred.HealthPercent < getSliderItem(Language.miscMenu, "hp.percent.for.rengar"))
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(x => x.IsValidTarget(1000) && x.ChampionName == "Rengar"))
                {
                    Program.R.Cast();
                }
            }
        }

        public static void SpellBreaker()
        {
            if (getCheckBoxItem(Language.miscMenu, "katarina.r") && Program.R.IsReady() &&
                Kindred.HealthPercent < getSliderItem(Language.miscMenu, "hp.percent.for.broke"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.ChampionName == "Katarina" && x.IsValidTarget(Program.R.Range) &&
                                x.HasBuff("katarinarsound") && !Kindred.IsDead && !x.IsDead && !x.IsZombie))
                {
                    Program.R.Cast();
                }
            }
            if (getCheckBoxItem(Language.miscMenu, "lucian.r") && Program.R.IsReady() &&
                Kindred.HealthPercent < getSliderItem(Language.miscMenu, "hp.percent.for.broke"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.ChampionName == "Lucian" && x.IsValidTarget(Program.R.Range) && x.HasBuff("lucianr") &&
                                !Kindred.IsDead && !x.IsDead && !x.IsZombie))
                {
                    Program.R.Cast();
                }
            }
            if (getCheckBoxItem(Language.miscMenu, "missfortune.r") && Program.R.IsReady() &&
                Kindred.HealthPercent < getSliderItem(Language.miscMenu, "hp.percent.for.broke"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.ChampionName == "MissFortune" && x.IsValidTarget(Program.R.Range) &&
                                x.HasBuff("missfortunebulletsound") && !Kindred.IsDead && !x.IsDead && !x.IsZombie))
                {
                    Program.R.Cast();
                }
            }
        }

        public static void AntiGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) <= 300)
            {
                Program.Q.Cast(gapcloser.End.Extend(ObjectManager.Player.ServerPosition,
                    ObjectManager.Player.LSDistance(gapcloser.End) + Program.Q.Range));
            }
        }

        public static int AaIndicator(AIHeroClient enemy)
        {
            var aCalculator = ObjectManager.Player.CalcDamage(enemy, DamageType.Physical, Kindred.TotalAttackDamage);
            var killableAaCount = enemy.Health/aCalculator;
            var totalAa = (int) Math.Ceiling(killableAaCount);
            return totalAa;
        }

        public static void Protector(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!Program.R.IsReady() && Kindred.IsDead && Kindred.IsZombie && sender.IsAlly && !sender.IsMe &&
                !getCheckBoxItem(Language.miscMenu, "protector"))
            {
                return;
            }
            if (sender is AIHeroClient && Program.R.IsReady() && sender.IsEnemy && !spell.SData.IsAutoAttack()
                && !sender.IsDead && !sender.IsZombie && sender.IsValidTarget(1000))
            {
                foreach (var protector in SpellDatabase.Spells.Where(x => x.spellName == spell.SData.Name
                                                                          &&
                                                                          getCheckBoxItem(Language.miscMenu,
                                                                              "hero." + x.spellName)))
                {
                    if (protector.spellType == SpellType.Circular && Kindred.LSDistance(spell.End) <= 200 &&
                        sender.LSGetSpellDamage(Kindred, protector.spellName) > Kindred.Health)
                    {
                        Program.R.Cast();
                    }
                    if (protector.spellType == SpellType.Cone && Kindred.LSDistance(spell.End) <= 200 &&
                        sender.LSGetSpellDamage(Kindred, protector.spellName) > Kindred.Health)
                    {
                        Program.R.Cast();
                    }
                    if (protector.spellType == SpellType.Line && Kindred.LSDistance(spell.End) <= 200
                        && sender.LSGetSpellDamage(Kindred, protector.spellName) > Kindred.Health)
                    {
                        Program.R.Cast();
                    }
                }
            }
        }

        public static void ClassicUltimate()
        {
            var minHp = getSliderItem(Language.miscMenu, "min.hp.for.r");
            foreach (var ally in HeroManager.Allies.Where(o => o.HealthPercent < minHp && !o.IsRecalling() && !o.IsDead && !o.IsZombie && Kindred.LSDistance(o.Position) < Program.R.Range && !o.InFountain()))
            {
                if (getCheckBoxItem(Language.miscMenu, "respite." + ally.CharData.BaseSkinName) && Kindred.CountEnemiesInRange(1500) >= 1 && ally.CountEnemiesInRange(1500) >= 1)
                {
                    Program.R.Cast();
                }
            }
        }

        public static void AdvancedQ(Spell spell, AIHeroClient unit, int count)
        {
            switch (getBoxItem(Language.comboMenu, "q.combo.style"))
            {
                case 0:
                    spell.Cast(Game.CursorPos);
                    break;
                case 1:
                    if (unit == null) return;
                    CollisionObjectCheckCast(spell, unit, count);
                    break;
                case 2:
                    if (unit == null) return;
                    CastSafePosition(spell, unit);
                    break;
            }
        }

        public static void CastSafePosition(Spell spell, AIHeroClient hero)
        {
            if (
                Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                    Prediction.GetPrediction(hero, 0f, hero.AttackRange).UnitPosition.To2D(), spell.Range,
                    Orbwalking.GetRealAutoAttackRange(hero)).Count() > 0)
            {
                spell.Cast(
                    Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                        Prediction.GetPrediction(hero, 0f, hero.AttackRange).UnitPosition.To2D(), spell.Range,
                        Orbwalking.GetRealAutoAttackRange(hero)).MinOrDefault(i => i.LSDistance(Game.CursorPos)));
            }
            else
            {
                spell.Cast(ObjectManager.Player.ServerPosition.Extend(hero.ServerPosition, -spell.Range));
            }
        }

        private static void CollisionObjectCheckCast(Spell spell, AIHeroClient unit, int count)
        {
            if (spell.GetPrediction(unit).CollisionObjects.Count < count)
            {
                if ((spell.GetPrediction(unit).CollisionObjects.Any(x => x.IsChampion() && x.IsEnemy)))
                {
                    spell.Cast(Game.CursorPos);
                }
            }
        }

        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient &&
                args.Target.IsValid)
            {
                if (Program.Q.IsReady() && getCheckBoxItem(Language.comboMenu, "q.combo") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < Program.Q.Range &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    AdvancedQ(Program.Q, (AIHeroClient) args.Target, 3);
                }
            }
        }
    }
}