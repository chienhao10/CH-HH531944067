﻿namespace ElEkko
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SharpDX;

    using Color = System.Drawing.Color;
    using SebbyLib;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
    using EloBuddy.SDK;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class ElEkko
    {
        #region Static Fields


        public static Dictionary<Spells, LeagueSharp.Common.Spell> spells = new Dictionary<Spells, LeagueSharp.Common.Spell>
                                                             {
                                                                 { Spells.Q, new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.Q, 950) },
                                                                 { Spells.W, new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.W, 1600) },
                                                                 { Spells.E, new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.E, 425) },
                                                                 { Spells.R, new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.R, 400) }
                                                             };

        private static readonly Dictionary<float, float> incomingDamage = new Dictionary<float, float>();

        private static readonly Dictionary<float, float> instantDamage = new Dictionary<float, float>();

        private static EloBuddy.SpellSlot ignite;

        #endregion

        #region Public Properties

        public static Vector3 FleePosition { get; set; }

        public static float IncomingDamage
        {
            get
            {
                return incomingDamage.Sum(e => e.Value) + instantDamage.Sum(e => e.Value);
            }
        }

        public static bool IsJumpPossible { get; set; }

        public static string ScriptVersion
        {
            get
            {
                return typeof(ElEkko).Assembly.GetName().Version.ToString();
            }
        }

        public static EloBuddy.GameObject Troy { get; set; }

        #endregion

        #region Properties

        private static EloBuddy.AIHeroClient Player
        {
            get
            {
                return EloBuddy.ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static float GetComboDamage(EloBuddy.Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (ignite == EloBuddy.SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != EloBuddy.SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            }

            return damage + 15 + (12 * Player.Level) + Player.FlatMagicDamageMod;
        }


        public static void OnLoad()
        {
            if (EloBuddy.ObjectManager.Player.CharData.BaseSkinName != "Ekko")
            {
                return;
            }

            ignite = Player.GetSpellSlot("summonerdot");

            spells[Spells.Q].SetSkillshot(0.25f, 60, 1650f, false, SkillshotType.SkillshotLine);
            spells[Spells.W].SetSkillshot(2.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            ElEkkoMenu.Initialize();
            EloBuddy.Game.OnUpdate += OnUpdate;
            EloBuddy.Drawing.OnDraw += Drawings.OnDraw;
            EloBuddy.Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            EloBuddy.GameObject.OnCreate += Obj_AI_Base_OnCreate;
            EloBuddy.GameObject.OnDelete += Obj_AI_Base_OnDelete;
        }

        #endregion

        #region Methods

        private static void AutoHarass()
        {
            var target = EloBuddy.SDK.TargetSelector.GetTarget(spells[Spells.Q].Range, EloBuddy.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Player.LSIsRecalling())
            {
                return;
            }

            if (ElEkkoMenu.Harass["ElEkko.AutoHarass.Q"].Cast<CheckBox>().CurrentValue)
            {
                var mana = ElEkkoMenu.Harass["ElEkko.Harass.Q.Mana"].Cast<Slider>().CurrentValue;

                if (Player.ManaPercent < mana)
                {
                    return;
                }

                if (spells[Spells.Q].IsReady() && target.LSDistance(Player.Position) <= spells[Spells.Q].Range - 50
                    && !Player.IsDashing())
                {
                    spells[Spells.Q].Cast(target);
                }
            }
        }

        private static int CountPassive(EloBuddy.Obj_AI_Base target)
        {
            var ekkoPassive = target.Buffs.FirstOrDefault(x => x.Name.Equals("EkkoStacks", StringComparison.InvariantCultureIgnoreCase));
            if (ekkoPassive != null)
            {
                return ekkoPassive.Count;
            }

            return 0;
        }


        private static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            return GetFirstWallPoint(from.LSTo2D(), to.LSTo2D(), step);
        }

        private static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).LSNormalized();

            for (float d = 0; d < from.LSDistance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = EloBuddy.NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(EloBuddy.CollisionFlags.Wall) || flags.HasFlag(EloBuddy.CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }

        private static float IgniteDamage(EloBuddy.AIHeroClient target)
        {
            if (ignite == EloBuddy.SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != EloBuddy.SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }


        private static void KillSteal()
        {
            var isActive = ElEkkoMenu.KillSteal["ElEkko.Killsteal.Active"].Cast<CheckBox>().CurrentValue;
            if (isActive)
            {
                foreach (var hero in
                    EloBuddy.ObjectManager.Get<EloBuddy.AIHeroClient>()
                        .Where(
                            hero =>
                            EloBuddy.ObjectManager.Player.LSDistance(hero.ServerPosition) <= spells[Spells.Q].Range && !hero.IsMe
                            && hero.LSIsValidTarget() && hero.IsEnemy && !hero.IsInvulnerable))
                {
                    var qDamage = spells[Spells.Q].GetDamage(hero);
                    var useQ = ElEkkoMenu.KillSteal["ElEkko.Killsteal.Q"].Cast<CheckBox>().CurrentValue;
                    var useR = ElEkkoMenu.KillSteal["ElEkko.Killsteal.R"].Cast<CheckBox>().CurrentValue;
                    var useIgnite = ElEkkoMenu.KillSteal["ElEkko.Killsteal.Ignite"].Cast<CheckBox>().CurrentValue;

                    if (useQ && hero.Health - qDamage < 0 && spells[Spells.Q].IsReady()
                        && spells[Spells.Q].IsInRange(hero))
                    {
                        spells[Spells.Q].Cast(hero);
                    }

                    if (useR && spells[Spells.R].IsReady())
                    {
                        if (spells[Spells.R].GetDamage(hero) > hero.Health)
                        {
                            if (Troy != null)
                            {
                                if (hero.LSDistance(Troy.Position) <= spells[Spells.R].Range)
                                {
                                    spells[Spells.R].Cast();
                                }
                            }
                        }
                    }

                    if (useIgnite && Player.LSDistance(hero) <= 600 && IgniteDamage(hero) >= hero.Health)
                    {
                        Player.Spellbook.CastSpell(ignite, hero);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(EloBuddy.GameObject obj, EventArgs args)
        {
            var particle = obj as EloBuddy.Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    Troy = particle;
                }
            }
        }

        private static void Obj_AI_Base_OnDelete(EloBuddy.GameObject obj, EventArgs args)
        {
            var particle = obj as EloBuddy.Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    Troy = null;
                }
            }
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(EloBuddy.Obj_AI_Base sender, EloBuddy.GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy)
            {
                if (Player != null && spells[Spells.R].IsReady())
                {
                    if ((!(sender is EloBuddy.AIHeroClient) || args.SData.IsAutoAttack()) && args.Target != null
                        && args.Target.NetworkId == Player.NetworkId)
                    {
                        incomingDamage.Add(
                            Player.ServerPosition.LSDistance(sender.ServerPosition) / args.SData.MissileSpeed + EloBuddy.Game.Time,
                            (float)sender.LSGetAutoAttackDamage(Player));
                    }
                    else if (sender is EloBuddy.AIHeroClient)
                    {
                        var attacker = (EloBuddy.AIHeroClient)sender;
                        var slot = attacker.GetSpellSlot(args.SData.Name);

                        if (slot != EloBuddy.SpellSlot.Unknown)
                        {
                            if (slot == attacker.GetSpellSlot("SummonerDot") && args.Target != null
                                && args.Target.NetworkId == Player.NetworkId)
                            {
                                instantDamage.Add(
                                    EloBuddy.Game.Time + 2,
                                    (float)attacker.GetSummonerSpellDamage(Player, LeagueSharp.Common.Damage.SummonerSpell.Ignite));
                            }
                            else if (slot.HasFlag(EloBuddy.SpellSlot.Q | EloBuddy.SpellSlot.W | EloBuddy.SpellSlot.E | EloBuddy.SpellSlot.R)
                                     && ((args.Target != null && args.Target.NetworkId == Player.NetworkId)
                                         || args.End.LSDistance(Player.ServerPosition) < Math.Pow(args.SData.LineWidth, 2)))
                            {
                                instantDamage.Add(EloBuddy.Game.Time + 2, (float)attacker.LSGetSpellDamage(Player, slot));
                            }
                        }
                    }
                }
            }

            if (sender.IsMe)
            {
                if (args.SData.Name.Equals("EkkoE", StringComparison.InvariantCultureIgnoreCase))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, EloBuddy.SDK.Orbwalker.ResetAutoAttack);
                }
            }
        }

        private static void OnCombo()
        {
            var target = EloBuddy.SDK.TargetSelector.GetTarget(spells[Spells.Q].Range, EloBuddy.DamageType.Magical);
            if (target == null || !target.LSIsValidTarget())
            {
                return;
            }

            var useQ = ElEkkoMenu.Combo["ElEkko.Combo.Q"].Cast<CheckBox>().CurrentValue;
            var useW = ElEkkoMenu.Combo["ElEkko.Combo.W"].Cast<CheckBox>().CurrentValue;
            var useE = ElEkkoMenu.Combo["ElEkko.Combo.E"].Cast<CheckBox>().CurrentValue;
            var useR = ElEkkoMenu.Combo["ElEkko.Combo.R"].Cast<CheckBox>().CurrentValue;
            var useRkill = ElEkkoMenu.Combo["ElEkko.Combo.R.Kill"].Cast<CheckBox>().CurrentValue;
            var useIgnite = ElEkkoMenu.Combo["ElEkko.Combo.Ignite"].Cast<CheckBox>().CurrentValue;

            var enemies = ElEkkoMenu.Combo["ElEkko.Combo.W.Count"].Cast<Slider>().CurrentValue;
            var enemiesRrange = ElEkkoMenu.Combo["ElEkko.Combo.R.Enemies"].Cast<Slider>().CurrentValue;

            if (useQ && spells[Spells.Q].IsReady() && target.LSDistance(Player.Position) <= spells[Spells.Q].Range - 50
                && !Player.IsDashing())
            {
                spells[Spells.Q].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady())
            {
                if (target.LSDistance(Player.Position) >= spells[Spells.E].Range)
                {
                    return;
                }

                if (Player.LSCountEnemiesInRange(spells[Spells.W].Range) >= enemies)
                {
                    var pred = spells[Spells.W].GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spells[Spells.W].Cast(pred.CastPosition);
                    }
                }
                else if (target.HasBuffOfType(EloBuddy.BuffType.Slow) || target.HasBuffOfType(EloBuddy.BuffType.Taunt)
                         || target.HasBuffOfType(EloBuddy.BuffType.Stun)
                         || target.HasBuffOfType(EloBuddy.BuffType.Snare)
                         && target.LSDistance(Player.Position) <= spells[Spells.E].Range)
                {
                    var pred = spells[Spells.W].GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spells[Spells.W].Cast(pred.CastPosition);
                    }
                }
                else
                {
                    if (target.ServerPosition.LSDistance(Player.Position)
                        > spells[Spells.E].Range * spells[Spells.E].Range)
                    {
                        if (spells[Spells.W].GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                        {
                            var pred = spells[Spells.W].GetPrediction(target);
                            if (pred.Hitchance >= HitChance.High)
                            {
                                spells[Spells.W].Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }

            if (useE && spells[Spells.E].IsReady() && !spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target)
                && !EloBuddy.ObjectManager.Player.UnderTurret(true) && target.HasBuff("EkkoStacks"))
            {
                        spells[Spells.E].Cast(target.Position);
            }

            if (useR && spells[Spells.R].IsReady())
            {
                if (target.Health < spells[Spells.R].GetDamage(target))
                {
                    if (Troy != null)
                    {
                        if (target.LSDistance(Troy.Position) <= spells[Spells.R].Range)
                        {
                            spells[Spells.R].Cast();
                        }
                    }
                }

                var enemyCount =
                    HeroManager.Enemies.Count(
                        h => h.LSIsValidTarget() && h.LSDistance(Troy.Position) < spells[Spells.R].Range);
                if (enemyCount >= enemiesRrange)
                {
                    if (Troy != null)
                    {
                        if (target.LSDistance(Troy.Position) <= spells[Spells.R].Range)
                        {
                            spells[Spells.R].Cast();
                        }
                    }
                }
            }

            if (useIgnite && Player.LSDistance(target) <= 600 && IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = EloBuddy.SDK.TargetSelector.GetTarget(spells[Spells.Q].Range, EloBuddy.DamageType.Magical);
            if (target == null || !target.LSIsValidTarget())
            {
                return;
            }

            var useQ = ElEkkoMenu.Harass["ElEkko.Harass.Q"].Cast<CheckBox>().CurrentValue;
            var useE = ElEkkoMenu.Harass["ElEkko.Harass.E"].Cast<CheckBox>().CurrentValue;
            var mana = ElEkkoMenu.Harass["ElEkko.Harass.Q.Mana"].Cast<Slider>().CurrentValue;

            if (Player.ManaPercent < mana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && target.LSDistance(Player.Position) <= spells[Spells.Q].Range
                && !Player.IsDashing())
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && !spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target)
                && !EloBuddy.ObjectManager.Player.UnderTurret(true) && target.HasBuff("EkkoStacks"))
            {
                        spells[Spells.E].Cast(target.Position);
            }
        }

        private static void OnJungleClear()
        {
            var useQ = ElEkkoMenu.JungleClear["ElEkko.JungleClear.Q"].Cast<CheckBox>().CurrentValue;
            var useW = ElEkkoMenu.JungleClear["ElEkko.JungleClear.W"].Cast<CheckBox>().CurrentValue;
            var mana = ElEkkoMenu.JungleClear["ElEkko.JungleClear.mana"].Cast<Slider>().CurrentValue;
            var qMinions = ElEkkoMenu.JungleClear["ElEkko.JungleClear.Minions"].Cast<Slider>().CurrentValue;

            if (Player.ManaPercent < mana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (minions.Count <= 0)
            {
                return;
            }

            var minionsInRange = minions.Where(x => spells[Spells.Q].IsInRange(x));
            var objAiBases = minionsInRange as IList<EloBuddy.Obj_AI_Base> ?? minionsInRange.ToList();
            if (objAiBases.Count() >= qMinions && useQ)
            {
                var qKills = 0;
                foreach (var minion in objAiBases)
                {
                    if (spells[Spells.Q].GetDamage(minion) < minion.Health)
                    {
                        qKills++;

                        if (qKills >= qMinions)
                        {
                            var bestFarmPos = spells[Spells.Q].GetLineFarmLocation(minions);
                            spells[Spells.Q].Cast(bestFarmPos.Position);
                        }
                    }
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                var mobs = MinionManager.GetMinions(
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Count <= 0)
                {
                    return;
                }

                var bestFarmPos = spells[Spells.W].GetCircularFarmLocation(minions);
                spells[Spells.W].Cast(bestFarmPos.Position);
            }
        }

        private static void OnLaneClear()
        {
            var useQ = ElEkkoMenu.Clear["ElEkko.LaneClear.Q"].Cast<CheckBox>().CurrentValue;
            var mana = ElEkkoMenu.Clear["ElEkko.LaneClear.mana"].Cast<Slider>().CurrentValue;
            var qMinions = ElEkkoMenu.Clear["ElEkko.LaneClear.Minions"].Cast<Slider>().CurrentValue;

            if (Player.ManaPercent < mana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            if (minions.Count <= 0)
            {
                return;
            }

            var minionsInRange = minions.Where(x => spells[Spells.Q].IsInRange(x));
            var objAiBases = minionsInRange as IList<EloBuddy.Obj_AI_Base> ?? minionsInRange.ToList();
            if (objAiBases.Count() >= qMinions && useQ)
            {
                var qKills = 0;
                foreach (var minion in objAiBases)
                {
                    if (spells[Spells.Q].GetDamage(minion) < minion.Health)
                    {
                        qKills++;

                        if (qKills >= qMinions)
                        {
                            var bestFarmPos = spells[Spells.Q].GetLineFarmLocation(minions);
                            spells[Spells.Q].Cast(bestFarmPos.Position);
                            return;
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnLaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnJungleClear();
            }

           
            var twoStacksQ = ElEkkoMenu.Combo["ElEkko.Combo.Auto.Q"].Cast<CheckBox>().CurrentValue;
            if (twoStacksQ)
            {
                var qtarget = EloBuddy.SDK.TargetSelector.GetTarget(spells[Spells.Q].Range, EloBuddy.DamageType.Magical);
                if (qtarget == null || !qtarget.IsValid || SebbyLib.OktwCommon.CanMove(qtarget))
                {
                    return;
                }

                if (CountPassive(qtarget) == 2 && qtarget.LSDistance(Player.Position) <= spells[Spells.Q].Range)
                {
                    var pred = spells[Spells.Q].GetPrediction(qtarget);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spells[Spells.Q].Cast(pred.CastPosition);
                    }
                }
            }

            SaveMode();
            KillSteal();
            AutoHarass();

            var rtext = ElEkkoMenu.Misc["ElEkko.R.text"].Cast<CheckBox>().CurrentValue;
            if (Troy != null && rtext)
            {
                var enemyCount =
                    HeroManager.Enemies.Count(
                        h => h.LSIsValidTarget() && h.LSDistance(Troy.Position) < spells[Spells.R].Range);
                EloBuddy.Drawing.DrawText(
                    EloBuddy.Drawing.Width * 0.44f,
                    EloBuddy.Drawing.Height * 0.80f,
                    Color.White,
                    "There are {0} in R range",
                    enemyCount);
            }
        }

        private static void SaveMode()
        {
            if (Player.LSIsRecalling() || Player.InFountain())
            {
                return;
            }

            var useR = ElEkkoMenu.Combo["ElEkko.Combo.R"].Cast<CheckBox>().CurrentValue;
            var playerHp = ElEkkoMenu.Combo["ElEkko.Combo.R.HP"].Cast<Slider>().CurrentValue;

            if (useR && spells[Spells.R].IsReady())
            {
                if (Player.HealthPercent < playerHp && Player.LSCountEnemiesInRange(600) > 0)
                {
                    spells[Spells.R].Cast();
                }
            }
        }

        #endregion
    }
}