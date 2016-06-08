using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;

namespace OlafxQx.Modes
{
    internal static class ModeJungle
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;

        public static void Init(Menu mainMenu)
        {
            MenuLocal = mainMenu.AddSubMenu("Jungle", "Jungle");
            {
                InitSimpleMenu();
                MenuLocal.Add("Jungle.Youmuu.BaronDragon", new ComboBox("Items: Use for Baron/Dragon", 3, "Off", "Dragon", "Baron", "Both"));
                MenuLocal.Add("Jungle.Youmuu.BlueRed", new ComboBox("Items: Use for Blue/Red", 3, "Off", "Dragon", "Baron", "Both"));
                MenuLocal.Add("Jungle.Item", new ComboBox("Items: Other (Like Tiamat/Hydra)", 1, "Off", "On"));
            }

            Game.OnUpdate += OnUpdate;
        }

        static void InitSimpleMenu()
        {
            string[] strQSimple = new string[4];
            {
                strQSimple[0] = "Off";
                strQSimple[1] = "Just Big Mobs";
                for (var i = 2; i < 4; i++)
                {
                    strQSimple[i] = "Mob Count >= " + i;
                }
                MenuLocal.Add("Jungle.Simple.UseQ", new ComboBox("Q:", 0, strQSimple));
            }

            string[] strSimpleW = new string[6];
            {
                strSimpleW[0] = "Off";
                strSimpleW[1] = "Just Big Mobs";
                for (var i = 2; i < 6; i++)
                {
                    strSimpleW[i] = "If need to AA count >= " + (i + 2);
                }
                MenuLocal.Add("Jungle.Simple.UseW", new ComboBox("W:", 0, strSimpleW));
            }
            MenuLocal.Add("Jungle.Simple.UseE", new ComboBox("E:", 2, "Off", "On", "Just Big Mobs"));
        }

        private static void OnUpdate(EventArgs args)
        {
            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) && ModeConfig.MenuFarm["Farm.Enable"].Cast<KeyBind>().CurrentValue)
            {
                ExecuteSimpleMode();
            }
        }

        private static void ExecuteSimpleMode()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count <= 0)
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < CommonManaManager.JungleMinManaPercent(mobs[0]))
            {
                return;
            }

            if (Q.IsReady() && MenuLocal["Jungle.Simple.UseQ"].Cast<ComboBox>().CurrentValue != 0)
            {
                var qCount = MenuLocal["Jungle.Simple.UseQ"].Cast<ComboBox>().CurrentValue;

                if (qCount == 1)
                {
                    if (CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        Q.Cast(mobs[0].Position - 15);
                    }
                }
                else
                {
                    if (mobs.Count >= qCount)
                    {
                        Q.Cast(mobs[0].Position - 15);
                    }
                }
            }

            if (W.IsReady() && mobs[0].LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165) && MenuLocal["Jungle.Simple.UseW"].Cast<ComboBox>().CurrentValue != 0)
            {
                var wCount = MenuLocal["Jungle.Simple.UseW"].Cast<ComboBox>().CurrentValue;
                if (wCount == 1)
                {
                    if (CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var totalAa = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165)).Sum(mob => (int)mob.Health);

                    totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                    if (totalAa >= wCount + 2 || CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        W.Cast();
                    }
                }
            }

            if (E.CanCast(mobs[0]) && MenuLocal["Jungle.Simple.UseE"].Cast<ComboBox>().CurrentValue != 0)
            {
                var qCount = MenuLocal["Jungle.Simple.UseE"].Cast<ComboBox>().CurrentValue;

                if (qCount == 1)
                {
                    if (CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        E.CastOnUnit(mobs[0]);
                    }
                }
                else
                {
                    E.CastOnUnit(mobs[0]);
                }
            }
        }
    }
}
