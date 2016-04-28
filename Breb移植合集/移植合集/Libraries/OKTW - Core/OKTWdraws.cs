using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Properties;
using SebbyLib;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Core
{
    internal class OKTWdraws
    {
        private static Menu Sub;
        public static Font Tahoma13, Tahoma13B, TextBold;
        private readonly Menu Config = Program.Config;
        public Spell Q, W, E, R;
        private float IntroTimer = Game.Time;

        private AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static bool getCheckBoxItem(string item)
        {
            return Sub[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Sub[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Sub[item].Cast<KeyBind>().CurrentValue;
        }

        public void LoadOKTW()
        {
            Sub = Config.AddSubMenu("Utility, Draws OKTW©");
            Sub.Add("disableDraws", new CheckBox("DISABLE UTILITY DRAWS", false));

            Sub.AddGroupLabel("Enemy info grid");
            Sub.Add("championInfo", new CheckBox("Game Info"));
            Sub.Add("ShowKDA", new CheckBox("Show flash and R CD"));
            Sub.Add("posX", new Slider("posX", 20));
            Sub.Add("posY", new Slider("posY", 10));
            Sub.AddSeparator();

            Sub.Add("GankAlert", new CheckBox("Gank Alert"));
            Sub.Add("HpBar", new CheckBox("Dmg indicators BAR OKTW© style"));
            Sub.Add("ShowClicks", new CheckBox("Show enemy clicks"));
            Sub.Add("SS", new CheckBox("SS notification"));
            Sub.Add("showWards", new CheckBox("Show hidden objects, wards"));
            Sub.Add("minimap", new CheckBox("Mini-map hack"));

            Tahoma13B = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 14,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType
                });

            Tahoma13 = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 14,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType
                });

            TextBold = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Impact",
                    Height = 30,
                    Weight = FontWeight.Normal,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType
                });

            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R);

            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static Bitmap LoadImg(string imgName)
        {
            var bitmap = Resources.ResourceManager.GetObject(imgName) as Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        public static void drawText(string msg, Vector3 Hero, Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1] + weight, color, msg);
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int) vPosX, (int) vPosY, vColor);
        }

        public static void DrawFontTextMap(Font vFont, string vText, Vector3 Pos, ColorBGRA vColor)
        {
            var wts = Drawing.WorldToScreen(Pos);
            vFont.DrawText(null, vText, (int) wts[0], (int) wts[1], vColor);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (getCheckBoxItem("minimap"))
            {
                foreach (var enemy in Program.Enemies)
                {
                    if (!enemy.IsVisible)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null)
                        {
                            var wts = Drawing.WorldToMinimap(ChampionInfoOne.LastVisablePos);
                            DrawFontTextScreen(Tahoma13, enemy.ChampionName[0] + enemy.ChampionName[1].ToString(),
                                wts[0], wts[1], SharpDX.Color.Yellow);
                        }
                    }
                }
            }
            if (getCheckBoxItem("showWards"))
            {
                foreach (var obj in OKTWward.HiddenObjList)
                {
                    if (obj.type == 1)
                    {
                        Utility.DrawCircle(obj.pos, 100, Color.Yellow, 3, 20, true);
                    }

                    if (obj.type == 2)
                    {
                        Utility.DrawCircle(obj.pos, 100, Color.HotPink, 3, 20, true);
                    }

                    if (obj.type == 3)
                    {
                        Utility.DrawCircle(obj.pos, 100, Color.Orange, 3, 20, true);
                    }
                }
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem("disableDraws"))
                return;

            if (getCheckBoxItem("showWards"))
            {
                var circleSize = 30;
                foreach (var obj in OKTWward.HiddenObjList.Where(obj => Render.OnScreen(Drawing.WorldToScreen(obj.pos)))
                    )
                {
                    if (obj.type == 1)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Yellow);
                        DrawFontTextMap(Tahoma13, "" + (int) (obj.endTime - Game.Time), obj.pos, SharpDX.Color.Yellow);
                    }

                    if (obj.type == 2)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.HotPink);
                        DrawFontTextMap(Tahoma13, "VW", obj.pos, SharpDX.Color.HotPink);
                    }
                    if (obj.type == 3)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Orange);
                        DrawFontTextMap(Tahoma13, "! " + (int) (obj.endTime - Game.Time), obj.pos, SharpDX.Color.Orange);
                    }
                }
            }


            var blink = true;

            if ((int) (Game.Time*10)%2 == 0)
                blink = false;

            var HpBar = getCheckBoxItem("HpBar");
            var championInfo = getCheckBoxItem("championInfo");
            var GankAlert = getCheckBoxItem("GankAlert");
            var ShowKDA = getCheckBoxItem("ShowKDA");
            var ShowClicks = getCheckBoxItem("ShowClicks");
            var posY = getSliderItem("posY")*0.01f*Drawing.Height;
            var posX = getSliderItem("posX")*0.01f*Drawing.Width;
            float positionDraw = 0;
            float positionGang = 500;
            var Width = 103;
            var Height = 8;
            var XOffset = 10;
            var YOffset = 20;
            var FillColor = System.Drawing.Color.GreenYellow;
            var Color = System.Drawing.Color.Azure;
            float offset = 0;

            foreach (var enemy in Program.Enemies)
            {
                if (getCheckBoxItem("SS"))
                {
                    offset += 0.15f;
                    if (!enemy.IsVisible && !enemy.IsDead)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null && enemy != Program.jungler)
                        {
                            if ((int) (Game.Time*10)%2 == 0 && Game.Time - ChampionInfoOne.LastVisableTime > 3 &&
                                Game.Time - ChampionInfoOne.LastVisableTime < 7)
                            {
                                DrawFontTextScreen(TextBold,
                                    "SS " + enemy.ChampionName + " " +
                                    (int) (Game.Time - ChampionInfoOne.LastVisableTime), Drawing.Width*offset,
                                    Drawing.Height*0.02f, SharpDX.Color.OrangeRed);
                            }
                            if (Game.Time - ChampionInfoOne.LastVisableTime >= 7)
                            {
                                DrawFontTextScreen(TextBold,
                                    "SS " + enemy.ChampionName + " " +
                                    (int) (Game.Time - ChampionInfoOne.LastVisableTime), Drawing.Width*offset,
                                    Drawing.Height*0.02f, SharpDX.Color.OrangeRed);
                            }
                        }
                    }
                }

                if (enemy.IsValidTarget() && ShowClicks)
                {
                    var lastWaypoint = enemy.GetWaypoints().Last().To3D();
                    if (lastWaypoint.IsValid())
                    {
                        drawLine(enemy.Position, lastWaypoint, 1, Color.Red);

                        if (enemy.GetWaypoints().Count() > 1)
                            DrawFontTextMap(Tahoma13, enemy.ChampionName, lastWaypoint, SharpDX.Color.WhiteSmoke);
                    }
                }

                if (HpBar && enemy.IsHPBarRendered && Render.OnScreen(Drawing.WorldToScreen(enemy.Position)))
                {
                    var barPos = enemy.HPBarPosition;

                    float QdmgDraw = 0, WdmgDraw = 0, EdmgDraw = 0, RdmgDraw = 0, damage = 0;
                    ;

                    if (Q.IsReady())
                        damage = damage + Q.GetDamage(enemy);

                    if (W.IsReady() && Player.ChampionName != "Kalista")
                        damage = damage + W.GetDamage(enemy);

                    if (E.IsReady())
                        damage = damage + E.GetDamage(enemy);

                    if (R.IsReady())
                        damage = damage + R.GetDamage(enemy);

                    if (Q.IsReady())
                        QdmgDraw = Q.GetDamage(enemy)/damage;

                    if (W.IsReady() && Player.ChampionName != "Kalista")
                        WdmgDraw = W.GetDamage(enemy)/damage;

                    if (E.IsReady())
                        EdmgDraw = E.GetDamage(enemy)/damage;

                    if (R.IsReady())
                        RdmgDraw = R.GetDamage(enemy)/damage;

                    var percentHealthAfterDamage = Math.Max(0, enemy.Health - damage)/enemy.MaxHealth;

                    var yPos = barPos.Y + YOffset;
                    var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + XOffset + Width*enemy.Health/enemy.MaxHealth;

                    var differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + XOffset + 107*percentHealthAfterDamage;

                    for (var i = 0; i < differenceInHP; i++)
                    {
                        if (Q.IsReady() && i < QdmgDraw*differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.Cyan);
                        else if (W.IsReady() && i < (QdmgDraw + WdmgDraw)*differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.Orange);
                        else if (E.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw)*differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.Yellow);
                        else if (R.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw + RdmgDraw)*differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.YellowGreen);
                    }
                }

                var kolor = Color.GreenYellow;

                if (enemy.IsDead)
                    kolor = Color.Gray;
                else if (!enemy.IsVisible)
                    kolor = Color.OrangeRed;

                var kolorHP = Color.GreenYellow;

                if (enemy.IsDead)
                    kolorHP = Color.GreenYellow;
                else if ((int) enemy.HealthPercent < 30)
                    kolorHP = Color.Red;
                else if ((int) enemy.HealthPercent < 60)
                    kolorHP = Color.Orange;

                if (championInfo)
                {
                    positionDraw += 15;
                    DrawFontTextScreen(Tahoma13, "" + enemy.Level, posX - 25, posY + positionDraw, SharpDX.Color.White);
                    DrawFontTextScreen(Tahoma13, enemy.ChampionName, posX, posY + positionDraw, SharpDX.Color.White);

                    if (true)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (Game.Time - ChampionInfoOne.FinishRecallTime < 4)
                        {
                            DrawFontTextScreen(Tahoma13, "FINISH", posX - 90, posY + positionDraw,
                                SharpDX.Color.GreenYellow);
                        }
                        else if (ChampionInfoOne.StartRecallTime <= ChampionInfoOne.AbortRecallTime &&
                                 Game.Time - ChampionInfoOne.AbortRecallTime < 4)
                        {
                            DrawFontTextScreen(Tahoma13, "ABORT", posX - 90, posY + positionDraw,
                                SharpDX.Color.Yellow);
                        }
                        else if (Game.Time - ChampionInfoOne.StartRecallTime < 8)
                        {
                            var recallPercent = (int) ((Game.Time - ChampionInfoOne.StartRecallTime)/8*100);
                            var recallX1 = posX - 90;
                            var recallY1 = posY + positionDraw + 3;
                            var recallX2 = recallX1 + recallPercent/2 + 1;
                            var recallY2 = posY + positionDraw + 3;
                            Drawing.DrawLine(recallX1, recallY1, recallX1 + 50, recallY2, 8, Color.Red);
                            Drawing.DrawLine(recallX1, recallY1, recallX2, recallY2, 8, Color.White);
                        }
                    }

                    if (ShowKDA)
                    {
                        var fSlot = enemy.Spellbook.Spells[4];

                        if (fSlot.Name != "SummonerFlash")
                            fSlot = enemy.Spellbook.Spells[5];

                        if (fSlot.Name == "SummonerFlash")
                        {
                            var fT = fSlot.CooldownExpires - Game.Time;
                            if (fT < 0)
                                DrawFontTextScreen(Tahoma13, "F rdy", posX + 110, posY + positionDraw,
                                    SharpDX.Color.GreenYellow);
                            else
                                DrawFontTextScreen(Tahoma13, "F " + (int) fT, posX + 110, posY + positionDraw,
                                    SharpDX.Color.Yellow);
                        }

                        if (enemy.Level > 5)
                        {
                            var rSlot = enemy.Spellbook.Spells[3];
                            var t = rSlot.CooldownExpires - Game.Time;

                            if (t < 0)
                                DrawFontTextScreen(Tahoma13, "R rdy", posX + 145, posY + positionDraw,
                                    SharpDX.Color.GreenYellow);
                            else
                                DrawFontTextScreen(Tahoma13, "R " + (int) t, posX + 145, posY + positionDraw,
                                    SharpDX.Color.Yellow);
                        }
                        else
                            DrawFontTextScreen(Tahoma13, "R ", posX + 145, posY + positionDraw, SharpDX.Color.Yellow);
                    }

                    //Drawing.DrawText(posX - 70, posY + positionDraw, kolor, enemy.Level + " lvl");
                }

                var Distance = Player.Distance(enemy.Position);
                if (GankAlert && !enemy.IsDead && Distance > 1200)
                {
                    var wts =
                        Drawing.WorldToScreen(ObjectManager.Player.Position.Extend(enemy.Position, positionGang).To3D());

                    wts[0] = wts[0];
                    wts[1] = wts[1] + 15;

                    if ((int) enemy.HealthPercent > 0)
                        Drawing.DrawLine(wts[0], wts[1], wts[0] + (int) enemy.HealthPercent/2 + 1, wts[1], 8, kolorHP);

                    if ((int) enemy.HealthPercent < 100)
                        Drawing.DrawLine(wts[0] + (int) enemy.HealthPercent/2, wts[1], wts[0] + 50, wts[1], 8,
                            Color.White);

                    if (Distance > 3500 && enemy.IsVisible)
                    {
                        DrawFontTextMap(Tahoma13, enemy.ChampionName,
                            Player.Position.Extend(enemy.Position, positionGang).To3D(), SharpDX.Color.White);
                    }
                    else if (!enemy.IsVisible)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null)
                        {
                            if (Game.Time - ChampionInfoOne.LastVisableTime > 3 &&
                                Game.Time - ChampionInfoOne.LastVisableTime < 7)
                            {
                                if (blink)
                                    DrawFontTextMap(Tahoma13,
                                        "SS " + enemy.ChampionName + " " +
                                        (int) (Game.Time - ChampionInfoOne.LastVisableTime),
                                        Player.Position.Extend(enemy.Position, positionGang).To3D(),
                                        SharpDX.Color.Yellow);
                            }
                            else
                            {
                                DrawFontTextMap(Tahoma13,
                                    "SS " + enemy.ChampionName + " " +
                                    (int) (Game.Time - ChampionInfoOne.LastVisableTime),
                                    Player.Position.Extend(enemy.Position, positionGang).To3D(), SharpDX.Color.Yellow);
                            }
                        }
                        else
                            DrawFontTextMap(Tahoma13, "SS " + enemy.ChampionName,
                                Player.Position.Extend(enemy.Position, positionGang).To3D(), SharpDX.Color.Yellow);
                    }
                    else if (blink)
                    {
                        DrawFontTextMap(Tahoma13B, enemy.ChampionName,
                            Player.Position.Extend(enemy.Position, positionGang).To3D(), SharpDX.Color.OrangeRed);
                    }

                    if (Distance < 3500 && enemy.IsVisible &&
                        !Render.OnScreen(
                            Drawing.WorldToScreen(Player.Position.Extend(enemy.Position, Distance + 500).To3D())))
                    {
                        drawLine(Player.Position.Extend(enemy.Position, 100).To3D(),
                            Player.Position.Extend(enemy.Position, positionGang - 100).To3D(),
                            (int) ((3500 - Distance)/300), Color.OrangeRed);
                    }
                    else if (Distance < 3500 && !enemy.IsVisible &&
                             !Render.OnScreen(
                                 Drawing.WorldToScreen(Player.Position.Extend(enemy.Position, Distance + 500).To3D())))
                    {
                        var need = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (need != null && Game.Time - need.LastVisableTime < 5)
                        {
                            drawLine(Player.Position.Extend(enemy.Position, 100).To3D(),
                                Player.Position.Extend(enemy.Position, positionGang - 100).To3D(),
                                (int) ((3500 - Distance)/300), Color.Gray);
                        }
                    }
                }
                positionGang = positionGang + 100;
            }

            if (Program.AIOmode == 2)
            {
                Drawing.DrawText(Drawing.Width*0.2f, Drawing.Height*1f, Color.Cyan, "OKTW AIO only utility mode ON");
            }
        }
    }
}