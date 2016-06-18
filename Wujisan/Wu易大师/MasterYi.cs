using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using WuAIO.Bases;
using WuAIO.Managers;
using ScalingTypes = WuAIO.Managers.DamageManager.ScalingTypes;
using Color = SharpDX.Color;

namespace WuAIO
{
    class MasterYi : HeroBase
    {
        readonly List<string> DodgeSpells = new List<string>() { "SorakaQ", "SorakaE", "TahmKenchW", "TahmKenchQ", "Bushwhack", "ForcePulse", "KarthusFallenOne", "KarthusWallOfPain", "KarthusLayWasteA1", "KarmaWMantra", "KarmaQMissileMantra", "KarmaSpiritBind", "KarmaQ", "JinxW", "JinxE", "JarvanIVGoldenAegis", "HowlingGaleSpell", "SowTheWind", "ReapTheWhirlwind", "IllaoiE", "HeimerdingerUltWDummySpell", "HeimerdingerUltEDummySpell", "HeimerdingerW", "HeimerdingerE", "HecarimUlt", "HecarimRampAttack", "GravesQLineSpell", "GravesQLineMis", "GravesClusterShot", "GravesSmokeGrenade", "GangplankR", "GalioIdolOfDurand", "GalioResoluteSmite", "FioraE", "EvelynnR", "EliseHumanE", "EkkoR", "EkkoW", "EkkoQ", "DravenDoubleShot", "InfectedCleaverMissileCast", "DariusExecute", "DariusAxeGrabCone", "DariusNoxianTacticsONH", "DariusCleave", "PhosphorusBomb", "MissileBarrage", "BraumQ", "BrandFissure", "BardR", "BardQ", "AatroxQ", "AatroxE", "AzirE", "AzirEWrapper", "AzirQWrapper", "AzirQ", "AzirR", "Pulverize", "AhriSeduce", "CurseoftheSadMummy", "InfernalGuardian", "Incinerate", "Volley", "EnchantedCrystalArrow", "BraumRWrapper", "CassiopeiaPetrifyingGaze", "FeralScream", "Rupture", "EzrealEssenceFlux", "EzrealMysticShot", "EzrealTrueshotBarrage", "FizzMarinerDoom", "GnarW", "GnarBigQMissile", "GnarQ", "GnarR", "GragasQ", "GragasE", "GragasR", "RiftWalk", "LeblancSlideM", "LeblancSlide", "LeonaSolarFlare", "UFSlash", "LuxMaliceCannon", "LuxLightStrikeKugel", "LuxLightBinding", "yasuoq3w", "VelkozE", "VeigarEventHorizon", "VeigarDarkMatter", "VarusR", "ThreshQ", "ThreshE", "ThreshRPenta", "SonaQ", "SonaR", "ShenShadowDash", "SejuaniGlacialPrisonCast", "RivenMartyr", "JavelinToss", "NautilusSplashZone", "NautilusAnchorDrag", "NamiR", "NamiQ", "DarkBindingMissile", "StaticField", "RocketGrab", "RocketGrabMissile", "timebombenemybuff", "NocturneUnspeakableHorror", "SyndraQ", "SyndraE", "SyndraR", "VayneCondemn", "Dazzle", "Overload", "AbsoluteZero", "IceBlast", "LeblancChaosOrb", "JudicatorReckoning", "KatarinaQ", "NullLance", "Crowstorm", "FiddlesticksDarkWind", "BrandWildfire", "Disintegrate", "FlashFrost", "Frostbite", "AkaliMota", "InfiniteDuress", "PantheonW", "blindingdart", "JayceToTheSkies", "IreliaEquilibriumStrike", "maokaiunstablegrowth", "nautilusgandline", "runeprison", "WildCards", "BlueCardAttack", "RedCardAttack", "GoldCardAttack", "AkaliShadowDance", "Headbutt", "PowerFist", "BrandConflagration", "CaitlynYordleTrap", "CaitlynAceintheHole", "CassiopeiaNoxiousBlast", "CassiopeiaMiasma", "CassiopeiaTwinFang", "Feast", "DianaArc", "DianaTeleport", "EliseHumanQ", "EvelynnE", "Terrify", "FizzPiercingStrike", "Parley", "GarenQAttack", "GarenR", "IreliaGatotsu", "IreliaEquilibriumStrike", "SowTheWind", "JarvanIVCataclysm", "JaxLeapStrike", "JaxEmpowerTwo", "JaxCounterStrike", "JayceThunderingBlow", "KarmaSpiritBind", "NetherBlade", "KatarinaR", "JudicatorRighteousFury", "KennenBringTheLight", "LeblancChaosOrbM", "BlindMonkRKick", "LeonaZenithBlade", "LeonaShieldOfDaybreak", "LissandraW", "LissandraQ", "LissandraR", "LuluQ", "LuluW", "LuluE", "LuluR", "SeismicShard", "AlZaharMaleficVisions", "AlZaharNetherGrasp", "MaokaiUnstableGrowth", "MordekaiserMaceOfSpades", "MordekaiserChildrenOfTheGrave", "SoulShackles", "NamiW", "NasusW", "NautilusGrandLine", "Takedown", "NocturneParanoia", "PoppyDevastatingBlow", "PoppyHeroicCharge", "QuinnE", "PuncturingTaunt", "RenektonPreExecute", "SpellFlux", "SejuaniWintersClaw", "TwoShivPoisen", "Fling", "SkarnerImpale", "SonaHymnofValor", "SwainTorment", "SwainDecrepify", "BlindingDart", "OrianaIzunaCommand", "OrianaDetonateCommand", "DetonatingShot", "BusterShot", "TrundleTrollSmash", "TrundlePain", "MockingShout", "Expunge", "UdyrBearStance", "UrgotHeatseekingLineMissile", "UrgotSwap2", "VeigarBalefulStrike", "VeigarPrimordialBurst", "ViR", "ViktorPowerTransfer", "VladimirTransfusion", "VolibearQ", "HungeringStrike", "XenZhaoComboTarget", "XenZhaoSweep", "YasuoQ3W", "YasuoQ3Mis", "YasuoQ3", "YasuoRKnockUpComboW" };
        readonly List<string> MenuSpells = new List<string>();

