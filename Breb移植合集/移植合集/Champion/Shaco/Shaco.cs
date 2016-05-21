using System;
using System.Collections.Generic;
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
    internal class Shaco
    {
        public static Menu config;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static bool hasGhost = false;
        public static bool GhostDelay;
        public static int GhostRange = 2200;
        public static int LastAATick;
        public static float cloneTime, lastBox;
        public static IncomingDamage IncDamages;

        public static Menu menuD, menuC, menuH, menuLC, menuM;

        public Shaco()
        {
            IncDamages = new IncomingDamage();
            InitShaco();
            InitMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static bool ShacoClone
        {
            get { return player.Spellbook.GetSpell(SpellSlot.R).Name == "HallucinateGuide"; }
        }

        private static bool ShacoStealth
        {
            get { return player.HasBuff("Deceive"); }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (ShacoClone)
            {
                var clone = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(m => m.Name == player.Name && !m.IsMe);

                if (args == null || clone == null)
                {
                    return;
                }
                if (hero.NetworkId != clone.NetworkId)
                {
                    return;
                }
                LastAATick = Utils.GameTimeTickCount;
            }
            if (hero.IsMe && args.SData.Name == "JackInTheBox")
            {
                lastBox = Environment.TickCount;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range + player.MoveSpeed*3, DamageType.Physical);
            if (ShacoStealth && target != null && target.Health > ComboDamage(target) &&
                CombatHelper.IsFacing(target, player.Position) &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableAttacking = false;
            }

            if (!ShacoClone)
            {
                cloneTime = Environment.TickCount;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo(target);
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

            if (E.IsReady())
            {
                var ksTarget =
                    HeroManager.Enemies.FirstOrDefault(
                        h => h.IsValidTarget() && !CombatHelper.IsInvulnerable2(h) && h.Health < E.GetDamage(h));
                if (ksTarget != null)
                {
                    if ((getCheckBoxItem(menuM, "ks") || getCheckBoxItem(menuM, "ksq")) &&
                        E.CanCast(ksTarget))
                    {
                        E.Cast(ksTarget);
                    }
                    if (Q.IsReady() && getCheckBoxItem(menuM, "ks") &&
                        ksTarget.LSDistance(player) < Q.Range + E.Range && ksTarget.LSDistance(player) > E.Range &&
                        !player.Position.Extend(ksTarget.Position, Q.Range).IsWall() &&
                        player.Mana > Q.Instance.SData.Mana + E.Instance.SData.Mana)
                    {
                        Q.Cast(player.Position.Extend(ksTarget.Position, Q.Range));
                    }
                }
            }

            if (getKeyBindItem(menuM, "stackBox") && W.IsReady())
            {
                var box =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.LSDistance(player) < W.Range && m.Name == "Jack In The Box" && !m.IsDead)
                        .OrderBy(m => m.LSDistance(Game.CursorPos))
                        .FirstOrDefault();

                if (box != null)
                {
                    W.Cast(box.Position);
                }
                else
                {
                    if (player.LSDistance(Game.CursorPos) < W.Range)
                    {
                        W.Cast(Game.CursorPos);
                    }
                    else
                    {
                        W.Cast(player.Position.Extend(Game.CursorPos, W.Range));
                    }
                }
            }

            if (ShacoClone && !GhostDelay && getCheckBoxItem(menuM, "autoMoveClone"))
            {
                moveClone();
            }
            var data = IncDamages.GetAllyData(player.NetworkId);
            if (getCheckBoxItem(menuC, "userCC") && R.IsReady() && target != null && player.LSDistance(target) < Q.Range &&
                data.AnyCC)
            {
                R.Cast();
            }
        }

        private void Combo(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            var cmbDmg = ComboDamage(target);
            var dist = (float) (Q.Range + player.MoveSpeed*2.5);
            if (ShacoClone && !GhostDelay && getCheckBoxItem(menuC, "useClone") &&
                !getCheckBoxItem(menuM, "autoMoveClone"))
            {
                moveClone();
            }
            if (getCheckBoxItem(menuC, "WaitForStealth") && ShacoStealth && cmbDmg < target.Health)
            {
                return;
            }
            if (getCheckBoxItem(menuC, "useq") && Q.IsReady() &&
                Game.CursorPos.LSDistance(target.Position) < 250 && target.LSDistance(player) < dist &&
                (target.LSDistance(player) >= getSliderItem(menuC, "useqMin") ||
                 (cmbDmg > target.Health && player.CountEnemiesInRange(2000) == 1)))
            {
                if (target.LSDistance(player) < Q.Range)
                {
                    Q.Cast(Prediction.GetPrediction(target, 0.5f).UnitPosition, getCheckBoxItem(config, "packets"));
                }
                else
                {
                    if (!CheckWalls(target) || Helpers.Environment.Map.GetPath(player, target.Position) < dist)
                    {
                        Q.Cast(
                            player.Position.Extend(target.Position, Q.Range), getCheckBoxItem(config, "packets"));
                    }
                }
            }
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "usew") && W.IsReady() && !target.UnderTurret(true) &&
                target.Health > cmbDmg && player.LSDistance(target) < W.Range)
            {
                HandleW(target);
            }
            if (getCheckBoxItem(menuC, "usee") && E.CanCast(target))
            {
                E.CastOnUnit(target, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuC, "user") && R.IsReady() && !ShacoClone && target.HealthPercent < 75 &&
                cmbDmg < target.Health && target.HealthPercent > cmbDmg && target.HealthPercent > 25)
            {
                R.Cast(getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuC, "useIgnite") &&
                player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health && hasIgnite)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private void moveClone()
        {
            var Gtarget = TargetSelector.GetTarget(2200, DamageType.Physical);
            switch (getBoxItem(menuM, "ghostTarget"))
            {
                case 0:
                    Gtarget = TargetSelector.GetTarget(GhostRange, DamageType.Physical);
                    break;
                case 1:
                    Gtarget =
                        ObjectManager.Get<AIHeroClient>()
                            .Where(i => i.IsEnemy && !i.IsDead && player.LSDistance(i) <= GhostRange)
                            .OrderBy(i => i.Health)
                            .FirstOrDefault();
                    break;
                case 2:
                    Gtarget =
                        ObjectManager.Get<AIHeroClient>()
                            .Where(i => i.IsEnemy && !i.IsDead && player.LSDistance(i) <= GhostRange)
                            .OrderBy(i => player.LSDistance(i))
                            .FirstOrDefault();
                    break;
                default:
                    break;
            }
            var clone = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(m => m.Name == player.Name && !m.IsMe);
            if (clone != null && Gtarget != null && Gtarget.IsValid)
            {
                if (clone.CanMove)
                {
                    Player.IssueOrder(GameObjectOrder.MovePet, Gtarget);
                }

                if (clone.LSDistance(Gtarget) < 130 && (CanCloneAttack(clone) || clone.CanAttack))
                {
                    Player.IssueOrder(GameObjectOrder.AutoAttackPet, Gtarget);
                }

                if (player.HealthPercent > 25)
                {
                    var prediction = Prediction.GetPrediction(Gtarget, 2);
                    R.Cast(Gtarget.Position.LSExtend(prediction.UnitPosition, Orbwalking.GetRealAutoAttackRange(Gtarget)), getCheckBoxItem(config, "packets"));
                }

                GhostDelay = true;
                Utility.DelayAction.Add(200, () => GhostDelay = false);
            }
        }

        private bool CheckWalls(AIHeroClient target)
        {
            var step = player.LSDistance(target)/15;
            for (var i = 1; i < 16; i++)
            {
                if (player.Position.Extend(target.Position, step*i).IsWall())
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleW(AIHeroClient target)
        {
            var turret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .OrderByDescending(t => t.LSDistance(target))
                    .FirstOrDefault(t => t.IsEnemy && t.LSDistance(target) < 3000 && !t.IsDead);
            if (turret != null)
            {
                CastW(target, target.Position, turret.Position);
            }
            else
            {
                if (target.IsMoving)
                {
                    var pred = Prediction.GetPrediction(target, 2);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        CastW(target, target.Position, pred.UnitPosition);
                    }
                }
                else
                {
                    W.Cast(player.Position.Extend(target.Position, W.Range - player.LSDistance(target)));
                }
            }
        }

        public static bool CanCloneAttack(Obj_AI_Minion clone)
        {
            if (clone != null)
            {
                return Utils.GameTimeTickCount >=
                       LastAATick + Game.Ping + 100 + (clone.AttackDelay - clone.AttackCastDelay)*1000;
            }
            return false;
        }

        private void CastW(AIHeroClient target, Vector3 from, Vector3 to)
        {
            var positions = new List<Vector3>();

            for (var i = 1; i < 11; i++)
            {
                positions.Add(from.LSExtend(to, 42*i));
            }
            var best =
                positions.OrderByDescending(p => p.LSDistance(target.Position))
                    .FirstOrDefault(
                        p => !p.IsWall() && p.LSDistance(player.Position) < W.Range && p.LSDistance(target.Position) > 350);
            if (best != null && best.IsValid())
            {
                W.Cast(best, getCheckBoxItem(config, "packets"));
            }
        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuH, "useeH") && E.CanCast(target))
            {
                E.Cast(target, getCheckBoxItem(config, "packets"));
            }
        }

        private void Clear()
        {
            var bestPosition =
                W.GetCircularFarmLocation(MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly), 300);
            if (getCheckBoxItem(menuLC, "usewLC") && W.IsReady() &&
                bestPosition.MinionsHit > getSliderItem(menuLC, "whitLC"))
            {
                W.Cast(bestPosition.Position, getCheckBoxItem(config, "packets"));
            }

            var mob = Jungle.GetNearest(player.Position);

            if (getCheckBoxItem(menuLC, "useeLC") && E.IsReady() && mob != null && mob.Health < E.GetDamage(mob))
            {
                E.Cast(mob, getCheckBoxItem(config, "packets"));
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(180, 109, 111, 126));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawww"), W.Range, Color.FromArgb(180, 109, 111, 126));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(180, 109, 111, 126));
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

        private float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;

            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.E);
            }

            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void InitShaco()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 425);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R);
        }

        private void InitMenu()
        {
            config = MainMenu.AddMenu("Shaco", "Shaco");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.Add("drawww", new CheckBox("Draw W range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));

            // Combo Settings
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("useqMin", new Slider("   Min range", 200, 0, 400));
            menuC.Add("usew", new CheckBox("Use W"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("user", new CheckBox("Use R"));
            menuC.Add("useClone", new CheckBox("   Move clone"));
            menuC.Add("userCC", new CheckBox("   Dodge targeted CC"));
            menuC.Add("WaitForStealth", new CheckBox("Block spells in stealth"));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useeH", new CheckBox("Use E"));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("usewLC", new CheckBox("Use W"));
            menuLC.Add("whitLC", new Slider("   Min mob", 2, 1, 5));
            menuLC.Add("useeLC", new CheckBox("Use E to secure buff"));

            // Misc Settings
            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("ghostTarget", new ComboBox("Ghost target priority", 0, "Targetselector", "Lowest health", "Closest to you"));
            menuM.Add("ksq", new CheckBox("KS E"));
            menuM.Add("ks", new CheckBox("KS Q+E"));
            menuM.Add("autoMoveClone", new CheckBox("Always move clone", false));
            menuM.Add("stackBox", new KeyBind("Stack boxes", false, KeyBind.BindTypes.HoldActive, 'T'));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}