using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

using CommonCollision = LeagueSharp.Common.Collision;

using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using SPrediction;
using EloBuddy.SDK.Menu.Values;

namespace WreckingBall
{
    internal class WreckingBall
    {

        private enum PriorityMode
        {
            Wardjump = 0,
            Flash = 1,
        }

        private const string ChampName = "LeeSin";

        private const string Version = "1.8.3";

        private static AIHeroClient leeHero;

        private static AIHeroClient rTarget;

        private static AIHeroClient rTargetSecond;

        private static Obj_AI_Minion mostRecentWard;

        private static LeagueSharp.Common.Spell spellQ, spellQ2, spellW, spellW2, spellE, spellE2, spellR;

        private static SpellSlot igniteSlot, flashSlot;

        private static Menu wbMenu;

        private static readonly string[] QSpellNames = { string.Empty, "BlindMonkQOne", "BlindMonkQTwo" };

        private static readonly string[] WSpellNames = { string.Empty, "BlindMonkWOne", "BlindMonkWTwo" };

        private static readonly string[] ESpellNames = { string.Empty, "BlindMonkEOne", "BlindMonkETwo" };

        private static int lastWjTick = 0;

        private static int lastWjTickMenu = 0;

        private static int lastFlashTickMenu = 0;

        private static int lastSwitchT;

        private static int distTargetKickPos;

        private static int distLeeKickPos;

        private static int distLeeToWardjump;

        private static bool bubbaKushing;

        private static int mainMode;

        private static PriorityMode bubbaPriorityMode;


        private static readonly Items.Item[] WjItems =
            {
                new Items.Item(1408, 600), //Enchanted - Warrior
                new Items.Item(1409, 600), //Enchanted - Cinderhulk 
                new Items.Item(1410, 600), //Enchanted - Runic Echoes  
                new Items.Item(1411, 600), //Enchanted - Devourer
                new Items.Item(2301, 600), //EOT Watchers
                new Items.Item(2302, 600), //EOT Oasis
                new Items.Item(2303, 600), //EOT Equinox
                new Items.Item((int)ItemId.Trackers_Knife, 600),
                new Items.Item((int)ItemId.Trackers_Knife_Enchantment_Cinderhulk, 600),
                new Items.Item((int)ItemId.Trackers_Knife_Enchantment_Devourer, 600),
                new Items.Item((int)ItemId.Trackers_Knife_Enchantment_Runic_Echoes, 600),
                new Items.Item((int)ItemId.Trackers_Knife_Enchantment_Sated_Devourer, 600),
                new Items.Item((int)ItemId.Trackers_Knife_Enchantment_Warrior, 600),
                new Items.Item((int)ItemId.Ruby_Sightstone, 600),
                new Items.Item((int)ItemId.Sightstone, 600),
                new Items.Item((int)ItemId.Warding_Totem_Trinket, 600),
                new Items.Item((int)ItemId.Vision_Ward, 600),
            };

        public static void WreckingBallLoad()
        {
            leeHero = ObjectManager.Player;

            if (leeHero.ChampionName != ChampName)
            {
                return;
            }

            ChampInfo.InitSpells();

            LoadMenu();

            SPrediction.Prediction.Initialize(wbMenu, "SPred Settings");

            bubbaPriorityMode = (PriorityMode)getBoxItem(mainMenu, "modePrio");

            spellQ = new LeagueSharp.Common.Spell(SpellSlot.Q, ChampInfo.Q.Range);
            spellQ2 = new LeagueSharp.Common.Spell(SpellSlot.Q, ChampInfo.Q2.Range);
            spellW = new LeagueSharp.Common.Spell(SpellSlot.W, ChampInfo.W.Range);
            spellW2 = new LeagueSharp.Common.Spell(SpellSlot.W, ChampInfo.W2.Range);
            spellE = new LeagueSharp.Common.Spell(SpellSlot.E, ChampInfo.E.Range);
            spellE2 = new LeagueSharp.Common.Spell(SpellSlot.E, ChampInfo.E2.Range);
            spellR = new LeagueSharp.Common.Spell(SpellSlot.R, ChampInfo.R.Range);

            spellQ.SetSkillshot(
                ChampInfo.Q.Delay,
                ChampInfo.Q.Width,
                ChampInfo.Q.Speed,
                true,
                SkillshotType.SkillshotLine);

            spellE.SetSkillshot(
                ChampInfo.E.Delay,
                ChampInfo.E.Width,
                ChampInfo.E.Speed,
                false,
                SkillshotType.SkillshotCircle);

            igniteSlot = leeHero.GetSpellSlot("summonerdot");
            flashSlot = leeHero.GetSpellSlot("summonerflash");

            Game.OnUpdate += WreckingBallOnUpdate;
            Drawing.OnDraw += WreckingBallOnDraw;
            GameObject.OnCreate += WreckingBallOnCreate;
        }

