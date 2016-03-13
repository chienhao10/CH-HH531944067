using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace Riven
{
    public static class Program
    {

        private static Spell.Skillshot Q, R2, E;
        private static Spell.Active W, R1;
        private static Spell.Skillshot flashSlot;
        private static Spell.Targeted ignite;

        private static Menu Menu;

        private const string IsFirstR = "RivenFengShuiEngine";
        private const string IsSecondR = "RivenIzunaBlade";
        public static SpellSlot Ignite, Flash;
        private static int _qStack = 1;
        public static bool CastR2;

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        public static void Main()
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        private static void AutoUseW()
        {
            if (AutoW > 0)
            {
                if (myHero.CountEnemiesInRange(W.Range) >= AutoW)
                {
                    W.Cast();
                }
            }
        }

        private static void ComboLogic()
        {
            if (ComboW)
            {
                var t = EntityManager.Heroes.Enemies.Find(x => x.IsValidTarget(W.Range) && !x.HasBuffOfType(BuffType.SpellShield));

                if (t != null)
                {
                    if (W.IsReady() && !Orbwalker.CanAutoAttack)
                    {
                        W.Cast();
                    }
                }
            }

            if (E.IsReady())
            {
                var t = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range + myHero.GetAutoAttackRange()));

                if (t == null)
                {
                    return;
                }

                var t12 = t.OrderByDescending(e => TargetSelector.GetPriority(e)).FirstOrDefault();

                if (t12 != null)
                {
                    if (myHero.Distance(t12) > myHero.GetAutoAttackRange() + 20)
                    {
                        E.Cast(t12.ServerPosition);
                    }
                }
                if (ComboE == 0)
                {
                    var t1 = t.OrderByDescending(e => TargetSelector.GetPriority(e)).FirstOrDefault();

                    if (t1 != null)
                        E.Cast(t1.ServerPosition);
                }
                else if (ComboE == 1)
                {
                    if (t != null)
                        E.Cast(Game.CursorPos);
                }
            }

            if (ComboR)
            {
                if (R1.IsReady())
                {
                    if (useR1 && !myHero.HasBuff("RivenFengShuiEngine"))
                    {
                        var t = TargetSelector.GetTarget(900, DamageType.Physical);
                        if (t == null)
                        {
                            return;
                        }
                        if (t.Distance(myHero.ServerPosition) < E.Range + myHero.AttackRange && myHero.CountEnemiesInRange(500) >= 1)
                            R1.Cast();
                    }

                    if (myHero.HasBuff("RivenFengShuiEngine"))
                    {
                        var t = TargetSelector.GetTarget(900, DamageType.Physical);
                        if (t == null)
                        {
                            return;
                        }
                        if (t.ServerPosition.Distance(myHero.ServerPosition) < 850)
                        {
                            switch (R2Mode)
                            {
                                case 0:
                                    if (Rdame(t, t.Health) > t.Health && t.IsValidTarget(R2.Range) && t.Distance(myHero.ServerPosition) < 600)
                                    {
                                        CastR2 = true;
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                    break;
                                case 1:
                                    if (t.HealthPercent < 25 && t.Health > Rdame(t, t.Health) + Damage.GetAutoAttackDamage(myHero, t) * 2)
                                    {
                                        CastR2 = true;
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                    break;
                                case 2:
                                    if (t.IsValidTarget(R2.Range) && t.Distance(myHero.ServerPosition) < 600)
                                    {
                                        CastR2 = true;
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                    break;
                                case 3:
                                    CastR2 = false;
                                    break;
                            }
                        }

                        if (CastR2)
                        {
                            R2.Cast(t);
                        }
                    }
                }
            }
        }

        #region Menu Items
        //public static bool useQ { get { return Menu["useQ"].Cast<CheckBox>().CurrentValue; } }
        public static int AutoW { get { return Menu["AutoW"].Cast<Slider>().CurrentValue; } }
        public static bool ComboW { get { return Menu["ComboW"].Cast<CheckBox>().CurrentValue; } }
        public static bool AutoShield { get { return Menu["AutoShield"].Cast<CheckBox>().CurrentValue; } }
        public static bool Winterrupt { get { return Menu["Winterrupt"].Cast<CheckBox>().CurrentValue; } }
        public static int R2Mode { get { return Menu["R2Mode"].Cast<Slider>().CurrentValue; } }
        public static bool useR1 { get { return Menu["useR1"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboR { get { return Menu["ComboR"].Cast<CheckBox>().CurrentValue; } }
        public static int ComboE { get { return Menu["ComboE"].Cast<Slider>().CurrentValue; } }
        public static bool harassQ { get { return Menu["harassQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool clearQ { get { return Menu["clearQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool jungleQ { get { return Menu["jungleQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool harassW { get { return Menu["harassW"].Cast<CheckBox>().CurrentValue; } }
        public static bool doBurst { get { return Menu["doBurst"].Cast<KeyBind>().CurrentValue; } }
        public static bool BurstItem { get { return Menu["BurstItem"].Cast<CheckBox>().CurrentValue; } }
        public static bool BurstIgnite { get { return Menu["BurstIgnite"].Cast<CheckBox>().CurrentValue; } }
        public static bool BurstFlash { get { return Menu["BurstFlash"].Cast<CheckBox>().CurrentValue; } }
        public static bool KeepQALive { get { return Menu["KeepQALive"].Cast<CheckBox>().CurrentValue; } }
        public static bool jungleW { get { return Menu["jungleW"].Cast<CheckBox>().CurrentValue; } }
        public static bool jungleE { get { return Menu["jungleE"].Cast<CheckBox>().CurrentValue; } }
        public static bool clearW { get { return Menu["clearW"].Cast<CheckBox>().CurrentValue; } }
        public static int getS { get { return Menu["Spell"].Cast<Slider>().CurrentValue; } }
        public static bool KillStealQ { get { return Menu["KillStealQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool KillStealW { get { return Menu["KillStealW"].Cast<CheckBox>().CurrentValue; } }
        public static bool KillStealE { get { return Menu["KillStealE"].Cast<CheckBox>().CurrentValue; } }
        public static bool KillStealR { get { return Menu["KillStealR"].Cast<CheckBox>().CurrentValue; } }

        #endregion

        private static void OnLoad(EventArgs args)
        {
            if (myHero.Hero != Champion.Riven)
            {
                return;
            }

            Menu = MainMenu.AddMenu("Riven", "Riven");
            Menu.AddLabel("Ported from Nechrito's Riven & Flower's Riven - Berb");
            Menu.AddSeparator();
            Menu.AddGroupLabel("Combo");
            Menu.Add("ComboR", new CheckBox("Use R"));
            Menu.Add("useR1", new CheckBox("Use R1"));
            Menu.Add("ComboW", new CheckBox("Always use W"));
            Menu.AddLabel("R2 Modes : ");
            Menu.Add("R2Mode", new Slider("0 : Killable | 1 : Max Damage | 2 : First Cast | 3 : Off", 0, 0, 3));
            Menu.AddSeparator();
            Menu.AddLabel("E Modes : ");
            Menu.Add("ComboE", new Slider("0 : To Target | 1 : To Mouse", 0, 0, 1));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Burst Combo");
            Menu.Add("doBurst", new KeyBind("Do Burst Combo (Need to redo)", false, KeyBind.BindTypes.HoldActive, 'T'));
            Menu.Add("BurstItem", new CheckBox("Use Items"));
            Menu.Add("BurstIgnite", new CheckBox("Use Ignite"));
            Menu.Add("BurstFlash", new CheckBox("Use Flash"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Harass");
            Menu.Add("harassQ", new CheckBox("Use Q"));
            Menu.Add("harassW", new CheckBox("Use W"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Lane Clear");
            Menu.Add("clearQ", new CheckBox("Use Q"));
            Menu.Add("clearW", new CheckBox("Use W"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Jungle Clear");
            Menu.Add("jungleQ", new CheckBox("Use Q"));
            Menu.Add("jungleW", new CheckBox("Use W"));
            Menu.Add("jungleE", new CheckBox("Use E"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Misc");
            Menu.Add("AutoW", new Slider("Auto W When X Enemy", 5, 0, 5));
            Menu.Add("AutoShield", new CheckBox("Auto Cast E"));
            Menu.Add("Winterrupt", new CheckBox("W interrupt"));
            Menu.Add("KeepQALive", new CheckBox("Keep Q Alive"));
            Menu.AddSeparator();
            Menu.Add("KillStealQ", new CheckBox("Use Q KS"));
            Menu.Add("KillStealW", new CheckBox("Use W KS"));
            Menu.Add("KillStealE", new CheckBox("Use E KS"));
            Menu.Add("KillStealR", new CheckBox("Use R KS"));
            Menu.AddSeparator();
            Menu.Add("Spell", new Slider("Spell based delay :", 0, 0, 100));
            Menu.AddSeparator();

            Q = new Spell.Skillshot(SpellSlot.Q, 260, SkillShotType.Circular, 250, 2200, 100);
            W = new Spell.Active(SpellSlot.W, 255);
            E = new Spell.Skillshot(SpellSlot.E, 270, SkillShotType.Linear);
            R1 = new Spell.Active(SpellSlot.R, (uint)myHero.GetAutoAttackRange());
            R2 = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 45);

            var flash = Player.Spells.FirstOrDefault(o => o.SData.Name == "SummonerFlash");
            var ign = Player.Spells.FirstOrDefault(o => o.SData.Name == "SummonerDot");

            if (flash != null)
            {
                SpellSlot flSlot = EloBuddy.SDK.Extensions.GetSpellSlotFromName(myHero, "SummonerFlash");

                flashSlot = new Spell.Skillshot(flSlot, 425, SkillShotType.Linear);
            }

            if (ign != null)
            {
                SpellSlot igslot = EloBuddy.SDK.Extensions.GetSpellSlotFromName(myHero, "SummonerDot");

                ignite = new Spell.Targeted(igslot, 600);
            }

            Game.OnTick += OnTick;
            Obj_AI_Base.OnSpellCast += AfterAAQLogic;
            Obj_AI_Base.OnPlayAnimation += OnPlay;
            Obj_AI_Base.OnProcessSpellCast += OnCasting;
            Interrupter.OnInterruptableSpell += Interrupt;
            Orbwalker.OnPostAttack += JungleClearELogic;
        }
        private static void Interrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsEnemy && W.IsReady() && sender.IsValidTarget() && !sender.IsZombie && Winterrupt)
            {
                if (sender.IsValidTarget(125 + myHero.BoundingRadius + sender.BoundingRadius)) W.Cast();
            }
        }

        private static void OnCasting(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == myHero.Type && AutoShield)
            {
                var epos = myHero.ServerPosition + (myHero.ServerPosition - sender.ServerPosition).Normalized() * 300;

                if (myHero.Distance(sender.ServerPosition) <= args.SData.CastRange)
                {
                    switch (args.SData.TargettingType)
                    {
                        case SpellDataTargetType.Unit:

                            if (args.Target.NetworkId == myHero.NetworkId)
                            {
                                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !args.SData.Name.Contains("NasusW"))
                                {
                                    if (E.IsReady()) E.Cast(epos);
                                }
                            }

                            break;
                        case SpellDataTargetType.SelfAoe:

                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                            {
                                if (E.IsReady()) E.Cast(epos);
                            }

                            break;
                    }
                    if (args.SData.Name.Contains("IreliaEquilibriumStrike"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady() && W.IsInRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TalonCutthroat"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RenektonPreExecute"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("GarenRPreCast"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }

                    if (args.SData.Name.Contains("GarenQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }

                    if (args.SData.Name.Contains("XenZhaoThrust3"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarQ"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDash"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDashAADummy"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TwitchEParticle"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("FizzPiercingStrike"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("HungeringStrike"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaRTrigger"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady() && W.IsInRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaE"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingSpinToWin"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                            else if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                }
            }
        }

        private static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                return;
            switch (args.Animation)
            {
                case "Spell1a":
                    _qStack = 1;
                    Core.DelayAction(() => CancelAnimation(), getS + 120 - Game.Ping);
                    break;
                case "Spell1b":
                    _qStack = 2;
                    Core.DelayAction(() => CancelAnimation(), getS + 120 - Game.Ping);
                    break;
                case "Spell1c":
                    _qStack = 0;
                    Core.DelayAction(() => CancelAnimation(), getS + 270 - Game.Ping);
                    break;
                case "Spell2":
                    EloBuddy.Player.DoEmote(Emote.Dance);
                    Orbwalker.ResetAutoAttack();
                    break;
                case "Spell3":
                    Orbwalker.ResetAutoAttack();
                    break;
            }
        }

        private static void CancelAnimation()
        {
            var target = TargetSelector.GetTarget(myHero.GetAutoAttackRange() + 50, DamageType.Physical);
            EloBuddy.Player.DoEmote(Emote.Dance);
            if (target != null) {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
            }
            Orbwalker.ResetAutoAttack();
        }

        private static double Rdame(Obj_AI_Base target, double health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75 ? 0.75 : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * (8 / 3);
                var rawdmg = new double[] { 80, 120, 160 }[R1.Level - 1] + 0.6 * myHero.FlatPhysicalDamageMod;
                return myHero.CalculateDamageOnUnit(target, DamageType.Physical, (float)(rawdmg * (1 + pluspercent)));
            }
            return 0;
        }

        private static void AfterAAQLogic(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            var t = args.Target;

            if (t == null)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Q.IsReady())
                {
                    if (t is AIHeroClient)
                        Q.Cast(t.Position);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (Q.IsReady())
                {
                    if (harassQ)
                        if (t is AIHeroClient)
                            Q.Cast(t.Position);
                }
            }

            /*
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
            {
                if (Q.IsReady() && QStack != 2)
                {
                    if (HarassQ)
                    {
                        if (t is Obj_AI_Hero)
                            Q.Cast(t.Position);
                    }
                }
            }
            */

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (t is Obj_AI_Minion)
                {
                    if (Q.IsReady())
                    {
                        if (clearQ)
                        {
                            foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, E.Range))
                            {
                                Q.Cast(minion);
                            }
                        }
                        if (jungleQ)
                        {
                            foreach (var camp in EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, E.Range))
                            {
                                Q.Cast(camp);
                            }
                        }
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            if (harassW)
            {
                var t = EntityManager.Heroes.Enemies.Find(x => x.IsValidTarget(W.Range) && !x.HasBuffOfType(BuffType.SpellShield));

                if (t != null)
                    if (W.IsReady())
                        W.Cast();
            }
        }

        private static void BurstLogic()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var e = TargetSelector.GetTarget(E.Range + flashSlot.Range, DamageType.Physical);

            if (e != null && !e.IsDead && e.IsValidTarget() && !e.IsZombie)
            {
                if (R1.IsReady())
                {
                    if (myHero.HasBuff("RivenFengShuiEngine"))
                    {
                        if (Q.IsReady())
                        {
                            if (E.IsReady() && W.IsReady())
                            {
                                if (e.Distance(myHero.ServerPosition) < E.Range + myHero.AttackRange + 100)
                                {
                                    E.Cast(e.Position);
                                }
                            }
                        }
                    }

                    if (E.IsReady())
                    {
                        if (e.Distance(myHero.ServerPosition) < myHero.AttackRange + E.Range + 100)
                        {
                            R1.Cast();
                            E.Cast(e.Position);
                        }
                    }
                }

                if (W.IsReady())
                {
                    if (EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(W.Range)))
                        W.Cast();
                }

                if (BurstItem)
                {
                    if (e.Distance(myHero.ServerPosition) <= E.Range + 500)
                    {
                        if (Item.CanUseItem(ItemId.Youmuus_Ghostblade) && Item.HasItem(ItemId.Youmuus_Ghostblade))
                            Item.UseItem(ItemId.Youmuus_Ghostblade);
                    }

                    if (myHero.IsInAutoAttackRange(e))
                    {
                        if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) && Item.HasItem(ItemId.Tiamat_Melee_Only))
                            Item.UseItem(ItemId.Tiamat_Melee_Only);

                        if (Item.CanUseItem(ItemId.Titanic_Hydra) && Item.HasItem(ItemId.Titanic_Hydra))
                            Item.UseItem(ItemId.Titanic_Hydra);

                        if (Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) && Item.HasItem(ItemId.Ravenous_Hydra_Melee_Only))
                            Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
                    }
                }

                if (BurstIgnite)
                {
                    if (e.HealthPercent < 50)
                    {
                        if (ignite.IsReady())
                        {
                            ignite.Cast(e);
                        }
                    }
                }

                if (BurstFlash)
                {
                    if (flashSlot.IsReady() && R1.IsReady() && R1.Name == "RivenFengShuiEngine" && E.IsReady() && W.IsReady())
                    {
                        if (e.Distance(myHero.ServerPosition) <= 780 && e.Distance(myHero.ServerPosition) >= E.Range + myHero.AttackRange + 85)
                        {
                            Core.DelayAction(delegate { flashSlot.Cast(e.ServerPosition); }, 500);
                            E.Cast(e.ServerPosition);
                            R1.Cast();
                        }
                    }
                }
            }
        }

        private static void KeelQLogic()
        {
            if (KeepQALive && !myHero.IsUnderTurret() && !myHero.IsRecalling() && myHero.HasBuff("RivenTriCleave"))
            {
                if (myHero.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                    Q.Cast(Game.CursorPos);
            }
        }

        private static void LaneClearLogic()
        {
            if (clearW)
            {
                if (W.IsReady())
                {
                    var WMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, W.Range).ToList();

                    if (WMinions != null)
                        if (WMinions.FirstOrDefault().IsValidTarget(W.Range))
                            if (WMinions.Count >= 3)
                                W.Cast();
                }
            }
        }

        private static void JungleClearLogic()
        {
            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, E.Range).OrderBy(x => x.MaxHealth).ToList();

            if (jungleW)
            {
                if (Mob != null)
                    if (Mob.FirstOrDefault().IsValidTarget(W.Range))
                        W.Cast();
            }
        }

        private static void JungleClearELogic(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (target is Obj_AI_Minion)
                {
                    if (jungleE)
                    {
                        var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, E.Range).OrderBy(x => x.MaxHealth).ToList();

                        if (Mob.FirstOrDefault().IsValidTarget(E.Range))
                        {
                            if (Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun) && !W.IsReady())
                            {
                                E.Cast(Game.CursorPos);
                            }
                            else if (!Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun))
                            {
                                E.Cast(Game.CursorPos);
                            }
                        }
                    }
                }
            }
        }

        private static void QuickHarassLogic()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (t != null && t.IsValidTarget())
            {
                if (_qStack == 2)
                {
                    if (E.IsReady())
                    {
                        E.Cast(myHero.ServerPosition + (myHero.ServerPosition - t.ServerPosition).Normalized() * E.Range);
                    }

                    if (!E.IsReady())
                    {
                        Q.Cast(myHero.ServerPosition + (myHero.ServerPosition - t.ServerPosition).Normalized() * E.Range);
                    }
                }

                if (W.IsReady())
                {
                    if (t.IsValidTarget(W.Range) && _qStack == 1)
                    {
                        W.Cast();
                    }
                }

                if (Q.IsReady())
                {
                    if (_qStack == 0)
                    {
                        if (t.IsValidTarget(myHero.AttackRange + myHero.BoundingRadius + 150))
                        {
                            Q.Cast(t.Position);
                        }
                    }
                }
            }
        }


        private static void KillStealLogic()
        {
            foreach (var e in EntityManager.Heroes.Enemies.Where(e => !e.IsZombie && !e.HasBuff("KindredrNoDeathBuff") && !e.HasBuff("Undying Rage") && !e.HasBuff("JudicatorIntervention") && e.IsValidTarget()))
            {
                if (Q.IsReady() && KillStealQ)
                {
                    if (myHero.HasBuff("RivenFengShuiEngine"))
                    {
                        if (e.Distance(myHero.ServerPosition) < myHero.AttackRange + myHero.BoundingRadius + 50 && myHero.GetSpellDamage(e, SpellSlot.Q) > e.Health + e.HPRegenRate)
                            Q.Cast(e.Position);
                    }
                    else if (!myHero.HasBuff("RivenFengShuiEngine"))
                    {
                        if (e.Distance(myHero.ServerPosition) < myHero.AttackRange + myHero.BoundingRadius && myHero.GetSpellDamage(e, SpellSlot.Q) > e.Health + e.HPRegenRate)
                            Q.Cast(e.Position);
                    }
                }

                if (W.IsReady() && KillStealW)
                {
                    if (e.IsValidTarget(W.Range) && myHero.GetSpellDamage(e, SpellSlot.W) > e.Health + e.HPRegenRate)
                    {
                        W.Cast();
                    }
                }

                if (R1.IsReady() && KillStealR)
                {
                    if (myHero.HasBuff("RivenWindScarReady"))
                    {
                        if (E.IsReady() && KillStealE)
                        {
                            if (myHero.ServerPosition.CountEnemiesInRange(R2.Range + E.Range) < 3 && myHero.HealthPercent > 50)
                            {
                                if (Rdame(e, e.Health) > e.Health + e.HPRegenRate && e.IsValidTarget(R2.Range + E.Range - 100))
                                {
                                    R1.Cast();
                                    E.Cast(e.Position);
                                    Core.DelayAction(() => { R2.Cast(e); }, 350);
                                }
                            }
                        }
                        else
                        {
                            if (Rdame(e, e.Health) > e.Health + e.HPRegenRate && e.IsValidTarget(R2.Range - 50))
                            {
                                R1.Cast();
                                R2.Cast(e);
                            }
                        }
                    }
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            KillStealLogic();
            AutoUseW();
            if (doBurst) BurstLogic();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) ComboLogic();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) HarassLogic();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClearLogic();
                JungleClearLogic();
            }
            KeelQLogic();
        }
    }
}
