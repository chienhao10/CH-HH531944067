using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using UnderratedAIO.Helpers;

namespace GragasTheDrunkCarry
{

    internal class GragasQ
    {
        public Vector3 position;
        public int time;

        public GragasQ(Vector3 _position, int _tickCount)
        {
            position = _position;
            time = _tickCount;
        }

        public float deltaT()
        {
            return System.Environment.TickCount - time;
        }
    }

    internal class Gragas
    {
        public static AIHeroClient Player;
        public static Spell Q, W, E, R;
        public static Menu Config, comboMenu, ksMenu, jungleMenu, clearMenu, harassMenu, drawMenu;
        public static GragasQ savedQ = null;
        public static GameObject Bomb;
        public static float LastMove;
        public static Vector3 qPos, rPos;
        public static bool justE, justR, justQ;

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

        public static void Game_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, 775);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 1050);

            Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.3f, 50, 1000, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.3f, 700, 1000, false, SkillshotType.SkillshotCircle);

            Config = MainMenu.AddMenu("Gragas", "Gragas");

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQ", new CheckBox("Use Q?"));
            comboMenu.Add("UseW", new CheckBox("Use W?"));
            comboMenu.Add("UseE", new CheckBox("Use E?"));
            comboMenu.Add("UseR", new CheckBox("Use R?"));
            comboMenu.Add("AutoB", new CheckBox("Auto Bomb?"));
            comboMenu.Add("Insec", new KeyBind("Insec", false, KeyBind.BindTypes.HoldActive, 'A'));

            ksMenu = Config.AddSubMenu("KillSteal", "KS");
            ksMenu.Add("QKS", new CheckBox("KS with Q"));
            ksMenu.Add("EKS", new CheckBox("KS with E"));
            ksMenu.Add("RKS", new CheckBox("KS with R"));
            ksMenu.Add("SmartKs", new CheckBox("Auto Ks?"));

            jungleMenu = Config.AddSubMenu("Jungle Farm", "JG");
            jungleMenu.Add("JQ", new CheckBox("Use Q"));
            jungleMenu.Add("JW", new CheckBox("Use E"));
            jungleMenu.Add("JE", new CheckBox("Use R"));

            clearMenu = Config.AddSubMenu("Wave Clear", "LC");
            clearMenu.Add("WQ", new CheckBox("Use Q"));
            clearMenu.Add("WW", new CheckBox("Use E"));
            clearMenu.Add("WE", new CheckBox("Use R"));

            harassMenu = Config.AddSubMenu("Harras", "Harras");
            harassMenu.Add("UseQH", new CheckBox("Use Q?"));
            harassMenu.Add("UseEH", new CheckBox("Use E?"));

            drawMenu = Config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("DrawIN", new CheckBox("Draw Insec Pos?"));
            drawMenu.Add("DrawQ", new CheckBox("Draw Q range"));
            drawMenu.Add("DrawE", new CheckBox("Draw E Range"));
            drawMenu.Add("DrawR", new CheckBox("Draw R Range"));

            Player = ObjectManager.Player;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += GameObject_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
        }

        private static void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "GragasQ")
                {
                    if (!justQ)
                    {
                        justQ = true;
                        qPos = args.End;
                        savedQ = new GragasQ(args.End, System.Environment.TickCount - 500);
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => justQ = false);
                    }
                }
                if (args.SData.Name == "GragasE")
                {
                    if (!justE)
                    {
                        justE = true;
                        var dist = Player.Distance(args.End);
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            (int)Math.Min(((dist > E.Range ? E.Range : dist) / E.Speed * 1000f), 250),
                            () => justE = false);
                    }
                }
                if (args.Slot == SpellSlot.R)
                {
                    if (!justR)
                    {
                        justR = true;
                        rPos = args.End;
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            300, () =>
                            {
                                justR = false;
                                rPos = Vector3.Zero;
                            });
                    }
                }
            }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                savedQ = new GragasQ(sender.Position, System.Environment.TickCount);
                Bomb = sender;
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                savedQ = null;
                Bomb = null;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var vTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                JungleFarm();
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (vTarget == null) return;
                if (getCheckBoxItem(comboMenu, "UseQ") && Q.IsReady() && (System.Environment.TickCount - LastMove > 50))
                {
                    Qcast(vTarget);
                    LastMove = System.Environment.TickCount;
                }

                if (E.IsReady() && Player.Distance(vTarget) <= E.Range && getCheckBoxItem(comboMenu, "UseE") &&
                    (System.Environment.TickCount - LastMove > 50))
                {
                    E.Cast(vTarget, true);
                    LastMove = System.Environment.TickCount;
                }

                if (getCheckBoxItem(comboMenu, "UseW") && W.IsReady() && (System.Environment.TickCount - LastMove > 50))
                {
                    W.Cast();
                    LastMove = System.Environment.TickCount;
                }

                if (getCheckBoxItem(comboMenu, "UseR") && R.IsReady() && GetCDamage(vTarget) >= vTarget.Health &&
                    (System.Environment.TickCount - LastMove > 50))
                {
                    R.Cast(vTarget);
                    LastMove = System.Environment.TickCount;
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (vTarget == null) return;
                if (getCheckBoxItem(harassMenu, "UseQH") && Q.IsReady())
                {
                    Qcast(vTarget);
                }

                if (getCheckBoxItem(harassMenu, "UseEH") && E.IsReady() && Player.Distance(vTarget) <= E.Range)
                {
                    E.Cast(vTarget, true);
                }
            }

            if (getCheckBoxItem(comboMenu, "AutoB") && Bomb != null)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy && hero.Distance(Bomb.Position) <= 250))
                {
                    Qcast(hero);
                }
            }

            AIHeroClient target = TargetSelector.GetTarget(1300, DamageType.Magical);
            if (getKeyBindItem(comboMenu, "Insec"))
            {
                if (target == null)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    return;
                }
                CastE(target);
                if (savedQ != null)
                {
                    if (savedQ != null && !SimpleQ && target.Distance(Player) < R.Range - 100 && target.Position.Distance(savedQ.position) < 550 + QExplosionRange / 2)
                    {
                        HandeR(target, savedQ.position, true);
                    }
                }
                else
                {
                    castInsec(target);
                }
            }
        }

        public static float GetKnockBackRange(Vector3 to, Vector3 from)
        {
            return R.Range - from.Distance(to);
        }

        public static Vector3 GetPredictedBarellPosition(AIHeroClient target)
        {
            var result = new Vector3();

            if (target.IsValid)
            {
                var etaR = Player.Distance(target) / R.Speed;
                var pred = LeagueSharp.Common.Prediction.GetPrediction(target, etaR);

                result = LeagueSharp.Common.Geometry.LSExtend(pred.UnitPosition, target.ServerPosition, GetKnockBackRange(target.ServerPosition, pred.UnitPosition));
            }
            return result;
        }

        private static void castInsec(AIHeroClient target)
        {
            if (Q.IsReady() && SimpleQ)
            {
                var pred = R.GetPrediction(target);
                if (R.IsReady() &&
                    target.Buffs.Any(
                        buff =>
                            buff.Type == BuffType.Snare || buff.Type == BuffType.Stun ||
                            buff.Type == BuffType.Suppression || buff.Type == BuffType.Knockup))
                {
                    if (pred.Hitchance >= HitChance.Medium && pred.CastPosition.Distance(Player.Position) < R.Range - 150)
                    {
                        R.Cast(pred.CastPosition.Extend(Player.Position, -150));
                    }
                }
                if (justR && rPos.IsValid())
                {
                    Q.Cast(rPos.Extend(pred.UnitPosition, 550 + QExplosionRange / 2f));
                }
            }
        }

        private static void HandeR(Obj_AI_Base target, Vector3 toVector3, bool toBarrel)
        {
            if (target == null || !toVector3.IsValid())
            {
                return;
            }
            var pred = LeagueSharp.Common.Prediction.GetPrediction(target, target.Distance(Player.ServerPosition) / R.Speed);
            if (pred.Hitchance >= HitChance.VeryHigh && !justE && !target.LSIsDashing())
            {
                var cast = pred.UnitPosition.Extend(toVector3, -100);
                if (Player.Distance(cast) < R.Range && checkBuffs(target, Player.Distance(cast)) && pred.UnitPosition.Distance(target.Position) < 15 && ((!CombatHelper.CheckWalls(target.Position, toVector3)) || (toBarrel && savedQ.position.Distance(target.Position) < QExplosionRange)))
                {
                    if (toBarrel && 4000 - savedQ.deltaT() > (Player.Distance(cast) + cast.Distance(savedQ.position)) / R.Speed)
                    {
                        R.Cast(cast);
                        return;
                    }
                    else if (!toBarrel)
                    {
                        R.Cast(cast);
                    }
                }
            }
        }

        private static bool checkBuffs(Obj_AI_Base hero, float distance)
        {
            var stun =
                hero.Buffs.Where(
                    buff =>
                        buff.Type == BuffType.Snare || buff.Type == BuffType.Stun || buff.Type == BuffType.Suppression ||
                        buff.Type == BuffType.Knockup)
                    .OrderByDescending(buff => CombatHelper.GetBuffTime(buff))
                    .FirstOrDefault();
            if (stun != null)
            {
                if (CombatHelper.GetBuffTime(stun) > distance / R.Speed)
                {
                    return true;
                }
            }
            return false;
        }

        public const int QExplosionRange = 300;

        private static bool SimpleQ
        {
            get { return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "GragasQ"; }
        }

        private static void CastE(AIHeroClient target)
        {
            if (E.CanCast(target))
            {
                E.CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        private static int GetCDamage(Obj_AI_Base target)
        {
            var damage = 0;
            if (Q.IsReady())
            {
                damage += (int)Q.GetDamage(target);
            }
            if (E.IsReady())
            {
                damage += (int)E.GetDamage(target);
            }
            if (R.IsReady())
            {
                damage += (int)R.GetDamage(target);
            }
            return damage;
        }

        private static void Qcast(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            if (!getCheckBoxItem(comboMenu, "UseQ")) return;
            if (!(target.Distance(Player) <= Q.Range)) return;
            if (Bomb == null)
            {
                Q.Cast(target, true);
            }

            if (Bomb != null && target.Distance(Bomb.Position) <= 250)
            {
                Q.Cast();
            }
        }

        private static bool IsWall(Vector3 pos)
        {
            var cFlags = NavMesh.GetCollisionFlags(pos);
            return cFlags == CollisionFlags.Wall;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            var vTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (vTarget != null && R.IsReady() && getCheckBoxItem(drawMenu, "DrawIN"))
            {
                Render.Circle.DrawCircle(rPos, 50, Color.Red);
            }
            if (getCheckBoxItem(drawMenu, "DrawQ"))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.DarkSlateGray);
            }
            if (getCheckBoxItem(drawMenu, "DrawE"))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.DarkSlateGray);
            }
            if (getCheckBoxItem(drawMenu, "DrawR"))
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.DarkSlateGray);
            }
        }
        public static void JungleFarm()
        {
            var minion =
                MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Q.IsReady() && getCheckBoxItem(jungleMenu, "JQ") && (System.Environment.TickCount - LastMove > 50))
            {
                if (minion.IsValidTarget(Q.Range))
                {
                    Qcast(minion);
                    LastMove = System.Environment.TickCount;
                }
            }

            if (E.IsReady() && getCheckBoxItem(jungleMenu, "JE") && (System.Environment.TickCount - LastMove > 50))
            {
                if (minion.IsValidTarget(E.Range))
                {
                    E.Cast(minion.Position);
                    LastMove = System.Environment.TickCount;
                }
            }


            if (W.IsReady() && getCheckBoxItem(jungleMenu, "JW") && (System.Environment.TickCount - LastMove > 50))
            {
                W.Cast();
                LastMove = System.Environment.TickCount;
            }
        }

        public static void WaveClear()
        {
            var minion =
                MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Q.IsReady() && getCheckBoxItem(clearMenu, "WQ") && (System.Environment.TickCount - LastMove > 50))
            {
                if (minion.IsValidTarget(Q.Range))
                {
                    Qcast(minion);
                    LastMove = System.Environment.TickCount;
                }
            }

            if (E.IsReady() && getCheckBoxItem(clearMenu, "WE") && (System.Environment.TickCount - LastMove > 50))
            {
                if (minion.IsValidTarget(E.Range))
                {
                    E.Cast(minion.Position);
                    LastMove = System.Environment.TickCount;
                }
            }

            if (W.IsReady() && getCheckBoxItem(clearMenu, "WW") && (System.Environment.TickCount - LastMove > 50))
            {
                W.Cast();
                LastMove = System.Environment.TickCount;
            }
        }
    }
}