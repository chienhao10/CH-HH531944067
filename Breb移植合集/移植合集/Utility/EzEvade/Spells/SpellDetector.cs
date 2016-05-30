using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Reflection.Emit;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EzEvade;
using SharpDX;
using LeagueSharp.Common;

namespace ezEvade
{
    public class SpecialSpellEventArgs : EventArgs
    {
        public bool noProcess { get; set; }
    }

    internal class SpellDetector
    {
        //public delegate void OnCreateSpellHandler(Spell spell);
        //public static event OnCreateSpellHandler OnCreateSpell;

        public delegate void OnProcessDetectedSpellsHandler();

        public static event OnProcessDetectedSpellsHandler OnProcessDetectedSpells;

        public delegate void OnProcessSpecialSpellHandler(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args,
            SpellData spellData, SpecialSpellEventArgs specialSpellArgs);

        public static event OnProcessSpecialSpellHandler OnProcessSpecialSpell;

        //public static event OnDeleteSpellHandler OnDeleteSpell;

        public static Dictionary<int, Spell> spells = new Dictionary<int, Spell>();
        public static Dictionary<int, Spell> drawSpells = new Dictionary<int, Spell>();
        public static Dictionary<int, Spell> detectedSpells = new Dictionary<int, Spell>();

        public static Dictionary<string, ChampionPlugin> championPlugins = new Dictionary<string, ChampionPlugin>();

        public static Dictionary<string, string> channeledSpells = new Dictionary<string, string>();

        public static Dictionary<string, SpellData> onProcessSpells = new Dictionary<string, SpellData>();
        public static Dictionary<string, SpellData> onMissileSpells = new Dictionary<string, SpellData>();

        public static Dictionary<string, SpellData> windupSpells = new Dictionary<string, SpellData>();

        private static int spellIDCount = 0;

        private static AIHeroClient myHero
        {
            get { return ObjectManager.Player; }
        }

        public static float lastCheckTime = 0;
        public static float lastCheckSpellCollisionTime = 0;

        public static Menu spellMenu;

        public SpellDetector(Menu mainMenu)
        {
            GameObject.OnCreate += SpellMissile_OnCreate;
            GameObject.OnDelete += SpellMissile_OnDelete;

            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;

            //Obj_AI_Hero.OnEnterVisiblityClient += Game_OnEnterVisiblity;

            Game.OnUpdate += Game_OnGameUpdate;

            spellMenu = mainMenu;
            spellMenu = mainMenu.AddSubMenuEx("Spells", "Spells");

            LoadSpellDictionary();
            InitChannelSpells();
        }

        private void Game_OnEnterVisiblity(AttackableUnit sender, EventArgs args)
        {
            Console.WriteLine(sender.Name);
        }

