namespace KappaUtility.Misc
{
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    using Trackers;

    internal class Surrender
    {
        internal static void OnLoad()
        {
            Obj_AI_Base.OnSurrender += Obj_AI_Base_OnSurrender;
        }

        private static void Obj_AI_Base_OnSurrender(Obj_AI_Base sender, Obj_AI_BaseSurrenderVoteEventArgs args)
        {
            if (sender == null || args == null)
            {
                return;
            }

            if (sender.IsAlly && Tracker.TrackMenu["Trackally"].Cast<CheckBox>().CurrentValue)
            {
                if (args.Type == SurrenderVoteType.Yes)
                {
                    Chat.Print("[Ally] " + sender.BaseSkinName + " Voted Yes On Surrender");
                }

                if (args.Type == SurrenderVoteType.No)
                {
                    Chat.Print("[Ally] " + sender.BaseSkinName + " Voted No On Surrender");
                }
            }

            if (sender.IsEnemy && Tracker.TrackMenu["Trackenemy"].Cast<CheckBox>().CurrentValue)
            {
                if (args.Type == SurrenderVoteType.Yes)
                {
                    Chat.Print("[Enemy] " + sender.BaseSkinName + " Voted Yes On Surrender");
                }

                if (args.Type == SurrenderVoteType.No)
                {
                    Chat.Print("[Enemy] " + sender.BaseSkinName + " Voted No On Surrender");
                }
            }
        }
    }
}