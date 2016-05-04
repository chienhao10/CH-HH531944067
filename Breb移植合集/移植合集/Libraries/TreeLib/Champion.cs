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

namespace TreeLib.Objects
{
    public class Champion
    {
        public static LeagueSharp.Common.Spell Q, W, E, R;
        public static Menu Menu;

        public Champion()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E);
            R = new LeagueSharp.Common.Spell(SpellSlot.R);

            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_OnUpdate;
        }

        public static List<AIHeroClient> Enemies
        {
            get { return HeroManager.Enemies; }
        }

        public static List<AIHeroClient> Allies
        {
            get { return HeroManager.Allies; }
        }

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static float AutoAttackRange
        {
            get { return Orbwalking.GetRealAutoAttackRange(Player); }
        }

        public virtual void Game_OnWndProc(WndEventArgs args) {}

        public virtual void GameObject_OnDelete(GameObject sender, EventArgs args) {}

        public virtual void GameObject_OnCreate(GameObject sender, EventArgs args) {}

        public virtual void Drawing_OnDraw(EventArgs args) {}

        public virtual void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args) {}

        public virtual void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args) { }

        public virtual void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args) { }

        private void Game_OnUpdate(EventArgs args)
        {
            OnUpdate();

            if (Player.IsDead)
            {
                return;
            }

            if (Player.IsDashing() || Player.IsChannelingImportantSpell()) /*|| Player.Spellbook.IsCastingSpell || 
                Player.Spellbook.IsAutoAttacking|| Player.IsWindingUp)*/
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                OnFarm();
            }
        }

        public virtual void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser) {}

        public virtual void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args) {}

        public virtual void OnUpdate() {}
        public virtual void OnCombo() {}
        public virtual void OnFarm() {}
    }
}