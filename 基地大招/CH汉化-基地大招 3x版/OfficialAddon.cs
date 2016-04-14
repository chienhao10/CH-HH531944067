using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using Rectangle = SharpDX.Rectangle;
using Sprite = EloBuddy.SDK.Rendering.Sprite;

namespace BaseUltPlusPlus
{
    public class OfficialAddon
    {
        private static String[] compatibleChamps = new[] { "Jinx", "Ezreal", "Ashe", "Draven", "Karthus" }; //Ziggs, Xerath, Lux
        private static readonly List<Recall> Recalls = new List<Recall>();
        private static readonly List<BaseUltUnit> BaseUltUnits = new List<BaseUltUnit>();
        private static readonly List<BaseUltSpell> BaseUltSpells = new List<BaseUltSpell>();
        private static readonly List<EnemyInfo> enemiesInfo = new List<EnemyInfo>();

        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static SpellDataInst Ultimate;
        
        
        private static Font Text;

        private static float BarX = Drawing.Width * 0.415f;
        private static float BarY = Drawing.Height * 0.80f;
        private static int BarW = (int)(Drawing.Width - 2 * BarX);
        private static float Scale = (float)BarW / 8000;
        private static int BarH = 6;

        public static void Initialize()
        {
            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 13, Width = 6, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });

            #region Spells

            BaseUltSpells.Add(new BaseUltSpell("Ezreal", SpellSlot.R, 1000 / 1000f, 2000, 160, false));
            BaseUltSpells.Add(new BaseUltSpell("Jinx", SpellSlot.R, 600 / 1000f, 1700, 140, true));
            BaseUltSpells.Add(new BaseUltSpell("Ashe", SpellSlot.R, 250 / 1000f, 1600, 130, true));
            BaseUltSpells.Add(new BaseUltSpell("Draven", SpellSlot.R, 400 / 1000f, 2000, 160, true));
            BaseUltSpells.Add(new BaseUltSpell("Karthus", SpellSlot.R, 3125 / 1000f, 0, 0, false));

            #endregion

            Ultimate = Player.Spellbook.GetSpell(BaseUltSpells.Find(h => h.Name == Player.ChampionName).Slot);

