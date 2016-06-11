using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace Marksman.Champions
{
    internal class Reticles
    {
        public GameObject Object { get; set; }
        public float NetworkId { get; set; }
        public Vector3 ReticlePos { get; set; }
        public double ExpireTime { get; set; }
    }

    internal class Draven : Champion
    {
        private static readonly List<Reticles> ExistingReticles = new List<Reticles>();
        public static LeagueSharp.Common.Spell Q, W, E, R;
        public int QStacks = 0;

        private static string Tab
        {
            get { return "    "; }
        }

        public Draven()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1100);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 20000);
            E.SetSkillshot(250f, 130f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(400f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnEndScene += DrawingOnOnEndScene;
            Utils.Utils.PrintMessage("Draven loaded.");
        }

        public void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && Program.misc["EGapCloser"].Cast<CheckBox>().CurrentValue && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && Program.misc["EInterruptable"].Cast<CheckBox>().CurrentValue && unit.LSIsValidTarget(E.Range))
            {
                E.Cast(unit);
            }
        }

        public override void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if ((sender.Name.Contains("Q_reticle_self")))
            {
                for (var i = 0; i < ExistingReticles.Count; i++)
                {
                    if (ExistingReticles[i].NetworkId == sender.NetworkId)
                    {
                        ExistingReticles.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public override void OnCreateObject(GameObject sender, EventArgs args)
        {
            if ((sender.Name.Contains("Q_reticle_self")))
            {
                ExistingReticles.Add(
                    new Reticles
                    {
                        Object = sender,
                        NetworkId = sender.NetworkId,
                        ReticlePos = sender.Position,
                        ExpireTime = Game.Time + 1.20
                    });
            }
        }

        private void DrawingOnOnEndScene(EventArgs args)
        {
            var rCircle = Program.combo["DrawRMini"].Cast<CheckBox>().CurrentValue;
            if (rCircle)
            {
                var maxRRange = Program.combo["UseRCMaxR"].Cast<Slider>().CurrentValue;
                var rMax = Program.combo["DrawRMax"].Cast<CheckBox>().CurrentValue;
#pragma warning disable 618
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, maxRRange, Color.DarkMagenta, 1, 23, true);
#pragma warning restore 618
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var drawOrbwalk = Program.marksmanDrawings["DrawOrbwalk"].Cast<CheckBox>().CurrentValue;
            var drawReticles = Program.marksmanDrawings["DrawReticles"].Cast<CheckBox>().CurrentValue;
            var drawCatchRadius = Program.marksmanDrawings["DrawCatchRadius"].Cast<CheckBox>().CurrentValue;

            if (drawOrbwalk)
            {
                Render.Circle.DrawCircle(GetOrbwalkPos(), 100, Color.Yellow);
            }

            if (drawReticles)
            {
                foreach (var existingReticle in ExistingReticles)
                {
                    Render.Circle.DrawCircle(existingReticle.ReticlePos, 100, Color.Green);
                }
            }

            if (drawCatchRadius)
            {
                if (GetOrbwalkPos() != Game.CursorPos &&
                    (ComboActive || LaneClearActive || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)))
                {
                    Render.Circle.DrawCircle(Game.CursorPos, Program.misc["CatchRadius"].Cast<Slider>().CurrentValue,
                        Color.Red);
                }
                else
                {
                    Render.Circle.DrawCircle(
                        Game.CursorPos, Program.misc["CatchRadius"].Cast<Slider>().CurrentValue, Color.CornflowerBlue);
                }
            }

            var drawE = Program.marksmanDrawings["DrawE"].Cast<CheckBox>().CurrentValue;
            if (drawE)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.FromArgb(100, 255, 0, 255));
            }

            var drawRMin = Program.combo["DrawRMin"].Cast<CheckBox>().CurrentValue;
            if (drawRMin)
            {
                var minRRange = Program.combo["UseRCMinR"].Cast<Slider>().CurrentValue;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, minRRange, Color.DarkRed, 2);
            }

            var drawRMax = Program.combo["DrawRMax"].Cast<CheckBox>().CurrentValue;
            if (drawRMax)
            {
                var maxRRange = Program.combo["UseRCMaxR"].Cast<Slider>().CurrentValue;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, maxRRange, Color.DarkMagenta, 2);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            var orbwalkPos = GetOrbwalkPos();
            var cursor = Game.CursorPos;

            if (ComboActive || LaneClearActive || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                if (orbwalkPos != cursor)
                {
                    Orbwalker.OrbwalkTo(orbwalkPos);
                }
                else
                {
                    Orbwalker.OrbwalkTo(cursor);
                }
            }

            AIHeroClient t;
            //Combo
            if (ComboActive)
            {
                var minRRange = Program.combo["UseRCMinR"].Cast<Slider>().CurrentValue;
                var maxRRange = Program.combo["UseRCMaxR"].Cast<Slider>().CurrentValue;

                t = TargetSelector.GetTarget(maxRRange, DamageType.Physical);
                if (!t.LSIsValidTarget())
                {
                    return;
                }
                if (W.IsReady() && Program.combo["UseWC"].Cast<CheckBox>().CurrentValue && t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) &&
                    ObjectManager.Player.Buffs.FirstOrDefault(
                        buff => buff.Name == "dravenfurybuff" || buff.Name == "DravenFury") == null)
                {
                    W.Cast();
                }
                if (IsFleeing(t) && Program.combo["UseEC"].Cast<CheckBox>().CurrentValue && t.LSIsValidTarget(E.Range))
                {
                    E.Cast(t);
                }

                if (Program.combo["UseRC"].Cast<CheckBox>().CurrentValue && R.IsReady())
                {
                    t = TargetSelector.GetTarget(maxRRange, DamageType.Physical);
                    if (t.LSDistance(ObjectManager.Player) >= minRRange && t.LSDistance(ObjectManager.Player) <= maxRRange &&
                        t.Health < ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R) * 2)
                    //R.GetHealthPrediction(target) <= 0)
                    {
                        R.Cast(t);
                    }
                }
            }

            //Peel from melees
            if (Program.misc["EPeel"].Cast<CheckBox>().CurrentValue) //Taken from ziggs(by pq/esk0r)
            {
                foreach (var pos in from enemy in ObjectManager.Get<AIHeroClient>()
                                    where
                                        enemy.LSIsValidTarget() &&
                                        enemy.LSDistance(ObjectManager.Player) <=
                                        enemy.BoundingRadius + enemy.AttackRange + ObjectManager.Player.BoundingRadius &&
                                        LeagueSharp.Common.Orbwalking.IsMelee(enemy)
                                    let direction =
                                        (enemy.ServerPosition.LSTo2D() - ObjectManager.Player.ServerPosition.LSTo2D()).LSNormalized()
                                    let pos = ObjectManager.Player.ServerPosition.LSTo2D()
                                    select pos + Math.Min(200, Math.Max(50, enemy.LSDistance(ObjectManager.Player) / 2)) * direction)
                {
                    E.Cast(pos.To3D());
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            Console.WriteLine("Hai");
            Console.WriteLine(Program.misc["maxqamount"].Cast<Slider>().CurrentValue);
            var qOnHero = QBuffCount();
            if (((ComboActive && Program.combo["UseQC"].Cast<CheckBox>().CurrentValue) ||
                 (HarassActive && Program.harass["UseQH"].Cast<CheckBox>().CurrentValue)) && qOnHero < 2 &&
                qOnHero + ExistingReticles.Count < Program.misc["maxqamount"].Cast<Slider>().CurrentValue)
            {
                Q.Cast();
                Console.WriteLine("Casted Q");
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseWC", new CheckBox("Use W"));
            config.Add("UseEC", new CheckBox("Use E"));
            config.Add("UseRC", new CheckBox("Use R"));
            config.Add("UseRCMinR", new Slider("Min. R Range", 350, 200, 750));
            config.Add("UseRCMaxR", new Slider("Max. R Range", 1000, 750, 3000));
            config.Add("DrawRMin", new CheckBox("Draw Min. R Range"));//.SetValue(new Circle(true, Color.DarkRed)));
            config.Add("DrawRMax", new CheckBox("Draw Max. R Range"));//.SetValue(new Circle(true, Color.DarkMagenta)));
            config.Add("DrawRMini", new CheckBox("Draw R on Mini Map"));

            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawE", new CheckBox("E range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawOrbwalk", new CheckBox("Draw orbwalk position"));//.SetValue(new Circle(true, Color.Yellow)));
            config.Add("DrawReticles", new CheckBox("Draw on reticles"));//.SetValue(new Circle(true, Color.Green)));
            config.Add("DrawCatchRadius", new CheckBox("Draw Catch Radius"));//.SetValue(new Circle(true, Color.Green)));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("maxqamount", new Slider("Max Qs to use simultaneous", 2, 1, 4));
            config.Add("EGapCloser", new CheckBox("Auto E Gap closers"));
            config.Add("EInterruptable", new CheckBox("Auto E interruptable spells"));
            config.Add("Epeel", new CheckBox("Peel self with E"));
            config.Add("CatchRadius", new Slider("Axe catch radius", 600, 200, 1000));
            return true;
        }

        public static int QBuffCount()
        {
            var buff =
                ObjectManager.Player.Buffs.FirstOrDefault(buff1 => buff1.Name.Equals("dravenspinningattack"));
            return ExistingReticles.Count + (buff != null ? buff.Count : 0);
        }

        public Vector3 GetOrbwalkPos()
        {
            if (ExistingReticles.Count <= 0)
            {
                return Game.CursorPos;
            }
            var myHero = ObjectManager.Player;
            var cursor = Game.CursorPos;
            var reticles =
                ExistingReticles.OrderBy(reticle => reticle.ExpireTime)
                    .FirstOrDefault(
                        reticle =>
                            reticle.ReticlePos.LSDistance(cursor) <= Program.misc["CatchRadius"].Cast<Slider>().CurrentValue &&
                            reticle.Object.IsValid &&
                            myHero.GetPath(reticle.ReticlePos).ToList().LSTo2D().PathLength() / myHero.MoveSpeed + Game.Time <
                            reticle.ExpireTime);

            return reticles != null && myHero.LSDistance(reticles.ReticlePos) >= 100 ? reticles.ReticlePos : cursor;
        }

        public static bool IsFleeing(AIHeroClient hero)
        {
            var position = E.GetPrediction(hero);
            return position != null &&
                   Vector3.DistanceSquared(ObjectManager.Player.Position, position.CastPosition) >
                   Vector3.DistanceSquared(hero.Position, position.CastPosition);
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {
            return true;
        }
    }
}
