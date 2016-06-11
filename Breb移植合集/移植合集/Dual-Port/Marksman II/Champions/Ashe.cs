#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

namespace Marksman.Champions
{
    internal class Ashe : Champion
    {
        public static LeagueSharp.Common.Spell Q;

        public static LeagueSharp.Common.Spell W;

        public static LeagueSharp.Common.Spell E;

        public static LeagueSharp.Common.Spell R;

        public Ashe()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1200);
            E = new LeagueSharp.Common.Spell(SpellSlot.E);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 4000);

            W.SetSkillshot(250f, (float)(45f * Math.PI / 180), 900f, true, SkillshotType.SkillshotCone);
            E.SetSkillshot(377f, 299f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(250f, 130f, 1600f, false, SkillshotType.SkillshotLine);

            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (!Program.misc["RInterruptable"].Cast<CheckBox>().CurrentValue)
                    return;

                BuffInstance aBuff =
                    (from fBuffs in
                        sender.Buffs.Where(
                            s =>
                                sender.Team != ObjectManager.Player.Team
                                && sender.LSDistance(ObjectManager.Player.Position) < 2500)
                     from b in new[] { "katarinar", "MissFortuneBulletTime", "crowstorm" }

                     where b.Contains(args.Buff.Name.ToLower())
                     select fBuffs).FirstOrDefault();

                if (aBuff != null && R.IsReady())
                {
                    R.Cast(sender.Position);
                }
            };

            Utils.Utils.PrintMessage("Ashe loaded.");
        }

        private static bool AsheQCastReady
        {
            get
            {
                return ObjectManager.Player.HasBuff("AsheQCastReady");
            }
        }

        public bool IsQActive
        {
            get
            {
                return ObjectManager.Player.HasBuff("FrostShot");
            }
        }

        private void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && Program.misc["RInterruptable"].Cast<CheckBox>().CurrentValue && unit.LSIsValidTarget(1500))
            {
                R.Cast(unit);
            }
        }

        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (W.IsReady()) fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.W);

            if (R.IsReady()) fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot"))
                == SpellState.Ready && ObjectManager.Player.LSDistance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.LSDistance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.LSDistance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, LeagueSharp.Common.Damage.DamageItems.Botrk);

            return fComboDamage;
        }

        public void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!Program.misc["EFlash"].Cast<CheckBox>().CurrentValue || unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            if (spell.SData.Name.ToLower() == "summonerflash" && unit.LSDistance(ObjectManager.Player.Position) < 2000)
            {
                E.Cast(spell.End);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (!ComboActive)
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (!t.LSIsValidTarget() || !W.IsReady()) return;

                if (Program.harass["UseWTH"].Cast<KeyBind>().CurrentValue)
                {
                    if (ObjectManager.Player.HasBuff("Recall")) return;
                    W.Cast(t);
                }

                if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Charm)
                    || t.HasBuffOfType(BuffType.Fear) || t.HasBuffOfType(BuffType.Taunt)
                    || t.HasBuff("zhonyasringshield") || t.HasBuff("Recall"))
                {
                    W.Cast(t.Position);
                }
            }

            /* [ Combo ] */
            if (ComboActive)
            {
                var useW = Program.combo["UseWC"].Cast<CheckBox>().CurrentValue;

                var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);

                if (Q.IsReady() && AsheQCastReady)
                {
                    if (t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 90))
                    {
                        Q.Cast();
                    }
                }

                if (useW && W.IsReady() && t.LSIsValidTarget())
                {
                    W.Cast(t);
                }

                var useR = Program.combo["UseRC"].Cast<CheckBox>().CurrentValue;
                if (useR && R.IsReady())
                {
                    var minRRange = Program.combo["UseRCMinRange"].Cast<Slider>().CurrentValue;
                    var maxRRange = Program.combo["UseRCMaxRange"].Cast<Slider>().CurrentValue;

                    t = TargetSelector.GetTarget(maxRRange, DamageType.Physical);
                    if (!t.LSIsValidTarget()) return;

                    var aaDamage = Orbwalking.InAutoAttackRange(t)
                                       ? ObjectManager.Player.LSGetAutoAttackDamage(t, true)
                                       : 0;

                    if (t.Health > aaDamage && t.Health <= ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R)
                        && ObjectManager.Player.LSDistance(t) >= minRRange)
                    {
                        R.Cast(t);
                    }
                }
            }

            //Harass
            if (HarassActive)
            {
                var target = TargetSelector.GetTarget(1200, DamageType.Physical);
                if (target == null) return;

                if (Program.harass["UseWH"].Cast<CheckBox>().CurrentValue && W.IsReady())
                    W.Cast(target);
            }

            //Manual cast R
            if (Program.misc["RManualCast"].Cast<KeyBind>().CurrentValue)
            {
                var rTarget = TargetSelector.GetTarget(2000, DamageType.Physical);
                R.Cast(rTarget);
            }
        }

        public override void ExecuteJungleClear()
        {
            if (Q.IsReady() && AsheQCastReady)
            {
                var jE = Program.jungleClear["UseQJ"].Cast<ComboBox>().CurrentValue;
                if (jE != 0)
                {
                    if (jE == 1)
                    {
                        var jungleMobs = Utils.Utils.GetMobs(
                            Orbwalking.GetRealAutoAttackRange(null) + 65,
                            Utils.Utils.MobTypes.BigBoys);
                        if (jungleMobs != null)
                        {
                            Q.Cast();
                        }
                    }
                    else
                    {
                        var totalAa =
                            MinionManager.GetMinions(
                                ObjectManager.Player.Position,
                                Orbwalking.GetRealAutoAttackRange(null) + 165,
                                MinionTypes.All,
                                MinionTeam.Neutral).Sum(mob => (int)mob.Health);
                        totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                        if (totalAa > jE)
                        {
                            Q.Cast();
                        }
                    }
                }
            }

            if (W.IsReady())
            {
                var jungleMobs = Marksman.Utils.Utils.GetMobs(W.Range, Marksman.Utils.Utils.MobTypes.All);
                if (jungleMobs != null)
                {
                    var jW = Program.jungleClear["UseWJ"].Cast<ComboBox>().CurrentValue;
                    switch (jW)
                    {
                        case 1:
                            {
                                jungleMobs = Marksman.Utils.Utils.GetMobs(W.Range, Marksman.Utils.Utils.MobTypes.All, jW);
                                W.CastOnUnit(jungleMobs);
                                break;
                            }
                        case 2:
                            {
                                jungleMobs = Utils.Utils.GetMobs(W.Range, Utils.Utils.MobTypes.BigBoys);
                                if (jungleMobs != null)
                                {
                                    W.CastOnUnit(jungleMobs);
                                }
                                break;
                            }
                    }
                }
            }
        }

        public override void ExecuteLaneClear()
        {

            if (Q.IsReady() && AsheQCastReady)
            {
                var jQ = Program.laneclear["UseQ.Lane"].Cast<ComboBox>().CurrentValue;
                /*
                if (jQ != 0)
                {
                    var totalAa = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && !m.IsDead && m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null))).Sum(mob => (int)mob.Health);
                    totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                    if (totalAa > jQ)
                    {
                        if (AsheQCastReady)
                            Q.Cast();
                    }
                }
                */
            }

            if (W.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, W.Range, MinionTypes.All, MinionTeam.Enemy);

                if (minions != null)
                {
                    var jE = Program.laneclear["UseW.Lane"].Cast<ComboBox>().CurrentValue;
                    if (jE != 0)
                    {
                        var mE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All);
                        if (mE.Count >= jE)
                        {
                            W.Cast(mE[0].Position);
                        }
                    }
                }
            }
        }



        public override bool ComboMenu(Menu config)
        {
            config.Add("UseWC", new CheckBox("W"));
            config.AddGroupLabel("R");
            {
                config.Add("UseRC", new CheckBox("Use R"));
                config.Add("UseRCMinRange", new Slider("Min. Range", 200, 200, 1000));
                config.Add("UseRCMaxRange", new Slider("Max. Range", 500, 500, 2000));
                config.Add("DrawRMin", new CheckBox("Draw Min. R Range"));//.SetValue(new Circle(true, System.Drawing.Color.DarkRed)));
                config.Add("DrawRMax", new CheckBox("Draw Max. R Range"));//.SetValue(new Circle(true, System.Drawing.Color.DarkMagenta)));
            }
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseWH", new CheckBox("W"));
            config.Add("UseWTH", new KeyBind("Use W (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            string[] strQ = new string[7];
            strQ[0] = "Off";

            for (var i = 1; i < 7; i++)
            {
                strQ[i] = "If need to AA more than >= " + i;
            }
            config.Add("UseQ.Lane", new ComboBox(Utils.Utils.Tab + "Use Q:", 0, strQ));

            string[] strW = new string[5];
            strW[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strW[i] = "If W it'll Hit >= " + i;
            }

            config.Add("UseW.Lane", new ComboBox(Utils.Utils.Tab + "Use W:", 0, strW));
            config.Add("UseQ.Lane.UnderTurret", new CheckBox(Utils.Utils.Tab + "Always Use Q Under Ally Turrent:"));
            config.Add("UseW.Lane.UnderTurret", new CheckBox(Utils.Utils.Tab + "Always Use W Under Ally Turrent:"));
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            string[] strQ = new string[8];
            {
                strQ[0] = "Off";
                strQ[1] = "Just for big Monsters";

                for (var i = 2; i < 8; i++)
                {
                    strQ[i] = "If need to AA more than >= " + i;
                }

                config.Add("UseQJ", new ComboBox("Use Q", 4, strQ));
            }

            string[] strW = new string[4];
            {
                strW[0] = "Off";
                strW[1] = "Just for big Monsters";

                for (var i = 2; i < 4; i++)
                {
                    strW[i] = "If Mobs Count >= " + i;
                }

                config.Add("UseWJ", new ComboBox("Use W", 1, strW));
            }
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawW", new CheckBox("W range"));//.SetValue(new Circle(true, System.Drawing.Color.CornflowerBlue)));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("RInterruptable", new CheckBox("Auto R Interruptable Spells"));
            config.Add("EFlash", new CheckBox("Use E against Flashes"));
            config.Add("RManualCast", new KeyBind("Cast R Manually(2000 range)", false, KeyBind.BindTypes.HoldActive, 'T'));
            return true;
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var drawW = Program.marksmanDrawings["DrawW"].Cast<CheckBox>().CurrentValue;
            if (drawW)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.CornflowerBlue);
            }

            var drawRMin = Program.combo["DrawRMin"].Cast<CheckBox>().CurrentValue;
            if (drawRMin)
            {
                var minRRange = Program.combo["UseRCMinRange"].Cast<Slider>().CurrentValue;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, minRRange, System.Drawing.Color.DarkRed, 2);
            }

            var drawRMax = Program.combo["DrawRMax"].Cast<CheckBox>().CurrentValue;
            if (drawRMax)
            {
                var maxRRange = Program.combo["UseRCMaxRange"].Cast<Slider>().CurrentValue;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, maxRRange, System.Drawing.Color.DarkMagenta, 2);
            }
        }
    }
}
