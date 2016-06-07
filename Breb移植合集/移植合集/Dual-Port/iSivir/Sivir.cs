using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using Utility = LeagueSharp.Common.Utility;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu.Values;

namespace iSivir
{
    class Sivir
    {
        #region Static Fields

        /// <summary>
        ///     The dictionary to call the Spell slot and the Spell Class
        /// </summary>
        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
                                                                         {
                                                                             {
                                                                                 SpellSlot.Q, 
                                                                                 new Spell(SpellSlot.Q, 1100f)
                                                                             }, 
                                                                             { SpellSlot.W, new Spell(SpellSlot.W) }, 
                                                                             { SpellSlot.E, new Spell(SpellSlot.E) }, 
                                                                         };

        #endregion

        #region Fields

        private static Menu Menu;

        public static Menu comboMenu, harassMenu, laneMenu, miscMenu;

        private static readonly List<DangerousSpell> DangerousSpells = new List<DangerousSpell>
                                                                           {
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Vayne", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "VayneCondemn", 
                                                                                       Slot = SpellSlot.E, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Galio", 
                                                                                       Delay = 0.25f, SpellName = "GalioR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Amumu", 
                                                                                       Delay = 0.25f, SpellName = "AmumuR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Rammus", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "rammusE", 
                                                                                       Slot = SpellSlot.E, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Skarner", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "skarnerR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Caitlyn", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName =
                                                                                           "CaitlynAceintheHoleMissile", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = true
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Tristana", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "TristanaR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Syndra", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "SyndraR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Alistar", 
                                                                                       Delay = 0f, SpellName = "Pulverize", 
                                                                                       Slot = SpellSlot.Q, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Nocturne", 
                                                                                       Delay = 500f, 
                                                                                       SpellName =
                                                                                           "NocturneUnspeakableHorror", 
                                                                                       Slot = SpellSlot.E, 
                                                                                       IsTargetMissle = false
                                                                                   }
                                                                           };

        #endregion

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Sivir") return;

