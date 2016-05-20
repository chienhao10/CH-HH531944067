namespace KappaUtility.Common
{
    using System.Collections.Generic;

    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    public static class SimpleMenu
    {
        public static void Checkbox(this Menu Menu, string id, string name, bool value = false)
        {
            Menu.Add(id, new CheckBox(name, value));
        }

        public static bool GetCheckbox(this Menu Menu, string id)
        {
            return Menu[id].Cast<CheckBox>().CurrentValue;
        }

        public static void Slider(this Menu Menu, string id, string name, int d = 0, int min = 0, int max = 100)
        {
            Menu.Add(id, new Slider(name, d, min, max));
        }

        public static int GetSlider(this Menu Menu, string id)
        {
            return Menu[id].Cast<Slider>().CurrentValue;
        }

        public static void ComboBox(this Menu Menu, string id, string name, List<string> options, int d = 0)
        {
            Menu.Add(id, new ComboBox(name, options, d));
        }

        public static int GetComboBox(this Menu Menu, string id)
        {
            return Menu[id].Cast<ComboBox>().CurrentValue;
        }
    }
}