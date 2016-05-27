namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using KappAzir.Utility;

    using SharpDX;

    public static class Jumper
    {
        public static int delay = Menus.JumperMenu.slider("delay");

        public static int range = Menus.JumperMenu.slider("range");

        public static void Jump(Vector3 pos)
        {
            var qpos = Player.Instance.ServerPosition.Extend(pos, Azir.Q.Range - 100).To3D();
            var wpos = Player.Instance.ServerPosition.Extend(pos, Azir.W.Range).To3D();
            var epos = Player.Instance.ServerPosition.Extend(pos, Azir.E.Range).To3D();
            var ready = Azir.E.IsReady() && Azir.Q.IsReady() && Player.Instance.Mana > Azir.Q.Mana() + Azir.E.Mana() + Azir.W.Mana();

            if (ready && Orbwalker.AzirSoldiers.Count(s => s.IsAlly && s.IsInRange(Player.Instance, range)) < 1)
            {
                if (LastCastedSpell.Spell == SpellSlot.E)
                {
                    return;
                }
                Azir.W.Cast(wpos);
            }
            else
            {
                if (ready)
                {
                    Core.DelayAction(
                        () =>
                            {
                                if (Azir.E.Cast(epos))
                                {
                                    Core.DelayAction(() => Azir.Q.Cast(qpos), delay);
                                }
                            },
                        100);
                }
            }

            if (LastCastedSpell.Spell == SpellSlot.E)
            {
                var timer = (Game.Time - LastCastedSpell.Time) * 100;
                if (timer - delay < 0.1f && Azir.Q.IsReady())
                {
                    Azir.Q.Cast(qpos);
                }
            }
        }
    }
}