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

namespace iAhri
{
    internal class Program
    {
        private static readonly string Author = "iCreative";
        private static readonly string AddonName = "IAhri";
        private static readonly float RefreshTime = 0.4f;
        private static readonly Dictionary<int, DamageInfo> PredictedDamage = new Dictionary<int, DamageInfo>();
        private static Menu menu;
        private static readonly Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>();
        private static Spell.Skillshot Q, W, E, R;
        private static Spell.Targeted Ignite;

        private static readonly Dictionary<string, object> _Q = new Dictionary<string, object>
        {
            {"MinSpeed", 400},
            {"MaxSpeed", 2500},
            {"Acceleration", -3200},
            {"Speed1", 1400},
            {"Delay1", 250},
            {"Range1", 880},
            {"Delay2", 0},
            {"Range2", int.MaxValue},
            {"IsReturning", false},
            {"Target", null},
            {"Object", null},
            {"LastObjectVector", null},
            {"LastObjectVectorTime", null},
            {"CatchPosition", null}
        };

        private static readonly Dictionary<string, object> _E = new Dictionary<string, object>
        {
            {"LastCastTime", 0f},
            {"Object", null}
        };

        private static readonly Dictionary<string, object> _R = new Dictionary<string, object> { { "EndTime", 0f } };

        private static AIHeroClient myHero
        {
            get { return ObjectManager.Player; }
        }

        private static Vector3 mousePos
        {
            get { return Game.CursorPos; }
        }

        private static float Overkill
        {
            get { return (100f + SubMenu["Misc"]["Overkill"].Cast<Slider>().CurrentValue) / 100f; }
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (myHero.Hero != Champion.Ahri)
            {
                return;
            }
            Chat.Print(AddonName + "made by" + Author + " loaded, have fun!.");
            Q = new Spell.Skillshot(SpellSlot.Q, 880, SkillShotType.Linear, 250, 1500, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, 0, 1400, 300)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 975, SkillShotType.Linear, 250, 1550, 60)
            {
                AllowedCollisionCount = 0
            };
            R = new Spell.Skillshot(SpellSlot.R, 800, SkillShotType.Circular, 0, 1400, 300)
            {
                AllowedCollisionCount = int.MaxValue
            };
            var slot = myHero.GetSpellSlotFromName("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot, 600);
            }

            menu = MainMenu.AddMenu(AddonName, AddonName + " by " + Author + "v1.0");
            menu.AddLabel(AddonName + " made by " + Author);

