using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using CM = KurisuNidalee.CastManager;
using Color = System.Drawing.Color;
using KL = KurisuNidalee.KurisuLib;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace KurisuNidalee
{
    internal class KurisuNidalee
    {
        internal static Menu Root;
        internal static AIHeroClient Target;

        public static Menu qHMenu,
            wHMenu,
            eHMenu,
            rHMenu,
            qCMenu,
            wCMenu,
            eCMenu,
            rCMenu,
            drawMenu,
            jungleMenu,
            autoMenu;

        internal static bool m;

        internal static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Nidalee")
            {
                return;
            }

            #region Root Menu

            Root = MainMenu.AddMenu("Kurisu's Nidalee", "nidalee");
            Root.Add("pstyle", new ComboBox(":: Play Style", 1, "Single Target", "Multi-Target"));
            Root.Add("usecombo2", new KeyBind(":: Tripple AA [beta]", false, KeyBind.BindTypes.HoldActive, 'Z'));
            Root.Add("flee", new KeyBind(":: Flee/Walljumper [active]", false, KeyBind.BindTypes.HoldActive, 'A'));
            Root.Add("ppred", new ComboBox(":: Prediction (F5 on Change)", 1, "Common", "OKTW"));
            Root.Add("ndhqch", new ComboBox("-> Min Hitchance", 2, "Low", "Medium", "High", "Very High"));

            # region Human

            qHMenu = Root.AddSubMenu("(Q)  Javelin", "ndhq");
            qHMenu.Add("ndhqcheck", new CheckBox("Check Hitchance"));
            qHMenu.Add("qsmcol", new CheckBox("-> Smite Collision", false));
            qHMenu.Add("ndhqco", new CheckBox("Enable in Combo"));
            qHMenu.Add("ndhqha", new CheckBox("Enable in Harass"));
            qHMenu.Add("ndhqjg", new CheckBox("Enable in Jungle"));
            qHMenu.Add("ndhqwc", new CheckBox("Enable in WaveClear", false));

            wHMenu = Root.AddSubMenu("(W) Bushwhack", "ndhw");
            wHMenu.Add("ndhwco", new CheckBox("Enable in Combo", false));
            wHMenu.Add("ndhwsp", new CheckBox("-> Reduce (W) Usage", false));
            wHMenu.Add("ndhwjg", new CheckBox("Enable in Jungle"));
            wHMenu.Add("ndhwwc", new CheckBox("Enable in WaveClear", false));
            wHMenu.Add("ndhwforce", new ComboBox("Location", 0, "Prediction", "Behind Target"));

            eHMenu = Root.AddSubMenu("(E)  Primal Surge", "ndhe");
            eHMenu.Add("ndheon", new CheckBox("Enable Healing"));
            eHMenu.Add("ndhemana", new Slider("-> Minumum Mana", 55, 1));
            eHMenu.Add("ndhesw", new CheckBox("Switch Forms if Heal", false));
            foreach (var hero in HeroManager.Allies)
            {
                eHMenu.Add("xx" + hero.ChampionName, new CheckBox("Heal on " + hero.ChampionName));
                eHMenu.Add("zz" + hero.ChampionName, new Slider(hero.ChampionName + " below Pct% ", 88, 1, 99));
            }
            eHMenu.Add("ndheord", new ComboBox("Ally Priority:", 1, "Low HP", "Most AD/AP", "Max HP"));

            rHMenu = Root.AddSubMenu("(R) Aspect of the Cougar", "ndhr");
            rHMenu.Add("ndhrco", new CheckBox("Enable in Combo"));
            rHMenu.Add("ndhrcreq", new CheckBox("-> Require Swipe/Takedown"));
            rHMenu.Add("ndhrha", new CheckBox("Enable in Harass"));
            rHMenu.Add("ndhrjg", new CheckBox("Enable in Jungle"));
            rHMenu.Add("ndhrjreq", new CheckBox("-> Require Swipe/Takedown"));
            rHMenu.Add("ndhrwc", new CheckBox("Enable in WaveClear", false));

            #endregion

            #region Cougar

            qCMenu = Root.AddSubMenu("(Q) Takedown", "ndcq");
            qCMenu.Add("ndcqco", new CheckBox("Enable in Combo"));
            qCMenu.Add("ndcqha", new CheckBox("Enable in Harass"));
            qCMenu.Add("ndcqjg", new CheckBox("Enable in Jungle"));
            qCMenu.Add("ndcqwc", new CheckBox("Enable in WaveClear"));

            wCMenu = Root.AddSubMenu("(W) Pounce", "ndcw");
            wCMenu.Add("ndcwcheck", new CheckBox("Check Hitchance", false));
            wCMenu.Add("ndcwch", new ComboBox("-> Min Hitchance", 2, "Low", "Medium", "High", "Very High"));
            wCMenu.Add("ndcwco", new CheckBox("Enable in Combo"));
            wCMenu.Add("ndcwhunt", new CheckBox("-> Ignore Checks if Hunted", false));
            wCMenu.Add("ndcwdistco", new CheckBox("-> Pounce Only if > AARange"));
            wCMenu.Add("ndcwjg", new CheckBox("Enable in Jungle"));
            wCMenu.Add("ndcwwc", new CheckBox("Enable in WaveClear"));
            wCMenu.Add("ndcwdistwc", new CheckBox("-> Pounce Only if > AARange", false));
            wCMenu.Add("ndcwene", new CheckBox("-> Dont Pounce into Enemies"));
            wCMenu.Add("ndcwtow", new CheckBox("-> Dont Pounce into Turret"));

            eCMenu = Root.AddSubMenu("(E) Swipe", "ndce");
            eCMenu.Add("ndcecheck", new CheckBox("Check Hitchance", false));
            eCMenu.Add("ndcech", new ComboBox("-> Min Hitchance", 2, "Low", "Medium", "High", "Very High"));
            eCMenu.Add("ndceco", new CheckBox("Enable in Combo"));
            eCMenu.Add("ndceha", new CheckBox("Enable in Harass"));
            eCMenu.Add("ndcejg", new CheckBox("Enable in Jungle"));
            eCMenu.Add("ndcewc", new CheckBox("Enable in WaveClear"));
            eCMenu.Add("ndcenum", new Slider("-> Minimum Minions Hit", 3, 1, 5));

            rCMenu = Root.AddSubMenu("(R) Aspect of the Cougar", "ndcr");
            rCMenu.Add("ndcrco", new CheckBox("Enable in Combo"));
            rCMenu.Add("ndcrha", new CheckBox("Enable in Harass"));
            rCMenu.Add("ndcrjg", new CheckBox("Enable in Jungle"));
            rCMenu.Add("ndcrwc", new CheckBox("Enable in WaveClear", false));

            #endregion

            drawMenu = Root.AddSubMenu(":: Draw Settings", "dmenu");
            drawMenu.Add("dp", new CheckBox(":: Draw Javelin Range", false));
            drawMenu.Add("dti", new CheckBox(":: Draw Javelin Timer", false));
            drawMenu.Add("dz", new CheckBox(":: Draw Pounce Range (Hunted)", false));
            drawMenu.Add("dt", new CheckBox(":: Draw Target", false));

            jungleMenu = Root.AddSubMenu(":: Jungle Settings", "xmenu");
            jungleMenu.Add("spcol", new CheckBox(":: Force (R) if (Q) Collision [jungle]", false));
            jungleMenu.Add("jgaacount",
                new KeyBind(":: AA Weaving jungle] [beta]", false, KeyBind.BindTypes.PressToggle, 'H'));
            jungleMenu.Add("aareq", new Slider("-> Required auto attack Count [jungle]", 2, 1, 5));
            jungleMenu.Add("kitejg", new CheckBox(":: Pounce Away (Kite) [jungle]", false));

            autoMenu = Root.AddSubMenu(":: Automatic Settings", "aamenu");
            autoMenu.Add("alvl6", new CheckBox(":: Auto (R) Level Up", false));
            autoMenu.Add("ndhqimm", new CheckBox(":: Auto (Q) Javelin Immobile", false));
            autoMenu.Add("ndhwimm", new CheckBox(":: Auto (W) Bushwhack Immobile", false));
            autoMenu.Add("ndhrgap", new CheckBox(":: Auto (R) Enemy Gapclosers"));
            autoMenu.Add("ndcegap", new CheckBox(":: Auto (E) Swipe Gapclosers"));
            autoMenu.Add("ndhqgap", new CheckBox(":: Auto (Q) Javelin Gapclosers"));
            autoMenu.Add("ndcqgap", new CheckBox(":: Auto (Q) Takedown Gapclosers"));

            #endregion

            Game.OnUpdate += Game_OnUpdate;

            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnDoCast;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
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

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy || sender.Type != Player.Type || !args.SData.IsAutoAttack())
            {
                return;
            }

            foreach (var ally in Allies().Where(hero => !hero.IsMelee))
            {
                if (ally.NetworkId != sender.NetworkId || !getCheckBoxItem(eHMenu, "xx" + ally.ChampionName))
                {
                    return;
                }

                if (args.Target.Type == GameObjectType.AIHeroClient || args.Target.Type == GameObjectType.obj_AI_Turret)
                {
                    if (KL.CanUse(KL.Spells["Primalsurge"], true, "on"))
                    {
                        if (ally.IsValidTarget(KL.Spells["Primalsurge"].Range) &&
                            ally.Health/ally.MaxHealth*100 <= 90)
                        {
                            if (!Player.Spellbook.IsChanneling && !Player.IsRecalling())
                            {
                                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) ||
                                    ally.Health/ally.MaxHealth*100 <= 20 || !KL.CatForm())
                                {
                                    if (Player.Mana/Player.MaxMana*100 <
                                        getSliderItem(eHMenu, "ndhemana") &&
                                        !(ally.Health/ally.MaxHealth*100 <= 20))
                                        return;

                                    if (KL.CatForm() == false)
                                        KL.Spells["Primalsurge"].CastOnUnit(ally);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack())
            {
                if (getKeyBindItem(Root, "usecombo2"))
                {
                    if (KL.CatForm() && KL.Spells["Aspect"].IsReady() && KL.SpellTimer["Javelin"].IsReady())
                    {
                        KL.Spells["Takedown"].Cast();

                        if (Player.HasBuff("Takedown"))
                        {
                            KL.Spells["Aspect"].Cast();
                        }
                    }

                    if (!KL.CatForm() && KL.SpellTimer["Javelin"].IsReady())
                    {
                        if (Utils.GameTimeTickCount - KL.LastBite <= 1200 || KL.SpellTimer["Javelin"].IsReady())
                        {
                            var targ = args.Target as Obj_AI_Base;
                            if (targ == null)
                            {
                                return;
                            }

                            if (targ.Path.Length < 1)
                                KL.Spells["Javelin"].Cast(targ.ServerPosition);

                            if (targ.Path.Length > 0)
                                KL.Spells["Javelin"].Cast(targ);
                        }
                    }
                }
            }
        }

        #region OnBuffAdd

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero != null && hero.IsEnemy && KL.SpellTimer["Javelin"].IsReady() &&
                getCheckBoxItem(autoMenu, "ndhqimm"))
            {
                if (hero.IsValidTarget(KL.Spells["Javelin"].Range))
                {
                    if (args.Buff.Type == BuffType.Stun || args.Buff.Type == BuffType.Snare ||
                        args.Buff.Type == BuffType.Taunt || args.Buff.Type == BuffType.Knockback)
                    {
                        if (!KL.CatForm())
                        {
                            KL.Spells["Javelin"].Cast(hero);
                            KL.Spells["Javelin"].CastIfHitchanceEquals(hero, HitChance.Immobile);
                        }
                        else
                        {
                            if (KL.Spells["Aspect"].IsReady() &&
                                KL.Spells["Javelin"].Cast(hero) == Spell.CastStates.Collision)
                                KL.Spells["Aspect"].Cast();
                        }
                    }
                }
            }

            if (hero != null && hero.IsEnemy && KL.SpellTimer["Bushwhack"].IsReady() &&
                getCheckBoxItem(autoMenu, "ndhwimm"))
            {
                if (hero.IsValidTarget(KL.Spells["Bushwhack"].Range))
                {
                    if (args.Buff.Type == BuffType.Stun || args.Buff.Type == BuffType.Snare ||
                        args.Buff.Type == BuffType.Taunt || args.Buff.Type == BuffType.Knockback)
                    {
                        KL.Spells["Bushwhack"].Cast(hero);
                        KL.Spells["Bushwhack"].CastIfHitchanceEquals(hero, HitChance.Immobile);
                    }
                }
            }
        }

        #endregion

        #region OnDraw

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || !Player.IsValid)
            {
                return;
            }

            foreach (
                var unit in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(900) && x.PassiveRooted()))
            {
                var b = unit.GetBuff("NidaleePassiveMonsterRoot");
                if (b.Caster.IsMe && b.EndTime - Game.Time > 0)
                {
                    var tpos = Drawing.WorldToScreen(unit.Position);
                    Drawing.DrawText(tpos[0], tpos[1], Color.DeepPink,
                        "ROOTED " + (b.EndTime - Game.Time).ToString("F"));
                }
            }

            if (getCheckBoxItem(drawMenu, "dti"))
            {
                var pos = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(pos[0] + 100, pos[1] - 135, Color.White,
                    "Q: " + KL.SpellTimer["Javelin"].ToString("F"));
            }

            if (getCheckBoxItem(drawMenu, "dt") && Target != null)
            {
                if (getBoxItem(Root, "pstyle") == 0)
                {
                    Render.Circle.DrawCircle(Target.Position, Target.BoundingRadius, Color.DeepPink, 6);
                }
            }

            if (getCheckBoxItem(drawMenu, "dp") && !KL.CatForm())
            {
                Render.Circle.DrawCircle(KL.Player.Position, KL.Spells["Javelin"].Range,
                    Color.FromArgb(155, Color.DeepPink), 4);
            }

            if (getCheckBoxItem(drawMenu, "dz") && KL.CatForm())
            {
                Render.Circle.DrawCircle(KL.Player.Position, KL.Spells["ExPounce"].Range,
                    Color.FromArgb(155, Color.DeepPink), 4);
            }
        }

        #endregion

        #region Ally Heroes

        internal static IEnumerable<AIHeroClient> Allies()
        {
            switch (getBoxItem(eHMenu, "ndheord"))
            {
                case 0:
                    return HeroManager.Allies.OrderBy(h => h.Health/h.MaxHealth*100);
                case 1:
                    return
                        HeroManager.Allies.OrderByDescending(h => h.BaseAttackDamage + h.FlatPhysicalDamageMod)
                            .ThenByDescending(h => h.FlatMagicDamageMod);
                case 2:
                    return HeroManager.Allies.OrderByDescending(h => h.MaxHealth);
            }

            return null;
        }

        #endregion

        internal static void Game_OnUpdate(EventArgs args)
        {
            Target = TargetSelector.GetTarget(KL.Spells["Javelin"].Range, DamageType.Magical);

            #region Active Modes

            if (getKeyBindItem(Root, "usecombo2"))
            {
                Combo2();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) || getKeyBindItem(Root, "flee"))
            {
                Flee();
            }

            #endregion

            #region Auto Heal

            // auto heal on ally hero
            if (KL.CanUse(KL.Spells["Primalsurge"], true, "on"))
            {
                if (!Player.Spellbook.IsChanneling && !Player.IsRecalling())
                {
                    if ((getKeyBindItem(Root, "flee") || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) &&
                        KL.CatForm())
                        return;

                    foreach (
                        var hero in
                            Allies().Where(
                                h => getCheckBoxItem(eHMenu, "xx" + h.ChampionName) &&
                                     h.IsValidTarget(KL.Spells["Primalsurge"].Range) &&
                                     h.Health/h.MaxHealth*100 <
                                     getSliderItem(eHMenu, "zz" + h.ChampionName)))
                    {
                        if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) ||
                            hero.Health/hero.MaxHealth*100 <= 20 || !KL.CatForm())
                        {
                            if (Player.Mana/Player.MaxMana*100 < getSliderItem(eHMenu, "ndhemana") &&
                                !(hero.Health/hero.MaxHealth*100 <= 20))
                                return;

                            if (KL.CatForm() == false)
                                KL.Spells["Primalsurge"].CastOnUnit(hero);

                            if (KL.CatForm() && getCheckBoxItem(eHMenu, "ndhesw") &&
                                KL.SpellTimer["Primalsurge"].IsReady() &&
                                KL.Spells["Aspect"].IsReady())
                                KL.Spells["Aspect"].Cast();
                        }
                    }
                }
            }

            #endregion
        }

        internal static void Orb(Obj_AI_Base target)
        {
            if (target != null && target.IsHPBarRendered && target.IsEnemy)
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }
        }

        internal static void Combo()
        {
            var solo = getBoxItem(Root, "pstyle") == 0;

            if (!Orbwalker.IsAutoAttacking)
            {
                CM.CastJavelin(
                    solo ? Target : TargetSelector.GetTarget(KL.Spells["Javelin"].Range, DamageType.Magical), "co");
                CM.SwitchForm(solo ? Target : TargetSelector.GetTarget(KL.Spells["Javelin"].Range, DamageType.Magical),
                    "co");
            }

            if (!getCheckBoxItem(wHMenu, "ndhwsp"))
            {
                CM.CastBushwhack(
                    solo ? Target : TargetSelector.GetTarget(KL.Spells["Bushwhack"].Range, DamageType.Magical), "co");
            }

            CM.CastTakedown(solo ? Target : TargetSelector.GetTarget(KL.Spells["Takedown"].Range, DamageType.Magical),
                "co");
            CM.CastPounce(solo ? Target : TargetSelector.GetTarget(KL.Spells["ExPounce"].Range, DamageType.Magical),
                "co");
            CM.CastSwipe(solo ? Target : TargetSelector.GetTarget(KL.Spells["Swipe"].Range, DamageType.Magical), "co");
        }

        internal static void Combo2()
        {
            var target = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Distance(Player.ServerPosition) <= 600
                                                                       && x.IsEnemy && x.IsHPBarRendered
                                                                       && !MinionManager.IsWard(x))
                .OrderByDescending(x => x.MaxHealth)
                .FirstOrDefault();

            Orb(target);

            if (target == null)
            {
                return;
            }

            if (Utils.GameTimeTickCount - KL.LastR >= 500 - Game.Ping)
            {
                if (!KL.CanUse(KL.Spells["Javelin"], true, "jg") && KL.CanUse(KL.Spells["Swipe"], false, "jg"))
                {
                    if (KL.CatForm() && target.IsValidTarget(KL.Spells["Swipe"].Range))
                    {
                        KL.Spells["Swipe"].Cast(target.ServerPosition);
                    }
                }

                if (!KL.CanUse(KL.Spells["Javelin"], true, "jg") &&
                    KL.CanUse(KL.Spells["Bushwhack"], false, "jg"))
                {
                    if (!KL.CatForm() && target.IsValidTarget(KL.Spells["Bushwhack"].Range) &&
                        KL.Player.ManaPercent > 40)
                    {
                        KL.Spells["Bushwhack"].Cast(target.ServerPosition);
                    }
                }

                if (!KL.CanUse(KL.Spells["Javelin"], true, "jg") && KL.CanUse(KL.Spells["Pounce"], false, "jg"))
                {
                    var r = target.IsHunted() ? KL.Spells["ExPounce"].Range : KL.Spells["Pounce"].Range;
                    if (KL.CatForm() && target.IsValidTarget(r))
                    {
                        KL.Spells["Pounce"].Cast(target.ServerPosition);
                    }
                }
            }

            if (KL.Spells["Takedown"].Level > 0 && KL.SpellTimer["Takedown"].IsReady() && !KL.CatForm())
            {
                if (KL.Spells["Aspect"].IsReady())
                {
                    KL.Spells["Aspect"].Cast();
                }
            }

            if (KL.Spells["Javelin"].Level > 0 && !KL.SpellTimer["Javelin"].IsReady() && !KL.CatForm())
            {
                if (KL.Spells["Aspect"].IsReady())
                {
                    KL.Spells["Aspect"].Cast();
                }
            }
        }

        internal static void Harass()
        {
            CM.CastJavelin(TargetSelector.GetTarget(KL.Spells["Javelin"].Range, DamageType.Magical), "ha");
            CM.CastTakedown(TargetSelector.GetTarget(KL.Spells["Takedown"].Range, DamageType.Magical), "ha");
            CM.CastSwipe(TargetSelector.GetTarget(KL.Spells["Swipe"].Range, DamageType.Magical), "ha");
            CM.SwitchForm(TargetSelector.GetTarget(KL.Spells["Javelin"].Range, DamageType.Magical), "ha");
        }

        internal static void Clear()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition,
                750f, MinionTypes.All, MinionTeam.All, MinionOrderTypes.MaxHealth);

            m = minions.Any(KL.IsJungleMinion);

            foreach (var unit in minions.OrderByDescending(KL.IsJungleMinion))
            {
                switch (unit.Team)
                {
                    case GameObjectTeam.Neutral:
                        if (!unit.Name.Contains("Mini"))
                        {
                            CM.CastJavelin(unit, "jg");
                            CM.CastBushwhack(unit, "jg");
                        }

                        CM.CastPounce(unit, "jg");
                        CM.CastTakedown(unit, "jg");
                        CM.CastSwipe(unit, "jg");

                        if (unit.PassiveRooted() && getKeyBindItem(jungleMenu, "jgaacount") &&
                            Player.Distance(unit.ServerPosition) > 450)
                        {
                            return;
                        }

                        CM.SwitchForm(unit, "jg");
                        break;
                    default:
                        if (unit.Team != Player.Team && unit.Team != GameObjectTeam.Neutral)
                        {
                            CM.CastJavelin(unit, "wc");
                            CM.CastPounce(unit, "wc");
                            CM.CastBushwhack(unit, "wc");
                            CM.CastTakedown(unit, "wc");
                            CM.CastSwipe(unit, "wc");
                            CM.SwitchForm(unit, "wc");
                        }
                        break;
                }
            }
        }

        #region Walljumper @Hellsing

        internal static void Flee()
        {
            if (!KL.CatForm() && KL.Spells["Aspect"].IsReady())
            {
                if (KL.SpellTimer["Pounce"].IsReady())
                    KL.Spells["Aspect"].Cast();
            }

            var wallCheck = KL.GetFirstWallPoint(KL.Player.Position, Game.CursorPos);

            if (wallCheck != null)
                wallCheck = KL.GetFirstWallPoint((Vector3) wallCheck, Game.CursorPos, 5);

            var movePosition = wallCheck != null ? (Vector3) wallCheck : Game.CursorPos;

            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);

            if (KL.CatForm() && KL.SpellTimer["Pounce"].IsReady() && wallCheck != null)
            {
                var wallPosition = movePosition;

                var direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                var maxAngle = 80f;
                var step = maxAngle/20;
                float currentAngle = 0;
                float currentStep = 0;
                var jumpTriggered = false;

                while (true)
                {
                    if (currentStep > maxAngle && currentAngle < 0)
                        break;

                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                    {
                        currentAngle = currentStep*(float) Math.PI/180;
                        currentStep += step;
                    }

                    else if (currentAngle > 0)
                        currentAngle = -currentAngle;

                    Vector3 checkPoint;

                    if (currentStep == 0)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + KL.Spells["Pounce"].Range*direction.To3D();
                    }

                    else
                        checkPoint = wallPosition + KL.Spells["Pounce"].Range*direction.Rotated(currentAngle).To3D();

                    if (checkPoint.IsWall())
                        continue;

                    wallCheck = KL.GetFirstWallPoint(checkPoint, wallPosition);

                    if (wallCheck == null)
                        continue;

                    var wallPositionOpposite = (Vector3) KL.GetFirstWallPoint((Vector3) wallCheck, wallPosition, 5);

                    if (KL.Player.GetPath(wallPositionOpposite).ToList().LSTo2D().PathLength() -
                        KL.Player.Distance(wallPositionOpposite) > 200)
                    {
                        if (KL.Player.Distance(wallPositionOpposite) <
                            KL.Spells["Pounce"].Range - KL.Player.BoundingRadius/2)
                        {
                            KL.Spells["Pounce"].Cast(wallPositionOpposite);
                            jumpTriggered = true;
                            break;
                        }
                    }
                    else
                    {
                        Render.Circle.DrawCircle(Game.CursorPos, 35, Color.Red, 2);
                    }
                }

                if (!jumpTriggered)
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }
            }

            else
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
                if (KL.CatForm() && KL.SpellTimer["Pounce"].IsReady())
                    KL.Spells["Pounce"].Cast(Game.CursorPos);
            }
        }

        #endregion
    }
}