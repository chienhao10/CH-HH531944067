using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AdEvade.Config;
using AdEvade.Config.Controls;
using AdEvade.Data;
using AdEvade.Data.Spells;
using AdEvade.Data.Spells.SpecialSpells;
using AdEvade.Draw;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using SharpDX;

using Spell = AdEvade.Data.Spells.Spell;
using SpellData = AdEvade.Data.Spells.SpellData;

namespace AdEvade
{
    public class SpecialSpellEventArgs : EventArgs
    {
        public bool NoProcess { get; set; }
    }

    public class SpellDetector
    {
        public delegate void OnCreateSpellHandler(Spell spell);

        public static event OnCreateSpellHandler OnCreateSpell;

        public delegate void OnProcessDetectedSpellsHandler();

        public static event OnProcessDetectedSpellsHandler OnProcessDetectedSpells;

        public delegate void OnProcessSpecialSpellHandler(Obj_AI_Base hero,
            GameObjectProcessSpellCastEventArgs args,
            SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs);

        public static event OnProcessSpecialSpellHandler OnProcessSpecialSpell;

        //public static event OnDeleteSpellHandler OnDeleteSpell;

        public static Dictionary<int, Spell> Spells = new Dictionary<int, Spell>();
        public static Dictionary<int, Spell> DrawSpells = new Dictionary<int, Spell>();
        public static Dictionary<int, Spell> DetectedSpells = new Dictionary<int, Spell>();

        public static Dictionary<string, IChampionPlugin> ChampionPlugins = new Dictionary<string, IChampionPlugin>();

        public static Dictionary<string, string> ChanneledSpells = new Dictionary<string, string>();

        public static Dictionary<string, SpellData> OnProcessSpells = new Dictionary<string, SpellData>();
        public static Dictionary<string, SpellData> OnMissileSpells = new Dictionary<string, SpellData>();

        public static Dictionary<string, SpellData> WindupSpells = new Dictionary<string, SpellData>();

        private static int _spellIdCount = 0;

        private static AIHeroClient MyHero
        {
            get { return ObjectManager.Player; }
        }

        public static float LastCheckTime = 0;
        public static float LastCheckSpellCollisionTime = 0;

        public static Menu SpellMenu;

        public SpellDetector(Menu mainMenu)
        {
            GameObject.OnCreate += SpellMissile_OnCreate;
            GameObject.OnDelete += SpellMissile_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Game.OnUpdate += Game_OnGameUpdate;
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;

            SpellMenu = mainMenu.AddSubMenu("技能", "技能");
        }

        private void Loading_OnLoadingComplete(EventArgs args)
        {
        }



        private void SpellMissile_OnCreate(GameObject obj, EventArgs args)
        {
            MissileClient missile;
            if (obj.IsMissileClient(out missile))
                return;

            SpellData spellData;
            if (missile.IsValidEvadeSpell(out spellData))
            {
                if (missile.IsInRange(spellData))
                {
                    var hero = missile.SpellCaster;

                    if (!hero.IsVisible && ConfigValue.DodgeFowSpells.GetBool())
                    {
                        CreateSpellData(hero, missile.StartPosition, missile.EndPosition, spellData, obj);
                        return;
                    }

                    if (spellData.UsePackets)
                    {
                        CreateSpellData(hero, missile.StartPosition, missile.EndPosition, spellData, obj);
                        return;
                    }

                    foreach (KeyValuePair<int, Spell> entry in Spells)
                    {
                        Spell spell = entry.Value;

                        var dir = (missile.EndPosition.To2D() - missile.StartPosition.To2D()).Normalized();

                        if (spell.Info.MissileName == missile.SData.Name
                            && spell.HeroId == missile.SpellCaster.NetworkId
                            && dir.AngleBetween(spell.Direction) < 10)
                        {
                            if (spell.Info.IsThreeWay == false
                                && spell.Info.IsSpecial == false)
                            {
                                spell.SpellObject = obj;
                            }
                        }
                    }
                }
            }
        }

