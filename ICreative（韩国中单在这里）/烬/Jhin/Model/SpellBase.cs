using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using Jhin;
using Jhin.Managers;
using Jhin.Utilities;
using SharpDX;

namespace Jhin.Model
{
    public enum SpellType
    {
        Self,
        Targeted,
        Linear,
        Circular,
        Cone,
        Unknown
    }

    public enum LastHitType
    {
        Never,
        Smartly,
        Always
    }

    public class CustomSettings
    {
        public int AllowedCollisionCount = int.MaxValue;
        public int CastDelay;
        public int HitChancePercent;
        public int Range;
        public GameObject Source;
        public int Speed;
        public SpellType Type = SpellType.Unknown;
        public int Width;
    }

    public class SpellBase
    {
        private readonly BestPositionResult _bestCircularObject = new BestPositionResult();

        private readonly BestPositionResult _bestObjectInLine = new BestPositionResult();
        private readonly Dictionary<Obj_AI_Base, float> _cachedDamage = new Dictionary<Obj_AI_Base, float>();

        private readonly Dictionary<int, Dictionary<int, Dictionary<int, bool>>> _cachedIsOnSegment =
            new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();

        private readonly Dictionary<int, Dictionary<int, bool>> _cachedObjectsInRange =
            new Dictionary<int, Dictionary<int, bool>>();

        private readonly List<Obj_AI_Base> _enemyHeroes = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _enemyMinions = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _killableMinions = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _laneClearMinions = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _monsters = new List<Obj_AI_Base>();

        public readonly Dictionary<int, Dictionary<int, PredictionResult>> CachedPredictions =
            new Dictionary<int, Dictionary<int, PredictionResult>>();

        public bool EnemyHeroesCanBeCalculated;
        public bool EnemyMinionsCanBeCalculated;
        public bool LaneClearMinionsCanBeCalculated;
        public bool MonstersCanBeCalculated;

        private string _name;
        private int _speed;

        private int _width;

        public int AllowedCollisionCount = int.MaxValue;
        public bool Aoe;

        public int CastDelay;
        public bool CollidesWithYasuoWall = true;

        public int LastCastTime;
        public Vector3 LastEndPosition;
        public int LastSentTime;
        public Vector3 LastStartPosition;
        public float MinHitChancePercent = 65f;

        public int Range;

        public GameObject RangeCheckSourceObject = AIO.MyHero;

        public Slider Slider;
        public SpellSlot Slot;

        public GameObject SourceObject = AIO.MyHero;
        public SpellType Type;

        public SpellBase(SpellSlot slot, SpellType? type, int range = int.MaxValue)
        {
            Slot = slot;
            Type = type ?? SpellType.Self;
            Range = range;
            Game.OnTick += delegate
            {
                if (FpsBooster.CanBeExecuted(CalculationType.Prediction))
                {
                }
                foreach (var pair in CachedPredictions)
                {
                    pair.Value.Clear();
                }
                if (FpsBooster.CanBeExecuted(CalculationType.Damage))
                {
                    _cachedDamage.Clear();
                }
                if (FpsBooster.CanBeExecuted(CalculationType.HealthPrediction))
                {
                    _killableMinions.Clear();
                }
                if (FpsBooster.CanBeExecuted(CalculationType.IsValidTarget))
                {
                }
                foreach (var pair in _cachedObjectsInRange)
                {
                    pair.Value.Clear();
                }
                foreach (var pair in _cachedIsOnSegment.SelectMany(pair => pair.Value))
                {
                    pair.Value.Clear();
                }
                EnemyHeroesCanBeCalculated = true;
                LaneClearMinionsCanBeCalculated = true;
                EnemyMinionsCanBeCalculated = true;
                MonstersCanBeCalculated = true;
                _bestObjectInLine.Hits = 0;
                _bestCircularObject.Hits = 0;
            };
        }

        public SpellDataInst Instance
        {
            get { return Slot != SpellSlot.Unknown ? AIO.MyHero.Spellbook.GetSpell(Slot) : null; }
        }

