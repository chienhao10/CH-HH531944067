namespace KappaUtility.Summoners
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;
    using EloBuddy.SDK.Enumerations;

    using KappaUtility.Common;

    using SharpDX;

    internal class Spells
    {
        public static Spell.Active Heal;

        public static Spell.Active Barrier;

        public static Spell.Targeted Ignite;

        public static Spell.Targeted Smite;

        public static Spell.Targeted Exhaust;

        public static Spell.Skillshot porotoss;

        public static readonly string[] Junglemobs =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder",
                "SRU_Baron", "SRU_Gromp", "SRU_Krug", "SRU_RiftHerald", "Sru_Crab", "SRU_Murkwolf", "SRU_Blue",
                "SRU_Red", "AscXerath"
            };

        public static Menu SummMenu { get; private set; }

        protected static bool loaded = false;

        internal static void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;

            SummMenu = Load.UtliMenu.AddSubMenu("召唤师技能");
            SummMenu.AddGroupLabel("召唤师技能设置");

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerDot")) != null)
            {
                SummMenu.AddGroupLabel("点燃设置");
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableIgnite",
                    new KeyBind("开启点燃开关", false, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableactiveIgnite",
                    new KeyBind("点燃开关", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawIgnite", new CheckBox("显示点燃线圈", false));
                SummMenu.AddGroupLabel("对英雄不使用点燃:");
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    CheckBox cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                    if (enemy.Team != Player.Instance.Team)
                    {
                        SummMenu.Add("DontIgnite" + enemy.BaseSkinName, cb);
                    }
                }

                SummMenu.AddSeparator();
                Ignite = new Spell.Targeted(Player.Instance.GetSpellSlotFromName("SummonerDot"), 600);
            }

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerBarrier")) != null)
            {
                SummMenu.AddGroupLabel("护盾设置");
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableBarrier",
                    new KeyBind("开启护盾开关", false, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableactiveBarrier",
                    new KeyBind("护盾开关", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("barrierme", new Slider("血量为 X%时使用", 30, 0, 100));
                SummMenu.AddSeparator();
                Barrier = new Spell.Active(Player.Instance.GetSpellSlotFromName("SummonerBarrier"));
            }

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerHeal")) != null)
            {
                SummMenu.AddGroupLabel("治疗设置");
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableHeal",
                    new KeyBind("治疗开关", false, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableactiveHeal",
                    new KeyBind("开启质量", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawHeal", new CheckBox("显示治疗线圈", false));
                SummMenu.Add("Healally", new Slider("友军血量 X%时使用", 25, 0, 100));
                SummMenu.Add("Healme", new Slider("自身血量为 X%时使用", 30, 0, 100));
                SummMenu.AddGroupLabel("对英雄不使用治疗:");
                foreach (var ally in ObjectManager.Get<AIHeroClient>())
                {
                    CheckBox cb = new CheckBox(ally.BaseSkinName) { CurrentValue = false };
                    if (ally.Team == Player.Instance.Team)
                    {
                        SummMenu.Add("DontHeal" + ally.BaseSkinName, cb);
                    }
                }

                SummMenu.AddSeparator();
                Heal = new Spell.Active(Player.Instance.GetSpellSlotFromName("SummonerHeal"), 850);
            }

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerExhaust")) != null)
            {
                SummMenu.AddGroupLabel("虚弱设置");
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableExhaust",
                    new KeyBind("虚弱开关", true, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableactiveExhaust",
                    new KeyBind("开启虚弱", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawExhaust", new CheckBox("显示虚弱范围", false));
                SummMenu.Add("exhaustally", new Slider("自身/友军血量 X%时使用", 35, 0, 100));
                SummMenu.Add("exhaustenemy", new Slider("敌军血量 X%时使用", 40, 0, 100));
                SummMenu.AddGroupLabel("对英雄不使用虚弱:");
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    var cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                    if (enemy.Team != Player.Instance.Team)
                    {
                        SummMenu.Add("DontExhaust" + enemy.BaseSkinName, cb);
                    }
                }

                Exhaust = new Spell.Targeted(Player.Instance.GetSpellSlotFromName("SummonerExhaust"), 650);
            }

            var smitespell = Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains("summonersmite"));
            if (smitespell != null)
            {
                SummMenu.AddGroupLabel("惩戒（重击）设置");
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableSmite",
                    new KeyBind("惩戒开关", true, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableactiveSmite",
                    new KeyBind("开启惩戒", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("smitemob", new CheckBox("惩戒野怪", false));
                SummMenu.Add("smitecombo", new CheckBox("连招惩戒", false));
                SummMenu.Add("smiteks", new CheckBox("惩戒抢头", false));
                SummMenu.Add("drawSmite", new CheckBox("显示惩戒范围", false));
                SummMenu.AddGroupLabel("对英雄不使用惩戒:");
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    var cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                    if (enemy.Team != Player.Instance.Team)
                    {
                        SummMenu.Add("DontSmite" + enemy.BaseSkinName, cb);
                    }
                }

                SummMenu.AddGroupLabel("Use Smite On Monster:");
                foreach (var mob in Junglemobs)
                {
                    SummMenu.Add(mob, new CheckBox(mob));
                }
                SummMenu.AddSeparator();

                Smite = new Spell.Targeted(smitespell.Slot, 555);
                Orbwalker.OnPostAttack += Summoners.Smite.Orbwalker_OnPostAttack;
            }

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerPoroThrow")) != null
                || Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains("summonersnowball")) != null)
            {
                SummMenu.AddGroupLabel("雪球设置");
                SummMenu.Add(Player.Instance.ChampionName + "EnablePoro", new KeyBind("Enable Poro Toggle", true, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add(
                    Player.Instance.ChampionName + "EnableactivePoro",
                    new KeyBind("开启雪球", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawporo", new CheckBox("显示雪球范围", false));
                SummMenu.AddGroupLabel("不丢雪球:");
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    var cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                    SummMenu.Add("Dontporo" + enemy.BaseSkinName, cb);
                }
                if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerPoroThrow")) != null)
                {
                    porotoss = new Spell.Skillshot(
                        Player.Instance.GetSpellSlotFromName("SummonerPoroThrow"),
                        2250,
                        SkillShotType.Linear,
                        50,
                        1000,
                        50) { AllowedCollisionCount = 0 };
                }
                if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerSnowball")) != null)
                {
                    porotoss = new Spell.Skillshot(Player.Instance.GetSpellSlotFromName("SummonerSnowball"), 1600, SkillShotType.Linear, 50, 1000, 50)
                                   { AllowedCollisionCount = 0 };
                }
            }
            loaded = true;
        }

        internal static void Drawings()
        {
            if (!loaded)
            {
                return;
            }

            if (Ignite != null)
            {
                if (SummMenu["drawIgnite"].Cast<CheckBox>().CurrentValue
                    && (SummMenu[Player.Instance.ChampionName + "EnableactiveIgnite"].Cast<KeyBind>().CurrentValue
                        || SummMenu[Player.Instance.ChampionName + "EnableIgnite"].Cast<KeyBind>().CurrentValue))
                {
                    Circle.Draw(Ignite.IsReady() ? Color.LightBlue : Color.Red, Ignite.Range, Player.Instance.Position);
                }
            }
            if (Heal != null)
            {
                if (SummMenu["drawheal"].Cast<CheckBox>().CurrentValue
                    && (SummMenu[Player.Instance.ChampionName + "EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                        || SummMenu[Player.Instance.ChampionName + "EnableHeal"].Cast<KeyBind>().CurrentValue))
                {
                    Circle.Draw(Heal.IsReady() ? Color.LightBlue : Color.Red, Heal.Range, Player.Instance.Position);
                }
            }
            if (Smite != null)
            {
                if (SummMenu["drawSmite"].Cast<CheckBox>().CurrentValue
                    && (SummMenu[Player.Instance.ChampionName + "EnableactiveSmite"].Cast<KeyBind>().CurrentValue
                        || SummMenu[Player.Instance.ChampionName + "EnableSmite"].Cast<KeyBind>().CurrentValue))
                {
                    Circle.Draw(Smite.IsReady() ? Color.LightBlue : Color.Red, Smite.Range, Player.Instance.Position);
                }
            }

            if (Exhaust != null)
            {
                if (SummMenu["drawExhaust"].Cast<CheckBox>().CurrentValue
                    && (SummMenu[Player.Instance.ChampionName + "EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                        || SummMenu[Player.Instance.ChampionName + "EnableExhaust"].Cast<KeyBind>().CurrentValue))
                {
                    Circle.Draw(Exhaust.IsReady() ? Color.LightBlue : Color.Red, Exhaust.Range, Player.Instance.Position);
                }
            }
            if (porotoss != null)
            {
                if (SummMenu["drawporo"].Cast<CheckBox>().CurrentValue)
                {
                    Circle.Draw(porotoss.IsReady() ? Color.LightBlue : Color.Red, porotoss.Range, Player.Instance.Position);
                }
            }
        }

        public static void Cast()
        {
            var target = TargetSelector.GetTarget(600, DamageType.True);

            var ally = ObjectManager.Get<AIHeroClient>().FirstOrDefault(a => a.IsValid && a.IsAlly && a.IsVisible);

            if (porotoss != null && !porotoss.Name.ToLower().Contains("snowballfollowupcast"))
            {
                if (SummMenu[Player.Instance.ChampionName + "Enableactiveporo"].Cast<KeyBind>().CurrentValue
                    || SummMenu[Player.Instance.ChampionName + "Enableporo"].Cast<KeyBind>().CurrentValue)
                {
                    foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(porotoss.Range)))
                    {
                        if (enemy != null)
                        {
                            var pred = porotoss.GetPrediction(enemy);
                            if (pred.HitChance >= HitChance.High)
                            {
                                porotoss.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }

            if (target == null)
            {
                return;
            }
            if (Ignite != null)
            {
                var ignitec = (SummMenu[Player.Instance.ChampionName + "EnableactiveIgnite"].Cast<KeyBind>().CurrentValue
                               || SummMenu[Player.Instance.ChampionName + "EnableIgnite"].Cast<KeyBind>().CurrentValue) && Ignite.IsReady();

                if (ignitec
                    && Player.Instance.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite)
                    >= target.TotalShieldHealth() + (target.HPRegenRate * 3))
                {
                    if (target.IsValidTarget(Ignite.Range) && !SummMenu["DontIgnite" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        Ignite.Cast(target);
                    }
                }
            }

            if (Exhaust != null)
            {
                var exhaustc = (SummMenu[Player.Instance.ChampionName + "EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                || SummMenu[Player.Instance.ChampionName + "EnableExhaust"].Cast<KeyBind>().CurrentValue) && Exhaust.IsReady();
                var Exhaustally = SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                var Exhaustenemy = SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;

                if (exhaustc && (target.IsValidTarget(Exhaust.Range) && !SummMenu["DontExhaust" + target.BaseSkinName].Cast<CheckBox>().CurrentValue))
                {
                    if (target.HealthPercent <= Exhaustenemy)
                    {
                        Exhaust.Cast(target);
                    }

                    if (ally != null && ally.HealthPercent <= Exhaustally)
                    {
                        Exhaust.Cast(target);
                    }
                }
            }
        }

        public static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((caster is AIHeroClient || caster is Obj_AI_Turret) && caster != null && target != null && caster.IsEnemy)
            {
                if (target.IsAlly && !target.IsMe)
                {
                    if (Exhaust != null)
                    {
                        var exhaustc = (SummMenu[Player.Instance.ChampionName + "EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                        || SummMenu[Player.Instance.ChampionName + "EnableExhaust"].Cast<KeyBind>().CurrentValue) && Exhaust.IsReady();
                        var Exhaustally = SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                        var Exhaustenemy = SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;

                        if (exhaustc
                            && (target.IsValidTarget(Exhaust.Range) && !SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue))
                        {
                            if (target.HealthPercent <= Exhaustenemy)
                            {
                                Exhaust.Cast(caster);
                            }

                            if (target.HealthPercent <= Exhaustally)
                            {
                                Exhaust.Cast(caster);
                            }
                        }
                    }

                    if (Heal != null && !SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (SummMenu[Player.Instance.ChampionName + "EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || SummMenu[Player.Instance.ChampionName + "EnableHeal"].Cast<KeyBind>().CurrentValue) && Heal.IsReady();
                        var healally = SummMenu["Healally"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.IsInRange(Player.Instance, Heal.Range))
                            {
                                if (target.HealthPercent <= healally)
                                {
                                    Heal.Cast();
                                }

                                if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                                {
                                    Heal.Cast();
                                }
                            }
                        }
                    }
                }

                if (target.IsMe)
                {
                    if (Heal != null && !SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (SummMenu[Player.Instance.ChampionName + "EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || SummMenu[Player.Instance.ChampionName + "EnableHeal"].Cast<KeyBind>().CurrentValue) && Heal.IsReady();
                        var healme = SummMenu["Healme"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.HealthPercent <= healme)
                            {
                                Heal.Cast();
                            }

                            if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                            {
                                Heal.Cast();
                            }
                        }
                    }

                    if (Exhaust != null)
                    {
                        var exhaustc = (SummMenu[Player.Instance.ChampionName + "EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                        || SummMenu[Player.Instance.ChampionName + "EnableExhaust"].Cast<KeyBind>().CurrentValue) && Exhaust.IsReady();
                        var Exhaustally = SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                        var Exhaustenemy = SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;
                        if (exhaustc && !SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            if (target.HealthPercent <= Exhaustenemy)
                            {
                                Exhaust.Cast(caster);
                            }

                            if (target.HealthPercent <= Exhaustally)
                            {
                                Exhaust.Cast(caster);
                            }
                        }
                    }

                    if (Barrier != null)
                    {
                        var barrierc = (SummMenu[Player.Instance.ChampionName + "EnableactiveBarrier"].Cast<KeyBind>().CurrentValue
                                        || SummMenu[Player.Instance.ChampionName + "EnableBarrier"].Cast<KeyBind>().CurrentValue) && Barrier.IsReady();
                        var barrierme = SummMenu["barrierme"].Cast<Slider>().CurrentValue;
                        if (barrierc)
                        {
                            if (target.HealthPercent <= barrierme)
                            {
                                Barrier.Cast();
                            }

                            if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                            {
                                Barrier.Cast();
                            }
                        }
                    }
                }
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((caster is AIHeroClient || caster is Obj_AI_Turret) && caster != null && target != null && caster.IsEnemy)
            {
                if (target.IsAlly && !target.IsMe)
                {
                    if (Heal != null && !SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (SummMenu[Player.Instance.ChampionName + "EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || SummMenu[Player.Instance.ChampionName + "EnableHeal"].Cast<KeyBind>().CurrentValue) && Heal.IsReady();
                        var healally = SummMenu["Healally"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.IsInRange(Player.Instance, Heal.Range))
                            {
                                if (target.HealthPercent <= healally)
                                {
                                    Heal.Cast();
                                }

                                if (caster.BaseAttackDamage > target.TotalShieldHealth() || caster.BaseAbilityDamage > target.TotalShieldHealth())
                                {
                                    Heal.Cast();
                                }
                            }
                        }
                    }

                    if (Exhaust != null)
                    {
                        var exhaustc = (SummMenu[Player.Instance.ChampionName + "EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                        || SummMenu[Player.Instance.ChampionName + "EnableExhaust"].Cast<KeyBind>().CurrentValue) && Exhaust.IsReady();
                        var Exhaustally = SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                        var Exhaustenemy = SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;
                        if (exhaustc && !SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            if (target.HealthPercent <= Exhaustenemy)
                            {
                                Exhaust.Cast(caster);
                            }

                            if (target.HealthPercent <= Exhaustally)
                            {
                                Exhaust.Cast(caster);
                            }
                        }
                    }
                }

                if (target.IsMe)
                {
                    if (Heal != null && !SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (SummMenu[Player.Instance.ChampionName + "EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || SummMenu[Player.Instance.ChampionName + "EnableHeal"].Cast<KeyBind>().CurrentValue) && Heal.IsReady();
                        var healme = SummMenu["Healme"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.HealthPercent <= healme)
                            {
                                Heal.Cast();
                            }

                            if (caster.BaseAttackDamage > target.TotalShieldHealth() || caster.BaseAbilityDamage > target.TotalShieldHealth())
                            {
                                Heal.Cast();
                            }
                        }

                        if (Exhaust != null)
                        {
                            var exhaustc = (SummMenu[Player.Instance.ChampionName + "EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                            || SummMenu[Player.Instance.ChampionName + "EnableExhaust"].Cast<KeyBind>().CurrentValue)
                                           && Exhaust.IsReady();
                            var Exhaustally = SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                            var Exhaustenemy = SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;
                            if (exhaustc && !SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue)
                            {
                                if (target.HealthPercent <= Exhaustenemy)
                                {
                                    Exhaust.Cast(caster);
                                }

                                if (target.HealthPercent <= Exhaustally)
                                {
                                    Exhaust.Cast(caster);
                                }
                            }
                        }
                    }

                    if (Barrier != null)
                    {
                        var barrierc = (SummMenu[Player.Instance.ChampionName + "EnableactiveBarrier"].Cast<KeyBind>().CurrentValue
                                        || SummMenu[Player.Instance.ChampionName + "EnableBarrier"].Cast<KeyBind>().CurrentValue) && Barrier.IsReady();
                        var barrierme = SummMenu["barrierme"].Cast<Slider>().CurrentValue;
                        if (barrierc)
                        {
                            if (target.HealthPercent <= barrierme)
                            {
                                Barrier.Cast();
                            }

                            if (caster.BaseAttackDamage > target.TotalShieldHealth() || caster.BaseAbilityDamage > target.TotalShieldHealth())
                            {
                                Barrier.Cast();
                            }
                        }
                    }
                }
            }
        }
    }
}