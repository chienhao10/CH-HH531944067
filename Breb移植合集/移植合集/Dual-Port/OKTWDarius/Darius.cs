using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Darius
    {
        private static Menu Config = Program.Config;
        public static LeagueSharp.Common.Spell Q, W, E, R;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 400);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 145);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 540);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 460);

            E.SetSkillshot(0.01f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);

            LoadMenuOKTW();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += afterAttack;
            Interrupter.OnPossibleToInterrupt += OnInterruptableSpell;
        }

        public static Menu drawMenu, qMenu, rMenu, farmMenu;

        private static void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw when skill rdy", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("haras", new CheckBox("Harras Q", true));
            qMenu.Add("qOutRange", new CheckBox("Auto Q only out range AA", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("autoRbuff", new CheckBox("Auto R if darius execute multi cast time out ", true));
            rMenu.Add("autoRdeath", new CheckBox("Auto R if darius execute multi cast and under 10 % hp", true));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmW", new CheckBox("Farm W", true));
            farmMenu.Add("farmQ", new CheckBox("Farm Q", true));
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

        private static void OnInterruptableSpell(AIHeroClient unit, InterruptableSpell spell)
        {
            if (E.IsReady() && unit.IsValidTarget(E.Range))
                E.Cast(unit);
        }


        private static void afterAttack(AttackableUnit target, EventArgs args)
        {
            if (Player.Mana < RMANA + WMANA || !W.IsReady())
                return;

            var t = target as AIHeroClient;

            if (t.IsValidTarget())
                W.Cast();

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (R.IsReady() && getKeyBindItem(rMenu, "useR"))
            {
                var targetR = TargetSelector.GetTarget(R.Range, DamageType.True);
                if (targetR.IsValidTarget())
                    R.Cast(targetR, true);
            }

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.LagFree(1) && W.IsReady())
                LogicW();
            if (Program.LagFree(2) && Q.IsReady())
                LogicQ();
            if (Program.LagFree(3) && E.IsReady())
                LogicE();
            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
        }

        private static void LogicW()
        {
            if (!Player.Spellbook.IsAutoAttacking && getCheckBoxItem(farmMenu, "farmW") && Program.Farm)
            {
                var minions = Cache.GetMinions(Player.Position, Player.AttackRange);

                int countMinions = 0;

                foreach (var minion in minions.Where(minion => minion.Health < W.GetDamage(minion)))
                {
                    countMinions++;
                }

                if (countMinions > 0)
                    W.Cast();
            }
        }

        private static void LogicE()
        {
            if (Player.Mana > RMANA + EMANA)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target.IsValidTarget() && ((Player.UnderTurret(false) && !Player.UnderTurret(true)) || Program.Combo))
                {
                    if (!SebbyLib.Orbwalking.InAutoAttackRange(target))
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (!getCheckBoxItem(qMenu, "qOutRange") || SebbyLib.Orbwalking.InAutoAttackRange(t))
                {
                    if (Player.Mana > RMANA + QMANA && Program.Combo)
                        Q.Cast();
                    else if (Program.Farm && Player.Mana > RMANA + QMANA + EMANA + WMANA && getCheckBoxItem(qMenu, "haras"))
                        Q.Cast();
                }

                if (!R.IsReady() && OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Q.Cast();
            }

            else if (getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA + EMANA + WMANA && Program.LaneClear)
            {
                var minionsList = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (minionsList.Any(x => Player.Distance(x.ServerPosition) > 300 && x.Health < Q.GetDamage(x) * 0.6))
                    Q.Cast();

            }
        }

        private static void LogicR()
        {
            var targetR = TargetSelector.GetTarget(R.Range, DamageType.True);
            if (targetR.IsValidTarget() && OktwCommon.ValidUlt(targetR) && getCheckBoxItem(rMenu, "autoRbuff"))
            {
                var buffTime = OktwCommon.GetPassiveTime(Player, "dariusexecutemulticast");
                if ((buffTime < 2 || (Player.HealthPercent < 10 && getCheckBoxItem(rMenu, "autoRdeath"))) && buffTime > 0)
                    R.Cast(targetR, true);
            }

            foreach (var target in Program.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
            {

                var dmgR = OktwCommon.GetKsDamage(target, R);
                if (target.HasBuff("dariushemo"))
                    dmgR += R.GetDamage(target) * target.GetBuff("dariushemo").Count * 0.2f;

                if (dmgR > target.Health + target.HPRegenRate)
                {
                    R.Cast(target);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy") && Q.IsReady())
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                    else
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy") && E.IsReady())
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
                    else
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy") && R.IsReady())
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Red, 1, 1);
                    else
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Red, 1, 1);
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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

    }
}