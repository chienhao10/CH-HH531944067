using System;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using PortAIO.Champion.Brand;
using SharpDX;
using Spell = LeagueSharp.Common.Spell;

namespace TheBrand.Commons.ComboSystem
{
    public abstract class Skill : Spell, IComparable<Skill>
    {
        protected readonly bool UseManaManager;

        public bool ComboEnabled = true, LaneclearEnabled = true, HarassEnabled = true;
            // If this skill should be used in Combo, ...

        /// <summary>
        ///     If set, methods like HasBeenCast will check if the spell name is equals the forced name. If not, it will assume
        ///     it's a cancelable spell like GarenE and has already been cast
        /// </summary>
        protected string ForcedSpellName;

        public HitChance MinComboHitchance = HitChance.Low, MinHarassHitchance = HitChance.VeryHigh;
        protected bool OnlyUpdateIfTargetValid = true, OnlyUpdateIfCastable = true;
        public bool SwitchClearToHarassOnTarget = true;

        protected Skill(SpellSlot slot, float range, DamageType damageType)
            : base(slot, range, damageType)
        {
            if (Slot == SpellSlot.R) HarassEnabled = false;
            UseManaManager = Instance.SData.ManaCostArray.MaxOrDefault(value => value) > 0;
        }

        protected Skill(SpellSlot slot)
            : this(slot, 3.402823E+38f, DamageType.Physical)
        {
        }

        protected ComboProvider Provider { get; private set; }

        #region Core routine

        /// <summary>
        ///     Add Initialisation logic in sub class. Called by ComboProvider.SetActive(skill)
        /// </summary>
        /// <param name="combo"></param>
        public virtual void Initialize(ComboProvider combo)
        {
            Provider = combo;
        }

        public virtual void Update(Orbwalker.ActiveModes mode, ComboProvider combo, AIHeroClient target)
        {
            if (mode == Orbwalker.ActiveModes.None) return;
            if (mode == Orbwalker.ActiveModes.LaneClear && SwitchClearToHarassOnTarget && target != null)
                mode = Orbwalker.ActiveModes.Harass;

            if (OnlyUpdateIfTargetValid && (mode == Orbwalker.ActiveModes.Combo || mode == Orbwalker.ActiveModes.Harass) &&
                !target.IsValidTarget()) return;
            if (OnlyUpdateIfCastable && !CanBeCast()) return; //Todo: check if nessecary with new comboSystem

            MinHitChance = mode == Orbwalker.ActiveModes.Combo ? MinComboHitchance : MinHarassHitchance;

            if (mode == Orbwalker.ActiveModes.Combo)
            {
                if (ComboEnabled)
                    Combo(combo, target);
            }

            if (mode == Orbwalker.ActiveModes.LaneClear || mode == Orbwalker.ActiveModes.JungleClear)
            {
                if (LaneclearEnabled && ObjectManager.Player.ManaPercent > Program.getMiscMenuSL("manaLC"))
                {
                    LaneClear(combo, target);
                }
            }

            if (mode == Orbwalker.ActiveModes.Harass)
            {
                if (HarassEnabled && ObjectManager.Player.ManaPercent > Program.getMiscMenuSL("manaH"))
                {
                    Harass(combo, target);
                }
            }
        }

        public abstract void Execute(AIHeroClient target);

        public virtual void Combo(ComboProvider combo, AIHeroClient target)
        {
            Execute(target);
        }

        public virtual void LaneClear(ComboProvider combo, AIHeroClient target)
        {
        }

        public virtual void Harass(ComboProvider combo, AIHeroClient target)
        {
            Execute(target);
        }

        public virtual void Draw()
        {
        }

        public abstract int GetPriority();

        #endregion

        #region API

        #region Cast overloads

        //Todo: only reset on successful cast

        public new CastStates Cast(Obj_AI_Base unit, bool packetCast = false, bool aoe = false)
        {
            var state = base.Cast(unit, packetCast, aoe);
            if (state == CastStates.SuccessfullyCasted)
                Provider.CancelSpellUpdates();
            return state;
        }

        public new bool Cast(bool packetCast = false)
        {
            var state = base.Cast(packetCast);
            if (state)
                Provider.CancelSpellUpdates();
            return state;
        }

