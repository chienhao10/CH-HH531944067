using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace LelBlanc
{
    internal class Program
    {
        /// <summary>
        /// Position for Last W
        /// </summary>
        public static Vector3 LastWPosition { get; set; }

        /// <summary>
        /// Position for Last W Ultimate
        /// </summary>
        public static Vector3 LastWUltimatePosition { get; set; }

        /// <summary>
        /// Position for Last W End Position
        /// </summary>
        public static Vector3 LastWEndPosition { get; set; }

        /// <summary>
        /// Position for Last W End Position
        /// </summary>
        public static Vector3 LastWUltimateEndPosition { get; set; }

        /// <summary>
        /// Contains All Active Spells
        /// </summary>
        public static Spell.Active WReturn, RReturn;

        /// <summary>
        /// Contains All Targeted Spells
        /// </summary>
        public static Spell.Targeted Q, QUltimate, Ignite;

        /// <summary>
        /// Contains All Skillshots
        /// </summary>
        public static Spell.Skillshot W, E, WUltimate, EUltimate;

        /// <summary>
        /// Contains the DamageIndicator
        /// </summary>
        public static DamageIndicator Indicator;

        /// <summary>
        /// Contains the Color Picker
        /// </summary>
        public static ColorPicker Picker;

        /// <summary>
        /// Called when the Program is Initialized
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the Game is finished loading
        /// </summary>
        /// <param name="args"></param>
        private static void Loading_OnLoadingComplete(System.EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Leblanc)
            {
                return;
            }

            Q = new Spell.Targeted(SpellSlot.Q, 720)
            {
                CastDelay = 500
            };

            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, 500, 1450, 220);

            WReturn = new Spell.Active(SpellSlot.W);

            RReturn = new Spell.Active(SpellSlot.R);

            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Linear, 300, 1650, 55)
            {
                AllowedCollisionCount = 0,
                MinimumHitChance = HitChance.High
            };

            QUltimate = new Spell.Targeted(SpellSlot.R, 720)
            {
                CastDelay = 500
            };

            WUltimate = new Spell.Skillshot(SpellSlot.R, 600, SkillShotType.Circular, 500, 1450, 220);

            EUltimate = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Linear, 300, 1650, 55)
            {
                AllowedCollisionCount = 0,
                MinimumHitChance = HitChance.High
            };

            if (Extension.HasSpell("summonerdot"))
            {
                Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
                Chat.Print("LelBlanc: Ignite Loaded", System.Drawing.Color.Red);
            }

            Chat.Print("LelBlanc: Addon Loaded", System.Drawing.Color.Blue);

            // Methods
            Config.Initialize();

            // Constructors
            Picker = new ColorPicker(Config.DrawingMenu, "draw_", System.Drawing.Color.FromArgb(255, 255, 0, 0),
                "Color Settings for Damage Indicator");
            Indicator = new DamageIndicator();

            // Events
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            Drawing.OnDraw += OnDraw.DrawRange;
        }

        /// <summary>
        /// Called when a Object gets created
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Args</param>
        private static void GameObject_OnCreate(GameObject sender, System.EventArgs args)
        {
            if (sender.Name == Player.Instance.Name)
            {
                Pet.LeBlancPet = sender;
            }
        }

        /// <summary>
        /// Called when a Object gets deleted
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Args</param>
        private static void GameObject_OnDelete(GameObject sender, System.EventArgs args)
        {
            if (sender.Name == Player.Instance.Name)
            {
                Pet.LeBlancPet = null;
            }
        }

        /// <summary>
        /// Gets Player New Path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe)
            {
                var path = Player.Instance.Position.Extend(args.Path[0], 1000);
                var extendedPath = new Vector3(path, NavMesh.GetHeightForPosition(path.X, path.Y));
                Pet.NewPath = extendedPath;
            }
        }

        /// <summary>
        /// Called when Processing Spell Cast
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.ToLower() == "leblancslide")
            {
                LastWPosition = args.Start;
                LastWEndPosition = args.End;
            }

            if (args.SData.Name.ToLower() == "leblancslidem")
            {
                LastWUltimatePosition = args.Start;
                LastWUltimateEndPosition = args.End;
            }
        }

        /// <summary>
        /// Called when On Spell Cast
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Arguments</param>
        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.ToLower() == "leblancslide")
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                    if (target != null && E.IsReady())
                    {
                        E.Cast(target);
                    }
                }
            }

            if (args.SData.Name.ToLower() == "leblancslidem")
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                    if (target != null && E.IsReady())
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        /// <summary>
        /// Called whenever the game is being runned.
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnUpdate(System.EventArgs args)
        {
            if (Config.MiscMenu["pet"].Cast<CheckBox>().CurrentValue)
            {
                Pet.MovePet();
            }
            if (Config.KillStealMenu["toggle"].Cast<CheckBox>().CurrentValue)
            {
                if (!LastWEndPosition.IsZero &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                {
                    LastWEndPosition = Vector3.Zero;
                }
                if (!LastWUltimateEndPosition.IsZero &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                {
                    LastWUltimatePosition = Vector3.Zero;
                }
                if (Modes.KillSteal.ResetW && Player.Instance.ServerPosition.IsInRange(LastWEndPosition, 100))
                {
                    Modes.KillSteal.ResetW = Extension.LogicReturn();

                    if (Modes.KillSteal.ResetW == false)
                        LastWEndPosition = Vector3.Zero;

                    #region Old Code

                    /*if (!E.IsReady())
                    {
                        var eEnemies =
                            EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(E.Range) && Extension.IsBeingE(t))
                                .ToArray();

                        if (eEnemies.Any())
                        {
                            var shouldUse = eEnemies.Any(t => LastWPosition.Distance(t) <= E.Range);

                            if (!shouldUse) return;

                            if (WReturn.IsReady() &&
                                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                            {
                                WReturn.Cast();
                                LastWEndPosition = Vector3.Zero;
                                Modes.KillSteal.ResetW = false;
                                return;
                            }
                        }
                        else
                        {
                            if (WReturn.IsReady() &&
                                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                            {
                                WReturn.Cast();
                                LastWEndPosition = Vector3.Zero;
                                Modes.KillSteal.ResetW = false;
                                return;
                            }
                        }
                    }

                    if (WReturn.IsReady() &&
                        Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                    {
                        WReturn.Cast();
                        LastWEndPosition = Vector3.Zero;
                        Modes.KillSteal.ResetW = false;
                        return;
                    }*/

                    #endregion
                }
                if (Modes.KillSteal.ResetW && Player.Instance.ServerPosition.IsInRange(LastWUltimateEndPosition, 100))
                {
                    Modes.KillSteal.ResetW = Extension.LogicReturn(true);

                    if (Modes.KillSteal.ResetW == false)
                        LastWEndPosition = Vector3.Zero;

                    #region Old Code

                    /*if (!E.IsReady())
                    {
                        var eEnemies =
                            EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(E.Range) && Extension.IsBeingE(t))
                                .ToArray();

                        if (eEnemies.Any())
                        {
                            var shouldUse = eEnemies.Any(t => LastWUltimatePosition.Distance(t) <= E.Range);

                            if (!shouldUse) return;

                            if (RReturn.IsReady() &&
                                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
                            {
                                RReturn.Cast();
                                LastWEndPosition = Vector3.Zero;
                                Modes.KillSteal.ResetW = false;
                                return;
                            }
                        }
                        else
                        {
                            if (RReturn.IsReady() &&
                                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
                            {
                                RReturn.Cast();
                                LastWUltimateEndPosition = Vector3.Zero;
                                Modes.KillSteal.ResetW = false;
                                return;
                            }
                        }
                    }


                    if (RReturn.IsReady() &&
                        Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
                    {
                        RReturn.Cast();
                        LastWUltimateEndPosition = Vector3.Zero;
                        Modes.KillSteal.ResetW = false;
                        return;
                    }*/

                    #endregion
                }

                Modes.KillSteal.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.Harass.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Modes.LaneClear.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Modes.JungleClear.Execute();
            }
        }
    }
}