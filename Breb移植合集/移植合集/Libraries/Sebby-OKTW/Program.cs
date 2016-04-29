using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby;
using OneKeyToWin_AIO_Sebby.Champions;
using OneKeyToWin_AIO_Sebby.Core;
using SharpDX;
using Color = System.Drawing.Color;
using HitChance = SebbyLib.Prediction.HitChance;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using PredictionOutput = SebbyLib.Prediction.PredictionOutput;
using Spell = LeagueSharp.Common.Spell;

namespace SebbyLib
{
    internal class Program
    {
        public static PredictionOutput DrawSpellPos;
        public static Spell Q, W, E, R, DrawSpell;
        public static float DrawSpellTime;
        public static int AIOmode;
        public static Menu Config;
        public static Obj_SpawnPoint enemySpawn;
        public static int HitChanceNum = 4, tickNum = 4, tickIndex;
        private static float dodgeTime = Game.Time;
        public static List<AIHeroClient> Enemies = new List<AIHeroClient>(), Allies = new List<AIHeroClient>();
        public static AIHeroClient jungler = ObjectManager.Player;

        public static bool Farm
        {
            get
            {
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                       Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                       Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
            }
        }

        public static bool Combo
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo); }
        }

        public static bool None
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None); }
        }

        public static bool LaneClear
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear); }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool IsJungler(AIHeroClient hero)
        {
            return hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite"));
        }

        public static bool getCheckBoxItem(string item)
        {
            return Config[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Config[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Config[item].Cast<KeyBind>().CurrentValue;
        }

        public static void GameOnOnGameLoad()
        {
            enemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R);

            Config = MainMenu.AddMenu("OneKeyToWin AIO", "OneKeyToWin_AIO" + ObjectManager.Player.ChampionName);

            #region MENU ABOUT OKTW

            Config.Add("debug", new CheckBox("Debug", false));
            Config.Add("debugChat", new CheckBox("Debug Chat", false));
            Config.Add("print", new CheckBox("OKTW NEWS in chat"));

            #endregion

            Config.Add("AIOmode", new Slider("AIO mode (0 : Util & Champ | 1 : Only Champ | 2 : Only Util)", 0, 0, 2));
            AIOmode = getSliderItem("AIOmode");

            if (AIOmode != 2)
            {
                if (Player.ChampionName != "MissFortune")
                {
                    new OktwTs().LoadOKTW();
                }
            }

            Config.Add("PredictionMODE", new Slider("Prediction MODE (0 : Common Pred | 1 : OKTW© PREDICTION)", 0, 0, 1));
            Config.Add("HitChance", new Slider("AIO mode (0 : Very High | 1 : High | 2 : Medium)", 0, 0, 2));
            Config.Add("debugPred", new CheckBox("Draw Aiming OKTW© PREDICTION", false));

            if (AIOmode != 2)
            {
                Config.Add("supportMode", new CheckBox("Support Mode", false));
                Config.Add("comboDisableMode", new CheckBox("Disable auto-attack in combo mode", false));
                Config.Add("manaDisable", new CheckBox("Disable mana manager in combo"));
                Config.Add("collAA", new CheckBox("Disable auto-attack if Yasuo wall collision"));

                #region LOAD CHAMPIONS

                switch (Player.ChampionName)
                {
                    case "Anivia":
                        PortAIO.Champion.Anivia.Program.LoadOKTW();
                        break;
                    case "Annie":
                        PortAIO.Champion.Annie.Program.LoadOKTW();
                        break;
                    case "Ashe":
                        PortAIO.Champion.Ashe.Program.LoadOKTW();
                        break;
                    case "Braum":
                        PortAIO.Champion.Braum.Program.LoadOKTW();
                        break;
                    case "Caitlyn":
                        PortAIO.Champion.Caitlyn.Program.LoadOKTW();
                        break;
                    case "Ekko":
                        PortAIO.Champion.Ekko.Program.LoadOKTW();
                        break;
                    case "Ezreal":
                        Ezreal.LoadOKTW();
                        break;
                    case "Graves":
                        Graves.LoadOKTW();
                        break;
                    case "Jayce":
                        Jayce.LoadOKTW();
                        break;
                    case "Jinx":
                        Jinx.LoadOKTW();
                        break;
                    case "Karthus":
                        Karthus.LoadOKTW();
                        break;
                    case "MissFortune":
                        MissFortune.LoadOKTW();
                        break;
                    case "Malzahar":
                        Malzahar.LoadOKTW();
                        break;
                    case "Orianna":
                        Orianna.LoadOKTW();
                        break;
                    case "Sivir":
                        Sivir.LoadOKTW();
                        break;
                    case "Twitch":
                        Twitch.LoadOKTW();
                        break;
                    case "Syndra":
                        Syndra.LoadOKTW();
                        break;
                    case "Velkoz":
                        Velkoz.LoadOKTW();
                        break;
                    case "Xerath":
                        Xerath.LoadOKTW();
                        break;
                    case "Swain":
                        Swain.LoadOKTW();
                        break;
                    case "Urgot":
                        Urgot.LoadOKTW();
                        break;
                    case "Ahri":
                        Ahri.LoadOKTW();
                        break;
                    case "Thresh":
                        Thresh.LoadOKTW();
                        break;
                }
            }

            #endregion

            foreach (var hero in HeroManager.Enemies)
            {
                if (hero.IsEnemy && hero.Team != Player.Team)
                {
                    Enemies.Add(hero);
                    if (IsJungler(hero))
                        jungler = hero;
                }
            }

            foreach (var hero in HeroManager.Allies)
            {
                if (hero.IsAlly && hero.Team == Player.Team)
                    Allies.Add(hero);
            }

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Drawing.OnDraw += OnDraw;
        }

        public static void debug(string msg)
        {
            if (getCheckBoxItem("debug"))
            {
                Console.WriteLine(msg);
            }
            if (getCheckBoxItem("debugChat"))
            {
                Chat.Print(msg);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            tickIndex++;

            if (tickIndex > 4)
                tickIndex = 0;

            if (!LagFree(0))
                return;
        }

        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            return false;
        }

        private static void OnDraw(EventArgs args)
        {
            if (AIOmode == 1)
                return;

            if (Game.Time - DrawSpellTime < 0.5 && getCheckBoxItem("debugPred") && getSliderItem("PredictionMODE") == 1)
            {
                if (DrawSpell.Type == SkillshotType.SkillshotLine)
                    OktwCommon.DrawLineRectangle(DrawSpellPos.CastPosition, Player.Position, (int)DrawSpell.Width, 1,
                        Color.DimGray);
                if (DrawSpell.Type == SkillshotType.SkillshotCircle)
                    Render.Circle.DrawCircle(DrawSpellPos.CastPosition, DrawSpell.Width, Color.DimGray, 1);

                drawText("Aiming " + DrawSpellPos.Hitchance,
                    Player.Position.Extend(DrawSpellPos.CastPosition, 400).To3D(), Color.Gray);
            }
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (AIOmode == 2)
                return;

            if (Combo && getCheckBoxItem("comboDisableMode"))
            {
                var t = (AIHeroClient)args.Target;
                if (6 * Player.GetAutoAttackDamage(t) < t.Health - OktwCommon.GetIncomingDamage(t) &&
                    !t.HasBuff("luxilluminatingfraulein") && !Player.HasBuff("sheen"))
                    args.Process = false;
            }

            if (!Player.IsMelee && OktwCommon.CollisionYasuo(Player.ServerPosition, args.Target.Position) &&
                getCheckBoxItem("collAA"))
            {
                args.Process = false;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && getCheckBoxItem("supportMode"))
            {
                if (args.Target.Type == GameObjectType.obj_AI_Minion) args.Process = false;
            }
        }

        public static void drawText(string msg, Vector3 Hero, Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length * 5, wts[1] + weight, color, msg);
        }

        public static void CastSpell(Spell QWER, Obj_AI_Base target)
        {
            if (getSliderItem("PredictionMODE") == 1)
            {
                var CoreType2 = Prediction.SkillshotType.SkillshotLine;
                var aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = Prediction.SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = Prediction.Prediction.GetPrediction(predInput2);

                if (QWER.Speed != float.MaxValue &&
                    OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if (getSliderItem("HitChance") == 0)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }
                }
                else if (getSliderItem("HitChance") == 1)
                {
                    if (poutput2.Hitchance >= HitChance.High)
                        QWER.Cast(poutput2.CastPosition);
                }
                else if (getSliderItem("HitChance") == 2)
                {
                    if (poutput2.Hitchance >= HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                }
                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;
                }
                DrawSpellPos = poutput2;
            }
            else if (getSliderItem("PredictionMODE") == 0)
            {
                if (getSliderItem("HitChance") == 0)
                {
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.VeryHigh);
                }
                else if (getSliderItem("HitChance") == 1)
                {
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                }
                else if (getSliderItem("HitChance") == 2)
                {
                    QWER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.Medium);
                }
            }
        }
    }
}