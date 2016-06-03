//Thanks to iSnorflake for tutorial and Trelli for updater


using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace AutoLevelup
{
    class Program
    {
        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;
        public static string tipo = "";
        private static SpellSlot Smite;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player

        public static void Game_OnGameLoad()
        {
            Smite = ObjectManager.Player.GetSpellSlot("SummonerSmite");
            if (Player.BaseSkinName == "Aatrox") abilitySequence = new int[] { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 2, 2 };
            else if (Player.BaseSkinName == "AurelionSol") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Ahri") abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Akali") abilitySequence = new int[] { 3, 1, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Alistar") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Amumu") abilitySequence = new int[] { 3, 2, 3, 1, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Anivia") abilitySequence = new int[] { 1, 3, 3, 1, 3, 4, 3, 2, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Annie") abilitySequence = new int[] { 1, 2, 1, 2, 1, 4, 1, 3, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Ashe") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Azir") abilitySequence = new int[] { 2, 1, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Blitzcrank") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Bard") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Brand") abilitySequence = new int[] { 2, 1, 3, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Braum") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Caitlyn") abilitySequence = new int[] { 1, 2, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Cassiopeia") abilitySequence = new int[] { 1, 3, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Chogath") abilitySequence = new int[] { 3, 1, 2, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Corki")
            {
                var player = HeroManager.Player;
                if (player.TotalMagicalDamage - player.BaseAbilityDamage > player.TotalAttackDamage - player.BaseAttackDamage)
                {
                    abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " AP";
                }

                else
                {
                    abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " AD";
                }
            }
            else if (Player.BaseSkinName == "Darius") abilitySequence = new int[] { 2, 1, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Diana")
            {
                rOff = -1;
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 1, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 2, 1, 2, 3, 1, 4, 1, 1, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "DrMundo") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Draven") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Elise") abilitySequence = new int[] { 2, 1, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Evelynn") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Ezreal")
            {
                var player = HeroManager.Player;
                if (player.TotalMagicalDamage - player.BaseAbilityDamage > player.TotalAttackDamage - player.BaseAttackDamage)
                {
                    abilitySequence = new int[] { 1, 2, 3, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
                    tipo = " AP";
                }
                else
                {
                    abilitySequence = new int[] { 1, 3, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 2, 2, 2, 4, 2, 2 };
                    tipo = " AD";
                }
            }
            else if (Player.BaseSkinName == "Ekko")
            {
                rOff = -1;
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 1, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "FiddleSticks") abilitySequence = new int[] { 2, 3, 2, 1, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Fiora") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Fizz")
            {
                rOff = -1;
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 3, 2, 1, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Galio") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Gangplank") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Garen") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Gnar") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Gragas") abilitySequence = new int[] { 3, 2, 1, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Graves") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Hecarim")
            {
                rOff = -1;
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Heimerdinger") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 2, 2 };
            else if (Player.BaseSkinName == "Irelia") abilitySequence = new int[] { 1, 3, 2, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Illaoi") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Janna") abilitySequence = new int[] { 3, 2, 1, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "JarvanIV")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 3, 1, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Jax")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 3, 2, 2, 1, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Jayce") { abilitySequence = new int[] { 1, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 2, 3, 3, 3, 3, 3 }; rOff = -1; }
            else if (Player.BaseSkinName == "Jinx") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Jhin") abilitySequence = new int[] { 2, 1, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Kindred") abilitySequence = new int[] { 2, 1, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Karma") { abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 }; rOff = -1; }
            else if (Player.BaseSkinName == "Karthus") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Kassadin") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Katarina") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Kalista") abilitySequence = new int[] { 3, 1, 3, 2, 3, 4, 1, 3, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Kayle") abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Kennen") abilitySequence = new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Khazix") abilitySequence = new int[] { 1, 2, 2, 3, 1, 4, 1, 1, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "KogMaw") abilitySequence = new int[] { 2, 1, 2, 3, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Leblanc") abilitySequence = new int[] { 2, 1, 2, 3, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "LeeSin") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Leona") abilitySequence = new int[] { 3, 1, 2, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Lissandra") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Lucian") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Lulu") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Lux") abilitySequence = new int[] { 1, 3, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Malphite") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Malzahar") abilitySequence = new int[] { 3, 2, 1, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Maokai")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "MasterYi") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "MissFortune") abilitySequence = new int[] { 3, 2, 3, 1, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Mordekaiser") abilitySequence = new int[] { 2, 3, 1, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Morgana") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Nami") abilitySequence = new int[] { 1, 2, 2, 3, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Nasus") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Nautilus") abilitySequence = new int[] { 1, 3, 2, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Nidalee") { abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 3, 3 }; rOff = -1; }
            else if (Player.BaseSkinName == "Nocturne") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Nunu") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Olaf")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 1, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 3, 3 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 3, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Orianna") abilitySequence = new int[] { 3, 1, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Pantheon")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 1, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Poppy") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Quinn") abilitySequence = new int[] { 3, 1, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Rammus")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 1, 2, 3, 3, 4, 3, 3, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 3, 2, 1, 3, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Renekton") abilitySequence = new int[] { 2, 3, 1, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Reksai") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Rengar")
            {
                var player = HeroManager.Player;
                if (player.TotalMagicalDamage - player.BaseAbilityDamage > player.TotalAttackDamage - player.BaseAttackDamage)
                {
                    abilitySequence = new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    tipo = " AP";
                }
                else
                {
                    abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " AD";
                }
            }
            else if (Player.BaseSkinName == "Riven") abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Rumble")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Ryze") abilitySequence = new int[] { 3, 2, 1, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Sejuani") abilitySequence = new int[] { 2, 1, 3, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Shaco")
            {
                var player = HeroManager.Player;
                if (player.TotalMagicalDamage - player.BaseAbilityDamage > player.TotalAttackDamage - player.BaseAttackDamage)
                {
                    abilitySequence = new int[] { 2, 1, 3, 2, 3, 4, 3, 3, 3, 2, 2, 2, 4, 1, 1, 4, 1, 1 };
                    tipo = " AP";
                }
                else
                {
                    abilitySequence = new int[] { 2, 1, 3, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    tipo = " AD";
                }
            }
            else if (Player.BaseSkinName == "Shen") abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Shyvana") abilitySequence = new int[] { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Singed") abilitySequence = new int[] { 1, 3, 1, 3, 1, 4, 1, 2, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Sion")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 1, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 3, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Sivir") abilitySequence = new int[] { 2, 1, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Skarner") abilitySequence = new int[] { 1, 2, 3, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Sona") abilitySequence = new int[] { 1, 2, 1, 2, 1, 4, 3, 1, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Soraka") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Swain") abilitySequence = new int[] { 2, 3, 1, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Syndra") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "TahmKench") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Taliyah") abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Talon") abilitySequence = new int[] { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Taric") abilitySequence = new int[] { 3, 1, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Teemo") abilitySequence = new int[] { 3, 1, 2, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
            else if (Player.BaseSkinName == "Thresh") abilitySequence = new int[] { 3, 1, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Tristana") abilitySequence = new int[] { 3, 2, 3, 1, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Trundle") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Tryndamere") abilitySequence = new int[] { 3, 1, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "TwistedFate") abilitySequence = new int[] { 2, 3, 1, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Twitch") abilitySequence = new int[] { 3, 2, 3, 1, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Udyr") abilitySequence = new int[] { 4, 2, 4, 3, 4, 3, 4, 3, 4, 3, 3, 2, 2, 2, 2, 1, 1, 1 };
            else if (Player.BaseSkinName == "Urgot") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Varus") abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Vayne") abilitySequence = new int[] { 1, 2, 3, 2, 2, 4, 1, 1, 1, 1, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Veigar") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Velkoz") abilitySequence = new int[] { 2, 1, 2, 3, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Vi") abilitySequence = new int[] { 2, 3, 1, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Viktor") abilitySequence = new int[] { 1, 3, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Vladimir") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Volibear") abilitySequence = new int[] { 2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Warwick") abilitySequence = new int[] { 2, 1, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "MonkeyKing")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 2, 3, 1, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 3, 1, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Xerath") abilitySequence = new int[] { 2, 1, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "XinZhao") abilitySequence = new int[] { 2, 1, 3, 1, 2, 4, 2, 2, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
            else if (Player.BaseSkinName == "Yasuo") abilitySequence = new int[] { 1, 3, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Yorick") abilitySequence = new int[] { 3, 1, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Zac")
            {
                if (Smite != SpellSlot.Unknown)
                {
                    abilitySequence = new int[] { 3, 2, 1, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
                    tipo = " Jungler";
                }
                else
                {
                    abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    tipo = " Lane";
                }
            }
            else if (Player.BaseSkinName == "Zed") abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Ziggs") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Zilean") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
            else if (Player.BaseSkinName == "Zyra") abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };

            Game.OnUpdate += Game_OnUpdate;
        }

        static void Game_OnUpdate(EventArgs args)
        { //AutoLevelup
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level + qOff;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level + wOff;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level + eOff;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level + rOff;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = new int[] { 0, 0, 0, 0 };
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);

            }
        }
    }
}