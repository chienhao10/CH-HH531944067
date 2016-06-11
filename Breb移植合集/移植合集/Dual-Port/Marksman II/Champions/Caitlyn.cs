#region

using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Common;
using SharpDX;
using Color = System.Drawing.Color;


#endregion

namespace Marksman.Champions
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using System.Linq;

    using Utils = LeagueSharp.Common.Utils;

    internal class Caitlyn : Champion
    {
        public static LeagueSharp.Common.Spell R;

        public LeagueSharp.Common.Spell E;

        public static LeagueSharp.Common.Spell Q;

        public bool ShowUlt;

        public string UltTarget;

        public static LeagueSharp.Common.Spell W;

        private bool canCastR = true;

        private string[] dangerousEnemies = new[] {"Zed", "Fizz", "Rengar", "JarvanIV", "Irelia", "Amumu", "DrMundo", "Ryze"};

        public Caitlyn()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1240);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 820);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 800);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnEndScene += DrawingOnOnEndScene;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
                {
                    if (W.IsReady())
                    {
                        BuffInstance aBuff =
                            (from fBuffs in
                                 sender.Buffs.Where(
                                     s =>
                                     sender.Team != ObjectManager.Player.Team
                                     && sender.LSDistance(ObjectManager.Player.Position) < W.Range)
                             from b in new[]
                                           {
                                               "teleport", /* Teleport */ "pantheon_grandskyfall_jump", /* Pantheon */ 
                                               "crowstorm", /* FiddleScitck */
                                               "zhonya", "katarinar", /* Katarita */
                                               "MissFortuneBulletTime", /* MissFortune */
                                               "gate", /* Twisted Fate */
                                               "chronorevive" /* Zilean */
                                           }
                             where args.Buff.Name.ToLower().Contains(b)
                             select fBuffs).FirstOrDefault();

                        if (aBuff != null)
                        {
                            W.Cast(sender.Position);
                        }
                    }
                };

            Marksman.Utils.Utils.PrintMessage("Caitlyn loaded.");
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.misc["Misc.AntiGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (E.IsReady() && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                //E.CastOnUnit(gapcloser.Sender);
            }
        }

        public void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender is Obj_AI_Turret && args.Target.IsMe)
            {
                canCastR = false;
            }
            else
            {
                canCastR = true;
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var enemies = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range * 2));
            IEnumerable<AIHeroClient> nResult =
                (from e in enemies join d in dangerousEnemies on e.ChampionName equals d select e)
                    .Distinct();

            foreach (var n in nResult)
            {
                Render.Circle.DrawCircle(n.Position, n.AttackRange * 3, Color.GreenYellow);
            }

            LeagueSharp.Common.Spell[] spellList = { Q, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 0, 255));
            }

            var drawUlt = Program.marksmanDrawings["DrawUlt"].Cast<CheckBox>().CurrentValue;
            if (drawUlt && ShowUlt)
            {
                //var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
                //Drawing.DrawText(playerPos.X - 65, playerPos.Y + 20, drawUlt.Color, "Hit R To kill " + UltTarget + "!");
            }
        }

        static void CastQ(Obj_AI_Base t)
        {
            if (t.LSIsValidTarget() && Q.IsReady() && ObjectManager.Player.LSDistance(t.ServerPosition) <= Q.Range)
            {
                if (Q.CastIfHitchanceEquals(t, Q.GetHitchance()))
                {
                    Q.Cast(t);
                }
                return;
            }
        }

        static void CastW(Obj_AI_Base t)
        {
            if (t.LSIsValidTarget(W.Range))
            {
                BuffType[] buffList =
                {
                    BuffType.Fear,
                    BuffType.Taunt,
                    BuffType.Stun,
                    BuffType.Slow,
                    BuffType.Snare
                };

                foreach (var b in buffList.Where(t.HasBuffOfType))
                {
                    W.Cast(t.Position);
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            var rCircle2 = Program.marksmanDrawings["Draw.UltiMiniMap"].Cast<CheckBox>().CurrentValue;
            if (rCircle2)
            {
#pragma warning disable 618
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.FromArgb(255, 255, 255, 255), 1, 23, true);
#pragma warning restore 618
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            Console.WriteLine(Q.GetHitchance().ToString());
            R.Range = 500 * (R.Level == 0 ? 1 : R.Level) + 1500;

            AIHeroClient t;

            if (W.IsReady() && Program.misc["AutoWI"].Cast<CheckBox>().CurrentValue)
            {
                t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (t.LSIsValidTarget(W.Range)
                    && (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare)
                        || t.HasBuffOfType(BuffType.Taunt) || t.HasBuff("zhonyasringshield") || t.HasBuff("Recall")))
                {
                    W.Cast(t.Position);
                }
            }

            if (Q.IsReady() && Program.misc["AutoQI"].Cast<CheckBox>().CurrentValue)
            {
                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t.LSIsValidTarget(Q.Range)
                    && (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Taunt) && (t.Health <= ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q)
                            || !Orbwalking.InAutoAttackRange(t))))
                {
                    CastQ(t);
                    //Q.Cast(t, false, true);
                }
            }

            if (R.IsReady())
            {
                t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget(R.Range) && t.Health <= R.GetDamage(t))
                {
                    if (Program.misc["UltHelp"].Cast<KeyBind>().CurrentValue && canCastR) R.Cast(t);
                    UltTarget = t.ChampionName;
                    ShowUlt = true;
                }
                else
                {
                    ShowUlt = false;
                }
            }
            else
            {
                ShowUlt = false;
            }


            {
                var pos = ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(Game.CursorPos.LSTo2D(), -300).To3D();
                //E.Cast(pos, true);
            }

            if (Program.misc["UseEQC"].Cast<KeyBind>().CurrentValue && E.IsReady() && Q.IsReady())
            {
                t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.LSIsValidTarget(E.Range)
                    && t.Health
                    < ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q)
                    + ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.E) + 20 && E.CanCast(t))
                {
                    //E.Cast(t);
                    CastQ(t);
                }
            }

            // PQ you broke it D:
            if ((!ComboActive && !HarassActive) || !Orbwalker.CanMove)
            {
                return;
            }

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
            var useE = Program.combo["UseEC"].Cast<CheckBox>().CurrentValue;
            var useR = Program.combo["UseRC"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ)
            {
                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t != null)
                {
                    CastQ(t);
                }
            }

            if (E.IsReady() && useE)
            {
                var enemies = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range));
                IEnumerable<AIHeroClient> nResult =
                    (from e in enemies join d in dangerousEnemies on e.ChampionName equals d select e)
                        .Distinct();

                foreach (var n in nResult)
                {
                    if (n.LSIsValidTarget(n.AttackRange * 3) && E.GetPrediction(n).CollisionObjects.Count == 1)
                    {
                        E.Cast(n.Position);
                        if (W.IsReady())
                            W.Cast(n.Position);
                    }
                }
            }

            if (R.IsReady() && useR)
            {
                t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t != null && t.Health <= R.GetDamage(t) && !Orbwalking.InAutoAttackRange(t) && canCastR)
                {
                    R.CastOnUnit(t);
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive)) return;

            var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;

            if (useQ) Q.Cast(t, false, true);
            base.Orbwalking_AfterAttack(target, args);
        }

        public override bool MainMenu(Menu config)
        {
            return base.MainMenu(config);
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Use Q"));
            config.Add("UseEC", new CheckBox("Use E"));
            config.Add("UseRC", new CheckBox("Use R"));

            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox(Marksman.Utils.Utils.Tab + "Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawE", new CheckBox(Marksman.Utils.Utils.Tab + "E range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawR", new CheckBox(Marksman.Utils.Utils.Tab + "R range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawUlt", new CheckBox(Marksman.Utils.Utils.Tab + "Ult Text", false));//.SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            config.Add("Draw.UltiMiniMap", new CheckBox(Marksman.Utils.Utils.Tab + "Draw Ulti Minimap", true));//.SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Misc.AntiGapCloser", new CheckBox("E Anti Gap Closer"));
            config.Add("UltHelp", new KeyBind("Ult Target on R", false, KeyBind.BindTypes.HoldActive, 'R'));
            config.Add("UseEQC", new KeyBind("Use E-Q Combo", false, KeyBind.BindTypes.HoldActive, 'T'));
            config.Add("Dash", new KeyBind("Dash to Mouse", false, KeyBind.BindTypes.HoldActive, 'Z'));
            config.Add("AutoQI", new CheckBox("Auto Q (Stun/Snare/Taunt/Slow)"));
            config.Add("AutoWI", new CheckBox("Auto W (Stun/Snare/Taunt)"));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            return true;
        }

        public override void ExecuteFlee()
        {
            if (E.IsReady())
            {
                var pos = Vector3.Zero;
                var enemy =
                    HeroManager.Enemies.FirstOrDefault(
                        e =>
                            e.LSIsValidTarget(E.Range +
                                            (ObjectManager.Player.MoveSpeed > e.MoveSpeed
                                                ? ObjectManager.Player.MoveSpeed - e.MoveSpeed
                                                : e.MoveSpeed - ObjectManager.Player.MoveSpeed)) && E.CanCast(e));

                pos = enemy?.Position ??
                      ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(Game.CursorPos.LSTo2D(), -300).To3D();
                //E.Cast(pos);
            }

            base.PermaActive();
        }
    }
}
