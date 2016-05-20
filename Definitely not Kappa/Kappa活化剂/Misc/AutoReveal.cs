namespace KappaUtility.Misc
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class AutoReveal
    {
        public static readonly Item Sweeping_Lens_Trinket = new Item(ItemId.Sweeping_Lens_Trinket, 550f);

        public static readonly Item Oracle_Alteration = new Item(ItemId.Oracle_Alteration, 550f);

        public static readonly Item Vision_Ward = new Item(ItemId.Vision_Ward, 550f);

        public static readonly Item Warding_Totem_Trinket = new Item(ItemId.Warding_Totem_Trinket, 550f);

        public static readonly Item Greater_Stealth_Totem_Trinket = new Item(ItemId.Greater_Stealth_Totem_Trinket, 550f);

        public static readonly Item Greater_Vision_Totem_Trinket = new Item(ItemId.Greater_Vision_Totem_Trinket, 550f);

        public static readonly Item Sightstone = new Item((int)ItemId.Sightstone, 550f);

        public static readonly Item Ruby_Sightstone = new Item((int)ItemId.Ruby_Sightstone, 550f);

        public static readonly Item Farsight_Alteration = new Item((int)ItemId.Farsight_Alteration, 1250);

        internal class AkaliSmoke
        {
            public GameObject Object { get; set; }

            public Vector3 Position { get; set; }
        }

        internal class Shaco
        {
            public GameObject Object { get; set; }

            public Vector3 Position { get; set; }
        }

        internal class Talon
        {
            public GameObject Object { get; set; }

            public Vector3 Position { get; set; }
        }

        internal class Rengar
        {
            public GameObject Object { get; set; }

            public Vector3 Position { get; set; }
        }

        internal class KhaZix
        {
            public GameObject Object { get; set; }

            public Vector3 Position { get; set; }
        }

        internal class Twitch
        {
            public GameObject Object { get; set; }

            public Vector3 Position { get; set; }
        }

        internal class Vayne
        {
            public GameObject Object { get; set; }

            public Vector3 Position { get; set; }
        }

        private static readonly AkaliSmoke Akalismoke = new AkaliSmoke();

        private static readonly Shaco shaco = new Shaco();

        private static readonly Talon talon = new Talon();

        private static readonly Rengar rengar = new Rengar();

        private static readonly KhaZix khazix = new KhaZix();

        private static readonly Vayne vayne = new Vayne();

        private static readonly Twitch twitch = new Twitch();

        public static int LastTickTime;

        public static Menu BushMenu { get; private set; }

        private static bool DontWard, Changing;

        internal static void OnLoad()
        {
            BushMenu = Load.UtliMenu.AddSubMenu("自动放眼");
            BushMenu.AddGroupLabel("自动放眼设置");
            BushMenu.Add("enable", new CheckBox("开启", false));
            BushMenu.Add("combo", new CheckBox("连招中使用", false));
            BushMenu.AddSeparator();
            BushMenu.AddGroupLabel("插眼隐身单位");
            BushMenu.Add("enables", new CheckBox("开启", false));
            BushMenu.Add("combos", new CheckBox("连招中使用", false));
            BushMenu.AddGroupLabel("选择显示英雄:");
            BushMenu.Add("akali", new CheckBox("阿卡丽", false));
            BushMenu.Add("shaco", new CheckBox("小丑", false));
            BushMenu.Add("rengar", new CheckBox("狮子狗", false));
            BushMenu.Add("talon", new CheckBox("泰隆", false));
            BushMenu.Add("twitch", new CheckBox("瘟疫鼠", false));
            BushMenu.Add("khazix", new CheckBox("螳螂", false));
            BushMenu.Add("vayne", new CheckBox("薇恩", false));

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        internal static void Drawings()
        {
            var targets = EntityManager.Heroes.Enemies.Where(x => !x.IsDead);
            foreach (var target in targets)
            {
                switch (target.BaseSkinName)
                {
                    case "Akali":
                        if (BushMenu["akali"].Cast<CheckBox>().CurrentValue)
                        {
                            if (Akalismoke.Object != null)
                            {
                                if (!target.IsVisible && target.Path.LastOrDefault().IsInRange(Player.Instance, 550))
                                {
                                    RevealCast();
                                }

                                Circle.Draw(Color.White, target.MoveSpeed, target.Position);
                                Drawing.DrawText(
                                    Drawing.WorldToScreen(target.Path.LastOrDefault()) - new Vector2(30, -30),
                                    System.Drawing.Color.White,
                                    "阿卡丽在附近",
                                    2);
                            }
                        }

                        break;

                    case "Shaco":
                        if (BushMenu["shaco"].Cast<CheckBox>().CurrentValue)
                        {
                            if (shaco.Object != null)
                            {
                                if (!target.IsVisible && target.Path.LastOrDefault().IsInRange(Player.Instance, 550))
                                {
                                    RevealCast();
                                }

                                Drawing.DrawText(
                                    Drawing.WorldToScreen(target.Path.LastOrDefault()) - new Vector2(30, -30),
                                    System.Drawing.Color.White,
                                    "小丑在附近",
                                    12);
                                Circle.Draw(Color.White, target.MoveSpeed, target.Position);
                            }
                        }

                        break;

                    case "Talon":
                        if (BushMenu["talon"].Cast<CheckBox>().CurrentValue)
                        {
                            if (talon.Object != null)
                            {
                                if (!target.IsVisible && target.Path.LastOrDefault().IsInRange(Player.Instance, 550))
                                {
                                    RevealCast();
                                }

                                Drawing.DrawText(
                                    Drawing.WorldToScreen(target.Path.LastOrDefault()) - new Vector2(30, -30),
                                    System.Drawing.Color.White,
                                    "泰隆在附近",
                                    12);
                                Circle.Draw(Color.White, target.MoveSpeed, target.Position);
                            }
                        }

                        break;

                    case "Rengar":
                        if (BushMenu["rengar"].Cast<CheckBox>().CurrentValue)
                        {
                            if (rengar.Object != null)
                            {
                                if (!target.IsVisible && target.Path.LastOrDefault().IsInRange(Player.Instance, 550))
                                {
                                    RevealCast();
                                }

                                Drawing.DrawText(
                                    Drawing.WorldToScreen(target.Path.LastOrDefault()) - new Vector2(30, -30),
                                    System.Drawing.Color.White,
                                    "狮子狗在附近",
                                    12);
                                Circle.Draw(Color.White, target.MoveSpeed, target.Position);
                            }
                        }

                        break;

                    case "KhaZix":
                        if (BushMenu["khazix"].Cast<CheckBox>().CurrentValue)
                        {
                            if (khazix.Object != null)
                            {
                                if (!target.IsVisible && target.Path.LastOrDefault().IsInRange(Player.Instance, 550))
                                {
                                    RevealCast();
                                }

                                Drawing.DrawText(
                                    Drawing.WorldToScreen(target.Path.LastOrDefault()) - new Vector2(30, -30),
                                    System.Drawing.Color.White,
                                    "螳螂在附近",
                                    12);
                                Circle.Draw(Color.White, target.MoveSpeed, target.Position);
                            }
                        }

                        break;

                    case "Twitch":
                        if (BushMenu["twitch"].Cast<CheckBox>().CurrentValue)
                        {
                            if (twitch.Object != null)
                            {
                                if (!target.IsVisible && target.Path.LastOrDefault().IsInRange(Player.Instance, 550))
                                {
                                    RevealCast();
                                }

                                Drawing.DrawText(
                                    Drawing.WorldToScreen(target.Path.LastOrDefault()) - new Vector2(30, -30),
                                    System.Drawing.Color.White,
                                    "老鼠在附近",
                                    12);
                                Circle.Draw(Color.White, target.MoveSpeed, target.Position);
                            }
                        }

                        break;

                    case "Vayne":
                        if (BushMenu["vayne"].Cast<CheckBox>().CurrentValue)
                        {
                            if (vayne.Object != null)
                            {
                                if (!target.IsVisible && target.Path.LastOrDefault().IsInRange(Player.Instance, 550))
                                {
                                    RevealCast();
                                }

                                Drawing.DrawText(
                                    Drawing.WorldToScreen(target.Path.LastOrDefault()) - new Vector2(30, -30),
                                    System.Drawing.Color.White,
                                    "薇恩在附近",
                                    12);
                                Circle.Draw(Color.White, target.MoveSpeed, target.Position);
                            }
                        }

                        break;
                }
            }
        }

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj != null && obj.Name.Contains("Akali_Base_smoke_bomb_tar"))
            {
                Akalismoke.Object = obj;
                Akalismoke.Position = obj.Position;
            }

            if (obj != null && obj.Name.Contains("JackintheboxPoof2"))
            {
                shaco.Object = obj;
                shaco.Position = obj.Position;
            }

            if (obj != null && obj.Name.Contains("talon_ult_sound"))
            {
                talon.Object = obj;
                talon.Position = obj.Position;
            }

            if (obj != null && obj.Name.Contains("Rengar_Base_R_Alert"))
            {
                rengar.Object = obj;
                rengar.Position = obj.Position;
            }

            if (obj != null && obj.Name.ToLower().Contains("khazix_base_r_cas"))
            {
                khazix.Object = obj;
                khazix.Position = obj.Position;
            }

            if (obj != null && obj.Name.ToLower().Contains("twitch_base_q_invisiible_outro"))
            {
                twitch.Object = obj;
                twitch.Position = obj.Position;
            }

            if (obj != null && obj.Name.ToLower().Contains("vayne_base_r_cas_invisible"))
            {
                vayne.Object = obj;
                vayne.Position = obj.Position;
            }
        }

        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj != null && obj.Name.Contains("Akali_Base_smoke_bomb_tar"))
            {
                Akalismoke.Object = null;
                LastTickTime = 0;
            }

            if (obj != null && obj.Name.Contains("JackintheboxPoof2"))
            {
                shaco.Object = null;
                LastTickTime = 0;
            }

            if (obj != null && obj.Name.Contains("talon_ult_sound"))
            {
                talon.Object = null;
                LastTickTime = 0;
            }

            if (obj != null && obj.Name.Contains("Rengar_Base_R_Alert"))
            {
                rengar.Object = null;
                LastTickTime = 0;
            }

            if (obj != null && obj.Name.ToLower().Contains("khazix_base_r_cas"))
            {
                khazix.Object = null;
                LastTickTime = 0;
            }

            if (obj != null && obj.Name.ToLower().Contains("twitch_base_q_invisiible_outro"))
            {
                twitch.Object = null;
                LastTickTime = 0;
            }

            if (obj != null && obj.Name.ToLower().Contains("vayne_base_q_cas"))
            {
                vayne.Object = null;
                LastTickTime = 0;
            }
        }

        public static void Reveal()
        {
            var enemies = EntityManager.Heroes.Enemies.Where(x => !x.IsDead && x.Distance(Player.Instance) < 1250);
            if (BushMenu["enable"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsDead)
            {
                var flags = Orbwalker.ActiveModesFlags;
                foreach (var target in enemies)
                {
                    if (!BushMenu["combo"].Cast<CheckBox>().CurrentValue)
                    {
                        if (NavMesh.IsWallOfGrass(target.Path.LastOrDefault(), 1)
                            && !NavMesh.IsWallOfGrass(Player.Instance.Position, 1)
                            && Player.Instance.Distance(target.Path.LastOrDefault()) < 500)
                        {
                            var target1 = target;
                            var wards =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        x =>
                                        x.Name.Contains("Ward") && x.IsAlly
                                        && x.Position.Distance(target1.Position) < 750);

                            foreach (var ward in
                                wards.Where(
                                    ward => (NavMesh.IsWallOfGrass(ward.Position, 50) && ward.Distance(target1) < 750)))
                            {
                                DontWard = true;
                            }

                            if (!DontWard)
                            {
                                WardCast();
                            }

                            if (DontWard && !Changing)
                            {
                                Changing = true;
                                Core.DelayAction(
                                    delegate
                                        {
                                            DontWard = false;
                                            Changing = false;
                                        },
                                    500);
                            }
                        }
                    }

                    if (BushMenu["combo"].Cast<CheckBox>().CurrentValue && flags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        if (NavMesh.IsWallOfGrass(target.Path.LastOrDefault(), 1)
                            && !NavMesh.IsWallOfGrass(Player.Instance.Position, 1)
                            && Player.Instance.Distance(target.Path.LastOrDefault()) < 500)
                        {
                            var target1 = target;
                            var wards =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        x =>
                                        x.Name.Contains("Ward") && x.IsAlly
                                        && x.Position.Distance(target1.Position) < 750);

                            foreach (var ward in
                                wards.Where(
                                    ward => NavMesh.IsWallOfGrass(ward.Position, 50) && ward.Distance(target1) < 750))
                            {
                                DontWard = true;
                            }

                            if (!DontWard)
                            {
                                WardCast();
                            }

                            if (DontWard && !Changing)
                            {
                                Changing = true;
                                Core.DelayAction(
                                    delegate
                                        {
                                            DontWard = false;
                                            Changing = false;
                                        },
                                    500);
                            }
                        }
                    }
                }
            }
        }

        public static void RevealCast()
        {
            var targets = EntityManager.Heroes.Enemies.Where(x => !x.IsDead);
            foreach (var target in targets)
            {
                if (Sweeping_Lens_Trinket.IsOwned() && Sweeping_Lens_Trinket.IsReady())
                {
                    if (Sweeping_Lens_Trinket.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }

                if (Oracle_Alteration.IsOwned() && Oracle_Alteration.IsReady())
                {
                    if (Oracle_Alteration.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }

                if (Vision_Ward.IsOwned() && Vision_Ward.IsReady())
                {
                    if (Vision_Ward.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }
            }
        }

        public static void WardCast()
        {
            var enemies = EntityManager.Heroes.Enemies.Where(x => !x.IsDead && x.Distance(Player.Instance) < 1250);

            foreach (var target in enemies)
            {
                if (Warding_Totem_Trinket.IsOwned() && Warding_Totem_Trinket.IsReady())
                {
                    if (Warding_Totem_Trinket.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }

                if (Greater_Stealth_Totem_Trinket.IsOwned() && Greater_Stealth_Totem_Trinket.IsReady())
                {
                    if (Greater_Stealth_Totem_Trinket.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }

                if (Greater_Vision_Totem_Trinket.IsOwned() && Greater_Vision_Totem_Trinket.IsReady())
                {
                    if (Greater_Vision_Totem_Trinket.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }

                if (Sightstone.IsOwned() && Sightstone.IsReady())
                {
                    if (Sightstone.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }

                if (Ruby_Sightstone.IsOwned() && Ruby_Sightstone.IsReady())
                {
                    if (Ruby_Sightstone.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }

                if (Farsight_Alteration.IsOwned() && Farsight_Alteration.IsReady())
                {
                    if (Farsight_Alteration.Cast(target.Path.LastOrDefault()))
                    {
                        return;
                    }
                }
            }
        }
    }
}