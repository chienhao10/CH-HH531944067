using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;
using SettingsMisc = Twitch.Config.MiscMenu;
using SettingsDrawing = Twitch.Config.DrawingMenu;

namespace Twitch
{
    public static class Events
    {
        private static Circle QCircle;

        static Events()
        {
            var QColor = new ColorBGRA(Color.GreenYellow.ToVector3(), 0.1f);
            QCircle = new Circle(QColor, 500.0f, 3F, true);

            Gapcloser.OnGapcloser += GapcloserOnOnGapcloser;
            Spellbook.OnCastSpell += OnRecall;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnRecall(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            // Stealth Recall
            if (SettingsMisc.StealthRecall && args.Slot == SpellSlot.Recall && !SpellManager.QActive && SpellManager.Q.IsReady() && !Player.Instance.IsInShopRange())
            {
                SpellManager.Q.Cast();
            }
        }

        private static void GapcloserOnOnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var W = SpellManager.W;
            if (SettingsMisc.GapcloserUseW && sender.IsEnemy && W.IsReady() && sender.IsValidTarget() && e.End.Distance(Player.Instance) <= 200.0f)
            {
                var pred = W.GetPrediction(sender);
                if (pred.HitChance >= HitChance.Medium)
                {
                    W.Cast(pred.CastPosition);
                    Debug.WriteChat("Casting W in AntiGapcloser, Target: {0}", sender.ChampionName);
                }
            }
        }

        public static void Initialize()
        {

        }

        private static void OnDraw(EventArgs args)
        {
            if (SettingsDrawing.DrawQ)
            {
                var QBuff = Player.Instance.GetBuff("TwitchHideInShadows");
                if (QBuff != null)
                {
                    QCircle.Radius = Player.Instance.MoveSpeed * (QBuff.EndTime - Game.Time) + Player.Instance.BoundingRadius;
                    QCircle.Draw(Player.Instance.Position);
                }
            }
            if (SettingsDrawing.DrawW)
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.W.IsReady()))
                {
                    Circle.Draw(Color.LightGreen, SpellManager.W.Range, Player.Instance.Position);
                }
            }
            if (SettingsDrawing.DrawE)
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.E.IsReady()))
                {
                    Circle.Draw(Color.DarkGreen, SpellManager.E.Range, Player.Instance.Position);
                }
            }
            if (SettingsDrawing.DrawR)
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.R.IsReady()))
                {
                    Circle.Draw(Color.Orange, SpellManager.R.Range, Player.Instance.Position);
                }
            }
            if (SettingsDrawing.DrawIgnite && SpellManager.HasIgnite())
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.Ignite.IsReady()))
                {
                    Circle.Draw(Color.Red, SpellManager.Ignite.Range, Player.Instance.Position);
                }
            }
        }
    }
}
