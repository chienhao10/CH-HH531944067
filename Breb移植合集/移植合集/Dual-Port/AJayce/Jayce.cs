using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using SkillshotType = SebbyLib.Prediction.SkillshotType;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace Jayce
{
    internal class Jayce : Helper
    {
        public static LeagueSharp.Common.Spell Q, W, E, R, Qm, Wm, Em, Qe;
        public static PredictionInput qpred;
        public static PredictionInput qpred1;
        public static void OnLoad()
        {
            if (Player.ChampionName != "Jayce") return;
            MenuConfig.OnLoad();
            //ranged
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1050);
            Qe = new LeagueSharp.Common.Spell(SpellSlot.Q, 1470);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, int.MaxValue);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 650);

            //   melee
            Qm = new LeagueSharp.Common.Spell(SpellSlot.Q, 600);
            Wm = new LeagueSharp.Common.Spell(SpellSlot.W, int.MaxValue);
            Em = new LeagueSharp.Common.Spell(SpellSlot.E, 240);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, int.MaxValue);
            qpred = new PredictionInput
            {
                Aoe = false,
                Collision = false,
                Speed = Qe.Speed,
                Delay = Qe.Delay,
                Range = Qe.Range,
                Radius = Qe.Width,
                Type = SkillshotType.SkillshotLine
            };

            qpred1 = new PredictionInput
            {
                Aoe = false,
                Collision = false,
                Speed = Q.Speed,
                Delay = Q.Delay,
                Range = Q.Range,
                Radius = Q.Width,
                Type = SkillshotType.SkillshotLine
            };

            Q.SetSkillshot(0.3f, 70f, 1500, true, LeagueSharp.Common.SkillshotType.SkillshotLine);
            Qe.SetSkillshot(0.3f, 70f, 2180, true, LeagueSharp.Common.SkillshotType.SkillshotLine);
            Qm.SetTargetted(0f, float.MaxValue);
            Em.SetTargetted(0f, float.MaxValue);
            Game.OnUpdate += OnUpdate;
            Spellbook.OnCastSpell += OnCastSpell;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += GeneralOnUpdate;
            Obj_AI_Base.OnSpellCast += OnDoCastRange;
            Obj_AI_Base.OnSpellCast += OnDoCastMelee;
            Obj_AI_Base.OnSpellCast += LaneClear;
            CustomEvents.Unit.OnDash += OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapClose;
            Interrupter2.OnInterruptableTarget += OnInterrupt;
            EloBuddy.Player.OnIssueOrder += OnOrder;
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

        private static void OnOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe) return;
            var on = getCheckBoxItem(MenuConfig.Config, "disorb");
            if (!on) return;
            var target = TargetSelector.GetTarget(1050, DamageType.Physical);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var spellbook = Player.Spellbook.GetSpell(SpellSlot.W);
                if (spellbook.State == SpellState.Surpressed && W.Level != 0)
                {
                    if (target.LSDistance(Player) <= Orbwalking.GetRealAutoAttackRange(target) - 10)
                    {
                        if (args.Order == GameObjectOrder.MoveTo)
                        {
                            args.Process = false;
                        }
                    }
                }
            }
        }

        private static void OnInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(MenuConfig.misc, "autoeint")) return;
            if (sender.IsMe || sender.IsAlly) return;
            if (sender.LSDistance(Player) <= Em.Range)
            {
                if (!Ismelee())
                {
                    R.Cast();
                }

                if (Ismelee())
                {
                    Em.Cast(sender);
                }
            }
        }

        private static void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (!getCheckBoxItem(MenuConfig.misc, "autoegap")) return;
            if (gapcloser.Sender.IsMe || gapcloser.Sender.IsAlly) return;

            if (!Ismelee() && R.IsReady())
            {
                R.Cast();
            }
            if (gapcloser.End.LSDistance(Player.Position) < Em.Range && Ismelee())
            {
                Em.Cast(gapcloser.Sender);
            }
        }


        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!getCheckBoxItem(MenuConfig.misc, "autoedash")) return;
            if (sender.IsMe || sender.IsAlly) return;
            if (args.Unit == null) return;

            if (Ismelee() && R.IsReady())
            {
                Em.Cast(args.Unit);
            }

        }

        private static void OnDoCastMelee(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

        }


        private static void OnDoCastRange(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name.ToLower().Contains("shockblast") && !Ismelee())
            {
                if (getKeyBindItem(MenuConfig.Config, "manualeq"))
                {
                    var pos = Player.Position.Extend(Game.CursorPos, Player.BoundingRadius + 150);
                    E.Cast(pos);
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && getCheckBoxItem(MenuConfig.combo, "useecr"))
                {
                    var target = TargetSelector.GetTarget(1470, DamageType.Physical);
                    if (target == null) return;
                    var castposition = Player.Position.Extend(target.Position, Player.BoundingRadius + 150);
                    E.Cast(castposition);
                }
            }
            if (!Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (!sender.IsMe) return;
            if (!args.SData.IsAutoAttack()) return;
            if (args.Target.Type != GameObjectType.AIHeroClient) return;
            if (Ismelee()) return;
            thisunit = (AIHeroClient)args.Target;
            if (W.IsReady())
            {
                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && getCheckBoxItem(MenuConfig.combo, "usewcr"))
                    || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && getCheckBoxItem(MenuConfig.harass, "usewhr")))
                {
                    W.Cast();
                    Orbwalker.ForcedTarget = ((AIHeroClient)args.Target);

                    // Orbwalking.ResetAutoAttackTimer();
                }
            }
        }


        private static void LaneClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) return;
            if (!Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (!sender.IsMe) return;
            if (!args.SData.IsAutoAttack()) return;
            if (Ismelee()) return;
            var obj = (Obj_AI_Base)args.Target;
            if (!getCheckBoxItem(MenuConfig.laneclear, "usewlr")) return;
            if (getSliderItem(MenuConfig.laneclear, "minmana") > Player.ManaPercent) return;

            if (W.IsReady() && obj.Health > Player.GetAutoAttackDamage(obj) + 30)
            {
                W.Cast();
                Orbwalker.ForcedTarget = ((Obj_AI_Base)args.Target);
            }
            var minions =
                MinionManager.GetMinions(Player.Position, 300);
            foreach (var min in minions.Where(
                x => x.NetworkId != ((Obj_AI_Base)args.Target).NetworkId && x.Health < Player.GetAutoAttackDamage(x) + 15))
            {
                if (obj.Health < Player.GetAutoAttackDamage(obj))
                {
                    if (W.IsReady())
                    {
                        W.Cast();
                        Orbwalker.ForcedTarget = min;
                    }
                }
            }
        }


        private static void GeneralOnUpdate(EventArgs args)
        {

            if (getKeyBindItem(MenuConfig.Config, "manualeq"))
            {
                ManualEq();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                FormChangeManager();
                if (!Ismelee())
                    Combo();
                else
                    Combomelee();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Laneclearrange(); // also has Melee Q
                Laneclearmelee();
            }
        }

        private static void Laneclearmelee()
        {
            if (getSliderItem(MenuConfig.laneclear, "minmana") > Player.ManaPercent) return;
            if (!Ismelee()) return;
            var min =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(x => x.LSDistance(Player) < 300 && !x.IsDead && x.IsEnemy).ToList();

            if (min.FirstOrDefault() == null)
            {
                minionscirclemelee = null;
                return;
            }



            foreach (var minions in min)
            {
                minionscirclemelee = new LeagueSharp.Common.Geometry.Polygon.Circle(minions.Position, 300);
                if (E.IsReady() && getCheckBoxItem(MenuConfig.laneclear, "useelm"))
                {
                    if (minions.Health < EMeleeDamage(minions))
                    {
                        Em.Cast(minions);
                    }
                }
            }

            var count = min.Where(x => minionscirclemelee.IsInside(x));
            var objAiMinions = count as IList<Obj_AI_Minion> ?? count.ToList();
            if (objAiMinions.Count() >= getSliderItem(MenuConfig.laneclear, "minhitwq"))
            {
                if (W.IsReady() && getCheckBoxItem(MenuConfig.laneclear, "usewlm"))
                    W.Cast();

                if (Q.IsReady() && getCheckBoxItem(MenuConfig.laneclear, "useqlm"))
                    Qm.Cast(objAiMinions.FirstOrDefault());
            }
        }

        private static void Laneclearrange()
        {
            if (getSliderItem(MenuConfig.laneclear, "minmana") > Player.ManaPercent) return;
            var min =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(x => x.LSDistance(Player) < Q.Range - 200 && !x.IsDead && x.IsEnemy && x.IsTargetable);


            var objAiMinions = min as IList<Obj_AI_Minion> ?? min.ToList();
            foreach (var minions in objAiMinions)
            {
                minionscircle = new LeagueSharp.Common.Geometry.Polygon.Circle(minions.Position, 250);
            }

            var count = objAiMinions.Where(x => minionscircle.IsInside(x));

            if (count.Count() < getSliderItem(MenuConfig.laneclear, "minhitwq")) return;
            if (!Ismelee() && Q.IsReady() && getCheckBoxItem(MenuConfig.laneclear, "useqlr"))
                Q.Cast(minionscircle.Center);
        }


        private static void ManualEq()
        {
            //   Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (Ismelee())
            {
                if (R.IsReady())
                    R.Cast();
            }

            if (Ismelee()) return;
            if (E.IsReady() && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
            }
        }

        private static void FormChangeManager()
        {
            if (!getCheckBoxItem(MenuConfig.combo, "usercf")) return;
            var target = TargetSelector.GetTarget(1470, DamageType.Physical);
            if (!target.IsValidTarget()) return;
            if (!R.IsReady()) return;

            if (Ismelee())
            {
                var aarange = Orbwalking.GetRealAutoAttackRange(target);
                if (SpellTimer["Qm"] > 1.1f && SpellTimer["Em"] > 0.4f && (Player.LSDistance(target) > aarange + 50 || SpellTimer["W"] < 0.8f))
                {
                    R.Cast();
                }

                if (target.LSDistance(Player) > Qm.Range + 30)
                {
                    R.Cast();
                }

                if (Player.Mana < Q.ManaCost && Player.LSDistance(target) > aarange)
                {
                    R.Cast();
                }
            }
            else
            {
                var getpred = Q.GetPrediction(target);
                var spellbook = Player.Spellbook.GetSpell(SpellSlot.W);
                var spellbookq = Player.Spellbook.GetSpell(SpellSlot.Q);
                if (target.IsValidTarget(Qm.Range - 20) && Player.Mana >= spellbookq.SData.Mana)
                {
                    if (getpred.Hitchance == HitChance.Collision || !Q.IsReady())
                    {
                        if (spellbook.State != SpellState.Surpressed &&
                            spellbook.Level != 0)
                        {
                            if (SpellTimer["Q"] > 1.2f && SpellTimer["W"] > 0.7f)
                            {
                                R.Cast();
                            }
                        }
                    }

                    if (SpellTimer["Q"] > 1.1 && (spellbook.State != SpellState.Surpressed ||
                        spellbook.Level == 0))
                    {
                        R.Cast();
                    }

                    if (target.Health <= QMeleeDamage() && Ready("Qm") && target.LSDistance(Player) < Qm.Range)
                    {
                        R.Cast();
                    }

                    if (target.Health < QMeleeDamage() + EMeleeDamage(target) && Ready("Qm") && Ready("Em") &&
                        target.LSDistance(Player) < Qm.Range)
                    {
                        R.Cast();
                    }
                }
            }
        }

        public static double QMeleeDamage()
        {
            return new double[] { 30, 70, 110, 150, 190, 230 }[Q.Level - 1]
                   + 1 * Player.FlatPhysicalDamageMod;
        }

        public static double EMeleeDamage(Obj_AI_Base target)
        {
            return (new[] { 8, 10.4, 12.8, 15.2, 17.6, 20 }[Q.Level - 1] / 100) * target.MaxHealth
                 + 1 * Player.FlatPhysicalDamageMod;
        }



        private static void Combomelee()
        {
            // if (Player.IsWindingUp) return;
            var target = TargetSelector.GetTarget(Qm.Range, DamageType.Physical);
            if (target == null) return;
            var expires = (Player.Spellbook.GetSpell(SpellSlot.R).CooldownExpires);
            var CD =
                (int)
                    (expires -
                     (Game.Time - 1));
            if (Player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target))
            {
                if (Wm.IsReady())
                    Wm.Cast();
            }

            foreach (var x in HeroManager.Enemies.Where(z => z.IsValidTarget(Em.Range)))
            {
                if (x.Health < EMeleeDamage(target) + 100)
                {
                    Em.Cast(target);
                }
            }

            if (Player.LSDistance(target) <= Em.Range - 80)
            {
                if (Qm.IsReady() && !Em.IsReady() && getCheckBoxItem(MenuConfig.combo, "useqcm"))
                {
                    Qm.Cast(target);
                }

                if (SpellTimer["Em"] > 1.6 && Qm.IsReady())
                {
                    Qm.Cast(target);
                }

                if (Em.IsReady() && getCheckBoxItem(MenuConfig.combo, "useecm"))
                {
                    var aarange = Orbwalking.GetRealAutoAttackRange(target);
                    if (SpellTimer["Qm"] < 2.2 &&
                        (Player.LSDistance(target) < aarange + 100 || (SpellTimer["Q"] < 1.2 && CD < 1.5)))
                    {
                        Em.Cast(target);
                    }

                    if (target.Health < EMeleeDamage(target) + 90)
                    {
                        Em.Cast(target);
                    }
                }
            }
            else
            {
                if ((SpellTimer["Q"] < 1.5 || SpellTimer["W"] < 0.8) && CD < 1 && Em.IsReady() && getCheckBoxItem(MenuConfig.combo, "useecm"))
                {
                    Em.Cast(target);
                }
                if (Qm.IsReady() && getCheckBoxItem(MenuConfig.combo, "useqcm"))
                {
                    Qm.Cast(target);
                }

            }
        }
        private static void Harass()
        {

            var target = TargetSelector.GetTarget(Qe.Range, DamageType.Physical);
            if (target == null) return;
            qpred.From = Qe.GetPrediction(target).CastPosition;
            qpred1.From = Q.GetPrediction(target).CastPosition;
            if (!Ismelee())
            {
                if (Q.IsReady() && E.IsReady() && Player.Mana >
                Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana + Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana
                    && getCheckBoxItem(MenuConfig.harass, "useqhr"))
                {
                    Qe.Cast(qpred.From);
                }

                if (Q.IsReady() && target.IsValidTarget(Q.Range) && (!E.IsReady() || Player.Mana <
                                    Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana +
                                    Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana)
                    && getCheckBoxItem(MenuConfig.harass, "useqhr"))
                {
                    Q.Cast(qpred1.From);
                }
            }
            else
            {
                if (Q.IsReady() && getCheckBoxItem(MenuConfig.harass, "useqhm"))
                {
                    Q.Cast(target);
                }
            }
        }
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Qe.Range, DamageType.Physical);
            if (target == null) return;
            var prede = Q.GetPrediction(target);
            var pred = Qe.GetPrediction(target);
            if (pred.CollisionObjects.Count >= 1) return;

            qpred.From = Qe.GetPrediction(target).CastPosition;
            qpred1.From = Q.GetPrediction(target).CastPosition;

            if (Q.IsReady() && E.IsReady() && getCheckBoxItem(MenuConfig.combo, "useqcr") &&
                Player.Mana >
                Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana + Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana)
            {
                Qe.Cast(qpred.From);
            }

            if ((Q.IsReady() && !E.IsReady()) || (Q.IsReady() && E.IsReady() && Player.Mana <
                Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana + Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana))
            {
                if (Player.LSDistance(target) < Q.Range && getCheckBoxItem(MenuConfig.combo, "useqcr"))
                {
                    Q.Cast(qpred1.From);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var x = Drawing.WorldToScreen(Player.Position).X;
            var y = Drawing.WorldToScreen(Player.Position).Y;
            if (Ismelee())
            {
                if (getCheckBoxItem(MenuConfig.drawings, "drawtimers"))
                {
                    Drawing.DrawText(x - 80, y, Color.Red,
                        "[Q] :" + ((int)SpellTimer["Q"]).ToString(CultureInfo.InvariantCulture));
                    Drawing.DrawText(x - 20, y, Color.Red,
                        "[W] :" + ((int)SpellTimer["W"]).ToString(CultureInfo.InvariantCulture));
                    Drawing.DrawText(x + 50, y, Color.Red,
                        "[E] :" + ((int)SpellTimer["E"]).ToString(CultureInfo.InvariantCulture));
                }

                if (Q.Level >= 1 && getCheckBoxItem(MenuConfig.drawings, "drawq"))
                {
                    Render.Circle.DrawCircle(Player.Position, Qm.Range, Color.Violet, 4);
                }

                if (E.Level >= 1 && getCheckBoxItem(MenuConfig.drawings, "drawe"))
                {
                    Render.Circle.DrawCircle(Player.Position, Em.Range, Color.Blue, 4);
                }
            }

            if (!Ismelee())
            {
                if (getCheckBoxItem(MenuConfig.drawings, "drawtimers"))
                {
                    Drawing.DrawText(x - 80, y, Color.Red,
                        "[Q] :" + ((int)SpellTimer["Q"]).ToString(CultureInfo.InvariantCulture));
                    Drawing.DrawText(x - 20, y, Color.Red,
                        "[W] :" + ((int)SpellTimer["W"]).ToString(CultureInfo.InvariantCulture));
                    Drawing.DrawText(x + 50, y, Color.Red,
                        "[E] :" + ((int)SpellTimer["E"]).ToString(CultureInfo.InvariantCulture));
                }
                if (Q.Level >= 1 && getCheckBoxItem(MenuConfig.drawings, "drawq"))
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Violet, 4);
                }

                if (E.Level >= 1 && getCheckBoxItem(MenuConfig.drawings, "drawe"))
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.Blue, 4);
                }
            }


        }

        public static bool Ismelee()
        {
            return Player.HasBuff("JayceStanceHammer");
        }

        private static void OnUpdate(EventArgs args)
        {
            SpellTimer["Q"] = ((TimeStamp["Q"] - Game.Time) > 0)
                ? (TimeStamp["Q"] - Game.Time)
                : 0;

            SpellTimer["Qm"] = ((TimeStamp["Qm"] - Game.Time) > 0)
                ? (TimeStamp["Qm"] - Game.Time)
                : 0;

            SpellTimer["E"] = ((TimeStamp["E"] - Game.Time) > 0)
                ? (TimeStamp["E"] - Game.Time)
                : 0;

            SpellTimer["Em"] = ((TimeStamp["Em"] - Game.Time) > 0)
                ? (TimeStamp["Em"] - Game.Time)
                : 0;
            SpellTimer["W"] = ((TimeStamp["W"] - Game.Time) > 0)
                ? (TimeStamp["W"] - Game.Time)
                : 0;

            SpellTimer["Wm"] = ((TimeStamp["Wm"] - Game.Time) > 0)
                ? (TimeStamp["Wm"] - Game.Time)
                : 0;

        }

        internal static Dictionary<string, float> TimeStamp = new Dictionary<string, float>
        {
            {"Q", 0f},
            {"W", 0f},
            {"E", 0f},
            {"Qm", 0f},
            {"Em", 0f},
            {"Wm", 0f}
        };

        /// <summary>
        /// Stores the current tickcount of the spell.
        /// </summary>
        internal static Dictionary<string, float> SpellTimer = new Dictionary<string, float>
        {
            {"Q", 0f},
            {"W", 0f},
            {"E", 0f},
            {"Qm", 0f},
            {"Em", 0f},
            {"Wm", 0f}
        };

        private static LeagueSharp.Common.Geometry.Polygon.Circle minionscircle;
        private static LeagueSharp.Common.Geometry.Polygon.Circle minionscirclemelee;
        private static AIHeroClient thisunit;


        public static float Cooldown
        {
            get { return Player.PercentCooldownMod; }
        }

        public static bool Ready(string spell)
        {
            return SpellTimer[spell] < 0.4f;
        }


        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

            if (!Ismelee())
            {
                if (args.Slot == SpellSlot.Q)
                {
                    var qperlevel = new[] { 8, 8, 8, 8, 8, 8 }[Q.Level - 1];
                    TimeStamp["Q"] = Game.Time + 1.5f + (qperlevel + (qperlevel * Cooldown));
                }

                if (args.Slot == SpellSlot.W)
                {
                    var wperlevel = new[] { 13, 11.4f, 9.8f, 8.2f, 6.6f, 5 }[W.Level - 1];
                    TimeStamp["W"] = Game.Time + 2.0f + (wperlevel + (wperlevel * Cooldown));
                }

                if (args.Slot == SpellSlot.E)
                {
                    var eperlevel = new[] { 16, 16, 16, 16, 16, 16 }[E.Level - 1];
                    TimeStamp["E"] = Game.Time + 1.5f + (eperlevel + (eperlevel * Cooldown));
                }
            }
            else
            {
                if (args.Slot == SpellSlot.Q)
                {
                    var qmperlevel = new[] { 16, 14, 12, 10, 8, 6 }[Qm.Level - 1];
                    TimeStamp["Qm"] = Game.Time + 1.5f + (qmperlevel + (qmperlevel * Cooldown));
                }

                if (args.Slot == SpellSlot.W)
                {
                    var wmperlevel = new[] { 10, 10, 10, 10, 10, 10 }[Wm.Level - 1];
                    TimeStamp["Wm"] = Game.Time + 1.5f + (wmperlevel + (wmperlevel * Cooldown));
                }

                if (args.Slot == SpellSlot.E)
                {
                    var emperlevel = new[] { 15, 14, 13, 12, 11, 10 }[Em.Level - 1];
                    TimeStamp["Em"] = Game.Time + 1.5f + (emperlevel + (emperlevel * Cooldown));
                }
            }

        }
    }
}

