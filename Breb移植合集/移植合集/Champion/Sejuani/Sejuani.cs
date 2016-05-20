using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace ElSejuani
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Sejuani
    {
        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 650)},
            {Spells.W, new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player) + 100)},
            {Spells.E, new Spell(SpellSlot.E, 1000)},
            {Spells.R, new Spell(SpellSlot.R, 1175)}
        };

        private static SpellSlot _ignite;

        #endregion

        #region Properties

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        public static BuffInstance GetFrost(Obj_AI_Base target)
        {
            return target.Buffs.FirstOrDefault(buff => buff.Name == "sejuanifrost");
        }

        public static void OnLoad()
        {
            if (Player.CharData.BaseSkinName != "Sejuani")
            {
                return;
            }

            Console.WriteLine("Injected");

            spells[Spells.Q].SetSkillshot(0, 70, 1600, true, SkillshotType.SkillshotLine);
            spells[Spells.R].SetSkillshot(250, 110, 1600, false, SkillshotType.SkillshotLine);

            _ignite = Player.GetSpellSlot("summonerdot");

            ElSejuaniMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.OnDraw;

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.IsValidTarget(spells[Spells.Q].Range))
            {
                return;
            }

            if (gapcloser.Sender.LSDistance(Player) > spells[Spells.Q].Range)
            {
                return;
            }

            var useQ = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.interuptMenu, "ElSejuani.Interupt.Q");
            var useR = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.interuptMenu, "ElSejuani.Interupt.R");

            if (gapcloser.Sender.IsValidTarget(spells[Spells.Q].Range))
            {
                if (useQ && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(gapcloser.Sender);
                }

                if (useR && !spells[Spells.Q].IsReady() && spells[Spells.R].IsReady())
                {
                    spells[Spells.R].Cast(gapcloser.Sender);
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var comboQ = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.cMenu, "ElSejuani.Combo.Q");
            var comboE = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.cMenu, "ElSejuani.Combo.E");
            var comboW = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.cMenu, "ElSejuani.Combo.E");
            var comboR = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.cMenu, "ElSejuani.Combo.R");
            var countEnemyR = ElSejuaniMenu.getSliderItem(ElSejuaniMenu.cMenu, "ElSejuani.Combo.R.Count");

            if (comboQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (comboW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (comboE && spells[Spells.E].IsReady() && IsFrozen(target) && target.IsValidTarget(spells[Spells.E].Range))
            {
                if (IsFrozen(target))
                {
                    spells[Spells.E].Cast();
                }

                if (IsFrozen(target)
                    && target.ServerPosition.LSDistance(Player.ServerPosition, true) <= spells[Spells.E].Range)
                {
                    spells[Spells.E].Cast();
                }
            }

            if (comboR && spells[Spells.R].IsReady())
            {
                foreach (
                    var x in
                        HeroManager.Enemies.Where(hero => !hero.IsDead && hero.IsValidTarget(spells[Spells.R].Range)))
                {
                    var pred = spells[Spells.R].GetPrediction(x);
                    if (pred.AoeTargetsHitCount >= countEnemyR)
                    {
                        spells[Spells.R].Cast(x);
                    }
                }
            }
        }

        private static HitChance GetHitchance()
        {
            switch (ElSejuaniMenu.getBoxItem(ElSejuaniMenu.miscMenu, "ElSejuani.hitChance"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var harassQ = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.hMenu, "ElSejuani.Harass.Q");
            var harassW = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.hMenu, "ElSejuani.Harass.W");
            var harassE = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.hMenu, "ElSejuani.Harass.E");
            var minmana = ElSejuaniMenu.getSliderItem(ElSejuaniMenu.hMenu, "ElSejuani.harass.mana");

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            if (harassQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (harassW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (harassE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                if (IsFrozen(target) && spells[Spells.E].GetDamage(target) > target.Health)
                {
                    spells[Spells.E].Cast();
                }

                if (IsFrozen(target)
                    && target.ServerPosition.LSDistance(Player.ServerPosition, true)
                    < Math.Pow(spells[Spells.E].Range*0.8, 2))
                {
                    spells[Spells.E].Cast();
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.LSDistance(Player) > spells[Spells.Q].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.Q].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast(sender);
            }
        }

        private static bool IsFrozen(Obj_AI_Base target)
        {
            return target.HasBuff("SejuaniFrost");
        }

        private static void JungleClear()
        {
            var clearQ = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.Q");
            var clearW = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.W");
            var clearE = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.E");
            var minmana = ElSejuaniMenu.getSliderItem(ElSejuaniMenu.lMenu, "minmanaclear");
            var minQ = ElSejuaniMenu.getSliderItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.Q.Count");

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.W].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].IsReady() && clearQ)
                {
                    if (spells[Spells.Q].GetLineFarmLocation(minions).MinionsHit >= minQ)
                    {
                        spells[Spells.Q].Cast(spells[Spells.Q].GetLineFarmLocation(minions).Position);
                    }
                }

                if (spells[Spells.W].IsReady() && clearW
                    && minion.ServerPosition.LSDistance(Player.ServerPosition, true) <= spells[Spells.W].Range)
                {
                    spells[Spells.W].Cast();
                }

                if (spells[Spells.E].IsReady() && clearE
                    && minions[0].Health + minions[0].HPRegenRate/2 <= spells[Spells.E].GetDamage(minion)
                    && minion.HasBuff("sejuanifrost"))
                {
                    spells[Spells.E].Cast();
                }
            }
        }

        private static void LaneClear()
        {
            var clearQ = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.Q");
            var clearW = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.W");
            var clearE = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.E");
            var minmana = ElSejuaniMenu.getSliderItem(ElSejuaniMenu.lMenu, "minmanaclear");
            var minQ = ElSejuaniMenu.getSliderItem(ElSejuaniMenu.lMenu, "ElSejuani.Clear.Q.Count");

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].IsReady() && clearQ)
                {
                    if (spells[Spells.Q].GetLineFarmLocation(minions).MinionsHit >= minQ)
                    {
                        spells[Spells.Q].Cast(spells[Spells.Q].GetLineFarmLocation(minions).Position);
                    }
                }

                if (spells[Spells.W].IsReady() && clearW && minion.ServerPosition.LSDistance(Player.ServerPosition, true) >= spells[Spells.W].Range)
                {
                    spells[Spells.W].Cast();
                }

                if (spells[Spells.E].IsReady() && clearE
                    && minions[0].Health + minions[0].HPRegenRate/2 <= spells[Spells.E].GetDamage(minion)
                    && minion.HasBuff("sejuanifrost"))
                {
                    spells[Spells.E].Cast();
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (ElSejuaniMenu.getKeyBindItem(ElSejuaniMenu.cMenu, "ElSejuani.Combo.Semi.R"))
            {
                SemiR();
            }
        }


        private static void SemiR()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (!spells[Spells.R].IsReady() || !target.IsValidTarget(spells[Spells.R].Range))
            {
                return;
            }

            var prediction = spells[Spells.R].GetPrediction(target);
            if (prediction.Hitchance >= HitChance.High)
            {
                spells[Spells.R].Cast(target);
            }
        }

        #endregion
    }
}