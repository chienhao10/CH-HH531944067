using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LCS_Janna.Plugins;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;
//using LCS_Janna.Plugins;

namespace LCS_Janna
{
    internal class Program
    {
        public static Menu Config, comboMenu, qsettings, esettings, rsettings;
        public static AIHeroClient Udyr = ObjectManager.Player;
        public static Spell Q, W, E, R;

        public static string[] HitchanceNameArray = {"Low", "Medium", "High", "Very High", "Only Immobile"};

        public static HitChance[] HitchanceArray =
        {
            HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh,
            HitChance.Immobile
        };

        public static string[] HighChamps =
        {
            "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
            "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
            "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
            "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
            "Zed", "Ziggs", "Kindred", "Jhin"
        };

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[qsettings[menuName].Cast<ComboBox>().CurrentValue];
        }

        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Janna")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 120f, 900f, false, SkillshotType.SkillshotLine);

            SpellDatabase.InitalizeSpellDatabase();

            Config = MainMenu.AddMenu("LCS 系列:风女", "LCS Series: Janna");

            comboMenu = Config.AddSubMenu(":: 连招设置", ":: Combo Settings");
            comboMenu.Add("q.combo", new CheckBox("使用 (Q)"));
            comboMenu.Add("w.combo", new CheckBox("使用 (W)"));

            qsettings = Config.AddSubMenu(":: Q 设置", ":: Q Settings");
            qsettings.Add("q.settings", new ComboBox("(Q) 模式 :", 0, "正常", "Q 命中 x 目标"));
            qsettings.Add("q.normal.hit.chance",
                new ComboBox("(Q) 命中率 (正常)", 2, "低", "中", "高", "非常规", "只不可移动的"));
            qsettings.Add("q.hit.count", new Slider("(Q) 命中敌人数量", 2, 1, 5));
            qsettings.Add("q.antigapcloser", new CheckBox("(Q) 防突进"));

