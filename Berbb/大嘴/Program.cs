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

namespace KogMaw
{
    public static class Program
    {

        private static Spell.Skillshot Q, E, R;
        private static Spell.Active W;

        private static int LastAATick;

        private static Menu Menu;

        private static bool IsZombie = myHero.HasBuff("kogmawicathiansurprise");
        private static bool wActive = myHero.HasBuff("kogmawbioarcanebarrage");

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        public static void Main()
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        #region Menu Items
        public static bool useQ { get { return Menu["useQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool useW { get { return Menu["useW"].Cast<CheckBox>().CurrentValue; } }
        public static bool useE { get { return Menu["useE"].Cast<CheckBox>().CurrentValue; } }
        public static bool useR { get { return Menu["useR"].Cast<CheckBox>().CurrentValue; } }
        public static int rLimit { get { return Menu["rLimit"].Cast<Slider>().CurrentValue; } }
        public static int rLimitH { get { return Menu["rLimitH"].Cast<Slider>().CurrentValue; } }
        public static int rLimitLC { get { return Menu["rLimitLC"].Cast<Slider>().CurrentValue; } }
        public static int rLimitJG { get { return Menu["rLimitJG"].Cast<Slider>().CurrentValue; } }
        public static int manaH { get { return Menu["manaH"].Cast<Slider>().CurrentValue; } }
        public static int manaLC { get { return Menu["manaLC"].Cast<Slider>().CurrentValue; } }
        public static int manaJG { get { return Menu["manaJG"].Cast<Slider>().CurrentValue; } }
        public static bool manaW { get { return Menu["manaW"].Cast<CheckBox>().CurrentValue; } }
        public static bool useWJG { get { return Menu["useWJG"].Cast<CheckBox>().CurrentValue; } }
        public static bool useEJG { get { return Menu["useEJG"].Cast<CheckBox>().CurrentValue; } }
        public static bool useRJG { get { return Menu["useRJG"].Cast<CheckBox>().CurrentValue; } }
        public static bool useQH { get { return Menu["useQH"].Cast<CheckBox>().CurrentValue; } }
        public static bool useEH { get { return Menu["useEH"].Cast<CheckBox>().CurrentValue; } }
        public static bool useRH { get { return Menu["useRH"].Cast<CheckBox>().CurrentValue; } }
        public static bool useELC { get { return Menu["useELC"].Cast<CheckBox>().CurrentValue; } }
        public static bool useRLC { get { return Menu["useRLC"].Cast<CheckBox>().CurrentValue; } }
        public static bool human { get { return Menu["human"].Cast<KeyBind>().CurrentValue; } }
        public static int esw { get { return Menu["esw"].Cast<Slider>().CurrentValue; } }
        public static int swatk { get { return Menu["swatk"].Cast<Slider>().CurrentValue; } }
        public static int swez { get { return Menu["swez"].Cast<Slider>().CurrentValue; } }
        public static int swchz { get { return Menu["swchz"].Cast<Slider>().CurrentValue; } }
        public static bool dontw { get { return Menu["dontw"].Cast<KeyBind>().CurrentValue; } }

        #endregion

        private static void OnLoad(EventArgs args)
        {
            if (myHero.Hero != Champion.KogMaw)
            {
                return;
            }

            Menu = MainMenu.AddMenu("L那边的SharpShooter 大嘴", "kogmaw");
            Menu.AddLabel("Ported from SharpShooter - Berb");
            Menu.AddLabel("由CH汉化，有些功能不太了解，我玩的时候不用没影响");
            Menu.AddSeparator();
            Menu.AddGroupLabel("连招");
            Menu.Add("useQ", new CheckBox("使用 Q"));
            Menu.Add("useW", new CheckBox("使用 W"));
            Menu.Add("useE", new CheckBox("使用 E"));
            Menu.Add("useR", new CheckBox("使用 R"));
            Menu.Add("manaW", new CheckBox("为W保存蓝"));
            Menu.Add("dontw", new KeyBind("连招时不移动", false, KeyBind.BindTypes.PressToggle, 'A'));
            Menu.Add("rLimit", new Slider("R叠加限制", 3, 1, 6));
            Menu.AddSeparator();
            Menu.AddGroupLabel("骚扰");
            Menu.Add("useQH", new CheckBox("使用 Q"));
            Menu.Add("useEH", new CheckBox("使用 E"));
            Menu.Add("useRH", new CheckBox("使用 R"));
            Menu.Add("rLimitH", new Slider("R叠加限制", 1, 1, 6));
            Menu.Add("manaH", new Slider("当蓝量高于 X 时进行骚扰 :", 60, 1, 100));
            Menu.AddSeparator();
            Menu.AddGroupLabel("清线");
            Menu.Add("useELC", new CheckBox("使用 E"));
            Menu.Add("useRLC", new CheckBox("使用 R"));
            Menu.Add("rLimitLC", new Slider("R叠加限制", 1, 1, 6));
            Menu.Add("manaLC", new Slider("当蓝量高于 X 时进行清线 :", 60, 1, 100));
            Menu.AddSeparator();
            Menu.AddGroupLabel("清野");
            Menu.Add("useWJG", new CheckBox("使用 W"));
            Menu.Add("useEJG", new CheckBox("使用 E"));
            Menu.Add("useRJG", new CheckBox("使用 R"));
            Menu.Add("rLimitJG", new Slider("R叠加限制", 2, 1, 6));
            Menu.Add("manaJG", new Slider("当蓝量高于 X 时进行清野 :", 60, 1, 100));
            Menu.AddSeparator();
            Menu.Add("human", new KeyBind("使用 人性化设置", false, KeyBind.BindTypes.PressToggle, 'T'));
            Menu.Add("esw", new Slider("开启 慢移动/不移动 :", 225, 0, 500));
            Menu.Add("swatk", new Slider("慢移动/攻击速度:", 225, 0, 500));
            Menu.Add("swez", new Slider("慢移动/逃跑区 :", 250, 0, 710));
            Menu.Add("swchz", new Slider("慢移动/追击区", 650, 0, 710));
            Menu.AddSeparator();

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 1650, 70);
            W = new Spell.Active(SpellSlot.W, (uint)myHero.GetAutoAttackRange());
            E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Linear, 500, 1400, 120);
            R = new Spell.Skillshot(SpellSlot.R, 1260, SkillShotType.Circular, 1500, int.MaxValue, 225);

            Drawing.OnDraw += OnDraw;
            Game.OnTick += OnTick;
            Orbwalker.OnPreAttack += OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static bool CanMove(float extraWindup)
        {
            if (LastAATick <= Environment.TickCount)
                return (Environment.TickCount + Game.Ping / 2 >= LastAATick + myHero.AttackCastDelay * 1000 + extraWindup);
            return false;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                LastAATick = Environment.TickCount;
            }
        }

        public static float getSpellMana(SpellSlot spell)
        {
            return myHero.Spellbook.GetSpell(spell).SData.Mana;
        }

        public static bool QIsReadyPerfectly()
        {
            var spell = Q;
            return spell != null && spell.Slot != SpellSlot.Unknown && spell.State != SpellState.Cooldown && spell.State != SpellState.Disabled && spell.State != SpellState.NoMana && spell.State != SpellState.NotLearned && spell.State != SpellState.Surpressed && spell.State != SpellState.Unknown && spell.State == SpellState.Ready;
        }

        public static bool WIsReadyPerfectly()
        {
            var spell = W;
            return spell != null && spell.Slot != SpellSlot.Unknown && spell.State != SpellState.Cooldown && spell.State != SpellState.Disabled && spell.State != SpellState.NoMana && spell.State != SpellState.NotLearned && spell.State != SpellState.Surpressed && spell.State != SpellState.Unknown && spell.State == SpellState.Ready;
        }

        public static bool EIsReadyPerfectly()
        {
            var spell = E;
            return spell != null && spell.Slot != SpellSlot.Unknown && spell.State != SpellState.Cooldown && spell.State != SpellState.Disabled && spell.State != SpellState.NoMana && spell.State != SpellState.NotLearned && spell.State != SpellState.Surpressed && spell.State != SpellState.Unknown && spell.State == SpellState.Ready;
        }

        public static bool RIsReadyPerfectly()
        {
            var spell = R;
            return spell != null && spell.Slot != SpellSlot.Unknown && spell.State != SpellState.Cooldown && spell.State != SpellState.Disabled && spell.State != SpellState.NoMana && spell.State != SpellState.NotLearned && spell.State != SpellState.Surpressed && spell.State != SpellState.Unknown && spell.State == SpellState.Ready;
        }

        private static void OnTick(EventArgs args)
        {
            if (myHero.IsDead) return;

            W = new Spell.Active(SpellSlot.W, (uint)(565 + 60 + W.Level * 30 + 65));
            R = new Spell.Skillshot(SpellSlot.R, (uint)(900 + R.Level * 300), SkillShotType.Circular, 1500, int.MaxValue, 225);

            if (human && wActive)
            {
                if (1 / myHero.AttackDelay > Convert.ToSingle(esw) / 100)
                {
                    if (EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(130 + swez)) || !(EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(130 + swchz))))
                    {
                        Orbwalker.DisableMovement = false;
                        if (Environment.TickCount + Game.Ping / 2 + 25 >= LastAATick + 1.0 / Convert.ToSingle(swatk) * 1000 * 100)
                        {
                            Orbwalker.DisableAttacking = false;
                        }
                        else
                        {
                            Orbwalker.DisableAttacking = true;
                        }
                    }
                    else
                    {
                        Orbwalker.DisableAttacking = false;
                        Orbwalker.DisableMovement = true;
                    }
                }
                else
                {
                    Orbwalker.DisableAttacking = false;
                    Orbwalker.DisableMovement = false;
                }
            }

