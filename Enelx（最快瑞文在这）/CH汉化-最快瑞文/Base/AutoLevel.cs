using System;
using EloBuddy;
using EloBuddy.SDK;

namespace Ex1L_Riven.Base
{
    internal class AutoLevel
    {
        private static int[] _sequence;

        static AutoLevel()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Variables.UseAutoLevel && Player.Instance.SpellTrainingPoints >= 1)
            {
                var spellpoints = Player.Instance.SpellTrainingPoints;

                switch (Variables.SelectedLevel)
                {
                    case "Q > E > W":
                        _sequence = new[] {1, 3, 2, 1, 1, 4, 3, 1, 3, 2, 4, 1, 3, 2, 3, 4, 2, 2};
                        break;
                    case "Q > W > E":
                        _sequence = new[] {1, 2, 3, 1, 1, 4, 2, 1, 2, 3, 4, 1, 2, 3, 2, 4, 3, 3};
                        break;
                    case "E > Q > W":
                        _sequence = new[] {3, 1, 2, 3, 3, 4, 1, 3, 1, 2, 4, 3, 1, 2, 1, 4, 2, 2};
                        break;
                }

                while (spellpoints >= 1)
                {
                    var skill = _sequence[Player.Instance.Level - spellpoints];

                    switch (skill)
                    {
                        case 1:
                            Core.DelayAction(delegate { Player.Instance.Spellbook.LevelSpell(SpellSlot.Q); },
                                Variables.LevelDelay);
                            break;
                        case 2:
                            Core.DelayAction(delegate { Player.Instance.Spellbook.LevelSpell(SpellSlot.W); },
                                Variables.LevelDelay);
                            break;
                        case 3:
                            Core.DelayAction(delegate { Player.Instance.Spellbook.LevelSpell(SpellSlot.E); },
                                Variables.LevelDelay);
                            break;
                        case 4:
                            Core.DelayAction(delegate { Player.Instance.Spellbook.LevelSpell(SpellSlot.R); },
                                Variables.LevelDelay);
                            break;
                    }
                    spellpoints--;
                }
            }
        }

        public static void Initialize()
        {
        }
    }
}