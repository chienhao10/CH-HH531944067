using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace AutoBuddy.MyChampLogic
{
    internal class Sivir : IChampLogic
    {
        public float MaxDistanceForAA { get { return int.MaxValue; } }
        public float OptimalMaxComboDistance { get { return AutoWalker.myHero.AttackRange; } }
        public float HarassDistance { get { return AutoWalker.myHero.AttackRange; } }


        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot QLine { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Active E { get; private set; }
        public static Spell.Active R { get; private set; }

        public Sivir()
        {
            skillSequence = new[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            ShopSequence =
                "3340:Buy,1036:Buy,2003:StartHpPot,1053:Buy,1042:Buy,1001:Buy,3006:Buy,1036:Buy,1038:Buy,3072:Buy,2003:StopHpPot,1042:Buy,1051:Buy,3086:Buy,1042:Buy,1042:Buy,1043:Buy,3085:Buy,2015:Buy,3086:Buy,3094:Buy,1018:Buy,1038:Buy,3031:Buy,1037:Buy,3035:Buy,3033:Buy";

            Q = new Spell.Skillshot(SpellSlot.Q, 1250, SkillShotType.Linear, 250, 1350, 90)
            {
                AllowedCollisionCount = int.MaxValue
            };
            QLine = new Spell.Skillshot(SpellSlot.Q, 1250, SkillShotType.Linear, 250, 1350, 90);
            W = new Spell.Active(SpellSlot.W, 750);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Active(SpellSlot.R, 1000);

            Game.OnTick += Game_OnTick;
           // Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast; 
        }

        public int[] skillSequence { get; private set; }
        public LogicSelector Logic { get; set; }

        public string ShopSequence { get; private set; }
/*
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var unit = sender as AIHeroClient;
            if (unit == null || !unit.IsValid)
            {
                return;
            }

            if (!unit.IsEnemy || !E.IsReady())
            {
                return;
            }

            if (args.End.Distance(Player.Instance) == 0)
                return;

            var type = args.SData.TargettingType;

            if (unit.ChampionName.Equals("Caitlyn") && args.Slot == SpellSlot.Q)
            {
                Core.DelayAction(() => E.Cast(),
                    (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                    (int)(args.End.Distance(Player.Instance) / args.SData.MissileSpeed) - 500);
            }
            if (unit.ChampionName.Equals("Zyra"))
            {
                Core.DelayAction(() => E.Cast(),
                    (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                    (int)(args.End.Distance(Player.Instance) / args.SData.MissileSpeed) - 500);
            }
            if (args.End.Distance(Player.Instance) < 250)
            {
                if (unit.ChampionName.Equals("Bard") && args.End.Distance(Player.Instance) < 300)
                {
                    Core.DelayAction(() => E.Cast(), (int)(unit.Distance(Player.Instance) / 7f) + 400);
                }
                else if (unit.ChampionName.Equals("Ashe"))
                {
                    Core.DelayAction(() => E.Cast(),
                        (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                        (int)args.End.Distance(Player.Instance));
                    return;
                }
                else if (unit.ChampionName.Equals("Varus") || unit.ChampionName.Equals("TahmKench") ||
                         unit.ChampionName.Equals("Lux"))
                {
                    Core.DelayAction(() => E.Cast(),
                        (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                        (int)(args.End.Distance(Player.Instance) / args.SData.MissileSpeed) - 500);
                }
                else if (unit.ChampionName.Equals("Amumu"))
                {
                    if (sender.Distance(Player.Instance) < 1100)
                        Core.DelayAction(() => E.Cast(),
                            (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                            (int)(args.End.Distance(Player.Instance) / args.SData.MissileSpeed) - 500);
                }
            }

            if (args.Target != null && type.Equals(SpellDataTargetType.Unit))
            {
                if (!args.Target.IsMe ||
                    (args.Target.Name.Equals("Barrel") && args.Target.Distance(Player.Instance) > 200 &&
                     args.Target.Distance(Player.Instance) < 400))
                {
                    return;
                }

                if (unit.ChampionName.Equals("Nautilus") ||
                    (unit.ChampionName.Equals("Caitlyn") && args.Slot.Equals(SpellSlot.R)))
                {
                    var d = unit.Distance(Player.Instance);
                    var travelTime = d / args.SData.MissileSpeed;
                    var delay = travelTime * 1000 - E.CastDelay + 1150;
                    //Console.WriteLine("TT: " + travelTime + " " + delay);
                    Core.DelayAction(() => E.Cast(), (int)delay);
                    return;
                }
                E.Cast();
            }

            if (type.Equals(SpellDataTargetType.Unit))
            {
                if (unit.ChampionName.Equals("Bard") && args.End.Distance(Player.Instance) < 300)
                {
                    Core.DelayAction(() => E.Cast(), 400 + (int)(unit.Distance(Player.Instance) / 7f));
                }
                else if (unit.ChampionName.Equals("Riven") && args.End.Distance(Player.Instance) < 260)
                {
                    E.Cast();
                }
                else
                {
                    E.Cast();
                }
            }
            else if (type.Equals(SpellDataTargetType.LocationAoe) &&
                     args.End.Distance(Player.Instance) < args.SData.CastRadius)
            {
                // annie moving tibbers
                if (unit.ChampionName.Equals("Annie") && args.Slot.Equals(SpellSlot.R))
                {
                    return;
                }
                E.Cast();
            }
            else if (type.Equals(SpellDataTargetType.Cone) &&
                     args.End.Distance(Player.Instance) < args.SData.CastRadius)
            {
                E.Cast();
            }
            else if (type.Equals(SpellDataTargetType.SelfAoe) || type.Equals(SpellDataTargetType.Self))
            {
                var d = args.End.Distance(Player.Instance.ServerPosition);
                var p = args.SData.CastRadius > 5000 ? args.SData.CastRange : args.SData.CastRadius;
                if (d < p)
                    E.Cast();
            }
        }
        */
        public void Harass(AIHeroClient target)
        {

        }

        public void Survi()
        {/*
            AIHeroClient target =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    chase => chase.Distance(AutoWalker.myHero) < 600 && chase.IsVisible());
            if (Q.IsReady())
            {
                if (target != null && target.IsValidTarget())
                {
                    Q.Cast(target.ServerPosition);
                }
            }*/
        }

        public void Combo(AIHeroClient target)
        {
            if (target.HealthPercent <= 50)
            {
                if (Q.IsReady())
                {
                    if (target != null && target.IsValidTarget())
                    {
                        Q.Cast(target.ServerPosition);
                    }
                }

                if (W.IsReady())
                {
                    if (target != null && target.IsValidTarget())
                    {
                        W.Cast();
                        Orbwalker.ResetAutoAttack();
                    }
                }

                if (R.IsReady())
                {
                    if (ObjectManager.Player.Position.CountAlliesInRange(1000) >= 2
                      && ObjectManager.Player.Position.CountEnemiesInRange(2000) >= 2)
                    {
                        R.Cast();
                    }
                }

                if (E.IsReady())
                {
                    E.Cast();
                }
            }
        }

        private void Game_OnTick(System.EventArgs args)
        {

        }
    }
}