namespace KappaUtility.Items
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;

    using Common;

    internal class AutoQSS
    {
        public static Spell.Active Cleanse;

        protected static readonly Item Mercurial_Scimitar = new Item(ItemId.Mercurial_Scimitar);

        protected static readonly Item Quicksilver_Sash = new Item(ItemId.Quicksilver_Sash);

        public static readonly Item Dervish_Blade = new Item(ItemId.Dervish_Blade);

        public static readonly Item Mikaels_Crucible = new Item(ItemId.Mikaels_Crucible);

        protected static bool loaded = false;

        public static Menu QssMenu { get; private set; }

        internal static void OnLoad()
        {
            QssMenu = Load.UtliMenu.AddSubMenu("自动净化");
            QssMenu.AddGroupLabel("自动净化设置");
            QssMenu.Checkbox("enable", "开启");
            QssMenu.Checkbox("Mercurial", "使用水银弯刀");
            QssMenu.Checkbox("Quicksilver", "使用水银饰带");
            QssMenu.Checkbox("Dervish_Blade", "使用净化刀");
            QssMenu.Checkbox("Mikaels_Crucible", "使用米凯尔");
            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerBoost")) != null)
            {
                QssMenu.Checkbox("Cleanse", "使用净化技能");
                Cleanse = new Spell.Active(Player.Instance.GetSpellSlotFromName("SummonerBoost"));
            }

            QssMenu.AddSeparator();
            QssMenu.AddGroupLabel("状态解除设置:");
            QssMenu.Checkbox("blind", "致盲");
            QssMenu.Checkbox("charm", "魅惑");
            QssMenu.Checkbox("disarm", "无力");
            QssMenu.Checkbox("fear", "恐惧");
            QssMenu.Checkbox("frenzy", "狂怒");
            QssMenu.Checkbox("silence", "沉默");
            QssMenu.Checkbox("snare", "监禁");
            QssMenu.Checkbox("sleep", "睡眠");
            QssMenu.Checkbox("stun", "晕眩");
            QssMenu.Checkbox("supperss", "压制");
            QssMenu.Checkbox("slow", "减速");
            QssMenu.Checkbox("knockup", "击飞");
            QssMenu.Checkbox("knockback", "击退");
            QssMenu.Checkbox("nearsight", "事业丢失");
            QssMenu.Checkbox("root", "禁锢");
            QssMenu.Checkbox("tunt", "U嘲讽");
            QssMenu.Checkbox("poly", "变形");
            QssMenu.Checkbox("poison", "中毒");

            QssMenu.AddSeparator();
            QssMenu.AddGroupLabel("大招解除设置:");
            QssMenu.Checkbox("liss", "丽桑卓R");
            QssMenu.Checkbox("naut", "泰坦R");
            QssMenu.Checkbox("zed", "劫R");
            QssMenu.Checkbox("vlad", "吸血鬼R");
            QssMenu.Checkbox("fizz", "小鱼人R");
            QssMenu.Checkbox("fiora", "剑姬R");
            QssMenu.AddSeparator();
            QssMenu.Slider("hp", "只在血量低于 X %时使用", 30);
            QssMenu.Slider("human", "人性化延迟", 150, 0, 1500);
            QssMenu.Slider("Rene", "附近敌方 X 数量使用", 1, 0, 5);
            QssMenu.Slider("enemydetect", "检测敌方数量 X 范围", 1000, 0, 2000);
            loaded = true;

            Obj_AI_Base.OnBuffGain += OnBuffGain;
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!loaded)
            {
                return;
            }

            if (QssMenu.GetCheckbox("enable"))
            {
                if (sender.IsMe)
                {
                    var debuff = (QssMenu.GetCheckbox("charm") && (args.Buff.Type == BuffType.Charm || Player.Instance.HasBuffOfType(BuffType.Charm)))
                                 || (QssMenu.GetCheckbox("tunt")
                                     && (args.Buff.Type == BuffType.Taunt || Player.Instance.HasBuffOfType(BuffType.Taunt)))
                                 || (QssMenu.GetCheckbox("stun") && (args.Buff.Type == BuffType.Stun || Player.Instance.HasBuffOfType(BuffType.Stun)))
                                 || (QssMenu.GetCheckbox("fear") && (args.Buff.Type == BuffType.Fear || Player.Instance.HasBuffOfType(BuffType.Fear)))
                                 || (QssMenu.GetCheckbox("silence")
                                     && (args.Buff.Type == BuffType.Silence || Player.Instance.HasBuffOfType(BuffType.Silence)))
                                 || (QssMenu.GetCheckbox("snare")
                                     && (args.Buff.Type == BuffType.Snare || Player.Instance.HasBuffOfType(BuffType.Snare)))
                                 || (QssMenu.GetCheckbox("supperss")
                                     && (args.Buff.Type == BuffType.Suppression || Player.Instance.HasBuffOfType(BuffType.Suppression)))
                                 || (QssMenu.GetCheckbox("sleep")
                                     && (args.Buff.Type == BuffType.Sleep || Player.Instance.HasBuffOfType(BuffType.Sleep)))
                                 || (QssMenu.GetCheckbox("poly")
                                     && (args.Buff.Type == BuffType.Polymorph || Player.Instance.HasBuffOfType(BuffType.Polymorph)))
                                 || (QssMenu.GetCheckbox("frenzy")
                                     && (args.Buff.Type == BuffType.Frenzy || Player.Instance.HasBuffOfType(BuffType.Frenzy)))
                                 || (QssMenu.GetCheckbox("disarm")
                                     && (args.Buff.Type == BuffType.Disarm || Player.Instance.HasBuffOfType(BuffType.Disarm)))
                                 || (QssMenu.GetCheckbox("nearsight")
                                     && (args.Buff.Type == BuffType.NearSight || Player.Instance.HasBuffOfType(BuffType.NearSight)))
                                 || (QssMenu.GetCheckbox("knockback")
                                     && (args.Buff.Type == BuffType.Knockback || Player.Instance.HasBuffOfType(BuffType.Knockback)))
                                 || (QssMenu.GetCheckbox("knockup")
                                     && (args.Buff.Type == BuffType.Knockup || Player.Instance.HasBuffOfType(BuffType.Knockup)))
                                 || (QssMenu.GetCheckbox("slow") && (args.Buff.Type == BuffType.Slow || Player.Instance.HasBuffOfType(BuffType.Slow)))
                                 || (QssMenu.GetCheckbox("poison")
                                     && (args.Buff.Type == BuffType.Poison || Player.Instance.HasBuffOfType(BuffType.Poison)))
                                 || (QssMenu.GetCheckbox("blind")
                                     && (args.Buff.Type == BuffType.Blind || Player.Instance.HasBuffOfType(BuffType.Blind)))
                                 || (QssMenu.GetCheckbox("zed") && args.Buff.Name == "zedrtargetmark")
                                 || (QssMenu.GetCheckbox("vlad") && args.Buff.Name == "vladimirhemoplaguedebuff")
                                 || (QssMenu.GetCheckbox("liss") && args.Buff.Name == "LissandraREnemy2")
                                 || (QssMenu.GetCheckbox("fizz") && args.Buff.Name == "fizzmarinerdoombomb")
                                 || (QssMenu.GetCheckbox("naut") && args.Buff.Name == "nautilusgrandlinetarget")
                                 || (QssMenu.GetCheckbox("fiora") && args.Buff.Name == "fiorarmark");
                    var enemys = QssMenu.GetSlider("Rene");
                    var hp = QssMenu.GetSlider("hp");
                    var enemysrange = QssMenu.GetSlider("enemydetect");
                    var countenemies = Helpers.CountEnemies(enemysrange);
                    var delay = QssMenu.GetSlider("human");
                    if (debuff && Player.Instance.HealthPercent <= hp && countenemies >= enemys)
                    {
                        Core.DelayAction(QssCast, delay);
                    }
                }
            }
        }

        public static void QssCast()
        {
            if (Quicksilver_Sash.IsOwned() && Quicksilver_Sash.IsReady() && QssMenu.GetCheckbox("Quicksilver"))
            {
                Quicksilver_Sash.Cast();
            }

            if (Mercurial_Scimitar.IsOwned() && Mercurial_Scimitar.IsReady() && QssMenu.GetCheckbox("Mercurial"))
            {
                Mercurial_Scimitar.Cast();
            }

            if (Dervish_Blade.IsOwned() && Dervish_Blade.IsReady() && QssMenu.GetCheckbox("Dervish_Blade"))
            {
                Dervish_Blade.Cast();
            }

            if (Cleanse != null)
            {
                if (QssMenu.GetCheckbox("Cleanse") && Cleanse.IsReady())
                {
                    Cleanse.Cast();
                }
            }
        }
    }
}
