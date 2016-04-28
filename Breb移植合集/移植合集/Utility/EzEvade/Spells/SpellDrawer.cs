using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Color = System.Drawing.Color;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EzEvade;
using SharpDX;

namespace ezEvade
{
    internal class SpellDrawer
    {
        public static Menu menu;

        private static AIHeroClient myHero { get { return ObjectManager.Player; } }


        public SpellDrawer(Menu mainMenu)
        {
            Drawing.OnDraw += Drawing_OnDraw;

            menu = mainMenu;
            Game_OnGameLoad();
        }

        private void Game_OnGameLoad()
        {
            //Console.WriteLine("SpellDrawer loaded");


            Menu drawMenu = menu.IsSubMenu ? menu.Parent.AddSubMenuEx("Draw", "Draw") : menu.AddSubMenuEx("Draw", "Draw");
            drawMenu.Add("DrawSkillShots", new CheckBox("Draw SkillShots"));
            drawMenu.Add("ShowStatus", new CheckBox("Show Evade Status"));
            drawMenu.Add("DrawSpellPos", new CheckBox("Draw Spell Position"));
            drawMenu.Add("DrawEvadePosition", new CheckBox("Draw Evade Position", false));

            Menu dangerMenu = drawMenu.Parent.AddSubMenuEx("DangerLevel Drawings", "DangerLevelDrawings");
            Menu lowDangerMenu = dangerMenu.Parent.AddSubMenuEx("Low", "LowDrawing");
            lowDangerMenu.Add("LowWidth", new Slider("Line Width", 1, 1, 15));

            Menu normalDangerMenu = dangerMenu.Parent.AddSubMenuEx("Normal", "NormalDrawing");
            normalDangerMenu.Add("NormalWidth", new Slider("Line Width", 2, 1, 15));

            Menu highDangerMenu = dangerMenu.Parent.AddSubMenuEx("High", "HighDrawing");
            highDangerMenu.Add("HighWidth", new Slider("Line Width", 3, 1, 15));

            Menu extremeDangerMenu = dangerMenu.Parent.AddSubMenuEx("Extreme", "ExtremeDrawing");
            extremeDangerMenu.Add("ExtremeWidth", new Slider("Line Width", 4, 1, 15));

            /*
            Menu undodgeableDangerMenu = new Menu("Undodgeable", "Undodgeable");
            undodgeableDangerMenu.AddItem(new MenuItem("Width", "Line Width").SetValue(new Slider(6, 1, 15)));
            undodgeableDangerMenu.AddItem(new MenuItem("Color", "Color").SetValue(new Circle(true, Color.FromArgb(255, 255, 0, 0))));*/
        }

