using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Geometry = LeagueSharp.Common.Geometry;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace Warwick
{
    internal class Program
    {
        private const string ChampionName = "Warwick";

        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q, W, E, R;

        public static Menu Config;
        public static Menu rMenu;
        public static Menu menuCombo, menuHarass, menuLane, menuJungle, menuAuto, menuInterrupt, menuDraw;

        public static SpellR SpellR;

        private static Dictionary<string, Tuple<Items.Item, EnumItemType, EnumItemTargettingType>> ItemDb;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool enemyInAutoAttackRange
        {
            get
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                return t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65);
            }
        }

        private static AIHeroClient UseWforAlly
        {
            get
            {
                return
                    (from ally in
                        HeroManager.Allies.Where(
                            a => a.LSDistance(Player.Position) < W.Range && GetPriorityAllies(a.ChampionName))
                     from enemies in
                         HeroManager.Enemies.Where(
                             e => e.LSDistance(ally) < ally.AttackRange + ally.BoundingRadius && !e.IsDead)
                     select ally).FirstOrDefault();
            }
        }

        private static float GetWTotalDamage
        {
            get
            {
                if (!W.IsReady())
                    return 0;

                var baseAttackSpeed = 0.679348;
                var wCdTime = 5;
                var passiveDamage = 0; //2.5 + (Player.Level * 0.5);

                var attackSpeed =
                    (float)Math.Round(Math.Floor(1 / Player.AttackDelay * 100) / 100, 2, MidpointRounding.ToEven);

                var aDmg = Math.Round(Math.Floor(Player.TotalAttackDamage * 100) / 100, 2, MidpointRounding.ToEven);
                aDmg = Math.Floor(aDmg);

                int[] wAttackSpeedLevel = { 40, 50, 60, 70, 80 };
                var totalAttackSpeedWithWActive =
                    (float)
                        Math.Round((attackSpeed + baseAttackSpeed / 100 * wAttackSpeedLevel[W.Level - 1]) * 100 / 100, 2,
                            MidpointRounding.ToEven);

                var totalPossibleDamage =
                    (float)
                        Math.Round(totalAttackSpeedWithWActive * wCdTime * aDmg * 100 / 100, 2,
                            MidpointRounding.ToEven);
                return totalPossibleDamage + passiveDamage;
            }
        }

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
                return;

            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 1250);
            E = new Spell(SpellSlot.E, 4500);
            R = new Spell(SpellSlot.R, 700);
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            ItemDb = new Dictionary<string, Tuple<Items.Item, EnumItemType, EnumItemTargettingType>>
            {
                {
                    "Tiamat",
                    new Tuple<Items.Item, EnumItemType, EnumItemTargettingType>(
                        new Items.Item(3077, 250f),
                        EnumItemType.AoE,
                        EnumItemTargettingType.EnemyObjects)
                },
                {
                    "Bilge",
                    new Tuple<Items.Item, EnumItemType, EnumItemTargettingType>(new Items.Item(3144, 450f),
                        EnumItemType.Targeted, EnumItemTargettingType.EnemyHero)
                },
                {
                    "Blade",
                    new Tuple<Items.Item, EnumItemType, EnumItemTargettingType>(
                        new Items.Item(3153, 450f),
                        EnumItemType.Targeted,
                        EnumItemTargettingType.EnemyHero)
                },
                {
                    "Hydra",
                    new Tuple<Items.Item, EnumItemType, EnumItemTargettingType>(
                        new Items.Item(3074, 250f),
                        EnumItemType.AoE,
                        EnumItemTargettingType.EnemyObjects)
                },
                {
                    "Randiun",
                    new Tuple<Items.Item, EnumItemType, EnumItemTargettingType>(
                        new Items.Item(3143, 490f),
                        EnumItemType.AoE,
                        EnumItemTargettingType.EnemyHero)
                }
            };


            Config = MainMenu.AddMenu("Warwick | the Blood Hunter", "Warwick");

            rMenu = Config.AddSubMenu("R", "R");
            SpellR = new SpellR();

            menuCombo = Config.AddSubMenu("Combo", "Combo");
            menuCombo.Add("Combo.W.Use",
                new ComboBox("Use W:", 3, "Off", "Just for me", "Just for Allies", "Smart W (Recommend!)"));

            menuHarass = Config.AddSubMenu("Harass", "Harass");
            menuHarass.Add("Harass.Q.Use", new CheckBox("Use Q"));
            menuHarass.Add("Harass.Q.UseT", new KeyBind("Toggle", false, KeyBind.BindTypes.PressToggle, 'T'));
            menuHarass.Add("Harass.Q.UseTEnemyUn", new CheckBox("Don't Use Q Under Turret"));
            menuHarass.Add("Harass.Q.MinMana", new Slider("Min. Mana Per.:", 20, 1));

            menuLane = Config.AddSubMenu("Lane Farm", "Lane Farm");
            menuLane.Add("Lane.Q.Use", new ComboBox("Use Q", 1, "Off", "Last Hit", "Only out of AA Range", "Everytime"));
            menuLane.Add("Lane.Q.MinMana", new Slider("Min. Mana Per.:", 35, 1));
            menuLane.Add("Lane.W.Use", new CheckBox("Use W"));
            menuLane.Add("Lane.W.MinObj", new Slider("Min. Farm Count:", 3, 1, 6));
            menuLane.Add("Lane.W.MinMana", new Slider("Min. Mana Per.:", 35, 1));
            menuLane.Add("Lane.Items.Use", new CheckBox("Use Items"));

            menuJungle = Config.AddSubMenu("Jungle Farm", "Jungle Farm");
            menuJungle.Add("Jungle.Q.Use", new CheckBox("Use Q"));
            menuJungle.Add("Jungle.Q.MinMana", new Slider("Min. Mana Per.:", 20, 1));
            menuJungle.Add("Jungle.W.Use", new CheckBox("Use W"));
            menuJungle.Add("Jungle.W.MinMana", new Slider("Min. Mana Per.:", 20, 1));
            menuJungle.Add("Jungle.Items.Use", new CheckBox("Use Items"));

            menuAuto = Config.AddSubMenu("Auto", "Auto");
            menuAuto.Add("Auto.Q.UseQHp", new CheckBox("Use Auto Q"));
            menuAuto.Add("Auto.Q.UseQHpMinHp", new Slider("Min. Heal:", 70, 1));
            menuAuto.Add("Auto.Q.UseQHpEnemyUn", new CheckBox("Check Enemy Under Turret Position"));
            menuAuto.AddGroupLabel("E Settings");
            menuAuto.Add("Auto.E.Use", new CheckBox("Always Turn On E Spell"));

            menuInterrupt = Config.AddSubMenu("Interruptable Target", "Interruptable Target");
            menuInterrupt.Add("Interrupt.R", new CheckBox("Use R"));

            menuDraw = Config.AddSubMenu("Draw/Notification", "Draw");
            menuDraw.Add("Draw.Disable", new CheckBox("Disable All Drawings", false));
            menuDraw.Add("Draw.E.Show", new CheckBox("Show Blood Scent (E) Marked Enemy"));
            menuDraw.Add("Draw.Q", new CheckBox("Draw Q")); //.SetValue(new Circle(true, Color.Bisque)));
            menuDraw.Add("Draw.W", new CheckBox("Draw W")); //.SetValue(new Circle(true, Color.Coral)));
            menuDraw.Add("Draw.E", new CheckBox("Draw E")); //.SetValue(new Circle(true, Color.Aqua)));
            menuDraw.Add("Draw.R", new CheckBox("Draw R")); //.SetValue(new Circle(true, Color.Chartreuse)));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnEndScene += DrawingOnOnEndScene;
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

        private static void DrawingOnOnEndScene(EventArgs args)
        {
        }

        private static bool GetPriorityAllies(string championName)
        {
            if (
                new[]
                {
                    "Azir", "Olaf", "Renekton", "Shyvana", "Sion", "Skarner", "Thresh", "Volibear", "MonkeyKing",
                    "Yorick", "Aatrox", "Darius", "Diana", "Ekko", "Evelynn", "Fiora", "Fizz", "Gangplank", "Gragas",
                    "Irelia", "Jax", "Jayce", "Kayle", "Kha'Zix", "Lee Sin", "Nocturne", "Nidalee", "Pantheon", "Poppy",
                    "RekSai", "Rengar", "Riven", "Shaco", "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao",
                    "Yasuo", "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "Kennen",
                    "KogMaw", "Lucian", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Talon", "Teemo",
                    "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Zed"
                }.Contains(championName))
            {
                return true;
            }
            return false;
        }

        public static Vector3 CenterOfVectors(Vector3[] vectors)
        {
            var sum = Vector3.Zero;
            if (vectors == null || vectors.Length == 0)
                return sum;

            sum = vectors.Aggregate(sum, (current, vec) => current + vec);
            return sum / vectors.Length;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (E.Level > 0)
                E.Range = 700 + 800 * E.Level;

            if (getCheckBoxItem(menuAuto, "Auto.E.Use") && Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 &&
                E.IsReady())
                E.Cast();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || getKeyBindItem(menuHarass, "Harass.Q.UseT"))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Laneclear();
                JungleClear();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (getCheckBoxItem(menuAuto, "Auto.Q.UseQHp") && getSliderItem(menuAuto, "Auto.Q.UseQHpMinHp") > Player.HealthPercent)
                {
                    if (Player.IsRecalling())
                        return;

                    if (Q.IsReady())
                    {
                        var enemy = HeroManager.Enemies.Where(obj => obj.IsValidTarget(Q.Range) && !obj.IsDead && !obj.IsZombie).FirstOrDefault();
                        if (enemy != null)
                        {
                            Q.CastOnUnit(enemy);
                        }
                        var monster = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                        if (monster != null)
                        {
                            Q.CastOnUnit(monster);
                        }
                        var minion = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();
                        if (minion != null)
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(menuInterrupt, "Interrupt.R"))
                return;

            if (R.IsReady() && unit.IsValidTarget(R.Range) && !unit.HasBuff("bansheeveil"))
                R.Cast(unit);
        }

        private static float GetComboDamage(Obj_AI_Base t)
        {
            var fComboDamage = 0d;

            if (Q.IsReady())
                fComboDamage += Player.GetSpellDamage(t, SpellSlot.Q);

            if (W.IsReady())
                fComboDamage += GetWTotalDamage;

            if (R.IsReady())
                fComboDamage += Player.GetSpellDamage(t, SpellSlot.R) + Player.TotalAttackDamage;

            return (float)fComboDamage;
        }

        public static int GetClosesAlliesToEnemy(AIHeroClient t)
        {
            if (!t.IsValidTarget(R.Range))
                return 0;

            return (from ally in
                HeroManager.Allies.Where(
                    a =>
                        !a.IsMe && !a.IsDead && !a.IsZombie && a.LSDistance(t) < 1200 && a.Health > t.Health / 2 &&
                        a.Health > a.Level * 40)
                    let aMov = ally.MoveSpeed
                    let aPos = ally.Position
                    let tPos = t.Position
                    where aPos.LSDistance(tPos) < aMov * 1.8
                    select aMov).Any()
                ? 1
                : 0;
        }

        private static void Combo()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (!t.LSIsValidTarget())
            {
                return;
            }

            if (R.IsReady())
            {
                var tR = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (!tR.LSIsValidTarget())
                {
                    return;
                }
                if (Q.IsReady() && (tR.Health < Player.LSGetSpellDamage(t, SpellSlot.Q)))
                {
                    return;
                }

                if (tR.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) && tR.Health < Player.TotalAttackDamage)
                {
                    return;
                }

                if (tR.HasBuff("bansheeveil")) // don't use R if enemy's banshee is active!
                    return;

                var useR = getBoxItem(rMenu, "R.Use");
                switch (useR)
                {
                    case 1:
                        {
                            if (tR.IsValidTarget(R.Range))
                            {
                                R.Cast(tR);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (tR.IsValidTarget(R.Range) &&
                                tR.Health <
                                GetComboDamage(tR) + Player.TotalAttackDamage +
                                (Q.IsReady() ||
                                 Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires < 1.8 &&
                                 Player.Mana >
                                 Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana +
                                 Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana
                                    ? Player.GetSpellDamage(t, SpellSlot.Q)
                                    : 0))
                                R.Cast(tR);
                            break;
                        }
                    case 3:
                        {
                            if (tR.IsValidTarget(R.Range) &&
                                ((tR.Health <
                                  GetComboDamage(tR) + Player.TotalAttackDamage +
                                  (Q.IsReady() ||
                                   Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires < 1.8 &&
                                   Player.Mana >
                                   Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana +
                                   Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana
                                      ? Player.GetSpellDamage(t, SpellSlot.Q)
                                      : 0)) || tR.CountAlliesInRange(800) >= 2 ||
                                 GetClosesAlliesToEnemy(tR) > 0))
                                R.Cast(tR);
                            break;
                        }
                }
            }

            if (Q.IsReady() && t.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(t);
            }

            var useW = getBoxItem(menuCombo, "Combo.W.Use");

            if (W.IsReady())
            {
                switch (useW)
                {
                    case 1:
                        {
                            if (enemyInAutoAttackRange)
                                W.Cast();
                            break;
                        }
                    case 2:
                        {
                            if (UseWforAlly != null)
                                W.Cast();
                        }
                        break;
                    case 3:
                        {
                            if (enemyInAutoAttackRange || UseWforAlly != null)
                                W.Cast();
                            break;
                        }
                }
            }

            CastItems(t);
        }

        private static void Harass()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!t.IsValidTarget())
                return;

            if (getCheckBoxItem(menuHarass, "Harass.Q.UseTEnemyUn") && t.UnderTurret())
                return;

            if (Player.ManaPercent < getSliderItem(menuHarass, "Harass.Q.MinMana"))
                return;

            if (Q.IsReady() && getCheckBoxItem(menuHarass, "Harass.Q.Use"))
            {
                Q.CastOnUnit(t);
            }
        }

        private static void Laneclear()
        {
            var useQ = Q.IsReady() && Player.ManaPercent > getSliderItem(menuLane, "Lane.Q.MinMana");
            var useW = W.IsReady() && Player.ManaPercent > getSliderItem(menuLane, "Lane.W.MinMana") &&
                       getCheckBoxItem(menuLane, "Lane.W.Use");

            var qSelectedIndex = getBoxItem(menuLane, "Lane.Q.Use");

            var qMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            if (qMinions.Count == 0)
                return;

            if (useQ)
            {
                switch (qSelectedIndex)
                {
                    case 1:
                        {
                            if (Q.GetDamage(qMinions[0]) > qMinions[0].Health)
                                Q.CastOnUnit(qMinions[0]);
                            break;
                        }

                    case 2:
                        {
                            if (Q.GetDamage(qMinions[0]) > qMinions[0].Health &&
                                Player.LSDistance(qMinions[0]) > Orbwalking.GetRealAutoAttackRange(null) + 65)
                                Q.CastOnUnit(qMinions[0]);
                            break;
                        }

                    case 3:
                        {
                            Q.CastOnUnit(qMinions[0]);
                            break;
                        }
                }
            }

            if (useW)
            {
                var minMinion = getSliderItem(menuLane, "Lane.W.MinObj");
                if (qMinions.Count >= minMinion)
                {
                    W.Cast();
                }
            }

            if (getCheckBoxItem(menuLane, "Lane.Items.Use"))
            {
                foreach (var item in from item in ItemDb
                                     where
                                         item.Value.ItemType == EnumItemType.AoE &&
                                         item.Value.TargetingType == EnumItemTargettingType.EnemyObjects
                                     let iMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, item.Value.Item.Range)
                                     where
                                         iMinions.Count >= getSliderItem(menuLane, "Lane.W.MinObj") &&
                                         item.Value.Item.IsReady()
                                     select item)
                {
                    item.Value.Item.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var useQ = Q.IsReady() && Player.ManaPercent > getSliderItem(menuJungle, "Jungle.Q.MinMana") &&
                       getCheckBoxItem(menuJungle, "Jungle.Q.Use");

            var useW = W.IsReady() && Player.ManaPercent > getSliderItem(menuJungle, "Jungle.W.MinMana") &&
                       getCheckBoxItem(menuJungle, "Jungle.W.Use");

            var qMobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range + 300, MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (qMobs.Count == 0)
                return;

            if (useQ)
            {
                var jungleMinions = new[]
                {
                    "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon",
                    "SRU_Baron", "Sru_Crab"
                };

                var xMobs =
                    jungleMinions.FirstOrDefault(
                        name => qMobs[0].Name.Substring(0, qMobs[0].Name.Length - 5).Equals(name));

                if (xMobs != null)
                {
                    if (qMobs[0].IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(qMobs[0]);
                    }
                }
                else
                {
                    Q.CastOnUnit(qMobs[0]);
                }
            }

            if (useW && qMobs[0].IsValidTarget(Q.Range))
            {
                W.Cast();
            }

            if (getCheckBoxItem(menuJungle, "Jungle.Items.Use"))
            {
                foreach (var item in from item in ItemDb
                                     where
                                         item.Value.ItemType == EnumItemType.AoE &&
                                         item.Value.TargetingType == EnumItemTargettingType.EnemyObjects
                                     let iMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, item.Value.Item.Range)
                                     where
                                         item.Value.Item.IsReady()
                                     select item)
                {
                    item.Value.Item.Cast();
                }
            }
        }

        private static void CastItems(AIHeroClient t)
        {
            foreach (var item in ItemDb)
            {
                if (item.Value.ItemType == EnumItemType.AoE &&
                    item.Value.TargetingType == EnumItemTargettingType.EnemyHero)
                {
                    if (t.IsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady())
                        item.Value.Item.Cast();
                }
                if (item.Value.ItemType == EnumItemType.Targeted &&
                    item.Value.TargetingType == EnumItemTargettingType.EnemyHero)
                {
                    if (t.IsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady())
                        item.Value.Item.Cast(t);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(menuDraw, "Draw.Disable"))
                return;

            if (getCheckBoxItem(menuDraw, "Draw.Disable"))
                return;

            if (getCheckBoxItem(menuDraw, "Draw.E.Show") && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(e => e.IsValidTarget(E.Range) && !e.IsDead))
                {
                    foreach (var d in from buff in enemy.Buffs
                                      where buff.Name == "bloodscent_target"
                                      select
                                          new Geometry.Polygon.Line(Player.Position, enemy.Position, Player.LSDistance(enemy.Position)))
                    {
                        d.Draw(Color.Red, 2);

                        Vector3[] x = { Player.Position, enemy.Position };
                        var aX =
                            Drawing.WorldToScreen(new Vector3(CenterOfVectors(x).X, CenterOfVectors(x).Y,
                                CenterOfVectors(x).Z));
                        Drawing.DrawText(aX.X - 15, aX.Y - 15, Color.GreenYellow, enemy.ChampionName);
                    }
                }
            }

            foreach (var spell in SpellList)
            {
                var menuItem = getCheckBoxItem(menuDraw, "Draw." + spell.Slot);
                if (menuItem && spell.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, Color.Bisque, 2);
            }
        }

        private struct Tuple<TA, TB, TC> : IEquatable<Tuple<TA, TB, TC>>
        {
            private readonly TA item;
            private readonly TB itemType;
            private readonly TC targetingType;

            public Tuple(TA pItem, TB pItemType, TC pTargetingType)
            {
                item = pItem;
                itemType = pItemType;
                targetingType = pTargetingType;
            }

            public TA Item
            {
                get { return item; }
            }

            public TB ItemType
            {
                get { return itemType; }
            }

            public TC TargetingType
            {
                get { return targetingType; }
            }

            public override int GetHashCode()
            {
                return item.GetHashCode() ^ itemType.GetHashCode() ^ targetingType.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                return Equals((Tuple<TA, TB, TC>)obj);
            }

            public bool Equals(Tuple<TA, TB, TC> other)
            {
                return other.item.Equals(item) && other.itemType.Equals(itemType) &&
                       other.targetingType.Equals(targetingType);
            }
        }

        private enum EnumItemType
        {
            Targeted,
            AoE
        }

        private enum EnumItemTargettingType
        {
            Ally,
            EnemyHero,
            EnemyObjects
        }
    }
}