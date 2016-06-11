using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;

namespace OlafxQx.Modes
{
    internal static class ModeLane
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;

        public static void Init(Menu mainMenu)
        {
            MenuLocal = mainMenu.AddSubMenu("Lane", "Lane");
            {
                string[] strQ = new string[6];
                {
                    strQ[0] = "Off";
                    strQ[1] = "Just for out of AA range";
                    for (var i = 2; i < 6; i++)
                    {
                        strQ[i] = "Minion Count >= " + i;
                    }
                    MenuLocal.Add("Lane.UseQ", new ComboBox("Q:", 1, strQ));
                    MenuLocal.Add("Lane.UseQ.Mode", new ComboBox("Q: Cast Mode:", 1, "Cast for Hit", "Cast for Kill"));
                }

                string[] strW = new string[6];
                {
                    strW[0] = "Off";
                    for (var i = 1; i < 6; i++)
                    {
                        strW[i] = "If need to AA count >= " + (i + 3);
                    }
                    MenuLocal.Add("Lane.UseW", new ComboBox("W:", 1, strW));
                }

                MenuLocal.Add("Lane.UseE", new ComboBox("E:", 1, "Off", "On: Last hit", "On: Health Prediction", "Both"));

                MenuLocal.Add("Lane.Item", new ComboBox("Items:", 1, "Off", "On"));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Execute(); 
            }
        }

        private static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < CommonManaManager.LaneMinManaPercent || !ModeConfig.MenuFarm["Farm.Enable"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }
         
            if (Q.IsReady() && MenuLocal["Lane.UseQ"].Cast<ComboBox>().CurrentValue != 0)
            {
                var qCount = MenuLocal["Lane.UseQ"].Cast<ComboBox>().CurrentValue;

                var objAiHero = from x1 in ObjectManager.Get<Obj_AI_Minion>()
                                where x1.LSIsValidTarget() && x1.IsEnemy
                                select x1
                                     into h
                                orderby h.LSDistance(ObjectManager.Player) descending
                                select h
                                         into x2
                                where x2.LSDistance(ObjectManager.Player) < Q.Range - 20 && !x2.IsDead
                                select x2;

                var aiMinions = objAiHero as Obj_AI_Minion[] ?? objAiHero.ToArray();

                var lastMinion = aiMinions.First();

                var qMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, ObjectManager.Player.LSDistance(lastMinion.Position));

                if (qMinions.Count > 0)
                {
                    var locQ = Q.GetLineFarmLocation(qMinions, Q.Width);

                    if (qMinions.Count == qMinions.Count(m => ObjectManager.Player.LSDistance(m) < Q.Range) && locQ.MinionsHit >= qCount && locQ.Position.IsValid())
                    {
                        Q.Cast(lastMinion.Position);
                    }
                }
            }

            if (MenuLocal["Lane.UseW"].Cast<ComboBox>().CurrentValue != 0 && W.IsReady())
            {
                var wCount = MenuLocal["Lane.UseW"].Cast<ComboBox>().CurrentValue;

                var totalAa =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            m =>
                                m.IsEnemy && !m.IsDead &&
                                m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null)))
                        .Sum(mob => (int)mob.Health);

                totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                if (totalAa >= wCount + 3)
                {
                    W.Cast();
                }
            }

            var useE = MenuLocal["Lane.UseE"].Cast<ComboBox>().CurrentValue;
            if (useE != 0 && E.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                if (useE == 1 || useE == 3)
                {
                    foreach (
                        var eMinion in
                            minions.Where(m => m.Health < E.GetDamage(m) && E.CanCast(m)))
                    {
                        E.CastOnUnit(eMinion);
                    }
                }

                if (useE == 2 || useE == 3)
                {
                    foreach (
                        var eMinion in
                            minions.Where(
                                m =>
                                    HealthPrediction.GetHealthPrediction(m,
                                        (int)(ObjectManager.Player.AttackCastDelay * 1000), Game.Ping / 2) < 0)
                                .Where(m => m.Health < E.GetDamage(m) && E.CanCast(m)))
                    {
                        E.CastOnUnit(eMinion);
                    }
                }
            }
        }
    }
}
