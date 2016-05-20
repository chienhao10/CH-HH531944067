using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX.IO;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace Tyler1
{
    class Program
    {
        private static Menu Menu;
        public static bool AutoCatch;
        public static bool CatchOnlyCloseToMouse;
        public static int MaxDistToMouse;
        public static bool OnlyCatchIfSafe;
        public static int MinQLaneclearManaPercent;
        public static Menu EMenu;
        public static bool ECombo;
        public static bool EGC;
        public static bool EInterrupt;
        public static Menu RMenu;
        public static bool RKS;
        public static bool RKSOnlyIfCantAA;
        public static int RIfHit;
        public static bool WCombo;
        public static bool UseItems;
        private static AIHeroClient Player = ObjectManager.Player;
        private static LeagueSharp.SDK.Spell Q, W, E, R;
        public static Color color = Color.DarkOrange;
        public static float MyRange = 550f;
        private static bool R1vs1;

        private static Dictionary<int, GameObject> Reticles;

        private static int AxesCount
        {
            get
            {
                var data = Player.GetBuff("dravenspinningattack");
                if (data == null || data.Count == -1)
                {
                    return 0;
                }
                return data.Count == 0 ? 1 : data.Count;
            }
        }

        private static int TotalAxesCount
        {
            get
            {
                return (ObjectManager.Player.HasBuff("dravenspinning") ? 1 : 0)
                       + (ObjectManager.Player.HasBuff("dravenspinningleft") ? 1 : 0) + Reticles.Count;
            }
        }

        public static void Load()
        {
            DelayAction.Add(1500, () =>
            {
                if (ObjectManager.Player.CharData.BaseSkinName != "Draven") return;
                InitSpells();
                FinishLoading();
                Reticles = new Dictionary<int, GameObject>();
                GameObject.OnCreate += OnCreate;
                GameObject.OnDelete += OnDelete;
            });
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var itemToDelete = Reticles.FirstOrDefault(ret => ret.Value.NetworkId == sender.NetworkId);
            Reticles.Remove(itemToDelete.Key);
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals("Draven_Base_Q_reticle_self.troy") && !sender.IsDead)
            {
                Reticles.Add(Variables.TickCount, sender);
            }
        }

        private static void InitSpells()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 1050);
            E.SetSkillshot(0.25f, 130, 1400, false, SkillshotType.SkillshotLine);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 3000);
            R.SetSkillshot(0.25f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        private static void FinishLoading()
        {
            Drawing.OnDraw += Draw;
            Game.OnUpdate += OnUpdate;
            Events.OnGapCloser += OnGapcloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            DelayAction.Add(3000, () => MyRange = ObjectManager.Player.GetAutoAttackRange());
            //Variables.Orbwalker.Enabled = true;
            //DelayAction.Add(1000, () => Variables.Orbwalker.Enabled = true);
            //DelayAction.Add(5000, () => Variables.Orbwalker.Enabled = true);
            //DelayAction.Add(10000, () => Variables.Orbwalker.Enabled = true);

            Menu = MainMenu.AddMenu("tyler1", "Tyler1");

            AutoCatch = (Menu.Add("tyler1auto", new CheckBox("Auto catch axes?", true))).CurrentValue;
            CatchOnlyCloseToMouse = Menu.Add("tyler1onlyclose", new CheckBox("Catch only axes close to mouse?", true)).CurrentValue;
            MaxDistToMouse = Menu.Add("tyler1maxdist", new Slider("Max axe distance to mouse", 500, 250, 1250)).CurrentValue;
            OnlyCatchIfSafe = Menu.Add("tyler1safeaxes", new CheckBox("Only catch axes if safe (anti melee)", false)).CurrentValue;
            MinQLaneclearManaPercent = Menu.Add("tyler1QLCMana", new Slider("Min Mana Percent for Q Laneclear", 60, 0, 100)).CurrentValue;

            EMenu = Menu.AddSubMenu("tyler1E", "E Settings: ");
            ECombo = EMenu.Add("tyler1ECombo", new CheckBox("Use E in Combo", true)).CurrentValue;
            EGC = EMenu.Add("tyler1EGC", new CheckBox("Use E on Gapcloser", true)).CurrentValue;
            EInterrupt = EMenu.Add("tyler1EInterrupt", new CheckBox("Use E to Interrupt", true)).CurrentValue;

            RMenu = Menu.AddSubMenu("tyler1R", "R Settings:");
            RKS = RMenu.Add("tyler1RKS", new CheckBox("Use R to steal kills", true)).CurrentValue;
            RKSOnlyIfCantAA = RMenu.Add("tyler1RKSOnlyIfCantAA", new CheckBox("Use R KS only if can't AA", true)).CurrentValue;
            RIfHit = RMenu.Add("tyler1RIfHit", new Slider("Use R if it will hit X enemies", 2, 1, 5)).CurrentValue;
            R1vs1 = RMenu.Add("tyler1R1v1", new CheckBox("Always use R in 1v1", true)).CurrentValue;
            WCombo = Menu.Add("tyler1WCombo", new CheckBox("Use W in Combo", true)).CurrentValue;
            UseItems = Menu.Add("tyler1Items", new CheckBox("Use Items?", true)).CurrentValue;
        }

        private static void OnUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            try
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Farm();
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target != null)
                {
                    Combo();
                    RCombo();
                }
                CatchAxes();
                KS();
                if (W.IsReady() && Player.HasBuffOfType(BuffType.Slow) &&
                    target.Distance(ObjectManager.Player) <= MyRange) W.Cast();
                R1V1(target);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void R1V1(AIHeroClient target)
        {
            if (R1vs1 && target != null && target.IsHPBarRendered && target.Distance(ObjectManager.Player) < 650 && !ShouldntUlt(target))
            {
                if (target.HealthPercent > ObjectManager.Player.HealthPercent &&
                    (target.MaxHealth <= ObjectManager.Player.MaxHealth + 300 ||
                     noobchamps.Contains(target.CharData.BaseSkinName)))
                {
                    var pred = R.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        R.Cast(pred.UnitPosition);
                    }
                }
            }
        }

        private static List<string> noobchamps = new List<string>
        {
            "Ahri",
            "Anivia",
            "Annie",
            "Ashe",
            "Azir",
            "Brand",
            "Caitlyn",
            "Cassiopeia",
            "Corki",
            "Draven",
            "Ezreal",
            "Graves",
            "Jinx",
            "Kalista",
            "Karma",
            "Karthus",
            "Katarina",
            "Kennen",
            "KogMaw",
            "Leblanc",
            "Kindred",
            "Lucian",
            "Lux",
            "Malzahar",
            "MasterYi",
            "MissFortune",
            "Orianna",
            "Quinn",
            "Sivir",
            "Syndra",
            "Talon",
            "Teemo",
            "Tristana",
            "TwistedFate",
            "Twitch",
            "Varus",
            "Vayne",
            "Veigar",
            "Velkoz",
            "Viktor",
            "Xerath",
            "Zed",
            "Ziggs",
            "Soraka",
            "Akali",
            "Diana",
            "Ekko",
            "Fiddlesticks",
            "Fiora",
            "Fizz",
            "Heimerdinger",
            "Illaoi",
            "Jayce",
            "Kassadin",
            "Kayle",
            "KhaZix",
            "Kindred",
            "Lissandra",
            "Mordekaiser",
            "Nidalee",
            "Riven",
            "Shaco",
            "Vladimir",
            "Yasuo",
            "Zilean"
        };

        /// <summary>
        /// Those buffs make the target either unkillable or a pain in the ass to kill, just wait until they end
        /// </summary>
        private static List<string> UndyingBuffs = new List<string>
        {
            "JudicatorIntervention",
            "UndyingRage",
            "FerociousHowl",
            "ChronoRevive",
            "ChronoShift",
            "lissandrarself",
            "kindredrnodeathbuff"
        };

        private static bool ShouldntUlt(AIHeroClient target)
        {
            //Dead or not a hero
            if (target == null || !target.IsHPBarRendered) return true;
            //Undying
            if (UndyingBuffs.Any(buff => target.HasBuff(buff))) return true;
            //Blitzcrank
            if (target.CharData.BaseSkinName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD")
                && !target.HasBuff("ManaBarrier"))
            {
                return true;
            }
            //Sivir
            return target.CharData.BaseSkinName == "Sivir" && target.HasBuffOfType(BuffType.SpellShield) ||
                   target.HasBuffOfType(BuffType.SpellImmunity);
        }

        private static void RCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target != null && target.IsHPBarRendered && !target.IsDead && !target.IsZombie)
            {
                var pred = R.GetPrediction(target);
                if (pred.Hitchance > HitChance.High && pred.AoeTargetsHit.Count >= RIfHit)
                {
                    R.Cast(pred.UnitPosition);
                }
            }
        }

        private static void Farm()
        {
            if (ObjectManager.Player.ManaPercent < MinQLaneclearManaPercent) return;
            if (
                ObjectManager.Get<Obj_AI_Minion>()
                    .Any(m => m.IsHPBarRendered && m.IsEnemy && m.Distance(ObjectManager.Player) < MyRange))
            {
                if (TotalAxesCount < 2) Q.Cast();
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target.Distance(Player) < MyRange + 100)
            {
                if (TotalAxesCount < 2) Q.Cast();
                if (WCombo && W.IsReady() && !Player.HasBuff("dravenfurybuff")) W.Cast();
            }
            if (ECombo && E.IsReady() && target.IsValidTarget(750))
            {
                var pred = E.GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                    E.Cast(pred.UnitPosition);
            }
        }

        private static void CatchAxes()
        {
            Vector3 Mouse = Game.CursorPos;
            if (!ObjectManager.Get<GameObject>().Any(x => x.Name.Equals("Draven_Base_Q_reticle_self.troy") && !x.IsDead) || !AutoCatch)
            {
                Orbwalker.DisableMovement = false;
            }
            if (AutoCatch)
            {
                foreach (var reticle in Reticles.Where(x => !x.Value.IsDead && (!x.Value.Position.IsUnderEnemyTurret() || (Mouse.IsUnderEnemyTurret() && ObjectManager.Player.IsUnderEnemyTurret()))).OrderBy(ret => ret.Key))
                {
                    var AXE = reticle.Value;
                    if (OnlyCatchIfSafe && GameObjects.EnemyHeroes.Count(e => e.IsHPBarRendered && e.IsMelee && e.ServerPosition.Distance(AXE.Position) < 350) >= 1)
                    {
                        break;
                    }
                    if (CatchOnlyCloseToMouse && AXE.Distance(Mouse) > MaxDistToMouse)
                    {
                        Orbwalker.DisableMovement = false;

                        if (GameObjects.EnemyHeroes.Count(e => e.IsHPBarRendered && e.IsMelee && e.ServerPosition.Distance(AXE.Position) < 350) >= 1)
                        {
                            //user probably doesn't want to go there, try the next reticle
                            break;
                        }
                        //maybe user just has potato reaction time
                        return;
                    }
                    if (AXE.Distance(Player.ServerPosition) > 60 && Orbwalker.CanMove)
                    {
                        Orbwalker.DisableMovement = false;
                        Orbwalker.OrbwalkTo(AXE.Position.Randomize());
                        Orbwalker.DisableMovement = true;
                    }
                    if (AXE.Distance(Player.ServerPosition) <= 70)
                    {
                        Orbwalker.DisableMovement = false;
                    }
                }
            }
        }


        /// <summary>
        /// Will need to add an actual missile check for the axes in air instead of brosciencing
        /// </summary>
        private static void KS()
        {
            if (!RKS) return;
            foreach (
                var enemy in
                    GameObjects.EnemyHeroes.Where(
                        e =>
                            e.IsHPBarRendered && e.Distance(ObjectManager.Player) < 3000 &&
                            (e.Distance(ObjectManager.Player) > MyRange + 150 || !RKSOnlyIfCantAA)))
            {
                if (enemy.Health < R.GetDamage(enemy) && !ShouldntUlt(enemy))
                {
                    var pred = R.GetPrediction(enemy);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        R.Cast(pred.UnitPosition);
                    }
                }
            }
        }

        private static void Draw(EventArgs args)
        {
            if (Player.IsDead) return;
            var reticles =
                ObjectManager.Get<GameObject>()
                    .Where(x => x.Name.Equals("Draven_Base_Q_reticle_self.troy") && !x.IsDead).ToArray();
            if (reticles.Any())
            {
                var PlayerPosToScreen = Drawing.WorldToScreen(ObjectManager.Player.Position);
                foreach (var AXE in reticles)
                {
                    var AXEToScreen = Drawing.WorldToScreen(AXE.Position);
                    Render.Circle.DrawCircle(AXE.Position, 140, Color.Red, 8);
                }

                Drawing.DrawLine(PlayerPosToScreen, Drawing.WorldToScreen(reticles[0].Position), 8, Color.Red);

                for (int i = 0; i < reticles.Length; i++)
                {
                    if (i < reticles.Length - 1)
                    {
                        Drawing.DrawLine(Drawing.WorldToScreen(reticles[i].Position),
                            Drawing.WorldToScreen(reticles[i + 1].Position), 8, Color.Red);
                    }
                }
                if (CatchOnlyCloseToMouse && MaxDistToMouse < 700 &&
                    ObjectManager.Get<GameObject>()
                        .Any(x => x.Name.Equals("Draven_Base_Q_reticle_self.troy") && !x.IsDead))
                {
                    Render.Circle.DrawCircle(Game.CursorPos, MaxDistToMouse, Color.Red, 8);
                }
            }
        }

        private static void OnGapcloser(object sender, Events.GapCloserEventArgs gapcloser)
        {
            if (EGC && E.IsReady() && gapcloser.Sender.Distance(ObjectManager.Player) < 800)
            {
                var pred = E.GetPrediction(gapcloser.Sender);
                if (pred.Hitchance > HitChance.High)
                {
                    E.Cast(pred.UnitPosition);
                }
            }
        }

        private static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (EInterrupt && E.IsReady() && args.Sender.Distance(ObjectManager.Player) < 950)
            {
                E.Cast(args.Sender.Position);
            }
        }
    }
}