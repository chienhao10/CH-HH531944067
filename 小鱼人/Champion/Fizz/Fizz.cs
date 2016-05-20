#region

using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using SimplisticTemplate.Champion.Fizz.Modes;
using SimplisticTemplate.Champion.Fizz.Utils;

#endregion

namespace SimplisticTemplate.Champion.Fizz
{
    internal static class Fizz
    {
        /*
        Spell Init
        */
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

        public static void Initialize()
        {
            Bootstrap.Init(null);

            InitSpells();
            InitMisc();
        }

        private static void InitSpells()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 550);
            W = new Spell.Active(SpellSlot.W, (uint) ObjectManager.Player.GetAutoAttackRange());
            E = new Spell.Skillshot(SpellSlot.E, 400, SkillShotType.Circular, 250, int.MaxValue, 330);
            R = new Spell.Skillshot(SpellSlot.R, 1300, SkillShotType.Linear, 250, 1200, 80);
            E.AllowedCollisionCount = int.MaxValue;
            R.AllowedCollisionCount = 0;

            //end of spell init
        }

        private static void InitMisc()
        {
            GameMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnSpellCast += Misc.ObjAiBaseOnOnProcessSpellCast;
            Drawing.OnDraw += Drawings.OnDraw;
            Drawing.OnEndScene += Drawings.OnDamageDraw;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    Combo.Execute();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    Harass.Execute();
                    break;
            }

            if (GameMenu.ComboMenu["qrcombo"].Cast<KeyBind>().CurrentValue) Combo.QRCombo();
        }
    }
}