        public List<Obj_AI_Base> EnemyHeroes
        {
            get
            {
                if (EnemyHeroesCanBeCalculated)
                {
                    EnemyHeroesCanBeCalculated = false;
                    _enemyHeroes.Clear();
                    _enemyHeroes.AddRange(UnitManager.ValidEnemyHeroes.Where(InRange));
                }
                return _enemyHeroes;
            }
        }

        public List<Obj_AI_Base> LaneClearMinions
        {
            get
            {
                if (LaneClearMinionsCanBeCalculated)
                {
                    LaneClearMinionsCanBeCalculated = false;
                    _laneClearMinions.Clear();
                    _laneClearMinions.AddRange(
                        Orbwalker.LaneClearMinionsList.Concat(Orbwalker.UnKillableMinionsList).Where(InRange));
                }
                return _laneClearMinions;
            }
        }

        public List<Obj_AI_Base> EnemyMinions
        {
            get
            {
                if (EnemyMinionsCanBeCalculated)
                {
                    EnemyMinionsCanBeCalculated = false;
                    _enemyMinions.Clear();
                    _enemyMinions.AddRange(EntityManager.MinionsAndMonsters.EnemyMinions.Where(InRange));
                }
                return _enemyMinions;
            }
        }

        public List<Obj_AI_Base> Monsters
        {
            get
            {
                if (MonstersCanBeCalculated)
                {
                    MonstersCanBeCalculated = false;
                    _monsters.Clear();
                    _monsters.AddRange(
                        EntityManager.MinionsAndMonsters.Monsters.Where(InRange).OrderByDescending(m => m.MaxHealth));
                }
                return _monsters;
            }
        }

        public string SlotName
        {
            get { return _name ?? Slot.ToString(); }
            set { _name = value; }
        }

        public int Speed
        {
            get { return _speed > 0 ? _speed : int.MaxValue; }
            set { _speed = value; }
        }

        public int Width
        {
            get { return _width > 0 ? _width : 1; }
            set { _width = value; }
        }

        public int Radius
        {
            get { return Width / 2; }
        }

        public bool SourceObjectIsValid
        {
            get { return SourceObject != null && SourceObject.IsValid && !SourceObject.IsDead; }
        }

        public GameObject Source
        {
            get { return SourceObjectIsValid ? SourceObject : AIO.MyHero; }
        }

        public bool RangeCheckSourceObjectIsValid
        {
            get
            {
                return RangeCheckSourceObject != null && RangeCheckSourceObject.IsValid && !RangeCheckSourceObject.IsDead;
            }
        }

        public GameObject RangeCheckSource
        {
            get { return RangeCheckSourceObjectIsValid ? RangeCheckSourceObject : AIO.MyHero; }
        }

        public bool IsLearned
        {
            get { return Slot != SpellSlot.Unknown && Instance.IsLearned; }
        }

        public string Name
        {
            get { return Slot != SpellSlot.Unknown ? Instance.Name : ""; }
        }

        public bool IsReady
        {
            get { return Slot != SpellSlot.Unknown && Instance.IsReady; }
        }

        public float Mana
        {
            get { return Slot != SpellSlot.Unknown ? Instance.SData.Mana : 0; }
        }

        public int Level
        {
            get { return Slot != SpellSlot.Unknown ? Instance.Level : 0; }
        }

        public float Cooldown
        {
            get { return Slot != SpellSlot.Unknown ? Instance.Cooldown : 0; }
        }

        public int RangeSqr
        {
            get { return Range * Range; }
        }

        public float HitChancePercent
        {
            get
            {
                if (Slider != null)
                {
                    return Slider.CurrentValue + (ModeManager.Harass ? 5f : 0f);
                }
                return MinHitChancePercent + (ModeManager.Harass ? 5f : 0f);
            }
        }

