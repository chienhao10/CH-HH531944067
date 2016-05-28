using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Kalista : PluginBase
    {
        public override void OnUpdate(EventArgs args)
        {
            if (!Orbwalker.CanMove)
                return;

            if (Q.IsReady())
            {
                var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (Q.CanCast(Qtarget) && !Player.Spellbook.IsAutoAttacking && !Player.LSIsDashing())
                    Q.Cast(Qtarget);
            }

            if (E.IsReady())
            {
                var eTarget =
                    HeroManager.Enemies.Where(
                        x =>
                            x.LSIsValidTarget(E.Range) && E.GetDamage(x) >= 1 &&
                            !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield))
                        .OrderByDescending(x => E.GetDamage(x))
                        .FirstOrDefault();

                if (eTarget != null && eTarget.Health <= E.GetDamage(eTarget))
                    E.Cast();
            }
        }
    }
}
