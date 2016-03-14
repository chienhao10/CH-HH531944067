using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using WuAIO.Bases;
using WuAIO.Managers;
using WuAIO.Extensions;
using WuAIO.Utilities;
using Circle = EloBuddy.SDK.Rendering.Circle;
using ScalingTypes = WuAIO.Managers.DamageManager.ScalingTypes;

namespace WuAIO
{
    class Azir : HeroBase
    {
        readonly AIHeroClient Player = EloBuddy.Player.Instance;

        bool WhyIDidThatAddonInsec;
        float LastQTime;
        Obj_AI_Minion InsecSoldier;

        readonly Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, 825, SkillShotType.Linear, 250, 1000, 70);
        readonly Spell.Skillshot W = new Spell.Skillshot(SpellSlot.W, 450, SkillShotType.Circular);
        readonly Spell.Skillshot E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Linear, 250, 1600, 100);
        readonly Spell.Skillshot R = new Spell.Skillshot(SpellSlot.R, 300, SkillShotType.Linear, 500, 1000, 532);

        public override void CreateMenu()
        {
            base.CreateMenu();

            var menu = MenuManager.AddSubMenu("线圈");
            {
                menu.NewCheckbox("disable", "关闭线圈", false);
                menu.NewCheckbox("damageindicator", "伤害显示");
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("e", "E");
                menu.NewCheckbox("r", "R");
            }

            menu = MenuManager.AddSubMenu("连招");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewSlider("q.hitchance%", "Q 命中率%", 65, 1, 100);
                menu.NewCheckbox("w", "W", true, true);
                menu.NewCheckbox("e", "E");
                menu.NewCheckbox("r", "R");
            }

            menu = MenuManager.AddSubMenu("骚扰");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewSlider("q.hitchance%", "Q 骚扰%", 65, 1, 100);
                menu.NewCheckbox("w", "W", true, true);
                menu.NewCheckbox("e", "E");
                menu.NewSlider("mana%", "最低蓝消耗%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("清线");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewSlider("mana%", "最低蓝消耗%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("尾兵");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewSlider("mana%", "最低蓝消耗%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("清野");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("e", "E");
                menu.NewSlider("mana%", "最低蓝消耗%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("杂项");
            {
                menu.NewCheckbox("ks", "抢头");
                menu.NewCheckbox("ks.r", "用R抢头 ?");
                menu.NewCheckbox("interrupter", "技能打断");
                menu.NewKeybind("insec.normal", "普通漂移推人", false, KeyBind.BindTypes.HoldActive, 'J');
                menu.NewKeybind("insec.godlike", "神级漂移推人", false, KeyBind.BindTypes.HoldActive, 'G');
            }
        }

        public override void CreateVariables()
        {
            new SkinManager(2);

            Q.MinimumHitChance = HitChance.Medium;
            E.MinimumHitChance = HitChance.High;
            R.MinimumHitChance = HitChance.High;

            Q.AllowedCollisionCount = int.MaxValue;
            R.AllowedCollisionCount = int.MaxValue;

            damageManager = new DamageManager
            (
                new List<Bases.Damage>()
                {
                    new Bases.Damage
                    (
                        Q, DamageType.Magical, new float[] { 0, 65, 85, 105, 125, 145 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }, ScalingTypes.AP)
                        }
                    ),

                    new Bases.Damage
                    (
                        W, DamageType.Magical, new float[] { 0, 0, 0, 0, 0, 0 },
                        
                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f }, ScalingTypes.AP)
                        }
                    ),

                    new Bases.Damage
                    (
                        E, DamageType.Magical, new float[] { 0, 60, 90, 120, 150, 180 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f }, ScalingTypes.AP)
                        }
                    ),

                    new Bases.Damage
                    (
                        R, DamageType.Magical, new float[] { 0, 150, 225, 300 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.6f, 0.6f, 0.6f }, ScalingTypes.AP)
                        }
                    )
                }
            );
        }

        public override void PermaActive()
        {
            base.PermaActive();

            if (!R.IsReady() || Player.CountEnemiesInRange(1100) == 0) { WhyIDidThatAddonInsec = false; }

            R.Width = 133 * (3 + R.Level);

            if (R.IsReady() && Game.Time - LastQTime > 0.1f && Game.Time - LastQTime < 1)
                Player.Spellbook.CastSpell(SpellSlot.R, Vectors.CorrectSpellRange(Game.CursorPos, R.Range));

            if (WhyIDidThatAddonInsec) { Orbwalker.DisableAttacking = true; Orbwalker.DisableMovement = true; }
            else { Orbwalker.DisableAttacking = false; Orbwalker.DisableMovement = false; }

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null) return;

            if (R.IsReady() && Player.Mana >= GetFuckingInsecMana(Target))
            {
                if (misc.IsActive("insec.normal")) Insec(Target);
                else if (misc.IsActive("insec.godlike")) WhyInsec(Target);
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || draw.IsActive("disable")) return;

            if (draw.IsActive("q"))
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Position);

            if (draw.IsActive("e"))
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Position);

            if (draw.IsActive("r"))
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Position);

            return;
        }

        public override void KS()
        {
            if (!misc.IsActive("ks") || !EntityManager.Heroes.Enemies.Any(it => Q.IsInRange(it))) return;
            
            if (Orbwalker.ValidAzirSoldiers.Any())
            {
                if (Q.IsReady())
                {
                    var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(Q.Range) && damageManager.SpellDamage(it, SpellSlot.Q) >= it.Health);
                    if (bye != null) Q.HitChanceCast(bye, 70);
                }

                if (E.IsReady())
                {
                    var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(E.Range) && damageManager.SpellDamage(it, SpellSlot.E) >= it.Health);
                    if (bye != null) { CastE(bye); return; }
                }
            }
            
            else if (W.IsReady())
            {
                if (Q.IsReady())
                {
                    var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(Q.Range - 150) && damageManager.SpellDamage(it, SpellSlot.Q) + damageManager.SpellDamage(it, SpellSlot.W, new int[] { 50, 55, 60, 65, 70, 75, 80, 85, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180 }[Player.Level]) >= it.Health);
                    if (bye != null)
                    {
                        if (W.Cast(Vectors.CorrectSpellRange(bye.ServerPosition, W.Range)))
                            Core.DelayAction(() => Q.HitChanceCast(bye, 70), 250);
                    }
                }
            }

            if (misc.IsActive("ks.r") && R.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(R.Range) && damageManager.SpellDamage(it, SpellSlot.R) >= it.Health);
                if (bye != null) { R.HitChanceCast(bye, 70); return; }
            }

            var WEnemy = EntityManager.Heroes.Enemies.FirstOrDefault(it => Orbwalker.ValidAzirSoldiers.Any(enemy => enemy.Distance(it) <= 275));

            if (WEnemy != null && damageManager.SpellDamage(WEnemy, SpellSlot.W, new int[] { 50, 55, 60, 65, 70, 75, 80, 85, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180 }[Player.Level]) >= WEnemy.Health) EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, WEnemy);

            return;
        }

        public override void Flee()
        {
            if (W.IsReady() && Q.IsReady() && E.IsReady())
            {
                var WPos = Vectors.CorrectSpellRange(Game.CursorPos, W.Range);

                if (W.Cast(WPos))
                {
                    Core.DelayAction(delegate
                    {
                        if (E.Cast(Vectors.CorrectSpellRange(Game.CursorPos, E.Range)))
                        {
                            Core.DelayAction(delegate
                            {
                                Q.Cast(Vectors.CorrectSpellRange(Game.CursorPos, Q.Range));
                            }, 100);
                        }

                        //int EDelay = (int)((Player.Distance(WPos) - 150) / 8 * 5);
                    }, 150);
                }

                return;
            }

            else if (W.IsReady())
            {
                if (W.Cast(Vectors.CorrectSpellRange(Game.CursorPos, W.Range)))
                    Core.DelayAction(() => E.Cast(Vectors.CorrectSpellRange(Game.CursorPos, E.Range)), 250);
            }

            else if (Orbwalker.ValidAzirSoldiers.Any(it => it.Distance(Game.CursorPos) <= 150))
                E.Cast(Vectors.CorrectSpellRange(Game.CursorPos, E.Range));

            else if (Orbwalker.ValidAzirSoldiers.Any() && Q.IsReady())
            {
                if (Q.Cast(Vectors.CorrectSpellRange(Game.CursorPos, Q.Range)))
                    Core.DelayAction(() => E.Cast(Vectors.CorrectSpellRange(Game.CursorPos, (E.Range))), 100);
            }

            return;
        }

        public override void Combo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (Target == null || !Target.IsValidTarget()) return;

            if (R.IsReady() && R.IsInRange(Target) && combo.IsActive("r") && damageManager.SpellDamage(Target, SpellSlot.R) >= Target.Health) R.HitChanceCast(Target, 70);

            if (W.IsReady() && (W.IsInRange(Target) || (Q.IsReady() && Q.IsInRange(Target))) && combo.IsActive("w"))
            {
                var WPos = Prediction.Position.PredictUnitPosition(Target, 1000).To3D();
                W.Cast(Vectors.CorrectSpellRange(WPos, W.Range));
            }

            else if (Orbwalker.ValidAzirSoldiers.Any())
            {
                if (Q.IsReady() && combo.IsActive("q") && Q.IsInRange(Target)) Q.HitChanceCast(Target, combo.Value("q.hitchance%"));
                if (E.IsReady() && combo.IsActive("e")) CastE(Target);
            }

            return;
        }

        public override void Harass()
        {
            var Target = TargetSelector.GetTarget(900, DamageType.Physical);

            if (Target == null || !Target.IsValidTarget() || Player.ManaPercent < harass.Value("mana%")) return;

            if (W.IsReady() && (W.IsInRange(Target) || (Q.IsReady() && Q.IsInRange(Target))) && combo.IsActive("w"))
            {
                var WPos = Prediction.Position.PredictUnitPosition(Target, 1000).To3D();
                W.Cast(Vectors.CorrectSpellRange(WPos, W.Range));
            }

            else if (Orbwalker.ValidAzirSoldiers.Any())
            {
                if (Q.IsReady() && combo.IsActive("q") && Q.IsInRange(Target)) Q.HitChanceCast(Target, combo.Value("q.hitchance%"));
                if (E.IsReady() && combo.IsActive("e")) CastE(Target);
            }

            return;
        }

        public override void LaneClear()
        {
            if (Player.ManaPercent < laneclear.Value("mana%")) return;

            if (Orbwalker.ValidAzirSoldiers.Any(it => it.Distance(Player) <= 900))
            {
                if (Q.IsReady() && laneclear.IsActive("q"))
                {
                    var QPos = Vectors.BestCircularFarmLocation(250, (int)Q.Range);
                    if (QPos != default(Vector3)) Q.Cast(QPos);
                }
            }

            else if (W.IsReady() && laneclear.IsActive("w"))
            {
                var WPos = Vectors.BestCircularFarmLocation(250, (int)W.Range);
                if (WPos != default(Vector3)) W.Cast(WPos);
            }

            return;
        }

        public override void JungleClear()
        {
            if (Player.ManaPercent < jungleclear.Value("mana%")) return;

            if (W.IsReady() && jungleclear.IsActive("w"))
            {
                var WMinion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, W.Range).FirstOrDefault();
                if (WMinion != null) { var WPos = Player.Position.Extend(WMinion, Player.Distance(WMinion) / 2).To3D(); W.Cast(WPos); }
            }

            if (E.IsReady() && jungleclear.IsActive("e"))
            {
                var EMinions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, E.Range);
                if (EMinions.Any()) CastE(EMinions);
            }

            if (Q.IsReady() && jungleclear.IsActive("q"))
            {
                var QMinion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Q.Range).FirstOrDefault();

                if (QMinion != null) Q.HitChanceCast(QMinion, 40);
            }

            return;
        }

        public override void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (Player.IsDead) return;

            if (Orbwalker.ValidAzirSoldiers.Any() && Q.IsReady() && target.IsValidTarget(Q.Range) && damageManager.SpellDamage(target, SpellSlot.Q) >= target.Health)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Player.ManaPercent >= laneclear.Value("mana%") && laneclear.IsActive("q")) { Q.Cast(target.ServerPosition); return; }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && Player.ManaPercent >= lasthit.Value("mana%") && lasthit.IsActive("q")) { Q.Cast(target.ServerPosition); return; }
            }

            return;
        }

        public override void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (Player.IsDead || !sender.IsEnemy || !misc.IsActive("interrupter")) return;

            if (R.IsReady() && e.DangerLevel == DangerLevel.High && sender.IsValidTarget(R.Range))
                R.HitChanceCast(sender, HitChance.Medium);

            return;
        }

        //------------------------------------|| Methods ||--------------------------------------

        //---------------------------------------Insec()-------------------------------------------------------

        void Insec(Obj_AI_Base target)
        {
            //Back distance = 300

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

            //Normal Insec

            if (Player.Distance(target) <= W.Range + Q.Range - 300)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

                if (Player.Distance(target) < 220)
                {
                    var tower = EntityManager.Turrets.Allies.FirstOrDefault(it => it.IsValidTarget(1000));

                    if (tower != null)
                    {
                        if (EloBuddy.Player.CastSpell(SpellSlot.R, Vectors.CorrectSpellRange(tower.Position, R.Range))) return;
                    }

                    if (EloBuddy.Player.CastSpell(SpellSlot.R, Vectors.CorrectSpellRange(Game.CursorPos, R.Range))) return;
                }

                else if (E.IsReady())
                {
                    if (Orbwalker.ValidAzirSoldiers.Any(it => it.Distance(Player) <= 900))
                    {
                        var ESoldier = Orbwalker.ValidAzirSoldiers.FirstOrDefault(it => it.Distance(target) <= 200);

                        if (ESoldier != null) E.Cast(Vectors.CorrectSpellRange(ESoldier.Position, E.Range));

                        else if (Q.IsReady())
                        {
                            Q.HitChanceCast(target, 60);
                        }
                    }

                    else if (W.IsReady())
                    {
                        var WPos = Prediction.Position.PredictUnitPosition(target, 1000).To3D();
                        W.Cast(Vectors.CorrectSpellRange(WPos, W.Range));
                        return;
                    }
                }
            }

            return;
        }

        //---------------------------------------WhyInsec()----------------------------------------------------

        void WhyInsec(Obj_AI_Base target)
        {
            //Back distance = 300

            //Why I did that

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

            var targetpos = Prediction.Position.PredictUnitPosition(target, 500).To3D();

            if (!WhyIDidThatAddonInsec && Orbwalker.ValidAzirSoldiers.Any(it => it.Distance(target) >= E.Width + target.BoundingRadius && it.Distance(target) <= (R.Width / 2) - 20))
            {
                WhyIDidThatAddonInsec = true;

                Core.DelayAction(() => WhyIDidThatAddonInsec = false, 3000);

                if (E.IsReady())
                {
                    InsecSoldier = Orbwalker.ValidAzirSoldiers.Where(it => it.Distance(target) <= (R.Width / 2) - 20).OrderByDescending(it => it.Distance(target)).First();

                    var rectangle = new Geometry.Polygon.Rectangle(Player.Position, InsecSoldier.Position, E.Width + target.BoundingRadius);

                    if (!rectangle.IsInside(targetpos) && InsecSoldier.Distance(target) <= (R.Width / 2) - 50)
                    {
                        var EDelay = (int)((((Player.Distance(InsecSoldier) - 100) / 8) * 5));

                        if (E.Cast(Vectors.CorrectSpellRange(InsecSoldier.Position, E.Range)))
                        {
                            //Delayed insec

                            Core.DelayAction(delegate
                            {
                                if (Player.Spellbook.CastSpell(SpellSlot.Q, Vectors.CorrectSpellRange(Game.CursorPos, Q.Range)))
                                {
                                    LastQTime = Game.Time;
                                }

                                else WhyIDidThatAddonInsec = false;
                            }, EDelay);
                        }
                        else WhyIDidThatAddonInsec = false;
                    }
                    else { WhyIDidThatAddonInsec = false; }
                }
                else WhyIDidThatAddonInsec = false;
            }

            else if (!WhyIDidThatAddonInsec && W.IsReady() && E.IsReady() && Q.IsReady() && W.IsInRange(target))
            {
                var rectangle = new Geometry.Polygon.Rectangle(Player.Position, targetpos, 2 * (target.BoundingRadius + E.Width + 20));
                //rectangle.Draw(System.Drawing.Color.Yellow);

                var circle = new Geometry.Polygon.Circle(targetpos, (R.Width - 20)/2);
                //circle.Draw(System.Drawing.Color.Red);

                var point = circle.Points.Where(it => !NavMesh.GetCollisionFlags(it).HasFlag(CollisionFlags.Wall) && W.IsInRange(it.To3D()) && !rectangle.IsInside(it)).OrderByDescending(it => it.Distance(Player)).FirstOrDefault();

                if (point != null && W.Cast(point.To3D()))
                {
                    WhyIDidThatAddonInsec = true;

                    Core.DelayAction(() => WhyIDidThatAddonInsec = false, 3000);

                    Core.DelayAction(delegate
                    {
                        InsecSoldier = Orbwalker.ValidAzirSoldiers.Where(it => it.Distance(target) <= (R.Width / 2) - 20).OrderByDescending(it => it.Distance(target)).First();

                        var EDelay = (int)((((Player.Distance(InsecSoldier) - 100) / 8) * 5));

                        if (E.Cast(Vectors.CorrectSpellRange(InsecSoldier.Position, E.Range)))
                        {
                            //Delayed insec

                            Core.DelayAction(delegate
                            {
                                if (Player.Spellbook.CastSpell(SpellSlot.Q, Vectors.CorrectSpellRange(Game.CursorPos, Q.Range)))
                                {
                                    LastQTime = Game.Time;
                                }

                                else WhyIDidThatAddonInsec = false;
                            }, EDelay);
                        }
                        else WhyIDidThatAddonInsec = false;

                    }, 350);
                }

            }

            return;
        }

        //------------------------------------CastE(Obj_AI_Base target)----------------------------------------

        void CastE(Obj_AI_Base target)
        {
            foreach (var soldier in Orbwalker.ValidAzirSoldiers)
            {
                var rectangle = new Geometry.Polygon.Rectangle(Player.Position, soldier.Position, 90);

                if (rectangle.IsInside(target))
                {
                    if (E.Cast(target)) return;
                }
            }

            return;
        }

        //----------------------------------CastE(List<Obj_AI_Base> targets)-----------------------------------

        void CastE(IEnumerable<Obj_AI_Base> targets)
        {
            var rectangles = new List<Geometry.Polygon.Rectangle>();

            foreach (var soldier in Orbwalker.ValidAzirSoldiers)
            {
                rectangles.Add(new Geometry.Polygon.Rectangle(Player.Position, soldier.Position, 90));
            }

            if (targets.Any(it => rectangles.Any(rectangle => rectangle.IsInside(it)))) E.Cast(Player.Position);

            return;
        }

        //-----------------------------------GetFuckingInsecMana()---------------------------------------------

        int GetFuckingInsecMana(AIHeroClient target)
        {
            var QMana = 70;
            var WMana = 40 + 2;
            var EMana = 60;
            var RMana = 100;

            if (Player.Distance(target) < 219)
                return RMana;

            if (E.IsReady() && Orbwalker.ValidAzirSoldiers.Any(it => it.Distance(target) <= 200))
                return RMana + EMana;

            return QMana + WMana + EMana + RMana;
        }
    }
}

/*
        //-------------------------------------------Game_OnTick----------------------------------------------

        static void Game_OnTick(EventArgs args)
        {
            if (Menu["HU3"].Cast<KeyBind>().CurrentValue)
            {
                switch (Menu["HU3Mode"].Cast<Slider>().CurrentValue)
                {
                    case 1:
                        EloBuddy.Player.DoEmote(Emote.Joke);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    case 2:
                        EloBuddy.Player.DoEmote(Emote.Taunt);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    case 3:
                        EloBuddy.Player.DoEmote(Emote.Dance);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    case 4:
                        EloBuddy.Player.DoEmote(Emote.Laugh);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    default:
                        break;
                }
                
            }
        }

    }//Class End
}
*/
