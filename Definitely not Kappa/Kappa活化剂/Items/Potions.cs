namespace KappaUtility.Items
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Common;

    using SharpDX;

    internal class Potions
    {
        public static Menu PotMenu { get; private set; }

        public static readonly Item Corrupting = new Item(ItemId.Corrupting_Potion);

        public static readonly Item Health = new Item(ItemId.Health_Potion);

        public static readonly Item Hunters = new Item(ItemId.Hunters_Potion);

        public static readonly Item Refillable = new Item((int)ItemId.Refillable_Potion);

        public static readonly Item Biscuit = new Item((int)ItemId.Total_Biscuit_of_Rejuvenation);

        public static int Corruptingh => PotMenu.GetSlider("CPH");

        public static int Healthh => PotMenu.GetSlider("HPH");

        public static int Huntersh => PotMenu.GetSlider("HPSH");

        public static int Refillableh => PotMenu.GetSlider("RPH");

        public static int Biscuith => PotMenu.GetSlider("BPH");

        public static int Corruptingn => PotMenu.GetSlider("CPN");

        public static int Healthn => PotMenu.GetSlider("HPN");

        public static int Huntersn => PotMenu.GetSlider("HPSN");

        public static int Refillablen => PotMenu.GetSlider("RPN");

        public static int Biscuitn => PotMenu.GetSlider("BPN");

        public static bool Corruptingc
            =>
                PotMenu.GetCheckbox("CP") && Corrupting.IsOwned(Player.Instance) && Corrupting.IsReady()
                && !Player.Instance.HasBuff(Corrupting.ItemInfo.Name);

        public static bool Healthc
            => PotMenu.GetCheckbox("HP") && Health.IsOwned(Player.Instance) && Health.IsReady() && !Player.Instance.HasBuff(Health.ItemInfo.Name);

        public static bool Huntersc
            => PotMenu.GetCheckbox("HPS") && Hunters.IsOwned(Player.Instance) && Hunters.IsReady() && !Player.Instance.HasBuff(Hunters.ItemInfo.Name);

        public static bool Refillablec
            =>
                PotMenu.GetCheckbox("RP") && Refillable.IsOwned(Player.Instance) && Refillable.IsReady()
                && !Player.Instance.HasBuff(Refillable.ItemInfo.Name);

        public static bool Biscuitc
            => PotMenu.GetCheckbox("BP") && Biscuit.IsOwned(Player.Instance) && Biscuit.IsReady() && !Player.Instance.HasBuff(Biscuit.ItemInfo.Name);

        public static readonly string[] PotBuffs = { "ItemCrystalFlask", "ItemCrystalFlaskJungle", "ItemDarkCrystalFlask", "RegenerationPotion" };

        internal static void OnLoad()
        {
            PotMenu = Load.UtliMenu.AddSubMenu("药水");
            PotMenu.AddGroupLabel("一般设置");
            PotMenu.Checkbox("mob", "受小兵攻击使用");
            PotMenu.Checkbox("jmob", "受野怪攻击使用", true);
            PotMenu.Checkbox("champ", "受英雄攻击使用", true);
            PotMenu.Checkbox("tower", "受塔攻击使用", true);
            PotMenu.AddSeparator(0);

            PotMenu.AddGroupLabel("自动喝药设置");
            PotMenu.Checkbox("CP", "腐蚀药水");
            PotMenu.Slider("CPH", "血量为 X %时使用", 65);
            PotMenu.Slider("CPN", "使用药水如果预计受到的伤害多于[{0}%]", 35);
            PotMenu.AddSeparator(0);

            PotMenu.Checkbox("HP", "红药");
            PotMenu.Slider("HPH", "血量为 X %时使用", 45);
            PotMenu.Slider("HPN", "使用药水如果预计受到的伤害多于[{0}%]", 35);
            PotMenu.AddSeparator(0);

            PotMenu.Checkbox("HPS", "猎人药水");
            PotMenu.Slider("HPSH", "血量为 X %时使用", 75);
            PotMenu.Slider("HPSN", "使用药水如果预计受到的伤害多于[{0}%]", 35);
            PotMenu.AddSeparator(0);

            PotMenu.Checkbox("RP", "可充药水");
            PotMenu.Slider("RPH", "血量为 X %时使用", 50);
            PotMenu.Slider("RPN", "使用药水如果预计受到的伤害多于[{0}%]", 35);
            PotMenu.AddSeparator(0);

            PotMenu.Checkbox("BP", "饼干");
            PotMenu.Slider("BPH", "血量为 X %时使用", 45);
            PotMenu.Slider("BPN", "使用药水如果预计受到的伤害多于[{0}%]", 35);

            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient) || Player.Instance.Buffs.Any(b => PotBuffs.Contains(b.Name)))
            {
                return;
            }

            var caster = sender;
            var enemy = sender as AIHeroClient;
            var target = (AIHeroClient)args.Target;
            var hit = args.End != Vector3.Zero && args.End.Distance(target) < 100;

            if (((caster is AIHeroClient && PotMenu["champ"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMinion && PotMenu["mob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMonster && PotMenu["jmob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Turret && PotMenu["tower"].Cast<CheckBox>().CurrentValue)) && caster.IsEnemy && target != null && target.IsMe)
            {
                var spelldamage = enemy.GetSpellDamage(target, args.Slot);
                var damagepercent = (spelldamage / target.TotalShieldHealth()) * 100;
                var death = damagepercent >= target.HealthPercent || spelldamage >= target.TotalShieldHealth()
                            || caster.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth()
                            || enemy.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth();
                ;

                if (!Player.Instance.IsRecalling() && Player.Instance.IsKillable() && hit && !death)
                {
                    if (Refillablec)
                    {
                        if (target.HealthPercent <= Refillableh || damagepercent >= Refillablen)
                        {
                            Refillable.Cast();
                        }
                    }

                    if (Healthc)
                    {
                        if (target.HealthPercent <= Healthh || damagepercent >= Healthn)
                        {
                            Health.Cast();
                        }
                    }

                    if (Huntersc)
                    {
                        if (target.HealthPercent <= Huntersh || damagepercent >= Huntersn)
                        {
                            Hunters.Cast();
                        }
                    }

                    if (Biscuitc)
                    {
                        if (target.HealthPercent <= Biscuith || damagepercent >= Biscuitn)
                        {
                            Biscuit.Cast();
                        }
                    }

                    if (Corruptingc)
                    {
                        if (target.HealthPercent <= Corruptingh || damagepercent >= Corruptingn)
                        {
                            Corrupting.Cast();
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient) || !args.Target.IsMe || Player.Instance.Buffs.FirstOrDefault(b => PotBuffs.Contains(b.Name)) != null)
            {
                return;
            }

            var caster = sender;
            var player = args.Target as AIHeroClient;
            if (((caster is AIHeroClient && PotMenu["champ"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMinion && PotMenu["mob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Minion && caster.IsMonster && PotMenu["jmob"].Cast<CheckBox>().CurrentValue)
                 || (caster is Obj_AI_Turret && PotMenu["tower"].Cast<CheckBox>().CurrentValue)) && caster.IsEnemy && player != null)
            {
                var aaprecent = (caster.GetAutoAttackDamage(player, true) / player.TotalShieldHealth()) * 100;
                var death = caster.GetAutoAttackDamage(player, true) >= player.TotalShieldHealth() || aaprecent >= player.HealthPercent;

                if (!player.IsRecalling() && Player.Instance.IsKillable() && !death)
                {
                    if (Refillablec)
                    {
                        if (player.HealthPercent <= Refillableh || aaprecent >= Refillablen)
                        {
                            Refillable.Cast();
                        }
                    }

                    if (Healthc)
                    {
                        if (player.HealthPercent <= Healthh || aaprecent >= Healthn)
                        {
                            Health.Cast();
                        }
                    }

                    if (Huntersc)
                    {
                        if (player.HealthPercent <= Huntersh || aaprecent >= Huntersn)
                        {
                            Hunters.Cast();
                        }
                    }

                    if (Biscuitc)
                    {
                        if (player.HealthPercent <= Biscuith || aaprecent >= Biscuitn)
                        {
                            Biscuit.Cast();
                        }
                    }

                    if (Corruptingc)
                    {
                        if (player.HealthPercent <= Corruptingh || aaprecent >= Corruptingn)
                        {
                            Corrupting.Cast();
                        }
                    }
                }
            }
        }
    }
}