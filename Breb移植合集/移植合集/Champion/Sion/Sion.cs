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
using Environment = UnderratedAIO.Helpers.Environment;
using Geometry = LeagueSharp.Common.Geometry;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Sion
    {
        public const int qWidth = 350;
        public static Menu config;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justQ, justE;
        public static float qStart;
        public static IncomingDamage IncDamages;
        public static Vector2 QCastPos = new Vector2();

        public static Menu menuD, menuC, menuH, menuLC, menuM;
        public Vector3 lastQPos;
        public double[] Rwave = {50, 70, 90};

        public Sion()
        {
            IncDamages = new IncomingDamage();
            InitSion();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Game.OnProcessPacket += Game_OnProcessPacket;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static bool activatedR
        {
            get { return player.HasBuff("SionR"); }
        }

        private static bool activatedW
        {
            get { return player.Spellbook.GetSpell(SpellSlot.W).Name == "sionwdetonate"; }
        }

        private static bool activatedP
        {
            get { return player.Spellbook.GetSpell(SpellSlot.Q).Name == "sionpassivespeed"; }
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint) WindowsMessages.WM_RBUTTONDOWN && Q.IsCharging)
            {
                Q.Cast(Game.CursorPos);
            }
        }

        private void Game_OnProcessPacket(GamePacketEventArgs args)
        {
            if (getCheckBoxItem(menuM, "NoRlock") && args.PacketData[0] == 0x83 && args.PacketData[7] == 0x47 &&
                args.PacketData[8] == 0x47)
            {
                args.Process = false;
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(menuM, "usewgc") && gapcloser.End.LSDistance(player.Position) < 200)
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void InitSion()
        {
            Q = new Spell(SpellSlot.Q, 740);
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("SionQ", "SionQ", 350, 740, 0.6f);
            W = new Spell(SpellSlot.W, 490);
            E = new Spell(SpellSlot.E, 775);
            E.SetSkillshot(0.25f, 80f, 1800, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Q.IsCharging || activatedR)
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }

            var data = IncDamages.GetAllyData(player.NetworkId);
            if (data != null && !activatedW && getCheckBoxItem(menuM, "AshieldB") && data.DamageCount >= getSliderItem(menuM, "wMinAggro") && player.ManaPercent > getSliderItem(menuM, "minmanaAgg"))
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }

            if (data != null && !activatedW && getCheckBoxItem(menuM, "AshieldB") && W.IsReady() && (data.DamageTaken > player.Health || data.DamageTaken > getWShield()/100f*getSliderItem(menuM, "AshieldDmg")))
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }

            if (data != null && activatedW && data.DamageTaken > player.GetBuff("sionwshieldstacks").Count && data.DamageTaken < player.Health)
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
        }


        private void Harass()
        {
            var perc = getSliderItem(menuH, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc || Orbwalker.IsAutoAttacking)
            {
                return;
            }
            var target = TargetSelector.GetTarget(1500, DamageType.Physical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }
            if (Q.IsReady() && getCheckBoxItem(menuH, "useqH"))
            {
                castQ(target);
            }
            if (getCheckBoxItem(menuH, "useeH"))
            {
                CastEHero(target);
            }
        }

        private void castQ(AIHeroClient target)
        {
            if (target == null && Q.IsCharging)
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Q);
            }
            if (Q.IsCharging)
            {
                var qTarget = TargetSelector.GetTarget(!Q.IsCharging ? Q.ChargedMaxRange / 2 : Q.ChargedMaxRange, DamageType.Physical);
                if (qTarget == null) return;
                if (qTarget == null && Q.IsCharging)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Q);
                }
                var start = ObjectManager.Player.ServerPosition.To2D();
                var end = start.Extend(QCastPos, Q.Range);
                var direction = (end - start).Normalized();
                var normal = direction.Perpendicular();

                var points = new List<Vector2>();
                var hitBox = qTarget.BoundingRadius;
                points.Add(start + normal * (Q.Width + hitBox));
                points.Add(start - normal * (Q.Width + hitBox));
                points.Add(end + Q.ChargedMaxRange * direction - normal * (Q.Width + hitBox));
                points.Add(end + Q.ChargedMaxRange * direction + normal * (Q.Width + hitBox));

                for (var i = 0; i <= points.Count - 1; i++)
                {
                    var A = points[i];
                    var B = points[i == points.Count - 1 ? 0 : i + 1];

                    if (qTarget.ServerPosition.To2D().LSDistance(A, B, true, true) < 50 * 50)
                    {
                        Q.Cast(qTarget, true);
                    }
                }
                checkCastedQ(target);
            }
            else if (Q.CanCast(target) && !Orbwalker.IsAutoAttacking && target != null)
            {
                var qPred = Prediction.GetPrediction(target, 0.3f);
                var qPred2 = Prediction.GetPrediction(target, 0.6f);
                var poly = GetPoly(qPred.UnitPosition);
                if (qPred2.Hitchance >= HitChance.High && poly.IsInside(qPred2.UnitPosition.To2D()) && poly.IsInside(target.ServerPosition))
                {
                    Q.StartCharging(qPred.CastPosition);
                }
            }
        }

        private void Clear()
        {
            var perc = getSliderItem(menuLC, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc || Orbwalker.IsAutoAttacking)
            {
                return;
            }
            if (getCheckBoxItem(menuLC, "useqLC"))
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                var bestPositionQ = Q.GetLineFarmLocation(minions, qWidth);

                if (bestPositionQ.MinionsHit >= getSliderItem(menuLC, "qMinHit") && !Q.IsCharging)
                {
                    Q.StartCharging(bestPositionQ.Position.To3D());
                    return;
                }
                if (Q.IsCharging && minions.Count(m => HealthPrediction.GetHealthPrediction(m, 500) < 0) > 0)
                {
                    var qMini = minions.FirstOrDefault();
                    if (qMini != null)
                    {
                        Q.Cast(qMini.Position, getCheckBoxItem(config, "packets"));
                    }
                }
            }

            if (getCheckBoxItem(menuLC, "useeLC") && E.IsReady() && !Q.IsCharging)
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                var bestPositionE = E.GetLineFarmLocation(minions);
                if (bestPositionE.MinionsHit >= getSliderItem(menuLC, "eMinHit"))
                {
                    E.Cast(bestPositionE.Position);
                    return;
                }
            }
            if (W.IsReady() && !activatedW && getCheckBoxItem(menuLC, "usewLC") &&
                getSliderItem(menuLC, "wMinHit") <=
                Environment.Minion.countMinionsInrange(player.Position, W.Range))
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
            if (W.IsReady() && activatedW && getCheckBoxItem(menuLC, "usewLC") &&
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Count(m => HealthPrediction.GetHealthPrediction(m, 500) < 0) > 0)
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private void Combo()
        {
            if (getCheckBoxItem(menuC, "user") && R.IsReady())
            {
                var rTarget = TargetSelector.GetTarget(2500, DamageType.Physical);
                if (rTarget == null) return;
                if (!activatedR && !Orbwalker.IsAutoAttacking)
                {
                    if (rTarget != null && !rTarget.IsInvulnerable && !rTarget.MagicImmune &&  rTarget.LSDistance(Game.CursorPos) < 300)
                    {
                        if (player.LSDistance(rTarget) + 100 > Environment.Map.GetPath(player, rTarget.Position) && ComboDamage(rTarget) > rTarget.Health && !CombatHelper.IsCollidingWith( player, rTarget.Position.LSExtend(player.Position, player.BoundingRadius + 15), player.BoundingRadius, new[] {CollisionableObjects.Heroes, CollisionableObjects.Walls}) && (ComboDamage(rTarget) - R.GetDamage(rTarget) < rTarget.Health || rTarget.LSDistance(player) > 400 || player.HealthPercent < 25) && rTarget.CountAlliesInRange(2500) + 1 >= rTarget.CountEnemiesInRange(2500))
                        {
                            R.Cast(rTarget.Position);
                        }
                    }
                }
            }

            var target = TargetSelector.GetTarget(1500, DamageType.Physical);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            var data = IncDamages.GetAllyData(player.NetworkId);
            if (!activatedW && W.IsReady() && getCheckBoxItem(menuC, "usew"))
            {
                if (data.DamageTaken > player.Health ||
                    (data.DamageTaken > getWShield()/100*getSliderItem(menuC, "shieldDmg")) ||
                    (target.LSDistance(player) < W.Range && getCheckBoxItem(menuC, "usewir")))
                {
                    W.Cast(getCheckBoxItem(config, "packets"));
                }
            }
            if (activatedW && getCheckBoxItem(menuC, "usew") && W.IsReady() &&
                player.LSDistance(target) < W.Range &&
                (target.Health < W.GetDamage(target) ||
                 (W.IsInRange(target) && !W.IsInRange(Prediction.GetPrediction(target, 0.2f).UnitPosition))))
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }

            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (activatedP)
            {
                if (Q.IsReady() && player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(target))
                {
                    Q.Cast(getCheckBoxItem(config, "packets"));
                }
                return;
            }

            var qTarget = TargetSelector.GetTarget(!Q.IsCharging ? Q.ChargedMaxRange / 2 : Q.ChargedMaxRange, DamageType.Physical);

            if (qTarget == null && Q.IsCharging)
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Q);
            }
            if (Q.IsCharging)
            {
                var start = ObjectManager.Player.ServerPosition.To2D();
                var end = start.Extend(QCastPos, Q.Range);
                var direction = (end - start).Normalized();
                var normal = direction.Perpendicular();

                var points = new List<Vector2>();
                var hitBox = qTarget.BoundingRadius;
                points.Add(start + normal * (Q.Width + hitBox));
                points.Add(start - normal * (Q.Width + hitBox));
                points.Add(end + Q.ChargedMaxRange * direction - normal * (Q.Width + hitBox));
                points.Add(end + Q.ChargedMaxRange * direction + normal * (Q.Width + hitBox));

                for (var i = 0; i <= points.Count - 1; i++)
                {
                    var A = points[i];
                    var B = points[i == points.Count - 1 ? 0 : i + 1];

                    if (qTarget.ServerPosition.To2D().LSDistance(A, B, true, true) < 50 * 50)
                    {
                        Q.Cast(qTarget, true);
                    }
                }
                checkCastedQ(target);
                return;
            }
            if (activatedR)
            {
                return;
            }
            if (getCheckBoxItem(menuC, "usee") && E.IsReady() && !Orbwalker.IsAutoAttacking)
            {
                CastEHero(target);
                return;
            }
            if (getCheckBoxItem(menuC, "useq") && !Orbwalker.IsAutoAttacking)
            {
                castQ(target);
            }
            if (!activatedW && W.IsReady() && data.AnyCC)
            {
                W.Cast(getCheckBoxItem(config, "packets"));
            }

            if (getCheckBoxItem(menuC, "userCC") && player.LSDistance(target) < Q.Range &&
                HeroManager.Enemies.FirstOrDefault(e => e.LSDistance(Game.CursorPos) < 300) != null && data.AnyCC)
            {
                R.Cast(Game.CursorPos, getCheckBoxItem(config, "packets"));
            }
        }

        private void checkCastedQ(Obj_AI_Base target)
        {
            if ((justQ && target.LSDistance(player) > Q.Range) || !target.IsValidTarget() || target == null)
            {
                return;
            }
            var poly = GetPoly(lastQPos);
            var heroes = HeroManager.Enemies.Where(e => poly.IsInside(e.Position) && e.IsValidTarget());
            if (heroes == null) return;
            var objAiHeroes = heroes as IList<AIHeroClient> ?? heroes.ToList();
            if (objAiHeroes.Any())
            {
                var escaping = objAiHeroes.Count(h => poly.IsOutside(Prediction.GetPrediction(h, 0.2f).UnitPosition.To2D()));
                var data = IncDamages.GetAllyData(player.NetworkId);
                if ((escaping > 0 && (objAiHeroes.Count() == 1 || (objAiHeroes.Count() >= 2 && System.Environment.TickCount - qStart > 1000))) || data.DamageTaken > player.Health || IncDamages.GetAllyData(player.NetworkId).AnyCC ||
                    IncDamages.GetEnemyData(target.NetworkId).DamageTaken > target.Health)
                {
                    Q.Cast(target, true);
                }
            }
        }

        private Geometry.Polygon GetPoly(Vector3 pos)
        {
            var POS = player.ServerPosition.LSExtend(pos, Q.ChargedMaxRange);
            var direction = (POS.To2D() - player.ServerPosition.To2D()).Normalized();

            var pos1 = (player.ServerPosition.To2D() - direction.Perpendicular()*qWidth/2f).To3D();

            var pos2 =
                (POS.To2D() + (POS.To2D() - player.ServerPosition.To2D()).Normalized() +
                 direction.Perpendicular()*qWidth/2f).To3D();

            var pos3 = (player.ServerPosition.To2D() + direction.Perpendicular()*qWidth/2f).To3D();

            var pos4 =
                (POS.To2D() + (POS.To2D() - player.ServerPosition.To2D()).Normalized() -
                 direction.Perpendicular()*qWidth/2f).To3D();
            var poly = new Geometry.Polygon();
            poly.Add(pos1);
            poly.Add(pos3);
            poly.Add(pos2);
            poly.Add(pos4);
            return poly;
        }

        private void CastEHero(AIHeroClient target)
        {
            if (E.CanCast(target))
            {
                E.CastIfHitchanceEquals(target, HitChance.High);
                return;
            }
            var pred = Prediction.GetPrediction(
                target, player.ServerPosition.LSDistance(target.ServerPosition)/E.Speed*1000);
            if (pred.UnitPosition.LSDistance(player.Position) > 1400 || pred.Hitchance < HitChance.High)
            {
                return;
            }
            var collision = E.GetCollision(player.Position.To2D(), new List<Vector2> {pred.CastPosition.To2D()});
            if (collision.Any(c => c.LSDistance(player) < E.Range) &&
                !CombatHelper.IsCollidingWith(
                    player, pred.CastPosition.LSExtend(player.Position, W.Width + 15), E.Width,
                    new[] {CollisionableObjects.Heroes, CollisionableObjects.Walls}))
            {
                E.Cast(pred.CastPosition);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawww"), W.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(180, 100, 146, 166));
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (activatedP)
            {
                return player.GetAutoAttackDamage(hero, true);
            }
            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q)*2f;
            }
            if (W.IsReady() || activatedW)
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.R)*hero.LSDistance(player) > 1000 ? 2f : 1.3f;
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private static double getWShield()
        {
            var shield = new double[] {30, 55, 80, 105, 130}[W.Level - 1] + 0.1f*player.MaxHealth +
                         0.4f*player.FlatMagicDamageMod;
            return shield;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "SionQ")
                {
                    if (!justQ)
                    {
                        QCastPos = args.End.To2D();
                        justQ = true;
                        qStart = System.Environment.TickCount;
                        lastQPos = player.Position.LSExtend(args.End, Q.Range);
                        Utility.DelayAction.Add(600, () => justQ = false);
                    }
                }
                if (args.SData.Name == "SionE")
                {
                    if (!justE)
                    {
                        justE = true;
                        Utility.DelayAction.Add(400, () => justE = false);
                    }
                }
            }
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
            config = MainMenu.AddMenu("Sion ", "Sion");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawww", new CheckBox("Draw W range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));

            // Combo Settings 
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usew", new CheckBox("Use W"));
            menuC.Add("usewir", new CheckBox("W : In range"));
            menuC.Add("shieldDmg", new Slider("W : Min dmg in shield %", 50, 1));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("user", new CheckBox("Use R"));
            menuC.Add("userCC", new CheckBox("Use R before CC"));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q"));
            menuH.Add("useeH", new CheckBox("Use E"));
            menuH.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));
            menuLC.Add("qMinHit", new Slider("   Min hit", 3, 1, 6));
            menuLC.Add("usewLC", new CheckBox("Use W"));
            menuLC.Add("wMinHit", new Slider("   Min hit", 3, 1, 6));
            menuLC.Add("useeLC", new CheckBox("Use E"));
            menuLC.Add("eMinHit", new Slider("   Min hit", 3, 1, 6));
            menuLC.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Settings
            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("usewgc", new CheckBox("Use W gapclosers", false));
            menuM.Add("AshieldB", new CheckBox("Auto W", false));
            menuM.Add("wMinAggro", new Slider("W: On aggro", 3, 1, 8));
            menuM.Add("AshieldDmg", new Slider("W: Min dmg in shield %", 100, 1));
            menuM.Add("minmanaAgg", new Slider("W: Min mana", 50, 1));
            menuM.Add("NoRlock", new CheckBox("Disable camera lock", false));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}