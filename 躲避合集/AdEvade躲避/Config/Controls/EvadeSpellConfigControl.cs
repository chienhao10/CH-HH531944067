using AdEvade.Data.EvadeSpells;
using EloBuddy.SDK.Menu;

namespace AdEvade.Config.Controls
{
    public class EvadeSpellConfigControl
    {
        public DynamicCheckBox UseSpellCheckBox;
        public StringSlider DangerLevelSlider;
        public StringSlider SpellModeSlider;

        public static readonly string[] SpellModes = {"无法躲避", "激活时间", "一直"};

        private readonly Menu _menu;
        public EvadeSpellConfigControl(Menu menu, string menuName, EvadeSpellData spell )
        {

            UseSpellCheckBox = new DynamicCheckBox(ConfigDataType.EvadeSpell, spell.Name, "使用技能", true, true, SpellConfigProperty.UseEvadeSpell);
            DangerLevelSlider = new StringSlider(ConfigDataType.EvadeSpell, spell.Name, "危险等级", (int) spell.Dangerlevel, SpellConfigProperty.DangerLevel, SpellConfigControl.DangerLevels);
            SpellModeSlider = new StringSlider(ConfigDataType.EvadeSpell, spell.Name, "技能模式", (int)EvadeSpell.GetDefaultSpellMode(spell), SpellConfigProperty.SpellMode, SpellModes);
            menu.AddGroupLabel(menuName);
            menu.Add(spell.Name + "使用技能躲避", UseSpellCheckBox.CheckBox);
            menu.Add(spell.Name + "技能躲避危险等级", DangerLevelSlider.Slider.Slider);
            menu.Add(spell.Name + "技能躲避模式", SpellModeSlider.Slider.Slider);
            Properties.SetEvadeSpell(spell.Name, new EvadeSpellConfig { DangerLevel = spell.Dangerlevel, Use = true, SpellMode = EvadeSpell.GetDefaultSpellMode(spell) });
        }

        public Menu GetMenu()
        {
            return _menu;
        }
    }
}