#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

#endregion

namespace JaxQx
{
    internal class Program
    {
        public const string ChampionName = "Jax";

        //Orbwalker instance

        private static bool shenBuffActive;

        private static bool eCounterStrike;

        public static Items Items;

        public static Extra Extra;

        public static Utils Utils;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;

        public static Spell W;

        public static Spell E;

        public static Spell R;

        public static string[] Wards =
        {
            "RelicSmallLantern", "RelicLantern", "SightWard", "wrigglelantern",
            "ItemGhostWard", "VisionWard", "BantamTrap", "JackInTheBox",
            "CaitlynYordleTrap", "Bushwhack"
        };

        public static Map map;


        public static float WardRange = 600f;

        public static int DelayTick;

        //Menu
        public static Menu Config, comboMenu, harassMenu, laneClearMenu, jungleMenu, miscMenu, drawingMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static string Tab
        {
            get { return "       "; }
        }

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Jax") return;

            Q = new Spell(SpellSlot.Q, 680f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetTargetted(0.50f, 75f);

            Items = new Items();

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //Create the menu
            Config = MainMenu.AddMenu("xQx|武器大师", "Jax");

            // Combo
            comboMenu = Config.AddSubMenu("连招", "Combo");
            comboMenu.Add("ComboUseQMinRange", new Slider("最低 Q 范围", 250, 250, (int) Q.Range));
            comboMenu.Add("Combo.CastE", new ComboBox("E 设置:", 1, "跳跃前使用", "跳跃后使用"));

            // Harass
            harassMenu = Config.AddSubMenu("骚扰", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("使用 Q"));
            harassMenu.Add("UseQHarassDontUnderTurret", new CheckBox("塔下不 Q"));
            harassMenu.Add("UseWHarass", new CheckBox("使用 W"));
            harassMenu.Add("UseEHarass", new CheckBox("使用 E"));
            harassMenu.Add("HarassMode", new ComboBox("骚扰 模式: ", 2, "Q+W", "Q+E", "预设"));
            harassMenu.Add("HarassMana", new Slider("最低蓝量%: ", 50));

            // Lane Clear
            laneClearMenu = Config.AddSubMenu("清线", "LaneClear");
            laneClearMenu.Add("UseQLaneClear", new CheckBox("使用 Q", false));
            laneClearMenu.Add("UseQLaneClearDontUnderTurret", new CheckBox("塔下不 Q"));
            laneClearMenu.Add("UseWLaneClear", new CheckBox("使用 W", false));
            laneClearMenu.Add("UseELaneClear", new CheckBox("使用 E", false));
            laneClearMenu.Add("LaneClearMana", new Slider("最低蓝量%: ", 50));

            // Jungling Farm
            jungleMenu = Config.AddSubMenu("清野", "JungleFarm");
            jungleMenu.Add("UseQJungleFarm", new CheckBox("使用 Q"));
            jungleMenu.Add("UseWJungleFarm", new CheckBox("使用 W", false));
            jungleMenu.Add("UseEJungleFarm", new CheckBox("使用 E", false));
            jungleMenu.Add("JungleFarmMana", new Slider("最低蓝量%: ", 50));

            // Misc
            miscMenu = Config.AddSubMenu("杂项", "Misc");
            miscMenu.Add("InterruptSpells", new CheckBox("技能打断"));
            miscMenu.Add("Misc.AutoW", new CheckBox("如果可 W,自动使用"));
            miscMenu.Add("Ward", new KeyBind("跳眼 / 逃跑", false, KeyBind.BindTypes.HoldActive, 'A'));

            Extra = new Extra();
            Utils = new Utils();
            PlayerSpells.Initialize();

            // Drawing
            drawingMenu = Config.AddSubMenu("线圈", "Drawings");
            drawingMenu.Add("DrawQRange", new CheckBox("Q 范围"));
            drawingMenu.Add("DrawQMinRange", new CheckBox("最低 Q 范围"));
            drawingMenu.Add("DrawWard", new CheckBox("眼距离"));

            map = new Map();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += OrbwalkingBeforeAttack;
            Obj_AI_Base.OnBuffLose += (sender, eventArgs) =>
            {
                if (sender.IsMe && eventArgs.Buff.Name.ToLower() == "sheen")
                {
                    shenBuffActive = false;
                }

                if (sender.IsMe && eventArgs.Buff.Name.ToLower() == "jaxcounterstrike")
                {
                    eCounterStrike = false;
                }
            };

            Obj_AI_Base.OnBuffGain += (sender, eventArgs) =>
            {
                if (sender.IsMe && eventArgs.Buff.Name.ToLower() == "sheen")
                {
                    shenBuffActive = true;
                }

                if (sender.IsMe && eventArgs.Buff.Name.ToLower() == "jaxcounterstrike")
                {
                    eCounterStrike = true;
                }
            };
        }

