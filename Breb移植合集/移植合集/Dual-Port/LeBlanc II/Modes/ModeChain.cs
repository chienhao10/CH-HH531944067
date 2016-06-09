using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Champion;
using Leblanc.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace Leblanc.Modes
{
    internal static class ModeChain
    {

        public static Menu MenuLocal { get; private set; }
        public static Menu MenuHunt { get; private set; }
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell E2 => Champion.PlayerSpells.E2;

        public static void Init()
        {
            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (ModeConfig.MenuKeys["Key.DoubleChain"].Cast<KeyBind>().CurrentValue)
            {
                ExecuteDoubleChain();
            }
        }

        private static void ExecuteDoubleChain()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                Drawing.DrawText(Drawing.Width * 0.45f, Drawing.Height * 0.78f, System.Drawing.Color.Red, "Double Stun Active!");

                foreach (var e in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            e =>
                                e.IsEnemy && !e.IsDead && e.IsVisible &&
                                ObjectManager.Player.LSDistance(e) < Modes.ModeSettings.MaxERange && !e.HasSoulShackle()))
                {
                    PlayerSpells.CastE(e);
                    PlayerSpells.CastE2(e);
                }
        }
    }
}
