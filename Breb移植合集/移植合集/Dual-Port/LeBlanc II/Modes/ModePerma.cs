using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leblanc.Common;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using EloBuddy;

namespace Leblanc.Modes
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
                if (Modes.ModeSettings.MenuSettingE["Settings.E.Auto"].Cast<ComboBox>().CurrentValue == 1)
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if (t.IsValidTarget() && t.CanStun())
                    {
                        //Champion.PlayerSpells.CastECombo(t);
                    }
                }
            }
        }
    }
}