        private static void WreckingBallOnUpdate(EventArgs args)
        {
            //Select most HP target

            List<AIHeroClient> inRangeHeroes =
                HeroManager.Enemies.Where(
                    x =>
                    x.IsValid && !x.IsDead && x.IsVisible
                    && x.Distance(leeHero.ServerPosition) < getSliderItem(mainMenu, "firstTargetRange")).ToList();

            rTarget = inRangeHeroes.Any() ? (mainMode == 0 ? ReturnMostHp(inRangeHeroes) : ReturnClosest(inRangeHeroes)) : null;

            //Select less HP target
            if (rTarget != null)
            {
                List<AIHeroClient> secondTargets =
                    HeroManager.Enemies.Where(
                        x =>
                        x.IsValid && !x.IsDead && x.IsVisible && x.NetworkId != rTarget.NetworkId
                        && x.Distance(rTarget.ServerPosition)
                        < getSliderItem(mainMenu, "secondTargetRange")).ToList();

                rTargetSecond = secondTargets.Any() ? ReturnLessHp(secondTargets) : null;
            }
            else
            {
                rTargetSecond = null;
            }

            if (rTarget != null && rTargetSecond != null && getKeyBindItem(mainMenu, "bubbaKey"))
            {
                if (!bubbaKushing)
                {
                    bubbaKushing = true;
                }

                BubbaKush();
            }
            else
            {
                if (bubbaKushing)
                {
                    bubbaKushing = false;
                }
            }

            switch (getBoxItem(mainMenu, "modePrio"))
            {
                case 0:
                    if (bubbaPriorityMode == PriorityMode.Flash)
                    {
                        bubbaPriorityMode = PriorityMode.Wardjump;
                    }
                    break;
                case 1:
                    if (bubbaPriorityMode == PriorityMode.Wardjump)
                    {
                        bubbaPriorityMode = PriorityMode.Flash;
                    }
                    break;
            }

            distTargetKickPos = getSliderItem(extraMenu, "distanceToKick");

            distLeeKickPos = getSliderItem(extraMenu, "distanceLeeKick");

            distLeeToWardjump = getSliderItem(extraMenu, "distanceToWardjump");

            mainMode = getBoxItem(mainMenu, "mainMode");


            if (getKeyBindItem(mainMenu, "switchKey") && Environment.TickCount > lastSwitchT + 450)
            {
                switch (getBoxItem(mainMenu, "mainMode"))
                {
                    case 0:
                        mainMenu["mainMode"].Cast<ComboBox>().CurrentValue = 1;
                        lastSwitchT = Environment.TickCount;
                        break;
                    case 1:
                        mainMenu["mainMode"].Cast<ComboBox>().CurrentValue = 0;
                        lastSwitchT = Environment.TickCount;
                        break;
                }
            }

            if (extraMenu["valuesToDefault"].Cast<CheckBox>().CurrentValue)
            {
                extraMenu["distanceToKick"].Cast<Slider>().CurrentValue = 150;
                extraMenu["distanceLeeKick"].Cast<Slider>().CurrentValue = 115;
                extraMenu["distanceToWardjump"].Cast<Slider>().CurrentValue = 485;
                extraMenu["valuesToDefault"].Cast<CheckBox>().CurrentValue = false;
            }

            /*if (wbMenu.Item("debug3").GetValue<KeyBind>().Active && CanWardJump())
            {
                WardJumpTo(Game.CursorPos);
            }*/
        }

        private static AIHeroClient ReturnMostHp(List<AIHeroClient> heroList)
        {
            AIHeroClient mostHp = null;

            foreach (var hero in heroList)
            {
                if (mostHp == null)
                {
                    mostHp = hero;
                }

                if (mostHp.MaxHealth < hero.MaxHealth)
                {
                    mostHp = hero;
                }
            }

            return mostHp;
        }

