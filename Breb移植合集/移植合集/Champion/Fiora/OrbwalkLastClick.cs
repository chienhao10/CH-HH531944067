using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace FioraProject
{
    using static Program;
    using static Combos;
    using static GetTargets;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
    using EloBuddy.SDK;
    public static class OrbwalkLastClick
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Vector2 LastClickPoint = new Vector2();
        public static void Init()
        {
            Game.OnUpdate +=Game_OnUpdate;
            Game.OnWndProc +=Game_OnWndProc;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!OrbwalkLastClickActive)
                return;
            if (!sender.IsMe)
                return;
            if (args.Order != GameObjectOrder.MoveTo)
                return;
            if (!Orbwalker.CanMove || Player.IsCastingInterruptableSpell())
                args.Process = false;
        }

        public static void OrbwalkLRCLK_ValueChanged(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (args.NewValue)
            {
                LastClickPoint = Game.CursorPos.LSTo2D();
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!OrbwalkLastClickActive)
                return;
            Combo();
            var target = GetTarget();
            Orbwalker.ForcedTarget = Orbwalking.InAutoAttackRange(target) ? target : null;
            Orbwalker.MoveTo(LastClickPoint.IsValid() ? LastClickPoint.To3D() : Game.CursorPos);
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_RBUTTONDOWN)
            {
                LastClickPoint = Game.CursorPos.To2D();
            }
        }
    }
}
