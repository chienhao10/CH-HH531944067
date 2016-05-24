using System;
using System.Collections.Generic;
using System.Linq;
using AutoJungle.Data;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoJungle
{
    internal class GameInfo
    {
        public bool WaitOnFountain { get; set; }
        public Vector3 MoveTo { get; set; }
        public Vector3 LastCheckPoint { get; set; }
        public Obj_AI_Base Target { get; set; }
        public bool ShouldRecall { get; set; }
        public float DamageTaken { get; set; }
        public int DamageCount { get; set; }
        public bool AttackedByTurret { get; set; }
        public Champdata Champdata { get; set; }
        public int NextItemPrice { get; set; }
        public bool Fighting { get; set; }
        public List<MonsterInfo> MonsterList = new List<MonsterInfo>();
        public int CurrentMonster { get; set; }
        public Vector3[] MovePath;
        public State GameState;
        public Vector3 LastClick;
        public int MinionsAround;
        public Obj_AI_Base SmiteableMob;
        public Vector3 SpawnPoint;
        public Vector3 SpawnPointEnemy;
        public int afk;
        public IEnumerable<Vector3> AllyStructures = new List<Vector3>();
        public IEnumerable<Vector3> EnemyStructures = new List<Vector3>();
        public Vector3 ClosestWardPos = Vector3.Zero;
        public const int ChampionRange = 1300;
        public bool GroupWithoutTarget;

        public GameInfo()
        {
            NextItemPrice = 350;
            if (ObjectManager.Player.Team == GameObjectTeam.Chaos)
            {
                SpawnPoint = new Vector3(14232f, 14354, 171.97f);
                SpawnPointEnemy = new Vector3(415.33f, 453.38f, 182.66f);
            }
            else
            {
                SpawnPoint = new Vector3(415.33f, 453.38f, 182.66f);
                SpawnPointEnemy = new Vector3(14232f, 14354, 171.97f);
            }
            GameState = State.Positioning;
            SetMonsterList();
            CurrentMonster = 1;

            var last =
                MonsterList.OrderBy(temp => temp.Position.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            if (!ObjectManager.Player.InFountain() && last != null && ObjectManager.Player.Level > 1)
            {
                CurrentMonster = last.Index;
            }
            else
            {
                CurrentMonster = 1;
            }
        }

        public bool IsUnderAttack()
        {
            return DamageTaken > 0f;
        }

        public bool CanBuyItem()
        {
            if (GameState != State.Positioning)
            {
                return false;
            }

            var current =
                ItemHandler.ItemList.Where(i => Items.HasItem(i.ItemId))
                    .OrderByDescending(i => i.Index)
                    .FirstOrDefault();
            var orderedList =
                ItemHandler.ItemList.Where(
                    i => current != null && (!Items.HasItem(i.ItemId) && i.Index > current.Index)).OrderBy(i => i.Index);
            var nextItem = orderedList.FirstOrDefault(i => i.Index == current.Index + 1);
            if (nextItem != null)
            {
                NextItemPrice = nextItem.Price;
            }
            if (nextItem != null && nextItem.Price < ObjectManager.Player.Gold)
            {
                if (Program.Debug)
                {
                    Console.WriteLine("Can buy: " + nextItem.Price);
                }
                return true;
            }
            return false;
        }

        public int EnemiesAround
        {
            get { return Champdata.Hero.CountEnemiesInRange(ChampionRange); }
        }

        public int AlliesAround
        {
            get { return Champdata.Hero.CountAlliesInRange(ChampionRange); }
        }


        public bool InDanger
        {
            get
            {
                return EnemiesAround > AlliesAround + 1 ||
                       Champdata.Hero.HealthPercent < Program.menu.Item("HealtToBack").GetValue<Slider>().Value;
            }
        }

        private void SetMonsterList()
        {
            if (ObjectManager.Player.Team == GameObjectTeam.Chaos)
            {
                MonsterList.Add(new MonsterInfo(Camps.pteam_Gromp, 1));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Blue, 2));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Wolf, 3));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Razorbeak, 4));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Red, 5));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Krug, 6));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Gromp, 7));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Blue, 8));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Wolf, 9));
                MonsterList.Add(new MonsterInfo(Camps.top_crab, 10));
                MonsterList.Add(new MonsterInfo(Camps.PURPLE_MID, 11));
                MonsterList.Add(new MonsterInfo(Camps.down_crab, 12));
                MonsterList.Add(new MonsterInfo(Camps.Dragon, 13));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Razorbeak, 14));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Red, 15));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Krug, 16));
            }
            else
            {
                MonsterList.Add(new MonsterInfo(Camps.bteam_Krug, 1));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Red, 2));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Razorbeak, 3));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Wolf, 4));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Blue, 5));
                MonsterList.Add(new MonsterInfo(Camps.bteam_Gromp, 6));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Razorbeak, 7));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Red, 8));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Krug, 9));
                MonsterList.Add(new MonsterInfo(Camps.top_crab, 10));
                MonsterList.Add(new MonsterInfo(Camps.BLUE_MID, 11));
                MonsterList.Add(new MonsterInfo(Camps.down_crab, 12));
                MonsterList.Add(new MonsterInfo(Camps.Dragon, 13));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Gromp, 14));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Blue, 15));
                MonsterList.Add(new MonsterInfo(Camps.pteam_Wolf, 16));
            }
        }

        public void Show()
        {
            var result =
                string.Format(
                    "WaitOnFountain: {0}\n" + "MoveTo: {1}\n" + "CheckPoint: {2}\n" + "Target: {3}\n" +
                    "ShouldRecall: {4}\n" + "IsUnderAttack: {5}\n" + "DamageTaken: {6}\n" + "AttackedByTurret: {7}\n" +
                    "NextItemPrice: {8}\n" + "CurrentMonster: {9}\n" + "GameState: {10}\n" + "MinionsAround: {11}\n" +
                    "SmiteableMob: {12}\n" + "InDanger: {13}\n" + "Afk: {14}\n" + "DamageCount: {15}\n", WaitOnFountain,
                    MoveTo.ToString(), LastCheckPoint.ToString(), Target == null ? "null" : Target.Name, ShouldRecall,
                    IsUnderAttack(), DamageTaken, AttackedByTurret, NextItemPrice, CurrentMonster, GameState,
                    MinionsAround, SmiteableMob == null ? "null" : SmiteableMob.Name, InDanger, afk, DamageCount);
            Console.WriteLine(result);
        }
    }

    internal enum State
    {
        Defending,
        Pushing,
        Grouping,
        Warding,
        Jungling,
        Ganking,
        LaneClear,
        Positioning,
        FightIng,
        Objective,
        Retreat,
        Null
    }
}