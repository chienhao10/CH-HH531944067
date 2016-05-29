using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;

namespace Irelia.Modes
{
    internal static class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;


        public static void Init()
        {
            MenuLocal = ModeConfig.MenuConfig.AddSubMenu("Combo", "Combo");
            MenuLocal.Add("Combo.Mode", new ComboBox("Mode:", 1, "Q -> E-> W", "Q -> AA -> E -> AA -> W -> AA"));
            MenuLocal.Add("Combo.Q", new ComboBox("Q:", 1, "Off", "On"));
            MenuLocal.Add("Combo.Q.KillSteal", new ComboBox("Q Kill Steal:", 1, "Off", "On"));
            MenuLocal.Add("Combo.W", new ComboBox("W:", 1, "Off", "On"));
            MenuLocal.Add("Combo.E", new ComboBox("E:", 1, "Off", "On: Everytime", "On: Just for stun"));
            MenuLocal.Add("Combo.R", new ComboBox("R:", 1, "Off", "On"));

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPreAttack += OrbwalkingOnBeforeAttack;

            Drawing.OnDraw += DrawingOnOnDraw;
        }


        private static Dictionary<int, int> JumpingObjects = new Dictionary<int, int>();

        private static void GetJumpingObjects()
        {
            ;
            var t = ObjectManager.Get<Obj_AI_Base>()
                     .OrderBy(obj => obj.LSDistance(ObjectManager.Player.ServerPosition))
                     .FirstOrDefault(
                         obj =>
                             !obj.IsAlly && !obj.IsMe && !obj.IsMinion && (obj is Obj_AI_Turret) &&
                              Game.CursorPos.LSDistance(obj.ServerPosition) <= Q.Range * 8);

            if (t == null)
            {
                return;
            }
            var toPolygon = new Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.LSTo2D(), ObjectManager.Player.Position.LSTo2D().LSExtend(t.Position.LSTo2D(), t.LSDistance(ObjectManager.Player.Position)), (E.Range / 2) + E.Range / 3).ToPolygon();
            toPolygon.Draw(System.Drawing.Color.Red, 1);

            var startPos = ObjectManager.Player.Position + Vector3.Normalize(ObjectManager.Player.Position - t.ServerPosition) * (Q.Range);

            for (var i = 1; i < (ObjectManager.Player.LSDistance(t.Position) / Q.Range) + 1; i++)
            {
                var targetBehind = startPos + Vector3.Normalize(t.ServerPosition - startPos) * i * Q.Range;


                var existsMinion = JumpingObjects[i];

                var minions =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.IsAlly && !m.IsDead)
                        .Where(m => toPolygon.IsInside(m) && m.LSDistance(targetBehind) < Q.Range && m.NetworkId != existsMinion
                        //&& m.Health < Q.GetDamage(m)
                        )
                        .OrderByDescending(m => m.LSDistance(ObjectManager.Player.Position))
                        .FirstOrDefault();

