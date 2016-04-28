using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using LeagueSharp.Common;
using SharpDX;
using Utility = LeagueSharp.Common.Utility;
using System.Collections.Generic;

namespace NechritoRiven
{
    public class Program
    {
        public const string IsFirstR = "RivenFengShuiEngine";
        public const string IsSecondR = "RivenIzunaBlade";
        public static Menu Menu;
        public static readonly AIHeroClient Player = ObjectManager.Player;
        public static SpellSlot Flash;
        public static int _qstack = 1;
        public static Render.Text Timer, Timer2;

        public static readonly Dictionary<string, Vector3> JumpPos = new Dictionary<string, Vector3>()
        {
            { "mid_Dragon" , new Vector3 (9122f, 4058f, 53.95995f) },
            { "left_dragon" , new Vector3 (9088f, 4544f, 52.24316f) },
            { "baron" , new Vector3 (5774f, 10706f, 55.77578F) },
            { "red_wolves" , new Vector3 (11772f, 8856f, 50.30728f) },
            { "blue_wolves" , new Vector3 (3046f, 6132f, 57.04655f) },
        };

        public static int GetWRange
        {
            get { return Player.HasBuff("RivenFengShuiEngine") ? 330 : 265; }
        }


        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Riven") return;

            Timer =
                new Render.Text(
                    "Q Expiry =>  " + ((double) (Logic._lastQ - Utils.GameTimeTickCount + 3800)/1000).ToString("0.0"),
                    (int) Drawing.WorldToScreen(Player.Position).X - 140,
                    (int) Drawing.WorldToScreen(Player.Position).Y + 10, 30, Color.DodgerBlue, "calibri");
            Timer2 =
                new Render.Text(
                    "R Expiry =>  " + (((double) Logic._lastR - Utils.GameTimeTickCount + 15000)/1000).ToString("0.0"),
                    (int) Drawing.WorldToScreen(Player.Position).X - 60,
                    (int) Drawing.WorldToScreen(Player.Position).Y + 10, 30, Color.DodgerBlue, "calibri");
            Spells.Ignite = Player.GetSpellSlot("summonerdot");

            MenuConfig.LoadMenu();
            Spells.Initialise();

            Game.OnUpdate += OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Logic.OnCast;
            Obj_AI_Base.OnProcessSpellCast += OnCasting;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            Obj_AI_Base.OnSpellCast += OnDoCastLc;
            Obj_AI_Base.OnPlayAnimation += OnPlay;
            Interrupter2.OnInterruptableTarget += Interrupt;
        }

        public static bool HasTitan()
        {
            return Items.HasItem(3748) && Items.CanUseItem(3748);
        }


