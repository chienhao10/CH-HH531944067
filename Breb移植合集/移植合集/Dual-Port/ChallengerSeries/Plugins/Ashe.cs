using System;
using System.Collections.Generic;
using System.Linq;
using Challenger_Series.Utils;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Challenger_Series.Plugins
{
    public class Ashe : CSPlugin
    {

        #region Spells
        public LeagueSharp.SDK.Spell Q { get; set; }
        public LeagueSharp.SDK.Spell Q2 { get; set; }
        public LeagueSharp.SDK.Spell W { get; set; }
        public LeagueSharp.SDK.Spell W2 { get; set; }
        public LeagueSharp.SDK.Spell E { get; set; }
        public LeagueSharp.SDK.Spell E2 { get; set; }
        public LeagueSharp.SDK.Spell R { get; set; }
        public LeagueSharp.SDK.Spell R2 { get; set; }
        #endregion Spells

        public Ashe()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 1100);
            W.SetSkillshot(250f, 75f, 1500f, true, SkillshotType.SkillshotLine);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 25000);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 1400);

            R.SetSkillshot(250f, 100f, 1600f, false, SkillshotType.SkillshotLine);
            InitMenu();
            AIHeroClient.OnSpellCast += OnDoCast;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            DelayedOnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Events.OnGapCloser += EventsOnOnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Q.IsReady() && args.Target is AIHeroClient && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && UseQCombo)
            {
                Q.Cast();
            }
        }

        private void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && args.DangerLevel >= LeagueSharp.SDK.DangerLevel.Medium && args.Sender.Distance(ObjectManager.Player) < 1300)
            {
                R.Cast(args.Sender.ServerPosition);
            }
        }

        private void EventsOnOnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (R.IsReady() && args.IsDirectedToPlayer && args.Sender.Distance(ObjectManager.Player) < 1300)
            {
                R.Cast(args.Sender.ServerPosition);
            }
        }

        private List<Vector2> OrderScoutPositions = new List<Vector2> { new Vector2(7200, 2700), new Vector2(6900, 4700), new Vector2(3200, 6700), new Vector2(2700, 8300) };
        private List<Vector2> ChaosScoutPositions = new List<Vector2> { new Vector2(8200, 10000), new Vector2(6800, 12000), new Vector2(11500, 8400), new Vector2(12000, 6700) };
        private Vector2 DragonScoutPosition = new Vector2(10300, 5000);
        private Vector2 BaronScoutPosition = new Vector2(4400, 9600);
        private Vector2 LastELocation = new Vector2(4400, 9600);

        public override void OnDraw(EventArgs args)
        {
            if (DrawWRange)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1100, Color.Turquoise);
            }
        }

        void WLogic()
        {
            var wTarget = TargetSelector.GetTarget(1100, DamageType.Physical);
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && UseWHarass && !ValidTargets.Any(e => e.InAutoAttackRange()))
            {
                var pred = W.GetPrediction(wTarget);
                if (!pred.CollisionObjects.Any() &&
                    pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) < 1100)
                {
                    if (pred.UnitPosition.CountEnemyHeroesInRange(400) >= 1)
                        W.Cast(pred.UnitPosition);
                }
            }
        }

        void RLogic()
        {
            var rTarget = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (R.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && UseRCombo)
            {
                var pred = R.GetPrediction(rTarget);
                if (pred.Hitchance >= HitChance.High)
                    R.Cast(pred.UnitPosition);
            }
        }

        void ELogic()
        {
            if (E.IsReady() && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && ValidTargets.Count(e => e.InAutoAttackRange()) == 0)
            {
                switch (ScoutMode)
                {
                    case 2:
                        {
                            if (LastELocation == BaronScoutPosition)
                            {
                                LastELocation = DragonScoutPosition;
                                E.Cast(DragonScoutPosition.RandomizeToVector3(-150, 150));
                            }
                            else
                            {
                                LastELocation = BaronScoutPosition;
                                E.Cast(BaronScoutPosition.RandomizeToVector3(-150, 150));
                            }
                            break;
                        }
                    case 0:
                        {
                            if (ObjectManager.Player.Team == GameObjectTeam.Order)
                            {
                                var pos =
                                    ChaosScoutPositions.Where(v2 => v2.Distance(LastELocation) > 500)
                                        .OrderBy(v2 => v2.Distance(ObjectManager.Player.Position.ToVector2()))
                                        .FirstOrDefault();
                                LastELocation = pos;
                                E.Cast(pos.RandomizeToVector3(-150, 150));
                            }
                            else
                            {
                                var pos =
                                    OrderScoutPositions.Where(v2 => v2.Distance(LastELocation) > 500)
                                        .OrderBy(v2 => v2.Distance(ObjectManager.Player.Position.ToVector2()))
                                        .FirstOrDefault();
                                LastELocation = pos;
                                E.Cast(pos.RandomizeToVector3(-150, 150));
                            }
                            break;
                        }
                    case 1:
                        {
                            if (ObjectManager.Player.Team == GameObjectTeam.Order)
                            {
                                var pos =
                                    ChaosScoutPositions.Where(v2 => v2.Distance(LastELocation) > 500)
                                        .OrderByDescending(v2 => v2.Distance(ObjectManager.Player.Position.ToVector2()))
                                        .FirstOrDefault();
                                LastELocation = pos;
                                E.Cast(pos.RandomizeToVector3(-150, 150));
                            }
                            else
                            {
                                var pos =
                                    OrderScoutPositions.Where(v2 => v2.Distance(LastELocation) > 500)
                                        .OrderByDescending(v2 => v2.Distance(ObjectManager.Player.Position.ToVector2()))
                                        .FirstOrDefault();
                                LastELocation = pos;
                                E.Cast(pos.RandomizeToVector3(-150, 150));
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (W.IsReady()) this.WLogic();
            if (R.IsReady()) this.RLogic();
            if (E.IsReady()) this.ELogic();
        }

        private void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (W.IsReady() && sender.IsMe && args.Target is AIHeroClient && !HasQEmpoweredAttack)
            {
                var name = args.SData.Name;
                var target = args.Target as AIHeroClient;
                if (name.Contains("AsheBasicAttack") || name.Contains("AsheCritAttack"))
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && UseWCombo && target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) > 300)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) < 1000 &&
                            !pred.CollisionObjects.Any())
                        {
                            if (pred.UnitPosition.CountEnemyHeroesInRange(400) > 0)
                                W.Cast(pred.UnitPosition);
                        }
                    }
                }
            }
        }

        private Menu ComboMenu;
        private bool UseQCombo;
        private bool UseWCombo;
        private bool UseRCombo;
        private bool UseWHarass;
        private bool UseRAntiGapclose;
        private bool UseRInterrupt;
        private bool DrawWRange;
        private int ScoutMode;
        public void InitMenu()
        {
            ComboMenu = MainMenu.AddSubMenu("Combo Settings: ", "Combo Settings: ");
            ComboMenu.Add("asheqcombo", new CheckBox("Use Q", true));
            ComboMenu.Add("ashewcombo", new CheckBox("Use W", true));
            ComboMenu.Add("ashercombo", new CheckBox("Use R", true));
            MainMenu.Add("ashewharass", new CheckBox("Use W Harass", true));
            MainMenu.Add("asherantigapclose", new CheckBox("Use R AntiGapclose", false));
            MainMenu.Add("asherinterrupt", new CheckBox("Use R Interrupt", true));
            MainMenu.Add("ashedraww", new CheckBox("Draw W Range?", false));
            MainMenu.Add("ashescoutmode", new ComboBox("Scout (E) Mode: ", 0, "EnemyJungleClosest", "EnemyJungleFarthest", "DragonBaron", "Custom", "Disabled"));

            UseQCombo = getCheckBoxItem(ComboMenu, "asheqcombo");
            UseWCombo = getCheckBoxItem(ComboMenu, "ashewcombo");
            UseRCombo = getCheckBoxItem(ComboMenu, "ashercombo");
            UseWHarass = getCheckBoxItem(MainMenu, "ashewharass");
            UseRAntiGapclose = getCheckBoxItem(MainMenu, "asherantigapclose");
            UseRInterrupt = getCheckBoxItem(MainMenu, "asherinterrupt");
            DrawWRange = getCheckBoxItem(MainMenu, "ashedraww");
            ScoutMode = getBoxItem(MainMenu, "ashescoutmode");
        }

        private bool HasQEmpoweredAttack => ObjectManager.Player.HasBuff("AsheQAttack");
    }
}