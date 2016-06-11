#region

using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Champions;
using Marksman.Utils;
using SharpDX;
using SharpDX.Direct3D9;


#endregion

namespace Marksman
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using System.Collections.Generic;

    using Color = SharpDX.Color;

    internal class Program
    {
        public static Menu Config;

        public static Marksman.Champions.Champion ChampionClass;

        static SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");


        //public static Utils.EarlyEvade EarlyEvade;

        public static LeagueSharp.Common.Spell Smite;

        public static SpellSlot SmiteSlot = SpellSlot.Unknown;

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };

        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };

        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };

        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static void Game_OnGameLoad()
        {
            Config = MainMenu.AddMenu("Marksman", "Marksman");
            ChampionClass = new Marksman.Champions.Champion();
            Common.CommonGeometry.Init();
            var BaseType = ChampionClass.GetType();

            /* Update this with Activator.CreateInstance or Invoke
               http://stackoverflow.com/questions/801070/dynamically-invoking-any-function-by-passing-function-name-as-string 
               For now stays cancer.
             */
            var championName = ObjectManager.Player.ChampionName.ToLowerInvariant();

            switch (championName)
            {
                case "ashe":
                    ChampionClass = new Ashe();
                    break;
                case "caitlyn":
                    ChampionClass = new Caitlyn();
                    break;
                case "corki":
                    ChampionClass = new Corki();
                    break;
                case "draven":
                    ChampionClass = new Draven();
                    break;
                case "ezreal":
                    ChampionClass = new Ezreal();
                    break;
                case "gnar":
                    ChampionClass = new Gnar();
                    break;
                case "graves":
                    ChampionClass = new Graves();
                    break;
                    /*
                case "jinx":
                    ChampionClass = new Jinx();
                    break;
                case "kalista":
                    ChampionClass = new Kalista();
                    break;
                case "kindred":
                    ChampionClass = new Kindred();
                    break;
                case "kogmaw":
                    ChampionClass = new Kogmaw();
                    break;
                case "lucian":
                    ChampionClass = new Lucian();
                    break;
                case "missfortune":
                    ChampionClass = new MissFortune();
                    break;
                case "quinn":
                    ChampionClass = new Quinn();
                    break;
                case "sivir":
                    ChampionClass = new Sivir();
                    break;
                case "teemo":
                    ChampionClass = new Teemo();
                    break;
                case "tristana":
                    ChampionClass = new Tristana();
                    break;
                case "twitch":
                    ChampionClass = new Twitch();
                    break;
                case "urgot":
                    ChampionClass = new Urgot();
                    break;
                case "vayne":
                    ChampionClass = new Vayne();
                    break;
                case "varus":
                    ChampionClass = new Varus();
                    break;
                    */
            }
            Config.DisplayName = "Marksman Lite";

            ChampionClass.Config = Config;

            MenuExtraTools = Config.AddSubMenu("Extra Tools", "ExtraTools");
            {
                MenuExtraTools.AddLabel("Press F5 for Load Extra Tools!");
                MenuExtraTools.Add("ExtraTools.Prediction", new ComboBox("Prediction:", 0, "LeagueSharp Common", "SPrediction (Synx)"));
                MenuExtraTools.Add("ExtraTools.AutoLevel", new CheckBox("Auto Leveler:", false));
                MenuExtraTools.Add("ExtraTools.AutoBush", new CheckBox("Auto Bush Ward:", false));
                MenuExtraTools.Add("ExtraTools.AutoPink", new CheckBox("Auto Pink Ward:", false));
                MenuExtraTools.Add("ExtraTools.Emote", new CheckBox("Emote:", false));
                MenuExtraTools.Add("ExtraTools.BuffTimer", new CheckBox("Buff Time Manager:", false));
                MenuExtraTools.Add("ExtraTools.Potition", new CheckBox("Potition Manager:", false));
                MenuExtraTools.Add("ExtraTools.Summoners", new CheckBox("Summoner Manager:", false));
                MenuExtraTools.Add("ExtraTools.Tracker", new CheckBox("Tracker:", false));
            }

            Common.CommonSettings.Init(Config);

            MenuActivator = Config.AddSubMenu("Activator", "Activator");
            {
                MenuActivator.Add("BOTRK", new CheckBox("BOTRK"));
                MenuActivator.Add("GHOSTBLADE", new CheckBox("Ghostblade"));
                MenuActivator.Add("SWORD", new CheckBox("Sword of the Divine"));
                MenuActivator.Add("MURAMANA", new CheckBox("Muramana"));
                MenuActivator.Add("UseItemsMode", new ComboBox("Use items on", 2, "No", "Mixed mode", "Combo mode", "Both"));

                if (MenuExtraTools["ExtraTools.AutoLevel"].Cast<CheckBox>().CurrentValue)
                {
                    Common.CommonAutoLevel.Init(MenuActivator);
                }

                if (MenuExtraTools["ExtraTools.AutoPink"].Cast<CheckBox>().CurrentValue)
                {
                    Common.CommonAutoPink.Initialize(MenuActivator);
                }

                if (MenuExtraTools["ExtraTools.AutoBush"].Cast<CheckBox>().CurrentValue)
                {
                    Common.CommonAutoBush.Init(MenuActivator);
                }

                if (MenuExtraTools["ExtraTools.Emote"].Cast<CheckBox>().CurrentValue)
                {
                    Common.CommonEmote.Init(MenuActivator);
                }
            }

            // If Champion is supported draw the extra menus
            if (BaseType != ChampionClass.GetType())
            {
                SetSmiteSlot();

                combo = Config.AddSubMenu("Combo", "Combo");
                if (ChampionClass.ComboMenu(combo))
                {
                    if (SmiteSlot != SpellSlot.Unknown)
                        combo.Add("ComboSmite", new CheckBox("Use Smite"));
                }

                harass = Config.AddSubMenu("Harass", "Harass");
                if (ChampionClass.HarassMenu(harass))
                {
                    harass.Add("HarassMana", new Slider("Min. Mana Percent", 50, 0, 100));
                }

                laneclear = Config.AddSubMenu("Lane Mode", "LaneClear");
                if (ChampionClass.LaneClearMenu(laneclear))
                {
                    laneclear.Add("Lane.Enabled", new KeyBind(":: Enable Lane Farm!", true, KeyBind.BindTypes.PressToggle, 'L'));

                    laneclear.AddGroupLabel("Min. Mana Settings");
                    {
                        laneclear.Add("LaneMana.Alone", new Slider("If I'm Alone %:", 30, 0, 100));
                        laneclear.Add("LaneMana.Enemy", new Slider("If Enemy Close %:", 60, 0, 100));
                    }
                }

                jungleClear = Config.AddSubMenu("Jungle Mode", "JungleClear");
                if (ChampionClass.JungleClearMenu(jungleClear))
                {
                    jungleClear.AddGroupLabel("Min. Mana Settings");
                    {
                        jungleClear.Add("Jungle.Mana.Ally", new Slider("Ally Mobs %:", 50, 0, 100));
                        jungleClear.Add("Jungle.Mana.Enemy", new Slider("Enemy Mobs %:", 30, 0, 100));
                        jungleClear.Add("Jungle.Mana.BigBoys", new Slider("Baron/Dragon %:", 70, 0, 100));
                    }
                    jungleClear.Add("Jungle.Items", new ComboBox(":: Use Items:", 3, "Off", "Use for Baron", "Use for Baron", "Both"));
                    jungleClear.Add("Jungle.Enabled", new KeyBind(":: Enable Jungle Farm!", true, KeyBind.BindTypes.PressToggle, 'J'));
                }

                /*----------------------------------------------------------------------------------------------------------*/
                Obj_AI_Base ally = (from aAllies in HeroManager.Allies from aSupportedChampions in new[] { "janna", "tahm", "leona", "lulu", "lux", "nami", "shen", "sona", "braum", "bard" } where aSupportedChampions == aAllies.ChampionName.ToLower() select aAllies).FirstOrDefault();

                if (ally != null)
                {
                    menuAllies = Config.AddSubMenu("Ally Combo", "Ally.Combo");
                    {
                        AIHeroClient Leona = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "leona");
                        if (Leona != null)
                        {
                            menuAllies.Add("Leona.ComboBuff", new CheckBox("Force Focus Marked Enemy for Bonus Damage"));
                        }

                        AIHeroClient Lux = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "lux");
                        if (Lux != null)
                        {
                            menuAllies.Add("Lux.ComboBuff", new CheckBox("Force Focus Marked Enemy for Bonus Damage"));
                        }

                        AIHeroClient Shen = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "shen");
                        if (Shen != null)
                        {
                            menuAllies.Add("Shen.ComboBuff", new CheckBox("Force Focus Q Marked Enemy Objects for Heal"));
                            menuAllies.Add("Shen.ComboBuff", new Slider("Minimum Heal:", 80));
                        }

                        AIHeroClient Tahm = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "Tahm");
                        if (Tahm != null)
                        {
                            menuAllies.Add("Tahm.ComboBuff", new CheckBox("Force Focus Marked Enemy for Stun"));
                        }

                        AIHeroClient Sona = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "Sona");
                        if (Sona != null)
                        {
                            menuAllies.Add("Sona.ComboBuff", new CheckBox("Force Focus to Marked Enemy"));
                        }

                        AIHeroClient Lulu = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "Lulu");
                        if (Lulu != null)
                        {
                            menuAllies.Add("Lulu.ComboBuff", new CheckBox("Force Focus to Enemy If I have E buff"));
                        }

                        AIHeroClient Nami = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "nami");
                        if (Nami != null)
                        {
                            menuAllies.Add("Nami.ComboBuff", new CheckBox("Force Focus to Enemy If I have E Buff"));
                        }
                    }
                }
                /*----------------------------------------------------------------------------------------------------------*/

                misc = Config.AddSubMenu("Misc", "Misc");
                if (ChampionClass.MiscMenu(misc))
                {
                    misc.Add("Misc.SaveManaForUltimate", new CheckBox("Save Mana for Ultimate", false));
                }

                marksmanDrawings = Config.AddSubMenu("Drawings", "MDrawings");
                if (ChampionClass.DrawingMenu(marksmanDrawings))
                {
                    //marksmanDrawings.AddSubMenu(drawing);
                }
                marksmanDrawings.AddGroupLabel("Global");
                {
                    marksmanDrawings.Add("Draw.TurnOff", new ComboBox("Drawings", 1, "Disable", "Enable", "Disable on Combo Mode", "Disable on Lane/Jungle Mode", "Both"));
                    marksmanDrawings.Add("Draw.KillableEnemy", new CheckBox("Killable Enemy Text", false));
                    marksmanDrawings.Add("Draw.MinionLastHit", new ComboBox("Minion Last Hit", 2, "Off", "On", "Just Out of AA Range Minions"));
                    marksmanDrawings.Add("Draw.DrawMinion", new CheckBox("Draw Minions Sprite", false));
                    marksmanDrawings.Add("Draw.DrawTarget", new CheckBox("Draw Target Sprite"));
                    marksmanDrawings.AddGroupLabel("Compare me with");
                    {
                        string[] strCompare = new string[HeroManager.Enemies.Count + 1];
                        strCompare[0] = "Off";
                        var i = 1;
                        foreach (var e in HeroManager.Enemies)
                        {
                            strCompare[i] = e.ChampionName;
                            i += 1;
                        }
                        marksmanDrawings.Add("Marksman.Compare.Set", new ComboBox("Set", 1, "Off", "Auto Compare at Startup"));
                        marksmanDrawings.Add("Marksman.Compare", new ComboBox("Compare me with", 0, strCompare));
                    }
                }
            }

            ChampionClass.MainMenu(Config);

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnUpdate += eventArgs =>
            {
                if (ChampionClass.LaneClearActive)
                {
                    ExecuteLaneClear();
                }

                if (ChampionClass.JungleClearActive)
                {
                    ExecuteJungleClear();
                }

                PermaActive();
            };

            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;

            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffRemove;

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            Console.Clear();
        }

        public static Menu MenuExtraTools, MenuActivator, combo, harass, laneclear, jungleClear, menuAllies, misc, marksmanDrawings;

        private static void CheckAutoWindUp()
        {
            var additional = 0;

            if (Game.Ping >= 100)
            {
                additional = Game.Ping / 100 * 10;
            }
            else if (Game.Ping > 40 && Game.Ping < 100)
            {
                additional = Game.Ping / 100 * 20;
            }
            else if (Game.Ping <= 40)
            {
                additional = +20;
            }
            var windUp = Game.Ping + additional;
            if (windUp < 40)
            {
                windUp = 40;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var turnOffDrawings = marksmanDrawings["Draw.TurnOff"].Cast<ComboBox>().CurrentValue;

            if (turnOffDrawings == 0)
            {
                return;
            }

            if ((turnOffDrawings == 2 || turnOffDrawings == 4) && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if ((turnOffDrawings == 3 || turnOffDrawings == 4) && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)))
            {
                return;
            }

            var drawMinionLastHit = marksmanDrawings["Draw.MinionLastHit"].Cast<ComboBox>().CurrentValue;
            if (drawMinionLastHit != 0)
            {
                var mx = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && m.IsEnemy).Where(m => m.Health <= ObjectManager.Player.TotalAttackDamage);

                if (drawMinionLastHit == 1)
                {
                    mx = mx.Where(m => m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65));
                }
                else
                {
                    mx = mx.Where(m => m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65 + 300) && m.LSDistance(ObjectManager.Player.Position) > Orbwalking.GetRealAutoAttackRange(null) + 65);
                }

                foreach (var minion in mx)
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius, System.Drawing.Color.GreenYellow, 1);
                }
            }

            if (ChampionClass != null)
            {
                ChampionClass.Drawing_OnDraw(args);
            }
        }

        private void MySupport()
        {

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Update the combo and harass values.
            ChampionClass.ComboActive = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);

            var vHarassManaPer = harass["HarassMana"].Cast<Slider>().CurrentValue;
            ChampionClass.HarassActive = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && ObjectManager.Player.ManaPercent >= vHarassManaPer;

            ChampionClass.ToggleActive = ObjectManager.Player.ManaPercent >= vHarassManaPer && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !ObjectManager.Player.LSIsRecalling();

            var vLaneClearManaPer = HeroManager.Enemies.Find(e => e.LSIsValidTarget(2000) && !e.IsZombie) == null
                ? laneclear["LaneMana.Alone"].Cast<Slider>().CurrentValue
                : laneclear["LaneMana.Enemy"].Cast<Slider>().CurrentValue;

            ChampionClass.LaneClearActive = (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) && ObjectManager.Player.ManaPercent >= vLaneClearManaPer && laneclear["Lane.Enabled"].Cast<KeyBind>().CurrentValue;

            ChampionClass.JungleClearActive = false;
            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) && jungleClear["Jungle.Enabled"].Cast<KeyBind>().CurrentValue)
            {
                List<Obj_AI_Base> mobs = MinionManager.GetMinions(ObjectManager.Player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);

                if (mobs.Count > 0)
                {
                    var minMana = jungleClear["Jungle.Mana.Enemy"].Cast<Slider>().CurrentValue;

                    if (mobs[0].BaseSkinName.ToLower().Contains("baron") || mobs[0].BaseSkinName.ToLower().Contains("dragon") || mobs[0].Team() == Jungle.GameObjectTeam.Neutral)
                    {
                        minMana = jungleClear["Jungle.Mana.BigBoys"].Cast<Slider>().CurrentValue;
                    }

                    else if (mobs[0].Team() == (Jungle.GameObjectTeam)ObjectManager.Player.Team)
                    {
                        minMana = jungleClear["Jungle.Mana.Ally"].Cast<Slider>().CurrentValue;
                    }

                    else if (mobs[0].Team() != (Jungle.GameObjectTeam)ObjectManager.Player.Team)
                    {
                        minMana = jungleClear["Jungle.Mana.Enemy"].Cast<Slider>().CurrentValue;
                    }

                    if (ObjectManager.Player.ManaPercent >= minMana)
                    {
                        ChampionClass.JungleClearActive = true;
                    }
                }
            }

            ChampionClass.Game_OnGameUpdate(args);

            UseSummoners();
            var useItemModes = MenuActivator["UseItemsMode"].Cast<ComboBox>().CurrentValue;

            //Items
            if (
                !((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                   (useItemModes == 2 || useItemModes == 3))
                  ||
                  (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                   (useItemModes == 1 || useItemModes == 3))))
                return;

            var botrk = MenuActivator["BOTRK"].Cast<CheckBox>().CurrentValue;
            var ghostblade = MenuActivator["GHOSTBLADE"].Cast<CheckBox>().CurrentValue;
            var sword = MenuActivator["SWORD"].Cast<CheckBox>().CurrentValue;
            var muramana = MenuActivator["MURAMANA"].Cast<CheckBox>().CurrentValue;
            var target = Orbwalker.LastTarget as Obj_AI_Base;

            var smiteReady = (SmiteSlot != SpellSlot.Unknown &&
                              ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready);

            if (smiteReady && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Smiteontarget(target as AIHeroClient);

            if (botrk)
            {
                if (target != null && target.Type == ObjectManager.Player.Type &&
                    target.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition) < 550)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);

                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, LeagueSharp.Common.Damage.DamageItems.Botrk);
                        if (hasCutGlass || ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth)
                            Items.UseItem(itemId, target);
                    }
                }
            }

            if (ghostblade && target != null && target.Type == ObjectManager.Player.Type &&
                !ObjectManager.Player.HasBuff("ItemSoTD") /*if Sword of the divine is not active */
                && Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3142);

            if (sword && target != null && target.Type == ObjectManager.Player.Type &&
                !ObjectManager.Player.HasBuff("spectralfury") /*if ghostblade is not active*/
                && Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3131);

            if (muramana && Items.HasItem(3042))
            {
                if (target != null && ChampionClass.ComboActive &&
                    target.Position.LSDistance(ObjectManager.Player.Position) < 1200)
                {
                    if (!ObjectManager.Player.HasBuff("Muramana"))
                    {
                        Items.UseItem(3042);
                    }
                }
                else
                {
                    if (ObjectManager.Player.HasBuff("Muramana"))
                    {
                        Items.UseItem(3042);
                    }
                }
            }
        }

        public static void UseSummoners()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            var t = Orbwalker.LastTarget as AIHeroClient;

            if (t != null && IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (ObjectManager.Player.LSDistance(t) < 650 &&
                    ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite) >=
                    t.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            ChampionClass.Orbwalking_AfterAttack(target, args);
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            ChampionClass.Orbwalking_BeforeAttack(target, args);
        }

        private static void ExecuteJungleClear()
        {
            ChampionClass.ExecuteJungleClear();
        }
        private static void ExecuteLaneClear()
        {
            ChampionClass.ExecuteLaneClear();
        }
        private static void PermaActive()
        {
            ChampionClass.PermaActive();
        }
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            ChampionClass.Obj_AI_Base_OnProcessSpellCast(sender, args);
        }
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (misc["Misc.SaveManaForUltimate"].Cast<CheckBox>().CurrentValue &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level > 0 &&
                Math.Abs(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Cooldown) < 0.00001 &&
                args.Slot != SpellSlot.R)
            {
                var lastMana = ObjectManager.Player.Mana - ObjectManager.Player.Spellbook.GetSpell(args.Slot).SData.Mana;
                if (lastMana < ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana)
                {
                    args.Process = false;
                }
            }

            ChampionClass.Spellbook_OnCastSpell(sender, args);
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            ChampionClass.OnCreateObject(sender, args);
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            ChampionClass.OnDeleteObject(sender, args);
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            ChampionClass.Obj_AI_Base_OnBuffAdd(sender, args);
        }

        private static void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            ChampionClass.Obj_AI_Base_OnBuffRemove(sender, args);
        }

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => Items.HasItem(i)))
                    return "s5_summonersmiteplayerganker";

                if (SmiteRed.Any(i => Items.HasItem(i)))
                    return "s5_summonersmiteduel";

                if (SmiteGrey.Any(i => Items.HasItem(i)))
                    return "s5_summonersmitequick";

                if (SmitePurple.Any(i => Items.HasItem(i)))
                    return "itemsmiteaoe";

                return "summonersmite";
            }
        }

        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => String.Equals(spell.Name, Smitetype, StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
                Smite = new LeagueSharp.Common.Spell(SmiteSlot, 700);
            }
        }

        private static void Smiteontarget(AIHeroClient t)
        {
            var useSmite = combo["ComboSmite"].Cast<CheckBox>().CurrentValue;
            var itemCheck = SmiteBlue.Any(i => Items.HasItem(i)) || SmiteRed.Any(i => Items.HasItem(i));
            if (itemCheck && useSmite &&
                ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready &&
                t.LSDistance(ObjectManager.Player.Position) < Smite.Range)
            {
                ObjectManager.Player.Spellbook.CastSpell(SmiteSlot, t);
            }
        }
        public static void DrawBox(Vector2 position, int width, int height, System.Drawing.Color color, int borderwidth, System.Drawing.Color borderColor)
        {
            Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, height, color);

            if (borderwidth > 0)
            {
                Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, borderwidth, borderColor);
                Drawing.DrawLine(position.X, position.Y + height, position.X + width, position.Y + height, borderwidth, borderColor);
                Drawing.DrawLine(position.X, position.Y + 1, position.X, position.Y + height, borderwidth, borderColor);
                Drawing.DrawLine(position.X + width, position.Y + 1, position.X + width, position.Y + height, borderwidth, borderColor);
            }
        }
    }
}
