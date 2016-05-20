using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Sebby_Ban_War
{
    class Program
    {

        public static Font Tahoma13;
        public static Menu Config;
        public static int LastMouseTime = Utils.TickCount;
        public static Vector2 LastMousePos = EloBuddy.Game.CursorPos.LSTo2D();
        public static int NewPathTime = Utils.TickCount;
        public static int LastType = 0; // 0 Move , 1 Attack, 2 Cast spell
        public static bool LastUserClickTime = false;
        public static int PathPerSecInfo;
        public static int PacketCast = Utils.TickCount;

        public static void Game_OnGameLoad()
        {
            Tahoma13 = new Font(EloBuddy.Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 14, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            Config = MainMenu.AddMenu("SBW - Sebby 人性化", "SBW - Sebby Ban War");
            Config.Add("enable", new CheckBox("开启",true));
            Config.Add("ClickTime", new Slider("最短点击时间 (100)",100,0,300));
            Config.Add("showCPS", new CheckBox("显示每秒进行的动作",true));
            Config.Add("blockOut", new CheckBox("屏蔽显示选择的目标动作 （没用过实在不会翻译）",true));
            Config.Add("skill", new CheckBox("屏蔽反人类技能施放动作", true));
            EloBuddy.Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            EloBuddy.Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            EloBuddy.Game.OnWndProc += Game_OnWndProc;
            EloBuddy.Game.OnUpdate += Game_OnUpdate;
            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            EloBuddy.Game.OnSendPacket += Game_OnSendPacket;
        }

        private static void Game_OnSendPacket(EloBuddy.GamePacketEventArgs args)
        {
            if(args.GetPacketId() == 270)
            {
                PathPerSecCounter++;
            }
        }

        public static int PathPerSecCounter = 0;
        public static int PathTimer = Utils.TickCount;

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Utils.TickCount - PathTimer > 1000)
            {
                PathPerSecInfo = PathPerSecCounter;
                PathTimer = Utils.TickCount;
                PathPerSecCounter = 0;
            }
        }

        private static void Obj_AI_Base_OnNewPath(EloBuddy.Obj_AI_Base sender, EloBuddy.GameObjectNewPathEventArgs args)
        {
            if (!sender.IsMe)
                return;

            PathPerSecCounter++;
        }

        private static void Game_OnWndProc(EloBuddy.WndEventArgs args)
        {
            if (args.Msg == 123)
            {
                LastUserClickTime = true;
            }
        }

        private static void Spellbook_OnCastSpell(EloBuddy.Spellbook sender, EloBuddy.SpellbookCastSpellEventArgs args)
        {

            if (!Config["enable"].Cast<CheckBox>().CurrentValue)
                return;
            
            var spellPosition = args.EndPosition;
            if (args.Target != null)
            {
                if (args.Target.IsMe)
                    return;

                if (Config["blockOut"].Cast<CheckBox>().CurrentValue && !Render.OnScreen(EloBuddy.Drawing.WorldToScreen(args.Target.Position)))
                {
                    //Console.WriteLine("BLOCK SPELL OUT SCREEN");
                    args.Process = false;
                    return;
                }
                spellPosition = args.Target.Position;
            }
            // IGNORE TARGETED SPELLS
            if (spellPosition.IsZero)
                return;

            if (args.Slot != EloBuddy.SpellSlot.Q && args.Slot != EloBuddy.SpellSlot.W && args.Slot != EloBuddy.SpellSlot.E && args.Slot != EloBuddy.SpellSlot.R)
                return;

            var spell = EloBuddy.ObjectManager.Player.Spellbook.Spells.FirstOrDefault(x => x.Slot == args.Slot);

            
            var screenPos = EloBuddy.Drawing.WorldToScreen(spellPosition);    
            if (Config["skill"].Cast<CheckBox>().CurrentValue && Utils.TickCount - LastMouseTime < LastMousePos.LSDistance(screenPos) / 20)
            {
                //Console.WriteLine("BLOCK SPELL");
                args.Process = false;
                return;
            }

            LastMouseTime = Utils.TickCount;
            LastMousePos = screenPos;
        }

        private static void Obj_AI_Base_OnIssueOrder(EloBuddy.Obj_AI_Base sender, EloBuddy.PlayerIssueOrderEventArgs args)
        {
            if (!Config["enable"].Cast<CheckBox>().CurrentValue)
                return;

            var screenPos = EloBuddy.Drawing.WorldToScreen(args.TargetPosition);
            var mouseDis = LastMousePos.LSDistance(screenPos);
            if (LastUserClickTime)
            {
                LastUserClickTime = false;
                return;
            }

            if (args.Order == EloBuddy.GameObjectOrder.AttackUnit && args.Target is EloBuddy.Obj_AI_Minion && LastType == 0 && Utils.TickCount - LastMouseTime > mouseDis / 15)
            {
                //Console.WriteLine("SBW farm protection");
                LastType = 1;
                LastMouseTime = Utils.TickCount;
                LastMousePos = screenPos;
                return;
            }
          
            //Console.WriteLine(args.Order);
            if (Utils.TickCount - LastMouseTime < Config["ClickTime"].Cast<Slider>().CurrentValue  + (mouseDis / 15))
            {
                //Console.WriteLine("BLOCK " + args.Order);
                args.Process = false;
                return;
            }

            //Console.WriteLine("DIS " + LastMousePos.Distance(screenPos) + " TIME " + (Utils.TickCount - LastMouseTime));
            if (args.Order == EloBuddy.GameObjectOrder.AttackUnit)
            {
                if (Config["blockOut"].Cast<CheckBox>().CurrentValue && !Render.OnScreen(screenPos))
                {
                    args.Process = false;
                    //Console.WriteLine("SBW BLOCK AA OUT SCREEN");
                }
                if (args.Target is EloBuddy.Obj_AI_Minion && LastType == 0)
                {
                    LastType = 1;
                    return;
                }
                LastType = 1;
            }
            else
                LastType = 0;

            LastMouseTime = Utils.TickCount;
            LastMousePos = screenPos;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config["showCPS"].Cast<CheckBox>().CurrentValue)
            {
                var h = EloBuddy.Drawing.Height * 0.2f;
                var w = EloBuddy.Drawing.Width * 0.15f;
                var color = Color.Yellow;
                if (PathPerSecInfo < 5)
                    color = Color.GreenYellow;
                else if (PathPerSecInfo > 8)
                    color = Color.OrangeRed;

                DrawFontTextScreen(Tahoma13, "SBW 服务器动作/秒: " + PathPerSecInfo, h, w, color);
            }
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = EloBuddy.Drawing.WorldToScreen(pos1);
            var wts2 = EloBuddy.Drawing.WorldToScreen(pos2);

            EloBuddy.Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }
    }
}
