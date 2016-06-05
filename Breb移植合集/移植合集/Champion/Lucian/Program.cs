using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Geometry = LeagueSharp.Common.Geometry;
using Prediction = LeagueSharp.Common.Prediction;

namespace LCS_Lucian
{
    internal class Program
    {
        public static Menu Config, comboMenu, harassMenu, clearMenu, jungleMenu, killStealMenu, miscMenu, drawMenu;

        public static bool UltActive
        {
            get { return ObjectManager.Player.HasBuff("LucianR"); }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Lucian")
            {
                return;
            }

            LucianSpells.Init();
            LucianMenu.MenuInit();

            Chat.Print(
                "<font color='#99FFFF'>LCS Series - Lucian loaded! </font><font color='#99FF00'> Be Rekkles ! Its Possible. Enjoy GODSPEED Spell + Passive Usage </font>");
            Chat.Print("<font color='##FFCC00'>LCS Series totally improved LCS player style.</font>");

            Game.OnUpdate += LucianOnUpdate;
            Obj_AI_Base.OnSpellCast += LucianOnDoCast;
            Drawing.OnDraw += LucianOnDraw;

            Config = LucianMenu.Config;
            comboMenu = LucianMenu.comboMenu;
            harassMenu = LucianMenu.harassMenu;
            clearMenu = LucianMenu.clearMenu;
            jungleMenu = LucianMenu.jungleMenu;
            killStealMenu = LucianMenu.killStealMenu;
            miscMenu = LucianMenu.miscMenu;
            drawMenu = LucianMenu.drawMenu;
        }

        private static void ECast(AIHeroClient enemy)
        {
            var range = Orbwalking.GetRealAutoAttackRange(enemy);
            var path = Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.LSTo2D(),
                Prediction.GetPrediction(enemy, 0.25f).UnitPosition.LSTo2D(), LucianSpells.E.Range, range);

