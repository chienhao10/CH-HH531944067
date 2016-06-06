using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
namespace KappaLeBlanc
{
    class Program : Helper
    {
        private static void Main(string[] args) { Loading.OnLoadingComplete += Game_OnStart; }
        public static readonly List<Slide> Slides = new List<Slide>();
        public static bool RavenForm;
        private static void Game_OnStart(EventArgs args)
        {
            if (myHero.Hero != Champion.Leblanc) return;

            CheckForUpdates();
            LBMenu.StartMenu();
            Lib.W.AllowedCollisionCount = int.MaxValue;
            DamageIndicator.Initialize(Extensions.GetComboDamage);
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawings;
            Gapcloser.OnGapcloser += Modes.AntiGapcloser.Execute;
            GameObject.OnCreate += ObjectOnCreate;
            GameObject.OnDelete += ObjectOnDelete;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (myHero.IsDead) return;

            Modes.Killsteal.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Modes.Combo.Execute();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                Modes.Laneclear.Execute();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                Modes.Flee.Execute();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || LBMenu.HSM["Auto"].Cast<KeyBind>().CurrentValue)
                Modes.Harass.Execute();

            if (CastCheckbox(LBMenu.Misc, "Clone"))
                Modes.CloneControl.Execute();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                    Lib.QlasTick = Environment.TickCount + 500;
                else if (args.Slot == SpellSlot.R && Lib.R.Name == "LeblancChaosOrbM")
                    Lib.QlasTick = Environment.TickCount + 500;
            }
        }

        private static void ObjectOnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == myHero.Name)
            {
                Clone = null;
            }
            if (sender.Name.Contains("displacement_blink_indicator"))
            {
                for (var i = 0; i < Slides.Count; i++)
                {
                    if (Slides[i].NetworkId == sender.NetworkId)
                    {
                        Slides.RemoveAt(i);
                        return;
                    }
                }
            }
            if (!(sender.Name.Contains("LeBlanc_Base_W_return_indicator.troy")))
                return;
            RavenForm = false;
        }
        private static void ObjectOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == myHero.Name)
            {
                Clone = sender;
            }
            if (sender.Name.Contains("LeBlanc_MirrorImagePoff.troy") || sender.Name.Contains("displacement_blink_indicator"))
            {
                Slides.Add(
                    new Slide
                    {
                        Object = sender,
                        Name = sender.Name,
                        NetworkId = sender.NetworkId,
                        Position = sender.Position,
                    });
            }
            if (!(sender.Name.Contains("LeBlanc_Base_W_return_indicator.troy")))
                return;
            RavenForm = true;
        }
        private static void Drawings(EventArgs args)
        {
            if (CastCheckbox(LBMenu.DrawM, "Q"))
            {
                new Circle() { Color = Lib.Q.IsReady() ? Color.White : Color.Tomato, Radius = Lib.Q.Range, BorderWidth = 2f }.Draw(myHero.Position);
            }
            if (CastCheckbox(LBMenu.DrawM, "W"))
            {
                new Circle() { Color = Lib.W.IsReady() ? Color.White : Color.Tomato, Radius = Lib.W.Range, BorderWidth = 2f }.Draw(myHero.Position);
            }
            if (CastCheckbox(LBMenu.DrawM, "E"))
            {
                new Circle() { Color = Lib.E.IsReady() ? Color.White : Color.Tomato, Radius = Lib.E.Range, BorderWidth = 2f }.Draw(myHero.Position);
            }

            var target = TargetSelector.GetTarget(Lib.E.Range, DamageType.Magical);
            if (target != null && !target.IsDead)
            {
                if (target.GetComboDamage() > target.Health)
                {
                    if (CastCheckbox(LBMenu.DrawM, "line"))
                    {
                        if (CastSlider(LBMenu.DrawM, "dist") >= myHero.Distance(target))
                        {
                            Drawing.DrawLine(myHero.ServerPosition.WorldToScreen(), target.ServerPosition.WorldToScreen(), 5, Color.Red);
                        }
                    }
                }
            }

            if (CastCheckbox(LBMenu.DrawM, "WPos"))
            {
                foreach (var slide in Slides)
                {
                    if (slide == null) return;
                    var pos1 = Drawing.WorldToScreen(myHero.ServerPosition);
                    var pos2 = Drawing.WorldToScreen(slide.Position);

                    if (slide.Name == "LeBlanc_Base_W_return_indicator.troy")
                    {
                        Drawing.DrawLine(pos1, pos2, 4, Color.Yellow);
                    }
                    if (slide.Name == "LeBlanc_Base_RW_return_indicator.troy")
                    {
                        Drawing.DrawLine(pos1, pos2, 4, Color.Purple);
                    }
                }
            }

            if (LBMenu.HSM["Auto"].Cast<KeyBind>().CurrentValue)
            {
                var pos = Drawing.WorldToScreen(myHero.ServerPosition);
                Drawing.DrawText(pos - new Vector2(-45, 30), Color.White, "Harass ON", 9);
            }
        }
    }    
}