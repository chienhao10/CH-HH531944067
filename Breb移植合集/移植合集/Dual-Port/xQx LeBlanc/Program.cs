#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeblancOLD.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

namespace LeblancOLD
{
    internal class Program
    {
        public const string ChampionName = "Leblanc";

        private static readonly List<Slide> ExistingSlide = new List<Slide>();
        private static bool leBlancClone;

        public static List<LeagueSharp.Common.Spell> SpellList = new List<LeagueSharp.Common.Spell>();

        public static LeagueSharp.Common.Spell Q, W, E, R;

        private static ComboType vComboType = ComboType.ComboQR;
        private static ComboKill vComboKill = ComboKill.FullCombo;
        private static bool _isComboCompleted = true;

        public static SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
        public static Items.Item Fqc = new Items.Item(3092, 750);

        //Menu
        public static Menu Config;
        public static Menu MenuExtras;

        private static readonly string[] LeBlancIsWeakAgainst =
        {
            "Galio", "Karma", "Sion", "Annie", "Syndra", "Diana",
            "Aatrox", "Mordekaiser", "Talon", "Morgana"
        };

        private static readonly string[] LeBlancIsStrongAgainst =
        {
            "Velkoz", "Ahri", "Karthus", "Fizz", "Ziggs",
            "Katarina", "Orianna", "Nidalee", "Yasuo", "Akali"
        };

        public static bool LeBlancClone
        {
            get { return leBlancClone; }
            set { leBlancClone = value; }
        }

