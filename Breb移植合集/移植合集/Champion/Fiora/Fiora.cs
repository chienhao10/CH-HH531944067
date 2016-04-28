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

namespace UnderratedAIO.Champions
{
    internal class Fiora
    {
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static List<PassiveManager> passives = new List<PassiveManager>();
        public static float Qradius = 175f;
        public static IncomingDamage IncDamages = new IncomingDamage();

        private static Menu config, comboMenu, drawingsMenu, laneClear, miscSettings;

        public static void OnLoad()
        {
            InitFiora();
            InitMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Orbwalker.OnPostAttack += AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var passiveType = GetPassive(sender.Name);
            if (passiveType != PassiveType.NULL)
            {
                var enemy =
                    HeroManager.Enemies.Where(e => e.IsValidTarget() && e.Distance(sender.Position) < 50)
                        .OrderBy(e => sender.Position.Distance(e.Position))
                        .FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                var temp = new PassiveManager(enemy);
                var alreadyAdded = passives.FirstOrDefault(p => p.Enemy.NetworkId == enemy.NetworkId);
                if (alreadyAdded != null)
                {
                    alreadyAdded.passives.Add(new Passive(passiveType, Environment.TickCount));
                    //Console.WriteLine("Updated: " + sender.Name);
                }
                else
                {
                    temp.passives.Add(new Passive(passiveType, Environment.TickCount));
                    passives.Add(temp);
                    //Console.WriteLine("NewAdded: " + sender.Name);
                }
            }
        }


        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var passiveType = GetPassive(sender.Name);
            if (passiveType != PassiveType.NULL)
            {
                var enemy = HeroManager.Enemies.OrderBy(e => sender.Position.Distance(e.Position)).FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                var deleted = passives.FirstOrDefault(p => p.Enemy.NetworkId == enemy.NetworkId);
                if (deleted != null)
                {
                    for (var i = 0; i < deleted.passives.Count; i++)
                    {
                        if (deleted.passives[i].Type == passiveType)
                        {
                            deleted.passives.RemoveAt(i);
                        }
                    }
                }
                //Console.WriteLine("Deleted: " + sender.Name);
            }
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            Orbwalker.DisableMovement = false;

