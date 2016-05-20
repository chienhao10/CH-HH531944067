namespace iTwitch
{
    using System;
    using System.Linq;

    using EloBuddy;

    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    static class Extensions
    {
        #region Public Methods and Operators

        public static void DrawTextOnScreen(this Vector3 location, string message, Color colour)
        {
            var worldToScreen = Drawing.WorldToScreen(location);
            Drawing.DrawText(worldToScreen[0] - message.Length * 5, worldToScreen[1] - 200, colour, message);
        }

        public static float GetPoisonDamage(Obj_AI_Base target)
        {
            if (target == null || !target.HasBuff("TwitchDeadlyVenom")) return 0;

            var baseDamage = Twitch.Spells[SpellSlot.E].GetDamage(target);

            if (target is AIHeroClient)
            {
                if (target.HasUndyingBuff() || target.Health < 1 || target.HasBuffOfType(BuffType.SpellShield)) return 0;

                if (target.HasBuff("meditate"))
                {
                    baseDamage *= 0.5f - 0.05f * target.Spellbook.GetSpell(SpellSlot.W).Level;
                }
            }

            if (target is Obj_AI_Minion)
            {
                if (target.Name.Contains("Baron") && ObjectManager.Player.HasBuff("barontarget"))
                {
                    baseDamage *= 0.5f;
                }

                if (target.Name.Contains("Dragon") && ObjectManager.Player.HasBuff("s5test_dragonslayerbuff"))
                {
                    baseDamage *= 1f - 0.07f * ObjectManager.Player.GetBuffCount("s5test_dragonslayerbuff");

                    // todo new dragons? :S
                }
            }

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
            {
                baseDamage *= 0.55f;
            }

            return baseDamage;
        }

        public static float GetPoisonStacks(this Obj_AI_Base target)
        {
            return target.GetBuffCount("TwitchDeadlyVenom");
        }

        public static float GetRealHealth(this Obj_AI_Base target)
        {
            return target.AttackShield > 0 ? target.Health + target.AttackShield : target.Health;
        }

        public static float GetRemainingBuffTime(this Obj_AI_Base target, string buffName)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => string.Equals(buff.Name, buffName, StringComparison.CurrentCultureIgnoreCase))
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault() - Game.Time;
        }

        public static bool HasUndyingBuff(this Obj_AI_Base target1)
        {
            var target = target1 as AIHeroClient;

            if (target == null) return false;

            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValid && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            // TODO poppy and jihn
            return false;
        }

        public static bool IsPoisonKillable(this Obj_AI_Base target)
        {
            return GetPoisonDamage(target) >= GetRealHealth(target);
        }

        #endregion
    }
}