        readonly AIHeroClient Player = EloBuddy.Player.Instance;

        private AIHeroClient Target;

        Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, 625);
        Spell.Active W = new Spell.Active(SpellSlot.W);
        Spell.Active E = new Spell.Active(SpellSlot.E);
        Spell.Active R = new Spell.Active(SpellSlot.R);

        public override void CreateMenu()
        {
            base.CreateMenu();

            var menu = MenuManager.AddSubMenu("线圈");
            {
                menu.NewCheckbox("disable", "屏蔽", false);
                menu.NewCheckbox("damageindicator", "伤害指示器");
                menu.NewCheckbox("q", "Q");
            }

            menu = MenuManager.AddSubMenu("连招");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("q.smartq", "智能Q");
                menu.NewCheckbox("q.saveqtododgespells", "保留Q进行躲避", true, true);
                menu.NewCheckbox("w.aareset", "W 普攻重置", true, true);
                menu.NewCheckbox("e", "E");
                menu.NewCheckbox("r", "R");
            }

            menu = MenuManager.AddSubMenu("骚扰");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w.aareset", "W 普攻重置");
                menu.NewCheckbox("e", "E");
            }

            menu = MenuManager.AddSubMenu("清线");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("q.jimwd", "使用Q如果可击杀小兵", true, true);
                menu.NewSlider("q.minminions", "最低小兵数量 Q", 4, 1, 4, true);
                menu.NewSlider("mana%", "最低蓝量%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("清野");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("e", "E");
                menu.NewSlider("mana%", "最低蓝量%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("杂项");
            {
                menu.NewCheckbox("ks", "抢头");
                menu.NewCheckbox("dodgefireballs", "尝试躲避小龙火球攻击", true, true);
                menu.NewCheckbox("gapcloser", "突进");
            }

            menu = MenuManager.AddSubMenu("Q/W Evade Options");
            {
                menu.NewCheckbox("q/wonlyoncombo", "只在连招中躲避");

                foreach (AIHeroClient hero in EntityManager.Heroes.Enemies)
                {
                    menu.AddGroupLabel(hero.BaseSkinName);
                    {
                        foreach (SpellDataInst spell in hero.Spellbook.Spells)
                        {
                            if (DodgeSpells.Any(el => el == spell.SData.Name))
                            {
                                menu.NewSlider(spell.Name, hero.BaseSkinName + " : " + spell.Slot.ToString() + " : " + spell.Name, 3, 0, 3);
                                MenuSpells.Add(spell.Name);
                            }
                        }
                    }

                    menu.AddSeparator();
                }
            }
        }

        public override void CreateVariables()
        {
            new SkinManager(9);

            damageManager = new DamageManager
            (
                new List<Bases.Damage>()
                {
                    new Bases.Damage
                    (
                        Q, DamageType.Physical, new float[] { 0, 25, 60, 95, 130, 165 },
                        
                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 1, 1, 1, 1, 1 }, ScalingTypes.AD)
                        },

                        true, true
                    ),
                    
                    new Bases.Damage
                    (
                        E, DamageType.True, new float[] { 0, 10, 15, 20, 25, 30 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.1f, 0.125f, 0.15f, 0.175f, 0.2f }, ScalingTypes.AD)
                        }
                    )
                }
            );
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || draw.IsActive("disable")) return;

            if (draw.IsActive("q"))
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Position);
        }

        public override void PermaActive()
        {
            Target = TargetSelector.GetTarget(700, DamageType.Physical);
        }

        public override void KS()
        {
            if (!EntityManager.Heroes.Enemies.Any(it => !it.IsDead && it.IsValidTarget(Q.Range)) || !misc.IsActive("ks")) return;

            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && GetQDamage(enemy) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }
        }

        public override void Combo()
        {
            if (Target == null) return;

            if (Q.IsReady() && Target.IsValidTarget(Q.Range) && combo.IsActive("q"))
            {
                if (combo.IsActive("q.smartq")) QLogic();
                else if (combo.IsActive("q.saveqtododgespells")) { }
                else { Q.Cast(Target); }
            }

            if (R.IsReady() && Player.Distance(Target) <= Player.GetAutoAttackRange(Target) + 300 && combo.IsActive("r")) R.Cast();

            if (E.IsReady() && Player.Distance(Target) <= Player.GetAutoAttackRange(Target) - 50 && combo.IsActive("e")) E.Cast();
        }

        public override void Harass()
        {
            if (Target == null || Player.ManaPercent < harass.Value("mana%")) return;

            if (Q.IsReady() && Target.IsValidTarget(Q.Range) && harass.IsActive("q")) Q.Cast(Target);

            if (E.IsReady() && Player.Distance(Target) <= Player.GetAutoAttackRange(Target) - 50 && harass.IsActive("e")) E.Cast();
        }

        public override void LaneClear()
        {
            if (Player.ManaPercent < laneclear.Value("mana%") || !Q.IsReady() || !laneclear.IsActive("q")) return;

            var ListMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, 1000).OrderBy(minion => minion.Distance(Player));

            int hits = new int();

            if (ListMinions.Any())
            {
                if (!(ListMinions.First().Distance(Player) > Q.Range))
                {
                    hits += 1;

                    for (int i = 0; i < ListMinions.Count(); i++)
                    {
                        if (i + 1 == ListMinions.Count()) break;
                        else if (ListMinions.ElementAt(i).Distance(ListMinions.ElementAt(i + 1)) <= 200) { hits += 1; }
                        else break;
                    }

                    if (hits >= laneclear.Value("q.minminions"))
                    {
                        if (laneclear.IsActive("q.jimwd"))
                        {
                            if ((GetQDamage(ListMinions.First()) > ListMinions.First().Health || GetQDamage(ListMinions.ElementAt(1)) > ListMinions.ElementAt(1).Health)) Q.Cast(ListMinions.First());
                        }
                        else { Q.Cast(ListMinions.First()); }
                    }
                }
            }
        }

        public override void JungleClear()
        {
            if (E.IsReady() && jungleclear.IsActive("e") && EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Player.GetAutoAttackRange()).Any()) E.Cast();
        }

        public override void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead || !(sender is AIHeroClient)) return;

            var EOMenu = MenuManager.Menus[HERO_MENU]["Q/W Evade Options"].Keys.Last();

            if (EOMenu.IsActive("q/wonlyoncombo") && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) return;

            if (sender.IsValidTarget() && sender.IsEnemy && MenuSpells.Any(el => el == args.SData.Name) && Player.Distance(sender) <= args.SData.CastRange)
            {
                if (Q.IsReady() && (EOMenu.Value(args.SData.Name) == 1 || EOMenu.Value(args.SData.Name) == 3))
                {
                    if (args.SData.Name == "JaxCounterStrike") { Core.DelayAction(() => Dodge(), 2000 - Game.Ping - 200); return; }

                    if (args.SData.Name == "KarthusFallenOne") { Core.DelayAction(() => Dodge(), 3000 - Game.Ping - 200); return; }

                    if (args.SData.Name == "ZedR" && args.Target.IsMe) { Core.DelayAction(() => Dodge(), 750 - Game.Ping - 200); return; }

                    if (args.SData.Name == "SoulShackles") { Core.DelayAction(() => Dodge(), 3000 - Game.Ping - 200); return; }

                    if (args.SData.Name == "AbsoluteZero") { Core.DelayAction(() => Dodge(), 3000 - Game.Ping - 200); return; }

                    if (args.SData.Name == "NocturneUnspeakableHorror" && args.Target.IsMe) { Core.DelayAction(() => Dodge(), 2000 - Game.Ping - 200); return; }

                    Core.DelayAction(delegate
                    {
                        if (Target != null && Target.IsValidTarget(Q.Range)) Q.Cast(Target);
                    }, (int)args.SData.SpellCastTime - Game.Ping - 100);

                    Core.DelayAction(delegate
                    {
                        if (sender.IsValidTarget(Q.Range)) Q.Cast(sender);
                    }, (int)args.SData.SpellCastTime - Game.Ping - 50);

                    return;
                }

                else if (W.IsReady() && Player.IsFacing(sender) && EOMenu.Value(args.SData.Name) > 1 && ((args.Target != null && args.Target.IsMe) || Player.Position.To2D().Distance(args.Start.To2D(), args.End.To2D(), true, true) < args.SData.LineWidth * args.SData.LineWidth || args.End.Distance(Player) < args.SData.CastRadius))
                {
                    int delay = (int)(Player.Distance(sender) / ((args.SData.MissileMaxSpeed + args.SData.MissileMinSpeed) / 2) * 1000) - 150 + (int)args.SData.SpellCastTime;

                    if (args.SData.Name != "ZedR" && args.SData.Name != "NocturneUnpeakableHorror")
                    {
                        Core.DelayAction(() => W.Cast(), delay);
                        if (Target != null) Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, Target), delay + 100);
                    }
                    return;
                }
            }
        }

        public override void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Player.IsDead || sender.IsMe || sender.IsAlly) return;

            if (!misc.IsActive("gapcloser") || !Q.IsReady() || !sender.IsValidTarget(Q.Range)) return;

            Q.Cast(sender);
        }

        public override void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Player.IsDead || Orbwalker.LastTarget != target) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Q.IsReady())
            {
                if (Player.ManaPercent < jungleclear.Value("mana%") || !jungleclear.IsActive("q")) return;

                var QMinion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Player.GetAutoAttackRange()).FirstOrDefault();

                if (QMinion != null) Q.Cast(QMinion);

                return;
            }

            if (W.IsReady() && Player.Distance(target) <= Player.GetAutoAttackRange() - 50)
            {
                var AARCombo = combo.IsActive("w.aareset");

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && AARCombo)
                {
                    if (W.Cast())
                    {
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                    }

                    return;
                }

                var AARHarass = harass.IsActive("w.aareset");

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && AARHarass)
                {
                    if (W.Cast())
                    {
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                    }

                    return;
                }
            }
        }

        public override void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            var trytododge = misc.IsActive("dodgefireballs");

            if (!trytododge || !(sender is Obj_AI_Minion) || !sender.Name.Contains("SRU_Dragon")) return;

            if (args.Animation == "Spell1")
            {
                var delay = (int)((500 - Game.Ping) * Player.Distance(sender) / 74.6f);

                Core.DelayAction(() => Q.Cast(sender), delay - Game.Ping);
                //Chat.Print(Player.Distance(sender));
            }
        }

        //------------------------------------|| Methods ||--------------------------------------

        //-------------------------------------GetQDamage()---------------------------------------

        float GetQDamage(Obj_AI_Base unit)
        {
            if (!unit.IsValidTarget()) return 0;

            if (unit.IsMinion)
                return Player.CalculateDamageOnUnit(unit, DamageType.Physical, new[] { 0, 100, 160, 220, 280, 340 }[Q.Level] + Player.TotalAttackDamage, true, true);

            return Player.CalculateDamageOnUnit(unit, DamageType.Physical, new[] { 0, 25, 60, 95, 130, 165 }[Q.Level] + Player.TotalAttackDamage, true, true);

        }

        //----------------------------------------QLogic()-----------------------------------------

        void QLogic()
        {
            if (Target.IsDashing()) Q.Cast(Target);
            if (Target.HealthPercent <= 30) Q.Cast(Target);
            if (Player.HealthPercent <= 30) Q.Cast(Target);
            if (GetQDamage(Target) >= Target.Health) Q.Cast(Target);
        }

        //----------------------------------------Dodge()------------------------------------------

        void Dodge()
        {
            if (Target != null)
            {
                if (Q.IsInRange(Target))
                {
                    Q.Cast(Target);
                    return;
                }
            }

            var champ = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(Q.Range));

            if (champ != null) { Q.Cast(champ); return; }

            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(it => it.IsValidTarget(Q.Range));

            if (minion != null) { Q.Cast(minion); return; }

            return;
        }
    }
}