            foreach (var enemy in EntityManager.Heroes.Enemies)
                enemiesInfo.Add(new EnemyInfo(enemy));
        }
        //compatibleChamps.Any(h => h == Player.ChampionName)
        public static void Game_OnUpdate()
        {
            foreach (EnemyInfo enemy in enemiesInfo.Where(x => x.Player.IsHPBarRendered))
                enemy.LastSeen = Core.GameTickCount;

            foreach (var recall in Recalls)
            {
                //Compute
                if (BaseUltUnits.All(h => h.Unit.NetworkId != recall.Unit.NetworkId))
                {
                    var spell = BaseUltSpells.Find(h => h.Name == Player.ChampionName);
                    if (Ultimate.IsReady && Ultimate.Level > 0)
                        BaseUltCalcs(recall);
                }
            }

            foreach (var unit in BaseUltUnits)
            {
                var timeLimit = Program.BaseUltMenu["timeLimit"].Cast<Slider>().CurrentValue;

                if (Math.Round(unit.FireTime, 1) <= Core.GameTickCount && ((Core.GameTickCount - timeLimit) >= enemiesInfo.FirstOrDefault(x => x.Player.Equals(unit.Unit)).LastSeen))
                {
                    if (Ultimate.IsReady && !Program.BaseUltMenu["nobaseult"].Cast<KeyBind>().CurrentValue && !unit.processed)
                    {
                        Player.Spellbook.CastSpell(Ultimate.Slot, GetFountainPos());
                        unit.processed = true;
                    }
                }
            }
        }


        private static void BaseUltCalcs(Recall recall)
        {
            float recallEndTime = recall.Started + recall.Duration;
            float timeNeeded = GetBaseUltTravelTime(Player);
            float recallCountDown = recallEndTime - Core.GameTickCount;
            float delay = recallEndTime - timeNeeded;

            bool collision = Program.BaseUltMenu["checkcollision"].Cast<CheckBox>().CurrentValue ? GetCollision(Player.ChampionName).Any() : false;
            var spellData = BaseUltSpells.Find(h => h.Name == Player.ChampionName);
            var Target = enemiesInfo.FirstOrDefault(x => x.Player.Equals(recall.Unit));

            if (Target == null)
                return;

            if (recallCountDown >= timeNeeded && !collision && IsTargetKillable(Target, recallCountDown)
                && Program.BaseUltMenu["target" + recall.Unit.ChampionName].Cast<CheckBox>().CurrentValue
                && Program.BaseUltMenu["baseult"].Cast<CheckBox>().CurrentValue)
            {
                BaseUltUnits.Add(new BaseUltUnit(recall.Unit, delay));
            }
            else if (BaseUltUnits.Any(h => h.Unit.NetworkId == recall.Unit.NetworkId))
            {
                BaseUltUnits.Remove(BaseUltUnits.Find(h => h.Unit.NetworkId == recall.Unit.NetworkId));
            }

        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (!Program.BaseUltMenu["showrecalls"].Cast<CheckBox>().CurrentValue && !Recalls.Any())
                return;


            foreach (var recall in Recalls.OrderBy(x => x.Unit.Health))
            {
                double RecallProgress = recall.Started + recall.Duration - Core.GameTickCount;
                bool isBaseUlt = BaseUltUnits.Any(h => h.Unit.NetworkId == recall.Unit.NetworkId);

                Color colorIndicator = isBaseUlt ? Color.Red : Color.White;

                DrawRect(BarX, BarY, (int)(Scale * RecallProgress), BarH, 1, System.Drawing.Color.FromArgb(100, colorIndicator));
                DrawRect(BarX + (int)(Scale * RecallProgress), BarY + 1, 2, BarH - 1, 1, System.Drawing.Color.FromArgb(255, Color.White));

                if (isBaseUlt)
                {
                    var unit = BaseUltUnits.FirstOrDefault(h => h.Unit.NetworkId == recall.Unit.NetworkId);

                    if (unit == null)
                        continue;

                    DrawRect(BarX + (int)(Scale * unit.FireTime), BarY + 4, 4, BarH - 1, 1, System.Drawing.Color.FromArgb(255, Color.Black));
                }

                Text.DrawText(null, recall.Unit.BaseSkinName, (int)BarX + (int)(Scale * RecallProgress - (float)(recall.Unit.BaseSkinName.Length * Text.Description.Width) / 2), (int)BarY - 5 - Text.Description.Height - 1, new ColorBGRA(255, 255, 255, 255));
            }

            if (Recalls.Any())
            {
                DrawRect(BarX, BarY, BarW, BarH, 1, System.Drawing.Color.FromArgb(40, System.Drawing.Color.White));

                DrawRect(BarX - 1, BarY + 1, 1, BarH, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY - 1, BarW + 2, 1, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY + BarH, BarW + 2, 1, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
                DrawRect(BarX + 1 + BarW, BarY + 1, 1, BarH, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
            }
        }

        public static void DrawRect(float x, float y, int width, int height, float thickness, System.Drawing.Color color)
        {
            for (int i = 0; i < height; i++)
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
        }

        private static Vector3 GetFountainPos()
        {
            //switch (Game.MapId)
            //{
            //    case GameMapId.SummonersRift:
            //        {
            //            return Player.Instance.Team == GameObjectTeam.Order
            //                ? new Vector3(14296, 14362, 171)
            //                : new Vector3(408, 414, 182);
            //        }
            //}

            //return new Vector3();

            return ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;
        }

        private static bool IsTargetKillable(EnemyInfo target, float countDown)
        {
            float totalUltDamage = (float)GetBaseUltSpellDamage(target.Player, Player);
            float targetHealth = GetTargetHealth(target, countDown);

            if (totalUltDamage < targetHealth)
                return false;

            return true;
        }

        public static bool HasPotionActive(AIHeroClient hero)
        {
            string[] potionStrings = {
                    RegenerationSpellBook.HealthPotion.BuffName,
                    RegenerationSpellBook.HealthPotion.BuffNameCookie,
                    RegenerationSpellBook.RefillablePotion.BuffName,
                    RegenerationSpellBook.CorruptingPotion.BuffName,
                    RegenerationSpellBook.HuntersPotion.BuffName
                };

            return hero.Buffs.Any(x => potionStrings.Contains(x.Name));
        }

        /// <summary>
        /// per second
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static float GetPotionRegenRate(BuffInstance buff)
        {
            if (buff.Name == RegenerationSpellBook.HealthPotion.BuffName ||
                buff.Name == RegenerationSpellBook.HealthPotion.BuffNameCookie)
            {
                return RegenerationSpellBook.HealthPotion.RegenRate;
            }
            if (buff.Name == RegenerationSpellBook.RefillablePotion.BuffName)
            {
                return RegenerationSpellBook.RefillablePotion.RegenRate;
            }
            if (buff.Name == RegenerationSpellBook.CorruptingPotion.BuffName)
            {
                return RegenerationSpellBook.CorruptingPotion.RegenRate;
            }
            if (buff.Name == RegenerationSpellBook.HuntersPotion.BuffName)
            {
                return RegenerationSpellBook.HuntersPotion.RegenRate;
            }

            return float.NaN;
        }

        public static BuffInstance GetPotionBuff(AIHeroClient hero)
        {
            string[] potionStrings = {
                    RegenerationSpellBook.HealthPotion.BuffName,
                    RegenerationSpellBook.HealthPotion.BuffNameCookie,
                    RegenerationSpellBook.RefillablePotion.BuffName,
                    RegenerationSpellBook.CorruptingPotion.BuffName,
                    RegenerationSpellBook.HuntersPotion.BuffName
                };

            return hero.Buffs.First(x => potionStrings.Contains(x.Name));
        }

        private static float GetTargetHealth(EnemyInfo target, float additionalTime)
        {
            if (target.Player.IsHPBarRendered)
                return target.Player.Health;

            float regen = (HasPotionActive(target.Player) && Program.BaseUltMenu["trackPotion"].Cast<CheckBox>().CurrentValue) ? target.Player.HPRegenRate + GetPotionRegenRate(GetPotionBuff(target.Player)) : target.Player.HPRegenRate;

            float predictedHealth = target.Player.Health + (regen * ((Core.GameTickCount - target.LastSeen + additionalTime) / 1000f));

            return predictedHealth > target.Player.MaxHealth ? target.Player.MaxHealth : predictedHealth;
        }

        private static float GetBaseUltTravelTime(AIHeroClient source, Vector3? dest = null)
        {
            try
            {
                var targetpos = dest.HasValue ? dest.Value : GetFountainPos();
                float speed = BaseUltSpells.First(x => x.Name == source.ChampionName).Speed;
                float delay = BaseUltSpells.First(x => x.Name == source.ChampionName).Delay;


                float distance = source.ServerPosition.Distance(targetpos);

                float missilespeed = speed;
                bool isJinx = source.ChampionName.ToLower().Contains("jinx");

                if (isJinx && distance > 1350)
                {
                    const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second

                    var acceldifference = distance - 1350f;

                    if (acceldifference > 150f) //it only accelerates 150 units
                        acceldifference = 150f;

                    var difference = distance - 1500f;

                    missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) +
                        difference * 2200f) / distance;
                }

                //var result = (distance / missilespeed + delay) * 1000;
                return (distance / missilespeed + delay - 0.065f) * 1000;
            }
            catch
            {
                return int.MaxValue;
            }
        }

        private static double GetBaseUltSpellDamage(AIHeroClient target, AIHeroClient source)
        {
            var level = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1;
            if (source.ChampionName == "Jinx")
            {
                {
                    var damage = new float[] { 250, 350, 450 }[level] +
                                 new float[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                                 1 * ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Physical, damage);
                }
            }
            if (source.ChampionName == "Ezreal")
            {
                {
                    var damage = new float[] { 350, 500, 650 }[level] + 0.9f * ObjectManager.Player.FlatMagicDamageMod +
                                 1 * ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Magical, damage) * 0.7f;
                }
            }
            if (source.ChampionName == "Ashe")
            {
                {
                    var damage = new float[] { 250, 425, 600 }[level] + 1 * ObjectManager.Player.FlatMagicDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Magical, damage);
                }
            }
            if (source.ChampionName == "Draven")
            {
                {
                    var damage = new float[] { 175, 275, 375 }[level] + 1.1f * ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Physical, damage) * 0.7f;
                }
            }

            return 0;
        }

        public static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var enemy = EntityManager.Heroes.Enemies.FirstOrDefault(x => x.Equals(sender));

            if(enemy == null || args.Type != TeleportType.Recall)
                return;

            switch (args.Status)
            {
                case TeleportStatus.Start:
                {
                    Recalls.Add(new Recall(enemy, RecallStatus.Active){
                        Started = args.Start,
                        Duration = args.Duration
                    });
                    break;
                }

                case TeleportStatus.Abort:
                {
                    var abortedEnemy = Recalls.FirstOrDefault(x => x.Unit.Equals(enemy));

                    if(abortedEnemy == null)
                        return;

                    Recalls.Remove(abortedEnemy);
                    removeFromBaseUlt(abortedEnemy.Unit);
                    break;
                }

                case TeleportStatus.Finish:
                {
                    var finishedEnemy = Recalls.FirstOrDefault(x => x.Unit.Equals(enemy));

                    if (finishedEnemy == null)
                        return;

                    Recalls.Remove(finishedEnemy);
                    removeFromBaseUlt(finishedEnemy.Unit);
                    break;
                }
            }
        }

        private static void removeFromBaseUlt(AIHeroClient obj)
        {
            var target = BaseUltUnits.FirstOrDefault(x => x.Unit.Equals(obj));
            
            if (target == null)
                return;

            BaseUltUnits.Remove(target);
        }

        private static IEnumerable<Obj_AI_Base> GetCollision(string sourceName)
        {
            var heroEntry = BaseUltSpells.First(x => x.Name == sourceName);

            if(!heroEntry.Collision)
                return new List<Obj_AI_Base>();

            Vector3 enemyBaseVec = GetFountainPos();

            return (from unit in EntityManager.Heroes.Enemies.Where(h => ObjectManager.Player.Distance(h) < 2000)
                    let pred =
                        Prediction.Position.PredictLinearMissile(unit, 2000, (int)heroEntry.Radius, (int)heroEntry.Delay,
                            heroEntry.Speed, -1)
                    let endpos = ObjectManager.Player.ServerPosition.Extend(enemyBaseVec, 2000)
                    let projectOn = pred.UnitPosition.To2D().ProjectOn(ObjectManager.Player.ServerPosition.To2D(), endpos)
                    where projectOn.SegmentPoint.Distance(endpos) < (int)heroEntry.Radius + unit.BoundingRadius
                    select unit).Cast<Obj_AI_Base>().ToList();
        }

        public static void Drawing_OnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
        }

        public static void Drawing_OnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
        }

        /// <summary>
        /// Regens per second
        /// </summary>
        private class RegenerationSpellBook
        {
            public static class HealthPotion
            {
                public static string BuffName = "RegenerationPotion";
                public static string BuffNameCookie = "ItemMiniRegenPotion";
                public static float RegenRate
                {
                    get { return 10; }
                }
                public static float Duration = 15000;
            }
            public static class RefillablePotion
            {
                public static string BuffName = "ItemCrystalFlask";
                public static float RegenRate
                {
                    get { return 10; }
                }
                public static float Duration = 12000;
            }

            public static class HuntersPotion
            {
                public static string BuffName = "ItemCrystalFlaskJungle";
                public static float RegenRate
                {
                    get { return 9; }
                }
                public static float Duration = 8000;
            }
            public static class CorruptingPotion
            {
                public static string BuffName = "ItemDarkCrystalFlask";
                public static float RegenRate
                {
                    get { return 16; }
                }
                public static float Duration = 12000;
            }
        }
    }

    public class Recall
    {
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
        public BaseUltUnit(AIHeroClient unit, float fireTime)
        {
            Unit = unit;
            FireTime = fireTime;
            processed = false;
        }

        public AIHeroClient Unit { get; set; }
        public float FireTime { get; set; }
        public bool processed { get; set; }
    }
    
    public class EnemyInfo
    {
        public AIHeroClient Player;
        public float LastSeen;

        public EnemyInfo(AIHeroClient player)
        {
            Player = player;
            LastSeen = 0;
        }
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