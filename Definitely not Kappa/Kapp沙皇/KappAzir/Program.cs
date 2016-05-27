namespace KappAzir
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;

    using Modes;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Azir)
            {
                return;
            }

            Azir.Execute();
            Menus.Execute();
            Game.OnTick += Base.Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Base.Obj_AI_Base_OnProcessSpellCast;
            Gapcloser.OnGapcloser += Base.Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Base.Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Base.Drawing_OnDraw;
            GameObject.OnCreate += Base.GameObject_OnCreate;
            Orbwalker.OnPreAttack += Base.Orbwalker_OnPreAttack;
        }
    }
}