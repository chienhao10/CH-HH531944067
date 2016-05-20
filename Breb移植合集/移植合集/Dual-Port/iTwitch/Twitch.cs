namespace iTwitch
{
using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System.Drawing;
using SebbyLib;

    public class Twitch
    {
        #region Static Fields

        /// <summary>
        ///     The dictionary to call the Spell slot and the Spell Class
        /// </summary>
        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
                                                                         {
                                                                             { SpellSlot.Q, new Spell(SpellSlot.Q) },
                                                                             { SpellSlot.W, new Spell(SpellSlot.W, 950f) },
                                                                             { SpellSlot.E, new Spell(SpellSlot.E, 1200f) },
                                                                             { SpellSlot.R, new Spell(SpellSlot.R) },
                                                                         };

        #endregion

        #region Fields

        public static Menu Menu { get; set; }
        public static Menu comboOptions, harassOptions, laneclearOptions, miscOptions, drawOptions;


        #endregion

        #region Public Methods and Operators

        public static void LoadMenu()
        {
            Menu = MainMenu.AddMenu("iTwitch 2.0", "com.itwitch");

            comboOptions = Menu.AddSubMenu(":: iTwith 2.0 - Combo", "com.itwitch.combo");
            comboOptions.Add("com.itwitch.combo.useW", new CheckBox("Use W", true));
            comboOptions.Add("com.itwitch.combo.useEKillable", new CheckBox("Use E Killable", true));



            harassOptions = Menu.AddSubMenu(":: iTwith 2.0 - Harass", "com.itwitch.harass");
            harassOptions.Add("com.itwitch.harass.useW", new CheckBox("Use W", true));
            harassOptions.Add("com.itwitch.harass.useEKillable", new CheckBox("Use E", true));


            miscOptions = Menu.AddSubMenu(":: iTwitch 2.0 - Misc", "com.itwitch.misc");
            miscOptions.Add("com.itwitch.misc.autoYo", new CheckBox("Yomuus with R", true));
            miscOptions.Add("com.itwitch.misc.saveManaE", new CheckBox("Save Mana for E", true));
            miscOptions.Add("com.itwitch.misc.recall", new KeyBind("Stealth Recall", false, KeyBind.BindTypes.HoldActive, 'T'));


            drawOptions = Menu.AddSubMenu(":: iTwith 2.0 - Drawing", "com.itwitch.drawing");
            drawOptions.Add("com.itwitch.drawing.drawQTime", new CheckBox("Draw Q Time", true));
            drawOptions.Add("com.itwitch.drawing.drawEStacks", new CheckBox("Draw E Stacks", true));
            drawOptions.Add("com.itwitch.drawing.drawEStackT", new CheckBox("Draw E Stack Time", true));
            drawOptions.Add("com.itwitch.drawing.drawRTime", new CheckBox("Draw R Time", true));

        }


        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }


        public static void LoadSpells()
        {
            Spells[SpellSlot.W].SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
        }

        public static void OnCombo()
        {
            if (getCheckBoxItem(comboOptions, "com.itwitch.combo.useEKillable") && Spells[SpellSlot.E].IsReady())
            {
                var killableTarget =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                        x.IsValidTarget(Spells[SpellSlot.E].Range) && Spells[SpellSlot.E].IsInRange(x)
                        && x.IsPoisonKillable());
                if (killableTarget != null)
                {
                    Spells[SpellSlot.E].Cast();
                }
            }

            if (getCheckBoxItem(comboOptions, "com.itwitch.combo.useW") && Spells[SpellSlot.W].IsReady())
            {
                var wTarget = TargetSelector.GetTarget(Spells[SpellSlot.W].Range, DamageType.Physical);
                if (wTarget.IsValidTarget(Spells[SpellSlot.W].Range))
                {
                    var prediction = Spells[SpellSlot.W].GetPrediction(wTarget);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Spells[SpellSlot.W].Cast(prediction.CastPosition);
                    }
                }
            }
        }

        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Twitch") return;

            LoadSpells();
            LoadMenu();

            Spellbook.OnCastSpell += (sender, eventArgs) =>
            {
                if (eventArgs.Slot == SpellSlot.Recall && Spells[SpellSlot.Q].IsReady()
                    && getKeyBindItem(miscOptions, "com.itwitch.misc.recall"))
                {
                    Spells[SpellSlot.Q].Cast();
                    Utility.DelayAction.Add(
                        (int)(Spells[SpellSlot.Q].Delay + 300),
                        () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall));
                    eventArgs.Process = false;
                    return;
                }

                if (eventArgs.Slot == SpellSlot.R && getCheckBoxItem(miscOptions, "com.itwitch.misc.autoYo"))
                {
                    if (!HeroManager.Enemies.Any(x => ObjectManager.Player.Distance(x) <= Spells[SpellSlot.R].Range)) return;

                    if (Items.HasItem(ItemData.Youmuus_Ghostblade.Id))
                    {
                        Items.UseItem(ItemData.Youmuus_Ghostblade.Id);
                    }
                }

                if (getCheckBoxItem(miscOptions, "com.itwitch.misc.saveManaE") && eventArgs.Slot == SpellSlot.W)
                {
                    if (ObjectManager.Player.Mana <= Spells[SpellSlot.E].ManaCost + 10)
                    {
                        eventArgs.Process = false;
                    }
                }
            };

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        public static void OnHarass()
        {
            if (getCheckBoxItem(harassOptions, "com.itwitch.harass.useEKillable") && Spells[SpellSlot.E].IsReady())
            {
                var target =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                        x.IsValidTarget(Spells[SpellSlot.E].Range) && Spells[SpellSlot.E].IsInRange(x)
                        && x.IsPoisonKillable());
                if (target != null)
                {
                    Spells[SpellSlot.E].Cast();
                }
            }

            if (getCheckBoxItem(harassOptions, "com.itwitch.harass.useW") && Spells[SpellSlot.W].IsReady())
            {
                var wTarget = TargetSelector.GetTarget(Spells[SpellSlot.W].Range, DamageType.Physical);
                if (wTarget.IsValidTarget(Spells[SpellSlot.W].Range))
                {
                    var prediction = Spells[SpellSlot.W].GetPrediction(wTarget);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Spells[SpellSlot.W].Cast(prediction.CastPosition);
                    }
                }
            }
        }

        #endregion

        #region Methods

        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawQTime")
                && ObjectManager.Player.HasBuff("TwitchHideInShadows"))
            {
                var position = new Vector3(
                    ObjectManager.Player.Position.X,
                    ObjectManager.Player.Position.Y - 30,
                    ObjectManager.Player.Position.Z);
                position.DrawTextOnScreen(
                    "Stealth:  " + $"{ObjectManager.Player.GetRemainingBuffTime("TwitchHideInShadows"):0.0}",
                    System.Drawing.Color.AntiqueWhite);
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawRTime")
                && ObjectManager.Player.HasBuff("TwitchFullAutomatic"))
            {
                ObjectManager.Player.Position.DrawTextOnScreen(
                    "Ultimate:  " + $"{ObjectManager.Player.GetRemainingBuffTime("TwitchFullAutomatic"):0.0}",
                    System.Drawing.Color.AntiqueWhite);
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawEStacks"))
            {
                foreach (var source in
                    HeroManager.Enemies.Where(x => x.HasBuff("TwitchDeadlyVenom") && !x.IsDead && x.IsVisible))
                {
                    var position = new Vector3(source.Position.X, source.Position.Y + 10, source.Position.Z);
                    position.DrawTextOnScreen($"{"Stacks: " + source.GetPoisonStacks()}", System.Drawing.Color.AntiqueWhite);
                }
            }

            if (getCheckBoxItem(drawOptions, "com.itwitch.drawing.drawEStackT"))
            {
                foreach (var source in
                    HeroManager.Enemies.Where(x => x.HasBuff("TwitchDeadlyVenom") && !x.IsDead && x.IsVisible))
                {
                    var position = new Vector3(source.Position.X, source.Position.Y - 30, source.Position.Z);
                    position.DrawTextOnScreen(
                        "Stack Timer:  " + $"{source.GetRemainingBuffTime("TwitchDeadlyVenom"):0.0}",
                        System.Drawing.Color.AntiqueWhite);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (getKeyBindItem(miscOptions, "com.itwitch.misc.recall"))
            {
                Spells[SpellSlot.Q].Cast();
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }

            }      
        #endregion
    }
}