namespace AutoSteal.Misc
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Genesis.Library;
    using Genesis.Library.Spells;

    internal class Damage
    {
        protected static SpellBase Spells => SpellManager.CurrentSpells;

        public static int Jsadd;

        public static int Ksadd;

        internal static int JsCalcDamage(Obj_AI_Minion mob)
        {
            var aa = ObjectManager.Player.GetAutoAttackDamage(mob, true) * 1.5;
            var damage = aa;

            if (Spells.Q.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "QJ"].Cast<CheckBox>().CurrentValue)
                // Qdamage
            {
                damage += ObjectManager.Player.GetSpellDamage(mob, SpellSlot.Q);
            }

            if (Spells.W.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "WJ"].Cast<CheckBox>().CurrentValue)
                // Wdamage
            {
                damage += ObjectManager.Player.GetSpellDamage(mob, SpellSlot.W);
            }

            if (Spells.E.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "EJ"].Cast<CheckBox>().CurrentValue)
                // Edamage
            {
                damage += ObjectManager.Player.GetSpellDamage(mob, SpellSlot.E);
            }

            if (Spells.R.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "RJ"].Cast<CheckBox>().CurrentValue)
                // Rdamage
            {
                damage += ObjectManager.Player.GetSpellDamage(mob, SpellSlot.R);
            }

            return (int)damage;
        }

        internal static float JsTravelTime(Obj_AI_Minion mob)
        {
            var tt = mob.Distance(ObjectManager.Player);
            var travelime = tt;

            if (Spells.Q.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "QJ"].Cast<CheckBox>().CurrentValue)
                // Qdamage
            {
                travelime += tt / (Spells.Q.Handle.SData.MissileSpeed + Spells.Q.CastDelay) + Game.Ping / 2f / 1000;
                Jsadd += 1;
            }

            if (Spells.W.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "WJ"].Cast<CheckBox>().CurrentValue)
                // Wdamage
            {
                travelime += tt / (Spells.W.Handle.SData.MissileSpeed + Spells.W.CastDelay) + Game.Ping / 2f / 1000;
                Jsadd += 1;
            }

            if (Spells.E.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "EJ"].Cast<CheckBox>().CurrentValue)
                // Edamage
            {
                travelime += tt / (Spells.E.Handle.SData.MissileSpeed + Spells.E.CastDelay) + Game.Ping / 2f / 1000;
                Jsadd += 1;
            }

            if (Spells.R.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "RJ"].Cast<CheckBox>().CurrentValue)
                // Rdamage
            {
                travelime += tt / (Spells.R.Handle.SData.MissileSpeed + Spells.R.CastDelay) + Game.Ping / 2f / 1000;
                Jsadd += 1;
            }

            return (float)travelime / Jsadd;
        }

        internal static int KsCalcDamage(AIHeroClient target)
        {
            var aa = ObjectManager.Player.GetAutoAttackDamage(target, true) * 1.5;
            var damage = aa;

            if (Spells.Q.IsReady()
                && Program.KillStealMenu[Player.Instance.ChampionName + "QC"].Cast<CheckBox>().CurrentValue) // Qdamage
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (Spells.W.IsReady()
                && Program.KillStealMenu[Player.Instance.ChampionName + "WC"].Cast<CheckBox>().CurrentValue) // Wdamage
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }

            if (Spells.E.IsReady()
                && Program.KillStealMenu[Player.Instance.ChampionName + "EC"].Cast<CheckBox>().CurrentValue) // Edamage
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }

            if (Spells.R.IsReady()
                && Program.KillStealMenu[Player.Instance.ChampionName + "RC"].Cast<CheckBox>().CurrentValue) // Rdamage
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);
            }

            return (int)damage;
        }

        internal static float KsTravelTime(AIHeroClient target)
        {
            var tt = target.Distance(ObjectManager.Player);
            var travelime = tt;

            if (Spells.Q.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "QJ"].Cast<CheckBox>().CurrentValue)
                // Qdamage
            {
                travelime += tt / (Spells.Q.Handle.SData.MissileSpeed + Spells.Q.CastDelay) + Game.Ping / 2f / 1000;
                Ksadd += 1;
            }

            if (Spells.W.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "WJ"].Cast<CheckBox>().CurrentValue)
                // Wdamage
            {
                travelime += tt / (Spells.W.Handle.SData.MissileSpeed + Spells.W.CastDelay) + Game.Ping / 2f / 1000;
                Ksadd += 1;
            }

            if (Spells.E.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "EJ"].Cast<CheckBox>().CurrentValue)
                // Edamage
            {
                travelime += tt / (Spells.E.Handle.SData.MissileSpeed + Spells.E.CastDelay) + Game.Ping / 2f / 1000;
                Ksadd += 1;
            }

            if (Spells.R.IsReady()
                && Program.JungleStealMenu[Player.Instance.ChampionName + "RJ"].Cast<CheckBox>().CurrentValue)
                // Rdamage
            {
                travelime += tt / (Spells.R.Handle.SData.MissileSpeed + Spells.R.CastDelay) + Game.Ping / 2f / 1000;
                Ksadd += 1;
            }

            return (float)travelime / Ksadd;
        }
    }
}