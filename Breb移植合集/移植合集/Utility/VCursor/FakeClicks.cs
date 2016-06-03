#region LICENSE

/*
 Copyright 2014 - 2014 LeagueSharp
 FakeClicks.cs is part of LeagueSharp.Common.
 
 LeagueSharp.Common is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 LeagueSharp.Common is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with LeagueSharp.Common. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace VCursor
{
    /// <summary>
    ///     Simulates clicks.
    /// </summary>
    internal static class FakeClicks
    {
        #region Static Fields

        /// <summary>
        ///     Gets a value indicating whether this <see cref="FakeClicks" /> is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool Enabled
        {
            get { return root["Enable"].Cast<CheckBox>().CurrentValue; }
        }

        /// <summary>
        ///     If the user is attacking
        ///     Currently used for the second style of fake clicks
        /// </summary>
        private static bool attacking;

        /// <summary>
        ///     The delta t for click frequency
        /// </summary>
        private static readonly float deltaT = 0.15f;

        /// <summary>
        ///     The last direction of the player
        /// </summary>
        /// <summary>
        ///     The last endpoint the player was moving to.
        /// </summary>
        private static Vector3 lastEndpoint;

        /// <summary>
        ///     The last order the player had.
        /// </summary>
        /// <summary>
        ///     The time of the last order the player had.
        /// </summary>
        private static float lastOrderTime;

        /// <summary>
        ///     The last time a click was done.
        /// </summary>
        private static float lastTime;

        /// <summary>
        ///     The Player.
        /// </summary>
        private static AIHeroClient player;

        /// <summary>
        ///     The Random number generator
        /// </summary>
        private static readonly Random r = new Random(Utils.TickCount);

        /// <summary>
        ///     The root menu.
        /// </summary>
        private static Menu root;

        private static bool MoveCursor => Program.FollowMovement;

        #endregion

        #region Methods

        /// <summary>
        ///     The move fake click after attacking
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            attacking = false;
            var t = target as AIHeroClient;
            if (t != null)
            {
                var pos = RandomizePosition(t.Position);
                if (MoveCursor)
                {
                    MouseManager.StartPathWorld(pos);
                }
                ShowClick(pos, ClickType.Move);
            }
        }

        /// <summary>
        ///     The before attack fake click.
        ///     Currently used for the second style of fake clicks
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void BeforeAttackFake(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (root["ClickMode"].Cast<ComboBox>().CurrentValue == 1)
            {
                var pos = RandomizePosition(args.Target.Position);
                if (MoveCursor)
                {
                    MouseManager.StartPathWorld(pos);
                }
                ShowClick(pos, ClickType.Attack);
                attacking = true;
            }
        }

        /// <summary>
        ///     The fake click before you cast a spell
        /// </summary>
        /// <param name="s">
        ///     The Spell Book.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void BeforeSpellCast(Spellbook s, SpellbookCastSpellEventArgs args)
        {
            var target = args.Target;

            if (target == null)
            {
                return;
            }

            if (target.Position.LSDistance(player.Position) >= 5f)
            {
                if (MoveCursor)
                {
                    MouseManager.StartPathWorld(args.Target.Position);
                }
                ShowClick(args.Target.Position, ClickType.Attack);
            }
        }

        /// <summary>
        ///     The on new path fake.
        ///     Currently used for the second style of fake clicks
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void DrawFake(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe)
            {
                if (lastTime + deltaT < Game.Time && args.Path.LastOrDefault() != lastEndpoint && args.Path.LastOrDefault().LSDistance(player.ServerPosition) >= 5f && root["Enable"].Cast<CheckBox>().CurrentValue && root["ClickMode"].Cast<ComboBox>().CurrentValue == 1)
                {
                    lastEndpoint = args.Path.LastOrDefault();
                    if (!attacking)
                    {
                        ShowClick(Game.CursorPos, ClickType.Move);
                    }
                    else
                    {
                        ShowClick(Game.CursorPos, ClickType.Attack);
                    }

                    lastTime = Game.Time;
                }
            }
        }

        /// <summary>
        ///     The OnIssueOrder event delegate.
        ///     Currently used for the first style of fake clicks
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe &&
                (args.Order == GameObjectOrder.MoveTo || args.Order == GameObjectOrder.AttackUnit ||
                 args.Order == GameObjectOrder.AttackTo) &&
                lastOrderTime + r.NextFloat(deltaT, deltaT + .2f) < Game.Time && root["Enable"].Cast<CheckBox>().CurrentValue &&
                root["ClickMode"].Cast<ComboBox>().CurrentValue == 0)
            {
                var vect = args.TargetPosition;
                vect.Z = player.Position.Z;
                if (args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                {
                    var pos = RandomizePosition(vect);
                    if (MoveCursor)
                    {
                        MouseManager.StartPathWorld(pos);
                    }
                    ShowClick(pos, ClickType.Attack);
                }
                else
                {
                    if (MoveCursor)
                    {
                        MouseManager.StartPathWorld(vect);
                    }
                    ShowClick(vect, ClickType.Move);
                }

                lastOrderTime = Game.Time;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize(Menu menu)
        {
            root = menu.AddSubMenu("FakeClicks", "FakeClicks");
            root.Add("Enable", new CheckBox("Enable", false));
            root.Add("ClickMode", new ComboBox("Click Mode", 0, "Evade, No Cursor Position", "Cursor Position, No Evade"));

            player = ObjectManager.Player;

            Obj_AI_Base.OnNewPath += DrawFake;
            Spellbook.OnCastSpell += BeforeSpellCast;
            Orbwalker.OnPreAttack += BeforeAttackFake;
            Orbwalker.OnPostAttack += AfterAttack;
            Player.OnIssueOrder += OnIssueOrder;
        }

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnGameLoad(EventArgs args) {}

        /// <summary>
        ///     Shows the click.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="type">The type.</param>
        private static void ShowClick(Vector3 position, ClickType type)
        {
            if (!Enabled)
            {
                return;
            }

            Hud.ShowClick(type, position);
        }

        /// <summary>
        ///     The RandomizePosition function to randomize click location.
        /// </summary>
        /// <param name="input">
        ///     The input Vector3.
        /// </param>
        /// <returns>
        ///     A Vector within 100 units of the unit
        /// </returns>
        private static Vector3 RandomizePosition(Vector3 input)
        {
            if (r.Next(2) == 0)
            {
                input.X += r.Next(100);
            }
            else
            {
                input.Y += r.Next(100);
            }

            return input;
        }

        #endregion
    }
}