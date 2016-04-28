/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/

using System;

namespace GodJungleTracker.Classes
{
    public class Packets
    {
        public static OnAttack Attack;
        public static OnMissileHit MissileHit;
        public static OnDisengaged Disengaged;
        public static OnMonsterSkill MonsterSkill;
        public static OnCreateGromp CreateGromp;
        public static OnCreateCampIcon CreateCampIcon;

        static Packets() 
        {
            try
            {
                Attack = new OnAttack();
                MissileHit = new OnMissileHit();
                Disengaged = new OnDisengaged();
                MonsterSkill = new OnMonsterSkill();
                CreateGromp = new OnCreateGromp();
                CreateCampIcon = new OnCreateCampIcon();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public class OnAttack
        {
            public OnAttack(int header = 0, int length = 71)
            {
                Length = length;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
        }

        public class OnMissileHit
        {
            public OnMissileHit(int header = 0, int length = 35)
            {
                Length = length;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
        }

        public class OnDisengaged
        {
            public OnDisengaged(int header = 0, int length = 68)
            {
                Length = length;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
        }

        public class OnMonsterSkill
        {
            public OnMonsterSkill(int header = 0, int length = 47, int length2 = 68)
            {
                Length = length;
                Length2 = length2;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
        }

        public class OnCreateGromp
        {
            public OnCreateGromp(int header = 0, int length = 302, int length2 = 311)
            {
                Length = length;
                Length2 = length2;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
        }

        public class OnCreateCampIcon
        {
            public OnCreateCampIcon(int header = 0, int length = 74, int length2 = 86, int length3 = 83, int length4 = 62, int length5 = 71)
            {
                Length = length;
                Length2 = length2;
                Length3 = length3;
                Length4 = length4;
                Length5 = length5;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
            public int Length3 { get; set; }
            public int Length4 { get; set; }
            public int Length5 { get; set; }
        }
    }
}
