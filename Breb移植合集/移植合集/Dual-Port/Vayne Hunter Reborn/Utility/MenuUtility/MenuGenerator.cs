using System;
using System.Drawing;
using LeagueSharp.Common;
using VayneHunter_Reborn.External;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace VayneHunter_Reborn.Utility.MenuUtility
{
    class MenuGenerator
    {

        public static Menu comboMenu, harassMenu, farmMenu, miscMenu, drawMenu;

        public static void OnLoad()
        {
            var RootMenu = Variables.Menu;

            comboMenu = RootMenu.AddSubMenu("[VHR] Combo", "dz191.vhr.combo");
            comboMenu.AddGroupLabel("Mana Manager");
            comboMenu.AddManaLimiter(Enumerations.Skills.Q, "combo");
            comboMenu.AddManaLimiter(Enumerations.Skills.E, "combo");
            comboMenu.AddManaLimiter(Enumerations.Skills.R, "combo");
            comboMenu.AddSeparator();
            comboMenu.AddSkill(Enumerations.Skills.Q, "combo");
            comboMenu.AddSkill(Enumerations.Skills.E, "combo");
            comboMenu.AddSkill(Enumerations.Skills.R, "combo", false);
            comboMenu.AddSeparator();
            comboMenu.Add("dz191.vhr.combo.r.minenemies", new Slider("Min. R Enemies", 2, 1, 5));
            comboMenu.Add("dz191.vhr.combo.q.2wstacks", new CheckBox("Only Q if 2W Stacks on Target", false));

            harassMenu = RootMenu.AddSubMenu("[VHR] Harass", "dz191.vhr.mixed");
            harassMenu.AddGroupLabel("Mana Manager");
            harassMenu.AddManaLimiter(Enumerations.Skills.Q, "harass");
            harassMenu.AddManaLimiter(Enumerations.Skills.E, "harass");
            harassMenu.AddSeparator();
            harassMenu.AddSkill(Enumerations.Skills.Q, "harass");
            harassMenu.AddSkill(Enumerations.Skills.E, "harass");
            harassMenu.AddSeparator();
            harassMenu.Add("dz191.vhr.mixed.q.2wstacks", new CheckBox("Only Q if 2W Stacks on Target"));
            harassMenu.Add("dz191.vhr.mixed.ethird", new CheckBox("Use E for Third Proc"));

            farmMenu = RootMenu.AddSubMenu("[VHR] Farm", "dz191.vhr.farm");
            farmMenu.AddSkill(Enumerations.Skills.Q, "laneclear");
            farmMenu.AddManaLimiter(Enumerations.Skills.Q, "laneclear", 45, true);
            farmMenu.AddSeparator();
            farmMenu.AddSkill(Enumerations.Skills.Q, "lasthit");
            farmMenu.AddManaLimiter(Enumerations.Skills.Q, "lasthit", 45, true);
            farmMenu.AddSeparator();
            farmMenu.Add("dz191.vhr.farm.condemnjungle", new CheckBox("Use E to condemn jungle mobs", true));
            farmMenu.Add("dz191.vhr.farm.qjungle", new CheckBox("Use Q against jungle mobs", true));

            miscMenu = RootMenu.AddSubMenu("[VHR] Misc", "dz191.vhr.misc");
            miscMenu.AddGroupLabel("Misc - Q (Tumble)");
            miscMenu.Add("dz191.vhr.misc.condemn.qlogic", new ComboBox("Q Logic", 0, "Reborn", "Normal", "Kite melees", "Kurisu"));
            miscMenu.Add("dz191.vhr.mixed.mirinQ", new CheckBox("Q to Wall when Possible (Mirin Mode)", true));
            miscMenu.Add("dz191.vhr.misc.tumble.smartq", new CheckBox("Try to QE when possible"));
            miscMenu.Add("dz191.vhr.misc.tumble.noaastealthex", new KeyBind("Don't AA while stealthed", false, KeyBind.BindTypes.PressToggle, 'K'));
            miscMenu.Add("dz191.vhr.misc.tumble.noqenemies", new CheckBox("Don't Q into enemies"));
            miscMenu.Add("dz191.vhr.misc.tumble.noqenemies.old", new CheckBox("Use Old Don't Q into enemies"));
            miscMenu.Add("dz191.vhr.misc.tumble.dynamicqsafety", new CheckBox("Use dynamic Q Safety Distance"));
            miscMenu.Add("dz191.vhr.misc.tumble.qspam", new CheckBox("Ignore Q checks"));
            miscMenu.Add("dz191.vhr.misc.tumble.qinrange", new CheckBox("Q For KS", true));
            miscMenu.Add("dz191.vhr.misc.tumble.noaa.enemies", new Slider("Min Enemies for No AA Stealth", 3, 2, 5));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("Misc - E (Condemn)");
            miscMenu.Add("dz191.vhr.misc.condemn.condemnmethod", new ComboBox("Condemn Method", 0, "VH Revolution", "VH Reborn", "Marksman/Gosu", "Shine#"));
            miscMenu.Add("dz191.vhr.misc.condemn.pushdistance", new Slider("E Push Distance", 420, 350, 470));
            miscMenu.Add("dz191.vhr.misc.condemn.accuracy", new Slider("Accuracy (Revolution Only)", 45, 1, 65));
            miscMenu.Add("dz191.vhr.misc.condemn.enextauto", new KeyBind("E Next Auto", false, KeyBind.BindTypes.PressToggle, 'T'));
            miscMenu.Add("dz191.vhr.misc.condemn.onlystuncurrent", new CheckBox("Only stun current target"));
            miscMenu.Add("dz191.vhr.misc.condemn.autoe", new CheckBox("Auto E"));
            miscMenu.Add("dz191.vhr.misc.condemn.eks", new CheckBox("Smart E KS"));
            miscMenu.Add("dz191.vhr.misc.condemn.noeaa", new Slider("Don't E if Target can be killed in X AA", 1, 0, 4));
            miscMenu.Add("dz191.vhr.misc.condemn.trinketbush", new CheckBox("Trinket Bush on Condemn", true));
            miscMenu.Add("dz191.vhr.misc.condemn.lowlifepeel", new CheckBox("Peel with E when low health"));
            miscMenu.Add("dz191.vhr.misc.condemn.condemnflag", new CheckBox("Condemn to J4 flag", true));
            miscMenu.Add("dz191.vhr.misc.condemn.noeturret", new CheckBox("No E Under enemy turret"));
            miscMenu.Add("dz191.vhr.misc.condemn.repelflash", new CheckBox("Use E on Enemy Flashes"));
            miscMenu.Add("dz191.vhr.misc.condemn.repelkindred", new CheckBox("Use E to push enemies out of kindred ult"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("Misc - General");
            miscMenu.Add("dz191.vhr.misc.general.antigp", new CheckBox("Anti Gapcloser"));
            miscMenu.Add("dz191.vhr.misc.general.interrupt", new CheckBox("Interrupter", true));
            miscMenu.Add("dz191.vhr.misc.general.antigpdelay", new Slider("Anti Gapcloser Delay (ms)", 0, 0, 1000));
            miscMenu.Add("dz191.vhr.misc.general.specialfocus", new CheckBox("Focus targets with 2 W marks"));
            miscMenu.Add("dz191.vhr.misc.general.reveal", new CheckBox("Stealth Reveal (Pink Ward / Lens)"));
            miscMenu.Add("dz191.vhr.misc.general.disablemovement", new CheckBox("Disable Orbwalker Movement", false));
            miscMenu.Add("dz191.vhr.misc.general.disableattk", new CheckBox("Disable Orbwalker Attack", false));

            drawMenu = RootMenu.AddSubMenu("[VHR] Drawings", "dz191.vhr.draw");
            drawMenu.Add("dz191.vhr.draw.spots", new CheckBox("Draw Spots", false));
            drawMenu.Add("dz191.vhr.draw.range", new CheckBox("Draw Enemy Ranges", false));
            drawMenu.Add("dz191.vhr.draw.condemn", new CheckBox("Draw Condemn Rectangles", false));
            drawMenu.Add("dz191.vhr.draw.qpos", new CheckBox("Reborn Q Position (Debug)", false));

            CustomAntigapcloser.BuildMenu(RootMenu);
        }
    }
}
