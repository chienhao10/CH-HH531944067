using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TreeLib.Extensions;
using Color = SharpDX.Color;
using Geometry = LeagueSharp.Common.Geometry;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using ezEvade;

namespace jesuisFiora
{
    internal static class Program
    {
        #region Static Fields

        public static Menu Menu;

        public static Color ScriptColor = new Color(255, 0, 255);

        #endregion

        #region Public Properties

        public static LeagueSharp.Common.Spell E
        {
            get { return SpellManager.E; }
        }

        public static LeagueSharp.Common.Spell Q
        {
            get { return SpellManager.Q; }
        }

        public static LeagueSharp.Common.Spell R
        {
            get { return SpellManager.R; }
        }

        public static LeagueSharp.Common.Spell W
        {
            get { return SpellManager.W; }
        }

        #endregion

        #region Properties

        private static IEnumerable<AIHeroClient> Enemies
        {
            get { return HeroManager.Enemies; }
        }

        private static float FioraAutoAttackRange
        {
            get { return Orbwalking.GetRealAutoAttackRange(Player); }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static List<Obj_AI_Base> QJungleMinions
        {
            get { return MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral); }
        }

        private static List<Obj_AI_Base> QLaneMinions
        {
            get { return MinionManager.GetMinions(Q.Range); }
        }

        #endregion

        #region Public Methods and Operators

        public static bool CastItems(Obj_AI_Base target)
        {
            if (Player.LSIsDashing() || Orbwalker.IsAutoAttacking)
            {
                return false;
            }

            var botrk = ItemManager.Botrk;
            if (botrk.IsValidAndReady() && botrk.Cast(target))
            {
                return true;
            }

            var cutlass = ItemManager.Cutlass;
            if (cutlass.IsValidAndReady() && cutlass.Cast(target))
            {
                return true;
            }

            var youmuus = ItemManager.Youmuus;
            if (youmuus.IsValidAndReady() && youmuus.Cast())
            {
                return true;
            }

            var units =
                MinionManager.GetMinions(385, MinionTypes.All, MinionTeam.NotAlly).Count(o => !(o is Obj_AI_Turret));
            var heroes = Player.GetEnemiesInRange(385).Count;
            var count = units + heroes;

            var tiamat = ItemManager.Tiamat;
            if (tiamat.IsValidAndReady() && count > 0 && tiamat.Cast())
            {
                return true;
            }

            var hydra = ItemManager.RavenousHydra;
            if (hydra.IsValidAndReady() && count > 0 && hydra.Cast())
            {
                return true;
            }

            var titanic = ItemManager.TitanicHydra;
            return titanic.IsValidAndReady() && count > 0 && titanic.Cast();
        }

        public static bool CastQ(AIHeroClient target, FioraPassive passive, bool force = false)
        {
            if (!Q.IsReady() || !target.IsValidTarget(Q.Range))
            {
                return false;
            }

            var qPos = GetBestCastPosition(target, passive);

            if (!Q.IsInRange(qPos.Position) || qPos.Position.DistanceToPlayer() < 75)
            {
                Console.WriteLine("NOT IN RANGE");
                return false;
            }

            // cast q because we don't care
            if (!getCheckBoxItem(passiveM, "QPassive") || force)
            {
                Console.WriteLine("FORCE Q");
                return Q.Cast(qPos.Position);
            }

            // q pos under turret
            if (getCheckBoxItem(qMenu, "QBlockTurret") && qPos.Position.UnderTurret(true))
            {
                return false;
            }

            var forcePassive = getCheckBoxItem(passiveM, "QPassive");
            var passiveType = qPos.PassiveType.ToString();

            // passive type is none, no special checks needed
            if (passiveType == "None")
            {
                //  Console.WriteLine("NO PASSIVE");
                return !forcePassive && Q.Cast(qPos.Position);
            }

            if (getCheckBoxItem(passiveM, "QInVitalBlock") && qPos.SimplePolygon.IsInside(Player.ServerPosition))
            {
                return false;
            }

            var active = passiveM["Q" + passiveType] != null && getCheckBoxItem(passiveM, "Q" + passiveType);

            if (!active)
            {
                return false;
            }

            if (qPos.Position.DistanceToPlayer() < 730)
            {
                return (from point in GetQPolygon(qPos.Position).Points
                        from vitalPoint in
                            qPos.Polygon.Points.OrderBy(p => p.DistanceToPlayer()).ThenByDescending(p => p.Distance(target))
                        where point.Distance(vitalPoint) < 20
                        select point).Any() && Q.Cast(qPos.Position);
            }

            Console.WriteLine("DEFAULT CAST");
            return !forcePassive && Q.Cast(qPos.Position);
        }