            SubMenu["Combo"] = menu.AddSubMenu("连招", "Combo");
            SubMenu["Combo"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["Combo"].Add("W", new CheckBox("使用 W", true));
            SubMenu["Combo"].Add("E", new CheckBox("使用 E", true));
            SubMenu["Combo"].Add("R", new CheckBox("使用 R", true));
            SubMenu["Combo"].Add("CatchQR", new CheckBox("使用R来接住Q", true));
            SubMenu["Combo"].Add("CatchQRPriority", new CheckBox("优先使用R接住Q", true));

            SubMenu["Harass"] = menu.AddSubMenu("骚扰", "Harass");
            SubMenu["Harass"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["Harass"].Add("W", new CheckBox("使用 W", false));
            SubMenu["Harass"].Add("E", new CheckBox("使用 E", false));
            SubMenu["Harass"].Add("Mana", new Slider("最低蓝量使用:", 20, 0, 100));

            SubMenu["JungleClear"] = menu.AddSubMenu("清野", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["JungleClear"].Add("W", new CheckBox("使用 W", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("使用 E", true));
            SubMenu["JungleClear"].Add("Mana", new Slider("最低蓝量使用:", 20, 0, 100));

            SubMenu["KillSteal"] = menu.AddSubMenu("抢头", "KillSteal");
            SubMenu["KillSteal"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["KillSteal"].Add("W", new CheckBox("使用 W", true));
            SubMenu["KillSteal"].Add("E", new CheckBox("使用 E", true));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("使用点燃", true));

            SubMenu["Flee"] = menu.AddSubMenu("逃跑", "Flee");
            SubMenu["Flee"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["Flee"].Add("R", new CheckBox("使用 R", true));

            SubMenu["Draw"] = menu.AddSubMenu("线圈", "Draw");
            SubMenu["Draw"].Add("Line", new CheckBox("显示Q路径", true));

            SubMenu["Misc"] = menu.AddSubMenu("杂项", "Misc");
            SubMenu["Misc"].Add("Overkill", new Slider("伤害预测 %（防浪费技能在可击杀敌人身上）", 10, 0, 100));
            SubMenu["Misc"].Add("CatchQMovement", new CheckBox("移动接Q", false));
            SubMenu["Misc"].Add("Gapclose", new CheckBox("使用E造成间距", true));
            SubMenu["Misc"].Add("Channeling", new CheckBox("使用E打断技能", true));

            Game.OnTick += OnTick;
            GameObject.OnCreate += OnCreateObj;
            GameObject.OnDelete += OnDeleteObj;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnBuffGain += OnApplyBuff;
            Obj_AI_Base.OnBuffLose += OnRemoveBuff;
            Gapcloser.OnGapcloser += OnGapCloser;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
        }

        public static bool IsWall(Vector3 v)
        {
            var v2 = v.To2D();
            return NavMesh.GetCollisionFlags(v2.X, v2.Y).HasFlag(CollisionFlags.Wall);
        }

        private static void OnTick(EventArgs args)
        {
            if (myHero.IsDead)
            {
                return;
            }
            if (_Q["Object"] != null)
            {
                //Q.Range = (uint)_Q["Range2"];
                Q.CastDelay = (int)_Q["Delay2"];
                Q.SourcePosition = ((GameObject)_Q["Object"]).Position;
                if (_Q["LastObjectVector"] != null)
                {
                    Q.Speed = (int)(((Vector3)Q.SourcePosition).Distance((Vector3)_Q["LastObjectVector"])
                                     / (Game.Time - (float)_Q["LastObjectVectorTime"]));
                }
                _Q["LastObjectVector"] = new Vector3(((Vector3)Q.SourcePosition).X, ((Vector3)Q.SourcePosition).Y,
                    ((Vector3)Q.SourcePosition).Z);
                _Q["LastObjectVectorTime"] = Game.Time;
            }
            else
            {
                //Q.Range = (float)_Q["Range1"];
                Q.CastDelay = (int)_Q["Delay1"];
                Q.Speed = (int)_Q["Speed1"];
                Q.SourcePosition = myHero.Position;
            }
            CatchQ();
            KillSteal();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                     Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    JungleClear();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
        }

        private static void KillSteal()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsValidTarget(E.Range) && enemy.HealthPercent <= 40)
                {
                    var damageI = GetBestCombo(enemy);
                    if (damageI.Damage >= enemy.TotalShieldHealth())
                    {
                        if (SubMenu["KillSteal"]["Q"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, Q.Slot) >= enemy.TotalShieldHealth() || damageI.Q))
                        {
                            CastQ(enemy);
                        }
                        if (SubMenu["KillSteal"]["W"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, W.Slot) >= enemy.TotalShieldHealth() || damageI.W))
                        {
                            CastW(enemy);
                        }
                        if (SubMenu["KillSteal"]["E"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, E.Slot) >= enemy.TotalShieldHealth() || damageI.E))
                        {
                            CastE(enemy);
                        }
                    }
                    if (Ignite != null && SubMenu["KillSteal"]["Ignite"].Cast<CheckBox>().CurrentValue &&
                        Ignite.IsReady() &&
                        myHero.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite) >= enemy.TotalShieldHealth())
                    {
                        Ignite.Cast(enemy);
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target.IsValidTarget())
            {
                if (SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue)
                {
                    CastR(target);
                }
                if (SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue)
                {
                    CastE(target);
                }
                if ((Game.Time - (float)_E["LastCastTime"] <= (float)(E.CastDelay / 1000 * 1.1)) ||
                    (_E["Object"] != null &&
                     myHero.Position.Distance(target.Position) >
                     myHero.Position.Distance(((GameObject)_E["Object"]).Position)))
                {
                    return;
                }
                if (SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue)
                {
                    CastQ(target);
                }
                if (SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue)
                {
                    CastW(target);
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target.IsValidTarget() && myHero.ManaPercent >= SubMenu["Harass"]["Mana"].Cast<Slider>().CurrentValue)
            {
                if (SubMenu["Harass"]["E"].Cast<CheckBox>().CurrentValue)
                {
                    CastE(target);
                }
                if ((Game.Time - (float)_E["LastCastTime"] <= (float)(E.CastDelay / 1000f * 1.1f)) ||
                    (_E["Object"] != null &&
                     myHero.Position.Distance(target.Position) >
                     myHero.Position.Distance(((GameObject)_E["Object"]).Position)))
                {
                    return;
                }
                if (SubMenu["Harass"]["Q"].Cast<CheckBox>().CurrentValue)
                {
                    CastQ(target);
                }
                if (SubMenu["Harass"]["W"].Cast<CheckBox>().CurrentValue)
                {
                    CastW(target);
                }
            }
        }

