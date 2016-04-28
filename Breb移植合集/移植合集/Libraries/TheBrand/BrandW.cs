using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using PortAIO.Champion.Brand;
using TheBrand.Commons.ComboSystem;

namespace TheBrand
{
    internal class BrandW : Skill
    {
        private BrandE _brandE;
        private BrandQ _brandQ;
        public Color PredictedWColor = Color.Red;

        public BrandW(SpellSlot slot)
            : base(slot)
        {
            SetSkillshot(1.15f, 230f, int.MaxValue, false, SkillshotType.SkillshotCircle);
                // adjusted the range, for some reason the prediction was off, and missplaced it alot
            Range = 920;
        }

        public override void Initialize(ComboProvider combo)
        {
            _brandE = combo.GetSkill<BrandE>();
            _brandQ = combo.GetSkill<BrandQ>();
            Drawing.OnDraw += Draw;
            base.Initialize(combo);
        }

        private void Draw(EventArgs args)
        {
            if (!Program.getDrawMenuCB("WPred")) return;
            try
            {
                var target = Provider.Target;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || target == null) return;
                var prediction = GetPrediction(target, true);
                if (prediction.CastPosition.Distance(ObjectManager.Player.Position) < 900)
                    Render.Circle.DrawCircle(prediction.CastPosition, 240f, PredictedWColor);
            }
            catch
            {
            }
        }

        public override void Execute(AIHeroClient target)
        {
            if (!Provider.ShouldBeDead(target))
            {
                Cast(target, aoe: Program.getMiscMenuCB("aoeW"));
            }
        }

        public override float GetDamage(AIHeroClient enemy)
        {
            var baseDamage = base.GetDamage(enemy);
            return enemy.HasBuff("brandablaze") || _brandE.CanBeCast() && enemy.Distance(ObjectManager.Player) < 650
                ? baseDamage*1.25f
                : baseDamage;
        }

        public override void LaneClear(ComboProvider combo, AIHeroClient target)
        {
            var locationM =
                GetCircularFarmLocation(MinionManager.GetMinions(900 + 120, MinionTypes.All, MinionTeam.NotAlly));
            if (locationM.MinionsHit >= Program.getLaneMenuSL("MinWtargets"))
                Cast(locationM.Position);
        }

        public override int GetPriority()
        {
            return 2;
        }
    }
}