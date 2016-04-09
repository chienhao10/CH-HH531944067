using EloBuddy;
using EloBuddy.SDK.Menu;
using Jhin;
using Jhin.Utilities;

namespace Jhin.Managers
{
    public static class DrawingsManager
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Drawings"); }
        }

        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                if (AIO.MyHero.IsDead || Menu.CheckBox("Disable"))
                {
                    return;
                }
                AIO.CurrentChampion.OnDraw();
                CircleManager.Draw();
                ToggleManager.Draw();
            };
            Drawing.OnEndScene += delegate
            {
                if (AIO.MyHero.IsDead || Menu.CheckBox("Disable"))
                {
                    return;
                }
                AIO.CurrentChampion.OnEndScene();
                if (Menu.CheckBox("DamageIndicator"))
                {
                    DamageIndicator.Draw();
                }
            };
        }
    }
}