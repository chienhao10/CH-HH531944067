namespace KappaUtility.Trackers
{
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

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
                    Chat.Print("[Ally] " + sender.BaseSkinName + " 选择了投降", System.Drawing.Color.Green);
                }

                if (args.Type == SurrenderVoteType.No)
                {
                    Chat.Print("[Ally] " + sender.BaseSkinName + " 没选择投降", System.Drawing.Color.Green);
                }
            }

            if (sender.IsEnemy && Tracker.TrackMenu["Trackenemy"].Cast<CheckBox>().CurrentValue)
            {
                if (args.Type == SurrenderVoteType.Yes)
                {
                    Chat.Print("[Enemy] " + sender.BaseSkinName + " 选择了投降", System.Drawing.Color.Red);
                }

                if (args.Type == SurrenderVoteType.No)
                {
                    Chat.Print("[Enemy] " + sender.BaseSkinName + " 没选择投降", System.Drawing.Color.Red);
                }
            }
        }
    }
}