using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Mordekaiser.Events
{
    internal class Combo
    {
        private static bool isAttackingToTarget;
        public static float GhostAttackDelay = 1200;

        public Combo()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        private static AIHeroClient GetTarget
        {
            get { return TargetSelector.GetTarget(Spells.R.Range, DamageType.Physical); }
        }

        private static bool MordekaiserHaveSlave
        {
            get { return Utils.Player.Self.Spellbook.GetSpell(SpellSlot.R).Name == "mordekaisercotgguide"; }
        }

        public static Obj_AI_Base HowToTrainYourDragon
        {
            get
            {
                if (!MordekaiserHaveSlave)
                    return null;

                return
                    ObjectManager.Get<Obj_AI_Base>()
                        .FirstOrDefault(
                            m =>
                                m.LSDistance(Utils.Player.Self.Position) < 15000 && !m.Name.Contains("inion") && m.IsAlly &&
                                m.HasBuff("mordekaisercotgpetbuff2"));
            }
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var hero = args.Target as AIHeroClient;
                isAttackingToTarget = hero != null;
            }
        }


        private static void Game_OnUpdate(EventArgs args)
        {
            if (Utils.Player.Self.IsDead)
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ExecuteQ();
                ExecuteW();
                ExecuteE();
                ExecuteR();
                CastItems();

                var t = TargetSelector.GetTarget(4500, DamageType.Physical);
                if (t.IsValidTarget())
                {
                    if (HowToTrainYourDragon != null)
                    {
                        var m = HowToTrainYourDragon;
                        if (!MordekaiserHaveSlave || !(Environment.TickCount >= GhostAttackDelay))
                        {
                            return;
                        }

                        var ghostOption = Menu.getBoxItem(Menu.MenuGhost, "Ghost.Use");

                        switch (ghostOption)
                        {
                            case 1:
                            {
                                t = TargetSelector.GetTarget(Utils.Player.AutoAttackRange*2, DamageType.Physical);
                                Spells.R.Cast(t);
                            }
                                break;
                            case 2:
                            {
                                t = TargetSelector.GetTarget(4500, DamageType.Physical);
                                Spells.R.Cast(t);
                            }
                                break;
                        }
                        GhostAttackDelay = Environment.TickCount + m.AttackDelay*1000;
                    }
                }
            }
        }

        private static void ExecuteQ()
        {
            if (!Spells.Q.IsReady())
                return;

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (!isAttackingToTarget)
            {
                return;
            }

            var t = GetTarget;

            if (!t.IsValidTarget(Utils.Player.AutoAttackRange))
            {
                return;
            }

            Spells.Q.Cast(t);
        }

        private static void ExecuteW()
        {
            if (!Spells.W.IsReady() ||
                Utils.Player.Self.Spellbook.GetSpell(SpellSlot.W).Name == "mordekaisercreepingdeath2")
                return;

            if (Utils.Player.Self.CountEnemiesInRange(Spells.WDamageRadius) > 0)
            {
                if (Menu.getBoxItem(Menu.MenuW, "Selected" + Utils.Player.Self.ChampionName) == 1)
                {
                    Spells.W.CastOnUnit(Utils.Player.Self);
                }
            }
            else
            {
                Spells.W.CastOnUnit(Utils.Player.Self);
            }

            var ghost = Utils.HowToTrainYourDragon;
            if (ghost != null)
            {
                if (ghost.CountEnemiesInRange(Spells.WDamageRadius) == 0)
                    return;

                if (Menu.getBoxItem(Menu.MenuW, "SelectedGhost") == 1)
                {
                    Spells.W.CastOnUnit(ghost);
                }
            }

            foreach (var ally in HeroManager.Allies.Where(
                a => !a.IsDead && !a.IsMe && a.Position.LSDistance(Utils.Player.Self.Position) < Spells.W.Range)
                .Where(ally => ally.CountEnemiesInRange(Spells.WDamageRadius) > 0)
                .Where(ally => Menu.getBoxItem(Menu.MenuW, "Selected" + Utils.Player.Self.ChampionName) == 1)
                )
            {
                Spells.W.CastOnUnit(ally);
            }
        }

        private static void ExecuteE()
        {
            if (!Spells.E.IsReady())
            {
                return;
            }

            if (!Menu.getCheckBoxItem(Menu.MenuE, "UseE.Combo"))
            {
                return;
            }

            var t = GetTarget;

            if (!t.IsValidTarget(Spells.E.Range))
            {
                return;
            }

            Spells.E.Cast(t);
        }

        private static void ExecuteR()
        {
            if (!Menu.getCheckBoxItem(Menu.MenuR, "UseR.Active"))
                return;

            if (!Spells.R.IsReady())
                return;

            var t = TargetSelector.GetTarget(Spells.R.Range, DamageType.Magical);
            if (t.IsValidTarget() && t.Health <= ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R))
            {
                Spells.R.Cast(t);
            }
        }

        private static void CastItems()
        {
            var t = TargetSelector.GetTarget(750, DamageType.Physical);
            if (!t.IsValidTarget())
                return;

            foreach (var item in Items.ItemDb)
            {
                if (item.Value.ItemType == Items.EnumItemType.AoE &&
                    item.Value.TargetingType == Items.EnumItemTargettingType.EnemyHero)
                {
                    if (t.IsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady())
                        item.Value.Item.Cast();
                }
                if (item.Value.ItemType == Items.EnumItemType.Targeted &&
                    item.Value.TargetingType == Items.EnumItemTargettingType.EnemyHero)
                {
                    if (t.IsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady())
                        item.Value.Item.Cast(t);
                }
            }
        }
    }
}