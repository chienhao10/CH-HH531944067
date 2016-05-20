using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SAutoCarry.Champions.Helpers;
using SCommon.Damage;
using SharpDX;
using Champion = SCommon.PluginBase.Champion;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using TargetSelector = SCommon.TS.TargetSelector;
using Utility = LeagueSharp.Common.Utility;

namespace SAutoCarry.Champions
{
    /*
    public class Azir : Champion
    {
        public static Menu comboMenu, harassMenu, laneClearMenu, miscMenu, antiGapMenu;
        private int CastET, CastQT;
        private Vector2 CastQLocation, CastELocation, InsecLocation, InsecTo, JumpTo;
        private int lastLaneClearTick;

        public Azir() : base("Azir", "SAutoCarry - Azir")
        {
            SoldierMgr.Initialize(this);
            Game.OnUpdate += BeforeOrbwalk;
            Drawing.OnDraw += BeforeDraw;
            CreateConfigMenu();
            Game.OnWndProc += Game_OnWndProc;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public bool ComboUseQ
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Azir.Combo.UseQ"); }
        }

        public bool ComboUseQOnlyOutOfRange
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Azir.Combo.UseQOnlyOutOfAA"); }
        }

        public bool ComboUseQAlwaysMaxRange
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Azir.Combo.UseQAlwaysMaxRange"); }
        }

        public bool ComboUseQWhenNoWAmmo
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Azir.Combo.UseQWhenNoWAmmo"); }
        }

        public bool ComboUseW
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Azir.Combo.UseW"); }
        }

        public bool ComboUseE
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Azir.Combo.UseE"); }
        }

        public bool ComboUseR
        {
            get { return getCheckBoxItem(comboMenu, "SAutoCarry.Azir.Combo.UseR"); }
        }

        public int ComboRMinHit
        {
            get { return getSliderItem(comboMenu, "SAutoCarry.Azir.Combo.RMinHit"); }
        }

        public int ComboRMinHP
        {
            get { return getSliderItem(comboMenu, "SAutoCarry.Azir.Combo.RMinHP"); }
        }

        public bool HarassUseQ
        {
            get { return getCheckBoxItem(harassMenu, "SAutoCarry.Azir.Harass.UseQ"); }
        }

        public bool HarassUseW
        {
            get { return getCheckBoxItem(harassMenu, "SAutoCarry.Azir.Harass.UseW"); }
        }

        public int HarassMaxSoldierCount
        {
            get { return getSliderItem(harassMenu, "SAutoCarry.Azir.Harass.MaxSoldier"); }
        }

        public bool HarassToggle
        {
            get { return getKeyBindItem(harassMenu, "SAutoCarry.Azir.Harass.Toggle"); }
        }

        public int HarassMinMana
        {
            get { return getSliderItem(harassMenu, "SAutoCarry.Azir.Harass.ManaPercent"); }
        }

        public bool LaneClearUseQ
        {
            get { return getCheckBoxItem(laneClearMenu, "SAutoCarry.Azir.LaneClear.UseQ"); }
        }

        public bool LaneClearUseW
        {
            get { return getCheckBoxItem(laneClearMenu, "SAutoCarry.Azir.LaneClear.UseW"); }
        }

        public int LaneClearQMinMinion
        {
            get { return getSliderItem(laneClearMenu, "SAutoCarry.Azir.LaneClear.MinQMinion"); }
        }

        public bool LaneClearToggle
        {
            get { return getKeyBindItem(laneClearMenu, "SAutoCarry.Azir.LaneClear.Toggle"); }
        }

        public int LaneClearMinMana
        {
            get { return getSliderItem(laneClearMenu, "SAutoCarry.Azir.LaneClear.ManaPercent"); }
        }

        public bool JumpActive
        {
            get { return getKeyBindItem(miscMenu, "SAutoCarry.Azir.Misc.Jump"); }
        }

        public bool JumpEQActive
        {
            get { return getKeyBindItem(miscMenu, "SAutoCarry.Azir.Misc.JumpEQ"); }
        }

        public bool InsecActive
        {
            get { return getKeyBindItem(miscMenu, "SAutoCarry.Azir.Misc.Insec"); }
        }

        public bool WQKillSteal
        {
            get { return getCheckBoxItem(miscMenu, "SAutoCarry.Azir.Misc.WQKillSteal"); }
        }

        public bool BlockR
        {
            get { return getCheckBoxItem(miscMenu, "SAutoCarry.Azir.Misc.BlockR"); }
        }

        public bool AntiGapCloserEnabled
        {
            get { return getCheckBoxItem(antiGapMenu, "SAutoCarry.Azir.Misc.AntiGapCloser.Enable"); }
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

        public static void CreateConfigMenu()
        {
            ConfigMenu = MainMenu.AddMenu("SAutoCarry - Azir", "Azir");
            ConfigMenu.AddLabel("This is a very complicated addon that conflicts with the target selector of EB.");
            ConfigMenu.AddLabel("1. Just ignore EB's Target Selector.");

            comboMenu = ConfigMenu.AddSubMenu("Combo", "SAutoCarry.Azir.Combo");
            comboMenu.Add("SAutoCarry.Azir.Combo.UseQ", new CheckBox("Use Q"));
            comboMenu.Add("SAutoCarry.Azir.Combo.UseQOnlyOutOfAA", new CheckBox("Use Q Only When Enemy out of range"));
            comboMenu.Add("SAutoCarry.Azir.Combo.UseQAlwaysMaxRange", new CheckBox("Always Cast Q To Max Range", false));
            comboMenu.Add("SAutoCarry.Azir.Combo.UseQWhenNoWAmmo", new CheckBox("Use Q When Out of W Ammo", false));
            comboMenu.Add("SAutoCarry.Azir.Combo.UseW", new CheckBox("Use W"));
            comboMenu.Add("SAutoCarry.Azir.Combo.UseE", new CheckBox("Use E If target is killable"));
            comboMenu.Add("SAutoCarry.Azir.Combo.UseR", new CheckBox("Use R"));
            comboMenu.Add("SAutoCarry.Azir.Combo.RMinHit", new Slider("Min R Hit", 1, 1, 5));
            comboMenu.Add("SAutoCarry.Azir.Combo.RMinHP", new Slider("Use R whenever my health < ", 20));

            harassMenu = ConfigMenu.AddSubMenu("Harass", "SAutoCarry.Azir.Harass");
            harassMenu.Add("SAutoCarry.Azir.Harass.UseQ", new CheckBox("Use Q"));
            harassMenu.Add("SAutoCarry.Azir.Harass.UseW", new CheckBox("Use W"));
            harassMenu.Add("SAutoCarry.Azir.Harass.MaxSoldier", new Slider("Max Soldier Count", 1, 1, 3));
            harassMenu.Add("SAutoCarry.Azir.Harass.ManaPercent", new Slider("Min. Mana Percent", 40));
            harassMenu.Add("SAutoCarry.Azir.Harass.Toggle",
                new KeyBind("Toggle Harass", false, KeyBind.BindTypes.PressToggle, 'J'));

            laneClearMenu = ConfigMenu.AddSubMenu("LaneClear", "SAutoCarry.Azir.LaneClear");
            laneClearMenu.Add("SAutoCarry.Azir.LaneClear.UseQ", new CheckBox("Use Q"));
            laneClearMenu.Add("SAutoCarry.Azir.LaneClear.MinQMinion", new Slider("Q Min. Minions", 3, 1, 5));
            laneClearMenu.Add("SAutoCarry.Azir.LaneClear.UseW", new CheckBox("Use W"));
            laneClearMenu.Add("SAutoCarry.Azir.LaneClear.Toggle",
                new KeyBind("Toggle Spellfarm", false, KeyBind.BindTypes.PressToggle, 'L'));
            laneClearMenu.Add("SAutoCarry.Azir.LaneClear.ManaPercent", new Slider("Min. Mana Percent", 40));

            miscMenu = ConfigMenu.AddSubMenu("Misc", "SAutoCarry.Azir.Misc");
            miscMenu.Add("SAutoCarry.Azir.Misc.Jump",
                new KeyBind("Jump To Cursor (Always Jumps Max Range)", false, KeyBind.BindTypes.HoldActive, 'G'));
            miscMenu.Add("SAutoCarry.Azir.Misc.JumpEQ",
                new KeyBind("Jump To Cursor (Jumps with juke)", false, KeyBind.BindTypes.HoldActive, 'A'));
            miscMenu.Add("SAutoCarry.Azir.Misc.Insec",
                new KeyBind("Insec Selected Target", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("SAutoCarry.Azir.Misc.WQKillSteal", new CheckBox("Use W->Q to KillSteal"));
            miscMenu.Add("SAutoCarry.Azir.Misc.BlockR", new CheckBox("Block R if wont hit anyone"));

            antiGapMenu = ConfigMenu.AddSubMenu("AntiGapCloser (R)", "SAutoCarry.Azir.Misc.AntiGapCloser");
            foreach (var enemy in HeroManager.Enemies)
            {
                if (AntiGapcloser.Spells.Any(p => p.ChampionName == enemy.ChampionName))
                {
                    var enemy1 = enemy;
                    var spells = AntiGapcloser.Spells.Where(p => p.ChampionName == enemy1.ChampionName);
                    foreach (var gapcloser in spells)
                    {
                        antiGapMenu.Add("SAutoCarry.Azir.Misc.AntiGapCloser." + gapcloser.SpellName,
                            new CheckBox(string.Format("{0} ({1})", gapcloser.ChampionName, gapcloser.Slot), false));
                    }
                }
            }
            antiGapMenu.Add("SAutoCarry.Azir.Misc.AntiGapCloser.Enable", new CheckBox("Enabled"));
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 825f);
            Spells[Q].SetSkillshot(0.25f, 70f, 1600f, false, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 450f);
            Spells[W].SetSkillshot(0.25f, 70f, 0f, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 1250f);
            Spells[E].SetSkillshot(0.25f, 100, 1700f, false, SkillshotType.SkillshotLine);

            Spells[R] = new Spell(SpellSlot.R, 450f);
            Spells[R].SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
        }

        public void Combo()
        {
            var t = TargetSelector.GetTarget(Spells[Q].Range, DamageType.Magical);
            var extendedTarget = TargetSelector.GetTarget(Spells[Q].Range + 400, DamageType.Magical);

            if (t != null)
            {
                if (ComboUseR && Spells[R].IsReady() && t.IsValidTarget(Spells[R].Range) && ShouldCast(SpellSlot.R, t))
                    Spells[R].CastIfHitchanceEquals(t, HitChance.High);

                if (ComboUseW && Spells[W].IsReady() && ShouldCast(SpellSlot.W, t))
                    Spells[W].Cast(ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), 450));

                if (ComboUseQ && Spells[Q].IsReady() && ShouldCast(SpellSlot.Q, t))
                {
                    foreach (var soldier in SoldierMgr.ActiveSoldiers)
                    {
                        if (ObjectManager.Player.ServerPosition.LSDistance(t.ServerPosition) < Spells[Q].Range)
                        {
                            Spells[Q].UpdateSourcePosition(soldier.Position, ObjectManager.Player.ServerPosition);
                            var predRes = Spells[Q].GetPrediction(t);
                            if (predRes.Hitchance >= HitChance.High)
                            {
                                var pos = predRes.CastPosition.To2D();
                                if (ComboUseQAlwaysMaxRange)
                                    pos = ObjectManager.Player.ServerPosition.To2D().LSExtend(pos, Spells[Q].Range);
                                Spells[Q].Cast(pos);
                                return;
                            }
                        }
                    }
                }
            }

            if (extendedTarget != null)
            {
                if (ComboUseE && Spells[E].IsReady() && ShouldCast(SpellSlot.E, extendedTarget))
                {
                    foreach (var soldier in SoldierMgr.ActiveSoldiers)
                    {
                        if (Spells[E].WillHit(extendedTarget, soldier.Position))
                        {
                            Spells[E].Cast(soldier.Position);
                            return;
                        }
                    }
                }
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassMinMana)
                return;

            var t = TargetSelector.GetTarget(Spells[Q].Range, DamageType.Magical);
            if (t == null)
                return;

            if (HarassUseW && Spells[W].IsReady() && ShouldCast(SpellSlot.W, t))
                Spells[W].Cast(ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), 450));

            if (HarassUseQ && Spells[Q].IsReady() && ShouldCast(SpellSlot.Q, t))
            {
                foreach (var soldier in SoldierMgr.ActiveSoldiers)
                {
                    if (ObjectManager.Player.ServerPosition.LSDistance(t.ServerPosition) < Spells[Q].Range)
                    {
                        Spells[Q].UpdateSourcePosition(soldier.Position, ObjectManager.Player.ServerPosition);
                        if (Spells[Q].CastIfHitchanceEquals(t, HitChance.High))
                            return;
                    }
                }
            }
        }


        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana || !LaneClearToggle)
                return;

            if (Utils.TickCount - lastLaneClearTick > 250)
            {
                if (LaneClearUseW && Spells[W].IsReady() && Spells[W].Instance.Ammo != 0)
                {
                    var minions = MinionManager.GetMinions(Spells[W].Range + SoldierMgr.SoldierAttackRange/2f);
                    if (minions.Count > 1)
                    {
                        var loc =
                            MinionManager.GetBestCircularFarmLocation(
                                minions.Select(p => p.ServerPosition.To2D()).ToList(), SoldierMgr.SoldierAttackRange,
                                Spells[W].Range);
                        if (loc.MinionsHit > 2)
                            Spells[W].Cast(loc.Position);
                    }
                }

                if (LaneClearUseQ && Spells[Q].IsReady())
                {
                    var bestfarm =
                        MinionManager.GetBestCircularFarmLocation(
                            MinionManager.GetMinions(Spells[Q].Range + 100)
                                .Select(p => p.ServerPosition.To2D())
                                .ToList(), SoldierMgr.SoldierAttackRange, Spells[Q].Range + 100);
                    if (bestfarm.MinionsHit >= LaneClearQMinMinion)
                        Spells[Q].Cast(bestfarm.Position);
                }
                lastLaneClearTick = Utils.TickCount;
            }
        }

        private void BeforeOrbwalk(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }

            if (Spells[R].IsReady())
                Spells[R].Width = 133*(3 + Spells[R].Level);

            if (JumpActive)
                Jump(Game.CursorPos);

            if (JumpEQActive)
                Jump(Game.CursorPos, true);

            if (InsecActive)
                Insec();

            if (WQKillSteal)
                KillSteal();

            if (HarassToggle)
                Harass();
        }

        private void BeforeDraw(EventArgs args)
        {
            if (InsecTo.IsValid())
                Render.Circle.DrawCircle(InsecTo.To3D2(), 200, Color.DarkBlue);
        }

        public void KillSteal()
        {
            if (!Spells[Q].IsReady() || (SoldierMgr.ActiveSoldiers.Count == 0 && !Spells[W].IsReady()))
                return;

            foreach (
                var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(Spells[Q].Range + 100) && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 20)
                {
                    if (SoldierMgr.ActiveSoldiers.Count == 0)
                        Spells[W].Cast(ObjectManager.Player.Position.To2D().Extend(target.Position.To2D(), 450));
                    else
                        Spells[Q].CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }

        public void Jump(Vector3 pos, bool juke = false, bool castq = true)
        {
            Orbwalker.OrbwalkTo(pos);
            if (Math.Abs(Spells[E].Cooldown) < 0.00001)
            {
                var extended = ObjectManager.Player.ServerPosition.To2D().Extend(pos.To2D(), 800f);
                if (!JumpTo.IsValid())
                    JumpTo = pos.To2D();

                if (Spells[W].IsReady() && SoldierMgr.ActiveSoldiers.Count == 0)
                {
                    if (juke)
                    {
                        var outRadius = 250/(float) Math.Cos(2*Math.PI/12);

                        for (var i = 1; i <= 12; i++)
                        {
                            var angle = i*2*Math.PI/12;
                            var x = ObjectManager.Player.Position.X + outRadius*(float) Math.Cos(angle);
                            var y = ObjectManager.Player.Position.Y + outRadius*(float) Math.Sin(angle);
                            if (NavMesh.GetCollisionFlags(x, y).HasFlag(CollisionFlags.Wall) &&
                                !ObjectManager.Player.ServerPosition.To2D().Extend(new Vector2(x, y), 500f).IsWall())
                            {
                                Spells[W].Cast(ObjectManager.Player.ServerPosition.To2D()
                                    .Extend(new Vector2(x, y), 800f));
                                return;
                            }
                        }
                    }
                    Spells[W].Cast(extended);
                }

                if (SoldierMgr.ActiveSoldiers.Count > 0 && Spells[Q].IsReady())
                {
                    var closestSoldier =
                        SoldierMgr.ActiveSoldiers.MinOrDefault(s => s.Position.To2D().LSDistance(extended, true));
                    CastELocation = closestSoldier.Position.To2D();
                    CastQLocation = closestSoldier.Position.To2D().Extend(JumpTo, 800f);

                    if (CastELocation.LSDistance(JumpTo) > ObjectManager.Player.ServerPosition.To2D().LSDistance(JumpTo) &&
                        !juke && castq)
                    {
                        CastQLocation = extended;
                        CastET = Utils.TickCount + 250;
                        Spells[Q].Cast(CastQLocation);
                    }
                    else
                    {
                        Spells[E].Cast(CastELocation, true);
                        if (ObjectManager.Player.ServerPosition.To2D().LSDistance(CastELocation) < 700 && castq)
                            Utility.DelayAction.Add(250, () => Spells[Q].Cast(CastQLocation, true));
                    }
                }
            }
            else
            {
                if (Spells[Q].IsReady() && CastELocation.LSDistance(ObjectManager.Player.ServerPosition) <= 200 && castq)
                    Spells[Q].Cast(CastQLocation, true);

                JumpTo = Vector2.Zero;
            }
        }

        public void Insec()
        {
            if (TargetSelector.SelectedTarget != null)
            {
                if (TargetSelector.SelectedTarget.IsValidTarget(900))
                {
                    if (Spells[Q].IsReady())
                    {
                        if (Spells[R].IsReady())
                        {
                            var direction = (TargetSelector.SelectedTarget.ServerPosition - ObjectManager.Player.ServerPosition).To2D().Normalized();
                            var insecPos = TargetSelector.SelectedTarget.ServerPosition.To2D() + direction*200f;
                            if (!InsecLocation.IsValid())
                                InsecLocation = ObjectManager.Player.ServerPosition.To2D();
                            Jump(insecPos.To3D());
                        }
                    }
                    else if (ObjectManager.Player.ServerPosition.LSDistance(TargetSelector.SelectedTarget.ServerPosition) < 400 && InsecLocation.IsValid())
                    {
                        if (InsecTo.IsValid() && InsecTo.LSDistance(ObjectManager.Player.ServerPosition.To2D()) < 1500)
                            Spells[R].Cast(InsecTo);
                        else
                            Spells[R].Cast(InsecLocation);
                        if (!Spells[R].IsReady())
                            InsecLocation = Vector2.Zero;
                    }
                }
                else
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }
            }
            else
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }
        }


        public bool ShouldCast(SpellSlot slot, AIHeroClient target)
        {
            switch (slot)
            {
                case SpellSlot.Q:
                {
                    if (ComboUseQOnlyOutOfRange && SoldierMgr.InAARange(target))
                        return false;

                    if (SoldierMgr.ActiveSoldiers.Count == 0)
                        return false;

                    if (ComboUseQWhenNoWAmmo && Spells[W].Instance.Ammo != 0)
                        return false;

                    return true;
                }

                case SpellSlot.W:
                {
                    if (Spells[W].Instance.Ammo == 0)
                        return false;

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                        SoldierMgr.ActiveSoldiers.Count >= HarassMaxSoldierCount)
                        return false;

                    return true;
                }

                case SpellSlot.E:
                {
                    if (CalculateDamageE(target) + AutoAttack.GetDamage(target) >= target.Health &&
                        HeroManager.Enemies.Count(p => p.IsValidTarget() && p.LSDistance(target.ServerPosition) < 600) < 2)
                        return true;

                    return false;
                }

                case SpellSlot.R:
                {
                    if (CalculateDamageR(target) >= target.Health - 150)
                        return true;

                    if (ObjectManager.Player.HealthPercent < ComboRMinHP)
                        return true;

                    if (
                        IsWallStunable(target.ServerPosition.To2D(),
                            ObjectManager.Player.ServerPosition.To2D()
                                .Extend(Spells[R].GetPrediction(target).UnitPosition, 200 - target.BoundingRadius)) &&
                        CalculateDamageR(target) >= target.Health/2f)
                        return true;

                    if (ComboRMinHit > 1 && Spells[R].GetHitCount() > ComboRMinHit)
                        return true;

                    return false;
                }
            }

            return false;
        }

        private bool IsWallStunable(Vector2 from, Vector2 to)
        {
            var count = from.LSDistance(to);
            for (uint i = 0; i <= count; i += 25)
            {
                var pos = from.Extend(ObjectManager.Player.ServerPosition.To2D(), -i);
                var colFlags = NavMesh.GetCollisionFlags(pos.X, pos.Y);
                if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                    return true;
            }
            return false;
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender,
            GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "azirq")
                    Orbwalker.ResetAutoAttack();

                if (JumpActive || InsecActive)
                {
                    if (args.SData.Name == "azire" && Utils.TickCount - CastQT < 500 + Game.Ping)
                    {
                        Spells[Q].Cast(CastQLocation, true);
                        CastQT = 0;
                    }

                    if (args.SData.Name == "azirq" && Utils.TickCount - CastET < 500 + Game.Ping)
                    {
                        Spells[E].Cast(CastELocation, true);
                        CastET = 0;
                    }
                }
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (AntiGapCloserEnabled)
            {
                if (getCheckBoxItem(antiGapMenu,
                    "SAutoCarry.Azir.Misc.AntiGapCloser." + gapcloser.Sender.Spellbook.GetSpell(gapcloser.Slot).Name))
                {
                    if (gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < 200)
                        Spells[R].Cast(gapcloser.End.Extend(gapcloser.Start, 100));
                }
            }
        }

        public override double CalculateDamageW(AIHeroClient target)
        {
            return SoldierMgr.GetAADamage(target);
        }


        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint) WindowsMessages.WM_LBUTTONDBLCLK)
            {
                var clickedObject = ObjectManager.Get<GameObject>().Where(p => p.Position.LSDistance(Game.CursorPos, true) < 40000).OrderBy(q => q.Position.LSDistance(Game.CursorPos, true)).FirstOrDefault();

                if (clickedObject != null)
                {
                    InsecTo = clickedObject.Position.To2D();
                    if (clickedObject.IsMe)
                        InsecTo = Vector2.Zero;
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R && BlockR)
            {
                args.Process = false;
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.IsValidTarget(Spells[R].Range + 200))
                    {
                        var pred = Spells[R].GetPrediction(enemy);
                        args.Process |= pred.Hitchance > HitChance.Low;
                    }
                }
            }
        }
    }
    */
}