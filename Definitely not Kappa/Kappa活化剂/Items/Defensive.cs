namespace KappaUtility.Items
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;

    using Common;

    internal class Defensive
    {
        public static readonly Item Zhonyas = new Item(ItemId.Zhonyas_Hourglass);

        public static readonly Item Seraph = new Item(ItemId.Seraphs_Embrace);

        public static readonly Item FOTM = new Item(ItemId.Face_of_the_Mountain, 600);

        public static readonly Item Solari = new Item(ItemId.Locket_of_the_Iron_Solari, 600);

        public static readonly Item Randuin = new Item(ItemId.Randuins_Omen, 500f);

        public static int Seraphh => DefMenu.GetSlider("Seraphh");

        public static int Solarih => DefMenu.GetSlider("Solarih");

        public static int FaceOfTheMountainh => DefMenu.GetSlider("FaceOfTheMountainh");

        public static int Zhonyash => DefMenu.GetSlider("Zhonyash");

        public static int Seraphn => DefMenu.GetSlider("Seraphn");

        public static int Solarin => DefMenu.GetSlider("Solarin");

        public static int FaceOfTheMountainn => DefMenu.GetSlider("FaceOfTheMountainn");

        public static int Zhonyasn => DefMenu.GetSlider("Zhonyasn");

        public static bool Seraphc => DefMenu.GetCheckbox("Seraph") && Seraph.IsOwned(Player.Instance) && Seraph.IsReady();

        public static bool Solaric => DefMenu.GetCheckbox("Solari") && Solari.IsOwned(Player.Instance) && Solari.IsReady();

        public static bool FaceOfTheMountainc => DefMenu.GetCheckbox("FaceOfTheMountain") && FOTM.IsOwned(Player.Instance) && FOTM.IsReady();

        public static bool Zhonyasc => DefMenu.GetCheckbox("Zhonyas") && Zhonyas.IsOwned(Player.Instance) && Zhonyas.IsReady();

        public static Menu DefMenu { get; private set; }

        protected static bool loaded = false;

        internal static void OnLoad()
        {
            DefMenu = Load.UtliMenu.AddSubMenu("防守物品");
            DefMenu.AddGroupLabel("防守物品设置");
            DefMenu.Checkbox("Zhonyas", "使用中亚");
            DefMenu.Slider("Zhonyash", "血量为X时使用中亚", 35);
            DefMenu.Slider("Zhonyasn", "使用中亚如果预计受到的伤害多于[{0}%]", 50);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("Seraph", "使用炽天使之拥");
            DefMenu.Slider("Seraphh", "血量为X时使用炽天使", 45);
            DefMenu.Slider("Seraphn", "使用炽天使如果预计受到的伤害多于[{0}%]", 45);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("FaceOfTheMountain", "使用崇山圣盾");
            DefMenu.Slider("FaceOfTheMountainh", "血量为X时使用圣盾", 50);
            DefMenu.Slider("FaceOfTheMountainn", "使用圣盾如果预计受到的伤害多于[{0}%]", 50);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("Solari", "使用钢铁烈阳之匣");
            DefMenu.Slider("Solarih", "血量为X时使用铁盒", 30);
            DefMenu.Slider("Solarin", "使用鸟盾如果预计受到的伤害多于 [{0}%]", 35);
            DefMenu.AddSeparator();
            DefMenu.Checkbox("Randuin", "使用兰顿之兆");
            DefMenu.Slider("Randuinh", "X名敌人时使用兰顿", 2, 1, 5);
            DefMenu.AddSeparator();
            DefMenu.AddGroupLabel("金身躲避");
            DefMenu.Checkbox("ZhonyasD", "躲避危险技能");
            DamageHandler.OnLoad();
            Zhonya.OnLoad();
            loaded = true;
        }

        internal static void Items()
        {
            if (!loaded)
            {
                return;
            }

            if (Randuin.IsReady() && Randuin.IsOwned(Player.Instance) && Helpers.CountEnemies((int)Randuin.Range) >= DefMenu.GetSlider("Randuinh")
                && DefMenu.GetCheckbox("Randuin"))
            {
                Randuin.Cast();
            }
        }

        public static void defcast(Obj_AI_Base caster, Obj_AI_Base target, Obj_AI_Base enemy, float dmg)
        {
            var damagepercent = (dmg / target.TotalShieldHealth()) * 100;
            var death = damagepercent >= target.HealthPercent || dmg >= target.TotalShieldHealth();

            if (target.IsValidTarget(Defensive.FOTM.Range) && Defensive.FaceOfTheMountainc)
            {
                if (Defensive.FaceOfTheMountainh >= target.HealthPercent || death || damagepercent >= Defensive.FaceOfTheMountainn)
                {
                    Defensive.FOTM.Cast(target);
                }
            }

            if (target.IsValidTarget(Defensive.Solari.Range) && Defensive.Solaric)
            {
                if (Defensive.Solarih >= target.HealthPercent || death || damagepercent >= Defensive.Solarin)
                {
                    Defensive.Solari.Cast();
                }
            }

            if (target.IsMe)
            {
                if (Defensive.Seraphc)
                {
                    if (Defensive.Seraphh >= target.HealthPercent || death || damagepercent >= Defensive.Seraphn)
                    {
                        Defensive.Seraph.Cast();
                    }
                }

                if (Defensive.Zhonyasc)
                {
                    if (Defensive.Zhonyash >= target.HealthPercent || death || damagepercent >= Defensive.Zhonyasn)
                    {
                        Defensive.Zhonyas.Cast();
                    }
                }
            }
        }
    }
}