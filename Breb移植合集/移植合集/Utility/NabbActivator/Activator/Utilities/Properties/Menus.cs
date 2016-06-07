using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using ExorSDK.Utilities;

namespace NabbActivator
{
    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        /// <summary>
        ///     Sets the menu.
        /// </summary>
        public static void Initialize()
        {
            Vars.Menu = MainMenu.AddMenu("NabbActivator", "activator");


            /// <summary>
            ///     Sets the menu for the Q.
            /// </summary>
            Vars.SmiteMiscMenu = Vars.Menu.AddSubMenu("Smite", "smite");
            {
                Vars.SmiteMiscMenu.Add("combo", new CheckBox("Combo", true));
                Vars.SmiteMiscMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.SmiteMiscMenu.Add("stacks", new CheckBox("Keep 1 Stack for Dragon/Baron/Herald", true));
                Vars.SmiteMiscMenu.Add("limit", new CheckBox("Only on Dragon/Baron/Herald", true));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings", "drawings");
            {
                Vars.DrawingsMenu.Add("range", new CheckBox("Smite Range"));
                Vars.DrawingsMenu.Add("damage", new CheckBox("Smite Damage"));

            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.SliderMenu = Vars.Menu.AddSubMenu("Consumables Menu", "consumables");
            {
                Vars.SliderMenu.Add("health", new Slider("Consumables: Health < x%", 50, 10, 100));
                Vars.SliderMenu.Add("mana", new Slider("Consumables: Mana < x%", 50, 10, 100));
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.KeysMenu = Vars.Menu.AddSubMenu("Keybinds Menu", "keys");
            {

                Vars.KeysMenu.Add("combo", new KeyBind("Combo", false, KeyBind.BindTypes.HoldActive, 32));
                Vars.KeysMenu.Add("laneclear", new KeyBind("LaneClear:", false, KeyBind.BindTypes.HoldActive, 'V'));
                Vars.KeysMenu.Add("smite", new KeyBind("Use Smite(toggle)", false, KeyBind.BindTypes.PressToggle, 'Y'));

            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.TypesMenu = Vars.Menu.AddSubMenu("Types Menu", "types");
            {
                Vars.TypesMenu.Add("offensives", new CheckBox("Offensives Damage"));
                Vars.TypesMenu.Add("defensives", new CheckBox("Defensives"));
                Vars.TypesMenu.Add("spells", new CheckBox("Spells"));
                Vars.TypesMenu.Add("cleansers", new CheckBox("Cleansers"));
                Vars.TypesMenu.Add("potions", new CheckBox("Potions"));
                Vars.TypesMenu.Add("resetters", new CheckBox("Tiamat/Hydra/Titanic"));
                Vars.TypesMenu.Add("randomizer", new CheckBox("Humanizer"));
            }
        }
    }
}
