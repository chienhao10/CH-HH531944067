namespace KappaEkko.Modes
{
    using EloBuddy.SDK.Menu.Values;

    using Logics;

    internal class PermaActive
    {
        public static void Active()
        {
            var Qstacks = Menu.MiscMenu["Qstacks"].Cast<CheckBox>().CurrentValue;
            var RAoe = Menu.UltMenu["RAoe"].Cast<CheckBox>().CurrentValue;
            var REscape = Menu.UltMenu["REscape"].Cast<CheckBox>().CurrentValue;

            if (Spells.R.IsReady() && Spells.EkkoREmitter != null)
            {
                if (RAoe)
                {
                    Rlogic.Aoe();
                }

                if (REscape)
                {
                    Rlogic.Escape();
                }
            }

            if (Spells.Q.IsReady())
            {
                if (Qstacks)
                {
                    Qlogic.OnStacks();
                }
            }
        }
    }
}