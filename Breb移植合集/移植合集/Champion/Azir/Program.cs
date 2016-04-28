using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;
using SAutoCarry.Champions.Helpers;

namespace HeavenStrikeAzir
{
    class Program
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static LeagueSharp.Common.Spell _q, _w, _e, _r , _q2, _r2;

        public static Menu _menu;

        public static int qcount,ecount;
        public static bool Eisready { get { return Player.Mana >= _e.Instance.SData.Mana && Utils.GameTimeTickCount - ecount >= _e.Instance.Cooldown * 1000f; } }

        public static string drawQ = "Draw Q", drawW = "Draw W", drawQE = "Draw Q+E", drawInsec = "Draw Insec";

        public static Menu spellMenu, Combo, Harass, _Auto, Draw;

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

        public static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "Azir")
                return;
            SoldierMgr.InitializeA();
            //Spells
            _q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1175);
            _q2 = new LeagueSharp.Common.Spell(SpellSlot.Q);
            _w = new LeagueSharp.Common.Spell(SpellSlot.W, 450);
            _e = new LeagueSharp.Common.Spell(SpellSlot.E, 1100);
            _r = new LeagueSharp.Common.Spell(SpellSlot.R, 250);
            _r2 = new LeagueSharp.Common.Spell(SpellSlot.R);
            _q.SetSkillshot(0.0f, 65, 1500, false, SkillshotType.SkillshotLine);
            _q.MinHitChance = LeagueSharp.Common.HitChance.Medium;

            //Menu instance
            _menu = MainMenu.AddMenu(Player.ChampionName, Player.ChampionName);
            
            //spell menu
            spellMenu = _menu.AddSubMenu("技能", "Spells");
            spellMenu.Add("EQdelay", new Slider("EQ 延迟（人性化）", 100, 0, 300));
            spellMenu.Add("EQmouse", new KeyBind("E Q 至鼠标", false, KeyBind.BindTypes.HoldActive, 'G'));
            spellMenu.Add("insec", new KeyBind("神推（选择的目标）", false, KeyBind.BindTypes.HoldActive, 'Y'));
            spellMenu.Add("insecmode", new ComboBox("神推模式", 0, "最近的友军", "最近的塔", "鼠标", "最后记录位置"));
            spellMenu.Add("insecpolar", new KeyBind("神推坐标记录", false, KeyBind.BindTypes.HoldActive, 'T'));

            //combo
            Combo = _menu.AddSubMenu("连招", "Combo");
            Combo.Add("QC", new CheckBox("Q"));
            Combo.Add("WC", new CheckBox("W"));
            Combo.Add("donotqC", new CheckBox("保留 Q 如果目标在士兵范围内", false));

            //Harass
            Harass = _menu.AddSubMenu("骚扰", "Harass");
            Harass.Add("QH", new CheckBox("Q"));
            Harass.Add("WH", new CheckBox("W"));
            Harass.Add("donotqH", new CheckBox("保留 Q 如果目标在士兵范围内", false));

            // AUTO
            _Auto = _menu.AddSubMenu("自动", "Auto");
            _Auto.Add("RKS", new CheckBox("使用 R 其他"));
            _Auto.Add("RTOWER", new CheckBox("R 目标至塔"));
            _Auto.Add("RGAP", new CheckBox("R 防突击", false));

            //Drawing
            Draw = _menu.AddSubMenu("线圈", "Drawing");
            Draw.Add("drawQ", new CheckBox(drawQ));
            Draw.Add("drawW", new CheckBox(drawW));
            Draw.Add("drawInsec", new CheckBox(drawInsec));

            GameObjects.Initialize();
            Soldiers.AzirSoldier();
            AzirCombo.Initialize();
            AzirHarass.Initialize();
            AzirFarm.Initialize();
            JumpToMouse.Initialize();
            Insec.Initialize();

            //Listen to events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnDoCast;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;
            if (target.IsEnemy && _r.IsReady() && target.IsValidTarget() && !target.IsZombie && RGAP)
            {
                if (target.IsValidTarget(250)) _r.Cast(target.Position);
            }
        }
        

        public static int EQdelay { get{ return getSliderItem(spellMenu, "EQdelay"); } }
        public static bool drawinsecLine { get{ return getCheckBoxItem(Draw, "drawInsec"); } }
        public static uint insecpointkey { get{ return spellMenu["insecpolar"].Cast<KeyBind>().Keys.Item1; } }
        public static bool eqmouse { get { return getKeyBindItem(spellMenu, "EQmouse"); } }
        public static bool RTOWER { get { return getCheckBoxItem(_Auto, "RTOWER"); } }
        public static bool RKS { get { return getCheckBoxItem(_Auto, "RKS"); } }
        public static bool RGAP { get { return getCheckBoxItem(_Auto, "RGAP"); } }
        public static bool qcombo { get { return getCheckBoxItem(Combo, "QC"); } }
        public static bool wcombo { get { return getCheckBoxItem(Combo, "WC"); } }
        public static bool donotqcombo { get { return getCheckBoxItem(Combo, "donotqC"); } }
        public static bool qharass { get { return getCheckBoxItem(Harass, "QH"); } }
        public static bool wharass { get { return getCheckBoxItem(Harass, "WH"); } }
        public static bool donotqharass { get { return getCheckBoxItem(Harass, "donotqH"); } }
        public static bool insec { get { return getKeyBindItem(spellMenu, "insec"); } }
        public static int insecmode { get { return getBoxItem(spellMenu, "insecmode"); } }
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name.ToLower().Contains("azirq"))
            {
                Qtick = Utils.GameTimeTickCount;
                qcount = Utils.GameTimeTickCount;
 
            }
            if (args.SData.Name.ToLower().Contains("azirw"))
            {

            }
            if (args.SData.Name.ToLower().Contains("azire"))
            {
                ecount = Utils.GameTimeTickCount;

            }
            if (args.SData.Name.ToLower().Contains("azirr"))
            {

            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Auto();
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (getCheckBoxItem(Draw, "drawQ"))
                Render.Circle.DrawCircle(Player.Position, _q.Range, Color.Yellow);
            if (getCheckBoxItem(Draw, "drawW"))
                Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Yellow);
        }

        private static void Auto()
        {
            if (RKS)
            {
                if (_r.IsReady())
                {
                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(250) && !x.IsZombie && x.Health < _r.GetDamage(x)))
                    {
                        _r.Cast(hero.Position);
                    }
                }
            }
            if(RTOWER)
            {
                if (_r.IsReady())
                {
                    var turret = ObjectManager.Get<Obj_AI_Turret>().Where(x => x.IsAlly && !x.IsDead).OrderByDescending(x => x.Distance(Player.Position)).LastOrDefault();
                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(250) && !x.IsZombie))
                    {
                        if (Player.ServerPosition.Distance(turret.Position)+100 >= hero.Distance(turret.Position) && hero.Distance(turret.Position) <= 775 + 250)
                        {
                            var pos = Player.Position.Extend(turret.Position, 250);
                            _r.Cast(pos);
                        }
                    }
                }
            }
        }



        public static bool  Qisready()
        {
            if (Utils.GameTimeTickCount - Qtick >= _q.Instance.Cooldown * 1000)
            {
                return true;
            }
            else
                return false;
        }
        public static int Qtick;

        public static double Wdamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, DamageType.Magical,
                        new double[]
                        {
                            50, 55, 60, 65, 70, 75, 80, 85, 90, 100, 110, 120, 130,
                            140, 150, 160, 170, 180
                        }[Player.Level - Player.SpellTrainingPoints - 1] + 0.6 * Player.FlatMagicDamageMod);
        }
        
    }
}
