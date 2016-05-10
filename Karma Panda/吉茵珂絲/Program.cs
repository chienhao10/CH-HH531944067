using System.ComponentModel;
using System.Media;
using System.Net;
using EloBuddy.Sandbox;

namespace Jinx
{
    using System;
    using System.IO;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    /// <summary>
    /// Made by KarmaPanda
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Spell Q
        /// </summary>
        public static Spell.Active Q;

        /// <summary>
        /// The Spells W, E, and R
        /// </summary>
        public static Spell.Skillshot W, E, R;

        /// <summary>
        /// Champion's Name
        /// </summary>
        public const string ChampionName = "Jinx";

        /// <summary>
        /// Stores Damage Indicator
        /// </summary>
        public static DamageIndicator.DamageIndicator Indicator;
        
        //public static SoundPlayer AllahAkbar;

        /// <summary>
        /// Called when the Program is run
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the Game finishes loading
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.BaseSkinName != ChampionName)
            {
                return;
            }

            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 1500, SkillShotType.Linear, 500, 3300, 60)
            {
                AllowedCollisionCount = 0
            };
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Circular, 250, 1750, 315);
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 500, 1500, 140);

            Config.Initialize();
            Indicator = new DamageIndicator.DamageIndicator();

            Chat.Print("Jin-XXX: Loaded", System.Drawing.Color.AliceBlue);
            //Chat.Print("Jin-XXX: Check out the Menu and adjust to your preference.", System.Drawing.Color.Aqua);
            //Chat.Print("Jin-XXX: Please be sure to upvote if you enjoy!", System.Drawing.Color.OrangeRed);

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += ActiveStates.Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;

            /*try
            {
                var sandBox = SandboxConfig.DataDirectory + @"\JinXXX\";

                if (!Directory.Exists(sandBox))
                {
                    Directory.CreateDirectory(sandBox);
                }

                // Credits to iRaxe for the Original Idea
                if (!File.Exists(sandBox + "Allahu_Akbar_Sound_Effect_Download_Link.wav"))
                {
                    var client = new WebClient();
                    client.DownloadFile("http://upload.karmapanda.org/cc9981d8638aa6617e5ee96130134c05.wav",
                        sandBox + "Allahu_Akbar_Sound_Effect_Download_Link.wav");
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                }

                if (File.Exists(sandBox + "Allahu_Akbar_Sound_Effect_Download_Link.wav"))
                {
                    AllahAkbar = new SoundPlayer
                    {
                        SoundLocation =
                            SandboxConfig.DataDirectory + @"\JinXXX\" + "Allahu_Akbar_Sound_Effect_Download_Link.wav"
                    };
                    AllahAkbar.Load();
                }
            }
            catch (Exception e)
            {
                Chat.Print("Failed to load Allah Akbar: " + e);
            }*/
        }
        
        /*private static void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Chat.Print("Failed Downloading: " + e.Error);
            AllahAkbar = new SoundPlayer
            {
                SoundLocation =
                    SandboxConfig.DataDirectory + @"\JinXXX\" + "Allahu_Akbar_Sound_Effect_Download_Link.wav"
            };
            AllahAkbar.Load();
        }*/

        /// <summary>
        /// Called Before Attack
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            #region Combo

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var useQ = Config.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;

                // If the player has the rocket
                if (useQ && Program.Q.IsReady() && Essentials.FishBones())
                {
                    //var target = TargetSelector.GetTarget(Essentials.FishBonesRange(), DamageType.Physical);

                    if (target != null && target.IsValidTarget())
                    {
                        if (Player.Instance.Distance(target) <= Essentials.MinigunRange &&
                            target.CountEnemiesInRange(100) <
                            Config.ComboMenu["qCountC"].Cast<Slider>().CurrentValue)
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = target;
                        }
                    }
                }
            }

            #endregion

            #region LastHit

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                var useQ = Config.LastHitMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaQ = Config.LastHitMenu["manaQ"].Cast<Slider>().CurrentValue;
                var qCountM = Config.LastHitMenu["qCountM"].Cast<Slider>().CurrentValue;

                // Force Minigun if there is a lasthittable minion in minigun range and there is no targets more than the setting amount.
                var kM = Orbwalker.LastHitMinionsList.Where(
                    t => t.IsEnemy &&
                         t.Health <= (Player.Instance.GetAutoAttackDamage(t) * 0.9) && t.IsValidTarget() &&
                         t.Distance(Player.Instance) <= Essentials.MinigunRange);
                if (useQ && Essentials.FishBones() && kM.Count() < qCountM)
                {
                    Program.Q.Cast();
                }
            }

            #endregion

            #region Lane Clear

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Essentials.FishBonesRange()).OrderByDescending(t => t.Health);

                if (Essentials.FishBones())
                {
                    foreach (var m in minion)
                    {
                        var minionsAoe =
                            EntityManager.MinionsAndMonsters.EnemyMinions.Count(
                                t => t.IsValidTarget() && t.Distance(m) <= 100);

                        if (m.Distance(Player.Instance) <= Essentials.MinigunRange && m.IsValidTarget() &&
                            (minionsAoe < Config.LaneClearMenu["qCountM"].Cast<Slider>().CurrentValue ||
                             m.Health > (Player.Instance.GetAutoAttackDamage(m))))
                        {
                            Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                        else if (m.Distance(Player.Instance) <= Essentials.MinigunRange &&
                                 !Orbwalker.LastHitMinionsList.Contains(m))
                        {
                            Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                        else
                        {
                            foreach (
                                var kM in
                                    Orbwalker.LastHitMinionsList.Where(
                                        kM =>
                                            kM.IsValidTarget() &&
                                            kM.Health <= (Player.Instance.GetAutoAttackDamage(kM)*0.9) &&
                                            kM.Distance(Player.Instance) <= Essentials.MinigunRange))
                            {
                                Q.Cast();
                                Orbwalker.ForcedTarget = kM;
                            }
                        }
                    }
                }
            }

            #endregion

            if (Essentials.FishBones() && target.IsStructure() &&
                target.Distance(Player.Instance) <= Essentials.MinigunRange)
            {
                Q.Cast();
            }
        }

        /// <summary>
        /// Called After Attack
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
        }

        /// <summary>
        /// Called when a Spell gets Casted
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Spell</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //var allahAkbarT = Config.MiscMenu["allahAkbarT"].Cast<CheckBox>().CurrentValue;
            var autoE = Config.MiscMenu["autoE"].Cast<CheckBox>().CurrentValue;
            var eSlider = Config.MiscMenu["eSlider"].Cast<Slider>().CurrentValue;

            /*if (AllahAkbar != null)
            {
                if (allahAkbarT && sender.IsMe && args.SData.Name.Equals("JinxR"))
                {
                    AllahAkbar.Play();
                }
            }*/

            if (!autoE || sender.IsMinion || !E.IsReady())
            {
                return;
            }

            if (sender.IsEnemy && sender.IsValidTarget(E.Range) && Essentials.ShouldUseE(args.SData.Name))
            {
                var prediction = E.GetPrediction(sender);

                if (prediction.HitChancePercent >= eSlider)
                {
                    E.Cast(prediction.CastPosition);
                }
            }

            if (sender.IsAlly && args.SData.Name == "RocketGrab" && E.IsInRange(sender))
            {
                Essentials.GrabTime = Game.Time;
            }
        }

        /// <summary>
        /// Called when it is possible to Interrupt
        /// </summary>
        /// <param name="sender">The Interruptable Target</param>
        /// <param name="e">The Information</param>
        private static void Interrupter_OnInterruptableSpell(
            Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender != null && !sender.IsAlly && Config.MiscMenu["interruptE"].Cast<CheckBox>().CurrentValue
                && Player.Instance.ManaPercent >= Config.MiscMenu["interruptmanaE"].Cast<Slider>().CurrentValue
                && (E.IsInRange(sender) && E.IsReady() && sender.IsValidTarget() && e.DangerLevel == DangerLevel.High))
            {
                var pred = E.GetPrediction(sender);

                if (pred != null && pred.HitChancePercent >= 75)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Called when it is possible to Gapclose
        /// </summary>
        /// <param name="sender">The Gapclosable Target</param>
        /// <param name="e">The Information</param>
        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender != null && sender.IsEnemy && Config.MiscMenu["gapcloserE"].Cast<CheckBox>().CurrentValue
                && Player.Instance.ManaPercent >= Config.MiscMenu["gapclosermanaE"].Cast<Slider>().CurrentValue
                && (E.IsInRange(sender) && E.IsReady() && sender.IsValidTarget()))
            {
                var pred = E.GetPrediction(sender);

                if (pred != null && pred.HitChancePercent >= 75)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Called every time the Game Draws
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead)
            {
                return;
            }

            var drawQ = Config.DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue;
            var drawW = Config.DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue;
            var drawE = Config.DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue;
            var predW = Config.DrawingMenu["predW"].Cast<CheckBox>().CurrentValue;
            var predR = Config.DrawingMenu["predR"].Cast<CheckBox>().CurrentValue;

            if (drawQ)
            {
                Circle.Draw(Q.IsReady() ? Color.Green : Color.Red,
                    !Essentials.FishBones() ? Essentials.FishBonesRange() : Essentials.MinigunRange,
                    Player.Instance.Position);
            }

            if (drawW)
            {
                Circle.Draw(W.IsReady() ? Color.Red : Color.Green, W.Range, Player.Instance.Position);
            }

            if (drawE)
            {
                Circle.Draw(E.IsReady() ? Color.Red : Color.Green, E.Range, Player.Instance.Position);
            }

            if (predW)
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget() && W.IsInRange(t))
                        .OrderBy(t => t.Distance(Player.Instance))
                        .FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                var wPred = W.GetPrediction(enemy).CastPosition;
                Essentials.DrawLineRectangle(wPred.To2D(), Player.Instance.Position.To2D(), W.Width, 1,
                    W.IsReady() ? System.Drawing.Color.YellowGreen : System.Drawing.Color.Red);
            }

            if (predR)
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            t.IsValidTarget()
                            && t.Distance(Player.Instance) >= Config.MiscMenu["rRange"].Cast<Slider>().CurrentValue &&
                            t.Distance(Player.Instance) <= R.Range)
                        .OrderBy(t => t.Distance(Player.Instance))
                        .FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                var rPred = R.GetPrediction(enemy).CastPosition;
                Essentials.DrawLineRectangle(rPred.To2D(), Player.Instance.Position.To2D(), R.Width, 1,
                    R.IsReady() ? System.Drawing.Color.YellowGreen : System.Drawing.Color.Red);
            }
        }

        /// <summary>
        /// Called every time the Game Ticks
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ForcedTarget != null)
            {
                if (!Player.Instance.IsInAutoAttackRange(Orbwalker.ForcedTarget) ||
                    !Orbwalker.ForcedTarget.IsValidTarget() ||
                    Essentials.HasUndyingBuff(Orbwalker.ForcedTarget as Obj_AI_Base))
                {
                    Orbwalker.ForcedTarget = null;
                }

                var target = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange(),
                    DamageType.Physical);

                if (Orbwalker.ForcedTarget is AIHeroClient && target != null &&
                    ((Orbwalker.ForcedTarget.NetworkId != target.NetworkId) && !Essentials.HasUndyingBuff(target) &&
                     Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
                {
                    Orbwalker.ForcedTarget = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange(),
                        DamageType.Physical);
                }
            }

            if (Player.Instance.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                StateManager.LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                StateManager.JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                StateManager.Flee();
            }
        }
    }
}