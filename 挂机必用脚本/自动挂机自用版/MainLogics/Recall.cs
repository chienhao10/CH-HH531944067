using System;
using System.Linq;
using AutoBuddy.Humanizers;
using AutoBuddy.Utilities.AutoShop;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AutoBuddy.MainLogics
{
    internal class Recall
    {
        private readonly Slider flatGold, goldPerLevel;
        private readonly LogicSelector current;
        private readonly Obj_SpawnPoint spawn;
        private bool active;
        private GrassObject g;
        //private float lastRecallGold;
        private float lastRecallTime;
        private int recallsWithGold; //TODO repair shop and remove this tempfix

        public Recall(LogicSelector currentLogic, Menu parMenu)
        {
            Menu menu = parMenu.AddSubMenu("Recall settings", "ergtrh");
            flatGold=new Slider("Minimum base gold to recall", 560, 0, 4000);
            goldPerLevel = new Slider("Minmum gold per level to recall", 70, 0, 300);
            menu.Add("mingold", flatGold);
            menu.Add("goldper", goldPerLevel);
            menu.AddSeparator(100);
            menu.AddLabel(
    @"
Example: Your champ has lvl 10
Base gold = 560
Gold per level = 70
Minimum gold = 560+70*10 = 1260

AutoBuddy won't recall if you have less gold than needed for next item.

            ");
            current = currentLogic;
            foreach (
                Obj_SpawnPoint so in
                    ObjectManager.Get<Obj_SpawnPoint>().Where(so => so.Team == ObjectManager.Player.Team))
            {
                spawn = so;
            }
            Core.DelayAction(ShouldRecall, 3000);
            Drawing.OnDraw += Drawing_OnDraw;
        }


        private void ShouldRecall()
        {
            if (active)
            {
                Core.DelayAction(ShouldRecall, 500);
                return;
            }
            if (current.current == LogicSelector.MainLogics.CombatLogic)
            {
                Core.DelayAction(ShouldRecall, 500);
                return;
            }

            if ((AutoWalker.myHero.Gold > flatGold.CurrentValue+AutoWalker.myHero.Level*goldPerLevel.CurrentValue&&AutoWalker.myHero.Gold>ShopGlobals.GoldForNextItem && AutoWalker.myHero.InventoryItems.Length < 8 &&
                 recallsWithGold <= 30) || AutoWalker.myHero.HealthPercent() < 25)
            {
                if (AutoWalker.myHero.Gold > (AutoWalker.myHero.Level + 2)*150 && AutoWalker.myHero.InventoryItems.Length < 8 &&
                    recallsWithGold <= 30)
                    recallsWithGold++;
                current.SetLogic(LogicSelector.MainLogics.RecallLogic);
            }
            Core.DelayAction(ShouldRecall, 500);
        }

        public void Activate()
        {
            if (active) return;
            active = true;
            g = null;
            Game.OnTick += Game_OnTick;
        }

        public void Deactivate()
        {
            lastRecallTime = 0;
            active = false;
            Game.OnTick -= Game_OnTick;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                return;

            Drawing.DrawText(250, 55, System.Drawing.Color.Gold,
                "Recall, active: " + active+" next item: "+ShopGlobals.Next+" gold needed:"+ShopGlobals.GoldForNextItem);
        }

        private void Game_OnTick(EventArgs args)
        {
            AutoWalker.SetMode(Orbwalker.ActiveModes.Combo);
            if (ObjectManager.Player.Distance(spawn) < 400 && ObjectManager.Player.HealthPercent() > 85 &&
                (ObjectManager.Player.ManaPercent > 80 || ObjectManager.Player.PARRegenRate <= .0001))

                current.SetLogic(LogicSelector.MainLogics.PushLogic);
            else if (ObjectManager.Player.Distance(spawn) < 2000)
                AutoWalker.WalkTo(spawn.Position);
            else if (!ObjectManager.Player.IsRecalling() && Game.Time > lastRecallTime)
            {
                Obj_AI_Turret nearestTurret =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(t => t.Team == ObjectManager.Player.Team && !t.IsDead())
                        .OrderBy(t => t.Distance(ObjectManager.Player))
                        .First();
                Vector3 recallPos = nearestTurret.Position.Extend(spawn, 300).To3DWorld();
                if (AutoWalker.myHero.HealthPercent() > 35)
                {
                    if (g == null)
                    {

                        g = ObjectManager.Get<GrassObject>()
                            .Where(gr => gr.Distance(AutoWalker.MyNexus) < AutoWalker.myHero.Distance(AutoWalker.MyNexus)&&gr.Distance(AutoWalker.myHero)>Orbwalker.HoldRadius)
                            .OrderBy(gg => gg.Distance(AutoWalker.myHero)).FirstOrDefault(gr => ObjectManager.Get<GrassObject>().Count(gr2=>gr.Distance(gr2)<65)>=4);
                    }
                    if (g != null && g.Distance(AutoWalker.myHero) < nearestTurret.Position.Distance(AutoWalker.myHero))
                    {
                        AutoWalker.SetMode(Orbwalker.ActiveModes.Flee);
                        recallPos = g.Position;
                    }
                }

                if ((!AutoWalker.myHero.IsMoving && ObjectManager.Player.Distance(recallPos) < Orbwalker.HoldRadius + 30) || (AutoWalker.myHero.IsMoving && ObjectManager.Player.Distance(recallPos) < 30))
                {
                    CastRecall();
                }
                else
                    AutoWalker.WalkTo(recallPos);
            }
        }

        private void CastRecall()
        {
            if (Game.Time < lastRecallTime || AutoWalker.Recalling() || ObjectManager.Player.Distance(spawn) < 500) return;
            lastRecallTime = Game.Time + 2f;
            Core.DelayAction(CastRecall2, 300);
        }

        private void CastRecall2()//Kappa
        {
            if (AutoWalker.Recalling() || ObjectManager.Player.Distance(spawn) < 500) return;
            lastRecallTime = Game.Time + 2f;
            AutoWalker.SetMode(Orbwalker.ActiveModes.None);
            if (!AutoWalker.Recalling())
            {
                if (AutoWalker.B.IsReady())
                {
                    AutoWalker.B.Cast();
                }
            }  
        }
    }
}