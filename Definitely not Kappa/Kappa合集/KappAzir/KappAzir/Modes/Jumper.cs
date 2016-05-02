namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using Mario_s_Lib;

    using SharpDX;
    using static Menus;
    using static SpellsManager;

    internal class Jumper : ModeManager
    {
        public static int delay = delay = FleeMenu.GetSliderValue("delay");

        public static int range = delay = FleeMenu.GetSliderValue("range");

        private static float et = 0;

        public static Vector3 castpos;

        public static void jump(Vector3 qpos, Vector3 pos)
        {
            castpos = qpos;
            if (Orbwalker.AzirSoldiers.Count(s => s.Distance(Azir) < range) < 1)
            {
                if (E.IsReady() && Q.IsReady())
                {
                    if (W.Cast(Azir.ServerPosition.Extend(pos, W.Range).To3D()))
                    {
                        Core.DelayAction(
                            () =>
                                {
                                    if (E.Cast(Azir.ServerPosition.Extend(pos, E.Range).To3D()))
                                    {
                                        et = Game.Time;
                                        if (Game.Time - et < 1 && Game.Time - et > 0.2f)
                                        {
                                            Core.DelayAction(() => Q.Cast(Azir.ServerPosition.Extend(qpos, Q.Range).To3D()), delay);
                                        }
                                    }
                                },
                            150);
                    }
                }
            }
            else
            {
                if (E.IsReady() && Q.IsReady())
                {
                    Core.DelayAction(
                        () =>
                            {
                                if (E.Cast(Azir.ServerPosition.Extend(pos, E.Range).To3D()))
                                {
                                    if (Game.Time - et < 1 && Game.Time - et > 0.2f)
                                    {
                                        Core.DelayAction(() => Q.Cast(Azir.ServerPosition.Extend(qpos, Q.Range).To3D()), delay);
                                    }
                                }
                            },
                        150);

                    Core.DelayAction(
                        () =>
                            {
                                if (Q.Cast(Azir.ServerPosition.Extend(qpos, Q.Range).To3D()))
                                {
                                    Core.DelayAction(() => E.Cast(Azir.ServerPosition.Extend(pos, E.Range).To3D()), delay);
                                }
                            },
                        250);
                }
            }
        }

        internal static void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast1;
        }

        private static void Obj_AI_Base_OnProcessSpellCast1(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (FleeMenu.GetKeyBindValue("flee") || FleeMenu.GetKeyBindValue("insect") || FleeMenu.GetKeyBindValue("insected")))
            {
                if (args.SData.Name == "AzirE" && Q.IsReady())
                {
                    Core.DelayAction(() => Q.Cast(Azir.ServerPosition.Extend(castpos, Q.Range).To3D()), delay);
                }
                if (args.SData.Name == "AzirQ" && E.IsReady())
                {
                    Core.DelayAction(() => E.Cast(Azir.ServerPosition.Extend(castpos, E.Range).To3D()), delay);
                }
            }
        }
    }
}