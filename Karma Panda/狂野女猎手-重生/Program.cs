using System.Linq;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;

// ReSharper disable InconsistentNaming

namespace NidaleeBuddyEvolution
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;

    internal class Program
    {
        /// <summary>
        /// Javelin Toss
        /// </summary>
        public static Spell.Skillshot QHuman;

        /// <summary>
        /// Bushwhack
        /// </summary>
        public static Spell.Skillshot WHuman;

        /// <summary>
        /// Primal Surge
        /// </summary>
        public static Spell.Targeted EHuman;

        /// <summary>
        /// Takedown
        /// </summary>
        public static Spell.Targeted QCat;

        /// <summary>
        /// Pounce
        /// </summary>
        public static Spell.Skillshot WCat;

        /// <summary>
        /// Swipe
        /// </summary>
        public static Spell.Skillshot ECat;

        /// <summary>
        /// Javelin -> Pounce
        /// </summary>
        public static Spell.Skillshot WExtended;

        /// <summary>
        /// Aspect of the Cougar
        /// </summary>
        public static Spell.Active R;

        /// <summary>
        /// Smite
        /// </summary>
        public static Spell.Targeted Smite;

        /// <summary>
        /// Ignite
        /// </summary>
        public static Spell.Targeted Ignite;

        /// <summary>
        /// Stores Damage Indicator
        /// </summary>
        public static DamageIndicator.DamageIndicator Indicator;

        /// <summary>
        /// Called when Program Starts
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when Loading is Completed
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Nidalee)
            {
                return;
            }

            // Human Form
            QHuman = new Spell.Skillshot(SpellSlot.Q, 1500, SkillShotType.Linear, 500, 1300, 40)
            {
                AllowedCollisionCount = 0
            };
            WHuman = new Spell.Skillshot(SpellSlot.W, 875, SkillShotType.Circular, 500, 1450, 100);
            EHuman = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);

            // Javelin Toss -> Pounce
            WExtended = new Spell.Skillshot(SpellSlot.W, 740, SkillShotType.Circular, 500, int.MaxValue, 210);

            // Cougar Form
            QCat = new Spell.Targeted(SpellSlot.Q, 500);
            WCat = new Spell.Skillshot(SpellSlot.W, 375, SkillShotType.Circular, 500, int.MaxValue, 210);
            ECat = new Spell.Skillshot(SpellSlot.E, 310, SkillShotType.Cone, 500, int.MaxValue,
                (int) (15.00*Math.PI/180.00))
            {
                ConeAngleDegrees = 90
            };

            // Ignite
            if (Essentials.HasSpell("ignite"))
            {
                Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            }

            // Smite
            if (Essentials.HasSpell("smite"))
            {
                Essentials.SetSmiteSlot();
            }

            // Initializes the Menu
            NidaleeMenu.Create();

            // Initializes the DamageIndicator
            Indicator = new DamageIndicator.DamageIndicator();

            // Prints Success Message
            Chat.Print("Sucessfully Injected NidaleeBuddy Evolution", System.Drawing.Color.Green);

            // Events
            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Events.SpellsOnUpdate;
            Game.OnUpdate += Events.KillSteal;
            Game.OnUpdate += Events.JungleSteal;
            Game.OnUpdate += Events.AutoE;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        /// Called when a Minion is unkillable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            var useQC = NidaleeMenu.LastHitMenu["useQC"].Cast<CheckBox>().CurrentValue;
            var useEC = NidaleeMenu.LastHitMenu["useEC"].Cast<CheckBox>().CurrentValue;
            var useR = NidaleeMenu.LastHitMenu["useR"].Cast<CheckBox>().CurrentValue;

            if (target == null)
            {
                return;
            }

            if (Essentials.CatForm())
            {
                if (useQC && QCat.IsReady() && target.IsValidTarget(QCat.Range) &&
                    Essentials.DamageLibrary.CalculateDamage(target, true, false, false, false) >= args.RemainingHealth)
                {
                    QCat.Cast(target);
                }

                if (useEC && ECat.IsReady() && target.IsValidTarget(ECat.Range) &&
                    Essentials.DamageLibrary.CalculateDamage(target, false, false, true, false) >= args.RemainingHealth)
                {
                    ECat.Cast(target.ServerPosition);
                }

                if (useR && R.IsReady() && !Player.Instance.IsInAutoAttackRange(target) &&
                    Player.Instance.Distance(target) <= Essentials.HumanRange &&
                    args.RemainingHealth <= Player.Instance.GetAutoAttackDamage(target))
                {
                    R.Cast();
                    Orbwalker.ForcedTarget = target;
                }
            }
        }

        /// <summary>
        /// Called when a Game Object Gets Deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null) return;

            if (missile.SpellCaster.IsMe && missile.SData.Name == "JavelinToss")
            {
                var target =
                    EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget() && Essentials.IsHunted(t));

                if (target == null)
                {
                    return;
                }

                Essentials.LastHuntedTarget = target;
            }
        }

        /// <summary>
        /// Called when Game Draws
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Essentials.CatForm()) return;

            if (NidaleeMenu.DrawingMenu["drawQH"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(QHuman.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, QHuman.Range,
                    Player.Instance.Position);
            }

            if (NidaleeMenu.DrawingMenu["drawPred"].Cast<CheckBox>().CurrentValue)
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget() && QHuman.IsInRange(t))
                        .OrderBy(t => t.Distance(Player.Instance))
                        .FirstOrDefault();

                if (enemy != null)
                {
                    var qPred = QHuman.GetPrediction(enemy).CastPosition;
                    Essentials.DrawLineRectangle(qPred.To2D(), Player.Instance.Position.To2D(), QHuman.Width, 1,
                        QHuman.IsReady() ? System.Drawing.Color.Yellow : System.Drawing.Color.Red);
                }
            }
        }

        /// <summary>
        /// Called when a spell gets casted.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Args</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "pounce")
            {
                var unit = args.Target as AIHeroClient;

                if (unit == null)
                {
                    return;
                }

                if (unit.IsValidTarget() && Essentials.IsHunted(unit))
                    Essentials.TimeStamp["Pounce"] = Game.Time + 1.5f;
                else
                    Essentials.TimeStamp["Pounce"] = Game.Time + (5 + (5*Player.Instance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "swipe")
            {
                Essentials.TimeStamp["Swipe"] = Game.Time + (5 + (5*Player.Instance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "primalsurge")
            {
                Essentials.TimeStamp["Primalsurge"] = Game.Time + (12 + (12*Player.Instance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "bushwhack")
            {
                var wperlevel = new[] {0, 13, 12, 11, 10, 9}[WCat.Level];
                Essentials.TimeStamp["Bushwhack"] = Game.Time +
                                                    (wperlevel + (wperlevel*Player.Instance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "javelintoss")
            {
                Essentials.TimeStamp["Javelin"] = Game.Time + (6 + (6*Player.Instance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.IsAutoAttack() && Player.Instance.HasBuff("Takedown"))
            {
                Essentials.TimeStamp["Takedown"] = Game.Time + (5 + (5*Player.Instance.PercentCooldownMod));
            }
        }

        /// <summary>
        /// Called after a attack.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var t = target as AIHeroClient;
            var m = target as Obj_AI_Base;

            if (!NidaleeMenu.MiscMenu["useQC_AfterAttack"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                NidaleeMenu.ComboMenu["useQC"].Cast<CheckBox>().CurrentValue && t != null)
            {
                if (t.IsValidTarget(QCat.Range) && !target.IsStructure())
                {
                    QCat.Cast(t);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                NidaleeMenu.LaneClearMenu["useQC"].Cast<CheckBox>().CurrentValue && m != null)
            {
                if (m.IsValidTarget(QCat.Range) && !target.IsStructure())
                {
                    QCat.Cast(m);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                NidaleeMenu.JungleClearMenu["useQC"].Cast<CheckBox>().CurrentValue && m != null)
            {
                if (m.IsValidTarget(QCat.Range) && !target.IsStructure())
                {
                    QCat.Cast(m);
                }
            }
        }

        /// <summary>
        /// Called before a attack.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!NidaleeMenu.MiscMenu["useQC_BeforeAttack"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                NidaleeMenu.ComboMenu["useQC"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(QCat.Range) && !target.IsStructure())
                {
                    QCat.Cast(target as AIHeroClient);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                NidaleeMenu.LaneClearMenu["useQC"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(QCat.Range) && !target.IsStructure())
                {
                    QCat.Cast(target as Obj_AI_Base);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                NidaleeMenu.JungleClearMenu["useQC"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(QCat.Range) && !target.IsStructure())
                {
                    QCat.Cast(target as Obj_AI_Base);
                }
            }
        }

        /// <summary>
        /// Called when Game Updates
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ForcedTarget != null)
            {
                if (!Player.Instance.IsInAutoAttackRange(Orbwalker.ForcedTarget) ||
                    !Orbwalker.ForcedTarget.IsValidTarget() || Orbwalker.ForcedTarget.IsInvulnerable)
                {
                    Orbwalker.ForcedTarget = null;
                }

                var target = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange(),
                    DamageType.Physical);

                if (Orbwalker.ForcedTarget != null && target != null &&
                    ((Orbwalker.ForcedTarget.NetworkId != target.NetworkId) &&
                     Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
                {
                    Orbwalker.ForcedTarget = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange(),
                        DamageType.Physical);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                StateManager.JungleClear();
            }
        }
    }
}