                if (minions != null)
                {
                    var j = JumpingObjects.Find(x => x.Key == minions.NetworkId);
                    if (minions.NetworkId != j.Key && i != j.Value)
                    {
                        JumpingObjects.Remove(existsMinion);
                    }

                    JumpingObjects.Add(minions.NetworkId, i);

                    Render.Circle.DrawCircle(minions.Position, minions.BoundingRadius, System.Drawing.Color.Red);
                }
                Render.Circle.DrawCircle(targetBehind, Q.Range, System.Drawing.Color.Yellow);
            }
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            return;
            //GetJumpingObjects();

            //return;
            //if (!Q.IsReady())
            //{
            //    return;
            //}

            //var SearchRange = Q.Range * 4;
            //var t = TargetSelector.GetTarget(SearchRange);
            //if (!t.LSIsValidTarget())
            //{
            //    return;
            //}
            //if (t.LSIsValidTarget(Q.Range))
            //{
            //    return;
            //}
            //var toPolygon = new Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.LSTo2D(), ObjectManager.Player.Position.LSTo2D().LSExtend(t.Position.LSTo2D(), SearchRange), (E.Range / 2) + E.Range / 3).ToPolygon();
            //toPolygon.Draw(System.Drawing.Color.Red, 1);

            //var minions =
            //    ObjectManager.Get<Obj_AI_Base>()
            //        .Where(m => !m.IsAlly && !m.IsDead)
            //        .Where(m => toPolygon.IsInside(m) && m.Health < Q.GetDamage(m))
            //        .OrderByDescending(m => m.LSIsValidTarget(Q.Range + 150))
            //        .FirstOrDefault();

            //if (minions != null)
            //{
            //    Render.Circle.DrawCircle(minions.Position, 115f, System.Drawing.Color.DarkRed);
            //    if (minions.LSIsValidTarget(Q.Range))
            //    {
            //        Q.CastOnUnit(minions);
            //    }
            //}
            //Render.Circle.DrawCircle(t.Position, E.Range, System.Drawing.Color.GreenYellow);
        }

        private static int BladesSpellCount
        {
            get
            {
                return
                    ObjectManager.Player.Buffs.Where(buff => buff.Name.ToLower() == "ireliatranscendentbladesspell")
                        .Select(buff => buff.Count)
                        .FirstOrDefault();
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

        private static void OrbwalkingOnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!Common.CommonHelper.ShouldCastSpell(TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65, DamageType.Physical)))
            {
                return;
            }

            if (!W.IsReady() || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || getBoxItem(MenuLocal, "Combo.W") == 0)
            {
                return;
            }

            if (Common.CommonHelper.ShouldCastSpell((AIHeroClient)args.Target) && args.Target is AIHeroClient)
            {
                W.Cast();
            }
        }

        private static void OnUpdate(EventArgs args)
        {

            //foreach (var b in ObjectManager.Player.Buffs)
            //{
            //    Console.WriteLine(b.DisplayName + " : " + b.Count);
            //}
            //Console.WriteLine("-------------------------------------------------");


            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            ExecuteCombo();
        }

        private static void ExecuteCombo()
        {
            if (!Common.CommonHelper.ShouldCastSpell(TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65, DamageType.Physical)))
            {
                return;
            }

            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (!t.LSIsValidTarget())
            {
                return;
            }

            if (t.LSIsValidTarget(Q.Range) && getBoxItem(MenuLocal, "Combo.Q.KillSteal") == 1)
            {
                var enemy = HeroManager.Enemies.Find(e => Q.CanCast(e) && e.Health < Q.GetDamage(e));
                if (enemy != null)
                {
                    Champion.PlayerSpells.CastQCombo(enemy);
                }
            }

            if (t.LSIsValidTarget(Q.Range) && getBoxItem(MenuLocal, "Combo.Q") == 1 && t.Health < Q.GetDamage(t))
            {

                var closesMinion =
                    MinionManager.GetMinions(Q.Range)
                        .Where(
                            m =>
                                m.LSDistance(t.Position) < Orbwalking.GetRealAutoAttackRange(null) &&
                                m.Health < Q.GetDamage(m) - 15)
                        .OrderBy(m1 => m1.LSDistance(t.Position))
                        .FirstOrDefault();

                if (closesMinion != null)
                {
                    Q.CastOnUnit(closesMinion);
                }
                else
                {
                    Champion.PlayerSpells.CastQCombo(t);
                }
            }

            if (t.LSIsValidTarget(Q.Range) && getBoxItem(MenuLocal, "Combo.Q") == 1 && !t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            {
                Champion.PlayerSpells.CastQCombo(t);
            }

            if (t.LSIsValidTarget(E.Range))
            {
                switch (getBoxItem(MenuLocal, "Combo.E"))
                {
                    case 1:
                        {
                            Champion.PlayerSpells.CastECombo(t);
                            break;
                        }
                    case 2:
                        {
                            if (t.Health > ObjectManager.Player.Health)
                            {
                                Champion.PlayerSpells.CastECombo(t);
                            }
                            break;
                        }
                }
            }

            if (R.IsReady() && getBoxItem(MenuLocal, "Combo.R") == 1 && t.LSIsValidTarget(R.Range) && BladesSpellCount >= 0)
            {
                if (!t.LSIsValidTarget(Q.Range + Orbwalking.GetRealAutoAttackRange(null)) && t.Health < R.GetDamage(t) * 4)
                {
                    PredictionOutput rPredictionOutput = R.GetPrediction(t);
                    Vector3 castPosition = rPredictionOutput.CastPosition.LSExtend(ObjectManager.Player.Position, -(ObjectManager.Player.LSDistance(t.ServerPosition) >= 450 ? 80 : 120));
                    if (rPredictionOutput.Hitchance >= (ObjectManager.Player.LSDistance(t.ServerPosition) >= R.Range / 2 ? HitChance.VeryHigh : HitChance.High) && ObjectManager.Player.LSDistance(castPosition) < R.Range)
                    {
                        R.Cast(castPosition);
                    }
                }

                if (CommonMath.GetComboDamage(t) > t.Health && t.LSIsValidTarget(Q.Range) && Q.IsReady())
                {
                    R.Cast(t, false, true);
                }

                if (BladesSpellCount > 0 && BladesSpellCount <= 3)
                {
                    var enemy = HeroManager.Enemies.Find(e => e.Health < R.GetDamage(e) * BladesSpellCount && e.LSIsValidTarget(R.Range));
                    if (enemy == null)
                    {
                        foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(R.Range)))
                        {
                            R.Cast(e);
                        }
                    }
                    else
                    {
                        R.Cast(enemy);
                    }
                }
            }
        }
    }
}