            esettings = Config.AddSubMenu(":: E 设置", ":: E Settings");
            esettings.AddGroupLabel(":: 可吸收技能");
            foreach (
                var spell in
                    HeroManager.Enemies.SelectMany(
                        enemy =>
                            SpellDatabase.EvadeableSpells.Where(
                                p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
            {
                esettings.Add(string.Format("e.protect.{0}", spell.SpellName),
                    new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            esettings.AddSeparator();
            esettings.AddGroupLabel(":: 可吸收 指向性技能");
            foreach (
                var spell in
                    HeroManager.Enemies.SelectMany(
                        enemy =>
                            SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted))
                )
            {
                esettings.Add(string.Format("e.protect.targetted.{0}", spell.SpellName),
                    new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            esettings.AddSeparator();

            esettings.AddGroupLabel(":: 开团技能");
            foreach (
                var spell in
                    HeroManager.Allies.SelectMany(
                        ally => SpellDatabase.EscapeSpells.Where(p => p.ChampionName == ally.ChampionName)))
            {
                esettings.Add(string.Format("e.engage.{0}", spell.SpellName),
                    new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            esettings.AddSeparator();
            esettings.AddGroupLabel(":: 白名单");
            foreach (var ally in HeroManager.Allies.Where(x => x.IsValid))
            {
                esettings.Add("e." + ally.ChampionName,
                    new CheckBox("(E): " + ally.ChampionName, HighChamps.Contains(ally.ChampionName)));
            }
            esettings.AddSeparator();
            esettings.Add("turret.hp.percent", new Slider("塔血量%", 10, 1, 99));
            esettings.Add("protect.carry.from.turret", new CheckBox("塔下保护C位"));
            esettings.Add("min.mana.for.e", new Slider("最低蓝量", 50, 1, 99));

            rsettings = Config.AddSubMenu(":: R 设置", ":: R Settings");
            rsettings.AddGroupLabel(":: 可阻止的技能");
            foreach (
                var spell in
                    HeroManager.Enemies.SelectMany(
                        enemy =>
                            SpellDatabase.EvadeableSpells.Where(
                                p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
            {
                rsettings.Add(string.Format("r.protect.{0}", spell.SpellName),
                    new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            rsettings.AddGroupLabel(":: 可阻止的指向性技能");
            foreach (
                var spell in
                    HeroManager.Enemies.SelectMany(
                        enemy =>
                            SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted))
                )
            {
                rsettings.Add(string.Format("r.protect.targetted.{0}", spell.SpellName),
                    new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot)));
            }
            rsettings.Add("spell.damage.percent", new Slider("最低技能伤害%", 10, 1, 99));

            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            AntiGapcloser.OnEnemyGapcloser += OnGapcloser;
            Game.OnUpdate += OnUpdate;
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

        private static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsEnemy && gapcloser.End.Distance(ObjectManager.Player.Position) < 200 &&
                gapcloser.Sender.IsValidTarget(Q.Range) && getCheckBoxItem(qsettings, "q.antigapcloser"))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (E.IsReady())
            {
                if (sender.IsAlly && sender is AIHeroClient && getCheckBoxItem(esettings, "e.engage." + args.SData.Name)
                    && getCheckBoxItem(esettings, "e." + sender.CharData.BaseSkinName) &&
                    sender.Distance(ObjectManager.Player.Position) <= E.Range
                    && !sender.IsDead && !sender.IsZombie && sender.IsValid)
                {
                    E.CastOnUnit(sender);
                }

                if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsAlly &&
                    args.Target.Type == GameObjectType.AIHeroClient
                    && args.SData.IsAutoAttack() &&
                    ObjectManager.Player.ManaPercent >= getSliderItem(esettings, "min.mana.for.e")
                    && getCheckBoxItem(esettings, "e." + ((AIHeroClient) args.Target).ChampionName) &&
                    ((AIHeroClient) args.Target).Distance(ObjectManager.Player.Position) < E.Range)
                {
                    E.Cast((AIHeroClient) args.Target);
                }

                if (sender is Obj_AI_Turret && args.Target.IsAlly &&
                    ObjectManager.Player.ManaPercent >= getSliderItem(esettings, "min.mana.for.e")
                    && getCheckBoxItem(esettings, "e." + ((AIHeroClient) args.Target).ChampionName) &&
                    ((AIHeroClient) args.Target).Distance(ObjectManager.Player.Position) < E.Range
                    && getCheckBoxItem(esettings, "protect.carry.from.turret"))
                {
                    E.Cast((AIHeroClient) args.Target);
                }

                if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                    && !args.SData.IsAutoAttack() &&
                    (getCheckBoxItem(esettings, "e.protect." + args.SData.Name) ||
                     getCheckBoxItem(esettings, "e.protect.targetted." + args.SData.Name))
                    && sender.IsEnemy &&
                    sender.LSGetSpellDamage((AIHeroClient) args.Target, args.SData.Name) >
                    ((AIHeroClient) args.Target).Health)
                {
                    E.Cast((AIHeroClient) args.Target);
                }

                if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsAlly &&
                    args.Target.Type == GameObjectType.obj_AI_Turret
                    && args.SData.IsAutoAttack() &&
                    ObjectManager.Player.ManaPercent >= getSliderItem(esettings, "min.mana.for.e")
                    && ((AIHeroClient) args.Target).Distance(ObjectManager.Player.Position) < E.Range
                    && ((AIHeroClient) args.Target).HealthPercent < getSliderItem(esettings, "turret.hp.percent"))
                {
                    E.Cast((AIHeroClient) args.Target);
                }
            }

            if (R.IsReady())
            {
                if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                    && !args.SData.IsAutoAttack() &&
                    (getCheckBoxItem(rsettings, "r.protect." + args.SData.Name) ||
                     getCheckBoxItem(rsettings, "r.protect.targetted." + args.SData.Name))
                    && sender.IsEnemy &&
                    sender.LSGetSpellDamage((AIHeroClient) args.Target, args.SData.Name) >
                    ((AIHeroClient) args.Target).Health
                    &&
                    sender.LSGetSpellDamage((AIHeroClient) args.Target, args.SData.Name)*100/
                    ((AIHeroClient) args.Target).Health < getSliderItem(rsettings, "spell.damage.percent"))
                {
                    R.Cast((AIHeroClient) args.Target);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }
        }

        private static void OnCombo()
        {
            if (getCheckBoxItem(comboMenu, "q.combo"))
            {
                switch (getBoxItem(qsettings, "q.settings"))
                {
                    case 0:
                        foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                        {
                            Q.CastIfHitchanceEquals(enemy, HikiChance("q.normal.hit.chance"));
                        }
                        break;
                    case 1:
                        if (ObjectManager.Player.CountEnemiesInRange(Q.Range) >= getSliderItem(qsettings, "q.hit.count"))
                        {
                            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && Q.GetHitCount() >= getSliderItem(qsettings, "q.hit.count")))
                            {
                                Q.CastIfWillHit(enemy, getSliderItem(qsettings, "q.hit.count"));
                            }
                        }
                        break;
                }
            }
            if (getCheckBoxItem(comboMenu, "w.combo") && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.CastOnUnit(enemy);
                }
            }
        }
    }
}