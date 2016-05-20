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

        private static Spell Q, W;
        private static readonly float Qangle = 28 * (float)Math.PI / 180;
        private static Vector2 PingLocation;
        private static int LastPingT;
        private static AIHeroClient Player;
        private static int CastQTick;

        public static Menu q, w, menuItems, r, misc, drawings, laneclearMenu, jungleclearMenu;

        private static void Ping(Vector2 position)
        {
            if (Utils.TickCount - LastPingT < 30 * 1000)
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

            W = new Spell(SpellSlot.W);

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

            laneclearMenu = Config.AddSubMenu("Laneclear", "laneclearset");
            laneclearMenu.Add("laneclearUseQ", new CheckBox("Use Q"));
            laneclearMenu.Add("laneclearQmana", new Slider("Cast Q If Mana % >", 50));
            laneclearMenu.Add("laneclearQmc", new Slider("Cast Q If Hit Minion Number >=", 5, 2, 7));
            laneclearMenu.Add("laneclearUseW", new CheckBox("Use W"));
            laneclearMenu.Add("laneclearredmc", new Slider("Red Instead of Blue If Minion Number >=", 3, 2, 5));
            laneclearMenu.Add("laneclearbluemana", new Slider("Blue Instead of Red If Mana % <", 30));

            jungleclearMenu = Config.AddSubMenu("Jungleclear", "jungleclearset");
            jungleclearMenu.Add("jungleclearUseQ", new CheckBox("Use Q"));
            jungleclearMenu.Add("jungleclearQmana", new Slider("Cast Q If Mana % >0", 30));
            jungleclearMenu.Add("jungleclearUseW", new CheckBox("Use W"));
            jungleclearMenu.Add("jungleclearbluemana", new Slider("Pick Blue If Mana % <", 30));

            menuItems = Config.AddSubMenu("Items", "Items");
            menuItems.Add("itemBotrk", new CheckBox("Botrk"));
            menuItems.Add("itemYoumuu", new CheckBox("Youmuu"));
            menuItems.Add("itemMode", new ComboBox("Use items on", 2, "No", "Mixed mode", "Combo mode", "Both"));

            r = Config.AddSubMenu("R - Destiny", "R");
            r.Add("AutoY", new CheckBox("Select yellow card after R"));

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("PingLH", new CheckBox("Ping low health enemies (Only local)"));
            misc.Add("DontGoldCardDuringCombo", new CheckBox("Don't select gold card on combo", false));

            drawings = Config.AddSubMenu("Drawings", "Drawings");
            drawings.Add("Qcircle", new CheckBox("Q Range"));
            //.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            drawings.Add("Rcircle", new CheckBox("R Range"));
            //.SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
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

        static void LaneClear()
        {
            if (Q.IsReady() && getCheckBoxItem(laneclearMenu, "laneclearUseQ") && Player.ManaPercent > getSliderItem(laneclearMenu, "laneclearQmana"))
            {
                var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);
                var locQ = Q.GetLineFarmLocation(allMinionsQ);

                if (locQ.MinionsHit >= getSliderItem(laneclearMenu, "laneclearQmc"))
                    Q.Cast(locQ.Position);
            }

            if (W.IsReady() && getCheckBoxItem(laneclearMenu, "laneclearUseW"))
            {
                var minioncount = MinionManager.GetMinions(Player.Position, 1500).Count;

                if (minioncount > 0)
                {
                    if (Player.ManaPercent > getSliderItem(laneclearMenu, "laneclearbluemana"))
                    {
                        if (minioncount >= getSliderItem(laneclearMenu, "laneclearredmc"))
                            CardSelector.StartSelecting(Cards.Red);
                        else
                            CardSelector.StartSelecting(Cards.Blue);
                    }
                    else
                        CardSelector.StartSelecting(Cards.Blue);
                }
            }
        }

        static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0)
                return;

            if (Q.IsReady() && getCheckBoxItem(jungleclearMenu, "jungleclearUseQ") && Player.ManaPercent > getSliderItem(jungleclearMenu, "jungleclearQmana"))
            {
                Q.Cast(mobs[0].Position);
            }

            if (W.IsReady() && getCheckBoxItem(jungleclearMenu, "jungleclearUseW"))
            {
                if (Player.ManaPercent > getSliderItem(jungleclearMenu, "jungleclearbluemana"))
                {
                    if (mobs.Count >= 2)
                        CardSelector.StartSelecting(Cards.Red);
                    else
                        CardSelector.StartSelecting(Cards.Yellow);
                }
                else
                    CardSelector.StartSelecting(Cards.Blue);
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
            var originalDirection = Q.Range * (position - startPoint).Normalized();
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

                    if (point.LSDistance(startPoint, endPoint, true, true) <
                        (Q.Width + hitBoxes[i]) * (Q.Width + hitBoxes[i]))
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
            var originalDirection = Q.Range * (unitPosition - startPoint).Normalized();

            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = Q.GetPrediction(enemy);
                    if (pos.Hitchance >= HitChance.Medium)
                    {
                        points.Add(pos.UnitPosition.To2D());
                        hitBoxes.Add((int)enemy.BoundingRadius);
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


            if (startPoint.LSDistance(unitPosition) < 900)
            {
                for (var i = 0; i < 3; i++)
                {
                    var pos = posiblePositions[i];
                    var direction = (pos - startPoint).Normalized().Perpendicular();
                    var k = 2 / 3 * (unit.BoundingRadius + Q.Width);
                    posiblePositions.Add(startPoint - k * direction);
                    posiblePositions.Add(startPoint + k * direction);
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
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q) * 2;
            dmg += Player.GetSpellDamage(hero, SpellSlot.W);
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q);

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += ObjectManager.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }

            return (float)dmg;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getCheckBoxItem(misc, "PingLH"))
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready && h.IsValidTarget() && h.IsEnemy && ComboDamage(h) > h.Health))
                {
                    Ping(enemy.Position.To2D());
                }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
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
            if (getKeyBindItem(w, "SelectYellow") || combo && !getCheckBoxItem(misc, "DontGoldCardDuringCombo"))
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
                    if (enemy.IsValidTarget(Q.Range * 2))
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
                    target.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition) < 450)
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