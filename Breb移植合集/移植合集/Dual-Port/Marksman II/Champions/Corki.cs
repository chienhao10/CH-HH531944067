#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

namespace Marksman.Champions
{
    internal class Corki : Champion
    {
        public LeagueSharp.Common.Spell Q, W, E;
        public LeagueSharp.Common.Spell R1, R2;

        public Corki()
        {
            Utils.Utils.PrintMessage("Corki loaded");

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 825f, DamageType.Magical) { MinHitChance = HitChance.High };
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 600f, DamageType.Magical);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700f);
            R1 = new LeagueSharp.Common.Spell(SpellSlot.R, 1300f, DamageType.Magical) { MinHitChance = HitChance.High };
            R2 = new LeagueSharp.Common.Spell(SpellSlot.R, 1500f, DamageType.Magical) { MinHitChance = HitChance.VeryHigh };

            Q.SetSkillshot(0.35f, 240f, 1300f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.35f, 140f, 1500f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0f, 45 * (float)Math.PI / 180, 1500, false, SkillshotType.SkillshotCone);

            R1.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            R2.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            bool drawKillableMinions = Program.laneclear["Lane.UseQ.DrawKM"].Cast<CheckBox>().CurrentValue;
            if (drawKillableMinions && Q.IsReady() && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                foreach (Obj_AI_Base m in
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range)
                        .Where(x => E.CanCast(x) && x.Health < Q.GetDamage(x)))
                {
                    Render.Circle.DrawCircle(m.Position, (float)(m.BoundingRadius * 2), Color.Wheat, 5);
                }
            }

            LeagueSharp.Common.Spell[] spellList = { Q, E };
            foreach (LeagueSharp.Common.Spell spell in spellList)
            {
                bool menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem && spell.Slot == SpellSlot.Q)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, System.Drawing.Color.Aqua);
                if (menuItem && spell.Equals(spell.Slot == SpellSlot.E))
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, System.Drawing.Color.Wheat);
            }

            var drawR = Program.marksmanDrawings["DrawR1"].Cast<CheckBox>().CurrentValue;
            if (drawR)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R1.Range, Color.DarkOrange);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (R1.IsReady() && Program.misc["UseRM"].Cast<CheckBox>().CurrentValue)
            {
                bool bigRocket = HasBigRocket();
                foreach (
                    AIHeroClient hero in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                hero =>
                                    hero.LSIsValidTarget(bigRocket ? R2.Range : R1.Range) &&
                                    R1.GetDamage(hero) * (bigRocket ? 1.5f : 1f) > hero.Health))
                {
                    if (bigRocket)
                    {
                        R2.Cast(hero, false, true);
                    }
                    else
                    {
                        R1.Cast(hero, false, true);
                    }
                }
            }

            if ((!ComboActive && !HarassActive) || !Orbwalker.CanMove) return;

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
            var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;
            var useR = ComboActive ? Program.combo["UseRC"].Cast<CheckBox>().CurrentValue : Program.harass["UseRH"].Cast<CheckBox>().CurrentValue;
            var rLim = ComboActive ? Program.combo["RlimC"].Cast<Slider>().CurrentValue : Program.harass["RlimH"].Cast<Slider>().CurrentValue;

            if (useQ && Q.IsReady())
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (t != null)
                    if (Q.Cast(t, false, true) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        return;
            }

            if (useE && E.IsReady())
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.IsValidTarget())
                    if (E.Cast(t, false, true) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        return;
            }

            if (useR && R1.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo > rLim)
            {
                bool bigRocket = HasBigRocket();
                AIHeroClient t = TargetSelector.GetTarget(bigRocket ? R2.Range : R1.Range, DamageType.Magical);

                if (t.LSIsValidTarget())
                {
                    if (bigRocket)
                    {
                        R2.Cast(t, false, true);
                    }
                    else
                    {
                        R1.Cast(t, false, true);
                    }
                }
            }
        }

        public override void ExecuteLaneClear()
        {
            int laneQValue = Program.laneclear["Lane.UseQ"].Cast<ComboBox>().CurrentValue;
            if (laneQValue != 0 && Q.IsReady())
            {
                Vector2 minions = Q.GetCircularFarmMinions(laneQValue);
                if (minions != Vector2.Zero)
                {
                    Q.Cast(minions);
                }
            }

            int laneEValue = Program.laneclear["Lane.UseE"].Cast<ComboBox>().CurrentValue;
            if (laneEValue != 0 && E.IsReady())
            {
                int minCount = E.GetMinionCountsInRange();
                if (minCount >= laneEValue)
                {
                    E.Cast();
                }
            }

            int laneRValue = Program.laneclear["Lane.UseR"].Cast<ComboBox>().CurrentValue;
            if (laneRValue != 0 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= Program.laneclear["Lane.UseR.Lim"].Cast<Slider>().CurrentValue)
            {
                int rocketType = Program.laneclear["Lane.UseR.Bomb"].Cast<ComboBox>().CurrentValue;
                if (R1.IsReady() && (rocketType == 0 || rocketType == 2) && !HasBigRocket())
                {
                    Vector2 minions = R1.GetCircularFarmMinions(laneRValue);
                    if (minions != Vector2.Zero)
                    {
                        R1.Cast(minions);
                    }
                }
                if (R2.IsReady() && (rocketType == 1 || rocketType == 2) && HasBigRocket())
                {
                    Vector2 minions = R2.GetCircularFarmMinions(laneRValue);
                    if (minions != Vector2.Zero)
                    {
                        R2.Cast(minions);
                    }
                }
            }
        }

        public override void ExecuteJungleClear()
        {
            int jungleQValue = Program.jungleClear["Jungle.UseQ"].Cast<ComboBox>().CurrentValue;
            if (jungleQValue != 0 && W.IsReady())
            {
                Obj_AI_Base jungleMobs = Utils.Utils.GetMobs(Q.Range,
                    jungleQValue != 1 ? Utils.Utils.MobTypes.All : Utils.Utils.MobTypes.BigBoys,
                    jungleQValue != 1 ? jungleQValue : 1);
                if (jungleMobs != null)
                {
                    Q.Cast(jungleMobs);
                }
            }

            int jungleEValue = Program.jungleClear["Jungle.UseE"].Cast<ComboBox>().CurrentValue;
            if (W.IsReady() && jungleEValue != 0)
            {
                Obj_AI_Base jungleMobs = Utils.Utils.GetMobs(E.Range,
                    jungleEValue != 1 ? Utils.Utils.MobTypes.All : Utils.Utils.MobTypes.BigBoys,
                    jungleEValue != 1 ? jungleEValue : 1);

                if (jungleMobs != null)
                {
                    E.Cast();
                }
            }

            int jungleRValue = Program.jungleClear["Jungle.UseR"].Cast<ComboBox>().CurrentValue;
            if (jungleRValue != 0 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo > Program.jungleClear["Jungle.UseR.Lim"].Cast<Slider>().CurrentValue)
            {
                Obj_AI_Base jungleMobs = Utils.Utils.GetMobs(R1.Range, jungleRValue != 1 ? Utils.Utils.MobTypes.All : Utils.Utils.MobTypes.BigBoys, jungleRValue != 1 ? jungleRValue : 1);
                if (jungleMobs != null)
                {
                    var rocketType = Program.jungleClear["Jungle.UseR.Bomb"].Cast<ComboBox>().CurrentValue;
                    if (R1.IsReady() && (rocketType == 0 || rocketType == 1) && !HasBigRocket())
                    {
                        R1.Cast(jungleMobs);
                    }

                    if (R2.IsReady() && (rocketType == 1 || rocketType == 2) && HasBigRocket())
                    {
                        R2.Cast(jungleMobs);
                    }
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive))
                return;

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
            var useE = ComboActive ? Program.combo["UseEC"].Cast<CheckBox>().CurrentValue : Program.harass["UseEH"].Cast<CheckBox>().CurrentValue;
            var useR = ComboActive ? Program.combo["UseRC"].Cast<CheckBox>().CurrentValue : Program.harass["UseRH"].Cast<CheckBox>().CurrentValue;
            var rLim = ComboActive ? Program.combo["RlimC"].Cast<Slider>().CurrentValue : Program.harass["RlimH"].Cast<Slider>().CurrentValue;

            if (useQ && Q.IsReady())
                if (Q.Cast(t, false, true) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                    return;

            if (useE && E.IsReady())
                if (E.Cast(t, false, true) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                    return;

            if (useR && R1.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo > rLim)
            {
                if (HasBigRocket())
                {
                    R2.Cast(t, false, true);
                }
                else
                {
                    R1.Cast(t, false, true);
                }
            }
        }

        public bool HasBigRocket()
        {
            return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName.ToLower() == "corkimissilebarragecounterbig");
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseEC", new CheckBox("Use E"));
            config.Add("UseRC", new CheckBox("Use R"));
            config.Add("RlimC", new Slider("Keep R Stacks", 0, 0, 7));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            config.Add("UseEH", new CheckBox("Use E", false));
            config.Add("UseRH", new CheckBox("Use R"));
            config.Add("RlimH", new Slider("Keep R Stacks", 3, 0, 7));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, System.Drawing.Color.Aqua, 1)));
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(false, System.Drawing.Color.Wheat, 1)));
            config.Add("DrawR1", new CheckBox("R range"));//.SetValue(new Circle(false, System.Drawing.Color.DarkOrange, 1)));
            config.Add("Draw.Packet", new ComboBox("Show Turbo-Packet Remaining Time", 2, "Off", "Everytime", "Show 20 secs Left"));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("ShowPosition", new KeyBind("Show Position", false, KeyBind.BindTypes.HoldActive, 'H'));
            config.Add("UseRM", new CheckBox("Use R To Killsteal"));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            string[] strQ = new string[4];
            {
                strQ[0] = "Off";
                for (var i = 1; i < 4; i++)
                {
                    strQ[i] = "Mobs Count >= " + i;
                }
                config.Add("Lane.UseQ", new ComboBox("Q: Use", 2, strQ));
            }

            config.Add("Lane.UseQ.Prepare", new ComboBox("Q: Prepare Minions for multi farm", 2, "Off", "On", "Just Under Ally Turret"));
            config.Add("Lane.UseQ.DrawKM", new CheckBox("Q: Draw Killable Minions"));//.SetValue(new Circle(true, Color.Wheat, 85f))).SetFontStyle(FontStyle.Regular, Q.MenuColor());


            string[] strW = new string[6];
            {
                strW[0] = "Off";
                for (var i = 1; i < 6; i++)
                {
                    strW[i] = "Mobs Count >= " + i;
                }
            }

            string[] strE = new string[7];
            {
                strE[0] = "Off";
                for (var i = 1; i < 7; i++)
                {
                    strE[i] = "Mobs Count >= " + i;
                }

                config.Add("Lane.UseE", new ComboBox("E: Use", 2, strE));
            }

            string[] strR = new string[4];
            {
                strR[0] = "Off";
                for (var i = 1; i < 4; i++)
                {
                    strR[i] = "Minion Count >= " + i;
                }

                config.Add("Lane.UseR", new ComboBox("R:", 3, strR));
                config.Add("Lane.UseR.Lim", new Slider("R: Keep Stacks", 0, 0, 7));
                config.Add("Lane.UseR.Bomb", new ComboBox("R: Rocket Type", 0, "Small-Rocked", "Big-Rocked", "Both"));
            }
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            string[] strQ = new string[4];
            {
                strQ[0] = "Off";
                strQ[1] = "Just for big Monsters";

                for (var i = 2; i < 4; i++)
                {
                    strQ[i] = "Mobs Count >= " + i;
                }

                config.Add("Jungle.UseQ", new ComboBox("Q: Use", 1, strQ));
            }

            string[] strE = new string[4];
            {
                strE[0] = "Off";
                strE[1] = "Just for big Monsters";

                for (var i = 2; i < 4; i++)
                {
                    strE[i] = "Mobs Count >= " + i;
                }

                config.Add("Jungle.UseE", new ComboBox("E: Use", 1, strE));
            }

            string[] strR = new string[4];
            {
                strR[0] = "Off";
                strR[1] = "Just big Monsters";
                for (var i = 2; i < 4; i++)
                {
                    strR[i] = "Mob Count >= " + i;
                }

                config.Add("Jungle.UseR", new ComboBox("R:", 3, strR));
                config.Add("Jungle.UseR.Lim", new Slider("R: Keep Stacks", 0, 0, 7));
                config.Add("Jungle.UseR.Bomb", new ComboBox("R: Rocked Type", 0, "Small-Rocked", "Big-Rocked", "Both"));
            }

            return true;
        }
    }
}
