using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using RengarPro_Revamped.Helper;

namespace RengarPro_Revamped
{
    public static class LaneClear
    {
        public static void Initialize()
        {
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
        }

        public static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            try
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    return;
                if (Player.Instance.Mana < 5 || ((int) Player.Instance.Mana == 5 && !MenuChecker.LaneClearSaveStacks))
                {
                    if (MenuChecker.LaneClearUseQ && Standarts.Q.IsReady())
                    {
                        Standarts.Q.Cast();
                    }
                    else
                    {
                        if (ItemUsage.CanUse())
                            ItemUsage.UseItem();
                    }
                }
                else
                {
                    if (ItemUsage.CanUse())
                        ItemUsage.UseItem();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            try
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    return;
                if (Player.Instance.Mana < 5 || ((int) Player.Instance.Mana == 5 && !MenuChecker.LaneClearSaveStacks))
                {
                    if (MenuChecker.LaneClearUseW && Standarts.W.IsReady())
                    {
                        var minion = EntityManager.MinionsAndMonsters
                            .GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 400)
                            .OrderBy(x => x.Health).FirstOrDefault();
                        if (minion.IsValidTarget())
                            Standarts.W.Cast();
                    }
                    if (MenuChecker.LaneClearUseE && Standarts.E.IsReady())
                    {
                        var minion = EntityManager.MinionsAndMonsters
                            .GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Standarts.E.Range)
                            .OrderBy(x => x.Health).FirstOrDefault();
                        if (minion.IsValidTarget())
                            Standarts.E.Cast(minion);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public static class JungleClear
    {
        public static void Initialize()
        {
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
        }

        public static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            try
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    return;

                if (Player.Instance.Mana < 5 || ((int) Player.Instance.Mana == 5 && !MenuChecker.JungleClearSaveStacks))
                {
                    if (MenuChecker.JungleClearUseQ && Standarts.Q.IsReady())
                    {
                        Standarts.Q.Cast();
                    }
                    else
                    {
                        if (ItemUsage.CanUse())
                            ItemUsage.UseItem();
                    }
                }
                else
                {
                    if (ItemUsage.CanUse())
                        ItemUsage.UseItem();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            try
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    return;

                if (Player.Instance.Mana < 5 || ((int) Player.Instance.Mana == 5 && !MenuChecker.JungleClearSaveStacks))
                {
                    if (MenuChecker.JungleClearUseW && Standarts.W.IsReady())
                    {
                        var minion = EntityManager.MinionsAndMonsters
                            .GetJungleMonsters(Player.Instance.Position, 400)
                            .OrderBy(x => x.Health).FirstOrDefault();
                        if (minion.IsValidTarget())
                        {
                            Standarts.W.Cast();
                        }
                    }
                    if (MenuChecker.JungleClearUseE && Standarts.E.IsReady())
                    {
                        var minion = EntityManager.MinionsAndMonsters
                            .GetJungleMonsters(Player.Instance.Position, Standarts.E.Range)
                            .OrderBy(x => x.Health).FirstOrDefault();
                        if (minion.IsValidTarget())
                            Standarts.E.Cast(minion);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public static class ItemUsage
    {
        public static bool CanUse()
        {
            if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) || Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only))
            {
                return true;
            }

            return false;
        }

        public static void UseItem()
        {
            Item.UseItem(ItemId.Tiamat_Melee_Only);
            Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
        }
    }
}