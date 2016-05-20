using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;

namespace UnderratedAIO.Helpers.SkillShot
{
    public static class SkillshotDetector
    {
        public delegate void OnSkillshotDelegate(Skillshot spell);

        internal static bool InitComplete;

        public static SpellList<Skillshot> ActiveSkillshots = new SpellList<Skillshot>();
        internal static event OnDetectSkillshotH OnDetectSkillshot;

        internal static event OnDeleteMissileH OnDeleteMissile;

        public static void Init()
        {
            if (!InitComplete)
            {
                InitComplete = true;
                Collision.Init();
                //Detect when the skillshots are created.
                Obj_AI_Base.OnProcessSpellCast += HeroOnProcessSpellCast;
                //Game.OnProcessPacket += GameOnOnGameProcessPacket;
                //Detect when projectiles collide.
                OnDetectSkillshot += OnDetectSkillshotProcessing;
                GameObject.OnCreate += SpellMissileOnCreate;
                GameObject.OnDelete += SpellMissileOnDelete;

                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;
            }
        }


        /// <summary>
        ///     Returns true if the point is not inside the detected skillshots.
        /// </summary>
        public static IsSafeResult IsSafe(Vector2 point)
        {
            var result = new IsSafeResult {SkillshotList = new List<Skillshot>()};

            foreach (var skillshot in ActiveSkillshots)
            {
                result.SkillshotList.Add(skillshot);
            }

            result.IsSafe = result.SkillshotList.Count == 0;

            return result;
        }

        private static void OnDetectSkillshotProcessing(Skillshot skillshot)
        {
            try
            {
                var hero = (AIHeroClient) skillshot.Caster;
                //Check if the skillshot is already added.
                var alreadyAdded = false;

                // Integration disabled

                foreach (var item in ActiveSkillshots)
                {
                    if (item.SkillshotData.SpellName == skillshot.SkillshotData.SpellName &&
                        item.Caster.NetworkId == skillshot.Caster.NetworkId &&
                        skillshot.Direction.AngleBetween(item.Direction) < 5 &&
                        (skillshot.StartPosition.LSDistance(item.StartPosition) < 100 ||
                         skillshot.SkillshotData.FromObjects.Length == 0))
                    {
                        alreadyAdded = true;
                    }
                }

                //Add the skillshot to the detected skillshot list.
                if (!alreadyAdded)
                {
                    //Multiple skillshots like twisted fate Q.
                    if (skillshot.DetectionType == DetectionType.ProcessSpell)
                    {
                        if (skillshot.SkillshotData.MultipleNumber != -1)
                        {
                            var originalDirection = skillshot.Direction;

                            for (var i = -(skillshot.SkillshotData.MultipleNumber - 1)/2;
                                i <= (skillshot.SkillshotData.MultipleNumber - 1)/2;
                                i++)
                            {
                                var end = skillshot.StartPosition +
                                          skillshot.SkillshotData.Range*
                                          originalDirection.Rotated(skillshot.SkillshotData.MultipleAngle*i);
                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SkillshotData, skillshot.StartTick,
                                    skillshot.StartPosition, end, skillshot.Caster);

                                ActiveSkillshots.Add(skillshotToAdd);
                            }
                            return;
                        }

                        if (skillshot.SkillshotData.SpellName == "UFSlash")
                        {
                            skillshot.SkillshotData.MissileSpeed = 1600 + (int) skillshot.Caster.MoveSpeed;
                        }

                        if (skillshot.SkillshotData.Invert)
                        {
                            var newDirection = -(skillshot.EndPosition - skillshot.StartPosition).Normalized();
                            var end = skillshot.StartPosition +
                                      newDirection*skillshot.StartPosition.LSDistance(skillshot.EndPosition);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SkillshotData, skillshot.StartTick,
                                skillshot.StartPosition, end, skillshot.Caster);
                            ActiveSkillshots.Add(skillshotToAdd);
                            return;
                        }

                        if (skillshot.SkillshotData.Centered)
                        {
                            var start = skillshot.StartPosition - skillshot.Direction*skillshot.SkillshotData.Range;
                            var end = skillshot.StartPosition + skillshot.Direction*skillshot.SkillshotData.Range;
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SkillshotData, skillshot.StartTick, start, end,
                                skillshot.Caster);
                            ActiveSkillshots.Add(skillshotToAdd);
                            return;
                        }

                        if (skillshot.SkillshotData.SpellName == "SyndraE" ||
                            skillshot.SkillshotData.SpellName == "syndrae5")
                        {
                            var angle = 60;
                            var edge1 =
                                (skillshot.EndPosition - skillshot.Caster.ServerPosition.To2D()).Rotated(
                                    -angle/2*(float) Math.PI/180);
                            var edge2 = edge1.Rotated(angle*(float) Math.PI/180);

                            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                            {
                                var v = minion.ServerPosition.To2D() - skillshot.Caster.ServerPosition.To2D();
                                if (minion.Name == "Seed" && edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                    minion.LSDistance(skillshot.Caster) < 800 &&
                                    (minion.Team != ObjectManager.Player.Team))
                                {
                                    var start = minion.ServerPosition.To2D();
                                    var end = skillshot.Caster.ServerPosition.To2D()
                                        .Extend(
                                            minion.ServerPosition.To2D(),
                                            skillshot.Caster.LSDistance(minion) > 200 ? 1300 : 1000);

                                    var skillshotToAdd = new Skillshot(
                                        skillshot.DetectionType, skillshot.SkillshotData, skillshot.StartTick, start,
                                        end, skillshot.Caster);
                                    ActiveSkillshots.Add(skillshotToAdd);
                                }
                            }
                            return;
                        }

                        if (skillshot.SkillshotData.SpellName == "AlZaharCalloftheVoid")
                        {
                            var start = skillshot.EndPosition - skillshot.Direction.Perpendicular()*400;
                            var end = skillshot.EndPosition + skillshot.Direction.Perpendicular()*400;
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SkillshotData, skillshot.StartTick, start, end,
                                skillshot.Caster);
                            ActiveSkillshots.Add(skillshotToAdd);
                            return;
                        }

