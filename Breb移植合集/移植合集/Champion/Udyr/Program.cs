using System;
using System.Collections.Generic;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace D_Udyr
{
    internal class Program
    {
        public const string ChampionName = "Udyr";

        private static readonly List<Spell> SpellList = new List<Spell>();

        public static bool Tiger;

        public static bool Turtle;

        public static bool Bear;

        public static bool Phoenix;

        private static Spell _q;

        private static Spell _w;

        private static Spell _e;

        private static Spell _r;

        private static Menu _config;

        private static AIHeroClient _player;

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis;

        public static Menu comboMenu, forestGump, farm;

        public static readonly string[] Smitetype =
        {
            "s5_summonersmiteplayerganker", "s5_summonersmiteduel", "s5_summonersmitequick", "itemsmiteaoe",
            "summonersmite"
        };

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (_player.ChampionName != "Udyr")
                return;

            _q = new Spell(SpellSlot.Q, 200);
            _w = new Spell(SpellSlot.W, 200);
            _e = new Spell(SpellSlot.E, 200);
            _r = new Spell(SpellSlot.R, 200);

            SpellList.Add(_q);
            SpellList.Add(_w);
            SpellList.Add(_e);
            SpellList.Add(_r);

            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);

            //Udyr
            _config = MainMenu.AddMenu("D-Udyr", "D-Udyr");

            //Combo
            comboMenu = _config.AddSubMenu("Main", "Main");
            comboMenu.Add("delaycombo", new Slider("Delay between Skills", 200, 0, 1500));
            comboMenu.Add("AutoShield", new CheckBox("Auto Shield"));
            comboMenu.Add("AutoShield%", new Slider("AutoShield HP %", 50));
            comboMenu.Add("TargetRange", new Slider("Range to Use E", 1000, 600, 1500));
            comboMenu.Add("StunCycle", new KeyBind("Stun Cycle", false, KeyBind.BindTypes.HoldActive, 'Z'));

            //Forest gump
            forestGump = _config.AddSubMenu("Forest Gump", "Forest Gump");
            forestGump.Add("ForestE", new CheckBox("Use E"));
            forestGump.Add("ForestW", new CheckBox("Use W"));
            forestGump.Add("Forest", new KeyBind("Forest gump", false, KeyBind.BindTypes.HoldActive, 'N'));
            forestGump.Add("Forest-Mana", new Slider("Forest gump Mana", 50));

            //Farm
            farm = _config.AddSubMenu("Farm", "Farm");
            farm.Add("delayfarm", new Slider("Delay between Skills", 2000, 1000, 3000));
            farm.Add("Farm-Mana", new Slider("Mana Limit", 50));
            farm.AddGroupLabel("Lane");
            farm.Add("Use-Q-Farm", new CheckBox("Use Q"));
            farm.Add("Use-W-Farm", new CheckBox("Use W"));
            farm.Add("Use-E-Farm", new CheckBox("Use E"));
            farm.Add("Use-R-Farm", new CheckBox("Use R"));
            farm.AddGroupLabel("Jungle");
            farm.Add("Use-Q-Jungle", new CheckBox("Use Q"));
            farm.Add("Use-W-Jungle", new CheckBox("Use W"));
            farm.Add("Use-E-Jungle", new CheckBox("Use E"));
            farm.Add("Use-R-Jungle", new CheckBox("Use R"));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += OnGameUpdate;

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            Tiger = args.SData.Name == "UdyrTigerStance";

            Turtle = args.SData.Name == "UdyrTurtleStance";

            Bear = args.SData.Name == "UdyrBearStance";

            Phoenix = args.SData.Name == "UdyrPhoenixStance";
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

        private static void OnGameUpdate(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (getKeyBindItem(comboMenu, "StunCycle"))
            {
                StunCycle();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                 Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) &&
                100*(_player.Mana/_player.MaxMana) > getSliderItem(farm, "Farm-Mana"))
            {
                Farm();
                JungleClear();
            }

            if (getCheckBoxItem(comboMenu, "AutoShield") &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                AutoW();
            }

            if (getKeyBindItem(forestGump, "Forest") &&
                100*(_player.Mana/_player.MaxMana) > getSliderItem(forestGump, "Forest-Mana"))
            {
                Forest();
            }

            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;
        }

        private static void Farm()
        {
            var minions = MinionManager.GetMinions(_player.ServerPosition, 500.0F);
            if (minions.Count < 2) return;

            foreach (var minion in minions)
            {
                if (getCheckBoxItem(farm, "Use-R-Farm") && _r.IsReady())
                {
                    _r.Cast();
                }

                if (getCheckBoxItem(farm, "Use-Q-Farm") && _q.IsReady())
                {
                    _q.Cast();
                }

                if (getCheckBoxItem(farm, "Use-W-Farm") && _w.IsReady())
                {
                    _w.Cast();
                }

                if (getCheckBoxItem(farm, "Use-E-Farm") && _e.IsReady())
                {
                    _e.Cast();
                }
            }
        }

        private static void Forest()
        {
            if (_player.HasBuff("Recall") || _player.InFountain()) return;
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (_e.IsReady() && getCheckBoxItem(forestGump, "ForestE"))
            {
                _e.Cast();
            }

            if (_w.IsReady() && getCheckBoxItem(forestGump, "ForestW"))
            {
                _w.Cast();
            }
        }

        private static void AutoW()
        {
            if (_w.IsReady())
            {
                if (_player.HasBuff("Recall") || _player.InFountain()) return;
                if (_player.CountEnemiesInRange(1000) >= 1 &&
                    _player.Health <= _player.MaxHealth*getSliderItem(comboMenu, "AutoShield%")/100)
                {
                    _w.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var minions = MinionManager.GetMinions(_player.ServerPosition, 400, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var delay = getSliderItem(farm, "delayfarm");

            foreach (var minion in minions)
            {
                if (getCheckBoxItem(farm, "Use-Q-Jungle") && _q.IsReady() && minion.IsValidTarget())
                {
                    Utility.DelayAction.Add(delay, () => _q.Cast());
                    return;
                }

                if (getCheckBoxItem(farm, "Use-R-Jungle") && _r.IsReady() && minion.IsValidTarget())
                {
                    Utility.DelayAction.Add(delay, () => _r.Cast());
                    return;
                }

                if (getCheckBoxItem(farm, "Use-W-Jungle") && _w.IsReady() && minion.IsValidTarget())
                {
                    Utility.DelayAction.Add(delay, () => _w.Cast());
                    return;
                }

                if (getCheckBoxItem(farm, "Use-E-Jungle") && _e.IsReady() && minion.IsValidTarget())
                {
                    Utility.DelayAction.Add(delay, () => _e.Cast());
                    return;
                }
            }
        }


        private static void Combo()
        {
            //Create target

            var target = TargetSelector.GetTarget(getSliderItem(comboMenu, "TargetRange"), DamageType.Magical);
            var delay = getSliderItem(comboMenu, "delaycombo");
            if (target != null && _player.LSDistance(target) <= getSliderItem(comboMenu, "TargetRange"))
            {
                if (_e.IsReady() && !target.HasBuff("udyrbearstuncheck"))
                {
                    _e.Cast();
                    return;
                }

                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level >=
                    ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level)
                    if (_q.Cast()) return;

                if (_r.IsReady() && target.HasBuff("udyrbearstuncheck"))
                {
                    Utility.DelayAction.Add(delay, () => _r.Cast());
                    return;
                }

                if (_q.IsReady() && target.HasBuff("udyrbearstuncheck"))
                {
                    Utility.DelayAction.Add(delay, () => _q.Cast());
                    return;
                }

                if (_w.IsReady() && target.HasBuff("udyrbearstuncheck"))
                {
                    Utility.DelayAction.Add(delay, () => _w.Cast());
                }
            }
        }

        private static void StunCycle()
        {
            AIHeroClient closestEnemy = null;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (enemy.IsValidTarget(800) && !enemy.HasBuff("udyrbearstuncheck"))
                {
                    if (_e.IsReady())
                    {
                        _e.Cast();
                    }

                    if (closestEnemy == null)
                    {
                        closestEnemy = enemy;
                    }
                    else if (_player.LSDistance(closestEnemy) < _player.LSDistance(enemy))
                    {
                        closestEnemy = enemy;
                    }
                    else if (enemy.HasBuff("udyrbearstuncheck"))
                    {
                        Chat.Print(closestEnemy.ChampionName + " has buff already !!!");
                        closestEnemy = enemy;
                        Chat.Print(enemy.ChampionName + "is the new target");
                    }

                    if (!enemy.HasBuff("udyrbearstuncheck"))
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, closestEnemy);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getKeyBindItem(forestGump, "Forest"))
            {
                Drawing.DrawText(
                    Drawing.Width*0.02f,
                    Drawing.Height*0.92f,
                    Color.GreenYellow,
                    "Forest Is On");
            }
            else
                Drawing.DrawText(
                    Drawing.Width*0.02f,
                    Drawing.Height*0.92f,
                    Color.OrangeRed,
                    "Forest Is Off");
        }
    }
}