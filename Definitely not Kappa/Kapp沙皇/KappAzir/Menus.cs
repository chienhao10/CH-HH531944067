namespace KappAzir
{
    using Mario_s_Lib;

    using System.Collections.Generic;
    using System.Drawing;

    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using System.Linq;

    internal class Menus
    {
        public const string SpellsMenuID = "Spellsmenuid";

        public const string ComboMenuID = "combomenuid";

        public const string FleeMenuID = "fleemenuid";

        public const string HarassMenuID = "harassmenuid";

        public const string AutoHarassMenuID = "autoharassmenuid";

        public const string LaneClearMenuID = "laneclearmenuid";

        public const string LastHitMenuID = "lasthitmenuid";

        public const string JungleClearMenuID = "jungleclearmenuid";

        public const string KillStealMenuID = "killstealmenuid";

        public const string DrawingsMenuID = "drawingsmenuid";

        public const string MiscMenuID = "miscmenuid";

        public static Menu FirstMenu;

        public static Menu SpellsMenu;

        public static Menu ComboMenu;

        public static Menu FleeMenu;

        public static Menu HarassMenu;

        public static Menu AutoHarassMenu;

        public static Menu LaneClearMenu;

        public static Menu LasthitMenu;

        public static Menu JungleClearMenu;

        public static Menu KillStealMenu;

        public static Menu DrawingsMenu;

        public static Menu MiscMenu;

        //These colorslider are from Mario`s Lib
        public static ColorSlide QColorSlide;

        public static ColorSlide WColorSlide;

        public static ColorSlide EColorSlide;

        public static ColorSlide RColorSlide;

        public static ColorSlide DamageIndicatorColorSlide;

        public static void CreateMenu()
        {
            FirstMenu = MainMenu.AddMenu("KappAzir", "KappAzir");
            FleeMenu = FirstMenu.AddSubMenu("• Jumper", FleeMenuID);
            SpellsMenu = FirstMenu.AddSubMenu("• Spells Handler", SpellsMenuID);
            ComboMenu = FirstMenu.AddSubMenu("• Combo", ComboMenuID);
            HarassMenu = FirstMenu.AddSubMenu("• Harass", HarassMenuID);
            AutoHarassMenu = FirstMenu.AddSubMenu("• AutoHarass", AutoHarassMenuID);
            LaneClearMenu = FirstMenu.AddSubMenu("• LaneClear", LaneClearMenuID);
            LasthitMenu = FirstMenu.AddSubMenu("• LastHit", LastHitMenuID);
            JungleClearMenu = FirstMenu.AddSubMenu("• JungleClear", JungleClearMenuID);
            KillStealMenu = FirstMenu.AddSubMenu("• KillSteal", KillStealMenuID);
            MiscMenu = FirstMenu.AddSubMenu("• Misc", MiscMenuID);
            DrawingsMenu = FirstMenu.AddSubMenu("• Drawings", DrawingsMenuID);

            FleeMenu.AddGroupLabel("Jumper - Flee Mode");
            FleeMenu.Add("flee", new KeyBind("Jumper Key", false, KeyBind.BindTypes.HoldActive, 'A'));
            FleeMenu.CreateSlider("Use Soldiers in {0} Range", "range", 1000, 150, 1500);
            FleeMenu.CreateSlider("EQ Delay = {0}", "delay", 250, 150, 500);
            FleeMenu.AddSeparator(0);
            FleeMenu.AddLabel("This is used for the Delay of Casting between E > Q");
            FleeMenu.AddLabel("Used In Insec / Flee Mode");
            FleeMenu.AddSeparator(0);
            FleeMenu.AddGroupLabel("Insec Mode");
            FleeMenu.AddLabel("Select a Target then hold the insec key");
            FleeMenu.Add("insect", new KeyBind("Normal InSec", false, KeyBind.BindTypes.HoldActive, 'S'));
            FleeMenu.Add("insected", new KeyBind("New InSec", false, KeyBind.BindTypes.HoldActive, 'Z'));
            FleeMenu.AddGroupLabel("Normal Insec Settings");
            FleeMenu.CreateCheckBox(" - Push Enemy To Allis", "Ally");
            FleeMenu.CreateCheckBox(" - Push Enemy To Ally Tower", "Tower");
            FleeMenu.AddSeparator(0);
            FleeMenu.AddGroupLabel("New Insec Settings");
            FleeMenu.CreateComboBox("Q Position", "qpos", new List<string> { "To Mouse", "To Old Position", "To Tower", "To Ally" });
            FleeMenu.CreateComboBox("R Position", "rpos", new List<string> { "To Mouse", "To Old Position", "To Tower", "To Ally" });
            FleeMenu.CreateSlider("Lower Q Distance by [{0}]", "dis", 0, 0, 500);

            SpellsMenu.AddGroupLabel("Spells Handler");
            SpellsMenu.CreateCheckBox(" - R Anti GapCloser", "rUseGap");
            SpellsMenu.CreateCheckBox(" - R Interrupter", "rUseInt");
            SpellsMenu.CreateComboBox("Interrupter DangerLevel", "Intdanger", new List<string> { "High", "Medium", "Low" });
            SpellsMenu.AddGroupLabel("Hit Chance");
            SpellsMenu.CreateComboBox("HitChance", "chance", new List<string> { "High", "Medium", "Low" });

            ComboMenu.AddGroupLabel("Spells");
            ComboMenu.CreateCheckBox(" - Use Q", "qUse");
            ComboMenu.CreateCheckBox(" - Use W", "wUse");
            ComboMenu.CreateCheckBox(" - Use E", "eUse");
            ComboMenu.CreateCheckBox(" - Use R", "rUse");
            ComboMenu.CreateSlider("Create Tower If Enemies Near [{0}]", "TowerPls", 2, 1, 5);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Q Extra Settings");
            ComboMenu.CreateCheckBox(" - Use Q If Target Not In Soldiers Range", "qUseAA", false);
            ComboMenu.CreateSlider("Soldier Count to Cast Q [{0}]", "Qcount", 1, 1, 3);
            ComboMenu.CreateSlider("Q AoE hit [{0}]", "Qaoe", 2, 1, 5);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("W Extra Settings");
            ComboMenu.CreateCheckBox(" - Use W If Target Not In Soldiers Range", "wUseAA", false);
            ComboMenu.CreateCheckBox(" - Save 1 W Stack", "wSave", false);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("E Extra Settings");
            ComboMenu.CreateCheckBox(" - Use E Only if target Killable", "eUsekill");
            ComboMenu.CreateCheckBox(" - E Dive Towers", "eUseDive", false);
            ComboMenu.CreateSlider("No E if Target Health more than my health by [{0}%]", "eHealth", 15);
            ComboMenu.CreateSlider("No E if Enemies Near target more than [{0}]", "eSave", 2, 1, 5);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("R Extra Settings");
            ComboMenu.CreateCheckBox(" - R Over Kill Check", "rOverKill");
            ComboMenu.CreateCheckBox(" - Use R Finisher", "rUsekill");
            ComboMenu.CreateCheckBox(" - Use R Saver", "rUseSave", false);
            ComboMenu.CreateCheckBox(" - Push Enemy To Allis", "rUseAlly");
            ComboMenu.CreateCheckBox(" - Push Enemy To Ally Tower", "rUseTower");
            ComboMenu.Add("Rcast", new KeyBind("Semi-Auto R", false, KeyBind.BindTypes.HoldActive, 'R'));
            ComboMenu.CreateSlider("R AoE hit [{0}]", "Raoe", 2, 1, 5);

            HarassMenu.AddGroupLabel("Spells");
            HarassMenu.CreateCheckBox(" - Use Q", "qUse");
            HarassMenu.CreateCheckBox(" - Use W", "wUse");
            HarassMenu.CreateCheckBox(" - Use E", "eUse");
            HarassMenu.AddGroupLabel("Settings");
            HarassMenu.CreateCheckBox(" - Save 1 W Stack", "wSave");
            HarassMenu.CreateCheckBox(" - E Dive Towers", "eUseDive", false);
            HarassMenu.CreateSlider("No E if Enemies Near target more than [{0}]", "eSave", 3, 1, 5);
            HarassMenu.CreateSlider("Mana must be more than [{0}%] to use Harass spells", "manaSlider", 60);

            AutoHarassMenu.AddGroupLabel("Spells");
            AutoHarassMenu.CreateCheckBox(" - Use Q", "qUse");
            AutoHarassMenu.CreateCheckBox(" - Use W", "wUse");
            AutoHarassMenu.CreateCheckBox(" - Use E", "eUse");
            AutoHarassMenu.AddGroupLabel("Settings");
            AutoHarassMenu.CreateCheckBox(" - Save 1 W Stack", "wSave");
            AutoHarassMenu.CreateCheckBox(" - Always AutoAttack with soldiers", "attack", false);
            AutoHarassMenu.CreateCheckBox(" - E Dive Towers", "eDive", false);
            AutoHarassMenu.CreateSlider("No E if Enemies Near target more than [{0}]", "eSave", 3, 1, 5);
            AutoHarassMenu.CreateKeyBind("Enable/Disable AutoHrass", "autoHarassKey", 'Z', 'U');
            AutoHarassMenu.CreateSlider("Mana must be more than [{0}%] to use AutoHarass spells", "manaSlider", 60);

            LaneClearMenu.AddGroupLabel("Spells");
            LaneClearMenu.CreateCheckBox(" - Use Q", "qUse");
            LaneClearMenu.CreateCheckBox(" - Use W", "wUse");
            LaneClearMenu.AddGroupLabel("Settings");
            LaneClearMenu.CreateCheckBox(" - Save 1 W Stack", "wSave");
            LaneClearMenu.CreateCheckBox(" - Use W On Enemy Turret", "wTurret");
            LaneClearMenu.CreateSlider("Mana must be more than [{0}%] to use LaneClear spells", "manaSlider", 75);

            LasthitMenu.AddGroupLabel("Spells");
            LasthitMenu.CreateCheckBox(" - Use Q", "qUse");
            LasthitMenu.AddGroupLabel("Settings");
            LasthitMenu.CreateSlider("Mana must be more than [{0}%] to use LastHit spells", "manaSlider", 75);

            JungleClearMenu.AddGroupLabel("Spells");
            JungleClearMenu.CreateCheckBox(" - Use Q", "qUse");
            JungleClearMenu.CreateCheckBox(" - Use W", "wUse");
            JungleClearMenu.AddGroupLabel("Settings");
            JungleClearMenu.CreateCheckBox(" - Save 1 W Stack", "wSave");
            JungleClearMenu.CreateSlider("Mana must be more than [{0}%] to use JungleClear spells", "manaSlider", 50);

            KillStealMenu.AddGroupLabel("Spells");
            KillStealMenu.CreateCheckBox(" - Use Q", "qUse");
            KillStealMenu.CreateCheckBox(" - Use W", "wUse");
            KillStealMenu.CreateCheckBox(" - Use E", "eUse");
            KillStealMenu.CreateCheckBox(" - Use R", "rUse");

            MiscMenu.AddGroupLabel("Skin Changer");

            var skinList = Mario_s_Lib.DataBases.Skins.SkinsDB.FirstOrDefault(list => list.Champ == Player.Instance.Hero);
            if (skinList != null)
            {
                MiscMenu.CreateComboBox("Choose the skin", "skinComboBox", skinList.Skins);
                MiscMenu.Get<ComboBox>("skinComboBox").OnValueChange +=
                    delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args) { Player.Instance.SetSkinId(sender.CurrentValue); };
            }

            MiscMenu.AddGroupLabel("Auto Level UP");
            MiscMenu.CreateCheckBox("Activate Auto Leveler", "activateAutoLVL", false);
            MiscMenu.AddLabel("The auto leveler will always focus R than the rest of the spells");
            MiscMenu.CreateComboBox("1st Spell to focus", "firstFocus", new List<string> { "Q", "W", "E" });
            MiscMenu.CreateComboBox("2nd Spell to focus", "secondFocus", new List<string> { "Q", "W", "E" }, 1);
            MiscMenu.CreateComboBox("3rd Spell to focus", "thirdFocus", new List<string> { "Q", "W", "E" }, 2);
            MiscMenu.CreateSlider("Delay slider", "delaySlider", 200, 150, 500);

            DrawingsMenu.AddGroupLabel("Setting");
            DrawingsMenu.CreateCheckBox(" - Draw Spell`s range only if they are ready.", "readyDraw");
            DrawingsMenu.CreateCheckBox(" - Draw damage indicator.", "damageDraw");
            DrawingsMenu.CreateCheckBox(" - Draw damage indicator percent.", "perDraw");
            DrawingsMenu.CreateCheckBox(" - Draw damage indicator statistics.", "statDraw", false);
            DrawingsMenu.AddGroupLabel("Spells");
            DrawingsMenu.CreateCheckBox(" - Draw Q.", "qDraw");
            DrawingsMenu.CreateCheckBox(" - Draw W.", "wDraw");
            DrawingsMenu.CreateCheckBox(" - Draw E.", "eDraw");
            DrawingsMenu.CreateCheckBox(" - Draw R.", "rDraw");
            DrawingsMenu.AddGroupLabel("Drawings Color");
            QColorSlide = new ColorSlide(DrawingsMenu, "qColor", Color.Red, "Q Color:");
            WColorSlide = new ColorSlide(DrawingsMenu, "wColor", Color.Purple, "W Color:");
            EColorSlide = new ColorSlide(DrawingsMenu, "eColor", Color.Orange, "E Color:");
            RColorSlide = new ColorSlide(DrawingsMenu, "rColor", Color.DeepPink, "R Color:");
            DamageIndicatorColorSlide = new ColorSlide(DrawingsMenu, "healthColor", Color.YellowGreen, "DamageIndicator Color:");
        }
    }
}