// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Zed.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//             This program is free software: you can redistribute it and/or modify
//             it under the terms of the GNU General Public License as published by
//             the Free Software Foundation, either version 3 of the License, or
//             (at your option) any later version.
//   
//             This program is distributed in the hope that it will be useful,
//             but WITHOUT ANY WARRANTY; without even the implied warranty of
//             MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//             GNU General Public License for more details.
//   
//             You should have received a copy of the GNU General Public License
//             along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   TODO The zed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using iDZed.Utils;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace iDZed
{
    /// <summary>
    ///     TODO The zed.
    /// </summary>
    internal static class Zed
    {
        #region Properties

        // private static bool _deathmarkKilled = false;

        /// <summary>
        ///     Gets the player.
        /// </summary>
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     TODO The on load.
        /// </summary>
        public static void OnLoad()
        {
            if (Player.ChampionName != "Zed")
            {
                return;
            }

            ShadowManager.OnLoad();

            InitMenu();
            InitSpells();
            InitEvents();
        }

        #endregion

        #region Delegates

        #endregion

        #region Static Fields

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     TODO The menu.
        /// </summary>
        public static Menu Menu;

        /// <summary>
        ///     TODO The w shadow spell.
        /// </summary>
        public static readonly SpellDataInst WShadowSpell = Player.Spellbook.GetSpell(SpellSlot.W);

        /// <summary>
        ///     TODO The r shadow spell.
        /// </summary>
        private static readonly SpellDataInst RShadowSpell = Player.Spellbook.GetSpell(SpellSlot.R);

        /// <summary>
        ///     TODO The e cast range.
        /// </summary>
        public static readonly float ERange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange - 10;

        /// <summary>
        ///     TODO The r cast range.
        /// </summary>
        public static readonly float RRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.CastRange;

        /// <summary>
        ///     TODO The _spells.
        /// </summary>
        public static readonly Dictionary<SpellSlot, Spell> _spells = new Dictionary<SpellSlot, Spell>
        {
            {
                SpellSlot.Q,
                new Spell(SpellSlot.Q, 925f)
            },
            {
                SpellSlot.W,
                new Spell(SpellSlot.W, 550f)
            },
            {
                SpellSlot.E,
                new Spell(SpellSlot.E, ERange)
            },
            {
                SpellSlot.R,
                new Spell(SpellSlot.R, RRange)
            }
        };

        #endregion

        #region Methods

        /// <summary>
        ///     TODO The cast e.
        /// </summary>
        private static void CastE()
        {
            if (!_spells[SpellSlot.E].IsReady())
            {
                return;
            }

            if (
                HeroManager.Enemies.Count(
                    hero =>
                        hero.IsValidTarget()
                        && (hero.LSDistance(Player.ServerPosition) <= _spells[SpellSlot.E].Range
                            || (ShadowManager.WShadow.ShadowObject != null
                                && hero.LSDistance(ShadowManager.WShadow.Position) <= _spells[SpellSlot.E].Range)
                            || (ShadowManager.RShadow.ShadowObject != null
                                && hero.LSDistance(ShadowManager.RShadow.Position) <= _spells[SpellSlot.E].Range))) > 0)
            {
                _spells[SpellSlot.E].Cast();
            }
        }

        /// <summary>
        ///     TODO The cast q.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        private static void CastQ(AIHeroClient target)
        {
            if (_spells[SpellSlot.Q].IsReady())
            {
                if (GetMarkedTarget() != null)
                {
                    target = GetMarkedTarget();
                }

                if (ShadowManager.WShadow.Exists
                    && ShadowManager.WShadow.ShadowObject.Distance(target.ServerPosition)
                    < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.Q].UpdateSourcePosition(
                        ShadowManager.WShadow.Position,
                        ShadowManager.WShadow.Position);
                    if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.useqpred"))
                    {
                        var prediction = _spells[SpellSlot.Q].GetPrediction(target);
                        if (prediction.Hitchance >= GetHitchance())
                        {
                            if (ShadowManager.WShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                            {
                                _spells[SpellSlot.Q].Cast(prediction.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (ShadowManager.WShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                        {
                            _spells[SpellSlot.Q].Cast(target.ServerPosition);
                        }
                    }
                }
                else if (ShadowManager.RShadow.Exists
                         && ShadowManager.RShadow.ShadowObject.Distance(target.ServerPosition)
                         < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.Q].UpdateSourcePosition(
                        ShadowManager.RShadow.Position,
                        ShadowManager.RShadow.Position);
                    if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.useqpred"))
                    {
                        var prediction = _spells[SpellSlot.Q].GetPrediction(target);
                        if (prediction.Hitchance >= GetHitchance())
                        {
                            if (ShadowManager.RShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                            {
                                _spells[SpellSlot.Q].Cast(prediction.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (ShadowManager.RShadow.ShadowObject.Distance(target) <= _spells[SpellSlot.Q].Range)
                        {
                            _spells[SpellSlot.Q].Cast(target.ServerPosition);
                        }
                    }
                }
                else
                {
                    _spells[SpellSlot.Q].UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                    if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.useqpred"))
                    {
                        var prediction = _spells[SpellSlot.Q].GetPrediction(target);
                        if (prediction.Hitchance >= GetHitchance())
                        {
                            if (Player.Distance(target) <= _spells[SpellSlot.Q].Range
                                && target.IsValidTarget(_spells[SpellSlot.Q].Range))
                            {
                                _spells[SpellSlot.Q].Cast(prediction.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (Player.Distance(target) <= _spells[SpellSlot.Q].Range
                            && target.IsValidTarget(_spells[SpellSlot.Q].Range))
                        {
                            _spells[SpellSlot.Q].Cast(target.ServerPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     TODO The cast w.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        private static void CastW(AIHeroClient target)
        {
            if (!HasEnergy(new[] {SpellSlot.W, SpellSlot.Q}))
            {
                return;
            }

            if (ShadowManager.WShadow.IsUsable)
            {
                if (_spells[SpellSlot.W].IsReady() && WShadowSpell.ToggleState == 0
                    && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                {
                    var position = Player.ServerPosition.To2D()
                        .Extend(target.ServerPosition.To2D(), _spells[SpellSlot.W].Range);
                    if (position.Distance(target) <= _spells[SpellSlot.Q].Range)
                    {
                        if (IsPassWall(Player.ServerPosition, target.ServerPosition))
                        {
                            return;
                        }

                        _spells[SpellSlot.W].Cast(position);
                        _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                    }
                }
            }

            if (ShadowManager.CanGoToShadow(ShadowManager.WShadow) && WShadowSpell.ToggleState == 2)
            {
                if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.swapw")
                    && ShadowManager.WShadow.ShadowObject.Distance(target.ServerPosition)
                    < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.W].Cast();
                }
            }
        }

        /// <summary>
        ///     TODO The combo.
        /// </summary>
        private static void Combo()
        {
            var target = GetAssasinationTarget();

            if (target == null)
            {
                return;
            }

            switch (getBoxItem(comboMenu, "com.idz.zed.combo.mode"))
            {
                case 0: // Line mode
                    if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.user") && _spells[SpellSlot.R].IsReady()
                        && (target.Health + 20
                            >= _spells[SpellSlot.Q].GetDamage(target) + _spells[SpellSlot.E].GetDamage(target)
                            + ObjectManager.Player.GetAutoAttackDamage(target)))
                    {
                        if (!HasEnergy(new[] {SpellSlot.W, SpellSlot.R, SpellSlot.Q, SpellSlot.E}))
                        {
                            return;
                        }

                        if (ShadowManager.WShadow.Exists)
                        {
                            CastQ(target);
                            CastE();
                        }
                        else
                        {
                            DoLineCombo(target);
                        }
                    }
                    else
                    {
                        DoNormalCombo(target);
                    }

                    break;
                case 1: // triangle mode
                    if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.user") && _spells[SpellSlot.R].IsReady()
                        && (target.Health + 20
                            >= _spells[SpellSlot.Q].GetDamage(target) + _spells[SpellSlot.E].GetDamage(target)
                            + ObjectManager.Player.GetAutoAttackDamage(target)))
                    {
                        if (!HasEnergy(new[] {SpellSlot.W, SpellSlot.R}))
                        {
                            return;
                        }

                        if (ShadowManager.WShadow.Exists)
                        {
                            CastQ(target);
                            CastE();
                        }
                        else
                        {
                            DoTriangleCombo(target);
                        }
                    }
                    else
                    {
                        DoNormalCombo(target);
                    }

                    break;
            }
        }

        /// <summary>
        ///     TODO The do line combo.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        private static void DoLineCombo(AIHeroClient target)
        {
            if (ShadowManager.RShadow.IsUsable)
            {
                if (getCheckBoxItem(miscMenu, "checkQWE"))
                {
                    if (_spells[SpellSlot.Q].IsReady() && _spells[SpellSlot.W].IsReady()
                        && _spells[SpellSlot.E].IsReady())
                    {
                        if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                        {
                            _spells[SpellSlot.R].Cast(target);
                        }
                    }
                }
                else
                {
                    if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                    {
                        _spells[SpellSlot.R].Cast(target);
                    }
                }
            }

            if (GetMarkedTarget() != null)
            {
                target = GetMarkedTarget();
            }

            if (ShadowManager.RShadow.Exists && ShadowManager.WShadow.IsUsable)
            {
                var wCastLocation = Player.ServerPosition
                                    - Vector3.Normalize(target.ServerPosition - Player.ServerPosition)*400;

                if (ShadowManager.WShadow.IsUsable && WShadowSpell.ToggleState == 0
                    && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                {
                    _spells[SpellSlot.W].Cast(wCastLocation);

                    // Maybe add a delay giving the target a chance to flash / zhonyas then it will place w at best location for more damage
                    _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                }
            }

            if (ShadowManager.WShadow.Exists && ShadowManager.RShadow.Exists)
            {
                CastQ(target);
                CastE();
            }
            else if (ShadowManager.RShadow.Exists && !ShadowManager.WShadow.IsUsable && !ShadowManager.WShadow.Exists)
            {
                CastQ(target);
                CastE();
            }

            if (ShadowManager.CanGoToShadow(ShadowManager.WShadow) && WShadowSpell.ToggleState == 2)
            {
                // && !_deathmarkKilled)
                if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.swapw")
                    && ShadowManager.WShadow.ShadowObject.Distance(target.ServerPosition)
                    < Player.Distance(target.ServerPosition))
                {
                    _spells[SpellSlot.W].Cast();
                }
            }
        }

        /// <summary>
        ///     TODO The do normal combo.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        private static void DoNormalCombo(AIHeroClient target)
        {
            if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.usew")
                && (_spells[SpellSlot.Q].IsReady() || _spells[SpellSlot.E].IsReady()))
            {
                CastW(target);
                if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.useq"))
                {
                    Utility.DelayAction.Add(105, () => CastQ(target));
                }

                if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.usee"))
                {
                    Utility.DelayAction.Add(105, CastE);
                }
            }
            else
            {
                CastQ(target);
                CastE();
            }
        }

        /// <summary>
        ///     TODO The do triangle combo.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        private static void DoTriangleCombo(AIHeroClient target)
        {
            // I'm dumb, this triangular combo is only good for targets the Zhonyas, we can still use it for that i guess :^)
            if (ShadowManager.RShadow.IsUsable && !target.HasBuffOfType(BuffType.Invulnerability))
            {
                // Cast Ultimate m8 :S
                if (getCheckBoxItem(miscMenu, "checkQWE"))
                {
                    if (_spells[SpellSlot.Q].IsReady() && _spells[SpellSlot.W].IsReady()
                        && _spells[SpellSlot.E].IsReady())
                    {
                        if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                        {
                            _spells[SpellSlot.R].Cast(target);
                        }
                    }
                }
                else
                {
                    if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(target))
                    {
                        _spells[SpellSlot.R].Cast(target);
                    }
                }
            }

            if (GetMarkedTarget() != null)
            {
                target = GetMarkedTarget();
            }

            if (ShadowManager.RShadow.Exists && ShadowManager.WShadow.IsUsable)
            {
                var bestWPosition = VectorHelper.GetBestPosition(
                    target,
                    VectorHelper.GetVertices(target)[0],
                    VectorHelper.GetVertices(target)[1]);

                // Maybe add a delay giving the target a chance to flash / zhonyas then it will place w at best perpendicular location m8
                if (WShadowSpell.ToggleState == 0 && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                {
                    _spells[SpellSlot.W].Cast(bestWPosition);

                    // Allow half a second for the target to flash / zhonyas? :S
                    _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                }
            }

            if (WShadowSpell.ToggleState == 2)
            {
                _spells[SpellSlot.W].Cast();
            }

            if (ShadowManager.WShadow.Exists && ShadowManager.RShadow.Exists)
            {
                CastQ(target);
                CastE();
            }
            else if (ShadowManager.RShadow.Exists && !ShadowManager.WShadow.IsUsable && !ShadowManager.WShadow.Exists)
            {
                CastQ(target);
                CastE();
            }
        }

        /// <summary>
        ///     TODO The drawing_ on draw.
        /// </summary>
        /// <param name="args">
        ///     TODO The args.
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawShadows"))
            {
                foreach (var shadow in
                    ShadowManager._shadowsList.Where(sh => sh.State != ShadowState.NotActive && sh.ShadowObject != null)
                    )
                {
                    Render.Circle.DrawCircle(shadow.Position, 60f, Color.Orange);
                }
            }

            foreach (
                var spell in
                    _spells.Where(
                        s => getCheckBoxItem(drawMenu, "com.idz.zed.drawing.draw" + GetStringFromSpellSlot(s.Key)))
                )
            {
                Render.Circle.DrawCircle(Player.Position, spell.Value.Range, Color.Aqua);
            }
        }

        /// <summary>
        ///     TODO The game_ on update.
        /// </summary>
        /// <param name="args">
        ///     TODO The args.
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            OnFlee();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Laneclear();
            }
        }

        /// <summary>
        ///     TODO The get assasination target.
        /// </summary>
        /// <param name="range">
        ///     TODO The range.
        /// </param>
        /// <param name="damageType">
        ///     TODO The damage type.
        /// </param>
        /// <returns>
        /// </returns>
        private static AIHeroClient GetAssasinationTarget(float range = 0, DamageType damageType = DamageType.Physical)
        {
            if (Math.Abs(range) < 0.00001)
            {
                range = _spells[SpellSlot.R].IsReady()
                    ? _spells[SpellSlot.R].Range
                    : _spells[SpellSlot.W].Range + _spells[SpellSlot.Q].Range/2f;
            }

            return TargetSelector.GetTarget(range, damageType);
        }

        /// <summary>
        ///     TODO The get hitchance.
        /// </summary>
        /// <returns>
        /// </returns>
        private static HitChance GetHitchance()
        {
            switch (getBoxItem(miscMenu, "com.idz.zed.misc.hitchance"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        /// <summary>
        ///     TODO The get marked target.
        /// </summary>
        /// <returns>
        /// </returns>
        private static AIHeroClient GetMarkedTarget()
        {
            return
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                        x.IsValidTarget(_spells[SpellSlot.W].Range + _spells[SpellSlot.Q].Range)
                        && x.HasBuff("zedulttargetmark") && x.IsVisible);
        }

        /// <summary>
        ///     TODO The get string from spell slot.
        /// </summary>
        /// <param name="sp">
        ///     TODO The sp.
        /// </param>
        /// <returns>
        /// </returns>
        private static string GetStringFromSpellSlot(SpellSlot sp)
        {
            switch (sp)
            {
                case SpellSlot.Q:
                    return "Q";
                case SpellSlot.W:
                    return "W";
                case SpellSlot.E:
                    return "E";
                case SpellSlot.R:
                    return "R";
                default:
                    return "unk";
            }
        }

        /// <summary>
        ///     TODO The harass.
        /// </summary>
        private static void Harass()
        {
            if (!getCheckBoxItem(harassMenu, "com.idz.zed.harass.useHarass"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(_spells[SpellSlot.W].Range + _spells[SpellSlot.Q].Range,
                DamageType.Physical);
            if (target == null)
            {
                return;
            }
            switch (getBoxItem(harassMenu, "com.idz.zed.harass.harassMode"))
            {
                case 0: // "W-E-Q"
                    if (_spells[SpellSlot.W].IsReady() && ShadowManager.WShadow.IsUsable
                        && WShadowSpell.ToggleState == 0
                        && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0
                        && Player.Distance(target) <= _spells[SpellSlot.W].Range + 300
                        && _spells[SpellSlot.Q].IsReady())
                    {
                        _spells[SpellSlot.W].Cast(target.ServerPosition);
                        _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                    }
                    else if ((!_spells[SpellSlot.W].IsReady() || WShadowSpell.ToggleState != 0) &&
                             _spells[SpellSlot.Q].IsReady() && target.Distance(Player) < _spells[SpellSlot.Q].Range)
                    {
                        if (getCheckBoxItem(harassMenu, "fast.harass"))
                        {
                            _spells[SpellSlot.Q].Cast(target.Position);
                        }
                        else
                        {
                            if (_spells[SpellSlot.Q].GetPrediction(target).Hitchance < HitChance.High)
                            {
                                _spells[SpellSlot.Q].Cast(target.Position);
                            }
                        }
                    }
                    else if (WShadowSpell.ToggleState != 0 && !_spells[SpellSlot.Q].IsReady() &&
                             _spells[SpellSlot.E].IsReady())
                    {
                        foreach (var shdw in ShadowManager._shadowsList.Where(x => x.Type == ShadowType.Normal))
                        {
                            _spells[SpellSlot.E].Cast();
                        }
                    }


                    break;
            }
        }

        /// <summary>
        ///     TODO The has energy.
        /// </summary>
        /// <param name="spells">
        ///     TODO The spells.
        /// </param>
        /// <returns>
        /// </returns>
        private static bool HasEnergy(IEnumerable<SpellSlot> spells)
        {
            if (!getCheckBoxItem(miscMenu, "energyManagement"))
            {
                return true;
            }

            var totalCost = spells.Sum(slot => Player.Spellbook.GetSpell(slot).SData.Mana);
            return Player.Mana >= totalCost;
        }

        /// <summary>
        ///     TODO The init events.
        /// </summary>
        private static void InitEvents()
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += OnCreateObject;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
        }

        /// <summary>
        ///     TODO The init menu.
        /// </summary>
        public static Menu comboMenu, harassMenu, lastHitMenu, laneclearMenu, drawMenu, fleeMenu, miscMenu;


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

        private static void InitMenu()
        {
            Menu = MainMenu.AddMenu("iDZed - Reworked", "com.idz.zed");

            comboMenu = Menu.AddSubMenu(":: Combo", "com.idz.zed.combo");
            comboMenu.Add("com.idz.zed.combo.useq", new CheckBox("Use Q"));
            comboMenu.Add("com.idz.zed.combo.useqpred", new CheckBox("Q Prediction: On = slower, off = faster", false));
            comboMenu.Add("com.idz.zed.combo.usew", new CheckBox("Use W"));
            comboMenu.Add("com.idz.zed.combo.usee", new CheckBox("Use E"));
            comboMenu.Add("com.idz.zed.combo.user", new CheckBox("Use R"));
            comboMenu.Add("com.idz.zed.combo.swapw", new CheckBox("Swap W For Follow", false));
            comboMenu.Add("com.idz.zed.combo.swapr", new CheckBox("Swap R On kill"));
            comboMenu.Add("com.idz.zed.combo.mode", new ComboBox("Combo Mode", 0, "Line Mode", "Triangle Mode"));

            harassMenu = Menu.AddSubMenu(":: Harass", "com.idz.zed.harass");
            harassMenu.Add("com.idz.zed.harass.useHarass", new CheckBox("Use Harass"));
            harassMenu.Add("fast.harass", new CheckBox("Q Prediction: On = slower, off = faster", false));
            harassMenu.Add("com.idz.zed.harass.harassMode", new ComboBox("Harass Mode", 0, "W-E-Q"));

            lastHitMenu = Menu.AddSubMenu(":: LastHit", "com.idz.zed.lasthit");
            lastHitMenu.Add("com.idz.zed.lasthit.useQ", new CheckBox("Use Q in LastHit"));
            lastHitMenu.Add("com.idz.zed.lasthit.useE", new CheckBox("Use E in LastHit"));

            laneclearMenu = Menu.AddSubMenu(":: Laneclear", "com.idz.zed.laneclear");
            laneclearMenu.Add("com.idz.zed.laneclear.useQ", new CheckBox("Use Q in laneclear"));
            laneclearMenu.Add("com.idz.zed.laneclear.qhit", new Slider("Min minions for Q", 3, 1, 10));
            laneclearMenu.Add("com.idz.zed.laneclear.useE", new CheckBox("Use E in laneclear"));
            laneclearMenu.Add("com.idz.zed.laneclear.ehit", new Slider("Min minions for E", 3, 1, 10));

            drawMenu = Menu.AddSubMenu(":: Drawing", "com.idz.zed.drawing");
            foreach (var slot in _spells.Select(entry => entry.Key))
            {
                drawMenu.Add("com.idz.zed.drawing.draw" + GetStringFromSpellSlot(slot),
                    new CheckBox("Draw " + GetStringFromSpellSlot(slot) + " Range"));
            }
            drawMenu.Add("drawShadows", new CheckBox("Draw Shadows"));

            fleeMenu = Menu.AddSubMenu(":: Flee", "com.idz.zed.flee");
            fleeMenu.Add("fleeActive", new KeyBind("Flee Key", false, KeyBind.BindTypes.PressToggle, 'P'));
            fleeMenu.Add("autoEFlee", new CheckBox("Auto E when fleeing"));

            miscMenu = Menu.AddSubMenu(":: Misc", "com.idz.zed.misc");
            miscMenu.Add("energyManagement", new CheckBox("Use Energy Management"));
            miscMenu.Add("safetyChecks", new CheckBox("Check Safety for shadow swapping"));
            miscMenu.Add("com.idz.zed.misc.hitchance",
                new ComboBox("Q Hitchance", 2, "Low", "Medium", "High", "Very High"));
            miscMenu.Add("checkQWE", new CheckBox("Check Other Spells before ult"));
        }

        /// <summary>
        ///     TODO The init spells.
        /// </summary>
        private static void InitSpells()
        {
            _spells[SpellSlot.Q].SetSkillshot(0.25F, 50F, 1600F, false, SkillshotType.SkillshotLine);
            _spells[SpellSlot.W].SetSkillshot(0.75F, 75F, 1000F, false, SkillshotType.SkillshotCircle);
            //_spells[SpellSlot.E].SetSkillshot(0f, 220f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        /// <summary>
        ///     TODO The is pass wall.
        /// </summary>
        /// <param name="start">
        ///     TODO The start.
        /// </param>
        /// <param name="end">
        ///     TODO The end.
        /// </param>
        /// <returns>
        /// </returns>
        private static bool IsPassWall(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);
            for (uint i = 0; i <= count; i += 25)
            {
                var pos = start.To2D().Extend(Player.ServerPosition.To2D(), -i);
                if (pos.IsWall())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     TODO The laneclear.
        /// </summary>
        private static void Laneclear()
        {
            var allMinionsQ = MinionManager.GetMinions(
                Player.ServerPosition,
                _spells[SpellSlot.Q].Range,
                MinionTypes.All,
                MinionTeam.NotAlly);
            var allMinionsE = MinionManager.GetMinions(
                Player.ServerPosition,
                _spells[SpellSlot.Q].Range,
                MinionTypes.All,
                MinionTeam.NotAlly);
            if (getCheckBoxItem(laneclearMenu, "com.idz.zed.laneclear.useQ") && _spells[SpellSlot.Q].IsReady()
                && !Orbwalker.IsAutoAttacking)
            {
                var bestPositionQ =
                    MinionManager.GetBestLineFarmLocation(
                        allMinionsQ.Select(x => x.ServerPosition.To2D()).ToList(),
                        _spells[SpellSlot.Q].Width,
                        _spells[SpellSlot.Q].Range);
                if (bestPositionQ.MinionsHit >= getSliderItem(laneclearMenu, "com.idz.zed.laneclear.qhit"))
                {
                    _spells[SpellSlot.Q].Cast(bestPositionQ.Position);
                }
            }

            if (getCheckBoxItem(laneclearMenu, "com.idz.zed.laneclear.useE") && _spells[SpellSlot.E].IsReady()
                && !Orbwalker.IsAutoAttacking)
            {
                var eLocation =
                    MinionManager.GetBestLineFarmLocation(
                        allMinionsE.Select(x => x.ServerPosition.To2D()).ToList(),
                        _spells[SpellSlot.E].Width,
                        _spells[SpellSlot.E].Range);
                if (eLocation.MinionsHit >= getSliderItem(laneclearMenu, "com.idz.zed.laneclear.ehit"))
                {
                    _spells[SpellSlot.E].Cast();
                }
            }
        }

        /// <summary>
        ///     TODO The last hit.
        /// </summary>
        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(Player.ServerPosition, 1000f, MinionTypes.All, MinionTeam.NotAlly);
            if (getCheckBoxItem(lastHitMenu, "com.idz.zed.lasthit.useQ") && _spells[SpellSlot.Q].IsReady())
            {
                var qMinion =
                    allMinions.FirstOrDefault(
                        x => _spells[SpellSlot.Q].IsInRange(x) && x.IsValidTarget(_spells[SpellSlot.Q].Range));

                if (qMinion != null && _spells[SpellSlot.Q].GetDamage(qMinion) > qMinion.Health
                    && !Orbwalking.InAutoAttackRange(qMinion))
                {
                    _spells[SpellSlot.Q].Cast(qMinion);
                }
            }

            if (getCheckBoxItem(lastHitMenu, "com.idz.zed.lasthit.useE") && _spells[SpellSlot.E].IsReady())
            {
                var minions =
                    MinionManager.GetMinions(
                        Player.ServerPosition,
                        _spells[SpellSlot.E].Range,
                        MinionTypes.All,
                        MinionTeam.NotAlly)
                        .FindAll(
                            minion =>
                                !Orbwalking.InAutoAttackRange(minion)
                                && minion.Health < 0.75*_spells[SpellSlot.E].GetDamage(minion));
                if (minions.Count >= 1)
                {
                    _spells[SpellSlot.E].Cast();
                }
            }
        }

        /// <summary>
        ///     TODO The on create object.
        /// </summary>
        /// <param name="sender">
        ///     TODO The sender.
        /// </param>
        /// <param name="args">
        ///     TODO The args.
        /// </param>
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter))
            {
            }

            if (getCheckBoxItem(comboMenu, "com.idz.zed.combo.swapr"))
            {
                if (sender.Name == "Zed_Base_R_buf_tell.troy")
                {
                    // _deathmarkKilled = true;
                    if (RShadowSpell.ToggleState == 2 && ShadowManager.CanGoToShadow(ShadowManager.RShadow))
                    {
                        _spells[SpellSlot.R].Cast();
                    }
                }
            }
        }

        /// <summary>
        ///     TODO The on flee.
        /// </summary>
        private static void OnFlee()
        {
            if (!getKeyBindItem(fleeMenu, "fleeActive") ||
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (_spells[SpellSlot.W].IsReady() && ShadowManager.WShadow.IsUsable)
            {
                _spells[SpellSlot.W].Cast(Game.CursorPos);
            }

            if (ShadowManager.WShadow.Exists && ShadowManager.CanGoToShadow(ShadowManager.WShadow))
            {
                _spells[SpellSlot.W].Cast();
            }

            CastE();
        }

        /// <summary>
        ///     TODO The on spell cast.
        /// </summary>
        /// <param name="sender1">
        ///     TODO The sender 1.
        /// </param>
        /// <param name="args">
        ///     TODO The args.
        /// </param>
        private static void OnSpellCast(Obj_AI_Base sender1, GameObjectProcessSpellCastEventArgs args)
        {
            var sender = sender1 as AIHeroClient;
            if (sender != null && sender.IsEnemy && sender.Team != Player.Team)
            {
                if (args.SData.Name == "ZhonyasHourglass" && sender.HasBuff("zedulttargetmark")
                    && Player.Distance(sender, true) < _spells[SpellSlot.W].Range - 20*_spells[SpellSlot.W].Range - 20)
                {
                    var bestPosition = VectorHelper.GetBestPosition(
                        sender,
                        VectorHelper.GetVertices(sender, true)[0],
                        VectorHelper.GetVertices(sender, true)[1]);
                    if (_spells[SpellSlot.W].IsReady() && WShadowSpell.ToggleState == 0
                        && Environment.TickCount - _spells[SpellSlot.W].LastCastAttemptT > 0)
                    {
                        _spells[SpellSlot.W].Cast(bestPosition);
                        _spells[SpellSlot.W].LastCastAttemptT = Environment.TickCount + 500;
                    }
                }
            }
        }

        #endregion
    }
}