        private static void Flee()
        {
            if (SubMenu["Flee"]["R"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.Cast(mousePos);
            }
            if (SubMenu["Flee"]["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.Cast(mousePos);
            }
        }

        private static void JungleClear()
        {
            if (myHero.ManaPercent >= SubMenu["JungleClear"]["Mana"].Cast<Slider>().CurrentValue)
            {
                foreach (var minion in EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.Position, 1000f).Where(minion => minion.IsValidTarget() &&
                                                                                                                                  myHero.ManaPercent >= SubMenu["JungleClear"]["Mana"].Cast<Slider>().CurrentValue))
                {
                    if (SubMenu["JungleClear"]["E"].Cast<CheckBox>().CurrentValue)
                    {
                        CastE(minion);
                    }
                    if ((Game.Time - (float)_E["LastCastTime"] <= (float)(E.CastDelay / 1000 * 1.1)) ||
                        (_E["Object"] != null &&
                         myHero.Position.Distance(minion.Position) >
                         myHero.Position.Distance(((GameObject)_E["Object"]).Position)))
                    {
                        return;
                    }
                    if (SubMenu["JungleClear"]["Q"].Cast<CheckBox>().CurrentValue)
                    {
                        CastQ(minion);
                    }
                    if (SubMenu["JungleClear"]["W"].Cast<CheckBox>().CurrentValue)
                    {
                        CastW(minion);
                    }
                }
            }
        }

        private static void CastQ(Obj_AI_Base target)
        {
            if (Q.IsReady() && target.IsValidTarget())
            {
                var maxspeed = (int)_Q["MaxSpeed"];
                var acc = (int)_Q["Acceleration"];
                var speed = Math.Sqrt(Math.Pow(maxspeed, 2) + 2 * acc * myHero.Distance(target));
                var tf = (speed - maxspeed) / acc;
                Q.Speed = (int)(myHero.Distance(target) / tf);
                var r = Q.GetPrediction(target);
                if (r.HitChance >= HitChance.High)
                {
                    Q.Cast(r.CastPosition);
                    _Q["Target"] = target;
                }
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (W.IsReady() && target.IsValidTarget())
            {
                var r = W.GetPrediction(target);
                if (r.HitChance >= HitChance.Medium)
                {
                    if (target.Type == myHero.Type)
                    {
                        if (_Q["Object"] != null || Orbwalker.LastTarget.NetworkId == target.NetworkId)
                        {
                            myHero.Spellbook.CastSpell(W.Slot);
                        }
                    }
                    else
                    {
                        myHero.Spellbook.CastSpell(W.Slot);
                    }
                }
            }
        }

        private static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady() && target.IsValidTarget())
            {
                var r = E.GetPrediction(target);
                if (r.HitChance >= HitChance.High && target.IsEnemy)
                {
                    E.Cast(r.CastPosition);
                }
            }
        }

