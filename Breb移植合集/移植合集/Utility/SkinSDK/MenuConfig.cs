

using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SDK_SkinChanger
{
    class MenuConfig
    {
        private const string MenuName = "SDK SkinChanger";
        public static Menu MainMenu { get; set; } = EloBuddy.SDK.Menu.MainMenu.AddMenu(MenuName, MenuName);

        public static void Load()
        {
            SkinMenu = MainMenu.AddSubMenu("SkinChanger", "SkinChanger");
            SkinMenu.Add("Skins", new ComboBox("Skins", 0, "Default", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" ));
            SkinChanger = SkinMenu["Skins"].Cast<ComboBox>().CurrentValue;
            SkinMenu["Skins"].Cast<ComboBox>().OnValueChange += MenuConfig_OnValueChange;
        }

        public static AIHeroClient Player => ObjectManager.Player;

        private static void MenuConfig_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            Player.SetSkin(Player.CharData.BaseSkinName, args.NewValue);
        }

        public static Menu SkinMenu;
        public static int SkinChanger;
    }
}
