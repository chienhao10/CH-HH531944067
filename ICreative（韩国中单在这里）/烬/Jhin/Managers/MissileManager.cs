using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin;

namespace Jhin.Managers
{
    public static class MissileManager
    {
        private const float PercentOffset = 1.25f;
        public static List<MissileClient> Missiles = new List<MissileClient>();

        public static bool MissileWillHitMyHero
        {
            get
            {
                foreach (var m in Missiles.Where(m => m.IsValidMissile()))
                {
                    var canCast = false;
                    if (m.Target != null)
                    {
                        canCast = m.Target.IsMe;
                    }
                    if (!m.EndPosition.IsZero && m.SData.LineWidth > 0)
                    {
                        var width = ((m.SData.TargettingType == SpellDataTargetType.LocationAoe
                            ? m.SData.CastRadius
                            : m.SData.LineWidth) + AIO.MyHero.BoundingRadius) * PercentOffset;
                        var extendedEndPos = m.EndPosition + (m.EndPosition - m.StartPosition).Normalized() * width;
                        canCast =
                            AIO.MyHero.ServerPosition.To2D()
                                .Distance(m.StartPosition.To2D(), extendedEndPos.To2D(), true, true) <= width * width;
                    }
                    if (canCast)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static void Initialize()
        {
            /*
            Missiles =
                ObjectManager.Get<MissileClient>()
                    .Where(
                        m =>
                            m.IsValidMissile() && m.SpellCaster != null && m.SpellCaster.Team != AIO.MyHero.Team &&
                            !m.SpellCaster.IsMinion)
                    .ToList();*/
            /*
    GameObject.OnCreate += delegate(GameObject sender, EventArgs args)
    {
        var missile = sender as MissileClient;
        if (missile != null)
        {
            var spellCaster = missile.SpellCaster;
            if (spellCaster.Team != AIO.MyHero.Team && !spellCaster.IsMinion)
            {
                Missiles.Add(missile);
            }
        }
    };
    GameObject.OnDelete += delegate(GameObject sender, EventArgs args)
    {
        var missile = sender as MissileClient;
        if (missile != null)
        {
            Missiles.RemoveAll(m => m.IdEquals(missile));
        }
    };*/
        }
    }
}