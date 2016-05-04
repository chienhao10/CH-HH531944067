using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KoreanAIO.Managers;
using KoreanAIO.Model;
using SharpDX;

namespace KoreanAIO.Champions
{
    public class Diana : ChampionBase
    {
        private const int QHeroWidth = 100;
        private const string QMissileName = "dianaarcthrow";
        public Vector3 LastMissileVector = Vector3.Zero;
        public int LastMissileVectorTime;
        public bool MissileIsValid;
        public Vector3 QEndPosition = Vector3.Zero;
        public MissileClient QMissile;

        public Diana()
        {
            Q = new SpellBase(SpellSlot.Q, SpellType.Circular, 825)
            {
                Width = 185,
                CastDelay = 250,
                Speed = 1640
            };
            W = new SpellBase(SpellSlot.W, SpellType.Self, 250);
            E = new SpellBase(SpellSlot.E, SpellType.Self, 450)
            {
                CastDelay = 250
            };
            R = new SpellBase(SpellSlot.R, SpellType.Targeted, 825)
            {
                Speed = 2500
            };
            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    switch (args.Slot)
                    {
                        case SpellSlot.Q:
                            Q.LastCastTime = Core.GameTickCount;
                            QEndPosition = args.End; // +(args.End - sender.ServerPosition).Normalized() * Q.Width / 2;
                            break;
                        case SpellSlot.R:
                            R.LastCastTime = Core.GameTickCount;
                            break;
                    }
                }
            };
            GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (missile != null && missile.SpellCaster != null && missile.SpellCaster.IsMe)
                {
                    if (missile.SData.Name.Equals(QMissileName))
                    {
                        QMissile = missile;
                    }
                }
            };
            GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (missile != null && missile.SpellCaster != null && missile.SpellCaster.IsMe)
                {
                    if (missile.SData.Name.Equals(QMissileName))
                    {
                        QMissile = null;
                    }
                }
            };
            Gapcloser.OnGapcloser += delegate (AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
            {
                if (sender.IsEnemy)
                {
                    if (AutomaticMenu.CheckBox("Gapcloser"))
                    {
                        E.Cast(sender);
                    }
                }
            };
            Interrupter.OnInterruptableSpell +=
                delegate (Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
                {
                    if (sender.IsEnemy)
                    {
                        if (AutomaticMenu.CheckBox("Interrupter"))
                        {
                            E.Cast(sender);
                        }
                    }
                };
            Dash.OnDash += delegate (Obj_AI_Base sender, Dash.DashEventArgs args)
            {
                if (sender.IsEnemy)
                {
                    if (AutomaticMenu.CheckBox("Gapcloser"))
                    {
                        E.Cast(sender);
                    }
                }
            };
            Q.AddConfigurableHitChancePercent();
            MenuManager.AddSubMenu("Combo");
            {
                ComboMenu.AddValue("Q", new CheckBox("Use Q"));
                ComboMenu.AddValue("W", new CheckBox("Use W"));
                ComboMenu.AddValue("QR", new CheckBox("Use QR on minion to gapclose"));
                ComboMenu.AddValue("Ignite", new CheckBox("Use Ignite if target is killable", false));
                ComboMenu.AddStringList("E", "Use E", new[] { "Never", "Smartly", "Always" }, 1);
                ComboMenu.AddStringList("R", "Use R", new[] { "Never", "Smartly", "Always" }, 1);
                ComboMenu.AddValue("2ndR", new CheckBox("Use always second r", false));
            }

            MenuManager.AddSubMenu("Harass");
            {
                HarassMenu.AddValue("Q", new CheckBox("Use Q"));
                HarassMenu.AddValue("W", new CheckBox("Use W"));
                HarassMenu.AddValue("E", new CheckBox("Use E", false));
                HarassMenu.AddValue("ManaPercent", new Slider("Minimum Mana Percent", 25));
            }

            MenuManager.AddSubMenu("Clear");
            {
                ClearMenu.AddValue("LaneClear", new GroupLabel("LaneClear"));
                {
                    ClearMenu.AddValue("LaneClear.Q", new Slider("Use Q if hit >= {0}", 4, 0, 10));
                    ClearMenu.AddValue("LaneClear.W", new Slider("Use W if hit >= {0}", 3, 0, 10));
                    ClearMenu.AddValue("LaneClear.ManaPercent", new Slider("Minimum Mana Percent", 50));
                }
                ClearMenu.AddValue("LastHit", new GroupLabel("LastHit"));
                {
                    ClearMenu.AddStringList("LastHit.Q", "Use Q", new[] { "Never", "Smartly", "Always" }, 1);
                    ClearMenu.AddValue("LastHit.ManaPercent", new Slider("Minimum Mana Percent", 50));
                }
                ClearMenu.AddValue("JungleClear", new GroupLabel("JungleClear"));
                {
                    ClearMenu.AddValue("JungleClear.Q", new CheckBox("Use Q"));
                    ClearMenu.AddValue("JungleClear.W", new CheckBox("Use W"));
                    ClearMenu.AddValue("JungleClear.R", new CheckBox("Use R"));
                    ClearMenu.AddValue("JungleClear.ManaPercent", new Slider("Minimum Mana Percent", 20));
                }
            }

            MenuManager.AddKillStealMenu();
            {
                KillStealMenu.AddValue("Q", new CheckBox("Use Q"));
                KillStealMenu.AddValue("W", new CheckBox("Use W"));
                KillStealMenu.AddValue("R", new CheckBox("Use R"));
            }

            MenuManager.AddSubMenu("Automatic");
            {
                AutomaticMenu.AddValue("Gapcloser", new CheckBox("Use E on hero gapclosing / dashing"));
                AutomaticMenu.AddValue("Interrupter", new CheckBox("Use E on channeling spells"));
            }
            MenuManager.AddDrawingsMenu();
            {
                Q.AddDrawings();
                W.AddDrawings();
                E.AddDrawings(false);
                R.AddDrawings();
            }
        }

        protected override void PermaActive()
        {
            Range = Q.Range + Q.Width;
            Target = TargetSelector.GetTarget(UnitManager.ValidEnemyHeroesInRange, DamageType.Magical);
            MissileIsValid = QMissile != null && QMissile.IsValid && !QMissile.IsDead;
            if (QMissile != null && MissileIsValid)
            {
                Q.SourceObject = QMissile;
            }
            else
            {
                Q.SourceObject = MyHero;
            }
            if (MissileIsValid)
            {
                if (Core.GameTickCount - LastMissileVectorTime > 0)
                {
                    if (!LastMissileVector.IsZero)
                    {
                        Q.Speed =
                            (int)
                                (1000 * Q.Source.Distance(LastMissileVector) / (Core.GameTickCount - LastMissileVectorTime));
                    }
                    LastMissileVector = new Vector3(Q.Source.Position.X, Q.Source.Position.Y, Q.Source.Position.Z);
                    LastMissileVectorTime = Core.GameTickCount;
                }
                Q.CastDelay = 0;
                Q.Range = (int)(825 + MyHero.GetDistance(Q.Source));
            }
            else
            {
                Q.CastDelay = 250;
                Q.Speed = 1640;
                Q.Range = 825;
            }
            base.PermaActive();
        }

        protected override void KillSteal(Menu menu)
        {
            foreach (var enemy in UnitManager.ValidEnemyHeroesInRange.Where(h => h.HealthPercent <= 40f))
            {
                var result = GetBestCombo(enemy);
                if (menu.CheckBox("Q") && (result.Q || Q.IsKillable(enemy)))
                {
                    CastQ(enemy);
                }
                if (menu.CheckBox("W") && (result.W || W.IsKillable(enemy)))
                {
                    W.Cast(enemy);
                }
                if (menu.CheckBox("R") && (result.R || R.IsKillable(enemy)))
                {
                    CastR(enemy);
                }
            }
            base.KillSteal(menu);
        }

        protected void GapClose(Obj_AI_Base target)
        {
            if (target != null && Q.IsReady && R.IsReady && !R.IsInRange(target))
            {
                var enemyNear =
                    UnitManager.ValidEnemyHeroesInRange.Where(
                        h =>
                            !target.IdEquals(h) && !Q.IsKillable(h) &&
                            h.GetDistanceSqr(target) < MyHero.GetDistanceSqr(target))
                        .OrderBy(h => h.GetDistanceSqr(target))
                        .FirstOrDefault();
                if (enemyNear != null)
                {
                    CastQ(enemyNear);
                    CastR(enemyNear, false);
                    return;
                }
                var minionNear =
                    R.EnemyMinions.Where(
                        h =>
                            !target.IdEquals(h) && !Q.IsKillable(h) &&
                            h.GetDistanceSqr(target) < MyHero.GetDistanceSqr(target))
                        .OrderBy(h => h.GetDistanceSqr(target))
                        .FirstOrDefault();
                if (minionNear != null)
                {
                    CastQ(minionNear);
                    CastR(minionNear, false);
                }
            }
        }

        protected override void Combo(Menu menu)
        {
            if (Target != null)
            {
                var bestCombo = GetBestCombo(Target);
                if (menu.CheckBox("Ignite") && Ignite.IsReady && bestCombo.IsKillable)
                {
                    Ignite.Cast(Target);
                }
                if (menu.Slider("E") > 0 && !MyHero.InAutoAttackRange(Target))
                {
                    if (menu.Slider("E") != 2 || bestCombo.IsKillable)
                    {
                        var path = Prediction.Position.GetRealPath(Target);
                        if (path.Any() && path.Last().Distance(MyHero, true) > Target.GetDistanceSqr(MyHero))
                        {
                            E.Cast(Target);
                        }
                    }
                }
                if (menu.CheckBox("W"))
                {
                    W.Cast(Target);
                }
                if (menu.CheckBox("Q"))
                {
                    CastQ(Target);
                }
                if (menu.CheckBox("QR"))
                {
                    if (bestCombo.IsKillable)
                    {
                        GapClose(Target);
                    }
                }
                if (menu.Slider("R") > 0)
                {
                    switch (menu.Slider("R"))
                    {
                        case 1:
                            CastR(Target);
                            break;
                        case 2:
                            R.Cast(Target);
                            break;
                    }
                }
            }
            base.Combo(menu);
        }

        protected override void Harass(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("ManaPercent"))
            {
                if (Target != null)
                {
                    if (menu.CheckBox("Q"))
                    {
                        CastQ(Target);
                    }
                    if (menu.CheckBox("W"))
                    {
                        W.Cast(Target);
                    }
                    if (menu.CheckBox("E"))
                    {
                        E.Cast(Target);
                    }
                }
            }
            base.Harass(menu);
        }

        protected override void LaneClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LaneClear.ManaPercent"))
            {
                Q.LaneClear(true, menu.Slider("LaneClear.Q"));
                W.LaneClear(true, menu.Slider("LaneClear.W"));
            }
            base.LaneClear(menu);
        }

        protected override void LastHit(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LastHit.ManaPercent"))
            {
                Q.LastHit((LastHitType)menu.Slider("LastHit.Q"));
            }
            base.LastHit(menu);
        }

        protected override void JungleClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("JungleClear.ManaPercent"))
            {
                if (menu.CheckBox("JungleClear.Q"))
                {
                    Q.JungleClear();
                }
                if (menu.CheckBox("JungleClear.W"))
                {
                    W.JungleClear();
                }
                if (menu.CheckBox("JungleClear.R"))
                {
                    var minion = R.JungleClear(false);
                    if (minion != null)
                    {
                        CastR(minion);
                    }
                }
            }
            base.JungleClear(menu);
        }

        protected void CastQ(Obj_AI_Base target)
        {
            if (Q.IsReady && target != null)
            {
                Q.Cast(target,
                    new CustomSettings { Width = target.Type == GameObjectType.AIHeroClient ? QHeroWidth : Q.Width });
            }
        }

        protected void CastR(Obj_AI_Base target, bool checkDamage = true)
        {
            if (target != null && R.IsReady && R.IsInRange(target))
            {
                var bestCombo = GetBestCombo(target);
                if (!TargetHaveQ(target))
                {
                    if (Q.IsReady)
                    {
                        CastQ(target);
                        return;
                    }
                    if (Core.GameTickCount - Q.LastCastTime <= 250 || Q.Instance.Cooldown < 3)
                    {
                        return;
                    }
                    if (MissileIsValid && MyHero.GetDistanceSqr(QMissile) <= MyHero.GetDistanceSqr(target))
                    {
                        var timeToArriveQ = QMissile.GetDistance(target) / Q.Speed;
                        var timeToArriveR = MyHero.GetDistance(target) / R.Speed + Game.Ping / 2000f;
                        if (timeToArriveR <= timeToArriveQ)
                        {
                            var pred = Q.GetPrediction(target);
                            if (pred.HitChancePercent >= Q.HitChancePercent &&
                                pred.CastPosition.To2D()
                                    .Distance(QMissile.Position.To2D(), QEndPosition.To2D(), true, true) <=
                                (Q.Radius + target.BoundingRadius).Pow())
                            {
                                //Chat.Print("Misaya combo");
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (ComboMenu.CheckBox("2ndR"))
                    {
                        //Chat.Print("Use always second r");
                    }
                    else if (!bestCombo.IsKillable)
                    {
                        return;
                    }
                    else if (bestCombo.R && checkDamage && !MyHero.InAutoAttackRange(target))
                    {
                        //Chat.Print("Killable");
                    }
                    else
                    {
                        return;
                    }
                }
                R.Cast(target);
            }
        }

        protected bool TargetHaveQ(Obj_AI_Base target)
        {
            return target.TargetHaveBuff("dianamoonlight");
        }

        public override float GetSpellDamage(SpellSlot slot, Obj_AI_Base target)
        {
            if (target != null)
            {
                var level = slot.GetSpellDataInst().Level;
                switch (slot)
                {
                    case SpellSlot.Q:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            35f * level + 30f + 0.7f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.W:
                        return 3 * MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            12f * level + 10f + 0.2f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.E:
                        return 0f;
                    case SpellSlot.R:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            60f * level + 40f + 0.6f * MyHero.FlatMagicDamageMod);
                }
            }
            return base.GetSpellDamage(slot, target);
        }
    }
}