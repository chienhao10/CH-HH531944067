using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace HumanziedBaseUlt
{
    class Main : Events
    {
        private readonly AIHeroClient me = ObjectManager.Player;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local       

        public Main()
        {
            Listing.config = MainMenu.AddMenu("HumanizedBaseUlts", "humanizedBaseUlts");
            Listing.config.Add("on", new CheckBox("Enabled"));
            Listing.config.Add("min20", new CheckBox("20 min passed"));
            Listing.config.Add("minDelay", new Slider("Minimum ultimate delay", 1000, 0, 2500));
            Listing.config.AddLabel("The time to let the enemy regenerate health in base");
            Listing.config.Add("humanizedDelayOff", new CheckBox("TURN OFF HUMANIZED DELAY AT ALL", false));

            Listing.config.AddSeparator(20);
            Listing.config.Add("fountainReg", new Slider("Fountain regeneration speed", 89, 50, 100));
            Listing.config.Add("fountainRegMin20", new Slider("Fountain regeneration speed after minute 20", 331, 300, 350));


            Listing.potionMenu = Listing.config.AddSubMenu("Potions");
            Listing.potionMenu.AddLabel("[Regeneration Speed in HP/Sec.]");
            Listing.potionMenu.Add("healPotionRegVal", new Slider("Heal Potion / Cookie", 11, 5, 20));
            Listing.potionMenu.Add("crystalFlaskRegVal", new Slider("Crystal Flask", 10, 5, 20));
            Listing.potionMenu.Add("crystalFlaskJungleRegVal", new Slider("Crystal Flask Jungle", 9, 5, 20));
            Listing.potionMenu.Add("darkCrystalFlaskVal", new Slider("Dark Crystal Flask", 16, 5, 20));


            Listing.snipeMenu = Listing.config.AddSubMenu("Enemy Recall Snipe");
            Listing.snipeMenu.AddLabel("[Premade feature added, too]");

            Listing.snipeMenu.Add("snipeEnabled", new CheckBox("Enabled"));
            AddStringList(Listing.snipeMenu, "minSnipeHitChance", "Minimum Snipe HitChance", 
                new []{ "Impossible", "Low", "Above Average", "Very High"}, 2);
            Listing.snipeMenu.Add("snipeDraw", new CheckBox("Draw Snipe paths"));
            Listing.snipeMenu.Add("snipeCinemaMode", new CheckBox("Cinematic mode ™"));

            Listing.allyconfig = Listing.config.AddSubMenu("Premades");
            foreach (var ally in EntityManager.Heroes.Allies)
            {
                bool isKarthus = ally.ChampionName.ToLower().Contains("karthus");
                if (Listing.UltSpellDataList.Any(x => x.Key == ally.ChampionName))
                    Listing.allyconfig.Add(ally.ChampionName + "/Premade", new CheckBox(!isKarthus ? ally.ChampionName :
                       ally.ChampionName + " (Only for premade damage)", ally.IsMe));
            }
            

            Listing.MiscMenu = Listing.config.AddSubMenu("Miscellaneous");
            Listing.MiscMenu.AddLabel("[Draven]");
            Listing.MiscMenu.Add("dravenCastBackBool", new CheckBox("Enable 'Draven Cast Back'"));
            Listing.MiscMenu.Add("dravenCastBackDelay", new Slider("Cast Back X ms earlier", 400, 0, 500));

            Listing.MiscMenu.AddSeparator();
            Listing.MiscMenu.Add("allyMessaging", new CheckBox("Send information to premades"));
            Listing.MiscMenu.AddLabel("If only 1 person uses this addon the others get infromed via chat whisper");

            RecallTracker.RecallTracker.Init();

            Game.OnUpdate += GameOnOnUpdate;
            Teleport.OnTeleport += TeleportOnOnTeleport;
        }

        private void AddStringList(Menu m, string uniqueId, string displayName, string[] values, int defaultValue)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = displayName + ": " + values[args.NewValue];
            };
        }

        private void TeleportOnOnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var invisEnemiesEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender == sender);

            switch (args.Status)
            {
                case TeleportStatus.Start:
                    if (invisEnemiesEntry == null)
                        return;
                    if (Listing.teleportingEnemies.All(x => x.Sender != sender))
                    {
                        Listing.teleportingEnemies.Add(new Listing.PortingEnemy
                        {
                            Sender = (AIHeroClient) sender,
                            Duration = args.Duration,
                            StartTick = args.Start,
                        });
                    }
                    break;
                case TeleportStatus.Abort:
                    var teleportingEnemiesEntry = Listing.teleportingEnemies.FirstOrDefault(x => x.Sender.Equals(sender));
                    if (teleportingEnemiesEntry != null)
                        Listing.teleportingEnemies.Remove(teleportingEnemiesEntry);
                    break;

                case TeleportStatus.Finish:
                    var teleportingEnemiesEntry2 = Listing.teleportingEnemies.FirstOrDefault(x => x.Sender.Equals(sender));
                    if (teleportingEnemiesEntry2 != null)
                        Core.DelayAction(() => Listing.teleportingEnemies.Remove(teleportingEnemiesEntry2), 10000);
                    break;
            }
        }
              

        private void OnOnEnemyVisible(AIHeroClient sender)
        {
            Listing.visibleEnemies.Add(sender);
            var invisEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender.Equals(sender));

            if (invisEntry != null)
                Listing.invisEnemiesList.Remove(invisEntry);

            var snipeEntry = Listing.Pathing.enemySnipeProcs.FirstOrDefault(x => x.target == sender);

            if (snipeEntry != null)
            {
                snipeEntry.CancelProcess();
                Listing.Pathing.enemySnipeProcs.Remove(snipeEntry);
            }
        }
        

        private void OnOnEnemyInvisible(InvisibleEventArgs args)
        {
            Listing.visibleEnemies.Remove(args.sender);
            Listing.invisEnemiesList.Add(args);

            if (Listing.snipeMenu["snipeEnabled"].Cast<CheckBox>().CurrentValue)
            {
                var spell = me.Spellbook.GetSpell(SpellSlot.R);
                var cooldown = spell.CooldownExpires - Game.Time;

                if (cooldown <= 0 && me.Level >= 6)
                    Listing.Pathing.enemySnipeProcs.Add(new SnipePrediction(args));
            }
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            Listing.config.Get<CheckBox>("min20").CurrentValue = Game.Time > 1221;

            UpdateEnemyVisibility();
            Listing.Pathing.UpdateEnemyPaths();
            CheckRecallingEnemies();
        }

        Vector3 enemySpawn {
            get { return ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position; }
        }

        private void CheckRecallingEnemies()
        {
            if (!Listing.config.Get<CheckBox>("on").CurrentValue)
                return;

            foreach (Listing.PortingEnemy portingEnemy in Listing.teleportingEnemies.OrderBy(x => x.Sender.Health))
            {
                var enemy = portingEnemy.Sender;
                InvisibleEventArgs invisEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender.Equals(enemy));

                if (invisEntry == null) //enemy visible
                    return;

                int recallEndTime = portingEnemy.StartTick + portingEnemy.Duration;
                float timeLeft = recallEndTime - Core.GameTickCount;
                float regedHealthRecallFinished = Algorithm.SimulateHealthRegen(enemy, invisEntry.StartTime, recallEndTime);
                float totalEnemyHOnRecallEnd = enemy.Health + regedHealthRecallFinished;
                float aioDmg = Damage.GetAioDmg(enemy, timeLeft, enemySpawn, totalEnemyHOnRecallEnd);
                float regenerationDelayTime = Algorithm.SimulateRealDelayTime(enemy, recallEndTime, aioDmg);

                bool force0Delay = Listing.config.Get<CheckBox>("humanizedDelayOff").CurrentValue;
                if (force0Delay)
                    regenerationDelayTime = 0;

                if (!Damage.isRegenDelaySet)
                {
                    Damage.SetRegenerationDelay(regenerationDelayTime, timeLeft);
                    //new check but now with estimated reg delay
                    float totalEnemyHpAtArrival = totalEnemyHOnRecallEnd +
                                                  (regenerationDelayTime/1000)*Algorithm.GetFountainReg(enemy);
                    aioDmg = Damage.GetAioDmg(enemy, timeLeft, enemySpawn, totalEnemyHpAtArrival);
                }

                if (aioDmg > totalEnemyHOnRecallEnd)
                {
                    if (regenerationDelayTime < Listing.config.Get<Slider>("minDelay").CurrentValue &&
                        !Listing.config.Get<CheckBox>("humanizedDelayOff").CurrentValue)
                    {
                        Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.DelayTooSmall, regenerationDelayTime);
                        continue;
                    }

                    CheckUltCast(enemy, timeLeft, aioDmg, regenerationDelayTime);
                    return; //clear every porting enemy expect the current target and return => wait for next loop run
                }

                if (aioDmg > 0)  //dmg there but not enough
                {
                    Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.NotEnougDamage, aioDmg);
                }
            }
        }

        private void CheckUltCast(AIHeroClient enemy, float timeLeft, float aioDmg, float regenerationDelayTime)
        {
            Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.OwnDelayInfo, regenerationDelayTime);
            float travelTime = Algorithm.GetUltTravelTime(me, enemySpawn);

            float delay = timeLeft + regenerationDelayTime - travelTime;

            #region ownCheck
            if (RecallTracker.RecallTracker.Recalls.Any(x => x.Unit.ChampionName == enemy.ChampionName))
            {
                
                var recall = RecallTracker.RecallTracker.Recalls.First(x => x.Unit.ChampionName == enemy.ChampionName);
                recall.SetBaseUltShootTime(Environment.TickCount + delay);
            }

            if (travelTime > timeLeft + regenerationDelayTime && Damage.DidOwnChampWait)
            {
                CastUltInBase(enemy);
                Debug.Init(enemy, Algorithm.GetLastEstimatedEnemyReg(), aioDmg);
            }
            #endregion ownCheck


            //Cleaning
            Listing.teleportingEnemies.RemoveAll(x => x.Sender.ChampionName != enemy.ChampionName);
            AllyMessaging.SendBaseUltInfoToAllies(timeLeft, regenerationDelayTime);
        }

        private void CastUltInBase(AIHeroClient enemy)
        {
            float travelTime = Algorithm.GetUltTravelTime(me, enemySpawn);

            if (Listing.teleportingEnemies.All(x => x.Sender != enemy))
                return;

            Player.CastSpell(SpellSlot.R, enemySpawn);

            /*Draven*/
            if (Listing.MiscMenu.Get<CheckBox>("dravenCastBackBool").CurrentValue && me.ChampionName == "Draven")
            {
                int castBackReduction = Listing.MiscMenu.Get<Slider>("dravenCastBackDelay").CurrentValue;
                Core.DelayAction(() =>
                {
                    Player.CastSpell(SpellSlot.R);
                }, (int)(travelTime - castBackReduction));
            }
            /*Draven*/
        }

        private void UpdateEnemyVisibility()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsHPBarRendered && !Listing.visibleEnemies.Contains(enemy))
                {
                    OnOnEnemyVisible(enemy);
                }
                else if (!enemy.IsHPBarRendered && Listing.visibleEnemies.Contains(enemy))
                {
                    OnOnEnemyInvisible(new InvisibleEventArgs
                    {
                        StartTime = Core.GameTickCount,
                        sender = enemy,
                        LastRealPath = Listing.Pathing.GetLastEnemyPath(enemy)
                    });
                }
            }
        }
    }
}