        private static void CastR(Obj_AI_Base target)
        {
            if (R.IsReady() && target.IsValidTarget())
            {
                var damageI = GetBestCombo(target);
                if (SubMenu["Combo"]["CatchQRPriority"].Cast<CheckBox>().CurrentValue)
                {
                    if ((float)_R["EndTime"] > 0)
                    {
                        //have active r
                        if (_Q["Object"] != null)
                        {
                            if ((bool)_Q["IsReturning"] &&
                                myHero.Distance((GameObject)_Q["Object"]) < myHero.Distance((Obj_AI_Base)_Q["Target"]))
                            {
                                R.Cast(mousePos);
                            }
                            else
                            {
                                return;
                            }
                        }
                        if (!Q.IsReady() &&
                            (float)_R["EndTime"] - Game.Time <= myHero.Spellbook.GetSpell(R.Slot).Cooldown)
                        {
                            R.Cast(mousePos);
                        }
                    }
                    if (damageI.Damage >= target.TotalShieldHealth() && mousePos.Distance(target) < myHero.Distance(target))
                    {
                        if (damageI.R)
                        {
                            if (myHero.Distance(target) > 400)
                            {
                                R.Cast(mousePos);
                            }
                        }
                    }
                }
                else
                {
                    if ((float)_R["EndTime"] > 0)
                    {
                        if (!Q.IsReady() &&
                            (float)_R["EndTime"] - Game.Time <= myHero.Spellbook.GetSpell(R.Slot).Cooldown)
                        {
                            R.Cast(mousePos);
                        }
                    }
                    if (damageI.Damage >= target.TotalShieldHealth() && mousePos.Distance(target) < myHero.Distance(target))
                    {
                        if (damageI.R)
                        {
                            if (myHero.Distance(target) > 400)
                            {
                                R.Cast(mousePos);
                            }
                        }
                    }
                }
            }
        }

