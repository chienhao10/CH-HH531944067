using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace PippyTaric
{
    internal class Program
    {
        private const string champName = "Taric";

        public static bool DebugMode = false;
        private static readonly Color pippyTaricColor = Color.FromArgb(60, 222, 203);
        private static readonly string[] qStringList = {"Don't use", "Only on me", "Only on ally"};
        private static readonly string buffName = "taricgemcraftbuff";

        private static Spell theQ, theW, theE, theR;
        private static SpellSlot ignite;
        private static Menu TaricMenu;
        private static AIHeroClient target;
        private static Dictionary<string, float> spellInfo;

        private static bool SWcombo;
        private static bool hasPassive;

        public static Menu comboMenu, harassMenu, healingMenu, drawMenu, miscMenu;

        public static void LoadStuff()
        {
            if (ObjectManager.Player.ChampionName != champName)
            {
                return;
            }

            MySpellInfo.Initialize();
            spellInfo = MySpellInfo.SpellTable;

            //Spells
            theQ = new Spell(SpellSlot.Q, spellInfo["qRange"]);
            theW = new Spell(SpellSlot.W, spellInfo["wRange"], DamageType.Magical);
            theE = new Spell(SpellSlot.E, spellInfo["eRange"], DamageType.Magical);
            theR = new Spell(SpellSlot.R, spellInfo["rRange"], DamageType.Magical);

            //Do we have ignite? I mean, you, not "we" ayy lmao
            ignite = ObjectManager.Player.GetSpellSlot("summonerdot");

            //Teh menu
            TaricMenuLoad();

            //Events
            Game.OnUpdate += TaricUpdate;
            Drawing.OnDraw += TaricDraw;
        }

        private static void TaricMenuLoad()
        {
            TaricMenu = MainMenu.AddMenu("Pippy Taric", "pippytaric");

            //Combo
            comboMenu = TaricMenu.AddSubMenu("Combo Settings", "combo");
            comboMenu.Add("useQ", new ComboBox("Use Q Mode:", 2, qStringList));
            comboMenu.Add("useQslider", new Slider("Min Hp% to heal", 75));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useWenemies", new Slider("   on >= X enemies", 1, 1, 5));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useErange",
                new Slider("Max Range to use", (int) spellInfo["eRange"], 1, (int) spellInfo["eRange"]));
            comboMenu.Add("useR", new CheckBox("Use R"));
            comboMenu.Add("useRenemies", new Slider("on >= X enemies", 2, 1, 5));
            comboMenu.Add("toggleCombo",
                new KeyBind("Toggle SpellWeaving Combo", false, KeyBind.BindTypes.PressToggle, 90));

            //Harass
            harassMenu = TaricMenu.AddSubMenu("Harass Settings", "harass");
            harassMenu.Add("useQharass", new CheckBox("Use Q in harass (only me)"));
            harassMenu.Add("useQharassSlider", new Slider("Min Hp% to heal", 75));
            harassMenu.Add("useEharass", new CheckBox("Use E in harass (max range)"));

            //Healing
            healingMenu = TaricMenu.AddSubMenu("Healing Settings", "healing");
            healingMenu.Add("ahSelf", new CheckBox("Auto Heal Self"));
            healingMenu.Add("ahSelfSlider", new Slider("at % HP", 70));
            healingMenu.Add("ahOther", new CheckBox("Auto Heal Other", false));
            healingMenu.Add("ahOtherSlider", new Slider("at % HP", 70));

            //Drawing
            drawMenu = TaricMenu.AddSubMenu("Drawing Settings", "draw");
            drawMenu.Add("qRangeDraw", new CheckBox("Draw Q Range")); //;//.SetValue(new Circle(true, pippyTaricColor));
            drawMenu.Add("wRangeDraw", new CheckBox("Draw W Range")); //.SetValue(new Circle(true, pippyTaricColor));
            drawMenu.Add("eRangeDrawMax", new CheckBox("Draw E MAX Range")); //.SetValue(new Circle(false, Color.Red));
            drawMenu.Add("eRangeDraw", new CheckBox("Draw set E range"));
                //.SetValue(new Circle(true, pippyTaricColor));
            drawMenu.Add("rRangeDraw", new CheckBox("Draw R Range")); //.SetValue(new Circle(true, pippyTaricColor));
            drawMenu.Add("drawHide", new CheckBox("Hide Ranges if not available", false));
            drawMenu.Add("drawTarget", new CheckBox("Draw Target"));
            drawMenu.Add("drawMode", new CheckBox("Draw Combo Mode"));
            drawMenu.Add("drawDamage", new CheckBox("Draw Damage on HealthBar"));

            // Misc Menu
            miscMenu = TaricMenu.AddSubMenu("Misc Settings", "misc");
            miscMenu.Add("ksIgnite", new CheckBox("KS with Ignite"));
            miscMenu.Add("interrupt", new CheckBox("Interrupt Skills"));
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

        private static void TaricUpdate(EventArgs args)
        {
            target = TargetSelector.GetTarget(GetTargetRange(), DamageType.Magical);

            SWcombo = getKeyBindItem(comboMenu, "toggleCombo");

            hasPassive = ObjectManager.Player.HasBuff(buffName);

            HealingManager();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (!SWcombo)
                {
                    Combo();
                }
                else
                {
                    SpellWeaving();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }

        private static float GetTargetRange()
        {
            var eValue = getSliderItem(comboMenu, "useErange");
            var wValue = spellInfo["wRange"];

            if (eValue >= wValue)
            {
                return eValue;
            }

            return wValue;
        }

        private static void Combo()
        {
            var allyList = ObjectManager.Get<AIHeroClient>().ToList().FindAll(ally => ally.IsAlly);

            if (target == null)
            {
                return;
            }

            if (theQ.IsReady())
            {
                switch (getBoxItem(comboMenu, "useQ"))
                {
                    case 0:
                        break;
                    case 1:
                        if (ObjectManager.Player.HealthPercent <= getSliderItem(comboMenu, "useQslider"))
                        {
                            theQ.CastOnUnit(ObjectManager.Player);
                        }
                        break;
                    case 2:
                        foreach (var ally in allyList)
                        {
                            if (!ally.IsDead)
                            {
                                if (ally.HealthPercent <= getSliderItem(comboMenu, "useQslider"))
                                {
                                    theQ.CastOnUnit(ally);
                                }
                            }
                        }
                        break;
                }
            }

            if (theW.IsReady())
            {
                if (getCheckBoxItem(comboMenu, "useW"))
                {
                    if (ObjectManager.Player.CountEnemiesInRange(spellInfo["wRange"]) >=
                        getSliderItem(comboMenu, "useWenemies"))
                    {
                        theW.Cast();
                    }
                }
            }

            if (theE.IsReady())
            {
                if (getCheckBoxItem(comboMenu, "useE"))
                {
                    if (ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) <=
                        getSliderItem(comboMenu, "useErange"))
                    {
                        theE.CastOnUnit(target);
                    }
                }
            }

            if (theR.IsReady())
            {
                if (getCheckBoxItem(comboMenu, "useR"))
                {
                    if (ObjectManager.Player.CountEnemiesInRange(spellInfo["rRange"]) >=
                        getSliderItem(comboMenu, "useRenemies"))
                    {
                        theR.Cast();
                    }
                }
            }
        }

        private static void Harass()
        {
            if (target == null)
            {
                return;
            }

            if (theQ.IsReady())
            {
                if (getCheckBoxItem(harassMenu, "useQharass"))
                {
                    if (ObjectManager.Player.HealthPercent <= getSliderItem(harassMenu, "useQharassSlider"))
                    {
                        theQ.Cast(ObjectManager.Player);
                    }
                }
            }

            if (theE.IsReady())
            {
                if (getCheckBoxItem(harassMenu, "useEharass"))
                {
                    if (ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) <= spellInfo["eRange"])
                    {
                        theE.CastOnUnit(target);
                    }
                }
            }
        }

        private static void HealingManager()
        {
            var HealSelf = getCheckBoxItem(healingMenu, "ahSelf");
            var HealOther = getCheckBoxItem(healingMenu, "ahOther");

            var HealSelfSlider = getSliderItem(healingMenu, "ahSelfSlider");
            var HealOtherSlider = getSliderItem(healingMenu, "ahOtherSlider");

            var AllyList = ObjectManager.Get<AIHeroClient>().ToList().FindAll(ally => ally.IsAlly);

            if (!theQ.IsReady() || (!HealSelf && !HealOther))
            {
                return;
            }

            if (HealSelf)
            {
                if (ObjectManager.Player.HealthPercent <= HealSelfSlider)
                {
                    theQ.CastOnUnit(ObjectManager.Player);
                }
            }

            if (HealOther)
            {
                foreach (var ally in AllyList)
                {
                    if (ObjectManager.Player.ServerPosition.Distance(ally.ServerPosition) <= spellInfo["qRange"])
                    {
                        if (ally.HealthPercent <= HealOtherSlider)
                        {
                            theQ.CastOnUnit(ally);
                        }
                    }
                }
            }
        }

        private static void SpellWeaving()
        {
            if (SWcombo && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target != null)
            {
                if (theE.IsReady() && target.IsValidTarget(spellInfo["eRange"]) && !GetPassiveState())
                {
                    theE.CastOnUnit(target);
                }
                else if (theR.IsReady() && !theE.IsReady()
                         && target.IsValidTarget(spellInfo["rRange"]) && !GetPassiveState())
                {
                    theR.Cast();
                }
                else if (theW.IsReady() && !theR.IsReady() && !theE.IsReady()
                         && target.IsValidTarget(spellInfo["wRange"]) && !GetPassiveState())
                {
                    theW.Cast();
                }
                else if (theQ.IsReady() && !theW.IsReady() && !theR.IsReady() && !theE.IsReady()
                         && target.IsValidTarget(spellInfo["qRange"]) && !GetPassiveState())
                {
                    theQ.CastOnUnit(ObjectManager.Player);
                }
            }
        }

        private static void TaricDraw(EventArgs args)
        {
            var HideNotReady = getCheckBoxItem(drawMenu, "drawHide");
            var ScreenPos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (getCheckBoxItem(drawMenu, "qRangeDraw"))
            {
                if (!(!theQ.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["qRange"],
                        pippyTaricColor);
                }
            }

            if (getCheckBoxItem(drawMenu, "wRangeDraw"))
            {
                if (!(!theW.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["wRange"],
                        pippyTaricColor);
                }
            }

            if (getCheckBoxItem(drawMenu, "eRangeDrawMax"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["eRange"],
                    Color.Red);
            }

            if (getCheckBoxItem(drawMenu, "eRangeDraw"))
            {
                if (!(!theE.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position,
                        getSliderItem(comboMenu, "useErange"),
                        pippyTaricColor);
                }
            }

            if (getCheckBoxItem(drawMenu, "rRangeDraw"))
            {
                if (!(!theR.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["rRange"], pippyTaricColor);
                }
            }

            if (getCheckBoxItem(drawMenu, "drawTarget") && target != null)
            {
                Render.Circle.DrawCircle(target.Position, target.BoundingRadius, Color.Orange, 8);
            }

            if (getCheckBoxItem(drawMenu, "drawMode"))
            {
                if (getKeyBindItem(comboMenu, "toggleCombo"))
                {
                    Drawing.DrawText(ScreenPos[0] - 60, ScreenPos[1] + 50, Color.LimeGreen, "SpellWeaving ON");
                }
                else
                {
                    Drawing.DrawText(ScreenPos[0] - 60, ScreenPos[1] + 50, Color.Red, "SpellWeaving OFF");
                }
            }
        }

        private static bool GetPassiveState()
        {
            return hasPassive;
        }
    }
}