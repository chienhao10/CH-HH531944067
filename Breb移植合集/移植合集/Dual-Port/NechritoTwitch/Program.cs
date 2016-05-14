using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;

namespace Nechrito_Twitch
{
    internal class Program
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        private static float GetDamage(AIHeroClient target)
        {
            return Spells._e.GetDamage(target);
        }

        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Twitch") return;

            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnUpdate += Game_OnUpdate;
            MenuConfig.LoadMenu();
            Spells.Initialise();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) LaneClear(); JungleClear();
            AutoE();
            //Recall();
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Spells._e.Range, DamageType.Physical);
            if (target == null) return;

            if (MenuConfig.UseW)
            {
                if (target.IsValidTarget(Spells._w.Range) && Spells._w.CanCast(target))
                {
                    Spells._w.Cast(target);
                }
            }




        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(Spells._e.Range, DamageType.Physical);
            if (!Orbwalking.InAutoAttackRange(target) && target.GetBuffCount("twitchdeadlyvenom") >= MenuConfig.ESlider && Player.ManaPercent > 50 && Spells._e.IsReady())
            {
                Spells._e.Cast(target);
            }
            if (MenuConfig.harassW)
            {
                if (target.IsValidTarget(Spells._w.Range) && Spells._w.CanCast(target))
                {
                    Spells._w.Cast(target);
                }
            }
        }
        public static void LaneClear()
        {
            if (MenuConfig.laneW)
            {
                var minions = MinionManager.GetMinions(Player.Position, Spells._w.Range);
                if (minions.Count >= 5)
                    Spells._w.Cast(minions[5].ServerPosition);
            }
            /* Laneclear E Lasthit, Add stacks for enemies + minion lasthit
            var minion = MinionManager.GetMinions(Player.Position, Spells._e.Range);
            foreach (var m in minion)
            {
                if (m.HasBuff("twitchdeadlyvenom") && Spells._e.IsKillable(m))
                        Spells._e.Cast(m);
            }
            */
        }

        public static void JungleClear()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var mobs = MinionManager.GetMinions(Player.Position, Spells._w.Range, MinionTypes.All,
                       MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (mobs.Count != 0)
                {
                    Spells._w.Cast(mobs[0].ServerPosition);
                    foreach (var m in mobs)
                    {
                        if (m.HasBuff("twitchdeadlyvenom"))
                        {
                            if (Spells._e.IsKillable(m))
                                Spells._e.Cast();
                        }
                    }
                }
            }
        }

        /* Recall Bug with MenuConfig
        private static void Recall()
        {
            if (MenuConfig.QRecall)
            {
                if (Spells._q.IsReady() && Spells._recall.IsReady())
                {
                    Spells._q.Cast();
                    Spells._recall.Cast();
                }
            }
            
        }
        */
        public static void AutoE()
        {
            var mob = MinionManager.GetMinions(Spells._e.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            foreach (var m in mob)
            {
                if ((m.CharData.BaseSkinName.Contains("Dragon") || m.CharData.BaseSkinName.Contains("Baron")))
                    if (Spells._e.IsKillable(m))
                        Spells._e.Cast();



            }
            if (MenuConfig.KsE)
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(enemy => enemy.IsValidTarget(Spells._e.Range) && Spells._e.IsKillable(enemy)))
                    Spells._e.Cast(enemy);

            }
        }


        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
                if (MenuConfig.dind)
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(GetDamage(enemy), new ColorBGRA(255, 204, 0, 170));
                }
            }
        }

    }
}
