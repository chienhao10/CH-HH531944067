using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Plugins
{
    public class Heimerdinger : PluginBase
    {
        public Vector2 Pos;
        public bool Rcast;

        public Heimerdinger()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 525);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1100);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 925);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 100);


            W.SetSkillshot(250f, 200, 1400, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.51f, 120, 1200, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (R.IsReady() && !Rcast)
                {
                    R.Cast();
                    Rcast = true;
                }
                if (W.IsReady() && Target.LSIsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(Target);
                    W.Cast(pred.CastPosition);
                    Rcast = false;
                }
                if (E.IsReady() && Target.LSIsValidTarget(E.Range))
                {
                    var pred = E.GetPrediction(Target);
                    E.Cast(pred.CastPosition);
                    Rcast = false;
                }
                if (Q.IsReady() && Player.LSCountEnemiesInRange(1300) >= 2)
                {
                    var rnd = new Random();
                    Pos.X = Player.Position.X + rnd.Next(-50, 50);
                    Pos.Y = Player.Position.Y + rnd.Next(-50, 50);
                    Q.Cast(Pos.To3D());
                    Rcast = false;
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