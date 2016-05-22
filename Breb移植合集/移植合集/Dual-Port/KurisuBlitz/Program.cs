using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu.Values;

namespace KurisuBlitzcrank
{
    class Program
    {
        // Keepo
        private static Menu Menu { get; set; }

        public static Menu keyMenu, qsmenu, comenu, drmenu, fmenu, exmenu;
        internal static Random Rand;
        internal static Spell Q, W, E, R;
        internal static AIHeroClient Player => ObjectManager.Player;


        internal static int Limiter;
        internal static int LastFlash;
        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Blitzcrank")
            {
                return;
            }

            Rand = new Random();
            Q = new Spell(SpellSlot.Q, 950f);
            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 155f);
            R = new Spell(SpellSlot.R, 545f);


            Menu = MainMenu.AddMenu("Blitzcrank", "D-blitzcrank");

            keyMenu = Menu.AddSubMenu("Keys", "kemenu");
            keyMenu.Add("grabkey", new KeyBind("Grab [active]", false, KeyBind.BindTypes.HoldActive, 'T'));


            qsmenu = Menu.AddSubMenu("Auto Grab", "auqmenu");
            foreach (var hero in HeroManager.Enemies)
            qsmenu.Add("auq" + hero.NetworkId, new CheckBox("Auto grab: " + hero.ChampionName + " (Dashing/Immobile/Casting)"));
            qsmenu.AddSeparator();
            qsmenu.Add("pred", new Slider("Hitchance", 5, 1, 4));
            qsmenu.Add("fpred", new Slider("Flash Hitchance", 4, 1, 4));
            qsmenu.Add("maxq", new Slider("Maximum Q Range", (int) Q.Range, 100, (int)Q.Range));
            qsmenu.Add("minq", new Slider("Minimum Q Range", 420, 100, (int)Q.Range));
            qsmenu.Add("grabhp", new Slider("Dont grab if below HP%", 0, 0, 100));
            qsmenu.AddSeparator();
            qsmenu.AddLabel("Q Blacklist");
            foreach (var hero in HeroManager.Enemies)
            qsmenu.Add("blq" + hero.NetworkId, new CheckBox("Blacklist: " + hero.ChampionName, false));


            comenu = Menu.AddSubMenu("Combo", "cmenu");
            comenu.Add("useqcombo", new CheckBox("Use Q"));
            comenu.Add("useecombo", new CheckBox("Use E"));
            comenu.Add("usercombo", new CheckBox("Use R"));

            fmenu = Menu.AddSubMenu("Flee", "fmenu");
            fmenu.Add("usewflee", new CheckBox("Use W"));
            fmenu.Add("useeflee", new CheckBox("Use E"));

            exmenu = Menu.AddSubMenu("Extra", "exmenu");
            exmenu.Add("int", new CheckBox("Interrupt"));
            exmenu.Add("supp", new CheckBox("Support"));

