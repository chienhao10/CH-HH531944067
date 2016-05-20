using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using Version = System.Version;

namespace SephLissandra
{
    internal static class Lissandra
    {
        public static Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public static AIHeroClient Player;
        public static bool jumping;
        private static Vector2 MissilePosition;
        private static MissileClient LissEMissile;
        public static Menu Config = LissMenu.Config;
        public static Menu comboMenu = LissMenu.comboMenu;
        public static Menu ksMenu = LissMenu.ksMenu;
        public static Menu harassMenu = LissMenu.harassMenu;
        public static Menu lastHitMenu = LissMenu.lastHitMenu;
        public static Menu clearMenu = LissMenu.clearMenu;
        public static Menu interruptMenu = LissMenu.interruptMenu;
        public static Menu blackListMenu = LissMenu.blackListMenu;
        public static Menu miscMenu = LissMenu.miscMenu;
        public static Menu drawMenu = LissMenu.drawMenu;

        private static Dictionary<string, Spell> Spells;

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

        public static void OnLoad()
        {
            Player = ObjectManager.Player;
            DefineSpells();
            if (Player.CharData.BaseSkinName != "Lissandra")
            {
                return;
            }
            LissMenu.CreateMenu();
            Game.OnUpdate += GameTick;
            Game.OnUpdate += MonitorMissilePosition;
            AntiGapcloser.OnEnemyGapcloser += OnGapClose;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnDraw += OnDraw;

            Config = LissMenu.Config;
            comboMenu = LissMenu.comboMenu;
            ksMenu = LissMenu.ksMenu;
            harassMenu = LissMenu.harassMenu;
            lastHitMenu = LissMenu.lastHitMenu;
            clearMenu = LissMenu.clearMenu;
            interruptMenu = LissMenu.interruptMenu;
            blackListMenu = LissMenu.blackListMenu;
            miscMenu = LissMenu.miscMenu;
            drawMenu = LissMenu.drawMenu;
        }


        private static void DefineSpells()
        {
            Spells = new Dictionary<string, Spell>
            {
                {"Q", new Spell(SpellSlot.Q, 715f)},
                {"Qtest", new Spell(SpellSlot.Q, 715f)},
                {"Q2", new Spell(SpellSlot.Q, 825f)},
                {"W", new Spell(SpellSlot.W, 450f)},
                {"E", new Spell(SpellSlot.E, 1050f)},
                {"R", new Spell(SpellSlot.R, 550f)},
                {"Ignite", new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600)}
            };
            Spells["Q"].SetSkillshot(0.250f, 75f, 2200f, false, SkillshotType.SkillshotLine);
            Spells["Qtest"].SetSkillshot(0.250f, 75f, 2200f, true, SkillshotType.SkillshotLine);
            Spells["Q2"].SetSkillshot(0.250f, 90f, 2200f, false, SkillshotType.SkillshotLine);
            Spells["E"].SetSkillshot(0.250f, 125f, 850f, false, SkillshotType.SkillshotLine);
        }


        private static void GameTick(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (getKeyBindItem(miscMenu, "Misc.EMouse"))
            {
                EToMouse(Game.CursorPos);
            }

            if (getCheckBoxItem(ksMenu, "Killsteal"))
            {
                KillSteal();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ComboHandler();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                getKeyBindItem(harassMenu, "Keys.HarassT"))
            {
                HarassHandler();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                FarmHandler();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClearHandler();
            }
        }

        private static void EToMouse(Vector3 Position)
        {
            if (Spells["E"].IsReady() && LissEMissile == null && !LissUtils.CanSecondE())
            {
                Spells["E"].Cast(Position);
                jumping = true;
            }
        }


        private static void MonitorMissilePosition(EventArgs args)
        {
            if (LissEMissile == null || Player.IsDead)
            {
                return;
            }
            MissilePosition = LissEMissile.Position.To2D();
            if (jumping)
            {
                if (Vector2.Distance(MissilePosition, LissEMissile.EndPosition.To2D()) < 40)
                {
                    Spells["E"].CastOnUnit(Player);
                    jumping = false;
                }
                Utility.DelayAction.Add(2000, delegate { jumping = false; });
            }
        }


