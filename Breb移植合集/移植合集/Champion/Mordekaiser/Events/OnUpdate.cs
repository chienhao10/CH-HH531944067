using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace Mordekaiser.Events
{
    internal class OnUpdate
    {
        private static readonly float wHitRange = 450f;

        public OnUpdate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static AIHeroClient Player
        {
            get { return Utils.Player.Self; }
        }

        private static Spell W
        {
            get { return Spells.W; }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            ExecuteLogicW();
        }

        private static void ExecuteLogicW()
        {
            if (!W.IsReady() || Player.Spellbook.GetSpell(SpellSlot.W).Name == "mordekaisercreepingdeath2")
                return;

            if (Player.CountEnemiesInRange(wHitRange) > 0)
            {
                if (Menu.getBoxItem(Menu.MenuW, "Selected" + Player.NetworkId) == 2)
                {
                    W.CastOnUnit(Utils.Player.Self);
                }
            }

            var ghost = Utils.HowToTrainYourDragon;
            if (ghost != null)
            {
                if (ghost.CountEnemiesInRange(wHitRange) == 0)
                    return;

                if (Menu.getBoxItem(Menu.MenuW, "SelectedGhost") == 2)
                {
                    W.CastOnUnit(ghost);
                }
            }

            foreach (var ally in HeroManager.Allies.Where(
                a => !a.IsDead && !a.IsMe && a.Position.LSDistance(Player.Position) < W.Range)
                .Where(ally => ally.CountEnemiesInRange(wHitRange) > 0)
                .Where(ally => Menu.getBoxItem(Menu.MenuW, "Selected" + ally.NetworkId) == 2)
                )
            {
                W.CastOnUnit(ally);
            }
        }
    }
}