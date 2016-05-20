using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Champion = SCommon.PluginBase.Champion;
using Color = System.Drawing.Color;
using Utility = LeagueSharp.Common.Utility;

namespace SAutoCarry.Champions.Helpers
{
    public static class SoldierMgr
    {
        public const int SoldierAttackRange = 315;

        private static Champion s_Champion;
        private static bool s_ProcessNextSoldier;

        public static Menu m;

        public static bool SoldierAttacking { get; private set; }
        public static int LastSoldierSpawn { get; private set; }
        public static List<GameObject> ActiveSoldiers { get; private set; }

        public static void Initialize(Champion champ)
        {
            s_Champion = champ;
            ActiveSoldiers = new List<GameObject>();

            m = MainMenu.AddMenu("SoldierMgr", "SAutoCarry.Helpers.SoldierMgr.Root");
            m.Add("SAutoCarry.Helpers.SoldierMgr.Root.DrawRanges", new CheckBox("Draw Soldier Range"));

            GameObject.OnCreate += AIHeroClient_OnCreate;
            Obj_AI_Base.OnPlayAnimation += AIHeroClient_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        public static void InitializeA()
        {
            ActiveSoldiers = new List<GameObject>();

            //m = MainMenu.AddMenu("SoldierMgr", "SAutoCarry.Helpers.SoldierMgr.Root");
            //m.Add("SAutoCarry.Helpers.SoldierMgr.Root.DrawRanges", new CheckBox("Draw Soldier Range"));

            GameObject.OnCreate += AIHeroClient_OnCreate;
            Obj_AI_Base.OnPlayAnimation += AIHeroClient_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        public static float GetAADamage(AIHeroClient target)
        {
            var dmg =
                (float)
                    ObjectManager.Player.CalcDamage(target, DamageType.Magical,
                        new[] {50, 55, 60, 65, 70, 75, 80, 85, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180}[
                            ObjectManager.Player.Level - 1] + 0.6f * ObjectManager.Player.TotalMagicalDamage);
            dmg += dmg*0.25f*(ActiveSoldiers.Count(p => p.Position.LSDistance(target.Position) < SoldierAttackRange) - 1);
            return dmg;
        }

        public static bool InAARange(Obj_AI_Base target)
        {
            return ActiveSoldiers.Any(soldier => Vector2.DistanceSquared(target.Position.To2D(), soldier.Position.To2D()) <= SoldierAttackRange*SoldierAttackRange);
        }

        private static void AIHeroClient_OnCreate(GameObject sender, EventArgs args)
        {
            if (s_ProcessNextSoldier && sender.Name == "AzirSoldier")
            {
                ActiveSoldiers.Add(sender);
                s_ProcessNextSoldier = false;
            }
        }

        private static void AIHeroClient_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (args.Animation == "Death")
            {
                var idx = ActiveSoldiers.FindIndex(p => p.NetworkId == sender.NetworkId);
                if (idx != -1)
                    ActiveSoldiers.RemoveAt(idx);
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.ToLower() == "azirbasicattacksoldier")
                {
                    SoldierAttacking = true;
                    Utility.DelayAction.Add((int) (ObjectManager.Player.AttackCastDelay*1000),
                        () => SoldierAttacking = false);
                }
                else if (args.SData.Name == ObjectManager.Player.GetSpell(SpellSlot.W).SData.Name)
                {
                    s_ProcessNextSoldier = true;
                    LastSoldierSpawn = Utils.TickCount;
                }
            }
        }
    }
}