        private void SpellMissile_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.GetType() != typeof (MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            MissileClient missile = (MissileClient) obj;

            // todo: keepo
            //if (missile.SpellCaster.IsMe)
            //    Console.WriteLine("Missile: " + missile.SData.Name);

            SpellData spellData;

            if (missile.SpellCaster != null && missile.SpellCaster.Team != myHero.Team &&
                missile.SData.Name != null && onMissileSpells.TryGetValue(missile.SData.Name, out spellData)
                && missile.StartPosition != null && missile.EndPosition != null)
            {

                if (missile.StartPosition.LSDistance(myHero.Position) < spellData.range + 1000)
                {
                    var hero = missile.SpellCaster;

                    if (hero.IsVisible)
                    {
                        if (spellData.usePackets)
                        {
                            CreateSpellData(hero, missile.StartPosition, missile.EndPosition, spellData, obj);
                            return;
                        }

                        var objectAssigned = false;

                        foreach (KeyValuePair<int, Spell> entry in detectedSpells)
                        {
                            Spell spell = entry.Value;
                            var dir = (missile.EndPosition.LSTo2D() - missile.StartPosition.LSTo2D()).LSNormalized();

                            if (spell.info.missileName.Equals(missile.SData.Name, StringComparison.InvariantCultureIgnoreCase) ||
                               (spell.info.missileName + "_urf").Equals(missile.SData.Name, StringComparison.InvariantCultureIgnoreCase)
                                && spell.heroID == missile.SpellCaster.NetworkId
                                && dir.LSAngleBetween(spell.direction) < 10)
                            {

                                if (spell.info.isThreeWay == false
                                    && spell.info.isSpecial == false)
                                {
                                    spell.spellObject = obj;
                                    objectAssigned = true;
                                    break;

                                    /*if(spell.spellType == SpellType.Line)
                                    {
                                        if (missile.SData.LineWidth != spell.info.radius)
                                        {
                                            Console.WriteLine("Wrong radius " + spell.info.spellName + ": "
                                            + spell.info.radius + " vs " + missile.SData.LineWidth);
                                        }

                                        if (missile.SData.MissileSpeed != spell.info.projectileSpeed)
                                        {
                                            Console.WriteLine("Wrong speed " + spell.info.spellName + ": "
                                            + spell.info.projectileSpeed + " vs " + missile.SData.MissileSpeed);
                                        }
                                        
                                    }*/

                                    //var acquisitionTime = EvadeUtils.TickCount - spell.startTime;
                                    //Console.WriteLine("AcquiredTime: " + acquisitionTime);
                                }
                            }
                        }

                        if (objectAssigned == false)
                        {
                            CreateSpellData(hero, missile.StartPosition, missile.EndPosition, spellData, obj);
                        }
                    }
                    else
                    {
                        if (ObjectCache.menuCache.cache["DodgeFOWSpells"].Cast<CheckBox>().CurrentValue)
                        {
                            CreateSpellData(hero, missile.StartPosition, missile.EndPosition, spellData, obj);
                        }
                    }
                }
            }
        }

        private void SpellMissile_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.GetType() != typeof (MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            var missile = (MissileClient) obj;
            //SpellData spellData;

            foreach (var spell in spells.Values.ToList().Where(
                s => (s.spellObject != null && s.spellObject.NetworkId == obj.NetworkId))) //isAlive
            {
                //Console.WriteLine("Distance: " + obj.Position.LSDistance(myHero.Position));

                DelayAction.Add(1, () => DeleteSpell(spell.spellID));
            }
        }

