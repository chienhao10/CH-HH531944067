using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using Damage = LeagueSharp.Common.Damage;


namespace KurisuRiven
{
    internal class KurisuRiven
    {
        #region Riven: Main

        private static int lastq;
        private static int lastw;
        private static int laste;
        private static int lastaa;
        private static int lasthd;
        private static int lastwd;
        private static int LastTick;

        private static bool canq;
        private static bool canw;
        private static bool cane;
        private static bool canmv;
        private static bool canaa;
        private static bool canws;
        private static bool canhd;
        private static bool hashd;

        private static bool didq;
        private static bool didw;
        private static bool dide;
        private static bool didws;
        private static bool didaa;
        private static bool didhd;
        private static bool didhs;
        private static bool ssfl;
        public static int LastAATick;

        private static Spell q, w, e, r;
        private static AIHeroClient player = ObjectManager.Player;
        private static HpBarIndicator hpi = new HpBarIndicator();
        private static Obj_AI_Base qtarg; // semi q target

        private static int cc;
        private static int pc;
        private static bool uo;
        private static SpellSlot flash;

        public static Menu rivenMenu, farmMenu, harassMenu, keybindsMenu, qMenu, wMenu, eMenu, r1Menu, r2Menu, drawMenu;

        private static float truerange;
        private static Vector3 movepos;
        #endregion

        # region Riven: Utils

        private static bool Getcheckboxvalue(Menu menu, string menuvalue)
        {
            return menu[menuvalue].Cast<CheckBox>().CurrentValue;
        }
        private static bool Getkeybindvalue(Menu menu, string menuvalue)
        {
            return menu[menuvalue].Cast<KeyBind>().CurrentValue;
        }
        private static int Getslidervalue(Menu menu, string menuvalue)
        {
            return menu[menuvalue].Cast<Slider>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static float xtra(float dmg)
        {
            return r.IsReady() ? (float)(dmg + (dmg * 0.2)) : dmg;
        }

        private static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDamage(unit) / 1.65 >= unit.Health;
        }

        private static Obj_AI_Base GetCenterMinion()
        {
            var minionposition = MinionManager.GetMinions(300 + q.Range).Select(x => x.Position.LSTo2D()).ToList();
            var center = MinionManager.GetBestCircularFarmLocation(minionposition, 250, 300 + q.Range);

            return center.MinionsHit >= 3
                ? MinionManager.GetMinions(1000).OrderBy(x => x.LSDistance(center.Position)).FirstOrDefault()
                : null;
        }

        private static void TryIgnote(Obj_AI_Base target)
        {
            var ignote = player.GetSpellSlot("summonerdot");
            if (player.Spellbook.CanUseSpell(ignote) == SpellState.Ready)
            {
                if (target.LSDistance(player.ServerPosition) <= 600)
                {
                    if (cc <= Getslidervalue(r1Menu, "userq") && q.IsReady() && Getcheckboxvalue(r1Menu, "useignote"))
                    {
                        if (ComboDamage(target) >= target.Health &&
                            target.Health / target.MaxHealth * 100 > Getslidervalue(r1Menu, "overk") ||
                            Getkeybindvalue(keybindsMenu, "shycombo"))
                        {
                            if (r.IsReady() && uo)
                            {
                                player.Spellbook.CastSpell(ignote, target);
                            }
                        }
                    }
                }
            }
        }

        private static void useinventoryitems(Obj_AI_Base target)
        {
            if (Items.HasItem(3142) && Items.CanUseItem(3142))
                Items.UseItem(3142);

            if (target.LSDistance(player.ServerPosition, true) <= 450 * 450)
            {
                if (Items.HasItem(3144) && Items.CanUseItem(3144))
                    Items.UseItem(3144, target);
                if (Items.HasItem(3153) && Items.CanUseItem(3153))
                    Items.UseItem(3153, target);
            }
        }

        private static readonly string[] minionlist =
        {
            // summoners rift
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", 
            
            // twisted treeline
            "TT_NGolem5", "TT_NGolem2", "TT_NWolf6", "TT_NWolf3",
            "TT_NWraith1", "TT_Spider"
        };

        #endregion

        #region Riven: ctor

        public KurisuRiven()
        {
            if (player.ChampionName != "Riven")
            {
                return;
            }

            w = new Spell(SpellSlot.W, 250f);
            e = new Spell(SpellSlot.E, 270f);

            q = new Spell(SpellSlot.Q, 260f);
            q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);

            r = new Spell(SpellSlot.R, 900f);
            r.SetSkillshot(0.25f, 90f, 1600f, false, SkillshotType.SkillshotCircle);

            flash = player.GetSpellSlot("summonerflash");
            OnDoCast();

            OnPlayAnimation();
            Interrupter();
            OnGapcloser();
            OnCast();
            Drawings();
            OnMenuLoad();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Chat.Print("<b><font color=\"#66FF33\">Kurisu's Riven</font></b> - Loaded!");
        }
        #endregion

