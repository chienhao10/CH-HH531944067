namespace KappaUtility.Items
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class AutoQSS
    {
        public static Spell.Active Cleanse;

        protected static readonly Item Mercurial_Scimitar = new Item(ItemId.Mercurial_Scimitar);

        protected static readonly Item Quicksilver_Sash = new Item(ItemId.Quicksilver_Sash);

        public static Menu QssMenu { get; private set; }

        internal static void OnLoad()
        {
            QssMenu = Load.UtliMenu.AddSubMenu("自动净化");
            QssMenu.AddGroupLabel("自动净化设置");
            QssMenu.Add("enable", new CheckBox("开启", false));
            QssMenu.Add("Mercurial", new CheckBox("使用水银弯刀", false));
            QssMenu.Add("Quicksilver", new CheckBox("使用水银饰带", false));
            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerBoost")) != null)
            {
                QssMenu.Add("Cleanse", new CheckBox("Use Cleanse Spell", false));
                Cleanse = new Spell.Active(Player.Instance.GetSpellSlotFromName("SummonerBoost"));
            }

            QssMenu.AddSeparator();
            QssMenu.AddGroupLabel("状态解除设置:");
            QssMenu.Add("blind", new CheckBox("致盲", false));
            QssMenu.Add("charm", new CheckBox("魅惑", false));
            QssMenu.Add("disarm", new CheckBox("无力", false));
            QssMenu.Add("fear", new CheckBox("恐惧", false));
            QssMenu.Add("frenzy", new CheckBox("狂怒", false));
            QssMenu.Add("silence", new CheckBox("沉默", false));
            QssMenu.Add("snare", new CheckBox("监禁", false));
            QssMenu.Add("sleep", new CheckBox("睡眠", false));
            QssMenu.Add("stun", new CheckBox("晕眩", false));
            QssMenu.Add("supperss", new CheckBox("压制", false));
            QssMenu.Add("slow", new CheckBox("减速", false));
            QssMenu.Add("knockup", new CheckBox("击飞", false));
            QssMenu.Add("knockback", new CheckBox("击退", false));
            QssMenu.Add("nearsight", new CheckBox("视野丢失", false));
            QssMenu.Add("root", new CheckBox("禁锢", false));
            QssMenu.Add("tunt", new CheckBox("嘲讽", false));
            QssMenu.Add("poly", new CheckBox("变形", false));
            QssMenu.Add("poison", new CheckBox("中毒", false));

            QssMenu.AddSeparator();
            QssMenu.AddGroupLabel("大招解除设置:");
            QssMenu.Add("liss", new CheckBox("丽桑卓R", false));
            QssMenu.Add("naut", new CheckBox("泰坦R", false));
            QssMenu.Add("zed", new CheckBox("劫R", false));
            QssMenu.Add("vlad", new CheckBox("吸血鬼R", false));
            QssMenu.Add("fizz", new CheckBox("小鱼人R", false));
            QssMenu.Add("fiora", new CheckBox("剑姬R", false));
            QssMenu.AddSeparator();
            QssMenu.Add("hp", new Slider("只在血量低于 X %时使用", 25, 0, 100));
            QssMenu.Add("human", new Slider("人性化延迟", 150, 0, 1500));
            QssMenu.Add("Rene", new Slider("附近敌方数量使用", 1, 0, 5));
            QssMenu.Add("enemydetect", new Slider("检测敌方数量范围", 1000, 0, 2000));
            Obj_AI_Base.OnBuffGain += OnBuffGain;
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (QssMenu["enable"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.IsMe)
                {
                    var debuff = (QssMenu["charm"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Charm)
                                 || (QssMenu["tunt"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Taunt)
                                 || (QssMenu["stun"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Stun)
                                 || (QssMenu["fear"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Fear)
                                 || (QssMenu["silence"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Silence)
                                 || (QssMenu["snare"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Snare)
                                 || (QssMenu["supperss"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Suppression)
                                 || (QssMenu["sleep"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Sleep)
                                 || (QssMenu["poly"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Polymorph)
                                 || (QssMenu["frenzy"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Frenzy)
                                 || (QssMenu["disarm"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Disarm)
                                 || (QssMenu["nearsight"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.NearSight)
                                 || (QssMenu["knockback"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Knockback)
                                 || (QssMenu["knockup"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Knockup)
                                 || (QssMenu["slow"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Slow)
                                 || (QssMenu["poison"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Type == BuffType.Poison)
                                 || (QssMenu["blind"].Cast<CheckBox>().CurrentValue && args.Buff.Type == BuffType.Blind)
                                 || (QssMenu["zed"].Cast<CheckBox>().CurrentValue && args.Buff.Name == "zedrtargetmark")
                                 || (QssMenu["vlad"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Name == "vladimirhemoplaguedebuff")
                                 || (QssMenu["liss"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Name == "LissandraREnemy2")
                                 || (QssMenu["fizz"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Name == "fizzmarinerdoombomb")
                                 || (QssMenu["naut"].Cast<CheckBox>().CurrentValue
                                     && args.Buff.Name == "nautilusgrandlinetarget")
                                 || (QssMenu["fiora"].Cast<CheckBox>().CurrentValue && args.Buff.Name == "fiorarmark");
                    var enemys = QssMenu["Rene"].Cast<Slider>().CurrentValue;
                    var hp = QssMenu["hp"].Cast<Slider>().CurrentValue;
                    var enemysrange = QssMenu["enemydetect"].Cast<Slider>().CurrentValue;
                    var delay = QssMenu["human"].Cast<Slider>().CurrentValue;
                    if (debuff && Player.Instance.HealthPercent <= hp
                        && enemys >= Player.Instance.Position.CountEnemiesInRange(enemysrange))
                    {
                        Core.DelayAction(QssCast, delay);
                    }
                }
            }
        }

        public static void QssCast()
        {
                if (Quicksilver_Sash.IsOwned() && Quicksilver_Sash.IsReady()
                    && QssMenu["Quicksilver"].Cast<CheckBox>().CurrentValue)
                {
                    Quicksilver_Sash.Cast();
                }

                if (Mercurial_Scimitar.IsOwned() && Mercurial_Scimitar.IsReady()
                    && QssMenu["Mercurial"].Cast<CheckBox>().CurrentValue)
                {
                    Mercurial_Scimitar.Cast();
                }

            if (Cleanse != null)
            {
                if (QssMenu["Cleanse"].Cast<CheckBox>().CurrentValue && Cleanse.IsReady())
                {
                    Cleanse.Cast();
                }
            }
        }
    }
}