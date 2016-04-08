using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Notifications;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace Baseult
{
    class Baseult
    {
        public static int LineThickness = 4;

        public static int Length = 260;

        public static int Height = 25;

        public static readonly List<Recall> Recalls = new List<Recall>();

        public static readonly List<BaseUltUnit> BaseUltUnits = new List<BaseUltUnit>();

        public static readonly List<BaseUltSpell> BaseUltSpells = new List<BaseUltSpell>();

        public static readonly List<Champion> CompatibleChampions = new List<Champion>
        {
            Champion.Ashe,
            Champion.Draven,
            Champion.Ezreal,
            Champion.Jinx
        };

        public static bool IsCompatibleChamp()
        {
            return CompatibleChampions.Any(x => x.Equals(Player.Instance.Hero));
        }

        public static void Load()
        {
            Teleport.OnTeleport += OnTeleport;
            Game.OnUpdate += Game_OnUpdate;

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                Recalls.Add(new Recall(hero, RecallStatus.Inactive));
            }

            #region Spells

            BaseUltSpells.Add(new BaseUltSpell("Ashe", SpellSlot.R, 250, 1600, 130, true));
            BaseUltSpells.Add(new BaseUltSpell("Draven", SpellSlot.R, 400, 2000, 160, true));
            BaseUltSpells.Add(new BaseUltSpell("Ezreal", SpellSlot.R, 1000, 2000, 160, false));
            BaseUltSpells.Add(new BaseUltSpell("Jinx", SpellSlot.R, 600, 1700, 140, true));

            #endregion
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            foreach (var recall in Recalls)
            {
                if (recall.Status == RecallStatus.Active)
                {
                    if (recall.Unit.IsEnemy
                        && BaseUltUnits.All(h => h.Unit.NetworkId != recall.Unit.NetworkId))
                    {
                        var spell = BaseUltSpells.Find(h => h.Name == Player.Instance.ChampionName);
                        if (Player.Instance.Spellbook.GetSpell(spell.Slot).IsReady
                            && Player.Instance.Spellbook.GetSpell(spell.Slot).Level > 0)
                        {
                            BaseUltCalcs(recall);
                        }
                    }
                }

                if (recall.Status != RecallStatus.Active)
                {
                    var baseultUnit = BaseUltUnits.Find(h => h.Unit.NetworkId == recall.Unit.NetworkId);
                    if (baseultUnit != null)
                    {
                        BaseUltUnits.Remove(baseultUnit);
                    }
                }
            }

            foreach (var unit in BaseUltUnits)
            {
                if (unit.Collision)
                {
                    continue;
                }

                if (unit.Unit.IsVisible)
                {
                    unit.LastSeen = Game.Time;
                }

                if (Math.Round(unit.FireTime, 1) == Math.Round(Game.Time, 1) && Game.Time >= unit.LastSeen)
                {
                    var spell =
                        Player.Instance.Spellbook.GetSpell(
                            BaseUltSpells.Find(h => h.Name == Player.Instance.ChampionName).Slot);
                    if (spell.IsReady)
                    {
                        Player.Instance.Spellbook.CastSpell(spell.Slot, GetFountainPos());
                    }
                }
            }
        }

        public static void DrawRect(float x, float y, float width, float height, float thickness, Color color)
        {
            for (var i = 0; i < height; i++)
            {
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
            }
        }

        public static Vector3 GetFountainPos()
        {
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
                    {
                        return Player.Instance.Team == GameObjectTeam.Order
                            ? new Vector3(14296, 14362, 171)
                            : new Vector3(408, 414, 182);
                    }
            }

            return new Vector3();
        }

        public static double GetRecallPercent(Recall recall)
        {
            var recallDuration = recall.Duration;
            var cd = recall.Started + recallDuration - Environment.TickCount;
            var percent = cd / recallDuration;
            return percent;
        }

        public static float GetBaseUltTravelTime(float delay, float speed)
        {
            var distance = Vector3.Distance(Player.Instance.ServerPosition, GetFountainPos());
            var missilespeed = speed;
            if (Player.Instance.ChampionName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f;
                var acceldifference = distance - 1350f;
                if (acceldifference > 150f)
                {
                    acceldifference = 150f;
                }

                var difference = distance - 1500f;
                missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference)
                                + difference * 2200f) / distance;
            }

            return distance / missilespeed + (delay - 65) / 1000;
        }

        public static double GetBaseUltSpellDamage(BaseUltSpell spell, AIHeroClient target)
        {
            var level = Player.Instance.Spellbook.GetSpell(spell.Slot).Level - 1;
            switch (spell.Name)
            {
                case "Ashe":
                    {
                        var damage = new float[] { 250, 425, 600 }[level] + 1 * Player.Instance.FlatMagicDamageMod;
                        return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, damage);
                    }

                case "Draven":
                    {
                        var damage = new float[] { 175, 275, 375 }[level] + 1.1f * Player.Instance.FlatPhysicalDamageMod;
                        return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, damage) * 0.7;
                    }

                case "Ezreal":
                    {
                        var damage = new float[] { 350, 500, 650 }[level] + 0.9f * Player.Instance.FlatMagicDamageMod
                                     + 1 * Player.Instance.FlatPhysicalDamageMod;
                        return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, damage) * 0.7;
                    }

                case "Jinx":
                    {
                        var damage = new float[] { 250, 350, 450 }[level]
                                     + new float[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health)
                                     + 1 * Player.Instance.FlatPhysicalDamageMod;
                        /*Chat.Print("Flat Damage: " + new float[] { 250, 350, 450 }[level]);
                        Chat.Print("Bonus Damage: " + new float[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health));
                        Chat.Print("Damage On Unit: " + Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, damage));
                        Chat.Print("Unit Health: " + target.Health);*/
                        return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, damage);
                    }
            }

            return 0;
        }

        private static void BaseUltCalcs(Recall recall)
        {
            var finishedRecall = recall.Started + recall.Duration;
            var spellData = BaseUltSpells.Find(h => h.Name == Player.Instance.ChampionName);
            var timeNeeded = GetBaseUltTravelTime(spellData.Delay, spellData.Speed);
            var fireTime = finishedRecall - timeNeeded;
            var spellDmg = GetBaseUltSpellDamage(spellData, recall.Unit) - recall.Unit.MaxHealth * 0.1;
            var collision = GetCollision(spellData.Radius, spellData).Any();
            if (fireTime > Game.Time && fireTime < recall.Started + recall.Duration && recall.Unit.Health < spellDmg
                && BaseultMenu.CheckBox(BaseultMenu.Settings,"Ult/" + recall.Unit.ChampionName) && BaseultMenu.CheckBox(BaseultMenu.Settings,"UseBaseUlt"))
            {
                BaseUltUnits.Add(new BaseUltUnit(recall.Unit, fireTime, collision));
            }
            else if (BaseUltUnits.Any(h => h.Unit.NetworkId == recall.Unit.NetworkId))
            {
                BaseUltUnits.Remove(BaseUltUnits.Find(h => h.Unit.NetworkId == recall.Unit.NetworkId));
            }
        }

        public static void OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var unit = Recalls.Find(h => h.Unit.NetworkId == sender.NetworkId);

            if (unit == null || args.Type != TeleportType.Recall)
                return;

            switch (args.Status)
            {
                case TeleportStatus.Start:
                    {
                        unit.Status = RecallStatus.Active;
                        unit.Started = Game.Time;
                        unit.Duration = (float)args.Duration / 1000;
                        break;
                    }

                case TeleportStatus.Abort:
                    {
                        unit.Status = RecallStatus.Abort;
                        unit.Ended = Game.Time;

                        if (Game.Time == unit.Ended)
                        {
                            Core.DelayAction(() => unit.Status = RecallStatus.Inactive, 2000);
                        }
                        break;
                    }

                case TeleportStatus.Finish:
                    {
                        unit.Status = RecallStatus.Finished;
                        unit.Ended = Game.Time;

                        if (Game.Time == unit.Ended)
                        {
                            Core.DelayAction(() => unit.Status = RecallStatus.Inactive, 2000);
                        }
                        break;
                    }
            }
        }

        public static IEnumerable<Obj_AI_Base> GetCollision(float spellwidth, BaseUltSpell spell)
        {
            return (from unit in EntityManager.Heroes.Enemies.Where(h => Player.Instance.Distance(h) < 2000)
                    let pred =
                        Prediction.Position.PredictLinearMissile(
                            unit,
                            2000,
                            (int)spell.Radius,
                            (int)spell.Delay,
                            spell.Speed,
                            -1)
                    let endpos = Player.Instance.ServerPosition.Extend(GetFountainPos(), 2000)
                    let projectOn = pred.UnitPosition.To2D().ProjectOn(Player.Instance.ServerPosition.To2D(), endpos)
                    where projectOn.SegmentPoint.Distance(endpos) < spellwidth + unit.BoundingRadius
                    select unit).Cast<Obj_AI_Base>().ToList();
        }
    }

    public class Recall
    {
        public int TextPos;

        public Recall(AIHeroClient unit, RecallStatus status)
        {
            Unit = unit;
            Status = status;
        }

        public AIHeroClient Unit { get; set; }

        public RecallStatus Status { get; set; }

        public float Started { get; set; }

        public float Ended { get; set; }

        public float Duration { get; set; }
    }

    public class BaseUltUnit
    {
        public BaseUltUnit(AIHeroClient unit, float fireTime, bool collision)
        {
            Unit = unit;
            FireTime = fireTime;
            Collision = collision;
        }

        public AIHeroClient Unit { get; set; }

        public float FireTime { get; set; }

        public bool Collision { get; set; }

        public float LastSeen { get; set; }

        public float PredictedPos { get; set; }
    }

    public class BaseUltSpell
    {
        public BaseUltSpell(string name, SpellSlot slot, float delay, float speed, float radius, bool collision)
        {
            Name = name;
            Slot = slot;
            Delay = delay;
            Speed = speed;
            Radius = radius;
            Collision = collision;
        }

        public string Name { get; set; }

        public SpellSlot Slot { get; set; }

        public float Delay { get; set; }

        public float Speed { get; set; }

        public float Radius { get; set; }

        public bool Collision { get; set; }
    }

    public enum RecallStatus
    {
        Active,
        Inactive,
        Finished,
        Abort
    }
}
