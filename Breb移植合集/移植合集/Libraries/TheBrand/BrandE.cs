using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using PortAIO.Champion.Brand;
using TheBrand.Commons;
using TheBrand.Commons.ComboSystem;

namespace TheBrand
{
    internal class BrandE : Skill
    {
        private BrandQ _brandQ;
        private Obj_AI_Base _recentFarmTarget;


        public BrandE(SpellSlot slot)
            : base(slot)
        {
        }

        public override void Initialize(ComboProvider combo)
        {
            _brandQ = combo.GetSkill<BrandQ>();
            Orbwalker.OnUnkillableMinion += OnMinionUnkillable;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            base.Initialize(combo);
        }

        private void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            _recentFarmTarget = args.Target.Type == GameObjectType.obj_AI_Base
                ? (Obj_AI_Base) args.Target
                : _recentFarmTarget;
        }

        public override void Update(Orbwalker.ActiveModes mode, ComboProvider combo, AIHeroClient target)
        {
            if (Program.getMiscMenuCB("eKS") &&
                (mode == Orbwalker.ActiveModes.Combo || !Program.getMiscMenuCB("KSCombo")))
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(650)))
                {
                    if (!IsKillable(enemy)) continue;
                    Cast(enemy);
                }
            base.Update(mode, combo, target);
        }

        public override void Execute(AIHeroClient target)
        {
            var distance = target.LSDistance(ObjectManager.Player);
                //Todo: make him use fireminions even in range, just for showoff and potential AOE. Check if hes on fire too though
            if (distance < 950 && distance > 650 && Program.getMiscMenuCB("eMinion"))
            {
                var fireMinion =
                    MinionManager.GetMinions(650, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None)
                        .Where(minion => minion.HasBuff("brandablaze") && minion.LSDistance(target) < 300)
                        .MinOrDefault(minion => minion.LSDistance(target));
                if (fireMinion != null)
                {
                    if (Cast(fireMinion) == CastStates.SuccessfullyCasted && !target.HasSpellShield())
                        Provider.SetMarked(target);
                }
            }
            if (distance < 650)
            {
                if (Cast(target) == CastStates.SuccessfullyCasted && !target.HasSpellShield())
                    Provider.SetMarked(target);
            }
        }


        public override void LaneClear(ComboProvider combo, AIHeroClient target)
        {
            var minions =
                MinionManager.GetMinions(650, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(minion => minion.HasBuff("brandablaze"))
                    .ToArray();
            if (!minions.Any()) return;
            Obj_AI_Base bestMinion = null;
            var neighbours = -1;
            foreach (var minion in minions)
            {
                var currentNeighbours = minions.Count(neighbour => neighbour.LSDistance(minion) < 300);
                if (currentNeighbours <= neighbours) continue;
                bestMinion = minion;
                neighbours = currentNeighbours;
            }
            Cast(bestMinion);
        }

        private void OnMinionUnkillable(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (!Program.getMiscMenuCB("eFarmAssist")) return;
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                target.Position.LSDistance(ObjectManager.Player.Position) < 650 &&
                (_recentFarmTarget == null || target.NetworkId != _recentFarmTarget.NetworkId))
            {
                Cast(target);
            }
        }

        public override int GetPriority()
        {
            return Provider.Target != null ? (Provider.Target.HasBuff("brandablaze") ? 0 : 4) : 0;
        }
    }
}