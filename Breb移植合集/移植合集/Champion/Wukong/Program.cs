#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

#endregion

namespace Wukong
{
    internal class Program
    {
        public const string ChampionName = "MonkeyKing";
        private static readonly AIHeroClient Player = ObjectManager.Player;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        private static bool monkeykingdecoystealth;
        private static readonly SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");
        //Menu
        public static Menu Config;
        public static int ultUsed;

        public static Menu menuCombo, menuHarass, menuLane, menuJungle, menuMisc, menuDraw;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "MonkeyKing")
                return;
            if (Player.IsDead)
                return;

            Q = new Spell(SpellSlot.Q, 375f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 640f);
            R = new Spell(SpellSlot.R, 375f);

            E.SetTargetted(0.5f, 2000f);

            SpellList.Add(Q);
            SpellList.Add(E);
            SpellList.Add(R);

            //Create the menu
            Config = MainMenu.AddMenu("xQx | Monkey King", "MonkeyKing");

            AssassinManager.Load();

            // Combo
            menuCombo = Config.AddSubMenu("R", "R");
            menuCombo.Add("UseRComboEnemyCount", new Slider("Use R if Enemy Count >= (0 = off)", 1, 0, 5));
            menuCombo.AddGroupLabel("Force Ultimate For:");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                menuCombo.Add("forceUlti" + enemy.NetworkId, new ComboBox(enemy.ChampionName, 0, "Off", "Everytime", "Just Killable"));
            }

            // Harass
            menuHarass = Config.AddSubMenu("Harass", "Harass");
            menuHarass.Add("UseQHarass", new CheckBox("Use Q"));
            menuHarass.Add("UseEHarass", new CheckBox("Use E"));
            menuHarass.Add("UseEHarassTurret", new CheckBox("Don't E Under Enemy Turret"));
            menuHarass.Add("HarassMana", new Slider("Min. Mana Percent: ", 50));

            // Lane Clear
            menuLane = Config.AddSubMenu("LaneClear", "LaneClear");
            menuLane.Add("UseQLaneClear", new CheckBox("Use Q", false));
            menuLane.Add("UseELaneClear", new CheckBox("Use E", false));
            menuLane.Add("LaneClearMana", new Slider("Min. Mana Percent: ", 50));

            // Jungling Farm
            menuJungle = Config.AddSubMenu("JungleFarm", "JungleFarm");
            menuJungle.Add("UseQJungleFarm", new CheckBox("Use Q"));
            menuJungle.Add("UseEJungleFarm", new CheckBox("Use E", false));
            menuJungle.Add("JungleFarmMana", new Slider("Min. Mana Percent: ", 50));

            // Misc
            menuMisc = Config.AddSubMenu("Misc", "Misc");
            menuMisc.Add("Misc.AutoQ", new ComboBox("Auto Q if it Will Hit", 1, "Off", "On", "On: Just If I'm Visible"));
            menuMisc.Add("Misc.BlockR", new CheckBox("Block R if it Won't Hit", false));
            menuMisc.Add("InterruptSpells", new CheckBox("Interrupt Spells"));

            // Drawing
            menuDraw = Config.AddSubMenu("Drawings", "Drawings");
            menuDraw.Add("QRange", new CheckBox("Q Range"));
                //.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));
            menuDraw.Add("ERange", new CheckBox("E Range"));
                //.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));
            menuDraw.Add("RRange", new CheckBox("R Range"));
                //.SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += (sender, eventArgs) =>
            {
                if (sender.IsMe && eventArgs.Buff.DisplayName == "MonkeyKingDecoyStealth")
                    monkeykingdecoystealth = true;
            };

            Obj_AI_Base.OnBuffLose += (sender, eventArgs) =>
            {
                if (sender.IsMe && eventArgs.Buff.DisplayName == "MonkeyKingDecoyStealth")
                    monkeykingdecoystealth = false;
            };
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

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = getCheckBoxItem(menuDraw, spell.Slot + "Range");
                if (menuItem)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, Color.FromArgb(255, 255, 255, 255), 1);
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "MonkeyKingSpinToWin")
            {
                ultUsed = (int) Game.Time;
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!getCheckBoxItem(menuMisc, "Misc.BlockR"))
            {
                return;
            }

            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (args.Slot == SpellSlot.R && !t.IsValidTarget())
            {
                args.Process = false;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var miscAutoQ = getBoxItem(menuMisc, "Misc.AutoQ");
            AIHeroClient t;
            switch (miscAutoQ)
            {
                case 1:
                    t = TargetSelector.GetTarget(Q.Range - 20f, DamageType.Physical);
                    if (t.IsValidTarget() && Q.IsReady())
                    {
                        Q.CastOnUnit(t);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, t);
                    }
                    break;
                case 2:
                    if (!monkeykingdecoystealth)
                    {
                        t = TargetSelector.GetTarget(Q.Range - 20f, DamageType.Physical);
                        if (t.IsValidTarget() && Q.IsReady())
                        {
                            Q.CastOnUnit(t);
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, t);
                        }
                    }
                    break;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && (int) Game.Time > ultUsed + 4)
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && (int) Game.Time > ultUsed + 4)
            {
                var vMana = getSliderItem(menuHarass, "HarassMana");
                if (Player.ManaPercent >= vMana)
                    Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var clearMana = getSliderItem(menuLane, "LaneClearMana");
                if (Player.ManaPercent >= clearMana)
                {
                    LaneClear();
                }

                var jgMana = getSliderItem(menuJungle, "JungleFarmMana");
                if (Player.ManaPercent >= jgMana)
                {
                    JungleFarm();
                }
            }
        }

        private static void Combo()
        {
            var t = GetTarget(E.Range);
            if (!t.IsValidTarget())
                return;

            if (E.IsReady() && t.IsValidTarget(E.Range))
            {
                E.CastOnUnit(t);
            }

            if (Q.IsReady() && t.IsValidTarget(Q.Range))
            {
                Q.Cast();
            }

            if (E.IsReady() && t.IsValidTarget(E.Range))
            {
                E.CastOnUnit(t);
            }

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite) > t.Health &&
                    Player.LSDistance(t) <= 550)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }

            if (R.IsReady())
            {
                var valR = getSliderItem(menuCombo, "UseRComboEnemyCount");
                if (valR > 0 && Player.CountEnemiesInRange(R.Range) >= valR)
                {
                    R.Cast();
                }
                else if (menuCombo["forceUlti" + t.NetworkId] != null && t.IsValidTarget(R.Range))
                {
                    switch (getBoxItem(menuCombo, "forceUlti" + t.NetworkId))
                    {
                        case 1:
                            R.CastIfHitchanceEquals(t, HitChance.High);
                            break;
                        case 2:
                        {
                            if (t.Health < GetComboDamage(t))
                                R.CastIfHitchanceEquals(t, HitChance.High);
                        }
                            break;
                    }
                }
            }
        }

        private static void Harass()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (!t.IsValidTarget())
                return;

            var useQ = getCheckBoxItem(menuHarass, "UseQHarass") && Q.IsReady();
            var useE = getCheckBoxItem(menuHarass, "UseEHarass") && E.IsReady();

            if (useQ && t.IsValidTarget(Q.Range))
            {
                Q.Cast();
            }

            if (useE && t.IsValidTarget(E.Range))
            {
                if (getCheckBoxItem(menuHarass, "UseEHarassTurret"))
                {
                    if (!t.UnderTurret())
                        E.CastOnUnit(t);
                }
                else
                    E.CastOnUnit(t);
            }
        }

        private static void JungleFarm()
        {
            var useQ = getCheckBoxItem(menuJungle, "UseQJungleFarm");
            var useE = getCheckBoxItem(menuJungle, "UseEJungleFarm");

            var mobs = MinionManager.GetMinions(
                Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0)
                return;

            var mob = mobs[0];
            if (mobs.Count > 0)
            {
                if (useQ && Q.IsReady())
                    Q.Cast();

                if (useE && E.IsReady())
                    E.CastOnUnit(mob);
            }
        }

        private static void LaneClear()
        {
            var useQ = getCheckBoxItem(menuLane, "UseQLaneClear") && Q.IsReady();
            var useE = getCheckBoxItem(menuLane, "UseELaneClear") && E.IsReady();

            if (useQ)
            {
                var minionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

                foreach (var vMinion in from vMinion in minionsQ
                    let vMinionEDamage = Player.GetSpellDamage(vMinion, SpellSlot.Q)
                    where vMinion.Health <= vMinionEDamage && vMinion.Health > Player.GetAutoAttackDamage(vMinion)
                    select vMinion)
                {
                    Q.Cast();
                }
            }

            if (useE)
            {
                var allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range);
                if (allMinionsE.Count >= 2)
                    E.CastOnUnit(allMinionsE[0]);
            }
        }

        private static float GetComboDamage(Obj_AI_Base vTarget)
        {
            var fComboDamage = 0d;

            if (Q.IsReady())
                fComboDamage += Player.GetSpellDamage(vTarget, SpellSlot.Q);

            if (E.IsReady())
                fComboDamage += Player.GetSpellDamage(vTarget, SpellSlot.E);

            if (R.IsReady())
                fComboDamage += Player.GetSpellDamage(vTarget, SpellSlot.R);

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                fComboDamage += Player.GetSummonerSpellDamage(vTarget, Damage.SummonerSpell.Ignite);

            return (float) fComboDamage;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var interruptSpells = getCheckBoxItem(menuMisc, "InterruptSpells");
            if (!interruptSpells)
                return;

            if (Player.LSDistance(unit) < Orbwalking.GetRealAutoAttackRange(Player) && R.IsReady())
            {
                R.Cast();
            }
        }

        private static AIHeroClient GetTarget(float vDefaultRange = 0,
            DamageType vDefaultDamageType = DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
                vDefaultRange = Q.Range;

            if (!AssassinManager.getCheckBoxItem("AssassinActive"))
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);

            var assassinRange = AssassinManager.getSliderItem("AssassinSearchRange");

            var vEnemy =
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        enemy =>
                            enemy.Team != Player.Team && !enemy.IsDead && enemy.IsVisible &&
                            AssassinManager.assMenu["Assassin" + enemy.NetworkId] != null &&
                            AssassinManager.getCheckBoxItem("Assassin" + enemy.NetworkId) &&
                            Player.LSDistance(enemy) < assassinRange);

            if (AssassinManager.getBoxItem("AssassinSelectOption") == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            var objAiHeroes = vEnemy as AIHeroClient[] ?? vEnemy.ToArray();
            var t = !objAiHeroes.Any() ? TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType) : objAiHeroes[0];
            return t;
        }
    }
}