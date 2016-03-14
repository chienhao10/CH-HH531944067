using EloBuddy;
using EloBuddy.SDK;

namespace ScaryKalista
{
    using System;
    using System.Linq;
    using Color = System.Drawing.Color;

    public class DamageIndicator
    {
        private static int _height;
        private static int _width;
        private static int _xOffset;
        private static int _yOffset;

        public static Color EnemyColor = Color.Lime;
        public static Color JungleColor = Color.White;

        private static DamageToUnitDelegate _damageToUnit;
        public delegate float DamageToUnitDelegate(Obj_AI_Base minion);

        public static DamageToUnitDelegate DamageToUnit
        {
            get
            {
                return _damageToUnit;
            }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnEndScene += OnEndScene;
                }

                _damageToUnit = value;
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (_damageToUnit == null) return;

            if (Config.DrawMenu.IsChecked("draw.enemyE"))
            {
                foreach (var hero in EntityManager.Heroes.Enemies
                    .Where(x => x.IsValidTarget()
                            && x.IsHPBarRendered
                            && x.HasRendBuff()))
                {
                    _height = 9;
                    _width = 104;
                    _xOffset = hero.ChampionName == "Jhin" ? -9 : 2;
                    _yOffset = hero.ChampionName == "Jhin" ? -5 : 9;

                    DrawLine(hero);
                }
            }
        
            if (Config.DrawMenu.IsChecked("draw.jungleE"))
            {
                foreach (
                var unit in
                EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Spells.E.Range)
                        .Where(x => x.IsValidTarget()
                                    && x.IsHPBarRendered
                                    && x.HasRendBuff()))
                {
                    if ((unit.Name.Contains("Blue") || unit.Name.Contains("Red")) && !unit.Name.Contains("Mini"))
                    {
                        _height = 9;
                        _width = 142;
                        _xOffset = -4;
                        _yOffset = 7;
                    }
                    else if (unit.Name.Contains("Dragon"))
                    {
                        _height = 10;
                        _width = 143;
                        _xOffset = -4;
                        _yOffset = 8;
                    }
                    else if (unit.Name.Contains("Baron"))
                    {
                        _height = 12;
                        _width = 191;
                        _xOffset = -29;
                        _yOffset = 6;
                    }
                    else if (unit.Name.Contains("Herald"))
                    {
                        _height = 10;
                        _width = 142;
                        _xOffset = -4;
                        _yOffset = 7;
                    }
                    else if ((unit.Name.Contains("Razorbeak") || unit.Name.Contains("Murkwolf")) && !unit.Name.Contains("Mini"))
                    {
                        _width = 74;
                        _height = 3;
                        _xOffset = 30;
                        _yOffset = 7;
                    }
                    else if (unit.Name.Contains("Krug") && !unit.Name.Contains("Mini"))
                    {
                        _width = 80;
                        _height = 3;
                        _xOffset = 27;
                        _yOffset = 7;
                    }
                    else if (unit.Name.Contains("Gromp"))
                    {
                        _width = 86;
                        _height = 3;
                        _xOffset = 24;
                        _yOffset = 6;
                    }
                    else if (unit.Name.Contains("Crab"))
                    {
                        _width = 61;
                        _height = 2;
                        _xOffset = 36;
                        _yOffset = 21;
                    }
                    else if (unit.Name.Contains("RedMini") || unit.Name.Contains("BlueMini") || unit.Name.Contains("RazorbeakMini"))
                    {
                        _height = 2;
                        _width = 49;
                        _xOffset = 42;
                        _yOffset = 6;
                    }
                    else if (unit.Name.Contains("KrugMini") || unit.Name.Contains("MurkwolfMini"))
                    {
                        _height = 2;
                        _width = 55;
                        _xOffset = 39;
                        _yOffset = 6;
                    }
                    else
                    {
                        continue;
                    }

                    DrawLine(unit);
                }
            }
        }

        private static void DrawLine(Obj_AI_Base unit)
        {
            var damage = _damageToUnit(unit);
            if (damage <= 0) return;

            var barPos = unit.HPBarPosition;

            //Get remaining HP after damage applied in percent and the current percent of health
            var percentHealthAfterDamage = Math.Max(0, unit.GetTotalHealth() - damage) / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
            var currentHealthPercentage = unit.GetTotalHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

            //Calculate start and end point of the bar indicator
            var startPoint = barPos.X + _xOffset + (percentHealthAfterDamage * _width);
            var endPoint = barPos.X + _xOffset + (currentHealthPercentage * _width);
            var yPos = barPos.Y + _yOffset;

            //Draw the line
            Drawing.DrawLine(startPoint, yPos, endPoint, yPos, _height, unit is AIHeroClient ? EnemyColor : JungleColor);
        }
    }
}
