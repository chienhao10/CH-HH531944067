using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using AutoJungle.Data;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace AutoJungle
{
    internal class Program
    {
        public static GameInfo _GameInfo = new GameInfo();

        public static Menu menu;

        public static float UpdateLimiter, ResetTimer, GameStateChanging;

        public static readonly AIHeroClient player = ObjectManager.Player;

        public static Random Random = new Random(Environment.TickCount);

        public static ItemHandler ItemHandler;

        public static Vector3 pos;

        #region Main

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_GameInfo.SmiteableMob != null)
            {
                Jungle.CastSmite(_GameInfo.SmiteableMob);
            }
            CastHighPrioritySpells();
            if (ShouldSkipUpdate())
            {
                return;
            }
            SetGameInfo();
            if (_GameInfo.WaitOnFountain)
            {
                return;
            }
            //Checking Afk
            if (CheckAfk())
            {
                return;
            }
            if (HighPriorityPositioning())
            {
                MoveToPos();
                return;
            }

            //Check the camp, maybe its cleared
            CheckCamp();
            if (Debug)
            {
                /* Console.WriteLine("Items: ");
                foreach (var i in player.InventoryItems)
                {
                    Console.WriteLine("\t Name: {0}, ID: {1}({2})", i.IData.TranslatedDisplayName, i.Id, (int) i.Id);
                }*/
                _GameInfo.Show();
                /*
                foreach (var v in _GameInfo.MonsterList)
                {
                    Console.WriteLine(
                        v.name + ": " + v.IsAlive() + " Next: " + ((Environment.TickCount - v.TimeAtDead) / 1000));
                }*/
            }
            //Shopping
            if (Shopping())
            {
                return;
            }

            //Recalling
            if (RecallHander())
            {
                return;
            }
            if (getCheckBoxItem("UseTrinket"))
            {
                PlaceWard();
            }
            MoveToPos();

            CastSpells();
        }

        public static bool getCheckBoxItem(string item)
        {
            return menu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return menu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return menu[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return menu[item].Cast<ComboBox>().CurrentValue;
        }

        private static void CastHighPrioritySpells()
        {
            var target = _GameInfo.Target;
            switch (ObjectManager.Player.ChampionName)
            {
                case "Jax":
                    var eActive = player.HasBuff("JaxCounterStrike");
                    if (_GameInfo.GameState == State.Jungling || _GameInfo.GameState == State.LaneClear)
                    {
                        var targetMob = _GameInfo.Target;
                        if (Champdata.E.IsReady() && targetMob.LSIsValidTarget(350) &&
                            (player.ManaPercent > 40 || player.HealthPercent < 60 || player.Level == 1) && !eActive &&
                            _GameInfo.DamageCount >= 2 || _GameInfo.DamageTaken > player.Health * 0.2f)
                        {
                            Champdata.E.Cast();
                        }
                        return;
                    }
                    if (_GameInfo.GameState == State.FightIng)
                    {
                        if (Champdata.E.IsReady() &&
                            ((Champdata.Q.CanCast(target) && !eActive) || (target.LSIsValidTarget(350)) ||
                             ((_GameInfo.DamageCount >= 2 || _GameInfo.DamageTaken > player.Health * 0.2f) || !eActive)))
                        {
                            Champdata.E.Cast();
                        }
                        return;
                    }
                    break;
            }
        }

        private static bool HighPriorityPositioning()
        {
            if (player.ChampionName == "Skarner")
            {
                var capturablePoints =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(o => o.LSDistance(player) < 700 && !o.IsAlly && o.Name == "SkarnerPassiveCrystal")
                        .OrderBy(o => o.LSDistance(player))
                        .FirstOrDefault();
                if (capturablePoints != null)
                {
                    _GameInfo.MoveTo = capturablePoints.Position;
                    _GameInfo.GameState = State.Positioning;
                    return true;
                }
            }
            return false;
        }

        private static void PlaceWard()
        {
            if (_GameInfo.ClosestWardPos.IsValid() && Items.CanUseItem(3340))
            {
                Items.UseItem(3340, _GameInfo.ClosestWardPos);
            }
        }

        private static bool CheckAfk()
        {
            if (player.IsMoving || player.Spellbook.IsAutoAttacking || player.LSIsRecalling() || player.Level == 1)
            {
                _GameInfo.afk = 0;
            }
            else
            {
                _GameInfo.afk++;
            }
            if (_GameInfo.afk > 15 && !player.InFountain())
            {
                player.Spellbook.CastSpell(SpellSlot.Recall);
                return true;
            }
            return false;
        }

        private static void CheckCamp()
        {
            MonsterInfo nextMob = GetNextMob();
            if (nextMob != null && !nextMob.IsAlive())
            {
                //Console.WriteLine(nextMob.name + " skipped: " + (Environment.TickCount - nextMob.TimeAtDead / 1000f));
                _GameInfo.CurrentMonster++;
                _GameInfo.MoveTo = nextMob.Position;
                nextMob =
                    _GameInfo.MonsterList.OrderBy(m => m.Index).FirstOrDefault(m => m.Index == _GameInfo.CurrentMonster);
            }
            if (_GameInfo.GameState == State.Positioning)
            {
                if (Helpers.GetRealDistance(player, _GameInfo.MoveTo) < 500 && _GameInfo.MinionsAround == 0 &&
                    player.Level > 1)
                {
                    _GameInfo.CurrentMonster++;
                    if (nextMob != null)
                    {
                        _GameInfo.MoveTo = nextMob.Position;
                    }
                    //Console.WriteLine("CheckCamp - MoveTo: CurrentMonster++");
                }

                var probablySkippedMob = Helpers.GetNearest(player.Position, 1000);
                if (probablySkippedMob != null && probablySkippedMob.LSDistance(_GameInfo.MoveTo) > 200)
                {
                    var monster = _GameInfo.MonsterList.FirstOrDefault(m => probablySkippedMob.Name.Contains(m.name));
                    if (monster != null && monster.Index < 13)
                    {
                        _GameInfo.MoveTo = probablySkippedMob.Position;
                    }
                }
            }
        }

        private static void SetGameInfo()
        {
            _GameInfo.GroupWithoutTarget = false;
            ResetDamageTakenTimer();
            AutoLevel.Enable();
            _GameInfo.WaitOnFountain = WaitOnFountain();
            _GameInfo.ShouldRecall = ShouldRecall();
            _GameInfo.GameState = SetGameState();
            _GameInfo.MoveTo = GetMovePosition();
            _GameInfo.Target = GetTarget();
            _GameInfo.MinionsAround = Helpers.getMobs(player.Position, 700).Count;
            _GameInfo.SmiteableMob = Helpers.GetNearest(player.Position);
            _GameInfo.AllyStructures = GetStructures(true, _GameInfo.SpawnPointEnemy);
            _GameInfo.EnemyStructures = GetStructures(false, _GameInfo.SpawnPoint);
            _GameInfo.ClosestWardPos = Helpers.GetClosestWard();
        }

        private static IEnumerable<Vector3> GetStructures(bool ally, Vector3 basePos)
        {
            var turrets =
                ObjectManager.Get<Obj_Turret>()
                    .Where(t => t.IsAlly == ally && t.IsValid && t.Health > 0 && t.Health < t.MaxHealth)
                    .OrderBy(t => t.Position.LSDistance(basePos))
                    .Select(t => t.Position);
            var inhibs =
                ObjectManager.Get<Obj_BarracksDampener>()
                    .Where(t => t.IsAlly == ally && t.IsValid && t.Health > 0 && !t.IsDead && t.Health < t.MaxHealth)
                    .OrderBy(t => t.Position.LSDistance(basePos))
                    .Select(t => t.Position);
            var nexus =
                ObjectManager.Get<Obj_HQ>()
                    .Where(t => t.IsAlly == ally && t.IsValid && t.Health > 0 && !t.IsDead && t.Health < t.MaxHealth)
                    .OrderBy(t => t.Position.LSDistance(basePos))
                    .Select(t => t.Position);

            return turrets.Concat(inhibs).Concat(nexus);
        }

        #region MainFunctions

        private static bool RecallHander()
        {
            if ((_GameInfo.GameState != State.Positioning && _GameInfo.GameState != State.Retreat) ||
                !_GameInfo.MonsterList.Any(m => m.Position.LSDistance(player.Position) < 800))
            {
                return false;
            }
            if (Helpers.getMobs(player.Position, 1300).Count > 0)
            {
                return false;
            }
            if (player.InFountain() || player.ServerPosition.LSDistance(_GameInfo.SpawnPoint) < 1000)
            {
                return false;
            }
            if ((_GameInfo.ShouldRecall && !player.LSIsRecalling() && !player.InFountain()) &&
                (_GameInfo.GameState == State.Positioning ||
                 (_GameInfo.GameState == State.Retreat &&
                  (_GameInfo.afk > 15 ||
                   ObjectManager.Get<Obj_AI_Base>().Count(o => o.IsEnemy && o.LSDistance(player) < 2000) == 0))))
            {
                if (player.LSDistance(_GameInfo.SpawnPoint) > 6000)
                {
                    player.Spellbook.CastSpell(SpellSlot.Recall);
                }
                else
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, _GameInfo.SpawnPoint);
                }
                return true;
            }

            if (player.LSIsRecalling())
            {
                return true;
            }

            return false;
        }

        private static void CastSpells()
        {
            if (_GameInfo.Target == null)
            {
                return;
            }
            switch (_GameInfo.GameState)
            {
                case State.FightIng:
                    _GameInfo.Champdata.Combo();
                    break;
                case State.Ganking:
                    break;
                case State.Jungling:
                    _GameInfo.Champdata.JungleClear();
                    UsePotions();
                    break;
                case State.LaneClear:
                    _GameInfo.Champdata.JungleClear();
                    UsePotions();
                    break;
                case State.Objective:
                    if (_GameInfo.Target is AIHeroClient)
                    {
                        _GameInfo.Champdata.Combo();
                    }
                    else
                    {
                        _GameInfo.Champdata.JungleClear();
                    }
                    break;
                default:
                    break;
            }
        }

        private static void UsePotions()
        {
            if (Items.HasItem(2031) && Items.CanUseItem(2031) && player.HealthPercent < 80 &&
                !player.Buffs.Any(b => b.Name.Equals("ItemCrystalFlask")))
            {
                Items.UseItem(2031);
            }
        }

        private static void MoveToPos()
        {
            if ((_GameInfo.GameState != State.Positioning && _GameInfo.GameState != State.Ganking &&
                 _GameInfo.GameState != State.Retreat && _GameInfo.GameState != State.Grouping) ||
                !_GameInfo.MoveTo.IsValid())
            {
                return;
            }
            if (!Helpers.CheckPath(player.GetPath(_GameInfo.MoveTo)))
            {
                _GameInfo.CurrentMonster++;
                if (Debug)
                {
                    Console.WriteLine("MoveTo: CurrentMonster++2");
                }
            }
            if (_GameInfo.GameState == State.Retreat && _GameInfo.MoveTo.LSDistance(player.Position) < 100)
            {
                return;
            }
            if (_GameInfo.MoveTo.IsValid() &&
                (_GameInfo.MoveTo.LSDistance(_GameInfo.LastClick) > 150 || (!player.IsMoving && _GameInfo.afk > 10)))
            {
                if (player.IsMoving)
                {
                    int x, y;
                    x = (int)_GameInfo.MoveTo.X;
                    y = (int)_GameInfo.MoveTo.Y;
                    EloBuddy.Player.IssueOrder(
                        GameObjectOrder.MoveTo,
                        new Vector3(Random.Next(x, x + 100), Random.Next(y, y + 100), _GameInfo.MoveTo.Z));
                }
                else
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, _GameInfo.MoveTo);
                }
            }
        }

        private static bool Shopping()
        {
            if (!player.InFountain())
            {
                if (Debug)
                {
                    Console.WriteLine("Shopping: Not in shop - false");
                }
                return false;
            }
            var current =
                ItemHandler.ItemList.Where(i => Items.HasItem(i.ItemId))
                    .OrderByDescending(i => i.Index)
                    .FirstOrDefault();

            if (current != null)
            {
                var currentIndex = current.Index;
                var orderedList =
                    ItemHandler.ItemList.Where(i => !Items.HasItem(i.ItemId) && i.Index > currentIndex)
                        .OrderBy(i => i.Index);
                var itemToBuy = orderedList.FirstOrDefault();
                if (itemToBuy == null)
                {
                    if (Debug)
                    {
                        Console.WriteLine("Shopping: No next Item - false");
                    }
                    return false;
                }
                if (itemToBuy.Price <= player.Gold)
                {
                    Shop.BuyItem((ItemId)itemToBuy.ItemId);
                    if (itemToBuy.Index > 9 && Items.HasItem(2031))
                    {
                        Shop.SellItem(player.InventoryItems.First(i => i.Id == (ItemId)2031).Slot);
                    }
                    var nextItem = orderedList.FirstOrDefault(i => i.Index == itemToBuy.Index + 1);
                    if (nextItem != null)
                    {
                        _GameInfo.NextItemPrice = nextItem.Price;
                    }
                    if (Debug)
                    {
                        Console.WriteLine("Shopping: Shopping- " + itemToBuy.Name + " - true");
                    }
                    return true;
                }
            }
            else
            {
                Shop.BuyItem((ItemId)ItemHandler.ItemList.FirstOrDefault(i => i.Index == 1).ItemId);
                var nextItem = ItemHandler.ItemList.FirstOrDefault(i => i.Index == 2);
                if (nextItem != null)
                {
                    _GameInfo.NextItemPrice = nextItem.Price;
                }
                return true;
            }


            if (Debug)
            {
                Console.WriteLine("Shopping: End - false");
            }
            return false;
        }

        private static Obj_AI_Base GetTarget()
        {
            switch (_GameInfo.GameState)
            {
                case State.Objective:
                    var obj = Helpers.GetNearest(player.Position, GameInfo.ChampionRange);
                    if (obj != null && (obj.Name.Contains("Dragon") || obj.Name.Contains("Baron")) && (HealthPrediction.GetHealthPrediction(obj, 3000) + 500 < Jungle.smiteDamage(obj) || (_GameInfo.EnemiesAround == 0 && player.Level > 8 && MinionManager.GetMinions(player.Position, GameInfo.ChampionRange, MinionTypes.All, MinionTeam.NotAlly).Take(5).FirstOrDefault(m => m.Name.Contains("Sru_Crab") && m.Health < m.MaxHealth) == null)))
                    {
                        return obj;
                    }
                    else
                    {
                        return _GameInfo.EnemiesAround > 0 ? Helpers.GetTargetEnemy() : null;
                    }
                case State.FightIng:
                    return Helpers.GetTargetEnemy();
                case State.Ganking:
                    return null;
                case State.Jungling:
                    return Helpers.getMobs(player.Position, 1000).OrderByDescending(m => m.MaxHealth).FirstOrDefault();
                case State.LaneClear:
                    return
                        Helpers.getMobs(player.Position, GameInfo.ChampionRange)
                            .Where(m => !m.UnderTurret(true))
                            .OrderByDescending(m => player.LSGetAutoAttackDamage(m, true) > m.Health)
                            .ThenBy(m => m.LSDistance(player))
                            .FirstOrDefault();
                case State.Pushing:
                    var enemy = Helpers.GetTargetEnemy();
                    if (enemy != null)
                    {
                        _GameInfo.Target = enemy;
                        _GameInfo.Champdata.Combo();
                        return enemy;
                    }
                    var enemyTurret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .FirstOrDefault(
                                t =>
                                    t.IsEnemy && !t.IsDead && t.LSDistance(player) < 2000 &&
                                    Helpers.getAllyMobs(t.Position, 500).Count > 0);
                    if (enemyTurret != null)
                    {
                        _GameInfo.Champdata.JungleClear();
                        return enemyTurret;
                    }
                    var mob =
                        Helpers.getMobs(player.Position, GameInfo.ChampionRange)
                            .OrderBy(m => m.UnderTurret(true))
                            .ThenByDescending(m => player.LSGetAutoAttackDamage(m, true) > m.Health)
                            .ThenBy(m => m.LSDistance(player))
                            .FirstOrDefault();
                    if (mob != null)
                    {
                        _GameInfo.Target = mob;
                        _GameInfo.Champdata.JungleClear();
                        return mob;
                    }
                    break;
                case State.Defending:
                    var enemyDef = Helpers.GetTargetEnemy();
                    if (enemyDef != null && !_GameInfo.InDanger)
                    {
                        _GameInfo.Target = enemyDef;
                        _GameInfo.Champdata.Combo();
                        return enemyDef;
                    }
                    var mobDef =
                        Helpers.getMobs(player.Position, GameInfo.ChampionRange)
                            .OrderByDescending(m => m.LSCountEnemiesInRange(500) == 0)
                            .ThenByDescending(m => player.LSGetAutoAttackDamage(m, true) > m.Health)
                            .ThenBy(m => m.LSCountEnemiesInRange(500))
                            .FirstOrDefault();
                    if (mobDef != null)
                    {
                        _GameInfo.Target = mobDef;
                        _GameInfo.Champdata.JungleClear();
                        return mobDef;
                    }
                    break;
                default:
                    break;
            }

            if (Debug)
            {
                Console.WriteLine("GetTarget: Cant get target");
            }
            return null;
        }


        private static bool CheckObjective(Vector3 pos)
        {
            if ((pos.LSCountEnemiesInRange(800) > 0 || pos.CountAlliesInRange(800) > 0) && !CheckForRetreat(null, pos))
            {
                var obj = Helpers.GetNearest(pos);
                if (obj != null && obj.Health < obj.MaxHealth - 300)
                {
                    if (player.LSDistance(pos) > Jungle.smiteRange)
                    {
                        _GameInfo.MoveTo = pos;
                        return true;
                    }
                }
            }
            if ((Jungle.SmiteReady() || (player.Level >= 14 && player.HealthPercent > 80)) && player.Level >= 9 &&
                player.LSDistance(Camps.Dragon.Position) < GameInfo.ChampionRange)
            {
                var drake = Helpers.GetNearest(player.Position, GameInfo.ChampionRange);
                if (drake != null && drake.Name.Contains("Dragon"))
                {
                    _GameInfo.CurrentMonster = 13;
                    _GameInfo.MoveTo = drake.Position;
                    return true;
                }
            }
            return false;
        }

        private static bool CheckGanking()
        {
            AIHeroClient gankTarget = null;
            if (player.Level >= getSliderItem("GankLevel") &&
                ((player.Mana > _GameInfo.Champdata.R.ManaCost && player.MaxMana > 100) || player.MaxMana <= 100))
            {
                var heroes =
                    HeroManager.Enemies.Where(
                        e =>
                            e.LSDistance(player) < getSliderItem("GankRange") && e.LSIsValidTarget() &&
                            !e.UnderTurret(true) && !CheckForRetreat(e, e.Position)).OrderBy(e => player.LSDistance(e));
                foreach (var possibleTarget in heroes)
                {
                    var myDmg = Helpers.GetComboDMG(player, possibleTarget);
                    if (player.Level + 1 <= possibleTarget.Level)
                    {
                        continue;
                    }
                    if (Helpers.AlliesThere(possibleTarget.Position) + 1 <
                        possibleTarget.Position.LSCountEnemiesInRange(GameInfo.ChampionRange))
                    {
                        continue;
                    }
                    if (Helpers.GetComboDMG(possibleTarget, player) > player.Health)
                    {
                        continue;
                    }
                    var ally =
                        HeroManager.Allies.Where(a => !a.IsDead && a.LSDistance(possibleTarget) < 2000)
                            .OrderBy(a => a.LSDistance(possibleTarget))
                            .FirstOrDefault();
                    var hp = possibleTarget.Health - myDmg * getSliderItem("GankFrequency") / 100f;
                    if (ally != null)
                    {
                        hp -= Helpers.GetComboDMG(ally, possibleTarget) *
                              getSliderItem("GankFrequency") / 100;
                    }
                    if (hp < 0)
                    {
                        gankTarget = possibleTarget;
                        break;
                    }
                }
            }
            if (gankTarget != null)
            {
                var gankPosition =
                    Helpers.GankPos.Where(p => p.LSDistance(gankTarget.Position) < 2000)
                        .OrderBy(p => player.LSDistance(gankTarget.Position))
                        .FirstOrDefault();
                if (gankTarget.LSDistance(player) > 2000 && gankPosition.IsValid() &&
                    gankPosition.LSDistance(gankTarget.Position) < 2000 &&
                    player.LSDistance(gankTarget) > gankPosition.LSDistance(gankTarget.Position))
                {
                    _GameInfo.MoveTo = gankPosition;
                    return true;
                }
                else if (gankTarget.LSDistance(player) <= 2000)
                {
                    _GameInfo.MoveTo = gankTarget.Position;
                    return true;
                }
                else if (!gankPosition.IsValid())
                {
                    _GameInfo.MoveTo = gankTarget.Position;
                    return true;
                }
            }
            return false;
        }

        private static State SetGameState()
        {
            var enemy = Helpers.GetTargetEnemy();
            State tempstate = State.Null;
            if (CheckForRetreat(enemy, player.Position))
            {
                tempstate = State.Retreat;
            }
            if (tempstate == State.Null && _GameInfo.EnemiesAround == 0 &&
                (CheckObjective(Camps.Baron.Position) || CheckObjective(Camps.Dragon.Position)))
            {
                tempstate = State.Objective;
            }
            if (tempstate == State.Null && _GameInfo.GameState != State.Retreat && _GameInfo.GameState != State.Pushing &&
                _GameInfo.GameState != State.Defending &&
                ((enemy != null && !CheckForRetreat(enemy, enemy.Position) &&
                  Helpers.GetRealDistance(player, enemy.Position) < GameInfo.ChampionRange)) ||
                player.HasBuff("skarnerimpalevo"))
            {
                tempstate = State.FightIng;
            }
            if (tempstate == State.Null && player.Level >= 6 && CheckForGrouping())
            {
                if (_GameInfo.MoveTo.LSDistance(player.Position) <= GameInfo.ChampionRange)
                {
                    if (
                        ObjectManager.Get<Obj_AI_Turret>()
                            .FirstOrDefault(t => t.LSDistance(_GameInfo.MoveTo) < GameInfo.ChampionRange && t.IsAlly) !=
                        null && (_GameInfo.GameState == State.Grouping || _GameInfo.GameState == State.Defending))
                    {
                        tempstate = State.Defending;
                    }
                    else if (_GameInfo.GameState != State.Grouping && _GameInfo.GameState != State.Retreat &&
                             _GameInfo.GameState != State.Jungling)
                    {
                        tempstate = State.Pushing;
                    }
                }
                if (tempstate == State.Null &&
                    (_GameInfo.MoveTo.LSDistance(player.Position) > GameInfo.ChampionRange || _GameInfo.GroupWithoutTarget) &&
                    (_GameInfo.GameState == State.Positioning || _GameInfo.GameState == State.Grouping))
                {
                    tempstate = State.Grouping;
                }
            }
            if (tempstate == State.Null && _GameInfo.EnemiesAround == 0 &&
                (_GameInfo.GameState == State.Ganking || _GameInfo.GameState == State.Positioning) && CheckGanking())
            {
                tempstate = State.Ganking;
            }
            if (tempstate == State.Null && _GameInfo.MinionsAround > 0 &&
                (_GameInfo.MonsterList.Any(m => m.Position.LSDistance(player.Position) < 700) ||
                 _GameInfo.SmiteableMob != null) && _GameInfo.GameState != State.Retreat)
            {
                tempstate = State.Jungling;
            }
            if (tempstate == State.Null && CheckLaneClear(player.Position))
            {
                tempstate = State.LaneClear;
            }
            if (tempstate == State.Null)
            {
                tempstate = State.Positioning;
            }
            if (tempstate == _GameInfo.GameState)
            {
                return tempstate;
            }
            else if (Environment.TickCount - GameStateChanging > 1300 || _GameInfo.GameState == State.Retreat ||
                     tempstate == State.FightIng)
            {
                GameStateChanging = Environment.TickCount;
                return tempstate;
            }
            else
            {
                return _GameInfo.GameState;
            }
        }

        private static bool CheckLaneClear(Vector3 pos)
        {
            return (Helpers.AlliesThere(pos) == 0 || Helpers.AlliesThere(pos) >= 2 ||
                    player.LSDistance(_GameInfo.SpawnPoint) < 6000 || player.LSDistance(_GameInfo.SpawnPointEnemy) < 6000 ||
                    player.Level >= 14) && pos.LSCountEnemiesInRange(GameInfo.ChampionRange) == 0 &&
                   Helpers.getMobs(pos, GameInfo.ChampionRange).Count +
                   _GameInfo.EnemyStructures.Count(p => p.LSDistance(pos) < GameInfo.ChampionRange) > 0 &&
                   !_GameInfo.MonsterList.Any(m => m.Position.LSDistance(pos) < 600) && _GameInfo.SmiteableMob == null &&
                   _GameInfo.GameState != State.Retreat;
        }

        private static bool CheckForRetreat(Obj_AI_Base enemy, Vector3 pos)
        {
            if (_GameInfo.GameState == State.Jungling)
            {
                return false;
            }
            if (enemy != null && !enemy.UnderTurret(true) && player.LSDistance(enemy) < 350 && !_GameInfo.AttackedByTurret)
            {
                return false;
            }
            var indanger = ((Helpers.GetHealth(true, pos) +
                             ((player.LSDistance(pos) < GameInfo.ChampionRange) ? 0 : player.Health)) * 1.3f <
                            Helpers.GetHealth(false, pos) && pos.LSCountEnemiesInRange(GameInfo.ChampionRange) > 1 &&
                            Helpers.AlliesThere(pos, 500) == 0) ||
                           player.HealthPercent < getSliderItem("HealtToBack");
            if (indanger || _GameInfo.AttackedByTurret)
            {
                if (((enemy != null && Helpers.AlliesThere(pos, 600) > 0) && player.HealthPercent > 25))
                {
                    return false;
                }
                if (_GameInfo.AttackedByTurret)
                {
                    if ((enemy != null &&
                         (enemy.Health > player.LSGetAutoAttackDamage(enemy, true) * 2 ||
                          enemy.LSDistance(player) > Orbwalking.GetRealAutoAttackRange(enemy) + 20) || enemy == null))
                    {
                        return true;
                    }
                }
                if (indanger)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckForGrouping()
        {
            //Checking grouping allies
            var ally =
                HeroManager.Allies.FirstOrDefault(
                    a => Helpers.AlliesThere(a.Position) >= 2 && a.LSDistance(_GameInfo.SpawnPointEnemy) < 7000);
            if (ally != null && !CheckForRetreat(null, ally.Position) &&
                Helpers.CheckPath(player.GetPath(ally.Position)))
            {
                _GameInfo.MoveTo = ally.Position.LSExtend(player.Position, 200);
                _GameInfo.GroupWithoutTarget = true;
                if (Debug)
                {
                    Console.WriteLine("CheckForGrouping() - Checking grouping allies");
                }
                return true;
            }
            //Checknig base after recall
            if (player.LSDistance(_GameInfo.SpawnPoint) < 5000)
            {
                var mob =
                    Helpers.getMobs(_GameInfo.SpawnPoint, 5000)
                        .OrderByDescending(m => Helpers.getMobs(m.Position, 300).Count)
                        .FirstOrDefault();
                if (mob != null && Helpers.getMobs(mob.Position, 300).Count > 700 &&
                    Helpers.CheckPath(player.GetPath(mob.Position)) && !CheckForRetreat(null, mob.Position))
                {
                    _GameInfo.MoveTo = mob.Position;
                    if (Debug)
                    {
                        Console.WriteLine("CheckForGrouping() - Checknig base after recall");
                    }
                    return true;
                }
            }
            //Checknig enemy turrets
            foreach (var vector in
                _GameInfo.EnemyStructures.Where(
                    s =>
                        s.LSDistance(player.Position) < getSliderItem("GankRange") &&
                        CheckLaneClear(s)))
            {
                var aMinis = Helpers.getAllyMobs(vector, GameInfo.ChampionRange);
                if (aMinis.Count > 1)
                {
                    var eMinis =
                        Helpers.getMobs(vector, GameInfo.ChampionRange)
                            .OrderByDescending(m => Helpers.getMobs(m.Position, 300).Count)
                            .FirstOrDefault();
                    if (eMinis != null)
                    {
                        var pos = eMinis.Position;
                        if (Helpers.CheckPath(player.GetPath(pos)) && !CheckForRetreat(null, pos))
                        {
                            _GameInfo.MoveTo = pos;
                            if (Debug)
                            {
                                Console.WriteLine("CheckForGrouping() - Checknig enemy turrets 1");
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (Helpers.CheckPath(player.GetPath(vector)) && !CheckForRetreat(null, vector))
                        {
                            _GameInfo.MoveTo = vector;
                            if (Debug)
                            {
                                Console.WriteLine("CheckForGrouping() - Checknig enemy turrets 2");
                            }
                            return true;
                        }
                    }
                }
            }
            //Checknig ally turrets
            foreach (var vector in
                _GameInfo.AllyStructures.Where(
                    s => s.LSDistance(player.Position) < getSliderItem("GankRange")))
            {
                var eMinis = Helpers.getMobs(vector, GameInfo.ChampionRange);
                if (!CheckLaneClear(vector))
                {
                    continue;
                }
                if (eMinis.Count > 3)
                {
                    var temp = eMinis.OrderByDescending(m => Helpers.getMobs(m.Position, 300).Count).FirstOrDefault();
                    if (temp != null)
                    {
                        var pos = temp.Position;
                        if (Helpers.CheckPath(player.GetPath(pos)) && !CheckForRetreat(null, pos))
                        {
                            _GameInfo.MoveTo = pos;
                            if (Debug)
                            {
                                Console.WriteLine("CheckForGrouping() - Checknig ally turrets 1");
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (Helpers.CheckPath(player.GetPath(vector)) && !CheckForRetreat(null, vector))
                        {
                            _GameInfo.MoveTo = vector;
                            if (Debug)
                            {
                                Console.WriteLine("CheckForGrouping() - Checknig ally turrets 2");
                            }
                            return true;
                        }
                    }
                }
            }
            //follow minis
            var minis = Helpers.getAllyMobs(player.Position, 1000);
            if (minis.Count >= 5 && player.Level >= 8)
            {
                var objAiBase = minis.OrderBy(m => m.LSDistance(_GameInfo.SpawnPointEnemy)).FirstOrDefault();
                if (objAiBase != null &&
                    (objAiBase.CountAlliesInRange(GameInfo.ChampionRange) == 0 ||
                     objAiBase.CountAlliesInRange(GameInfo.ChampionRange) >= 2 || player.Level >= 14) &&
                    Helpers.getMobs(objAiBase.Position, 1000).Count == 0)
                {
                    _GameInfo.MoveTo = objAiBase.Position.LSExtend(_GameInfo.SpawnPoint, 100);
                    _GameInfo.GroupWithoutTarget = true;
                    if (Debug)
                    {
                        Console.WriteLine("CheckForGrouping() - follow minis");
                    }
                    return true;
                }
            }
            //Checking free enemy minionwaves
            if (player.Level > 8)
            {
                var miniwaves =
                    Helpers.getMobs(player.Position, getSliderItem("GankRange"))
                        .Where(m => Helpers.getMobs(m.Position, 1200).Count > 6 && CheckLaneClear(m.Position))
                        .OrderByDescending(m => m.LSDistance(_GameInfo.SpawnPoint) < 7000)
                        .ThenByDescending(m => m.LSDistance(player) < 2000)
                        .ThenByDescending(m => Helpers.getMobs(m.Position, 1200).Count);
                foreach (var miniwave in
                    miniwaves.Where(miniwave => Helpers.getMobs(miniwave.Position, 1200).Count >= 6)
                        .Where(
                            miniwave =>
                                !CheckForRetreat(null, miniwave.Position) &&
                                Helpers.CheckPath(player.GetPath(miniwave.Position))))
                {
                    _GameInfo.MoveTo = miniwave.Position.LSExtend(player.Position, 200);
                    if (Debug)
                    {
                        Console.WriteLine("CheckForGrouping() - Checking free enemy minionwavess");
                    }
                    return true;
                }
            }
            //Checking ally mobs, pushing
            if (player.Level > 8)
            {
                var miniwave =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            m =>
                                m.LSDistance(_GameInfo.SpawnPointEnemy) < 7000 &&
                                Helpers.getAllyMobs(m.Position, 1200).Count >= 7)
                        .OrderByDescending(m => m.LSDistance(_GameInfo.SpawnPointEnemy) < 7000)
                        .ThenBy(m => m.LSDistance(player))
                        .FirstOrDefault();
                if (miniwave != null && Helpers.CheckPath(player.GetPath(miniwave.Position)) &&
                    !CheckForRetreat(null, miniwave.Position) && CheckLaneClear(miniwave.Position))
                {
                    _GameInfo.MoveTo = miniwave.Position.LSExtend(player.Position, 200);
                    return true;
                }
            }
            return false;
        }

        private static Vector3 GetMovePosition()
        {
            switch (_GameInfo.GameState)
            {
                case State.Retreat:
                    var enemyTurret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .FirstOrDefault(t => t.IsEnemy && !t.IsDead && t.LSDistance(player) < 2000);
                    var allyTurret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .OrderBy(t => t.LSDistance(player))
                            .FirstOrDefault(
                                t =>
                                    t.IsAlly && !t.IsDead && t.LSDistance(player) < 4000 &&
                                    t.LSCountEnemiesInRange(1200) == 0);
                    var enemy = _GameInfo.Target;
                    if (_GameInfo.AttackedByTurret && enemyTurret != null)
                    {
                        if (allyTurret != null)
                        {
                            return allyTurret.Position;
                        }
                        var nextPost = Prediction.GetPrediction(player, 1);
                        if (!nextPost.UnitPosition.UnderTurret(true))
                        {
                            return nextPost.CastPosition;
                        }
                        else
                        {
                            return _GameInfo.SpawnPoint;
                        }
                    }
                    if (allyTurret != null && player.LSDistance(_GameInfo.SpawnPoint) > player.LSDistance(allyTurret))
                    {
                        return allyTurret.Position.LSExtend(_GameInfo.SpawnPoint, 300);
                    }
                    return _GameInfo.SpawnPoint;
                case State.Objective:
                    return _GameInfo.MoveTo;
                case State.Grouping:
                    return _GameInfo.MoveTo;
                case State.Defending:
                    return Vector3.Zero;
                case State.Pushing:
                    return Vector3.Zero;
                case State.Warding:
                    return _GameInfo.MoveTo;
                case State.FightIng:
                    return Vector3.Zero;
                case State.Ganking:
                    return _GameInfo.MoveTo;
                case State.Jungling:
                    return Vector3.Zero;
                case State.LaneClear:
                    return Vector3.Zero;
                default:
                    MonsterInfo nextMob = GetNextMob();
                    if (nextMob != null)
                    {
                        return nextMob.Position;
                    }
                    var firstOrDefault = _GameInfo.MonsterList.FirstOrDefault(m => m.Index == 1);
                    if (firstOrDefault != null)
                    {
                        return firstOrDefault.Position;
                    }
                    break;
            }

            if (Debug)
            {
                Console.WriteLine("GetMovePosition: Can't get Position");
            }
            return Vector3.Zero;
        }

        private static MonsterInfo GetNextMob()
        {
            MonsterInfo nextMob = null;
            if (!getCheckBoxItem("EnemyJungle"))
            {
                if (player.Team == GameObjectTeam.Chaos)
                {
                    nextMob =
                        _GameInfo.MonsterList.OrderBy(m => m.Index)
                            .FirstOrDefault(m => m.Index == _GameInfo.CurrentMonster && !m.ID.Contains("bteam"));
                }
                else
                {
                    nextMob =
                        _GameInfo.MonsterList.OrderBy(m => m.Index)
                            .FirstOrDefault(m => m.Index == _GameInfo.CurrentMonster && !m.ID.Contains("pteam"));
                }
            }
            else
            {
                nextMob =
                    _GameInfo.MonsterList.OrderBy(m => m.Index).FirstOrDefault(m => m.Index == _GameInfo.CurrentMonster);
            }
            return nextMob;
        }

        private static void ResetDamageTakenTimer()
        {
            if (Environment.TickCount - ResetTimer > 1200)
            {
                ResetTimer = Environment.TickCount;
                _GameInfo.DamageTaken = 0f;
                _GameInfo.DamageCount = 0;
            }
            if (_GameInfo.CurrentMonster == 13 && player.Level <= 9)
            {
                _GameInfo.CurrentMonster++;
            }
            if (_GameInfo.CurrentMonster > 16)
            {
                _GameInfo.CurrentMonster = 1;
            }
        }

        private static bool ShouldRecall()
        {
            if (player.HealthPercent <= getSliderItem("HealtToBack"))
            {
                if (Debug)
                {
                    Console.WriteLine("ShouldRecall: Low Health - true");
                }
                return true;
            }
            if (_GameInfo.CanBuyItem())
            {
                if (Debug)
                {
                    Console.WriteLine("ShouldRecall: Can buy item - true");
                }
                return true;
            }
            if (Helpers.getMobs(_GameInfo.SpawnPoint, 5000).Count > 6)
            {
                if (Debug)
                {
                    Console.WriteLine("ShouldRecall: Def base - true");
                }
                return true;
            }
            if (_GameInfo.GameState == State.Retreat && player.LSCountEnemiesInRange(GameInfo.ChampionRange) == 0)
            {
                if (Debug)
                {
                    Console.WriteLine("ShouldRecall: After retreat - true");
                }
                return true;
            }
            if (Debug)
            {
                Console.WriteLine("ShouldRecall: End - false");
            }
            return false;
        }

        private static bool WaitOnFountain()
        {
            if (!player.InFountain())
            {
                return false;
            }
            if (player.InFountain() && player.LSIsRecalling())
            {
                return false;
            }
            if (player.HealthPercent < 90 || (player.ManaPercent < 90 && player.MaxMana > 100))
            {
                if (player.IsMoving)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.HoldPosition, player.Position);
                }
                return true;
            }
            return false;
        }

        private static bool ShouldSkipUpdate()
        {
            if (!getCheckBoxItem("Enabled"))
            {
                return true;
            }
            if (Environment.TickCount - UpdateLimiter <= 400)
            {
                return true;
            }
            if (player.IsDead)
            {
                return true;
            }
            if (player.LSIsRecalling() && !player.InFountain())
            {
                return true;
            }
            UpdateLimiter = Environment.TickCount - Random.Next(0, 100);
            return false;
        }

        public static bool Debug
        {
            get { return getKeyBindItem("debug"); }
        }

        #endregion

        #endregion

        #region Events

        private static void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            AIHeroClient target = args.Target as AIHeroClient;
            if (target != null)
            {
                if (target.IsMe && sender.IsValid && !sender.IsDead && sender.IsEnemy && target.IsValid)
                {
                    if (Orbwalking.IsAutoAttack(args.SData.Name))
                    {
                        _GameInfo.DamageTaken += (float)sender.LSGetAutoAttackDamage(player, true);
                        _GameInfo.DamageCount++;
                    }
                    if (sender is Obj_AI_Turret && !_GameInfo.AttackedByTurret)
                    {
                        _GameInfo.AttackedByTurret = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(2000, () => _GameInfo.AttackedByTurret = false);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Debug)
            {
                if (pos.IsValid())
                {
                    Render.Circle.DrawCircle(pos, 50, Color.Crimson, 7);
                }

                foreach (var m in Helpers.mod)
                {
                    Render.Circle.DrawCircle(m, 50, Color.Crimson, 7);
                }

                if (_GameInfo.LastClick.IsValid())
                {
                    Render.Circle.DrawCircle(_GameInfo.LastClick, 70, Color.Blue, 7);
                }
                if (_GameInfo.MoveTo.IsValid())
                {
                    Render.Circle.DrawCircle(_GameInfo.MoveTo, 77, Color.BlueViolet, 7);
                }
                foreach (var e in _GameInfo.EnemyStructures)
                {
                    Render.Circle.DrawCircle(e, 300, Color.Red, 7);
                }
                foreach (var a in _GameInfo.AllyStructures)
                {
                    Render.Circle.DrawCircle(a, 300, Color.DarkGreen, 7);
                }
                if (_GameInfo.ClosestWardPos.IsValid())
                {
                    Render.Circle.DrawCircle(_GameInfo.ClosestWardPos, 70, Color.LawnGreen, 7);
                }
            }
            if (getCheckBoxItem("State"))
            {
                Drawing.DrawText(150f, 200f, Color.Aqua, _GameInfo.GameState.ToString());
            }
        }

        private static void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe)
            {
                _GameInfo.LastClick = args.Path.Last();
            }
        }

        #endregion

        #region Init

        public static void OnGameLoad()
        {
            if (Game.MapId != GameMapId.SummonersRift)
            {
                Chat.Print("The map is not supported!");
                return;
            }
            _GameInfo.Champdata = new Champdata();
            if (_GameInfo.Champdata.Hero == null)
            {
                Chat.Print("The champion is not supported!");
                return;
            }
            Jungle.setSmiteSlot();
            if (Jungle.smiteSlot == SpellSlot.Unknown)
            {
                Console.WriteLine("Items: ");
                foreach (var i in player.InventoryItems)
                {
                    Console.WriteLine("\t Name: {0}, ID: {1}({2})", i.DisplayName, i.Id, (int)i.Id);
                }
                Chat.Print("You don't have smite!");
                return;
            }

            ItemHandler = new ItemHandler(_GameInfo.Champdata.Type);
            CreateMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            Game.OnEnd += Game_OnEnd;
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
        }

        private static void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Position.LSDistance(player.Position) < 600)
            {
                var closest = _GameInfo.MonsterList.FirstOrDefault(m => m.Position.LSDistance(sender.Position) < 600);
                if (closest != null && _GameInfo.GameState == State.Jungling &&
                    Helpers.getMobs(sender.Position, 600).Where(m => !m.IsDead).Count() == 0)
                {
                    if (Environment.TickCount - closest.TimeAtDead > closest.RespawnTime)
                    {
                        closest.TimeAtDead = Environment.TickCount;
                    }
                }
            }
        }

        private static void Game_OnEnd(GameEndEventArgs args)
        {
            if (getCheckBoxItem("AutoClose"))
            {
                Console.WriteLine("END");
                Thread.Sleep(Random.Next(10000, 13000));
                Game.QuitGame();
            }
        }

        private static void CreateMenu()
        {
            menu = MainMenu.AddMenu("爱台湾S7自动打野", "AutoJungle");

            menu.Add("Enabled", new CheckBox("开启"));
            menu.Add("AutoClose", new CheckBox("游戏结束自动关闭"));

            menu.AddGroupLabel("打野设置");
            menu.Add("HealtToBack", new Slider("低血量(%)回城", 35, 0, 100));
            menu.Add("UseTrinket", new CheckBox("使用 饰品"));
            menu.Add("EnemyJungle", new CheckBox("进入敌方野区"));

            menu.AddGroupLabel("Gank 设置");
            menu.Add("GankLevel", new Slider("最低等级开始GANK", 5, 1, 18));
            menu.Add("GankFrequency", new Slider("Gank 频繁量", 100, 0, 100));
            menu.Add("GankRange", new Slider("搜索敌人范围", 7000, 0, 20000));
            menu.Add("ComboSmite", new CheckBox("使用惩戒"));

            menu.AddGroupLabel("调试");
            menu.Add("debug", new KeyBind("控制台显示", false, KeyBind.BindTypes.HoldActive, 'T'));
            menu.Add("State", new CheckBox("显示 游戏状态", false));
            menu.AddLabel("合成打野装备后，F5将会导致挂机停止！");
            menu.AddLabel("支持的英雄: 易,狼人,龙女,赵信,武器,梦魇,寡妇,雷霆,蛮王");
            menu.AddLabel("如果此脚本有问题，麻烦回报给Chienhao或者L#的LOVETAIWAN");
        }

        #endregion
    }
}