        private static AIHeroClient _sh;
        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (ulong)WindowsMessages.WM_LBUTTONDOWN)
            {
                _sh = HeroManager.Enemies
                     .FindAll(hero => hero.LSIsValidTarget() && hero.LSDistance(Game.CursorPos, true) < 40000) // 200 * 200
                     .OrderBy(h => h.LSDistance(Game.CursorPos, true)).FirstOrDefault();
            }
        }

        private static AIHeroClient riventarget()
        {
            var cursortarg = HeroManager.Enemies
                .Where(x => x.LSDistance(Game.CursorPos) <= 1400 && x.LSDistance(player.ServerPosition) <= 1400)
                .OrderBy(x => x.LSDistance(Game.CursorPos)).FirstOrDefault(x => x.LSIsValidTarget());

            var closetarg = HeroManager.Enemies
                .Where(x => x.LSDistance(player.ServerPosition) <= e.Range + 100)
                .OrderBy(x => x.LSDistance(player.ServerPosition)).FirstOrDefault(x => x.LSIsValidTarget());

            return _sh ?? cursortarg ?? closetarg;
        }

        private static bool wrektAny()
        {
            return Getcheckboxvalue(wMenu, "req") &&
                 player.GetEnemiesInRange(1250).Any(ez => Getcheckboxvalue(wMenu, "w" + ez.ChampionName));
        }

        private static bool rrektAny()
        {
            return Getcheckboxvalue(r2Menu, "req2") &&
                 player.GetEnemiesInRange(1250).Any(ez => Getcheckboxvalue(r2Menu, "r" + ez.ChampionName));
        }

        #region Riven: OnDoCast
        private static void OnDoCast()
        {
            Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    if (Getkeybindvalue(keybindsMenu, "shycombo"))
                    {
                        if (riventarget().LSIsValidTarget() && !riventarget().IsZombie && !riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (shy() && uo && !canhd)
                            {
                                if (riventarget().HasBuffOfType(BuffType.Stun))
                                    r.Cast(riventarget().ServerPosition);

                                if (!riventarget().HasBuffOfType(BuffType.Stun))
                                    r.CastIfHitchanceEquals(riventarget(), HitChance.Medium);
                            }
                        }
                    }

                    if (Getkeybindvalue(keybindsMenu, "shycombo"))
                    {
                        if (riventarget().LSIsValidTarget() && !riventarget().IsZombie &&
                           !riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);
                            if (Items.CanUseItem(3748))
                                Items.UseItem(3748);
                        }
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        if (riventarget().LSIsValidTarget(e.Range + 200))
                        {
                            if (player.Health / player.MaxHealth * 100 <= Getslidervalue(eMenu, "vhealth"))
                            {
                                if (Getcheckboxvalue(eMenu, "usecomboe") && cane)
                                {
                                    if (!riventarget().IsMelee)
                                    {
                                        e.Cast(riventarget().ServerPosition);
                                    }
                                    else
                                    {
                                        e.Cast(Game.CursorPos);
                                    }
                                }
                            }
                        }
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        if (Utils.GameTimeTickCount - lasthd < 1600)
                        {
                            if (w.IsReady() && riventarget().LSDistance(player.ServerPosition) <= w.Range + 25)
                            {
                                w.Cast();
                            }
                        }

                        if (qtarg != null && riventarget() != null)
                        {
                            if (qtarg.NetworkId == riventarget().NetworkId)
                            {
                                if (Items.CanUseItem(3077))
                                    Items.UseItem(3077);
                                if (Items.CanUseItem(3074))
                                    Items.UseItem(3074);
                                if (Items.CanUseItem(3748))
                                    Items.UseItem(3748);
                            }
                        }
                    }

                    else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    {
                        if (!player.UnderTurret(true) || !HeroManager.Enemies.Any(x => x.LSIsValidTarget(1400)))
                        {
                            if (Utils.GameTimeTickCount - lasthd < 1600 && args.Target is Obj_AI_Minion)
                            {
                                if (w.IsReady() && args.Target.Position.LSDistance(player.ServerPosition) <= w.Range + 25)
                                {
                                    w.Cast();
                                }
                            }

                            if (qtarg.IsValid<Obj_AI_Minion>() && !qtarg.Name.StartsWith("Minion"))
                            {
                                if (Items.CanUseItem(3077))
                                    Items.UseItem(3077);
                                if (Items.CanUseItem(3074))
                                    Items.UseItem(3074);
                                if (Items.CanUseItem(3748))
                                    Items.UseItem(3748);
                            }
                        }
                    }
                }

                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    didaa = false;
                    canmv = true;
                    canaa = true;
                    canq = true;
                    cane = true;
                    canw = true;
                }
            };
        }

        #endregion

        #region Riven: OnUpdate

        private static bool isteamfightkappa;
        private static void Game_OnUpdate(EventArgs args)
        {
            // harass active
            didhs = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);

            // ulti check
            uo = player.GetSpell(SpellSlot.R).Name != "RivenFengShuiEngine";

            // hydra check
            hashd = Items.HasItem(3077) || Items.HasItem(3074) || Items.HasItem(3748);
            canhd = Items.CanUseItem(3077) || Items.CanUseItem(3074) || Items.CanUseItem(3748);

            // my radius
            truerange = player.AttackRange + player.LSDistance(player.BBox.Minimum) + 1;

            // if no valid target cancel to cursor pos
            if (!qtarg.LSIsValidTarget(truerange + 100))
                qtarg = player;

            if (!riventarget().LSIsValidTarget())
                _sh = null;

            if (!canmv && didq)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) ||
                    Getkeybindvalue(keybindsMenu, "shycombo"))
                {
                    if (Player.IssueOrder(GameObjectOrder.MoveTo, movepos))
                    {
                        didq = false;
                        Utility.DelayAction.Add(40, () =>
                        {
                            canmv = true;
                            canaa = true;
                        });
                    }
                }

                else if (qtarg.LSIsValidTarget(q.Range) && Getcheckboxvalue(keybindsMenu, "semiq"))
                {
                    if (Player.IssueOrder(GameObjectOrder.MoveTo, movepos))
                    {
                        didq = false;
                        Utility.DelayAction.Add(40, () =>
                        {
                            canmv = true;
                            canaa = true;
                        });
                    }
                }
            }

            // move target position
            if (qtarg != player && qtarg.LSDistance(player.ServerPosition) < r.Range)
                movepos = player.Position.LSExtend(Game.CursorPos, player.LSDistance(Game.CursorPos) + 500);

            // move to game cursor pos
            if (qtarg == player)
                movepos = player.ServerPosition + (Game.CursorPos - player.ServerPosition).LSNormalized() * 125;

            SemiQ();
            AuraUpdate();
            CombatCore();

            if (riventarget().LSIsValidTarget())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    ComboTarget(riventarget());
                    TryIgnote(riventarget());
                }
            }

            if (Getkeybindvalue(keybindsMenu, "shycombo"))
            {
                OrbTo(riventarget(), 350);

                if (riventarget().LSIsValidTarget())
                {
                    SomeDash(riventarget());

                    if (w.IsReady() && riventarget().LSDistance(player.ServerPosition) <= w.Range + 50)
                    {
                        checkr();
                        w.Cast();
                    }

                    else if (q.IsReady() && riventarget().LSDistance(player.ServerPosition) <= truerange + 100)
                    {
                        //if (Utils.GameTimeTickCount - lastw < 500 && Utils.GameTimeTickCount - lasthd < 1000)
                        //{
                        //DoOneQ(riventarget().ServerPosition);
                        //}
                        checkr();
                        TryIgnote(riventarget());

                        if (canq && !canhd && Utils.GameTimeTickCount - lasthd >= 300)
                        {
                            if (Utils.GameTimeTickCount - lastw >= 300 + Game.Ping)
                            {
                                useinventoryitems(riventarget());
                                q.Cast(riventarget().ServerPosition);
                            }
                        }
                    }
                }
            }

            if (didhs && riventarget().LSIsValidTarget())
            {
                HarassTarget(riventarget());
            }

            if (player.IsValid && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
                Wave();
            }

            if (player.IsValid && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            WindSlashExecute();
            Windslash();

            isteamfightkappa = player.CountAlliesInRange(1500) > 1 && player.LSCountEnemiesInRange(1350) > 2 ||
                               player.LSCountEnemiesInRange(1200) > 2;

            ChangeR1();
            ChangeR2();
        }

        #endregion

        #region Riven: Menu
        private static void OnMenuLoad()
        {
            rivenMenu = MainMenu.AddMenu("Riven", "Riven");

            keybindsMenu = rivenMenu.AddSubMenu("keybinds Options");
            keybindsMenu.Add("shycombo", new KeyBind("Burst Combo", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
            keybindsMenu.Add("semiq", new CheckBox("Auto Q Harass/Jungle", false));

            drawMenu = rivenMenu.AddSubMenu("Drawings Options");
            drawMenu.Add("linewidth", new Slider("Line Width", 1, 1, 6));
            drawMenu.Add("drawengage", new CheckBox("Draw Engage Range"));
            drawMenu.Add("drawr2", new CheckBox("Draw R2 Range"));
            drawMenu.Add("drawburst", new CheckBox("Draw Burst Range"));
            drawMenu.Add("drawf", new CheckBox("Draw Target"));
            drawMenu.Add("drawdmg", new CheckBox("Draw Combo Damage Fill"));
            drawMenu.Add("drawr1mode", new CheckBox("Draw R1 Mode"));
            drawMenu.Add("drawr2mode", new CheckBox("Draw R2 Mode"));


            qMenu = rivenMenu.AddSubMenu("Q Options");
            qMenu.Add("wq3", new CheckBox("Ward + Q3 (Flee)"));
            qMenu.Add("qint", new CheckBox("Interrupt with 3rd Q"));
            qMenu.Add("keepq", new CheckBox("Use Q Before Expiry"));
            qMenu.Add("usegap", new CheckBox("Gapclose with Q", false));
            qMenu.Add("gaptimez", new Slider("Gapclose Q Delay (ms)", 115, 0, 200));
            qMenu.Add("QD", new Slider("Ping Delay", 56, 20, 300));
            qMenu.Add("QLD", new Slider("Spell Delay", 56, 20, 300));
            qMenu.Add("safeq", new CheckBox("Block Q into multiple enemies", false));

            wMenu = rivenMenu.AddSubMenu("W Options");
            wMenu.Add("req", new CheckBox("Required Targets"));
            wMenu.AddSeparator();
            foreach (var hero in HeroManager.Enemies)
            {
                wMenu.Add("w" + hero.ChampionName, new CheckBox("Only W if it will hit: " + hero.ChampionName));

            }
            wMenu.AddSeparator();
            wMenu.Add("usecombow", new CheckBox("Use W in Combo"));
            wMenu.Add("fq", new CheckBox("Ignore for now", false));
            wMenu.Add("wint", new CheckBox("Use on Interrupt"));
            wMenu.Add("wgap", new CheckBox("Use on Gapcloser"));

            eMenu = rivenMenu.AddSubMenu("E Options");
            eMenu.Add("usecomboe", new CheckBox("Use E in Combo"));
            eMenu.Add("vhealth", new Slider("Use E if HP% <=", 60, 0, 100));
            eMenu.Add("safee", new CheckBox("Block E into multiple enemies"));

            r1Menu = rivenMenu.AddSubMenu("R1 Options");
            r1Menu.Add("useignote", new CheckBox("Combo with Ignite"));
            r1Menu.Add("user", new KeyBind("Use R1 in Combo", false, KeyBind.BindTypes.PressToggle, "H".ToCharArray()[0]));
            r1Menu.Add("ultwhen", new ComboBox("Use R1 when", 1, "Normal Kill", "Hard Kill", "Always"));
            r1Menu.Add("switchr1", new KeyBind("Switch R1 Priority", false, KeyBind.BindTypes.HoldActive, 'L'));
            r1Menu.Add("overk", new Slider("Dont R1 if target HP % <=", 25, 1, 99));
            r1Menu.Add("userq", new Slider("Use only if Q Count <=", 2, 1, 3));
            r1Menu.Add("multib", new ComboBox("Burst When", 1, "Damage Check", "Always"));
            r1Menu.Add("flashb", new CheckBox("Burst: Flash in Burst"));

            r2Menu = rivenMenu.AddSubMenu("R2 Options");
            r2Menu.Add("req2", new CheckBox("Required Targets"));
            r2Menu.AddSeparator();

            foreach (var hero in HeroManager.Enemies)
            {
                r2Menu.Add("r" + hero.ChampionName, new CheckBox("Only R2 if it will hit: " + hero.ChampionName));

            }
            r2Menu.AddSeparator();

            r2Menu.Add("usews", new CheckBox("Use R2 in Combo"));
            r2Menu.Add("rhitc", new ComboBox("-> Hitchance", 2, "Medium", "High", "Very High"));
            r2Menu.Add("saver", new CheckBox("Save R2 (When in AA Range)", false));
            r2Menu.Add("overaa", new Slider("Dont R2 if target will die in AA", 2, 1, 6));
            r2Menu.Add("wsmode", new ComboBox("Use R2 when", 1, "Kill Only", "Max Damage"));
            r2Menu.Add("switchr2", new KeyBind("Switch R2 Priority", false, KeyBind.BindTypes.HoldActive, 'K'));
            r2Menu.Add("keepr", new CheckBox("Use R2 Before Expiry"));


            harassMenu = rivenMenu.AddSubMenu("Harass Options");
            harassMenu.Add("useharassw", new CheckBox("Use W in Harass"));
            harassMenu.Add("usegaph", new CheckBox("Use E in Harass"));
            harassMenu.Add("qtoo", new ComboBox("Use Escape/Flee", 1, "Away from Target", "To Ally Turret", "To Cursor"));
            harassMenu.Add("useitemh", new CheckBox("Use Tiamat/Hydra"));

            farmMenu = rivenMenu.AddSubMenu("Farming Options");
            farmMenu.Add("usejungleq", new CheckBox("Use Q in Jungle"));
            farmMenu.Add("fq2", new CheckBox("-> Q after W"));
            farmMenu.Add("usejunglew", new CheckBox("Use W in Jungle"));
            farmMenu.Add("usejunglee", new CheckBox("Use E in Jungle"));
            farmMenu.AddSeparator();
            farmMenu.Add("uselaneq", new CheckBox("Use Q in WaveClear"));
            farmMenu.Add("useaoeq", new CheckBox("Try Q AoE WaveClear"));
            farmMenu.Add("uselanew", new CheckBox("Use W in WaveClear"));
            farmMenu.Add("wminion", new Slider("Use W Minions >=", 3, 1, 6));
            farmMenu.Add("uselanee", new CheckBox("Use E in WaveClear"));

        }

        #endregion

        #region Riven : Some Dash
        private static bool canburst()
        {
            if (riventarget() == null || !r.IsReady())
            {
                return false;
            }

            if (IsLethal(riventarget()) && getBoxItem(r1Menu, "multib") == 0)
            {
                return true;
            }

            if (Getkeybindvalue(keybindsMenu, "shycombo"))
            {
                if (shy())
                {
                    return true;
                }
            }

            return false;
        }

        private static bool shy()
        {
            if (r.IsReady() && riventarget() != null && getBoxItem(r1Menu, "multib") != 0)
            {
                return true;
            }

            return false;
        }

        private static void doFlash()
        {
            if (riventarget() == null)
            {
                return;
            }
            if (riventarget() != null && (canburst() || shy()))
            {
                if (!flash.IsReady() || !Getcheckboxvalue(r1Menu, "flashb"))
                    return;

                if (Getkeybindvalue(keybindsMenu, "shycombo"))
                {
                    if (riventarget().LSDistance(player.ServerPosition) > e.Range + 50 &&
                        riventarget().LSDistance(player.ServerPosition) <= e.Range + w.Range + 275)
                    {
                        var second =
                            HeroManager.Enemies.Where(
                                x => x.NetworkId != riventarget().NetworkId &&
                                     x.LSDistance(riventarget().ServerPosition) <= r.Range)
                                .OrderByDescending(xe => xe.LSDistance(riventarget().ServerPosition))
                                .FirstOrDefault();

                        if (second != null)
                        {
                            var pos = riventarget().ServerPosition +
                                      (riventarget().ServerPosition - second.ServerPosition).LSNormalized() * 75;

                            player.Spellbook.CastSpell(flash, pos);
                        }

                        else
                        {
                            player.Spellbook.CastSpell(flash,
                                riventarget().ServerPosition.LSExtend(player.ServerPosition, 115));
                        }
                    }
                }
            }
        }

        private static void SomeDash(AIHeroClient target)
        {
            if (!Getkeybindvalue(keybindsMenu, "shycombo") || !target.IsValid<AIHeroClient>() || uo)
                return;

            if (riventarget() == null || !r.IsReady())
                return;

            if (flash.IsReady() && w.IsReady() && (canburst() || shy()) && getBoxItem(r1Menu, "multib") != 2)
            {
                if (e.IsReady() && target.LSDistance(player.ServerPosition) <= e.Range + w.Range + 275)
                {
                    if (target.LSDistance(player.ServerPosition) > e.Range + truerange + 50)
                    {
                        e.Cast(target.ServerPosition);

                        if (!uo)
                            r.Cast();
                    }
                }

                if (!e.IsReady() && target.LSDistance(player.ServerPosition) <= w.Range + 275)
                {
                    if (target.LSDistance(player.ServerPosition) > truerange + 50)
                    {
                        if (!uo)
                            r.Cast();
                    }
                }
            }

            else
            {
                if (e.IsReady() && target.LSDistance(player.ServerPosition) <= e.Range + w.Range - 25)
                {
                    if (target.LSDistance(player.ServerPosition) > truerange + 50)
                    {
                        e.Cast(target.ServerPosition);

                        if (!uo)
                            r.Cast();
                    }
                }

                if (!e.IsReady() && target.LSDistance(player.ServerPosition) <= w.Range - 10)
                {
                    if (!uo)
                        r.Cast();
                }
            }
        }

        #endregion

        #region Riven: Combo

        private static void ComboTarget(AIHeroClient target)
        {
            OrbTo(target);
            TryIgnote(target);

            var endq = player.Position.LSExtend(target.Position, q.Range + 35);
            var ende = player.Position.LSExtend(target.Position, e.Range + 35);

            if (target.LSDistance(player.ServerPosition) <= q.Range + 90 && q.IsReady())
            {
                if (Utils.GameTimeTickCount - lastw < 500 && Utils.GameTimeTickCount - lasthd < 1000)
                {
                    if (target.LSDistance(player.ServerPosition) <= q.Range + 90 && q.IsReady())
                    {
                        DoOneQ(target.ServerPosition);
                    }
                }
            }

            if (e.IsReady() &&

               (target.LSDistance(player.ServerPosition) <= e.Range + w.Range ||
                uo && target.LSDistance(player.ServerPosition) > truerange + 200) &&
                 target.LSDistance(player.ServerPosition) > truerange + 100)
            {
                if (Getcheckboxvalue(eMenu, "usecomboe") && cane)
                {
                    if (Getcheckboxvalue(eMenu, "safee"))
                    {
                        if (ende.CountEnemiesInRange(200) <= 2)
                        {
                            e.Cast(target.IsMelee ? Game.CursorPos : target.ServerPosition);
                        }
                    }

                    else
                    {
                        e.Cast(target.IsMelee ? Game.CursorPos : target.ServerPosition);
                    }
                }

                if (target.LSDistance(player.ServerPosition) <= e.Range + w.Range)
                {
                    checkr();

                    if (!canburst() && canhd && uo)
                    {
                        if (Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                }

                if (!canburst() && canhd)
                {
                    if (Items.CanUseItem(3077))
                        Items.UseItem(3077);
                    if (Items.CanUseItem(3074))
                        Items.UseItem(3074);
                }
            }

            if (w.IsReady() && Getcheckboxvalue(wMenu, "usecombow") && target.LSDistance(player.ServerPosition) <= w.Range)
            {
                if (target.LSDistance(player.ServerPosition) <= w.Range && Utils.GameTimeTickCount - lasthd > 1600)
                {
                    useinventoryitems(target);
                    checkr();

                    if (Getcheckboxvalue(wMenu, "usecombow") && canw)
                    {
                        if (!isteamfightkappa ||
                             isteamfightkappa && !wrektAny() ||
                             Getcheckboxvalue(wMenu, "w" + target.ChampionName))
                        {
                            w.Cast();
                        }
                    }
                }
            }

            var catchRange = e.IsReady() ? e.Range + truerange + 200 : truerange + 200;
            if (q.IsReady() && target.LSDistance(player.ServerPosition) <= q.Range + 100)
            {
                useinventoryitems(target);
                checkr();

                if (IsLethal(target))
                {
                    if (canhd) return;
                }

                if (getBoxItem(r2Menu, "wsmode") == 1 && IsLethal(target))
                {
                    if (cc == 2 && e.IsReady() && cane)
                    {
                        e.Cast(target.ServerPosition);
                    }
                }

                if (canq)
                {
                    if (Getcheckboxvalue(qMenu, "safeq"))
                    {
                        if (endq.CountEnemiesInRange(200) <= 2)
                        {
                            q.Cast(target.ServerPosition);
                        }
                    }

                    else
                    {
                        q.Cast(target.ServerPosition);
                    }
                }
            }

            else if (q.IsReady() && target.LSDistance(player.ServerPosition) > catchRange)
            {
                if (Getcheckboxvalue(qMenu, "usegap"))
                {
                    if (Utils.GameTimeTickCount - lastq >= Getslidervalue(qMenu, "gaptimez") * 10)
                    {
                        if (q.IsReady() && Utils.GameTimeTickCount - laste >= 600)
                        {
                            q.Cast(target.ServerPosition);
                        }
                    }
                }
            }

            else if (target.Health <= q.GetDamage(target) * 2 + player.LSGetAutoAttackDamage(target) * 2)
            {
                if (target.LSDistance(player.ServerPosition) > truerange + q.Range + 10)
                {
                    if (target.LSDistance(player.ServerPosition) <= q.Range * 2)
                    {
                        if (Utils.GameTimeTickCount - lastq >= 250)
                        {
                            q.Cast(target.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Harass

        private static void HarassTarget(Obj_AI_Base target)
        {
            if (target == null)
                return;
            Vector3 qpos;
            switch (getBoxItem(harassMenu, "qtoo"))
            {
                case 0:
                    qpos = player.ServerPosition +
                        (player.ServerPosition - target.ServerPosition).LSNormalized() * 500;
                    break;
                case 1:
                    var tt = ObjectManager.Get<Obj_AI_Turret>()
                        .Where(t => (t.IsAlly)).OrderBy(t => t.LSDistance(player.Position)).First();
                    if (tt != null)
                        qpos = tt.Position;
                    else
                        qpos = player.ServerPosition +
                                (player.ServerPosition - target.ServerPosition).LSNormalized() * 500;
                    break;
                default:
                    qpos = Game.CursorPos;
                    break;
            }

            if (q.IsReady())
                OrbTo(target);

            if (cc == 2 && canq && q.IsReady())
            {
                if (!e.IsReady())
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;

                    canaa = false;
                    canmv = false;

                    if (Player.IssueOrder(GameObjectOrder.MoveTo, qpos))
                    {
                        Utility.DelayAction.Add(150 - Game.Ping / 2, () =>
                          {
                              q.Cast(qpos);

                              Orbwalker.DisableAttacking = false;
                              Orbwalker.DisableMovement = false;

                              canaa = true;
                              canmv = true;
                          });
                    }
                }
            }

            if (e.IsReady() && (cc == 3 || !q.IsReady() && cc == 0))
            {
                if (player.LSDistance(target.ServerPosition) <= 300)
                {
                    if (Getcheckboxvalue(harassMenu, "usegaph") && cane)
                        e.Cast(qpos);
                }
            }

            if (!target.ServerPosition.UnderTurret(true))
            {
                if (q.IsReady() && canq && (cc < 2 || e.IsReady()))
                {
                    if (target.LSDistance(player.ServerPosition) <= truerange + q.Range)
                    {
                        q.Cast(target.ServerPosition);
                    }
                }
            }

            if (e.IsReady() && cane && q.IsReady() && cc < 1 &&
                target.LSDistance(player.ServerPosition) > truerange + 100 &&
                target.LSDistance(player.ServerPosition) <= e.Range + truerange + 50)
            {
                if (!target.ServerPosition.UnderTurret(true))
                {
                    if (Getcheckboxvalue(harassMenu, "usegaph") && cane)
                    {
                        e.Cast(target.ServerPosition);
                    }
                }
            }

            else if (w.IsReady() && canw && target.LSDistance(player.ServerPosition) <= w.Range + 10)
            {
                if (!player.ServerPosition.UnderTurret(true))
                {
                    if (Getcheckboxvalue(harassMenu, "useharassw") && canw)
                    {
                        w.Cast();
                    }
                }
            }
        }

        #endregion

        #region Riven: Windslash

        private static void WindSlashExecute()
        {
            if (uo && Getcheckboxvalue(r2Menu, "usews") && r.IsReady())
            {
                #region Killsteal
                foreach (var t in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(r.Range)))
                {
                    if (t != null)
                    {
                        if (Getcheckboxvalue(r2Menu, "saver"))
                        {
                            if (player.GetAutoAttackDamage(t, true) * Getslidervalue(r2Menu, "overaa") >= t.Health &&
                                player.HealthPercent > 65)
                            {
                                if (Orbwalking.InAutoAttackRange(t) && player.CountEnemiesInRange(r.Range) > 1)
                                {
                                    continue;
                                }
                            }
                        }

                        if (Rdmg(t) >= t.Health)
                        {
                            var p = r.GetPrediction(t, true, -1f, new[] { CollisionableObjects.YasuoWall });
                            if (p.Hitchance == (HitChance)getBoxItem(r2Menu, "rhitc") + 4 && canws && !t.HasBuff("kindredrnodeathbuff"))
                            {
                                r.Cast(p.CastPosition);
                            }
                        }
                    }
                }
                #endregion
            }
        }

        private static void Windslash()
        {
            if (uo && Getcheckboxvalue(r2Menu, "usews") && r.IsReady())
            {
                if (Getkeybindvalue(keybindsMenu, "shycombo") && canburst())
                {
                    if (riventarget().LSDistance(player.ServerPosition) <= player.AttackRange + 100)
                    {
                        if (canhd) return;
                    }
                }

                #region MaxDmage

                if (getBoxItem(r2Menu, "wsmode") == 1)
                {
                    if (riventarget() != null)
                    {
                        if (riventarget().LSIsValidTarget(r.Range) && !riventarget().IsZombie)
                        {
                            if (Rdmg(riventarget()) / riventarget().MaxHealth * 100 >= 50)
                            {
                                var p = r.GetPrediction(riventarget(), true, -1f, new[] { CollisionableObjects.YasuoWall });
                                if (p.Hitchance >= HitChance.Medium && canws && !riventarget().HasBuff("kindredrnodeathbuff"))
                                {
                                    if (!isteamfightkappa || Getcheckboxvalue(r2Menu, "r" + riventarget().ChampionName) ||
                                         isteamfightkappa && !rrektAny())
                                    {
                                        r.Cast(p.CastPosition);
                                    }
                                }
                            }

                            if (q.IsReady() && cc <= 2)
                            {
                                var aadmg = player.LSGetAutoAttackDamage(riventarget(), true) * 2;
                                var currentrdmg = Rdmg(riventarget());
                                var qdmg = Qdmg(riventarget()) * 2;

                                var damage = aadmg + currentrdmg + qdmg;

                                if (riventarget().Health <= xtra((float)damage))
                                {
                                    if (riventarget().LSDistance(player.ServerPosition) <= truerange + q.Range)
                                    {
                                        var p = r.GetPrediction(riventarget(), true, -1f, new[] { CollisionableObjects.YasuoWall });
                                        if (p.Hitchance == HitChance.High && canws && !riventarget().HasBuff("kindredrnodeathbuff"))
                                        {
                                            if (!isteamfightkappa || Getcheckboxvalue(r2Menu, "r" + riventarget().ChampionName) ||
                                                 isteamfightkappa && !rrektAny())
                                            {
                                                r.Cast(p.CastPosition);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }

                foreach (var t in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(r.Range)))
                {
                    if (t != null)
                    {
                        if (Getcheckboxvalue(r2Menu, "saver"))
                        {
                            if (player.LSGetAutoAttackDamage(t, true) * Getslidervalue(r2Menu, "overaa") >= t.Health && player.HealthPercent > 65)
                            {
                                if (Orbwalking.InAutoAttackRange(t) && player.LSCountEnemiesInRange(r.Range) > 1)
                                {
                                    continue;
                                }
                            }
                        }

                        if (Rdmg(t) >= t.Health)
                        {
                            var p = r.GetPrediction(t, true, -1f, new[] { CollisionableObjects.YasuoWall });
                            if (p.Hitchance == (HitChance)getBoxItem(r2Menu, "rhitc") + 4 && canws && !t.HasBuff("kindredrnodeathbuff"))
                            {
                                r.Cast(p.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Lane/Jungle

        private static void Clear()
        {
            var minions = MinionManager.GetMinions(player.Position, 600f,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            foreach (var unit in minions.Where(m => !m.Name.Contains("Mini")))
            {
                OrbTo(unit);

                if (Utils.GameTimeTickCount - lastw < 500 && Utils.GameTimeTickCount - lasthd < 1000)
                {
                    if (unit.Distance(player.ServerPosition) <= q.Range + 90 && q.IsReady())
                    {
                        DoOneQ(unit.ServerPosition);
                    }
                }

                if (Utils.GameTimeTickCount - laste < 600)
                {
                    if (unit.LSDistance(player.ServerPosition) <= w.Range + 45)
                    {
                        if (Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                }

                if (e.IsReady() && cane && Getcheckboxvalue(farmMenu, "usejunglee"))
                {
                    if (player.Health / player.MaxHealth * 100 <= 70 ||
                        unit.LSDistance(player.ServerPosition) > truerange + 30)
                    {
                        e.Cast(unit.ServerPosition);
                    }
                }

                if (w.IsReady() && canw && Getcheckboxvalue(farmMenu, "usejunglew") && Utils.GameTimeTickCount - lasthd > 1600)
                {
                    if (unit.LSDistance(player.ServerPosition) <= w.Range + 25)
                    {
                        w.Cast();
                    }
                }

                if (q.IsReady() && canq && Getcheckboxvalue(farmMenu, "usejungleq"))
                {
                    if (unit.LSDistance(player.ServerPosition) <= q.Range + 90)
                    {
                        if (canhd) return;

                        if (qtarg != null && qtarg.NetworkId == unit.NetworkId)
                            q.Cast(unit.ServerPosition);
                    }
                }
            }
        }

        private static void Wave()
        {
            var minions = MinionManager.GetMinions(player.Position, 600f);

            foreach (var unit in minions.Where(x => x.IsMinion))
            {
                OrbTo(Getcheckboxvalue(farmMenu, "useaoeq") && GetCenterMinion().LSIsValidTarget()
                    ? GetCenterMinion()
                    : unit);

                if (q.IsReady() && unit.LSDistance(player.ServerPosition) <= truerange + 100)
                {
                    if (canq && Getcheckboxvalue(farmMenu, "uselaneq") && minions.Count >= 2 &&
                        (!player.ServerPosition.LSExtend(unit.ServerPosition, q.Range).UnderTurret(true) ||
                        !HeroManager.Enemies.Any(x => x.LSIsValidTarget(1400))))
                    {
                        if (GetCenterMinion().LSIsValidTarget() && Getcheckboxvalue(farmMenu, "useaoeq"))
                            q.Cast(GetCenterMinion());
                        else
                            q.Cast(unit.ServerPosition);
                    }
                }

                if (w.IsReady())
                {
                    if (minions.Count(m => m.LSDistance(player.ServerPosition) <= w.Range + 10) >= Getslidervalue(farmMenu, "wminion"))
                    {
                        if (canw && Getcheckboxvalue(farmMenu, "uselanew"))
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);

                            w.Cast();
                        }
                    }
                }

                if (e.IsReady() && !player.ServerPosition.LSExtend(unit.ServerPosition, e.Range).UnderTurret(true))
                {
                    if (unit.LSDistance(player.ServerPosition) > truerange + 30)
                    {
                        if (cane && Getcheckboxvalue(farmMenu, "uselanee"))
                        {
                            if (GetCenterMinion().LSIsValidTarget() && Getcheckboxvalue(farmMenu, "useaoeq"))
                                e.Cast(GetCenterMinion());
                            else
                                e.Cast(unit.ServerPosition);
                        }
                    }

                    else if (player.Health / player.MaxHealth * 100 <= 70)
                    {
                        if (cane && Getcheckboxvalue(farmMenu, "uselanee"))
                        {
                            if (GetCenterMinion().LSIsValidTarget() && Getcheckboxvalue(farmMenu, "useaoeq"))
                                q.Cast(GetCenterMinion());
                            else
                                q.Cast(unit.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Flee

        private static void Flee()
        {
            if (canmv)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (cc > 2 && Items.GetWardSlot() != null && Getcheckboxvalue(qMenu, "wq3"))
            {
                var attacker = HeroManager.Enemies.FirstOrDefault(x => x.LSDistance(player.ServerPosition) <= q.Range + 50);
                if (attacker.LSIsValidTarget(q.Range))
                {
                    if (Utils.GameTimeTickCount - lastwd >= 1000 && didq)
                    {
                        Utility.DelayAction.Add(100,
                            () => Items.UseItem((int)Items.GetWardSlot().Id, attacker.ServerPosition));
                    }
                }
            }

            if (player.LSCountEnemiesInRange(w.Range) > 0)
            {
                if (w.IsReady())
                    w.Cast();
            }

            if (ssfl)
            {
                if (Utils.GameTimeTickCount - lastq >= 600)
                {
                    q.Cast(Game.CursorPos);
                }

                if (cane && e.IsReady())
                {
                    if (cc >= 2 || !q.IsReady() && !player.HasBuff("RivenTriCleave"))
                    {
                        if (!player.ServerPosition.LSExtend(Game.CursorPos, e.Range + 10).LSIsWall())
                            e.Cast(Game.CursorPos);
                    }
                }
            }

            else
            {
                if (q.IsReady())
                {
                    q.Cast(Game.CursorPos);
                }

                if (e.IsReady() && Utils.GameTimeTickCount - lastq >= 250)
                {
                    if (!player.ServerPosition.LSExtend(Game.CursorPos, e.Range).LSIsWall())
                        e.Cast(Game.CursorPos);
                }
            }
        }

        #endregion

        #region Riven: Semi Q 

        private static void SemiQ()
        {
            if (canq && Utils.GameTimeTickCount - lastaa >= 150)
            {
                if (Getcheckboxvalue(keybindsMenu, "semiq"))
                {
                    if (q.IsReady() && Utils.GameTimeTickCount - lastaa < 1200 && qtarg != null)
                    {
                        if (qtarg.LSIsValidTarget(q.Range + 100) &&
                            !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                            !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                            !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                            !Getkeybindvalue(keybindsMenu, "shycombo"))
                        {
                            if (qtarg.IsValid<AIHeroClient>() && !qtarg.UnderTurret(true))
                                q.Cast(qtarg.ServerPosition);
                        }

                        if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                            !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                            !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                            !Getkeybindvalue(keybindsMenu, "shycombo"))
                        {
                            if (qtarg.LSIsValidTarget(q.Range + 100) && !qtarg.Name.Contains("Mini"))
                            {
                                if (!qtarg.Name.StartsWith("Minion") && minionlist.Any(name => qtarg.Name.StartsWith(name)))
                                {
                                    q.Cast(qtarg.ServerPosition);
                                }
                            }

                            if (qtarg.LSIsValidTarget(q.Range + 100))
                            {
                                if (qtarg.IsValid<AIHeroClient>() || qtarg.IsValid<Obj_AI_Turret>())
                                {
                                    if (uo)
                                        q.Cast(qtarg.ServerPosition);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Check R
        private static void checkr()
        {
            if (!r.IsReady() || uo || !Getkeybindvalue(r1Menu, "user"))
            {
                return;
            }

            if (Getkeybindvalue(keybindsMenu, "shycombo"))
            {
                r.Cast();
                return;
            }

            var targets = HeroManager.Enemies.Where(ene => ene.LSIsValidTarget(r.Range));
            var heroes = targets as IList<AIHeroClient> ?? targets.ToList();

            foreach (var target in heroes)
            {
                if (cc > Getslidervalue(r1Menu, "userq") || target == null)
                {
                    return;
                }

                if (target.Health / target.MaxHealth * 100 <= Getslidervalue(r1Menu, "overk") && IsLethal(target))
                {
                    if (heroes.Count() < 2)
                    {
                        continue;
                    }
                }

                if (getBoxItem(r1Menu, "ultwhen") == 2)
                    r.Cast();

                if (q.IsReady() || Utils.GameTimeTickCount - lastq < 1000 && cc < 3)
                {
                    if (heroes.Count() < 2)
                    {
                        if (target.Health / target.MaxHealth * 100 <= Getslidervalue(r1Menu, "overk") && IsLethal(target))
                            return;
                    }

                    if (heroes.Count(ene => ene.LSDistance(player.ServerPosition) <= 750) > 1)
                        r.Cast();

                    if (getBoxItem(r1Menu, "ultwhen") == 0)
                    {
                        if ((ComboDamage(target) / 1.3) >= target.Health && target.Health >= (ComboDamage(target) / 1.8))
                        {
                            r.Cast();
                        }
                    }

                    if (getBoxItem(r1Menu, "ultwhen") == 1)
                    {
                        if (ComboDamage(target) >= target.Health && target.Health >= ComboDamage(target) / 1.8)
                        {
                            r.Cast();
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: On Cast
        private static void OnCast()
        {
            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (args.SData.IsAutoAttack())
                {
                    qtarg = (Obj_AI_Base)args.Target;
                    lastaa = Utils.GameTimeTickCount;
                }

                if (!didq && args.SData.IsAutoAttack())
                {
                    var targ = (AttackableUnit)args.Target;
                    if (targ != null && player.LSDistance(targ.Position) <= q.Range + 120)
                    {
                        didaa = true;
                        canaa = false;
                        canq = false;
                        canw = false;
                        cane = false;
                        canws = false;
                        // canmv = false;
                    }
                }

                if (args.SData.Name.ToLower().Contains("ward"))
                    lastwd = Utils.GameTimeTickCount;

                switch (args.SData.Name)
                {
                    case "ItemTiamatCleave":
                        lasthd = Utils.GameTimeTickCount;
                        didhd = true;
                        canhd = false;

                        if (getBoxItem(r2Menu, "wsmode") == 1 || Getkeybindvalue(keybindsMenu, "shycombo"))
                        {
                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                            {
                                if (canburst() && uo)
                                {
                                    if (riventarget().LSIsValidTarget() && !riventarget().IsZombie && !riventarget().HasBuff("kindredrnodeathbuff"))
                                    {
                                        if (!isteamfightkappa || Getcheckboxvalue(r2Menu, "r" + riventarget().ChampionName) ||
                                             isteamfightkappa && !rrektAny() || Getkeybindvalue(keybindsMenu, "shycombo"))
                                        {
                                            Utility.DelayAction.Add(140 - Game.Ping / 2,
                                                () =>
                                                {
                                                    if (riventarget().HasBuffOfType(BuffType.Stun))
                                                        r.Cast(riventarget().ServerPosition);

                                                    if (!riventarget().HasBuffOfType(BuffType.Stun))
                                                        r.Cast(r.CastIfHitchanceEquals(riventarget(), HitChance.Medium));
                                                });
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "RivenTriCleave":
                        canq = false;
                        cc += 1;
                        didq = true;
                        didaa = false;
                        lastq = Utils.GameTimeTickCount;
                        canmv = false;

                        var dd = new[] { (291 - Getslidervalue(qMenu, "Qld") - (Game.Ping - Getslidervalue(qMenu, "Qd"))), (290 - Getslidervalue(qMenu, "Qld") - (Game.Ping - Getslidervalue(qMenu, "Qd"))), (343 - Getslidervalue(qMenu, "Qld") - (Game.Ping - Getslidervalue(qMenu, "Qd"))) };

                        Utility.DelayAction.Add(dd[Math.Max(cc, 1) - 1], () =>
                        {
                            Orbwalker.ResetAutoAttack();

                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) ||
                                Getkeybindvalue(keybindsMenu, "shycombo"))
                                Player.DoEmote(Emote.Dance);

                            else if (qtarg.LSIsValidTarget(450) && Getcheckboxvalue(keybindsMenu, "semiq"))
                                Player.DoEmote(Emote.Dance);
                        });


                        if (!uo) ssfl = false;
                        break;
                    case "RivenMartyr":
                        canq = false;
                        canmv = false;
                        didw = true;
                        lastw = Utils.GameTimeTickCount;
                        canw = false;

                        break;
                    case "RivenFeint":
                        canmv = false;
                        dide = true;
                        didaa = false;
                        laste = Utils.GameTimeTickCount;
                        cane = false;

                        if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                        {
                            if (uo && r.IsReady() && cc == 2 && q.IsReady())
                            {
                                var btarg = TargetSelector.GetTarget(r.Range, DamageType.Physical);
                                if (btarg.LSIsValidTarget())
                                    r.CastIfHitchanceEquals(btarg, HitChance.Medium);
                                else
                                    r.Cast(Game.CursorPos);
                            }
                        }

                        if (Getkeybindvalue(keybindsMenu, "shycombo"))
                        {
                            if (cc == 2 && !uo && r.IsReady() && riventarget() != null)
                            {
                                checkr();
                                Utility.DelayAction.Add(240 - Game.Ping, () => q.Cast(riventarget().ServerPosition));
                            }
                        }

                        if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                        {
                            if (cc == 2 && !uo && r.IsReady() && riventarget() != null)
                            {
                                checkr();
                                Utility.DelayAction.Add(240 - Game.Ping, () => q.Cast(riventarget().ServerPosition));
                            }

                            if (getBoxItem(r2Menu, "wsmode") == 1 && cc == 2 && uo)
                            {
                                if (riventarget().LSIsValidTarget(r.Range + 100) && IsLethal(riventarget()))
                                {
                                    Utility.DelayAction.Add(100 - Game.Ping,
                                    () => r.Cast(r.CastIfHitchanceEquals(riventarget(), HitChance.Medium)));
                                }
                            }
                        }

                        break;
                    case "RivenFengShuiEngine":
                        ssfl = true;
                        doFlash();
                        break;
                    case "RivenIzunaBlade":
                        ssfl = false;
                        didws = true;
                        canws = false;

                        if (w.IsReady() && riventarget().LSIsValidTarget(w.Range + 55))
                            w.Cast();

                        else if (q.IsReady() && riventarget().LSIsValidTarget())
                            q.Cast(riventarget().ServerPosition);

                        break;
                }
            };
        }

        #endregion

        #region Riven: Misc Events

        private static void DoOneQ(Vector3 pos)
        {
            canq = false;

            if (q.IsReady() && Utils.GameTimeTickCount - lastq > 5000)
            {
                if (q.Cast(pos))
                {
                    lastq = Utils.GameTimeTickCount;
                    didq = true;
                    canq = false;
                }
            }
        }

        private static void ChangeR1()
        {
            var changetime = Environment.TickCount - LastTick;


            if (Getkeybindvalue(r1Menu, "switchr1"))
            {
                if (getBoxItem(r1Menu, "ultwhen") == 0 && LastTick + 400 < Environment.TickCount)
                {
                    LastTick = Environment.TickCount;
                    r1Menu["ultwhen"].Cast<ComboBox>().CurrentValue = 1;
                }

                if (getBoxItem(r1Menu, "ultwhen") == 1 && LastTick + 400 < Environment.TickCount)
                {
                    LastTick = Environment.TickCount;
                    r1Menu["ultwhen"].Cast<ComboBox>().CurrentValue = 2;
                }
                if (getBoxItem(r1Menu, "ultwhen") == 2 && LastTick + 400 < Environment.TickCount)
                {
                    LastTick = Environment.TickCount;
                    r1Menu["ultwhen"].Cast<ComboBox>().CurrentValue = 0;
                }
            }

        }

        private static void ChangeR2()
        {
            var changetime = Environment.TickCount - LastTick;


            if (Getkeybindvalue(r2Menu, "switchr2"))
            {
                if (getBoxItem(r2Menu, "wsmode") == 0 && LastTick + 400 < Environment.TickCount)
                {
                    LastTick = Environment.TickCount;
                    r2Menu["wsmode"].Cast<ComboBox>().CurrentValue = 1;
                }

                if (getBoxItem(r2Menu, "wsmode") == 1 && LastTick + 400 < Environment.TickCount)
                {
                    LastTick = Environment.TickCount;
                    r2Menu["wsmode"].Cast<ComboBox>().CurrentValue = 0;
                }
            }

        }

        private static void Interrupter()
        {
            Interrupter2.OnInterruptableTarget += (sender, args) =>
            {
                if (Getcheckboxvalue(wMenu, "wint") && w.IsReady())
                {
                    if (!sender.Position.UnderTurret(true))
                    {
                        if (sender.LSIsValidTarget(w.Range))
                            w.Cast();

                        if (sender.LSIsValidTarget(w.Range + e.Range) && e.IsReady())
                        {
                            e.Cast(sender.ServerPosition);
                        }
                    }
                }

                if (Getcheckboxvalue(qMenu, "qint") && q.IsReady() && cc >= 2)
                {
                    if (!sender.Position.UnderTurret(true))
                    {
                        if (sender.LSIsValidTarget(q.Range))
                            q.Cast(sender.ServerPosition);

                        if (sender.LSIsValidTarget(q.Range + e.Range) && e.IsReady())
                        {
                            e.Cast(sender.ServerPosition);
                        }
                    }
                }
            };
        }

        private static void OnGapcloser()
        {
            AntiGapcloser.OnEnemyGapcloser += gapcloser =>
            {
                if (Getcheckboxvalue(wMenu, "wgap") && w.IsReady())
                {
                    if (gapcloser.Sender.LSIsValidTarget(w.Range) && gapcloser.Sender.IsEnemy)
                    {
                        if (!gapcloser.Sender.ServerPosition.UnderTurret(true))
                        {
                            if (!isteamfightkappa || Getcheckboxvalue(wMenu, "w" + gapcloser.Sender.ChampionName) || isteamfightkappa && !wrektAny())
                            {
                                w.Cast();
                            }
                        }
                    }
                }
            };
        }

        private void OnPlayAnimation()
        {
        }

        #endregion

        #region Riven: Aura

        private static void AuraUpdate()
        {
            if (!player.IsDead)
            {
                foreach (var buff in player.Buffs)
                {
                    //if (buff.Name == "RivenTriCleave")
                    //    cc = buff.Count;

                    if (buff.Name == "rivenpassiveaaboost")
                        pc = buff.Count;
                }

                if (player.HasBuff("RivenTriCleave") && !Getkeybindvalue(keybindsMenu, "shycombo"))
                {
                    if (player.GetBuff("RivenTriCleave").EndTime - Game.Time <= 0.25f)
                    {
                        if (!player.LSIsRecalling() && !player.Spellbook.IsChanneling)
                        {
                            var qext = player.ServerPosition.LSTo2D() +
                                       player.Direction.LSTo2D().LSPerpendicular() * q.Range + 100;

                            if (Getcheckboxvalue(qMenu, "keepq"))
                            {
                                if (qext.To3D().CountEnemiesInRange(200) <= 1 && !qext.To3D().UnderTurret(true))
                                {
                                    q.Cast(Game.CursorPos);
                                }
                            }
                        }
                    }
                }

                if (r.IsReady() && uo && Getcheckboxvalue(r2Menu, "keepr"))
                {
                    if (player.GetBuff("RivenFengShuiEngine").EndTime - Game.Time <= 0.25f)
                    {
                        if (!riventarget().LSIsValidTarget(r.Range) || riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (e.IsReady() && uo)
                                e.Cast(Game.CursorPos);

                            r.Cast(Game.CursorPos);
                        }

                        if (riventarget().LSIsValidTarget(r.Range) && !riventarget().HasBuff("kindredrnodeathbuff"))
                            r.CastIfHitchanceEquals(riventarget(), HitChance.High);
                    }
                }

                if (!player.HasBuff("rivenpassiveaaboost"))
                    Utility.DelayAction.Add(1000, () => pc = 1);

                if (cc > 2)
                    Utility.DelayAction.Add(1000, () => cc = 0);
            }
        }

        #endregion

        #region Riven : Combat/Orbwalk

        private static void OrbTo(Obj_AI_Base target, float rangeoverride = 0f)
        {
            if (canmv)
            {
                if (Getkeybindvalue(keybindsMenu, "shycombo"))
                {
                    if (target.LSIsValidTarget(truerange + 100))
                        Orbwalker.OrbwalkTo(Game.CursorPos);

                    else
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }

            if (canmv && canaa)
            {
                if (q.IsReady() || Utils.GameTimeTickCount - lastq <= 400 - Game.Ping && cc < 3)
                {
                    if (target.LSIsValidTarget(truerange + 200 + rangeoverride))
                    {
                        LastAATick = 0;
                    }
                }
            }
        }

        private static void CombatCore()
        {
            if (didaa && Utils.GameTimeTickCount - lastaa >=
                100 - Game.Ping / 2 + 55 + player.AttackCastDelay * 1000)
                didaa = false;

            if (didhd && canhd && Utils.GameTimeTickCount - lasthd >= 250)
                didhd = false;

            if (didq && Utils.GameTimeTickCount - lastq >= 500)
                didq = false;

            if (didw && Utils.GameTimeTickCount - lastw >= 266)
            {
                didw = false;
                canmv = true;
                canaa = true;
            }

            if (dide && Utils.GameTimeTickCount - laste >= 350)
            {
                dide = false;
                canmv = true;
                canaa = true;
            }

            if (didws && Utils.GameTimeTickCount - laste >= 366)
            {
                didws = false;
                canmv = true;
                canaa = true;
            }

            if (!canw && w.IsReady() && !(didaa || didq || dide))
                canw = true;

            if (!cane && e.IsReady() && !(didaa || didq || didw))
                cane = true;

            if (!canws && r.IsReady() && !didaa && uo)
                canws = true;

            if (!canaa && !(didq || didw || dide || didws || didhd || didhs) &&
                Utils.GameTimeTickCount - lastaa >= 1000)
                canaa = true;

            if (!canmv && !(didq || didw || dide || didws || didhd || didhs) &&
                Utils.GameTimeTickCount - lastaa >= 1100)
                canmv = true;
        }

        #endregion

        #region Riven: Math/Damage

        private static float ComboDamage(Obj_AI_Base target, bool checkq = false)
        {
            if (target == null)
                return 0f;

            var ignote = player.GetSpellSlot("summonerdot");
            var ad = (float)player.LSGetAutoAttackDamage(target);
            var runicpassive = new[] { 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };

            var ra = ad +
                        (float)
                            ((+player.FlatPhysicalDamageMod + player.BaseAttackDamage) *
                            runicpassive[Math.Min(player.Level, 18) / 3]);

            var rw = Wdmg(target);
            var rq = Qdmg(target);
            var rr = r.IsReady() ? Rdmg(target) : 0;

            var ii = (ignote != SpellSlot.Unknown && player.GetSpell(ignote).State == SpellState.Ready && r.IsReady()
                ? player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                : 0);

            var tmt = Items.HasItem(3077) && Items.CanUseItem(3077)
                ? player.GetItemDamage(target, Damage.DamageItems.Tiamat)
                : 0;

            var hyd = Items.HasItem(3074) && Items.CanUseItem(3074)
                ? player.GetItemDamage(target, Damage.DamageItems.Hydra)
                : 0;

            var tdh = Items.HasItem(3748) && Items.CanUseItem(3748)
                ? player.GetItemDamage(target, Damage.DamageItems.Hydra)
                : 0;

            var bwc = Items.HasItem(3144) && Items.CanUseItem(3144)
                ? player.GetItemDamage(target, Damage.DamageItems.Bilgewater)
                : 0;

            var brk = Items.HasItem(3153) && Items.CanUseItem(3153)
                ? player.GetItemDamage(target, Damage.DamageItems.Botrk)
                : 0;

            var items = tmt + hyd + tdh + bwc + brk;

            var damage = (rq * 3 + ra * 3 + rw + rr + ii + items);

            return xtra((float)damage);
        }


        private static double Wdmg(Obj_AI_Base target)
        {
            double dmg = 0;
            if (w.IsReady() && target != null)
            {
                dmg += player.CalcDamage(target, DamageType.Physical,
                    new[] { 50, 80, 110, 150, 170 }[w.Level - 1] + 1 * player.FlatPhysicalDamageMod + player.BaseAttackDamage);
            }

            return dmg;
        }

        private static double Qdmg(Obj_AI_Base target)
        {
            double dmg = 0;
            if (q.IsReady() && target != null)
            {
                dmg += player.CalcDamage(target, DamageType.Physical,
                    -10 + (q.Level * 20) + (0.35 + (q.Level * 0.05)) * (player.FlatPhysicalDamageMod + player.BaseAttackDamage));
            }

            return dmg;
        }

        private static double Rdmg(Obj_AI_Base target)
        {
            double dmg = 0;

            if (r.IsReady() && target != null)
            {
                dmg += player.CalcDamage(target, DamageType.Physical,
                    (new double[] { 80, 120, 160 }[Math.Max(r.Level, 1) - 1] + 0.6 * player.FlatPhysicalDamageMod) *
                    (((target.MaxHealth - target.Health) / target.MaxHealth) * 2.67 + 1));
            }

            return dmg;
        }

        #endregion

        #region Riven: Drawings

        private static void Drawings()
        {
            Drawing.OnDraw += args =>
            {
                var R1 = Drawing.WorldToScreen(ObjectManager.Player.Position);
                var R2 = Drawing.WorldToScreen(ObjectManager.Player.Position);
                if (!player.IsDead)
                {
                    if (riventarget().LSIsValidTarget())
                    {
                        var tpos = Drawing.WorldToScreen(riventarget().Position);

                        if (Getcheckboxvalue(drawMenu, "drawf"))
                        {
                            Render.Circle.DrawCircle(riventarget().Position, 120, System.Drawing.Color.GreenYellow);

                        }

                        if (riventarget().HasBuff("Stun"))
                        {
                            var b = riventarget().GetBuff("Stun");
                            if (b.Caster.IsMe && b.EndTime - Game.Time > 0)
                            {
                                Drawing.DrawText(tpos[0], tpos[1], System.Drawing.Color.Lime, "STUNNED " + (b.EndTime - Game.Time).ToString("F"));
                            }
                        }
                    }

                    if (_sh.LSIsValidTarget())
                    {
                        if (Getcheckboxvalue(drawMenu, "drawf"))
                        {
                            Render.Circle.DrawCircle(_sh.Position, 90, System.Drawing.Color.Green, 6);

                        }
                    }

                    if (Getcheckboxvalue(drawMenu, "drawengage"))
                    {
                        Render.Circle.DrawCircle(player.Position,
                                player.AttackRange + e.Range + 35, System.Drawing.Color.Red,
                               Getslidervalue(drawMenu, "linewidth"));
                    }

                    if (Getcheckboxvalue(drawMenu, "drawr2"))
                    {

                        Render.Circle.DrawCircle(player.Position, r.Range, System.Drawing.Color.Green, Getslidervalue(drawMenu, "linewidth"));
                    }

                    if (Getcheckboxvalue(drawMenu, "drawburst") && (canburst() || shy()) && riventarget().LSIsValidTarget())
                    {
                        var xrange = Getcheckboxvalue(r1Menu, "flashb") && flash.IsReady() ? 255 : 0;
                        Render.Circle.DrawCircle(riventarget().Position, e.Range + w.Range - 25 + xrange,
                            System.Drawing.Color.Green, Getslidervalue(drawMenu, "linewidth"));
                    }
                    if (Getcheckboxvalue(drawMenu, "drawr1mode"))
                    {
                        if (getBoxItem(r1Menu, "ultwhen") == 0) Drawing.DrawText(R1[0] - 45, R1[1] + 20, Color.White, "R1:Normal Kill");
                        else if (getBoxItem(r1Menu, "ultwhen") == 1) Drawing.DrawText(R1[0] - 45, R1[1] + 20, Color.White, "R1:Hard Kill");
                        else if (getBoxItem(r1Menu, "ultwhen") == 2) Drawing.DrawText(R1[0] - 45, R1[1] + 20, Color.White, "R1:Always");
                    }
                    if (Getcheckboxvalue(drawMenu, "drawr2mode"))
                    {
                        if (getBoxItem(r2Menu, "wsmode") == 0) Drawing.DrawText(R2[0] - 45, R2[1] + 50, Color.White, "R2:Kill Only");
                        else if (getBoxItem(r2Menu, "wsmode") == 1) Drawing.DrawText(R2[0] - 45, R2[1] + 50, Color.White, "R2:Max Damage");
                    }
                }
            };

            Drawing.OnEndScene += args =>
            {
                if (!Getcheckboxvalue(drawMenu, "drawdmg"))
                    return;

                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
                {
                    var color = r.IsReady() && IsLethal(enemy)
                        ? new ColorBGRA(0, 255, 0, 90)
                        : new ColorBGRA(255, 255, 0, 90);

                    hpi.unit = enemy;
                    hpi.drawDmg(ComboDamage(enemy), color);
                }

            };
        }

        #endregion
    }
}
