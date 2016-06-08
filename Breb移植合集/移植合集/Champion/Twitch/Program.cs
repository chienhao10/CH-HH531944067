using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using System.Collections.Generic;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Twitch
    {
        private static readonly Menu Config = Program.Config;
        public static Spell Q, W, E, R;
        public static float QMANA, WMANA, EMANA, RMANA;

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, laneClear;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
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

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 975);

            W.SetSkillshot(0.25f, 100f, 1410f, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("notif", new CheckBox("Notification (timers)"));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            laneClear = Config.AddSubMenu("Laneclear");
            laneClear.Add("laneUseW", new CheckBox("Use W"));
            laneClear.Add("laneUseE", new CheckBox("Use E"));
            laneClear.Add("minWTargetsLC", new Slider("Minimum targets for W", 4, 1, 10));
            laneClear.Add("minETargetsLC", new Slider("Minimum targets for E", 4, 1, 10));
            laneClear.Add("minMana", new Slider("Mana"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("countQ", new Slider("Auto Q if x enemies are going in your direction 0-disable", 3, 0, 5));
            qMenu.Add("autoQ", new CheckBox("Auto Q in combo"));
            qMenu.Add("recallSafe", new CheckBox("Safe Q recall (Press B)"));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("AutoW"));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("Eks", new CheckBox("E ks"));
            eMenu.Add("countE", new Slider("Auto E if x stacks & out range AA", 6, 0, 6));
            eMenu.Add("5e", new CheckBox("Always E if 6 stacks"));
            eMenu.Add("jungleE", new CheckBox("Jungle ks E"));
            eMenu.Add("Edead", new CheckBox("Cast E before Twitch die"));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("Rks", new CheckBox("R KS out range AA"));
            rMenu.Add("countR", new Slider("Auto R if x enemies (combo)", 3, 0, 5));


            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public static Vector3 _PlayerPos
        {
            get { return ObjectManager.Player.Position; }
        }

        public static float PlayerMana
        {
            get { return ObjectManager.Player.ManaPercent; }
        }

        public static int EStacks(Obj_AI_Base obj)
        {
            var twitchECount = 0;
            for (var i = 1; i < 7; i++)
            {
                if (ObjectManager.Get<Obj_GeneralParticleEmitter>()
                .Any(e => e.Position.LSDistance(obj.ServerPosition) <= 55 &&
                e.Name == "twitch_poison_counter_0" + i + ".troy"))
                {
                    twitchECount = i;
                }
            }
            return twitchECount;
        }

        public static void Execute()
        {
            if (getCheckBoxItem(laneClear, "laneUseE") && E.IsReady() && PlayerMana >= getSliderItem(laneClear, "minMana"))
            {
                var minionCount = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _PlayerPos, E.Range) .Count(m => m.LSIsValidTarget() && EStacks(m) >= 1);
                if (minionCount >= getSliderItem(laneClear, "MinETargetsLC"))
                {
                    E.Cast();
                }
            }
            if (getCheckBoxItem(laneClear, "laneUseW") && W.IsReady() && PlayerMana >= getSliderItem(laneClear, "minMana"))
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _PlayerPos, W.Range).Where(m => m.LSIsValidTarget());
                var position = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, W.Width, (int)W.Range);
                if (position.HitNumber >= getSliderItem(laneClear, "minWTargetsLC"))
                {
                    W.Cast(position.CastPosition);
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Recall && Q.IsReady() && getCheckBoxItem(qMenu, "recallSafe"))
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Q);
                Utility.DelayAction.Add(200, () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall));
                args.Process = false;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.Farm)
            {
                Execute();
            }

            if (Program.LagFree(1) && E.IsReady())
                LogicE();
            if (Program.LagFree(2) && Q.IsReady() && !Orbwalker.IsAutoAttacking)
                LogicQ();
            if (Program.LagFree(3) && getCheckBoxItem(wMenu, "autoW") && W.IsReady() && !Orbwalker.IsAutoAttacking)
                LogicW();
            if (Program.LagFree(4) && R.IsReady() && Program.Combo)
                LogicR();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (getCheckBoxItem(eMenu, "Edead") && E.IsReady() && sender.IsEnemy && sender.LSIsValidTarget(1500))
            {
                double dmg = 0;

                if (args.Target != null && args.Target.IsMe)
                {
                    dmg = dmg + sender.LSGetSpellDamage(Player, args.SData.Name);
                }
                else
                {
                    var castArea = Player.LSDistance(args.End)*(args.End - Player.ServerPosition).LSNormalized() +
                                   Player.ServerPosition;
                    if (castArea.LSDistance(Player.ServerPosition) < Player.BoundingRadius/2)
                    {
                        dmg = dmg + sender.LSGetSpellDamage(Player, args.SData.Name);
                    }
                }

                if (Player.Health - dmg < Player.LSCountEnemiesInRange(600)*Player.Level*10)
                {
                    E.Cast();
                }
            }
        }

        private static void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (!Orbwalking.InAutoAttackRange(t) && getCheckBoxItem(rMenu, "Rks") &&
                    Player.LSGetAutoAttackDamage(t)*4 > t.Health)
                    R.Cast();

                if (t.LSCountEnemiesInRange(450) >= getSliderItem(rMenu, "countR") && 0 != getSliderItem(rMenu, "countR"))
                    R.Cast();
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (Program.Combo && Player.Mana > WMANA + RMANA + EMANA &&
                    (Player.LSGetAutoAttackDamage(t)*2 < t.Health || !Orbwalking.InAutoAttackRange(t)))
                    Program.CastSpell(W, t);
                else if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
        }

        private static void LogicQ()
        {
            if (getCheckBoxItem(qMenu, "autoQ") && Program.Combo && Orbwalker.LastTarget.IsValid<AIHeroClient>() &&
                Player.Mana > RMANA + QMANA)
                Q.Cast();

            if (getSliderItem(qMenu, "countQ") == 0 || Player.Mana < RMANA + QMANA)
                return;

            var count = Program.Enemies.Where(enemy => enemy.LSIsValidTarget(3000)).Select(enemy => enemy.GetWaypoints()).Count(waypoints => Player.LSDistance(waypoints.Last().To3D()) < 600);

            if (count >= getSliderItem(qMenu, "countQ"))
                Q.Cast();
        }

        private static int getEBuffCount(Obj_AI_Base obj)
        {
            var twitchECount = 0;
            for (var i = 1; i < 7; i++)
            {
                if (ObjectManager.Get<Obj_GeneralParticleEmitter>()
                    .Any(e => e.Position.LSDistance(obj.ServerPosition) <= 175 &&
                              e.Name == "twitch_poison_counter_0" + i + ".troy"))
                {
                    twitchECount = i;
                }
            }
            return twitchECount;
        }

        private static void LogicE()
        {
            foreach (
                var enemy in
                    Program.Enemies.Where(enemy => enemy.LSIsValidTarget(E.Range) && enemy.HasBuff("TwitchDeadlyVenom")))
            {
                if (getCheckBoxItem(eMenu, "Eks") && (E.GetDamage(enemy) + E.GetDamage(enemy, 1)) > enemy.Health)
                {
                    Program.debug("DUPAAA1");
                    E.Cast();
                }

                if (Player.Mana > RMANA + EMANA)
                {
                    var buffNum = getEBuffCount(enemy);
                    if (getCheckBoxItem(eMenu, "5e") && buffNum == 6)
                    {
                        Program.debug("DUPAAA2");
                        E.Cast();
                    }

                    var buffTime = OktwCommon.GetPassiveTime(enemy, "twitchdeadlyvenom");
                    if (!Orbwalking.InAutoAttackRange(enemy) && (Player.ServerPosition.LSDistance(enemy.ServerPosition) > 950 || buffTime < 1) && 0 < getSliderItem(eMenu, "countE") && buffNum >= getSliderItem(eMenu, "countE"))
                    {
                        Program.debug("DUPAAA3 " + buffTime);
                        E.Cast();
                    }
                }
            }
            JungleE();
        }

        private static float passiveDmg(Obj_AI_Base target)
        {
            if (!target.HasBuff("TwitchDeadlyVenom"))
                return 0;
            float dmg = 6;
            if (Player.Level < 17)
                dmg = 5;
            if (Player.Level < 13)
                dmg = 4;
            if (Player.Level < 9)
                dmg = 3;
            if (Player.Level < 5)
                dmg = 2;
            var buffTime = OktwCommon.GetPassiveTime(target, "TwitchDeadlyVenom");
            return dmg*OktwCommon.GetBuffCount(target, "TwitchDeadlyVenom")*buffTime - target.HPRegenRate*buffTime;
        }

        private static void JungleE()
        {
            if (!getCheckBoxItem(eMenu, "jungleE") || Player.Mana < RMANA + EMANA)
                return;

            var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (E.IsKillable(mob))
                {
                    Program.debug("DUPAAA");
                    E.Cast();
                }
            }
        }

        private static void SetMana()
        {
            if ((Program.getCheckBoxItem("manaDisable") && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = EMANA - Player.PARRegenRate*E.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawText(string msg, AIHeroClient Hero, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1], color, msg);
        }

        public static void drawText(string msg, Vector3 Hero, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1] - 200, color, msg);
        }

        public static void drawText2(string msg, Vector3 Hero, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1] - 200, color, msg);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "notif"))
            {
                if (Player.HasBuff("TwitchHideInShadows"))
                    drawText2(
                        "Q:  " + string.Format("{0:0.0}", OktwCommon.GetPassiveTime(Player, "TwitchHideInShadows")),
                        Player.Position, Color.Yellow);
                if (Player.HasBuff("twitchhideinshadowsbuff"))
                    drawText2(
                        "Q AS buff:  " +
                        string.Format("{0:0.0}", OktwCommon.GetPassiveTime(Player, "twitchhideinshadowsbuff")),
                        Player.Position, Color.YellowGreen);
                if (Player.HasBuff("TwitchFullAutomatic"))
                    drawText2(
                        "R ACTIVE:  " +
                        string.Format("{0:0.0}", OktwCommon.GetPassiveTime(Player, "TwitchFullAutomatic")),
                        Player.Position, Color.OrangeRed);
            }

            foreach (
                var enemy in
                    Program.Enemies.Where(enemy => enemy.LSIsValidTarget(2000) && enemy.HasBuff("TwitchDeadlyVenom")))
            {
                if (passiveDmg(enemy) > enemy.Health)
                    drawText("IS DEAD", enemy, Color.Yellow);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }
    }
}