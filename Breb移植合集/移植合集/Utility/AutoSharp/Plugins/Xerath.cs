using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utils = LeagueSharp.Common.Utils;

namespace AutoSharp.Plugins
{
    class Xerath : PluginBase
    {
        //Spells
        public static List<LeagueSharp.Common.Spell> SpellList = new List<LeagueSharp.Common.Spell>();
        
        private bool AttacksEnabled
        {
            get
            {
                if (IsCastingR)
                    return false;

                if (Q.IsCharging)
                    return false;

                return IsPassiveUp || (!Q.IsReady() && !W.IsReady() && !E.IsReady());
            }
        }

        public bool IsPassiveUp
        {
            get { return ObjectManager.Player.HasBuff("xerathascended2onhit"); }
        }

        public bool IsCastingR
        {
            get
            {
                return ObjectManager.Player.HasBuff("XerathLocusOfPower2") ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        LeagueSharp.Common.Utils.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
            }
        }

        public class RCharge
        {
            public static int CastT;
            public static int Index;
            public static Vector3 Position;
            public static bool TapKeyPressed;
        }

        public override void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampionName) return;

            //Create the spells
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1550);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1150);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //Add the events we are going to use:
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Game.OnWndProc += Game_OnWndProc;
            Orbwalker.OnPreAttack += OrbwalkingOnBeforeAttack;
            EloBuddy.Player.OnIssueOrder += AIHeroClient_OnIssueOrder;
        }

        void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {                  
            if (Player.LSDistance(sender) < E.Range)
            {
                E.Cast(sender);
            }
        }

        void AIHeroClient_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (IsCastingR)
            {
                args.Process = false;
            }
        }

        private void OrbwalkingOnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            args.Process = AttacksEnabled;
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            if (Player.LSDistance(gapcloser.Sender) < E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYUP)
                RCharge.TapKeyPressed = true;
        }

        void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "XerathLocusOfPower2")
                {
                    RCharge.CastT = 0;
                    RCharge.Index = 0;
                    RCharge.Position = new Vector3();
                    RCharge.TapKeyPressed = false;
                }
                else if (args.SData.Name == "xerathlocuspulse")
                {
                    RCharge.CastT = LeagueSharp.Common.Utils.TickCount;
                    RCharge.Index++;
                    RCharge.Position = args.End;
                    RCharge.TapKeyPressed = false;
                }
            }
        }

        private void Combo()
        {
            if (R.IsReady() && Heroes.EnemyHeroes.Any(h => h.HealthPercent < 30)) R.Cast();
            UseSpells(true, true, true);
        }

        private void UseSpells(bool useQ, bool useW, bool useE)
        {
            var qTarget = TargetSelector.GetTarget(Q.ChargedMaxRange, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range + W.Width * 0.5f, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            //Hacks.DisableCastIndicator = Q.IsCharging && useQ;

            if (eTarget != null && useE && E.IsReady())
            {
                if (Player.LSDistance(eTarget) < E.Range * 0.4f)
                    E.Cast(eTarget);
                else if ((!useW || !W.IsReady()))
                    E.Cast(eTarget);
            }

            if (useQ && Q.IsReady() && qTarget != null)
            {
                if (Q.IsCharging)
                {
                    Q.Cast(qTarget, false, false);
                }
                else if (!useW || !W.IsReady() || Player.LSDistance(qTarget) > W.Range)
                {
                    Q.StartCharging();
                }
            }

            if (wTarget != null && useW && W.IsReady())
                W.Cast(wTarget, false, true);
        }

        private AIHeroClient GetTargetNearMouse(float distance)
        {
            AIHeroClient bestTarget = null;
            var bestRatio = 0f;

            if (TargetSelector.SelectedTarget.LSIsValidTarget() && !TargetSelector.SelectedTarget.IsInvulnerable &&
                (Game.CursorPos.LSDistance(TargetSelector.SelectedTarget.ServerPosition) < distance && ObjectManager.Player.LSDistance(TargetSelector.SelectedTarget) < R.Range))
            {
                return TargetSelector.SelectedTarget;
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (!hero.LSIsValidTarget(R.Range) || hero.IsInvulnerable || Game.CursorPos.LSDistance(hero.ServerPosition) > distance)
                {
                    continue;
                }

                var damage = (float)ObjectManager.Player.CalcDamage(hero, DamageType.Magical, 100);
                var ratio = damage / (1 + hero.Health) * TargetSelector.GetPriority(hero);

                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    bestTarget = hero;
                }
            }

            return bestTarget;
        }

        private void WhileCastingR()
        {
            var rTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (rTarget != null)
            {
                //Wait at least 0.6f if the target is going to die or if the target is to far away
                if (rTarget.Health - R.GetDamage(rTarget) < 0)
                    if (LeagueSharp.Common.Utils.TickCount - RCharge.CastT <= 700) return;

                if ((RCharge.Index != 0 && rTarget.LSDistance(RCharge.Position) > 1000))
                    if (LeagueSharp.Common.Utils.TickCount - RCharge.CastT <=
                        Math.Min(2500, rTarget.LSDistance(RCharge.Position) - 1000)) return;

                R.Cast(rTarget, true);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            Orbwalker.DisableMovement = false;

            R.Range = 1850 + R.Level*1050;

            if (IsCastingR)
            {
                Orbwalker.DisableMovement = true;
                WhileCastingR();
                return;
            }
            Combo();
        }
    }
}