        public new bool Cast(Vector2 fromPosition, Vector2 toPosition)
        {
            var state = base.Cast(fromPosition, toPosition);
            if (state)
                Provider.CancelSpellUpdates();
            return state;
        }

        public new bool Cast(Vector3 fromPosition, Vector3 toPosition)
        {
            var state = base.Cast(fromPosition, toPosition);
            if (state)
                Provider.CancelSpellUpdates();
            return state;
        }

        public new bool Cast(Vector2 position, bool packetCast = false)
        {
            var state = base.Cast(position, packetCast);
            if (state)
                Provider.CancelSpellUpdates();
            return state;
        }

        public new bool Cast(Vector3 position, bool packetCast = false)
        {
            var state = base.Cast(position, packetCast);
            if (state)
                Provider.CancelSpellUpdates();
            return state;
        }

        #endregion

        #region QueueCast Overloads

        public bool QueueCast(Action action)
        {
            if (!CanBeCast()) return false;
            Provider.AddQueuedCast(this, action);
            return true;
        }

        public bool QueueCast()
        {
            if (!CanBeCast()) return false;
            Provider.AddQueuedCast(this, () => Cast());
            return true;
        }

        public bool QueueCast(Vector2 target)
        {
            if (!CanBeCast()) return false;
            Provider.AddQueuedCast(this, () => Cast(target));
            return true;
        }

        public bool QueueCast(Vector2 from, Vector2 to)
        {
            if (!CanBeCast()) return false;
            Provider.AddQueuedCast(this, () => Cast(from, to));
            return true;
        }

        public bool QueueCast(Vector3 target)
        {
            if (!CanBeCast()) return false;
            Provider.AddQueuedCast(this, () => Cast(target));
            return true;
        }

        public bool QueueCast(Vector3 from, Vector3 to)
        {
            if (!CanBeCast()) return false;
            Provider.AddQueuedCast(this, () => Cast(from, to));
            return true;
        }


        public bool QueueCast(Obj_AI_Base target, bool aoe = false)
        {
            if (!CanBeCast()) return false;
            Provider.AddQueuedCast(this, () => Cast(target, false, aoe));
            return true;
        }

        #endregion

        /// <summary>
        ///     Note: if you already got the hitchance enum, just set the MinComboHitchance field!
        /// </summary>
        /// <param name="hitchance"></param>
        public void SetMinComboHitchance(string hitchance)
        {
            MinComboHitchance = hitchance.ToEnum<HitChance>();
        }

        /// <summary>
        ///     Note: if you already got the hitchance enum, just set the MinHarassHitchance field!
        /// </summary>
        /// <param name="hitchance"></param>
        public void SetMinHarassHitchance(string hitchance)
        {
            MinHarassHitchance = hitchance.ToEnum<HitChance>();
        }

        public virtual float GetDamage(AIHeroClient enemy)
        {
            return Instance.GetState() == SpellState.Ready ? ObjectManager.Player.GetSpellDamage(enemy, Slot) : 0f;
        }

        /// <summary>
        ///     If the spell seems available.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBeCast(bool checkSpellName = false)
        {
            return Instance.GetState() == SpellState.Ready &&
                   (!checkSpellName || (ForcedSpellName != null && ForcedSpellName == Instance.Name));
        }

        /// <summary>
        ///     If the spell seems available.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBeCast(string name)
        {
            return Instance.GetState() == SpellState.Ready && Instance.Name == name;
        }

        /// <summary>
        ///     If the spell has been cast. Will NOT check if still in safecast time.
        /// </summary>
        /// <returns></returns>
        public bool HasBeenCast()
        {
            return (Instance.GetState() == SpellState.Cooldown) ||
                   (ForcedSpellName != null && ForcedSpellName != Instance.Name);
        }

        public int CompareTo(Skill obj)
        {
            return obj.GetPriority() - GetPriority();
        }

        /// <summary>
        ///     If the Spell has a certain hitchance and is ready to cast and if the target is valid
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="minHitChance"></param>
        /// <param name="aoe"></param>
        /// <returns></returns>
        public bool CouldHit(Obj_AI_Base unit, HitChance minHitChance = HitChance.Low, bool aoe = false)
        {
            return GetPrediction(unit, aoe).Hitchance >= minHitChance && CanBeCast() && unit.IsValidTarget();
        }

        #endregion
    }
}