        public static bool CastR(Obj_AI_Base target)
        {
            return R.IsReady() && target.IsValidTarget(R.Range) && R.Cast(target).IsCasted();
        }

        public static bool CastW(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget(W.Range))
            {
                Console.WriteLine("CAST W");
                return W.Cast(Game.CursorPos);
            }

            var cast = W.GetPrediction(target);
            var castPos = W.IsInRange(cast.CastPosition) ? cast.CastPosition : target.ServerPosition;

            Console.WriteLine("CAST W");
            return W.Cast(castPos);
        }

        public static bool ComboR(Obj_AI_Base target)
        {
            if (getCheckBoxItem(rMenu, "RComboSelected"))
            {
                var unit = TargetSelector.SelectedTarget;
                if (unit != null && unit.IsValid && unit.NetworkId.Equals(target.NetworkId) && CastR(target))
                {
                    return true;
                }
                return false;
            }

            if (!CastR(target))
            {
                return false;
            }

            return true;
        }

        public static void DuelistMode()
        {
            if (!getCheckBoxItem(rMenu, "RCombo") || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                !getBoxItem(rMenu, "RMode").Equals(0) || !R.IsReady() ||
                Player.CountEnemiesInRange(R.Range) == 0)

            {
                return;
            }

            var vitalCalc = getSliderItem(rMenu, "RKillVital");
            foreach (var obj in
                Enemies.Where(
                    enemy =>
                        getCheckBoxItem(rMenu, "Duelist" + enemy.ChampionName) && enemy.IsValidTarget(R.Range) &&
                        GetComboDamage(enemy, vitalCalc) >= enemy.Health &&
                        enemy.Health > Player.GetSpellDamage(enemy, SpellSlot.Q) + enemy.GetPassiveDamage(1)))
            {
                if (getCheckBoxItem(rMenu, "RComboSelected"))
                {
                    var unit = TargetSelector.SelectedTarget;
                    if (unit != null && unit.IsValid && unit.NetworkId.Equals(obj.NetworkId) && CastR(obj))
                    {
                        return;
                    }
                    return;
                }

                if (CastR(obj))
                {
                    //Hud.SelectedUnit = obj;
                }

                if (getCheckBoxItem(draw, "DuelistDraw"))
                {
                    var pos = obj.HPBarPosition;
                    Drawing.DrawText(pos.X, pos.Y - 30, System.Drawing.Color.DeepPink, "Killable!");
                }
            }
        }

        public static bool IsFarmMode()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public static void Farm()
        {
            if (!getKeyBindItem(farm, "FarmEnabled") || !IsFarmMode())
            {
                return;
            }

            var active = getCheckBoxItem(farm, "QFarm") && Q.IsReady() /*&& !Q.HasManaCondition()*/&& Player.ManaPercent >= getSliderItem(farm, "QFarmMana");
            var onlyLastHit = getCheckBoxItem(farm, "QLastHit");

            if (!active)
            {
                return;
            }

            var laneMinions = QLaneMinions;
            var jungleMinions = QJungleMinions;

            var jungleKillable = jungleMinions.FirstOrDefault(obj => obj.Health < Player.GetSpellDamage(obj, SpellSlot.Q));
            if (jungleKillable != null && Q.Cast(jungleKillable).IsCasted())
            {
                return;
            }

            var jungle = jungleMinions.MinOrDefault(obj => obj.Health);
            if (!onlyLastHit && jungle != null && Q.Cast(jungle).IsCasted())
            {
                return;
            }

            var killable = laneMinions.FirstOrDefault(obj => obj.Health < Player.GetSpellDamage(obj, SpellSlot.Q));

            if (getCheckBoxItem(farm, "QFarmAA") && killable != null && killable.IsValidTarget(FioraAutoAttackRange) &&
                !Player.UnderTurret(false))
            {
                return;
            }

            if (killable != null && Q.Cast(killable).IsCasted())
            {
                return;
            }

            var lane = laneMinions.MinOrDefault(obj => obj.Health);
            if (!onlyLastHit && lane != null && Q.Cast(lane).IsCasted()) { }
        }

