using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;

namespace jesuisFiora
{
    internal static class UltTarget
    {
        public static int CastTime;
        public static int EndTime;
        public static AIHeroClient Target;

        static UltTarget()
        {
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.IsMe || !args.Slot.Equals(SpellSlot.R))
            {
                return;
            }

            Target = args.Target as AIHeroClient;
            CastTime = Utils.TickCount;
            EndTime = CastTime + 8000;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Target != null && Target.IsValid && (Target.IsDead || !Target.HasUltPassive()))
            {
                Target = null;
            }
        }
    }
}