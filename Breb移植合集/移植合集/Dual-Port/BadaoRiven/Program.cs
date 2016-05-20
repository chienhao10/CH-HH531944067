using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using ItemData = LeagueSharp.Common.Data.ItemData;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace HeavenStrikeRiven
{
    public class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static string R1name = "RivenFengShuiEngine";

        private static string R2name = "RivenIzunaBlade";

        private static Spell Q, W, E, R;

        private static SpellSlot flash = Player.GetSpellSlot("summonerflash");

        private static Menu Menu;
        public static Menu spellMenu, burstMenu, miscMenu, drawMenu, clearMenu;


        public static bool waitE, waitQ, waitAA, waitW, waitTiamat, waitR1, waitR2, midAA, canAA, forceQ, forceW, forceT, forceR, waitR, castR, forceEburst, qGap
            , R2style;
        public static int waitQTick, waitR2Tick;
        private static AttackableUnit TTTar = null;

        public static float cE, cQ, cAA, cW, cTiamt, cR1, cR2, Wind, countforce, Rstate, R2countdonw;
        public static int Qstate = 1;

        public static void OnStart()
        {
            if (Player.ChampionName != "Riven")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 250);
            R = new Spell(SpellSlot.R, 900);
            R.SetSkillshot(0.25f, 45, 1600, false, SkillshotType.SkillshotCone);
            R.MinHitChance = HitChance.Medium;

            Menu = MainMenu.AddMenu("Badao Riven", "Riven");

            spellMenu = Menu.AddSubMenu("Spells", "Spells");
            spellMenu.Add("RcomboAlways", new CheckBox("Always use R", false));
            spellMenu.Add("RcomboKillable", new CheckBox("R combo Killable", true));
            spellMenu.Add("R2comboKS", new CheckBox("R2 KS", true));
            spellMenu.Add("R2comboMaxdmg", new CheckBox("R Max damage", true));
            spellMenu.Add("R2badaostyle", new CheckBox("R2 Badao Style", true));
            spellMenu.Add("Ecombo", new CheckBox("E Combo", true));
            spellMenu.Add("QGap", new CheckBox("Q Gap", true));
            spellMenu.Add("Qexpire", new CheckBox("Use Q Before Expiry", true));
            spellMenu.Add("Qmode", new ComboBox("Q cast mode", 1, "Lock Target", "To Mouse"));

            burstMenu = Menu.AddSubMenu("Burst Combo", "Burst");
            burstMenu.Add("doBurst", new KeyBind("Burst", false, KeyBind.BindTypes.HoldActive, 'T'));
            burstMenu.Add("UseFlash", new CheckBox("Use Flash", true));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("Winterrupt", new CheckBox("W Gapcloser", true));
            miscMenu.Add("Wgapcloser", new CheckBox("W Interrupt", true));
            miscMenu.Add("flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'Z'));
            miscMenu.Add("WallJumpHelper", new KeyBind("WallJump", false, KeyBind.BindTypes.HoldActive, 'N'));
            miscMenu.Add("Drawdmg", new CheckBox("Draw dmg text", true));

            clearMenu = Menu.AddSubMenu("Clear", "Clear");
            clearMenu.Add("UseTiamat", new CheckBox("Use Tiamat", true));
            clearMenu.Add("useQ", new CheckBox("Use Q", true));
            clearMenu.Add("useW", new CheckBox("Use W", true));
            clearMenu.Add("useE", new CheckBox("Use E", true));

            Drawing.OnDraw += OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPostAttack += AfterAttack;
            Orbwalker.OnAttack += OnAttack;
            Obj_AI_Base.OnProcessSpellCast += oncast;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Interrupter2.OnInterruptableTarget += interrupt;
            AntiGapcloser.OnEnemyGapcloser += gapcloser;

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


        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Animation == "Spell1a")
            {
                if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
                    Utility.DelayAction.Add(280 - Game.Ping, () => Chat.Say("/d"));
                Qstate = 2;
            }
            else if (args.Animation == "Spell1b")
            {
                if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
                    Utility.DelayAction.Add(300 - Game.Ping, () => Chat.Say("/d"));
                Qstate = 3;
            }
            else if (args.Animation == "Spell1c")
            {
                if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
                    Utility.DelayAction.Add(380 - Game.Ping, () => Chat.Say("/d"));
                Qstate = 1;
            }
        }

        public static int Qmode { get { return spellMenu["Qmode"].Cast<ComboBox>().CurrentValue; } }
        public static bool Qstrangecancel { get { return spellMenu["QStrangeCancel"].Cast<CheckBox>().CurrentValue; } }
        public static bool Rcomboalways { get { return spellMenu["RcomboAlways"].Cast<CheckBox>().CurrentValue; } }
        public static bool RcomboKillable { get { return spellMenu["RcomboKillable"].Cast<CheckBox>().CurrentValue; } }
        public static bool R2comboKS { get { return spellMenu["R2comboKS"].Cast<CheckBox>().CurrentValue; } }
        public static bool R2comboMaxdmg { get { return spellMenu["R2comboMaxdmg"].Cast<CheckBox>().CurrentValue; } }
        public static bool R2BadaoStyle { get { return spellMenu["R2badaostyle"].Cast<CheckBox>().CurrentValue; } }
        public static bool Ecombo { get { return spellMenu["Ecombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool QGap { get { return spellMenu["QGap"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseQBeforeExpiry { get { return spellMenu["Qexpire"].Cast<CheckBox>().CurrentValue; } }
        public static bool BurstActive { get { return burstMenu["Burst"].Cast<KeyBind>().CurrentValue; } }
        public static bool FlashBurst { get { return burstMenu["UseFlash"].Cast<CheckBox>().CurrentValue; } }
        public static bool Wgapcloser { get { return miscMenu["Wgapcloser"].Cast<CheckBox>().CurrentValue; } }
        public static bool Winterrupt { get { return miscMenu["Winterrupt"].Cast<CheckBox>().CurrentValue; } }
        public static bool Drawdamage { get { return miscMenu["Drawdmg"].Cast<CheckBox>().CurrentValue; } }
        public static bool FleeActive { get { return miscMenu["flee"].Cast<KeyBind>().CurrentValue; } }
        public static bool WallJumpHelperActive { get { return miscMenu["WallJumpHelper"].Cast<KeyBind>().CurrentValue; } }
        public static bool UseTiamatClear { get { return clearMenu["UseTiamat"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseQClear { get { return clearMenu["UseQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseWClear { get { return clearMenu["UseW"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseEClear { get { return clearMenu["UseE"].Cast<CheckBox>().CurrentValue; } }
        public static bool doBurst { get { return burstMenu["doBurst"].Cast<KeyBind>().CurrentValue; } }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            SolvingWaitList();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                fastharass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }

            if (doBurst)
                Burst();

            if (WallJumpHelperActive)
                walljump();

            if (FleeActive)
                flee();
        }
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            var target = TargetSelector.SelectedTarget;
            if (target != null && target.IsValidTarget() && !target.IsZombie)
                Render.Circle.DrawCircle(target.Position, 150, Color.AliceBlue, 15);
            if (Drawdamage)
                foreach (var hero in HeroManager.Enemies)
                {
                    if (hero.IsValidTarget(1500))
                    {
                        var dmg = totaldame(hero) > hero.Health ? 100 : totaldame(hero) * 100 / hero.Health;
                        var dmg1 = Math.Round(dmg);
                        var x = Drawing.WorldToScreen(hero.Position);
                        Color mau = dmg1 == 100 ? Color.Red : Color.Yellow;
                        Drawing.DrawText(x[0], x[1], mau, dmg1.ToString() + " %");
                    }
                }
        }
        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            TTTar = target;
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (HasItem())
                {
                    CastItem();
                }
                else if (R2BadaoStyle && R.IsReady() && R.Instance.Name == R2name && Qstate == 3)
                {
                    if (target is Obj_AI_Base)
                    {
                        R.Cast(target as Obj_AI_Base);
                    }
                    if (Q.IsReady())
                    {
                        Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (W.IsReady() && InWRange(target))
                {
                    W.Cast();
                    if (Q.IsReady())
                    {
                        Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady())
                {
                    callbackQ(TTTar);
                }
                else if (E.IsReady() && Ecombo)
                {
                    E.Cast(target.Position);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if (HasItem() && UseTiamatClear)
                {
                    CastItem();
                }
                else if (W.IsReady() && InWRange(target) && UseWClear)
                {
                    W.Cast();
                    if (Q.IsReady() && UseQClear)
                    {
                        Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady() && UseQClear)
                {
                    callbackQ(TTTar);
                }
                else if (E.IsReady() && UseEClear)
                {
                    E.Cast(target.Position);
                }
            }
            if (doBurst)
            {
                if (HasItem())
                {
                    CastItem();
                    if (R.IsReady() && R.Instance.Name == R2name)
                    {
                        if (target is AIHeroClient)
                        {
                            callbackR2(TTTar);
                        }
                        if (Q.IsReady())
                        {
                            Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                        }
                    }
                    else if (Q.IsReady())
                    {
                        callbackQ(TTTar);
                    }

                }
                else if (R.IsReady() && R.Instance.Name == R2name)
                {
                    if (target is AIHeroClient)
                    {
                        R.Cast(target as AIHeroClient);
                    }
                    if (Q.IsReady())
                    {
                        Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (W.IsReady() && InWRange(target))
                {
                    W.Cast();
                    if (Q.IsReady())
                    {
                        Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady())
                {
                    callbackQ(TTTar);
                }
                else if (E.IsReady() && Ecombo)
                {
                    E.Cast(target.Position);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (HasItem())
                {
                    CastItem();
                }
                else if (W.IsReady() && InWRange(target))
                {
                    W.Cast();
                    if (Q.IsReady())
                    {
                        Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady())
                {
                    Q.Cast(target.Position);
                }
                else if (E.IsReady())
                {
                    E.Cast(target.Position);
                }
            }
        }
        public static void interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && W.IsReady() && sender.IsValidTarget() && !sender.IsZombie && Winterrupt)
            {
                if (sender.IsValidTarget(125 + Player.BoundingRadius + sender.BoundingRadius)) W.Cast();
            }
        }
        public static void gapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;
            if (target.IsEnemy && W.IsReady() && target.IsValidTarget() && !target.IsZombie && Wgapcloser)
            {
                if (target.IsValidTarget(125 + Player.BoundingRadius + target.BoundingRadius)) W.Cast();
            }
        }
        private static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;

            if (!sender.IsMe)
            {
                return;
            }
            if (spell.Name.Contains("ItemTiamatCleave"))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (Q.IsReady())
                    {
                        callbackQ(TTTar);
                    }
                }
            }
            if (args.SData.IsAutoAttack())
            {

            }
            if (spell.Name.Contains("RivenTriCleave"))
            {

                waitQ = false;
                Orbwalker.ResetAutoAttack();
                if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
                {
                    Utility.DelayAction.Add(40, () => Reset(40));
                }

                cQ = Utils.GameTimeTickCount;
            }
            if (spell.Name.Contains("RivenMartyr"))
            {
                Utility.DelayAction.Add(160 - Game.Ping, () => Chat.Say("/d"));

            }
            if (spell.Name.Contains("RivenFient"))
            {

                if (doBurst)
                {
                    if (R.IsReady() && R.Instance.Name == R1name)
                        Utility.DelayAction.Add(150, () => R.Cast());
                }
            }
            if (spell.Name.Contains("RivenFengShuiEngine"))
            {
                Utility.DelayAction.Add(140 - Game.Ping, () => Chat.Say("/d"));

            }
            if (spell.Name.Contains("rivenizunablade"))
            {
                Utility.DelayAction.Add(140 - Game.Ping, () => Chat.Say("/d"));

            }
        }

        private static void Reset(int t)
        {
            Utility.DelayAction.Add(0, () => Orbwalker.ResetAutoAttack());
            for (int i = 10; i < t; i = i + 10)
            {
                if (i - Game.Ping >= 0)
                    Utility.DelayAction.Add(i - Game.Ping, () => Cancel());
            }
        }
        private static void Cancel()
        {
           EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.LSExtend(Game.CursorPos, Player.LSDistance(Game.CursorPos) + 500));
            if (Qstrangecancel) Chat.Say("/d");
        }
        public static void OnAttack(AttackableUnit target, EventArgs args)
        {
            if (target.IsMe && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }
        }

        private static void Burst()
        {
            var target = TargetSelector.SelectedTarget;
            Orbwalker.ForcedTarget = target;
            Orbwalker.OrbwalkTo(target.ServerPosition);
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Orbwalking.InAutoAttackRange(target) && (!R.IsReady() || (R.IsReady() && R.Instance.Name == R1name)))
                {
                    W.Cast();
                }
                if (Orbwalking.InAutoAttackRange(target) && R.IsReady())
                {
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    Utility.DelayAction.Add(350, () => CastItem());
                    Utility.DelayAction.Add(400, () => W.Cast());
                }
                if (!Orbwalking.InAutoAttackRange(target) && E.IsReady() && R.IsReady() && Player.LSDistance(target.Position) <= E.Range + Player.BoundingRadius + target.BoundingRadius)
                {
                    E.Cast(Player.Position.LSExtend(target.Position, 200));
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    Utility.DelayAction.Add(350, () => CastItem());
                    Utility.DelayAction.Add(400, () => W.Cast());
                }
                if (!Orbwalking.InAutoAttackRange(target) && !E.IsReady() && R.IsReady() && !Player.IsDashing()
                    && flash != SpellSlot.Unknown && flash.IsReady() && FlashBurst && Player.LSDistance(target.Position) <= 425 + Player.BoundingRadius + target.BoundingRadius)
                {
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    var x = Player.LSDistance(target.Position) > 425 ? Player.Position.LSExtend(target.Position, 425) : target.Position;
                    Player.Spellbook.CastSpell(flash, x);
                    Utility.DelayAction.Add(350, () => CastItem());
                    Utility.DelayAction.Add(400, () => W.Cast());
                }
                if (!Orbwalking.InAutoAttackRange(target) && E.IsReady() && flash != SpellSlot.Unknown && flash.IsReady() && FlashBurst
                    && R.IsReady() && Player.LSDistance(target.Position) <= E.Range + Player.BoundingRadius + target.BoundingRadius + 425
                    && Player.LSDistance(target.Position) > Player.BoundingRadius + target.BoundingRadius + 425)
                {
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    E.Cast(Player.Position.LSExtend(target.Position, 200));
                    Utility.DelayAction.Add(350, () => Player.Spellbook.CastSpell(flash, target.Position));
                    Utility.DelayAction.Add(350, () => CastItem());
                    Utility.DelayAction.Add(500, () => W.Cast());
                }
            }
        }

        private static void Combo()
        {
            if (Q.IsReady() && QGap && !Player.IsDashing())
            {
                var target = HeroManager.Enemies.Where(x => x.IsValidTarget()).OrderByDescending(x => 1 - x.LSDistance(Player.Position)).FirstOrDefault();
                if (!Player.IsDashing() && Utils.GameTimeTickCount - cQ >= 1000 && target.IsValidTarget())
                {
                    if (LeagueSharp.Common.Prediction.GetPrediction(Player, 100).UnitPosition.LSDistance(target.Position) <= Player.LSDistance(target.Position))
                        Q.Cast(Game.CursorPos);
                }
            }
            if (W.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && InWRange(x));
                if (targets.Any())
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && Ecombo)
            {
                var target = TargetSelector.GetTarget(325 + Player.AttackRange + 70, DamageType.Physical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    E.Cast(target.Position);
                }
            }
            if (R.IsReady())
            {
                if (R.Instance.Name == R1name)
                {
                    if (Rcomboalways)
                    {
                        var target = TargetSelector.GetTarget(325 + Player.AttackRange + 70, DamageType.Physical);
                        if (target.IsValidTarget() && !target.IsZombie && E.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            var targetR = TargetSelector.GetTarget(200 + Player.BoundingRadius + 70, DamageType.Physical);
                            if (targetR.IsValidTarget() && !targetR.IsZombie)
                            {
                                R.Cast();
                            }
                        }

                    }
                    if (RcomboKillable)
                    {
                        var targetR = TargetSelector.GetTarget(200 + Player.BoundingRadius + 70, DamageType.Physical);
                        if (targetR.IsValidTarget() && !targetR.IsZombie && basicdmg(targetR) <= targetR.Health && totaldame(targetR) >= targetR.Health)
                        {
                            R.Cast();
                        }
                        if (targetR.IsValidTarget() && !targetR.IsZombie && Player.CountEnemiesInRange(800) >= 2)
                        {
                            R.Cast();
                        }
                    }
                }
                else if (R.Instance.Name == R2name)
                {
                    if (R2comboKS)
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health < Rdame(target, target.Health))
                                R.Cast(target);
                        }
                    }
                    if (R2comboMaxdmg)
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health / target.MaxHealth <= 0.25)
                                R.Cast(target);
                        }
                    }
                    if (R2BadaoStyle && !Q.IsReady())
                    {
                        var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                        if (target.IsValidTarget() && !target.IsZombie)
                        {
                            R.Cast(target);
                        }
                    }
                    var targethits = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (targethits.IsValidTarget() && !targethits.IsZombie)
                        R.CastIfWillHit(targethits, 4);

                }
            }
        }
        private static void Clear()
        {
            var targetW = MinionManager.GetMinions(Player.Position, WRange() + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
            var targetW2 = MinionManager.GetMinions(Player.Position, WRange() + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (targetW != null && InWRange(targetW) && W.IsReady() && UseWClear)
            {
                W.Cast();
            }
            if (targetW2 != null && InWRange(targetW2) && W.IsReady() && UseWClear)
            {
                W.Cast();
            }
            if (targetW != null && InWRange(targetW) && E.IsReady() && UseEClear)
            {
                E.Cast(targetW.Position);
            }
            if (targetW2 != null && InWRange(targetW2) && E.IsReady()  && UseEClear)
            {
                E.Cast(targetW2.Position);
            }
        }
        public static void fastharass()
        {
            if (W.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && InWRange(x));
                if (targets.Any())
                {
                    W.Cast();
                }
            }
            if (E.IsReady())
            {
                var target = TargetSelector.GetTarget(325 + Player.AttackRange + 70, DamageType.Physical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    E.Cast(target.Position);
                }
            }
        }
        private static void SolvingWaitList()
        {
            if (!Q.IsReady(1000)) Qstate = 1;
            if (waitQ == true && TTTar.IsValidTarget())
            {
                //if (Utils.GameTimeTickCount - cQ >= 350 + Player.AttackCastDelay - Game.Ping / 2)
                if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.LaneClear)
                {
                    if (Qmode == 0 && TTTar != null)
                        Q.Cast(TTTar.Position);
                    else
                        Q.Cast(Game.CursorPos);
                }
                else
                {
                    if (Qmode == 0 && TTTar != null)
                        Q.Cast(TTTar.Position);
                    else
                        Q.Cast(Game.CursorPos);
                }
                if (Environment.TickCount - waitQTick >= 500 + Game.Ping / 2)
                    waitQ = false;
            }
            if (waitR2 == true && TTTar.IsValidTarget())
            {
                R.Cast(TTTar as Obj_AI_Base);
                if (Environment.TickCount - waitQTick >= 500 + Game.Ping / 2)
                    waitQ = false;
            }
            if (Q.IsReady() && UseQBeforeExpiry && !Player.IsRecalling())
            {
                if (Qstate != 1 && Utils.GameTimeTickCount - cQ <= 3800 - Game.Ping / 2 && Utils.GameTimeTickCount - cQ >= 3300 - Game.Ping / 2) { Q.Cast(Game.CursorPos); }
            }
        }
        public static bool HasItem()
        {
            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
        }

        private static bool InWRange(AttackableUnit target)
        {
            if (Player.HasBuff("RivenFengShuiEngine"))
            {
                return
                    target.BoundingRadius + 200 + Player.BoundingRadius >= Player.LSDistance(target.Position);
            }
            else
            {
                return
                   target.BoundingRadius + 125 + Player.BoundingRadius >= Player.LSDistance(target.Position);
            }
        }
        private static float WRange()
        {
            if (Player.HasBuff("RivenFengShuiEngine"))
            {
                return
                    200 + Player.BoundingRadius;
            }
            else
            {
                return
                   125 + Player.BoundingRadius;
            }
        }
        private static void callbackQ(AttackableUnit target)
        {
            waitQ = true;
            TTTar = target;
            waitQTick = Environment.TickCount;
        }
        private static void callbackR2(AttackableUnit target)
        {
            waitR2 = true;
            TTTar = target;
            waitR2Tick = Environment.TickCount;
        }
        public static void checkbuff()
        {
            String temp = "";
            foreach (var buff in Player.Buffs)
            {
                temp += (buff.Name + "(" + buff.Count + ")" + "(" + buff.Type.ToString() + ")" + ", ");
            }
            Chat.Say(temp);
        }
        public static double basicdmg(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.GetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.IsReady())
                {
                    var qnhan = 4 - Qstate;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.GetAutoAttackDamage(target) * (1 + passivenhan);
                return dmg;
            }
            else { return 0; }
        }
        public static double totaldame(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.GetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.IsReady())
                {
                    var qnhan = 4 - Qstate;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.GetAutoAttackDamage(target) * (1 + passivenhan);
                if (R.IsReady())
                {
                    if (Rstate == 0)
                    {
                        var rdmg = Rdame(target, target.Health - dmg * 1.2);
                        return dmg * 1.2 + rdmg;
                    }
                    else if (Rstate == 1)
                    {
                        var rdmg = Rdame(target, target.Health - dmg);
                        return rdmg + dmg;
                    }
                    else return dmg;
                }
                else return dmg;
            }
            else return 0;
        }
        public static double Rdame(Obj_AI_Base target, double health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75 ? 0.75 : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * (8 / 3);
                var rawdmg = new double[] { 80, 120, 160 }[R.Level - 1] + 0.6 * Player.FlatPhysicalDamageMod;
                return Player.CalcDamage(target, DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            else return 0;
        }

        public static void walljump()
        {
            var x = Player.Position.LSExtend(Game.CursorPos, 100);
            var y = Player.Position.LSExtend(Game.CursorPos, 30);
            if (!x.IsWall() && !y.IsWall()) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, x);
            if (x.IsWall() && !y.IsWall()) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, y);
            if (LeagueSharp.Common.Prediction.GetPrediction(Player, 500).UnitPosition.Distance(Player.Position) <= 10) { Q.Cast(Game.CursorPos); }
        }
        public static void flee()
        {
           EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var x = Player.Position.LSExtend(Game.CursorPos, 300);
            if (Q.IsReady() && !Player.IsDashing()) Q.Cast(Game.CursorPos);
            if (E.IsReady() && !Player.IsDashing()) E.Cast(x);
        }

    }
}