        public static bool Flee()
        {
            if (!getKeyBindItem(qMenu, "QFlee"))
            {
                return false;
            }

            if (!Player.LSIsDashing() && Player.GetWaypoints().Last().Distance(Game.CursorPos) > 100)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (Q.IsReady())
            {
                Q.Cast(Player.ServerPosition.Extend(Game.CursorPos, Q.Range + 10));
            }

            return true;
        }

        public static QPosition GetBestCastPosition(AIHeroClient target, FioraPassive passive)
        {
            if (passive == null || passive.Target == null)
            {
                return new QPosition(Q.GetPrediction(target).UnitPosition);
            }

            return new QPosition(passive.CastPosition, passive.Passive, passive.Polygon, passive.SimplePolygon);
        }

        public static float GetComboDamage(AIHeroClient unit)
        {
            return GetComboDamage(unit, 0);
        }

        public static float GetComboDamage(AIHeroClient unit, int maxStacks)
        {
            var d = 2 * Player.GetAutoAttackDamage(unit);

            if (ItemManager.Youmuus.IsValidAndReady())
            {
                d += Player.GetAutoAttackDamage(unit, true) * 2;
            }

            if (Q.IsReady())
            {
                d += Player.GetSpellDamage(unit, SpellSlot.Q);
            }

            if (E.IsReady())
            {
                d += 2 * Player.GetAutoAttackDamage(unit);
            }

            if (maxStacks == 0)
            {
                if (R.IsReady())
                {
                    d += (float)unit.GetPassiveDamage(4);
                }
                else
                {
                    d += (float)unit.GetPassiveDamage();
                }
            }
            else
            {
                d += (float)unit.GetPassiveDamage(maxStacks);
            }

            return (float)d;
        }

        public static Geometry.Polygon GetQPolygon(Vector3 destination)
        {
            var polygon = new Geometry.Polygon();
            for (var i = 10; i < SpellManager.QSkillshotRange; i += 10)
            {
                if (i > SpellManager.QSkillshotRange)
                {
                    break;
                }

                polygon.Add(Player.ServerPosition.Extend(destination, i));
            }

            return polygon;
        }

        public static void KillstealQ()
        {
            if (!getCheckBoxItem(qMenu, "QKillsteal"))
            {
                return;
            }

            var unit =
                Enemies.FirstOrDefault(
                    o => o.IsValidTarget(Q.Range) && o.Health < Q.GetDamage(o) + o.GetPassiveDamage());
            if (unit != null)
            {
                CastQ(unit, unit.GetNearestPassive(), true);
            }
        }

        public static void KillstealW()
        {
            if (!getCheckBoxItem(wSpells, "WKillsteal"))
            {
                return;
            }

            if (getCheckBoxItem(wSpells, "WTurret") && Player.UnderTurret(true))
            {
                return;
            }

            var unit =
                Enemies.FirstOrDefault(
                    o => o.IsValidTarget(W.Range) && o.Health < W.GetDamage(o) && !o.IsValidTarget(FioraAutoAttackRange));
            if (unit != null)
            {
                W.Cast(unit);
            }
        }

