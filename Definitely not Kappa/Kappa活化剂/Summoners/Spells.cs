namespace KappaUtility.Summoners
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;
    using EloBuddy.SDK.Enumerations;

    using Common;

    using SharpDX;

    internal class Spells
    {
        public static Spell.Active Heal;

        public static Spell.Active Barrier;

        public static Spell.Targeted Ignite;

        public static Spell.Targeted Smite;

        public static Spell.Targeted Exhaust;

        public static Spell.Skillshot porotoss;

        public static readonly string[] SRJunglemobs =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water",
                "SRU_Dragon_Elder", "SRU_Baron", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "SRU_RiftHerald",
                "Sru_Crab", "SRU_Murkwolf", "SRU_Blue", "SRU_Red", "AscXerath"
            };

        public static readonly string[] TTJunglemobs = { "TT_NWraith", "TT_NWolf", "TT_NGolem", "TT_Spiderboss" };

        public static Menu SummMenu { get; private set; }

        protected static bool loaded = false;

        internal static void OnLoad()
        {
            SummMenu = Load.UtliMenu.AddSubMenu("召唤师技能");
            SummMenu.AddGroupLabel("召唤师技能设置");

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerDot")) != null)
            {
                SummMenu.AddGroupLabel("点燃设置");
                SummMenu.Add("EnableIgnite", new KeyBind("开启点燃", false, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add("EnableactiveIgnite", new KeyBind("启用点燃开关", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawIgnite", new CheckBox("显示点燃线圈", false));
                SummMenu.Checkbox("drawIgniteStat", "显示点燃状态");
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
                SummMenu.Add("EnableBarrier", new KeyBind("开启护盾开关", false, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add("EnableactiveBarrier", new KeyBind("护盾开关", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Checkbox("drawBarrierStat", "显示护盾状态");
                SummMenu.Add("barrierme", new Slider("血量为 X%时使用", 30, 0, 100));
                SummMenu.AddSeparator();
                Barrier = new Spell.Active(Player.Instance.GetSpellSlotFromName("SummonerBarrier"));
            }

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerHeal")) != null)
            {
                SummMenu.AddGroupLabel("治疗设置");
                SummMenu.Add("EnableHeal", new KeyBind("治疗开关", false, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add("EnableactiveHeal", new KeyBind("开启治疗", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawHeal", new CheckBox("显示治疗范围", false));
                SummMenu.Checkbox("drawHealStat", "显示治疗状态");
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
                SummMenu.Add("EnableExhaust", new KeyBind("启用虚弱开关", true, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add("EnableactiveExhaust", new KeyBind("开启虚弱", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawExhaust", new CheckBox("显示虚弱范围", false));
                SummMenu.Checkbox("drawExhaustStat", "显示虚弱状态");
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
                SummMenu.Add("EnableSmite", new KeyBind("启用惩戒开关", true, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add("EnableactiveSmite", new KeyBind("开启惩戒", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawSmite", new CheckBox("显示惩戒范围", false));
                SummMenu.Checkbox("drawSmiteStat", "显示惩戒状态");
                SummMenu.AddSeparator(1);
                SummMenu.AddGroupLabel("惩戒设置:");
                SummMenu.Add("smitemob", new CheckBox("惩戒野怪", false));
                SummMenu.Add("smitesavej", new CheckBox("保留1次惩戒", false));

                if (Game.MapId == GameMapId.SummonersRift)
                {
                    SummMenu.AddLabel("惩戒召唤师峡谷野怪:");
                    foreach (var mob in SRJunglemobs)
                    {
                        SummMenu.Add(mob, new CheckBox(mob));
                    }
                    SummMenu.AddSeparator();
                }

                if (Game.MapId == GameMapId.TwistedTreeline)
                {
                    SummMenu.AddLabel("惩戒扭曲丛林野怪:");
                    foreach (var mob in TTJunglemobs)
                    {
                        SummMenu.Add(mob, new CheckBox(mob));
                    }
                    SummMenu.AddSeparator(1);
                }

                SummMenu.AddGroupLabel("惩戒英雄:");
                SummMenu.Add("smitecombo", new CheckBox("连招惩戒", false));
                SummMenu.Add("smiteks", new CheckBox("惩戒抢头", false));
                SummMenu.Add("smitesaveh", new CheckBox("保留1次惩戒", false));
                SummMenu.AddLabel("不为以下使用惩戒:");
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    var cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                    if (enemy.Team != Player.Instance.Team)
                    {
                        SummMenu.Add("DontSmite" + enemy.BaseSkinName, cb);
                    }
                }

                Smite = new Spell.Targeted(smitespell.Slot, 555);
                Orbwalker.OnPostAttack += Summoners.Smite.Orbwalker_OnPostAttack;
            }

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerPoroThrow")) != null
                || Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains("summonersnowball")) != null)
            {
                SummMenu.AddGroupLabel("雪球设置");
                SummMenu.Add("EnablePoro", new KeyBind("启用雪球开关", true, KeyBind.BindTypes.PressToggle, 'M'));
                SummMenu.Add("EnableactivePoro", new KeyBind("开启雪球", false, KeyBind.BindTypes.HoldActive));
                SummMenu.Add("drawporo", new CheckBox("显示雪球范围", false));
                SummMenu.Checkbox("drawporoStat", "显示雪球状态");
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

        public static System.Drawing.Color c(Spell.SpellBase spell)
        {
            return spell.IsReady() ? System.Drawing.Color.White : System.Drawing.Color.OrangeRed;
        }

        public static string ready(Spell.SpellBase spell)
        {
            return spell.IsReady() ? ": Ready" : ": CoolDown";
        }

        internal static void Drawings()
        {
            if (!loaded)
            {
                return;
            }

            var pos = Player.Instance.ServerPosition;
            var posx = pos.WorldToScreen().X;
            var posy = pos.WorldToScreen().Y;
            if (Ignite != null)
            {
                if (SummMenu["EnableactiveIgnite"].Cast<KeyBind>().CurrentValue || SummMenu["EnableIgnite"].Cast<KeyBind>().CurrentValue)
                {
                    if (SummMenu["drawIgnite"].Cast<CheckBox>().CurrentValue)
                    {
                        Circle.Draw(Ignite.IsReady() ? Color.LightBlue : Color.Red, Ignite.Range, pos);
                    }
                    if (SummMenu["drawIgniteStat"].Cast<CheckBox>().CurrentValue)
                    {
                        Drawing.DrawText(posx, posy, c(Ignite), "Ignite" + ready(Ignite), 2);
                    }
                }
            }

            if (Heal != null)
            {
                if (SummMenu["EnableactiveHeal"].Cast<KeyBind>().CurrentValue || SummMenu["EnableHeal"].Cast<KeyBind>().CurrentValue)
                {
                    if (SummMenu["drawheal"].Cast<CheckBox>().CurrentValue)
                    {
                        Circle.Draw(Heal.IsReady() ? Color.LightBlue : Color.Red, Heal.Range, pos);
                    }

                    if (SummMenu["drawHealStat"].Cast<CheckBox>().CurrentValue)
                    {
                        Drawing.DrawText(posx, posy, c(Heal), "Heal" + ready(Heal), 2);
                    }
                }
            }

            if (Smite != null)
            {
                if (SummMenu["EnableactiveSmite"].Cast<KeyBind>().CurrentValue || SummMenu["EnableSmite"].Cast<KeyBind>().CurrentValue)
                {
                    if (SummMenu["drawSmite"].Cast<CheckBox>().CurrentValue)
                    {
                        Circle.Draw(Smite.IsReady() ? Color.LightBlue : Color.Red, Smite.Range, pos);
                    }

                    if (SummMenu["drawSmiteStat"].Cast<CheckBox>().CurrentValue)
                    {
                        Drawing.DrawText(posx, posy, c(Smite), "Smite" + ready(Smite), 2);
                    }
                }
            }

            if (Exhaust != null)
            {
                if (SummMenu["EnableactiveExhaust"].Cast<KeyBind>().CurrentValue || SummMenu["EnableExhaust"].Cast<KeyBind>().CurrentValue)
                {
                    if (SummMenu["drawExhaust"].Cast<CheckBox>().CurrentValue)
                    {
                        Circle.Draw(Exhaust.IsReady() ? Color.LightBlue : Color.Red, Exhaust.Range, pos);
                    }

                    if (SummMenu["drawExhaustStat"].Cast<CheckBox>().CurrentValue)
                    {
                        Drawing.DrawText(posx, posy, c(Exhaust), "Exhaust" + ready(Exhaust), 2);
                    }
                }
            }

            if (porotoss != null)
            {
                if (SummMenu["drawporo"].Cast<CheckBox>().CurrentValue)
                {
                    Circle.Draw(porotoss.IsReady() ? Color.LightBlue : Color.Red, porotoss.Range, pos);
                }

                if (SummMenu["drawporoStat"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawText(posx, posy, c(porotoss), "PoroToss" + ready(porotoss), 2);
                }
            }
        }

        public static void Cast()
        {
            var target = TargetSelector.GetTarget(600, DamageType.True);

            var ally = ObjectManager.Get<AIHeroClient>().FirstOrDefault(a => a.IsValid && a.IsAlly && a.IsVisible);

            if (porotoss != null && !porotoss.Name.ToLower().Contains("snowballfollowupcast"))
            {
                if (SummMenu["Enableactiveporo"].Cast<KeyBind>().CurrentValue || SummMenu["Enableporo"].Cast<KeyBind>().CurrentValue)
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
                var ignitec = (SummMenu["EnableactiveIgnite"].Cast<KeyBind>().CurrentValue || SummMenu["EnableIgnite"].Cast<KeyBind>().CurrentValue)
                              && Ignite.IsReady();

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
                var exhaustc = (SummMenu["EnableactiveExhaust"].Cast<KeyBind>().CurrentValue || SummMenu["EnableExhaust"].Cast<KeyBind>().CurrentValue)
                               && Exhaust.IsReady();
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
    }
}