using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using TreeLib.SpellData;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;
using TreeLib.Objects;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace LuluLicious
{
    internal static class Utility
    {
        public static AIHeroClient GetBestWETarget()
        {
            var ally = HeroManager.Allies.Where(h => h.IsValidTarget(SpellManager.W.Range, false)).OrderByDescending(h => Lulu.getSliderItem(Lulu.superMMenu, h.NetworkId + "WEPriority")).FirstOrDefault();
            return ally == null || Lulu.getSliderItem(Lulu.superMMenu, ally.NetworkId + "WEPriority") == 0 ? null : ally;
        }

        public static AIHeroClient GetBestWTarget()
        {
            var enemy = HeroManager.Enemies.Where(h => h.IsValidTarget(SpellManager.W.Range)).MaxOrDefault(o => Lulu.getSliderItem(Lulu.wMenu, o.NetworkId + "WPriority"));
            return enemy == null || Lulu.getSliderItem(Lulu.wMenu, enemy.NetworkId + "WPriority") == 0 ? null : enemy;
        }

        public static float GetPredictedHealthPercent(this AIHeroClient hero)
        {
            var dmg = 0d;
            foreach (var skillshot in TreeLib.SpellData.Evade.GetSkillshotsAboutToHit(hero, 400))
            {
                try
                {
                    dmg += skillshot.Unit.GetDamageSpell(hero, skillshot.SpellData.SpellName).CalculatedDamage;
                }
                catch {}
            }

            return (float) ((hero.Health - dmg) / hero.MaxHealth * 100);
        }

        public static float GetComboDamage(this Obj_AI_Base unit)
        {
            var d = 0d;

            if (SpellManager.Q.IsReady())
            {
                d += SpellManager.Q.GetDamage(unit);
            }

            if (SpellManager.E.IsReady())
            {
                d += SpellManager.Q.GetDamage(unit);
            }

            d += (float) ObjectManager.Player.GetAutoAttackDamage(unit, true);

            var dl = ObjectManager.Player.GetMastery(MasteryData.Ferocity.DoubleEdgedSword);
            if (dl != null && dl.IsActive())
            {
                d *= 1.03f;
            }

            var assassin = ObjectManager.Player.GetMastery((MasteryData.Cunning) 83);
            if (assassin != null && assassin.IsActive() && ObjectManager.Player.CountAlliesInRange(800) == 0)
            {
                d *= 1.02f;
            }

            var ignite = TreeLib.Managers.SpellManager.Ignite;
            if (ignite != null && ignite.IsReady())
            {
                d += (float) ObjectManager.Player.GetSummonerSpellDamage(unit, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            }

            var tl = ObjectManager.Player.GetMastery(MasteryData.Cunning.ThunderlordsDecree);
            if (tl != null && tl.IsActive() && !ObjectManager.Player.HasBuff("masterylordsdecreecooldown"))
            {
                d += 10 * ObjectManager.Player.Level + .3f * ObjectManager.Player.FlatPhysicalDamageMod +
                     .1f * ObjectManager.Player.TotalMagicalDamage;
            }

            if (ItemData.Ludens_Echo.GetItem() != null)
            {
                var b = ObjectManager.Player.GetBuff("itemmagicshankcharge");
                if (b != null && b.IsActive && b.Count >= 70)
                {
                    d += 100 + ObjectManager.Player.TotalMagicalDamage * .1f;
                }
            }

            return (float) d;
        }
    }
}