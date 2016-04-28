using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Mordekaiser.Events;
using Mordekaiser.Logics;

namespace Mordekaiser
{
    internal class Program
    {
        public const string ChampionName = "Mordekaiser";
        public static readonly AIHeroClient Player = ObjectManager.Player;
        public static EloBuddy.SDK.Menu.Menu Config;

        public static Menu Menu;
        public static Items Items;
        public static Utils Utils;
        public static Draws Draws;

        public static OnUpdate OnUpdate;
        public static Combo Combo;
        public static Harass Harass;
        public static LaneClear LaneClear;
        public static JungleClear JungleClear;
        public static DamageCalc DamageCalc;

        public static SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampionName)
                return;

            Spells.Initiate();

            Config = MainMenu.AddMenu(string.Format("xQx | {0}", ChampionName), ChampionName);

            PlayerSpells.Initialize();

            DamageCalc = new DamageCalc();
            Utils = new Utils();
            Menu = new Menu();
            Items = new Items();

            Draws = new Draws();
            Combo = new Combo();
            Harass = new Harass();
            LaneClear = new LaneClear();
            JungleClear = new JungleClear();
            OnUpdate = new OnUpdate();
            LogicW.Initiate();

            Config.Add("GameMode", new ComboBox("Game Mode:", 0, "AP", "AD", "Hybrid", "Tanky"));

            Chat.Print(
                "Mordekasier</font> <font color='#ff3232'> How to Train Your Dragon </font> <font color='#FFFFFF'>Loaded!</font>");
        }
    }
}