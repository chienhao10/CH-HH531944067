using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Mordekaiser.Logics
{
    public class LogicW
    {
        public static void Initiate()
        {
            Game.OnUpdate += GameOnUpdate;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private static void GameOnUpdate(EventArgs args)
        {
            if (Utils.Player.Self.Spellbook.GetSpell(SpellSlot.W).Name != "mordekaisercreepingdeath2")
                return;

            var countEnemy = Utils.Player.Self.CountAlliesInRange(Spells.WDamageRadius);

            if (countEnemy == 0)
                return;

            var t = TargetSelector.GetTarget(Spells.WDamageRadius, DamageType.Magical);
            if (!t.IsValidTarget())
                return;

            var targetMovementSpeed = t.MoveSpeed;
            var myMovementSpeed = Utils.Player.Self.MoveSpeed;

            if (myMovementSpeed <= targetMovementSpeed)
            {
                if (!t.IsFacing(Utils.Player.Self) && t.Path.Count() >= 1 &&
                    t.LSDistance(Utils.Player.Self) > Spells.WDamageRadius - 20)
                {
                    Spells.W.Cast();
                }
            }
        }

        public static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (args.SData.Name == ((AIHeroClient) sender).GetSpell(SpellSlot.W).Name)
            {
                Spells.WCastedTime = ((AIHeroClient) sender).GetSpell(SpellSlot.W).Name == "mordekaisercreepingdeath2" ? Environment.TickCount : 0;
            }
        }
    }
}