        private static void CatchQ()
        {
            if (_Q["Object"] != null)
            {
                _Q["CatchPosition"] = null;
                var target = _Q["Target"] != null
                    ? (Obj_AI_Base)_Q["Target"]
                    : TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target != null && target.IsValidTarget())
                {
                    var r = Q.GetPrediction(target);
                    if (myHero.Distance(r.CastPosition) <= myHero.Distance((GameObject)_Q["Object"]))
                    {
                        //Chat.Print("2");
                        var TimeLeft = myHero.Distance(target) / Q.Speed;
                        var qObject = ((GameObject)_Q["Object"]).Position;
                        var ExtendedPos = qObject + (r.CastPosition - qObject).Normalized() * 1500;
                        var ClosestToTargetLine = myHero.Position.To2D().ProjectOn(qObject.To2D(), ExtendedPos.To2D());
                        var ClosestToHeroLine = r.CastPosition.To2D().ProjectOn(qObject.To2D(), myHero.Position.To2D());
                        if (ClosestToTargetLine.IsOnSegment && ClosestToHeroLine.IsOnSegment &&
                            ClosestToTargetLine.SegmentPoint.Distance(qObject.To2D()) <
                            r.CastPosition.To2D().Distance(qObject.To2D()))
                        {
                            //Chat.Print("3");
                            if (ClosestToTargetLine.SegmentPoint.Distance(myHero.Position.To2D()) <
                                myHero.MoveSpeed * TimeLeft)
                            {
                                if (SubMenu["Misc"]["CatchQMovement"].Cast<CheckBox>().CurrentValue)
                                {
                                    //Chat.Print("4");
                                    if (ClosestToHeroLine.SegmentPoint.Distance(r.CastPosition.To2D()) > Q.Width)
                                    {
                                        Orbwalker.OrbwalkTo(ClosestToTargetLine.SegmentPoint.To3D());
                                    }
                                }
                            }
                            else if (ClosestToTargetLine.SegmentPoint.Distance(myHero.Position.To2D()) <
                                     450 + myHero.MoveSpeed * TimeLeft)
                            {
                                if (SubMenu["Combo"]["CatchQR"].Cast<CheckBox>().CurrentValue &&
                                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                                {
                                    if (ClosestToHeroLine.SegmentPoint.Distance(r.CastPosition.To2D()) > Q.Width)
                                    {
                                        var rPos = myHero.Position +
                                                   (ClosestToTargetLine.SegmentPoint.To3D() - myHero.Position)
                                                       .Normalized() *
                                                   myHero.Distance(ClosestToTargetLine.SegmentPoint.To3D()) * 1.2f;
                                        R.Cast(rPos);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("missile"))
            {
                var missile = sender as MissileClient;
                if (missile == null || !missile.IsValid || missile.SpellCaster == null || !missile.SpellCaster.IsValid)
                {
                    return;
                }
                if (missile.SpellCaster.IsMe)
                {
                    var name = missile.SData.Name.ToLower();
                    if (name.Contains("ahriorbmissile"))
                    {
                        _Q["Object"] = sender;
                        _Q["IsReturning"] = false;
                    }
                    else if (name.Contains("ahriorbreturn"))
                    {
                        _Q["Object"] = sender;
                        _Q["IsReturning"] = true;
                    }
                    else if (name.Contains("ahriseducemissile"))
                    {
                        _E["Object"] = sender;
                    }
                }
            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("missile"))
            {
                var missile = sender as MissileClient;
                if (missile == null || !missile.IsValid || missile.SpellCaster == null || !missile.SpellCaster.IsValid)
                {
                    return;
                }
                if (missile.SpellCaster.IsMe)
                {
                    var name = missile.SData.Name.ToLower();
                    if (name.Contains("ahriorbreturn"))
                    {
                        _Q["Object"] = null;
                        _Q["IsReturning"] = false;
                        _Q["Target"] = null;
                        _Q["LastObjectVector"] = null;
                    }
                    else if (name.Contains("ahriseducemissile"))
                    {
                        _E["Object"] = null;
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (myHero.IsDead)
            {
                return;
            }
            if (_Q["Object"] != null && SubMenu["Draw"]["Line"].Cast<CheckBox>().CurrentValue)
            {
                var asd = (GameObject)_Q["Object"];
                var p1 = Drawing.WorldToScreen(myHero.Position);
                var p2 = Drawing.WorldToScreen(asd.Position);
                Drawing.DrawLine(p1, p2, Q.Width, Color.FromArgb(100, 255, 255, 255));
            }
        }

        private static void OnGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (SubMenu["Misc"]["Gapclose"].Cast<CheckBox>().CurrentValue)
            {
                CastE(args.Sender);
            }
        }

        private static void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (SubMenu["Misc"]["Channeling"].Cast<CheckBox>().CurrentValue)
            {
                CastE(args.Sender);
            }
        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (Q.Slot == SpellSlot.Q)
                {
                    _Q["IsReturning"] = false;
                    _Q["Object"] = null;
                }
                else if (E.Slot == SpellSlot.E)
                {
                    _E["Object"] = null;
                    _E["LastCastTime"] = Game.Time;
                }
            }
        }

        private static void OnApplyBuff(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe)
            {
                var buff = args.Buff;
                if (buff.Name.ToLower() == "ahritumble")
                {
                    _R["EndTime"] = Game.Time + buff.EndTime - buff.StartTime;
                }
            }
        }

        private static void OnRemoveBuff(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe)
            {
                var buff = args.Buff;
                if (buff.Name.ToLower() == "ahritumble")
                {
                    _R["EndTime"] = 0f;
                }
            }
        }

        private static Obj_AI_Base LastHit(Spell.SpellBase spell)
        {
            return null;
        }

        private static float Damage(Obj_AI_Base target, SpellSlot slot)
        {
            if (target.IsValidTarget())
            {
                if (slot == SpellSlot.Q)
                {
                    return
                        myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            25f * Q.Level + 15 + 0.35f * myHero.TotalMagicalDamage) +
                        myHero.CalculateDamageOnUnit(target, DamageType.True,
                            25f * Q.Level + 15 + 0.35f * myHero.TotalMagicalDamage);
                }
                if (slot == SpellSlot.W)
                {
                    return 1.6f *
                           myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                               25f * W.Level + 15 + 0.4f * myHero.TotalMagicalDamage);
                }
                if (slot == SpellSlot.E)
                {
                    return myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                        35f * E.Level + 25 + 0.5f * myHero.TotalMagicalDamage);
                }
                if (slot == SpellSlot.R)
                {
                    return 3 *
                           myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                               40f * R.Level + 30 + 0.3f * myHero.TotalMagicalDamage);
                }
            }
            return myHero.GetSpellDamage(target, slot);
        }

