using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KappaBaseUlt
{
    using EloBuddy;
    using EloBuddy.SDK.Enumerations;

    internal class Database
    {
        internal struct Hero
        {
            public Champion Champion;

            public SpellSlot Slot;

            public SkillShotType Type;

            public uint Range;

            public int Width;

            public int CastDelay;

            public int Speed;

            public float AllowedCollisionCount;
        }

        internal struct Damage
        {
            public Champion Champion;

            public DamageType DamageType;

            public float[] Floats;

            public float Float;
        }

        internal static readonly List<Damage> Damages = new List<Damage>()
                                                            {
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Ashe, DamageType = DamageType.Magical,
                                                                        Floats = new float[] { 250, 425, 600 }, Float = 1f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Draven, DamageType = DamageType.Physical,
                                                                        Floats = new float[] { 175, 275, 375 }, Float = 1.1f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Ezreal, DamageType = DamageType.Magical,
                                                                        Floats = new float[] { 350, 500, 600 }, Float = 1f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Lux, DamageType = DamageType.Magical,
                                                                        Floats = new float[] { 300, 400, 500 }, Float = 0.75f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Karthus, DamageType = DamageType.Magical,
                                                                        Floats = new float[] { 300, 400, 500 }, Float = 0.6f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Pantheon, DamageType = DamageType.Magical,
                                                                        Floats = new float[] { 200, 350, 500 }, Float = 0.5f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Gangplank, DamageType = DamageType.Magical,
                                                                        Floats = new float[] { 50, 70, 90 }, Float = 0.1f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Jinx, DamageType = DamageType.Physical,
                                                                        Floats = new float[] { 25, 35, 45 }, Float = 0.1f
                                                                    },
                                                                new Damage
                                                                    {
                                                                        Champion = Champion.Ziggs, DamageType = DamageType.Magical,
                                                                        Floats = new float[] { 200, 300, 400 }, Float = 0.73f
                                                                    }
                                                            };

        internal static readonly List<Hero> Champions = new List<Hero>()
                                                            {
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Ashe, Type = SkillShotType.Linear, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = 0, CastDelay = 250, Speed = 1600, Width = 250,
                                                                        Range = int.MaxValue
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Draven, Type = SkillShotType.Linear, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = int.MaxValue, CastDelay = 300, Speed = 2000, Width = 160,
                                                                        Range = int.MaxValue
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Ezreal, Type = SkillShotType.Linear, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = int.MaxValue, CastDelay = 1000, Speed = 2000,
                                                                        Width = 160, Range = int.MaxValue
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Karthus, Type = SkillShotType.Circular, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = int.MaxValue, CastDelay = 3000, Speed = int.MaxValue,
                                                                        Width = int.MaxValue, Range = int.MaxValue
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Lux, Type = SkillShotType.Linear, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = int.MaxValue, CastDelay = 1000, Speed = int.MaxValue,
                                                                        Width = 250, Range = 3340
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Pantheon, Type = SkillShotType.Circular, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = int.MaxValue, CastDelay = 4700, Speed = int.MaxValue,
                                                                        Width = 250, Range = 5500
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Gangplank, Type = SkillShotType.Circular, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = int.MaxValue, CastDelay = 250, Speed = int.MaxValue,
                                                                        Width = 700, Range = int.MaxValue
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Jinx, Type = SkillShotType.Linear, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = 0, CastDelay = 500, Speed = 2100, Width = 140,
                                                                        Range = int.MaxValue
                                                                    },
                                                                new Hero
                                                                    {
                                                                        Champion = Champion.Ziggs, Type = SkillShotType.Circular, Slot = SpellSlot.R,
                                                                        AllowedCollisionCount = int.MaxValue, CastDelay = 250, Speed = 1500, Width = 500,
                                                                        Range = 5300
                                                                    }
                                                            };
    }
}