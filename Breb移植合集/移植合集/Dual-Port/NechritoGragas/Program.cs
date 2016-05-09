using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

namespace Nechrito_Gragas
{
    class Program
    {
        public static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };

        public static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };
        
        public static AIHeroClient Player => ObjectManager.Player;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        
        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Gragas") return;
            MenuConfig.LoadMenu();
            Spells.Initialise();
            Game.OnUpdate += OnTick;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }
        private static void OnTick(EventArgs args)
        {
            SmiteCombo();
            Killsteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Mode.ComboLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mode.HarassLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Mode.JungleLogic();
            }
        }
        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    var minions = MinionManager.GetMinions(Player.ServerPosition, 600f).FirstOrDefault();
                    {
                        if (minions == null)
                            return;
                        if (Spells._w.IsReady() && MenuConfig.LaneW)
                            Spells._w.Cast();
                        if (Spells._e.IsReady() && MenuConfig.LaneE)
                            Spells._e.Cast(GetCenterMinion());
                        if (Spells._q.IsReady() && MenuConfig.LaneQ)
                            Spells._q.Cast(GetCenterMinion());
                    }
                   
                }
            }
        }
        
        public static Obj_AI_Base GetCenterMinion()
        {
            var minionposition = MinionManager.GetMinions(300 + Spells._q.Range).Select(x => x.Position.LSTo2D()).ToList();
            var center = MinionManager.GetBestCircularFarmLocation(minionposition, 250, 300 + Spells._q.Range);

            return center.MinionsHit >= 3
                ? MinionManager.GetMinions(1000).OrderBy(x => x.LSDistance(center.Position)).FirstOrDefault()
                : null;
        }
        private static void Killsteal()
        {
            if(Spells._q.IsReady() && Spells._r.IsReady())
            if (Spells._q.IsReady())
                {
                    var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells._q.Range) && !x.IsZombie);
                    foreach (var target in targets)
                    {
                        if (target.Health < Spells._q.GetDamage(target) + Spells._q.GetDamage(target))   
                        {
                            var pos = Spells._r.GetPrediction(target).CastPosition + 60;
                          
                            Spells._q.Cast(pos);
                            Spells._r.Cast(pos);
                        }
                    }
                }
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells._q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._q.GetDamage(target))
                    {
                        var pos = Spells._q.GetPrediction(target).CastPosition;
                        Spells._q.Cast(pos);
                    }
                }
            }
            if (Spells._e.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells._e.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._e.GetDamage(target))
                    {
                        var pos = Spells._e.GetPrediction(target).CastPosition;
                        Spells._e.Cast(pos);
                    }
                }
            }
            if (Spells._r.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells._r.Range + Spells._e.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._r.GetDamage(target) && !target.IsInvulnerable && (Player.LSDistance(target.Position) <= Spells._e.Range) && (Player.LSDistance(target.Position) >= Spells._r.Range))
                    {
                        var pos = Spells._r.GetPrediction(target).CastPosition + 60;
                        Spells._e.Cast(target);
                        Spells._r.Cast(pos);
                    }
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (MenuConfig.dind)
                {
                    var ezkill = Spells._r.IsReady() && Dmg.IsLethal(enemy)
                        ? new ColorBGRA(0, 255, 0, 120)
                        : new ColorBGRA(255, 255, 0, 120);
                    Indicator.unit = enemy;
                    Indicator.drawDmg(Dmg.ComboDmg(enemy), ezkill);
                }
            }
        }
        protected static void SmiteCombo()
        {
            if (BlueSmite.Any(id => Items.HasItem(id)))
            {
                Spells.Smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
                return;
            }

            if (RedSmite.Any(id => Items.HasItem(id)))
            {
                Spells.Smite = Player.GetSpellSlot("s5_summonersmiteduel");
                return;
            }

            Spells.Smite = Player.GetSpellSlot("summonersmite");
        }
    }
}
