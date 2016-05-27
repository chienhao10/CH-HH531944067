using System;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using EloBuddy;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using Damage = LeagueSharp.Common.Damage;

namespace HoolaMasterYi
{
    public class Program
    {
        private static Menu Menu, comboMenu, harassMenu, clearMenu, jungleMenu, miscMenu, drawMenu, ksMenu;
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        private static Spell Q, W, E, R;
        private static int LastAATick;
        private static bool AutoQ => getCheckBoxItem(miscMenu, "AutoQ");
        private static bool AutoQOnly => getKeyBindItem(miscMenu, "AutoQOnly");
        private static bool KsQ => getCheckBoxItem(ksMenu, "KsQ");
        private static bool KsT => getCheckBoxItem(ksMenu, "KsT");
        private static bool KsB => getCheckBoxItem(ksMenu, "KsB");
        private static bool CQ => getCheckBoxItem(comboMenu, "CQ");
        private static bool CW => getCheckBoxItem(comboMenu, "CW");
        private static bool CE => getCheckBoxItem(comboMenu, "CE");
        private static bool CR => getCheckBoxItem(comboMenu, "CR");
        private static bool CT => getCheckBoxItem(comboMenu, "CT");
        private static bool CY => getCheckBoxItem(comboMenu, "CY");
        private static bool CB => getCheckBoxItem(comboMenu, "CB");
        private static bool HQ => getCheckBoxItem(harassMenu, "HQ");
        private static bool HW => getCheckBoxItem(harassMenu, "HW");
        private static bool HE => getCheckBoxItem(harassMenu, "HE");
        private static bool HT => getCheckBoxItem(harassMenu, "HT");
        private static bool HY => getCheckBoxItem(harassMenu, "HY");
        private static bool HB => getCheckBoxItem(harassMenu, "HB");
        private static bool LW => getCheckBoxItem(clearMenu, "LW");
        private static bool LE => getCheckBoxItem(clearMenu, "LE");
        private static bool LI => getCheckBoxItem(clearMenu, "LI");
        private static bool JQ => getCheckBoxItem(jungleMenu, "JQ");
        private static bool JW => getCheckBoxItem(jungleMenu, "JW");
        private static bool JE => getCheckBoxItem(jungleMenu, "JE");
        private static bool JI => getCheckBoxItem(jungleMenu, "JI");
        private static bool AutoY => getCheckBoxItem(miscMenu, "AutoY");
        private static bool DQ => getCheckBoxItem(drawMenu, "DQ");
        private static bool Dind => getCheckBoxItem(drawMenu, "Dind");

       public static void OnGameLoad()
        {
            Chat.Print("Hoola Master Yi - Loaded Successfully, Good Luck! :)");
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            OnMenuLoad();

            Q.SetTargetted(0.25f, float.MaxValue);

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += DetectSpell;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            Obj_AI_Base.OnPlayAnimation += OnPlay;
            Spellbook.OnCastSpell += OnCast;
            Obj_AI_Base.OnSpellCast += OnDoCastJC;
            Obj_AI_Base.OnProcessSpellCast += BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += BeforeAttackJC;
            Obj_AI_Base.OnProcessSpellCast += DetectBlink;
            Drawing.OnDraw += OnDraw;
        }

        private static void DetectBlink(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe || args.SData.IsAutoAttack()) return;

            if (Spelldatabase.list.Contains(args.SData.Name.ToLower()) &&
                (((Player.LSDistance(args.End) >= Q.Range) && AutoQOnly) || !AutoQOnly) && Q.IsReady() && AutoQ)
                Q.Cast((Obj_AI_Base)args.Target);
        }