        private static void ComboHandler()
        {
            var Target = TargetSelector.GetTarget(Spells["E"].Range * 0.94f, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget())
            {
                Target =
                    HeroManager.Enemies.FirstOrDefault(
                        h =>
                            h.IsValidTarget() &&
                            (Vector3.Distance(h.ServerPosition, Player.ServerPosition) < Spells["E"].Range * 0.94) &&
                            !h.IsZombie);
            }

            if (Target != null && !Target.IsInvulnerable)
            {
                if (getCheckBoxItem(comboMenu, "Combo.UseQ") && SpellSlot.Q.IsReady())
                {
                    CastQ(Target);
                }
                if (getCheckBoxItem(comboMenu, "Combo.UseW") && SpellSlot.W.IsReady())
                {
                    CastW(Target);
                }
                if (getCheckBoxItem(comboMenu, "Combo.UseE") && SpellSlot.E.IsReady())
                {
                    CastE(Target);
                }
                if (getCheckBoxItem(comboMenu, "Combo.UseR") && SpellSlot.R.IsReady() && !Target.IsZombie)
                {
                    CastR(Target);
                }
            }
        }


        private static void OnGapClose(ActiveGapcloser args)
        {
            if (Player.IsDead)
            {
                return;
            }
            var sender = args.Sender;

            if (getCheckBoxItem(interruptMenu, "Interrupter.AntiGapClose") && sender.IsValidTarget())
            {
                if (getCheckBoxItem(interruptMenu, "Interrupter.AG.UseW") &&
                    Vector3.Distance(args.End, Player.ServerPosition) <= Spells["W"].Range)
                {
                    Utility.DelayAction.Add(300, () => Spells["W"].CastOnUnit(Player));
                    return;
                }
                if (getCheckBoxItem(interruptMenu, "Interrupter.AG.UseR") &&
                    !getCheckBoxItem(blackListMenu, "Blacklist." + sender.NetworkId) &&
                    Vector3.Distance(sender.ServerPosition, Player.ServerPosition) <= Spells["R"].Range)
                {
                    Spells["R"].Cast(sender);
                }
            }
        }


        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (getCheckBoxItem(interruptMenu, "Interrupter") && sender.IsValidTarget())
            {
                if (getCheckBoxItem(interruptMenu, "Interrupter.UseW") &&
                    Vector3.Distance(sender.ServerPosition, Player.ServerPosition) <= Spells["W"].Range)
                {
                    Spells["W"].CastOnUnit(Player);
                    return;
                }
                if (getCheckBoxItem(interruptMenu, "Interrupter.UseR") &&
                    !getCheckBoxItem(blackListMenu, "Blacklist." + sender.NetworkId) &&
                    Vector3.Distance(sender.ServerPosition, Player.ServerPosition) <= Spells["R"].Range)
                {
                    Spells["R"].Cast(sender);
                }
            }
        }

        private static void CastQ(AIHeroClient target)
        {
            var maxhit = (from hero in
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget() && !h.IsInvulnerable &&
                        Vector3.Distance(h.ServerPosition, Player.ServerPosition) < Spells["Q2"].Range)
                          select Spells["Q2"].GetPrediction(hero)
                into prediction
                          where
                              prediction.CollisionObjects.Count > 0 &&
                              prediction.Hitchance >= LissUtils.GetHitChance("Hitchance.Q")
                          let enemieshit = prediction.CollisionObjects.Where(x => x is AIHeroClient)
                          select prediction).ToDictionary(prediction => prediction.CastPosition,
                    prediction => prediction.CollisionObjects.Count);

            var bestpair = maxhit.MaxOrDefault(x => x.Value);
            if (bestpair.Value > 0)
            {
                var bestpos = bestpair.Key;
                Spells["Q2"].Cast(bestpos);
                return;
            }


            var distbw = Vector3.Distance(Player.ServerPosition, target.ServerPosition);

            if (distbw < Spells["Q"].Range)
            {
                var prediction2 = Spells["Q"].GetPrediction(target);
                if (prediction2.Hitchance >= LissUtils.GetHitChance("Hitchance.Q"))
                {
                    Spells["Q"].Cast(target);
                    return;
                }
            }

            if (distbw > Spells["Qtest"].Range && distbw < Spells["Q2"].Range)
            {
                var testQ = Spells["Qtest"].GetPrediction(target);
                var collobjs = testQ.CollisionObjects;
                if ((testQ.Hitchance == HitChance.Collision || collobjs.Count > 0) && collobjs.All(x => x.IsTargetable))
                {
                    var pred = Spells["Q2"].GetPrediction(target);
                    if (pred.Hitchance >= LissUtils.GetHitChance("Hitchance.Q"))
                    {
                        Spells["Q2"].Cast(target);
                    }
                }
            }
        }

        private static void CastW(AIHeroClient target)
        {
            if (Vector3.Distance(target.ServerPosition, Player.ServerPosition) <= Spells["W"].Range)
            {
                Spells["W"].CastOnUnit(Player);
                return;
            }
            if (
                HeroManager.Enemies.Any(
                    h =>
                        h.IsValidTarget() &&
                        (Vector3.Distance(h.ServerPosition, Player.ServerPosition) < Spells["W"].Range) && !h.IsZombie))
            {
                Spells["W"].CastOnUnit(Player);
            }
        }

        private static void CastE(AIHeroClient target)
        {
            if (LissEMissile == null && !LissUtils.CanSecondE())
            {
                var PredManager =
                    HeroManager.Enemies.Where(
                        h =>
                            h.IsValidTarget() && !h.IsZombie &&
                            Vector3.Distance(h.ServerPosition, Player.ServerPosition) <= Spells["E"].Range)
                        .Select(hero => Spells["E"].GetPrediction(hero))
                        .Select(
                            pred =>
                                new Tuple<Vector3, int, HitChance, List<AIHeroClient>>(pred.CastPosition,
                                    pred.AoeTargetsHitCount, pred.Hitchance, pred.AoeTargetsHit));

                var BestLocation = PredManager.MaxOrDefault(x => x.Item4.Count);
                if (BestLocation.Item3 >= LissUtils.GetHitChance("Hitchance.E") && Spells["E"].IsReady())
                {
                    Spells["E"].Cast(BestLocation.Item1);
                }
            }
            SecondEChecker(target);
        }

        //return asap to check the most amount of times 
        private static void SecondEChecker(AIHeroClient target)
        {
            if (LissUtils.AutoSecondE() && LissUtils.isHealthy() && LissEMissile != null && Spells["E"].IsReady())
            {
                if (Vector2.Distance(MissilePosition, target.ServerPosition.To2D()) <
                    Vector3.Distance(Player.ServerPosition, target.ServerPosition) &&
                    !LissUtils.PointUnderEnemyTurret(MissilePosition) &&
                    Vector3.Distance(target.ServerPosition, LissEMissile.EndPosition) >
                    Vector3.Distance(Player.ServerPosition, target.ServerPosition))
                {
                    Spells["E"].CastOnUnit(Player);
                    return;
                }
                var Enemiesatpoint = LissEMissile.Position.GetEnemiesInRange(Spells["R"].Range);
                var enemiesatpointR = Enemiesatpoint.Count;

                if ((enemiesatpointR >= getSliderItem(comboMenu, "Combo.ecountR") && SpellSlot.R.IsReady()) ||
                    Enemiesatpoint.Any(
                        e =>
                            e.IsKillableFromPoint(LissEMissile.Position) &&
                            Vector3.Distance(LissEMissile.Position, e.ServerPosition) <
                            Vector3.Distance(Player.ServerPosition, e.ServerPosition)))
                {
                    if (LissUtils.PointUnderEnemyTurret(MissilePosition) &&
                        getCheckBoxItem(miscMenu, "Misc.DontETurret"))
                    {
                        return;
                    }
                    Spells["E"].CastOnUnit(Player);
                    return;
                }
                var enemiesatpointW = LissEMissile.Position.CountEnemiesInRange(Spells["W"].Range);
                if (enemiesatpointW >= getSliderItem(comboMenu, "Combo.ecountW") && SpellSlot.W.IsReady())
                {
                    if (LissUtils.PointUnderEnemyTurret(MissilePosition) &&
                        getCheckBoxItem(miscMenu, "Misc.DontETurret"))
                    {
                        return;
                    }
                    Spells["E"].CastOnUnit(Player);
                }
            }
        }

        public static bool IsKillableFromPoint(this AIHeroClient target, Vector3 Point, bool ExcludeE = false)
        {
            double totaldmgavailable = 0;
            if (SpellSlot.Q.IsReady() && getCheckBoxItem(comboMenu, "Combo.UseQ") &&
                Vector3.Distance(Point, target.ServerPosition) < Spells["Q"].Range + 35)
            {
                totaldmgavailable += Player.GetSpellDamage(target, SpellSlot.Q);
            }
            if (SpellSlot.W.IsReady() && getCheckBoxItem(comboMenu, "Combo.UseW") &&
                Vector3.Distance(Point, target.ServerPosition) < Spells["W"].Range + 35)
            {
                totaldmgavailable += Player.GetSpellDamage(target, SpellSlot.W);
            }
            if (SpellSlot.E.IsReady() && getCheckBoxItem(comboMenu, "Combo.UseE") &&
                Vector3.Distance(Point, target.ServerPosition) < Spells["E"].Range + 35 && !LissUtils.CanSecondE() &&
                LissEMissile == null && !ExcludeE)
            {
                totaldmgavailable += Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (SpellSlot.R.IsReady() && getCheckBoxItem(comboMenu, "Combo.UseR") &&
                Vector3.Distance(Point, target.ServerPosition) < Spells["Q"].Range + 35)
            {
                totaldmgavailable += Player.GetSpellDamage(target, SpellSlot.R);
            }

            if (Spells["Ignite"].IsReady() && getCheckBoxItem(ksMenu, "Killsteal.UseIgnite") &&
                Vector3.Distance(Point, target.ServerPosition) < Spells["Ignite"].Range + 15)
            {
                totaldmgavailable += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }
            return totaldmgavailable > target.Health;
        }


        private static void CastR(AIHeroClient currenttarget)
        {
            var Check =
                HeroManager.Enemies
                    .Where(
                        h =>
                            h.IsValidTarget(Spells["R"].Range) &&
                            h.CountEnemiesInRange(Spells["R"].Range) >= getSliderItem(comboMenu, "Combo.Rcount") &&
                            !getCheckBoxItem(blackListMenu, "Blacklist." + h.NetworkId) && h.HealthPercent > getSliderItem(comboMenu, "Combo.MinRHealth")).ToList();

            if (Player.CountEnemiesInRange(Spells["R"].Range) >= getSliderItem(comboMenu, "Combo.Rcount"))
            {
                Check.Add(Player);
            }
            if (Check.Any())
            {
                if (Check.Contains(Player) && !LissUtils.isHealthy())
                {
                    Spells["R"].CastOnUnit(Player);
                    return;
                }
                var target = Check.MaxOrDefault(TargetSelector.GetPriority);
                if (target != null)
                {
                    Spells["R"].Cast(target);
                    return;
                }
            }
            if (getCheckBoxItem(blackListMenu, "Blacklist." + currenttarget.NetworkId))
            {
                return;
            }
            if (currenttarget.IsKillableFromPoint(Player.ServerPosition) && Player.LSDistance(currenttarget) < Spells["R"].Range)
            {
                Spells["R"].Cast(currenttarget);
                return;
            }


            if (LissUtils.PointUnderAllyTurret(currenttarget.ServerPosition))
            {
                Spells["R"].Cast(currenttarget);
                return;
            }

            var dmgto = Player.GetSpellDamage(currenttarget, SpellSlot.R);
            if (dmgto > currenttarget.Health && currenttarget.Health >= 0.40 * dmgto)
            {
                Spells["R"].Cast(currenttarget);
                return;
            }

            var enemycount = getSliderItem(comboMenu, "Combo.Rcount");
            if (!LissUtils.isHealthy() && Player.CountEnemiesInRange(Spells["R"].Range - 100) >= enemycount)
            {
                Spells["R"].CastOnUnit(Player);
                return;
            }

            var possibilities =
                HeroManager.Enemies.Where(
                    h =>
                        (h.IsValidTarget() &&
                         Vector3.Distance(h.ServerPosition, Player.ServerPosition) <= Spells["R"].Range ||
                         (h.IsKillableFromPoint(Player.ServerPosition) && h.IsValidTarget() && !h.IsInvulnerable)) &&
                        !getCheckBoxItem(blackListMenu, "Blacklist." + h.NetworkId)).ToList();

            var arranged = possibilities.OrderByDescending(h => h.CountEnemiesInRange(Spells["R"].Range));
            if (getCheckBoxItem(miscMenu, "Misc.PrioritizeUnderTurret"))
            {
                var EnemyUnderTurret =
                    arranged.Where(h => LissUtils.PointUnderAllyTurret(h.ServerPosition) && !h.IsInvulnerable);

                var Enemytofocus = EnemyUnderTurret.MaxOrDefault(h => h.CountEnemiesInRange(Spells["R"].Range));
                if (Enemytofocus != null)
                {
                    Spells["R"].Cast(Enemytofocus);
                    return;
                }
            }

            var UltTarget = arranged.FirstOrDefault();

            if (UltTarget != null)
            {
                if (!LissUtils.isHealthy() &&
                    Player.CountEnemiesInRange(Spells["R"].Range) >
                    UltTarget.CountEnemiesInRange(Spells["R"].Range) + 1)
                {
                    Spells["R"].CastOnUnit(Player);
                    return;
                }
                Spells["R"].Cast(UltTarget);
            }
        }

        private static void KillSteal()
        {
            var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsInvulnerable & !x.IsZombie);

            var objAiHeroes = targets as IList<AIHeroClient> ?? targets.ToList();
            if (SpellSlot.Q.IsReady() && getCheckBoxItem(ksMenu, "Killsteal.UseQ"))
            {
                var qtarget =
                    objAiHeroes.Where(x => x.LSDistance(Player.Position) < Spells["Q"].Range)
                        .MinOrDefault(x => x.Health);
                if (qtarget != null)
                {
                    var qdmg = Player.GetSpellDamage(qtarget, SpellSlot.Q);
                    if (qtarget.Health < qdmg)
                    {
                        var pred = Spells["Q"].GetPrediction(qtarget);
                        if (pred != null)
                        {
                            Spells["Q"].Cast(qtarget);
                        }
                    }
                }
            }
            if (SpellSlot.W.IsReady() && getCheckBoxItem(ksMenu, "Killsteal.UseW"))
            {
                var wtarget =
                    objAiHeroes.Where(x => x.LSDistance(Player.Position) < Spells["W"].Range)
                        .MinOrDefault(x => x.Health);
                if (wtarget != null)
                {
                    var wdmg = Player.GetSpellDamage(wtarget, SpellSlot.W);
                    if (wtarget.Health < wdmg)
                    {
                        Spells["W"].CastOnUnit(Player);
                    }
                }
            }

            var etarget =
                objAiHeroes.Where(x => x.LSDistance(Player.Position) < Spells["E"].Range).MinOrDefault(x => x.Health);
            if (SpellSlot.E.IsReady() && LissEMissile == null && !LissUtils.CanSecondE() &&
                getCheckBoxItem(ksMenu, "Killsteal.UseE"))
            {
                if (etarget != null)
                {
                    var edmg = Player.GetSpellDamage(etarget, SpellSlot.E);
                    if (etarget.Health < edmg)
                    {
                        var pred = Spells["E"].GetPrediction(etarget);
                        if (pred != null)
                        {
                            Spells["E"].Cast(etarget);
                        }
                    }
                }
            }

            if (LissEMissile != null && etarget != null && etarget.HealthPercent > 5 && etarget.HealthPercent < 15 &&
                LissUtils.isHealthy() && getCheckBoxItem(ksMenu, "Killsteal.UseE2"))
            {
                if (Vector3.Distance(LissEMissile.Position, etarget.Position) < Spells["Q"].Range &&
                    SpellSlot.Q.IsReady() && etarget.Health < Player.GetSpellDamage(etarget, SpellSlot.Q))
                {
                    if (LissUtils.PointUnderEnemyTurret(MissilePosition) &&
                        getCheckBoxItem(miscMenu, "Misc.DontETurret"))
                    {
                        return;
                    }
                    Spells["E"].CastOnUnit(Player);
                }
            }

            if (Spells["Ignite"].IsReady() && getCheckBoxItem(ksMenu, "Killsteal.UseIgnite"))
            {
                var igntarget =
                    objAiHeroes.Where(x => x.LSDistance(Player.Position) < Spells["Ignite"].Range)
                        .MinOrDefault(x => x.Health);
                if (igntarget != null)
                {
                    var igniteDmg = Player.GetSummonerSpellDamage(igntarget, Damage.SummonerSpell.Ignite);
                    if (igniteDmg > igntarget.Health)
                    {
                        Spells["Ignite"].Cast(igntarget);
                    }
                }
            }

            if (SpellSlot.R.IsReady() && getCheckBoxItem(ksMenu, "Killsteal.UseR"))
            {
                var Rtarget =
                    objAiHeroes.Where(
                        h =>
                            (Vector3.Distance(Player.ServerPosition, h.ServerPosition) < Spells["R"].Range) &&
                            h.CountEnemiesInRange(Spells["R"].Range) > 1 &&
                            h.Health < Player.GetSpellDamage(h, SpellSlot.R) &&
                            !getCheckBoxItem(blackListMenu, "Blacklist." + h.NetworkId)).MinOrDefault(h => h.Health);
                if (Rtarget != null)
                {
                    Spells["R"].Cast(Rtarget);
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss != null && miss.IsValid)
            {
                if (miss.SpellCaster.IsMe && miss.SpellCaster.IsValid && miss.SData.Name == "LissandraEMissile")
                {
                    LissEMissile = miss;
                }
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss == null || !miss.IsValid) return;
            if (miss.SpellCaster is AIHeroClient && miss.SpellCaster.IsValid && miss.SpellCaster.IsMe &&
                miss.SData.Name == "LissandraEMissile")
            {
                LissEMissile = null;
                MissilePosition = new Vector2(0, 0);
            }
        }


        private static void HarassHandler()
        {
            if (Player.ManaPercent < getSliderItem(harassMenu, "Harass.Mana"))
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "Harass.UseQ"))
            {
                var target = TargetSelector.GetTarget(Spells["Q2"].Range, DamageType.Magical);
                if (target != null)
                {
                    CastQ(target);
                }
            }
            if (getCheckBoxItem(harassMenu, "Harass.UseW"))
            {
                var target = TargetSelector.GetTarget(Spells["W"].Range, DamageType.Magical);
                if (target != null && target.IsValidTarget())
                {
                    CastW(target);
                }
            }
            if (getCheckBoxItem(harassMenu, "Harass.UseE") && LissEMissile == null && !LissUtils.CanSecondE())
            {
                var target = TargetSelector.GetTarget(Spells["E"].Range, DamageType.Magical);
                if (target != null && !target.IsInvulnerable)
                {
                    CastE(target);
                }
            }
        }

        private static void FarmHandler()
        {
            if (Player.ManaPercent < getSliderItem(lastHitMenu, "Farm.Mana"))
            {
                return;
            }
            var Minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        m =>
                            m.IsValidTarget() &&
                            (Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["Q"].Range ||
                             Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["W"].Range ||
                             Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["E"].Range));

            if (SpellSlot.Q.IsReady() && getCheckBoxItem(lastHitMenu, "Farm.UseQ"))
            {
                var KillableMinionsQ =
                    Minions.Where(
                        m =>
                            m.Health < Player.GetSpellDamage(m, SpellSlot.Q) &&
                            Vector3.Distance(m.ServerPosition, Player.ServerPosition) > Player.AttackRange);
                if (KillableMinionsQ.Any())
                {
                    Spells["Q"].Cast(KillableMinionsQ.FirstOrDefault().ServerPosition);
                }
            }
            if (SpellSlot.W.IsReady() && getCheckBoxItem(lastHitMenu, "Farm.UseW"))
            {
                var KillableMinionsW =
                    Minions.Where(
                        m =>
                            m.Health < Player.GetSpellDamage(m, SpellSlot.W) &&
                            Vector3.Distance(Player.ServerPosition, m.ServerPosition) < Spells["W"].Range);
                if (KillableMinionsW.Any())
                {
                    Spells["W"].CastOnUnit(Player);
                }
            }

            if (SpellSlot.E.IsReady() && getCheckBoxItem(lastHitMenu, "Farm.UseE") && LissEMissile == null &&
                !LissUtils.CanSecondE() && LissEMissile == null)
            {
                var KillableMinionsE =
                    Minions.Where(
                        m =>
                            m.Health < Player.GetSpellDamage(m, SpellSlot.E) &&
                            Vector3.Distance(m.ServerPosition, Player.ServerPosition) > Player.AttackRange);
                if (KillableMinionsE.Any())
                {
                    Spells["E"].Cast(KillableMinionsE.FirstOrDefault().ServerPosition);
                }
            }
        }

        private static void WaveClearHandler()
        {
            var Minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        m =>
                            m.IsValidTarget() &&
                            (Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["Q"].Range ||
                             Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["W"].Range ||
                             Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["E"].Range));


            if (SpellSlot.Q.IsReady() && getCheckBoxItem(clearMenu, "Waveclear.UseQ"))
            {
                var qminions =
                    Minions.Where(
                        m =>
                            Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["Q"].Range &&
                            m.IsValidTarget());
                var QLocation =
                    MinionManager.GetBestLineFarmLocation(qminions.Select(m => m.ServerPosition.To2D()).ToList(),
                        Spells["Q"].Width, Spells["Q"].Range);
                if (QLocation.Position != null && QLocation.MinionsHit > 1)
                {
                    Spells["Q"].Cast(QLocation.Position);
                }
            }

            if (SpellSlot.E.IsReady() && getCheckBoxItem(clearMenu, "Waveclear.UseE"))
            {
                if (LissEMissile == null && !LissUtils.CanSecondE())
                {
                    var Eminions =
                        Minions.Where(
                            m => Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["E"].Range);
                    var ELocation =
                        MinionManager.GetBestLineFarmLocation(Eminions.Select(m => m.ServerPosition.To2D()).ToList(),
                            Spells["E"].Width, Spells["E"].Range);
                    if (ELocation.Position != null && ELocation.MinionsHit > 0)
                    {
                        Spells["E"].Cast(ELocation.Position);
                    }
                }
                else if (LissEMissile != null && getCheckBoxItem(clearMenu, "Waveclear.UseE2") &&
                         Vector2.Distance(MissilePosition, LissEMissile.EndPosition.To2D()) <= 15 &&
                         SpellSlot.E.IsReady())
                {
                    if (LissUtils.PointUnderEnemyTurret(MissilePosition) &&
                        getCheckBoxItem(miscMenu, "Misc.DontETurret"))
                    {
                        return;
                    }
                    Spells["E"].CastOnUnit(Player);
                }
            }

            if (SpellSlot.W.IsReady() && getCheckBoxItem(clearMenu, "Waveclear.UseW"))
            {
                var wminions =
                    Minions.Where(
                        m =>
                            Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells["W"].Range &&
                            m.IsValidTarget());
                if (wminions.Count() > getSliderItem(clearMenu, "Waveclear.Wcount"))
                {
                    Spells["W"].CastOnUnit(Player);
                }
            }
        }


        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (getCheckBoxItem(miscMenu, "Misc.Debug"))
            {
                if (LissEMissile != null)
                {
                    var misposwts = Drawing.WorldToScreen(LissEMissile.Position);
                    Drawing.DrawText(misposwts.X, misposwts.Y, Color.Red, LissEMissile.Position.ToString());
                    Render.Circle.DrawCircle(LissEMissile.Position, 200, Color.Red);
                }
            }

            var DrawQ = getCheckBoxItem(drawMenu, "Drawing.DrawQ");
            var DrawW = getCheckBoxItem(drawMenu, "Drawing.DrawW");
            var DrawE = getCheckBoxItem(drawMenu, "Drawing.DrawE");
            var DrawR = getCheckBoxItem(drawMenu, "Drawing.DrawR");

            if (DrawQ)
            {
                Render.Circle.DrawCircle(Player.Position, Spells["Q"].Range, Color.White);
            }
            if (DrawW)
            {
                Render.Circle.DrawCircle(Player.Position, Spells["W"].Range, Color.Green);
            }
            if (DrawE)
            {
                Render.Circle.DrawCircle(Player.Position, Spells["E"].Range, Color.RoyalBlue);
            }
            if (DrawR)
            {
                Render.Circle.DrawCircle(Player.Position, Spells["R"].Range, Color.Red);
            }
        }
    }
}