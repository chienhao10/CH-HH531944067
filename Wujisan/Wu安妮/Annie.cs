using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using WuAIO.Bases;
using WuAIO.Managers;
using WuAIO.Extensions;
using SharpDX;
using Circle = EloBuddy.SDK.Rendering.Circle;
using ScalingTypes = WuAIO.Managers.DamageManager.ScalingTypes;

namespace WuAIO
{
    class Annie : HeroBase
    {
        readonly AIHeroClient Player = EloBuddy.Player.Instance;
        Obj_AI_Minion Tibbers;
        const float ANGLE = 5 * (float)Math.PI / 18;
        Spell.Skillshot Flash;

        Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, 625);
        Spell.Skillshot W = new Spell.Skillshot(SpellSlot.W, 625, SkillShotType.Cone, 250, int.MaxValue, 210);
        Spell.Active E = new Spell.Active(SpellSlot.E);
        Spell.Skillshot R = new Spell.Skillshot(SpellSlot.R, 600, SkillShotType.Circular, 50, int.MaxValue, 250);

        public override void CreateMenu()
        {
            base.CreateMenu();

            var menu = MenuManager.AddSubMenu("线圈");
            {
                menu.NewCheckbox("disable", "关闭线圈", false);
                menu.NewCheckbox("damageindicator", "伤害显示");
                menu.NewCheckbox("q/w/r", "Q/W/R");
                menu.NewCheckbox("flash+r", "闪现 + R");
                menu.NewCheckbox("ultpos&hits", "大招施放位置&命中数量");
            }

            menu = MenuManager.AddSubMenu("连招");
            {
                menu.NewCheckbox("aa", "平A ?");
                menu.NewCheckbox("aa.maxrange", "最远距离平A?", false);
                menu.NewCheckbox("q", "Q", true, true);
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("r", "R", true, true);
                menu.NewCheckbox("r.jiws", "能击晕时直接R");
                menu.NewSlider("r.minenemies", "最低R敌人数量", 2, 1, 5, true);
            }

            menu = MenuManager.AddSubMenu("骚扰");
            {
                menu.NewCheckbox("aa.maxrange", "最远距离平A?", false);
                menu.NewCheckbox("q", "Q", true, true);
                menu.NewCheckbox("w", "W");
                menu.NewSlider("mana%", "最低蓝消耗%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("清线");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewSlider("q.mode", "Q 模式, 0 : 一直, 1 : 附近无敌人, 2 : 保留晕眩", 1, 0, 2);
                menu.NewCheckbox("w", "W", true, true);
                menu.NewSlider("w.minminions", "最低小兵数量W", 3, 1, 7);
                menu.NewSlider("mana%", "最低蓝消耗%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("尾兵");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewSlider("q.mode", "Q 模式, 0 : 一直, 1 : 附近无敌人, 2 : 保留晕眩", 0, 0, 2);
            }

            menu = MenuManager.AddSubMenu("清野");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewSlider("mana%", "最低蓝消耗%", 30, 1, 99, true);
            }

            menu = MenuManager.AddSubMenu("杂项");
            {
                menu.NewCheckbox("ks", "抢头");
                menu.NewCheckbox("stackstun", "叠加晕眩");
                menu.NewCheckbox("interrupter", "技能打断");
                menu.NewCheckbox("gapcloser", "击晕突击的敌人");
                menu.NewKeybind("autor", "对目标自动R (无视最低敌人数量设置):", false, KeyBind.BindTypes.HoldActive, 'J', true);
                menu.NewKeybind("autoflash+r", "闪现 + R (不无视最低敌人数量设置):", false, KeyBind.BindTypes.HoldActive, 'J', true);
            }
        }

        public override void CreateVariables()
        {
            new SkinManager(9);

            var slot = Player.GetSpellSlotFromName("summonerflash");

            if (slot != SpellSlot.Unknown)
                Flash = new Spell.Skillshot(slot, 425, SkillShotType.Linear);

            W.MinimumHitChance = HitChance.Medium;
            W.AllowedCollisionCount = int.MaxValue;
            W.ConeAngleDegrees = 50;

            R.AllowedCollisionCount = int.MaxValue;
            R.MinimumHitChance = HitChance.Medium;

            damageManager = new DamageManager
            (
                new List<Bases.Damage>()
                {
                    new Bases.Damage
                    (
                        Q, DamageType.Magical, new float[] { 0, 80, 115, 150, 185, 220 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f }, ScalingTypes.AP)
                        },
                        true, true
                    ),

                    new Bases.Damage
                    (
                        W, DamageType.Magical, new float[] { 0, 70, 115, 160, 205, 250 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.85f, 0.85f, 0.85f, 0.85f, 0.85f }, ScalingTypes.AP)
                        }
                    ),

