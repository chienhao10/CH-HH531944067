using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Damage = LeagueSharp.Common.Damage;
using Environment = UnderratedAIO.Helpers.Environment;
using Spell = LeagueSharp.Common.Spell;

namespace UnderratedAIO.Champions
{
    internal class Nocturne
    {
        public static Menu config;
        public static Spell P, Q, W, E, R;
        public static int[] rRanges = {2500, 3250, 4000};
        private static float lastR;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static IncomingDamage IncDamages;

        public static Menu menuD, menuC, menuH, menuLC;

        public Nocturne()
        {
            IncDamages = new IncomingDamage();
            InitNocturne();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private static bool qTrailOnMe
        {
            get { return player.Buffs.Any(buff => buff.Name == "nocturneduskbringerhaste"); }
        }

        private static bool uBlades
        {
            get { return player.Buffs.Any(buff => buff.Name == "nocturneumbrablades"); }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass(target);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(GetTargetRange(), DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var data = IncDamages.GetAllyData(player.NetworkId);
            if (getCheckBoxItem(menuC, "usew") && W.IsReady() && data.AnyCC)
            {
                W.Cast();
            }
            var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var cmbdmg = ComboDamage(target);
            var dist = player.Distance(target);
            if (lastR > 4000f)
            {
                lastR = 0f;
            }
            if (getCheckBoxItem(menuC, "useq") && Q.CanCast(target) &&
                dist < getSliderItem(menuC, "useqMaxRange") && !player.IsDashing())
            {
                Q.CastIfHitchanceEquals(target, dist < 550 ? HitChance.Medium : HitChance.High,
                    getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuC, "usee") && E.CanCast(eTarget) &&
                dist < getSliderItem(menuC, "useeMaxRange"))
            {
                E.Cast(eTarget, getCheckBoxItem(config, "packets"));
            }
            if (getCheckBoxItem(menuC, "user") && R.IsReady())
            {
                R.Range = rRanges[R.Level - 1];
            }
            if (getCheckBoxItem(menuC, "user") && lastR.Equals(0) && !target.UnderTurret(true) &&
                R.CanCast(target) &&
                ((qTrailOnMe && eBuff(target) && target.MoveSpeed > player.MoveSpeed && dist > 360 &&
                  target.HealthPercent < 60) ||
                 (dist < rRanges[R.Level - 1] && dist > 900 &&
                  target.CountAlliesInRange(2000) >= target.CountEnemiesInRange(2000) &&
                  cmbdmg + Environment.Hero.GetAdOverTime(player, target, 5) > target.Health &&
                  (target.Health > Q.GetDamage(target) || !Q.IsReady())) ||
                 (player.HealthPercent < 40 && target.HealthPercent < 40 && target.CountAlliesInRange(1000) == 1 &&
                  target.CountEnemiesInRange(1000) == 1)))
            {
                R.Cast(target, getCheckBoxItem(config, "packets"));
                lastR = System.Environment.TickCount;
            }
            if (getCheckBoxItem(menuC, "user") && !lastR.Equals(0) && R.CanCast(target) &&
                (cmbdmg*1.6 + Environment.Hero.GetAdOverTime(player, target, 5) > target.Health ||
                 R.GetDamage(target) > target.Health ||
                 (qTrailOnMe && eBuff(target) && target.MoveSpeed > player.MoveSpeed && dist > 360 &&
                  target.HealthPercent < 60)))
            {
                var time = System.Environment.TickCount - lastR;
                if (time > 3500f || player.Distance(target) > E.Range || cmbdmg > target.Health ||
                    (player.HealthPercent < 40 && target.HealthPercent < 40))
                {
                    R.Cast(target, getCheckBoxItem(config, "packets"));
                    lastR = 0f;
                }
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private float GetTargetRange()
        {
            if (R.IsReady())
            {
                return rRanges[R.Level - 1];
            }
            return Q.Range;
        }

        private void Clear()
        {
            if (Environment.Minion.KillableMinion(player.AttackRange))
            {
                return;
            }
            var perc = getSliderItem(menuLC, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var bestPositionQ =
                Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (getCheckBoxItem(menuLC, "useqLC") && Q.IsReady() &&
                bestPositionQ.MinionsHit >= getSliderItem(menuLC, "qhitLC"))
            {
                Q.Cast(bestPositionQ.Position, getCheckBoxItem(config, "packets"));
            }
        }

        private void Harass(AIHeroClient target)
        {
            if (Environment.Minion.KillableMinion(player.AttackRange))
            {
                return;
            }
            var perc = getSliderItem(menuH, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuH, "useqH") && Q.CanCast(target))
            {
                Q.Cast(target, getCheckBoxItem(config, "packets"));
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady() && Q.Instance.SData.Mana < player.Mana)
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (E.IsReady() && E.Instance.SData.Mana < player.Mana)
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.R);
            }
            //damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Base))
            {
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawrr"), R.Level >= 1 ? rRanges[R.Level - 1] : rRanges[0],
                Color.FromArgb(180, 100, 146, 166));

            if (!getCheckBoxItem(menuD, "bestpospas"))
            {
                return;
            }

            var bestPositionP =
                P.GetCircularFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (bestPositionP.Position.IsValid() && bestPositionP.MinionsHit > 2 && uBlades)
            {
                Drawing.DrawCircle(bestPositionP.Position.To3D(), 150f, Color.Crimson);
            }
        }

        private void InitNocturne()
        {
            P = new Spell(SpellSlot.Q, 1000);
            P.SetSkillshot(3000, Orbwalking.GetRealAutoAttackRange(player) + 50, 3000, false,
                SkillshotType.SkillshotCircle);
            Q = new Spell(SpellSlot.Q, 1150);
            Q.SetSkillshot(0.25f, 60f, 1350, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, rRanges[0]);
        }

        private static bool eBuff(AIHeroClient target)
        {
            return target.Buffs.Any(buff => buff.Name == "NocturneUnspeakableHorror");
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

        private void InitMenu()
        {
            config = MainMenu.AddMenu("Nocturne", "Nocturne");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawrr", new CheckBox("Draw R range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("bestpospas", new CheckBox("Best position for passive", false));
            menuD.Add("drawcombo", new CheckBox("Draw combo damage"));

            // Combo
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("useqMaxRange", new Slider("Q max Range", 1000, 0, (int) Q.Range));
            menuC.Add("usew", new CheckBox("Use W against targeted CC"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("useeMaxRange", new Slider("E max Range", 300, 0, (int) E.Range));
            menuC.Add("user", new CheckBox("Use R in close range"));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q"));
            menuH.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));
            menuLC.Add("qhitLC", new Slider("Min hit", 2, 1, 10));
            menuLC.Add("minmana", new Slider("Keep X% mana", 1, 1));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}