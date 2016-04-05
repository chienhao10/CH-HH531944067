using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using Ex1L_Riven.Base;
using SharpDX;

namespace Ex1L_Riven
{
    internal class Riven
    {
        public static int Qnumerator = 0;
        public static int Qlastcast = 0;
        public static bool PerformQaa = false;
        public static bool R1Activated;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Riven) return;

            Spells.Initialize();
            ModeController.Initialize();
            MenuDesigner.Initialize();
            Variables.Initialize();
            Logic.IniInitialize();
            AutoLevel.Initialize();

            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Orbwalker.OnPostAttack += OnPostAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Gapcloser.OnGapcloser += OnGapcloser;
            Drawing.OnDraw += OnDraw;

            Chat.Print("Ex1L Riven made by Enelx !");
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && ModeController.Activemode(Orbwalker.ActiveModes.Combo) && ModeController.Mode == 1)
            {
                var target = TargetSelector.GetTarget(1000, DamageType.Physical);

                // E Cancels
                if (args.SData.Name == Spells.E.Name && Spells.R1.IsReady() && R1Activated == false)
                {
                    Logic.CastR(target);
                }

                if (args.SData.Name == Spells.E.Name && Items.Tiamat.IsReady() || Items.Hydra.IsReady())
                {
                    Logic.CastTiaHydra(target);
                }

                if (args.SData.Name == Spells.E.Name && Spells.W.IsReady())
                {
                    Logic.CastW(target);
                }

                if (args.SData.Name == Spells.E.Name && R1Activated && target.Health <= Spells.R2Damage(target, target.Health))
                {
                    var prediction = Spells.R2.GetPrediction(target);
                    Spells.R2.Cast(prediction.CastPosition);
                }

                // R1 Cancels

                if (args.SData.Name == Spells.R1.Name && Spells.W.IsReady())
                {
                    Logic.CastW(target);
                }

                if (args.SData.Name == Spells.R1.Name && Spells.Q.IsReady())
                {
                    Logic.CastQ(target);
                }

                // W Cancels

                if (args.SData.Name == Spells.W.Name && Spells.Q.IsReady())
                {
                    Logic.CastQ(target);
                }

                // R2 Cancels

                if (args.SData.Name == Spells.R2.Name && Spells.W.IsReady())
                {
                    Logic.CastW(target);
                }

                if (args.SData.Name == Spells.R2.Name && Spells.Q.IsReady())
                {
                    Logic.CastQ(target);
                }
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation == "Spell1a" || args.Animation == "Spell1b" || args.Animation == "Spell1c")
                {
                    if (args.Animation == "Spell1a" || args.Animation == "Spell1b")
                    {
                        if (Logic.Qtarget.IsValidTarget() && Player.Instance.Distance(Logic.Qtarget) < 270)
                        {
                            if (Variables.UseEmoteCancel)
                            {
                                Core.DelayAction(delegate
                                {
                                    Logic.DoEmote();
                                    Logic.ResetAa();
                                }, Variables.Q1Q2Delay - Game.Ping);
                            }
                        }
                    }
                    else if (args.Animation == "Spell1c")
                    {
                        if (Logic.Qtarget.IsValidTarget() && Player.Instance.Distance(Logic.Qtarget) < 270)
                        {
                            if (Variables.UseEmoteCancel)
                            {
                                Core.DelayAction(delegate
                                {
                                    Logic.DoEmote();
                                    Logic.ResetAa();
                                }, Variables.Q3Delay - Game.Ping);
                            }
                        }
                    }
                }
            }
        }

        private static void OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (PerformQaa &&
                ModeController.Activemode(Orbwalker.ActiveModes.Flee) ||
                ModeController.Activemode(Orbwalker.ActiveModes.Harass) ||
                ModeController.Activemode(Orbwalker.ActiveModes.Combo) ||
                ModeController.Activemode(Orbwalker.ActiveModes.LaneClear))
            {
                Spells.Q.Cast(target.Position);
                Logic.ResetAa();
            }
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.Buff.Name == "RivenFengShuiEngine")
            {
                R1Activated = true;
            }

            if (Variables.UseItems && Items.Qss.IsOwned() || Items.Mercurial.IsOwned())
            {
                var type = args.Buff.Type;
                var name = args.Buff.Name;

                switch (type)
                {
                    case BuffType.Blind:
                        Logic.CastQss();
                        break;
                    case BuffType.Charm:
                        Logic.CastQss();
                        break;
                    case BuffType.Fear:
                        Logic.CastQss();
                        break;
                    case BuffType.Polymorph:
                        Logic.CastQss();
                        break;
                    case BuffType.Silence:
                        Logic.CastQss();
                        break;
                    case BuffType.Snare:
                        Logic.CastQss();
                        break;
                    case BuffType.Stun:
                        Logic.CastQss();
                        break;
                    case BuffType.Suppression:
                        Logic.CastQss();
                        break;
                    case BuffType.Taunt:
                        Logic.CastQss();
                        break;
                }

                switch (name)
                {
                    case "zedrdeathmark":
                        Logic.CastQss();
                        break;
                    case "vladimirhemoplague":
                        Logic.CastQss();
                        break;
                    case "fizzmarinerdoom":
                        Logic.CastQss();
                        break;
                    case "mordekaiserchildrenofthegrave":
                        Logic.CastQss();
                        break;
                }
            }
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == "rivenwindslashready")
            {
                R1Activated = false;
            }
        }

        private static void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsEnemy && Variables.UseInterruptW && sender.IsValidTarget(Spells.W.Range) && Spells.W.IsReady())
            {
                Spells.W.Cast();
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsEnemy && Variables.UseGapcloserWe && sender.IsValidTarget(Spells.W.Range))
            {
                if (Spells.E.IsReady())
                {
                    Player.CastSpell(SpellSlot.E, Game.CursorPos);
                }
                Spells.W.Cast();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead || Shop.IsOpen || MainMenu.IsOpen) return;

            if (Variables.DrawR2Range)
            {
                switch (ModeController.Mode)
                {
                    case 1:
                        Circle.Draw(Color.SpringGreen, Spells.R2.Range, Player.Instance.Position);
                        break;
                    case 2:
                        Circle.Draw(Color.Crimson, Spells.R2.Range, Player.Instance.Position);
                        break;
                }
                    
            }

            if (Variables.DrawR1Status)
            {
                var textPos = Drawing.WorldToScreen(Player.Instance.Position);
                Drawing.DrawText(textPos.X - 35, textPos.Y + 20, System.Drawing.Color.White, Variables.UseRCombo ? "R1 Status [on]" : "R1 Status [off]");
            }
        }
    }
}