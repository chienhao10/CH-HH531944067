using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules;
using VayneHunter_Reborn.Modules.ModuleList.Condemn;
using VayneHunter_Reborn.Modules.ModuleList.Misc;
using VayneHunter_Reborn.Modules.ModuleList.Tumble;
using VayneHunter_Reborn.Utility.Helpers;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace VayneHunter_Reborn.Utility
{
    class Variables
    {
        private const float Range = 1200f;

        public static Menu Menu { get; set; }

        public static float LastCondemnFlashTime { get; set; }

        public static Dictionary<SpellSlot, LeagueSharp.Common.Spell> spells = new Dictionary<SpellSlot, LeagueSharp.Common.Spell>
        {
            { SpellSlot.Q, new LeagueSharp.Common.Spell(SpellSlot.Q) },
            { SpellSlot.W, new LeagueSharp.Common.Spell(SpellSlot.W) },
            { SpellSlot.E, new LeagueSharp.Common.Spell(SpellSlot.E, 650f) {Width = 1f} },
            { SpellSlot.R, new LeagueSharp.Common.Spell(SpellSlot.R) }
        };

        public static List<IModule> moduleList = new List<IModule>()
        {
            new AutoE(),
            new EKS(),
            new LowLifePeel(),
            new NoAAStealth(),
            new QKS(),
            new WallTumble(),
            new Focus2WStacks(),
            new Reveal(),
            new DisableMovement(),
            new CondemnJungleMobs(),
            new FlashRepel(),
            new FlashCondemn()
        };

        public static IEnumerable<AIHeroClient> MeleeEnemiesTowardsMe
        {
            get
            {
                return
                    HeroManager.Enemies.FindAll(
                        m => m.IsMelee() && m.LSDistance(ObjectManager.Player) <= PlayerHelper.GetRealAutoAttackRange(m, ObjectManager.Player)
                            && (m.ServerPosition.To2D() + (m.BoundingRadius + 25f) * m.Direction.To2D().Perpendicular()).LSDistance(ObjectManager.Player.ServerPosition.To2D()) <= m.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition)
                            && m.IsValidTarget(Range, false));
            }
        }

        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                            m.LSDistance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.IsValidTarget(1500, false) &&
                            m.CountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }

    }
}
