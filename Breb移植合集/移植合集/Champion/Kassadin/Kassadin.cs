using System;
using System.Globalization;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using Damage = LeagueSharp.Common.Damage;
using Geometry = LeagueSharp.Common.Geometry;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace Kassawin
{
    internal class Kassadin : Helper
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static SpellSlot _ignite;
        public static Spell Q, W, E, R;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, Color.Red, "monospace");
        private static readonly System.Drawing.Color _color = System.Drawing.Color.Red;
        private static readonly System.Drawing.Color FillColor = System.Drawing.Color.Blue;

        private static DamageToUnitDelegate _damageToUnit;


        public static int GetPassiveBuff
        {
            get
            {
                var data = Player.Buffs.FirstOrDefault(b => b.Name.ToLower() == "forcepulsecounter");
                if (data != null)
                {
                    return data.Count == -1 ? 0 : data.Count == 0 ? 1 : data.Count;
                }
                return 0;
            }
        }

        public static Geometry.Polygon SafeZone { get; set; }

        public static Geometry.Polygon.Arc newCone { get; set; }
        public static bool EnableDrawingDamage { get; set; }
        public static System.Drawing.Color DamageFillColor { get; set; }


        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDrawChamp;
                }
                _damageToUnit = value;
            }
        }

        public static System.Drawing.Color Color1
        {
            get { return _color; }
        }

        public static void OnLoad()
        {
            if (Player.ChampionName != "Kassadin") return;
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 200);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 700);
            DamageToUnit = GetComboDamage;
            E.SetSkillshot(0.25f, 20, int.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.25f, 270, int.MaxValue, false, SkillshotType.SkillshotCircle);

            MenuConfig.OnLoad();
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            Obj_AI_Base.OnSpellCast += OnDoCasts;
            Drawing.OnDraw += OnDraw;
        }

        private static float GetComboDamage(AIHeroClient enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Q.GetDamage(enemy);

            if (W.IsReady())
                damage += W.GetDamage(enemy);

            if (E.IsReady() && eCanCast())
                damage += W.GetDamage(enemy);

            if (R.IsReady())
                damage += R.GetDamage(enemy);

            if (Player.GetSpellSlot("summonerdot").IsReady())
                damage += IgniteDamage(enemy);

            return (float)damage;
        }

        public static int DangerLevel(Interrupter2.InterruptableTargetEventArgs args)
        {
            switch (args.DangerLevel)
            {
                case Interrupter2.DangerLevel.Low:
                    return 1;

                case Interrupter2.DangerLevel.Medium:
                    return 2;

                case Interrupter2.DangerLevel.High:
                    return 3;
            }
            return 0;
        }

        private static void OnDraw(EventArgs args)
        {
            var draw = getCheckBoxItem(drawMenu, "enabledraw");
            if (!draw) return;

            var qdraw = getCheckBoxItem(drawMenu, "drawq");
            var edraw = getCheckBoxItem(drawMenu, "drawe");
            var rdraw = getCheckBoxItem(drawMenu, "drawr");
            var drawcount = getCheckBoxItem(drawMenu, "drawcount");

            if (qdraw)
            {
                if (Q.Level >= 1)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Purple);
            }

            if (edraw)
            {
                if (E.Level >= 1)
                    Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.MidnightBlue);
            }

            if (rdraw)
            {
                if (R.Level >= 1)
                    Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Crimson);
            }
            if (drawcount)
            {
                var pos = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.Red, "[R] Stack");
                Drawing.DrawText(pos.X + 70, pos.Y, System.Drawing.Color.Purple, ForcePulseCount().ToString());
            }
        }

        private static void OnDoCasts(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Orbwalking.IsAutoAttack(args.SData.Name)) return;

            if (!sender.IsMe) return;

            if (!args.SData.IsAutoAttack()) return;

            if (args.Target.Type != GameObjectType.obj_AI_Minion) return;

            var usew = getCheckBoxItem(farmMenu, "usewl");

            if (!usew) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var minions = MinionManager.GetMinions(Player.Position, 300);

                if (W.IsReady())
                {
                    if (((Obj_AI_Base)args.Target).Health > Player.GetAutoAttackDamage((Obj_AI_Base)args.Target) + 50)
                    {
                        W.Cast();
                        Orbwalker.ResetAutoAttack();
                    }
                    foreach (var min in minions.Where(x => x.NetworkId != ((Obj_AI_Base)args.Target).NetworkId))
                    {
                        if (((Obj_AI_Base)args.Target).Health > Player.GetAutoAttackDamage((Obj_AI_Base)args.Target))
                        {
                            W.Cast();
                            Orbwalker.ResetAutoAttack();
                        }
                    }
                }
            }
        }

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (!sender.IsMe) return;
            if (args.Target.Type != GameObjectType.AIHeroClient) return;

            var target = (AIHeroClient)args.Target;
            var usew = getCheckBoxItem(comboMenu, "usew");
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) return;

            if (args.SData.IsAutoAttack())
            {
                if (target == null) return;
                {
                    if (W.IsReady() && usew)
                    {
                        W.Cast();
                        Orbwalker.ResetAutoAttack();
                    }
                }
            }
        }

        private static bool eCanCast()
        {
            return Player.HasBuff("forcepulsecancast");
        }

        public static int ForcePulseCount()
        {
            var manacost = Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
            switch (manacost.ToString(CultureInfo.InvariantCulture))
            {
                case "50":
                    return 0;
                case "100":
                    return 1;
                case "200":
                    return 2;
                case "400":
                    return 4;
                case "800":
                    return 5;
            }
            return 0;
        }


        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (getKeyBindItem(miscMenu, "fleemode") || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                fleeMode();
            }

            Killsteal();
        }

        private static void JungleClear()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.FirstOrDefault() == null) return;

            var useq = getCheckBoxItem(farmMenu, "useqj");
            var usee = getCheckBoxItem(farmMenu, "useej");
            var user = getCheckBoxItem(farmMenu, "userj");
            var mana = getSliderItem(farmMenu, "minmanajungleclear");
            var rslider = getSliderItem(farmMenu, "rcountj");
            if (Player.ManaPercent < mana) return;
            if (Orbwalker.IsAutoAttacking) return;

            if (useq)
            {
                if (Player.LSDistance(minions[0]) < Orbwalking.GetRealAutoAttackRange(minions[0]) && !W.IsReady())
                {
                    Q.Cast(minions[0]);
                }
                else
                {
                    Q.Cast(minions[0]);
                }
            }

            if (usee && eCanCast())
            {
                if (E.IsReady())
                {
                    E.Cast(minions[0]);
                }
            }

            if (user)
            {
                if (ForcePulseCount() >= rslider) return;
                if (R.IsReady())
                {
                    R.Cast(minions[0]);
                }
            }
        }

        private static void LastHit()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            if (minions.FirstOrDefault() == null) return;
            var useq = getCheckBoxItem(farmMenu, "useqlh");
            var mana = getSliderItem(farmMenu, "minmanalasthit");
            if (Player.ManaPercent < mana) return;

            if (useq)
            {
                if (minions.FirstOrDefault() == null) return;
                if (minions[0].Health >= Q.GetDamage(minions[0])) return;

                if (Player.LSDistance(minions[0]) < Orbwalking.GetRealAutoAttackRange(minions[0]) && !W.IsReady())
                {
                    Q.Cast(minions[0]);
                }
                else
                {
                    Q.Cast(minions[0]);
                }
            }
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            var useq = getCheckBoxItem(farmMenu, "useql");
            if (useq)
            {
                foreach (var mins in minions)
                {
                    if (mins.Health <= Q.GetDamage(mins))
                    {
                        if (mins.Health >= Player.GetAutoAttackDamage(mins) + 50 &&
                            mins.LSDistance(Player) <= Orbwalking.GetRealAutoAttackRange(mins))
                        {
                            Q.Cast(mins);
                        }

                        if (mins.LSDistance(Player) >= Orbwalking.GetRealAutoAttackRange(mins) + 100)
                        {
                            Q.Cast(mins);
                        }
                    }
                }
            }

            if (minions.FirstOrDefault() == null) return;


            if (Player.ManaPercent < getSliderItem(farmMenu, "minmanalaneclear")) return;

            var usee = getCheckBoxItem(farmMenu, "useel");
            var user = getCheckBoxItem(farmMenu, "userl");
            var useeslider = getSliderItem(farmMenu, "useels");
            var userslider = getSliderItem(farmMenu, "userls");
            var count = getSliderItem(farmMenu, "rcountl");

            if (usee)
            {
                if (E.IsReady() && eCanCast())
                {
                    var miniosn =
                        MinionManager.GetMinions(Player.Position, 400);
                    {
                        if (miniosn.FirstOrDefault() != null)
                        {
                            var predict = E.GetCircularFarmLocation(miniosn, 500);
                            var minhit = predict.MinionsHit;
                            if (minhit >= useeslider)
                            {
                                E.Cast(predict.Position);
                            }
                        }
                    }
                }
            }

            if (user)
            {
                if (ForcePulseCount() >= count) return;
                if (R.IsReady())
                {
                    var min = MinionManager.GetMinions(Player.Position, R.Range);
                    if (min.FirstOrDefault() != null)
                    {
                        var prediction = R.GetCircularFarmLocation(min, R.Width);
                        var predict = prediction.MinionsHit;
                        if (predict >= userslider)
                        {
                            R.Cast(prediction.Position);
                        }
                    }
                }
            }
        }

        private static void Killsteal()
        {
            var target = TargetSelector.GetTarget(Q.Range + 500, DamageType.Magical);
            if (!target.IsValidTarget(Q.Range + 500)) return;
            var extendedposition = Player.Position.Extend(target.ServerPosition, 500);
            var ks = getCheckBoxItem(ksMenu, "ks");
            if (!ks) return;
            var qks = getCheckBoxItem(ksMenu, "qks");
            var rks = getCheckBoxItem(ksMenu, "rks");
            var eks = getCheckBoxItem(ksMenu, "eks");
            var rgks = getCheckBoxItem(ksMenu, "rgks");
            if (target.LSDistance(Player) > Q.Range - 20 && rgks)
            {
                if ((target.Health < Q.GetDamage(target) && Q.IsReady()) ||
                    (target.Health < E.GetDamage(target) && E.IsReady()))
                    R.Cast(extendedposition);
            }

            if (target.Health < Q.GetDamage(target) && target.IsValidTarget(Q.Range))
            {
                if (qks)
                    Q.Cast(target);
            }

            if (target.Health < E.GetDamage(target) && eCanCast() && target.LSDistance(Player) < 500)
            {
                if (eks)
                    E.Cast(target.Position);
            }

            if (target.Health < R.GetDamage(target) && R.IsReady() && target.IsValidTarget(700))
            {
                if (rks)
                    R.Cast(extendedposition);
            }
        }

        private static void fleeMode()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var extendedposition = Player.Position.Extend(Game.CursorPos, 500);

            if (R.IsReady())
            {
                R.Cast(extendedposition);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null) return;

            var mana = getSliderItem(harassMenu, "harassmana");
            if (Player.ManaPercent < mana) return;
            var useq = getCheckBoxItem(harassMenu, "useqharass");
            var usee = getCheckBoxItem(harassMenu, "useeharass");

            if (useq)
            {
                if (Q.IsReady())
                {
                    Q.Cast(target);
                }
            }

            if (E.IsReady() && eCanCast() && target.LSDistance(Player) < 500)
            {
                if (usee)
                    E.Cast(target);
            }
        }

        public static SpellSlot GetIgniteSlot()
        {
            return _ignite;
        }

        public static void SetIgniteSlot(SpellSlot nSpellSlot)
        {
            _ignite = nSpellSlot;
        }

        public static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range + 200, DamageType.Magical);
            if (target == null) return;
            var useq = getCheckBoxItem(comboMenu, "useq");
            var user = getCheckBoxItem(comboMenu, "user");
            var usee = getCheckBoxItem(comboMenu, "usee");
            var userturret = getCheckBoxItem(comboMenu, "usert");
            var ignite = getCheckBoxItem(comboMenu, "useignite");
            if (Orbwalker.IsAutoAttacking) return;

            SetIgniteSlot(Player.GetSpellSlot("summonerdot"));

            if (ignite)
            {
                if (target.IsValidTarget(Q.Range) &&
                    (target.Health < IgniteDamage(target) + Q.GetDamage(target)))
                    Player.Spellbook.CastSpell(GetIgniteSlot(), target);
            }

            if (Q.IsReady() && useq && target.IsValidTarget(Q.Range))
            {
                if (Player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target) && !W.IsReady())
                    Q.Cast(target);
                else if (Player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(target))
                {
                    Q.Cast(target);
                }
            }

            if (E.IsReady() && usee && target.LSDistance(Player) < 500 && eCanCast())
            {
                if (Player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target) && !W.IsReady())
                    E.Cast(target.Position);
                else if (Player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(target))
                {
                    Utility.DelayAction.Add(200, () => E.Cast(target.Position));
                }
            }

            var rCount = getSliderItem(comboMenu, "rcount");
            var extendedposition = Player.Position.LSExtend(target.Position, 500);
            if (ForcePulseCount() < rCount && user && R.IsReady())
            {
                if (userturret)
                {
                    if (target.UnderTurret(true))
                    {
                        return;
                    }
                }
                if (Player.HealthPercent > getSliderItem(comboMenu, "rhp"))
                {
                    if (Q.IsReady() || E.IsReady() || W.IsReady())
                    {
                        R.Cast(extendedposition);
                    }
                }
            }
        }

        public static void Drawing_OnDrawChamp(EventArgs args)
        {
            if (!getCheckBoxItem(drawMenu, "drawdamage"))
                return;

            var target = TargetSelector.GetTarget(4000, DamageType.Magical);
            if (target == null)
                return;

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = DamageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = "Killable " + (int)(unit.Health - damage);
                    Text.OnEndScene();
                }
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);
                var differenceInHp = xPosCurrentHp - xPosDamage;
                var pos1 = barPos.X + 9 + 107 * percentHealthAfterDamage;
                for (var i = 0; i < differenceInHp; i++)
                {
                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                }
            }
        }
    }
}