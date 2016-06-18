using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace WuAIO.Bases.Interface
{
    interface IHero
    {
        //Initializing
        void CreateMenu();
        void CreateVariables();
        void TriggerEvents();

        //Orbwalker modes and others
        void Combo();
        void Harass();
        void LaneClear();
        void JungleClear();
        void LastHit();
        void Flee();
        void PermaActive();
        void KS();

        //Events
        void Game_OnTick(EventArgs args);
        void Drawing_OnDraw(EventArgs args);
        void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e);
        void Orbwalker_OnAttack(AttackableUnit target, EventArgs args);
        void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args);
        void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args);
        void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e);
        void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args);
        void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args);
        void Obj_AI_Turret_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args);
        void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args);
        void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e);
    }
}
