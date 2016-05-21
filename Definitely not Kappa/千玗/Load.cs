namespace KappaKindred
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;

    using Events;

    internal class Load
    {
        public static void Execute()
        {
            if (Player.Instance.ChampionName != "Kindred")
            {
                return;
            }

            Menu.Load();
            Spells.Load();

            Game.OnUpdate += OnUpdate.Update;
            Drawing.OnDraw += OnDraw.Draw;
            Gapcloser.OnGapcloser += OnGapcloser.Gapcloser_OnGapcloser;
            Orbwalker.OnPostAttack += OnPostAttack.PostAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast.OnSpell;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack.OnAttack;
            AttackableUnit.OnDamage += OnDamage.Damage;
        }
    }
}