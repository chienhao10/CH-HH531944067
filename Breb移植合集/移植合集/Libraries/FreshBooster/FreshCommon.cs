using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using LeagueSharp.Common;
using PortAIO.Champion.Blitzcrank;
using SharpDX;
using Color = System.Drawing.Color;

namespace FreshBooster
{
    internal enum SpellType
    {
        AP,
        AD,
        TRUE
    }

    internal class AllChampionSpellDB
    {
        public bool CanAttack;
        public bool CanMove;
        public bool CanSpell;
        public bool Dangerous;
        public string Name;
        public SpellType Type;
    }

    internal class FreshCommon
    {

        public static AllChampionSpellDB[] SpellDB1 =
        {
            //Aatrox Q E
            new AllChampionSpellDB
            {
                Name = "",
                Type = SpellType.AD,
                CanAttack = true,
                CanMove = true,
                CanSpell = true,
                Dangerous = true
            }
            /*
            
            Ahri Q,W,E,R
            Akali QER
            Alistar QW
            Amumu QER
            Anavia QER
            Anni QWR
            Ashe WR
            Azir QE
            Bard Q
            Blitzcrank QER
            Brand QWER
            Braum QR
            Caytryn QER
            Casiopea QWER
            Chogas QWR
            Corki QWER
            Drius QER
            Diana QWER
            DR Mundo Q
            Draven QER
            Ekko Q
            Elise QQ W E
            Evlyn Q E R
            Ezreal QWER
            Fiddlesticks QWER
            Fiora Q
            Fizz Q E R
            Galio QER
            Ganplank R
            Garen QR
            Gragas QER
            Graves QR
            Hecarim ER
            Heimerdinger WER
            Illaoii QE
            Irelia QER
            Janna QWR
            Jarvan QER
            Jax QW
            Jayce QWE 
            Jin QWR
            Jinx WER
            Kalistar QR
            Karma QW RQ RW
            Kathus QR
            Kasadin QER
            Katarina QWR
            Kayle Q
            Kenen QWR
            Kazix QW
            Kindred Q
            Kogmo QER
            Leblanc QWE
            Leesin QER
            Leona QER
            Lissandra QWR
            Lusian QWR
            Lulu QE
            Lux QER
            Malpite QER
            Malzahar QWR
            Maokai QW
            Master Yi Q
            Missfotune QER
            Modekaiser QE
            Morgana QWR
            Nami QWR
            Nasus QWE
            Nautilus QER
            Nidalee QE
            Nocturne QER
            NuNu ER
            Olaf QE
            Orianna QWR
            Pantheon QWER
            Poppy QER
            Quinn QER
            Rammus QE
            Reksai QE
            Renekton QWE 
            Rengar QWE
            Riven QWR
            Rumble QER
            Ryze QWE
            Sejuani QWER
            Sako E
            Shen QE
            Shyvana QER
            */
        };

        public static int TickCount(int time)
        {
            return Environment.TickCount + time;
        }

        public static int NowTime()
        {
            return Environment.TickCount;
        }

        public static HitChance Hitchance(string Type)
        {
            var result = HitChance.Low;
            if (ObjectManager.Player.ChampionName == EloBuddy.Champion.Blitzcrank.ToString())
            {
                switch (Program.getQHitChance())
                {
                    case 1:
                        result = HitChance.OutOfRange;
                        break;
                    case 2:
                        result = HitChance.Impossible;
                        break;
                    case 3:
                        result = HitChance.Low;
                        break;
                    case 4:
                        result = HitChance.Medium;
                        break;
                    case 5:
                        result = HitChance.High;
                        break;
                    case 6:
                        result = HitChance.VeryHigh;
                        break;
                }
            }

            if (ObjectManager.Player.ChampionName == EloBuddy.Champion.Veigar.ToString())
            {
                switch (Champion.Veigar.getQHitChance())
                {
                    case 1:
                        result = HitChance.OutOfRange;
                        break;
                    case 2:
                        result = HitChance.Impossible;
                        break;
                    case 3:
                        result = HitChance.Low;
                        break;
                    case 4:
                        result = HitChance.Medium;
                        break;
                    case 5:
                        result = HitChance.High;
                        break;
                    case 6:
                        result = HitChance.VeryHigh;
                        break;
                }
            }

            return result;
        }

        public static Obj_AI_Base GetFindObj(string name, float range, Vector3 Pos)
        {
            var CusPos = Pos;
            if (ObjectManager.Player.LSDistance(CusPos) > range)
                CusPos = ObjectManager.Player.Position.Extend(Game.CursorPos, range).To3D();
            var GetObj =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        f =>
                            f.IsAlly && !f.IsMe && f.Position.LSDistance(ObjectManager.Player.Position) < range &&
                            f.LSDistance(CusPos) < 150);
            return GetObj;
        }

        public static void MovingPlayer(Vector3 Pos)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Pos);
        }

        public static Vector2 ToScreen(Vector3 Target)
        {
            var target = Drawing.WorldToScreen(Target);
            return target;
        }

        public static bool HasBuff(AIHeroClient Hero)
        {
            if (Hero.HasBuffOfType(BuffType.Flee))
                return true;
            if (Hero.HasBuffOfType(BuffType.Charm))
                return true;
            if (Hero.HasBuffOfType(BuffType.Polymorph))
                return true;
            if (Hero.HasBuffOfType(BuffType.Snare))
                return true;
            if (Hero.HasBuffOfType(BuffType.Stun))
                return true;
            if (Hero.HasBuffOfType(BuffType.Taunt))
                return true;
            if (Hero.HasBuffOfType(BuffType.Suppression))
                return true;
            return false;
        }

        public static Color LineColor(AIHeroClient Hero)
        {
            if (Hero.HealthPercent <= 10)
                return Color.Red;
            if (Hero.HealthPercent <= 25)
                return Color.OrangeRed;
            if (Hero.HealthPercent <= 35)
                return Color.Yellow;
            if (Hero.HealthPercent <= 45)
                return Color.YellowGreen;
            if (Hero.HealthPercent <= 60)
                return Color.Green;
            return Color.Green;
        }
    }
}