using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Jhin.Utilities;
using SharpDX;

namespace Jhin.Managers
{
    public class MyValueBase
    {
        public string LastTranslation;
        public string OriginalName;
        public ValueBase Value;
    }

    public class MyMenu
    {
        public string LastTranslation;
        public Menu Menu;
        public string OriginalName;
    }

    public static class MenuManager
    {
        public static Menu Menu;
        public static bool MenuCompleted;
        public static readonly Dictionary<string, MyMenu> SubMenus = new Dictionary<string, MyMenu>();
        public static readonly List<Action> PendingActions = new List<Action>();

        public static readonly Dictionary<string, Dictionary<string, MyValueBase>> ValuesBasePerMenu =
            new Dictionary<string, Dictionary<string, MyValueBase>>();

        public static readonly List<Tuple<Slider, string, string[]>> StringLists =
            new List<Tuple<Slider, string, string[]>>();

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("Jhin", "Jhin Build: 6.4.0, Champion: " + AIO.MyHero.ChampionName);
            var displayNames = Enum.GetValues(typeof (Language)).Cast<Language>().ToArray();
            var slider = Menu.Add("Language",
                new Slider("Language: English", (int) LanguageTranslator.CurrentCulture, 0, displayNames.Length - 1));
            slider.DisplayName = "Language".GetTranslationFromId() + ": " +
                                 displayNames[slider.CurrentValue].ToString().GetTranslationFromId();
            slider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = "Language".GetTranslationFromId() + ": " +
                                     displayNames[args.NewValue].ToString().GetTranslationFromId();
                Translate((Language) args.OldValue, (Language) args.NewValue);
            };
        }

        public static TValue AddValue<TValue>(this Menu menu, string id, TValue value) where TValue : ValueBase
        {
            menu.Add(id, value);
            if (!ValuesBasePerMenu.ContainsKey(menu.UniqueMenuId))
            {
                ValuesBasePerMenu.Add(menu.UniqueMenuId, new Dictionary<string, MyValueBase>());
            }
            ValuesBasePerMenu[menu.UniqueMenuId].Add(id,
                new MyValueBase {Value = value, LastTranslation = value.DisplayName, OriginalName = value.DisplayName});
            return value;
        }

        public static Menu AddSubMenu(string name)
        {
            SubMenus.Add(name, new MyMenu {Menu = Menu.AddSubMenu(name), LastTranslation = name, OriginalName = name});
            return SubMenus[name].Menu;
        }

        public static void Translate(Language from, Language to)
        {
            foreach (var pair in SubMenus)
            {
                var translation =
                    LanguageTranslator.GetTranslationFromDisplayName(from, to, pair.Value.LastTranslation) ??
                    pair.Value.OriginalName;
                if (translation != null)
                {
                    pair.Value.Menu.DisplayName = translation;
                    pair.Value.LastTranslation = translation;
                }
            }
            foreach (var pair in ValuesBasePerMenu)
            {
                foreach (var pair2 in pair.Value)
                {
                    var translation =
                        LanguageTranslator.GetTranslationFromDisplayName(from, to, pair2.Value.LastTranslation) ??
                        pair2.Value.OriginalName;
                    if (translation != null)
                    {
                        pair2.Value.Value.DisplayName = translation;
                        pair2.Value.LastTranslation = translation;
                    }
                }
            }
            foreach (var tuple in StringLists)
            {
                tuple.Item1.DisplayName =
                    (LanguageTranslator.GetTranslationFromDisplayName(Language.English,
                        LanguageTranslator.CurrentLanguage, tuple.Item2) ?? tuple.Item2) + ": " +
                    (LanguageTranslator.GetTranslationFromDisplayName(Language.English,
                        LanguageTranslator.CurrentLanguage, tuple.Item3[tuple.Item1.CurrentValue]) ??
                     tuple.Item3[tuple.Item1.CurrentValue]);
            }
        }

        public static void AddKillStealMenu()
        {
            var menu = AddSubMenu("KillSteal");
            menu.AddValue("Ignite", new CheckBox("Use Ignite"));
            menu.AddValue("Smite", new CheckBox("Use Smite"));
            menu.AddSeparator();
        }

        public static void AddDrawingsMenu()
        {
            var menu = AddSubMenu("Drawings");
            menu.AddValue("Disable", new CheckBox("Disable all drawings", false));
            menu.AddSeparator();
            menu.AddValue("DamageIndicator", new CheckBox("Draw damage indicator"));
            CircleManager.Circles.Add(new Circle(menu.AddValue("Target", new CheckBox("Draw circle on target")),
                new ColorBGRA(255, 0, 0, 100), () => 120f, () => AIO.CurrentChampion.Target != null,
                () => AIO.CurrentChampion.Target) {Width = 5});
            menu.AddSeparator();
        }

        public static Menu GetSubMenu(string s)
        {
            return (from t in SubMenus where t.Key.Equals(s) select t.Value.Menu).FirstOrDefault();
        }


        public static void AddStringList(this Menu m, string uniqueId, string displayName, string[] displayNames,
            int defaultValue = 0)
        {
            var slider = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, displayNames.Length - 1));
            slider.DisplayName =
                (LanguageTranslator.GetTranslationFromDisplayName(Language.English, LanguageTranslator.CurrentLanguage,
                    displayName) ?? displayName) + ": " +
                (LanguageTranslator.GetTranslationFromDisplayName(Language.English, LanguageTranslator.CurrentLanguage,
                    displayNames[slider.CurrentValue]) ?? displayNames[slider.CurrentValue]);
            slider.OnValueChange +=
                delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    slider.DisplayName =
                        (LanguageTranslator.GetTranslationFromDisplayName(Language.English,
                            LanguageTranslator.CurrentLanguage, displayName) ?? displayName) + ": " +
                        (LanguageTranslator.GetTranslationFromDisplayName(Language.English,
                            LanguageTranslator.CurrentLanguage, displayNames[args.NewValue]) ??
                         displayNames[args.NewValue]);
                };
            StringLists.Add(new Tuple<Slider, string, string[]>(slider, displayName, displayNames));
        }

        public static int Slider(this Menu m, string s)
        {
            if (m != null && AIO.Initialized)
            {
                return m[s].Cast<Slider>().CurrentValue;
            }
            return -1;
        }

        public static bool CheckBox(this Menu m, string s)
        {
            return m != null && AIO.CurrentChampion != null && AIO.Initialized && m[s].Cast<CheckBox>().CurrentValue;
        }

        public static bool KeyBind(this Menu m, string s)
        {
            return m != null && AIO.CurrentChampion != null && AIO.Initialized && m[s].Cast<KeyBind>().CurrentValue;
        }
    }
}
