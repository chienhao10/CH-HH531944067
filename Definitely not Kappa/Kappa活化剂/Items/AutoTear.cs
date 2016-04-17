namespace KappaUtility.Items
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class AutoTear
    {
        protected static bool loaded = false;

        public static readonly Item Tear = new Item(ItemId.Tear_of_the_Goddess);

        public static readonly Item Tearcs = new Item(ItemId.Tear_of_the_Goddess_Crystal_Scar);

        public static readonly Item Arch = new Item(ItemId.Archangels_Staff);

        public static readonly Item Archcs = new Item(ItemId.Archangels_Staff_Crystal_Scar);

        public static readonly Item Mana = new Item(ItemId.Manamune);

        public static readonly Item Manacs = new Item(ItemId.Manamune_Crystal_Scar);

        public static readonly Item Mura = new Item(ItemId.Muramana);

        public static readonly Item Sera = new Item(ItemId.Seraphs_Embrace);

        public static Menu TearMenu { get; private set; }

        internal static void OnLoad()
        {
            TearMenu = Load.UtliMenu.AddSubMenu("自动叠加女神");
            TearMenu.AddGroupLabel("叠加设置");
            TearMenu.Add(Player.Instance.ChampionName + "enable", new KeyBind("开关按键", false, KeyBind.BindTypes.PressToggle, 'M'));
            TearMenu.Add(Player.Instance.ChampionName + "shop", new CheckBox("商店范围内叠加", false));
            TearMenu.Add(Player.Instance.ChampionName + "enemy", new CheckBox("附近有敌人停止叠加"));
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("蓝量控制器");
            TearMenu.Add(Player.Instance.ChampionName + "manasave", new Slider("保存蓝量 %", 85));
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("物品设置");
            TearMenu.Add(Player.Instance.ChampionName + "tear", new CheckBox("叠加女神"));
            TearMenu.Add(Player.Instance.ChampionName + "arch", new CheckBox("叠加大天使"));
            TearMenu.Add(Player.Instance.ChampionName + "mana", new CheckBox("叠加魔神利刃"));
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("使用技能叠加");
            TearMenu.Add(Player.Instance.ChampionName + "Q", new CheckBox("使用 Q", false));
            TearMenu.Add(Player.Instance.ChampionName + "W", new CheckBox("使用 W", false));
            TearMenu.Add(Player.Instance.ChampionName + "E", new CheckBox("使用 E", false));
            TearMenu.Add(Player.Instance.ChampionName + "R", new CheckBox("使用 R", false));
            loaded = true;
        }

        internal static void OnUpdate()
        {
            if (!loaded)
            {
                return;
            }

            if (TearMenu[Player.Instance.ChampionName + "enable"].Cast<KeyBind>().CurrentValue)
            {
                var items = ((Tearcs.IsOwned() || Tear.IsOwned()) && TearMenu[Player.Instance.ChampionName + "tear"].Cast<CheckBox>().CurrentValue)
                            || ((Arch.IsOwned() || Archcs.IsOwned()) && TearMenu[Player.Instance.ChampionName + "arch"].Cast<CheckBox>().CurrentValue)
                            || ((Manacs.IsOwned() || Mana.IsOwned()) && TearMenu[Player.Instance.ChampionName + "mana"].Cast<CheckBox>().CurrentValue);
                var items2 = Sera.IsOwned() || Mura.IsOwned();
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions;

                foreach (var minion in
                    minions.Where(
                        minion =>
                        items && !items2
                        && Player.Instance.ManaPercent > TearMenu[Player.Instance.ChampionName + "manasave"].Cast<Slider>().CurrentValue))
                {
                    if (TearMenu[Player.Instance.ChampionName + "enemy"].Cast<CheckBox>().CurrentValue
                        && Player.Instance.CountEnemiesInRange(1250) >= 1)
                    {
                        return;
                    }

                    if (TearMenu[Player.Instance.ChampionName + "shop"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange())
                    {
                        return;
                    }

                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) || Player.Instance.Spellbook.IsChanneling)
                    {
                        return;
                    }

                    Cast(minion);
                }
            }
        }

        internal static void Cast(Obj_AI_Base target)
        {
            var useQ = Player.GetSpell(SpellSlot.Q).IsReady && TearMenu[Player.Instance.ChampionName + "Q"].Cast<CheckBox>().CurrentValue;

            var useW = Player.GetSpell(SpellSlot.W).IsReady && TearMenu[Player.Instance.ChampionName + "W"].Cast<CheckBox>().CurrentValue;

            var useE = Player.GetSpell(SpellSlot.E).IsReady && TearMenu[Player.Instance.ChampionName + "E"].Cast<CheckBox>().CurrentValue;

            var useR = Player.GetSpell(SpellSlot.R).IsReady && TearMenu[Player.Instance.ChampionName + "R"].Cast<CheckBox>().CurrentValue;
            if (useQ)
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
            }

            if (useW)
            {
                Player.CastSpell(SpellSlot.W, Game.CursorPos);
            }

            if (useE)
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }

            if (useR)
            {
                Player.CastSpell(SpellSlot.R, Game.CursorPos);
            }
        }
    }
}