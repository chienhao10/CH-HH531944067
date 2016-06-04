using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Ekko : Champion
    {

        private static GameObject RMissile, WMissile;
        private Spell Q1;

        private float QMANA, WMANA, EMANA, RMANA;
        public Ekko()
        {

            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Lich_Bane),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Abyssal_Scepter),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Needlessly_Large_Rod
                        }
            };
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly)
            {
                if (obj.Name == "Ekko")
                    RMissile = obj;
                if (obj.Name == "Ekko_Base_W_Cas.troy")
                    WMissile = obj;
            }
        }


        public override void useQ(Obj_AI_Base t)
        {
            if (!Q.IsReady() || t == null)
                return;
            var qDmg = GetQdmg(t);
            if (qDmg > t.Health)
                Q.Cast(t, true);
            else if (ObjectManager.Player.Mana > RMANA + QMANA)
                Q.Cast(t);
            if (player.Mana > RMANA + QMANA + WMANA)
            {
                foreach (var enemy in LXOrbwalker.AllEnemys.Where(enemy => enemy.LSIsValidTarget(Q.Range)))
                    Q.Cast(enemy, true);
            }
        }

        public override void useW(Obj_AI_Base t)
        {
            if (!W.IsReady())
                return;
            var qDmg = GetQdmg(t);
            if (t.HasBuffOfType(BuffType.Slow) || t.LSCountEnemiesInRange(250) > 1)
            {
                W.Cast(t);

            }
            if (ObjectManager.Player.Mana > RMANA + WMANA + EMANA + QMANA)
                W.Cast(t);
            else if (!ObjectManager.Player.UnderTurret(true) && ObjectManager.Player.Mana > ObjectManager.Player.MaxMana * 0.8 && ObjectManager.Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA)
                W.Cast(t);
            else if (ObjectManager.Player.Mana > RMANA + WMANA + EMANA)
            {
                foreach (var enemy in LXOrbwalker.AllEnemys.Where(enemy => enemy.LSIsValidTarget(W.Range)))
                    W.Cast(enemy, true);
            }
        }

        public override void useE(Obj_AI_Base t)
        {
            if (!E.IsReady() || t == null)
                return;
            if (WMissile != null && WMissile.IsValid)
            {
                if (WMissile.Position.LSCountEnemiesInRange(200) > 0 && WMissile.Position.LSDistance(player.ServerPosition) < 100)
                {
                    E.Cast(player.Position.LSExtend(WMissile.Position, E.Range), true);
                }
            }


            if (E.IsReady() && ObjectManager.Player.Mana > RMANA + EMANA
                 && ObjectManager.Player.LSCountEnemiesInRange(260) > 0
                 && ObjectManager.Player.Position.LSExtend(Game.CursorPos, E.Range).LSCountEnemiesInRange(500) < 3
                 && t.Position.LSDistance(Game.CursorPos) > t.Position.LSDistance(ObjectManager.Player.Position))
            {
                E.Cast(ObjectManager.Player.Position.LSExtend(Game.CursorPos, E.Range), true);
            }
            else if (ObjectManager.Player.Health > ObjectManager.Player.MaxHealth * 0.4
                && ObjectManager.Player.Mana > RMANA + EMANA
                && !ObjectManager.Player.UnderTurret(true)
                && ObjectManager.Player.Position.LSExtend(Game.CursorPos, E.Range).LSCountEnemiesInRange(700) < 3)
            {
                if (t.LSIsValidTarget() && player.Mana > QMANA + EMANA + WMANA && t.Position.LSDistance(Game.CursorPos) + 300 < t.Position.LSDistance(player.Position))
                {
                    E.Cast(player.Position.LSExtend(Game.CursorPos, E.Range), true);
                }
            }
            else if (t.LSIsValidTarget() && GetEdmg(t) + GetWdmg(t) > t.Health)
            {
                E.Cast(player.Position.LSExtend(t.Position, E.Range), true);
            }
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            foreach (var t in LXOrbwalker.AllEnemys.Where(t => RMissile != null && RMissile.IsValid && t.LSIsValidTarget() && RMissile.Position.LSDistance(Prediction.GetPrediction(t, R.Delay).CastPosition) < 350 && RMissile.Position.LSDistance(t.ServerPosition) < 350))
            {

                var comboDmg = GetRdmg(t) + GetWdmg(t);
                if (Q.IsReady())
                    comboDmg += GetQdmg(t);
                if (E.IsReady())
                    comboDmg += GetEdmg(t);
                if (t.Health < comboDmg)
                    R.Cast();



            }

            if (player.Health < player.LSCountEnemiesInRange(600) * player.Level * 15)
            {
                R.Cast();
            }
        }

        private void SetMana()
        {
            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = QMANA - ObjectManager.Player.Level * 2;
            else
                RMANA = R.Instance.SData.Mana; ;

            if (ObjectManager.Player.Health < ObjectManager.Player.MaxHealth * 0.2)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
            }
        }

        public override void useSpells()
        {
            SetMana();
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 700);
            Q1 = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 330f);
            R = new Spell(SpellSlot.R, 1200f);

            Q.SetSkillshot(0.25f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.5f, 150f, 1000f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(2.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.6f, 375f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private double GetQdmg(Obj_AI_Base t)
        {
            double dmg = 90 + (30 * Q.Level) + player.FlatMagicDamageMod * 0.8;
            return player.CalcDamage(t, DamageType.Magical, dmg);
        }
        private double GetEdmg(Obj_AI_Base t)
        {
            double dmg = 20 + (30 * E.Level) + (player.FlatMagicDamageMod * 0.2);
            return player.CalcDamage(t, DamageType.Magical, dmg);
        }
        private double GetWdmg(Obj_AI_Base t)
        {
            if (t.Health < t.MaxHealth * 0.3)
            {
                double hp = t.MaxHealth - t.Health;
                double dmg = ((player.FlatMagicDamageMod / 45) + 5) * 0.01;
                double dmg2 = hp * dmg;
                return player.CalcDamage(t, DamageType.Magical, dmg2);

            }
            else
                return 0;

        }

        private double GetRdmg(Obj_AI_Base t)
        {
            double dmg = 50 + (150 * R.Level) + player.FlatMagicDamageMod * 1.3;
            return player.CalcDamage(t, DamageType.Magical, dmg);
        }

        public override void farm()
        {
            base.farm();
            if (Q.IsReady())
            {
                var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q1.Range, MinionTypes.All);
                var Qfarm = Q.GetLineFarmLocation(allMinionsQ, 100);
                if (Qfarm.MinionsHit > 5 && Q1.IsReady())
                    Q.Cast(Qfarm.Position);
            }
        }
    }
}
