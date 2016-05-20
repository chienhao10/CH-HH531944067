using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK;

namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyJungleClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
        }

        private static void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                return;
            if (target.Team != GameObjectTeam.Neutral)
                return;
            if (target.Position.LSDistance(ObjectManager.Player.Position) <= 200 + 125 + 140)
                BadaoChecker.BadaoUseTiamat();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                return;
            if (!BadaoPoppyHelper.ManaJungle())
                return;
            if (BadaoPoppyHelper.UseEJungle() && Environment.TickCount - BadaoPoppyVariables.QCastTick >= 1250)
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.E.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion.BadaoIsValidTarget() && BadaoMath.GetFirstWallPoint(minion.Position.LSTo2D(),
                    minion.Position.LSTo2D().LSExtend(ObjectManager.Player.Position.LSTo2D(), -300 - minion.BoundingRadius)) != null)
                {
                    BadaoMainVariables.E.Cast(minion);
                }
            }
            if (BadaoPoppyHelper.UseQJungle())
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.Q.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    if(BadaoMainVariables.Q.Cast(minion) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        BadaoPoppyVariables.QCastTick = Environment.TickCount;
                }
            }
        }
    }
}
