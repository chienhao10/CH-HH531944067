using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace OneForWeek.Util.Misc
{
    internal class Igniter
    {
        public static Spell.Targeted Ignite;
        public static Menu IgniteMenu;

        public static void Init()
        {
            if (ObjectManager.Player.GetSpellSlotFromName("summonerdot") == SpellSlot.Unknown) return;

            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 550);

            IgniteMenu = MainMenu.AddMenu("Igniter", "点燃助手");
            IgniteMenu.AddGroupLabel("线圈");
            IgniteMenu.Add("useIgnite", new CheckBox("使用点燃", true));
            IgniteMenu.AddGroupLabel("杂项");
            IgniteMenu.Add("minRange", new Slider("最近点燃距离: ", 400, 1, 550));
            IgniteMenu.Add("drawRange", new CheckBox("最远点燃距离", false));

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (!Misc.IsChecked(IgniteMenu, "drawRange")) return;

            Circle.Draw(Ignite.IsReady() ? Color.Blue : Color.Red, Ignite.Range, Player.Instance.Position);
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || !Ignite.IsReady() || !Misc.IsChecked(IgniteMenu, "useIgnite")) return;

            var target2 = ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(h => h.IsValidTarget(Ignite.Range) && h.Distance(Player.Instance) >= Misc.GetSliderValue(IgniteMenu, "minRange") && h.Health < ObjectManager.Player.GetSummonerSpellDamage(h, DamageLibrary.SummonerSpells.Ignite));

            if (target2.IsValidTarget(Ignite.Range))
                Ignite.Cast(target2);
        }
    }
}