        private static void DetectSpell(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target.LSIsDashing() && (((Player.LSDistance(target.GetWaypoints().Last()) >= Q.Range) && AutoQOnly) || !AutoQOnly) && Q.IsReady() && AutoQ && Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo || Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass)
                Q.Cast(target);
        }

        static void OnDraw(EventArgs args)
        {
            if (DQ) Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
        }

        static void BeforeAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !args.Target.IsValid || !Orbwalking.IsAutoAttack(args.SData.Name)) return;

            if (args.Target is AIHeroClient)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (CR) R.Cast();
                    if (CY) CastYoumoo();
                    if (CE) E.Cast();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (HY) CastYoumoo();
                    if (HE) E.Cast();
                }
            }
            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    var Minions = MinionManager.GetMinions(ItemData.Ravenous_Hydra_Melee_Only.Range);
                    if (Minions[0].IsValid && Minions.Count != 0) if (LE) E.Cast();
                }
            }
        }

        static void BeforeAttackJC(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !args.Target.IsValid || !Orbwalking.IsAutoAttack(args.SData.Name)) return;

            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    var Mobs = MinionManager.GetMinions(ItemData.Ravenous_Hydra_Melee_Only.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    if (Mobs[0].IsValid && Mobs.Count != 0) if (JE) E.Cast();
                }
            }
        }

        private static void OnCast(Spellbook Sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R && AutoY) CastYoumoo();
        }

        private static void OnPlay(Obj_AI_Base Sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!Sender.IsMe) return;
            if (args.Animation.Contains("Spell2"))
            {
                LastAATick = 0;
            }
        }

        static void UseCastItem(int t)
        {
            for (int i = 0; i < t; i = i + 1)
            {
                if (HasItem)
                    Utility.DelayAction.Add(i, CastItem);
            }
        }
        static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
        }
        static void CastYoumoo()
        {
            if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                ItemData.Youmuus_Ghostblade.GetItem().Cast();
        }
        static void CastBOTRK(AIHeroClient target)
        {
            if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady())
                ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
            if (ItemData.Bilgewater_Cutlass.GetItem().IsReady())
                ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
        }
        static bool HasItem => (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady());


        private static void OnDoCast(Obj_AI_Base Sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Sender.IsMe || !args.Target.IsValid && !Orbwalking.IsAutoAttack(args.SData.Name)) return;

            if (args.Target is AIHeroClient && args.Target.IsValid)
            {
                var target = (AIHeroClient)args.Target;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (CB) CastBOTRK(target);
                    if (CT) UseCastItem(300);
                    if (CW) Utility.DelayAction.Add(1, () => W.Cast());
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (HB) CastBOTRK(target);
                    if (HT) UseCastItem(300);
                    if (HW) Utility.DelayAction.Add(1, () => W.Cast());
                }
            }
            if (args.Target is Obj_AI_Minion && args.Target.IsValid)
            {
                var Minions = MinionManager.GetMinions(ItemData.Ravenous_Hydra_Melee_Only.Range);
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    if (Minions.Count != 0 && Minions[0].IsValid)
                    {
                        if (LI) UseCastItem(300);
                        if (LW) Utility.DelayAction.Add(1, () => W.Cast());
                    }
                }
            }

        }
        private static void OnDoCastJC(Obj_AI_Base Sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Sender.IsMe || !args.Target.IsValid && !Orbwalking.IsAutoAttack(args.SData.Name)) return;

            if (args.Target is Obj_AI_Minion && args.Target.IsValid)
            {
                var Mobs = MinionManager.GetMinions(ItemData.Ravenous_Hydra_Melee_Only.Range, MinionTypes.All, MinionTeam.Neutral);
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    if (Mobs[0].IsValid && Mobs.Count != 0)
                    {
                        if (Q.IsReady() && JQ) Q.Cast(Mobs[0]);
                        if (!Q.IsReady() || (Q.IsReady() && !JQ))
                        {
                            if (JI) UseCastItem(300);
                            if (JW) Utility.DelayAction.Add(1, () => W.Cast());
                        }
                    }
                }
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

        private static void OnMenuLoad()
        {

            Menu = MainMenu.AddMenu("Hoola Master Yi", "hoolamasteryi");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("CQ", new CheckBox("Use Q", true));
            comboMenu.Add("CW", new CheckBox("Use W", true));
            comboMenu.Add("CE", new CheckBox("Use E", true));
            comboMenu.Add("CR", new CheckBox("Use R", false));
            comboMenu.Add("CT", new CheckBox("Use Tiamat/Hydra", true));
            comboMenu.Add("CY", new CheckBox("Use Youumu", true));
            comboMenu.Add("CB", new CheckBox("Use BORK", false));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("HQ", new CheckBox("Use Q", true));
            harassMenu.Add("HW", new CheckBox("Use W", true));
            harassMenu.Add("HE", new CheckBox("Use E", true));
            harassMenu.Add("HT", new CheckBox("Use Tiamat/Hydra", true));
            harassMenu.Add("HY", new CheckBox("Use Youumu", true));
            harassMenu.Add("HB", new CheckBox("Use BORK", true));

            clearMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
            clearMenu.Add("LW", new CheckBox("Use W", false));
            clearMenu.Add("LE", new CheckBox("Use E", false));
            clearMenu.Add("LI", new CheckBox("Use Tiamat/Hydra", false));

            jungleMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
            jungleMenu.Add("JQ", new CheckBox("Use Q", true));
            jungleMenu.Add("JW", new CheckBox("Use W", false));
            jungleMenu.Add("JE", new CheckBox("Use E", true));
            jungleMenu.Add("JI", new CheckBox("Use Tiamat/Hydra", true));

            ksMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
            ksMenu.Add("KsQ", new CheckBox("Ks Q", true));
            ksMenu.Add("KsT", new CheckBox("Ks Tiamat/Hydra", true));
            ksMenu.Add("KsB", new CheckBox("Ks BOTRK", false));

            drawMenu = Menu.AddSubMenu("Draw", "Draw");
            drawMenu.Add("Dind", new CheckBox("Draw Damage Indicator", true));
            drawMenu.Add("DQ", new CheckBox("Draw Q", true));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("AutoQ", new CheckBox("Q Follow Dashing Target", true));
            miscMenu.Add("AutoQOnly", new KeyBind("Follow Q If Will Can't Q Only", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("AutoY", new CheckBox("Use Youumus While R", true));

        }

        static void killsteal()
        {
            if (KsQ && Q.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.IsValid && target.Health < Q.GetDamage(target) && (!target.HasBuff("kindrednodeathbuff") || !target.HasBuff("Undying Rage") || !target.HasBuff("JudicatorIntervention")) && (!Orbwalking.InAutoAttackRange(target) || !Orbwalker.CanAutoAttack))
                        Q.Cast(target);
                }
            }
            if (KsB &&
                (ItemData.Bilgewater_Cutlass.GetItem().IsReady() ||
                 ItemData.Blade_of_the_Ruined_King.GetItem().IsReady()))
            {
                var targets =
                    HeroManager.Enemies.Where(
                        x => x.LSIsValidTarget(ItemData.Blade_of_the_Ruined_King.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Damage.GetItemDamage(Player, target, Damage.DamageItems.Bilgewater)) ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
                    if (target.Health < Damage.GetItemDamage(Player, target, Damage.DamageItems.Botrk)) ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
                }
            }
            if (KsT &&
                (ItemData.Tiamat_Melee_Only.GetItem().IsReady() ||
                 ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady()))
            {
                var targets =
                    HeroManager.Enemies.Where(
                        x => x.LSIsValidTarget(ItemData.Ravenous_Hydra_Melee_Only.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Damage.GetItemDamage(Player, target, Damage.DamageItems.Tiamat)) ItemData.Tiamat_Melee_Only.GetItem().Cast();
                    if (target.Health < Damage.GetItemDamage(Player, target, Damage.DamageItems.Hydra)) ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                }
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            killsteal();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && CQ) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && HQ) Harass();
        }

        static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Q.IsReady() && target.IsValid) Q.Cast(target);
        }

        static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Q.IsReady() && target.IsValid) Q.Cast(target);
        }

        static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;

                if (Q.IsReady())
                    damage += Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy, true);

                if (E.IsReady())
                    damage += E.GetDamage(enemy);

                if (W.IsReady())
                    damage += (float)Player.GetAutoAttackDamage(enemy, true);

                if (!Player.Spellbook.IsAutoAttacking)
                    damage += (float)Player.GetAutoAttackDamage(enemy, true);

                return damage;
            }
            return 0;
        }

        static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (Dind)
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(getComboDamage(enemy), new ColorBGRA(255, 204, 0, 160));
                }


            }
        }
    }
}