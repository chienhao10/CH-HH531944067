#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using Marksman.Common;

#endregion

namespace Marksman.Champions
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using Marksman.Utils;

    internal class Ezreal : Champion
    {
        public static LeagueSharp.Common.Spell Q;

        public static LeagueSharp.Common.Spell E;

        public static LeagueSharp.Common.Spell W;

        public static LeagueSharp.Common.Spell R;

        private static bool haveIceBorn = false;

        public Ezreal()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1190);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 950);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);

            E = new LeagueSharp.Common.Spell(SpellSlot.E);

            R = new LeagueSharp.Common.Spell(SpellSlot.R, 2500);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                //if (sender.IsMe)
                //Game.PrintChat(args.Buff.Name);
            };

            Utils.PrintMessage("Ezreal loaded");
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null && (ComboActive || HarassActive) && !t.HasKindredUltiBuff())
            {
                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;

                if (Q.IsReady() && useQ)
                {
                    Q.CastIfHitchanceGreaterOrEqual(t);
                }
                else if (W.IsReady() && useW)
                {
                    W.CastIfHitchanceGreaterOrEqual(t);
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            foreach (var enemy in HeroManager.Enemies.Where(enemy => R.IsReady() && enemy.LSIsValidTarget() && R.GetDamage(enemy) > enemy.Health))
            {
                Marksman.Common.CommonGeometry.DrawBox(new Vector2(Drawing.Width * 0.43f, Drawing.Height * 0.80f), 185, 18, Color.FromArgb(242, 255, 236, 6), 1, System.Drawing.Color.Black);
                Marksman.Common.CommonGeometry.DrawText(Marksman.Common.CommonGeometry.Text, "Killable enemy with ultimate: " + enemy.ChampionName, Drawing.Width * 0.435f, Drawing.Height * 0.803f, SharpDX.Color.Black);
            }

            LeagueSharp.Common.Spell[] spellList = { Q, W };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 0, 255));
            }

            var drawRMin = Program.combo["DrawRMin"].Cast<CheckBox>().CurrentValue;
            if (drawRMin)
            {
                var minRRange = Program.combo["UseRCMinRange"].Cast<Slider>().CurrentValue;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, minRRange, Color.DarkRed, 2);
            }

            var drawRMax = Program.combo["DrawRMax"].Cast<CheckBox>().CurrentValue;
            if (drawRMax)
            {
                var maxRRange = Program.combo["UseRCMaxRange"].Cast<Slider>().CurrentValue;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, maxRRange, Color.DarkMagenta, 2);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            Console.WriteLine(Q.GetHitchance().ToString());
            haveIceBorn = ObjectManager.Player.InventoryItems.Any(i => i.Id == ItemId.Iceborn_Gauntlet);

            if (Program.misc["ChargeR.Enable"].Cast<CheckBox>().CurrentValue && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var rCooldown = Program.misc["ChargeR.Cooldown"].Cast<Slider>().CurrentValue;
                var rMinMana = Program.misc["ChargeR.MinMana"].Cast<Slider>().CurrentValue;

                if (ObjectManager.Player.ManaPercent >= rMinMana && R.Cooldown >= rCooldown)
                {
                    var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                    foreach (var hit in from minions in vMinions
                                        select Q.GetPrediction(minions)
                        into qP
                                        let hit = qP.CastPosition.LSExtend(ObjectManager.Player.Position, -140)
                                        where qP.Hitchance >= HitChance.High
                                        select hit)
                    {
                        Q.Cast(hit);
                    }
                }
            }



            // 3070 tear of the goddess
            //  foreach (var i in ObjectManager.Player.InventoryItems)
            //  {
            //Game.PrintChat(i.IData.Id.ToString());
            //    }

            if (Program.misc["PingCH"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        enemy =>
                            R.IsReady() && enemy.LSIsValidTarget() && R.GetDamage(enemy) > enemy.Health
                            && enemy.LSDistance(ObjectManager.Player) > Q.Range))
                {
                    Utils.MPing.Ping(enemy.Position.LSTo2D());
                }
            }

            AIHeroClient t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var toggleQ = Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue;
            var toggleW = Program.harass["UseWTH"].Cast<KeyBind>().CurrentValue;
            if ((toggleQ || toggleW) && t.LSIsValidTarget(Q.Range) && ToggleActive)
            {
                if (Q.IsReady() && toggleQ)
                {
                    if (ObjectManager.Player.HasBuff("Recall")) return;

                    var useQt = (Program.harass["DontQToggleHarass" + t.ChampionName] != null
                                 && Program.harass["DontQToggleHarass" + t.ChampionName].Cast<CheckBox>().CurrentValue == false);
                    if (useQt)
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                    }
                }

                if (W.IsReady() && t.LSIsValidTarget(W.Range) && toggleW)
                {
                    if (ObjectManager.Player.HasBuff("Recall")) return;
                    var useWt = (Program.harass["DontWToggleHarass" + t.ChampionName] != null
                                 && Program.harass["DontWToggleHarass" + t.ChampionName].Cast<CheckBox>().CurrentValue == false);
                    if (useWt)
                    {
                        W.Cast(t);
                    }
                }
            }

            if (ComboActive || HarassActive)
            {
                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                var useQ = ComboActive ? Program.combo["UseQC"].Cast<CheckBox>().CurrentValue : Program.harass["UseQH"].Cast<CheckBox>().CurrentValue;
                var useW = ComboActive ? Program.combo["UseWC"].Cast<CheckBox>().CurrentValue : Program.harass["UseWH"].Cast<CheckBox>().CurrentValue;
                var useR = Program.combo["UseRC"].Cast<CheckBox>().CurrentValue;

                if (Orbwalker.CanMove && !t.HasKindredUltiBuff())
                {
                    if (useQ && Q.IsReady() && t.LSIsValidTarget(Q.Range))
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                    }

                    if (useW && W.IsReady() && t.LSIsValidTarget(W.Range))
                    {
                        W.Cast(t);
                    }

                    if (R.IsReady() && useR)
                    {
                        var maxRRange = Program.combo["UseRCMaxRange"].Cast<Slider>().CurrentValue;
                        var minRRange = Program.combo["UseRCMinRange"].Cast<Slider>().CurrentValue;

                        if (Q.IsReady() && t.LSIsValidTarget(Q.Range) && Q.GetPrediction(t).CollisionObjects.Count == 0
                            && t.Health < ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q)) return;

                        if (t.LSIsValidTarget() && ObjectManager.Player.LSDistance(t) >= minRRange
                            && ObjectManager.Player.LSDistance(t) <= maxRRange
                            && t.Health <= ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R))
                        {
                            R.Cast(t);
                        }
                    }
                }
            }

            if (R.IsReady() && Program.misc["CastR"].Cast<KeyBind>().CurrentValue)
            {
                t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget()) R.Cast(t);
            }
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.AddGroupLabel("Q Settings : ");
            config.Add("Lane.UseQ", new CheckBox("Q: Everytime"));
            config.Add("Lane.UseQ.AARange", new CheckBox("Q: Auto of AA Range"));
            config.Add("Lane.Q.HeatlhPrediction", new CheckBox("Q: Health Prediciton"));
            return true;
        }

        public override void ExecuteLaneClear()
        {
            if (!Q.IsReady())
            {
                return;
            }

            if (!Program.laneclear["Lane.UseQ"].Cast<CheckBox>().CurrentValue && !Program.laneclear["Lane.UseQ.AARange"].Cast<CheckBox>().CurrentValue && !Program.laneclear["Lane.Q.HeatlhPrediction"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);

            if (Program.laneclear["Lane.UseQ"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var minions in
                    vMinions.Where(
                        minions => minions.Health < ObjectManager.Player.LSGetSpellDamage(minions, SpellSlot.Q)))
                {
                    var qP = Q.GetPrediction(minions);
                    var hit = qP.CastPosition.LSExtend(ObjectManager.Player.Position, -140);
                    if (qP.Hitchance >= HitChance.High) Q.Cast(hit);
                }
            }

            if (Program.laneclear["Lane.UseQ.AARange"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var minions in
                    vMinions.Where(
                        minions =>
                            minions.Health < ObjectManager.Player.LSGetSpellDamage(minions, SpellSlot.Q) &&
                            !minions.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
                {
                    var qP = Q.GetPrediction(minions);
                    var hit = qP.CastPosition.LSExtend(ObjectManager.Player.Position, -140);
                    if (qP.Hitchance >= HitChance.High) Q.Cast(hit);
                }
            }

            if (Program.laneclear["Lane.Q.HeatlhPrediction"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var n in vMinions)
                {
                    var xH = HealthPrediction.GetHealthPrediction(n, (int)(ObjectManager.Player.AttackCastDelay * 1000), Game.Ping + (int)Q.Delay);
                    if (xH < 0)
                    {
                        if (n.Health < Q.GetDamage(n) && Q.CanCast(n))
                        {
                            Q.Cast(n);
                        }
                    }
                }
            }
        }

        public override void ExecuteJungleClear()
        {
            if (!Q.IsReady() || Program.jungleClear["Jungle.Q"].Cast<ComboBox>().CurrentValue == 0)
            {
                return;
            }
            var jungleMobs = Utils.GetMobs(Q.Range, Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                if (haveIceBorn)
                {
                    Q.Cast(jungleMobs);
                }
                else
                {
                    switch (Program.jungleClear["Jungle.Q"].Cast<ComboBox>().CurrentValue)
                    {
                        case 1:
                            {
                                Q.Cast(jungleMobs);
                                break;
                            }
                        case 2:
                            {
                                jungleMobs = Utils.GetMobs(Q.Range, Utils.MobTypes.BigBoys);
                                if (jungleMobs != null)
                                {
                                    Q.Cast(jungleMobs);
                                }
                                break;
                            }
                    }
                }
            }
        }

        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (Q.IsReady()) fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q);

            if (W.IsReady()) fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.W);

            if (E.IsReady()) fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.E);

            if (R.IsReady()) fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot"))
                == SpellState.Ready && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float)ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Botrk);

            return fComboDamage;
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Q"));
            config.Add("UseWC", new CheckBox("W"));

            config.AddGroupLabel("R");
            {
                config.Add("UseRC", new CheckBox("Use R"));
                config.Add("UseRCMinRange", new Slider("Min. Range", 200, 200, 1000));
                config.Add("UseRCMaxRange", new Slider("Max. Range", 1500, 500, 2000));
                config.Add("DrawRMin", new CheckBox("Draw Min. R Range"));//.SetValue(new Circle(true, Color.DarkRed)));
                config.Add("DrawRMax", new CheckBox("Draw Max. R Range"));//.SetValue(new Circle(true, Color.DarkMagenta)));
            }
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddGroupLabel("Q Settings : ");
            {
                config.Add("UseQH", new CheckBox("Use Q:"));
                config.Add("UseQTH", new KeyBind("Toggle:", false, KeyBind.BindTypes.PressToggle, 'T'));
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
                {
                    config.Add("DontQToggleHarass" + enemy.ChampionName, new CheckBox("Don't Q : " + enemy.ChampionName, false));
                }
            }
            config.AddGroupLabel("W Settings : ");
            {
                config.Add("UseWH", new CheckBox("Use W:"));
                config.Add("UseWTH", new KeyBind("Toggle:", false, KeyBind.BindTypes.PressToggle, 'H'));
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
                {
                    config.Add("DontWToggleHarass" + enemy.ChampionName, new CheckBox("Don't W : " + enemy.ChampionName, false));
                }
            }
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("ChargeR.Enable", new CheckBox("Charge R with Q", false));
            config.Add("ChargeR.Cooldown", new Slider("^ if R cooldown >", 20, 10, 120));
            config.Add("ChargeR.MinMana", new Slider("^^ And Min. Mana > %", 50, 0, 100));
            config.AddSeparator();
            config.Add("CastR", new KeyBind("Cast R (2000 Range)", false, KeyBind.BindTypes.HoldActive, 'T'));
            config.Add("PingCH", new CheckBox("Ping Killable Enemy with R"));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawW", new CheckBox("W range", false));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            return true;
        }


        public override bool JungleClearMenu(Menu config)
        {
            config.Add("Jungle.Q", new ComboBox("Use Q", 1, "Off", "On", "Just big Monsters"));
            return true;
        }

        public override void PermaActive()
        {

        }
    }
}
