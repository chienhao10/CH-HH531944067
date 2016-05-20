#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

#endregion

namespace ChewyMoonsShaco
{
    internal class ChewyMoonShaco
    {
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell R;
        public static Menu Menu;
        public static List<LeagueSharp.Common.Spell> SpellList;
        public static Items.Item Tiamat;
        public static Items.Item Hydra;
        public static int cloneAct = 0;
        public static AIHeroClient player = ObjectManager.Player;
        public static void OnGameLoad()
        {
            if (player.BaseSkinName != "Shaco")
            {
                return;
            }

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 400);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 425);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 625);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 200);

            SpellList = new List<LeagueSharp.Common.Spell> { Q, E, W, R };

            CreateMenu();
            Illuminati.Init();

            Tiamat = ItemData.Tiamat_Melee_Only.GetItem();
            Hydra = ItemData.Ravenous_Hydra_Melee_Only.GetItem();

            Game.OnUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += OrbwalkingOnAfterAttack;
            
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }


        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(!getCheckBoxItem(escapeMenu, "Evade"))return;
            if (sender.IsAlly) return;
            if (!sender.IsChampion()) return;

            //Need to calc Delay/Time for misille to hit !

            if (DangerDB.TargetedList.Contains(args.SData.Name))
            {
                if (args.Target.IsMe)
                    R.Cast();
            }

            if (DangerDB.CircleSkills.Contains(args.SData.Name))
            {
                if (player.LSDistance(args.End) < args.SData.LineWidth)
                    R.Cast();
            }

            if (DangerDB.Skillshots.Contains(args.SData.Name))
            {
                if (new LeagueSharp.Common.Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(player))
                {
                    R.Cast();
                }
            }
        }


        private static void OrbwalkingOnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!(target is AIHeroClient))
            {
                return;
            }

            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (Hydra.IsReady())
            {
                Hydra.Cast();
            }
            else if (Tiamat.IsReady())
            {
                Tiamat.Cast();
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

        public static Menu comboMenu, harassMenu, ksMenu, escapeMenu, illuminatiMenu, drawingMenu, miscMenu;

        private static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("[Chewy's Shaco]", "cmShaco");

            // Combo
            comboMenu = Menu.AddSubMenu("Combo", "cmShacoCombo");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useR", new CheckBox("Use R"));
            comboMenu.Add("cloneOrb", new CheckBox("Clone Orbwalking"));
            comboMenu.Add("useItems", new CheckBox("Use items"));

            // Harass
            harassMenu = Menu.AddSubMenu("Harass", "cmShacoHarass");
            harassMenu.Add("useEHarass", new CheckBox("Use E"));

            // Ks
            ksMenu = Menu.AddSubMenu("KS", "cmShacoKS");
            ksMenu.Add("ksE", new CheckBox("Use E"));

            //Escape
            escapeMenu = Menu.AddSubMenu("Escape", "esc");
            escapeMenu.Add("Escape", new KeyBind("Escape", false, KeyBind.BindTypes.HoldActive, 'Z'));
            escapeMenu.Add("EscapeR", new KeyBind("Escape With Ultimate", false, KeyBind.BindTypes.HoldActive, 226));
            escapeMenu.Add("Evade", new CheckBox("Evade With Ultimate", false));

            // ILLUMINATI
            illuminatiMenu = Menu.AddSubMenu("Illuminati", "cmShacoTriangleIlluminatiSp00ky");
            illuminatiMenu.Add("PlaceBox", new KeyBind("Place Box", false, KeyBind.BindTypes.HoldActive, 73));
            illuminatiMenu.Add("RepairTriangle", new CheckBox("Repair Triangle & Auto Form Triangle"));
            illuminatiMenu.Add("BoxDistance", new Slider("Box Distance", 600, 101, 1200));
            illuminatiMenu["BoxDistance"].Cast<Slider>().OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    Illuminati.TriangleLegDistance = args.NewValue;
                };

            // Drawing
            drawingMenu = Menu.AddSubMenu("Drawings", "cmShacoDrawing");
            drawingMenu.Add("drawQ", new CheckBox("Draw Q"));
            drawingMenu.Add("drawQPos", new CheckBox("Draw Q Pos"));
            drawingMenu.Add("drawW", new CheckBox("Draw W"));
            drawingMenu.Add("drawE", new CheckBox("Draw E"));

            // Misc
            miscMenu = Menu.AddSubMenu("Misc", "cmShacoMisc");
            miscMenu.Add("usePackets", new CheckBox("Use packets"));
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var qCircle = getCheckBoxItem(drawingMenu, "drawQ");
            var wCircle = getCheckBoxItem(drawingMenu, "drawW");
            var eCircle = getCheckBoxItem(drawingMenu, "drawE");
            var qPosCircle = getCheckBoxItem(drawingMenu, "drawQPos");

            var pos = player.Position;

            if (qCircle)
            {
                Render.Circle.DrawCircle(pos, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (wCircle)
            {
                Render.Circle.DrawCircle(pos, W.Range, W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (eCircle)
            {
                Render.Circle.DrawCircle(pos, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }

            if (!qPosCircle)
            {
                return;
            }

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.LSIsValidTarget()))
            {
                Drawing.DrawLine(
                    Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(ShacoUtil.GetQPos(enemy, false)), 2,
                    Color.Aquamarine);
            }
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(escapeMenu, "EscapeR"))
            {
                if (R.IsReady() && Q.IsReady())
                {
                    R.Cast();
                }
                Escape();
            }

            if (getKeyBindItem(escapeMenu, "Escape"))
            {
                Escape();
            }


            if (getCheckBoxItem(ksMenu, "ksE"))
            {
                KillSecure();
            }

            if (getKeyBindItem(illuminatiMenu, "PlaceBox"))
            {
                Illuminati.PlaceInitialBox();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }

        public static void Escape()
        {
            Q.Cast(Game.CursorPos);
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var clone = getClone();

            if (clone != null)
            {

                var pos = Game.CursorPos.LSExtend(clone.Position, clone.LSDistance(Game.CursorPos) + 2000);
                R.Cast(pos);

            }

            
        }

        public static Obj_AI_Base getClone()
        {
            Obj_AI_Base Clone = null;
            foreach (var unit in ObjectManager.Get<Obj_AI_Base>().Where(clone => !clone.IsMe && clone.Name == player.Name))
            {
                Clone = unit;
            }

            return Clone;

        }

        private static void KillSecure()
        {
            if (!E.IsReady())
            {
                return;
            }

            foreach (var target in
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsEnemy)
                    .Where(x => !x.IsDead)
                    .Where(x => x.LSDistance(player) <= E.Range)
                    .Where(target => player.LSGetSpellDamage(target, SpellSlot.E) > target.Health))
            {
                E.CastOnUnit(target, getCheckBoxItem(miscMenu, "usePackets"));
                return;
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range , DamageType.Physical);

            var useQ = getCheckBoxItem(comboMenu, "useQ");
            var useW = getCheckBoxItem(comboMenu, "useW");
            var useE = getCheckBoxItem(comboMenu, "useE");
            var packets = getCheckBoxItem(miscMenu, "usePackets");

            foreach (var spell in SpellList.Where(x => x.IsReady()))
            {
                if (spell.Slot == SpellSlot.Q && useQ)
                {
                    if (!target.IsValidTarget(Q.Range))
                    {
                        continue;
                    }

                    var pos = ShacoUtil.GetQPos(target, true);
                    Q.Cast(pos, packets);
                }


                if(target!=null)
                if (spell.Slot == SpellSlot.R && target.IsValidTarget() && player.LSDistance(target) < 400 &&
                    player.HasBuff("Deceive") && getCheckBoxItem(comboMenu, "useR"))
                {
                    R.Cast();
                }

                if (spell.Slot == SpellSlot.W && useW)
                {
                    //TODO: Make W based on waypoints
                    if (!target.IsValidTarget(W.Range))
                    {
                        continue;
                    }

                    var pos = ShacoUtil.GetQPos(target, true, 100);
                    W.Cast(pos, packets);
                }

                if (spell.Slot != SpellSlot.E || !useE)
                {
                    continue;
                }
                if (!target.IsValidTarget(E.Range))
                {
                    continue;
                }

                E.CastOnUnit(target);
            }

            if (!getCheckBoxItem(comboMenu, "cloneOrb")) return;
            if(!hasClone())return;
            Obj_AI_Base clone = getClone();

                if (Environment.TickCount > cloneAct + 200)
                {
                    if (target != null)
                    {
                        if (clone.Spellbook.IsAutoAttacking)
                            return;
                        R.Cast(target);
                    }
                    else
                    {
                        R.Cast(Game.CursorPos);
                    }
                    cloneAct = Environment.TickCount;
                }
            
        }

        private static void Harass()
        {
            var useE = getCheckBoxItem(harassMenu, "useEHarass");
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (!target.IsValidTarget(E.Range))
            {
                return;
            }

            if (useE && E.IsReady())
            {
                E.CastOnUnit(target);
            }
        }

        public static bool hasClone()
        {
            return player.GetSpell(SpellSlot.R).Name.Equals("hallucinateguide");
        }
    }
}