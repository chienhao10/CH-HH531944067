using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;
using SharpDX;

namespace ARAMDetFull.Champions
{
    class Jarvan : Champion
    {
        private Spell Q2;
        private static bool _rCasted;

        public Jarvan()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Sunfire_Cape, ItemId.Banshees_Veil, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Frozen_Mallet),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Last_Whisper),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Phage
                }
            };
            LXOrbwalker.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Chat.Print("Jarvan in");
        }

        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "JarvanIVCataclysm")
                {
                    _rCasted = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(3500, () => _rCasted = false);
                }
            }
            

            if (!W.IsReady() || sender == null || args.Target == null || sender.IsAlly || !(sender is AIHeroClient) || !(args.Target.IsMe))
                return;
                W.Cast();


        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {

        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (R.IsReady() && _rCasted && ARAMSimulator.balance < 100)
                R.Cast();

            if (target == null || !R.IsReady() || !safeGap(target) || _rCasted)
                return;
            R.CastOnUnit(target);


        }

        public override void useSpells()
        {
            if (player.IsChannelingImportantSpell())
                return;
            var tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null && E.IsReady() && safeGap(tar))
                if (tar != null) useE(tar);

            tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 770);
            Q2 = new Spell(SpellSlot.Q, 880);
            W = new Spell(SpellSlot.W, 520);
            E = new Spell(SpellSlot.E, 860, DamageType.Magical);
            R = new Spell(SpellSlot.R, 650);
            Q.SetSkillshot(0.6f, 70, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(0.25f, 140, 1450, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 175, float.MaxValue, false, SkillshotType.SkillshotCircle);

        }


        public override void farm()
        {
            if (player.ManaPercent < 65)
                return;

            var AllMinions = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            foreach (var minion in AllMinions)
            {
                if (Q.IsReady() && Q.GetDamage(minion) > minion.Health)
                {
                    Q.Cast(minion);
                    return;
                }
                if (E.IsReady() && E.GetDamage(minion) > minion.Health)
                {
                    E.Cast(minion);
                }
            }
        }
    }
}
