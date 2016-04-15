namespace Khappa_Zix.Load
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;

    using Misc;

    using Modes;

    internal class Load
    {
        public static Spell.Targeted Q { get; private set; }

        public static Spell.Skillshot W { get; private set; }

        public static Spell.Skillshot E { get; private set; }

        public static Spell.Active R { get; private set; }

        internal static readonly AIHeroClient player = ObjectManager.Player;

        internal static bool EvolvedQ, EvolvedW, EvolvedE, EvolvedR;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (player.ChampionName != "Khazix")
            {
                return;
            }

            Q = new Spell.Targeted(SpellSlot.Q, 325);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 225, 828, 80);
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Circular, 25, 1000, 100);
            R = new Spell.Active(SpellSlot.R);

            menu.Load();
            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += JumpsHandler.JumpLogic;
            Player.OnIssueOrder += Player_OnIssueOrder;
            Obj_AI_Base.OnBasicAttack += JumpsHandler.Obj_AI_Base_OnBasicAttack;
            Orbwalker.OnPreAttack += JumpsHandler.Orbwalker_OnPreAttack;
            Spellbook.OnCastSpell += JumpsHandler.Spellbook_OnCastSpell;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
        }

        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && args.Order == GameObjectOrder.AttackUnit && player.HasBuff("KhazixRStealth")
                && menu.Combo["NoAA"].Cast<CheckBox>().CurrentValue)
            {
                args.Process = false;
            }
        }

        internal static bool IsIsolated(Obj_AI_Base target)
        {
            return
                !ObjectManager.Get<Obj_AI_Base>()
                     .Any(
                         x =>
                         x.NetworkId != target.NetworkId && x.Team == target.Team && x.Distance(target) <= 500
                         && (x.Type == GameObjectType.AIHeroClient || x.Type == GameObjectType.obj_AI_Minion
                             || x.Type == GameObjectType.obj_AI_Turret));
        }

        internal static double GetQDamage(Obj_AI_Base target)
        {
            if (Q.Range < 326)
            {
                return 0.984
                       * player.GetSpellDamage(
                           target,
                           SpellSlot.Q,
                           (DamageLibrary.SpellStages)(IsIsolated(target) ? 1 : 0));
            }

            if (Q.Range > 325)
            {
                var isolated = IsIsolated(target);
                if (isolated)
                {
                    return 0.984 * player.GetSpellDamage(target, SpellSlot.Q, (DamageLibrary.SpellStages)3);
                }

                return player.GetSpellDamage(target, SpellSlot.Q, 0);
            }

            return 0;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!EvolvedQ && player.HasBuff("khazixqevo"))
            {
                Q.Range = 375;
                EvolvedQ = true;
            }

            if (!EvolvedW && player.HasBuff("khazixwevo"))
            {
                W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 225, 828, 100);
                EvolvedW = true;
            }

            if (!EvolvedE && player.HasBuff("khazixeevo"))
            {
                E.Range = 1000;
                EvolvedE = true;
            }

            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass)
                && menu.Mana["harass"].Cast<Slider>().CurrentValue < player.ManaPercent)
            {
                Harass.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                && menu.Mana["lane"].Cast<Slider>().CurrentValue < player.ManaPercent)
            {
                Clear.LaneClear();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LastHit)
                && menu.Mana["last"].Cast<Slider>().CurrentValue < player.ManaPercent)
            {
                Clear.LastHit();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                && menu.Mana["jungle"].Cast<Slider>().CurrentValue < player.ManaPercent)
            {
                Clear.JungleClear();
            }

            KillSteal.Steal();
        }
    }
}