        private static DamageInfo GetComboDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r)
        {
            var comboDamage = 0f;
            var manaWasted = 0f;
            if (target.IsValidTarget())
            {
                if (q)
                {
                    comboDamage += Damage(target, Q.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
                }
                if (w)
                {
                    comboDamage += Damage(target, W.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
                }
                if (e)
                {
                    comboDamage += Damage(target, E.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
                }
                if (r)
                {
                    comboDamage += Damage(target, R.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
                }
                if (Ignite != null && Ignite.IsReady())
                {
                    comboDamage += myHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                }
                comboDamage += myHero.GetAutoAttackDamage(target, true);
            }
            comboDamage = comboDamage * Overkill;
            return new DamageInfo(comboDamage, manaWasted);
        }

        private static DamageInfo GetBestCombo(Obj_AI_Base target)
        {
            var q = Q.IsReady() ? new[] { false, true } : new[] { false };
            var w = W.IsReady() ? new[] { false, true } : new[] { false };
            var e = E.IsReady() ? new[] { false, true } : new[] { false };
            var r = R.IsReady() ? new[] { false, true } : new[] { false };
            if (target.IsValidTarget())
            {
                DamageInfo damageI2;
                if (PredictedDamage.ContainsKey(target.NetworkId))
                {
                    var damageI = PredictedDamage[target.NetworkId];
                    if (Game.Time - damageI.Time <= RefreshTime)
                    {
                        return damageI;
                    }
                    bool[] best =
                    {
                        Q.IsReady(),
                        W.IsReady(),
                        E.IsReady(),
                        R.IsReady()
                    };
                    var bestdmg = 0f;
                    var bestmana = 0f;
                    foreach (var q1 in q)
                    {
                        foreach (var w1 in w)
                        {
                            foreach (var e1 in e)
                            {
                                foreach (var r1 in r)
                                {
                                    damageI2 = GetComboDamage(target, q1, w1, e1, r1);
                                    var d = damageI2.Damage;
                                    var m = damageI2.Mana;
                                    if (myHero.Mana >= m)
                                    {
                                        if (bestdmg >= target.TotalShieldHealth())
                                        {
                                            if (d >= target.TotalShieldHealth() && (d < bestdmg || m < bestmana))
                                            {
                                                bestdmg = d;
                                                bestmana = m;
                                                best = new[] { q1, w1, e1, r1 };
                                            }
                                        }
                                        else
                                        {
                                            if (d >= bestdmg)
                                            {
                                                bestdmg = d;
                                                bestmana = m;
                                                best = new[] { q1, w1, e1, r1 };
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    PredictedDamage[target.NetworkId] = new DamageInfo(best[0], best[1], best[2], best[3], bestdmg,
                        bestmana, Game.Time);
                    return PredictedDamage[target.NetworkId];
                }
                damageI2 = GetComboDamage(target, Q.IsReady(), W.IsReady(), E.IsReady(), R.IsReady());
                PredictedDamage[target.NetworkId] = new DamageInfo(false, false, false, false, damageI2.Damage,
                    damageI2.Mana, Game.Time - Game.Ping * 2);
                return GetBestCombo(target);
            }
            return new DamageInfo(false, false, false, false, 0, 0, 0);
        }
    }

    internal class DamageInfo
    {
        public float Damage;
        public bool E;
        public float Mana;
        public bool Q;
        public bool R;
        public float Time;
        public bool W;

        public DamageInfo(bool Q, bool W, bool E, bool R, float Damage, float Mana, float Time)
        {
            this.Q = Q;
            this.W = W;
            this.E = E;
            this.R = R;
            this.Damage = Damage;
            this.Mana = Mana;
            this.Time = Time;
        }

        public DamageInfo(float Damage, float Mana)
        {
            this.Damage = Damage;
            this.Mana = Mana;
        }
    }
}