using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;
using SharpDX;

namespace ARAMDetFull.Champions
{
    abstract class Champion
    {
        protected Champion()
        {
            Chat.Print(player.ChampionName+" plugin loaded!");
        }

        public bool safeGap(Obj_AI_Base target)
        {
            return safeGap(target.Position.LSTo2D()) || MapControl.fightIsOn(target);
        }

        public bool safeGap(Vector2 position)
        {
            return player.HealthPercent < 18 || (!Sector.inTowerRange(position) &&
                   (MapControl.balanceAroundPoint(position, 700) >= -1)) || position.LSDistance(ARAMSimulator.fromNex.Position, true) < player.Position.LSDistance(ARAMSimulator.fromNex.Position, true);
        }

        public static AIHeroClient player = ObjectManager.Player;

        public Spell Q, W, E, R;

        /* Skill Use */
        public abstract void useQ(Obj_AI_Base target);
        public abstract void useW(Obj_AI_Base target);
        public abstract void useE(Obj_AI_Base target);
        public abstract void useR(Obj_AI_Base target);

        public abstract void setUpSpells();

        public abstract void useSpells();

        public virtual void escape(){ }
        public virtual void farm() { }
        public virtual void killSteal() { }
        public virtual void alwaysCheck() { }

        public virtual void kiteBack(Vector2 pos) { }
    }
}
