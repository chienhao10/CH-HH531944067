using AdEvade.Data.Spells;
using EloBuddy.SDK.Menu;

namespace AdEvade.Config.Controls
{
    public class SpellConfigControl
    {

        public static readonly string[] DangerLevels = { "低", "正常", "高", "极端" };


        public DynamicCheckBox DodgeCheckBox;
        public DynamicCheckBox DrawCheckBox;
        public DynamicSlider SpellRadiusSlider;
        public StringSlider DangerLevelSlider;
        private readonly Menu _menu;
        private readonly SpellData _spell;
        public SpellConfigControl(Menu menu, string label, SpellData spell, bool enableSpell)
        {
            _menu = menu;
            _spell = spell;
            _menu.AddGroupLabel(label);

            DodgeCheckBox = new DynamicCheckBox(ConfigDataType.Spells, spell.SpellName, "躲避", enableSpell, true, SpellConfigProperty.Dodge);
            DrawCheckBox = new DynamicCheckBox(ConfigDataType.Spells, spell.SpellName, "显示", enableSpell, true, SpellConfigProperty.Draw);
            SpellRadiusSlider = new DynamicSlider(ConfigDataType.Spells, spell.SpellName, "半径", (int)spell.Radius, (int)spell.Radius - 100, (int)spell.Radius + 100, true, SpellConfigProperty.Radius);
            DangerLevelSlider = new StringSlider(ConfigDataType.Spells, spell.SpellName, "危险等级", (int) spell.Dangerlevel,SpellConfigProperty.DangerLevel, DangerLevels);
        }

        public void AddToMenu()
        {
            _menu.Add(_spell.SpellName + "_躲避", DodgeCheckBox.CheckBox);
            _menu.Add(_spell.SpellName + "_显示", DrawCheckBox.CheckBox);
            _menu.Add(_spell.SpellName + "_半径", SpellRadiusSlider.Slider);
            _menu.Add(_spell.SpellName + "_危险等级", DangerLevelSlider.Slider.Slider);
        }
    }
}