namespace KappaUtility.Items
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Common;

    internal class Potions
    {
        public static Menu PotMenu { get; private set; }

        public static readonly Item Corrupting = new Item(ItemId.Corrupting_Potion);

        public static readonly Item Health = new Item(ItemId.Health_Potion);

        public static readonly Item Hunters = new Item(ItemId.Hunters_Potion);

        public static readonly Item Refillable = new Item((int)ItemId.Refillable_Potion);

        public static readonly Item Biscuit = new Item((int)ItemId.Total_Biscuit_of_Rejuvenation);

        public static int Corruptingh => PotMenu["CPH"].Cast<Slider>().CurrentValue;

        public static int Healthh => PotMenu["HPH"].Cast<Slider>().CurrentValue;

        public static int Huntersh => PotMenu["HPSH"].Cast<Slider>().CurrentValue;

        public static int Refillableh => PotMenu["RPH"].Cast<Slider>().CurrentValue;

        public static int Biscuith => PotMenu["BPH"].Cast<Slider>().CurrentValue;

        public static int Corruptingn => PotMenu["CPN"].Cast<Slider>().CurrentValue;

        public static int Healthn => PotMenu["HPN"].Cast<Slider>().CurrentValue;

        public static int Huntersn => PotMenu["HPSN"].Cast<Slider>().CurrentValue;

        public static int Refillablen => PotMenu["RPN"].Cast<Slider>().CurrentValue;

        public static int Biscuitn => PotMenu["BPN"].Cast<Slider>().CurrentValue;

        public static bool Corruptingc
            =>
                PotMenu["CP"].Cast<CheckBox>().CurrentValue && Corrupting.IsOwned() && Corrupting.IsReady()
                && !Player.Instance.HasBuff(Corrupting.ItemInfo.Name);

        public static bool Healthc
            => PotMenu["HP"].Cast<CheckBox>().CurrentValue && Health.IsOwned() && Health.IsReady() && !Player.Instance.HasBuff(Health.ItemInfo.Name);

        public static bool Huntersc
            =>
                PotMenu["HPS"].Cast<CheckBox>().CurrentValue && Hunters.IsOwned() && Hunters.IsReady()
                && !Player.Instance.HasBuff(Hunters.ItemInfo.Name);

        public static bool Refillablec
            =>
                PotMenu["RP"].Cast<CheckBox>().CurrentValue && Refillable.IsOwned() && Refillable.IsReady()
                && !Player.Instance.HasBuff(Refillable.ItemInfo.Name);

        public static bool Biscuitc
            =>
                PotMenu["BP"].Cast<CheckBox>().CurrentValue && Biscuit.IsOwned() && Biscuit.IsReady()
                && !Player.Instance.HasBuff(Biscuit.ItemInfo.Name);

        internal static void OnLoad()
        {
            PotMenu = Load.UtliMenu.AddSubMenu("药水");
            PotMenu.AddGroupLabel("一般设置");
            PotMenu.Add("mob", new CheckBox("受塔攻击使小兵", false));
            PotMenu.Add("jmob", new CheckBox("受野怪攻击使用"));
            PotMenu.Add("champ", new CheckBox("受英雄攻击使用"));
            PotMenu.Add("tower", new CheckBox("受塔攻击使用"));
            PotMenu.AddGroupLabel("自动喝药设置");
            PotMenu.Add("CP", new CheckBox("腐蚀药水", false));
            PotMenu.Add("CPH", new Slider("血量为 X %时使用", 65));
            PotMenu.Add("CPN", new Slider("使用药水如果预计受到的伤害多于[{0}%]", 35));
            PotMenu.Add("HP", new CheckBox("红药", false));
            PotMenu.Add("HPH", new Slider("血量为 X %时使用", 45));
            PotMenu.Add("HPN", new Slider("使用药水如果预计受到的伤害多于[{0}%]", 35));
            PotMenu.Add("HPS", new CheckBox("猎人药水", false));
            PotMenu.Add("HPSH", new Slider("血量为 X %时使用", 75));
            PotMenu.Add("HPSN", new Slider("使用药水如果预计受到的伤害多于[{0}%]", 35));
            PotMenu.Add("RP", new CheckBox("可充药水", false));
            PotMenu.Add("RPH", new Slider("血量为 X %时使用", 50));
            PotMenu.Add("RPN", new Slider("使用药水如果预计受到的伤害多于[{0}%]", 35));
            PotMenu.Add("BP", new CheckBox("饼干", false));
            PotMenu.Add("BPH", new Slider("血量为 X %时使用", 40));
            PotMenu.Add("BPN", new Slider("使用药水如果预计受到的伤害多于[{0}%]", 35));

            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;
            if (((caster is AIHeroClient && PotMenu["champ"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMinion && PotMenu["mob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMonster && PotMenu["jmob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Turret && PotMenu["tower"].Cast<CheckBox>().CurrentValue)) && caster.IsEnemy && target != null && target.IsMe)
            {
                if (!Player.Instance.IsRecalling() && Player.Instance.IsKillable())
                {
                    if (Refillablec)
                    {
                        if (target.HealthPercent <= Refillableh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Refillablen || args.SData.PhysicalDamageRatio >= Refillablen)
                        {
                            Refillable.Cast();
                        }
                    }

                    if (Healthc)
                    {
                        if (target.HealthPercent <= Healthh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Healthn || args.SData.PhysicalDamageRatio >= Healthn)
                        {
                            Health.Cast();
                        }
                    }

                    if (Huntersc)
                    {
                        if (target.HealthPercent <= Huntersh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Huntersn || args.SData.PhysicalDamageRatio >= Huntersn)
                        {
                            Hunters.Cast();
                        }
                    }

                    if (Biscuitc)
                    {
                        if (target.HealthPercent <= Biscuith || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Biscuitn || args.SData.PhysicalDamageRatio >= Biscuitn)
                        {
                            Biscuit.Cast();
                        }
                    }

                    if (Corruptingc)
                    {
                        if (target.HealthPercent <= Corruptingh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Corruptingn || args.SData.PhysicalDamageRatio >= Corruptingn)
                        {
                            Corrupting.Cast();
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;
            if (((caster is AIHeroClient && PotMenu["champ"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMinion && PotMenu["mob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMonster && PotMenu["jmob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Turret && PotMenu["tower"].Cast<CheckBox>().CurrentValue)) && caster.IsEnemy && target != null && target.IsMe)
            {
                if (!Player.Instance.IsRecalling() && Player.Instance.IsKillable())
                {
                    if (Refillablec)
                    {
                        if (caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth() || target.HealthPercent <= Refillableh
                            || args.SData.SpellDamageRatio >= Refillablen || args.SData.PhysicalDamageRatio >= Refillablen)
                        {
                            Refillable.Cast();
                        }
                    }

                    if (Healthc)
                    {
                        if (target.HealthPercent <= Healthh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Healthn || args.SData.PhysicalDamageRatio >= Healthn)
                        {
                            Health.Cast();
                        }
                    }

                    if (Huntersc)
                    {
                        if (target.HealthPercent <= Huntersh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Huntersn || args.SData.PhysicalDamageRatio >= Huntersn)
                        {
                            Hunters.Cast();
                        }
                    }

                    if (Biscuitc)
                    {
                        if (target.HealthPercent <= Biscuith || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Biscuitn || args.SData.PhysicalDamageRatio >= Biscuitn)
                        {
                            Biscuit.Cast();
                        }
                    }

                    if (Corruptingc)
                    {
                        if (target.HealthPercent <= Corruptingh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= Corruptingn || args.SData.PhysicalDamageRatio >= Corruptingn)
                        {
                            Corrupting.Cast();
                        }
                    }
                }
            }
        }
    }
}