            ClearList();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            var data = IncDamages.GetAllyData(player.NetworkId);
            if (data != null && W.IsReady())
            {
                var enemy = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if ((getCheckBoxItem(comboMenu, "usew") &&
                     Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && enemy != null &&
                     data.DamageTaken >= enemy.GetAutoAttackDamage(player) - 5) ||
                    (getCheckBoxItem(comboMenu, "usewDangerous") && data.DamageTaken > player.Health*0.1f))
                {
                    var bestPositionW =
                        W.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                            MinionTypes.All, MinionTeam.NotAlly));
                    if (enemy != null)
                    {
                        W.Cast(enemy, getCheckBoxItem(config, "packets"));
                    }
                    else if (bestPositionW.MinionsHit > 0)
                    {
                        W.Cast(bestPositionW.Position, getCheckBoxItem(config, "packets"));
                    }
                }
            }
        }

        private static void ClearList()
        {
            foreach (var passive in passives)
            {
                for (var i = 0; i < passive.passives.Count; i++)
                {
                    if (Environment.TickCount - passive.passives[i].time > 15000)
                    {
                        passive.passives.RemoveAt(i);
                    }
                }
            }
        }

        private static void AfterAttack(AttackableUnit targetO, EventArgs args)
        {
            if (!(targetO is AIHeroClient))
            {
                return;
            }

            var targ = (AIHeroClient) targetO;
            var passivePositions = GetPassivePositions(targetO);
            var rapid = player.GetAutoAttackDamage(targ)*3 + ComboDamage(targ) > targ.Health ||
                        (player.Health < targ.Health && player.Health < player.MaxHealth/2);
            if (E.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                (getCheckBoxItem(comboMenu, "usee") || getKeyBindItem(comboMenu, "RapidAttack") || rapid) &&
                !Orbwalker.CanAutoAttack)
            {
                E.Cast(getCheckBoxItem(config, "packets"));
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Q.IsReady() &&
                (getKeyBindItem(comboMenu, "RapidAttack") || rapid) && !Orbwalker.CanAutoAttack &&
                passivePositions.Any())
            {
                var passive = GetClosestPassivePosition(targ);
                var pos = GetQpoint(targ, passive);
                if (pos.IsValid())
                {
                    Q.Cast(pos, getCheckBoxItem(config, "packets"));
                }
                else
                {
                    var pos2 = GetQpoint(targ, Prediction.GetPrediction(targ, 2).UnitPosition);
                    if (pos2.IsValid())
                    {
                        Q.Cast(pos2, getCheckBoxItem(config, "packets"));
                    }
                }
            }
            //var pos = GetClosestPassivePosition(targetO);
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && targetO.NetworkId == target.NetworkId &&
                R.IsReady() && R.CanCast(target) &&
                HealthPrediction.GetHealthPrediction(target, 1000) > player.GetAutoAttackDamage(target) &&
                ComboDamage(target) + player.GetAutoAttackDamage(target)*5 > target.Health &&
                ((getSliderItem(comboMenu, "userally") <=
                  HeroManager.Allies.Count(
                      a => a.IsValid && !a.IsDead && a.Distance(target) < 600 && a.HealthPercent < 90) &&
                  getCheckBoxItem(comboMenu, "usertf")) ||
                 (player.HealthPercent < 75 && getCheckBoxItem(comboMenu, "user"))))
            {
                R.CastOnUnit(target, getCheckBoxItem(config, "packets"));
            }
        }

        private static bool CheckQusage(Vector3 pos, AIHeroClient target)
        {
            return pos.IsValid() && pos.Distance(player.Position) < Q.Range &&
                   (target.HasBuff("fiorapassivemanager") || target.HasBuff("fiorarmark")) && !pos.IsWall() &&
                   Qradius > pos.Distance(target.Position);
        }

        private static List<Vector3> GetPassivePositions(AttackableUnit target)
        {
            var temp = new List<Vector3>();
            var query = passives.FirstOrDefault(t => t.Enemy.NetworkId == target.NetworkId);
            if (query != null)
            {
                temp = query.getPositions();
            }
            return temp;
        }

        private static Vector3 GetClosestPassivePosition(AttackableUnit target)
        {
            var temp = GetPassivePositions(target);
            return temp.OrderBy(p => p.Distance(player.Position)).FirstOrDefault();
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (target == null)
            {
                return;
            }

            var data = IncDamages.GetAllyData(player.NetworkId);
            if (getCheckBoxItem(comboMenu, "usewCC") && W.IsReady() && data.AnyCC)
            {
                Console.WriteLine("asdafwfq");
                W.Cast(target.Position, getCheckBoxItem(config, "packets"));
            }

            var closestPassive = GetClosestPassivePosition(target);
            if (closestPassive.IsValid() && getCheckBoxItem(comboMenu, "MoveToVitals") && !Orbwalker.CanAutoAttack &&
                !Orbwalker.IsAutoAttacking && Game.CursorPos.Distance(target.Position) < 350)
            {
                //orbwalker.SetMovement(false);
                Orbwalker.DisableMovement = true;
                Player.IssueOrder(GameObjectOrder.MoveTo,
                    target.Position.LSExtend(closestPassive,
                        Math.Max(player.BoundingRadius + target.BoundingRadius, 100)));
            }

            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(comboMenu, "useq") && Q.IsReady() &&
                getSliderItem(comboMenu, "useqMin") <= player.Distance(target) &&
                (closestPassive.IsValid() || (target.HealthPercent < 30)) && !Orbwalker.IsAutoAttacking)
            {
                var pos = GetQpoint(target, closestPassive);
                if (pos.IsValid())
                {
                    Q.Cast(pos, getCheckBoxItem(config, "packets"));
                }
                else if (target.HealthPercent < 30)
                {
                    if (
                        CheckQusage(
                            target.Position.LSExtend(
                                Prediction.GetPrediction(target, player.Distance(target)/1600).UnitPosition, Qradius),
                            target))
                    {
                        Q.Cast(
                            target.Position.Extend(
                                Prediction.GetPrediction(target, player.Distance(target)/1600).UnitPosition, Qradius),
                            getCheckBoxItem(config, "packets"));
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "usew") && W.IsReady() && target.Distance(player) > 350f &&
                W.GetDamage(target) > target.Health)
            {
                W.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
            }

            if (getCheckBoxItem(comboMenu, "useIgnite") && hasIgnite && ComboDamage(target) > target.Health &&
                !Q.IsReady() &&
                (target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target) || player.HealthPercent < 15))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }


        public static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (args == null || hero == null)
            {
                return;
            }
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (targetW != null)
            {
                hero = targetW;
            }
            if (hero.IsMe)
            {
                if (args.SData.Name == "FioraE")
                {
                    Orbwalker.ResetAutoAttack();
                }
            }
        }

        private static void Clear()
        {
            var perc = getSliderItem(laneClear, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }

            var bestPositionW =
                W.GetLineFarmLocation(MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                    MinionTypes.All, MinionTeam.NotAlly));
            if (getCheckBoxItem(laneClear, "usewLC") && bestPositionW.MinionsHit >= getSliderItem(laneClear, "wMinHit"))
            {
                W.Cast(bestPositionW.Position, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(laneClear, "useeLC") &&
                Helpers.Environment.Minion.countMinionsInrange(player.Position, Q.Range) >= 2)
            {
                E.Cast(getCheckBoxItem(config, "packets"));
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingsMenu, "drawqq"))
            {
                Render.Circle.DrawCircle(player.Position, Q.Range, Color.FromArgb(180, 58, 100, 150));
            }

            if (getCheckBoxItem(drawingsMenu, "drawww"))
            {
                Render.Circle.DrawCircle(player.Position, W.Range, Color.FromArgb(180, 58, 100, 150));
            }

            if (getCheckBoxItem(drawingsMenu, "drawrr"))
            {
                Render.Circle.DrawCircle(player.Position, R.Range, Color.FromArgb(180, 58, 100, 150));
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.W);
            }
            if (R.IsReady())
            {
                damage += GetPassiveDamage(hero, 4);
            }
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private static void InitFiora()
        {
            Q = new Spell(SpellSlot.Q, 400f);
            Q.SetSkillshot(0.25f, 50f, 1600f, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 750f);
            W.SetSkillshot(0.75f, 80, 2000f, false, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 500);
        }

        private static PassiveType GetPassive(string name)
        {
            switch (name)
            {
                case "Fiora_Base_Passive_SW.troy":
                    return PassiveType.SW;

                case "Fiora_Base_Passive_SE.troy":
                    return PassiveType.SE;

                case "Fiora_Base_Passive_NW.troy":
                    return PassiveType.NW;

                case "Fiora_Base_Passive_NE.troy":
                    return PassiveType.NE;

                case "Fiora_Base_R_Mark_SW_FioraOnly.troy":
                    return PassiveType.SW;

                case "Fiora_Base_R_Mark_SE_FioraOnly.troy":
                    return PassiveType.SE;

                case "Fiora_Base_R_Mark_NW_FioraOnly.troy":
                    return PassiveType.NW;

                case "Fiora_Base_R_Mark_NE_FioraOnly.troy":
                    return PassiveType.NE;
            }
            return PassiveType.NULL;
        }

        public static Vector3 GetQpoint(AIHeroClient target, Vector3 passive)
        {
            var ponts = new List<Vector3>();
            var predEnemy = Prediction.GetPrediction(target, ObjectManager.Player.Distance(target)/1600).UnitPosition;
            for (var i = 2; i < 8; i++)
            {
                ponts.Add(predEnemy.To2D().Extend(passive.To2D(), i*25).To3D());
            }

            return
                ponts.Where(p => CheckQusage(p, target))
                    .OrderByDescending(p => p.Distance(target.Position))
                    .FirstOrDefault();
        }

        public static double GetPassiveDamage(Obj_AI_Base target, int passives)
        {
            return passives*(0.03f + 0.027 + 0.001f*player.Level*player.FlatPhysicalDamageMod/100f)*
                   target.MaxHealth;
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

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Fiora", "Fiora");

            // Draw settings
            drawingsMenu = config.AddSubMenu("Drawings ", "dsettings");
            drawingsMenu.Add("drawqq", new CheckBox("Draw Q range"));
            drawingsMenu.Add("drawww", new CheckBox("Draw W range"));
            drawingsMenu.Add("drawrr", new CheckBox("Draw R range"));

            // Combo Settings
            comboMenu = config.AddSubMenu("Combo ", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("useqMin", new Slider("Min distance", 250, 0, 400));
            comboMenu.Add("usew", new CheckBox("Use W AA"));
            comboMenu.Add("usewDangerous", new CheckBox("Only on low health"));
            comboMenu.Add("usewCC", new CheckBox("W to CC"));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("user", new CheckBox("R 1v1"));
            comboMenu.Add("usertf", new CheckBox("R teamfight"));
            comboMenu.Add("userally", new Slider("Min allies", 2, 1, 5));
            comboMenu.Add("RapidAttack", new KeyBind("Fast AA Combo", false, KeyBind.BindTypes.PressToggle, 'T'));
            comboMenu.Add("MoveToVitals", new CheckBox("Move to vitals"));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            // LaneClear Settings
            laneClear = config.AddSubMenu("LaneClear ", "Lcsettings");
            laneClear.Add("usewLC", new CheckBox("Use W"));
            laneClear.Add("wMinHit", new Slider("Min hit", 3, 1, 6));
            laneClear.Add("useeLC", new CheckBox("Use E"));
            laneClear.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Settings
            miscSettings = config.AddSubMenu("Misc ", "Msettings");
            miscSettings.Add("autoW", new CheckBox("Auto W AA"));
            miscSettings.Add("minmanaP", new Slider("Min mana percent", 1, 1));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }

    internal class PassiveManager
    {
        public AIHeroClient Enemy;
        public List<Passive> passives = new List<Passive>();

        public PassiveManager(AIHeroClient enemy)
        {
            Enemy = enemy;
        }

        public List<Vector3> getPositions()
        {
            var list = new List<Vector3>();
            var predEnemy = Prediction.GetPrediction(Enemy, ObjectManager.Player.Distance(Enemy)/1600).UnitPosition;
            foreach (var passive in passives)
            {
                switch (passive.Type)
                {
                    case PassiveType.NE:
                        list.Add(new Vector2(predEnemy.X, predEnemy.Y + 100).To3D());
                        break;
                    case PassiveType.NW:
                        list.Add(new Vector2(predEnemy.X + 100, predEnemy.Y).To3D());
                        break;
                    case PassiveType.SW:
                        list.Add(new Vector2(predEnemy.X, predEnemy.Y - 100).To3D());
                        break;
                    case PassiveType.SE:
                        list.Add(new Vector2(predEnemy.X - 100, predEnemy.Y).To3D());
                        break;
                }
            }
            return list;
        }
    }

    public enum PassiveType
    {
        SW,
        SE,
        NW,
        NE,
        NULL
    }

    internal class Passive
    {
        public float time;
        public PassiveType Type;

        public Passive(PassiveType getPassive, int tickCount)
        {
            Type = getPassive;
            time = tickCount;
        }
    }
}