        private static AIHeroClient ReturnClosest(List<AIHeroClient> herolist)
        {
            AIHeroClient closest = null;

            foreach (var hero in herolist)
            {
                if (closest == null)
                {
                    closest = hero;
                }

                if (closest.Distance(leeHero) > hero.Distance(leeHero))
                {
                    closest = hero;
                }
            }

            return closest;
        }

        private static AIHeroClient ReturnLessHp(List<AIHeroClient> heroList)
        {
            AIHeroClient lessHp = null;

            foreach (var hero in heroList)
            {
                if (lessHp == null)
                {
                    lessHp = hero;
                }

                if (lessHp.Health > hero.Health)
                {
                    lessHp = hero;
                }
            }

            return lessHp;
        }

        private static void BubbaKush()
        {
            Console.WriteLine("1");
            if (spellR.Instance.State == SpellState.NotLearned || !spellR.Instance.IsReady())
            {
                Console.WriteLine("2");
                return;
            }

            Console.WriteLine("3");
            var flashVector = GetFlashVector();
            Console.WriteLine("4");
            var gpUnit = GetClosestDirectEnemyUnitToPos(flashVector);
            Console.WriteLine("5");
            var doFlash = getCheckBoxItem(mainMenu, "useFlash");
            Console.WriteLine("6");
            var doWardjump = getCheckBoxItem(mainMenu, "useWardjump");
            Console.WriteLine("7");
            var doQ = getCheckBoxItem(mainMenu, "useQ");
            Console.WriteLine("8");
            var qPredHc = getBoxItem(mainMenu, "useQpred") + 4;
            Console.WriteLine("9");
            if (doQ)
            {
                Console.WriteLine("10");
                if (leeHero.Distance(flashVector) > 425 && leeHero.Distance(flashVector) < ChampInfo.Q.Range + distLeeToWardjump && CanQ1())
                {
                    Console.WriteLine("11");
                    if (gpUnit != null)
                    {
                        Console.WriteLine("12");
                        var pred = spellQ.GetSPrediction(gpUnit as AIHeroClient);
                        Console.WriteLine("13");
                        if (pred.HitChance >= (HitChance)qPredHc)
                        {
                            Console.WriteLine("14");
                            spellQ.Cast(pred.CastPosition);
                            Console.WriteLine("15");
                        }
                    }
                }
                Console.WriteLine("17");
                if (spellQ2.Instance.IsReady() && spellQ2.Instance.Name.ToLower() == QSpellNames[2].ToLower())
                {
                    Console.WriteLine("16");
                    spellQ2.Cast();
                }
                Console.WriteLine("18");
            }

            Console.WriteLine("19");

            if (bubbaPriorityMode == PriorityMode.Wardjump)
            {
                if (leeHero.Distance(flashVector) < distLeeToWardjump && CanWardJump() && doWardjump)
                {
                    lastWjTickMenu = Environment.TickCount;
                    WardJumpTo(flashVector);
                }
                else if (leeHero.Distance(flashVector) < 425 && leeHero.Distance(rTarget) < ChampInfo.R.Range
                         && flashSlot.IsReady() && doFlash && !CanWardJump() && Environment.TickCount > lastWjTickMenu + 1200)
                {
                    if (spellR.CastOnUnit(rTarget))
                    {
                        leeHero.Spellbook.CastSpell(flashSlot, flashVector);
                    }
                }
                else
                {
                    if (leeHero.Distance(flashVector) > distLeeKickPos)
                    {
                        if (getCheckBoxItem(mainMenu, "moveItself"))
                        {
                            EloBuddy.Player.IssueOrder(
                                GameObjectOrder.MoveTo,
                                getBoxItem(mainMenu, "moveItselfMode") == 0
                                    ? flashVector
                                    : Game.CursorPos);
                        }
                    }
                    else
                    {
                        spellR.CastOnUnit(rTarget);
                    }
                }
            }
            else if (bubbaPriorityMode == PriorityMode.Flash)
            {
                if (leeHero.Distance(flashVector) < 425 && leeHero.Distance(rTarget) < ChampInfo.R.Range
                    && flashSlot.IsReady() && doFlash)
                {
                    if (spellR.CastOnUnit(rTarget))
                    {
                        leeHero.Spellbook.CastSpell(flashSlot, flashVector);
                        lastFlashTickMenu = Environment.TickCount;
                    }
                }
                else if (!flashSlot.IsReady() && leeHero.Distance(flashVector) < distLeeToWardjump && CanWardJump()
                         && doWardjump && Environment.TickCount > lastFlashTickMenu + 1200)
                {
                    WardJumpTo(flashVector);
                }
                else
                {
                    if (leeHero.Distance(flashVector) > distLeeKickPos)
                    {
                        if (getCheckBoxItem(mainMenu, "moveItself"))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, getBoxItem(mainMenu, "moveItselfMode") == 0 ? flashVector : Game.CursorPos);
                        }
                    }
                    else
                    {
                        spellR.CastOnUnit(rTarget);
                    }
                }
            }
        }

        private static Obj_AI_Base GetClosestDirectEnemyUnitToPos(Vector3 pos)
        {
            List<Obj_AI_Base> possibleHeroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && pos.Distance(x.ServerPosition) < distLeeToWardjump && x.Health > spellQ.GetDamage(x)).ToList().ConvertAll(x => (Obj_AI_Base)x);

            List<Obj_AI_Base> possibleMinions = MinionManager.GetMinions(pos, distLeeToWardjump).Where(x => x.Health > spellQ.GetDamage(x)).ToList();

            List<Obj_AI_Base> allPossible = possibleHeroes.Concat(possibleMinions).ToList();

            allPossible = allPossible.OrderBy(unit => unit.Distance(pos)).ToList();

            Obj_AI_Base bestUnit = null;

            foreach (var candidate in allPossible)
            {
                var sCol = SPrediction.Collision.GetCollisions(
                    leeHero.ServerPosition.To2D(),
                    candidate.ServerPosition.To2D(),
                    ChampInfo.Q.Range, ChampInfo.Q.Width, ChampInfo.Q.Delay, ChampInfo.Q.Speed);

                var sColUnits = sCol.Units;

                var realUnits = new List<Obj_AI_Base>();

                if (sColUnits.Any())
                {
                    realUnits.AddRange(sColUnits.Where(unit => unit.NetworkId != leeHero.NetworkId && unit.NetworkId != candidate.NetworkId));
                }

                if (realUnits.Any())
                {
                    continue;
                }

                bestUnit = candidate;
                break;
            }

            return bestUnit;
        }

        private static void WardJumpTo(Vector3 pos)
        {
            var myWard = Items.GetWardSlot();

            if (Environment.TickCount > lastWjTick + 650)
            {
                if (Items.UseItem((int)myWard.Id, pos))
                {
                    lastWjTick = Environment.TickCount;

                    LeagueSharp.Common.Utility.DelayAction.Add(
                        100,
                        () =>
                            {
                                if (spellW.CastOnUnit(mostRecentWard))
                                {
                                    lastWjTickMenu = Environment.TickCount;
                                }
                            });
                }
            }
        }

        private static void WreckingBallOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("ward") && !(sender is Obj_GeneralParticleEmitter))
            {
                var ward = (Obj_AI_Base)sender;

                LeagueSharp.Common.Utility.DelayAction.Add(
                    10,
                    () =>
                        {
                            if (ward.Buffs.Any(x => x.SourceName == "Lee Sin"))
                            {
                                mostRecentWard = (Obj_AI_Minion)ward;
                            }
                        });
            }
        }

        private static bool CanWardJump()
        {
            return spellW.Instance.IsReady() && spellW.Instance.Name.ToLower() == WSpellNames[1].ToLower() && Items.GetWardSlot().IsValidSlot();
        }

        private static bool CanQ1()
        {
            return spellQ.Instance.IsReady() && spellQ.Instance.Name.ToLower() == QSpellNames[1].ToLower();
        }

        /*private static float GetWardjumpTime(Vector3 pos)
        {
            var distance = leeHero.Distance(pos);
            var speed = ChampInfo.W.Speed;

            var time = distance / speed;

            return time;
        }*/

        private static Vector3 GetFlashVector(bool forDraws = false)
        {
            var secondTargetPredPos = SPrediction.Prediction.GetFastUnitPosition(rTargetSecond, GetTimeBetweenTargets() + ChampInfo.R.Delay);

            var fVector = new Vector3(secondTargetPredPos.To3D().ToArray()).LSExtend(
                rTarget.ServerPosition, rTarget.Distance(secondTargetPredPos.To3D()) + distTargetKickPos);

            var fVectorDraw = new Vector3(secondTargetPredPos.To3D().ToArray()).LSExtend(rTarget.Position, rTarget.Distance(secondTargetPredPos.To3D()) + distTargetKickPos);

            return !forDraws ? fVector : fVectorDraw;
        }

        private static float GetTimeBetweenTargets()
        {
            var distance = rTarget.Distance(rTargetSecond);
            var speed = ChampInfo.R.Speed;

            var time = distance / speed;

            return time;
        }

        private static void WreckingBallOnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingsMenu, "disableAllDraw") || leeHero.IsDead)
            {
                return;
            }

            var heroPos = Drawing.WorldToScreen(leeHero.Position);
            var textDimension = Drawing.GetTextEntent("Bubba Kush Active!", 23);
            const int OffsetValue = -30;
            const int OffsetValueInfo = -50;
            var offSet = new Vector2(heroPos.X, heroPos.Y - OffsetValue);
            var offSetInfo = new Vector2(heroPos.X, heroPos.Y - OffsetValueInfo);

            var simpleCircles = getCheckBoxItem(drawingsMenu, "simpleCircles");

            if (getKeyBindItem(mainMenu, "bubbaKey"))
            {
                Drawing.DrawText(offSet.X - textDimension.Width / 2f, offSet.Y, System.Drawing.Color.Lime, "Bubba Kush Active!");
            }
            else
            {
                Drawing.DrawText(offSet.X - textDimension.Width / 2f, offSet.Y, System.Drawing.Color.Red, "Bubba Kush Inactive!");
            }

            if (getCheckBoxItem(drawingsMenu, "bestTarget"))
            {
                if (rTarget != null)
                {
                    if (!simpleCircles)
                    {
                        Drawing.DrawCircle(rTarget.Position, 200, System.Drawing.Color.Red);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(
                            rTarget.Position,
                            200,
                            System.Drawing.Color.Red);
                    }
                }
            }

            if (getCheckBoxItem(drawingsMenu, "secondTarget"))
            {
                if (rTargetSecond != null)
                {
                    if (!simpleCircles)
                    {
                        Drawing.DrawCircle(
                            rTargetSecond.Position,
                            150,
                            System.Drawing.Color.DeepSkyBlue);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(
                            rTargetSecond.Position,
                            150,
                            System.Drawing.Color.DeepSkyBlue);
                    }
                }
            }

            if (getCheckBoxItem(drawingsMenu, "traceLine"))
            {
                if (rTarget != null && rTargetSecond != null)
                {
                    Drawing.DrawLine(Drawing.WorldToScreen(rTarget.Position), Drawing.WorldToScreen(rTargetSecond.Position), 5, System.Drawing.Color.BlueViolet);
                }
            }

            if (getCheckBoxItem(drawingsMenu, "myRangeDraw"))
            {
                if (!simpleCircles)
                {
                    Drawing.DrawCircle(
                        leeHero.Position,
                        getSliderItem(mainMenu, "firstTargetRange"),
                        System.Drawing.Color.Yellow);
                }
                else
                {
                    Render.Circle.DrawCircle(
                        leeHero.Position,
                        getSliderItem(mainMenu, "firstTargetRange"),
                        System.Drawing.Color.Yellow);
                }
            }

            if (getCheckBoxItem(drawingsMenu, "rTargetDraw"))
            {
                if (rTarget != null)
                {
                    if (!simpleCircles)
                    {
                        Drawing.DrawCircle(
                            rTarget.Position,
                            getSliderItem(mainMenu, "secondTargetRange"),
                            System.Drawing.Color.Orange);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(
                            rTarget.Position,
                            getSliderItem(mainMenu, "secondTargetRange"),
                            System.Drawing.Color.Orange);
                    }
                }
            }

            if (bubbaKushing)
            {
                if (spellR.Instance.State == SpellState.NotLearned)
                {
                    Drawing.DrawText(offSetInfo.X, offSetInfo.Y, System.Drawing.Color.AliceBlue, "R is not leveled up yet");
                }
                else
                {
                    if (!spellR.Instance.IsReady())
                    {
                        Drawing.DrawText(offSetInfo.X, offSetInfo.Y, System.Drawing.Color.AliceBlue, "R is not ready yet");
                    }
                }
            }

            if (rTarget != null && rTargetSecond != null)
            {
                if (getCheckBoxItem(drawingsMenu, "predVector"))
                {
                    var flashVector = GetFlashVector(true);

                    Render.Circle.DrawCircle(flashVector, distLeeKickPos, System.Drawing.Color.Blue);
                }
            }
        }

        public static Menu mainMenu, extraMenu, drawingsMenu;

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

        private static void LoadMenu()
        {
            wbMenu = MainMenu.AddMenu("Wrecking Ball (Bubba Kush)", "wreckingball");

            mainMenu = wbMenu.AddSubMenu("Main Settings", "mainsettings");
            mainMenu.Add("bubbaKey", new KeyBind("Bubba Kush Key (Toggle)", false, KeyBind.BindTypes.PressToggle, 84));
            mainMenu.Add("mainMode", new ComboBox("Bubba Kush Mode", 0, "Most MaxHP >> Less HP", "Closest >> Less HP"));
            mainMenu.Add("switchKey", new KeyBind("Switch mode Key", false, KeyBind.BindTypes.HoldActive, 90));
            mainMenu.Add("useFlash", new CheckBox("Use Flash to get to Kick pos"));
            mainMenu.Add("useWardjump", new CheckBox("Use Wardjump to get to Kick pos"));
            mainMenu.Add("modePrio", new ComboBox("Kick pos method priority:", 0, "Wardjump", "Flash"));
            mainMenu.Add("useQ", new CheckBox("Use Q skill to gapclose to Kick pos", false));
            mainMenu.Add("useQpred", new ComboBox("Q Min Prediction", 1, "Medium", "High", "Very High"));
            mainMenu.Add("moveItself", new CheckBox("Move to Kick pos..."));
            mainMenu.Add("moveItselfMode", new ComboBox("If ^ true then move:", 1, "Automatically", "Manually"));
            mainMenu.Add("firstTargetRange", new Slider("Range to check for most HP enemy", 1000, 0, 2000));
            mainMenu.Add("secondTargetRange", new Slider("Range to check for second target", 600, 0, 650));

            extraMenu = wbMenu.AddSubMenu("Extra Settings", "extrasettings");
            extraMenu.AddGroupLabel("Adjust if needed");
            extraMenu.Add("distanceToKick", new Slider("Distance from first target to Kick Pos", 150, 100, 200));
            extraMenu.Add("distanceLeeKick", new Slider("Min distance from Lee to Kick Pos", 115, 10, 150));
            extraMenu.Add("distanceToWardjump", new Slider("Min distance to Kick Pos for Wardjump", 485, 300, 650));
            extraMenu.Add("valuesToDefault", new CheckBox("Reset values to default", false));

            drawingsMenu = wbMenu.AddSubMenu("Draw Settings", "drawing");
            drawingsMenu.Add("simpleCircles", new CheckBox("Use Simple Circles"));
            drawingsMenu.Add("bestTarget", new CheckBox("Draw Circle around R target"));//.SetValue(new Circle(false, System.Drawing.Color.Red));
            drawingsMenu.Add("secondTarget", new CheckBox("Draw Circle around second target"));//.SetValue(new Circle(false, System.Drawing.Color.DeepSkyBlue));
            drawingsMenu.Add("traceLine", new CheckBox("Draw Line from first to second target"));//.SetValue(new Circle(false, System.Drawing.Color.BlueViolet));
            drawingsMenu.Add("predVector", new CheckBox("Draw predicted Kick Vector"));//.SetValue(new Circle(false, System.Drawing.Color.Blue));
            drawingsMenu.AddGroupLabel("Ranges :");
            drawingsMenu.Add("myRangeDraw", new CheckBox("Draw R target search range"));//.SetValue(new Circle(false, System.Drawing.Color.Yellow));
            drawingsMenu.Add("rTargetDraw", new CheckBox("Draw second target search range"));//.SetValue(new Circle(false, System.Drawing.Color.Orange));
            drawingsMenu.AddSeparator();
            drawingsMenu.Add("disableAllDraw", new CheckBox("Disable all drawings", false));
        }
    }
}
