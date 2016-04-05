namespace KappaUtility.Items
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Potions
    {
        public static Menu PotMenu { get; private set; }

        public static readonly Item Corrupting = new Item(ItemId.Corrupting_Potion);

        public static readonly Item Health = new Item(ItemId.Health_Potion);

        public static readonly Item Hunters = new Item(ItemId.Hunters_Potion);

        public static readonly Item Refillable = new Item((int)ItemId.Refillable_Potion);

        public static readonly Item Biscuit = new Item((int)ItemId.Total_Biscuit_of_Rejuvenation);

        internal static void OnLoad()
        {
            PotMenu = Load.UtliMenu.AddSubMenu("药水");
            PotMenu.AddGroupLabel("自动喝药设置");
            PotMenu.Add("CP", new CheckBox("腐蚀药水", false));
            PotMenu.Add("CPH", new Slider("血量为 X %时使用", 65, 0, 100));
            PotMenu.Add("HP", new CheckBox("红药", false));
            PotMenu.Add("HPH", new Slider("血量为 X %时使用", 45, 0, 100));
            PotMenu.Add("HPS", new CheckBox("猎人药水", false));
            PotMenu.Add("HPSH", new Slider("血量为 X %时使用", 75, 0, 100));
            PotMenu.Add("RP", new CheckBox("可充药水", false));
            PotMenu.Add("RPH", new Slider("血量为 X %时使用", 50, 0, 100));
            PotMenu.Add("BP", new CheckBox("饼干", false));
            PotMenu.Add("BPH", new Slider("血量为 X %时使用", 40, 0, 100));
        }
    }
}