        public static void CastTitan()
        {
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                Items.UseItem(3748);
                Orbwalker.ResetAutoAttack();
            }
        }

        public static bool IsLethal(Obj_AI_Base unit)
        {
            return Dmg.Totaldame(unit)/1.65 >= unit.Health;
        }

        private static void OnDoCastLc(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(args.SData.Name)) return;
            Logic._qtarget = (Obj_AI_Base)args.Target;

            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    var minions = MinionManager.GetMinions(800f).FirstOrDefault();
                    if (minions == null)
                        return;

                    if (Spells._e.IsReady() && MenuConfig.LaneE)
                        Spells._e.Cast(minions.ServerPosition);

                    if (Spells._q.IsReady() && MenuConfig.LaneQ && !MenuConfig.FastC)
                    {
                        Spells._q.Cast(Logic.GetCenterMinion());
                        Logic.CastHydra();
                    }

                    if (Spells._q.IsReady() && MenuConfig.LaneQ && MenuConfig.FastC)
                    {
                        Logic.CastHydra();
                        Utility.DelayAction.Add(1, () => Logic.ForceCastQ(minions));
                    }

                    if (Spells._w.IsReady() && MenuConfig.LaneW)
                    {
                        var minion = MinionManager.GetMinions(Player.Position, Spells._w.Range);
                        foreach (var m in minion)
                        {
                            if (m.Health < Spells._w.GetDamage(m) && minion.Count > 2)
                                Spells._w.Cast(m);
                        }
                    }
                }
            }
        }

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spellName = args.SData.Name;
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(spellName)) return;

            if (args.Target is Obj_AI_Minion)
            {
                Lane.LaneLogic();
            }

            var @base = args.Target as Obj_AI_Turret;
            if (@base != null)
                if (@base.IsValid && args.Target != null && Spells._q.IsReady() && MenuConfig.LaneQ &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) Spells._q.Cast(@base);

            var hero = args.Target as AIHeroClient;
            if (hero == null) return;
            var target = hero;

            if (Spells._r.IsReady() && Spells._r.Instance.Name == IsSecondR)
                if (target.Health < Dmg.Rdame(target, target.Health) + Player.GetAutoAttackDamage(target) &&
                    target.Health > Player.GetAutoAttackDamage(target)) Spells._r.Cast(target.Position);
            if (Spells._w.IsReady())
                if (target.Health < Spells._w.GetDamage(target) + Player.GetAutoAttackDamage(target) &&
                    target.Health > Player.GetAutoAttackDamage(target)) Spells._w.Cast();
            if (Spells._q.IsReady())
                if (target.Health < Spells._q.GetDamage(target) + Player.GetAutoAttackDamage(target) &&
                    target.Health > Player.GetAutoAttackDamage(target)) Spells._q.Cast(target);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Spells._e.IsReady())
                    Spells._e.Cast(target.Position);

                if (Spells._w.IsReady() && Logic.InWRange(target))
                    Spells._w.Cast();

                if (Spells._q.IsReady())
                {
                    Logic.ForceItem();
                    Utility.DelayAction.Add(0, () => Logic.ForceCastQ(target));
                }
            }


            if (MenuConfig.fastHKey)
            {
                if (HasTitan())
                {
                    CastTitan();
                    return;
                }
                if (Spells._w.IsReady() && Logic.InWRange(target))
                {
                    Logic.ForceItem();
                    Utility.DelayAction.Add(1, Logic.ForceW);
                    Utility.DelayAction.Add(2, () => Logic.ForceCastQ(target));
                }
                else if (Spells._q.IsReady())
                {
                    Logic.ForceItem();
                    Utility.DelayAction.Add(1, () => Logic.ForceCastQ(target));
                }
                else if (Spells._e.IsReady() && !Orbwalking.InAutoAttackRange(target) && !Logic.InWRange(target))
                {
                    Spells._e.Cast(target.Position);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (HasTitan())
                {
                    CastTitan();
                    return;
                }
                if (_qstack == 2 && Spells._q.IsReady())
                {
                    Logic.ForceItem();
                    Utility.DelayAction.Add(1, () => Logic.ForceCastQ(target));
                }
            }

            if (!MenuConfig.burstKey)
            {
                if (HasTitan())
                {
                    CastTitan();
                    return;
                }
                if (Spells._w.IsReady())
                {
                    Spells._w.Cast(target.Position);
                }
                if (Spells._r.IsReady() && Spells._r.Instance.Name == IsSecondR)
                {
                    Logic.ForceItem();
                    Utility.DelayAction.Add(1, Logic.ForceR2);
                }
                else if (Spells._q.IsReady())
                {
                    Logic.ForceItem();
                    Utility.DelayAction.Add(1, () => Logic.ForceCastQ(target));
                }
            }
        }

        private static void Interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && Spells._w.IsReady() && sender.IsValidTarget() && !sender.IsZombie)
            {
                if (sender.IsValidTarget(125 + Player.BoundingRadius + sender.BoundingRadius)) Spells._w.Cast();
            }
        }

        private static void OnTick(EventArgs args)
        {
            Timer.X = (int) Drawing.WorldToScreen(Player.Position).X - 60;
            Timer.Y = (int) Drawing.WorldToScreen(Player.Position).Y + 43;
            Timer2.X = (int) Drawing.WorldToScreen(Player.Position).X - 60;
            Timer2.Y = (int) Drawing.WorldToScreen(Player.Position).Y + 65;
            Logic.ForceSkill();
            Killsteal();
            if (Utils.GameTimeTickCount - Logic._lastQ >= 3650 && _qstack != 1 && !Player.IsRecalling() &&
                !Player.InFountain() && MenuConfig.KeepQ && Player.HasBuff("RivenTriCleave") &&
                !Player.Spellbook.IsChanneling &&
                Spells._q.IsReady()) Spells._q.Cast(Game.CursorPos);


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.ComboLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Jungle.JungleLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass.HarassLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee.FleeLogic();
            }

            if (MenuConfig.fastHKey)
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
                FastHarass.FastHarassLogic();
            }

            if (MenuConfig.burstKey)
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
                Burst.BurstLogic();
            }
        }

        public static float QDamage(bool useR = false)
        {
            return (float) (new double[] {10, 30, 50, 70, 90}[Spells._q.Level - 1] +
                            (Spells._r.IsReady() && useR && !Player.HasBuff("RivenFengShuiEngine")
                                ? Player.TotalAttackDamage*1.2
                                : Player.TotalAttackDamage)/100*
                            new double[] {40, 45, 50, 55, 60}[Spells._q.Level - 1]);
        }

        public static float WDamage()
        {
            return (float) (new double[] {50, 80, 110, 140, 170}[Spells._w.Level - 1] +
                            1*ObjectManager.Player.FlatPhysicalDamageMod);
        }

        public static void LaneClear()
        {
            //Orbwalker.ForcedTarget = null;
            if (Logic._lastQ + 260 > Environment.TickCount) return;
            foreach (var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.IsValidTarget(Spells._w.Range)))
            {
                if (MenuConfig.LaneQ && Spells._q.IsReady() && minion.Health <= Player.CalculateDamageOnUnit(minion, DamageType.Physical, QDamage()) && !Orbwalker.IsAutoAttacking)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Q, minion.Position);
                    Logic.CastHydra();
                    return;
                }
                if (MenuConfig.LaneW && Spells._w.IsReady() && minion.Health <= Player.CalculateDamageOnUnit(minion, DamageType.Physical, WDamage()))
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                    return;
                }
            }
        }

        private static void Killsteal()
        {
            if (Spells._q.IsReady())
            {
                // R range because auto-gapclose! (Yes, i'm smart. Give Contrib pls)
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells._r.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._q.GetDamage(target) && Logic.InQRange(target))
                        Spells._q.Cast(target);
                }
            }
            if (Spells._r.IsReady() && Spells._r.Instance.Name == IsSecondR)
            {
                var targets =
                    HeroManager.Enemies.Where(x => x.IsValidTarget(Spells._r.Range + Spells._e.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._r.GetDamage(target) && !target.IsInvulnerable &&
                        (Player.Distance(target.Position) <= 1870) && (Player.Distance(target.Position) >= 1600))
                    {
                        Spells._e.Cast(target);
                        Utility.DelayAction.Add(60, () => Spells._r.Cast(target));
                    }
                }
            }
            if (Spells._w.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells._r.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._w.GetDamage(target) && Logic.InWRange(target))
                        Spells._w.Cast();
                }
            }
            if (Spells._r.IsReady() && Spells._r.Instance.Name == IsSecondR)
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells._r.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Dmg.Rdame(target, target.Health) && !target.IsInvulnerable)
                        Spells._r.Cast(target.Position);
                }
            }
            if (Spells.Ignite.IsReady())
            {
                var target = TargetSelector.GetTarget(600f, DamageType.True);
                if (target.IsValidTarget(600f) && Dmg.IgniteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Spells.Ignite, target);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);


            if (_qstack != 1 && MenuConfig.DrawTimer1)
            {
                Timer.text = "Q Expiry =>  " +
                             ((double) (Logic._lastQ - Utils.GameTimeTickCount + 3800)/1000).ToString("0.0") +
                             "s";
                Timer.OnEndScene();
            }

            if (Player.HasBuff("RivenFengShuiEngine") && MenuConfig.DrawTimer2)
            {
                Timer2.text = "R Expiry =>  " +
                              (((double) Logic._lastR - Utils.GameTimeTickCount + 15000)/1000).ToString("0.0") + "s";
                Timer2.OnEndScene();
            }

            if (MenuConfig.DrawCb)
                Render.Circle.DrawCircle(Player.Position, 250 + Player.AttackRange + 70,
                    Spells._e.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);
            if (MenuConfig.DrawBt && Flash != SpellSlot.Unknown)
                Render.Circle.DrawCircle(Player.Position, 750,
                    Spells._r.IsReady() && Flash.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);

            if (MenuConfig.DrawFh)
                Render.Circle.DrawCircle(Player.Position, 450 + Player.AttackRange + 70,
                    Spells._e.IsReady() && Spells._q.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);
            if (MenuConfig.DrawHs)
                Render.Circle.DrawCircle(Player.Position, 400,
                    Spells._q.IsReady() && Spells._w.IsReady()
                        ? System.Drawing.Color.FromArgb(120, 0, 170, 255)
                        : System.Drawing.Color.IndianRed);
            if (MenuConfig.DrawAlwaysR)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 20, System.Drawing.Color.DodgerBlue, "Force R  (     )");
                Drawing.DrawText(heropos.X + 53, heropos.Y + 20,
                    MenuConfig.AlwaysR ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red,
                    MenuConfig.AlwaysR ? "On" : "Off");
            }
            if (MenuConfig.ForceFlash)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.DodgerBlue, "Force Flash  (     )");
                Drawing.DrawText(heropos.X + 83, heropos.Y + 40,
                    MenuConfig.AlwaysF ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red,
                    MenuConfig.AlwaysF ? "On" : "Off");
            }
        }

        private static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;
            var t = 0;
            switch (args.Animation) // Logic from Fluxy
            {
                case "Spell1a":
                    Logic._lastQ = Utils.GameTimeTickCount;
                    t = 291;
                    _qstack = 2;
                    break;
                case "Spell1b":
                    Logic._lastQ = Utils.GameTimeTickCount;
                    t = 291;
                    _qstack = 3;
                    break;
                case "Spell1c": // q3?
                    Logic._lastQ = Utils.GameTimeTickCount;
                    t = 343;
                    _qstack = 1;
                    break;
                case "Spell2":
                    t = 170;
                    break;
                case "Spell3":
                    if (MenuConfig.burstKey || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                        MenuConfig.fastHKey || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                        Logic.CastYoumoo();
                    break;
                case "Spell4a":
                    t = 0;
                    Logic._lastR = Utils.GameTimeTickCount;
                    break;
                case "Spell4b":
                    t = 150;
                    var target = TargetSelector.SelectedTarget;
                    if (Spells._q.IsReady() && target.IsValidTarget()) Logic.ForceCastQ(target);
                    break;
            }

            if (t != 0 && (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None))
            {
                Orbwalker.ResetAutoAttack();
                Core.DelayAction(CancelAnimation, t - MenuConfig.Qld - (Game.Ping - MenuConfig.Qd));
            }
        }

        private static void CancelAnimation()
        {
            if (MenuConfig.QReset)
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
            }
            else if (MenuConfig.Qstrange && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                if (MenuConfig.AnimDance) EloBuddy.Player.DoEmote(Emote.Dance);
                if (MenuConfig.AnimLaugh) EloBuddy.Player.DoEmote(Emote.Laugh);
                if (MenuConfig.AnimTaunt) EloBuddy.Player.DoEmote(Emote.Taunt);
                if (MenuConfig.AnimTalk) EloBuddy.Player.DoEmote(Emote.Joke);
            }
            Orbwalker.ResetAutoAttack();
        }

        private static void OnCasting(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == Player.Type)
            {
                var epos = Player.ServerPosition +
                           (Player.ServerPosition - sender.ServerPosition).Normalized()*300;

                if (Player.Distance(sender.ServerPosition) <= args.SData.CastRange)
                {
                    switch (args.SData.TargettingType)
                    {
                        case SpellDataTargetType.Unit:

                            if (args.Target.NetworkId == Player.NetworkId)
                            {
                                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) ||
                                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                                    !args.SData.Name.Contains("NasusW"))
                                {
                                    if (Spells._e.IsReady()) Spells._e.Cast(epos);
                                }
                            }
                            break;
                        case SpellDataTargetType.SelfAoe:

                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) ||
                                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                            {
                                if (Spells._e.IsReady()) Spells._e.Cast(epos);
                            }
                            break;
                    }
                    if (args.SData.Name.Contains("IreliaEquilibriumStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._w.IsReady() && Logic.InWRange(sender)) Spells._w.Cast();
                            else if (Spells._e.IsReady()) Spells._e.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TalonCutthroat"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._w.IsReady()) Spells._w.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RenektonPreExecute"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._w.IsReady()) Spells._w.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("GarenRPreCast"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast(epos);
                        }
                    }

                    if (args.SData.Name.Contains("GarenQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady())
                                Spells._e.Cast(EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo,
                                    Player.Position.LSExtend(Game.CursorPos, Player.Distance(Game.CursorPos) + 10)));
                        }
                    }

                    if (args.SData.Name.Contains("XenZhaoThrust3"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._w.IsReady()) Spells._w.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarQ"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDashAADummy"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("TwitchEParticle"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("FizzPiercingStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady())
                                Spells._e.Cast(EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo,
                                    Player.Position.LSExtend(Game.CursorPos, Player.Distance(Game.CursorPos) + 10)));
                        }
                    }
                    if (args.SData.Name.Contains("HungeringStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaRTrigger"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._w.IsReady() && Logic.InWRange(sender)) Spells._w.Cast();
                            else if (Spells._e.IsReady())
                                Spells._e.Cast(EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo,
                                    Player.Position.LSExtend(Game.CursorPos, Player.Distance(Game.CursorPos) + 10)));
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaE"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._w.IsReady()) Spells._w.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady()) Spells._e.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingSpinToWin"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (Spells._e.IsReady())
                                Spells._e.Cast(EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo,
                                    Player.Position.LSExtend(Game.CursorPos, Player.Distance(Game.CursorPos) + 10)));
                            else if (Spells._w.IsReady()) Spells._w.Cast();
                        }
                    }
                }
            }
        }
    }
}