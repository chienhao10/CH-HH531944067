using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Irelia.Common;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = SharpDX.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace Irelia.Modes
{
    internal class DangerousSpells
    {
        public string SpellName { get; private set; }
        public string ChampionName { get; private set; }
        public SpellSlot SpellSlot { get; private set; }
        public SkillType Type{ get; private set; }

        public enum SkillType
        {
            Target,
            Zone
        }

        public DangerousSpells(string spellName, string championName, SpellSlot spellSlot, SkillType type)
        {
            SpellName = spellName;
            ChampionName = championName;
            SpellSlot = spellSlot;
            Type = type;
        }
    }

    internal static class ModeJump
    {
        public static Menu MenuLocal { get; private set; }

        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;

        private static Obj_AI_Base JumpObject;
        private static AIHeroClient JumpTarget;

        public static List<DangerousSpells> DangerousSpells = new List<DangerousSpells>();

        private static void InitDangerousSpells()
        {
            DangerousSpells.Add(new DangerousSpells("malzaharR", "malzahar", SpellSlot.R, Modes.DangerousSpells.SkillType.Target));
            DangerousSpells.Add(new DangerousSpells("skarnerR", "skarner", SpellSlot.R, Modes.DangerousSpells.SkillType.Target));
            DangerousSpells.Add(new DangerousSpells("warwickR", "warwick", SpellSlot.R, Modes.DangerousSpells.SkillType.Target));

            DangerousSpells.Add(new DangerousSpells("fiddlesticksR", "fiddlesticks", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
            DangerousSpells.Add(new DangerousSpells("pantheonR", "pantheon", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
            DangerousSpells.Add(new DangerousSpells("shenR", "shen", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
            DangerousSpells.Add(new DangerousSpells("twistedfateR", "twistedfate", SpellSlot.R, Modes.DangerousSpells.SkillType.Zone));
        }

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = ParentMenu.AddSubMenu("Q Jump Double / Multi", "QDoubleJump");
            {
                MenuLocal.Add("Jump.Mode", new ComboBox("Jump Mode:", 4, "Off", "Everytime", "If can stun target", "If can kill target", "Can stun + can kill"));
                //MenuLocal.Add("Jump.ModeDesc1", CommonHelper.Tab + "Tip: You can change Jump Mode with mouse scroll").SetFontStyle(FontStyle.Regular, Color.GreenYellow));
                //MenuLocal.Add("Jump.ModeDesc2", CommonHelper.Tab + "Tip: Jump Mode only works on Combo Mode").SetFontStyle(FontStyle.Regular, Color.GreenYellow));
                MenuLocal.Add("Jump.Draw.Arrows", new CheckBox("Draw Jump Arrows"));
                MenuLocal.Add("Jump.Draw.Status", new CheckBox("Show Jump Status"));
                MenuLocal.Add("Jump.Recommended", new CheckBox("Load Recommended Settings"));
                MenuLocal.Add("Jump.Enabled", new KeyBind("Enabled:", false, KeyBind.BindTypes.PressToggle, 'G'));

                MenuLocal.AddGroupLabel("Q Jump Blockable Spells");
                {
                    MenuLocal.Add("Jump.Block.Teleport", new CheckBox("Enemy Teleport:"));

                    foreach (var d in DangerousSpells)
                    {
                        foreach (var t in HeroManager.Enemies.Where(t => string.Equals(JumpTarget.ChampionName, d.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            MenuLocal.Add("Jump.Block." + d.ChampionName + d.SpellSlot, new CheckBox(JumpTarget.ChampionName + " : " + d.SpellSlot));
                        }
                    }
                }
                 
                InitDangerousSpells();

                Game.OnUpdate += GameOnOnUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Game.OnWndProc += Game_OnWndProc;
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != 0x20a)
            {
                return;
            }
            var newValue = getBoxItem(MenuLocal, "Jump.Mode") + 1;

            if (newValue == 5)
            {
                newValue = 0;
            }

            MenuLocal["Jump.Mode"].Cast<ComboBox>().CurrentValue = newValue;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            JumpTarget = TargetSelector.GetTarget(Q.Range*3, DamageType.Physical);
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (!JumpTarget.LSIsValidTarget())
            {
                return;
            }
            
            if (!JumpObject.LSIsValidTarget(Q.Range))
            {
                return;
            }

            if (!Q.IsReady())
            {
                return;
            }

            if (!getKeyBindItem(MenuLocal, "Jump.Enabled"))
            {
                return;
            }
            var jumpMode = getBoxItem(MenuLocal, "Jump.Mode");
            if (jumpMode != 0)
            {
                switch (jumpMode)
                {
                    case 1:
                    {
                        Q.CastOnUnit(JumpObject);
                        break;
                    }
                    case 2:
                        {
                            if (JumpTarget.CanStun())
                            {
                                Q.CastOnUnit(JumpObject);
                            }
                            break;
                        }
                    case 3:
                        {
                            if (JumpTarget.Health < CommonMath.GetComboDamage(JumpTarget))
                            {
                                Q.CastOnUnit(JumpObject);
                            }
                            break;
                        }
                    case 4:
                        {
                            if (JumpTarget.CanStun() || JumpTarget.Health < CommonMath.GetComboDamage(JumpTarget))
                            {
                                Q.CastOnUnit(JumpObject);
                            }
                            break;
                        }
                }

            }
            
            //if (!JumpTarget.LSIsValidTarget(Q.Range) && !JumpTarget.LSIsValidTarget(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65))
            //{
            //    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, JumpTarget);
            //}


            return;

            /*
            if (JumpTarget.UnderTurret(true) && getBoxItem(MenuLocal, "Jump.TurretControl") == 0)
            {
                return;
            }


            if (JumpTarget.UnderTurret(true) 
                && getBoxItem(MenuLocal, "Jump.TurretControl") == 1 
                && JumpTarget.Health < Common.CommonMath.GetComboDamage(JumpTarget))
            {
                Q.CastOnUnit(JumpObject);
            }

            var jumpQ = getBoxItem(MenuLocal, "Jump.TurretControl");

            switch (jumpQ)
            {
                case 0:
                {
                    Q.CastOnUnit(JumpObject);
                    break;
                }

                case 1:
                {
                    if (JumpTarget.CanStun())
                    {
                        Q.CastOnUnit(JumpObject);
                    }
                    break;
                }

                case 2:
                {
                    if (JumpTarget.Health < Common.CommonMath.GetComboDamage(JumpTarget))
                    {
                        Q.CastOnUnit(JumpObject);
                    }
                    break;
                }
                case 3:
                {
                    if (JumpTarget.CanStun() && JumpTarget.Health < Common.CommonMath.GetComboDamage(JumpTarget))
                    {
                        Q.CastOnUnit(JumpObject);
                    }
                    break;
                }
            }
            */
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (getCheckBoxItem(MenuLocal, "Jump.Draw.Status"))
            {
                var enabled = getKeyBindItem(MenuLocal, "Jump.Enabled");
                var stat = getBoxItem(MenuLocal, "Jump.Mode");
                CommonHelper.DrawText(CommonHelper.TextStatus, "Q Jump: " + stat, (int)ObjectManager.Player.HPBarPosition.X + 145, (int)ObjectManager.Player.HPBarPosition.Y + 5, enabled && stat != 0 ? Color.White : Color.Gray);
            }

            if (!getCheckBoxItem(MenuLocal, "Jump.Draw.Arrows"))
            {
                return;
            }
            if (JumpTarget.LSIsValidTarget(Q.Range))
            {
                return;
            }

            if (JumpTarget.LSIsValidTarget() && ObjectManager.Player.LSDistance(JumpTarget) > Q.Range)
            {
                
                var toPolygon = new Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.LSTo2D(), ObjectManager.Player.Position.LSTo2D().LSExtend(JumpTarget.Position.LSTo2D(), Q.Range * 3), 250).ToPolygon();
                toPolygon.Draw(System.Drawing.Color.Red, 1);
                var otherEnemyObjects =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(m => m.IsEnemy && !m.IsDead && !m.IsZombie && m.LSIsValidTarget(Q.Range) && m.NetworkId != JumpTarget.NetworkId)
                        .Where(m => toPolygon.IsInside(m))
                        .Where(m => ObjectManager.Player.LSDistance(JumpTarget) > ObjectManager.Player.LSDistance(m))
                        .Where(m => m.Health < Q.GetDamage(m))
                        .Where(m => !m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165))
                        .OrderBy(m => m.LSDistance(JumpTarget.Position));

                JumpObject = otherEnemyObjects.FirstOrDefault(m => m.LSDistance(JumpTarget.Position) <= Q.Range * 2 && m.LSDistance(JumpTarget.Position) > Orbwalking.GetRealAutoAttackRange(null));

                if (JumpObject != null)
                {
                    if (JumpObject.LSIsValidTarget(Q.Range))// && JumpTarget.Health <= ComboDamage(t, R.Instance.Ammo - 1 < 0 ? 0: R.Instance.Ammo - 1) && Utils.UltiChargeCount >= 2)
                    {
                        var startpos = ObjectManager.Player.Position;
                        var endpos = JumpObject.Position;
                        var endpos1 = JumpObject.Position + (startpos - endpos).LSTo2D().LSNormalized().LSRotated(30 * (float)Math.PI / 180).To3D() * ObjectManager.Player.BoundingRadius * 2;
                        var endpos2 = JumpObject.Position + (startpos - endpos).LSTo2D().LSNormalized().LSRotated(-30 * (float)Math.PI / 180).To3D() * ObjectManager.Player.BoundingRadius * 2;

                        var width = 1;

                        var x = new LeagueSharp.Common.Geometry.Polygon.Line(startpos, endpos); x.Draw(System.Drawing.Color.Blue, width);
                        var y = new LeagueSharp.Common.Geometry.Polygon.Line(endpos, endpos1); y.Draw(System.Drawing.Color.Blue, width + 1);
                        var z = new LeagueSharp.Common.Geometry.Polygon.Line(endpos, endpos2); z.Draw(System.Drawing.Color.Blue, width + 1);

                        Vector3[] objectCenter = new[] { ObjectManager.Player.Position, JumpObject.Position };
                        var aX = Drawing.WorldToScreen(new Vector3(Common.CommonHelper.CenterOfVectors(objectCenter).X, Common.CommonHelper.CenterOfVectors(objectCenter).Y, Common.CommonHelper.CenterOfVectors(objectCenter).Z));
                        Drawing.DrawText(aX.X - 15, aX.Y - 15, System.Drawing.Color.White, "1st Jump");

                        /*---------------------------------------------------------------------------------------------------------*/
                        var xStartPos = JumpObject.Position;
                        var xEndPos = JumpTarget.Position;
                        var xEndPos1 = JumpTarget.Position + (xStartPos - xEndPos).LSTo2D().LSNormalized().LSRotated(30 * (float)Math.PI / 180).To3D() * JumpObject.BoundingRadius * 2;
                        var xEndPost2 = JumpTarget.Position + (xStartPos - xEndPos).LSTo2D().LSNormalized().LSRotated(-30 * (float)Math.PI / 180).To3D() * JumpObject.BoundingRadius * 2;

                        var xWidth = 1;

                        var x1 = new LeagueSharp.Common.Geometry.Polygon.Line(xStartPos, xEndPos); x1.Draw(System.Drawing.Color.IndianRed, xWidth);

                        var y1 = new LeagueSharp.Common.Geometry.Polygon.Line(xEndPos, xEndPos1); y1.Draw(System.Drawing.Color.IndianRed, xWidth + 1);
                        var z1 = new LeagueSharp.Common.Geometry.Polygon.Line(xEndPos, xEndPost2); z1.Draw(System.Drawing.Color.IndianRed, xWidth + 1);

                        Vector3[] enemyCenter = new[] { JumpObject.Position, JumpTarget.Position };
                        var bX =
                            Drawing.WorldToScreen(new Vector3(Common.CommonHelper.CenterOfVectors(enemyCenter).X, Common.CommonHelper.CenterOfVectors(enemyCenter).Y,
                                Common.CommonHelper.CenterOfVectors(enemyCenter).Z));
                    }
                }
            }
        }
    }
}