        public float GetDamage(Obj_AI_Base target)
        {
            if (!IsReady && Slot != SpellSlot.R)
            {
                return 0f;
            }
            if (!_cachedDamage.ContainsKey(target))
            {
                _cachedDamage[target] = AIO.CurrentChampion.GetSpellDamage(Slot, target);
            }
            return _cachedDamage[target];
        }

        public bool IsKillable(Obj_AI_Base target)
        {
            return target.TotalShieldHealth() + target.HPRegenRate * 2 <= GetDamage(target);
        }

        public SpellBase AddConfigurableHitChancePercent(int defaultValue = 0)
        {
            if (!MenuManager.SubMenus.ContainsKey("Prediction"))
            {
                MenuManager.AddSubMenu("Prediction");
            }
            Slider = MenuManager.GetSubMenu("Prediction")
                .AddValue(SlotName,
                    new Slider(SlotName + ": HitChancePercent",
                        defaultValue > 0 ? defaultValue : (int)MinHitChancePercent));
            return this;
        }

        public SpellBase AddDrawings(bool defaultValue = true, ColorBGRA? color = null)
        {
            var checkBox = MenuManager.GetSubMenu("Drawings")
                .AddValue(SlotName, new CheckBox("Draw " + SlotName + " range", defaultValue));
            CircleManager.Circles.Add(new Circle(checkBox, color ?? new ColorBGRA(255, 255, 255, 100), () => Range,
                () => IsReady, () => RangeCheckSource));
            return this;
        }

        public SpellBase SetSourceFunction(Func<GameObject> func)
        {
            Game.OnTick += delegate { SourceObject = func(); };
            return this;
        }

        public SpellBase SetRangeCheckSourceFunction(Func<GameObject> func)
        {
            Game.OnTick += delegate { RangeCheckSourceObject = func(); };
            return this;
        }

        public SpellBase SetRangeFunction(Func<int> func)
        {
            Game.OnTick += delegate { Range = func(); };
            return this;
        }

        public SpellBase SetSlotFunction(Func<SpellSlot> func)
        {
            Game.OnTick += delegate { Slot = func(); };
            return this;
        }

        public int GetArrivalTime(Obj_AI_Base target)
        {
            var result = CastDelay;
            if (Speed != int.MaxValue)
            {
                result += (int)(1000 * SourceObject.GetDistance(target) / Speed);
            }
            return result;
        }

        public int GetArrivalTime(Vector3 position)
        {
            var result = CastDelay;
            if (Speed != int.MaxValue)
            {
                result += (int)(1000 * SourceObject.Distance(position) / Speed);
            }
            return result;
        }

        public bool InRange(Obj_AI_Base target)
        {
            switch (Type)
            {
                case SpellType.Targeted:
                    return RangeCheckSourceObject.InRange(target,
                        Range + AIO.MyHero.BoundingRadius + target.BoundingRadius - 65);
                case SpellType.Circular:
                    return RangeCheckSourceObject.InRange(target, Range + Radius + target.BoundingRadius);
                case SpellType.Linear:
                    return RangeCheckSourceObject.InRange(target, Range + Width + target.BoundingRadius);
            }
            //Self
            return RangeCheckSourceObject.InRange(target, Range + target.BoundingRadius);
        }

        public bool PredictedPosInRange(Obj_AI_Base target, GameObject sourceObj = null)
        {
            var source = sourceObj ?? RangeCheckSource;
            var pred = GetPrediction(target);
            switch (Type)
            {
                case SpellType.Targeted:
                    return RangeCheckSource.IsInRange(pred.CastPosition,
                        Range + AIO.MyHero.BoundingRadius + target.BoundingRadius - 65);
                case SpellType.Circular:
                    return source.IsInRange(pred.CastPosition, Range + Radius);
                case SpellType.Linear:
                    return source.IsInRange(pred.CastPosition, Range);
            }
            //Self
            return source.IsInRange(pred.CastPosition, Range + target.BoundingRadius / 2);
        }

