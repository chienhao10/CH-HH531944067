using AdEvade.Config;
using AdEvade.Config.Controls;
using EloBuddy.SDK.Menu.Values;

namespace AdEvade.Helpers
{
    public static class MenuHelpers
    {
        public static ValueBase<T> Add<T>(this EloBuddy.SDK.Menu.Menu menu, IDynamicControl<T> item)
        {
            return menu.Add(item.GetConfigValue().Name(), item.GetControl());
        } 
    }
}