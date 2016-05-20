using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Modes
{
    internal static class None
    {
        public static void ImmobileExecute()
        {
            if (Spells.E.IsReady() && Menus.getCheckBoxItem(Menus.miscMenu, "auto.e.immobile"))
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && x.IsEnemyImmobile()))
                {
                    Spells.E.Cast(enemy);
                }
            }
        }

        public static void KillSteal()
        {
            if (ObjectManager.Player.IsActive(Spells.R))
            {
                return;
            }
            if (Spells.Q.IsReady() && Menus.getCheckBoxItem(Menus.ksMenu, "q.ks"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) &&
                                                                     x.Health < Spells.Q.GetDamage(x)))
                {
                    Spells.Q.CastOnUnit(enemy);
                }
            }
            if (Spells.W.IsReady() && Menus.getCheckBoxItem(Menus.ksMenu, "w.ks"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.LSDistance(ObjectManager.Player) <
                                Menus.getSliderItem(Menus.wMenu, "w.combo.max.distance") &&
                                x.LSDistance(ObjectManager.Player) >
                                Menus.getSliderItem(Menus.wMenu, "w.combo.min.distance") && x.IsValid &&
                                Spells.W.GetPrediction(x).Hitchance >= Menus.wMenu.HikiChance("w.hit.chance") &&
                                x.Health < Spells.W.GetDamage(x) && !x.IsDead && !x.IsZombie && x.IsValid))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }

        public static void TeleportE()
        {
            if (Spells.E.IsReady() && Menus.getCheckBoxItem(Menus.eMenu, "e.combo") &&
                Menus.getCheckBoxItem(Menus.eMenu, "e.combo.teleport"))
            {
                foreach (
                    var obj in
                        ObjectManager.Get<Obj_AI_Base>()
                            .Where(
                                x =>
                                    x.Team != ObjectManager.Player.Team &&
                                    x.LSDistance(ObjectManager.Player) < Spells.E.Range
                                    && x.HasBuff("teleport_target") && !x.IsDead && !x.IsZombie))
                {
                    Spells.E.Cast(obj);
                }
            }
        }
    }
}