        public PredictionResult GetPrediction(Obj_AI_Base target, CustomSettings custom = null)
        {
            var source = custom != null && custom.Source != null ? custom.Source : Source;
            if (!CachedPredictions.ContainsKey(source.NetworkId))
            {
                CachedPredictions.Add(source.NetworkId, new Dictionary<int, PredictionResult>());
            }
            if (!CachedPredictions[source.NetworkId].ContainsKey(target.NetworkId))
            {
                var speed = custom != null && custom.Speed > 0 ? custom.Speed : Speed;
                var castDelay = custom != null && custom.CastDelay > 0 ? custom.CastDelay : CastDelay;
                var range = custom != null && custom.Range > 0 ? custom.Range : Range;
                var width = custom != null && custom.Width > 0 ? custom.Width : Width;
                var allowedCollisionCount = custom != null && custom.AllowedCollisionCount > 0
                    ? custom.AllowedCollisionCount
                    : AllowedCollisionCount;
                var type = custom != null && custom.Type != SpellType.Unknown ? custom.Type : Type;
                PredictionResult result;
                switch (type)
                {
                    case SpellType.Circular:
                        result = Prediction.Position.PredictCircularMissile(target, range, width, castDelay, speed,
                            source.Position);
                        break;
                    case SpellType.Cone:
                        result = Prediction.Position.PredictConeSpell(target, range, width, castDelay, speed,
                            source.Position);
                        break;
                    case SpellType.Self:
                        result = Prediction.Position.PredictCircularMissile(target, range, width, castDelay, speed,
                            source.Position);
                        break;
                    default:
                        result = Prediction.Position.PredictLinearMissile(target, range, width, castDelay, speed,
                            allowedCollisionCount, source.Position);
                        break;
                }
                CachedPredictions[source.NetworkId].Add(target.NetworkId, result);
            }
            return CachedPredictions[source.NetworkId][target.NetworkId];
        }

        public bool WillHitYasuoWall(Vector3 position)
        {
            return Speed > 0 && CollidesWithYasuoWall && YasuoWallManager.WillHitYasuoWall(Source.Position, position);
        }

        public void Cast(Obj_AI_Base target, CustomSettings custom = null)
        {
            if (!IsReady || Chat.IsOpen || AIO.MyHero.Spellbook.IsCastingSpell || !InRange(target))
            {
                return;
            }
            if (ModeManager.Harass && target.Type == GameObjectType.AIHeroClient)
            {
                if (Orbwalker.ShouldWait)
                {
                    return;
                }
            }
            if (ModeManager.LaneClear || ModeManager.LastHit || ModeManager.Harass)
            {
                if (Orbwalker.LastTarget != null && Orbwalker.LastTarget.Type == GameObjectType.obj_AI_Minion &&
                    !Orbwalker.CanMove)
                {
                    return;
                }
            }
            if (Type == SpellType.Linear || Type == SpellType.Circular || Type == SpellType.Cone)
            {
                var pred = GetPrediction(target, custom);
                if (pred.HitChancePercent >= HitChancePercent)
                {
                    if (WillHitYasuoWall(pred.CastPosition) || !PredictedPosInRange(target))
                    {
                        return;
                    }
                    if (AIO.MyHero.Spellbook.CastSpell(Slot, pred.CastPosition))
                    {
                        LastSentTime = Core.GameTickCount;
                    }
                }
            }
            else if (Type == SpellType.Targeted)
            {
                if (WillHitYasuoWall(target.ServerPosition))
                {
                    return;
                }
                if (AIO.MyHero.Spellbook.CastSpell(Slot, target))
                {
                    LastSentTime = Core.GameTickCount;
                }
            }
            else if (Type == SpellType.Self)
            {
                var pred = GetPrediction(target, custom);
                if (pred.HitChancePercent >= HitChancePercent)
                {
                    if (!PredictedPosInRange(target))
                    {
                        return;
                    }
                    if (AIO.MyHero.Spellbook.CastSpell(Slot))
                    {
                        LastSentTime = Core.GameTickCount;
                    }
                }
            }
        }