        public void SpellMissile_OnCreateOld(GameObject obj, EventArgs args)
        {

            if (obj.GetType() != typeof (MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            var missile = (MissileClient) obj;

            SpellData spellData;

            if (missile.SpellCaster != null && missile.SpellCaster.Team != myHero.Team &&
                missile.SData.Name != null && onMissileSpells.TryGetValue(missile.SData.Name, out spellData)
                && missile.StartPosition != null && missile.EndPosition != null)
            {

                if (missile.StartPosition.LSDistance(myHero.Position) < spellData.range + 1000)
                {
                    var hero = missile.SpellCaster;

                    if (hero.IsVisible)
                    {
                        if (spellData.usePackets)
                        {
                            CreateSpellData(hero, missile.StartPosition, missile.EndPosition, spellData, obj);
                            return;
                        }

                        foreach (KeyValuePair<int, Spell> entry in spells)
                        {
                            Spell spell = entry.Value;

                            var dir = (missile.EndPosition.LSTo2D() - missile.StartPosition.LSTo2D()).LSNormalized();

                            if (spell.info.missileName == missile.SData.Name
                                && spell.heroID == missile.SpellCaster.NetworkId
                                && dir.AngleBetween(spell.direction) < 10)
                            {
                                if (spell.info.isThreeWay == false && spell.info.isSpecial == false)
                                {
                                    spell.spellObject = obj;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ObjectCache.menuCache.cache["DodgeFOWSpells"].Cast<CheckBox>().CurrentValue)
                        {
                            CreateSpellData(hero, missile.StartPosition, missile.EndPosition, spellData, obj);
                        }
                    }
                }
            }
        }

        public void SpellMissile_OnDeleteOld(GameObject obj, EventArgs args)
        {
            if (obj.GetType() != typeof (MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            var missile = (MissileClient) obj;
            //SpellData spellData;

            foreach (var spell in spells.Values.ToList().Where(
                s => (s.spellObject != null && s.spellObject.NetworkId == obj.NetworkId))) //isAlive
            {
                DelayAction.Add(1, () => DeleteSpell(spell.spellID));
            }
        }

        public void RemoveNonDangerousSpells()
        {
            foreach (var spell in spells.Values.ToList().Where(
                s => (s.GetSpellDangerLevel() < 3)))
            {
                DelayAction.Add(1, () => DeleteSpell(spell.spellID));
            }
        }

        private void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                /*var castTime2 = (hero.Spellbook.CastTime - Game.Time) * 1000;
                if (castTime2 > 0)
                {
                    Console.WriteLine(args.SData.Name + ": " + castTime2);
                }*/

                // todo: keepo
                //if (hero.IsMe)
                //    Console.WriteLine("Spell: " + args.SData.Name);

                SpellData spellData;

                if (hero.Team != myHero.Team && onProcessSpells.TryGetValue(args.SData.Name, out spellData))
                {
                    if (spellData.usePackets == false)
                    {
                        var specialSpellArgs = new SpecialSpellEventArgs();
                        if (OnProcessSpecialSpell != null)
                        {
                            OnProcessSpecialSpell(hero, args, spellData, specialSpellArgs);
                        }

                        if (specialSpellArgs.noProcess == false && spellData.noProcess == false)
                        {
                            bool foundMissile = false;

                            if (spellData.isThreeWay == false && spellData.isSpecial == false)
                            {
                                foreach (KeyValuePair<int, Spell> entry in detectedSpells)
                                {
                                    Spell spell = entry.Value;

                                    var dir = (args.End.LSTo2D() - args.Start.LSTo2D()).LSNormalized();

                                    if (spell.spellObject != null
                                        && (spell.info.spellName.Equals(args.SData.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                           (spell.info.spellName.ToLower() + "_urf").Equals(args.SData.Name, StringComparison.InvariantCultureIgnoreCase))
                                        && spell.heroID == hero.NetworkId
                                        && dir.AngleBetween(spell.direction) < 10)
                                    {
                                        foundMissile = true;
                                        break;
                                    }
                                }
                            }

                            if (foundMissile == false)
                            {
                                CreateSpellData(hero, hero.ServerPosition, args.End, spellData, null);
                            }

                            /*if (spellData.spellType == SpellType.Line)
                            {
                                var castTime = (hero.Spellbook.CastTime - Game.Time) * 1000;

                                if (Math.Abs(castTime - spellData.spellDelay) > 5)
                                {
                                    Console.WriteLine("Wrong delay " + spellData.spellName + ": "
                                        + spellData.spellDelay + " vs " + castTime);
                                }
                            }*/

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void CreateSpellData(Obj_AI_Base hero, Vector3 spellStartPos, Vector3 spellEndPos,
            SpellData spellData, GameObject obj = null, float extraEndTick = 0.0f, bool processSpell = true,
            SpellType spellType = SpellType.None, bool checkEndExplosion = true, float spellRadius = 0)
        {
            if (checkEndExplosion && spellData.hasEndExplosion)
            {
                CreateSpellData(hero, spellStartPos, spellEndPos,
                    spellData, obj, extraEndTick, false,
                    spellData.spellType, false);

                CreateSpellData(hero, spellStartPos, spellEndPos,
                    spellData, obj, extraEndTick, true,
                    SpellType.Circular, false);

                return;
            }

            if (spellStartPos.LSDistance(myHero.Position) < spellData.range + 1000)
            {
                Vector2 startPosition = spellStartPos.LSTo2D();
                Vector2 endPosition = spellEndPos.LSTo2D();
                Vector2 direction = (endPosition - startPosition).LSNormalized();
                float endTick = 0;

                if (spellType == SpellType.None)
                {
                    spellType = spellData.spellType;
                }

                if (spellData.fixedRange) //for diana q
                {
                    if (endPosition.LSDistance(startPosition) > spellData.range)
                    {
                        //var heroCastPos = hero.ServerPosition.LSTo2D();
                        //direction = (endPosition - heroCastPos).LSNormalized();
                        endPosition = startPosition + direction*spellData.range;
                    }
                }

                if (spellType == SpellType.Line)
                {
                    endTick = spellData.spellDelay + (spellData.range/spellData.projectileSpeed)*1000;
                    endPosition = startPosition + direction*spellData.range;

                    if (spellData.useEndPosition)
                    {
                        var range = spellEndPos.LSTo2D().LSDistance(spellStartPos.LSTo2D());
                        endTick = spellData.spellDelay + (range/spellData.projectileSpeed)*1000;
                        endPosition = spellEndPos.LSTo2D();
                    }

                    if (obj != null)
                        endTick -= spellData.spellDelay;
                }
                else if (spellType == SpellType.Circular)
                {
                    endTick = spellData.spellDelay;

                    if (spellData.projectileSpeed == 0)
                    {
                        endPosition = hero.ServerPosition.LSTo2D();
                    }
                    else if (spellData.projectileSpeed > 0)
                    {
                        if (spellData.spellType == SpellType.Line &&
                            spellData.hasEndExplosion &&
                            spellData.useEndPosition == false)
                        {
                            endPosition = startPosition + direction*spellData.range;
                        }

                        endTick = endTick + 1000*startPosition.LSDistance(endPosition)/spellData.projectileSpeed;
                    }
                }
                else if (spellType == SpellType.Arc)
                {
                    endTick = endTick + 1000*startPosition.LSDistance(endPosition)/spellData.projectileSpeed;

                    if (obj != null)
                        endTick -= spellData.spellDelay;
                }
                else if (spellType == SpellType.Cone)
                {
                    return;
                }
                else
                {
                    return;
                }

                if (spellData.invert)
                {
                    var dir = (startPosition - endPosition).LSNormalized();
                    endPosition = startPosition + dir * startPosition.LSDistance(endPosition);
                }

                endTick += extraEndTick;

                Spell newSpell = new Spell
                {
                    startTime = EvadeUtils.TickCount,
                    endTime = EvadeUtils.TickCount + endTick,
                    startPos = startPosition,
                    endPos = endPosition,
                    height = spellEndPos.Z + spellData.extraDrawHeight,
                    direction = direction,
                    heroID = hero.NetworkId,
                    info = spellData,
                    spellType = spellType
                };

                newSpell.radius = spellRadius > 0 ? spellRadius : newSpell.GetSpellRadius();

                if (obj != null)
                {
                    newSpell.spellObject = obj;
                    newSpell.projectileID = obj.NetworkId;
                }

                int spellID = CreateSpell(newSpell, processSpell);

                DelayAction.Add((int) (endTick + spellData.extraEndTime), () => DeleteSpell(spellID));
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            UpdateSpells();

            if (EvadeUtils.TickCount - lastCheckSpellCollisionTime > 100)
            {
                CheckSpellCollision();
                lastCheckSpellCollisionTime = EvadeUtils.TickCount;
            }

            if (EvadeUtils.TickCount - lastCheckTime > 1)
            {
                //CheckCasterDead();                
                CheckSpellEndTime();
                AddDetectedSpells();
                lastCheckTime = EvadeUtils.TickCount;
            }
        }

        public static void UpdateSpells()
        {
            foreach (var spell in detectedSpells.Values)
            {
                spell.UpdateSpellInfo();
            }
        }

        private void CheckSpellEndTime()
        {
            foreach (KeyValuePair<int, Spell> entry in detectedSpells)
            {
                Spell spell = entry.Value;

                foreach (var hero in EntityManager.Heroes.Enemies)
                {
                    if (hero.IsDead && spell.heroID == hero.NetworkId)
                    {
                        if (spell.spellObject == null)
                            DelayAction.Add(1, () => DeleteSpell(entry.Key));
                    }
                }

                if (spell.endTime + spell.info.extraEndTime < EvadeUtils.TickCount
                    || CanHeroWalkIntoSpell(spell) == false)
                {
                    DelayAction.Add(1, () => DeleteSpell(entry.Key));
                }
            }
        }

        private static void CheckSpellCollision()
        {
            if (ObjectCache.menuCache.cache["CheckSpellCollision"].Cast<CheckBox>().CurrentValue == false)
            {
                return;
            }

            foreach (KeyValuePair<int, Spell> entry in detectedSpells)
            {
                Spell spell = entry.Value;

                var collisionObject = spell.CheckSpellCollision();

                if (collisionObject != null)
                {
                    spell.predictedEndPos = spell.GetSpellProjection(collisionObject.ServerPosition.LSTo2D());

                    if (spell.currentSpellPosition.LSDistance(collisionObject.ServerPosition)
                        < collisionObject.BoundingRadius + spell.radius)
                    {
                        DelayAction.Add(1, () => DeleteSpell(entry.Key));
                    }
                }
            }
        }

        public static bool CanHeroWalkIntoSpell(Spell spell)
        {
            if (ObjectCache.menuCache.cache["AdvancedSpellDetection"].Cast<CheckBox>().CurrentValue)
            {
                Vector2 heroPos = myHero.Position.LSTo2D();
                var extraDist = myHero.LSDistance(ObjectCache.myHeroCache.serverPos2D);

                if (spell.spellType == SpellType.Line)
                {
                    var walkRadius = ObjectCache.myHeroCache.moveSpeed*(spell.endTime - EvadeUtils.TickCount)/1000 +
                                     ObjectCache.myHeroCache.boundingRadius + spell.info.radius + extraDist + 10;
                    var spellPos = spell.currentSpellPosition;
                    var spellEndPos = spell.GetSpellEndPosition();

                    var projection = heroPos.ProjectOn(spellPos, spellEndPos);

                    return projection.SegmentPoint.LSDistance(heroPos) <= walkRadius;
                }
                else if (spell.spellType == SpellType.Circular)
                {
                    var walkRadius = ObjectCache.myHeroCache.moveSpeed*(spell.endTime - EvadeUtils.TickCount)/1000 +
                                     ObjectCache.myHeroCache.boundingRadius + spell.info.radius + extraDist + 10;

                    if (heroPos.LSDistance(spell.endPos) < walkRadius)
                    {
                        return true;
                    }

                }
                else if (spell.spellType == SpellType.Arc)
                {
                    var spellRange = spell.startPos.LSDistance(spell.endPos);
                    var midPoint = spell.startPos + spell.direction*(spellRange/2);
                    var arcRadius = spell.info.radius*(1 + spellRange/100);

                    var walkRadius = ObjectCache.myHeroCache.moveSpeed*(spell.endTime - EvadeUtils.TickCount)/1000 +
                                     ObjectCache.myHeroCache.boundingRadius + arcRadius + extraDist + 10;

                    if (heroPos.LSDistance(midPoint) < walkRadius)
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

            foreach (KeyValuePair<int, Spell> entry in detectedSpells)
            {
                Spell spell = entry.Value;
                EvadeHelper.fastEvadeMode =ObjectCache.menuCache.cache[spell.info.spellName + "FastEvade"].Cast<CheckBox>().CurrentValue;

                float evadeTime, spellHitTime;
                spell.CanHeroEvade(myHero, out evadeTime, out spellHitTime);

                spell.spellHitTime = spellHitTime;
                spell.evadeTime = evadeTime;

                var extraDelay = ObjectCache.gamePing +
                                 ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;

                if (spell.spellHitTime - extraDelay < 1500 && CanHeroWalkIntoSpell(spell))
                    //if(true)
                {
                    Spell newSpell = spell;
                    int spellID = spell.spellID;

                    if (!drawSpells.ContainsKey(spell.spellID))
                    {
                        drawSpells.Add(spellID, newSpell);
                    }

                    //var spellFlyTime = Evade.GetTickCount - spell.startTime;
                    if (spellHitTime < ObjectCache.menuCache.cache["SpellDetectionTime"].Cast<Slider>().CurrentValue
                        &&
                        !ObjectCache.menuCache.cache[spell.info.spellName + "FastEvade"].Cast<CheckBox>().CurrentValue)
                    {
                        continue;
                    }

                    if (EvadeUtils.TickCount - spell.startTime <
                        ObjectCache.menuCache.cache["ReactionTime"].Cast<Slider>().CurrentValue
                        &&
                        !ObjectCache.menuCache.cache[spell.info.spellName + "FastEvade"].Cast<CheckBox>().CurrentValue)
                    {
                        continue;
                    }

                    var dodgeInterval = ObjectCache.menuCache.cache["DodgeInterval"].Cast<Slider>().CurrentValue;
                    if (Evade.lastPosInfo != null && dodgeInterval > 0)
                    {
                        var timeElapsed = EvadeUtils.TickCount - Evade.lastPosInfo.timestamp;

                        if (dodgeInterval > timeElapsed &&
                            !ObjectCache.menuCache.cache[spell.info.spellName + "FastEvade"].Cast<CheckBox>()
                                .CurrentValue)
                        {
                            //var delay = dodgeInterval - timeElapsed;
                            //DelayAction.Add((int)delay, () => SpellDetector_OnProcessDetectedSpells());
                            continue;
                        }
                    }

                    if (!spells.ContainsKey(spell.spellID))
                    {
                        if (!(Evade.isDodgeDangerousEnabled() && newSpell.GetSpellDangerLevel() < 3)
                            &&
                            ObjectCache.menuCache.cache[newSpell.info.spellName + "DodgeSpell"].Cast<CheckBox>()
                                .CurrentValue)
                        {
                            if (newSpell.spellType == SpellType.Circular
                                &&
                                ObjectCache.menuCache.cache["DodgeCircularSpells"].Cast<CheckBox>().CurrentValue ==
                                false)
                            {
                                //return spellID;
                                continue;
                            }

                            if (myHero.HealthPercent <=
                                ObjectCache.menuCache.cache[spell.info.spellName + "DodgeIgnoreHP"].Cast<Slider>()
                                    .CurrentValue ||
                                !ObjectCache.menuCache.cache["ChaseModeMinHP"].Cast<CheckBox>().CurrentValue)

                            {
                                spells.Add(spellID, newSpell);
                                spellAdded = true;
                            }
                        }
                    }

                    if (ObjectCache.menuCache.cache["CheckSpellCollision"].Cast<CheckBox>().CurrentValue
                        && spell.predictedEndPos != Vector2.Zero)
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

        private static int CreateSpell(Spell newSpell, bool processSpell = true)
        {
            int spellID = spellIDCount++;
            newSpell.spellID = spellID;

            newSpell.UpdateSpellInfo();
            detectedSpells.Add(spellID, newSpell);

            if (processSpell)
            {
                CheckSpellCollision();
                AddDetectedSpells();
            }

            return spellID;
        }

        public static void DeleteSpell(int spellID)
        {
            spells.Remove(spellID);
            drawSpells.Remove(spellID);
            detectedSpells.Remove(spellID);
        }

        public static int GetCurrentSpellID()
        {
            return spellIDCount;
        }

        public static List<int> GetSpellList()
        {
            return SpellDetector.spells.Select(entry => entry.Value).Select(spell => spell.spellID).ToList();
        }

        public static float GetLowestEvadeTime(out Spell lowestSpell)
        {
            float lowest = float.MaxValue;
            lowestSpell = null;

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.spells)
            {
                Spell spell = entry.Value;

                if (spell.spellHitTime != float.MinValue)
                {
                    //Console.WriteLine("spellhittime: " + spell.spellHitTime);
                    lowest = Math.Min(lowest, (spell.spellHitTime - spell.evadeTime));
                    lowestSpell = spell;
                }
            }

            return lowest;
        }

        public static Spell GetMostDangerousSpell(bool hasProjectile = false)
        {
            int maxDanger = 0;
            Spell maxDangerSpell = null;

            foreach (Spell spell in SpellDetector.spells.Values)
            {
                if (!hasProjectile || (spell.info.projectileSpeed > 0 && spell.info.projectileSpeed != float.MaxValue))
                {
                    var dangerlevel = spell.dangerlevel;

                    if (dangerlevel > maxDanger)
                    {
                        maxDanger = dangerlevel;
                        maxDangerSpell = spell;
                    }
                }
            }

            return maxDangerSpell;
        }

        public static void InitChannelSpells()
        {
            channeledSpells["Drain"] = "FiddleSticks";
            channeledSpells["Crowstorm"] = "FiddleSticks";
            channeledSpells["KatarinaR"] = "Katarina";
            channeledSpells["AbsoluteZero"] = "Nunu";
            channeledSpells["GalioIdolOfDurand"] = "Galio";
            channeledSpells["MissFortuneBulletTime"] = "MissFortune";
            channeledSpells["Meditate"] = "MasterYi";
            channeledSpells["NetherGrasp"] = "Malzahar";
            channeledSpells["ReapTheWhirlwind"] = "Janna";
            channeledSpells["KarthusFallenOne"] = "Karthus";
            channeledSpells["KarthusFallenOne2"] = "Karthus";
            channeledSpells["VelkozR"] = "Velkoz";
            channeledSpells["XerathLocusOfPower2"] = "Xerath";
            channeledSpells["ZacE"] = "Zac";
            channeledSpells["Pantheon_Heartseeker"] = "Pantheon";
            channeledSpells["JhinR"] = "Jhin";
            channeledSpells["OdinRecall"] = "AllChampions";
            channeledSpells["Recall"] = "AllChampions";
        }

        public static void LoadDummySpell(SpellData spell)
        {
            string menuName = spell.charName + " (" + spell.spellKey.ToString() + ") Settings";

            var enableSpell = !spell.defaultOff;

            spellMenu.AddGroupLabel(menuName);
            //    Menu newSpellMenu = spellMenu.IsSubMenu ? spellMenu.Parent.AddSubMenuEx(menuName, spell.charName + spell.spellName + "Settings") : spellMenu.AddSubMenuEx(menuName, spell.charName + spell.spellName + "Settings");
            spellMenu.Add(spell.spellName + "DodgeSpell", new CheckBox("Dodge Spell", enableSpell));
            spellMenu.Add(spell.spellName + "DrawSpell", new CheckBox("Draw Spell", enableSpell));
            spellMenu.Add(spell.spellName + "SpellRadius",new Slider("Spell Radius", (int) spell.radius, (int) spell.radius - 100, (int) spell.radius + 100));
            spellMenu.Add(spell.spellName + "FastEvade", new CheckBox("Force Fast Evade", spell.dangerlevel == 4));
            spellMenu.Add(spell.spellName + "DodgeIgnoreHP",new Slider("Ignore above HP %", spell.dangerlevel == 1 ? 80 : 100));
            var slider = spellMenu.Add(spell.spellName + "DangerLevel",
                new Slider("Danger Level", spell.dangerlevel - 1, 0, 3));
            var array = new[] {"Low", "Normal", "High", "Extreme"};
            slider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = array[args.NewValue];
            };
            slider.DisplayName = array[slider.CurrentValue];

            ObjectCache.menuCache.AddMenuToCache(spellMenu);
        }

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
            if (championPlugins.ContainsKey(spell.charName))
            {
                championPlugins[spell.charName].LoadSpecialSpell(spell);
            }

            championPlugins["AllChampions"].LoadSpecialSpell(spell);
        }

        private void LoadSpecialSpellPlugins()
        {
            championPlugins.Add("AllChampions", new SpecialSpells.AllChampions());

            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                var championPlugin = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && t.Namespace == "ezEvade.SpecialSpells"
                                && t.Name == hero.ChampionName
                    ).ToList().FirstOrDefault();

                if (championPlugin != null)
                {
                    if (!championPlugins.ContainsKey(hero.ChampionName))
                    {
                        championPlugins.Add(hero.ChampionName,
                            (ChampionPlugin) NewInstance(championPlugin));
                    }
                }
            }
        }

        private void LoadSpellDictionary()
        {
            LoadSpecialSpellPlugins();

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.IsMe)
                {
                    foreach (var spell in SpellWindupDatabase.Spells.Where(
                        s => (s.charName == hero.ChampionName)))
                    {
                        if (!windupSpells.ContainsKey(spell.spellName))
                        {
                            windupSpells.Add(spell.spellName, spell);
                        }
                    }
                }

                if (hero.Team != myHero.Team)
                {
                    foreach (var spell in SpellDatabase.Spells.Where(
                        s => (s.charName == hero.ChampionName) || (s.charName == "AllChampions")))
                    {
                        //Console.WriteLine(spell.spellName); 

                        if (!(spell.spellType == SpellType.Circular
                              || spell.spellType == SpellType.Line
                              || spell.spellType == SpellType.Arc))
                            continue;

                        if (spell.charName == "AllChampions")
                        {
                            SpellSlot slot = hero.LSGetSpellSlot(spell.spellName);
                            if (slot == SpellSlot.Unknown)
                            {
                                continue;
                            }
                        }

                        if (!onProcessSpells.ContainsKey(spell.spellName))
                        {
                            if (spell.missileName == "")
                                spell.missileName = spell.spellName;

                            onProcessSpells.Add(spell.spellName, spell);
                            onMissileSpells.Add(spell.missileName, spell);

                            if (spell.extraSpellNames != null)
                            {
                                foreach (string spellName in spell.extraSpellNames)
                                {
                                    onProcessSpells.Add(spellName, spell);
                                }
                            }

                            if (spell.extraMissileNames != null)
                            {
                                foreach (string spellName in spell.extraMissileNames)
                                {
                                    onMissileSpells.Add(spellName, spell);
                                }
                            }

                            LoadSpecialSpell(spell);

                            string menuName = spell.charName + " (" + spell.spellKey.ToString() + ") Settings";

                            var enableSpell = !spell.defaultOff;

                            spellMenu.AddGroupLabel(menuName);
                            //        Menu newSpellMenu = spellMenu.IsSubMenu ? spellMenu.Parent.AddSubMenuEx(menuName, spell.charName + spell.spellName + "Settings") : spellMenu.AddSubMenuEx(menuName, spell.charName + spell.spellName + "Settings");
                            spellMenu.Add(spell.spellName + "DodgeSpell", new CheckBox("Dodge Spell", enableSpell));
                            spellMenu.Add(spell.spellName + "DrawSpell", new CheckBox("Draw Spell", enableSpell));
                            spellMenu.Add(spell.spellName + "SpellRadius",new Slider("Spell Radius", (int) spell.radius, (int) spell.radius - 100,(int) spell.radius + 100));
                            spellMenu.Add(spell.spellName + "FastEvade", new CheckBox("Force Fast Evade", spell.dangerlevel == 4));
                            spellMenu.Add(spell.spellName + "DodgeIgnoreHP", new Slider("Ignore above HP %", spell.dangerlevel == 1 ? 80 : 100));
                            var slider = spellMenu.Add(spell.spellName + "DangerLevel",
                                new Slider("Danger Level", spell.dangerlevel - 1, 0, 3));
                            var array = new[] {"Low", "Normal", "High", "Extreme"};
                            slider.OnValueChange +=
                                delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                                {
                                    sender.DisplayName = array[args.NewValue];
                                };
                            slider.DisplayName = array[slider.CurrentValue];
                        }
                    }
                }
            }
        }
    }
}