        private static void OrbwalkingBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target is AIHeroClient)
            {
                if (W.IsReady() && getCheckBoxItem(miscMenu, "Misc.AutoW") && !shenBuffActive)
                {
                    W.Cast();
                }

                foreach (
                    var item in
                        Items.ItemDb.Where(
                            i =>
                                i.Value.ItemType == Items.EnumItemType.OnTarget &&
                                i.Value.TargetingType == Items.EnumItemTargettingType.EnemyHero &&
                                i.Value.Item.IsReady()))
                {
                    //Game.PrintChat(item.Value.Item.Id.ToString());
                    item.Value.Item.Cast();
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawQRange = getCheckBoxItem(drawingMenu, "DrawQRange");
            if (drawQRange)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.FromArgb(255, 255, 255, 255), 1);
            }

            var drawWard = getCheckBoxItem(drawingMenu, "DrawWard");
            if (drawWard)
            {
                Render.Circle.DrawCircle(Player.Position, WardRange, Color.FromArgb(255, 255, 255, 255), 1);
            }

            var drawMinQRange = getCheckBoxItem(drawingMenu, "DrawQMinRange");
            if (drawMinQRange)
            {
                var minQRange = getSliderItem(comboMenu, "ComboUseQMinRange");
                Render.Circle.DrawCircle(Player.Position, minQRange, Color.GreenYellow, 1);
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs arg)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (arg.Slot == SpellSlot.Q && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                E.IsReady())
            {
                if (getBoxItem(comboMenu, "Combo.CastE") == 0)
                {
                    E.Cast();
                }
            }

            if (Wards.ToList().Contains(arg.SData.Name))
            {
                Jumper.testSpellCast = arg.End.To2D();
                Polygon pol;
                if ((pol = map.getInWhichPolygon(arg.End.To2D())) != null)
                {
                    Jumper.testSpellProj = pol.getProjOnPolygon(arg.End.To2D());
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (DelayTick - Environment.TickCount <= 250)
            {
                DelayTick = Environment.TickCount;
            }

            if (getKeyBindItem(miscMenu, "Ward"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                Jumper.wardJump(Game.CursorPos.To2D());
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var existsMana = Player.MaxMana/100*getSliderItem(harassMenu, "HarassMana");
                if (Player.Mana >= existsMana)
                {
                    Harass();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var existsManalc = Player.MaxMana/100*getSliderItem(laneClearMenu, "LaneClearMana");
                if (Player.Mana >= existsManalc)
                {
                    LaneClear();
                }

                var existsManajg = Player.MaxMana/100*getSliderItem(jungleMenu, "JungleFarmMana");
                if (Player.Mana >= existsManajg)
                {
                    JungleFarm();
                }
            }
        }

        private static void Combo()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t == null)
            {
                return;
            }

            if (t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 95) && shenBuffActive)
            {
                return;
            }

            var minQRange = getSliderItem(comboMenu, "ComboUseQMinRange");

            if (Q.IsReady() && Q.GetDamage(t) > t.Health)
            {
                Q.Cast(t);
            }

            if (E.IsReady())
            {
                switch (getBoxItem(comboMenu, "Combo.CastE"))
                {
                    case 0:
                        if (E.IsReady() && Q.IsReady() && t.IsValidTarget(Q.Range))
                        {
                            if (Player.Distance(t) >= minQRange && t.IsValidTarget(Q.Range)) Q.CastOnUnit(t);
                            E.Cast();
                        }
                        break;
                    case 1:
                        if (E.IsReady() && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 95))
                        {
                            E.Cast();
                        }
                        break;
                }

                if (eCounterStrike && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                {
                    E.Cast();
                }
            }

            if (Q.IsReady() && Player.Distance(t) >= minQRange && t.IsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }


            if (ObjectManager.Player.Distance(t) <= E.Range)
            {
                CastItems();
                //UseItems(t);
            }

            if (W.IsReady() && ObjectManager.Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(t)) > 0)
            {
                W.Cast();
            }

