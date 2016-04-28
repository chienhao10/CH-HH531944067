#region License
/* Copyright (c) LeagueSharp 2016
 * No reproduction is allowed in any way unless given written consent
 * from the LeagueSharp staff.
 * 
 * Author: imsosharp
 * Date: 2/21/2016
 * File: Soraka.cs
 */
#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using Challenger_Series.Utils;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Challenger_Series
{
    public class Soraka : CSPlugin
    {

        public static LeagueSharp.Common.Spell Q, W, E, R;

        public Soraka()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 750);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 550);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 900);
            R = new LeagueSharp.Common.Spell(SpellSlot.R);

            Q.SetSkillshot(0.5f, 125, 1750, false, LeagueSharp.Common.SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1750, false, LeagueSharp.Common.SkillshotType.SkillshotCircle);

            InitializeMenu();

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            DelayedOnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += OnCreateObj;
            //Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += this.OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            this._rand = new Random();
        }

        private Random _rand;

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsEnemy)
            {
                var sdata = SpellDatabase.GetByName(args.SData.Name);
                if (sdata != null && args.End.Distance(ObjectManager.Player.ServerPosition) < 900 &&
                    sdata.SpellTags != null &&
                    sdata.SpellTags.Any(st => st == LeagueSharp.SDK.SpellTags.Dash || st == LeagueSharp.SDK.SpellTags.Blink || st == LeagueSharp.SDK.SpellTags.Interruptable))
                {
                    E.Cast(args.Start.Extend(args.End, sdata.Range - this._rand.Next(5, 50)));
                }
            }
        }

        private void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (args.Sender.Distance(ObjectManager.Player) < 800)
            {
                E.Cast(args.Sender);
            }
        }

        /*
        private void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            var ally = EntityManager.Heroes.Allies.FirstOrDefault(a => a.Distance(args.End) < 300 || args.Sender.Distance(a) < 300);
            if (ally.IsHPBarRendered && ally.Distance(ObjectManager.Player) < 800)
            {
                E.Cast(ally.ServerPosition.Randomize(-25, 25));
            }
        }
        */

        private void OnCreateObj(GameObject obj, EventArgs args)
        {
            if (obj.Name != "missile" && obj.IsEnemy && obj.Distance(ObjectManager.Player.ServerPosition) < 900)
            {
                //J4 wall E
                if (obj.Name.ToLower() == "jarvanivwall")
                {
                    var enemyJ4 = ValidTargets.First(h => h.CharData.BaseSkinName.Contains("Jarvan"));
                    if (enemyJ4 != null && enemyJ4.LSIsValidTarget())
                    E.Cast(enemyJ4.ServerPosition);
                }
                if (obj.Name.ToLower().Contains("soraka_base_e_rune.troy") && EntityManager.Heroes.Enemies.Count(e => e.IsHPBarRendered && e.Distance(obj.Position) < 300) > 0)
                {
                    Q.Cast(obj.Position);
                }
                if (EntityManager.Heroes.Allies.All(h => h.CharData.BaseSkinName != "Rengar"))
                {
                    if (obj.Name == "Rengar_LeapSound.troy")
                    {
                        E.Cast(obj.Position);
                    }
                    if (obj.Name == "Rengar_Base_P_Buf_Max.troy" || obj.Name == "Rengar_Base_P_Leap_Grass.troy")
                    {
                        E.Cast(ObjectManager.Player.ServerPosition);
                    }
                }
            }
        }

        #region Events

        public override void OnUpdate(EventArgs args)
        {
            base.OnUpdate(args);
            if (ObjectManager.Player.LSIsRecalling()) return;
            WLogic();
            RLogic();
            if (!getCheckBoxItem(MainMenu, "noneed4spacebar") && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) return;
            QLogic();
            ELogic();
            EAntiMelee();
            Orbwalker.DisableAttacking = getCheckBoxItem(MainMenu, "blockaas");
        }


        public override void OnDraw(EventArgs args)
        {
            base.OnDraw(args);
            if (getCheckBoxItem(MainMenu, "draww"))
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 550, W.IsReady() ? Color.Turquoise : Color.Red);
            if (getCheckBoxItem(MainMenu, "drawq"))
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 800, Q.IsReady() ? Color.DarkMagenta : Color.Red);
            if (getCheckBoxItem(MainMenu, "drawdebug"))
            {
                foreach (var healingCandidate in EntityManager.Heroes.Allies.Where(a => !a.IsMe && a.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 550 && !HealBlacklistMenu["dontheal" + a.CharData.BaseSkinName].Cast<CheckBox>().CurrentValue))
                {
                    if (healingCandidate != null)
                    {
                        var wtsPos = Drawing.WorldToScreen(healingCandidate.Position);
                        Drawing.DrawText(wtsPos.X, wtsPos.Y, Color.White,
                            "1W Heals " + Math.Round(GetWHealingAmount()) + "HP");
                    }
                }
            }
        }

        #endregion Events

        #region Menu

        private Menu PriorityMenu;
        private Menu HealBlacklistMenu;
        private Menu UltBlacklistMenu;

        public void EAntiMelee()
        {
            var victim =
                GameObjects.AllyHeroes.Where(a => a.Distance(ObjectManager.Player) < 900).FirstOrDefault(
                    a => GameObjects.EnemyHeroes.Any(e => e.IsMelee && e.IsHPBarRendered && e.Distance(a) < 200));
            if (victim != null)
            {
                E.Cast(victim.ServerPosition);
            }
        }

        public override void InitializeMenu()
        {
            HealBlacklistMenu = MainMenu.AddSubMenu("Do NOT Heal (W): ", "healblacklist");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                var championName = ally.CharData.BaseSkinName;
                HealBlacklistMenu.Add("dontheal" + championName, new CheckBox(championName, false));
            }

            UltBlacklistMenu = MainMenu.AddSubMenu("Do NOT Ult (R): ", "ultblacklist");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                var championName = ally.CharData.BaseSkinName;
                UltBlacklistMenu.Add("dontult" + championName, new CheckBox(championName, false));
            }

            PriorityMenu = MainMenu.AddSubMenu("Heal Priority", "sttcselector");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                PriorityMenu.Add("STTCSelector" + ally.ChampionName + "Priority", new Slider(ally.ChampionName, GetPriorityFromDb(ally.ChampionName), 1, 5));
            }

            MainMenu.Add("rakaqonlyifmyhp", new Slider("Only Q if my HP < %", 100, 0, 100));
            MainMenu.Add("noneed4spacebar", new CheckBox("PLAY ONLY WITH MOUSE! NO SPACEBAR", true));
            MainMenu.Add("wmyhp", new Slider("Don't Heal (W) if Below HP%: ", 20, 1));
            MainMenu.Add("dontwtanks", new CheckBox("Don't Heal (W) Tanks", true));
            MainMenu.Add("atanktakesxheals", new Slider("A TANK takes X Heals (W) to  FULLHP", 15, 5, 30));
            MainMenu.Add("ultmyhp", new Slider("Ult if MY HP% < ", 15, 1, 25));
            MainMenu.Add("ultallyhp", new Slider("Ult If Ally HP% < ", 15, 5, 35));
            MainMenu.Add("checkallysurvivability", new CheckBox("Check if ult will save ally", true));
            MainMenu.Add("ultafterignite", new CheckBox("ULT (R) after IGNITE", false));
            MainMenu.Add("blockaas", new CheckBox("Block AutoAttacks?", true));

            MainMenu.Add("draww", new CheckBox("Draw W?", true));
            MainMenu.Add("drawq", new CheckBox("Draw Q?", true));
            MainMenu.Add("drawdebug", new CheckBox("Draw Heal Info", false));
        }

        #endregion Menu

        #region ChampionData

        public double GetQHealingAmount()
        {
            var spellLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            if (spellLevel < 1) return 0;
            return Math.Min(
                new double[] {25, 35, 45, 55, 65}[spellLevel - 1] +
                0.4*ObjectManager.Player.FlatMagicDamageMod +
                (0.1*(ObjectManager.Player.MaxHealth - ObjectManager.Player.Health)),
                new double[] {50, 70, 90, 110, 130}[spellLevel - 1] +
                0.8*ObjectManager.Player.FlatMagicDamageMod);
        }

        public double GetWHealingAmount()
        {
            var spellLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
            if (spellLevel < 1) return 0;
            return new double[] {120, 150, 180, 210, 240}[spellLevel - 1] +
                   0.6*ObjectManager.Player.FlatMagicDamageMod;
        }

        public double GetRHealingAmount()
        {
            var spellLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;
            if (spellLevel < 1) return 0;
            return new double[] {120, 150, 180, 210, 240}[spellLevel - 1] +
                   0.6*ObjectManager.Player.FlatMagicDamageMod;
        }

        public int GetWManaCost()
        {
            var spellLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
            if (spellLevel < 1) return 0;
            return new[] {40, 45, 50, 55, 60}[spellLevel - 1];
        }

        public double GetWHealthCost()
        {
            return 0.10*ObjectManager.Player.MaxHealth;
        }

        #endregion ChampionData

        #region ChampionLogic

        public bool CanW()
        {
            return !ObjectManager.Player.InFountain() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level >= 1 &&
                   ObjectManager.Player.Health - GetWHealthCost() >
                   getSliderItem(MainMenu, "wmyhp") /100f*ObjectManager.Player.MaxHealth;
        }

        public void QLogic()
        {
            if (!Q.IsReady() || (ObjectManager.Player.Mana < 3*GetWManaCost() && CanW())) return;
            var shouldntKS =
                EntityManager.Heroes.Allies.Any(
                    h => h.Position.Distance(ObjectManager.Player.Position) < 600 && !h.IsDead && !h.IsMe);

            foreach (var hero in ValidTargets.Where(h => h.IsValidTarget(925)))
            {
                if (shouldntKS && Q.GetDamage(hero) > hero.Health)
                {
                    break;
                }
                var pred = Q.GetPrediction(hero);
                if ((int) pred.Hitchance > (int) HitChance.Medium && pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) < Q.Range)
                {
                    Q.Cast(hero);
                }
            }
        }

        public void WLogic()
        {
            if (!W.IsReady() || !CanW()) return;
            foreach (var ally in EntityManager.Heroes.Allies.Where(
                a =>
                    !a.IsMe && a.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 700 &&
                    a.MaxHealth - a.Health > GetWHealingAmount() && !a.LSIsRecalling())
                .OrderByDescending(GetPriority)
                .ThenBy(ally => ally.Health))
            {
                if (ally == null || ally.IsDead || ally.IsZombie) break;
                if (HealBlacklistMenu["dontheal" + ally.CharData.BaseSkinName] != null && HealBlacklistMenu["dontheal" + ally.CharData.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    break;
                }

                if (MainMenu["dontwtanks"] != null && getCheckBoxItem(MainMenu, "dontwtanks") && ally.Health > 500 && getSliderItem(MainMenu, "atanktakesxheals") * GetWHealingAmount() < ally.MaxHealth - ally.Health)
                {
                    break;
                }
                W.Cast(ally);
            }
        }

        public void ELogic()
        {
            if (!E.IsReady()) return;
            var goodTarget =
                ValidTargets.OrderByDescending(GetPriority).FirstOrDefault(
                    e =>
                        e.IsValidTarget(900) && e.HasBuffOfType(BuffType.Knockup) || e.HasBuffOfType(BuffType.Snare) ||
                        e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Suppression) || e.IsCharmed ||
                        e.IsCastingInterruptableSpell() || e.HasBuff("ChronoRevive") || e.HasBuff("ChronoShift"));
            if (goodTarget != null)
            {
                var pos = goodTarget.ServerPosition;
                if (pos.Distance(ObjectManager.Player.ServerPosition) < 900)
                {
                    E.Cast(goodTarget.ServerPosition);
                }
            }
            foreach (
                var enemyMinion in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            m =>
                                m.IsEnemy && m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < E.Range &&
                                m.HasBuff("teleport_target")))
            {
                DelayAction.Add(3250, () =>
                {
                    if (enemyMinion != null && enemyMinion.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 900)
                    {
                        E.Cast(enemyMinion.ServerPosition);
                    }
                });
            }
        }

        public void RLogic()
        {
            if (!R.IsReady()) return;
            if (ObjectManager.Player.CountEnemyHeroesInRange(900) >= 1 && ObjectManager.Player.Health > 1 &&
                ObjectManager.Player.HealthPercent <= getSliderItem(MainMenu, "ultmyhp"))
            {
                R.Cast();
            }
            var minAllyHealth = getSliderItem(MainMenu, "ultallyhp");
            if (minAllyHealth <= 1) return;
            foreach (var ally in EntityManager.Heroes.Allies.Where(h => !h.IsMe && h.Health > 50))
            {
                if (HealBlacklistMenu["dontheal" + ally.CharData.BaseSkinName].Cast<CheckBox>().CurrentValue) break;
                if (getCheckBoxItem(MainMenu, "ultafterignite") && ally.HasBuff("summonerdot") && ally.Health > 400) break;
                if (getCheckBoxItem(MainMenu, "checkallysurvivability") && ally.CountAllyHeroesInRange(800) == 0 &&
                    ally.CountEnemyHeroesInRange(800) > 2) break;
                if (ally.CountEnemyHeroesInRange(800) >= 1 && ally.HealthPercent > 2 &&
                    ally.HealthPercent <= minAllyHealth && !ally.IsZombie && !ally.IsDead)
                {
                    R.Cast();
                }
            }
        }

        #endregion ChampionLogic

        #region STTCSelector        

        public float GetPriority(AIHeroClient hero)
        {
            var p = 1;
            if (PriorityMenu["STTCSelector" + hero.ChampionName + "Priority"] != null)
            {
                p = PriorityMenu["STTCSelector" + hero.ChampionName + "Priority"].Cast<Slider>().CurrentValue;
            }
            else
            {
                p = GetPriorityFromDb(hero.ChampionName);
            }

            switch (p)
            {
                case 2:
                    return 1.5f;
                case 3:
                    return 1.75f;
                case 4:
                    return 2f;
                case 5:
                    return 2.5f;
                default:
                    return 1f;
            }
        }

        private static int GetPriorityFromDb(string championName)
        {
            string[] p1 =
            {
                "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Taric", "TahmKench", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

            string[] p2 =
            {
                "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", "Gragas", "Irelia", "Jax",
                "Lee Sin", "Maokai", "Morgana", "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze", "Swain",
                "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao", "RekSai"
            };

            string[] p3 =
            {
                "Akali", "Diana", "Ekko", "Fiddlesticks", "Fiora", "Fizz", "Heimerdinger", "Jayce", "Kassadin",
                "Kayle", "Kha'Zix", "Lissandra", "Mordekaiser", "Nidalee", "Riven", "Shaco", "Vladimir", "Yasuo",
                "Zilean"
            };

            string[] p4 =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Kindred",
                "Leblanc", "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra",
                "Talon", "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "Velkoz", "Viktor",
                "Xerath", "Zed", "Ziggs", "Jhin", "Soraka"
            };

            if (p1.Contains(championName))
            {
                return 1;
            }
            if (p2.Contains(championName))
            {
                return 2;
            }
            if (p3.Contains(championName))
            {
                return 3;
            }
            return p4.Contains(championName) ? 4 : 1;
        }

        #endregion STTCSelector

    }
}