using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Slutty_ryze
{
    class Champion
    {
        #region Variable Declaration
        private static SpellSlot _ignite;
        public static readonly AIHeroClient Player = ObjectManager.Player;
        private const string _champName = "Ryze";
        //private static Spell _q, _w, _e, _r, _qn;
        // Does not work as a property o-o
        public static LeagueSharp.Common.Spell Q, W, E, R, Qn;
        #endregion
        #region Public Properties
        //public static Spell Q
        //{
        //    get { return _q; }
        //    set { _q = value; }
        //}

        //public static Spell QN
        //{
        //    get { return _qn; }
        //    set { _qn = value; }
        //}
        //public static Spell W
        //{
        //    get { return _w; }
        //    set { _w = value; }
        //}
        //public static Spell WE
        //{
        //    get { return _e; }
        //    set { _e = value; }
        //}
        //public static Spell R
        //{
        //    get { return _r; }
        //    set { _r = value; }
        //}
        public static string ChampName
        {
            get
            {
                return _champName;
            }
        }
        #endregion
        #region Public Functions
        public static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        public static SpellSlot GetIgniteSlot()
        {
            return _ignite;
        }

        public static void SetIgniteSlot(SpellSlot nSpellSlot)
        {
            _ignite = nSpellSlot;
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (Q.IsReady() || Player.Mana <= Q.Instance.SData.Mana)
                return Q.GetDamage(enemy);

            if (E.IsReady() || Player.Mana <= E.Instance.SData.Mana)
                return E.GetDamage(enemy);

            if (W.IsReady() || Player.Mana <= W.Instance.SData.Mana)
                return W.GetDamage(enemy);

            return 0;
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


        public static void AutoPassive()
        {
            var minions = MinionManager.GetMinions(
                GlobalManager.GetHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (GlobalManager.GetHero.Mana < getSliderItem(MenuManager.passiveMenu, "ManapSlider")) return;

            //Maybe check if any minons can be killed?
            //foreach(var minion in minions)
            //if(minion.Headth < Champion.Q.GetDamage)
            //break;

            if (GlobalManager.GetHero.IsRecalling()) return;

            if (minions.Count >= 1) return;

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target != null) return;

            var stackSliders = getSliderItem(MenuManager.passiveMenu, "stackSlider");
            if (GlobalManager.GetHero.InFountain()) return;

            if (GlobalManager.GetPassiveBuff >= stackSliders)
                return;

            if (Utils.TickCount - Q.LastCastAttemptT >=
                getSliderItem(MenuManager.passiveMenu, "autoPassiveTimer") * 1000 - (100 + (Game.Ping / 2)) &&
                Q.IsReady())
            {
                if (!Game.CursorPos.IsZero)
                    Q.Cast(Game.CursorPos);
            }
        }



        //        public static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        //        {
        //            if (!sender.IsEnemy) return;
        //
        //            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);
        //            var qSpell = GlobalManager.Config.Item("useQW2D");
        //
        //            if (sender.NetworkId != target.NetworkId) return;
        //            if (!qSpell) return;
        //            if (!Champion.Q.IsReady() || !(args.EndPos.Distance(GlobalManager.GetHero) < Champion.Q.Range)) return;
        //            var delay = (int)(args.EndTick - Game.Time - Champion.Q.Delay - 0.1f);
        //
        //            if (delay > 0)
        //                Utility.DelayAction.Add(delay * 1000, () => Champion.Q.Cast(args.EndPos));
        //            else
        //                Champion.Q.Cast(args.EndPos);
        //
        //            if (!Champion.Q.IsReady() || !(args.EndPos.Distance(GlobalManager.GetHero) < Champion.Q.Range)) return;
        //
        //            if (delay > 0)
        //                Utility.DelayAction.Add(delay * 1000, () => Champion.Q.Cast(args.EndPos));
        //            else
        //                Champion.W.CastOnUnit(target);
        //        }
        public static void AABlock()
        {
            Orbwalker.DisableAttacking = (getCheckBoxItem(MenuManager.combo1Menu, "AAblock"));
        }

        public static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            var qSpell = getCheckBoxItem(MenuManager.ksMenu, "useQ2KS");
            var wSpell = getCheckBoxItem(MenuManager.ksMenu, "useW2KS");
            var eSpell = getCheckBoxItem(MenuManager.ksMenu, "useE2KS");
            if (qSpell
                && Q.GetDamage(target) > target.Health
                && target.IsValidTarget(Q.Range))
                Q.Cast(target);

            if (wSpell
                && W.GetDamage(target) > target.Health
                && target.IsValidTarget(W.Range))
                W.Cast(target);

            if (eSpell
                && E.GetDamage(target) > target.Health
                && target.IsValidTarget(E.Range))
                E.Cast(target);
        }

        public static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (W.IsReady() && W.Level > 0 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }

            var mura = getCheckBoxItem(MenuManager.itemMenu, "muramana");

            if (!mura) return;

            var muramanai = Items.HasItem(ItemManager.Muramana) ? 3042 : 3043;

            if (!args.Target.IsValid<AIHeroClient>() || !args.Target.IsEnemy || !Items.HasItem(muramanai) ||
                !Items.CanUseItem(muramanai))
                return;

            if (!GlobalManager.GetHero.HasBuff("Muramana"))
                Items.UseItem(muramanai);
        }

        #endregion


        internal static void RyzeInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var wSpell = getCheckBoxItem(MenuManager.eventMenu, "useW2I");
            if (!wSpell || !sender.LSIsValidTarget(W.Range)) return;
            W.Cast(sender);
        }

        internal static void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.LSDistance(Player.ServerPosition) < W.Range && getCheckBoxItem(MenuManager.eventMenu, "useQW2D"))
            {
                W.Cast(gapcloser.Sender);
            }
        }
    }
}
