using System.Collections.Generic;
using EloBuddy;

namespace KA_Lux.DMGHandler
{
    public class DangerousSpell
    {
        public DangerousSpell(Champion _hero, SpellSlot slot, bool defaultvalue)
        {
            Slot = slot;
            Hero = _hero;
            DefaultValue = defaultvalue;
        }

        public SpellSlot Slot;
        public Champion Hero;
        public bool DefaultValue;
    }

    public class DangerousSpells
    {
        public static List<DangerousSpell> Spells = new List<DangerousSpell>
        {
            new DangerousSpell(Champion.Ahri, SpellSlot.E, false),
            new DangerousSpell(Champion.Amumu, SpellSlot.R, true),
            new DangerousSpell(Champion.Annie, SpellSlot.R, true),
            new DangerousSpell(Champion.Ashe, SpellSlot.R, true),
            new DangerousSpell(Champion.Azir, SpellSlot.R, true),
            new DangerousSpell(Champion.Brand, SpellSlot.R, true),
            new DangerousSpell(Champion.Braum, SpellSlot.R, true),
            new DangerousSpell(Champion.Caitlyn, SpellSlot.R, true),
            new DangerousSpell(Champion.Cassiopeia, SpellSlot.R, true),
            new DangerousSpell(Champion.Chogath, SpellSlot.R, true),
            new DangerousSpell(Champion.Darius, SpellSlot.R, true),
            new DangerousSpell(Champion.Draven, SpellSlot.R, true),
            new DangerousSpell(Champion.Ekko, SpellSlot.R, true),
            new DangerousSpell(Champion.Evelynn, SpellSlot.R, false),
            new DangerousSpell(Champion.Ezreal, SpellSlot.R, true),
            new DangerousSpell(Champion.FiddleSticks, SpellSlot.R, true),
            new DangerousSpell(Champion.Fiora, SpellSlot.R, true),
            new DangerousSpell(Champion.Fizz, SpellSlot.R, true),
            new DangerousSpell(Champion.Galio, SpellSlot.R, true),
            new DangerousSpell(Champion.Garen, SpellSlot.R, true),
            new DangerousSpell(Champion.Gnar, SpellSlot.R, true),
            new DangerousSpell(Champion.Gragas, SpellSlot.R, true),
            new DangerousSpell(Champion.Graves, SpellSlot.R, true),
            new DangerousSpell(Champion.Hecarim, SpellSlot.R, true),
            new DangerousSpell(Champion.Illaoi, SpellSlot.R, true),
            new DangerousSpell(Champion.JarvanIV, SpellSlot.R, true),
            new DangerousSpell(Champion.Jinx, SpellSlot.R, true),
            new DangerousSpell(Champion.Kalista, SpellSlot.E, false),
            new DangerousSpell(Champion.Karthus, SpellSlot.R, true),
            new DangerousSpell(Champion.Kassadin, SpellSlot.R, false),
            new DangerousSpell(Champion.Katarina, SpellSlot.R, true),
            new DangerousSpell(Champion.Kennen, SpellSlot.R, true),
            new DangerousSpell(Champion.Leblanc, SpellSlot.R, true),
            new DangerousSpell(Champion.LeeSin, SpellSlot.R, true),
            new DangerousSpell(Champion.Leona, SpellSlot.R, true),
            new DangerousSpell(Champion.Lissandra, SpellSlot.R, true),
            new DangerousSpell(Champion.Lux, SpellSlot.R, true),
            new DangerousSpell(Champion.Malphite, SpellSlot.R, true),
            new DangerousSpell(Champion.Malzahar, SpellSlot.R, true),
            new DangerousSpell(Champion.MissFortune, SpellSlot.R, true),
            new DangerousSpell(Champion.MonkeyKing, SpellSlot.R, true),
            new DangerousSpell(Champion.Mordekaiser, SpellSlot.R, true),
            new DangerousSpell(Champion.Morgana, SpellSlot.R, true),
            new DangerousSpell(Champion.Nami, SpellSlot.R, true),
            new DangerousSpell(Champion.Nautilus, SpellSlot.R, true),
            new DangerousSpell(Champion.Nocturne, SpellSlot.R, true),
            new DangerousSpell(Champion.Nunu, SpellSlot.R, true),
            new DangerousSpell(Champion.Orianna, SpellSlot.R, true),
            new DangerousSpell(Champion.Poppy, SpellSlot.R, true),
            new DangerousSpell(Champion.Riven, SpellSlot.R, true),
            new DangerousSpell(Champion.Rumble, SpellSlot.R, true),
            new DangerousSpell(Champion.Sejuani, SpellSlot.R, true),
            new DangerousSpell(Champion.Skarner, SpellSlot.R, true),
            new DangerousSpell(Champion.Sona, SpellSlot.R, true),
            new DangerousSpell(Champion.Syndra, SpellSlot.R, true),
            new DangerousSpell(Champion.Talon, SpellSlot.R, true),
            new DangerousSpell(Champion.Tristana, SpellSlot.R, true),
            new DangerousSpell(Champion.Trundle, SpellSlot.R, false),
            new DangerousSpell(Champion.Varus, SpellSlot.R, true),
            new DangerousSpell(Champion.Veigar, SpellSlot.R, true),
            new DangerousSpell(Champion.Velkoz, SpellSlot.R, true),
            new DangerousSpell(Champion.Vi, SpellSlot.R, true),
            new DangerousSpell(Champion.Viktor, SpellSlot.R, true),
            new DangerousSpell(Champion.Vladimir, SpellSlot.R, true),
            new DangerousSpell(Champion.Warwick, SpellSlot.R, true),
            new DangerousSpell(Champion.Yasuo, SpellSlot.R, true),
            new DangerousSpell(Champion.Zed, SpellSlot.R, true),
            new DangerousSpell(Champion.Zyra, SpellSlot.R, true),
        };
    }
}
