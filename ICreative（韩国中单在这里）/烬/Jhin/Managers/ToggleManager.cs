using System;
using System.Collections.Generic;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Jhin;
using Jhin.Utilities;
using SharpDX;
using Color = System.Drawing.Color;

namespace Jhin.Managers
{
    public static class ToggleManager
    {
        private static readonly Dictionary<KeyBind, Action> Toggles = new Dictionary<KeyBind, Action>();
        private static readonly Dictionary<KeyBind, Text> Texts = new Dictionary<KeyBind, Text>();
        private static readonly Color EnabledColor = Color.FromArgb(255, 255, 255, 255);
        private static readonly Color DisabledColor = Color.FromArgb(100, 255, 255, 255);

        public static void RegisterToggle(KeyBind key, Action action)
        {
            if (!Toggles.ContainsKey(key))
            {
                Toggles.Add(key, action);
                Texts.Add(key, new Text("", new Font("Arial", 10F, FontStyle.Bold)));
                Game.OnTick += delegate
                {
                    if (key.CurrentValue)
                    {
                        action();
                    }
                };
            }
        }

        public static void Draw()
        {
            if (Toggles.Count > 0 && AIO.CurrentChampion.DrawingsMenu.CheckBox("Toggles"))
            {
                var count = 0;
                foreach (var pair in Texts)
                {
                    var value = pair.Key.CurrentValue;
                    pair.Value.Color = value ? EnabledColor : DisabledColor;
                    pair.Value.TextValue = pair.Key.DisplayName + ": " +
                                           (value ? "Enabled".GetTranslationFromId() : "Disabled".GetTranslationFromId());
                    pair.Value.Position = AIO.MyHero.Position.WorldToScreen() +
                                          new Vector2(-pair.Value.Bounding.Width/2f,
                                              45f + (pair.Value.Bounding.Height + 5f)*count);
                    pair.Value.Draw();
                    count++;
                }
            }
        }
    }
}