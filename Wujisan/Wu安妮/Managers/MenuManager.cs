using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace WuAIO.Managers
{
    static class MenuManager
    {
        public static Menu CurrentMainMenu;
        public static Dictionary<string, Dictionary<string, Dictionary<Menu, Dictionary<string, Values>>>> Menus = new Dictionary<string, Dictionary<string, Dictionary<Menu, Dictionary<string, Values>>>>();

        //public List<string> MainMenuNames = new List<string>();

        public static Menu Init(string mainMenuName)
        {
            //MainMenuNames.Add(mainMenuName);

            //Adding the "MainMenu" identifier to the menus list and initializing a new dictionary containing all submenu identifiers that will follow
            Menus.Add(mainMenuName, new Dictionary<string, Dictionary<Menu, Dictionary<string, Values>>>());

            return CurrentMainMenu = MainMenu.AddMenu(mainMenuName, mainMenuName);
        }

        public static Menu AddSubMenu(string subMenuName, Menu mainMenu = null)
        {
            if (mainMenu != null)
            {
                //Adding the submenu identifier to the Menus list
                Menus[mainMenu.DisplayName].Add(subMenuName, new Dictionary<Menu, Dictionary<string, Values>>());

                //Adding the submenu to the Menus list and initializing the list of identifiers and values
                Menus[mainMenu.DisplayName][subMenuName].Add(mainMenu.AddSubMenu(subMenuName, subMenuName), new Dictionary<string, Values>());

                return Menus[mainMenu.DisplayName][subMenuName].Keys.Last();
            }
            
            //Adding the submenu identifier to the Menus list
            Menus[CurrentMainMenu.DisplayName].Add(subMenuName, new Dictionary<Menu, Dictionary<string, Values>>());
            
            //Adding the submenu to the Menus list and initializing the list of identifiers and values
            Menus[CurrentMainMenu.DisplayName][subMenuName].Add(CurrentMainMenu.AddSubMenu(subMenuName, subMenuName), new Dictionary<string, Values>());

            return Menus[CurrentMainMenu.DisplayName][subMenuName].Keys.Last();
        }

        #region Menu.Values

        public enum Values
        {
            Checkbox = 0, Slider = 1, KeyBind = 2
        };

        public static void NewCheckbox(this Menu menu, string identifier, string displayName, bool defaultValue = true, bool separatorBefore = false)
        {
            if (separatorBefore) menu.AddSeparator();
            
            menu.Add(identifier, new CheckBox(displayName, defaultValue));

            Menus[menu.Parent.DisplayName][menu.DisplayName][menu].Add(identifier, Values.Checkbox);
        }

        public static void NewSlider(this Menu menu, string identifier, string displayName, int defaultValue, int minValue, int maxValue, bool separatorBefore = false)
        {
            if (separatorBefore) menu.AddSeparator();

            menu.Add(identifier, new Slider(displayName, defaultValue, minValue, maxValue));

            Menus[menu.Parent.DisplayName][menu.DisplayName][menu].Add(identifier, Values.Slider);
        }

        public static void NewKeybind(this Menu menu, string identifier, string displayName, bool defaultValue, KeyBind.BindTypes bindType, char key, bool separatorBefore = false)
        {
            if (separatorBefore) menu.AddSeparator();

            menu.Add(identifier, new KeyBind(displayName, defaultValue, bindType, key));

            Menus[menu.Parent.DisplayName][menu.DisplayName][menu].Add(identifier, Values.KeyBind);
        }

        #endregion

        public static bool IsActive(this Menu menu, string identifier)
        {
            return (bool)Return(menu, identifier);
        }

        public static int Value(this Menu menu, string identifier)
        {
            return (int)Return(menu, identifier);
        }

        public static object Return(this Menu menu, string identifier)
        {
            if (!Menus[menu.Parent.DisplayName][menu.DisplayName][menu].Keys.Any(it => it == identifier))
            {
                Console.WriteLine("The identifier {0} doesn't exists", identifier);
                return null;
            }

            switch (Menus[menu.Parent.DisplayName][menu.DisplayName][menu][identifier])
            {
                case Values.Checkbox:
                    return menu[identifier].Cast<CheckBox>().CurrentValue;

                case Values.Slider:
                    return menu[identifier].Cast<Slider>().CurrentValue;

                case Values.KeyBind:
                    return menu[identifier].Cast<KeyBind>().CurrentValue;

                default:
                    return null;
            }
        }
    }
}