        public static void OrbwalkToPassive(AIHeroClient target, FioraPassive passive)
        {
            if (Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (getCheckBoxItem(passiveM, "OrbwalkAA") && Orbwalker.CanAutoAttack && target.IsValidTarget(FioraAutoAttackRange))
            {
                Console.WriteLine("RETURN");
                return;
            }

            if (getCheckBoxItem(passiveM, "OrbwalkQ") && Q.IsReady())
            {
                return;
            }

            if (passive == null || passive.Target == null || passiveM["Orbwalk" + passive.Passive] == null || !getCheckBoxItem(passiveM, "Orbwalk" + passive.Passive))
            {
                return;
            }

            var pos = passive.OrbwalkPosition; //PassivePosition;

            if (pos == Vector3.Zero)
            {
                return;
            }

            var underTurret = getCheckBoxItem(passiveM, "OrbwalkTurret") && pos.UnderTurret(true);
            var outsideAARange = getCheckBoxItem(passiveM, "OrbwalkAARange") && Player.Distance(pos) > FioraAutoAttackRange + 250 + (passive.Type.Equals(FioraPassive.PassiveType.UltPassive) ? 50 : 0);
            if (underTurret || outsideAARange)
            {
                return;
            }

            var path = Player.GetPath(pos);
            var point = path.Length < 3 ? pos : path.Skip(path.Length / 2).FirstOrDefault();
            //  Console.WriteLine(path.Length);
            Console.WriteLine("ORBWALK TO PASSIVE: " + Player.Distance(pos));
            Orbwalker.OrbwalkTo(target.IsMoving ? point : pos);
        }

        public static AIHeroClient GetTarget(bool aaTarget = false)
        {
            if (aaTarget)
            {
                if (UltTarget.Target.IsValidTarget(1000))
                {
                    return UltTarget.Target;
                }

                return TargetSelector.GetTarget(FioraAutoAttackRange, DamageType.Physical);
            }

            if (UltTarget.Target.IsValidTarget(Q.Range))
            {
                return UltTarget.Target;
            }

            return TargetSelector.GetTarget(Q.Range, DamageType.Physical);
        }

        #endregion

        #region Methods

        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            var targ = target as Obj_AI_Base;

            if (!target.IsMe || targ == null)
            {
                return;
            }

            //Orbwalker.OrbwalkTo(Vector3.Zero);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !getKeyBindItem(farm, "FarmEnabled"))
            {
                return;
            }

            var hero = targ as AIHeroClient;
            if (hero != null && hero.GetUltPassiveCount() > 1)
            {
                return;
            }

            var lastCast = Player.LastCastedspell();
            if (lastCast != null && lastCast.Name == R.Instance.Name && Environment.TickCount - lastCast.Tick < 200)
            {
                return;
            }

            if (E.IsActive() && E.IsReady() && /*!E.HasManaCondition() &&*/ E.Cast())
            {
                Console.WriteLine("AFRTE");
                return;
            }

            if (ItemManager.IsActive())
            {
                CastItems(targ);
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Q.IsReady())
            {
                var vitalCircle = getCheckBoxItem(draw, "QVitalDraw");
                if (vitalCircle)
                {
                    Render.Circle.DrawCircle(Player.Position, SpellManager.QSkillshotRange, System.Drawing.Color.Purple);
                }

                var qCircle = getCheckBoxItem(draw, "QDraw");
                if (qCircle)
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Purple);
                }
            }

            if (getCheckBoxItem(draw, "1Draw") && Player.Spellbook.GetSpell(SpellSlot.W).IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.DeepPink);
            }

            if (getCheckBoxItem(draw, "3Draw") && Player.Spellbook.GetSpell(SpellSlot.R).IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.White);
            }
        }

        public static void Game_OnGameLoad()
        {
            if (!Player.IsChampion("Fiora"))
            {
                return;
            }

            TreeLib.SpellData.Evade.Init();

            Menu = MainMenu.AddMenu("jesuisFiora", "je suis Fiora");

            passiveM = Menu.AddSubMenu("Passive", "Vital Settings");
            passiveM.AddGroupLabel("Orbwalker Vital");
            passiveM.Add("OrbwalkPassive", new KeyBind("Orbwalk to Target Vital", true, KeyBind.BindTypes.PressToggle, 'N'));
            passiveM.Add("OrbwalkCombo", new CheckBox("In Combo"));
            passiveM.Add("OrbwalkHarass", new CheckBox("In Harass"));
            passiveM.Add("OrbwalkPrepassive", new CheckBox("Orbwalk PreVital"));
            passiveM.Add("OrbwalkUltPassive", new CheckBox("Orbwalk Ultimate Vital"));
            passiveM.Add("OrbwalkPassiveTimeout", new CheckBox("Orbwalk Near Timeout Vital"));
            passiveM.Add("OrbwalkSelected", new CheckBox("Only Selected Target", true));
            passiveM.Add("OrbwalkTurret", new CheckBox("Block Under Turret", false));
            passiveM.Add("OrbwalkQ", new CheckBox("Only if Q Down", false));
            passiveM.Add("OrbwalkAARange", new CheckBox("Only in AA Range", false));
            passiveM.Add("OrbwalkAA", new CheckBox("Only if not able to AA", false));
            passiveM.AddSeparator();
            passiveM.AddGroupLabel("Q Vital");
            passiveM.Add("QPassive", new CheckBox("Only Q to Vitals", true));
            passiveM.Add("QUltPassive", new CheckBox("Q to Ultimate Vital"));
            passiveM.Add("QPrepassive", new CheckBox("Q to PreVital", false));
            passiveM.Add("QPassiveTimeout", new CheckBox("Q to Near Timeout Vital"));
            passiveM.Add("QInVitalBlock", new CheckBox("Block Q inside Vital Polygon"));
            passiveM.AddSeparator();
            passiveM.Add("DrawCenter", new CheckBox("Draw Vital Center"));
            passiveM.Add("DrawPolygon", new CheckBox("Draw Vital Polygon", false));
            passiveM.Add("SectorMaxRadius", new Slider("Vital Polygon Range", 310, 300, 400));
            passiveM.Add("SectorAngle", new Slider("Vital Polygon Angle", 70, 60, 90));
            passiveM.AddSeparator();

            qMenu = Menu.AddSubMenu("Q", "Q");
            qMenu.Add("QCombo", new CheckBox("Use in Combo"));
            qMenu.Add("QHarass", new CheckBox("Use in Harass"));
            qMenu.Add("QRangeDecrease", new Slider("Decrease Q Range", 10, 0, 150));
            Q.Range = 750 - qMenu["QRangeDecrease"].Cast<Slider>().CurrentValue;
            qMenu["QRangeDecrease"].Cast<Slider>().OnValueChange += (sender, eventArgs) =>
            {
                Q.Range = 750 - eventArgs.NewValue;
            };
            qMenu.Add("QBlockTurret", new CheckBox("Block Q Under Turret", false));
            qMenu.Add("QFlee", new KeyBind("Q Flee", false, KeyBind.BindTypes.HoldActive, 'T'));
            qMenu.Add("QKillsteal", new CheckBox("Use for Killsteal"));

            wSpells = Menu.AddSubMenu("Blocked Spells");
            wSpells.Add("WSpells", new KeyBind("Enabled", true, KeyBind.BindTypes.PressToggle, 'U'));
            wSpells.Add("WMode", new ComboBox("W Spellblock to: ", 0, "Spell Caster", "Target"));
            wSpells.Add("WKillsteal", new CheckBox("Use for Killsteal"));
            wSpells.Add("WTurret", new CheckBox("Block W Under Enemy Turret", false));

            SpellBlock.Initialize(wSpells);
            Dispeller.Initialize(wSpells);

            eMenu = Menu.AddSubMenu("E", "E");
            eMenu.Add("ECombo", new CheckBox("Use in Combo"));
            eMenu.Add("EHarass", new CheckBox("Use in Harass"));

            rMenu = Menu.AddSubMenu("R", "R");
            rMenu.AddGroupLabel("Duelist Mode Champions");
            foreach (var enemy in Enemies)
            {
                rMenu.Add("Duelist" + enemy.ChampionName, new CheckBox("Use on " + enemy.ChampionName));
            }
            rMenu.AddSeparator();
            rMenu.Add("RCombo", new CheckBox("Use R"));
            rMenu.Add("RMode", new ComboBox("Cast Mode", 0, "Duelist", "Combo"));
            rMenu.Add("RToggle", new KeyBind("Toggle Mode", false, KeyBind.BindTypes.HoldActive, 'L'));
            rMenu["RToggle"].Cast<KeyBind>().OnValueChange += (sender, eventArgs) =>
            {
                if (!eventArgs.NewValue)
                {
                    return;
                }
                var mode = rMenu["RMode"].Cast<ComboBox>().CurrentValue;
                var index = mode == 0 ? 1 : 0;
                mode = index;
            };
            rMenu.Add("RKillVital", new Slider("Duelist Mode Min Vitals", 2, 0, 4));
            rMenu.Add("RComboSelected", new CheckBox("Use R Selected on Selected Unit Only"));
            rMenu.Add("RSmartQ", new CheckBox("Use Smart Q in Ult"));

            items = Menu.AddSubMenu("Items", "Items");
            items.Add("ItemsCombo", new CheckBox("Use in Combo"));
            items.Add("ItemsHarass", new CheckBox("Use in Harass"));

            farm = Menu.AddSubMenu("Farm", "Farm");
            farm.AddGroupLabel("Q");
            farm.Add("QFarm", new CheckBox("Use Q in Farm"));
            farm.Add("QLastHit", new CheckBox("Q Last Hit (Only Killable)", false));
            farm.Add("QFarmAA", new CheckBox("Only Q out of AA Range", false));
            farm.Add("QFarmMana", new Slider("Q Min Mana Percent", 40));
            farm.AddSeparator();
            farm.AddGroupLabel("E");
            farm.Add("ELaneClear", new CheckBox("Use in LaneClear"));
            farm.AddSeparator();
            farm.Add("FarmEnabled", new KeyBind("Farm Enabled", false, KeyBind.BindTypes.PressToggle, 'J'));
            farm.Add("ItemsLaneClear", new CheckBox("Use Items in LaneClear"));

            draw = Menu.AddSubMenu("Drawing", "Drawing");
            draw.Add("QVitalDraw", new CheckBox("Draw Q Vital Range"));//, System.Drawing.Color.Purple, SpellManager.QSkillshotRange, false);
            draw.Add("QDraw", new CheckBox("Draw Q Max Range"));//, System.Drawing.Color.Purple, Q.Range, false);
            draw.Add("1Draw", new CheckBox("Draw W"));//, System.Drawing.Color.DeepPink, W.Range, false);
            draw.Add("3Draw", new CheckBox("Draw R"));//, System.Drawing.Color.White, R.Range, false);
            draw.Add("DuelistDraw", new CheckBox("Duelist Mode: Killable Target"));

            misc = Menu.AddSubMenu("Misc", "Misc");
            misc.Add("ManaHarass", new Slider("Harass Min Mana Percent", 40));

            PassiveManager.Initialize();

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPostAttack += AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("<font color=\"{0}\">jesuisFiora Loaded!</font>", System.Drawing.Color.DeepPink);
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

        public static Menu misc, draw, farm, items, rMenu, eMenu, wSpells, qMenu, passiveM;

        public static bool IsComboMode()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Orbwalker.SetOrbwalkingPoint(Vector3.Zero);

            if (Player.IsDead || Flee())
            {
                return;
            }

            KillstealQ();
            KillstealW();
            DuelistMode();
            Farm();

            if (Player.LSIsDashing() || Orbwalker.IsAutoAttacking || Player.Spellbook.IsCastingSpell)
            {
                return;
            }

            if (!IsComboMode())
            {
                return;
            }

            var aaTarget = GetTarget(true);
            var passive = new FioraPassive();

            string s;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                s = "Combo";
            }
            else
            {
                s = "Harass";
            }

            if (aaTarget.IsValidTarget())
            {
                passive = aaTarget.GetNearestPassive();
                if (getKeyBindItem(passiveM, "OrbwalkPassive") && getCheckBoxItem(passiveM, "Orbwalk" + s))
                {
                    var selTarget = TargetSelector.SelectedTarget;
                    if (!getCheckBoxItem(passiveM, "OrbwalkSelected") || (selTarget != null && selTarget.NetworkId.Equals(aaTarget.NetworkId)))
                    {
                        OrbwalkToPassive(aaTarget, passive);
                    }
                }
                Orbwalker.ForcedTarget = aaTarget;
            }

            var target = GetTarget();
            if (!target.IsValidTarget(W.Range))
            {
                return;
            }

            var vital = aaTarget != null && target.NetworkId.Equals(aaTarget.NetworkId) ? passive : target.GetNearestPassive();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Player.ManaPercent < getSliderItem(misc, "ManaHarass"))
            {
                return;
            }

            if (R.IsActive() /*&& !R.HasManaCondition()*/&& getBoxItem(rMenu, "RMode").Equals(1) && ComboR(target))
            {
                return;
            }

            if (Q.IsActive()) // && !Q.HasManaCondition())
            {
                if (target.IsValidTarget(FioraAutoAttackRange) && !Orbwalking.IsAutoAttack(Player.LastCastedSpellName()))
                {
                    return;
                }

                if (target.ChampionName.Equals("Poppy") && target.HasBuff("poppywzone"))
                {
                    return;
                }

                var count = target.GetUltPassiveCount();

                if (!getCheckBoxItem(rMenu, "RSmartQ") || count == 0)
                {
                    CastQ(target, vital);
                    return;
                }
                if (count > 2)
                {
                    return;
                }

                CastQ(target, target.GetFurthestPassive());

                /*  var path = target.GetWaypoints();
                if (path.Count == 1 || Player.Distance(target) < 700)
                {
                    CastQ(target);
                    return;
                }

                var d = target.Distance(path[1]);
                var d2 = Player.Distance(path[1]);
                var t = d / target.MoveSpeed;
                var dT = Q.Delay + Game.Ping / 2000f - t;
                if ((dT > .2f || (d2 < 690 && dT > -1)) && CastQ(target))
                {
                    //  Console.WriteLine("{0} {1}", dT, d2);
                }*/
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!getKeyBindItem(wSpells, "WSpells") || !W.IsReady())
            {
                return;
            }

            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || !unit.IsEnemy)
            {
                return;
            }

            // spell handled by evade
            if (SpellDatabase.GetByName(args.SData.Name) != null)
            {
                Console.WriteLine("EVADE PROCESS SPELL RETURN");
                return;
            }

            Console.WriteLine("({0}) {1}", args.Slot, args.SData.Name);
            if (!SpellBlock.Contains(unit, args))
            {
                return;
            }

            var castUnit = unit;
            var type = args.SData.TargettingType;

            Console.WriteLine(
                "PassiveType: {0} Range: {1} Radius: {2}", type, args.SData.CastRange, args.SData.CastRadius);
            Console.WriteLine("Distance: " + args.End.DistanceToPlayer());

            if (!unit.IsValidTarget() || getBoxItem(wSpells, "WMode") == 1)
            {
                var target = TargetSelector.SelectedTarget;
                if (target == null || !target.IsValidTarget(W.Range))
                {
                    target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                }

                if (target != null && target.IsValidTarget(W.Range))
                {
                    castUnit = target;
                }
            }

            if (type.IsSkillShot())
            {
                if (unit.ChampionName.Equals("Bard") && args.End.DistanceToPlayer() < 300)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(400 + (int)(unit.Distance(Player) / 7f), () => CastW(castUnit));
                }
                else if (unit.ChampionName.Equals("Riven") && args.End.DistanceToPlayer() < 260)
                {
                    Console.WriteLine("RIVEN");
                    CastW(castUnit);
                }
                else if (args.End.DistanceToPlayer() < 60)
                {
                    CastW(castUnit);
                }
            }
            if (type.IsTargeted() && args.Target != null)
            {
                if (!args.Target.IsMe ||
                    (args.Target.Name.Equals("Barrel") && args.Target.DistanceToPlayer() > 200 &&
                     args.Target.DistanceToPlayer() < 400))
                {
                    return;
                }

                if (getCheckBoxItem(wSpells, "WTurret") && Player.UnderTurret(true))
                {
                    return;
                }

                if (unit.ChampionName.Equals("Nautilus") ||
                    (unit.ChampionName.Equals("Caitlyn") && args.Slot.Equals(SpellSlot.R)))
                {
                    var d = unit.DistanceToPlayer();
                    var travelTime = d / args.SData.MissileSpeed;
                    var delay = travelTime * 1000 - W.Delay + 150;
                    Console.WriteLine("TT: " + travelTime + " " + delay);
                    LeagueSharp.Common.Utility.DelayAction.Add((int)delay, () => CastW(castUnit));
                    return;
                }

                CastW(castUnit);
            }
            else if (type.Equals(SpellDataTargetType.LocationAoe) && args.End.DistanceToPlayer() < args.SData.CastRadius)
            {
                // annie moving tibbers
                if (unit.ChampionName.Equals("Annie") && args.Slot.Equals(SpellSlot.R))
                {
                    return;
                }
                CastW(castUnit);
            }
            else if (type.Equals(SpellDataTargetType.Cone) && args.End.DistanceToPlayer() < args.SData.CastRadius)
            {
                CastW(castUnit);
            }
            else if (type.Equals(SpellDataTargetType.SelfAoe) || type.Equals(SpellDataTargetType.Self))
            {
                var d = args.End.Distance(Player.ServerPosition);
                var p = args.SData.CastRadius > 5000 ? args.SData.CastRange : args.SData.CastRadius;
                Console.WriteLine(d + " " + " " + p);
                if (d < p)
                {
                    Console.WriteLine("CAST");
                    CastW(castUnit);
                }
            }
        }

        #endregion
    }
}