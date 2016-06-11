using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Remoting.Messaging;
using Color = System.Drawing.Color;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using EloBuddy.SDK.Menu;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using Damage = LeagueSharp.Common.Damage;
using EloBuddy.SDK.Menu.Values;


namespace UnderratedAIO.Champions
{
    internal class Renekton
    {
        public static Menu config;
        private static Menu comboMenu, drawingsMenu, laneMenu, harassMenu;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        private static float lastE;
        private static Vector3 lastEpos;
        private static Bool wChancel = false;
        public static IncomingDamage IncDamages;


        public static void OnLoad()
        {
            IncDamages = new IncomingDamage();
            InitRenekton();
            InitMenu();
            Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Renekton</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPreAttack += beforeAttack;
            Orbwalker.OnPostAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Game_OnDraw;
            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                Console.WriteLine(args.SData.Name);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (System.Environment.TickCount - lastE > 4100)
            {
                lastE = 0;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }
        }

        public static void CastHydra()
        {
            if (Items.CanUseItem(3074))
            {
                Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077))
            {
                Items.UseItem(3077);
            }
        }

        private static void afterAttack(AttackableUnit target, EventArgs args)
        {
            if (target is AIHeroClient &&
                ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                  checkFuryMode(SpellSlot.W, (Obj_AI_Base)target)) ||
                 Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)))
            {
                var time = Game.Time - W.Instance.CooldownExpires;
                if (getCheckBoxItem(config, "hyd") && time < -9 || (!W.IsReady() && time < -1))
                {
                    CastHydra();
                }
            }
            if (target is AIHeroClient && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                getCheckBoxItem(comboMenu, "usew") && checkFuryMode(SpellSlot.W, (Obj_AI_Base)target))
            {
                W.Cast();
            }
            if (target is AIHeroClient && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                getBoxItem(harassMenu, "useCH") == 0)
            {
                if (W.IsReady())
                {
                    W.Cast();
                    Orbwalker.ResetAutoAttack();
                    return;
                }
                if (Q.IsReady())
                {
                    Q.Cast();
                    return;
                }
                if (E.CanCast((Obj_AI_Base)target))
                {
                    E.Cast(target.Position);
                    return;
                }
            }
        }

        private static void beforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (W.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                args.Target is AIHeroClient && checkFuryMode(SpellSlot.W, (Obj_AI_Base)args.Target) &&
                getCheckBoxItem(comboMenu, "usew"))
            {
                if ((player.Mana > 40 && !fury) || (Q.IsReady() && canBeOpWIthQ(player.Position)))
                {
                    return;
                }

                W.Cast();
                return;
            }
            if (W.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                getCheckBoxItem(harassMenu, "usewH") && args.Target is AIHeroClient &&
                getBoxItem(harassMenu, "useCH") != 0)
            {
                W.Cast();
            }
        }

        private static bool rene
        {
            get { return player.Buffs.Any(buff => buff.Name == "renektonsliceanddicedelay"); }
        }

        private static bool fury
        {
            get { return player.Buffs.Any(buff => buff.Name == "renektonrageready"); }
        }

        private static bool renw
        {
            get { return player.Buffs.Any(buff => buff.Name == "renektonpreexecute"); }
        }

        private static void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range * 2, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var FuryQ = Damage.LSGetSpellDamage(player, target, SpellSlot.Q) * 0.5;
            var FuryW = Damage.LSGetSpellDamage(player, target, SpellSlot.W) * 0.5;
            var eDmg = Damage.LSGetSpellDamage(player, target, SpellSlot.E);
            var combodamage = ComboDamage(target);
            if (getCheckBoxItem(comboMenu, "useIgnite") && hasIgnite &&
                player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (player.LSDistance(target) > E.Range && E.IsReady() && (W.IsReady() || Q.IsReady()) && lastE.Equals(0) &&
                getCheckBoxItem(comboMenu, "usee"))
            {
                var closeGapTarget =
                    MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(i => i.LSDistance(target.ServerPosition) < Q.Range - 40)
                        .OrderByDescending(i => Environment.Minion.countMinionsInrange(i.Position, Q.Range))
                        .FirstOrDefault();
                if (closeGapTarget != null)
                {
                    if ((canBeOpWIthQ(closeGapTarget.Position) || fury) && !rene)
                    {
                        if (E.CanCast(closeGapTarget))
                        {
                            E.Cast(closeGapTarget.Position);
                            lastE = System.Environment.TickCount;
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(comboMenu, "useq") && Q.CanCast(target) && !renw && !player.LSIsDashing() &&
                checkFuryMode(SpellSlot.Q, target) &&
                (!W.IsReady() ||
                 ((W.IsReady() && !fury) || (player.Health < target.Health) ||
                  (Environment.Minion.countMinionsInrange(player.Position, Q.Range) +
                   player.CountEnemiesInRange(Q.Range) > 3 && fury))))
            {
                Q.Cast();
            }
            var distance = player.LSDistance(target.Position);
            if (getCheckBoxItem(comboMenu, "usee") && lastE.Equals(0) && E.CanCast(target) &&
                (eDmg > target.Health ||
                 (((W.IsReady() && canBeOpWIthQ(target.Position) && !rene) ||
                   (distance > target.LSDistance(player.Position.LSExtend(target.Position, E.Range)) - distance)))))
            {
                E.Cast(target.Position);
                lastE = System.Environment.TickCount;
                return;
            }
            if (getCheckBoxItem(comboMenu, "usee") && checkFuryMode(SpellSlot.E, target) && !lastE.Equals(0) &&
                (eDmg + player.GetAutoAttackDamage(target) > target.Health ||
                 (((W.IsReady() && canBeOpWIthQ(target.Position) && !rene) ||
                   (distance < target.Distance(player.Position.LSExtend(target.Position, E.Range)) - distance) ||
                   player.LSDistance(target) > E.Range - 100))))
            {
                var time = System.Environment.TickCount - lastE;
                if (time > 3600f || combodamage > target.Health || (player.LSDistance(target) > E.Range - 100))
                {
                    E.Cast(target.Position);
                    lastE = 0;
                }
            }
            var data = IncDamages.GetAllyData(player.NetworkId);
            if (((player.Health * 100 / player.MaxHealth) <= getSliderItem(comboMenu, "user") &&
                 data.DamageTaken > 30) ||
                getSliderItem(comboMenu, "userindanger") < player.CountEnemiesInRange(R.Range))
            {
                R.Cast();
            }
        }

        private static bool canBeOpWIthQ(Vector3 vector3)
        {
            if (fury)
            {
                return false;
            }
            if ((player.Mana > 45 && !fury) ||
                (Q.IsReady() &&
                 player.Mana + Environment.Minion.countMinionsInrange(vector3, Q.Range) * 2.5 +
                 player.LSCountEnemiesInRange(Q.Range) * 10 > 50))
            {
                return true;
            }
            return false;
        }

        private static bool canBeOpwithW()
        {
            if (player.Mana + 20 > 50)
            {
                return true;
            }
            return false;
        }

        private static void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            switch (getBoxItem(harassMenu, "useCH"))
            {
                case 1:
                    if (Q.IsReady() && E.IsReady() && lastE.Equals(0) && fury && !rene)
                    {
                        if (getCheckBoxItem(harassMenu, "donteqwebtower") &&
                            player.Position.LSExtend(target.Position, E.Range).UnderTurret(true))
                        {
                            return;
                        }
                        var closeGapTarget =
                            MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                .Where(i => i.LSDistance(target.ServerPosition) < Q.Range - 40)
                                .OrderByDescending(i => Environment.Minion.countMinionsInrange(i.Position, Q.Range))
                                .FirstOrDefault();
                        if (closeGapTarget != null)
                        {
                            lastEpos = player.ServerPosition;
                            Utility.DelayAction.Add(4100, () => lastEpos = new Vector3());
                            E.Cast(closeGapTarget.Position);
                            lastE = System.Environment.TickCount;
                            return;
                        }
                        else
                        {
                            lastEpos = player.ServerPosition;
                            Utility.DelayAction.Add(4100, () => lastEpos = new Vector3());
                            E.Cast(target.Position);
                            lastE = System.Environment.TickCount;
                            return;
                        }
                    }
                    if (player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target) && Q.IsReady() &&
                        E.IsReady() && E.IsReady())
                    {
                        Orbwalker.ForcedTarget = target;
                    }
                    return;
                    break;
                case 0:
                    if (Q.IsReady() && W.IsReady() && !rene && E.CanCast(target))
                    {
                        if (getCheckBoxItem(harassMenu, "donteqwebtower") &&
                            player.Position.LSExtend(target.Position, E.Range).UnderTurret(true))
                        {
                            return;
                        }
                        if (E.CastIfHitchanceEquals(target, HitChance.High))
                        {
                            lastE = System.Environment.TickCount;
                        }
                    }
                    if (rene && E.CanCast(target) && !lastE.Equals(0) && System.Environment.TickCount - lastE > 3600)
                    {
                        E.CastIfHitchanceEquals(target, HitChance.High);
                    }
                    if (player.LSDistance(target) < Orbwalking.GetRealAutoAttackRange(target) && Q.IsReady() &&
                        E.IsReady() && E.IsReady())
                    {
                        Orbwalker.ForcedTarget = target;
                    }
                    return;
                    break;
                default:
                    break;
            }

            if (getCheckBoxItem(harassMenu, "useqH") && Q.CanCast(target))
            {
                Q.Cast();
            }

            if (getBoxItem(harassMenu, "useCH") == 0 && !lastE.Equals(0) && rene &&
                !Q.IsReady() && !renw)
            {
                if (lastEpos.IsValid())
                {
                    E.Cast(player.Position.LSExtend(lastEpos, 350f));
                }
            }
        }

        private static void Clear()
        {
            if (getCheckBoxItem(laneMenu, "useqLC") && Q.IsReady() && !player.LSIsDashing())
            {
                if (Environment.Minion.countMinionsInrange(player.Position, Q.Range) >=
                    getSliderItem(laneMenu, "minimumMini"))
                {
                    Q.Cast();
                    return;
                }
            }
            if (getCheckBoxItem(laneMenu, "useeLC") && E.IsReady())
            {
                var minionsForE = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPosition = E.GetLineFarmLocation(minionsForE);
                if (bestPosition.Position.IsValid() &&
                    !player.Position.LSExtend(bestPosition.Position.To3D(), E.Range).UnderTurret(true) &&
                    !bestPosition.Position.LSIsWall())
                {
                    if (bestPosition.MinionsHit >= 2)
                    {
                        E.Cast(bestPosition.Position);
                    }
                }
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingsMenu, "drawqq"))
            {
                Render.Circle.DrawCircle(player.Position, Q.Range, Color.FromArgb(180, 58, 100, 150));
            }

            if (getCheckBoxItem(drawingsMenu, "drawee"))
            {
                Render.Circle.DrawCircle(player.Position, E.Range, Color.FromArgb(180, 58, 100, 150));
            }

            Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawingsMenu, "drawcombo");
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                if (getCheckBoxItem(drawingsMenu, "rDamage"))
                {
                    damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.R) * 15;
                }
            }


            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float)damage;
        }

        public static bool checkFuryMode(SpellSlot spellSlot, Obj_AI_Base target)
        {
            if (Damage.LSGetSpellDamage(player, target, spellSlot) > target.Health)
            {
                return true;
            }
            if (canBeOpWIthQ(player.Position))
            {
                return false;
            }
            if (!fury)
            {
                return true;
            }
            if (ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            switch (getBoxItem(comboMenu, "furyMode"))
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (spellSlot != SpellSlot.Q && Q.IsReady())
                    {
                        return false;
                    }
                    break;
                case 2:
                    if (spellSlot != SpellSlot.W && (W.IsReady() || renw))
                    {
                        return false;
                    }
                    break;
                case 3:
                    if (spellSlot != SpellSlot.E && rene)
                    {
                        return false;
                    }
                    break;
                default:
                    return true;
                    break;
            }
            return false;
        }

        private static void InitRenekton()
        {
            Q = new Spell(SpellSlot.Q, 300);
            W = new Spell(SpellSlot.W, player.AttackRange + 55);
            E = new Spell(SpellSlot.E, 450);
            E.SetSkillshot(
                E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false, SkillshotType.SkillshotCone);
            R = new Spell(SpellSlot.R, 300);
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

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Renekton", "Renekton");


            drawingsMenu = config.AddSubMenu("Drawings", "dsettings");
            drawingsMenu.Add("drawqq", new CheckBox("Draw Q Range"));
            drawingsMenu.Add("drawee", new CheckBox("Draw E Range"));
            drawingsMenu.Add("drawrr", new CheckBox("Draw R Range"));
            drawingsMenu.Add("drawcombo", new CheckBox("Draw combo damage"));


            comboMenu = config.AddSubMenu("Combo", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q", true));
            comboMenu.Add("usew", new CheckBox("Use W", true));
            comboMenu.Add("usee", new CheckBox("Use E", true));
            comboMenu.Add("user", new Slider("Use R under", 20, 0, 100));
            comboMenu.Add("userindanger", new Slider("Use R above X enemy", 2, 1, 5));
            comboMenu.Add("furyMode", new ComboBox("Fury priority", 0, "No priority", "Q", "W", "E"));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite", true));


            harassMenu = config.AddSubMenu("Harass", "hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q", true));
            harassMenu.Add("usewH", new CheckBox("Use W", true));
            harassMenu.Add("useCH", new ComboBox("Harass mode", 1, "Use harass combo", "E-furyQ-Eback if possible", "Basic"));
            harassMenu.Add("donteqwebtower", new CheckBox("Don't dash under tower", true));


            laneMenu = config.AddSubMenu("LaneClear", "lcsettings");
            laneMenu.Add("useqLC", new CheckBox("Use Q", true));
            laneMenu.Add("minimumMini", new Slider("Use Q min minion", 2, 1, 6));
            laneMenu.Add("usewLC", new CheckBox("Use W", true));
            laneMenu.Add("useeLC", new CheckBox("Use E", true));

        }
    }
}