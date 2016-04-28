using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace TheBrand.Commons.ComboSystem
{
    public class ComboProvider
    {
        // ReSharper disable InconsistentNaming
        public enum SpellOrder
        {
            RQWE,
            RQEW,
            RWQE,
            RWEQ,
            REQW,
            REWQ
        }

        // ReSharper restore InconsistentNaming
        private readonly Dictionary<int, float> _marks = new Dictionary<int, float>();

        private readonly List<Tuple<Skill, Action>> _queuedCasts = new List<Tuple<Skill, Action>>();
            //Todo: check if properly working

        public readonly Dictionary<string, List<InterruptableSpell>> InterruptableSpells =
            new Dictionary<string, List<InterruptableSpell>>();

        private bool _cancelSpellUpdates;
        public bool AntiGapcloser = true;
        public DamageType DamageType;
        public Dictionary<string, bool> GapcloserCancel = new Dictionary<string, bool>();
        public bool Interrupter = true;
        protected List<Skill> Skills;
        public AIHeroClient Target;
        public float TargetRange;

        /// <summary>
        ///     Represents a "combo" and it's logic. Manages skill logic.
        /// </summary>
        public ComboProvider(float targetSelectorRange, IEnumerable<Skill> skills)
        {
            Skills = skills as List<Skill> ?? skills.ToList();
            DamageType = Skills.Count(spell => spell.DamageType == DamageType.Magical) >
                         Skills.Count(spell => spell.DamageType == DamageType.Physical)
                ? DamageType.Magical
                : DamageType.Physical;
            TargetRange = targetSelectorRange;

            Drawing.OnDraw += _ =>
            {
                foreach (var skill in Skills)
                {
                    skill.Draw();
                }
            };

            Spellbook.OnCastSpell += (sender, args) =>
            {
                if (!sender.Owner.IsMe) return;

                for (var i = 0; i < _queuedCasts.Count; i++)
                {
                    if (_queuedCasts[i].Item1.Slot == args.Slot)
                    {
                        _queuedCasts.RemoveAt(i);
                        break;
                    }
                }
            };
        }

        /// <summary>
        ///     Represents a "combo" and it's logic. Manages skill logic.
        /// </summary>
        public ComboProvider(float targetSelectorRange, params Skill[] skills)
            : this(targetSelectorRange, skills.ToList())
        {
        }

        public static Orbwalker.ActiveModes GetMode()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return Orbwalker.ActiveModes.Combo;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                return Orbwalker.ActiveModes.LaneClear;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                return Orbwalker.ActiveModes.JungleClear;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                return Orbwalker.ActiveModes.Harass;
            }
            return Orbwalker.ActiveModes.None;
        }

        public class InterruptableSpell
        {
            public InterruptableDangerLevel DangerLevel;
            public bool FireEvent = true;
            public bool MovementInterrupts;
            public SpellSlot Slot;

            public InterruptableSpell(SpellSlot slot, InterruptableDangerLevel danger, bool movementInterrupts)
            {
                Slot = slot;
                DangerLevel = danger;
                MovementInterrupts = movementInterrupts;
            }
        }

        #region Menu creators

        public void CreateBasicMenu(Menu comboMenu, Menu harassMenu, Menu laneclearMenu,
            bool laneclearHarassSwitch = true)
        {
            if (comboMenu != null)
            {
                CreateComboMenu(comboMenu);
            }

            if (harassMenu != null)
            {
                CreateHarassMenu(harassMenu);
            }

            if (laneclearMenu != null)
            {
                CreateLaneclearMenu(laneclearMenu, laneclearHarassSwitch);
            }
        }

        public void CreateComboMenu(Menu comboMenu, params SpellSlot[] forbiddenSlots)
        {
            foreach (var skill in Skills)
            {
                var currentSkill = skill;
                if (forbiddenSlots.Contains(currentSkill.Slot)) continue;
                comboMenu.Add("Use" + skill.Slot, new CheckBox("Use " + skill.Slot));
                if (skill.IsSkillshot)
                {
                    comboMenu.Add(skill.Slot + "Hitchance",
                        new Slider(skill.Slot + " Hitchance (0 : Low | 1 : Medium | 2 : High | 3 : Very High)", 3, 0, 3));
                }
            }
        }

        public void CreateHarassMenu(Menu harassMenu, params SpellSlot[] forbiddenSlots)
        {
            foreach (var skill in Skills)
            {
                var currentSkill = skill;
                if (forbiddenSlots.Contains(currentSkill.Slot) || currentSkill.Slot == SpellSlot.R) continue;
                harassMenu.Add("Use" + skill.Slot, new CheckBox("Use " + skill.Slot));
                if (skill.IsSkillshot)
                {
                    harassMenu.Add(skill.Slot + "Hitchance",
                        new Slider(skill.Slot + " Hitchance (0 : Low | 1 : Medium | 2 : High | 3 : Very High)", 3, 0, 3));
                }
            }
        }

        public void CreateLaneclearMenu(Menu laneclearMenu, bool harassSwitch = true, params SpellSlot[] forbiddenSlots)
        {
            foreach (var skill in Skills)
            {
                var currentSkill = skill;
                if (forbiddenSlots.Contains(currentSkill.Slot) || currentSkill.Slot == SpellSlot.R) continue;
                laneclearMenu.Add("Use" + skill.Slot, new CheckBox("Use " + skill.Slot));
            }
            if (harassSwitch)
            {
                laneclearMenu.Add("Harassinsteadifenemynear", new CheckBox("Harass instead if enemy near", false));
            }
        }

        #endregion

        #region Core routines

        /// <summary>
        ///     Call to initialize all stuffs. If skills access the menu, this should be called after the menu creation
        /// </summary>
        public virtual void Initialize()
        {
            Skills.ForEach(skill => skill.Initialize(this));
        }

        /// <summary>
        ///     Call to initialize all stuffs. If skills access the menu, this should be called after the menu creation
        /// </summary>
        public virtual void Initialize(DamageType damageType)
        {
            DamageType = damageType;
            Initialize();
        }

        public virtual void Update()
        {
            Target = TargetSelector.GetTarget(TargetRange, DamageType);

            for (var i = 0; i < _queuedCasts.Count; i++)
            {
                if (_queuedCasts[i].Item1.HasBeenCast())
                    _queuedCasts.RemoveAt(i);
                else
                {
                    try
                    {
                        _queuedCasts[i].Item2();
                    }
                    catch
                    {
                        _queuedCasts.RemoveAt(i);
                    }
                    break;
                }
            }


            if (!ObjectManager.Player.Spellbook.IsCastingSpell)
            {
                Skills.Sort(); //Checked: this is not expensive
                foreach (var item in Skills)
                {
                    item.Update(GetMode(), this, Target);
                    if (_cancelSpellUpdates)
                    {
                        _cancelSpellUpdates = false;
                        break;
                    }
                }
            }
        }

        #endregion

        #region API

        public void CancelSpellUpdates()
        {
            _cancelSpellUpdates = true;
        }

        /// <summary>
        ///     Note: Do not use autoattacks as additionalSpellDamage!
        /// </summary>
        /// <param name="target"></param>
        /// <param name="additionalSpellDamage"></param>
        /// <returns></returns>
        public virtual bool ShouldBeDead(AIHeroClient target, float additionalSpellDamage = 0f)
        {
            //var healthPred = HealthPrediction.GetHealthPrediction(target, 1000);
            //return healthPred - (IgniteManager.GetRemainingDamage(target) + additionalSpellDamage) <= 0;
            return false;
        }

        /// <summary>
        ///     Estimates the damage the combo could do in it's current state
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public virtual float GetComboDamage(AIHeroClient enemy)
        {
            return Skills.Sum(skill => skill.ComboEnabled ? skill.GetDamage(enemy) : 0);
        }

        public void RemoveTopQueuedCast()
        {
            if (_queuedCasts.Count > 0) _queuedCasts.RemoveAt(_queuedCasts.Count - 1);
        }

        /// <summary>
        ///     Adds a cast to the cast-queue. The added castActions will be cast in the same order as they were added
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="castAction"></param>
        public void AddQueuedCast(Skill skill, Action castAction)
        {
            if (_queuedCasts.Any(t => t.Item1 == skill)) return;
            _queuedCasts.Add(new Tuple<Skill, Action>(skill, castAction));
        }

        /// <summary>
        ///     Returns the first skill of type Ts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSkill<T>() where T : Skill
        {
            return (T) Skills.FirstOrDefault(skill => skill is T);
        }

        /// <summary>
        ///     returns all skills
        /// </summary>
        /// <returns></returns>
        public Skill[] GetSkills()
        {
            return Skills.ToArray();
        }

        public void SetMarked(GameObject obj, float time = 1f)
        {
            _marks[obj.NetworkId] = Game.Time + time;
        }

        public bool IsMarked(GameObject obj)
        {
            return _marks.ContainsKey(obj.NetworkId) && _marks[obj.NetworkId] > Game.Time;
        }

        #endregion
    }
}