        private void DrawLineRectangle(Vector2 start, Vector2 end, int radius, int width, Color color)
        {
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightStartPos = start + pDir * radius;
            var leftStartPos = start - pDir * radius;
            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            var rStartPos = Drawing.WorldToScreen(new Vector3(rightStartPos.X, rightStartPos.Y, myHero.Position.Z));
            var lStartPos = Drawing.WorldToScreen(new Vector3(leftStartPos.X, leftStartPos.Y, myHero.Position.Z));
            var rEndPos = Drawing.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, myHero.Position.Z));
            var lEndPos = Drawing.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, myHero.Position.Z));

            Drawing.DrawLine(rStartPos, rEndPos, width, color);
            Drawing.DrawLine(lStartPos, lEndPos, width, color);
            Drawing.DrawLine(rStartPos, lStartPos, width, color);
            Drawing.DrawLine(lEndPos, rEndPos, width, color);
        }

        private void DrawEvadeStatus()
        {
            if (ObjectCache.menuCache.cache["ShowStatus"].Cast<CheckBox>().CurrentValue)
            {
                var heroPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
                var dimension = Drawing.GetTextEntent("Evade: ON", 12);

                if (ObjectCache.menuCache.cache["DodgeSkillShots"].Cast<KeyBind>().CurrentValue)
                {
                    if (Evade.isDodging)
                    {
                        Drawing.DrawText(heroPos.X - dimension.Width / 2, heroPos.Y, Color.Red, "Evade: ON");
                    }
                    else
                    {
                        Drawing.DrawText(heroPos.X - dimension.Width/2, heroPos.Y,
                            Evade.isDodgeDangerousEnabled() ? Color.Yellow : Color.White, "Evade: ON");
                    }
                }
                else
                {
                    if (ObjectCache.menuCache.cache["ActivateEvadeSpells"].Cast<KeyBind>().CurrentValue)
                    {
                        Drawing.DrawText(heroPos.X - dimension.Width / 2, heroPos.Y, Color.Purple, "Evade: Spell");
                    }
                    else
                    {
                        Drawing.DrawText(heroPos.X - dimension.Width / 2, heroPos.Y, Color.Gray, "Evade: OFF");
                    }
                }



            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {

            if (ObjectCache.menuCache.cache["DrawEvadePosition"].Cast<CheckBox>().CurrentValue)
            {
                //Render.Circle.DrawCircle(myHero.Position.ExtendDir(dir, 500), 65, Color.Red, 10);

                /*foreach (var point in myHero.Path)
                {
                    Render.Circle.DrawCircle(point, 65, Color.Red, 10);
                }*/

                if (Evade.lastPosInfo != null)
                {
                    var pos = Evade.lastPosInfo.position; //Evade.lastEvadeCommand.targetPosition;
                    Render.Circle.DrawCircle(new Vector3(pos.X, pos.Y, myHero.Position.Z), 65, Color.Red, 10);
                }
            }

            DrawEvadeStatus();

            if (ObjectCache.menuCache.cache["DrawSkillShots"].Cast<CheckBox>().CurrentValue == false)
            {
                return;
            }

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.drawSpells)
            {
                Spell spell = entry.Value;

                var dangerStr = spell.GetSpellDangerString();
                //var spellDrawingConfig = ObjectCache.menuCache.cache[dangerStr + "Color"].GetValue<Circle>();
                var spellDrawingWidth = ObjectCache.menuCache.cache[dangerStr + "Width"].Cast<Slider>().CurrentValue;

                if (ObjectCache.menuCache.cache[spell.info.spellName + "DrawSpell"].Cast<CheckBox>().CurrentValue)
                {
                    if (spell.spellType == SpellType.Line)
                    {
                        Vector2 spellPos = spell.currentSpellPosition;
                        Vector2 spellEndPos = spell.GetSpellEndPosition();

                        DrawLineRectangle(spellPos, spellEndPos, (int)spell.radius, spellDrawingWidth, Color.White);

                        /*foreach (var hero in ObjectManager.Get<AIHeroClient>())
                        {
                            Render.Circle.DrawCircle(new Vector3(hero.ServerPosition.X, hero.ServerPosition.Y, myHero.Position.Z), (int)spell.radius, Color.Red, 5);
                        }*/

                        if (ObjectCache.menuCache.cache["DrawSpellPos"].Cast<CheckBox>().CurrentValue)// && spell.spellObject != null)
                        {
                            //spellPos = SpellDetector.GetCurrentSpellPosition(spell, true, ObjectCache.gamePing);

                            /*if (true)
                            {
                                var spellPos2 = spell.startPos + spell.direction * spell.info.projectileSpeed * (Evade.GetTickCount - spell.startTime - spell.info.spellDelay) / 1000 + spell.direction * spell.info.projectileSpeed * ((float)ObjectCache.gamePing / 1000);
                                Render.Circle.DrawCircle(new Vector3(spellPos2.X, spellPos2.Y, myHero.Position.Z), (int)spell.radius, Color.Red, 8);
                            }*/

                            /*if (spell.spellObject != null && spell.spellObject.IsValid && spell.spellObject.IsVisible &&
                                  spell.spellObject.Position.To2D().Distance(ObjectCache.myHeroCache.serverPos2D) < spell.info.range + 1000)*/

                            Render.Circle.DrawCircle(new Vector3(spellPos.X, spellPos.Y, myHero.Position.Z), (int)spell.radius, Color.White, spellDrawingWidth);
                        }

                    }
                    else if (spell.spellType == SpellType.Circular)
                    {
                        Render.Circle.DrawCircle(new Vector3(spell.endPos.X, spell.endPos.Y, spell.height), (int)spell.radius, Color.White, spellDrawingWidth);

                        if (spell.info.spellName == "VeigarEventHorizon")
                        {
                            Render.Circle.DrawCircle(new Vector3(spell.endPos.X, spell.endPos.Y, spell.height), (int)spell.radius - 125, Color.White, spellDrawingWidth);
                        }
                    }
                    else if (spell.spellType == SpellType.Arc)
                    {                      
                        /*var spellRange = spell.startPos.Distance(spell.endPos);
                        var midPoint = spell.startPos + spell.direction * (spellRange / 2);

                        Render.Circle.DrawCircle(new Vector3(midPoint.X, midPoint.Y, myHero.Position.Z), (int)spell.radius, spellDrawingConfig.Color, spellDrawingWidth);
                        
                        Drawing.DrawLine(Drawing.WorldToScreen(spell.startPos.To3D()),
                                         Drawing.WorldToScreen(spell.endPos.To3D()), 
                                         spellDrawingWidth, spellDrawingConfig.Color);*/
                    }
                    else if (spell.spellType == SpellType.Cone)
                    {

                    }
                }
            }
        }
    }
}
