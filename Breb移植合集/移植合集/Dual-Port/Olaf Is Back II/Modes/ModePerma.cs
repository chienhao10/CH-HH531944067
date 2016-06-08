using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace OlafxQx.Modes
{
    internal class ModePerma
    {
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;
        public static void Init()
        {
            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !ObjectManager.Player.LSIsRecalling())
            {
                if (Modes.ModeSettings.MenuLocal["Settings.E.Auto"].Cast<ComboBox>().CurrentValue == 1)
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if (t.IsValidTarget())
                    {
                        Champion.PlayerSpells.CastE(t);
                    }
                }
            }
        }
    }
}
