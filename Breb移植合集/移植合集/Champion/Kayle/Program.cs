using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace SephKayle
{
    internal class Program
    {
        private static Menu Config, comboMenu, harassMenu, clearMenu, farmMenu, healMenu, ultMenu, miscMenu, drawMenu;
        private static AIHeroClient Player;
        private static readonly float incrange = 525;
        private static Spell Q, W, E, R, Ignite;

        private static bool Eon
        {
            get { return ObjectManager.Player.AttackRange > 400f; }
        }

        private static void CreateMenu()
        {
            Config = MainMenu.AddMenu("SephKayle", "SephKayle");

            // Combo Options
            comboMenu = Config.AddSubMenu("Combo", " Combo");
            comboMenu.Add("UseQ", new CheckBox("Use Q"));
            comboMenu.Add("UseW", new CheckBox("Use W"));
            comboMenu.Add("UseE", new CheckBox("Use E"));
            comboMenu.Add("UseR", new CheckBox("Use R"));

            // Harass
            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("Harass.Mode", new ComboBox("Harass Mode", 0, "Only Mixed", "Always"));
            harassMenu.Add("Harass.Mana", new Slider("Min Mana %", 30, 1));
            harassMenu.Add("Harass.Q", new CheckBox("Use Q"));


            // Waveclear Options
            clearMenu = Config.AddSubMenu("Clear", "Clear");
            clearMenu.Add("WC.Mana", new Slider("Min Mana %", 30, 1));
            clearMenu.Add("UseQwc", new CheckBox("Use Q"));
            clearMenu.Add("UseEwc", new CheckBox("Use E"));

            // Farm Options
            farmMenu = Config.AddSubMenu("Last Hit", "LH");
            farmMenu.Add("UseQfarm", new CheckBox("Use Q"));
            farmMenu.Add("UseEfarm", new CheckBox("Use E"));

            // HealManager Options
            healMenu = Config.AddSubMenu("HealManager", "Heal Manager");
            healMenu.Add("onlyhincdmg", new CheckBox("Only heal if incoming damage", false));
            healMenu.Add("hdamagedetection", new CheckBox("Disable damage detection", false));
            healMenu.Add("hcheckdmgafter", new CheckBox("Take HP after damage into consideration"));
            healMenu.AddSeparator();
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly))
            {
                healMenu.Add("heal" + hero.ChampionName, new CheckBox("Heal " + hero.ChampionName));
                healMenu.Add("hpct" + hero.ChampionName, new Slider("Health % " + hero.ChampionName, 35));
                healMenu.AddSeparator();
            }

            // UltimateManager Options
            ultMenu = Config.AddSubMenu("UltManager", "Ultimate Manager");
            ultMenu.Add("onlyuincdmg", new CheckBox("Only ult if incoming damage"));
            ultMenu.Add("udamagedetection", new CheckBox("Disable damage detection", false));
            ultMenu.Add("ucheckdmgafter", new CheckBox("Take HP after damage into consideration"));
            ultMenu.AddSeparator();
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly))
            {
                ultMenu.Add("ult" + hero.ChampionName, new CheckBox("Ultimate " + hero.ChampionName));
                ultMenu.Add("upct" + hero.ChampionName, new Slider("Health % " + hero.ChampionName, 25));
                ultMenu.AddSeparator();
            }

            // Misc Options
            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("killsteal", new CheckBox("Killsteal"));
            miscMenu.Add("UseElh", new CheckBox("Use E to lasthit"));
            miscMenu.Add("Healingon", new CheckBox("Healing On"));
            miscMenu.Add("Ultingon", new CheckBox("Ulting On"));
            miscMenu.Add("Recallcheck", new CheckBox("Recall check", false));
            miscMenu.Add("Debug", new CheckBox("Debug On", false));

            drawMenu = Config.AddSubMenu("Drawing", "Drawing");
            drawMenu.Add("disableall", new CheckBox("Disable all"));
            drawMenu.Add("DrawQ", new CheckBox("Draw Q"));
            drawMenu.Add("DrawW", new CheckBox("Draw W"));
            drawMenu.Add("DrawE", new CheckBox("Draw E"));
            drawMenu.Add("DrawR", new CheckBox("Draw R"));
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

        private static HarassMode GetHMode()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                return HarassMode.None;
            }
            var selindex = getBoxItem(harassMenu, "Harass.Mode");
            if (selindex == 0)
            {
                return HarassMode.Mixed;
            }
            return HarassMode.Always;
        }

        private static bool debug()
        {
            return getCheckBoxItem(miscMenu, "Debug");
        }

        public static void OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (Player.CharData.BaseSkinName != "Kayle")
            {
                return;
            }
            CreateMenu();
            DefineSpells();
            Game.OnUpdate += GameTick;
            Obj_AI_Base.OnProcessSpellCast += HealUltTrigger;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "disableall"))
            {
                return;
            }

            if (getCheckBoxItem(drawMenu, "DrawQ"))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
            }
            if (getCheckBoxItem(drawMenu, "DrawW"))
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Azure);
            }
            if (getCheckBoxItem(drawMenu, "DrawE"))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Crimson);
            }
            if (getCheckBoxItem(drawMenu, "DrawR"))
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Red);
            }
        }

        private static void KillSteal()
        {
            var target = ObjectManager.Get<AIHeroClient>()
                .Where(
                    x =>
                        x.IsInvulnerable && !x.IsDead && x.IsEnemy && !x.IsZombie && x.IsValidTarget() &&
                        x.Distance(Player.Position) <= 800)
                .OrderBy(x => x.Health).FirstOrDefault();
            if (target != null)
            {
                var igniteDmg = Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                double QDmg = Player.GetSpellDamage(target, SpellSlot.Q);
                var totalksdmg = igniteDmg + QDmg;

                if (target.Health <= QDmg && Player.Distance(target) <= Q.Range)
                {
                    Q.CastOnUnit(target);
                }
                if (target.Health <= igniteDmg && Player.Distance(target) <= Ignite.Range)
                {
                    Player.Spellbook.CastSpell(Ignite.Slot, target);
                }
                if (target.Health <= totalksdmg && Player.Distance(target) <= Q.Range)
                {
                    Q.CastOnUnit(target);
                    Player.Spellbook.CastSpell(Ignite.Slot, target);
                }
            }
        }

        private static void Combo()
        {
            var qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var etarget = TargetSelector.GetTarget(incrange, DamageType.Magical);

            if (getCheckBoxItem(comboMenu, "UseQ") && qtarget != null && Q.IsReady())
            {
                Q.Cast(qtarget);
            }
            if (getCheckBoxItem(comboMenu, "UseE") && etarget != null && E.IsReady() && !Eon)
            {
                E.CastOnUnit(Player);
            }
        }

        private static void WaveClear()
        {
            if (Player.ManaPercent < getSliderItem(clearMenu, "WC.Mana"))
            {
                return;
            }

            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && Player.Distance(m) <= incrange);
            if (minions.Any() && getCheckBoxItem(clearMenu, "UseEwc") && E.IsReady() && !Eon)
            {
                E.CastOnUnit(Player);
            }

            if (getCheckBoxItem(clearMenu, "UseQwc") && Q.IsReady())
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                var vminions = allMinions.Where(
                    minion =>
                        minion.IsValidTarget() && Player.Distance(minion) >
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) && Player.Distance(minion) <= Q.Range &&
                        HealthPrediction.GetHealthPrediction(minion,
                            (int) (Player.Distance(minion)*1000/1500) + 300 + Game.Ping/2) <
                        0.75*Player.GetSpellDamage(minion, SpellSlot.Q));
                var bestminion = vminions.MaxOrDefault(x => x.MaxHealth);
                if (bestminion != null)
                {
                    //Orbwalker.SetAttack(false);
                    Orbwalker.DisableAttacking = true;
                    Q.CastOnUnit(bestminion);
                    Orbwalker.DisableAttacking = false;
                    //Orbwalker.SetAttack(true);
                }
            }
        }


        private static void HealUltTrigger(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (getCheckBoxItem(miscMenu, "Recallcheck") && Player.IsRecalling())
            {
                return;
            }

            var target = args.Target as AIHeroClient;
            var senderhero = sender as AIHeroClient;
            var senderturret = sender as Obj_AI_Turret;

            if (sender.IsAlly || (target == null) || !target.IsAlly)
            {
                return;
            }
            float setvaluehealth = getSliderItem(healMenu, "hpct" + target.ChampionName);
            float setvalueult = getSliderItem(ultMenu, "upct" + target.ChampionName);

            var triggered = false;

            if (W.IsReady() && getCheckBoxItem(healMenu, "heal" + target.ChampionName) &&
                (target.HealthPercent <= setvaluehealth))
            {
                HealUltManager(true, false, target);
                triggered = true;
            }
            if (R.IsReady() && getCheckBoxItem(ultMenu, "ult" + target.ChampionName) &&
                (target.HealthPercent <= setvalueult) && target.Distance(Player) <= R.Range)
            {
                if (args.SData.Name.ToLower().Contains("minion") && target.HealthPercent > 5)
                {
                    return;
                }
                if (debug())
                {
                    Chat.Print("Ult target: " + target.ChampionName +
                               " Ult reason: Target hp percent below set value of: " + setvalueult +
                               " Current value is: " + target.HealthPercent + " Triggered by: Incoming spell: + " +
                               args.SData.Name);
                }
                HealUltManager(false, true, target);
                triggered = true;
            }

            if (triggered)
            {
                return;
            }

            var damage = sender.LSGetSpellDamage(target, args.SData.Name);
            var afterdmg = (target.Health - damage)/target.MaxHealth*100f;

            if (W.IsReady() && Player.Distance(target) <= W.Range &&
                getCheckBoxItem(healMenu, "heal" + target.ChampionName) &&
                (target.HealthPercent <= setvaluehealth ||
                 (getCheckBoxItem(healMenu, "hcheckdmgafter") && afterdmg <= setvaluehealth)))
            {
                if (getCheckBoxItem(healMenu, "hdamagedetection"))
                {
                    HealUltManager(true, false, target);
                }
            }

            if (R.IsReady() && Player.Distance(target) <= R.Range &&
                getCheckBoxItem(ultMenu, "ult" + target.ChampionName) &&
                (target.HealthPercent <= setvalueult ||
                 (getCheckBoxItem(ultMenu, "ucheckdmgafter") && afterdmg <= setvalueult)) &&
                (senderhero != null || senderturret != null || target.HealthPercent < 5f))
            {
                if (getCheckBoxItem(ultMenu, "udamagedetection"))
                {
                    if (args.SData.Name.ToLower().Contains("minion") && target.HealthPercent > 5)
                    {
                        return;
                    }
                    if (debug())
                    {
                        if (afterdmg <= setvalueult)
                        {
                            Chat.Print("Ult target: " + target.ChampionName +
                                       " Ult reason: Incoming spell damage will leave us below set value of " +
                                       setvalueult + " Current value is: " + target.HealthPercent +
                                       " and after spell health left is: " + afterdmg +
                                       " Triggered by: Incoming spell: + " + args.SData.Name);
                        }

                        else
                        {
                            Chat.Print("Ult target: " + target.ChampionName +
                                       " Ult reason: Incoming spell damage and health below set value of " + setvalueult +
                                       " Current value is: " + target.HealthPercent +
                                       " Triggered by: Incoming spell: + " + args.SData.Name);
                        }
                    }
                    HealUltManager(false, true, target);
                }
            }
        }


        private static void HealUltManager(bool forceheal = false, bool forceult = false, AIHeroClient target = null)
        {
            if (forceheal && target != null && W.IsReady() && Player.Distance(target) <= W.Range)
            {
                W.CastOnUnit(target);
                return;
            }
            if (forceult && target != null && R.IsReady() && Player.Distance(target) <= R.Range)
            {
                if (debug())
                {
                    Chat.Print("Forceult");
                }
                R.CastOnUnit(target);
                return;
            }

            if (getCheckBoxItem(miscMenu, "Healingon") && !getCheckBoxItem(healMenu, "onlyhincdmg"))
            {
                var herolistheal = ObjectManager.Get<AIHeroClient>()
                    .Where(
                        h =>
                            (h.IsAlly || h.IsMe) && !h.IsZombie && !h.IsDead &&
                            getCheckBoxItem(healMenu, "heal" + h.ChampionName) &&
                            h.HealthPercent <= getSliderItem(healMenu, "hpct" + h.ChampionName) &&
                            Player.Distance(h) <= R.Range)
                    .OrderByDescending(i => i.IsMe)
                    .ThenBy(i => i.HealthPercent);

                if (W.IsReady())
                {
                    if (herolistheal.Contains(Player) && !Player.IsRecalling() && !Player.InFountain())
                    {
                        W.CastOnUnit(Player);
                        return;
                    }
                    if (herolistheal.Any())
                    {
                        var hero = herolistheal.FirstOrDefault();

                        if (Player.Distance(hero) <= R.Range && !Player.IsRecalling() && !hero.IsRecalling() &&
                            !hero.InFountain())
                        {
                            W.CastOnUnit(hero);
                            return;
                        }
                    }
                }
            }

            if (getCheckBoxItem(miscMenu, "Ultingon") && !getCheckBoxItem(ultMenu, "onlyuincdmg"))
            {
                Console.WriteLine(Player.HealthPercent);
                var herolist = ObjectManager.Get<AIHeroClient>()
                    .Where(
                        h =>
                            (h.IsAlly || h.IsMe) && !h.IsZombie && !h.IsDead &&
                            getCheckBoxItem(ultMenu, "ult" + h.ChampionName) &&
                            h.HealthPercent <= getSliderItem(ultMenu, "upct" + h.ChampionName) &&
                            Player.Distance(h) <= R.Range && Player.CountEnemiesInRange(500) > 0)
                    .OrderByDescending(i => i.IsMe)
                    .ThenBy(i => i.HealthPercent);

                if (R.IsReady())
                {
                    if (herolist.Contains(Player))
                    {
                        if (debug())
                        {
                            Chat.Print("regultself");
                        }
                        R.CastOnUnit(Player);
                    }

                    else if (herolist.Any())
                    {
                        var hero = herolist.FirstOrDefault();

                        if (Player.Distance(hero) <= R.Range)
                        {
                            if (debug())
                            {
                                Chat.Print("regultotherorself");
                            }
                            R.CastOnUnit(hero);
                        }
                    }
                }
            }
        }

        private static void GameTick(EventArgs args)
        {
            if (Player.IsDead || getCheckBoxItem(miscMenu, "Recallcheck") && Player.IsRecalling())
            {
                return;
            }

            if (GetHMode() == HarassMode.Always)
            {
                Harass();
            }

            if (!getCheckBoxItem(healMenu, "onlyhincdmg") || !getCheckBoxItem(ultMenu, "onlyuincdmg"))
            {
                HealUltManager();
            }

            if (getCheckBoxItem(miscMenu, "killsteal"))
            {
                KillSteal();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                WaveClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                MixedLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LHlogic();
            }
        }


        private static void LHlogic()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var vminions = allMinions.Where(
                minion =>
                    minion.IsValidTarget() && Player.Distance(minion) >
                    Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) && Player.Distance(minion) <= Q.Range &&
                    HealthPrediction.GetHealthPrediction(minion,
                        (int) (Player.Distance(minion)*1000/1500) + 300 + Game.Ping/2) <
                    0.75*Player.GetSpellDamage(minion, SpellSlot.Q));

            if (getCheckBoxItem(farmMenu, "UseQfarm") && Q.IsReady())
            {
                var bestminion = vminions.MaxOrDefault(x => x.MaxHealth);
                if (bestminion != null)
                {
                    Orbwalker.DisableAttacking = true;
                    Q.CastOnUnit(bestminion);
                    Orbwalker.DisableAttacking = false;
                }
            }


            if (getCheckBoxItem(farmMenu, "UseEfarm") && E.IsReady() && !Eon)
            {
                var minions =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            m =>
                                m.IsValidTarget() && m.Team != Player.Team && Player.Distance(m) <= incrange &&
                                HealthPrediction.GetHealthPrediction(m,
                                    (int) (Player.Distance(m)*1000/1500) + 300 + Game.Ping/2) <
                                0.75*Player.GetAutoAttackDamage(m));
                if (minions.Any())
                {
                    E.CastOnUnit(Player);
                }
            }
            //TODO Better Calculations + More Logic for E activation
        }


        private static void MixedLogic()
        {
            if (GetHMode() == HarassMode.Mixed)
            {
                Harass();
            }

            if (getCheckBoxItem(farmMenu, "UseEfarm") && E.IsReady())
            {
                var minions =
                    ObjectManager.Get<Obj_AI_Base>().Where(m => m.Team != Player.Team && Player.Distance(m) <= incrange);
                if (minions.Any() && getCheckBoxItem(farmMenu, "UseEfarm") && !Eon)
                {
                    E.CastOnUnit(Player);
                }
            }

            if (getCheckBoxItem(farmMenu, "UseQfarm") && Q.IsReady())
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                var vminions = allMinions.Where(
                    minion =>
                        minion.IsValidTarget() && Player.Distance(minion) >
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) && Player.Distance(minion) <= Q.Range &&
                        HealthPrediction.GetHealthPrediction(minion,
                            (int) (Player.Distance(minion)*1000/1500) + 300 + Game.Ping/2) <
                        0.75*Player.GetSpellDamage(minion, SpellSlot.Q));

                var bestminion = vminions.MaxOrDefault(x => x.MaxHealth);
                if (bestminion != null)
                {
                    Orbwalker.DisableAttacking = true;
                    Q.CastOnUnit(bestminion);
                    Orbwalker.DisableAttacking = false;
                }
            }

            //TODO Better Calculations + More Logic for E activation
        }

        private static void Harass()
        {
            if (Player.ManaPercent < getSliderItem(harassMenu, "Harass.Mana"))
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "Harass.Q"))
            {
                var Targ = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (Targ != null && Q.IsReady() && Player.Distance(Targ) <= Q.Range)
                {
                    Q.Cast(Targ);
                }
            }
        }


        private static void DefineSpells()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 900);
            var ignite = ObjectManager.Player.Spellbook.GetSpell(ObjectManager.Player.GetSpellSlot("summonerdot"));
            if (ignite.Slot != SpellSlot.Unknown)
            {
                Ignite = new Spell(ignite.Slot, 600);
            }
        }

        private enum HarassMode
        {
            Mixed,
            Always,
            None
        }
    }
}