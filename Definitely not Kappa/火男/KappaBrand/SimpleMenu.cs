namespace KappaBrand
{
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal static class SimpleMenu
    {
        public static int combobox(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        public static int slider(this Menu m, string id)
        {
            return m[id].Cast<Slider>().CurrentValue;
        }

        public static bool checkbox(this Menu m, string id)
        {
            return m[id].Cast<CheckBox>().CurrentValue;
        }

        public static bool keybind(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        public static bool hide(this ComboBox m, ComboBox m2)
        {
            switch (m2.CurrentValue)
            {
                case 0:
                    {
                        return m.IsVisible = true;
                    }
            }
            return m.IsVisible = false;
        }

        public static bool hide(this CheckBox m, ComboBox m2)
        {
            switch (m2.CurrentValue)
            {
                case 0:
                    {
                        return m.IsVisible = true;
                    }
            }
            return m.IsVisible = false;
        }
    }
}