            drmenu = Menu.AddSubMenu("Drawings", "drmenu");
            drmenu.Add("drawq", new CheckBox("Draw Q"));
            drmenu.Add("drawr", new CheckBox("Draw R"));




            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Chat.Print("<b>Blitzcrank#</b> - Loaded!");

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

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drmenu, "drawq"))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.LawnGreen : Color.Red, 2);
            }

            if (getCheckBoxItem(drmenu, "drawr"))
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.LawnGreen : Color.Red, 2);
            }
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (getCheckBoxItem(exmenu, "supp"))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    var minion = args.Target as Obj_AI_Base;
                    if (minion != null && minion.IsMinion && minion.IsValidTarget())
                    {
                        if (HeroManager.Allies.Any(x => x.IsValidTarget(1000, false) && !x.IsMe))
                        {
                            if (Player.HasBuff("talentreaperdisplay"))
                            {
                                var b = Player.GetBuff("talentreaperdisplay");
                                if (b.Count > 0)
                                {
                                    args.Process = true;
                                    return;
                                }
                            }

                            args.Process = false;
                        }
                    }
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && sender.IsValidTarget() && getCheckBoxItem(exmenu, "int"))
            {
                if (R.IsReady() && Player.Distance(sender.ServerPosition) <= R.Range)
                {
                    if (args.DangerLevel >= Interrupter2.DangerLevel.High)
                    {
                        R.Cast();
                    }
                }

                if (Q.IsReady() && Player.Distance(sender.ServerPosition) <= getSliderItem(qsmenu, "maxq"))
                {
                    if (Player.HealthPercent < getSliderItem(qsmenu, "grabhp"))
                    {
                        return;
                    }

                    if (!getCheckBoxItem(qsmenu, "blq" + sender.NetworkId) &&
                        Player.Distance(sender.ServerPosition) > getSliderItem(qsmenu, "minq"))
                    {
                        Q.Cast(sender);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "summonerflash")
            {
                LastFlash = Utils.GameTimeTickCount;
            }

            var hero = sender as AIHeroClient;
            if (hero != null && hero.IsEnemy && Q.IsReady() && getCheckBoxItem(comenu, "useqcombo"))
            {
                if (Player.HealthPercent < getSliderItem(qsmenu, "grabhp"))
                {
                    return;
                }

                if (hero.IsValidTarget(getSliderItem(qsmenu, "maxq")) && hero.Health > Q.GetDamage(hero))
                {
                    if (!getCheckBoxItem(qsmenu, "blq" + hero.NetworkId) &&
                         getCheckBoxItem(qsmenu, "auq" + hero.NetworkId))
                    {
                        if (hero.Distance(Player.ServerPosition) > getSliderItem(qsmenu, "minq"))
                        {
                            Q.CastIfHitchanceEquals(hero, HitChance.VeryHigh);
                        }
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            Grab(getKeyBindItem(keyMenu, "grabkey"));
            Flee(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee));

            Combo(getCheckBoxItem(comenu, "useqcombo"), getCheckBoxItem(comenu, "useecombo"),
                  getCheckBoxItem(comenu, "usercombo"));

            Secure(!getCheckBoxItem(exmenu, "supp"), !getCheckBoxItem(exmenu, "supp"));


            foreach (var ene in HeroManager.Enemies.Where(x => x.IsValidTarget(getSliderItem(qsmenu, "maxq"))))
            {
                if (Player.HealthPercent < getSliderItem(qsmenu, "grabhp"))
                {
                    return;
                }

                if (!getCheckBoxItem(qsmenu, "blq" + ene.NetworkId) &&
                     getCheckBoxItem(qsmenu, "auq" + ene.NetworkId))
                {
                    if (ene.Distance(Player.ServerPosition) > getSliderItem(qsmenu, "minq") && Q.IsReady())
                    {
                        Q.CastIfHitchanceEquals(ene, HitChance.Dashing, true);
                        Q.CastIfHitchanceEquals(ene, HitChance.Immobile, true);
                    }
                }
            }
        }

        private static void Flee(bool enable)
        {
            if (!enable)
            {
                return;
            }

            if (W.IsReady())
            {
                W.Cast();
            }

            var ene = HeroManager.Enemies.FirstOrDefault(x => x.Distance(Player.ServerPosition) <= E.Range + 200);
            if (E.IsReady() && ene.IsValidTarget())
            {
                E.Cast();
            }

            if (Player.HasBuff("powerfist") && Orbwalking.InAutoAttackRange(ene))
            {
                if (Utils.GameTimeTickCount - Limiter >= 150 + Game.Ping)
                {
                  EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, ene);
                    Limiter = Utils.GameTimeTickCount;
                }

                return;
            }

            Orbwalker.OrbwalkTo(Game.CursorPos);
        }

        private static void Combo(bool useq, bool usee, bool user)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (useq && Q.IsReady())
            {
                var QT = TargetSelector.GetTarget(getSliderItem(qsmenu, "maxq"), DamageType.Magical);
                if (QT != null && getCheckBoxItem(qsmenu, "blq" + QT.NetworkId))
                {
                    return;
                }

                if (!(Player.HealthPercent < getSliderItem(qsmenu, "grabhp")))
                {
                    if (QT.IsValidTarget() && QT.Distance(Player.ServerPosition) > getSliderItem(qsmenu, "minq"))
                    {
                        if (!QT.IsZombie && !QT.IsInvulnerable)
                        {
                            var poutput = Q.GetPrediction(QT); // prediction output
                            if (Utils.GameTimeTickCount - LastFlash < 1500)
                            {
                                if (poutput.Hitchance == (HitChance) getSliderItem(qsmenu, "fpred") + 2)
                                {
                                    Q.Cast(poutput.CastPosition);
                                }
                            }

                            if (poutput.Hitchance == (HitChance)getSliderItem(qsmenu, "pred") + 2)
                            {
                                Q.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
            }

            if (usee && E.IsReady())
            {
                var ET =
                    HeroManager.Enemies.FirstOrDefault(
                        x => x.HasBuff("rocketgrab2") || x.Distance(Player.ServerPosition) <= E.Range + 200);

                if (ET != null)
                {
                    if (!ET.IsZombie && !ET.IsInvulnerable)
                    {
                        E.Cast();
                    }
                }
            }

            if (user && R.IsReady())
            {
                var RT = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (RT.IsValidTarget() && !RT.IsZombie)
                {
                    if (!RT.IsInvulnerable)
                    {
                        if (RT.Health > R.GetDamage(RT) && !E.IsReady() && RT.HasBuffOfType(BuffType.Knockup))
                        {
                            R.Cast();
                        }
                    }
                }
            }
        }

        private static void Grab(bool enable)
        {
            if (Q.IsReady() && enable)
            {
                var QT = TargetSelector.GetTarget(getSliderItem(qsmenu, "maxq"), DamageType.Magical);
                if (QT != null && getCheckBoxItem(qsmenu, "blq" + QT.NetworkId))
                {
                    return;
                }

                if (!(Player.HealthPercent < getSliderItem(qsmenu, "grabhp")))
                {
                    if (QT.IsValidTarget() && QT.Distance(Player.ServerPosition) > getSliderItem(qsmenu, "minq"))
                    {
                        if (!QT.IsZombie && !QT.IsInvulnerable)
                        {
                            var poutput = Q.GetPrediction(QT); // prediction output
                            if (Utils.GameTimeTickCount - LastFlash < 1500)
                            {
                                if (poutput.Hitchance == (HitChance) getSliderItem(qsmenu, "fpred") + 2)
                                {
                                    Q.Cast(poutput.CastPosition);
                                }
                            }

                            if (poutput.Hitchance == (HitChance) getSliderItem(qsmenu, "pred") + 2)
                            {
                                Q.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void Secure(bool useq, bool user)
        {
            if (useq && Q.IsReady())
            {
                var QT = HeroManager.Enemies.FirstOrDefault(x => Q.GetDamage(x) > x.Health);
                if (QT.IsValidTarget(getSliderItem(qsmenu, "maxq")))
                {
                    var poutput = Q.GetPrediction(QT); // prediction output
                    if (poutput.Hitchance >= (HitChance)getSliderItem(qsmenu, "pred") + 2)
                    {
                        if (!QT.IsZombie && !QT.IsInvulnerable)
                        {
                            Q.Cast(poutput.CastPosition);
                        }
                    }
                }
            }

            if (user && R.IsReady())
            {
                var RT = HeroManager.Enemies.FirstOrDefault(x => R.GetDamage(x) > x.Health);
                if (RT.IsValidTarget(R.Range) && !RT.IsZombie)
                {
                    if (!RT.IsInvulnerable)
                    {
                        R.Cast();
                    }
                }
            }
        }
    }
}
