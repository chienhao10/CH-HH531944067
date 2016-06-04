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
    public class Caitlyn : CSPlugin
    {

        public LeagueSharp.SDK.Spell Q { get; set; }
        public LeagueSharp.SDK.Spell Q2 { get; set; }
        public LeagueSharp.SDK.Spell W { get; set; }
        public LeagueSharp.SDK.Spell W2 { get; set; }
        public LeagueSharp.SDK.Spell E { get; set; }
        public LeagueSharp.SDK.Spell E2 { get; set; }
        public LeagueSharp.SDK.Spell R { get; set; }
        public LeagueSharp.SDK.Spell R2 { get; set; }

        public Caitlyn()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 1200);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 820);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 770);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.40f, 60f, 2200f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(3000f, 50f, 1000f, false, SkillshotType.SkillshotLine);
            InitMenu();
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            DelayedOnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Events.OnGapCloser += EventsOnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
        }

        public override void OnUpdate(EventArgs args)
        {
            if (Q.IsReady()) this.QLogic();
            if (W.IsReady()) this.WLogic();
            if (R.IsReady()) this.RLogic();
        }

        private void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe && args.Animation == "Spell3")
            {
                var target = TargetSelector.GetTarget(1000, DamageType.Physical);
                var pred = Q.GetPrediction(target);
                if (AlwaysQAfterE)
                {
                    if ((int)pred.Hitchance >= (int)HitChance.Medium
                        && pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) < 1100) Q.Cast(pred.UnitPosition);
                }
                else
                {
                    if ((int)pred.Hitchance > (int)HitChance.Medium
                        && pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) < 1100) Q.Cast(pred.UnitPosition);
                }
            }
        }

        private void EventsOnGapCloser(object oSender, Events.GapCloserEventArgs args)
        {
            var sender = args.Sender;
            if (UseEAntiGapclose)
            {
                if (args.IsDirectedToPlayer && args.Sender.Distance(ObjectManager.Player) < 800)
                {
                    if (E.IsReady())
                    {
                        E.Cast(sender.ServerPosition);
                    }
                }
            }
        }

        private void OnInterruptableTarget(object oSender, Events.InterruptableTargetEventArgs args)
        {
            var sender = args.Sender;
            if (!GameObjects.AllyMinions.Any(m => m.Position.Distance(sender.ServerPosition) < 100)
                && args.DangerLevel >= LeagueSharp.SDK.DangerLevel.Medium && ObjectManager.Player.Distance(sender) < 550)
            {
                W.Cast(sender.ServerPosition);
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            base.OnProcessSpellCast(sender, args);
            if (sender is AIHeroClient && sender.IsEnemy)
            {
                if (args.SData.Name == "summonerflash" && args.End.Distance(ObjectManager.Player.ServerPosition) < 350)
                {
                    E.Cast(args.End);
                }
            }
        }

        public override void OnDraw(EventArgs args)
        {
            var drawRange = DrawRange;
            if (drawRange > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, drawRange, Color.Gold);
            }
            if (DrawQRange == true)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1200, Color.ForestGreen);
            }
            if (DrawRRange == true)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 2000, Color.ForestGreen);
            }
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            /*if (orbwalkingActionArgs.Target is Obj_AI_Minion && HasPassive && FocusOnHeadShotting &&
                Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                var target = orbwalkingActionArgs.Target as Obj_AI_Minion;
                if (target != null && !target.CharData.BaseSkinName.Contains("MinionSiege") && target.Health > 60)
                {
                    var tg = (Obj_AI_Hero)TargetSelector.GetTarget(715, DamageType.Physical);
                    if (tg != null && tg.IsHPBarRendered)
                    {
                        Orbwalker.ForceTarget = tg;
                        orbwalkingActionArgs.Process = false;
                    }
                }
            }*/
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
            if (E.IsReady() && this.UseECombo)
            {
                if (!OnlyUseEOnMelees)
                {
                    var eTarget = TargetSelector.GetTarget(UseEOnEnemiesCloserThanSlider, DamageType.Physical);
                    if (eTarget != null)
                    {
                        var pred = E.GetPrediction(eTarget);
                        if (pred.CollisionObjects.Count == 0 && (int)pred.Hitchance >= (int)HitChance.High)
                        {
                            E.Cast(pred.UnitPosition);
                        }
                    }
                }
                else
                {
                    var eTarget =
                        ValidTargets.FirstOrDefault(
                            e =>
                            e.IsMelee && e.Distance(ObjectManager.Player) < UseEOnEnemiesCloserThanSlider
                            && !e.IsZombie);
                    var pred = E.GetPrediction(eTarget);
                    if (pred.CollisionObjects.Count == 0 && (int)pred.Hitchance >= (int)HitChance.Medium)
                    {
                        E.Cast(pred.UnitPosition);
                    }
                }
            }
        }

        private Menu ComboMenu;
        private Menu AutoWConfig;
        private bool UseQCombo;
        private bool UseECombo;
        private bool UseRCombo;
        private bool AlwaysQAfterE;
        private bool FocusOnHeadShotting;
        private bool UseWInterrupt;
        private bool OnlyUseEOnMelees;
        private bool UseEAntiGapclose;
        private int UseEOnEnemiesCloserThanSlider;
        private int DrawRange;
        private bool DrawQRange;
        private bool DrawRRange;
        private int QHarassMode;


        public void InitMenu()
        {
            ComboMenu = MainMenu.AddSubMenu("Combo Settings: ", "Combo Settings: ");
            ComboMenu.Add("caitqcombo", new CheckBox("Use Q", true));
            ComboMenu.Add("caitecombo", new CheckBox("Use E", true));
            ComboMenu.Add("caitrcombo", new KeyBind("Use R", false, KeyBind.BindTypes.HoldActive, 'R'));

            AutoWConfig = MainMenu.AddSubMenu("W Settings: ");
            AutoWConfig.Add("caitusewinterrupt", new CheckBox("Use W to Interrupt", true));

            //new Utils.Logic.PositionSaver(AutoWConfig, W);

            MainMenu.Add("caitfocusonheadshottingenemies", new CheckBox("Try to save Headshot for poking", true));
            MainMenu.Add("caitalwaysqaftere", new CheckBox("Always Q after E (EQ combo)", true));
            MainMenu.Add("caitqharassmode", new ComboBox("Q HARASS MODE: ", 0, "FULLDAMAGE", "ALLOWMINIONS", "DISABLED"));
            MainMenu.Add("caiteantigapclose", new CheckBox("Use E AntiGapclose", false));
            MainMenu.Add("caitescape", new Slider("Use E on enemies closer than", 400, 100, 650));
            MainMenu.Add("caiteonlymelees", new CheckBox("Only use E on melees", false));
            MainMenu.Add("caitdrawrange", new Slider("Draw a circle with radius: ", 800, 0, 1240));
            MainMenu.Add("caitqrange", new CheckBox("Draw Q Range", true));
            MainMenu.Add("caitrrange", new CheckBox("Draw R Range", true));
            
            UseQCombo = getCheckBoxItem(ComboMenu, "caitqcombo");
            UseECombo = getCheckBoxItem(ComboMenu, "caitecombo");
            UseRCombo = getKeyBindItem(ComboMenu, "caitrcombo");

            QHarassMode = getBoxItem(MainMenu, "caitqharassmode");
            UseEOnEnemiesCloserThanSlider = getSliderItem(MainMenu, "caitescape");

            DrawRange = getSliderItem(MainMenu, "caitdrawrange");
            DrawQRange = getCheckBoxItem(MainMenu, "caitqrange");
            DrawRRange = getCheckBoxItem(MainMenu, "caitrrange");

            AlwaysQAfterE = getCheckBoxItem(MainMenu, "caitalwaysqaftere");
            FocusOnHeadShotting = getCheckBoxItem(MainMenu, "caitfocusonheadshottingenemies");
            UseWInterrupt = getCheckBoxItem(AutoWConfig, "caitusewinterrupt");
            OnlyUseEOnMelees = getCheckBoxItem(MainMenu, "caiteonlymelees");
            UseEAntiGapclose = getCheckBoxItem(MainMenu, "caiteantigapclose");

        }

        #region Logic

        void QLogic()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (UseQCombo && Q.IsReady() && ObjectManager.Player.CountEnemyHeroesInRange(800) == 0
                    && ObjectManager.Player.CountEnemyHeroesInRange(1100) > 0)
                {
                    Q.CastIfWillHit(TargetSelector.GetTarget(1100, DamageType.Physical), 2);
                    var goodQTarget =
                        ValidTargets.FirstOrDefault(
                            t =>
                            t.Distance(ObjectManager.Player) < 1150 && t.Health < Q.GetDamage(t)
                            || SquishyTargets.Contains(t.CharData.BaseSkinName));
                    if (goodQTarget != null)
                    {
                        var pred = Q.GetPrediction(goodQTarget);
                        if ((int)pred.Hitchance > (int)HitChance.Medium)
                        {
                            Q.Cast(pred.UnitPosition);
                        }
                    }
                }
            }
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                && ObjectManager.Player.CountEnemyHeroesInRange(850) == 0)
            {
                var qHarassMode = QHarassMode;
                if (qHarassMode != 2)
                {
                    var qTarget = TargetSelector.GetTarget(1100, DamageType.Physical);
                    if (qTarget != null)
                    {
                        var pred = Q.GetPrediction(qTarget);
                        if ((int)pred.Hitchance > (int)HitChance.Medium)
                        {
                            if (qHarassMode == 1)
                            {
                                Q.Cast(pred.UnitPosition);
                            }
                            else if (pred.CollisionObjects.Count == 0)
                            {
                                Q.Cast(pred.UnitPosition);
                            }
                        }
                    }
                }
            }
        }

        void WLogic()
        {
            var goodTarget =
                ValidTargets.FirstOrDefault(
                    e =>
                    e.IsValidTarget(820) && e.HasBuffOfType(BuffType.Knockup) || e.HasBuffOfType(BuffType.Snare)
                    || e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Suppression) || e.IsCharmed
                    || e.IsCastingInterruptableSpell() || e.HasBuff("ChronoRevive") || e.HasBuff("ChronoShift"));
            if (goodTarget != null)
            {
                var pos = goodTarget.ServerPosition;
                if (pos.Distance(ObjectManager.Player.ServerPosition) < 820)
                {
                    W.Cast(goodTarget.ServerPosition);
                }
            }
            foreach (var enemyMinion in
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        m =>
                        m.IsEnemy && m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < W.Range
                        && m.HasBuff("teleport_target")))
            {

                W.Cast(enemyMinion.ServerPosition);
            }
        }

        void RLogic()
        {
            if (UseRCombo && R.IsReady() && ObjectManager.Player.CountEnemyHeroesInRange(900) == 0)
            {
                foreach (var rTarget in
                    ValidTargets.Where(
                        e =>
                        SquishyTargets.Contains(e.CharData.BaseSkinName) && R.GetDamage(e) > 0.1 * e.MaxHealth
                        || R.GetDamage(e) > e.Health))
                {
                    if (rTarget.Distance(ObjectManager.Player) > 1400)
                    {
                        var pred = R.GetPrediction(rTarget);
                        if (!pred.CollisionObjects.Any(obj => obj is AIHeroClient))
                        {
                            R.CastOnUnit(rTarget);
                        }
                        break;
                    }
                    R.CastOnUnit(rTarget);
                }
            }
        }

        #endregion

        private bool HasPassive => ObjectManager.Player.HasBuff("caitlynheadshot");

        private string[] SquishyTargets =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia",
                "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus",
                "Katarina", "Kennen", "KogMaw", "Kindred", "Leblanc", "Lucian", "Lux",
                "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon", "Teemo",
                "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "Velkoz",
                "Viktor", "Xerath", "Zed", "Ziggs", "Jhin", "Soraka"
            };
    }
}
