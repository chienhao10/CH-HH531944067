#region

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

#endregion

namespace TwistedFate
{
    internal class Program
    {
        private static Menu Config;

        private static Spell Q;
        private static readonly float Qangle = 28*(float) Math.PI/180;
        private static Vector2 PingLocation;
        private static int LastPingT;
        private static AIHeroClient Player;
        private static int CastQTick;

        public static Menu q, w, menuItems, r, misc, drawings;

        private static void Ping(Vector2 position)
        {
            if (Utils.TickCount - LastPingT < 30*1000)
            {
                return;
            }

            LastPingT = Utils.TickCount;
            PingLocation = position;
            SimplePing();

            Utility.DelayAction.Add(150, SimplePing);
            Utility.DelayAction.Add(300, SimplePing);
            Utility.DelayAction.Add(400, SimplePing);
            Utility.DelayAction.Add(800, SimplePing);
        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Fallback, PingLocation, true);
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

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "TwistedFate") return;
            Player = ObjectManager.Player;
            Q = new Spell(SpellSlot.Q, 1450);
            Q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);

            //Make the menu
            Config = MainMenu.AddMenu("Twisted Fate", "TwistedFate");

            q = Config.AddSubMenu("Q - Wildcards", "Q");
            q.Add("AutoQI", new CheckBox("Auto-Q immobile"));
            q.Add("AutoQD", new CheckBox("Auto-Q dashing"));
            q.Add("CastQ", new KeyBind("Cast Q (tap)", false, KeyBind.BindTypes.HoldActive, 'U'));

            w = Config.AddSubMenu("W - Pick a card", "W");
            w.Add("SelectYellow", new KeyBind("Select Yellow", false, KeyBind.BindTypes.HoldActive, 'W'));
            w.Add("SelectBlue", new KeyBind("Select Blue", false, KeyBind.BindTypes.HoldActive, 'E'));
            w.Add("SelectRed", new KeyBind("Select Red", false, KeyBind.BindTypes.HoldActive, 'T'));

            menuItems = Config.AddSubMenu("Items", "Items");
            menuItems.Add("itemBotrk", new CheckBox("Botrk"));
            menuItems.Add("itemYoumuu", new CheckBox("Youmuu"));
            menuItems.Add("itemMode", new ComboBox("Use items on", 2, "No", "Mixed mode", "Combo mode", "Both"));

            r = Config.AddSubMenu("R - Destiny", "R");
            r.Add("AutoY", new CheckBox("Select yellow card after R"));

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("PingLH", new CheckBox("Ping low health enemies (Only local)"));

            drawings = Config.AddSubMenu("Drawings", "Drawings");
            drawings.Add("Qcircle", new CheckBox("Q Range"));
                //.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            drawings.Add("Rcircle", new CheckBox("R Range"));
                //.SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawings.Add("Rcircle2", new CheckBox("R Range (minimap)"));
                //.SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += DrawingOnOnEndScene;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Orbwalker.OnPreAttack += OrbwalkingOnBeforeAttack;
        }

        private static void OrbwalkingOnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target is AIHeroClient)
                args.Process = CardSelector.Status != SelectStatus.Selecting &&
                               Utils.TickCount - CardSelector.LastWSent > 300;
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name.Equals("Gate", StringComparison.InvariantCultureIgnoreCase) &&
                getCheckBoxItem(r, "AutoY"))
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            var rCircle2 = getCheckBoxItem(drawings, "Rcircle2");
            if (rCircle2)
            {
                Utility.DrawCircle(ObjectManager.Player.Position, 5500, Color.FromArgb(255, 255, 255, 255), 1, 23, true);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var qCircle = getCheckBoxItem(drawings, "Qcircle");
            var rCircle = getCheckBoxItem(drawings, "Rcircle");

            if (qCircle)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.FromArgb(100, 255, 0, 255));
            }

            if (rCircle)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 5500, Color.FromArgb(100, 255, 255, 255));
            }
        }


        private static int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            var result = 0;

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Q.Range*(position - startPoint).Normalized();
            var originalEndPoint = startPoint + originalDirection;

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];

                for (var k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0) endPoint = originalEndPoint;
                    if (k == 1) endPoint = startPoint + originalDirection.Rotated(Qangle);
                    if (k == 2) endPoint = startPoint + originalDirection.Rotated(-Qangle);

                    if (point.Distance(startPoint, endPoint, true, true) <
                        (Q.Width + hitBoxes[i])*(Q.Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }

            return result;
        }

        private static void CastQ(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Q.Range*(unitPosition - startPoint).Normalized();

            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = Q.GetPrediction(enemy);
                    if (pos.Hitchance >= HitChance.Medium)
                    {
                        points.Add(pos.UnitPosition.To2D());
                        hitBoxes.Add((int) enemy.BoundingRadius);
                    }
                }
            }

            var posiblePositions = new List<Vector2>();

            for (var i = 0; i < 3; i++)
            {
                if (i == 0) posiblePositions.Add(unitPosition + originalDirection.Rotated(0));
                if (i == 1) posiblePositions.Add(startPoint + originalDirection.Rotated(Qangle));
                if (i == 2) posiblePositions.Add(startPoint + originalDirection.Rotated(-Qangle));
            }


            if (startPoint.Distance(unitPosition) < 900)
            {
                for (var i = 0; i < 3; i++)
                {
                    var pos = posiblePositions[i];
                    var direction = (pos - startPoint).Normalized().Perpendicular();
                    var k = 2/3*(unit.BoundingRadius + Q.Width);
                    posiblePositions.Add(startPoint - k*direction);
                    posiblePositions.Add(startPoint + k*direction);
                }
            }

            var bestPosition = new Vector2();
            var bestHit = -1;

            foreach (var position in posiblePositions)
            {
                var hits = CountHits(position, points, hitBoxes);
                if (hits > bestHit)
                {
                    bestPosition = position;
                    bestHit = hits;
                }
            }

            if (bestHit + 1 <= minTargets)
                return;

            Q.Cast(bestPosition.To3D(), true);
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            var dmg = 0d;
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q)*2;
            dmg += Player.GetSpellDamage(hero, SpellSlot.W);
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q);

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += ObjectManager.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }

            return (float) dmg;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getCheckBoxItem(misc, "PingLH"))
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready &&
                                    h.IsValidTarget() && ComboDamage(h) > h.Health))
                {
                    Ping(enemy.Position.To2D());
                }

            if (getKeyBindItem(q, "CastQ"))
            {
                CastQTick = Utils.TickCount;
            }

            if (Utils.TickCount - CastQTick < 500)
            {
                var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (qTarget != null)
                {
                    Q.Cast(qTarget);
                }
            }

            var combo = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);

            //Select cards.
            if (getKeyBindItem(w, "SelectYellow") || combo)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }

            if (getKeyBindItem(w, "SelectBlue"))
            {
                CardSelector.StartSelecting(Cards.Blue);
            }

            if (getKeyBindItem(w, "SelectRed"))
            {
                CardSelector.StartSelecting(Cards.Red);
            }

            //Auto Q
            var autoQI = getCheckBoxItem(q, "AutoQI");
            var autoQD = getCheckBoxItem(q, "AutoQD");


            if (ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready && (autoQD || autoQI))
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    if (enemy.IsValidTarget(Q.Range*2))
                    {
                        var pred = Q.GetPrediction(enemy);
                        if ((pred.Hitchance == HitChance.Immobile && autoQI) ||
                            (pred.Hitchance == HitChance.Dashing && autoQD))
                        {
                            CastQ(enemy, pred.UnitPosition.To2D());
                        }
                    }
                }


            var useItemModes = getBoxItem(menuItems, "itemMode");
            if (
                !((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                   (useItemModes == 2 || useItemModes == 3)) ||
                  (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                   (useItemModes == 1 || useItemModes == 3))))
                return;

            var botrk = getCheckBoxItem(menuItems, "itemBotrk");
            var youmuu = getCheckBoxItem(menuItems, "itemYoumuu");
            var target = Orbwalker.LastTarget as Obj_AI_Base;

            if (botrk)
            {
                if (target != null && target.Type == ObjectManager.Player.Type &&
                    target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 450)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);

                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass ||
                            ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth &&
                            Items.CanUseItem(itemId))
                            Items.UseItem(itemId, target);
                    }
                }
            }

            if (youmuu && target != null && target.Type == ObjectManager.Player.Type &&
                Orbwalking.InAutoAttackRange(target) && Items.CanUseItem(3142))
                Items.UseItem(3142);
        }
    }
}