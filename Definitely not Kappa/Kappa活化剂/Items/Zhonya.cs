namespace KappaUtility.Items
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using Common;

    internal class Zhonya
    {
        private static int castdelay = 50;

        private static readonly string[] DangerSpells =
            {
                "AzirR", "zedult", "ViR", "SyndraR", "CaitlynAceintheHole", "LissandraR", "GarenR",
                "DariusR", "BlindMonkRKick", "FallenOne"
            };

        internal static void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if (!Defensive.Zhonyas.IsOwned(Player.Instance) || !Defensive.Zhonyas.IsReady()
                || (!(caster is AIHeroClient) || target == null || !target.IsMe))
            {
                return;
            }

            if (Defensive.DefMenu.GetCheckbox("ZhonyasD"))
            {
                var Dangerspell = DangerSpells.FirstOrDefault(spell => args.SData.Name == spell && caster.IsEnemy);
                if (Dangerspell != null)
                {
                    Core.DelayAction(() => Defensive.Zhonyas.Cast(), (int)sender.Spellbook.GetSpell(SpellSlot.R).SData.SpellCastTime * 2);
                }
            }
        }
    }
}