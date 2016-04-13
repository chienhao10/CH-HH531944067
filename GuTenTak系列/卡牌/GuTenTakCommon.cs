using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GuTenTak.TwistedFate
{
    internal class Common : Program
    {
        public static object HeroManager { get; private set; }

        public static float GetComboDamage(AIHeroClient target)
        {
            var damage = 0f;
            if (Q.IsReady())
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }

            return damage;
        }

        public enum Cards
        {
            Red,
            Yellow,
            Blue,
            None,
        }

        public enum SelectStatus
        {
            Ready,
            Selecting,
            Selected,
            Cooldown,
        }

        internal class CardSelector
        {
            public static Cards SelectedCard;
            public static int LastW;
            public static SelectStatus Status { get; set; }

            public static int Delay => ModesMenu1["WHumanizer"].Cast<CheckBox>().CurrentValue ? ModesMenu1["WHumanizerms"].Cast<Slider>().CurrentValue : new Random().Next(ModesMenu1["WHumanizerrandom"].Cast<Slider>().CurrentValue, ModesMenu1["WHumanizerrandom2"].Cast<Slider>().CurrentValue);

            static CardSelector()
            {
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                Game.OnUpdate += Game_OnGameUpdate;
            }

            public static void StartSelecting(Cards card)
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && Status == SelectStatus.Ready)
                {
                    SelectedCard = card;
                    if (Environment.TickCount - LastW > 170 + Game.Ping / 2)
                    {
                        Program.W.Cast();
                        LastW = Environment.TickCount;
                    }
                }
            }

            public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (args.SData.Name == "PickACard")
                {
                    Status = SelectStatus.Selecting;
                }

                if (args.SData.Name.ToLower() == "goldcardlock" || args.SData.Name.ToLower() == "bluecardlock" ||
                    args.SData.Name.ToLower() == "redcardlock")
                {
                    Status = SelectStatus.Selected;
                    SelectedCard = Cards.None;
                }
            }

            private static void Game_OnGameUpdate(EventArgs args)
            {
                var wName = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name;
                var wState = ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.W);

                if ((wState == SpellState.Ready &&
                     wName == "PickACard" &&
                     (Status != SelectStatus.Selecting || Environment.TickCount - LastW > 500)) ||
                    ObjectManager.Player.IsDead)
                {
                    Status = SelectStatus.Ready;
                }
                else if (wState == SpellState.Cooldown &&
                         wName == "PickACard")
                {
                    SelectedCard = Cards.None;
                    Status = SelectStatus.Cooldown;
                }
                else if (wState == SpellState.Surpressed &&
                         !ObjectManager.Player.IsDead)
                {
                    Status = SelectStatus.Selected;
                }

                if (SelectedCard == Cards.Blue && wName.ToLower() == "bluecardlock" && Environment.TickCount - Delay > LastW)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
                }
                else if (SelectedCard == Cards.Yellow && wName.ToLower() == "goldcardlock" && Environment.TickCount - Delay > LastW)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
                }
                else if (SelectedCard == Cards.Red && wName.ToLower() == "redcardlock" && Environment.TickCount - Delay > LastW)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
                }
            }
        }

        public static void Combo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["ComboYellowCard"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(Target);
            if (!Target.IsValid()) return;

            if (!_Player.HasBuff("Pick A Card Gold") && PlayerInstance.IsInAutoAttackRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
            {
                Q.Cast(Target);
            }

            if (!PlayerInstance.IsInAutoAttackRange(Target) && Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
            {
                Q.Cast(Qp.CastPosition);
            }

            if (Target.IsInRange(Target,1000) && W.IsReady() && useW)
            {
                Common.CardSelector.StartSelecting(Common.Cards.Yellow);
            }
        }

        public static void LastHit()
        {
            var source =
    EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault
        (m =>
            m.IsValidTarget(1000) &&
            (Program.ModesMenu2["ManaLast"].Cast<Slider>().CurrentValue >= _Player.ManaPercent && m.IsEnemy && !m.IsDead && m.IsValid && !m.IsInvulnerable));
            if (source == null) return;
            if (ModesMenu2["LastBlue"].Cast<CheckBox>().CurrentValue)
            {
              Common.CardSelector.StartSelecting(Common.Cards.Blue);
            }
        }

        public static void Harass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(Target);
            if (!Target.IsValid()) return;

            if (Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High && _Player.ManaPercent >= Program.ModesMenu1["ManaHQ"].Cast<Slider>().CurrentValue)
            {
                Q.Cast(Qp.CastPosition);
            }

            if (Target.IsInRange(Target, 1000) && W.IsReady() && useW && _Player.ManaPercent >= Program.ModesMenu1["ManaHW"].Cast<Slider>().CurrentValue)
            {
                if (ModesMenu1["HarassPick"].Cast<ComboBox>().CurrentValue == 0)
                {
                    Common.CardSelector.StartSelecting(Common.Cards.Blue);
                }
                if (ModesMenu1["HarassPick"].Cast<ComboBox>().CurrentValue == 1)
                {
                    Common.CardSelector.StartSelecting(Common.Cards.Red);
                }
                if (ModesMenu1["HarassPick"].Cast<ComboBox>().CurrentValue == 2)
                {
                    Common.CardSelector.StartSelecting(Common.Cards.Yellow);
                }
            }
        }

        internal static void QAA(AttackableUnit target, EventArgs args)
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
        var Qp = Q.GetPrediction(Target);
            if (!Target.IsValid()) return;
            if (_Player.HasBuff("Pick A Card Gold") && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
            {
                Q.Cast(Target);
            }
            if (_Player.HasBuff("Pick A Card Red") && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
            {
                Q.Cast(Target);
            }
        }

        public static void Flee()
        {

        }


        internal static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;
            var type = args.Buff.Type;
            var duration = args.Buff.EndTime - Game.Time;
            var Name = args.Buff.Name.ToLower();

            if (ModesMenu3["Qssmode"].Cast<ComboBox>().CurrentValue == 0)
            {
                if (type == BuffType.Taunt && ModesMenu3["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ModesMenu3["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ModesMenu3["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ModesMenu3["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ModesMenu3["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Flee && ModesMenu3["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ModesMenu3["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ModesMenu3["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ModesMenu3["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ModesMenu3["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ModesMenu3["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ModesMenu3["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ModesMenu3["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ModesMenu3["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
            }
            if (ModesMenu3["Qssmode"].Cast<ComboBox>().CurrentValue == 1 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (type == BuffType.Taunt && ModesMenu3["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ModesMenu3["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ModesMenu3["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ModesMenu3["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ModesMenu3["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Flee && ModesMenu3["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ModesMenu3["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ModesMenu3["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ModesMenu3["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ModesMenu3["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ModesMenu3["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ModesMenu3["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ModesMenu3["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ModesMenu3["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
            }
        }


        internal static void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical); // 550 = Botrk.Range
            var hextech = TargetSelector.GetTarget(700, DamageType.Magical); // 700 = hextech.Range

            if (ModesMenu3["useYoumuu"].Cast<CheckBox>().CurrentValue && Program.Youmuu.IsOwned() && Program.Youmuu.IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(1500) == 1)
                {
                    Program.Youmuu.Cast();
                }
            }
            if (hextech != null)
            {
                if (ModesMenu3["usehextech"].Cast<CheckBox>().CurrentValue && Item.HasItem(Program.Cutlass.Id) && Item.CanUseItem(Program.Cutlass.Id))
                {
                    Item.UseItem(Program.hextech.Id, hextech);
                }
            }
            if (target != null)
            {
                if (ModesMenu3["useBotrk"].Cast<CheckBox>().CurrentValue && Item.HasItem(Program.Cutlass.Id) && Item.CanUseItem(Program.Cutlass.Id) &&
                    Player.Instance.HealthPercent < ModesMenu3["minHPBotrk"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ModesMenu3["enemyMinHPBotrk"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem(Program.Cutlass.Id, target);
                }
                if (ModesMenu3["useBotrk"].Cast<CheckBox>().CurrentValue && Item.HasItem(Program.Botrk.Id) && Item.CanUseItem(Program.Botrk.Id) &&
                    Player.Instance.HealthPercent < ModesMenu3["minHPBotrk"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ModesMenu3["enemyMinHPBotrk"].Cast<Slider>().CurrentValue)
                {
                    Program.Botrk.Cast(target);
                }
            }
        }

        internal static void DoQSS()
        {
            if (ModesMenu3["useQss"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady() && ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Qss.Cast(), ModesMenu3["QssDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady() && ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Simitar.Cast(), ModesMenu3["QssDelay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void UltQSS()
        {
            if (ModesMenu3["useQss"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady())
            {
                Core.DelayAction(() => Qss.Cast(), ModesMenu3["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady())
            {
                Core.DelayAction(() => Simitar.Cast(), ModesMenu3["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
        }

        public static void Skinhack()
        {
            if (ModesMenu3["skinhack"].Cast<CheckBox>().CurrentValue)
            {
                Player.SetSkinId((int)ModesMenu3["skinId"].Cast<ComboBox>().CurrentValue);
            }
        }



        public static void KillSteal()
        {
            if (Program.ModesMenu1["KS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
                {
                    if (enemy == null) return;
                    if (enemy.IsValidTarget(Q.Range))
                    {
                        var Qp = Q.GetPrediction(enemy);
                        if (DamageLib.QCalc(enemy) >= enemy.Health && Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && Qp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                        {
                            Q.Cast(Qp.CastPosition);
                        }
                    }
                }
            }
        }

        //Auto Q
        public static new void AutoQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
            if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                return;
            var Qp = Q.GetPrediction(target);
                if (Qp.HitChance == HitChance.Immobile && Program.ModesMenu1["AutoHarass"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu1["ManaAuto"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(target);
                }
        }
    }
}