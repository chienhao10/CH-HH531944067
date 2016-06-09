using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Champion;
using Leblanc.Common;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace Leblanc.Modes
{
    internal static class ModeJungle
    {
        public static Menu MenuLocal { get; private set; }

        private static LeagueSharp.Common.Spell Q => PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => PlayerSpells.E;

        public static void Init(Menu mainMenu)
        {
            MenuLocal = mainMenu.AddSubMenu("Jungle", "Jungle");
            {
                InitSimpleMenu();
                MenuLocal.AddGroupLabel("Item Settings : ");
                MenuLocal.Add("Jungle.Youmuu.BlueRed", new ComboBox("Items: Use for Blue/Red", 3, "Off", "Red", "Blue", "Both"));
                MenuLocal.Add("Jungle.Youmuu.BaronDragon", new ComboBox("Items: Use for Baron/Dragon", 3, "Off", "Dragon", "Baron", "Both"));
                MenuLocal.Add("Jungle.Item", new ComboBox("Items: Other (Tiamat/Hydra)", 1, "Off", "On"));
            }
            Game.OnUpdate += OnUpdate;
        }

        static void InitSimpleMenu()
        {
            MenuLocal.Add("Jungle.Simple.Q.Big", new ComboBox("Q Big Mobs:", 1, "Off", "On"));
            MenuLocal.Add("Jungle.Simple.Q.Small", new ComboBox("Q Small Mobs:", 1, "Off", "On: If Killable"));
            string[] strESimple = new string[5];
            {
                strESimple[0] = "Off";
                strESimple[1] = "Big Mobs";
                for (var i = 2; i < 5; i++)
                {
                    strESimple[i] = "If Need to AA Count >= " + (i + 2);
                }
                MenuLocal.Add("Jungle.Simple.W", new ComboBox("W:", 4, strESimple));
            }
            MenuLocal.Add("Jungle.Simple.E", new ComboBox("E:", 1, "Off", "On: Big Mobs", "On: Big Mobs [Just can stun]"));

            MenuLocal.AddGroupLabel("Min. Mana Control");
            MenuLocal.Add("MinMana.Jungle", new Slider("Min. Mana %:", 20, 0, 100));
            MenuLocal.Add("MinMana.DontCheckEnemyBuff", new ComboBox("Don't Check Min. Mana -> If Taking:", 2, "Off", "Ally Buff", "Enemy Buff", "Both"));
            MenuLocal.Add("MinMana.DontCheckBlueBuff", new CheckBox("Don't Check Min. Mana -> If Have Blue Buff:"));

            MenuLocal.Add("MinMana.Default", new CheckBox("Load Recommended Settings"))
                .OnValueChange +=
                (sender, args) =>
                {
                    if (args.NewValue)
                    {
                        LoadDefaultSettings();
                    }
                };
        }

        public static void LoadDefaultSettings()
        {
            MenuLocal["Jungle.Simple.Q.Big"].Cast<ComboBox>().CurrentValue = 1;
            MenuLocal["Jungle.Simple.Q.Small"].Cast<ComboBox>().CurrentValue =  1;

            string[] strESimple = new string[5];
            {
                strESimple[0] = "Off";
                strESimple[1] = "Big Mobs";
                for (var i = 2; i < 5; i++)
                {
                    strESimple[i] = "If Need to AA Count >= " + (i + 2);
                }
                MenuLocal["Jungle.Simple.W"].Cast<ComboBox>().CurrentValue = 4;
            }

            MenuLocal["Jungle.Simple.E"].Cast<ComboBox>().CurrentValue = 1;

            MenuLocal["MinMana.Jungle"].Cast<Slider>().CurrentValue = 20;
            MenuLocal["MinMana.DontCheckEnemyBuff"].Cast<ComboBox>().CurrentValue = 2;
            MenuLocal["MinMana.DontCheckBlueBuff"].Cast<CheckBox>().CurrentValue = true;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ExecuteSimpleMode();
            }
        }

        public static float JungleMinManaPercent(Obj_AI_Base mob)
        {
            // Enable / Disable Min Mana
            if (!ModeConfig.MenuFarm["Farm.MinMana.Enable"].Cast<KeyBind>().CurrentValue)
            {
                return 0f;
            }

            // Don't Control Min Mana if I have blue buff
            if (MenuLocal["MinMana.DontCheckBlueBuff"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.HasBlueBuff())
            {
                return 0f;
            }

            // Don't check min mana If I'm taking enemy blue / red
            var dontCheckMinMana = MenuLocal["MinMana.DontCheckEnemyBuff"].Cast<ComboBox>().CurrentValue;

            if ((dontCheckMinMana == 1 || dontCheckMinMana == 3)
                && mob.GetMobTeam(Q.Range) == (CommonManaManager.GameObjectTeam)ObjectManager.Player.Team
                && (mob.BaseSkinName == "SRU_Blue" || mob.BaseSkinName == "SRU_Red"))
            {
                return 0f;
            }

            if ((dontCheckMinMana == 2 || dontCheckMinMana == 3)
                && mob.GetMobTeam(Q.Range) != (CommonManaManager.GameObjectTeam)ObjectManager.Player.Team
                && (mob.BaseSkinName == "SRU_Blue" || mob.BaseSkinName == "SRU_Red"))
            {
                return 0f;
            }

            return MenuLocal["MinMana.Jungle"].Cast<Slider>().CurrentValue;
        }

        private static void ExecuteSimpleMode()
        {
            if (!ModeConfig.MenuFarm["Farm.Enable"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count <= 0)
            {
                return;
            }

            var mob = mobs[0];

            if (!Common.CommonHelper.ShouldCastSpell(mob))
            {
                return;
            }


            if (ObjectManager.Player.ManaPercent < JungleMinManaPercent(mob))
            {
                return;
            }

            if (Q.CanCast(mob))
            {

                var useQBig = MenuLocal["Jungle.Simple.Q.Big"].Cast<ComboBox>().CurrentValue;
                var useQSmall = MenuLocal["Jungle.Simple.Q.Small"].Cast<ComboBox>().CurrentValue;

                if (useQBig == 1 && CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                {
                    //Champion.PlayerSpells.CastQObjects(mob);
                }

                if (useQSmall == 1 && CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) != CommonManaManager.MobTypes.Big && mob.CanKillableWith(Q))
                {
                    //Champion.PlayerSpells.CastQObjects(mob);
                }
            }


            if (W.IsReady() && MenuLocal["Jungle.Simple.W"].Cast<ComboBox>().CurrentValue != 0 && mob.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            {
                var totalAa = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)).Sum(m => (int)m.Health);

                totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                if (totalAa >= MenuLocal["Jungle.Simple.W"].Cast<ComboBox>().CurrentValue + 2 || CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                {
                    W.Cast();
                }
            }

            if (E.CanCast(mob) && MenuLocal["Jungle.Simple.E"].Cast<ComboBox>().CurrentValue != 0)
            {
                var useE = MenuLocal["Jungle.Simple.E"].Cast<ComboBox>().CurrentValue;

                if (useE == 1 && CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                {
                    Champion.PlayerSpells.E.CastOnUnit(mob);
                }

                if (useE == 2 && CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big && mob.CanStun())
                {
                    Champion.PlayerSpells.E.CastOnUnit(mob);
                }
            }
        }
    }
}
