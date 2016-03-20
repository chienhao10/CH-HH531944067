using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace KKayle
{
    internal class Program
    {
        public const string ChampionName = "Kayle";
       
        public static Menu Menu, DrawMenu, ComboMenu, HarassMenu, FarmMenu, HealMenu, UltMenu;

        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        private static Spell.Targeted Ignite;


        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }
        public static float HealthPercent()
        {
            return (PlayerInstance.Health / PlayerInstance.MaxHealth) * 100;
        }


        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        private static bool Spell1(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        public static bool OnDamage = false;


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += DamageC;

        }


        //-----------------//
        // Start Game-----//
        // Game On Start-//
        static void Game_OnStart(EventArgs args)
        {
            
            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Bootstrap.Init(null);

                Chat.Print("KKayle Addon Loading Success");
                Q = new Spell.Targeted(SpellSlot.Q, 650);
                    Q.CastDelay = 5;
                W = new Spell.Targeted(SpellSlot.W, 900);
                E = new Spell.Active(SpellSlot.E, 650);
                R = new Spell.Targeted(SpellSlot.R, 900);
                if (Spell1("ignite"))   
                {
                    Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
                }

                Menu = MainMenu.AddMenu("K天使", "凯尔");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105 由 CH汉化");
                // Combo Menu
                ComboMenu = Menu.AddSubMenu("连招", "连招");
                ComboMenu.Add("ComboW", new CheckBox("连招使用W", true));
                ComboMenu.Add("useIgnite", new CheckBox("使用点燃", false));

                // Harass Menu
                HarassMenu = Menu.AddSubMenu("骚扰", "骚扰");
                HarassMenu.Add("HarassQ", new CheckBox("使用Q骚扰", true));
                HarassMenu.Add("HarassW", new CheckBox("使用W骚扰", false));
                HarassMenu.Add("HarassE", new CheckBox("使用E骚扰", true));
                HarassMenu.Add("ManaH", new Slider("不使用技能当蓝低于", 30));

                //Farm Menu
                FarmMenu = Menu.AddSubMenu("清线", "清线");
                FarmMenu.Add("ManaF", new Slider("不使用技能当蓝低于", 40));
                FarmMenu.Add("FarmQ", new CheckBox("使用Q尾兵", true));
                FarmMenu.Add("FarmE", new CheckBox("使用Q尾兵", true));
                FarmMenu.Add("MinionE", new Slider("小白数量多于时使用E技能清线", 3, 1, 5));
                FarmMenu.AddSeparator();
                FarmMenu.AddLabel("尾兵");
                FarmMenu.Add("LastQ", new CheckBox("使用Q尾兵", true));
               // FarmMenu.Add("LastE", new CheckBox("Use E to Last Hit", true));
               

                // Heal Menu
                var allies = EntityManager.Heroes.Allies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                HealMenu = Menu.AddSubMenu("治疗", "治疗");
                HealMenu.Add("autoW", new CheckBox("自动W", true));
                HealMenu.Add("HealSelf", new Slider("自身治疗当生命低于 %", 50));
                HealMenu.Add("HealAlly", new Slider("治疗友方当生命低于 %", 50));
                foreach (var a in allies)
                {
                    HealMenu.Add("autoHeal_" + a.BaseSkinName, new CheckBox("自动治疗友方 " + a.BaseSkinName));
                }
                

                //--------------//
                //---Ultmate---//
                //------------//

                var ally = EntityManager.Heroes.Allies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                UltMenu = Menu.AddSubMenu("大招", "大招");
                UltMenu.Add("autoR", new CheckBox("使用大招 ", true));
                UltMenu.Add("UltSelf", new Slider("对自身大招当生命低于 %", 20));
                UltMenu.Add("UltAlly", new Slider("对友方大招当生命低于  %", 20));
                foreach (var a in ally)
                {
                    UltMenu.Add("autoUlt_" + a.BaseSkinName, new CheckBox("对友方使用大招 " + a.BaseSkinName));
                }


                //------------//
                //-Draw Menu-//
                //----------//
                DrawMenu = Menu.AddSubMenu("线圈", "线圈");
                // DrawMenu.Add("drawDisable", new CheckBox("Desabilidatar todos os Draw", false));
                DrawMenu.Add("drawAA", new CheckBox("屏蔽AA范围", true));
                DrawMenu.Add("drawQ", new CheckBox("屏蔽Q线圈", true));
                DrawMenu.Add("drawW", new CheckBox("屏蔽W线圈", true));
                DrawMenu.Add("drawE", new CheckBox("屏蔽E线圈", true));
            }
            catch (Exception e)
            {
                Chat.Print("KKayle: Exception occured while Initializing Addon. Error: " + e.Message);
            }

            }
        
         
        // ------------//
        // Game OnDraw//
        // --------- //

        public static void Game_OnDraw(EventArgs args)
        {
      
            if (DrawMenu["drawAA"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.White, Radius = _Player.GetAutoAttackRange(), BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Aqua, Radius = 650, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Green, Radius = 900, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = _Player.GetAutoAttackRange() + 400, BorderWidth = 2f }.Draw(_Player.Position);
            }


        }

        //-----//
       // Heal //
      // -----//
       

        //-------------//
        //--Ultimate--//
        //-----------//
        
    

        // ------------//
        // Game On Update//
        // ------------//

        public static void Game_OnUpdate(EventArgs args)
        {

            

            //-------------//
            //----Modes----//
            //-------------//
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
               ModeManager.Combo();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))) 
            {
               ModeManager.Harass();

            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ModeManager.LaneClear();

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ModeManager.JungleClear();

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                ModeManager.LastHit();

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                ModeManager.Flee();

            }


            }


        public static void Game_OnTick(EventArgs args)
        {
            ModeManager.AutoHeal();
            ModeManager.AutoUlt();


        }
       

        public static void DamageC(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        
        {
           /* if (_Player.IsRecalling()) return;
            var target = args.Target as Obj_AI_Base;
            if (!target.IsAlly || sender.IsAlly || target == null)
            {

                return;
            }
            if (sender.IsEnemy && target.IsAlly && !target.IsMinion)
            {
                OnDamage = true;

            }

            if (target.IsUnderTurret())
            {
                OnDamage = true;
            }*/

        }
        static void KInterrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (args.DangerLevel == DangerLevel.High && sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < R.Range && R.IsReady())
            {
                R.Cast(Player.Instance);
            }

        }

        }

    }

