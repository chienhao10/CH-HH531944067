#region

using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

namespace Marksman.Champions
{
    internal class Gnar : Champion
    {
        private static readonly AIHeroClient vGnar = ObjectManager.Player;
        public LeagueSharp.Common.Spell E;
        public LeagueSharp.Common.Spell Q;
        public LeagueSharp.Common.Spell W;

        public Gnar()
        {
            Utils.Utils.PrintMessage("Gnar loaded.");

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            Q.SetSkillshot(0.5f, 50f, 1200f, false, SkillshotType.SkillshotLine);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 600);

            E = new LeagueSharp.Common.Spell(SpellSlot.E, 500);
            E.SetSkillshot(0.5f, 50f, 1200f, false, SkillshotType.SkillshotCircle);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (!Orbwalker.CanMove)
                return;

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
            if (ComboActive || HarassActive)
            {
                if (Q.IsReady() && useQ)
                {
                    var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                    if (t != null)
                    {
                        Q.Cast(t, false, true);
                    }
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null && (ComboActive || HarassActive))
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;

                if (W.IsReady() && useW)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                }
                else if (Q.IsReady() && useQ)
                {
                    Q.Cast(t);
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            LeagueSharp.Common.Spell[] spellList = {Q, W, E};
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
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q", false));
            config.Add("UseWH", new CheckBox("Use W", false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawW", new CheckBox("W range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            return false;
        }
    }
}