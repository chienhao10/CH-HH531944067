namespace KappaBrand
{
    using EloBuddy.SDK;

    using SharpDX;

    internal static class Colors
    {
        public static Color Select(Spell.SpellBase slot)
        {
            switch (Brand.DrawMenu.combobox(slot.Name))
            {
                case 0:
                    {
                        return Color.Chartreuse;
                    }
                case 1:
                    {
                        return Color.BlueViolet;
                    }
                case 2:
                    {
                        return Color.Aqua;
                    }
                case 3:
                    {
                        return Color.Purple;
                    }
                case 4:
                    {
                        return Color.White;
                    }
                case 5:
                    {
                        return Color.Orange;
                    }
                case 6:
                    {
                        return Color.Green;
                    }
            }
            return Color.White;
        }
    }
}