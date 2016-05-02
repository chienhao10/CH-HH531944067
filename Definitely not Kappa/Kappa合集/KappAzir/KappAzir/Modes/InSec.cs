namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using Mario_s_Lib;

    using SharpDX;
    using static Menus;
    using static SpellsManager;

    internal class InSec : ModeManager
    {
        public static float LastQTime;

        public static AIHeroClient target;

        public static Vector3 qpos;

        public static Vector3 rpos;

        internal static Vector3 insecLoc;

        internal static Vector3 soldposition;

        public static void New()
        {
            target = TargetSelector.SelectedTarget;
            if (target != null)
            {
                if (target.IsValidTarget(925))
                {
                    if (Q.IsReady())
                    {
                        if (R.IsReady())
                        {
                            switch (FleeMenu.GetComboBoxValue("qpos"))
                            {
                                case 0:
                                    {
                                        qpos = Game.CursorPos;
                                    }
                                    break;

                                case 1:
                                    {
                                        qpos = Azir.ServerPosition;
                                    }
                                    break;

                                case 2:
                                    {
                                        qpos = tower?.ServerPosition ?? Game.CursorPos;
                                    }
                                    break;

                                case 3:
                                    {
                                        qpos = ally?.ServerPosition ?? Game.CursorPos;
                                    }
                                    break;
                            }

                            switch (FleeMenu.GetComboBoxValue("rpos"))
                            {
                                case 0:
                                    {
                                        rpos = Game.CursorPos;
                                    }
                                    break;

                                case 1:
                                    {
                                        rpos = Azir.ServerPosition;
                                    }
                                    break;

                                case 2:
                                    {
                                        rpos = tower?.ServerPosition ?? Game.CursorPos;
                                    }
                                    break;

                                case 3:
                                    {
                                        rpos = ally?.ServerPosition ?? Game.CursorPos;
                                    }
                                    break;
                            }

                            var allready = Q.IsReady() && E.IsReady() && W.IsReady();

                            if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) > 0 && allready && ManaCheck(Azir) < Azir.Mana
                                && target.Distance(Azir) > 420)
                            {
                                soldposition = Orbwalker.AzirSoldiers.OrderBy(s => s.Distance(target)).FirstOrDefault(s => s != null).ServerPosition;
                                if (soldposition.IsInRange(target.ServerPosition, R.Width) && !Ehit(target))
                                {
                                    if (E.Cast())
                                    {
                                        Core.DelayAction(
                                            () =>
                                                {
                                                    if (Q.Cast(Azir.Position.Extend(qpos, Q.Range - FleeMenu.GetSliderValue("dis")).To3D()))
                                                    {
                                                        LastQTime = Game.Time;
                                                    }
                                                },
                                            FleeMenu.GetSliderValue("delay") + Game.Ping);
                                    }
                                }
                                else
                                {
                                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                                }
                            }
                            else
                            {
                                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            }
                        }
                        else
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        }
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
                else
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }
            else
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        public static void Normal()
        {
            target = TargetSelector.SelectedTarget;
            if (target != null)
            {
                if (target.IsValidTarget(R.Width) && R.IsReady())
                {
                    if (tower != null && FleeMenu.GetCheckBoxValue("Tower"))
                    {
                        R.Cast(Azir.Position.Extend(tower.ServerPosition, R.Range).To3D());
                    }
                    else if (ally != null && FleeMenu.GetCheckBoxValue("Ally"))
                    {
                        R.Cast(Azir.Position.Extend(ally.ServerPosition, R.Range).To3D());
                    }
                    else
                    {
                        R.Cast(Azir.Position.Extend(insecLoc, R.Range).To3D());
                    }
                }
                else
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                if (target.IsValidTarget(1100))
                {
                    if (Q.IsReady())
                    {
                        if (R.IsReady())
                        {
                            insecLoc = Vector3.Zero;
                            var direction = (TargetSelector.SelectedTarget.ServerPosition - ObjectManager.Player.ServerPosition).To2D().Normalized();
                            var insecPos = TargetSelector.SelectedTarget.ServerPosition.To2D() + (direction * 175f);
                            if (Orbwalker.AzirSoldiers.OrderBy(s => s.Distance(insecPos)).FirstOrDefault() != null)
                            {
                                soldposition = Orbwalker.AzirSoldiers.OrderBy(s => s.Distance(insecPos)).FirstOrDefault().ServerPosition;
                            }
                            insecLoc = Azir.ServerPosition;

                            Jumper.jump(insecPos.To3D(), insecPos.To3D());
                        }
                        else
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        }
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
                else
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }
            else
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }
    }
}