        public void Cast(Vector3 position)
        {
            if (!IsReady || Chat.IsOpen || AIO.MyHero.Spellbook.IsCastingSpell)
            {
                return;
            }
            if (ModeManager.LaneClear || ModeManager.LastHit || ModeManager.Harass)
            {
                if (Orbwalker.LastTarget != null && Orbwalker.LastTarget.Type == GameObjectType.obj_AI_Minion &&
                    !Orbwalker.CanMove)
                {
                    return;
                }
            }
            if (AIO.MyHero.Spellbook.CastSpell(Slot, position))
            {
                LastSentTime = Core.GameTickCount;
            }
        }

        public void Cast()
        {
            if (!IsReady || Chat.IsOpen || AIO.MyHero.Spellbook.IsCastingSpell)
            {
                return;
            }
            if (AIO.MyHero.Spellbook.CastSpell(Slot))
            {
                LastSentTime = Core.GameTickCount;
            }
        }

        public Obj_AI_Minion LaneClear(bool useCast = true, int numberOfHits = 1)
        {
            if (IsReady && numberOfHits >= 1)
            {
                BestPositionResult best;
                switch (Type)
                {
                    case SpellType.Linear:
                        best = GetBestObjectInLine(LaneClearMinions, numberOfHits);
                        if (best.Hits >= numberOfHits)
                        {
                            if (useCast)
                            {
                                Cast(best.Position);
                            }
                            return best.Target as Obj_AI_Minion;
                        }
                        break;
                    case SpellType.Circular:
                        best = GetBestCircularObject(LaneClearMinions, numberOfHits);
                        if (best.Hits >= numberOfHits)
                        {
                            if (useCast)
                            {
                                Cast(best.Position);
                            }
                            return best.Target as Obj_AI_Minion;
                        }
                        break;
                    case SpellType.Self:
                        var objects = ObjectsInRange(LaneClearMinions);
                        if (objects.Count >= numberOfHits)
                        {
                            if (useCast)
                            {
                                Cast();
                            }
                            return objects.FirstOrDefault() as Obj_AI_Minion;
                        }
                        break;
                    case SpellType.Targeted:
                        foreach (var minion in LaneClearMinions)
                        {
                            if (useCast)
                            {
                                Cast(minion);
                            }
                            return minion as Obj_AI_Minion;
                        }
                        break;
                }
            }
            return null;
        }

        public Obj_AI_Minion JungleClear(bool useCast = true, int numberOfHits = 1)
        {
            if (IsReady && numberOfHits >= 1)
            {
                BestPositionResult best;
                switch (Type)
                {
                    case SpellType.Linear:
                        best = GetBestObjectInLine(Monsters, numberOfHits);
                        if (best.Hits >= numberOfHits)
                        {
                            if (useCast)
                            {
                                Cast(best.Position);
                            }
                            return best.Target as Obj_AI_Minion;
                        }
                        break;
                    case SpellType.Circular:
                        best = GetBestCircularObject(Monsters, numberOfHits);
                        if (best.Hits >= numberOfHits)
                        {
                            if (useCast)
                            {
                                Cast(best.Position);
                            }
                            return best.Target as Obj_AI_Minion;
                        }
                        break;
                    case SpellType.Self:
                        var objects = ObjectsInRange(Monsters);
                        if (objects.Count >= numberOfHits)
                        {
                            if (useCast)
                            {
                                Cast();
                            }
                            return objects.FirstOrDefault() as Obj_AI_Minion;
                        }
                        break;
                    case SpellType.Targeted:
                        var minion = Monsters.FirstOrDefault();
                        if (minion != null)
                        {
                            if (useCast)
                            {
                                Cast(minion);
                            }
                            return minion as Obj_AI_Minion;
                        }
                        break;
                }
            }
            return null;
        }

