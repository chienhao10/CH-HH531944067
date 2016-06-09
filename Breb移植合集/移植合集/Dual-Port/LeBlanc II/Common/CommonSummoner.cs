using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace Leblanc.Common
{
    internal class CommonSummoner
    {
        public static SpellSlot SmiteSlot = SpellSlot.Unknown;
        public static SpellSlot IgniteSlot = SpellSlot.Unknown;
        public static SpellSlot FlashSlot = SpellSlot.Unknown;
        public static SpellSlot TeleportSlot = ObjectManager.Player.GetSpellSlot("SummonerTeleport");

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "s5_summonersmiteplayerganker";
                }

                if (SmiteRed.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "s5_summonersmiteduel";
                }

                if (SmiteGrey.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "s5_summonersmitequick";
                }

                if (SmitePurple.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "itemsmiteaoe";
                }

                return "summonersmite";
            }
        }

        public static void Init()
        {
            SetSmiteSlot();
            if (SmiteSlot != SpellSlot.Unknown)
            {
                Modes.ModeConfig.MenuConfig.Add("Spells.Smite", new CheckBox("Use Smite to Enemy!"));
            }

            SetIgniteSlot();
            if (IgniteSlot != SpellSlot.Unknown)
            {
                Modes.ModeConfig.MenuConfig.Add("Spells.Ignite", new CheckBox("Use Ignite!"));
            }

            SetFlatSlot();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                UseSpells();
        }

        private static void UseSpells()
        {
            var t = TargetSelector.GetTarget(Champion.PlayerSpells.Q.Range, DamageType.Magical);

            if (!t.IsValidTarget())
                return;

            if (SmiteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready)
            {
                //SmiteOnTarget(t);
            }

            if (IgniteSlot != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > t.Health
                    && ObjectManager.Player.LSDistance(t) <= 500)
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }
        }

        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => string.Equals(spell.Name, Smitetype, StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
            }
        }

        private static void SetIgniteSlot()
        {
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }

        private static void SetFlatSlot()
        {
            FlashSlot = ObjectManager.Player.GetSpellSlot("SummonerFlash");
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }


        private static void SmiteOnTarget(AIHeroClient t)
        {
            var range = 700f;
            var use = getCheckBoxItem(Modes.ModeConfig.MenuConfig, "Spells.Smite");
            var itemCheck = SmiteBlue.Any(i => LeagueSharp.Common.Items.HasItem(i)) ||
                            SmiteRed.Any(i => LeagueSharp.Common.Items.HasItem(i));
            if (itemCheck && use &&
                ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready &&
                t.LSDistance(ObjectManager.Player.Position) < range)
            {
                ObjectManager.Player.Spellbook.CastSpell(SmiteSlot, t);
            }
        }

        private static void IgniteOnTarget(AIHeroClient t)
        {
            var range = 550f;
            var use = getCheckBoxItem(Modes.ModeConfig.MenuConfig, "Spells.Ignite");
            if (use && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                t.LSDistance(ObjectManager.Player.Position) < range &&
                ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > t.Health)
            {
                ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, t);
            }
        }
    }
}