using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;

namespace FioraProject
{
    using static Program;
    using static GetTargets;
    using EloBuddy;
    class EvadeSkillShots
    {
        #region Evade
        public static void Evading()
        {
            var parry = Evade.EvadeSpellDatabase.Spells.FirstOrDefault(i => i.Enable && i.IsReady && i.Slot == SpellSlot.W);
            if (parry == null)
            {
                return;
            }
            var skillshot = Evade.Evade.SkillshotAboutToHit(ObjectManager.Player, 0 + Game.Ping + getSliderItem(Evade.Config.evadeMenu, "WDelay")).Where(i => parry.DangerLevel <= i.DangerLevel) .MaxOrDefault(i => i.DangerLevel);
            if (skillshot != null)
            {
                var target = GetTarget(W.Range);
                if (target.LSIsValidTarget(W.Range))
                    ObjectManager.Player.Spellbook.CastSpell(parry.Slot, target.Position);
                else
                {
                    var hero = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(W.Range));
                    if (hero != null)
                        ObjectManager.Player.Spellbook.CastSpell(parry.Slot, hero.Position);
                    else
                        ObjectManager.Player.Spellbook.CastSpell(parry.Slot, ObjectManager.Player.ServerPosition.LSExtend(skillshot.Start.To3D(), 100));
                }
            }
        }
        #endregion Evade

    }
}