        public List<Obj_AI_Base> LastHit(LastHitType? t, bool useCast = true)
        {
            var type = t ?? LastHitType.Smartly;
            if (IsReady && type > LastHitType.Never)
            {
                if (_killableMinions.Count == 0)
                {
                    if (type == LastHitType.Smartly)
                    {
                        if (LaneClearMinions.Count > 0)
                        {
                            _killableMinions.AddRange(
                                Prediction.Health.GetPrediction(LaneClearMinions.ToDictionary(minion => minion,
                                    GetArrivalTime))
                                    .Where(
                                        tuple =>
                                            tuple.Value > 0 && GetDamage(tuple.Key) >= tuple.Value &&
                                            tuple.Key.Health > tuple.Value)
                                    .OrderByDescending(pair => pair.Key.MaxHealth)
                                    .ThenBy(pair => pair.Value)
                                    .ToDictionary(tuple => tuple.Key, tuple => tuple.Value)
                                    .Keys);
                        }
                    }
                    else if (type == LastHitType.Always)
                    {
                        if (EnemyMinions.Count > 0)
                        {
                            _killableMinions.AddRange(
                                Prediction.Health.GetPrediction(EnemyMinions.ToDictionary(minion => minion,
                                    GetArrivalTime))
                                    .Where(tuple => tuple.Value > 0 && GetDamage(tuple.Key) >= tuple.Value)
                                    .OrderByDescending(pair => pair.Key.MaxHealth)
                                    .ThenBy(pair => pair.Value)
                                    .ToDictionary(tuple => tuple.Key, tuple => tuple.Value)
                                    .Keys);
                        }
                    }
                }
                var first = _killableMinions.FirstOrDefault();
                if (useCast && first != null)
                {
                    Cast(first);
                }
            }
            return _killableMinions;
        }

        public bool IsOnSegment(Obj_AI_Base point, GameObject startPoint, Obj_AI_Base endPoint)
        {
            if (!_cachedIsOnSegment.ContainsKey(point.NetworkId))
            {
                _cachedIsOnSegment.Add(point.NetworkId, new Dictionary<int, Dictionary<int, bool>>());
            }
            if (!_cachedIsOnSegment[point.NetworkId].ContainsKey(startPoint.NetworkId))
            {
                _cachedIsOnSegment[point.NetworkId].Add(startPoint.NetworkId, new Dictionary<int, bool>());
            }
            if (!_cachedIsOnSegment[point.NetworkId][startPoint.NetworkId].ContainsKey(endPoint.NetworkId))
            {
                _cachedIsOnSegment[point.NetworkId][startPoint.NetworkId].Add(endPoint.NetworkId,
                    GetPrediction(point).HitChancePercent >= HitChancePercent / 2 &&
                    GetPrediction(point)
                        .CastPosition.To2D()
                        .Distance(startPoint.Position.To2D(), GetPrediction(endPoint).CastPosition.To2D(), true, true) <=
                    (Radius + point.BoundingRadius).Pow());
            }
            if (!_cachedIsOnSegment[point.NetworkId].ContainsKey(endPoint.NetworkId))
            {
                _cachedIsOnSegment[point.NetworkId].Add(endPoint.NetworkId, new Dictionary<int, bool>());
            }
            if (!_cachedIsOnSegment[point.NetworkId][endPoint.NetworkId].ContainsKey(startPoint.NetworkId))
            {
                _cachedIsOnSegment[point.NetworkId][endPoint.NetworkId].Add(startPoint.NetworkId,
                    _cachedIsOnSegment[point.NetworkId][startPoint.NetworkId][endPoint.NetworkId]);
            }
            return _cachedIsOnSegment[point.NetworkId][startPoint.NetworkId][endPoint.NetworkId];
        }

        public List<Obj_AI_Base> ObjectsInLine(List<Obj_AI_Base> list, Obj_AI_Base baseEnd)
        {
            var hash = new List<Obj_AI_Base>();
            if (IsReady)
            {
                hash.AddRange(list.Where(obj => IsOnSegment(obj, Source, baseEnd)));
            }
            return hash;
        }

