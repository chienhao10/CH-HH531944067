using EloBuddy.SDK.Menu.Values;

namespace AdEvade.Config.Controls
{
    public interface IDynamicControl<T>
    {
        ConfigValue GetConfigValue();
        object GetValue();
        ValueBase<T> GetControl();
    }
}