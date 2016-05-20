namespace KappaUtility.Items
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Common;

    internal class Offensive
    {
        public static readonly Item Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 250f);

        public static readonly Item Titanic = new Item(ItemId.Titanic_Hydra, Player.Instance.GetAutoAttackRange());

        public static readonly Item Timat = new Item(ItemId.Tiamat_Melee_Only, 250f);

        public static readonly Item Cutlass = new Item((int)ItemId.Bilgewater_Cutlass, 550);

        public static readonly Item Botrk = new Item((int)ItemId.Blade_of_the_Ruined_King, 550);

        public static readonly Item Youmuu = new Item((int)ItemId.Youmuus_Ghostblade);

        protected static readonly Item Gunblade = new Item(ItemId.Hextech_Gunblade, 700f);

        protected static readonly Item ProtoBelt = new Item(ItemId.Will_of_the_Ancients, 600);

        protected static readonly Item GLP = new Item(3030, 600);

        public static Menu OffMenu { get; private set; }

        protected static bool loaded = false;

        internal static void OnLoad()
        {
            OffMenu = Load.UtliMenu.AddSubMenu("进攻物品");
            OffMenu.AddGroupLabel("进攻物品设置");
            OffMenu.Checkbox("Hydra", "使用九头 / 提亚马特 / 泰坦");
            OffMenu.Checkbox("useGhostblade", " 使用幽梦");
            OffMenu.Checkbox("UseBOTRK", " 使用破败");
            OffMenu.Checkbox("UseBilge", " 使用弯刀");
            OffMenu.Checkbox("UseGunblade", " 使用科技枪");
            OffMenu.Checkbox("UseBelt", " 使用 海克斯科技原型机腰带-01");
            OffMenu.Checkbox("UseGLP", " 使用 海克斯科技 GLP-800");
            OffMenu.AddSeparator();
            OffMenu.AddGroupLabel("设置");
            OffMenu.Checkbox("UseKS", " 抢头使用");
            OffMenu.Checkbox("UseCombo", " 连招使用");
            OffMenu.Slider("eL", "敌方血量 X 时使用", 65);
            OffMenu.Slider("oL", "自身血量 X 时使用", 65);

            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            loaded = true;
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!loaded)
            {
                return;
            }
            if (!target.IsEnemy || !(target is AIHeroClient))
            {
                return;
            }

            var useHydra = OffMenu["Hydra"].Cast<CheckBox>().CurrentValue
                           && ((Hydra.IsOwned(Player.Instance) && Hydra.IsReady()) || (Timat.IsOwned(Player.Instance) && Timat.IsReady())
                               || (Titanic.IsOwned(Player.Instance) && Titanic.IsReady()));
            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Youmuu.IsReady() && Youmuu.IsOwned(Player.Instance) && target.IsValidTarget(500) && OffMenu.GetCheckbox("useGhostblade"))
                {
                    Youmuu.Cast();
                }
                if (useHydra)
                {
                    if (Hydra.IsOwned() && Hydra.IsReady() && Hydra != null)
                    {
                        if (Hydra.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                        }
                    }

                    if (Timat.IsOwned() && Timat.IsReady() && Timat != null)
                    {
                        if (Timat.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                        }
                    }
                    if (Titanic.IsOwned() && Titanic.IsReady() && Titanic != null)
                    {
                        if (Titanic.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                        }
                    }
                }
            }
        }

        internal static void Items()
        {
            if (!loaded)
            {
                return;
            }

            if (OffMenu["UseKS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy != null && enemy.IsKillable() && enemy.IsValidTarget(600))
                    {
                        if (OffMenu.GetCheckbox("UseGunblade") && Player.Instance.GetItemDamage(enemy, ItemId.Hextech_Gunblade) >= enemy.Health)
                        {
                            Gunblade.Cast(enemy);
                        }
                        if (OffMenu.GetCheckbox("UseBOTRK") && Player.Instance.GetItemDamage(enemy, ItemId.Blade_of_the_Ruined_King) >= enemy.Health)
                        {
                            Botrk.Cast(enemy);
                        }
                        if (OffMenu.GetCheckbox("UseBilge") && Player.Instance.GetItemDamage(enemy, ItemId.Bilgewater_Cutlass) >= enemy.Health)
                        {
                            Cutlass.Cast(enemy);
                        }
                        if (OffMenu.GetCheckbox("UseBelt") && Player.Instance.GetItemDamage(enemy, ItemId.Will_of_the_Ancients) >= enemy.Health)
                        {
                            ProtoBelt.Cast(enemy.ServerPosition);
                        }
                        if (OffMenu.GetCheckbox("UseGLP") && Player.Instance.GetItemDamage(enemy, GLP.Id) >= enemy.Health)
                        {
                            GLP.Cast(enemy.ServerPosition);
                        }
                        if (OffMenu.GetCheckbox("Hydra"))
                        {
                            if (Hydra.IsOwned(Player.Instance) && Hydra.IsReady() && Player.Instance.GetItemDamage(enemy, Hydra.Id) >= enemy.Health)
                            {
                                Hydra.Cast();
                            }
                            if (Timat.IsOwned(Player.Instance) && Timat.IsReady() && Player.Instance.GetItemDamage(enemy, Timat.Id) >= enemy.Health)
                            {
                                Timat.Cast();
                            }
                            if (Titanic.IsOwned(Player.Instance) && Titanic.IsReady()
                                && Player.Instance.GetItemDamage(enemy, Titanic.Id) >= enemy.Health)
                            {
                                Titanic.Cast();
                            }
                        }
                    }
                }
            }

            var target = TargetSelector.GetTarget(600, DamageType.Physical);
            if (target == null || !target.IsValidTarget() || !OffMenu.GetCheckbox("UseCombo"))
            {
                return;
            }

            if (target.HealthPercent <= OffMenu.GetSlider("eL") || Player.Instance.HealthPercent <= OffMenu.GetSlider("oL"))
            {
                if (Gunblade.IsReady() && Gunblade.IsOwned(Player.Instance) && target.IsValidTarget(Gunblade.Range)
                    && OffMenu.GetCheckbox("UseGunblade"))
                {
                    Gunblade.Cast(target);
                }

                if (Botrk.IsReady() && Botrk.IsOwned(Player.Instance) && target.IsValidTarget(Botrk.Range) && OffMenu.GetCheckbox("UseBOTRK"))
                {
                    Botrk.Cast(target);
                }

                if (Cutlass.IsReady() && Cutlass.IsOwned(Player.Instance) && target.IsValidTarget(Cutlass.Range) && OffMenu.GetCheckbox("UseBilge"))
                {
                    Cutlass.Cast(target);
                }

                if (ProtoBelt.IsOwned(Player.Instance) && ProtoBelt.IsReady() && OffMenu.GetCheckbox("UseBelt"))
                {
                    ProtoBelt.Cast(target.ServerPosition);
                }

                if (GLP.IsOwned(Player.Instance) && GLP.IsReady() && OffMenu.GetCheckbox("UseGLP"))
                {
                    GLP.Cast(target.ServerPosition);
                }
            }
        }
    }
}