        private void SpellMissile_OnDelete(GameObject obj, EventArgs args)
        {
            MissileClient missile;
            if (!obj.IsMissileClient(out missile))
                return;

            foreach (var spell in Spells.Values.ToList().Where(
                s => (s.SpellObject != null && s.SpellObject.NetworkId == missile.NetworkId))) //isAlive
            {
                Core.DelayAction(() => DeleteSpell(spell.SpellId), 1);
            }
        }

        public void RemoveNonDangerousSpells()
        {
            foreach (var spell in Spells.Values.ToList().Where(
                s => ((int) s.GetSpellDangerLevel() < (int) SpellDangerLevel.High)))
            {
                Core.DelayAction(() => DeleteSpell(spell.SpellId), 1);
            }
        }

        private void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            ConsoleDebug.WriteLine(string.Format("'{0}' casted Spell: {1}", hero.Name, args.SData.Name));
            try
            {
                SpellData spellData;
                if (args.SData.ShouldEvade(hero, out spellData))
                {
                    if (!spellData.UsePackets)
                    {
                        var specialSpellArgs = new SpecialSpellEventArgs();
                        if (OnProcessSpecialSpell != null)
                        {
                            OnProcessSpecialSpell(hero, args, spellData, specialSpellArgs);
                        }

                        if (specialSpellArgs.NoProcess == false && spellData.NoProcess == false)
                        {
                            CreateSpellData(hero, hero.ServerPosition, args.End, spellData, null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleDebug.WriteLine(e);
            }
        }

        public static void CreateSpellData(
            Obj_AI_Base hero,
            Vector3 spellStartPos,
            Vector3 spellEndPos,
            SpellData spellData,
            GameObject obj = null,
            float extraEndTick = 0.0f,
            bool processSpell = true,
            SpellType spellType = SpellType.None,
            bool checkEndExplosion = true,
            float spellRadius = 0)
        {
            if (checkEndExplosion && spellData.HasEndExplosion)
            {
                CreateSpellData(hero, spellStartPos, spellEndPos,
                    spellData, obj, extraEndTick, false,
                    spellData.SpellType, false);

                CreateSpellData(hero, spellStartPos, spellEndPos,
                    spellData, obj, extraEndTick, true,
                    SpellType.Circular, false);

                return;
            }
            if (spellStartPos.Distance(MyHero.Position) < spellData.Range + ConfigValue.ExtraDetectionRange.GetInt())
            {
                Vector2 startPosition = spellStartPos.To2D();
                Vector2 endPosition = spellEndPos.To2D();
                Vector2 direction = (endPosition - startPosition).Normalized();
                float endTick = 0;

                if (spellType == SpellType.None)
                {
                    spellType = spellData.SpellType;
                }

                if (spellData.FixedRange) //for diana q
                {
                    if (endPosition.Distance(startPosition) > spellData.Range)
                    {
                        endPosition = startPosition + direction * spellData.Range;
                    }
                }
                if (spellType == SpellType.Line)
                {
                    endTick = spellData.SpellDelay + (spellData.Range / spellData.ProjectileSpeed) * 1000;
                    endPosition = startPosition + direction * spellData.Range;

                    if (spellData.UseEndPosition)
                    {
                        var range = spellEndPos.To2D().Distance(spellStartPos.To2D());
                        endTick = spellData.SpellDelay + (range / spellData.ProjectileSpeed) * 1000;
                        endPosition = spellEndPos.To2D();
                    }

                    if (obj != null)
                        endTick -= spellData.SpellDelay;
                }
                else if (spellType == SpellType.Circular)
                {
                    endTick = spellData.SpellDelay;

                    if (spellData.ProjectileSpeed == 0)
                    {
                        endPosition = hero.ServerPosition.To2D();
                    }
                    else if (spellData.ProjectileSpeed > 0)
                    {
                        if (spellData.SpellType == SpellType.Line &&
                            spellData.HasEndExplosion &&
                            spellData.UseEndPosition == false)
                        {
                            endPosition = startPosition + direction * spellData.Range;
                        }

                        endTick = endTick + 1000 * startPosition.Distance(endPosition) / spellData.ProjectileSpeed;
                    }
                }
                else if (spellType == SpellType.Arc)
                {
                    endTick = endTick + 1000 * startPosition.Distance(endPosition) / spellData.ProjectileSpeed;

                    if (obj != null)
                        endTick -= spellData.SpellDelay;
                }
                else if (spellType == SpellType.Cone)
                {
                    return;
                }
                else
                {
                    return;
                }
                endTick += extraEndTick;
                Spell newSpell = new Spell();
                newSpell.StartTime = EvadeUtils.TickCount;
                newSpell.EndTime = EvadeUtils.TickCount + endTick;
                newSpell.StartPos = startPosition;
                newSpell.EndPos = endPosition;
                newSpell.Height = spellEndPos.Z + spellData.ExtraDrawHeight;
                newSpell.Direction = direction;
                newSpell.HeroId = hero.NetworkId;
                newSpell.Info = spellData;
                newSpell.SpellType = spellType;
                newSpell.Radius = spellRadius > 0 ? spellRadius : newSpell.GetSpellRadius();
                if (obj != null)
                {
                    newSpell.SpellObject = obj;
                    newSpell.ProjectileId = obj.NetworkId;
                }
                int spellId = CreateSpell(newSpell, processSpell);
                Core.DelayAction(() => DeleteSpell(spellId), (int) (endTick + spellData.ExtraEndTime));
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            UpdateSpells();

            if (EvadeUtils.TickCount - LastCheckSpellCollisionTime > 100)
            {
                CheckSpellCollision();
                LastCheckSpellCollisionTime = EvadeUtils.TickCount;
            }

            if (EvadeUtils.TickCount - LastCheckTime > 1)
            {
                //CheckCasterDead();                
                CheckSpellEndTime();
                AddDetectedSpells();
                LastCheckTime = EvadeUtils.TickCount;
            }
        }

        public static void UpdateSpells()
        {
            foreach (var spell in DetectedSpells.Values)
            {
                spell.UpdateSpellInfo();
            }
        }

        private void CheckSpellEndTime()
        {
            foreach (KeyValuePair<int, Spell> entry in DetectedSpells)
            {
                Spell spell = entry.Value;

                foreach (var hero in EntityManager.Heroes.Enemies)
                {
                    if (hero.IsDead && spell.HeroId == hero.NetworkId)
                    {
                        if (spell.SpellObject == null)
                            Core.DelayAction(() => DeleteSpell(entry.Key), 1);
                    }
                }

                if (spell.EndTime + spell.Info.ExtraEndTime < EvadeUtils.TickCount
                    || CanHeroWalkIntoSpell(spell) == false)
                {
                    Core.DelayAction(() => DeleteSpell(entry.Key), 1);
                }
            }
        }

        private static void CheckSpellCollision()
        {
            if (!ConfigValue.CheckSpellCollision.GetBool())
            {
                return;
            }

            foreach (KeyValuePair<int, Spell> entry in DetectedSpells)
            {
                Spell spell = entry.Value;

                var collisionObject = spell.CheckSpellCollision();

                if (collisionObject != null)
                {
                    spell.PredictedEndPos = spell.GetSpellProjection(collisionObject.ServerPosition.To2D());

                    if (spell.CurrentSpellPosition.Distance(collisionObject.ServerPosition)
                        < collisionObject.BoundingRadius + spell.Radius)
                    {
                        Core.DelayAction(() => DeleteSpell(entry.Key), 1);
                    }
                }
            }
        }

        public static bool CanHeroWalkIntoSpell(Spell spell)
        {
            if (ConfigValue.AdvancedSpellDetection.GetBool())
            {
                Vector2 heroPos = MyHero.Position.To2D();
                var extraDist = MyHero.Distance(GameData.HeroInfo.ServerPos2D);

                if (spell.SpellType == SpellType.Line)
                {
                    var walkRadius = GameData.HeroInfo.MoveSpeed * (spell.EndTime - EvadeUtils.TickCount) / 1000 +
                                     GameData.HeroInfo.BoundingRadius + spell.Info.Radius + extraDist + 10;
                    var spellPos = spell.CurrentSpellPosition;
                    var spellEndPos = spell.GetSpellEndPosition();

                    var projection = heroPos.ProjectOn(spellPos, spellEndPos);

                    return projection.SegmentPoint.Distance(heroPos) <= walkRadius;
                }
                else if (spell.SpellType == SpellType.Circular)
                {
                    var walkRadius = GameData.HeroInfo.MoveSpeed * (spell.EndTime - EvadeUtils.TickCount) / 1000 +
                                     GameData.HeroInfo.BoundingRadius + spell.Info.Radius + extraDist + 10;

                    if (heroPos.Distance(spell.EndPos) < walkRadius)
                    {
                        return true;
                    }

                }
                else if (spell.SpellType == SpellType.Arc)
                {
                    var spellRange = spell.StartPos.Distance(spell.EndPos);
                    var midPoint = spell.StartPos + spell.Direction * (spellRange / 2);
                    var arcRadius = spell.Info.Radius * (1 + spellRange / 100);

                    var walkRadius = GameData.HeroInfo.MoveSpeed * (spell.EndTime - EvadeUtils.TickCount) / 1000 +
                                     GameData.HeroInfo.BoundingRadius + arcRadius + extraDist + 10;

                    if (heroPos.Distance(midPoint) < walkRadius)
                    {
                        return true;
                    }

                }

                return false;
            }


            return true;
        }

        private static void AddDetectedSpells()
        {
            bool spellAdded = false;

            foreach (KeyValuePair<int, Spell> entry in DetectedSpells)
            {
                Spell spell = entry.Value;

                float evadeTime, spellHitTime;
                spell.CanHeroEvade(MyHero, out evadeTime, out spellHitTime);

                spell.SpellHitTime = spellHitTime;
                spell.EvadeTime = evadeTime;

                var extraDelay = Game.Ping + Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);

                if (spell.SpellHitTime - extraDelay < 1500 && CanHeroWalkIntoSpell(spell))
                    //if(true)
                {
                    Spell newSpell = spell;
                    int spellId = spell.SpellId;

                    if (!DrawSpells.ContainsKey(spell.SpellId))
                    {
                        DrawSpells.Add(spellId, newSpell);
                    }

                    //var spellFlyTime = Evade.GetTickCount - spell.startTime;
                    if (spellHitTime < ConfigValue.SpellDetectionTime.GetInt())
                    {
                        continue;
                    }

                    if (EvadeUtils.TickCount - spell.StartTime < ConfigValue.ReactionTime.GetInt())
                    {
                        continue;
                    }

                    var dodgeInterval = ConfigValue.DodgeInterval.GetInt();
                    if (AdEvade.LastPosInfo != null && dodgeInterval > 0)
                    {
                        var timeElapsed = EvadeUtils.TickCount - AdEvade.LastPosInfo.Timestamp;

                        if (dodgeInterval > timeElapsed)
                        {
                            //var delay = dodgeInterval - timeElapsed;
                            //DelayAction.Add((int)delay, () => SpellDetector_OnProcessDetectedSpells());
                            continue;
                        }
                    }

                    if (!Spells.ContainsKey(spell.SpellId))
                    {
                        if (Config.Properties.GetSpell(newSpell.Info.SpellName).Dodge && !(AdEvade.IsDodgeDangerousEnabled() && (int) newSpell.Dangerlevel < (int) SpellDangerLevel.High))
                        {
                            if (newSpell.SpellType == SpellType.Circular
                                && !ConfigValue.DodgeCircularSpells.GetBool())
                            {
                                //return spellID;
                                continue;
                            }

                            Spells.Add(spellId, newSpell);

                            spellAdded = true;
                        }
                    }

                    if (ConfigValue.CheckSpellCollision.GetBool() && spell.PredictedEndPos != Vector2.Zero)
                    {
                        spellAdded = false;
                    }
                }
            }

            if (spellAdded && OnProcessDetectedSpells != null)
            {
                OnProcessDetectedSpells();
            }
        }

        //public static int CreateTestSpell(SpellPoint spell, SpellData data)
        //{
        //    if (spell.StartPosition.Distance(MyHero.Position) < data.Range + ConfigValue.ExtraDetectionRange.GetInt())
        //    {
        //        Vector2 startPosition = spell.StartPosition.To2D();
        //        Vector2 endPosition = spell.EndPosition.To2D();
        //        Vector2 direction = (endPosition - startPosition).Normalized();
        //        float endTick = 0;

        //        if (data.FixedRange) //for diana q
        //        {
        //            if (endPosition.Distance(startPosition) > data.Range)
        //            {
        //                endPosition = startPosition + direction*data.Range;
        //            }
        //        }
        //        if (data.SpellType == SpellType.Line)
        //        {
        //            endTick = data.SpellDelay + (data.Range/data.ProjectileSpeed)*1000;
        //            endPosition = startPosition + direction*data.Range;

        //            if (data.UseEndPosition)
        //            {
        //                var range = endPosition.Distance(startPosition);
        //                endTick = data.SpellDelay + (range/data.ProjectileSpeed)*1000;
        //            }
        //        }
        //        else if (data.SpellType == SpellType.Circular)
        //        {
        //            endTick = data.SpellDelay;

        //            if (data.ProjectileSpeed == 0)
        //            {
        //                endPosition = startPosition;
        //            }
        //            else if (data.ProjectileSpeed > 0)
        //            {
        //                if (data.SpellType == SpellType.Line &&
        //                    data.HasEndExplosion &&
        //                    data.UseEndPosition == false)
        //                {
        //                    endPosition = startPosition + direction*data.Range;
        //                }

        //                endTick = endTick + 1000*startPosition.Distance(endPosition)/data.ProjectileSpeed;
        //            }
        //        }
        //        else if (data.SpellType == SpellType.Arc)
        //        {
        //            endTick = endTick + 1000*startPosition.Distance(endPosition)/data.ProjectileSpeed;
        //        }
        //        else if (data.SpellType == SpellType.Cone)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //        Spell newSpell = new Spell();
        //        newSpell.StartTime = EvadeUtils.TickCount;
        //        newSpell.EndTime = EvadeUtils.TickCount + endTick;
        //        newSpell.StartPos = startPosition;
        //        newSpell.EndPos = endPosition;
        //        newSpell.Height = spell.EndPosition.Z + data.ExtraDrawHeight;
        //        newSpell.Direction = direction;
        //        newSpell.HeroId = 0;
        //        newSpell.Info = data;
        //        newSpell.SpellType = data.SpellType;
        //        newSpell.Radius = data.Radius > 0 ? data.Radius : newSpell.GetSpellRadius();
        //        int spellId = CreateSpell(newSpell);
        //        Core.DelayAction(() => DeleteSpell(spellId), (int) (endTick + data.ExtraEndTime));
        //        return spellId;
        //    }
        //    return 0;
        //}

        private static int CreateSpell(Spell newSpell, bool processSpell = true)
        {
            //Debug.DrawTopLeft(newSpell);
            int spellId = _spellIdCount++;
            newSpell.SpellId = spellId;

            newSpell.UpdateSpellInfo();
            DetectedSpells.Add(spellId, newSpell);

            if (processSpell)
            {
                CheckSpellCollision();
                AddDetectedSpells();
            }
            if (OnCreateSpell != null)
                OnCreateSpell.Invoke(newSpell);
            return spellId;
        }

        public static void DeleteSpell(int spellId)
        {
            Spells.Remove(spellId);
            DrawSpells.Remove(spellId);
            DetectedSpells.Remove(spellId);
        }

        public static int GetCurrentSpellId()
        {
            return _spellIdCount;
        }

        public static List<int> GetSpellList()
        {
            List<int> spellList = new List<int>();

            foreach (KeyValuePair<int, Spell> entry in Spells)
            {
                Spell spell = entry.Value;
                spellList.Add(spell.SpellId);
            }

            return spellList;
        }

        public static int GetHighestDetectedSpellId()
        {
            int highest = 0;

            foreach (var spell in Spells)
            {
                highest = Math.Max(highest, spell.Key);
            }

            return highest;
        }

        public static float GetLowestEvadeTime(out Spell lowestSpell)
        {
            float lowest = float.MaxValue;
            lowestSpell = null;

            foreach (KeyValuePair<int, Spell> entry in Spells)
            {
                Spell spell = entry.Value;

                if (spell.SpellHitTime != float.MinValue)
                {
                    //ConsoleDebug.WriteLine("spellhittime: " + spell.spellHitTime);
                    lowest = Math.Min(lowest, (spell.SpellHitTime - spell.EvadeTime));
                    lowestSpell = spell;
                }
            }

            return lowest;
        }

        public static Spell GetMostDangerousSpell(bool hasProjectile = false)
        {
            int maxDanger = 0;
            Spell maxDangerSpell = null;

            foreach (Spell spell in Spells.Values)
            {
                if (!hasProjectile || (spell.Info.ProjectileSpeed > 0 && spell.Info.ProjectileSpeed != float.MaxValue))
                {
                    var dangerlevel = spell.Dangerlevel;

                    if ((int) dangerlevel > maxDanger)
                    {
                        maxDanger = (int) dangerlevel;
                        maxDangerSpell = spell;
                    }
                }
            }

            return maxDangerSpell;
        }

        public static void InitChannelSpells()
        {

            ChanneledSpells["Drain"] = "FiddleSticks";
            ChanneledSpells["Crowstorm"] = "FiddleSticks";
            ChanneledSpells["KatarinaR"] = "Katarina";
            ChanneledSpells["AbsoluteZero"] = "Nunu";
            ChanneledSpells["GalioIdolOfDurand"] = "Galio";
            ChanneledSpells["MissFortuneBulletTime"] = "MissFortune";
            ChanneledSpells["Meditate"] = "MasterYi";
            ChanneledSpells["NetherGrasp"] = "Malzahar";
            ChanneledSpells["ReapTheWhirlwind"] = "Janna";
            ChanneledSpells["KarthusFallenOne"] = "Karthus";
            ChanneledSpells["KarthusFallenOne2"] = "Karthus";
            ChanneledSpells["VelkozR"] = "Velkoz";
            ChanneledSpells["XerathLocusOfPower2"] = "Xerath";
            ChanneledSpells["ZacE"] = "Zac";
            ChanneledSpells["Pantheon_Heartseeker"] = "Pantheon";

            ChanneledSpells["OdinRecall"] = Constants.AllChampions;
            ChanneledSpells["Recall"] = Constants.AllChampions;

        }

        //public static void LoadDummySpell(SpellData spell)
        //{
        //    string menuName = spell.CharName + " (" + spell.SpellKey + ") Settings";

        //    var enableSpell = !spell.DefaultOff;

        //    var spellConfig = new SpellConfigControl(SpellMenu, menuName, spell, enableSpell);
        //    spellConfig.AddToMenu();

        //    Properties.Spells.Add(spell.Name, spell.GetSpellConfig(spellConfig));
        //}

        //Credits to Kurisu
        public static object NewInstance(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
            var il = dynamic.GetILGenerator();

            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            var method = (Func<object>) dynamic.CreateDelegate(typeof (Func<object>));
            return method();
        }

        private void LoadSpecialSpell(SpellData spell)
        {
            if (ChampionPlugins.ContainsKey(spell.CharName))
            {
                ChampionPlugins[spell.CharName].LoadSpecialSpell(spell);
            }

            ChampionPlugins[Constants.AllChampions].LoadSpecialSpell(spell);
        }

        private void LoadSpecialSpellPlugins()
        {
            ChampionPlugins.Add(Constants.AllChampions, new AllChampions());
            ConsoleDebug.WriteLine("        Loading Special Spell Plugins...");
            foreach (var hero in EntityManager.Heroes.AllHeroes)
            {
                var championPlugin = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.Name == hero.ChampionName && t.GetInterfaces().Contains(typeof (IChampionPlugin)));
                if (championPlugin != null)
                {
                    if (!ChampionPlugins.ContainsKey(hero.ChampionName))
                    {
                        var plugin = (IChampionPlugin) NewInstance(championPlugin);
                        ChampionPlugins.Add(hero.ChampionName, plugin);
                        ConsoleDebug.WriteLine("        Loaded Champion Plugin: " + plugin.GetChampionName());
                    }
                }
            }
        }

        public void LoadSpellDictionary()
        {
            LoadSpecialSpellPlugins();
            foreach (var hero in EntityManager.Heroes.AllHeroes)
            {
                try
                {

                    if (hero.IsMe)
                    {
                        foreach (var spell in SpellWindupDatabase.Spells.Where(
                            s => (s.CharName == hero.ChampionName)))
                        {
                            if (!WindupSpells.ContainsKey(spell.SpellName))
                            {
                                WindupSpells.Add(spell.SpellName, spell);
                            }
                        }
                    }
                    if (hero.Team != MyHero.Team)
                    {
                        ConsoleDebug.WriteLine("        Hero Found: " + hero.ChampionName);
                        foreach (var spell in SpellDatabase.Spells.Where(
                            s => (s.CharName == hero.ChampionName) || (s.CharName == Constants.AllChampions)))
                        {
                            ConsoleDebug.WriteLine("            Hero Spell Found: " + spell.SpellName);

                            if (!(spell.SpellType == SpellType.Circular
                                  || spell.SpellType == SpellType.Line
                                  || spell.SpellType == SpellType.Arc))
                                continue;

                            if (spell.CharName == Constants.AllChampions)
                            {
                                SpellSlot slot = hero.GetSpellSlotFromName(spell.SpellName);
                                if (slot == SpellSlot.Unknown)
                                {
                                    ConsoleDebug.WriteLineColor("           {0}: Not Found!", ConsoleColor.Yellow, false, spell.SpellName);
                                    continue;
                                }
                            }

                            if (!OnProcessSpells.ContainsKey(spell.SpellName))
                            {
                                if (spell.MissileName == "")
                                    spell.MissileName = spell.SpellName;

                                OnProcessSpells.Add(spell.SpellName, spell);
                                OnMissileSpells.Add(spell.MissileName, spell);

                                if (spell.ExtraSpellNames != null)
                                {
                                    foreach (string spellName in spell.ExtraSpellNames)
                                    {
                                        OnProcessSpells.Add(spellName, spell);
                                    }
                                }

                                if (spell.ExtraMissileNames != null)
                                {
                                    foreach (string spellName in spell.ExtraMissileNames)
                                    {
                                        OnMissileSpells.Add(spellName, spell);
                                    }
                                }

                                LoadSpecialSpell(spell);
                                if (!Config.Properties.Spells.Any(x => x.Key == spell.SpellName))
                                {
                                    string menuName = spell.CharName + " (" + spell.SpellKey + ") 设置";
                                    var enableSpell = !spell.DefaultOff;
                                    var spellConfig = new SpellConfigControl(SpellMenu, menuName, spell, enableSpell);
                                    spellConfig.AddToMenu();

                                    Config.Properties.SetSpell(spell.SpellName, spell.GetSpellConfig(spellConfig));
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    
                    ConsoleDebug.WriteLine(ex.ToString(), ConsoleColor.Red);
                    throw;
                }
            }
        }
    }
}
