using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;

namespace Twitch
{
    class Damages
    {
        public static float ERawDamage(Obj_AI_Base target)
        {
            var stacks = SpellManager.EStacks(target);
            if (stacks <= 0)
            {
                return 0.0f;
            }
            return
                (int)
                    (new int[] { 20, 35, 50, 65, 80 }[SpellManager.E.Level - 1]) +
                     stacks * (new int[] { 15, 20, 25, 30, 35 }[SpellManager.E.Level - 1] + 0.2f * Player.Instance.TotalMagicalDamage + 0.25f * (Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage));
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, ERawDamage(target)) *
                   (Player.Instance.HasBuff("SummonerExhaustSlow") ? 0.6f : 1);
        }

        public static float IgniteDmg(Obj_AI_Base target)
        {
            return Player.Instance.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
        }

        #region DamageIndicator

        //Offsets
        private const float YOff = 2;
        private const float XOff = 1;
        private const float Width = 107;
        private const float Thick = 6;
        //Offsets
        private static Font _Font, _Font2;
        private static Color color = Color.Yellow;

        public static void InitDamageIndicator()
        {
            Drawing.OnEndScene += Drawing_OnEndScene;

            _Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                });

            _Font2 = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                });
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(e => e.IsValid && e.IsHPBarRendered && e.TotalShieldHealth() > 10)
                )
            {
                var damage = GetTotalDamage(enemy);

                if (Config.MiscMenu.ShowDamageIndicator)
                {
                    //Drawing Line Over Enemies Helth bar
                    var dmgPer = (enemy.TotalShieldHealth() - damage > 0 ? enemy.TotalShieldHealth() - damage : 0) /
                                 enemy.TotalShieldMaxHealth();
                    var currentHPPer = enemy.TotalShieldHealth() / enemy.TotalShieldMaxHealth();
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width),
                        (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1,
                        (int)enemy.HPBarPosition.Y + YOff);

                    EloBuddy.SDK.Rendering.Line.DrawLine(System.Drawing.Color.Red, Thick, initPoint, endPoint);
                }

                if (Config.MiscMenu.ShowStats)
                {
                    //Statistics
                    var posXStat = (int)enemy.HPBarPosition[0];
                    var posYStat = (int)enemy.HPBarPosition[1] - 7;
                    var mathStat = "-" + Math.Round(damage) + " / " +
                                   Math.Round(enemy.Health - damage);
                    _Font2.DrawText(null, mathStat, posXStat, posYStat, SharpDX.Color.Yellow);
                }

                if (Config.MiscMenu.ShowPercentage)
                {
                    //Percent
                    var posXPer = (int)enemy.HPBarPosition[0] + 106;
                    var posYPer = (int)enemy.HPBarPosition[1] - 12;
                    _Font.DrawText(null, string.Concat(Math.Ceiling((int)damage / enemy.TotalShieldHealth() * 100), "%"),
                        posXPer, posYPer, SharpDX.Color.Yellow);
                }
            }
        }

        private static float GetTotalDamage(Obj_AI_Base target)
        {
            var spells =
                Player.Spells.Where(
                    spell =>
                        (spell.Slot == SpellSlot.Q && Config.MiscMenu.CalculateQ) ||
                          (spell.Slot == SpellSlot.W && Config.MiscMenu.CalculateW) ||
                            (spell.Slot == SpellSlot.R && Config.MiscMenu.CalculateR));

            var damage = spells.Where(spe => spe.IsReady).Sum(s => Player.Instance.GetSpellDamage(target, s.Slot));

            return (damage + Player.Instance.GetAutoAttackDamage(target)) - 10;
        }
    }

    #endregion DamageIndicator
}
