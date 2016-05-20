using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Environment = System.Environment;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Zac
    {
        public static Menu config;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static int[] eRanges = {1150, 1300, 1450, 1600, 1750};
        public static float[] eChannelTimes = {0.9f, 1.05f, 1.2f, 1.35f, 1.5f};
        public static Vector3 farmPos;
        public static float zacETime;

        public static Menu menuD, menuC, menuH, menuLC, menuM;

        public Zac()
        {
            InitZac();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
        }

        private static bool rActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "ZacR"); }
        }

        private static bool eActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "ZacE"); }
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "ZacE")
            {
                if (zacETime == 0f)
                {
                    zacETime = Environment.TickCount;
                    Utility.DelayAction.Add(4000, () => { zacETime = 0f; });
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && getCheckBoxItem(menuM, "Interrupt") && sender.LSDistance(player) < R.Range)
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void InitZac()
        {
            Q = new Spell(SpellSlot.Q, 550);
            Q.SetSkillshot(0.55f, 120, float.MaxValue, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 320);
            E = new Spell(SpellSlot.E);
            E.SetSkillshot(0.75f, 230, 1500, false, SkillshotType.SkillshotCircle);
            E.SetCharged("ZacE", "ZacE", 295, eRanges[0], eChannelTimes[0]);
            R = new Spell(SpellSlot.R, 300);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (E.IsCharging || eActive)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }
            else
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }
        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (getCheckBoxItem(menuH, "useqH") && Q.CanCast(target))
            {
                Q.CastIfHitchanceEquals(target, HitChance.Medium, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuH, "usewH") && W.IsReady())
            {
                if (player.LSDistance(target) < W.Range)
                {
                    W.Cast(getCheckBoxItem(config, "packets"));
                }
            }
        }

        private void Clear()
        {
            var target = Jungle.GetNearest(player.Position, GetTargetRange());
            if (getCheckBoxItem(menuLC, "useqLC") && Q.IsReady() && !E.IsCharging)
            {
                if (target != null && Q.CanCast(target))
                {
                    Q.Cast(target.Position, getCheckBoxItem(config, "packets"));
                }
                else
                {
                    var bestPositionQ =
                        Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
                    if (bestPositionQ.MinionsHit >= getSliderItem(menuLC, "qMinHit"))
                    {
                        Q.Cast(bestPositionQ.Position, getCheckBoxItem(config, "packets"));
                    }
                }
            }
            if (getCheckBoxItem(menuLC, "usewLC") && W.IsReady() && !E.IsCharging)
            {
                if (target != null && target.LSDistance(player) < W.Range)
                {
                    W.Cast(getCheckBoxItem(config, "packets"));
                }
                else
                {
                    if (Helpers.Environment.Minion.countMinionsInrange(player.Position, W.Range) >=
                        getSliderItem(menuLC, "wMinHit"))
                    {
                        W.Cast(getCheckBoxItem(config, "packets"));
                    }
                }
            }
            if (getCheckBoxItem(menuLC, "collectBlobs") && !E.IsCharging)
            {
                var blob =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            o =>
                                !o.IsDead && o.IsValid && o.Name == "BlobDrop" && o.Team == player.Team &&
                                o.LSDistance(player) < Orbwalking.GetRealAutoAttackRange(player))
                        .OrderBy(o => o.LSDistance(player))
                        .FirstOrDefault();
                if (blob != null && !Orbwalker.CanAutoAttack && !Orbwalker.IsAutoAttacking)
                {
                    Orbwalker.DisableMovement = true;
                    Player.IssueOrder(GameObjectOrder.MoveTo, blob.Position);
                }
            }
            if (getCheckBoxItem(menuLC, "useeLC") && E.IsReady())
            {
                if (target != null && target.IsValidTarget())
                {
                    CastE(target);
                }
                else
                {
                    var bestPositionE =
                        E.GetCircularFarmLocation(
                            MinionManager.GetMinions(eRanges[E.Level - 1], MinionTypes.All, MinionTeam.NotAlly));
                    var castPos = Vector3.Zero;
                    if (bestPositionE.MinionsHit < getSliderItem(menuLC, "eMinHit") &&
                        farmPos.IsValid())
                    {
                        castPos = farmPos;
                    }
                    if (bestPositionE.MinionsHit >= getSliderItem(menuLC, "eMinHit"))
                    {
                        castPos = bestPositionE.Position.To3D();
                    }
                    if (castPos.IsValid())
                    {
                        farmPos = bestPositionE.Position.To3D();
                        Utility.DelayAction.Add(5000, () => { farmPos = Vector3.Zero; });
                        CastE(castPos);
                    }
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = null;
            target = E.IsCharging ? TargetSelector.GetTarget(GetTargetRange(), DamageType.Magical) : TargetSelector.GetTarget(GetTargetRange(), DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuC, "usew") && W.CanCast(target) && !E.IsCharging)
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !Q.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }

            if (rActive)
            {
                Orbwalker.DisableAttacking = true;
                return;
            }

            if (getCheckBoxItem(menuC, "usee") && E.IsReady() && player.CanMove)
            {
                CastE(target);
            }
            if (getCheckBoxItem(menuC, "useq") && Q.CanCast(target) && target.IsValidTarget() &&
                !E.IsCharging)
            {
                Q.CastIfHitchanceEquals(target, HitChance.Medium, getCheckBoxItem(config, "packets"));
            }

            if (R.IsReady() && getCheckBoxItem(menuC, "user") &&
                getSliderItem(menuC, "Rmin") <= player.CountEnemiesInRange(R.Range) &&
                !target.HasBuffOfType(BuffType.Knockback) && !target.HasBuffOfType(BuffType.Knockup) &&
                !target.HasBuffOfType(BuffType.Stun))
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (target.LSDistance(player) > eRanges[E.Level - 1])
            {
                return;
            }
            var eFlyPred = E.GetPrediction(target);
            var enemyPred = Prediction.GetPrediction(
                target, eChannelTimes[E.Level - 1] + target.LSDistance(player)/E.Speed/1000);
            if (E.IsCharging)
            {
                if (!eFlyPred.CastPosition.IsValid() || eFlyPred.CastPosition.IsWall())
                {
                    return;
                }
                if (eFlyPred.CastPosition.LSDistance(player.Position) < E.Range)
                {
                    E.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
                }
                else if (eFlyPred.UnitPosition.LSDistance(player.Position) < E.Range && target.LSDistance(player) < 500f)
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium, getCheckBoxItem(config, "packets"));
                }
                else if ((eFlyPred.CastPosition.LSDistance(player.Position) < E.Range &&
                          eRanges[E.Level - 1] - eFlyPred.CastPosition.LSDistance(player.Position) < 200) ||
                         (CombatHelper.GetAngle(player, eFlyPred.CastPosition) > 35))
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium, getCheckBoxItem(config, "packets"));
                }
                else if (eFlyPred.CastPosition.LSDistance(player.Position) < E.Range && zacETime != 0 &&
                         Environment.TickCount - zacETime > 2500)
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium, getCheckBoxItem(config, "packets"));
                }
            }
            else if (enemyPred.UnitPosition.LSDistance(player.Position) < eRanges[E.Level - 1] &&
                     getSliderItem(menuC, "Emin") < target.LSDistance(player.Position))
            {
                E.SetCharged("ZacE", "ZacE", 300, eRanges[E.Level - 1], eChannelTimes[E.Level - 1]);
                E.StartCharging(eFlyPred.UnitPosition);
            }
        }

        private void CastE(Vector3 target)
        {
            if (target.LSDistance(player.Position) > eRanges[E.Level - 1])
            {
                return;
            }
            if (E.IsCharging)
            {
                if (target.LSDistance(player.Position) < E.Range)
                {
                    E.Cast(target, getCheckBoxItem(config, "packets"));
                }
            }
            else if (target.LSDistance(player.Position) < eRanges[E.Level - 1])
            {
                E.SetCharged("ZacE", "ZacE", 295, eRanges[E.Level - 1], eChannelTimes[E.Level - 1]);
                E.StartCharging(target);
            }
        }

        private float GetTargetRange()
        {
            if (E.IsReady())
            {
                return eRanges[E.Level - 1];
            }
            return 600;
        }

        private float GetERange()
        {
            if (E.Level > 0)
            {
                return eRanges[E.Level - 1];
            }
            return eRanges[0];
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawww"), W.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), GetERange(), Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawrr"), R.Range, Color.FromArgb(180, 100, 146, 166));
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

        private void InitMenu()
        {
            config = MainMenu.AddMenu("Zac ", "Zac");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawww", new CheckBox("Draw W range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawrr", new CheckBox("Draw R range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));

            // Combo Settings
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usew", new CheckBox("Use W", false));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("Emin", new Slider("   E min range", 300, 0, 1550));
            menuC.Add("user", new CheckBox("Use R"));
            menuC.Add("Rmin", new Slider("   R min", 2, 1, 5));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q"));
            menuH.Add("usewH", new CheckBox("Use W"));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));
            menuLC.Add("qMinHit", new Slider("   Q min hit", 3, 1, 6));
            menuLC.Add("usewLC", new CheckBox("Use W"));
            menuLC.Add("wMinHit", new Slider("   W min hit", 3, 1, 6));
            menuLC.Add("useeLC", new CheckBox("Use E"));
            menuLC.Add("eMinHit", new Slider("   E min hit", 3, 1, 6));
            menuLC.Add("collectBlobs", new CheckBox("Collect nearby blobs"));

            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("Interrupt", new CheckBox("Cast R to interrupt spells"));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}