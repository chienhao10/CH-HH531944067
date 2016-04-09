using System;
using System.Linq;
using AddonTemplate.Logic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Aka_s_Vayne_reworked.Events
{
    internal class _game
    {
        public static void Skinhack()
        {
            if (MenuManager.MiscMenu["skinhack"].Cast<CheckBox>().CurrentValue)
            {
                Player.SetSkinId((int)MenuManager.MiscMenu["skinId"].Cast<ComboBox>().CurrentValue);
            }
        }

        public static void AAReset()
        {
            if (Functions.Events._game.Target != null)
            {
                if (Game.Time * 1000 > Variables.lastaa + Variables._Player.AttackDelay * 1000 - Game.Ping * 2.15)
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Functions.Events._game.Target);
                }
                else if (Game.Time * 1000 > Variables.lastaa + Variables._Player.AttackCastDelay * 1000 - Game.Ping / 2.15 + 50f && Game.Time * 1000 > Variables.lastmove + 150f)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    Variables.lastmove = Game.Time * 1000;
                }
            }
            else
            {
                if (Game.Time * 1000 > Variables.lastaa + Variables._Player.AttackCastDelay * 1000 - Game.Ping / 2.15 + 50f && Game.Time * 1000 > Variables.lastmove + 150f)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    Variables.lastmove = Game.Time * 1000;
                }
            }
        }

        public static void Customorbwalker()
        {
            if (Variables.stopmove && Game.Time * 1000 > Variables.lastaaclick + ObjectManager.Player.AttackCastDelay * 1000 + 25f)
            {
                Variables.stopmove = false;
            }

            if (!Variables.stopmove && Game.Time * 1000 > Variables.lastaa + ObjectManager.Player.AttackCastDelay * 1000 - Game.Ping / 2.15 + ObjectManager.Player.AttackSpeedMod * 15.2 && Game.Time * 1000 > Variables.lastmove + 150f)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                Variables.lastmove = Game.Time * 1000;
            }

            if (Functions.Events._game.Target != null)
            {
                if (Game.Time * 1000 > Variables.lastaa + ObjectManager.Player.AttackDelay * 1000 - Game.Ping * 2.15)
                {
                    Variables.stopmove = true;
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Functions.Events._game.Target);
                }
                Functions.Events._game.Botrk2(Functions.Events._game.Target);
            }
        }
    

        public static void AutoBuyTrinkets()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                if (Variables._Player.IsInShopRange() &&
                    MenuManager.MiscMenu["autobuyt"].Cast<CheckBox>().CurrentValue &&
                    Variables._Player.Level > 9 && Item.HasItem((int) ItemId.Warding_Totem_Trinket))
                {
                    //Shop.BuyItem(ItemId.Farsight_Orb_Trinket);
                }
                if (Variables._Player.IsInShopRange() &&
                    MenuManager.MiscMenu["autobuyt"].Cast<CheckBox>().CurrentValue &&
                    !Item.HasItem((int) ItemId.Sweeping_Lens_Trinket, Variables._Player) && Variables._Player.Level > 6 &&
                    EntityManager.Heroes.Enemies.Any(
                        h =>
                            h.BaseSkinName == "Rengar" || h.BaseSkinName == "Talon" ||
                            h.BaseSkinName == "Vayne"))
                {
                    Shop.BuyItem(ItemId.Sweeping_Lens_Trinket);
                }
            }
        }

        public static void FocusW()
        {

            if (MenuManager.ComboMenu["focusw"].Cast<CheckBox>().CurrentValue)
            {
                if (Functions.Events._game.FocusWTarget == null &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    return;
                }
                if (Functions.Events._game.FocusWTarget.IsValidTarget(Variables._Player.GetAutoAttackRange()) &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    TargetSelector.GetPriority(Functions.Events._game.FocusWTarget);
                }
                else
                {
                    TargetSelector.GetPriority(
                        TargetSelector.GetTarget(Variables._Player.AttackRange, DamageType.Physical));
                }
            }
        }

        public static void FastBotrk()
        {
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);

            if (MenuManager.ItemMenu["botrk"].Cast<CheckBox>().CurrentValue && target != null &&
                (target.Distance(Variables._Player) > 500f ||
                 (Variables._Player.Health/Variables._Player.MaxHealth)*100 <= 95))
            {
                Functions.Events._game.Botrk(target);
            }
        }

        public static void LevelUpSpells()
        {
            if (!MenuManager.MiscMenu["autolvl"].Cast<CheckBox>().CurrentValue) return;

            var qL = Variables._Player.Spellbook.GetSpell(SpellSlot.Q).Level + Variables.QOff;
            var wL = Variables._Player.Spellbook.GetSpell(SpellSlot.W).Level + Variables.WOff;
            var eL = Variables._Player.Spellbook.GetSpell(SpellSlot.E).Level + Variables.EOff;
            var rL = Variables._Player.Spellbook.GetSpell(SpellSlot.R).Level + Variables.ROff;
            if (qL + wL + eL + rL >= Variables._Player.Level) return;
            int[] level = {0, 0, 0, 0};
            for (var i = 0; i < Variables._Player.Level; i++)
            {
                level[Variables.AbilitySequence[i] - 1] = level[Variables.AbilitySequence[i] - 1] + 1;
            }
            if (qL < level[0]) Variables._Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Variables._Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Variables._Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Variables._Player.Spellbook.LevelSpell(SpellSlot.R);
        }

        public static void heal()
        {
            if (MenuManager.ItemMenu["heal"].Cast<CheckBox>().CurrentValue &&
                Variables._Player.CountEnemiesInRange(800) >= 1 &&
                Variables._Player.HealthPercent <= MenuManager.ItemMenu["hp"].Cast<Slider>().CurrentValue)
            {
                Program.Heal.Cast();
            }
            foreach (
                var ally in EntityManager.Heroes.Allies.Where(a => !a.IsDead))
            {
                if (MenuManager.ItemMenu["healally"].Cast<CheckBox>().CurrentValue && ally.CountEnemiesInRange(800) >= 1 &&
                    Variables._Player.Position.Distance(ally) < 600 &&
                    ally.HealthPercent <= MenuManager.ItemMenu["hpally"].Cast<Slider>().CurrentValue)
                {
                    Program.Heal.Cast();
                }
            }
            foreach (
                var ally in EntityManager.Heroes.Allies.Where(a => !a.IsDead))
            {
                if (MenuManager.ItemMenu["healally"].Cast<CheckBox>().CurrentValue &&
                    Variables._Player.Position.Distance(ally) < 600 && ally.HasBuff("summonerdot") &&
                    ally.HealthPercent <= MenuManager.ItemMenu["hpally"].Cast<Slider>().CurrentValue)
                {
                    Program.Heal.Cast();
                }
            }

            if (MenuManager.ItemMenu["heal"].Cast<CheckBox>().CurrentValue && Variables._Player.HasBuff("summonerdot") &&
                Variables._Player.HealthPercent <= MenuManager.ItemMenu["hp"].Cast<Slider>().CurrentValue)
            {
                Program.Heal.Cast();
            }
        }

        public static void autoBuy()
        {
            if (!MenuManager.MiscMenu["autobuy"].Cast<CheckBox>().CurrentValue) return;

            if (Variables.bought || Variables.ticks/Game.TicksPerSecond < 3)
            {
                Variables.ticks++;
                return;
            }

            Variables.bought = true;
            if (Game.MapId == GameMapId.SummonersRift && Variables._Player.Level == 1)
            {
                Shop.BuyItem(ItemId.Dorans_Blade);
                Shop.BuyItem(ItemId.Health_Potion);
                Shop.BuyItem(ItemId.Warding_Totem_Trinket);

            }
        }

        public static void AutoPotions()
        {
            if (MenuManager.ItemMenu["autopotion"].Cast<CheckBox>().CurrentValue && !Variables._Player.IsInShopRange() &&
                Variables._Player.HealthPercent <= MenuManager.ItemMenu["autopotionhp"].Cast<Slider>().CurrentValue &&
                !(Player.HasBuff("RegenerationPotion")))
            {
                if (Program.HPPot.IsReady() && Program.HPPot.IsOwned())
                {
                    Program.HPPot.Cast();
                }
            }

            if (MenuManager.ItemMenu["autobiscuit"].Cast<CheckBox>().CurrentValue && !Variables._Player.IsInShopRange() &&
    Variables._Player.HealthPercent <= MenuManager.ItemMenu["autobiscuithp"].Cast<Slider>().CurrentValue &&
    !(Player.HasBuff("RegenerationPotion")))
            {
                if (Program.Biscuit.IsReady() && Program.Biscuit.IsOwned())
                {
                    Program.Biscuit.Cast();
                }
            }
        }

        public static
            void EloBuddyOrbDisabler()

        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                if (Orbwalker.DisableAttacking)
                {
                    Orbwalker.DisableAttacking = false;
                }
                if (Orbwalker.DisableMovement)
                {
                    Orbwalker.DisableMovement = false;
                }
            }
            else
            {
                if (!Orbwalker.DisableAttacking)
                {
                    Orbwalker.DisableAttacking = true;
                }
                if (!Orbwalker.DisableMovement)
                {
                    Orbwalker.DisableMovement = true;
                }
            }
        }

        public static void LowlifeE()
        {
            if (!Program.E.IsReady() || !MenuManager.MiscMenu["LowLifeE"].Cast<CheckBox>().CurrentValue ||
                Variables._Player.HealthPercent > 25)
            {
                return;
            }

            var meleeEnemies = EntityManager.Heroes.Enemies.FindAll(m => m.IsMelee);

            if (meleeEnemies.Any())
            {
                var mostDangerous =
                    meleeEnemies.OrderByDescending(m => m.GetAutoAttackDamage(Variables._Player)).First();
                Program.E.Cast(mostDangerous);
            }
        }

        public static void QKs()
        {
            var currentTarget = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);

            if (!currentTarget.IsValidTarget() || currentTarget.IsZombie || currentTarget.IsInvulnerable || currentTarget.IsDead)
            {
                return;
            }

            if (currentTarget.ServerPosition.Distance(Variables._Player.ServerPosition) <=
                Variables._Player.GetAutoAttackRange())
            {
                return;
            }

            if (currentTarget.Health <
                Variables._Player.GetAutoAttackDamage(currentTarget) +
                Variables._Player.GetSpellDamage(currentTarget, SpellSlot.Q)
                && currentTarget.Health > 0)
            {
                var extendedPosition = (Vector3)Variables._Player.ServerPosition.Extend(
                    currentTarget.ServerPosition, 300f);
                if (extendedPosition.IsSafe())
                {
                    Orbwalker.ResetAutoAttack();
                    Player.CastSpell(SpellSlot.Q, extendedPosition);
                }
            }
        }
    }
}
