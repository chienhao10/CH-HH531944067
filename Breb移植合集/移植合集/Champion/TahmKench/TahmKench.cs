using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Damage = LeagueSharp.Common.Damage;
using Environment = UnderratedAIO.Helpers.Environment;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class TahmKench
    {
        public static Menu config;
        public static Spell Q, W, WSkillShot, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justWOut, justQ, blockW;
        public static IncomingDamage IncDamages;

        public static Menu menuD, menuC, menuH, menuLC, menuM, AllyDef, Shield;
        public Team lastWtarget = Team.Null;

        public TahmKench()
        {
            IncDamages = new IncomingDamage();
            InitTahmKench();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }


        private static bool SomebodyInYou
        {
            get { return player.HasBuff("tahmkenchwhasdevouredtarget"); }
        }

        private bool MinionInYou
        {
            get { return SomebodyInYou && lastWtarget == Team.Minion; }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(menuM, "useqgc") && Q.IsReady() &&
                gapcloser.End.LSDistance(player.Position) < 200 && !gapcloser.Sender.ChampionName.ToLower().Contains("yi"))
            {
                Q.Cast(gapcloser.End);
            }
        }

        private void InitTahmKench()
        {
            Q = new Spell(SpellSlot.Q, 800);

            Q.SetSkillshot(0.5f, 80, 2000, true, SkillshotType.SkillshotLine);

            WSkillShot = new Spell(SpellSlot.W, 900);
            WSkillShot.SetSkillshot(0.5f, 80, 900, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 235);
            W.SetTargetted(0.5f, float.MaxValue);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 1700);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            Orbwalker.DisableMovement = false;

            blockW = false;

            if (getCheckBoxItem(AllyDef, "useDevour") && W.IsReady() && !justQ)
            {
                EatAlly();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
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

            if (getCheckBoxItem(Shield, "useShield") && E.IsReady())
            {
                UseShield();
            }
        }

        private void UseShield()
        {
            var playerData = IncDamages.GetAllyData(player.NetworkId);
            if (playerData == null)
            {
                return;
            }

            if (getSliderItem(Shield, "ShieldUnderHealthP") > player.HealthPercent && playerData.DamageTaken > 50 &&
                player.AttackShield >= playerData.DamageTaken ||
                playerData.DamageTaken > player.Health)
            {
                E.Cast();
            }

            if (playerData.DamageTaken >
                player.Health*getSliderItem(Shield, "ShieldDamage")/100)
            {
                E.Cast();
            }
        }

        private void EatAlly()
        {
            var allies =
                HeroManager.Allies.Where(a => a.LSDistance(player) < W.Range && !a.IsMe)
                    .OrderByDescending(a => getSliderItem(AllyDef, "Priority" + a.NetworkId))
                    .ToArray();
            if (allies.Any())
            {
                for (var i = 0; i <= allies.Count() - 1; i++)
                {
                    var allyData = IncDamages.GetAllyData(allies[i].NetworkId);
                    if (allyData == null || !getCheckBoxItem(AllyDef, "useEat" + allies[i].NetworkId))
                    {
                        continue;
                    }
                    if (CheckCasting(allies[i]) && !allies[i].IsInvulnerable)
                    {
                        if (((getSliderItem(AllyDef, "EatUnderHealthP" + allies[i].NetworkId) >
                              allies[i].HealthPercent && allyData.ProjectileDamageTaken > 50) ||
                             allyData.ProjectileDamageTaken > allies[i].Health) && !allies[i].Spellbook.IsCastingSpell)
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }

                        if (allyData.ProjectileDamageTaken >
                            allies[i].Health*
                            getSliderItem(AllyDef, "EatDamage" + allies[i].NetworkId) /100)
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }
                        if (getCheckBoxItem(AllyDef, "targetedCC" + allies[i].NetworkId) && allyData.AnyCC)
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }
                        if (getCheckBoxItem(AllyDef, "ontargetedCC" + allies[i].NetworkId) &&
                            allyData.ProjectileDamageTaken > 50 &&
                            (allies[i].HasBuffOfType(BuffType.Knockup) || allies[i].HasBuffOfType(BuffType.Fear) ||
                             allies[i].HasBuffOfType(BuffType.Flee) || allies[i].HasBuffOfType(BuffType.Stun) ||
                             allies[i].HasBuffOfType(BuffType.Snare)))
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }
                    }
                    if (i + 1 <= allies.Count() - 1 &&
                        getSliderItem(AllyDef, "Priority" + allies[i + 1].NetworkId) <
                        getSliderItem(AllyDef, "Priority" + allies[i].NetworkId))
                    {
                        if (getCheckBoxItem(AllyDef, "allyPrior"))
                        {
                            blockW = true;
                        }
                        return;
                    }
                }
            }
        }

        private bool CheckCasting(AIHeroClient hero)
        {
            if (!getCheckBoxItem(AllyDef, "dontuseDevourcasting"))
            {
                return true;
            }
            if (hero.Spellbook.IsCastingSpell || hero.Spellbook.IsChanneling || hero.Spellbook.IsCharging)
            {
                return false;
            }
            return true;
        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            var perc = getSliderItem(menuH, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc || target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuH, "useqH") && Q.CanCast(target) && !justWOut)
            {
                handeQ(target, HitChance.VeryHigh);
            } //usewminiH
            if (getCheckBoxItem(menuH, "usewminiH"))
            {
                HandleWHarass(target);
            }
            if (getCheckBoxItem(menuH, "usewH"))
            {
                handleWEnemyHero(target);
            }
        }

        private void handleWEnemyHero(AIHeroClient target)
        {
            if (target.GetBuffCount("TahmKenchPDebuffCounter") == 3 && !CombatHelper.CheckCriticalBuffs(target) &&
                !target.HasBuffOfType(BuffType.Stun) && !target.HasBuffOfType(BuffType.Snare) && !Q.CanCast(target) &&
                !justQ && !IncDamages.GetEnemyData(target.NetworkId).IncSkillShot)
            {
                Orbwalker.DisableMovement = true;
                if (Game.CursorPos.LSDistance(target.Position) < 300)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, target.Position.LSExtend(player.Position, 100));
                }

                lastWtarget = Team.Enemy;
                W.CastOnUnit(target);
            }
        }

        private void HandleWHarass(AIHeroClient target)
        {
            if (W.IsReady() && MinionInYou && WSkillShot.CanCast(target))
            {
                WSkillShot.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
            }
            if (W.IsReady() && !SomebodyInYou && WSkillShot.CanCast(target) &&
                player.LSDistance(target) > getSliderItem(menuC, "usewminiRange"))
            {
                var mini =
                    MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .OrderBy(e => e.Health)
                        .FirstOrDefault();
                if (mini != null)
                {
                    lastWtarget = Team.Minion;
                    W.CastOnUnit(mini, true);
                }
            }
        }

        private void Clear()
        {
            var perc = getSliderItem(menuLC, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var bestPosition =
                WSkillShot.GetLineFarmLocation(
                    MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition, WSkillShot.Range, MinionTypes.All, MinionTeam.NotAlly));

            if (W.IsReady() && !SomebodyInYou && getCheckBoxItem(menuLC, "usewLC") &&
                bestPosition.MinionsHit >= getSliderItem(menuLC, "wMinHit") && !justQ)
            {
                var mini =
                    MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .OrderBy(e => e.Health)
                        .FirstOrDefault();
                if (mini != null)
                {
                    lastWtarget = Team.Minion;
                    W.CastOnUnit(mini, true);
                }
            }
            if (W.IsReady() && getCheckBoxItem(menuLC, "usewLC") && MinionInYou)
            {
                WSkillShot.Cast(bestPosition.Position, getCheckBoxItem(config, "packets"));
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.CanCast(target) && getCheckBoxItem(menuC, "useq") && !justWOut)
            {
                handeQ(target, HitChance.High);
            }
            if (W.IsReady() && !SomebodyInYou && getCheckBoxItem(menuC, "usew") && !blockW)
            {
                if (!getCheckBoxItem(menuC, "dontusewks") ||
                    (getWDamage(target) < HealthPrediction.GetHealthPrediction(target, 600) &&
                     player.CountAlliesInRange(1200) > 0) || player.CountAlliesInRange(1200) == 0)
                {
                    handleWEnemyHero(target);
                }
            }
            if (getCheckBoxItem(menuC, "usewmini"))
            {
                HandleWHarass(target);
            }
        }

        private double getWDamage(AIHeroClient target)
        {
            var r = R.Level - 1;
            var Rpercent = new[] {0.04f, 0.06f, 0.08f}[r >= 1 ? r : 1];
            var bonusDmg = 20 + (player.MaxHealth - (515f + 95f*(player.Level - 1f)))*Rpercent;
            var dmg = (new[] {0.20, 0.23, 0.26, 0.29, 0.32}[W.Level - 1] +
                       0.02*player.TotalMagicalDamage/100)*target.MaxHealth;
            return player.CalcDamage(target, DamageType.Magical, dmg + bonusDmg);
        }

        private void handeQ(AIHeroClient target, HitChance hitChance)
        {
            if (player.LSDistance(target) <= Orbwalking.GetRealAutoAttackRange(target) && !Orbwalker.CanAutoAttack &&
                target.GetBuffCount("TahmKenchPDebuffCounter") != 2)
            {
                Q.CastIfHitchanceEquals(target, hitChance, getCheckBoxItem(config, "packets"));
            }
            else if (player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(target))
            {
                Q.CastIfHitchanceEquals(target, hitChance, getCheckBoxItem(config, "packets"));
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawww"), W.Range, Color.FromArgb(180, 100, 146, 166));
            var r = R.Level - 1;
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawrr"), new[] {4000, 5000, 6000}[r >= 1 ? r : 1],
                Color.FromArgb(180, 100, 146, 166));
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //tahmkenchw
            //tahmkenchwcasttimeandanimation
            if (sender.IsMe)
            {
                if (args.SData.Name == "tahmkenchwcasttimeandanimation")
                {
                    justWOut = true;
                    Utility.DelayAction.Add(500, () => { justWOut = false; });
                }
                if (args.SData.Name == "TahmKenchQ")
                {
                    justQ = true;
                    Utility.DelayAction.Add(500, () => justQ = false);
                }
                if (args.Slot == SpellSlot.W)
                {
                    if (args.Target != null)
                    {
                        Console.WriteLine("----");
                        Console.WriteLine("Tahm ate :" + args.Target.Name);
                        Console.WriteLine("----");
                    }
                    else
                    {
                        Console.WriteLine("----");
                        Console.WriteLine("Tahm ate : null");
                        Console.WriteLine("----");
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
            config = MainMenu.AddMenu("TahmKench ", "TahmKench");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawww", new CheckBox("Draw W range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawrr", new CheckBox("Draw R range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));

            // Combo Settings 
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usew", new CheckBox("Use W on target"));
            menuC.Add("dontusewks", new CheckBox("   Block KS"));
            menuC.Add("usewmini", new CheckBox("Use W on minion"));
            menuC.Add("usewminiRange", new Slider("   Min range", 300, 0, (int) WSkillShot.Range));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q"));
            menuH.Add("usewH", new CheckBox("Use W on target"));
            menuH.Add("usewminiH", new CheckBox("Use W on minion"));
            menuH.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));
            menuLC.Add("usewLC", new CheckBox("Use w"));
            menuLC.Add("wMinHit", new Slider("   Min hit", 3, 1, 6));
            menuLC.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Menu
            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("useqgc", new CheckBox("Use Q gapclosers", false));

            // Shield Menu
            Shield = config.AddSubMenu("Shield(E)", "Shieldsettings");
            Shield.Add("ShieldUnderHealthP", new Slider("Shield Under X% health", 20));
            Shield.Add("ShieldDamage", new Slider("Damage in %health", 40));
            Shield.Add("useShield", new CheckBox("Enabled"));

            // Devour Menu
            AllyDef = config.AddSubMenu("Devour(W) on ally", "Devoursettings");
            foreach (var ally in HeroManager.Allies.Where(a => !a.IsMe))
            {
                AllyDef.AddGroupLabel(ally.ChampionName + "settings");
                AllyDef.Add("EatUnderHealthP" + ally.NetworkId, new Slider("Eat X% health", 20));
                AllyDef.Add("EatDamage" + ally.NetworkId, new Slider("Eat at Damage in %health", 40));
                AllyDef.Add("targetedCC" + ally.NetworkId, new CheckBox("Eat before Targeted CC"));
                AllyDef.Add("ontargetedCC" + ally.NetworkId, new CheckBox("Eat on CC"));
                AllyDef.Add("Priority" + ally.NetworkId, new Slider("Priority", Environment.Hero.GetPriority(ally.ChampionName), 1, 5));
                AllyDef.Add("useEat" + ally.NetworkId, new CheckBox("Enabled"));
                AllyDef.AddSeparator();
            }
            AllyDef.Add("dontuseDevourcasting", new CheckBox("Don't interrupt ally"));
            AllyDef.Add("allyPrior", new CheckBox("Prioritize ally over damage enemy"));
            AllyDef.Add("useDevour", new CheckBox("Enabled"));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }

    public enum Team
    {
        Null,
        Ally,
        Enemy,
        Minion
    }
}