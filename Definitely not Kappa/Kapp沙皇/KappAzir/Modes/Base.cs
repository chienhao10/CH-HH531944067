namespace KappAzir.Modes
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Rendering;

    using Utility;

    using SharpDX;

    public static class Base
    {
        public static void Game_OnTick(EventArgs args)
        {
            updatespells();

            if (Insec.NewInsec)
            {
                var rpos = Player.Instance.ServerPosition.Extend(Insec.insectpos(), Azir.R.Range).To3D();

                var qtime = Game.Time - Insec.qtime;
                if ((qtime > 0.1f && qtime < 0.1) || TargetSelector.SelectedTarget.IsValidTarget(Azir.R.Range - 75))
                {
                    Azir.R.Cast(rpos);
                }
            }

            if (Menus.ComboMenu.keybind("key"))
            {
                Combo.Execute();
            }

            if (Menus.HarassMenu.keybind("key") || Menus.HarassMenu.keybind("toggle"))
            {
                Harass.Execute();
            }
            if (Menus.LaneClearMenu.keybind("key"))
            {
                LaneClear.Execute();
            }
            if (Menus.JungleClearMenu.keybind("key"))
            {
                JungleClear.Execute();
            }
            if (Menus.JumperMenu.keybind("jump"))
            {
                Jumper.Jump(Game.CursorPos);
            }
            if (Menus.JumperMenu.keybind("normal"))
            {
                var target = TargetSelector.SelectedTarget;
                Insec.Normal(target);
            }

            if (Menus.JumperMenu.keybind("new"))
            {
                Insec.New();
            }

            if (Menus.Auto.checkbox("tower"))
            {
                var azirtower =
                    ObjectManager.Get<GameObject>()
                        .FirstOrDefault(o => o != null && o.Name.ToLower().Contains("towerclicker") && Player.Instance.Distance(o) < 500);
                if (azirtower != null && azirtower.CountEnemeis(800) >= Menus.Auto.slider("Tenemy"))
                {
                    Player.UseObject(azirtower);
                }
            }

            Insec.NormalInsec = Menus.JumperMenu.keybind("normal");
            Insec.NewInsec = Menus.JumperMenu.keybind("new");
        }

        public static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (target == null || args.Target == null || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                || Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly) < 1)
            {
                return;
            }
            var orbtarget = args.Target as Obj_AI_Base;
            foreach (var sold in Orbwalker.ValidAzirSoldiers)
            {
                if (sold != null)
                {
                    var sold1 = sold;
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                            m => m.IsInRange(sold1, sold1.GetAutoAttackRange()) && m.IsKillable());
                    if (minion != null && minion != orbtarget)
                    {
                        var killable = Player.Instance.GetAutoAttackDamage(orbtarget, true)
                                       >= Prediction.Health.GetPrediction(orbtarget, (int)Player.Instance.AttackDelay);
                        if (!killable)
                        {
                            args.Process = false;
                            Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        }
                    }
                }
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W)
            {
                Orbwalker.ResetAutoAttack();
            }

            LastCastedSpell.Spell = args.Slot;
            LastCastedSpell.Time = Game.Time;
        }

        public static void updatespells()
        {
            Azir.R.Width = 107 * (Azir.R.Level - 1) < 200 ? 220 : (107 * (Azir.R.Level - 1)) + 5;

            int count = Orbwalker.AzirSoldiers.Count(s => s.IsAlly);
            Azir.Q.Width = count != 0 ? 65 * count : 65;
        }

        public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !sender.IsValidTarget(300) || e.End == Vector3.Zero
                || !e.End.IsInRange(Player.Instance, 300) || !Menus.Auto.checkbox(e.SpellName) || !Menus.Auto.checkbox("Gap") || !Azir.R.IsReady())
            {
                return;
            }

            Azir.R.Cast(sender);
        }

        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !sender.IsValidTarget(Azir.R.Range) || e.DangerLevel == Common.danger()
                || !Menus.Auto.checkbox("int") || !Azir.R.IsReady())
            {
                return;
            }

            Azir.R.Cast(sender);
        }

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null && Azir.R.IsReady() && Menus.Auto.checkbox("gap") && Menus.Auto.checkbox("rengar")
                    && rengar.IsValidTarget(Azir.R.Range))
                {
                    Azir.R.Cast(rengar);
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            // Insec Helper
            var target = TargetSelector.SelectedTarget;
            var colors = System.Drawing.Color.White;

            if (Menus.DrawMenu.checkbox("insec") && (Insec.NormalInsec || Insec.NewInsec))
            {
                var insecpos = Insec.insectpos(target);
                float x;
                float y;
                if (target == null)
                {
                    x = Game.CursorPos.WorldToScreen().X;
                    y = Game.CursorPos.WorldToScreen().Y - 15;
                    Drawing.DrawText(x, y, colors, "SELECT A TARGET", 5);
                }
                else
                {
                    x = target.ServerPosition.WorldToScreen().X;
                    y = target.ServerPosition.WorldToScreen().Y;
                    Drawing.DrawText(x, y, colors, "SELECTED TARGET", 5);
                    Circle.Draw(Color.Red, target.BoundingRadius, target.ServerPosition);
                    if (Insec.NewInsec && !Orbwalker.AzirSoldiers.Any(s => s.IsInRange(target, 420) && s.IsAlly))
                    {
                        x = Game.CursorPos.WorldToScreen().X;
                        y = Game.CursorPos.WorldToScreen().Y - 15;
                        Drawing.DrawText(x, y, colors, "CREATE A SOLDIER NEAR THE TARGET FIRST", 5);
                    }
                }

                if (insecpos == Vector3.Zero)
                {
                    x = Game.CursorPos.WorldToScreen().X;
                    y = Game.CursorPos.WorldToScreen().Y - 15;
                    Drawing.DrawText(x, y, colors, "Cant Detect Insec Position", 5);
                }
                else
                {
                    x = insecpos.WorldToScreen().X;
                    y = insecpos.WorldToScreen().Y;
                    Drawing.DrawText(x, y, colors, "Insec Position", 5);
                }

                if (target != null && insecpos != Vector3.Zero)
                {
                    var pos = target.ServerPosition.Extend(insecpos, -200).To3D();
                    var rpos = Player.Instance.ServerPosition.Extend(insecpos, Azir.R.Range).To3D();
                    Circle.Draw(Color.White, 100, rpos);
                    Circle.Draw(Color.White, 100, pos);
                    Circle.Draw(Color.White, 200, insecpos);
                    Line.DrawLine(colors, pos, rpos);
                }
            }

            // Spells Drawings
            foreach (var spell in Azir.SpellList)
            {
                var color = Menus.ColorMenu.Color(spell.Slot.ToString());
                var draw = Menus.DrawMenu.checkbox(spell.Slot.ToString());

                if (draw)
                {
                    Circle.Draw(new ColorBGRA(color.R, color.G, color.B, color.A), spell.Range, Player.Instance.ServerPosition);
                }
            }
        }
    }
}