        public BestPositionResult GetBestObjectInLine(List<Obj_AI_Base> objAiBases, int minHits = 1,
            Obj_AI_Base target = null)
        {
            if (_bestObjectInLine.Hits == 0)
            {
                if (IsReady && objAiBases.Count >= minHits)
                {
                    var checkTarget = target != null;
                    foreach (var obj in objAiBases)
                    {
                        var pred = GetPrediction(obj);
                        if (pred.HitChancePercent >= HitChancePercent)
                        {
                            var res = ObjectsInLine(objAiBases.Where(o => !o.IdEquals(obj)).ToList(), obj);
                            res.Add(obj);
                            if (!checkTarget || res.Contains(target))
                            {
                                var count = res.Count;
                                if (_bestObjectInLine.Hits < count)
                                {
                                    _bestObjectInLine.Hits = count;
                                    _bestObjectInLine.Position = pred.CastPosition;
                                    _bestObjectInLine.Target = obj;
                                    _bestObjectInLine.ObjectsHit = res;
                                    if (_bestObjectInLine.Hits == objAiBases.Count)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return _bestObjectInLine;
        }

        public List<Obj_AI_Base> ObjectsInRange(List<Obj_AI_Base> list, GameObject startObj = null)
        {
            var hash = new List<Obj_AI_Base>();
            if (IsReady && list.Count > 0)
            {
                var startObject = startObj ?? Source;
                hash.AddRange(
                    list.Where(
                        obj =>
                            GetPrediction(obj).HitChancePercent >= HitChancePercent &&
                            PredictedPosInRange(obj, startObject)));
            }
            return hash;
        }

        public bool InRange(Obj_AI_Base target1, Obj_AI_Base target2)
        {
            if (!_cachedObjectsInRange.ContainsKey(target1.NetworkId))
            {
                _cachedObjectsInRange.Add(target1.NetworkId, new Dictionary<int, bool>());
            }
            if (!_cachedObjectsInRange[target1.NetworkId].ContainsKey(target2.NetworkId))
            {
                _cachedObjectsInRange[target1.NetworkId].Add(target2.NetworkId,
                    GetPrediction(target2).HitChancePercent >= HitChancePercent / 2 &&
                    GetPrediction(target1)
                        .CastPosition.IsInRange(GetPrediction(target2).CastPosition, Radius + target1.BoundingRadius));
            }
            if (!_cachedObjectsInRange.ContainsKey(target2.NetworkId))
            {
                _cachedObjectsInRange.Add(target2.NetworkId, new Dictionary<int, bool>());
            }
            if (!_cachedObjectsInRange[target2.NetworkId].ContainsKey(target1.NetworkId))
            {
                _cachedObjectsInRange[target2.NetworkId].Add(target1.NetworkId,
                    _cachedObjectsInRange[target1.NetworkId][target2.NetworkId]);
            }
            return _cachedObjectsInRange[target1.NetworkId][target2.NetworkId];
        }

        public BestPositionResult GetBestCircularObject(List<Obj_AI_Base> objAiBases, int minHits = 1,
            Obj_AI_Base target = null)
        {
            if (_bestCircularObject.Hits == 0)
            {
                _bestCircularObject.Hits = 0;
                if (IsReady && objAiBases.Count >= minHits)
                {
                    var checkTarget = target != null;
                    foreach (var obj in objAiBases)
                    {
                        var pred = GetPrediction(obj);
                        if (pred.HitChancePercent >= HitChancePercent)
                        {
                            var list = objAiBases.Where(o => !o.IdEquals(obj) && InRange(obj, o)).ToList();
                            list.Add(obj);
                            if (!checkTarget || list.Contains(target))
                            {
                                var count = list.Count;
                                if (_bestCircularObject.Hits < count)
                                {
                                    _bestCircularObject.Hits = count;
                                    _bestCircularObject.Position = pred.CastPosition;
                                    _bestCircularObject.Target = obj;
                                    _bestCircularObject.ObjectsHit = list;
                                    if (_bestCircularObject.Hits == objAiBases.Count)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return _bestCircularObject;
        }
    }
}
