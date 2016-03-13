using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace AkaYasuo
{
    internal class EventManager
    {
        public static void load()
        {
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            Modes.PermaActive.AutoR();
            Modes.PermaActive.KillSteal();
            Modes.PermaActive.sChoose();

            Events._game.AutoQ();
            Events._game.LevelUpSpells();
            Events._game.StackQ();

            if (MenuManager.FleeMenu["WJ"].Cast<KeyBind>().CurrentValue)
            {
                Yasuo.WallDash();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Modes.Harass.Harass2(); 
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Modes.Clear.NewLane();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Modes.Flee.Flee2();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) Modes.Clear.NewLane();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) Modes.LastHit.Lasthitmode();
            Modes.Combo.Load();
        }

        private static void OnDraw(EventArgs args)
        {
            Events._drawing.Drawings(args);
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs buff)
        {
            Events._objaibase.BuffGain(sender, buff);
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs buff)
        {
            Events._objaibase.BuffLose(sender, buff);
        }

        private static void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            Events._objaibase.Delete(sender, args);
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            Events._objaibase.Create(sender, args);
        }

        private static void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            Events._spellbook.StopCast(sender, args);
        }
    }
}
