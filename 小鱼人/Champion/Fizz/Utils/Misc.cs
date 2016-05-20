#region

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SimplisticTemplate.Champion.Fizz.Modes;

#endregion

namespace SimplisticTemplate.Champion.Fizz.Utils
{
    internal static class Misc
    {
        public static bool JumpValid { get; private set; }

        public static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is Obj_AI_Turret && args.Target.IsMe && Fizz.E.IsReady() &&
                GameMenu.MiscMenu["useETower"].Cast<CheckBox>().CurrentValue)
            {
                Fizz.E.Cast(Game.CursorPos);
            }

            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name == "FizzPiercingStrike")
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Core.DelayAction(() => Fizz.W.Cast(),
                        (int) (sender.Spellbook.CastEndTime - Game.Time) + Game.Ping/2 + 250);
                }
                else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                         GameMenu.HarassMenu["useEMode"].Cast<Slider>().CurrentValue == 0)
                {
                    Core.DelayAction(() => { JumpValid = true; },
                        (int) (sender.Spellbook.CastEndTime - Game.Time) + Game.Ping/2 + 250);
                }
            }

            if (args.SData.Name == "fizzjumptwo" || args.SData.Name == "fizzjumpbuffer")
            {
                Harass.LastPos = null;
                JumpValid = false;
            }
        }
    }
}