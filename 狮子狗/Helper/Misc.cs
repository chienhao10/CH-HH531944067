using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace RengarPro_Revamped.Helper
{
    internal class Misc
    {
        public static int[] BlueSmite = {3706, 1400, 1401, 1402, 1403};
        public static int[] RedSmite = {3715, 1415, 1414, 1413, 1412};
        public static SpellSlot Smite;

        public static void Init()
        {
            Game.OnTick += Game_OnTick;
        }

        public static void Game_OnTick(EventArgs args)
        {
            AutoHp();
            SmiteCombo();
            SkinHack();
            AutoYoumuu();
            BetaQ();
        }

        public static void BetaQ()
        {
            try
            {
                if (!BetaQVariables.BetaQActive)
                {
                    return;
                }
                if (TargetSelector.SelectedTarget.IsValidTarget(BetaQVariables.BetaQRange) &&
                    MenuChecker.ComboModeSelected == 1 && Standarts.RengarHasUltimate && Standarts.Q.IsReady())
                {
                    Standarts.Q.Cast();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void AutoYoumuu()
        {
            try
            {
                if (!MenuChecker.AutoYoumuuActive)
                {
                    return;
                }
                if (Standarts.RengarHasUltimate && Item.CanUseItem(ItemId.Youmuus_Ghostblade))
                {
                    Core.DelayAction(() => Item.UseItem(ItemId.Youmuus_Ghostblade), 600);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SkinHack()
        {
            try
            {
                if (!MenuChecker.SkinHackActive)
                {
                    Standarts.Rengar.SetSkinId(0);
                    return;
                }

                switch (MenuChecker.SkinHackValue)
                {
                    case 1:
                    {
                        Standarts.Rengar.SetSkinId(1);
                        break;
                    }
                    case 2:
                    {
                        Standarts.Rengar.SetSkinId(2);
                        break;
                    }
                    case 3:
                    {
                        Standarts.Rengar.SetSkinId(3);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SmiteCombo()
        {
            try
            {
                if (BlueSmite.Any(id => Item.HasItem(id)))
                {
                    Smite = Standarts.Rengar.GetSpellSlotFromName("s5_summonersmiteplayerganker");
                    return;
                }

                if (RedSmite.Any(id => Item.HasItem(id)))
                {
                    Smite = Standarts.Rengar.GetSpellSlotFromName("s5_summonersmiteduel");
                    return;
                }

                Smite = Standarts.Rengar.GetSpellSlotFromName("summonersmite");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void AutoHp()
        {
            try
            {
                if (!MenuChecker.AutoHpActive)
                {
                    return;
                }
                if (Standarts.Rengar.HealthPercent <= MenuChecker.AutoHpValue && Standarts.Ferocity == 5 &&
                    Standarts.W.IsReady())
                {
                    Standarts.W.Cast();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static class BetaQVariables
        {
            public static bool BetaQActive = Menu.ComboM["betaq.active"].Cast<CheckBox>().CurrentValue;
            public static int BetaQRange = Menu.ComboM["betaq.range"].Cast<Slider>().CurrentValue;
        }
    }
}