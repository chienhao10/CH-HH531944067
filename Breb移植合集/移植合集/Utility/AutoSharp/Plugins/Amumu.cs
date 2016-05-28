using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Amumu : PluginBase
    {
        private bool _wUse;

        public Amumu()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            Q.SetSkillshot(
                Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, true,
                SkillshotType.SkillshotLine);


            W = new LeagueSharp.Common.Spell(SpellSlot.W, 300);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 350);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 550);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                var qPred = Q.GetPrediction(Target);

                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(qPred.CastPosition);
                }

                if (W.IsReady() && !_wUse && Player.LSCountEnemiesInRange(R.Range) >= 1)
                {
                    W.Cast();
                    _wUse = true;
                }
                if (_wUse && Player.LSCountEnemiesInRange(R.Range) == 0)
                {
                    W.Cast();
                    _wUse = false;
                }

                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast();
                }

                if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                {
                    R.CastIfWillHit(Target, 2);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}