        public static Menu comboMenu, harassMenu, laneClearMenu, jungleMenu, runMenu, drawMenu;

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

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
                return;

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 720);
            Q.SetTargetted(0.5f, 1500f);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 670);
            W.SetSkillshot(0.6f, 220f, 1450f, false, SkillshotType.SkillshotCircle);

            E = new LeagueSharp.Common.Spell(SpellSlot.E, 900);
            E.SetSkillshot(0.3f, 55f, 1650f, true, SkillshotType.SkillshotLine);

            R = new LeagueSharp.Common.Spell(SpellSlot.R, 720);
            {
                SpellList.Add(Q);
                SpellList.Add(W);
                SpellList.Add(E);
                SpellList.Add(R);
            }

            Config = MainMenu.AddMenu(ChampionName, ChampionName);

            Common.CommonGeometry.Init();
            var x = new CommonBuffManager();

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ComboMode", new ComboBox("Combo", 1, "Auto", "Q-R Combo", "W-R Combo", "E-R Combo"));
            comboMenu.Add("ComboMode.Key", new CheckBox("Quick Change Combo Mode With Mouse Scroll Button"));
            comboMenu.Add("ComboSetEHitCh", new ComboBox("E Hit", 2, "Low", "Medium", "High", "Very High", "Immobile"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                comboMenu.Add("DontCombo" + enemy.CharData.BaseSkinName, new CheckBox("Don't Combo : " + enemy.CharData.BaseSkinName, false));
            }
            comboMenu.Add("ComboDblStun", new KeyBind("Double Stun!", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("ComboShowInfo", new CheckBox("Show Combo Status"));

            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.AddGroupLabel("Q");
            harassMenu.Add("HarassUseQ", new CheckBox("Use Q"));
            harassMenu.Add("HarassManaQ", new Slider("Q Min. Mana Percent: ", 50));
            harassMenu.Add("HarassUseTQ", new KeyBind("Use Q (toggle)!", false, KeyBind.BindTypes.PressToggle, 'J'));
            harassMenu.AddGroupLabel("W");
            harassMenu.Add("HarassUseW", new CheckBox("Use W"));
            harassMenu.Add("HarassManaW", new Slider("W Min. Mana Percent: ", 50));
            harassMenu.Add("HarassUseTW", new KeyBind("Use W (toggle)!", false, KeyBind.BindTypes.PressToggle, 'K'));
            harassMenu.AddGroupLabel("E");
            harassMenu.Add("HarassUseE", new CheckBox("Use E"));
            harassMenu.Add("HarassManaE", new Slider("E Min. Mana Percent: ", 50));
            harassMenu.Add("HarassUseTE", new KeyBind("Use E (toggle)!", false, KeyBind.BindTypes.PressToggle, 'K'));
            harassMenu.Add("HarassShowInfo", new CheckBox("Show Harass Toggle Status"));

            laneClearMenu = Config.AddSubMenu("Lane Clear", "LaneClear");
            laneClearMenu.Add("LaneClearUseQ", new ComboBox("Use Q", 3, "Off", "On: Last Hit", "On: Unkillable Minions", "On: Both"));
            laneClearMenu.Add("LaneClearUseW", new CheckBox("Use W", false));
            laneClearMenu.Add("LaneClearUseE", new CheckBox("Use E", false));
            laneClearMenu.Add("LaneClearMana", new Slider("Min. Mana Percent: ", 50));

            jungleMenu = Config.AddSubMenu("JungleFarm", "JungleFarm");
            jungleMenu.Add("JungleFarmUseQ", new CheckBox("Use Q"));
            jungleMenu.Add("JungleFarmUseW", new CheckBox("Use W"));
            jungleMenu.Add("JungleFarmUseE", new CheckBox("Use E"));
            jungleMenu.Add("JungleFarmMana", new Slider("Min. Mana Percent: ", 50));

            runMenu = Config.AddSubMenu("Run", "Run");
            runMenu.Add("RunUseW", new CheckBox("Use W"));
            runMenu.Add("RunUseR", new CheckBox("Use R"));
            runMenu.Add("RunActive", new KeyBind("Run!", false, KeyBind.BindTypes.HoldActive, 'A'));

            MenuExtras = Config.AddSubMenu("Extras", "Extras");
            MenuExtras.Add("InterruptSpells", new CheckBox("Interrupt Spells"));

            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.AddGroupLabel("Spells : ");
            drawMenu.Add("QRange", new CheckBox("Q Range", false));
            drawMenu.Add("WRange", new CheckBox("W Range"));
            drawMenu.Add("ERange", new CheckBox("E Range", false));
            drawMenu.Add("RRange", new CheckBox("R Range", false));
            drawMenu.AddGroupLabel("Others : ");
            drawMenu.Add("Show.JungleBuffs", new CheckBox("Show Jungle Buff Time Circle:"));
            drawMenu.Add("ActiveERange", new CheckBox("Active E Range", false));
            drawMenu.Add("WQRange", new CheckBox("W + Q Range", false));

            Game.OnUpdate += Game_OnUpdate;
            {
                Drawing.OnDraw += Drawing_OnDraw;
                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;
                Game.OnWndProc += Game_OnWndProc;
                Interrupter2.OnInterruptableTarget += Interrupter_OnPosibleToInterrupt;
            }

            Chat.Print(String.Format("<font color='#70DBDB'>xQx</font> <font color='#FFFFFF'>{0}</font> <font color='#70DBDB'>Loaded! Fixed Auto Return W Problem</font>", ChampionName));
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != 0x20a)
            {
                return;
            }

            if (!getCheckBoxItem(comboMenu, "ComboMode.Key"))
            {
                return;
            }

            var newValue = getBoxItem(comboMenu, "ComboMode") + 1;

            if (getBoxItem(comboMenu, "ComboMode") == 3)
            {
                newValue = 0;
            }

            comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue = newValue;
        }

        private static int FindCounterStatusForTarget(string enemyBaseSkinName)
        {
            if (LeBlancIsWeakAgainst.Contains(enemyBaseSkinName))
                return 1;

            if (LeBlancIsStrongAgainst.Contains(enemyBaseSkinName))
                return 2;

            return 0;
        }

        private static AIHeroClient EnemyHaveSoulShackle
        {
            get
            {
                return
                    (from hero in
                        ObjectManager.Get<AIHeroClient>().Where(hero => ObjectManager.Player.LSDistance(hero) <= 1100)
                     where hero.IsEnemy
                     from buff in hero.Buffs
                     where buff.Name.Contains("LeblancSoulShackle")
                     select hero).FirstOrDefault();
            }
        }

        private static bool DrawEnemySoulShackle
        {
            get
            {
                return
                    (from hero in
                        ObjectManager.Get<AIHeroClient>().Where(hero => ObjectManager.Player.LSDistance(hero) <= 1100)
                     where hero.IsEnemy
                     from buff in hero.Buffs
                     select (buff.Name.Contains("LeblancSoulShackle"))).FirstOrDefault();
            }
        }
        private static void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(MenuExtras, "InterruptSpells"))
            {
                return;
            }

            var isValidTarget = unit.LSIsValidTarget(E.Range) && args.DangerLevel == Interrupter2.DangerLevel.High;

            if (E.IsReady() && isValidTarget)
            {
                E.CastIfHitchanceEquals(unit, GetEHitChance);
            }
            else if (R.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM" &&
                     isValidTarget)
            {
                R.Cast(unit);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            leBlancClone = sender.Name.Contains("LeBlanc_MirrorImagePoff.troy");

            if (sender.Name.Contains("displacement_blink_indicator"))
            {
                ExistingSlide.Add(
                    new Slide
                    {
                        Object = sender,
                        NetworkId = sender.NetworkId,
                        Position = sender.Position,
                        ExpireTime = Game.Time + 4
                    });
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("displacement_blink_indicator"))
                return;

            for (var i = 0; i < ExistingSlide.Count; i++)
            {
                if (ExistingSlide[i].NetworkId == sender.NetworkId)
                {
                    ExistingSlide.RemoveAt(i);
                    return;
                }
            }
        }

        public static bool LeBlancStillJumped
        {
            get
            {
                return !W.IsReady() || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn";
            }
        }

        private static void UserSummoners(Obj_AI_Base t)
        {

            if (Fqc.IsReady())
                Fqc.Cast(t.ServerPosition);

        }

        private enum ComboType
        {
            Auto,
            ComboQR,
            ComboWR,
            ComboER
        }

        private enum ComboKill
        {
            None,
            FullCombo,
            WithoutW
        }

        private static void ExecuteCombo()
        {
            if (!R.IsReady())
                return;

            _isComboCompleted = false;

            AIHeroClient t;
            var cdQEx = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires;
            var cdWEx = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).CooldownExpires;
            var cdEEx = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).CooldownExpires;

            var cdQ = Game.Time < cdQEx ? cdQEx - Game.Time : 0;
            var cdW = Game.Time < cdWEx ? cdWEx - Game.Time : 0;
            var cdE = Game.Time < cdEEx ? cdEEx - Game.Time : 0;

            if (vComboType == ComboType.ComboQR && Q.IsReady())
            {

                t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (t == null)
                    return;

                Q.CastOnUnit(t, true);
                R.CastOnUnit(t, true);
            }

            if (vComboType == ComboType.ComboWR && W.IsReady())
            {
                t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (t == null)
                    return;

                if (!LeBlancStillJumped)
                    W.Cast(t, true, true);

                R.Cast(t, true, true);
            }

            if (vComboType == ComboType.ComboER && E.IsReady())
            {
                t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (t == null)
                    return;

                E.Cast(t);
                R.Cast(t);
            }
            _isComboCompleted = true;

            t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            UserSummoners(t);
        }

        private static void Combo()
        {
            var cdQEx = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires;
            var cdWEx = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).CooldownExpires;
            var cdEEx = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).CooldownExpires;

            var cdQ = Game.Time < cdQEx ? cdQEx - Game.Time : 0;
            var cdW = Game.Time < cdWEx ? cdWEx - Game.Time : 0;
            var cdE = Game.Time < cdEEx ? cdEEx - Game.Time : 0;

            var t = TargetSelector.GetTarget(Q.Range * 2, DamageType.Magical);
            var useR = (comboMenu["DontCombo" + t.CharData.BaseSkinName] != null &&
                        getCheckBoxItem(comboMenu, "DontCombo" + t.CharData.BaseSkinName) == false);

            if (!t.LSIsValidTarget())
                return;

            if (vComboKill == ComboKill.WithoutW && !LeBlancStillJumped)
            {
                W.Cast(t.Position);
            }

            if (R.IsReady())
            {
                if (vComboType == ComboType.Auto)
                {
                    if (Q.Level > W.Level)
                    {
                        if (Q.IsReady())
                            ExecuteCombo();
                    }
                    else
                    {
                        if (W.IsReady() && !leBlancClone)
                            ExecuteCombo();
                    }
                }
                else if ((vComboType == ComboType.ComboQR && Q.IsReady()) ||
                         (vComboType == ComboType.ComboWR && W.IsReady()) ||
                         (vComboType == ComboType.ComboER && E.IsReady()))
                    ExecuteCombo();
                else
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM") // R-Q
                    {
                        t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                        if (t.LSIsValidTarget(Q.Range) &&
                            t.Health < GetRQDamage + ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q))
                            R.CastOnUnit(t);
                    }
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM") // R-W
                    {
                        t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                        if (t.LSIsValidTarget(W.Range) &&
                            t.Health < GetRQDamage + ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q))
                            R.Cast(t, false, true);
                    }
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM") // R-E
                    {
                        t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                        if (t.LSIsValidTarget(E.Range) &&
                            t.Health < GetRQDamage + ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q))
                            R.CastIfHitchanceEquals(t, GetEHitChance);
                    }
                    _isComboCompleted = true;
                }
                return;
            }

            if (Q.IsReady() && t.LSIsValidTarget(Q.Range) && _isComboCompleted)
            {
                if (vComboType == ComboType.ComboQR)
                {
                    if (!R.IsReady())
                        Q.CastOnUnit(t);
                }
                else
                {
                    Q.CastOnUnit(t);
                }
            }

            if (W.IsReady() && t.LSIsValidTarget(W.Range) && !LeBlancStillJumped && _isComboCompleted)
            {
                if (vComboType == ComboType.ComboWR)
                {
                    if (!R.IsReady() && !LeBlancStillJumped) W.Cast(t, true, true);
                }

                else
                {
                    W.Cast(t, true, true);
                }
            }

            if (E.IsReady() && t.LSIsValidTarget(E.Range) && _isComboCompleted)
            {
                if (vComboType == ComboType.ComboER)
                {
                    if (!R.IsReady())
                        E.CastIfHitchanceEquals(t, GetEHitChance);
                }
                else
                {
                    E.CastIfHitchanceEquals(t, GetEHitChance);
                }
            }

            if (IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (ObjectManager.Player.LSDistance(t) < 650 &&
                    ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite) >= t.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }

        }

        private static void Harass()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (!t.LSIsValidTarget())
            {
                return;
            }

            var useQ = getCheckBoxItem(harassMenu, "HarassUseQ") && ObjectManager.Player.ManaPercent >= getSliderItem(harassMenu, "HarassManaQ");
            var useW = getCheckBoxItem(harassMenu, "HarassUseW") && ObjectManager.Player.ManaPercent >= getSliderItem(harassMenu, "HarassManaW");
            var useE = getCheckBoxItem(harassMenu, "HarassUseE") && ObjectManager.Player.ManaPercent >= getSliderItem(harassMenu, "HarassManaE");

            if (useQ && Q.IsReady() && t.LSIsValidTarget(Q.Range))
            {
                Q.CastOnUnit(t);
            }

            if (useW && W.IsReady() && t.LSIsValidTarget(W.Range))
            {
                var wP = W.GetPrediction(t);
                var hithere = wP.CastPosition.Extend(ObjectManager.Player.Position, -50);
                if (wP.Hitchance >= HitChance.High)
                {
                    W.Cast(hithere);
                }
            }

            if (useE && E.IsReady() && t.LSIsValidTarget(E.Range))
            {
                E.CastIfHitchanceEquals(t, GetEHitChance);
            }
        }

        private static float GetRQDamage
        {
            get
            {
                var xDmg = 0f;
                var perDmg = new[] { 100f, 200f, 300 };

                xDmg += ((ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod) * .65f) +
                        perDmg[R.Level - 1];
                var t = TargetSelector.GetTarget(2000, DamageType.Magical);
                if (t.LSIsValidTarget(2000))
                    xDmg +=
                        (float)
                            ObjectManager.Player.GetSpellDamage(
                                t, (vComboType == ComboType.ComboQR ? SpellSlot.Q : SpellSlot.E));
                return xDmg;
            }
        }

        private static float GetRWDamage
        {
            get
            {
                var xDmg = 0f;
                var perDmg = new[] { 150f, 300f, 450f };
                xDmg += ((ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod) * .98f) +
                        perDmg[R.Level - 1];

                var t = TargetSelector.GetTarget(2000, DamageType.Magical);
                if (t.LSIsValidTarget(2000))
                    xDmg += (float)ObjectManager.Player.GetSpellDamage(t, SpellSlot.W);

                return xDmg;
            }
        }

        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (!t.LSIsValidTarget(2000))
                return 0f;

            fComboDamage += Q.IsReady() ? (float)ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q) : 0;

            fComboDamage += W.IsReady() ? (float)ObjectManager.Player.GetSpellDamage(t, SpellSlot.W) : 0;

            fComboDamage += E.IsReady() ? (float)ObjectManager.Player.GetSpellDamage(t, SpellSlot.E) : 0;

            if (R.IsReady())
            {
                if (vComboType == ComboType.ComboQR || vComboType == ComboType.ComboER)
                {
                    fComboDamage += GetRQDamage;
                }

                if (vComboType == ComboType.ComboWR)
                {
                    fComboDamage += GetRWDamage;
                }
            }

            fComboDamage += IgniteSlot != SpellSlot.Unknown &&
                            ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready
                ? (float)ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite)
                : 0f;

            fComboDamage += Items.CanUseItem(3092)
                ? (float)ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.FrostQueenClaim)
                : 0;

            return (float)fComboDamage;
        }

        private static bool xEnemyHaveSoulShackle(AIHeroClient vTarget)
        {
            return (vTarget.HasBuff("LeblancSoulShackle"));
        }

        private static void Run()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var useW = getCheckBoxItem(runMenu, "RunUseW");
            var useR = getCheckBoxItem(runMenu, "RunUseR");

            if (useW && W.IsReady() && !LeBlancStillJumped)
                W.Cast(Game.CursorPos);

            if (useR && R.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
                R.Cast(Game.CursorPos);
        }

        private static void DoubleStun()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (getKeyBindItem(comboMenu, "ComboDblStun"))
            {
                Drawing.DrawText(Drawing.Width * 0.45f, Drawing.Height * 0.78f, Color.Red, "Double Stun Active!");

                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            enemy =>
                                enemy.IsEnemy && !enemy.IsDead && enemy.IsVisible &&
                                ObjectManager.Player.LSDistance(enemy) < E.Range + 200 && !xEnemyHaveSoulShackle(enemy)))
                {
                    if (E.IsReady() && ObjectManager.Player.LSDistance(enemy) < E.Range)
                    {
                        E.CastIfHitchanceEquals(enemy, GetEHitChance);
                    }
                    else if (R.IsReady() && ObjectManager.Player.LSDistance(enemy) < E.Range &&
                             ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
                    {
                        R.CastIfHitchanceEquals(enemy, GetEHitChance);
                    }
                }
            }
        }

        private static void SmartW()
        {
            /*
            if (!Config.Item("ComboSmartW"))
                return;

            var vTarget = EnemyHaveSoulShackle;
            foreach (var existingSlide in ExistingSlide)
            {
                var slide = existingSlide;

                var onSlidePositionEnemyCount =
                    (from enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                enemy =>
                                    enemy.Team != ObjectManager.Player.Team && !enemy.IsDead &&
                                    enemy.LSDistance(slide.Position) < 300f)
                     select enemy).Count();

                var onPlayerPositionEnemyCount =
                    (from enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                enemy =>
                                    enemy.Team != ObjectManager.Player.Team &&
                                    ObjectManager.Player.LSDistance(enemy) < Q.Range)
                     select enemy).Count();


                if (Config.Item("ComboDblStun").GetValue<KeyBind>().Active && E.IsReady() && R.IsReady())
                {
                    var onPlayerPositionEnemyCount2 =
                        (from enemy in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(
                                    enemy =>
                                        enemy.Team != ObjectManager.Player.Team &&
                                        ObjectManager.Player.LSDistance(enemy) < E.Range)
                         select enemy).Count();

                    if (onPlayerPositionEnemyCount2 == 2) { }
                }
                if (onPlayerPositionEnemyCount > onSlidePositionEnemyCount)
                {
                    if (LeBlancStillJumped)
                    {
                        var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        if (qTarget == null)
                            return;
                        if (ObjectManager.Player.Health < qTarget.Health || ObjectManager.Player.Level < qTarget.Level && !LeBlancStillJumped)
                            W.Cast();
                        else
                        {
                            if (Q.IsReady())
                                Q.CastOnUnit(qTarget);
                            if (R.IsReady())
                                R.CastOnUnit(qTarget);
                            if (E.IsReady())
                                E.Cast(qTarget);
                            W.Cast();
                        }
                    }
                }
                Game.PrintChat(slide.Position.ToString());
                Render.Circle.DrawCircle(slide.Position, 400f, Color.Red);

                Game.PrintChat("Slide Pos. Enemy Count: " + onSlidePositionEnemyCount);
                Game.PrintChat("ObjectManager.Player Pos. Enemy Count: " + onPlayerPositionEnemyCount);

                Game.PrintChat("W Posision : " + existingSlide.Position);
                Game.PrintChat("Target Position : " + vTarget.Position);
            }
            */
        }


        private static void LaneClear()
        {
            if (!Orbwalker.CanMove)
                return;

            if (!LeBlancStillJumped)
            {
                return;
            }

            var useW = getCheckBoxItem(laneClearMenu, "LaneClearUseW");

            var xUseQ = getBoxItem(laneClearMenu, "LaneClearUseQ");
            if (Q.IsReady() && xUseQ != 0)
            {
                var minionsQ = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                if (xUseQ == 1 || xUseQ == 3)
                {
                    foreach (Obj_AI_Base vMinion in
                        from vMinion in minionsQ
                        let vMinionQDamage = ObjectManager.Player.GetSpellDamage(vMinion, SpellSlot.Q)
                        where
                            vMinion.Health <= vMinionQDamage &&
                            vMinion.Health > ObjectManager.Player.GetAutoAttackDamage(vMinion)
                        select vMinion)
                    {
                        Q.CastOnUnit(vMinion);
                    }

                }

                if (xUseQ == 2 || xUseQ == 3)
                {
                    foreach (
                        var minion in
                            minionsQ.Where(
                                m =>
                                    HealthPrediction.GetHealthPrediction(m,
                                        (int)(ObjectManager.Player.AttackCastDelay * 1000), Game.Ping / 2 - 100) < 0)
                                .Where(m => m.Health <= Q.GetDamage(m)))
                    {
                        Q.CastOnUnit(minion);
                    }
                }
            }

            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 20);
            if (!useW || !W.IsReady())
                return;

            var minionsW = W.GetCircularFarmLocation(rangedMinionsW, W.Width * 0.75f);

            if (minionsW.MinionsHit < 2 || !W.IsInRange(minionsW.Position.To3D()))
                return;

            W.Cast(minionsW.Position);

        }

        private static void JungleFarm()
        {
            var useQ = getCheckBoxItem(jungleMenu, "JungleFarmUseQ");
            var useW = getCheckBoxItem(jungleMenu, "JungleFarmUseW");
            var useE = getCheckBoxItem(jungleMenu, "JungleFarmUseE");

            var mobs = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0)
                return;
            var mob = mobs[0];
            if (useQ && Q.IsReady())
                Q.CastOnUnit(mob);

            if (useW && W.IsReady() && mobs.Count >= 2 && !LeBlancStillJumped)
                W.Cast(mob.Position);

            if (useE && E.IsReady())
                E.Cast(mob);
        }

        private static void DoToggleHarass()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (getKeyBindItem(harassMenu, "HarassUseTQ"))
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (t.LSIsValidTarget() && Q.IsReady() && ObjectManager.Player.ManaPercent >= getSliderItem(harassMenu, "HarassManaQ"))
                {
                    Q.CastOnUnit(t);
                }
            }

            if (getKeyBindItem(harassMenu, "HarassUseTW"))
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (t.LSIsValidTarget() && W.IsReady() && ObjectManager.Player.ManaPercent >= getSliderItem(harassMenu, "HarassManaW"))
                {
                    var wP = W.GetPrediction(t);
                    var hithere = wP.CastPosition.Extend(ObjectManager.Player.Position, -50);
                    if (wP.Hitchance >= HitChance.High)
                    {
                        W.Cast(hithere);
                    }
                }
            }

            if (getKeyBindItem(harassMenu, "HarassUseTE"))
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (t.LSIsValidTarget() && E.IsReady() && ObjectManager.Player.ManaPercent >= getSliderItem(harassMenu, "HarassManaE"))
                {
                    E.CastIfHitchanceEquals(t, GetEHitChance);
                }
            }
        }

        private static void RefreshComboType()
        {
            var xCombo = getBoxItem(comboMenu, "ComboMode");
            switch (xCombo)
            {
                case 0:
                    vComboType = Q.Level > W.Level ? ComboType.ComboQR : ComboType.ComboWR;
                    break;
                case 1: //Q-R
                    vComboType = ComboType.ComboQR;
                    break;
                case 2: //W-R
                    vComboType = ComboType.ComboWR;
                    break;
                case 3: //E-R
                    vComboType = ComboType.ComboER;
                    break;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            RefreshComboType();

            var t = TargetSelector.GetTarget(W.Range * 2, DamageType.Physical);
            {
                var xComboText = "Combo Kill";
                if (t.LSIsValidTarget(W.Range))
                {
                    if (t.Health < GetComboDamage(t))
                    {
                        vComboKill = ComboKill.FullCombo;
                        Drawing.DrawText(t.HPBarPosition.X + 145, t.HPBarPosition.Y + 20, Color.Beige, xComboText);
                    }
                }

                else if (t.LSIsValidTarget(W.Range * 2 - 30))
                {
                    if (t.Health < GetComboDamage(t) - ObjectManager.Player.GetSpellDamage(t, SpellSlot.W))
                    {
                        vComboKill = ComboKill.WithoutW;
                        xComboText = "Jump + " + xComboText;
                        Drawing.DrawText(t.HPBarPosition.X + 145, t.HPBarPosition.Y + 20, Color.Beige, xComboText);
                    }
                }
            }

            _isComboCompleted = !R.IsReady();

            if (getKeyBindItem(comboMenu, "ComboDblStun"))
                DoubleStun();

            if (getKeyBindItem(runMenu, "RunActive"))
                Run();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo();

            DoToggleHarass();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Harass();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (ObjectManager.Player.ManaPercent >= getSliderItem(laneClearMenu, "LaneClearMana"))
                    LaneClear();

                if (ObjectManager.Player.ManaPercent >= getSliderItem(jungleMenu, "JungleFarmMana"))
                    JungleFarm();
            }
        }

        #region Drawing_OnDraw

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "Show.JungleBuffs"))
            {
                foreach (var hero in HeroManager.AllHeroes)
                {
                    var jungleBuffs =
                        (from b in hero.Buffs
                         join b1 in CommonBuffManager.JungleBuffs on b.DisplayName equals b1.BuffName
                         select new { b, b1 }).Distinct();

                    foreach (var buffName in jungleBuffs.ToList())
                    {
                        var circle1 =
                            new CommonGeometry.Circle2(new Vector2(hero.Position.X + 3, hero.Position.Y - 3),
                                140 + (buffName.b1.Number * 20),
                                Game.Time - buffName.b.StartTime, buffName.b.EndTime - buffName.b.StartTime).ToPolygon();
                        circle1.Draw(Color.Black, 3);

                        var circle =
                            new CommonGeometry.Circle2(hero.Position.To2D(), 140 + (buffName.b1.Number * 20),
                                Game.Time - buffName.b.StartTime, buffName.b.EndTime - buffName.b.StartTime).ToPolygon();
                        circle.Draw(buffName.b1.Color, 3);
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "ComboShowInfo"))
            {
                var xComboStr = "Combo Mode: ";
                System.Drawing.Color color = Color.FromArgb(100, 255, 200, 37);

                var xCombo = getBoxItem(comboMenu, "ComboMode");
                switch (xCombo)
                {

                    case 0:
                        xComboStr += "Auto";
                        color = Color.FromArgb(100, 255, 200, 37);
                        break;

                    case 1: //Q-R
                        xComboStr += "Q - R";
                        color = Color.FromArgb(100, 4, 0, 255);
                        break;

                    case 2: //W-R
                        xComboStr += "W - R";
                        color = Color.FromArgb(100, 255, 0, 0);
                        break;

                    case 3: //E-R
                        xComboStr += "E - R";
                        color = Color.FromArgb(100, 0, 255, 8);
                        break;
                }

                Common.CommonGeometry.DrawText(CommonGeometry.Text, xComboStr, ObjectManager.Player.HPBarPosition.X + 150, ObjectManager.Player.HPBarPosition.Y + 75, SharpDX.Color.Wheat);
            }

            //if (Config.Item("HarassShowInfo"))
            //{
            //    var xHarassInfo = "";
            //    if (Config.Item("HarassUseTQ").GetValue<KeyBind>().Active)
            //        xHarassInfo += "Q - ";

            //    if (Config.Item("HarassUseTW").GetValue<KeyBind>().Active)
            //        xHarassInfo += "W - ";

            //    if (Config.Item("HarassUseTE").GetValue<KeyBind>().Active)
            //        xHarassInfo += "E - ";
            //    if (xHarassInfo.Length < 1)
            //    {
            //        xHarassInfo = "Harass Toggle: OFF   ";
            //    }
            //    else
            //    {
            //        xHarassInfo = "Harass Toggle: " + xHarassInfo;
            //    }
            //    xHarassInfo = xHarassInfo.Substring(0, xHarassInfo.Length - 3);
            //    //Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.82f, Color.Wheat, xHarassInfo);

            //    //Common.CommonGeometry.DrawBox(new Vector2((int)ObjectManager.Player.HPBarPosition.X + 145, (int)ObjectManager.Player.HPBarPosition.Y + 15), 125, 18, Color.FromArgb(100, 255, 200, 37), 1, Color.Black);
            //    //Common.CommonGeometry.DrawText(CommonGeometry.Text, xHarassInfo, ObjectManager.Player.HPBarPosition.X + 150, ObjectManager.Player.HPBarPosition.Y + 17, SharpDX.Color.Wheat);

            //}

            foreach (var spell in SpellList)
            {
                var menuItem = getCheckBoxItem(drawMenu, spell.Slot + "Range");
                if (menuItem && spell.Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.Honeydew,
                        spell.IsReady() ? 5 : 1);
                }
            }

            var wqRange = getCheckBoxItem(drawMenu, "WQRange");
            if (wqRange && Q.IsReady() && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range + Q.Range, Color.Honeydew, Q.IsReady() && W.IsReady() ? 5 : 1); 
            }

            var activeERange = getCheckBoxItem(drawMenu, "ActiveERange");
            if (activeERange && EnemyHaveSoulShackle != null)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1100f, Color.Honeydew);
            }
            /*
            var wObjPosition = Config.Item("WObjPosition");
            var wObjTimeTick = Config.Item("WObjTimeTick");

             * foreach (var existingSlide in ExistingSlide)
            {
                if (wObjPosition.Active)
                    Render.Circle.DrawCircle(existingSlide.Position, 110f, wObjPosition.Color);

                if (!wObjTimeTick) continue;
                if (!(existingSlide.ExpireTime > Game.Time)) continue;

                var time = TimeSpan.FromSeconds(existingSlide.ExpireTime - Game.Time);
                var pos = Drawing.WorldToScreen(existingSlide.Position);
                var display = string.Format("{0}:{1:D2}", time.Minutes, time.Seconds);
                Drawing.DrawText(pos.X - display.Length * 3, pos.Y - 65, Color.GreenYellow, display);
            }

            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            enemy =>
                                enemy.IsEnemy && !enemy.IsDead && enemy.IsVisible &&
                                ObjectManager.Player.LSDistance(enemy) < E.Range + 1400 &&
                                !xEnemyHaveSoulShackle(enemy))) 
            {
                Render.Circle.DrawCircle(enemy.Position, 75f, Color.GreenYellow);
            }
             */
        }

        #endregion


        #region GetEHitChance

        private static HitChance GetEHitChance
        {
            get
            {
                HitChance hitChance;
                var eHitChance = getBoxItem(comboMenu, "ComboSetEHitCh");
                switch (eHitChance)
                {
                    case 0:
                        {
                            hitChance = HitChance.Low;
                            break;
                        }
                    case 1:
                        {
                            hitChance = HitChance.Medium;
                            break;
                        }
                    case 2:
                        {
                            hitChance = HitChance.High;
                            break;
                        }
                    case 3:
                        {
                            hitChance = HitChance.VeryHigh;
                            break;
                        }
                    case 4:
                        {
                            hitChance = HitChance.Immobile;
                            break;
                        }
                    default:
                        {
                            hitChance = HitChance.High;
                            break;
                        }
                }
                return hitChance;
            }
        }

        #endregion
    }
}
