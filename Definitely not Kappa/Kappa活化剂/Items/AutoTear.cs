namespace KappaUtility.Items
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Common;

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
            TearMenu.Checkbox("shop", "商店范围内叠加");
            TearMenu.Checkbox("enemy", "附近有敌人停止叠加");
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("蓝量控制器");
            TearMenu.Slider("manasave", "保存蓝量%", 85);
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("物品设置");
            TearMenu.Checkbox("tear", "叠加女神");
            TearMenu.Checkbox("arch", "叠加大天使");
            TearMenu.Checkbox("mana", "叠加魔神利刃");
            TearMenu.AddSeparator();
            TearMenu.AddGroupLabel("使用技能叠加");
            TearMenu.Checkbox(Player.Instance.ChampionName + "Q", "使用 Q");
            TearMenu.Checkbox(Player.Instance.ChampionName + "W", "使用 W");
            TearMenu.Checkbox(Player.Instance.ChampionName + "E", "使用 E");
            TearMenu.Checkbox(Player.Instance.ChampionName + "R", "使用 R");
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
                var items = ((Tearcs.IsOwned(Player.Instance) || Tear.IsOwned(Player.Instance)) && TearMenu.GetCheckbox("tear"))
                            || ((Arch.IsOwned() || Archcs.IsOwned()) && TearMenu.GetCheckbox("arch"))
                            || ((Manacs.IsOwned() || Mana.IsOwned()) && TearMenu.GetCheckbox("mana"));
                var items2 = Sera.IsOwned() || Mura.IsOwned();
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions;

                foreach (var minion in
                    minions.Where(minion => items && !items2 && Player.Instance.ManaPercent > TearMenu.GetSlider("manasave")))
                {
                    if (TearMenu.GetCheckbox("enemy") && Helpers.CountEnemies(1250) >= 1)
                    {
                        return;
                    }

                    if (TearMenu.GetCheckbox("shop") && !Player.Instance.IsInShopRange())
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
            var useQ = Player.GetSpell(SpellSlot.Q).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "Q");

            var useW = Player.GetSpell(SpellSlot.W).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "W");

            var useE = Player.GetSpell(SpellSlot.E).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "E");

            var useR = Player.GetSpell(SpellSlot.R).IsReady && TearMenu.GetCheckbox(Player.Instance.ChampionName + "R");
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