            LoadSpells();
            LoadMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            GameObject.OnCreate += OnCreateObject;
        }

        private static void OnCreateObject(GameObject sender, EventArgs arguments)
        {
            if (!(sender is MissileClient) || !sender.IsValid) return;
            var args = (MissileClient)sender;

            if (args.SData.Name != "CaitlynAceintheHoleMissile" || !args.Target.IsMe) return;

            if (Spells[SpellSlot.E].IsReady() && getCheckBoxItem(comboMenu, "com.isivir.combo.useE")
                && getCheckBoxItem(comboMenu, "CaitlynAceintheHoleMissile"))
            {
                Utility.DelayAction.Add(
                    (int)(args.StartPosition.Distance(ObjectManager.Player.Position) / 2000f + Game.Ping / 2f), 
                    () => Spells[SpellSlot.E].Cast());
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender1, GameObjectProcessSpellCastEventArgs args)
        {
            var sender = sender1 as AIHeroClient;

            if (sender == null || sender.IsMe || sender.IsAlly || !args.Target.IsMe || !Spells[SpellSlot.E].IsReady() || !getCheckBoxItem(comboMenu, "com.isivir.combo.useE") || args.SData.IsAutoAttack() || ObjectManager.Player.IsInvulnerable)
                return;

            if (sender.LSGetSpellDamage(ObjectManager.Player, args.Slot) >= ObjectManager.Player.Health
                && args.SData.TargettingType == SpellDataTargetType.Self)
            {
                Utility.DelayAction.Add((int)0.25f, () => Spells[SpellSlot.E].Cast());
            }

            foreach (var spell in DangerousSpells)
            {
                if (sender.ChampionName == spell.ChampionName && args.SData.Name == spell.SpellName && args.Slot == spell.Slot && !spell.IsTargetMissle)
                {
                    Utility.DelayAction.Add((int)spell.Delay, () => Spells[SpellSlot.E].Cast());
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Laneclear();
            }

            foreach (var target in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells[SpellSlot.Q].Range)))
            {
                if (getCheckBoxItem(miscMenu, "com.isivir.misc.qImmobile") && Spells[SpellSlot.Q].IsReady())
                {
                    Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.Immobile);
                }
            }
        }


        private static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!target.IsMe || !target.IsValid) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (getCheckBoxItem(comboMenu, "com.isivir.combo.useW") && target.IsValid<AIHeroClient>())
                    {
                        if (ObjectManager.Player.LSGetAutoAttackDamage((AIHeroClient)target, true)
                            * getSliderItem(miscMenu, "com.isivir.miscc.noW") > target.Health) return;

                        Spells[SpellSlot.W].Cast();
                    }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (getCheckBoxItem(harassMenu, "com.isivir.harass.useW") && target.IsValid<AIHeroClient>()
                        && ObjectManager.Player.ManaPercent
                        >= getSliderItem(harassMenu, "com.isivir.harass.mana"))
                    {
                        Spells[SpellSlot.W].Cast();
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    {
                        if (getCheckBoxItem(laneMenu, "com.isivir.laneclear.useW") && target.IsValid<Obj_AI_Minion>()
                        && ObjectManager.Player.ManaPercent
                        >= getSliderItem(laneMenu, "com.isivir.laneclear.mana"))
                    {
                        Spells[SpellSlot.W].Cast();
                    }
            }
                }
            }
        }

        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "com.isivir.combo.useQ");
            var target = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, DamageType.Physical);

            if (!Spells[SpellSlot.Q].IsReady() || !useQ) return;

            var prediction = Spells[SpellSlot.Q].GetPrediction(target);
            if (prediction.Hitchance >= HitChance.VeryHigh)
            {
                Spells[SpellSlot.Q].Cast(prediction.CastPosition);
            }
        }

        private static void Harass()
        {
            var useQ = getCheckBoxItem(harassMenu, "com.isivir.harass.useQ");
            var target = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, DamageType.Physical);

            if (!Spells[SpellSlot.Q].IsReady() || !useQ
                || ObjectManager.Player.ManaPercent < getSliderItem(harassMenu, "com.isivir.harass.mana")) return;

            var prediction = Spells[SpellSlot.Q].GetPrediction(target);
            if (prediction.Hitchance >= HitChance.High)
            {
                Spells[SpellSlot.Q].Cast(prediction.CastPosition);
            }
        }

        private static void Laneclear()
        {
            if (!Spells[SpellSlot.Q].IsReady() || !getCheckBoxItem(laneMenu, "com.isivir.laneclear.useQ")
                || ObjectManager.Player.ManaPercent < getSliderItem(laneMenu, "com.isivir.laneclear.mana")) return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells[SpellSlot.Q].Range);
            var farmLocation = Spells[SpellSlot.Q].GetLineFarmLocation(minions);

            if (farmLocation.MinionsHit >= getSliderItem(laneMenu, "com.isivir.laneclear.qMin"))
            {
                Spells[SpellSlot.Q].Cast(farmLocation.Position);
            }
        }

        private static void LoadMenu()
        {
            Menu = MainMenu.AddMenu("iSivir", "com.isivir");

            comboMenu = Menu.AddSubMenu("iSivir - Combo Options", "com.isivir.combo");
            comboMenu.Add("com.isivir.combo.useQ", new CheckBox("Use Q", true));
            comboMenu.Add("com.isivir.combo.useW", new CheckBox("Use W", true));
            comboMenu.Add("com.isivir.combo.useE", new CheckBox("Use E for targetted spells", true));

            comboMenu.AddGroupLabel("Auto Shield");
            {
                foreach (var spell in DangerousSpells)
                {
                    if (HeroManager.Enemies.Any(x => x.ChampionName == spell.ChampionName))
                    {
                        comboMenu.Add(spell.SpellName, new CheckBox(spell.ChampionName + ": " + spell.Slot, true));
                    }
                }

            harassMenu = Menu.AddSubMenu("iSivir - Harass Options", "com.isivir.harass");
            harassMenu.Add("com.isivir.harass.useQ", new CheckBox("Use Q", true));
            harassMenu.Add("com.isivir.harass.useW", new CheckBox("Use W", true));
            harassMenu.Add("com.isivir.harass.mana", new Slider("Min Mana %", 70, 0, 100));


            laneMenu = Menu.AddSubMenu("iSivir - Laneclear Options", "com.isivir.laneclear");
            laneMenu.Add("com.isivir.laneclear.useQ", new CheckBox("Use Q", true));
            laneMenu.Add("com.isivir.laneclear.qMin", new Slider("Min Minions for Q", 4, 0, 10));
            laneMenu.Add("com.isivir.laneclear.useW", new CheckBox("Use W", true));
            laneMenu.Add("com.isivir.laneclear.mana", new Slider("Min Mana %", 70, 0, 100));


            miscMenu = Menu.AddSubMenu("iSivir - Misc Options", "com.isivir.misc");
            miscMenu.Add("com.isivir.misc.qImmobile", new CheckBox("Auto Q Immobile", true));
            miscMenu.Add("com.isivir.miscc.noW", new Slider("No W if x aa can kill", 1, 0, 10));


            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        private static void LoadSpells()
        {
            Spells[SpellSlot.Q].SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);
        }
    }

    internal class DangerousSpell
    {
        public string ChampionName { get; set; }

        public string SpellName { get; set; }

        public float Delay { get; set; }

        public SpellSlot Slot { get; set; }

        public bool IsTargetMissle { get; set; }
    }
}