            if (path.Count() > 0)
            {
                var epos = path.MinOrDefault(x => x.LSDistance(Game.CursorPos));
                if (epos.To3D().UnderTurret(true) || epos.To3D().LSIsWall())
                {
                    return;
                }

                if (epos.To3D().CountEnemiesInRange(LucianSpells.E.Range - 100) > 0)
                {
                    return;
                }
                LucianSpells.E.Cast(epos);
            }
            if (path.Count() == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.LSExtend(enemy.ServerPosition, -LucianSpells.E.Range);
                if (epos.UnderTurret(true) || epos.LSIsWall())
                {
                    return;
                }

                // no intersection or target to close
                LucianSpells.E.Cast(ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition,
                    -LucianSpells.E.Range));
            }
        }

        private static void LucianOnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid)
            {
                if (getCheckBoxItem(comboMenu, "lucian.combo.start.e"))
                {
                    if (!LucianSpells.E.IsReady() && LucianSpells.Q.IsReady() &&
                        getCheckBoxItem(comboMenu, "lucian.q.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q.Range &&
                        Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        LucianSpells.Q.CastOnUnit((AIHeroClient)args.Target);
                    }

                    if (!LucianSpells.E.IsReady() && LucianSpells.W.IsReady() &&
                        getCheckBoxItem(comboMenu, "lucian.w.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.W.Range &&
                        Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        if (LucianSpells.W.GetDamage((AIHeroClient)args.Target) >= ((AIHeroClient)args.Target).Health)
                        {
                            if (LucianSpells.W.GetPrediction((AIHeroClient)args.Target).Hitchance >= HitChance.High)
                            {
                                LucianSpells.W.Cast(((AIHeroClient)args.Target));
                            }
                        }
                        else
                        {
                            if (getCheckBoxItem(comboMenu, "lucian.disable.w.prediction"))
                            {
                                LucianSpells.W.Cast(((AIHeroClient)args.Target).Position);
                            }
                            else
                            {
                                if (LucianSpells.W.GetPrediction((AIHeroClient)args.Target).Hitchance >= HitChance.Medium)
                                {
                                    LucianSpells.W.Cast(((AIHeroClient)args.Target).Position);
                                }
                            }
                        }
                    }
                    if (LucianSpells.E.IsReady() && getCheckBoxItem(comboMenu, "lucian.e.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q2.Range &&
                        Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (getBoxItem(comboMenu, "lucian.e.mode"))
                        {
                            case 0:
                                ECast((AIHeroClient)args.Target);
                                break;
                            case 1:
                                LucianSpells.E.Cast(Game.CursorPos);
                                break;
                        }
                    }
                }
                else
                {
                    if (LucianSpells.Q.IsReady() && getCheckBoxItem(comboMenu, "lucian.q.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q.Range &&
                        Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        LucianSpells.Q.CastOnUnit((AIHeroClient)args.Target);
                    }
                    if (LucianSpells.W.IsReady() && getCheckBoxItem(comboMenu, "lucian.w.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.W.Range &&
                        Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff")
                        && LucianSpells.W.GetPrediction((AIHeroClient)args.Target).Hitchance >= HitChance.Medium)
                    {
                        LucianSpells.W.Cast(((AIHeroClient)args.Target).Position);
                    }
                    if (LucianSpells.E.IsReady() && getCheckBoxItem(comboMenu, "lucian.e.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q2.Range &&
                        Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (getBoxItem(comboMenu, "lucian.e.mode"))
                        {
                            case 0:
                                ECast((AIHeroClient)args.Target);
                                break;
                            case 1:
                                LucianSpells.E.Cast(Game.CursorPos);
                                break;
                        }
                    }
                }
            }
            else if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion &&
                     args.Target.IsValid && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral
                     && ObjectManager.Player.ManaPercent > getSliderItem(jungleMenu, "lucian.jungle.mana"))
            {
                if (LucianSpells.Q.IsReady() && getCheckBoxItem(jungleMenu, "lucian.q.jungle") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q.Range &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                    ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.Q.CastOnUnit((Obj_AI_Minion)args.Target);
                }
                if (LucianSpells.W.IsReady() && getCheckBoxItem(jungleMenu, "lucian.w.jungle") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.W.Range &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                    ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.W.Cast(((Obj_AI_Minion)args.Target).Position);
                }
                if (LucianSpells.E.IsReady() && getCheckBoxItem(jungleMenu, "lucian.e.jungle") &&
                    ((Obj_AI_Minion)args.Target).LSIsValidTarget(1000) &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                    ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.E.Cast(Game.CursorPos);
                }
            }
        }

        private static void LucianOnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            if (getKeyBindItem(Config, "lucian.semi.manual.ult"))
            {
                SemiManual();
            }

            if (UltActive)
            {
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableAttacking = false;
            }

            if (getCheckBoxItem(killStealMenu, "lucian.q.ks") && (LucianSpells.Q.IsReady()))
            {
                KillstealQ();
                ExtendedQKillSteal();
            }

            if (getCheckBoxItem(killStealMenu, "lucian.w.ks") && LucianSpells.W.IsReady())
            {
                KillstealW();
            }
        }

        private static void SemiManual()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (UltActive)
            {
                return;
            }
            foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.R.Range) && LucianSpells.R.GetPrediction(x).CollisionObjects.Count == 0))
            {
                LucianSpells.R.Cast(enemy);
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(harassMenu, "lucian.harass.mana"))
            {
                return;
            }
            if ((LucianSpells.Q.IsReady() || LucianSpells.Q2.IsReady()) && getCheckBoxItem(harassMenu, "lucian.q.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                HarassQCast();
            }
            if (LucianSpells.W.IsReady() && getCheckBoxItem(harassMenu, "lucian.w.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.W.Range) && LucianSpells.W.GetPrediction(x).Hitchance >= HitChance.Medium))
                {
                    if (enemy != null)
                        LucianSpells.W.Cast(enemy);
                }
            }
        }

        private static readonly Func<AIHeroClient, Obj_AI_Base, bool> CheckDistance =
            (champ, minion) =>
            Math.Abs(
                champ.LSDistance(ObjectManager.Player) - (minion.LSDistance(ObjectManager.Player) + minion.LSDistance(champ)))
            <= 2;

        private static readonly Func<Vector3, Vector3, Vector3, bool> CheckLine = (v1, v2, v3) =>
        {
            float valor =
                Math.Abs(
                    (v1.X * v2.Y) + (v1.Y * v3.X) + (v2.X * v3.Y) - (v1.Y * v2.X) - (v1.X * v3.Y) - (v2.Y * v3.X));

            return valor > 500 && valor <= 2000;
        };

        private static void HarassQCast()
        {
            switch (getBoxItem(harassMenu, "lucian.q.type"))
            {
                case 0:
                    List<Obj_AI_Base> minions = MinionManager.GetMinions(LucianSpells.Q2.Range);
                    if (!LucianSpells.Q.IsReady() || minions.Count == 0 || ObjectManager.Player.CountEnemiesInRange(LucianSpells.Q2.Range) == 0)
                    {
                        return;
                    }

                    foreach (AIHeroClient target in ObjectManager.Player.GetEnemiesInRange(LucianSpells.Q2.Range))
                    {
                        List<Vector2> position = new List<Vector2> { target.Position.LSTo2D() };

                        Obj_AI_Base colisionMinion =
                            LucianSpells.Q2.GetCollision(ObjectManager.Player.Position.LSTo2D(), position)
                                .FirstOrDefault(
                                    minion => LucianSpells.Q.CanCast(minion) && LucianSpells.Q.IsInRange(minion) && CheckLine(ObjectManager.Player.Position, minion.Position, target.ServerPosition) && CheckDistance(target, minion) && target.LSDistance(ObjectManager.Player) > minion.LSDistance(ObjectManager.Player) && ObjectManager.Player.LSDistance(minion) + minion.LSDistance(target) <= ObjectManager.Player.LSDistance(target) + 10f);

                        if (colisionMinion != null)
                        {
                            LucianSpells.Q.CastOnUnit(colisionMinion);
                        }
                    }
                    break;
                case 1:
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.Q.Range)))
                    {
                        LucianSpells.Q.CastOnUnit(enemy);
                    }
                    break;
            }
        }

        private static void ExtendedQKillSteal()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.LSIsValidTarget(LucianSpells.Q.Range));
            var target = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(LucianSpells.Q2.Range));
            if (target != null)
            {
                if (target.LSDistance(ObjectManager.Player.Position) > LucianSpells.Q.Range && target.LSDistance(ObjectManager.Player.Position) < LucianSpells.Q2.Range && target.LSCountEnemiesInRange(LucianSpells.Q2.Range) >= 1 && target.Health < LucianSpells.Q.GetDamage(target) && !target.IsDead)
                {
                    foreach (var minion in minions)
                    {
                        if (LucianSpells.Q2.WillHit(target, ObjectManager.Player.ServerPosition.LSExtend(minion.ServerPosition, LucianSpells.Q2.Range), 0, HitChance.VeryHigh))
                        {
                            LucianSpells.Q2.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
        private static void KillstealW()
        {
            var target = HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.W.Range)).
                FirstOrDefault(x => x.Health < LucianSpells.W.GetDamage(x));

            var pred = LucianSpells.W.GetPrediction(target);

            if (target != null && pred.Hitchance >= HitChance.High)
            {
                LucianSpells.W.Cast(pred.CastPosition);
            }
        }

        private static void KillstealQ()
        {
            var target = HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.Q.Range)).FirstOrDefault(x => x.Health < LucianSpells.Q.GetDamage(x));
            if (target != null)
            {
                LucianSpells.Q.Cast(target);
            }
        }

        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(clearMenu, "lucian.clear.mana"))
            {
                return;
            }
            if (LucianSpells.Q.IsReady() && getCheckBoxItem(clearMenu, "lucian.q.clear") &&
                ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, LucianSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly))
                {
                    var prediction = Prediction.GetPrediction(minion, LucianSpells.Q.Delay, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRadius);

                    var collision = LucianSpells.Q.GetCollision(ObjectManager.Player.Position.LSTo2D(), new List<Vector2> { prediction.UnitPosition.LSTo2D() });

                    foreach (var cs in collision)
                    {
                        if (collision.Count >= getSliderItem(clearMenu, "lucian.q.minion.hit.count"))
                        {
                            if (collision.Last().LSDistance(ObjectManager.Player) - collision[0].LSDistance(ObjectManager.Player) <= 600 && collision[0].LSDistance(ObjectManager.Player) <= 500)
                            {
                                LucianSpells.Q.Cast(cs);
                            }
                        }
                    }
                }
            }
            if (LucianSpells.W.IsReady() && getCheckBoxItem(clearMenu, "lucian.w.clear") &&
                ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                if (LucianSpells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, LucianSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= getSliderItem(clearMenu, "lucian.w.minion.hit.count"))
                {
                    LucianSpells.W.Cast(
                        LucianSpells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position,
                            LucianSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
                }
            }
        }

        private static void LucianOnDraw(EventArgs args)
        {
            LucianDrawing.Init();
        }
    }
}