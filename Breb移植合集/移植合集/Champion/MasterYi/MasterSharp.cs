using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Utility = LeagueSharp.Common.Utility;
/*
 * ToDo:
 * Q doesnt shoot much < fixed
 * Full combo burst <-- done
 * Useles gate <-- fixed
 * Add Fulldmg combo starting from hammer <-- done
 * Auto ignite if killabe/burst <-- done
 * More advanced Q calc area on hit
 * MuraMune support <-- done
 * Auto gapclosers E <-- done
 * GhostBBlade active <-- done
 * packet cast E <-- done 
 * 
 * 
 * Auto ks with QE <-done
 * Interupt channel spells <-done
 * Omen support <- done
 * 
 * 
 * */


namespace MasterSharp
{
    internal class MasterSharp
    {
        public const string CharName = "MasterYi";
        public static Menu Config;
        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        public static Menu comboMenu, extraMenu, evadeMenu, debugMenu;

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != CharName)
                return;

            MasterYi.setSkillShots();
            try
            {
                TargetedSkills.setUpSkills();

                Config = MainMenu.AddMenu("MasterYi - Sharp", "MasterYi");

                //Combo
                comboMenu = Config.AddSubMenu("Combo Sharp", "combo");
                comboMenu.Add("comboWreset", new CheckBox("AA reset W"));
                comboMenu.Add("useQ", new CheckBox("Use Q to gap"));
                comboMenu.Add("useE", new CheckBox("Use E"));
                comboMenu.Add("useR", new CheckBox("Use R"));
                comboMenu.Add("useSmite", new CheckBox("Use Smite"));

                //Extra
                extraMenu = Config.AddSubMenu("Extra Sharp", "extra");
                extraMenu.Add("packets", new CheckBox("Use Packet cast", false));

                //SmartW
                evadeMenu = Config.AddSubMenu("Q & W Dodger");
                evadeMenu.Add("smartW", new CheckBox("Smart/Dodge W if cantQ"));
                evadeMenu.Add("smartQDogue", new CheckBox("Q use dodge"));
                evadeMenu.Add("useWatHP", new Slider("use W below HP", 100));
                evadeMenu.Add("wqOnDead", new CheckBox("W or Q if will kill", false));
                getSkilshotMenuQ();
                getSkilshotMenuW();

                //Debug
                debugMenu = Config.AddSubMenu("Drawing");
                debugMenu.Add("drawCir", new CheckBox("Draw circles"));
                debugMenu.Add("debugOn", new KeyBind("Debug stuff", false, KeyBind.BindTypes.HoldActive, 'A'));

                Drawing.OnDraw += onDraw;
                Game.OnUpdate += OnGameUpdate;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
                SkillshotDetector.OnDeleteMissile += OnDeleteMissile;
                CustomEvents.Unit.OnDash += onDash;
                Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            }
            catch
            {
                Chat.Print("Oops. Something went wrong - Sharp");
            }
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (MasterYi.W.IsReady() && getCheckBoxItem(comboMenu, "comboWreset") &&
                getSliderItem(evadeMenu, "useWatHP") >= MasterYi.player.HealthPercent && target is AIHeroClient &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                MasterYi.W.Cast();
                Utility.DelayAction.Add(100, Orbwalker.ResetAutoAttack);
            }
        }

        private static void onDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (MasterYi.selectedTarget != null && sender.NetworkId == MasterYi.selectedTarget.NetworkId &&
                MasterYi.Q.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                && sender.Distance(MasterYi.player) <= 600)
                MasterYi.Q.Cast(sender);
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

        public static void getSkilshotMenuQ()
        {
            evadeMenu.AddGroupLabel("Q Dodge : ");
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (spell.ChampionName == hero.ChampionName)
                        {
                            evadeMenu.AddLabel(spell.MenuItemName + " :");
                            evadeMenu.Add("qEvadeAll" + spell.MenuItemName,
                                new CheckBox("Evade with Q always", spell.IsDangerous));
                            evadeMenu.Add("qEvade" + spell.MenuItemName,
                                new CheckBox("Evade with Q Combo", spell.IsDangerous));
                            evadeMenu.AddSeparator();
                        }
                    }
                }
            }
        }

        public static void getSkilshotMenuW()
        {
            evadeMenu.AddGroupLabel("W Dodge : ");
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (spell.ChampionName == hero.ChampionName)
                        {
                            evadeMenu.AddLabel(spell.MenuItemName + " :");
                            evadeMenu.Add("wEvadeAll" + spell.MenuItemName,
                                new CheckBox("Evade with W always", spell.IsDangerous));
                            evadeMenu.Add("wEvade" + spell.MenuItemName,
                                new CheckBox("Evade with W Combo", spell.IsDangerous));
                            evadeMenu.AddSeparator();
                        }
                    }
                }
            }
        }

        public static bool skillShotMustBeEvaded(string Name)
        {
            if (evadeMenu["qEvade" + Name] != null)
            {
                return getCheckBoxItem(evadeMenu, "qEvade" + Name);
            }
            return true;
        }

        public static bool skillShotMustBeEvadedAllways(string Name)
        {
            if (evadeMenu["qEvadeAll" + Name] != null)
            {
                return getCheckBoxItem(evadeMenu, "qEvadeAll" + Name);
            }
            return true;
        }

        public static bool skillShotMustBeEvadedW(string Name)
        {
            if (evadeMenu["wEvade" + Name] != null)
            {
                return getCheckBoxItem(evadeMenu, "wEvade" + Name);
            }
            return true;
        }

        public static bool skillShotMustBeEvadedWAllways(string Name)
        {
            if (evadeMenu["wEvadeAll" + Name] != null)
            {
                return getCheckBoxItem(evadeMenu, "wEvadeAll" + Name);
            }
            return true;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(debugMenu, "debugOn")) //fullDMG
            {
                foreach (var buf in MasterYi.player.Buffs)
                {
                    //Console.WriteLine(buf.Name);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var target = TargetSelector.GetTarget(800, DamageType.Physical);
                Orbwalker.ForcedTarget = target;
                if (target != null)
                    MasterYi.selectedTarget = target;
                MasterYi.slayMaderDuker(target);
            }

            DetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());
            foreach (var skillShot in DetectedSkillshots)
            {
                if (skillShot.IsAboutToHit(250, MasterYi.player))
                {
                    MasterYi.evadeSkillShot(skillShot);
                }
            }

            //anti buferino
            foreach (var buf in MasterYi.player.Buffs)
            {
                var skill = TargetedSkills.dagerousBuffs.FirstOrDefault(ob => ob.sName.ToLower() == buf.Name.ToLower());
                if (skill != null)
                {
                    // Console.WriteLine("Evade: " + buf.Name);
                    MasterYi.evadeBuff(buf, skill);
                }
                // if(buf.EndTime-Game.Time<0.2f)
            }
        }

        private static void onDraw(EventArgs args)
        {
            if (!getCheckBoxItem(debugMenu, "drawCir"))
                return;
            Utility.DrawCircle(MasterYi.player.Position, 600, Color.Green);
        }


        public static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (arg.Target != null && arg.Target.NetworkId == MasterYi.player.NetworkId)
            {
                //Console.WriteLine(arg.SData.Name);
                if (obj is AIHeroClient)
                {
                    var hero = (AIHeroClient) obj;
                    var skill = TargetedSkills.targetedSkillsAll.FirstOrDefault(ob => ob.sName == arg.SData.Name);
                    if (skill != null)
                    {
                        //Console.WriteLine("Evade: " + arg.SData.Name);
                        MasterYi.evadeDamage(skill.useQ, skill.useW, arg, skill.delay);
                    }
                }
            }
        }


        private static void OnDeleteMissile(Skillshot skillshot, MissileClient missile)
        {
            if (skillshot.SpellData.SpellName == "VelkozQ")
            {
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                var direction = skillshot.Direction.Perpendicular();
                if (DetectedSkillshots.Count(s => s.SpellData.SpellName == "VelkozQSplit") == 0)
                {
                    for (var i = -1; i <= 1; i = i + 2)
                    {
                        var skillshotToAdd = new Skillshot(
                            DetectionType.ProcessSpell, spellData, Environment.TickCount, missile.Position.To2D(),
                            missile.Position.To2D() + i*direction*spellData.Range, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                }
            }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;

            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                    skillshot.Direction.AngleBetween(item.Direction) < 5 &&
                    (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0))
                {
                    alreadyAdded = true;
                }
            }

            //Check if the skillshot is from an ally.
            if (skillshot.Unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            //Check if the skillshot is too far away.
            if (skillshot.Start.Distance(ObjectManager.Player.ServerPosition.To2D()) >
                (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000)*1.5)
            {
                return;
            }

            //Add the skillshot to the detected skillshot list.
            if (!alreadyAdded)
            {
                //Multiple skillshots like twisted fate Q.
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;

                        for (var i = -(skillshot.SpellData.MultipleNumber - 1)/2;
                            i <= (skillshot.SpellData.MultipleNumber - 1)/2;
                            i++)
                        {
                            var end = skillshot.Start +
                                      skillshot.SpellData.Range*
                                      originalDirection.Rotated(skillshot.SpellData.MultipleAngle*i);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int) skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.Invert)
                    {
                        var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                        var end = skillshot.Start + newDirection*skillshot.Start.Distance(skillshot.End);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.Centered)
                    {
                        var start = skillshot.Start - skillshot.Direction*skillshot.SpellData.Range;
                        var end = skillshot.Start + skillshot.Direction*skillshot.SpellData.Range;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                    {
                        var angle = 60;
                        var edge1 =
                            (skillshot.End - skillshot.Unit.ServerPosition.To2D()).Rotated(
                                -angle/2*(float) Math.PI/180);
                        var edge2 = edge1.Rotated(angle*(float) Math.PI/180);

                        foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            var v = minion.ServerPosition.To2D() - skillshot.Unit.ServerPosition.To2D();
                            if (minion.Name == "Seed" && edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                minion.Distance(skillshot.Unit) < 800 &&
                                (minion.Team != ObjectManager.Player.Team))
                            {
                                var start = minion.ServerPosition.To2D();
                                var end = skillshot.Unit.ServerPosition.To2D()
                                    .Extend(
                                        minion.ServerPosition.To2D(),
                                        skillshot.Unit.Distance(minion) > 200 ? 1300 : 1000);

                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                    skillshot.Unit);
                                DetectedSkillshots.Add(skillshotToAdd);
                            }
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                    {
                        var start = skillshot.End - skillshot.Direction.Perpendicular()*400;
                        var end = skillshot.End + skillshot.Direction.Perpendicular()*400;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.Distance(skillshot.End);
                        var d2 = d1*0.4f;
                        var d3 = d2*0.69f;


                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");

                        var bounce1Pos = skillshot.End + skillshot.Direction*d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction*d3;

                        bounce1SpellData.Delay =
                            (int) (skillshot.SpellData.Delay + d1*1000f/skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int) (bounce1SpellData.Delay + d2*1000f/bounce1SpellData.MissileSpeed + 500);

                        var bounce1 = new Skillshot(
                            skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End, bounce1Pos,
                            skillshot.Unit);
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                            skillshot.Unit);

                        DetectedSkillshots.Add(bounce1);
                        DetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int) (1500 + 1500*skillshot.End.Distance(skillshot.Start)/skillshot.SpellData.Range);
                    }

                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();

                        foreach (var s in DetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                endPos = s.End;
                            }
                        }

                        foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (m.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team &&
                                skillshot.IsDanger(m.Position.To2D()))
                            {
                                endPos = m.Position.To2D();
                            }
                        }

                        if (!endPos.IsValid())
                        {
                            return;
                        }

                        skillshot.End = endPos + 200*(endPos - skillshot.Start).Normalized();
                        skillshot.Direction = (skillshot.End - skillshot.Start).Normalized();
                    }
                }

                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    var endCSpellData = SpellDatabase.GetByName("OriannaQend");

                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, endCSpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                        skillshot.Unit);

                    DetectedSkillshots.Add(skillshotToAdd);
                }


                //Dont allow fow detection.
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }
#if DEBUG
                //Console.WriteLine(Environment.TickCount + "Adding new skillshot: " + skillshot.SpellData.SpellName);
#endif

                DetectedSkillshots.Add(skillshot);
            }
        }
    }
}