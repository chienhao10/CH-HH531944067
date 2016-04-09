using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace AkaYasuo
{
    internal class EvadeSpellDatabase
    {
        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        static EvadeSpellDatabase()
        {
            if (ObjectManager.Player.ChampionName != "Yasuo")
            {
                return;
            }
            Spells.Add(
                new EvadeSpellData
                {
                    Name = "YasuoDashWrapper",
                    DangerLevel = 2,
                    Slot = SpellSlot.E,
                    EvadeType = EvadeTypes.Dash,
                    CastType = CastTypes.Target,
                    MaxRange = 475,
                    Speed = 1000,
                    Delay = 50,
                    FixedRange = true,
                    ValidTargets = new[] { SpellTargets.EnemyChampions, SpellTargets.EnemyMinions }
                });
            Spells.Add(
                new EvadeSpellData
                {
                    Name = "YasuoWMovingWall",
                    DangerLevel = 3,
                    Slot = SpellSlot.W,
                    EvadeType = EvadeTypes.WindWall,
                    CastType = CastTypes.Position,
                    MaxRange = 400,
                    Speed = int.MaxValue,
                    Delay = 250
                });
        }
    }
}