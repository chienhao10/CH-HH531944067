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
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Shen
    {
        private const int XOffset = 36;
        private const int YOffset = 9;
        private const int Width = 103;
        private const int Height = 8;
        public static Menu config;
        private static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, EFlash, R;
        private static readonly float bladeRadius = 325f;
        public static bool PingCasted;
        public static Vector3 blade, bladeOnCast;
        public static bool justW;
        public static IncomingDamage IncDamages;

        private static readonly Render.Text Text = new Render.Text(
            0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");

        public static Menu menuD, menuC, menuH, menuLC, menuU;

        public Shen()
        {
            IncDamages = new IncomingDamage();
            InitShen();
            InitMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
            AttackableUnit.OnDamage += Obj_AI_Base_OnDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
        }

        private void Obj_AI_Base_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            var t = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Source.NetworkId);
            var s = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Target.NetworkId);
            if (t != null && s != null && t.IsMe && ObjectManager.Get<Obj_AI_Turret>()
                .FirstOrDefault(tw => tw.LSDistance(t) < 750 && tw.LSDistance(s) < 750 && tw.IsAlly) != null)
            {
                if (getCheckBoxItem(menuU, "autotauntattower") && E.CanCast(s))
                {
                    E.Cast(s, getCheckBoxItem(config, "packets"));
                }
            }
        }


        private void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(menuU, "useeint"))
            {
                return;
            }
            if (unit.LSIsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(unit, getCheckBoxItem(config, "packets"));
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(150, 150, 62, 172));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(150, 150, 62, 172));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "draweeflash"), EFlash.Range, Color.FromArgb(50, 250, 248, 110));

            if (getCheckBoxItem(menuD, "drawallyhp"))
            {
                DrawHealths();
            }
            if (getCheckBoxItem(menuD, "drawincdmg"))
            {
                getIncDmg();
            }
            if (true)
            {
                Render.Circle.DrawCircle(blade, bladeRadius, Color.BlueViolet, 7);
            }
        }

        private static void DrawHealths()
        {
            float i = 0;
            foreach (
                var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead))
            {
                var playername = hero.Name;
                if (playername.Length > 13)
                {
                    playername = playername.Remove(9) + "...";
                }
                var champion = hero.BaseSkinName;
                if (champion.Length > 12)
                {
                    champion = champion.Remove(7) + "...";
                }
                var percent = (int) (hero.Health/hero.MaxHealth*100);
                var color = Color.Red;
                if (percent > 25)
                {
                    color = Color.Orange;
                }
                if (percent > 50)
                {
                    color = Color.Yellow;
                }
                if (percent > 75)
                {
                    color = Color.LimeGreen;
                }
                Drawing.DrawText(
                    Drawing.Width*0.8f, Drawing.Height*0.15f + i, color, playername + "(" + champion + ")");
                Drawing.DrawText(
                    Drawing.Width*0.9f, Drawing.Height*0.15f + i, color,
                    (int) hero.Health + " (" + percent + "%)");
                i += 20f;
            }
        }

        private static void getIncDmg()
        {
            var color = Color.Red;
            var result = CombatHelper.getIncDmg();
            var barPos = player.HPBarPosition;
            var damage = result;
            if (damage == 0)
            {
                return;
            }
            var percentHealthAfterDamage = Math.Max(0, player.Health - damage)/player.MaxHealth;
            var xPos = barPos.X + XOffset + Width*percentHealthAfterDamage;

            if (damage > player.Health)
            {
                Text.X = (int) barPos.X + XOffset;
                Text.Y = (int) barPos.Y + YOffset - 13;
                Text.text = ((int) (player.Health - damage)).ToString();
                Text.OnEndScene();
            }

            Drawing.DrawLine(xPos, barPos.Y + YOffset, xPos, barPos.Y + YOffset + Height, 3, color);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Ulti();
            if (getKeyBindItem(menuC, "useeflash") &&
                player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready)
            {
                FlashCombo();
            }

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
                Clear();
            }

            var bladeObj =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        o => (o.Name == "ShenSpiritUnit" || o.Name == "ShenArrowVfxHostMinion") && o.Team == player.Team)
                    .OrderBy(o => o.LSDistance(bladeOnCast))
                    .FirstOrDefault();
            if (bladeObj != null)
            {
                blade = bladeObj.Position;
            }
            if (W.IsReady() && blade.IsValid())
            {
                foreach (var ally in HeroManager.Allies.Where(a => a.LSDistance(blade) < bladeRadius))
                {
                    var data = IncDamages.GetAllyData(ally.NetworkId);
                    if (getSliderItem(menuU, "autowAgg") <= data.AADamageCount)
                    {
                        W.Cast();
                    }
                    if (data.AADamageTaken >= ally.Health*0.2f && getCheckBoxItem(menuU, "autow"))
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!getCheckBoxItem(menuU, "useeagc"))
            {
                return;
            }
            if (gapcloser.Sender.LSIsValidTarget(E.Range) && E.IsReady() &&
                player.LSDistance(gapcloser.Sender.Position) < 400)
            {
                E.Cast(gapcloser.End, getCheckBoxItem(config, "packets"));
            }
        }

        private static void Clear()
        {
            var minionsHP = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.LSIsValidTarget(400)).Sum(m => m.Health);

            if (getCheckBoxItem(menuLC, "useqLC") && minionsHP > 300 && CheckQDef())
            {
                Q.Cast();
            }
        }

        private static void Ulti()
        {
            if (!R.IsReady() || player.IsDead)
            {
                return;
            }

            foreach (var allyObj in
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        i =>
                            i.IsAlly && !i.IsMe && !i.IsDead &&
                            (((IncDamages.GetAllyData(i.NetworkId).DamageTaken > i.Health ||
                               (i.Health - IncDamages.GetAllyData(i.NetworkId).DamageTaken)*100f/
                               i.MaxHealth <= getSliderItem(menuU, "atpercent")) &&
                              i.LSCountEnemiesInRange(700) > 0) ||
                             IncDamages.GetAllyData(i.NetworkId).SkillShotDamage > i.Health)))
            {
                if (getCheckBoxItem(menuU, "user") &&
                    !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && R.IsReady() &&
                    player.LSCountEnemiesInRange(EFlash.Range + 50) < 1 &&
                    !getCheckBoxItem(menuU, "ult" + allyObj.BaseSkinName))
                {
                    R.Cast(allyObj);
                    return;
                }
                if (!PingCasted)
                {
                    //ping
                    PingCasted = true;
                    Utility.DelayAction.Add(5000, () => PingCasted = false);
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target != null && Q.IsReady() && getCheckBoxItem(menuH, "harassq"))
            {
                HandleQ(target);
            }
        }

        private static void Combo()
        {
            var minHit = getSliderItem(menuC, "useemin");
            var target = TargetSelector.GetTarget(E.Range + 400, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            var useE = getCheckBoxItem(menuC, "usee") && E.IsReady() &&
                       player.LSDistance(target.Position) < E.Range;
            if (useE)
            {
                if (minHit > 1)
                {
                    CastEmin(target, minHit);
                }
                else if ((player.LSDistance(target.Position) > Orbwalking.GetRealAutoAttackRange(target) ||
                          player.HealthPercent < 45 || player.LSCountEnemiesInRange(1000) == 1) &&
                         E.GetPrediction(target).Hitchance >= HitChance.High)
                {
                    CastETarget(target);
                }
            }
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (getCheckBoxItem(menuC, "useIgnite") && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target) && !Q.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.IsReady() && getCheckBoxItem(menuC, "useq"))
            {
                HandleQ(target);
            }
            if (getCheckBoxItem(menuC, "usew"))
            {
                foreach (var ally in HeroManager.Allies.Where(a => a.LSDistance(blade) < bladeRadius))
                {
                    var data = IncDamages.GetAllyData(ally.NetworkId);
                    if (data.AADamageTaken >= target.GetAutoAttackDamage(ally) - 10)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void CastETarget(AIHeroClient target)
        {
            var pred = E.GetPrediction(target);
            var poly = CombatHelper.GetPoly(pred.UnitPosition, E.Range, E.Width);
            var enemiesBehind =
                HeroManager.Enemies.Count(
                    e =>
                        e.NetworkId != target.NetworkId && e.LSIsValidTarget(E.Range) &&
                        (poly.IsInside(E.GetPrediction(e).UnitPosition) || poly.IsInside(e.Position)) &&
                        e.Position.LSDistance(player.Position) > player.LSDistance(pred.UnitPosition));
            if (pred.Hitchance >= HitChance.High)
            {
                if (enemiesBehind > 0)
                {
                    E.Cast(
                        player.ServerPosition.LSExtend(pred.CastPosition, E.Range),
                        getCheckBoxItem(config, "packets"));
                }
                else
                {
                    if (poly.IsInside(pred.UnitPosition) && poly.IsInside(target.Position))
                    {
                        E.Cast(
                            player.ServerPosition.LSExtend(
                                pred.CastPosition,
                                player.LSDistance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)),
                            getCheckBoxItem(config, "packets"));
                    }
                    else
                    {
                        E.Cast(
                            player.ServerPosition.LSExtend(
                                pred.CastPosition,
                                player.LSDistance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)),
                            getCheckBoxItem(config, "packets"));
                    }
                }
            }
        }

        private static void HandleQ(AIHeroClient target)
        {
            Q.UpdateSourcePosition(blade);
            var pred = Q.GetPrediction(target);
            var poly = CombatHelper.GetPoly(blade.LSExtend(player.Position, 30), player.LSDistance(blade), 150);
            if (((pred.Hitchance >= HitChance.VeryHigh && poly.IsInside(pred.UnitPosition)) ||
                 (target.LSDistance(blade) < 100) || (target.LSDistance(blade) < 500 && poly.IsInside(target.Position)) ||
                 player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target) || Orbwalker.IsAutoAttacking) &&
                CheckQDef())
            {
                Q.Cast();
            }
        }

        private static bool CheckQDef()
        {
            if (blade.CountAlliesInRange(bladeRadius) == 0 || !justW)
            {
                return true;
            }
            return false;
        }

        public static void CastEmin(AIHeroClient target, int min)
        {
            var MaxEnemy = player.LSCountEnemiesInRange(1580);
            if (MaxEnemy == 1)
            {
                CastETarget(target);
            }
            else
            {
                var MinEnemy = Math.Min(min, MaxEnemy);
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(i => i.LSDistance(player) < E.Range && i.IsEnemy && !i.IsDead && i.LSIsValidTarget()))
                {
                    for (var i = MaxEnemy; i > MinEnemy - 1; i--)
                    {
                        if (E.CastIfWillHit(enemy, i))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private static void FlashCombo()
        {
            var target = TargetSelector.GetTarget(EFlash.Range, DamageType.Magical);
            if (target != null && E.IsReady() && E.ManaCost < player.Mana &&
                player.LSDistance(target.Position) < EFlash.Range && player.LSDistance(target.Position) > 480 &&
                !getPosToEflash(target.Position).IsWall())
            {
                var pred = EFlash.GetPrediction(target);
                var poly = CombatHelper.GetPolyFromVector(getPosToEflash(target.Position), pred.UnitPosition, E.Width);
                var enemiesBehind =
                    HeroManager.Enemies.Count(
                        e =>
                            e.NetworkId != target.NetworkId && e.LSIsValidTarget(E.Range) &&
                            (poly.IsInside(E.GetPrediction(e).UnitPosition) || poly.IsInside(e.Position)) &&
                            e.Position.LSDistance(player.Position) > player.LSDistance(pred.UnitPosition));
                if (pred.Hitchance >= HitChance.High)
                {
                    Utility.DelayAction.Add(
                        30, () =>
                        {
                            if (enemiesBehind > 0)
                            {
                                E.Cast(
                                    player.ServerPosition.LSExtend(pred.CastPosition, E.Range),
                                    getCheckBoxItem(config, "packets"));
                            }
                            else
                            {
                                E.Cast(
                                    player.ServerPosition.LSExtend(
                                        pred.CastPosition,
                                        player.LSDistance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)),
                                    getCheckBoxItem(config, "packets"));
                            }
                        });
                    player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"), getPosToEflash(target.Position));
                }
            }
            Orbwalker.OrbwalkTo(Game.CursorPos);
        }


        public static Vector3 getPosToEflash(Vector3 target)
        {
            return target + (player.Position - target)/2;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.R)
                {
                    bladeOnCast = args.End;
                }
                if (args.SData.Name == "ShenW")
                {
                    justW = true;
                    Utility.DelayAction.Add(1750, () => { justW = false; });
                }
                if (args.SData.Name == "ShenE" && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (Q.IsReady() && CheckQDef() && blade.LSDistance(args.End) > bladeRadius/2f)
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void InitShen()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W); //2500f
            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(0.25f, 95f, 1250f, false, SkillshotType.SkillshotLine);
            EFlash = new Spell(SpellSlot.E, 990);
            EFlash.SetSkillshot(0.25f, 95f, 2500f, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R);
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

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Shen", "SRS_Shen");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(150, 150, 62, 172)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(150, 150, 62, 172)));
            menuD.Add("draweeflash", new CheckBox("Draw E+flash range"));
                //.SetValue(new Circle(true, Color.FromArgb(50, 250, 248, 110)));
            menuD.Add("drawallyhp", new CheckBox("Draw teammates' HP"));
            menuD.Add("drawincdmg", new CheckBox("Draw incoming damage"));

            // Combo Settings
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usew", new CheckBox("Block AA from target"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("useeflash", new KeyBind("Flash+E", false, KeyBind.BindTypes.HoldActive, 'T'));
            menuC.Add("useemin", new Slider("   Min target in teamfight", 1, 1, 5));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "hsettings");
            menuH.Add("harassq", new CheckBox("Harass with Q"));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));

            // Misc Settings
            menuU = config.AddSubMenu("Misc ", "usettings");
            menuU.Add("autow", new CheckBox("Auto block high dmg AA"));
            menuU.Add("autowAgg", new Slider("W on aggro", 4, 1, 10));
            menuU.Add("autotauntattower", new CheckBox("Auto taunt in tower range"));
            menuU.Add("useeagc", new CheckBox("Use E to anti gap closer", false));
            menuU.Add("useeint", new CheckBox("Use E to interrupt"));
            menuU.Add("user", new CheckBox("Use R"));
            menuU.Add("atpercent", new Slider("   Under % health", 20));
            menuU.AddSeparator();
            menuU.AddGroupLabel("Don't Ult : ");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly))
            {
                if (hero.BaseSkinName != player.BaseSkinName)
                {
                    menuU.Add("ult" + hero.BaseSkinName, new CheckBox(hero.BaseSkinName, false));
                }
            }

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}