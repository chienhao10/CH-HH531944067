#region LICENSE

// Copyright 2014 - 2015 LeagueSharp
// Default.cs is part of AutoSharp.
//
// AutoSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// AutoSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with AutoSharp. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace AutoSharp.Plugins
{
	public class Default : PluginBase
	{
		public Default()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 600);
			W = new LeagueSharp.Common.Spell(SpellSlot.W, 450);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, 200);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 600);

			var q = SpellData.GetSpellData(ObjectManager.Player.GetSpell(SpellSlot.Q).Name);
			var w = SpellData.GetSpellData(ObjectManager.Player.GetSpell(SpellSlot.W).Name);
			var e = SpellData.GetSpellData(ObjectManager.Player.GetSpell(SpellSlot.E).Name);
			var r = SpellData.GetSpellData(ObjectManager.Player.GetSpell(SpellSlot.R).Name);

			Q.SetSkillshot(q.SpellCastTime, q.LineWidth, q.MissileSpeed, true, SkillshotType.SkillshotLine);
			W.SetSkillshot(w.SpellCastTime, w.LineWidth, w.MissileSpeed, true, SkillshotType.SkillshotLine);
			E.SetTargetted(e.SpellCastTime, e.SpellCastTime);
			R.SetTargetted(r.SpellCastTime, r.SpellCastTime);
		}

		public override void OnUpdate(EventArgs args)
		{
			if (ComboMode)
			{
				var targetgrag = TargetSelector.GetTarget(900, DamageType.Magical);
				if (targetgrag == null) return;
				if (Q.CastCheck(targetgrag, "ComboQ"))
				{
					Q.Cast(targetgrag);
				}

				if (W.CastCheck(targetgrag, "ComboW"))
				{
					W.Cast(targetgrag);
				}

				if (E.CastCheck(targetgrag, "ComboE"))
				{
					E.Cast(targetgrag);

                if (R.CastCheck(targetgrag, "ComboR"))
					{
						R.Cast(targetgrag);
					}
				}
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