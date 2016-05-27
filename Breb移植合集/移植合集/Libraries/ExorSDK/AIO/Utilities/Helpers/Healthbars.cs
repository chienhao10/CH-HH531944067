using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.Data.Enumerations;
using EloBuddy;

namespace ExorSDK.Utilities
{
    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Healthbars
    {
        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        /// 

            /*
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                ObjectManager.Get<Obj_AI_Base>().Where(
                    h =>
                        !h.IsMe &&
                        h.LSIsValid() &&
                        Bools.IsPerfectRendTarget(h) &&
                        !h.CharData.BaseSkinName.Contains("Mini") &&
                        !h.CharData.BaseSkinName.Contains("Minion")).ForEach(unit =>
                    {
                        /// <summary>
                        ///     Defines what HPBar Offsets it should display.
                        /// </summary>
                        var mobOffset = Vars.JungleHpBarOffsetList.FirstOrDefault(x => x.BaseSkinName.Equals(unit.CharData.BaseSkinName));

                        var width = (int)(unit is Obj_AI_Minion ? mobOffset.Width : Vars.Width);
                        var height = (int)(unit is Obj_AI_Minion ? mobOffset.Height : Vars.Height);
                        var xOffset = (int)(unit is Obj_AI_Minion ? mobOffset.XOffset: Vars.XOffset);
                        var yOffset = (int)(unit is Obj_AI_Minion ? mobOffset.YOffset : Vars.YOffset);

                        var barPos = unit.HPBarPosition;

                        barPos.X += xOffset;
                        barPos.Y += yOffset;

                        var drawEndXPos = barPos.X + width * (unit.HealthPercent / 100);
                        var drawStartXPos = barPos.X + (Vars.GetRealHealth(unit) >
                            (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E) +
                            (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E, DamageStage.Buff)
                                ? width * (((Vars.GetRealHealth(unit) -
                                    ((float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E) +
                                     (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E, DamageStage.Buff))) / unit.MaxHealth * 100) / 100)
                                : 0);

                        Drawing.DrawLine(drawStartXPos, barPos.Y, drawEndXPos, barPos.Y, height, Vars.GetRealHealth(unit) <
                            (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E) +
                            (float)GameObjects.Player.LSGetSpellDamage(unit, SpellSlot.E, DamageStage.Buff)
                                ? Color.Blue 
                                : Color.Orange);

                        Drawing.DrawLine(drawStartXPos, barPos.Y, drawStartXPos, barPos.Y + height + 1, 1, Color.Lime);
                    }
                );
            };
        }
        */
    }
}