            if (E.IsReady() && ObjectManager.Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(t)) > 0)
            {
                E.Cast();
            }

            if (R.IsReady())
            {
                if (Player.Distance(t) < Player.AttackRange)
                {
                    if (
                        ObjectManager.Player.CountEnemiesInRange(
                            (int) Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)) >= 2
                        || t.Health > Player.Health)
                    {
                        R.CastOnUnit(Player);
                    }
                }
            }
        }

        private static void Harass()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t == null)
            {
                return;
            }
            var useQ = getCheckBoxItem(harassMenu, "UseQHarass");
            var useW = getCheckBoxItem(harassMenu, "UseWHarass");
            var useE = getCheckBoxItem(harassMenu, "UseEHarass");
            var useQDontUnderTurret = getCheckBoxItem(harassMenu, "UseQHarassDontUnderTurret");

            switch (getBoxItem(harassMenu, "HarassMode"))
            {
                case 0:
                {
                    if (Q.IsReady() && W.IsReady() && t != null && useQ && useW)
                    {
                        if (useQDontUnderTurret)
                        {
                            if (!t.UnderTurret())
                            {
                                Q.Cast(t);
                                W.Cast();
                            }
                        }
                        else
                        {
                            Q.Cast(t);
                            W.Cast();
                        }
                    }
                    break;
                }
                case 1:
                {
                    if (Q.IsReady() && E.IsReady() && t != null && useQ && useE)
                    {
                        if (useQDontUnderTurret)
                        {
                            if (!t.UnderTurret())
                            {
                                Q.Cast(t);
                                E.Cast();
                            }
                        }
                        else
                        {
                            Q.Cast(t);
                            E.Cast();
                        }
                    }
                    break;
                }
                case 2:
                {
                    if (Q.IsReady() && useQ && t != null && useQ)
                    {
                        if (useQDontUnderTurret)
                        {
                            if (!t.UnderTurret()) Q.Cast(t);
                        }
                        else Q.Cast(t);
                        UseItems(t);
                    }

                    if (W.IsReady() && useW && t != null && t.IsValidTarget(E.Range))
                    {
                        W.Cast();
                    }

                    if (E.IsReady() && useE && t != null && t.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(Player);
                    }
                    break;
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = getCheckBoxItem(laneClearMenu, "UseQLaneClear");
            var useW = getCheckBoxItem(laneClearMenu, "UseWLaneClear");
            var useE = getCheckBoxItem(laneClearMenu, "UseELaneClear");
            var useQDontUnderTurret = getCheckBoxItem(laneClearMenu, "UseQLaneClearDontUnderTurret");

            var vMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var vMinion in vMinions)
            {
                if (useQ && Q.IsReady() && Player.Distance(vMinion) > Orbwalking.GetRealAutoAttackRange(Player))
                {
                    if (useQDontUnderTurret)
                    {
                        if (!vMinion.UnderTurret()) Q.Cast(vMinion);
                    }
                    else Q.Cast(vMinion);
                }

                if (useW && W.IsReady()) W.Cast();

                if (useE && E.IsReady()) E.CastOnUnit(Player);
            }
        }

        private static void JungleFarm()
        {
            var useQ = getCheckBoxItem(jungleMenu, "UseQJungleFarm");
            var useW = getCheckBoxItem(jungleMenu, "UseWJungleFarm");
            var useE = getCheckBoxItem(jungleMenu, "UseEJungleFarm");

            var mobs = MinionManager.GetMinions(
                Player.ServerPosition,
                Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0) return;

            if (Q.IsReady() && useQ && Player.Distance(mobs[0]) > Player.AttackRange) Q.Cast(mobs[0]);

            if (W.IsReady() && useW) W.Cast();

            if (E.IsReady() && useE) E.CastOnUnit(Player);
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var interruptSpells = getCheckBoxItem(miscMenu, "InterruptSpells");
            if (!interruptSpells || !E.IsReady()) return;

            if (Player.Distance(unit) <= E.Range)
            {
                E.Cast();
            }
        }

        private static InventorySlot GetInventorySlot(int ID)
        {
            return
                ObjectManager.Player.InventoryItems.FirstOrDefault(
                    item =>
                        (item.Id == (ItemId) ID && item.Stacks >= 1) || (item.Id == (ItemId) ID && item.Charges >= 1));
        }

        public static void UseItems(AIHeroClient t)
        {
            if (t == null) return;

            int[] targeted = {3153, 3144, 3146, 3184, 3748};
            foreach (var itemId in
                targeted.Where(
                    itemId =>
                        LeagueSharp.Common.Items.HasItem(itemId) && LeagueSharp.Common.Items.CanUseItem(itemId)
                        && GetInventorySlot(itemId) != null && t.IsValidTarget(450)))
            {
                LeagueSharp.Common.Items.UseItem(itemId, t);
            }

            int[] nonTarget = {3180, 3143, 3131, 3074, 3077, 3142};
            foreach (var itemId in
                nonTarget.Where(
                    itemId =>
                        LeagueSharp.Common.Items.HasItem(itemId) && LeagueSharp.Common.Items.CanUseItem(itemId)
                        && GetInventorySlot(itemId) != null && t.IsValidTarget(450)))
            {
                LeagueSharp.Common.Items.UseItem(itemId);
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void CastItems()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!t.IsValidTarget()) return;

            foreach (var item in
                Items.ItemDb.Where(
                    item =>
                        item.Value.ItemType == Items.EnumItemType.AoE
                        && item.Value.TargetingType == Items.EnumItemTargettingType.EnemyObjects)
                    .Where(item => t.IsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady()))
            {
                item.Value.Item.Cast();
            }

            foreach (var item in
                Items.ItemDb.Where(
                    item =>
                        item.Value.ItemType == Items.EnumItemType.Targeted
                        && item.Value.TargetingType == Items.EnumItemTargettingType.EnemyHero)
                    .Where(item => t.IsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady()))
            {
                item.Value.Item.Cast(t);
            }
        }
    }
}