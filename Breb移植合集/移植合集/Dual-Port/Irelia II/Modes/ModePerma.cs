using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irelia.Common;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK;
using EloBuddy;

namespace Irelia.Modes
{
    internal class ModePerma
    {
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;
        public static void Init()
        {
            //Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                //if (Modes.ModeSettings.MenuSettingE.Item("Settings.E.Auto").GetValue<StringList>().SelectedIndex == 1)
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if (t.IsValidTarget() && t.CanStun())
                    {
                        Champion.PlayerSpells.CastECombo(t);
                    }
                }
            }
        }
    }
}
