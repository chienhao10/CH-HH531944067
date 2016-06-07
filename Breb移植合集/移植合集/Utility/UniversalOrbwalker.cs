using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using static SCommon.Orbwalking.Events;

namespace PortAIO.Utility
{
    class UniversalOrbwalker
    {

        public static SCommon.Orbwalking.Orbwalker LSOrb;
        public static bool loaded = false;

        public UniversalOrbwalker()
        {
            loaded = true;
            LSOrb = new SCommon.Orbwalking.Orbwalker();
            SCommon.TS.TargetSelector.Initialize();
        }

        public static bool getLSOrbwalker
        {
            get
            {
                return loaded ? Loader.orbwalker : false;
            }
        }

        public static void ResetAA()
        {
            if (getLSOrbwalker)
            {
                LSOrb.ResetAATimer();
            }
            else
            {
                Orbwalker.ResetAutoAttack();
            }
        }

        public static bool CanAttack()
        {
            if (getLSOrbwalker)
            {
                return LSOrb.CanAttack();
            }
            else
            {
                return Orbwalker.CanAutoAttack;
            }
        }

        public static bool CanMove()
        {
            if (getLSOrbwalker)
            {
                return LSOrb.CanMove();
            }
            else
            {
                return Orbwalker.CanMove;
            }
        }

        public static void ForcedTarget(AttackableUnit t)
        {
            if (getLSOrbwalker)
            {
                LSOrb.ForcedTarget = t;
            }
            else
            {
                Orbwalker.ForcedTarget = t;
            }
        }

        public static void ClearForcedTarget()
        {
            if (getLSOrbwalker)
            {
                LSOrb.ForcedTarget = null;
            }
            else
            {
                Orbwalker.ForcedTarget = null;
            }
        }

        public static void MoveTo(Vector3 pos)
        {
            if (getLSOrbwalker)
            {
                LSOrb.Move(pos);
            }
            else
            {
                Orbwalker.MoveTo(pos);
            }
        }

        public static bool IsAutoAttacking()
        {
            if (getLSOrbwalker)
            {
                return ObjectManager.Player.Spellbook.IsAutoAttacking;
            }
            else
            {
                return Orbwalker.IsAutoAttacking;
            }
        }

        public static AttackableUnit LastTarget()
        {
            if (getLSOrbwalker)
            {
                return LSOrb.LastTarget;
            }
            else
            {
                return Orbwalker.LastTarget;
            }
        }

        public static bool GetOrbwalkerMode(string str)
        {
            var s = str.ToLower();
            switch (s)
            {
                case "combo":
                    if (getLSOrbwalker)
                    {
                        return LSOrb.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo;
                    }
                    else
                    {
                        return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
                    }
                case "harass":
                    if (getLSOrbwalker)
                    {
                        return LSOrb.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed;
                    }
                    else
                    {
                        return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
                    }
                case "clear":
                    if (getLSOrbwalker)
                    {
                        return LSOrb.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear;
                    }
                    else
                    {
                        return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
                    }
                case "lasthit":
                    if (getLSOrbwalker)
                    {
                        return LSOrb.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LastHit;
                    }
                    else
                    {
                        return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
                    }
                case "none":
                    if (getLSOrbwalker)
                    {
                        return LSOrb.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.None;
                    }
                    else
                    {
                        return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None);
                    }
            }
            return false;
        }
    }
}
