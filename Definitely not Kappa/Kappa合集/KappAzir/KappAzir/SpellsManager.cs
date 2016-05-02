namespace KappAzir
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using
    

    static
Menus;
    using Mario_s_Lib;

    public static class SpellsManager
    {
        /*
        Targeted spells are like Katarina`s Q
        Active spells are like Katarina`s W
        Skillshots are like Ezreal`s Q
        Circular Skillshots are like Lux`s E and Tristana`s W
        Cone Skillshots are like Annie`s W and ChoGath`s W
        */

        //Remenber of putting the correct type of the spell here
        public static Spell.Skillshot Q;

        public static Spell.Skillshot Q2;

        public static Spell.Skillshot W;

        public static Spell.Skillshot E;

        public static Spell.Skillshot R;

        public static Spell.Skillshot Flash;

        /// <summary>
        /// It sets the values to the spells
        /// </summary>
        public static void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1000, 65) { AllowedCollisionCount = int.MaxValue };
            Q2 = new Spell.Skillshot(SpellSlot.Q, 870, SkillShotType.Linear, 250, 1000, 65) { AllowedCollisionCount = int.MaxValue };
            W = new Spell.Skillshot(SpellSlot.W, 525, SkillShotType.Circular);
            E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Linear, 250, 1200, 80) { AllowedCollisionCount = int.MaxValue };
            R = new Spell.Skillshot(SpellSlot.R, 350, SkillShotType.Linear, 500, 1000, 220) { AllowedCollisionCount = int.MaxValue };

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerFlash")) != null)
            {
                Flash = new Spell.Skillshot(Player.Instance.GetSpellSlotFromName("SummonerFlash"), 450, SkillShotType.Circular);
            }

            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        #region Damages

        internal static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Slot == SpellSlot.Q && sender.IsMe)
            {
                Orbwalker.ResetAutoAttack();
            }
        }

        /// <summary>
        /// It will return the damage but you need to set them before getting the damage
        /// </summary>
        /// <param name="target"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static float GetDamage(this Obj_AI_Base target, SpellSlot slot)
        {
            var damageType = DamageType.Magical;
            var AD = Player.Instance.FlatPhysicalDamageMod;
            var AP = Player.Instance.FlatMagicDamageMod;
            var sLevel = Player.GetSpell(slot).Level - 1;

            //You can get the damage information easily on wikia

            var dmg = 0f;

            switch (slot)
            {
                case SpellSlot.Q:
                    if (Q.IsReady())
                    {
                        //Information of Q damage
                        dmg += new float[] { 65, 85, 105, 125, 145 }[sLevel] + 0.5f * AP;
                    }
                    break;
                case SpellSlot.W:
                    if (W.IsReady())
                    {
                        //Information of W damage
                        dmg += 50 + (10 * Player.Instance.Level) + 0.4f * AP;
                    }
                    break;
                case SpellSlot.E:
                    if (E.IsReady())
                    {
                        //Information of E damage
                        dmg += new float[] { 60, 90, 120, 150, 180 }[sLevel] + 0.4f * AP;
                    }
                    break;
                case SpellSlot.R:
                    if (R.IsReady())
                    {
                        //Information of R damage
                        dmg += new float[] { 150, 225, 300 }[sLevel] + 0.6f * AP;
                    }
                    break;
            }
            return Player.Instance.CalculateDamageOnUnit(target, damageType, dmg - 10);
        }

        public static float Damage(this Obj_AI_Base target)
        {
            const DamageType damageType = DamageType.Magical;
            var AD = Player.Instance.FlatPhysicalDamageMod;
            var AP = Player.Instance.FlatMagicDamageMod;

            //You can get the damage information easily on wikia

            var dmg = 0f;

            if (Q.IsReady())
            {
                //Information of Q damage
                dmg += new float[] { 65, 85, 105, 125, 145 }[Player.GetSpell(SpellSlot.Q).Level - 1] + 0.5f * AP;
            }
            if (W.IsReady())
            {
                //Information of W damage
                dmg += 50 + (10 * Player.Instance.Level) + 0.4f * AP;
            }
            if (E.IsReady())
            {
                //Information of E damage
                dmg += new float[] { 60, 90, 120, 150, 180 }[Player.GetSpell(SpellSlot.E).Level - 1] + 0.4f * AP;
            }
            if (R.IsReady())
            {
                //Information of R damage
                dmg += new float[] { 150, 225, 300 }[Player.GetSpell(SpellSlot.R).Level - 1] + 0.6f * AP;
            }

            return Player.Instance.CalculateDamageOnUnit(target, damageType, dmg - 10);
        }

        #endregion Damages

        /// <summary>
        /// This event is triggered when a unit levels up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (MiscMenu.GetCheckBoxValue("activateAutoLVL") && sender.IsMe)
            {
                var delay = MiscMenu.GetSliderValue("delaySlider");
                Core.DelayAction(LevelUPSpells, delay);
            }
        }

        /// <summary>
        /// It will level up the spell using the values of the comboboxes on the menu as a priority
        /// </summary>
        private static void LevelUPSpells()
        {
            if (Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.R))
            {
                Player.Instance.Spellbook.LevelSpell(SpellSlot.R);
            }

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(GetSlotFromComboBox(MiscMenu.GetComboBoxValue("firstFocus"))))
            {
                Player.Instance.Spellbook.LevelSpell(GetSlotFromComboBox(MiscMenu.GetComboBoxValue("firstFocus")));
            }

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(GetSlotFromComboBox(MiscMenu.GetComboBoxValue("secondFocus"))))
            {
                Player.Instance.Spellbook.LevelSpell(GetSlotFromComboBox(MiscMenu.GetComboBoxValue("secondFocus")));
            }

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(GetSlotFromComboBox(MiscMenu.GetComboBoxValue("thirdFocus"))))
            {
                Player.Instance.Spellbook.LevelSpell(GetSlotFromComboBox(MiscMenu.GetComboBoxValue("thirdFocus")));
            }
        }

        /// <summary>
        /// It will transform the value of the combobox into a SpellSlot
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static SpellSlot GetSlotFromComboBox(this int value)
        {
            switch (value)
            {
                case 0:
                    return SpellSlot.Q;
                case 1:
                    return SpellSlot.W;
                case 2:
                    return SpellSlot.E;
            }
            Chat.Print("Failed getting slot");
            return SpellSlot.Unknown;
        }
    }
}