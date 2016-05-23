using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.SDK.Core.Utils;

namespace Taliyah
{
    using System;
    using System.Linq;
    using LeagueSharp.SDK;
    using SharpDX;

    using Menu = Menu;
    using SkillshotType = LeagueSharp.SDK.SkillshotType;
    using Spell = LeagueSharp.SDK.Spell;


    class Program
    {
        private static Menu main_menu;
        private static Spell Q, W, E;
        private static Vector3 lastE;
        private static int lastETick = Environment.TickCount;
        private static bool Q5x = true;
        private static bool EWCasting;
        static void Main()
        {
            Events.OnLoad += OnLoad;
        }

        private static Menu combo, harass, laneclear;


        private static void OnLoad(object sender, EventArgs e)
        {
            if (ObjectManager.Player.ChampionName != "Taliyah")
                return;

            main_menu = MainMenu.AddMenu("塔莉雅", "taliyah");

            main_menu.AddLabel("从 Shine 移植来的塔莉雅脚本");
            main_menu.AddLabel("使用Berb库");

            main_menu.AddSeparator();
            
            combo = main_menu.AddSubMenu("连招");
            harass = main_menu.AddSubMenu("骚扰");
            laneclear = main_menu.AddSubMenu("清线");

            main_menu.Add("onlyq5", new CheckBox("只施放 5x Q", false));
            main_menu.Add("antigap", new CheckBox("自动 E 防突进"));
            main_menu.Add("interrupt", new CheckBox("自动 W 技能打断"));

            //////////////////////////////////////////////////////////////////////////////////////////////////

            combo.Add("useq", new CheckBox("使用 Q"));
            combo.Add("usew", new CheckBox("使用 W"));
            combo.Add("usee", new CheckBox("使用 E"));

            //////////////////////////////////////////////////////////////////////////////////////////////////

            harass.Add("useq", new CheckBox("使用 Q"));
            harass.Add("manaperc", new Slider("最低蓝量使用", 40));

            //////////////////////////////////////////////////////////////////////////////////////////////////

            laneclear.Add("useq", new CheckBox("使用 Q"));
            laneclear.Add("useew", new CheckBox("使用 EW", false));
            laneclear.Add("minq", new Slider("最少 Q 命中数", 3, 1, 6));
            laneclear.Add("minew", new Slider("最少. EW 命中数", 5, 1, 6));
            laneclear.Add("manaperc", new Slider("最低蓝量使用", 40));
            
            //////////////////////////////////////////////////////////////////////////////////////////////////


            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0f, 60f, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 800f);
            W.SetSkillshot(0.5f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 700f);
            E.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Events.OnGapCloser += Events_OnGapCloser;
            Events.OnInterruptableTarget += Events_OnInterruptableTarget;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;

            Chat.Print("Taliyah | Loaded", Color.Azure);
        }

        private static bool GetBool(Menu m, string s) => m[s].Cast<CheckBox>().CurrentValue;
        private static int GetSlider(Menu m, string s) => m[s].Cast<Slider>().CurrentValue;

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.E)
                lastETick = Environment.TickCount;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Name == "Taliyah_Base_Q_aoe_bright.troy")
                Q5x = false;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Name == "Taliyah_Base_Q_aoe_bright.troy")
                Q5x = true;
        }

        private static void Combo()
        {
            if (W.Instance.Name == "TaliyahWNoClick")
            {
                if (Environment.TickCount - lastETick < 3000)
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, lastE, false);
            }
            else
            {
                if (W.IsReady()) //killable W
                {
                    var target = W.GetTarget();
                    if (target != null && target.Health < WDamage(target) - 50)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.High)
                            W.Cast(pred.UnitPosition);
                    }

                }

                if (!EWCasting)
                {
                    if (E.IsReady() && GetBool(combo,"usee"))
                    {
                        if (W.IsReady() && GetBool(combo, "usew"))
                        {
                            //e w combo
                            var target = W.GetTarget();
                            if (target != null)
                            {
                                var pred = W.GetPrediction(target);
                                if (pred.Hitchance >= HitChance.High)
                                {
                                    lastE = ObjectManager.Player.ServerPosition;
                                    E.Cast(ObjectManager.Player.ServerPosition.ToVector2() + (pred.CastPosition.ToVector2() - ObjectManager.Player.ServerPosition.ToVector2()).Normalized() * (E.Range - 200));
                                    DelayAction.Add(250, () => W.Cast(pred.UnitPosition));
                                    EWCasting = true;
                                }
                            }
                            return;
                        }
                        else
                        {
                            var target = E.GetTarget();
                            if (target != null)
                            {
                                E.Cast(target);
                                lastE = ObjectManager.Player.ServerPosition;
                            }
                        }
                    }
                }
                if (W.IsReady() && GetBool(combo, "usew") && !EWCasting)
                {
                    var target = W.GetTarget();
                    if (target != null)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.High)
                            W.Cast(pred.UnitPosition);
                    }
                }
            }
            var qTarget = Q.GetTarget();
            if (qTarget != null && GetBool(combo,"useq") && (!GetBool(main_menu, "onlyq5") || Q5x))
                Q.Cast(qTarget);

        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < GetSlider(harass, "manaperc"))
                return;

            if (GetBool(harass, "useq"))
            {
                var target = Q.GetTarget();
                if (target != null)
                    Q.Cast(target);
            }
        }

        private static void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < GetSlider(laneclear, "manaperc"))
                return;

            if (GetBool(laneclear, "useq") && Q.IsReady())
            {
                var farm = Q.GetCircularFarmLocation(ObjectManager.Get<Obj_AI_Minion>().Where(p => p.IsEnemy && p.DistanceToPlayer() < Q.Range).ToList());
                if (farm.MinionsHit >= GetSlider(laneclear, "minq"))
                    Q.Cast(farm.Position);
            }

            if (GetBool(laneclear, "useew") && W.IsReady() && E.IsReady())
            {
                var farm = W.GetCircularFarmLocation(ObjectManager.Get<Obj_AI_Minion>().Where(p => p.IsEnemy && p.DistanceToPlayer() < W.Range).ToList());
                if (farm.MinionsHit >= GetSlider(laneclear, "minew"))
                {
                    E.Cast(farm.Position);
                    lastE = ObjectManager.Player.ServerPosition;
                    if (W.Instance.Name == "TaliyahW")
                        W.Cast(farm.Position);
                    DelayAction.Add(250, () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, lastE, false));
                }
            }

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
                Combo();
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass)
                Harass();
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.LaneClear)
                LaneClear();

            if (W.Instance.Name == "TaliyahWNoClick")
                EWCasting = false;
        }

        private static void Events_OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs e)
        {
            if (GetBool(main_menu, "interrupt"))
            {
                if (e.Sender.DistanceToPlayer() < W.Range)
                    W.Cast(e.Sender.ServerPosition);
            }
        }

        private static void Events_OnGapCloser(object sender, Events.GapCloserEventArgs e)
        {
            if (GetBool(main_menu, "antigap"))
            {
                if (e.End.DistanceToPlayer() < E.Range)
                    E.Cast(e.Sender.ServerPosition);
            }
        }

        private static float WDamage(Obj_AI_Base target)
        {
            return (float)ObjectManager.Player.CalculateDamage(target, DamageType.Magical, new int[] { 60, 80, 100, 120, 140 }[W.Level] + ObjectManager.Player.TotalMagicalDamage * 0.4f);
        }
    }
}