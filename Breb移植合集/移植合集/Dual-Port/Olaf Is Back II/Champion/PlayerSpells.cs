using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace OlafxQx.Champion
{
    internal static class PlayerSpells
    {
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, W, E, R;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 980);
            Q.SetSkillshot(0.20f, 75f, 1500f, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R);

            SpellList.AddRange(new[] {Q, W, E, R});
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static int LastSeen(Obj_AI_Base t)
        {
            return Common.AutoBushHelper.EnemyInfo.Find(x => x.Player.NetworkId == t.NetworkId).LastSeenForE;
        }

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var hero = args.Target as AIHeroClient;
            if (hero != null)
            {
                var t = hero;
                if (!t.LSIsValidTarget(E.Range))
                {
                    return;
                }

                args.Process = Environment.TickCount - LastSeen(t) >=
                               (Modes.ModeSettings.MenuLocal["Settings.SpellCast.VisibleDelay"].Cast<ComboBox>().CurrentValue + 1)*250;
            }
        }

        public static void CastQ(Obj_AI_Base t, float range = 980)
        {
            if (!Q.IsReady() || !t.LSIsValidTarget(range))
            {
                return;
            }

            if (t.HaveOlafSlowBuff() && t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            {
                return;
            }

            
            var nDistance = ObjectManager.Player.LSDistance(t);
            var nExtend = 0;

            if (nDistance < 300)
            {
                nExtend = 40;
            }
            else if (nDistance >= 300 && nDistance < 500)
            {
                nExtend = 60;
            }
            else if (nDistance >= 500 && nDistance < 700)
            {
                nExtend = 80;
            }
            else if (nDistance >= 700 && nDistance < Q.Range)
            {
                nExtend = 100;
            }

            PredictionOutput qPredictionOutput = Q.GetPrediction(t);
            Vector3 castPosition = qPredictionOutput.CastPosition.LSExtend(ObjectManager.Player.Position, -nExtend);
            HitChance[] hitChances = new[]
            {
                HitChance.VeryHigh, HitChance.High, HitChance.Medium, HitChance.Low
            };

            if (qPredictionOutput.Hitchance >=
                (ObjectManager.Player.LSDistance(t.ServerPosition) >= 350
                    ? HitChance.VeryHigh
                    : hitChances[Modes.ModeSettings.QHitchance]) &&
                ObjectManager.Player.LSDistance(castPosition) < range)
            {
                Q.Cast(castPosition);
            }
        }

        public static void CastE(Obj_AI_Base t)
        {
            if (!E.CanCast(t))
            {
                return;
            }

            E.CastOnUnit(t);
        }
    }
}