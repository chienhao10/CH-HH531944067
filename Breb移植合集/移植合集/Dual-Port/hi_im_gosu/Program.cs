#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace hi_im_gosu
{
    public class Vayne
    {
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell R;

        private static Menu menu;

        private static Dictionary<string, SpellSlot> spellData;

        private static AIHeroClient tar;
        public const string ChampName = "Vayne";
        public static AIHeroClient Player;
        private static Menu qmenu;
        private static Menu emenu;

        /* Asuna VayneHunter Copypasta */
        private static readonly Vector2 MidPos = new Vector2(6707.485f, 8802.744f);

        private static readonly Vector2 DragPos = new Vector2(11514, 4462);

        private static float LastMoveC;

        private static void TumbleHandler()
        {
            if (!getKeyBindItem(menu, "walltumble"))
                return;

            if (Player.LSDistance(MidPos) >= Player.LSDistance(DragPos))
            {
                if (Player.Position.X < 12000 || Player.Position.X > 12070 || Player.Position.Y < 4800 ||
                Player.Position.Y > 4872)
                {
                    MoveToLimited(new Vector2(12050, 4827).To3D());
                }
                else
                {
                    MoveToLimited(new Vector2(12050, 4827).To3D());
                    Q.Cast(DragPos, true);
                }
            }
            else
            {
                if (Player.Position.X < 6908 || Player.Position.X > 6978 || Player.Position.Y < 8917 ||
                Player.Position.Y > 8989)
                {
                    MoveToLimited(new Vector2(6958, 8944).To3D());
                }
                else
                {
                    MoveToLimited(new Vector2(6958, 8944).To3D());
                    Q.Cast(MidPos, true);
                }
            }
        }

        private static void MoveToLimited(Vector3 where)
        {
            if (Environment.TickCount - LastMoveC < 80)
            {
                return;
            }

            LastMoveC = Environment.TickCount;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }

        /* End Asuna VayneHunter Copypasta */

        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampName) return;
            spellData = new Dictionary<string, SpellSlot>();

            menu = MainMenu.AddMenu("Gosu", "Gosu");

            menu.Add("aaqaa", new KeyBind("Auto -> Q -> AA", false, KeyBind.BindTypes.HoldActive, 'X'));
            menu.Add("walltumble", new KeyBind("Wall Tumble", false, KeyBind.BindTypes.HoldActive, 'U'));
            menu.Add("useR", new CheckBox("Use R Combo"));
            menu.Add("enemys", new Slider("If Enemys Around >=", 2, 1, 5));

            qmenu = menu.AddSubMenu("Tumble", "Tumble");
            qmenu.Add("UseQC", new CheckBox("Use Q Combo"));
            qmenu.Add("hq", new CheckBox("Use Q Harass"));
            qmenu.Add("restrictq", new CheckBox("Restrict Q usage?"));
            qmenu.Add("UseQJ", new CheckBox("Use Q Farm"));
            qmenu.Add("Junglemana", new Slider("Minimum Mana to Use Q Farm", 60, 1, 100));

            emenu = menu.AddSubMenu("Condemn", "Condemn");
            emenu.Add("UseEC", new CheckBox("Use E Combo"));
            emenu.Add("he", new CheckBox("Use E Harass"));
            emenu.Add("UseET", new KeyBind("Use E (Toggle)", false, KeyBind.BindTypes.PressToggle, 'T'));
            emenu.Add("Int_E", new CheckBox("Use E To Interrupt"));
            emenu.Add("Gap_E", new CheckBox("Use E To Gabcloser"));
            emenu.Add("PushDistance", new Slider("E Push Distance", 425, 475, 300));
            emenu.Add("UseEaa", new KeyBind("Use E after auto", false, KeyBind.BindTypes.PressToggle, 'G'));


            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 0f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, float.MaxValue);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, float.MaxValue);

            E.SetTargetted(0.25f, 2200f);
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.LSIsValidTarget(200) && getCheckBoxItem(emenu, "Gap_E") && gapcloser.Sender.IsEnemy)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && unit.LSIsValidTarget(550) && getCheckBoxItem(emenu, "Int_E") && unit.IsEnemy)
            {
                E.Cast(unit);
            }
        }


        public static void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) && 100 * (Player.Mana / Player.MaxMana) > getSliderItem(qmenu, "Junglemana"))
            {
                var mob = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                var Minions = MinionManager.GetMinions(Player.Position.LSExtend(Game.CursorPos, Q.Range), Player.AttackRange, MinionTypes.All);
                var useQ = getCheckBoxItem(qmenu, "UseQJ");
                int countMinions = 0;

                foreach (var minions in Minions.Where(minion => minion.Health < Player.LSGetAutoAttackDamage(minion) || minion.Health < Q.GetDamage(minion) + Player.LSGetAutoAttackDamage(minion) || minion.Health < Q.GetDamage(minion)))
                {
                    countMinions++;
                }

                if (countMinions >= 2 && useQ && Q.IsReady() && Minions != null)
                    Q.Cast(Player.Position.LSExtend(Game.CursorPos, Q.Range / 2));

                if (useQ && Q.IsReady() && Orbwalking.InAutoAttackRange(mob) && mob != null)
                {
                    Q.Cast(Game.CursorPos);
                }
            }

            if (!(target is AIHeroClient)) return;

            tar = (AIHeroClient)target;

            if (getKeyBindItem(menu, "aaqaa"))
            {
                if (Q.IsReady())
                {
                    Q.Cast(Game.CursorPos);
                }

                Orbwalker.OrbwalkTo(Game.CursorPos);
            }

            if (getKeyBindItem(emenu, "UseEaa"))
            {
                E.Cast((Obj_AI_Base)target);
                emenu["UseEaa"].Cast<KeyBind>().CurrentValue = !getCheckBoxItem(emenu, "UseEaa");
            }

            if (Q.IsReady() && ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && getCheckBoxItem(qmenu, "UseQC")) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && getCheckBoxItem(qmenu, "hq"))))
            {
                if (getCheckBoxItem(qmenu, "restrictq"))
                {
                    var after = ObjectManager.Player.Position + Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                    var disafter = Vector3.DistanceSquared(after, tar.Position);
                    if ((disafter < 630 * 630) && disafter > 150 * 150)
                    {
                        Q.Cast(Game.CursorPos);
                    }

                    if (Vector3.DistanceSquared(tar.Position, ObjectManager.Player.Position) > 630 * 630 && disafter < 630 * 630)
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
                else
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        public static Vector3 Normalize(Vector3 A)
        {
            double distance = Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new Vector3(new Vector2((float)(A.X / distance)), (float)(A.Y / distance));
        }

        public static List<Vector2> Points = new List<Vector2>();

        public static bool threeSixty(AIHeroClient unit, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) || ObjectManager.Player.LSIsDashing())
                return false;
            var prediction = E.GetPrediction(unit);
            var predictionsList = pos.IsValid() ? new List<Vector3> { pos.To3D() } : new List<Vector3> { unit.ServerPosition, unit.Position, prediction.CastPosition, prediction.UnitPosition };
            var wallsFound = 0;
            Points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < 425; i += (int)unit.BoundingRadius) // 420 = push distance
                {
                    var cPos = ObjectManager.Player.Position.LSExtend(position, ObjectManager.Player.LSDistance(position) + i);
                    Points.Add(cPos.LSTo2D());
                    if (NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }
            if (wallsFound / predictionsList.Count >= 33 / 100f)
            {
                return true;
            }

            return false;
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (getCheckBoxItem(menu, "useR") && R.IsReady() && ObjectManager.Player.LSCountEnemiesInRange(1000) >= getSliderItem(menu, "enemys") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                R.Cast();
            }

            if (getKeyBindItem(menu, "walltumble"))
            {
                TumbleHandler();
            }

            if (getKeyBindItem(menu, "aaqaa"))
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }

            if (!E.IsReady()) return;
            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && getCheckBoxItem(emenu, "UseEC")) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && getCheckBoxItem(emenu, "he")) || getKeyBindItem(emenu, "UseET"))
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.LSIsValidTarget(E.Range) && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity) && threeSixty(x)))
                {
                    if (Player.LSDistance(enemy) < 450)
                    {
                        E.Cast(enemy);
                    }
                }
            }
        }
    }
}