            if (CanMove(100))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (useQ)
                    {
                        if (QIsReadyPerfectly())
                        {
                            if (manaW && W.Level > 0 ? myHero.Mana - getSpellMana(SpellSlot.Q) >= getSpellMana(SpellSlot.W) : true)
                            {
                                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                                if (target != null)
                                {
                                    Q.Cast(target);
                                }
                            }
                        }
                    }


                    if (useW)
                    {
                        if (WIsReadyPerfectly())
                        {
                            if (EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(W.Range)))
                            {
                                W.Cast();
                            }
                        }

                    }


                    if (useE)
                    {
                        if (EIsReadyPerfectly())
                        {
                            if (manaW && W.Level > 0 ? myHero.Mana - getSpellMana(SpellSlot.E) >= getSpellMana(SpellSlot.W) : true)
                            {
                                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                                if (target != null)
                                {
                                    E.Cast(target);
                                }
                            }
                        }
                    }


                    if (useR)
                    {

                        if (RIsReadyPerfectly())
                        {
                            if (manaW && W.Level > 0 ? myHero.Mana - getSpellMana(SpellSlot.R) >= getSpellMana(SpellSlot.W) : true)
                            {
                                if (myHero.GetBuffCount("kogmawlivingartillerycost") < rLimit)
                                {
                                    var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                                    if (target != null)
                                        R.Cast(target);
                                }
                                else
                                {
                                    var killableTarget = EntityManager.Heroes.Enemies.FirstOrDefault(x => x.IsKillableAndValidTarget(myHero.GetSpellDamage(x, SpellSlot.R), DamageType.Magical, R.Range) && R.GetPrediction(x).HitChance >= HitChance.High);
                                    if (killableTarget != null)
                                        R.Cast(killableTarget);
                                }
                            }
                        }
                    }
                }
                else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (useQH)
                    {
                        if (QIsReadyPerfectly())
                            if (myHero.IsManaPercentOkay(manaH))
                            {
                                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                                if (target != null)
                                    Q.Cast(target);
                            }
                    }

                    if (useEH)
                    {
                        if (EIsReadyPerfectly())
                            if (myHero.IsManaPercentOkay(manaH))
                            {
                                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                                if (target != null)
                                    E.Cast(target);
                            }
                    }


                    if (useRH)
                    {
                        if (RIsReadyPerfectly())
                        {
                            if (myHero.IsManaPercentOkay(manaH))
                            {
                                if (myHero.GetBuffCount("kogmawlivingartillerycost") < rLimitH)
                                {
                                    var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                                    if (target != null)
                                        R.Cast(target);
                                }
                            }
                        }
                    }
                }
                else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, myHero.GetAutoAttackRange()))
                    {
                        if (useELC)
                        {
                            if (myHero.IsManaPercentOkay(manaLC))
                            {
                                if (EIsReadyPerfectly())
                                {
                                    var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, E.Range);
                                    var farmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, E.Width, (int)E.Range);
                                    if (farmLocation.HitNumber >= 3)
                                        E.Cast(farmLocation.CastPosition);
                                }
                            }
                        }

                        if (useRLC)
                        {
                            if (myHero.IsManaPercentOkay(manaLC))
                            {
                                if (RIsReadyPerfectly())
                                {
                                    if (myHero.GetBuffCount("kogmawlivingartillerycost") < rLimitLC)
                                    {
                                        var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, R.Range);
                                        var farmLocation = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, R.Width, (int)R.Range);
                                        if (farmLocation.HitNumber >= 2)
                                        {
                                            R.Cast(farmLocation.CastPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var jungleMobs in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Program.W.Range) && o.Team == GameObjectTeam.Neutral && o.IsVisible && !o.IsDead))
                    {

                        if (WIsReadyPerfectly())
                            if (useWJG)
                                W.Cast();

                        if (useEJG)
                        {
                            if (myHero.IsManaPercentOkay(manaJG))
                            {
                                if (EIsReadyPerfectly())
                                {
                                    var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, E.Range);
                                    var farmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, E.Width, (int)E.Range);
                                    if (farmLocation.HitNumber >= 2)
                                    {
                                        E.Cast(farmLocation.CastPosition);
                                    }
                                }
                            }
                        }

                        if (useRJG)
                        {
                            if (myHero.IsManaPercentOkay(manaJG))
                            {
                                if (RIsReadyPerfectly())
                                {
                                    if (myHero.GetBuffCount("kogmawlivingartillerycost") < rLimitJG)
                                    {
                                        var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, R.Range);
                                        var farmLocation = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, R.Width, (int)R.Range);
                                        if (farmLocation.HitNumber >= 2)
                                        {
                                            R.Cast(farmLocation.CastPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static bool IsManaPercentOkay(this AIHeroClient hero, int manaPercent)
        {
            return myHero.ManaPercent > manaPercent;
        }

        internal static bool IsKillableAndValidTarget(this AIHeroClient target, double calculatedDamage, DamageType damageType, float distance = float.MaxValue)
        {
            if (target == null || !target.IsValidTarget(distance) || target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return false;
            }

            if (target.HasBuff("Undying Rage"))
            {
                return false;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (target.HasBuff("DiplomaticImmunity") && !myHero.HasBuff("poppyulttargetmark"))
            {
                return false;
            }

            if (target.HasBuff("BansheesVeil"))
            {
                return false;
            }

            if (target.HasBuff("SivirShield"))
            {
                return false;
            }

            if (target.HasBuff("ShroudofDarkness"))
            {
                return false;
            }

            if (myHero.HasBuff("summonerexhaust"))
                calculatedDamage *= 0.6;

            if (target.ChampionName == "Blitzcrank")
                if (!target.HasBuff("manabarriercooldown"))
                    if (target.Health + target.HPRegenRate +
                        (damageType == DamageType.Physical ? target.AttackShield : target.MagicShield) +
                        target.Mana * 0.6 + target.PARRegenRate < calculatedDamage)
                        return true;

            if (target.ChampionName == "Garen")
                if (target.HasBuff("GarenW"))
                    calculatedDamage *= 0.7;

            if (target.HasBuff("FerociousHowl"))
                calculatedDamage *= 0.3;

            return target.Health + target.HPRegenRate + (damageType == DamageType.Physical ? target.AttackShield : target.MagicShield) < calculatedDamage - 2;
        }

        private static void OnDraw(EventArgs args)
        {
            if (dontw && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Orbwalker.DisableMovement == false)
            {
                Orbwalker.DisableMovement = true;
                Player.IssueOrder(GameObjectOrder.Stop, myHero);
            }

            if (!dontw || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Orbwalker.DisableMovement = false;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && dontw)
            {
                Drawing.DrawText(Drawing.Width * 0.5f, Drawing.Height * 0.3f, System.Drawing.Color.Orange, "W开启时不移动.", 50);
            }
        }

        private static void OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!args.Target.IsMe) return;

            if (IsZombie)
                args.Process = false;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (WIsReadyPerfectly())
                    if (useW)
                        if (args.Target.IsValidTarget(W.Range))
                            W.Cast();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                foreach (var jungleMobs in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Program.W.Range) && o.Team == GameObjectTeam.Neutral && o.IsVisible && !o.IsDead))
                {
                    if (jungleMobs.BaseSkinName == "SRU_Red" || jungleMobs.BaseSkinName == "SRU_Blue" || jungleMobs.BaseSkinName == "SRU_Gromp" || jungleMobs.BaseSkinName == "SRU_Murkwolf" || jungleMobs.BaseSkinName == "SRU_Razorbeak" || jungleMobs.BaseSkinName == "SRU_Krug" || jungleMobs.BaseSkinName == "Sru_Crab")
                    {
                        if (WIsReadyPerfectly())
                            if (useWJG)
                                W.Cast();
                    }
                }
            }
        }
    }
}
