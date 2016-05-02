namespace Khappa_Zix.Misc
{
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    using Load;

    internal class Misc
    {
        public static HitChance hitchance;

        public static void HitChances()
        {
            switch (menu.Misc["hitChance"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    {
                        hitchance = HitChance.High;
                    }
                    break;

                case 1:
                    {
                        hitchance = HitChance.Medium;
                    }
                    break;

                case 2:
                    {
                        hitchance = HitChance.Low;
                    }
                    break;
            }
        }
    }
}