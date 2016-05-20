using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AutoBuddy.Humanizers;
using AutoBuddy.Utilities;
using AutoBuddy.Utilities.AutoShop;
using AutoBuddy.Utilities.Pathfinder;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace AutoBuddy
{
    internal static class AutoWalker
    {
        public static string GameID;
        public static Spell.Active Ghost, Barrier, Heal, B;
        public static Spell.Skillshot Flash;
        public static Spell.Targeted Teleport, Ignite, Smite, Exhaust;
        public static readonly Obj_HQ MyNexus;
        public static readonly Obj_HQ EneMyNexus;
        public static readonly AIHeroClient myHero;
        public static readonly Obj_AI_Turret EnemyLazer;
        private static Orbwalker.ActiveModes activeMode = Orbwalker.ActiveModes.None;
        private static InventorySlot seraphs;
        private static int hpSlot;
        private static readonly ColorBGRA color;
        private static List<Vector3> PfNodes;
        private static readonly NavGraph NavGraph;
        private static bool oldWalk;
        public static bool newPF;
        public static EventHandler EndGame;
        static AutoWalker()
        {
            GameID = DateTime.Now.Ticks + ""+RandomString(10);
            newPF = MainMenu.GetMenu("AB").Get<CheckBox>("newPF").CurrentValue;
            NavGraph=new NavGraph(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline"));
            PfNodes=new List<Vector3>();
            color = new ColorBGRA(79, 219, 50, 255);
            MyNexus = ObjectManager.Get<Obj_HQ>().First(n => n.IsAlly);
            EneMyNexus = ObjectManager.Get<Obj_HQ>().First(n => n.IsEnemy);
            EnemyLazer =
                ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(tur => !tur.IsAlly && tur.GetLane() == Lane.Spawn);
            myHero = ObjectManager.Player;
            initSummonerSpells();

            Target = ObjectManager.Player.Position;
            Orbwalker.DisableMovement = false;

            Orbwalker.DisableAttacking = false;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OverrideOrbwalkPosition = () => Target;
            if (Orbwalker.HoldRadius > 130 || Orbwalker.HoldRadius < 80)
            {
                Chat.Print("=================WARNING=================", Color.Red);
                Chat.Print("Your hold radius value in orbwalker isn't optimal for AutoBuddy", Color.Aqua);
                Chat.Print("Please set hold radius through menu=>Orbwalker");
                Chat.Print("Recommended values: Hold radius: 80-130, Delay between movements: 100-250");
            }
            
            Drawing.OnDraw += Drawing_OnDraw;
            
            Core.DelayAction(OnEndGame, 20000);
            updateItems();
            oldOrbwalk();
            Game.OnTick += OnTick;
        }

        public static bool Recalling()
        {
            return myHero.IsRecalling();
        }

        private static void OnEndGame()
        {

            if (MyNexus != null && EneMyNexus != null && (MyNexus.Health > 1) && (EneMyNexus.Health > 1))
            {
                Core.DelayAction(OnEndGame, 5000);
                return;
            }

            if (EndGame != null)
                EndGame(null, new EventArgs());

            if (MainMenu.GetMenu("AB").Get<CheckBox>("autoclose").CurrentValue)
            Core.DelayAction(() => { Game.QuitGame(); }, 15000);

        }

        public static Vector3 Target { get; private set; }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (activeMode == Orbwalker.ActiveModes.LaneClear)
            {
                Orbwalker.ActiveModesFlags = (myHero.TotalAttackDamage < 150 &&
                    EntityManager.MinionsAndMonsters.EnemyMinions.Any(
                        en =>
                            en.Distance(myHero) < myHero.AttackRange + en.BoundingRadius &&
                            Prediction.Health.GetPrediction(en, 2000) < myHero.GetAutoAttackDamage(en))
                        ) ? Orbwalker.ActiveModes.Harass
                        : Orbwalker.ActiveModes.LaneClear;
            }
            else
                Orbwalker.ActiveModesFlags = activeMode;
        }

        public static void SetMode(Orbwalker.ActiveModes mode)
        {
            if (activeMode != Orbwalker.ActiveModes.Combo)
                Orbwalker.DisableAttacking = false;
            activeMode = mode;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                return;

            Circle.Draw(color,40, Target );
            for (int i = 0; i < PfNodes.Count-1; i++)
            {
                if(PfNodes[i].IsOnScreen()||PfNodes[i+1].IsOnScreen())
                    Line.DrawLine(Color.Aqua, 4, PfNodes[i], PfNodes[i+1]);
            }
        }

        public static void WalkTo(Vector3 tgt)
        {
            if (!newPF)
            {
                Target = tgt;
                return;
            }

            if (PfNodes.Any())
            {
                float dist = tgt.Distance(PfNodes[PfNodes.Count - 1]);
                if (dist > 900 || dist > 300 && myHero.Distance(tgt) < 2000)
                {
                    PfNodes = NavGraph.FindPathRandom(myHero.Position, tgt);
                }
                else
                {
                    PfNodes[PfNodes.Count - 1] = tgt;
                }
                Target = PfNodes[0];
            }
            else
            {
                if (tgt.Distance(myHero) > 900)
                {
                    PfNodes = NavGraph.FindPathRandom(myHero.Position, tgt);
                    Target = PfNodes[0];
                }
                else
                {
                    Target = tgt;
                }
            }
        }

        private static void updateItems()
        {
            hpSlot = BrutalItemInfo.GetHPotionSlot();
            seraphs = myHero.InventoryItems.FirstOrDefault(it => (int)it.Id == 3040);
            Core.DelayAction(updateItems, 5000);
            
        }
        public static void UseSeraphs()
        {
            if (seraphs != null && seraphs.CanUseItem())
                seraphs.Cast();
        }
        public static void UseGhost()
        {
            if (Ghost != null && Ghost.IsReady())
                Ghost.Cast();
        }
        public static void UseHPot()
        {
            if (hpSlot == -1)
                return;
            myHero.InventoryItems[hpSlot].Cast();
            hpSlot = -1;
        }
        public static void UseBarrier()
        {
            if (Barrier != null && Barrier.IsReady())
                Barrier.Cast();
        }
        public static void UseHeal()
        {
            if (ObjectManager.Player.CountEnemiesInRange(900) >= 1 && Heal != null && Heal.IsReady())
                Heal.Cast();
        }
        public static void UseIgnite(AIHeroClient target)
        {
            if (Ignite == null || !Ignite.IsReady() || target == null) 
                return;
            
            target = EntityManager.Heroes.Enemies.Where(en => en.Distance(myHero) < 600 + en.BoundingRadius)
                        .OrderBy(en => en.Health)
                        .FirstOrDefault();
            if (target != null && myHero.Distance(target) < 600 + target.BoundingRadius)
            {
                Ignite.Cast(target);
            }
                
        }

        private static void initSummonerSpells()
        {
            B = new Spell.Active(SpellSlot.Recall);

            Ghost = Player.Spells.FirstOrDefault(sp => sp.SData.Name.Contains("summonerhaste")) == null ? null : new Spell.Active(ObjectManager.Player.GetSpellSlotFromName("summonerhaste"));
            Flash = Player.Spells.FirstOrDefault(sp => sp.SData.Name.Contains("summonerflash")) == null ? null : new Spell.Skillshot(ObjectManager.Player.GetSpellSlotFromName("summonerflash"), 600, SkillShotType.Circular);
            Ignite = Player.Spells.FirstOrDefault(sp => sp.SData.Name.Contains("summonerdot")) == null ? null : new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            Exhaust = Player.Spells.FirstOrDefault(sp => sp.SData.Name.Contains("summonerexhaust")) == null ? null : new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerexhaust"), 600);
            Teleport = Player.Spells.FirstOrDefault(sp => sp.SData.Name.Contains("summonerteleport")) == null ? null : new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerteleport"), int.MaxValue);
            Smite = Player.Spells.FirstOrDefault(sp => sp.SData.Name.Contains("smite")) == null ? null : new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("smite"), 600);

            /*Ghost = new Spell.Active(ObjectManager.Player.GetSpellSlotFromName("summonerhaste"));
            Flash = new Spell.Skillshot(ObjectManager.Player.GetSpellSlotFromName("summonerflash"), 600, SkillShotType.Circular);
            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            Exhaust = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerexhaust"), 600);
            Teleport = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerteleport"), int.MaxValue);
            Smite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("smite"), 600);
*/
            Heal = new Spell.Active(ObjectManager.Player.GetSpellSlotFromName("summonerheal"), 600);
            Barrier = new Spell.Active(ObjectManager.Player.GetSpellSlotFromName("summonerbarrier"));
        }

#region old orbwalking, for those with not working orbwalker

        private static int maxAdditionalTime = 50;
        private static int adjustAnimation = 20;
        private static float holdRadius = 50;
        private static float movementDelay = .25f;

        private static float nextMove;



        private static void oldOrbwalk()
        {

            if (!MainMenu.GetMenu("AB").Get<CheckBox>("oldWalk").CurrentValue) return;
            oldWalk = true;
            Orbwalker.OnPreAttack+=Orbwalker_OnPreAttack;
        }


        private static void Orbwalker_OnPreAttack(AttackableUnit tgt, Orbwalker.PreAttackArgs args)
        {
            nextMove = Game.Time + ObjectManager.Player.AttackCastDelay +
                       (Game.Ping + adjustAnimation + RandGen.r.Next(maxAdditionalTime)) / 1000f;
        }

        private static void OnTick(EventArgs args)
        {
            if (PfNodes.Count != 0)
            {
                Target = PfNodes[0];
                if (ObjectManager.Player.Distance(PfNodes[0]) < 600)
                {
                    PfNodes.RemoveAt(0);
                    
                }

            }



            if (!oldWalk||ObjectManager.Player.Position.Distance(Target) < holdRadius || Game.Time < nextMove) return;
            nextMove = Game.Time + movementDelay;
            Player.IssueOrder(GameObjectOrder.MoveTo, Target, true);



        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }

#endregion

}