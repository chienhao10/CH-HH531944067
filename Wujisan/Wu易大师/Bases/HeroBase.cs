using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Events;
using WuAIO.Bases.Interface;
using WuAIO.Managers;

namespace WuAIO.Bases
{
    abstract class HeroBase : IHero
    {
        public static DamageManager damageManager { get; set; }

        public static string HERO_MENU { get { return "Wu" + EloBuddy.Player.Instance.ChampionName; } }
        protected Menu flee;
        protected Menu misc;
        protected Menu draw;
        protected Menu combo;
        protected Menu harass;
        protected Menu lasthit;
        protected Menu laneclear;
        protected Menu jungleclear;

        readonly AIHeroClient Player = EloBuddy.Player.Instance;

        protected HeroBase()
        {
            CreateMenu();

            flee = GetMenu("Flee");
            misc = GetMenu("Misc");
            combo = GetMenu("Combo");
            draw = GetMenu("Drawings");
            harass = GetMenu("Harass");
            lasthit = GetMenu("Last Hit");
            laneclear = GetMenu("Lane Clear");
            jungleclear = GetMenu("Jungle Clear");

            CreateVariables();
            TriggerEvents();
        }

        private Menu GetMenu(string displayName)
        {
            if (MenuManager.Menus[HERO_MENU].Keys.Any(it => it == displayName))
                return MenuManager.Menus[HERO_MENU][displayName].Keys.Last();

            return null;
        }

        public virtual void CreateMenu()
        {
            MenuManager.Init(HERO_MENU);
        }

        public virtual void TriggerEvents()
        {
            Game.OnTick += Game_OnTick;
            Dash.OnDash += Dash_OnDash;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnAttack += Orbwalker_OnAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Obj_AI_Turret.OnBasicAttack += Obj_AI_Turret_OnBasicAttack;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        public virtual void CreateVariables()
        {
            
        }

        public virtual void KS()
        {
            
        }

        public virtual void Flee()
        {
            
        }

        public virtual void Combo()
        {
            
        }

        public virtual void Harass()
        {
            
        }

        public virtual void LastHit()
        {
            
        }

        public virtual void LaneClear()
        {
            
        }

        public virtual void JungleClear()
        {
            
        }

        public virtual void PermaActive()
        {
            KS();
        }

        public virtual void Game_OnTick(EventArgs args)
        {
            if (Player.IsDead) return;

            PermaActive();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Flee();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) LastHit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) JungleClear();
        }

        public virtual void Drawing_OnDraw(EventArgs args)
        {
            //if (Player.IsDead || draw.IsActive("disable")) return;
        }

        public virtual void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            //if (Player.IsDead || !sender.IsEnemy) return;
        }

        public virtual void Orbwalker_OnAttack(AttackableUnit target, EventArgs args)
        {
            //if (Player.IsDead) return;
        }

        public virtual void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            //if (Player.IsDead || Orbwalker.LastTarget != target) return;
        }

        public virtual void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {

        }

        public virtual void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            //if (Player.IsDead || !sender.IsEnemy || !misc.IsActive("gapcloser")) return;
        }

        public virtual void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            
        }

        public virtual void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            //if (Player.IsDead) return;
        }

        public virtual void Obj_AI_Turret_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if (Player.IsDead || !(sender is Obj_AI_Turret)) return;
        }

        public virtual void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if (Player.IsDead || !(sender is AIHeroClient)) return;
        }

        public virtual void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            //if (Player.IsDead || !sender.IsEnemy || !misc.IsActive("interrupter")) return;
        }
    }
}
