#region LICENSE

// Copyright 2014 - 2014 Support
// Annie.cs is part of Support.
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace AutoSharp.Plugins
{
    public class Ryze : PluginBase
    {
        public Ryze()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 630);
            Q.SetTargetted(0.2f, float.MaxValue);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 600);

            E = new LeagueSharp.Common.Spell(SpellSlot.E, 600);
            E.SetTargetted(0.2f, float.MaxValue);

            R = new LeagueSharp.Common.Spell(SpellSlot.R);
        }



        // some part from DevRyze

        public override void OnUpdate(EventArgs args)
        {
            
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }

                if (Player.LSDistance(Target) >= 575 && !Target.LSIsFacing(Player) && W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast(Target);
                }

                if (Target.LSIsValidTarget(W.Range) && W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast(Target);
                }

                if (Target.LSIsValidTarget(E.Range) && W.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }

                if (R.IsReady())
                {
                    R.Cast();
                }
        }

      


        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

    }
}