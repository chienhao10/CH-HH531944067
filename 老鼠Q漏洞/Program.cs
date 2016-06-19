namespace TwitchExploit
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;

    public static class Program
    {
        public static SpellDataInst Q;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(System.EventArgs args)
        {
            Chat.Print("Twitch Exploit Loaded !");
            Q = Player.GetSpell(SpellSlot.Q);
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target != null && args.Target.IsEnemy && args.Target is AIHeroClient && sender.IsAlly && sender != null)
            {
                var target = (AIHeroClient)args.Target;
                if (target != null && target.Buffs.Any(b => b.Name.ToLower().Equals("twitchdeadlyvenom")))
                {
                    var death = sender.GetAutoAttackDamage(target, true) >= target.Health;
                    if (death)
                    {
                        Player.CastSpell(Q.Slot);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target != null && args.Target.IsEnemy && args.Target is AIHeroClient && sender.IsAlly && sender != null)
            {
                var caster = sender as AIHeroClient;
                var target = (AIHeroClient)args.Target;
                if (target != null && caster != null && target.Buffs.Any(b => b.Name.ToLower().Equals("twitchdeadlyvenom")))
                {
                    var spelldamage = caster.GetSpellDamage(target, args.Slot);
                    var damagepercent = (spelldamage / target.Health) * 100;
                    var death = damagepercent >= target.HealthPercent || spelldamage >= target.Health || caster.GetAutoAttackDamage(target, true) >= target.Health;
                    if (death)
                    {
                        Player.CastSpell(Q.Slot);
                    }
                }
            }
        }
    }
}