                        if (skillshot.SkillshotData.SpellName == "ZiggsQ")
                        {
                            var d1 = skillshot.StartPosition.LSDistance(skillshot.EndPosition);
                            var d2 = d1*0.4f;
                            var d3 = d2*0.69f;


                            var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                            var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");

                            var bounce1Pos = skillshot.EndPosition + skillshot.Direction*d2;
                            var bounce2Pos = bounce1Pos + skillshot.Direction*d3;

                            bounce1SpellData.Delay =
                                (int)
                                    (skillshot.SkillshotData.Delay + d1*1000f/skillshot.SkillshotData.MissileSpeed +
                                     500);
                            bounce2SpellData.Delay =
                                (int) (bounce1SpellData.Delay + d2*1000f/bounce1SpellData.MissileSpeed + 500);

                            var bounce1 = new Skillshot(
                                skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.EndPosition,
                                bounce1Pos, skillshot.Caster);
                            var bounce2 = new Skillshot(
                                skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                                skillshot.Caster);

                            ActiveSkillshots.Add(bounce1);
                            ActiveSkillshots.Add(bounce2);
                        }

                        if (skillshot.SkillshotData.SpellName == "ZiggsR")
                        {
                            skillshot.SkillshotData.Delay =
                                (int)
                                    (1500 +
                                     1500*skillshot.EndPosition.LSDistance(skillshot.StartPosition)/
                                     skillshot.SkillshotData.Range);
                        }

                        if (skillshot.SkillshotData.SpellName == "JarvanIVDragonStrike")
                        {
                            var endPos = new Vector2();

                            foreach (var s in ActiveSkillshots)
                            {
                                if (s.Caster.NetworkId == skillshot.Caster.NetworkId &&
                                    s.SkillshotData.Slot == SpellSlot.E)
                                {
                                    endPos = s.EndPosition;
                                }
                            }

                            foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                            {
                                if (m.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Caster.Team &&
                                    skillshot.IsDanger(m.Position.To2D()))
                                {
                                    endPos = m.Position.To2D();
                                }
                            }

                            if (!endPos.IsValid())
                            {
                                return;
                            }

                            skillshot.EndPosition = endPos + 200*(endPos - skillshot.StartPosition).Normalized();
                            skillshot.Direction = (skillshot.EndPosition - skillshot.StartPosition).Normalized();
                        }
                    }

                    if (skillshot.SkillshotData.SpellName == "OriannasQ")
                    {
                        var endCSpellData = SpellDatabase.GetByName("OriannaQend");

                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, endCSpellData, skillshot.StartTick, skillshot.StartPosition,
                            skillshot.EndPosition, skillshot.Caster);

