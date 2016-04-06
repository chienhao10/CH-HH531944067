using System;

namespace Vayne_Rot_Sec
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Program
    {
        public static Menu RotSec;

        public static readonly Item ZZrot = new Item(ItemId.ZzRot_Portal, 400);

        public static AIHeroClient Target;

        public static SpellDataInst E;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Vayne")
            {
                return;
            }

            RotSec = MainMenu.AddMenu("CH汉化-薇恩门", "Vayne Rot'Sec");
            RotSec.Add("Kappa", new KeyBind("薇恩门神E目标", false, KeyBind.BindTypes.HoldActive));
            RotSec.Add("delay", new Slider("延迟传送门使用 {0}毫秒", 100, 100, 250));
            RotSec.Add("Erange", new CheckBox("显示可放距离"));
            RotSec.Add("pred", new CheckBox("显示神E预判"));
            RotSec.AddSeparator();
            RotSec.AddGroupLabel("小贴士:");
            RotSec.AddLabel("选择目标后按下设定的按键就会进行放下传送门进行神E.");
            RotSec.AddLabel("只会在范围内使用，在自定义里面试试吧~！低调点哦 <3");

            E = Player.GetSpell(SpellSlot.E);
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (RotSec["Erange"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.MediumPurple, ZZrot.Range, Player.Instance.Position);
            }

            if (Target == null || Player.Instance.Distance(Target) > 1000)
            {
                return;
            }
            if (RotSec["pred"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawLine(
                    Drawing.WorldToScreen((Vector3)Target.Position.Extend(Player.Instance.Position, -100)),
                    Drawing.WorldToScreen((Vector3)Player.Instance.Position.Extend(Target.Position, -25)),
                    3,
                    System.Drawing.Color.White);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }

            Target = TargetSelector.SelectedTarget;
            if (Target == null || Player.Instance.Distance(Target) > 1000)
            {
                return;
            }
            if (RotSec["Kappa"].Cast<KeyBind>().CurrentValue && E.IsReady && ZZrot.IsReady()
                && Target.IsValidTarget(ZZrot.Range))
            {
                if (Player.CastSpell(SpellSlot.E, Target))
                {
                    Core.DelayAction(
                        () =>
                            {
                                ZZrot.Cast(
                                    Target.Position.To2D().Extend(Player.Instance.ServerPosition.To2D(), -100).To3D());
                            },
                        RotSec["delay"].Cast<Slider>().CurrentValue);
                }
            }
        }
    }
}