                    new Bases.Damage
                    (
                        R, DamageType.Magical, new float[] { 0, 175, 300, 425 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.8f, 0.8f, 0.8f }, ScalingTypes.AP)
                        }
                    )
                }
            );
        }

        public override void PermaActive()
        {
            base.PermaActive();

            Tibbers = EntityManager.MinionsAndMonsters.Minions.FirstOrDefault(it => !it.IsDead && it.IsValidTarget(2000) && it.Name.ToLower().Contains("tibbers") || it.Name.ToLower().Contains("infernal") || it.Name.ToLower().Contains("guardian"));

            //----Stack stun
            
            if (misc.IsActive("stackstun") && !Player.HasBuff("recall") && !Player.HasBuff("pyromania_particle"))
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && E.IsReady()) E.Cast();
                if (Player.IsInShopRange() && W.IsReady()) W.Cast(Game.CursorPos);
            }

            //----Auto R / Flash+R

            if (R.IsReady() && (misc.IsActive("autor") || misc.IsActive("autoflash+r")))
            {
                var Target = TargetSelector.GetTarget(Flash.Range + R.Range - 150, DamageType.Magical);

                if (misc.IsActive("autor") && Target.IsValidTarget(R.Range - 50)) R.Cast(Target);

                else if (misc.IsActive("autoflash+r") && Target.IsValidTarget(R.Range + Flash.Range - 150) && Flash.IsReady())
                {
                    var RPos = GetBestRPos(Target.ServerPosition.To2D());

                    if (RPos.First().Value > 0)
                    {
                        var FlashPos = Player.Position.Extend(RPos.First().Key, Flash.Range).To3D();

                        Flash.Cast(FlashPos);

                        Core.DelayAction(() => R.Cast(RPos.First().Key.To3D()), 200);
                    }
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || draw.IsActive("disable")) return;

            if (R.IsReady() && draw.IsActive("ultpos&hits"))
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (Target != null && Target.IsValidTarget())
                {
                    var PosAndHits = GetBestRPos(Target.ServerPosition.To2D());
                    var minenemiesr = combo.Value("r.minenemies");

                    if (PosAndHits.First().Value >= minenemiesr)
                    {
                        Circle.Draw(Color.Yellow, 70, PosAndHits.First().Key.To3D());
                        Drawing.DrawText(Drawing.WorldToScreen(Player.Position).X, Drawing.WorldToScreen(Player.Position).Y - 200, System.Drawing.Color.Yellow, string.Format("R命中 {0}个敌人", PosAndHits.First().Value));
                    }
                }
            }

            if (draw.IsActive("q/w/r"))
                Circle.Draw((Q.IsReady() || W.IsReady() || R.IsReady()) ? Color.Blue : Color.Red, R.Range, Player.Position);

            if (draw.IsActive("flash+r"))
                Circle.Draw(Flash.IsReady() && R.IsReady() ? Color.Blue : Color.Red, Flash.Range + R.Range, Player.Position);

        }

        public override void KS()
        {
            if (!misc.IsActive("ks") || !EntityManager.Heroes.Enemies.Any(it => Q.IsInRange(it))) return;

            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && damageManager.SpellDamage(enemy, SpellSlot.Q) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }

            if (W.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(W.Range) && damageManager.SpellDamage(enemy, SpellSlot.W) >= enemy.Health);
                if (bye != null) { W.Cast(bye); return; }
            }

            if (Q.IsReady() && W.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && damageManager.SpellDamage(enemy, SpellSlot.Q) + damageManager.SpellDamage(enemy, SpellSlot.W) >= enemy.Health);
                if (bye != null) { W.Cast(bye); return; }
            }
        }

        public override void Combo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            if (R.IsReady() && combo.IsActive("r") && R.IsInRange(Target))
            {
                if (combo.IsActive("r.jiws"))
                {
                    if (Player.HasBuff("pyromania_particle"))
                    {
                        var rpos = GetBestRPos(Target.ServerPosition.To2D());

                        if (rpos.Values.First() >= combo.Value("r.minenemies"))
                        {
                            var pos = rpos.Keys.First().To3D();
                            R.Cast(pos);
                        }
                    }
                }
                else
                {
                    var rpos = GetBestRPos(Target.ServerPosition.To2D());

                    if (rpos.Values.First() >= combo.Value("r.minenemies"))
                    {
                        var pos = rpos.Keys.First().To3D();
                        R.Cast(pos);
                    }
                }
            }

            if (W.IsReady() && combo.IsActive("w") && W.IsInRange(Target))
                W.HitChanceCast(Target, 75);

            if (Q.IsReady() && combo.IsActive("q") && Q.IsInRange(Target)) Q.Cast(Target);

            if (Player.HasBuff("infernalguardiantime"))
            {
                if (Tibbers != null && Tibbers.IsValid && Tibbers.IsInAutoAttackRange(Target) && Tibbers.CanAttack)
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, Target);

                else EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, Target);

            }

            return;
        }

        public override void Harass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            if (W.IsReady() && harass.IsActive("w") && W.IsInRange(Target))
                W.HitChanceCast(Target, 75);

            if (Q.IsReady() && harass.IsActive("q") && Q.IsInRange(Target)) Q.Cast(Target);

            if (Player.HasBuff("infernalguardiantime"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, Target);

                if (Tibbers != null && Tibbers.IsValid && Tibbers.IsInAutoAttackRange(Target))
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, Target);
            }

            return;
        }

        public override void LaneClear()
        {
            if (Player.ManaPercent < laneclear.Value("mana%")) return;

            if (Q.IsReady() && laneclear.IsActive("q"))
            {
                var IEMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, Q.Range).Where(minion => minion.Health <= damageManager.SpellDamage(minion, SpellSlot.Q)).OrderBy(minion => minion.Distance(Player.Position.To2D()));

                if (IEMinions.Any())
                {
                    switch (laneclear.Value("q.mode"))
                    {
                        case 0:
                            Q.Cast(IEMinions.First());
                            break;
                        case 1:
                            if (Player.CountEnemiesInRange(700) == 0) Q.Cast(IEMinions.First());
                            break;
                        case 2:
                            if (!Player.HasBuff("pyromania_particle")) Q.Cast(IEMinions.First());
                            break;
                    }
                }
            }

            if (laneclear.IsActive("w") && W.IsReady())
            {
                var WPos = GetBestWPos(true);
                if (WPos != default(Vector3)) W.Cast(WPos);
            }

            return;
        }

        public override void LastHit()
        {
            if (Q.IsReady() && lasthit.IsActive("q"))
            {
                var IEMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, Q.Range).Where(minion => minion.Health <= damageManager.SpellDamage(minion, SpellSlot.Q)).OrderBy(minion => minion.Distance(Player.Position.To2D()));

                if (IEMinions.Any())
                {
                    switch (lasthit.Value("q.mode"))
                    {
                        case 0:
                            Q.Cast(IEMinions.First());
                            break;
                        case 1:
                            if (Player.CountEnemiesInRange(700) == 0) Q.Cast(IEMinions.First());
                            break;
                        case 2:
                            if (!Player.HasBuff("pyromania_particle")) Q.Cast(IEMinions.First());
                            break;
                    }
                }
            }

            return;
        }

        public override void JungleClear()
        {
            if (Player.ManaPercent < jungleclear.Value("mana%")) return;

            var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Q.Range).FirstOrDefault(it => it.Health > Player.GetAutoAttackDamage(it));

            if (minion != null)
            {
                if (Q.IsReady() && jungleclear.IsActive("q")) Q.Cast(minion);

                if (W.IsReady() && jungleclear.IsActive("w")) W.Cast(minion);
            }

            return;
        }

        public override void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Player.IsDead || !sender.IsEnemy || !sender.IsValidTarget(W.Range) || !misc.IsActive("gapcloser")) return;

            if ((Player.HasBuff("pyromania_particle") || ((Player.GetBuffCount("pyromania") >= 3 && Q.IsReady() && W.IsReady()))))
            {
                if (W.IsReady()) W.Cast(sender);
                if (Q.IsReady()) Q.Cast(sender);
            }

            return;
        }

        public override void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!sender.IsValidTarget(R.Range) || args.DangerLevel < DangerLevel.High || !misc.IsActive("interrupter")) return;

            if (Player.HasBuff("pyromania_particle"))
            {
                if (W.IsReady()) { W.Cast(sender); return; }

                if (Q.IsReady()) { Q.Cast(sender); return; }

                if (R.IsReady()) { R.Cast(sender); return; }

                return;
            }
            else
            {
                var qisready = Q.IsReady();
                var wisready = W.IsReady();
                var eisready = E.IsReady();
                var risready = R.IsReady();

                if (Player.GetBuffCount("pyromania") >= 3)
                {
                    if (qisready)
                    {
                        if (wisready)
                        {
                            if (W.Cast(sender))
                                Core.DelayAction(() => Q.Cast(sender), 100);
                            return;
                        }

                        if (eisready)
                        {
                            if (E.Cast(sender))
                                Core.DelayAction(() => Q.Cast(sender), 100);
                            return;
                        }

                        if (risready)
                        {
                            if (R.Cast(sender))
                                Core.DelayAction(() => Q.Cast(sender), 100);
                            return;
                        }

                        return;
                    }

                    if (wisready)
                    {
                        if (eisready)
                        {
                            if (E.Cast(sender))
                                Core.DelayAction(() => W.Cast(sender), 100);
                            return;
                        }

                        if (risready)
                        {
                            if (R.Cast(sender))
                                Core.DelayAction(() => W.Cast(sender), 100);
                            return;
                        }

                        return;
                    }

                    if (eisready && risready)
                    {
                        if (E.Cast())
                            Core.DelayAction(() => R.Cast(sender), 100);
                    }
                }

                return;
            }
        }

        public override void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (!combo.IsActive("aa"))
                { args.Process = false; return; }

                if (!combo.IsActive("aa.maxrange") && Player.Distance(target) >= 530)
                { args.Process = false; return; }
            }
            
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (!harass.IsActive("aa.maxrange") && Player.Distance(target) >= 530)
                { args.Process = false; return; }
            }
        }

        //------------------------------------|| Extension ||--------------------------------------

        //-----------------------------CountRHits(Vector2 CastPosition)-----------------------------

        int CountRHits(Vector2 CastPosition)
        {
            int Hits = new int();

            foreach (Vector2 EnemyPos in GetEnemiesPosition())
            {
                if (CastPosition.Distance(EnemyPos) <= 250) Hits += 1;
            }

            return Hits;
        }

        //----------------------------GetBestRPos(Vector2 TargetPosition)---------------------------

        Dictionary<Vector2, int> GetBestRPos(Vector2 TargetPosition)
        {
            Dictionary<Vector2, int> PosAndHits = new Dictionary<Vector2, int>();

            List<Vector2> RPos = new List<Vector2>
            {
                new Vector2(TargetPosition.X - 250, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X - 250, TargetPosition.Y),

                new Vector2(TargetPosition.X - 200, TargetPosition.Y + 300),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y),

                new Vector2(TargetPosition.X - 160, TargetPosition.Y - 160),

                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 300),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 250),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y - 200),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y),

                new Vector2(TargetPosition.X, TargetPosition.Y + 300),
                new Vector2(TargetPosition.X, TargetPosition.Y + 270),
                new Vector2(TargetPosition.X, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X, TargetPosition.Y + 100),

                new Vector2(TargetPosition.X, TargetPosition.Y),

                new Vector2(TargetPosition.X, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X, TargetPosition.Y - 200),

                new Vector2(TargetPosition.X + 100, TargetPosition.Y),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y - 200),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 250),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 300),

                new Vector2(TargetPosition.X + 160, TargetPosition.Y - 160),

                new Vector2(TargetPosition.X + 200, TargetPosition.Y),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y + 300),

                new Vector2(TargetPosition.X + 250, TargetPosition.Y),
                new Vector2(TargetPosition.X + 250, TargetPosition.Y + 100),
            };

            foreach (Vector2 pos in RPos)
            {
                PosAndHits.Add(pos, CountRHits(pos));
            }

            Vector2 PosToGG = PosAndHits.First(pos => pos.Value == PosAndHits.Values.Max()).Key;
            int Hits = PosAndHits.First(pos => pos.Key == PosToGG).Value;

            return new Dictionary<Vector2, int>() { { PosToGG, Hits } };
        }

        //----------------------------------GetEnemiesPosition()------------------------------------

        List<Vector3> GetEnemiesPosition()
        {
            List<Vector3> Positions = new List<Vector3>();

            foreach (AIHeroClient Hero in EntityManager.Heroes.Enemies.Where(hero => !hero.IsDead && Player.Distance(hero) <= 1200))
            {
                Positions.Add(Prediction.Position.PredictUnitPosition(Hero, 500).To3D());
            }

            return Positions;
        }

        //--------------------------------------GetBestWPos----------------------------------------

        Vector3 GetBestWPos(bool minions = false, AIHeroClient Target = null)
        {
            if (minions)
            {
                var CS = new List<Geometry.Polygon.Sector>();

                var Minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(it => it.IsValidTarget(W.Range)).OrderByDescending(it => it.Distance(Player)).FirstOrDefault();

                if (Minion == null) return default(Vector3);

                var Vectors = new List<Vector3>() { new Vector3(Minion.ServerPosition.X + 550, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X - 550, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y + 550, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y - 550, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X + 230, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X - 230, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y + 230, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y - 230, Minion.ServerPosition.Z), Minion.ServerPosition };

                var CS1 = new Geometry.Polygon.Sector(Player.Position, Vectors[0], ANGLE, 585);
                var CS2 = new Geometry.Polygon.Sector(Player.Position, Vectors[1], ANGLE, 585);
                var CS3 = new Geometry.Polygon.Sector(Player.Position, Vectors[2], ANGLE, 585);
                var CS4 = new Geometry.Polygon.Sector(Player.Position, Vectors[3], ANGLE, 585);
                var CS5 = new Geometry.Polygon.Sector(Player.Position, Vectors[4], ANGLE, 585);
                var CS6 = new Geometry.Polygon.Sector(Player.Position, Vectors[5], ANGLE, 585);
                var CS7 = new Geometry.Polygon.Sector(Player.Position, Vectors[6], ANGLE, 585);
                var CS8 = new Geometry.Polygon.Sector(Player.Position, Vectors[7], ANGLE, 585);
                var CS9 = new Geometry.Polygon.Sector(Player.Position, Vectors[8], ANGLE, 585);

                CS.Add(CS1);
                CS.Add(CS2);
                CS.Add(CS3);
                CS.Add(CS4);
                CS.Add(CS5);
                CS.Add(CS6);
                CS.Add(CS7);
                CS.Add(CS8);
                CS.Add(CS9);

                var CSHits = new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                for (byte j = 0; j < 9; j++)
                {
                    foreach (Obj_AI_Base minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(it => it.IsValidTarget(W.Range)))
                    {
                        if (CS.ElementAt(j).IsInside(minion)) CSHits[j]++;
                    }
                }

                int i = CSHits.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;

                if (CSHits[i] < laneclear.Value("w.minminions")) return default(Vector3);

                return Vectors[i];
            }
            else if (Target != null && Target.IsValidTarget())
            {
                var CS = new List<Geometry.Polygon.Sector>();
                var Vectors = new List<Vector3>() { new Vector3(Target.ServerPosition.X + 550, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X - 550, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y + 550, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y - 550, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X + 230, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X - 230, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y + 230, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y - 230, Target.ServerPosition.Z), Target.ServerPosition };

                var CS1 = new Geometry.Polygon.Sector(Player.Position, Vectors[0], ANGLE, 585);
                var CS2 = new Geometry.Polygon.Sector(Player.Position, Vectors[1], ANGLE, 585);
                var CS3 = new Geometry.Polygon.Sector(Player.Position, Vectors[2], ANGLE, 585);
                var CS4 = new Geometry.Polygon.Sector(Player.Position, Vectors[3], ANGLE, 585);
                var CS5 = new Geometry.Polygon.Sector(Player.Position, Vectors[4], ANGLE, 585);
                var CS6 = new Geometry.Polygon.Sector(Player.Position, Vectors[5], ANGLE, 585);
                var CS7 = new Geometry.Polygon.Sector(Player.Position, Vectors[6], ANGLE, 585);
                var CS8 = new Geometry.Polygon.Sector(Player.Position, Vectors[7], ANGLE, 585);
                var CS9 = new Geometry.Polygon.Sector(Player.Position, Vectors[8], ANGLE, 585);

                CS.Add(CS1);
                CS.Add(CS2);
                CS.Add(CS3);
                CS.Add(CS4);
                CS.Add(CS5);
                CS.Add(CS6);
                CS.Add(CS7);
                CS.Add(CS8);
                CS.Add(CS9);

                var CSHits = new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                for (byte j = 0; j < 9; j++)
                {
                    foreach (AIHeroClient hero in EntityManager.Heroes.Enemies.Where(enemy => !enemy.IsDead && enemy.IsValidTarget(W.Range)))
                    {
                        if (CS.ElementAt(j).IsInside(hero)) CSHits[j]++;
                        if (hero == Target) CSHits[j] += 10;
                    }
                }

                byte i = (byte)CSHits.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;

                if (CSHits[i] <= 0) return default(Vector3);

                return Vectors[i];
            }

            return default(Vector3);
        }
    }
}
