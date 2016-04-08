using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using EloBuddy.SDK.Rendering;
using Color1 = System.Drawing.Color;

namespace SmiteGH
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static double TotalDamage = 0;

        public static Spell.Targeted Smite;
        public static Obj_AI_Base Monster;
        public static Text SmiteStatus; 
        public static string[] SupportedChampions =
        {
            "Nunu" , "Chogath", "Shaco", "Vi", "MasterYi", "Rengar",
            "Nasus" , "Khazix", "Fizz", "Elise", "Volibear",
            "Warwick", "Irelia", "Amumu", "Hecarim", "Pantheon",
            "Olaf", "LeeSin", "MonkeyKing", "Evelynn" , "Diana"
        };
        public static string[] MonstersNames =
        {
            "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith",
            "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", 
            "SRU_Red", "SRU_Krug", "SRU_Dragon", "Sru_Crab", "SRU_Baron"
        };
        public static Menu SmiteGHMenu, MobsToSmite, DrawingMenu;
        private static string[] SmiteNames = new[] { "s5_summonersmiteplayerganker", "itemsmiteaoe", "s5_summonersmitequick", "s5_summonersmiteduel", "summonersmite" };
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Bootstrap.Init(null);
            SmiteStatus = new EloBuddy.SDK.Rendering.Text("", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 9, System.Drawing.FontStyle.Bold));
            if (SmiteNames.Contains(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name.ToLower()))
            {
                Smite = new Spell.Targeted(SpellSlot.Summoner1, (uint)570f);
            }
            if (SmiteNames.Contains(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name.ToLower()))
            {
                Smite = new Spell.Targeted(SpellSlot.Summoner2, (uint)570f);
            }

            if (Smite == null) //if you don't have smite, so ? ^_^ Why we load?
                return;

            if (SupportedChampions.Contains(ObjectManager.Player.ChampionName))
                Chat.Print("[SmiteGH Loaded] " + ObjectManager.Player.ChampionName + " Has Loaded!", Color1.Violet);
            else
                Chat.Print("[SmiteGH Loaded] " + ObjectManager.Player.ChampionName + " is not supported, but Smite still working!", Color1.Violet);

            SmiteGHMenu = MainMenu.AddMenu("自动惩戒", "smitegh");
            SmiteGHMenu.AddGroupLabel("自动惩戒（重击）");
            SmiteGHMenu.AddSeparator();
            SmiteGHMenu.Add("active", new CheckBox("开启"));
            SmiteGHMenu.Add("activekey", new KeyBind("开启 (开关键)", true, KeyBind.BindTypes.PressToggle));
            if (SupportedChampions.Contains(ObjectManager.Player.ChampionName))
                SmiteGHMenu.Add("disable", new CheckBox("对英雄使用惩戒 (" + ObjectManager.Player.ChampionName + ")", true));
            SmiteGHMenu.AddSeparator();
            SmiteGHMenu.AddLabel("打野可以搭配打野计时使用");

            MobsToSmite = SmiteGHMenu.AddSubMenu("野怪", "Monsters");
            MobsToSmite.AddGroupLabel("野怪设置");
            MobsToSmite.AddSeparator();
            MobsToSmite.Add("killsmite", new CheckBox("惩戒抢头"));
            MobsToSmite.AddSeparator();
            
             if (Game.MapId == GameMapId.TwistedTreeline)
            {
                MobsToSmite.Add("TT_Spiderboss", new CheckBox("大蜘蛛 计时"));
                MobsToSmite.Add("TT_NGolem", new CheckBox("石头人 计时"));
                MobsToSmite.Add("TT_NWolf", new CheckBox("狼 计时"));
                MobsToSmite.Add("TT_NWraith", new CheckBox("幽鬼 计时"));
            }
            else
            {
                MobsToSmite.Add("SRU_Baron", new CheckBox("男爵 计时"));
                MobsToSmite.Add("SRU_RiftHerald", new CheckBox("峡谷先锋 计时"));
                MobsToSmite.Add("SRU_Dragon", new CheckBox("龙 计时"));
                MobsToSmite.Add("SRU_Blue", new CheckBox("蓝 计时"));
                MobsToSmite.Add("SRU_Red", new CheckBox("红 计时"));
                MobsToSmite.Add("SRU_Gromp", new CheckBox("蛤蟆 计时"));
                MobsToSmite.Add("SRU_Murkwolf", new CheckBox("狼 计时"));
                MobsToSmite.Add("SRU_Krug", new CheckBox("石头人 计时"));
                MobsToSmite.Add("SRU_Razorbeak", new CheckBox("4鸟 计时"));
                MobsToSmite.Add("Sru_Crab", new CheckBox("河蟹 计时"));
            }

            DrawingMenu = SmiteGHMenu.AddSubMenu("线圈", "drawing");
            DrawingMenu.AddGroupLabel("线圈设置");
            DrawingMenu.AddSeparator();
            DrawingMenu.Add("draw", new CheckBox("开启"));
            DrawingMenu.AddSeparator(10);
            DrawingMenu.Add("smite", new CheckBox("惩戒范围"));
            DrawingMenu.Add("drawTxt", new CheckBox("显示文字"));
            DrawingMenu.Add("killable", new CheckBox("可击杀野怪底下显示圆圈"));


            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_Settings;
        }

        public static void Drawing_Settings(EventArgs args)
        {
            if (DrawingMenu["draw"].Cast<CheckBox>().CurrentValue == false)
                return;

            if (DrawingMenu["drawTxt"].Cast<CheckBox>().CurrentValue)
            {
                if (SmiteGHMenu["active"].Cast<CheckBox>().CurrentValue || SmiteGHMenu["activekey"].Cast<KeyBind>().CurrentValue)
                {
                    SmiteStatus.Position = Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(40, -40);
                    SmiteStatus.Color = Color1.CadetBlue;
                    SmiteStatus.TextValue = "惩戒 : 开启";
                    SmiteStatus.Draw();
                }
                else
                {
                    SmiteStatus.Position = Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(40, -40);
                    SmiteStatus.Color = Color1.DarkRed;
                    SmiteStatus.TextValue = "惩戒 : 关闭";
                    SmiteStatus.Draw();
                }
            }

            if (DrawingMenu["smite"].Cast<CheckBox>().CurrentValue)
            {
                if (Smite.IsReady())
                    new Circle() { Color = Color1.CadetBlue, Radius = 500f + 20, BorderWidth = 2f }.Draw(ObjectManager.Player.Position);
                else
                    new Circle() { Color = Color1.DarkRed, Radius = 500f + 20, BorderWidth = 2f }.Draw(ObjectManager.Player.Position);
            }

            if (DrawingMenu["killable"].Cast<CheckBox>().CurrentValue)
            {
                Monster = GetNearest(ObjectManager.Player.ServerPosition);
                if (Monster != null)
                {
                    if (Monster.Health <= (TotalDamage == 0 ? GetSmiteDamage() : TotalDamage) && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < 900f)
                        Circle.Draw(Color.Purple, 100f, Monster.ServerPosition);
                }
            }
        }

        public static int GetSmiteDamage()
        {
            int[] CalcSmiteDamage =
            {
                20 * ObjectManager.Player.Level + 370,
                30 * ObjectManager.Player.Level + 330,
                40 * ObjectManager.Player.Level + 240,
                50 * ObjectManager.Player.Level + 100
            };
            return CalcSmiteDamage.Max();
        }

        static double SmiteChampDamage()
        {
            if (Smite.Slot == EloBuddy.SDK.Extensions.GetSpellSlotFromName(ObjectManager.Player, "s5_summonersmiteduel"))
            {
                var damage = new int[] { 54 + 6 * ObjectManager.Player.Level };
                return Player.CanUseSpell(Smite.Slot) == SpellState.Ready ? damage.Max() : 0;
            }

            if (Smite.Slot == EloBuddy.SDK.Extensions.GetSpellSlotFromName(ObjectManager.Player, "s5_summonersmiteplayerganker"))
            {
                var damage = new int[] { 20 + 8 * ObjectManager.Player.Level };
                return Player.CanUseSpell(Smite.Slot) == SpellState.Ready ? damage.Max() : 0;
            }
            return 0;
        }
        private static void Game_OnUpdate(EventArgs args)
        {

            if (MobsToSmite["killsmite"].Cast<CheckBox>().CurrentValue)
            {
                var KillEnemy =
                    EntityManager.Heroes.Enemies.FirstOrDefault(hero => hero.IsValidTarget(500f)
                        && SmiteChampDamage() >= hero.Health);
                if (KillEnemy != null)
                    Player.CastSpell(Smite.Slot, KillEnemy);
            }
                if (SmiteGHMenu["active"].Cast<CheckBox>().CurrentValue || SmiteGHMenu["activekey"].Cast<KeyBind>().CurrentValue)
                {
                    double SpellDamage = 0;
                    Monster = GetNearest(ObjectManager.Player.ServerPosition);
                        if (Monster != null && MobsToSmite[Monster.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            if (SupportedChampions.Contains(ObjectManager.Player.ChampionName))
                            {
                                if (SmiteGHMenu["disable"].Cast<CheckBox>().CurrentValue)
                                {
                                    switch (ObjectManager.Player.ChampionName)
                                    {
                                        #region Diana
                                        case "Diana":
                                            {
                                                Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, (uint)895, SkillShotType.Circular);
                                                Spell.Targeted R = new Spell.Targeted(SpellSlot.R, (uint)825);

                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.DianaQ(Q.Level) + GetDamages.DianaR(R.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Q.Cast(Monster.ServerPosition);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                else if (Smite.IsReady() && R.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < R.Range
                                                && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range && GetDamages.HasBuffs(Monster, "dianamoonlight"))
                                                {
                                                    SpellDamage = GetDamages.DianaR(R.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        R.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Evelynn
                                        case "Evelynn":
                                            {
                                                Spell.Targeted E = new Spell.Targeted(SpellSlot.E, (uint)(225f + 2 * 65f));
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && E.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < E.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Evelynn(E.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        E.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Irelia
                                        case "Irelia":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)650f + 20);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Irelia(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Q.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Amumu
                                        case "Amumu":
                                            {
                                                Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, (uint)1100f + 20, SkillShotType.Linear);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Amumu(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Q.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Hecarim
                                        case "Hecarim":
                                            {
                                                Spell.Active Q = new Spell.Active(SpellSlot.Q, (uint)350f + 20);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Hecarim(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Q.Cast();
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region MonkeyKing
                                        case "MonkeyKing":
                                            {
                                                Spell.Active Q = new Spell.Active(SpellSlot.Q, (uint)100f + 20);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.MonkeyKing(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Q.Cast();
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Pantheon
                                        case "Pantheon":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)600f + 20);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Pantheon(Q.Level, Monster);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Q.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Olaf
                                        case "Olaf":
                                            {
                                                Spell.Targeted E = new Spell.Targeted(SpellSlot.E, (uint)325f + 20);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && E.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < E.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Olaf(E.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        E.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Volibear
                                        case "Volibear":
                                            {
                                                Spell.Targeted W = new Spell.Targeted(SpellSlot.W, (uint)400f + 20);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && W.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < W.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Volibear(W.Level, Monster);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        W.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region WarWick
                                        case "Warwick":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)400f + 20);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Warwick(Q.Level, Monster);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        //Will add Spell's E too soon! ^-^
                                        #region LeeSin
                                        case "LeeSin":
                                            {
                                                Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, (uint)1300f, SkillShotType.Linear);
                                                Spell.Active Q2 = new Spell.Active(SpellSlot.Q, (uint)1500f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (GetDamages.BuffedEnemy != null)
                                                        SpellDamage = GetDamages.Q2Damage(Monster, Q2.Level);
                                                    else
                                                        SpellDamage = GetDamages.QDamage(Monster, Q.Level - 1) + GetDamages.Q2Damage(Monster, Q2.Level - 1);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        if (GetDamages.BuffedEnemy != null)
                                                            Q2.Cast(Monster);
                                                        else
                                                            Q.Cast(Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Nunu
                                        case "Nunu":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)200f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Nunu(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region ChoGath
                                        case "Chogath":
                                            {
                                                Spell.Targeted R = new Spell.Targeted(SpellSlot.R, (uint)175f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && R.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < R.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.ChoGath();
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(R.Slot, Monster);
                                                        // Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Shaco
                                        case "Shaco":
                                            {
                                                Spell.Targeted E = new Spell.Targeted(SpellSlot.E, (uint)625f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && E.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < E.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Shaco(E.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(E.Slot, Monster);
                                                        //  Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Vi
                                        case "Vi":
                                            {
                                                Spell.Active E = new Spell.Active(SpellSlot.E, (uint)600f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && E.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < E.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Vi(E.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(E.Slot, Monster);
                                                        //  Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region MasterYi
                                        case "MasterYi":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)600f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Master(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Rengar
                                        case "Rengar":
                                            {
                                                Spell.Active Q = new Spell.Active(SpellSlot.Q, (uint)ObjectManager.Player.AttackRange);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Rengar(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Nasus
                                        case "Nasus":
                                            {
                                                Spell.Active Q = new Spell.Active(SpellSlot.Q, (uint)ObjectManager.Player.AttackRange);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Nasus(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        //Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region KhaZix
                                        case "Khazix":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)325f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Khazix(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        // Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Fizz
                                        case "Fizz":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)550f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Fizz(Q.Level);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        //    Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Elise
                                        case "Elise":
                                            {
                                                Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, (uint)475f);
                                                //Smite and Spell  ==> OKAY
                                                if (Smite.IsReady() && Q.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Q.Range
                                                    && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    SpellDamage = GetDamages.Elise(Q.Level, Monster);
                                                    TotalDamage = SpellDamage + GetSmiteDamage();
                                                    if (Monster.Health <= TotalDamage)
                                                    {
                                                        Player.CastSpell(Q.Slot, Monster);
                                                        // Smite.Cast(Monster);
                                                    }
                                                }
                                                //If Spell is busy, Go Smite only! ^_^
                                                else if (Smite.IsReady() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                {
                                                    if (Monster.Health <= GetSmiteDamage())
                                                    {
                                                        Smite.Cast(Monster);
                                                    }
                                                    TotalDamage = 0;
                                                }
                                                break;
                                            }
                                        #endregion

                                        default:
                                            {
                                                if (Smite.IsReady() && Monster.Health <= GetSmiteDamage() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                                                    Smite.Cast(Monster);
                                                TotalDamage = 0;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (Smite.IsReady() && Monster.Health <= GetSmiteDamage() && Vector3.Distance(ObjectManager.Player.ServerPosition, Monster.ServerPosition) < Smite.Range)
                            {
                                Smite.Cast(Monster);
                                TotalDamage = 0;
                            }
                        }
                }
            
        }

        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {
            var mobs = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValid && MonstersNames.Any(name => minion.Name.ToLower().StartsWith(name.ToLower())) && !MonstersNames.Any(name => minion.Name.Contains("Mini")) && !MonstersNames.Any(name => minion.Name.Contains("Spawn")));
            var objAimobs = mobs as Obj_AI_Minion[] ?? mobs.ToArray();
            Obj_AI_Minion NearestMonster = objAimobs.FirstOrDefault();
            double? nearest = null;
            foreach (Obj_AI_Minion Monster in objAimobs)
            {
                double distance = Vector3.Distance(pos, Monster.Position);
                if (nearest == null || nearest > distance)
                {
                    nearest = distance;
                    NearestMonster = Monster;
                }
            }
            return NearestMonster;
        }
    }
}
