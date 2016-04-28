using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using TheBrand.Commons.ComboSystem;

namespace TheBrand
{
    internal class BrandQ : Skill
    {
        // ReSharper disable once InconsistentNaming
        private Skill[] _brandQWE;
        private BrandW _w;

        public BrandQ(SpellSlot slot)
            : base(slot)
        {
            SetSkillshot(0.625f, 50f, 1600f, true, SkillshotType.SkillshotLine);
            Range = 1045;
        }

        public override void Initialize(ComboProvider combo)
        {
            var skills = combo.GetSkills().ToList();
            skills.Remove(skills.First(skill => skill is BrandQ));
            _brandQWE = skills.ToArray();
            _w = combo.GetSkill<BrandW>();
            base.Initialize(combo);
        }


        public override void Execute(AIHeroClient target)
        {
            if (
                _brandQWE.Any(
                    spell =>
                        spell.Instance.State == SpellState.Ready ||
                        spell.Instance.CooldownExpires > Game.Time &&
                        spell.Instance.CooldownExpires - Game.Time < spell.Instance.Cooldown/2f) ||
                _w.Instance.Cooldown - (_w.Instance.CooldownExpires - Game.Time) < 1)
            {
                if ((!target.HasBuff("brandablaze") || target.GetBuff("brandablaze").EndTime - Game.Time < Delay) &&
                    !Provider.IsMarked(target) &&
                    !(ObjectManager.Player.GetSpellDamage(target, Instance.Slot) +
                      ObjectManager.Player.GetAutoAttackDamage(target, true) > target.Health))
                    return;
            }

            Cast(target);
        }

        public override int GetPriority()
        {
            return 3;
        }
    }
}