                        ActiveSkillshots.Add(skillshotToAdd);
                    }

                    /*
                    //Dont allow fow detection.
                    if (skillshot.SkillshotData.DisableFowDetection &&
                        skillshot.DetectionType == DetectionType.RecvPacket)
                    {
                        return;
                    }*/
                    ActiveSkillshots.Add(skillshot);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void TriggerOnDetectSkillshot(DetectionType detectionType,
            SkillshotData skillshotData,
            int startT,
            Vector2 start,
            Vector2 end,
            Obj_AI_Base unit)
        {
            var skillshot = new Skillshot(detectionType, skillshotData, startT, start, end, unit);

            if (OnDetectSkillshot != null)
            {
                OnDetectSkillshot(skillshot);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //TODO: Detect lux R and other large skillshots.
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            for (var i = ActiveSkillshots.Count - 1; i >= 0; i--)
            {
                var skillshot = ActiveSkillshots[i];
                if (skillshot.SkillshotData.ToggleParticleName != "" &&
                    sender.Name.Contains(skillshot.SkillshotData.ToggleParticleName))
                {
                    ActiveSkillshots.RemoveAt(i);
                }
            }
        }

        private static void SpellMissileOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
            {
                return; // only valid missile
            }

            var missile = (MissileClient)sender;
            var unit = missile.SpellCaster;

            if (!unit.IsValid<AIHeroClient>())
            {
                return; // only valid hero
            }

            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellData == null)
            {
                return; // only if database contains skillshot
            }

            var missilePosition = missile.Position.To2D();
            var unitPosition = missile.StartPosition.To2D();
            var endPos = missile.EndPosition.To2D();

            //Calculate the real end Point:
            var direction = (endPos - unitPosition).Normalized();
            if (unitPosition.LSDistance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = unitPosition + direction*spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos +
                         Math.Min(spellData.ExtraRange, spellData.Range - endPos.LSDistance(unitPosition))*direction;
            }

            var castTime = System.Environment.TickCount - Game.Ping/2 -
                           (spellData.MissileDelayed ? 0 : spellData.Delay) -
                           (int) (1000*missilePosition.LSDistance(unitPosition)/spellData.MissileSpeed);

            //Trigger the skillshot detection callbacks.
            TriggerOnDetectSkillshot(DetectionType.RecvPacket, spellData, castTime, unitPosition, endPos, unit);
        }

        /// <summary>
        ///     Delete the missiles that collide.
        /// </summary>
        private static void SpellMissileOnDelete(GameObject sender, EventArgs args)
        {
            if (OnDeleteMissile == null)
            {
                return; // no subscriptions
            }

            if (!sender.IsValid<MissileClient>())
            {
                return; // only valid missile
            }

            var missile = (MissileClient) sender;
            var unit = missile.SpellCaster;

            if (!unit.IsValid<AIHeroClient>())
            {
                return; // only valid hero
            }

            var spellName = missile.SData.Name;

            foreach (var skillshot in ActiveSkillshots)
            {
                if (skillshot.SkillshotData.MissileSpellName == spellName &&
                    skillshot.Caster.NetworkId == unit.NetworkId &&
                    (missile.EndPosition.To2D() - missile.StartPosition.To2D()).AngleBetween(skillshot.Direction) < 10 &&
                    skillshot.SkillshotData.CanBeRemoved)
                {
                    OnDeleteMissile(skillshot, missile);
                    break;
                }
            }

            ActiveSkillshots.RemoveAll(
                skillshot =>
                    (skillshot.SkillshotData.MissileSpellName == spellName ||
                     skillshot.SkillshotData.ExtraMissileNames.Contains(spellName)) &&
                    (skillshot.Caster.NetworkId == unit.NetworkId &&
                     ((missile.EndPosition.To2D() - missile.StartPosition.To2D()).AngleBetween(skillshot.Direction) < 10) &&
                     skillshot.SkillshotData.CanBeRemoved || skillshot.SkillshotData.ForceRemove)); // 
        }

        /// <summary>
        ///     Gets triggered when a unit casts a spell and the unit is visible.
        /// </summary>
        private static void HeroOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "dravenrdoublecast")
            {
                ActiveSkillshots.RemoveAll(
                    s => s.Caster.NetworkId == sender.NetworkId && s.SkillshotData.SpellName == "DravenRCast");
            }

            if (!sender.IsValid<AIHeroClient>())
            {
                return; // only valid hero
            }

            var spellData = SpellDatabase.GetByName(args.SData.Name);

            if (spellData == null)
            {
                return; // only if database contains skillshot
            }

            var startPos = new Vector2();

            if (spellData.FromObject != "")
            {
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if (obj.Name.Contains(spellData.FromObject))
                    {
                        startPos = obj.Position.To2D();
                    }
                }
            }
            else
            {
                startPos = sender.ServerPosition.To2D();
            }

            //For now only zed support.
            if (spellData.FromObjects != null && spellData.FromObjects.Length > 0)
            {
                foreach (var obj in ObjectManager.Get<GameObject>().Where(o => o.IsEnemy))
                {
                    if (spellData.FromObjects.Contains(obj.Name))
                    {
                        var start = obj.Position.To2D();
                        var end = start + spellData.Range*(args.End.To2D() - obj.Position.To2D()).Normalized();
                        TriggerOnDetectSkillshot(
                            DetectionType.ProcessSpell, spellData, System.Environment.TickCount - Game.Ping/2, start,
                            end, sender);
                    }
                }
            }

            if (!startPos.IsValid())
            {
                return;
            }

            var endPos = args.End.To2D();

            //Calculate the real end Point:
            var direction = (endPos - startPos).Normalized();
            if (startPos.LSDistance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = startPos + direction*spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos +
                         Math.Min(spellData.ExtraRange, spellData.Range - endPos.LSDistance(startPos))*direction;
            }

            //Trigger the skillshot detection callbacks.
            TriggerOnDetectSkillshot(
                DetectionType.ProcessSpell, spellData, System.Environment.TickCount - Game.Ping/2, startPos, endPos,
                sender);
        }

        /// <summary>
        ///     Detects the spells that have missile and are casted from fow.
        /// </summary>
        public static void GameOnOnGameProcessPacket(GamePacketEventArgs args)
        {
            //Gets received when a projectile is created.
            if (args.PacketData[0] == 0x3B)
            {
                var packet = new GamePacket(args.PacketData) {Position = 1};

                packet.ReadFloat(); //Missile network ID

                var missilePosition = new Vector3(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
                var unitPosition = new Vector3(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());

                packet.Position = packet.Size() - 119;
                var missileSpeed = packet.ReadFloat();

                packet.Position = 65;
                var endPos = new Vector3(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());

                packet.Position = 112;
                var id = packet.ReadByte();

                packet.Position = packet.Size() - 83;

                var unit = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint) packet.ReadInteger());

                if (!unit.IsValid<AIHeroClient>())
                {
                    return; // only valid hero
                }

                var spellData = SpellDatabase.GetBySpeed(unit.ChampionName, (int) missileSpeed, id);

                if (spellData == null)
                {
                    return; // only if database contains skillshot
                }

                if (spellData.SpellName != "Laser")
                {
                    return; // ingore lasers
                }

                var castTime = System.Environment.TickCount - Game.Ping/2 - spellData.Delay -
                               (int)
                                   (1000*missilePosition.SwitchYZ().To2D().LSDistance(unitPosition.SwitchYZ())/
                                    spellData.MissileSpeed);

                //Trigger the skillshot detection callbacks.
                TriggerOnDetectSkillshot(
                    DetectionType.RecvPacket, spellData, castTime, unitPosition.SwitchYZ().To2D(),
                    endPos.SwitchYZ().To2D(), unit);
            }
        }

        internal delegate void OnDeleteMissileH(Skillshot skillshot, MissileClient missile);

        internal delegate void OnDetectSkillshotH(Skillshot skillshot);

        public struct IsSafeResult
        {
            public bool IsSafe;
            public List<Skillshot> SkillshotList;
        }

        #region Debug

        private static void DebugSpellMissileOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
            {
                return;
            }

            var missile = (MissileClient)sender;

            if (missile.SpellCaster.IsValid<AIHeroClient>())
            {
                Console.WriteLine(
                    "{0} Missile Created:{1} Distance:{2} Radius:{3} Speed:{4}", System.Environment.TickCount,
                    missile.SData.Name, missile.StartPosition.LSDistance(missile.EndPosition),
                    missile.SData.CastRadiusSecondary, missile.SData.MissileSpeed);
            }
        }

        private static void DebugSpellMissileOnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
            {
                return;
            }

            var missile = (MissileClient)sender;

            if (missile.SpellCaster.IsValid<AIHeroClient>())
            {
                Console.WriteLine(
                    "{0} Missile Deleted:{1} Distance:{2}", System.Environment.TickCount, missile.SData.Name,
                    missile.EndPosition.LSDistance(missile.Position));
            }
        }

        #endregion
    }
}