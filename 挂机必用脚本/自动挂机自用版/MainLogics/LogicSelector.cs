using System;
using System.Collections.Generic;
using AutoBuddy.MyChampLogic;
using AutoBuddy.Utilities;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace AutoBuddy.MainLogics
{
    internal class LogicSelector
    {
        public readonly Combat combatLogic;
        public readonly Load loadLogic;
        public readonly LocalAwareness localAwareness;
        public readonly Push pushLogic;
        public readonly Recall recallLogic;
        public readonly MiscEvents GameEvents;
        public readonly Survi surviLogic;


        public readonly IChampLogic myChamp;
        public bool saveMylife;

        public LogicSelector(IChampLogic my, Menu menu)
        {
            myChamp = my;
            current = MainLogics.Nothing;
            surviLogic = new Survi(this);
            recallLogic = new Recall(this, menu);
            pushLogic = new Push(this);
            loadLogic = new Load(this);
            combatLogic = new Combat(this);
            GameEvents = new MiscEvents();

            Core.DelayAction(() => loadLogic.SetLane(), 1000);
            localAwareness = new LocalAwareness();
            
            Drawing.OnEndScene += Drawing_OnDraw;
            myChamp.Logic = this;
            AutoWalker.EndGame += end;
            Core.DelayAction(Watchdog, 3000);
        }

        public MainLogics current { get; set; }

        private void Drawing_OnDraw(System.EventArgs args)
        {
            if (!MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                return;

            Drawing.DrawText(250, 85, Color.Gold, current.ToString());
            Vector2 v = Game.CursorPos.WorldToScreen();
            Drawing.DrawText(v.X, v.Y - 20, Color.Gold, localAwareness.LocalDomination(Game.CursorPos) + " ");
        }

        public MainLogics SetLogic(MainLogics newlogic)
        {
            if (saveMylife) return current;
            if (newlogic != MainLogics.PushLogic)
                pushLogic.Deactivate();
            MainLogics old = current;
            switch (current)
            {
                case MainLogics.SurviLogic:
                    surviLogic.Deactivate();
                    break;

                case MainLogics.RecallLogic:
                    recallLogic.Deactivate();
                    break;
                case MainLogics.CombatLogic:
                    combatLogic.Deactivate();
                    break;
            }


            switch (newlogic)
            {
                case MainLogics.PushLogic:
                    pushLogic.Activate();
                    break;
                case MainLogics.LoadLogic:
                    loadLogic.Activate();
                    break;
                case MainLogics.SurviLogic:
                    surviLogic.Activate();

                    break;
                case MainLogics.RecallLogic:
                    recallLogic.Activate();
                    break;
                case MainLogics.CombatLogic:
                    combatLogic.Activate();
                    break;
            }


            current = newlogic;
            return old;
        }

        private void Watchdog()
        {
            Core.DelayAction(Watchdog, 500);
            if (current == MainLogics.Nothing && !loadLogic.waiting)
            {
                Chat.Print("Hang detected");
                loadLogic.SetLane();
            }
        }

        private void end(object o, EventArgs e)
        {

        }
        internal enum MainLogics
        {
            PushLogic,
            RecallLogic,
            LoadLogic,
            SurviLogic,
            CombatLogic,
            Nothing
        }
    }
}