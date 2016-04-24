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
using System.Collections;
using EloBuddy.SDK.Rendering;
using System.Drawing;

namespace Syndra
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static int tickCount = 0;
        public static int LastWGrabAttempt = 0;
        public static int LastQCastAttempt = 0;
        public static int LastWCastAttempt = 0;
        public static int LastECastAttempt = 0;
        public static int LastRCastAttempt = 0;
        public static int QECombo = 0; //same as above
        public static int WECombo = 0;
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        public static Spell.Skillshot EQ;
        public static int[] qDamage = { 50, 95, 140, 185, 264 };
        public static int[] wDamage = { 80, 120, 160, 200, 240 };
        public static int[] eDamage = { 70, 115, 160, 205, 250 };
        public static int[] rDamage = { 90, 135, 180};
        public static SpellSlot ignite;
        public static bool gotIgnite = false;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Syndra)
                return;
            Print("Loading..");
            // Initialize classes
            Config.Initialize();

            Bootstrap.Init(null);

            OrbManager.init();

            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Circular, 600, int.MaxValue, 125);
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 250, 1600, 140);
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Cone, 250, 2500, 22);
            R = new Spell.Targeted(SpellSlot.R, 675);
            EQ = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear, 500, 2500, 55);
            try
            {
                ignite = Player.Spells.FirstOrDefault(o => o.SData.Name == "summonerdot").Slot;
                gotIgnite = true;
            } catch (Exception e)
            {
            }
            TargetSelector.ActiveMode = TargetSelectorMode.LeastHealth;

            DamageIndicator.Initialize(StateHandler.GetComboDamage);
            Game.OnUpdate += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Gapcloser.OnGapcloser += OnGapCloser;
            //Player.OnIssueOrder += OnIssueOrder;
            AIHeroClient.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            AIHeroClient.OnBasicAttack += Obj_AI_Hero_OnBasicAttack;
            Print("Loaded!");
        }

        private static void Game_OnTick(EventArgs args)
        {
            tickCount++;
           //if(tickCount%30==0)
             //  Chat.Print(tickCount + " tick and " + LastWCastAttempt);
            if (Player.Instance.IsDead) return;
            if(R.Level == 3 && R.Range!=750)
                R = new Spell.Targeted(SpellSlot.R, 750);

            if (Config.Misc.Menu["autokill"].Cast<CheckBox>().CurrentValue)
                StateHandler.CheckForKS();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateHandler.Combo();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || (Config.Modes.Harass.AutoHarass && Player.Instance.ManaPercent >= Config.Modes.Harass.MinMana))
            {
                StateHandler.Harass(false);
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateHandler.WaveClear();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                StateHandler.LastHit();
            }
            else if(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2)
            {
                StateHandler.FinishW();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Drawing.DrawQ)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = Q.Range }.Draw(Player.Instance.Position);
            }
            if (Config.Drawing.DrawW)
            {
                new Circle() { Color = Color.Blue, BorderWidth = 1, Radius = W.Range }.Draw(Player.Instance.Position);
            }
            if (Config.Drawing.DrawQE)
            {
                new Circle() { Color = Color.Purple, BorderWidth = 1, Radius = EQ.Range }.Draw(Player.Instance.Position);
            }
            if (Config.Modes.Harass.AutoHarass && !Config.Drawing.RemoveHarassText)
                Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new SharpDX.Vector2(30, 11), System.Drawing.Color.White, "自动骚扰", 15);
            if (Config.Modes.Harass.AutoAaHarass && !Config.Drawing.RemoveHarassText)
                Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new SharpDX.Vector2(30, 22), System.Drawing.Color.White, "敌人普攻时骚扰", 15);

            if (OrbManager.Get_Current_Orb() != null)
                Drawing.DrawCircle(OrbManager.Get_Current_Orb().Position, W.Width, Color.Green);

            if (StateHandler.debug)
                Drawing.DrawCircle(StateHandler.qe, 80, Color.Red);

            if (Config.Drawing.DrawQELines)
            {
                foreach (var orb in OrbManager.GetAllOrbs())
                    if (!orb.Equals(OrbManager.Get_Current_Orb()) && SharpDX.Vector3.Distance(Player.Instance.Position, orb.ServerPosition) < Q.Range)
                    {
                        Drawing.DrawCircle(orb.Position, W.Width, Color.Red);

                        //var startPoint = orb.ServerPosition.To2D();//.Extend(_Player.ServerPosition.To2D(), 100); //_Player.ServerPosition.To2D();//
                        var endPoint = Player.Instance.ServerPosition.To2D()
                            .Extend(orb.ServerPosition.To2D(), EQ.Range /*Vector3.Distance(_Player.Position, orb) > 200 ? 1300 : 1000*/);
                        //Program.EQ.CastDelay = (int)Program.E.CastDelay;// +(int)(_Player.Distance(enemy) / Program.EQ.Speed * 1000);//(int)Vector3.Distance(_Player.Position, orb) / Program.E.Speed;
                        //EQ.SourcePosition = Player.Instance.ServerPosition;//orb;
                        Drawing.DrawLine(Player.Instance.Position.WorldToScreen(), endPoint.To3D().WorldToScreen(), 5, Color.Black);

                    }
                /*foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy == true))
                    if (enemy.IsValidTarget(EQ.Range))
                    {
                        Program.EQ.SourcePosition = Player.Instance.ServerPosition;//orb;
                        var enemyPred = Program.EQ.GetPrediction(enemy);
                        Drawing.DrawCircle(enemyPred.UnitPosition, enemy.BoundingRadius, Color.Black);
                        Drawing.DrawCircle(enemyPred.UnitPosition, enemy.BoundingRadius-1, Color.Black);
                        Drawing.DrawCircle(enemyPred.UnitPosition, enemy.BoundingRadius-2, Color.Black);

                    }*/
            }
        }

        static void OnInterruptableSpell(Obj_AI_Base sender, EloBuddy.SDK.Events.Interrupter.InterruptableSpellEventArgs args)
        {
            if (!Config.Misc.Menu["interrupt"].Cast<CheckBox>().CurrentValue || !sender.IsEnemy || !(sender is AIHeroClient)) return;

            if (StateHandler.debug)
                Chat.Print("Interrupting " + sender.Name);

            if (Player.Instance.Distance(sender) < E.Range && E.IsReady())
            {
                Q.Cast(sender.ServerPosition);
                E.Cast(sender.ServerPosition);
                StateHandler.qeb = true;
                StateHandler.qe = sender.ServerPosition;
            }
            else if (Player.Instance.Distance(sender) < EQ.Range && E.IsReady() && Q.IsReady())
            {
                StateHandler.UseQE(sender);
            }
            else if(Player.Instance.Distance(sender) < E.Range && E.IsReady())
            {
                E.Cast(sender);
            }
        }

        static void OnGapCloser(AIHeroClient sender, EloBuddy.SDK.Events.Gapcloser.GapcloserEventArgs args)
        {
            if (Config.Misc.Menu["antigap"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.IsEnemy && sender is AIHeroClient && Player.Instance.Distance(sender) < E.Range && E.IsReady())
                {
                    if (StateHandler.debug)
                        Chat.Print("USED E FOR Gapcloser " + args.SpellName);
                    E.Cast(sender.Position);
                }
            }
        }

        /*private static void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!Config.Modes.Combo.DisableAA)
            {
                return;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && sender.IsMe && args.Order == GameObjectOrder.AttackUnit)
            {
                args.Process = false;
            }
        }*/

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if(StateHandler.debug)Chat.Print(args.SData.Name);
            if (sender.IsMe && tickCount - QECombo < 50 &&
                (args.SData.Name == "SyndraQ"))
            {
                LastWCastAttempt = tickCount + 40;
                E.Cast(args.End);
            }

            if (sender.IsMe && tickCount - WECombo < 50 &&
                (args.SData.Name == "SyndraW" || args.SData.Name == "syndrawcast"))
            {
                LastWCastAttempt = tickCount + 40;
                E.Cast(args.End);
                if (StateHandler.debug)
                    Chat.Print("WE in SpellProcess");
            }
            
        }

        private static void Obj_AI_Hero_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && Config.Modes.Harass.AutoAaHarass && Q.IsReady()
                && Player.Instance.Distance(sender) < Q.Range + Q.Width && Player.Instance.Distance(sender) > 200
                && Player.Instance.ManaPercent >= Config.Modes.Harass.MinMana)
            {
                if (StateHandler.debug)
                    Chat.Print("Enemy AA " + sender.Name);
                StateHandler.Harass(true);
            }

            if (StateHandler.debug && sender.Distance(Player.Instance.Position) < 1000 && sender.IsEnemy && sender is AIHeroClient)
                Chat.Print("Enemy AA Cannot Q" + sender.Name);
        }



        private static void Print(string msg)
        {
            Chat.Print(
                "<font color='#ff3232'>Syndra</font>:</font> <font color='#FFFFFF'>" +
                msg + "</font>");
        }
    }
}
