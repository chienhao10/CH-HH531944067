using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;

namespace Nechrito_Nidalee
{
    class Core
    {
       /// <summary>
       /// TO DO
       /// Zhonyas Logic
       /// Passive buffs, extend pounce if target has buff
       /// Summoner Killsteal
       /// Flee Wall
       /// </summary>
        public static AIHeroClient Player => ObjectManager.Player;
        internal static bool CatForm()
        {
            return Player.CharData.BaseSkinName != "Nidalee";
        }
        public class Champion
        {
            public static SpellSlot Ignite;
            public static LeagueSharp.Common.Spell Javelin { get; set; }
            public static LeagueSharp.Common.Spell Takedown { get; set; }
            public static LeagueSharp.Common.Spell Pounce { get; set; }
            public static LeagueSharp.Common.Spell Swipe { get; set; }
            public static LeagueSharp.Common.Spell Bushwack { get; set; }
            public static LeagueSharp.Common.Spell Primalsurge { get; set; }
            public static LeagueSharp.Common.Spell Aspect { get; set; }
          
            public static void Load()
            {
                Javelin = new LeagueSharp.Common.Spell(SpellSlot.Q, 1500);
                Takedown = new LeagueSharp.Common.Spell(SpellSlot.Q, 400);
                Pounce = new LeagueSharp.Common.Spell(SpellSlot.W, 375);
                Swipe = new LeagueSharp.Common.Spell(SpellSlot.E, 300);
                Primalsurge = new LeagueSharp.Common.Spell(SpellSlot.E, 600);
                Bushwack= new LeagueSharp.Common.Spell(SpellSlot.W, 875);

                Aspect = new LeagueSharp.Common.Spell(SpellSlot.R);

                Pounce.SetSkillshot(0.50f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                Swipe.SetSkillshot(0.25f, (float)(15 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
                Bushwack.SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                Javelin.SetSkillshot(0.25f, 40f, 1500f, true, SkillshotType.SkillshotLine);

                Ignite = Player.GetSpellSlot("SummonerDot");
            }
        }
    }
}

