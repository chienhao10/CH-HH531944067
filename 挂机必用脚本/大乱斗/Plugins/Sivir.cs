using System;
using System.Windows.Forms;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Enumerations;

namespace AutoSharp.Plugins
{
    public static class Sivir
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Active E { get; private set; }
        public static Spell.Active R { get; private set; }
        public static int LastAggersiveSpell { get; set; }
        public static void Init()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1250, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 250, 1350, 90);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Active(SpellSlot.R);
            Q.MinimumHitChance = HitChance.High;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && !args.SData.IsAutoAttack() && args.Target != null && args.Target.NetworkId == ObjectManager.Player.NetworkId)
            {
                //Chat.Print("Lel");
                LastAggersiveSpell = Environment.TickCount;
            }
            //if (sender.IsEnemy && !args.SData.IsAutoAttack() && args.Target != null && args.Target.NetworkId == ObjectManager.Player.NetworkId)
            //{
            //    LastAggersiveSpell = Environment.TickCount;
            //}
            if (sender.IsMe && args.SData.Name == W.Name)
            {
                Orbwalker.ResetAutoAttack();
            }
            if (sender.IsMe && args.SData.Name == Q.Name)
            {
                Orbwalker.ResetAutoAttack();
            }
        }

        

        static void Orbwalker_OnPostAttack(AttackableUnit target, System.EventArgs args)
        {
            
            var target2 = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, DamageType.Physical);
            if (target2 != null && W.IsReady())
            {
                W.Cast();
                return;
            }
            target2 = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target2 != null && Q.IsReady())
            {
                Q.Cast(target2);
                return;
            }
        }

        static void Game_OnUpdate(System.EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
        }

        private static void Combo()
        {
            if (LastAggersiveSpell + 500 > Environment.TickCount && E.IsReady())
            {
                //Chat.Print("LASTAS");
                E.Cast();
            }
            var target2 = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target2 != null && Q.IsReady())
            {
                Q.Cast(target2);
                return;
            }
        }
    }
}