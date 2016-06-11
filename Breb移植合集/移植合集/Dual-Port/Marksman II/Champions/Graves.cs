#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

namespace Marksman.Champions
{
    internal class Graves : Champion
    {
        public LeagueSharp.Common.Spell Q;
        public LeagueSharp.Common.Spell W;
        public LeagueSharp.Common.Spell R;

        public Graves()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 920f); // Q likes to shoot a bit too far away, so moving the range inward.
            Q.SetSkillshot(0.26f, 10f*2*(float) Math.PI/180, 1950, false, SkillshotType.SkillshotCone);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1100f);
            W.SetSkillshot(0.30f, 250f, 1650f, false, SkillshotType.SkillshotCircle);

            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1100f);
            R.SetSkillshot(0.22f, 150f, 2100, true, SkillshotType.SkillshotLine);

            Drawing.OnDraw += DrawingOnOnDraw;

            Utils.Utils.PrintMessage("Graves loaded.");
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            var t = TargetSelector.GetTarget(Q.Range * 5, DamageType.Magical);
            if (!t.LSIsValidTarget())
            {
                return;
            }

            if (Q.IsReady())
            {

                var toPolygon = new Marksman.Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.LSTo2D(), ObjectManager.Player.Position.LSTo2D().LSExtend(t.Position.LSTo2D(), Q.Range - 200), 50).ToPolygon();
                toPolygon.Draw(System.Drawing.Color.Red, 2);
                
                if (toPolygon.IsInside(t))
                {
                    Render.Circle.DrawCircle(t.Position, t.BoundingRadius, Color.Black);
                    Q.Cast(t);
                }

                var xPos = ObjectManager.Player.Position.LSTo2D().LSExtend(t.Position.LSTo2D(), Q.Range);

                var toPolygon2 = new Marksman.Common.CommonGeometry.Rectangle(xPos, ObjectManager.Player.Position.LSTo2D().LSExtend(t.Position.LSTo2D(), Q.Range - 195), 260).ToPolygon();
                toPolygon2.Draw(System.Drawing.Color.Red, 2);

                if (toPolygon2.IsInside(t))
                {
                    Render.Circle.DrawCircle(t.Position, t.BoundingRadius, Color.Black);
                    Q.Cast(t);
                }
            }

        }

        private float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (Q.IsReady())
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q);

            if (W.IsReady())
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.W);

            if (R.IsReady())
                fComboDamage += (float) ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot")) ==
                SpellState.Ready && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Botrk);

            return fComboDamage;
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Q.IsReady() && Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                    return;
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t != null)
                    Q.Cast(t, false, true);
            }

            if (Program.combo["CastR"].Cast<KeyBind>().CurrentValue && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget())
                {
                    if (ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R) > t.Health && !t.IsZombie)
                    {
                        R.CastIfHitchanceEquals(t, HitChance.High, false);
                    }
                }
            }


            if ((!ComboActive && !HarassActive) || !Orbwalker.CanMove) return;

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
            var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;
            var useR = ComboActive ? Program.combo["UseRC"].Cast<CheckBox>().CurrentValue : false;

            if (Q.IsReady() && useQ)
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t != null)
                    Q.Cast(t, false, true);
            }

            if (W.IsReady() && useW)
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (t.LSIsValidTarget(W.Range) &&
                    (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                     t.HasBuffOfType(BuffType.Taunt) || t.HasBuff("zhonyasringshield") ||
                     t.HasBuff("Recall")))
                    W.Cast(t, false, true);
            }

            if (R.IsReady() && useR)
            {
                foreach (
                    var hero in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                hero =>
                                    hero.LSIsValidTarget(R.Range) &&
                                    ObjectManager.Player.LSGetSpellDamage(hero, SpellSlot.R, 1) - 20 > hero.Health))
                    R.Cast(hero, false, true);
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null && (ComboActive || HarassActive))
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;

                if (Q.IsReady() && useQ)
                    Q.Cast(t);

                if (W.IsReady() && useW)
                {
                    if (t.LSIsValidTarget(W.Range) &&
                        (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                         t.HasBuffOfType(BuffType.Taunt) || t.HasBuff("zhonyasringshield") ||
                         t.HasBuff("Recall")))
                        W.Cast(t);
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            LeagueSharp.Common.Spell[] spellList = {Q};
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 0, 255));
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W"));
            config.Add("UseRC", new CheckBox("Use R"));
            config.Add("CastR", new KeyBind("Cast R (Manual)", false, KeyBind.BindTypes.HoldActive, 'T'));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            config.Add("UseWH", new CheckBox("Use W", false));
            config.Add("UseQTH", new KeyBind("Use Q (Toggle)", false, KeyBind.BindTypes.HoldActive, 'H'));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {
            return true;
        }
    }
}
