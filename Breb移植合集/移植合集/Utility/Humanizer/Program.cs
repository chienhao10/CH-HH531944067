#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace Humanizer
{
    public class Program
    {
        public static Menu Menu;
        public static int LastMove;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Dictionary<SpellSlot, int> LastCast = new Dictionary<SpellSlot, int>();
        public static Render.Text BlockedMovement;
        public static Render.Text BlockedSpells;
        public static int BlockedSpellCount;
        public static int BlockedMoveCount;
        public static int NextMovementDelay;
        public static Vector3 LastMovementPosition = Vector3.Zero;

        public static List<SpellSlot> Items = new List<SpellSlot>
        {
            SpellSlot.Item1,
            SpellSlot.Item2,
            SpellSlot.Item3,
            SpellSlot.Item4,
            SpellSlot.Item5,
            SpellSlot.Item6,
            SpellSlot.Trinket
        };

        public static Menu spells, move;

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
            Menu = MainMenu.AddMenu("Humanizer", "Humanizer");

            spells = Menu.AddSubMenu("Spells", "Spells");
            foreach (var spell in Items)
            {
                spells.AddGroupLabel(spell.ToString());
                spells.Add("Enabled" + spell, new CheckBox("Delay " + spell));
                spells.Add("MinDelay" + spell, new Slider("Minimum Delay", 80));
                spells.Add("MaxDelay" + spell, new Slider("Maximum Delay", 200, 100, 400));
                LastCast.Add(spell, 0);
                spells.AddSeparator();
            }
            spells.Add("DrawSpells", new CheckBox("Draw Blocked Spell Count"));

            move = Menu.AddSubMenu("Movement", "Movement");
            move.Add("MovementEnabled", new CheckBox("Enabled"));
            move.Add("MovementHumanizeDistance", new CheckBox("Humanize Movement Distance"));
            move.Add("MovementHumanizeRate", new CheckBox("Humanize Movement Rate"));
            move.Add("MinDelay", new Slider("Minimum Delay", 80));
            move.Add("MaxDelay", new Slider("Maximum Delay", 200, 100, 400));
            move.Add("DrawMove", new CheckBox("Draw Blocked Movement Count"));

            BlockedSpells = new Render.Text("Blocked Spells: ", Drawing.Width - 200, Drawing.Height - 600, 28, Color.Green);
            BlockedSpells.VisibleCondition += sender => getCheckBoxItem(spells, "DrawSpells");
            BlockedSpells.TextUpdate += () => "Blocked Spells: " + BlockedSpellCount;
            BlockedSpells.Add();

            BlockedMovement = new Render.Text("Blocked Move: ", Drawing.Width - 200, Drawing.Height - 625, 28, Color.Green);
            BlockedMovement.VisibleCondition += sender => getCheckBoxItem(move, "DrawMove");
            BlockedMovement.TextUpdate += () => "Blocked Move: " + BlockedMoveCount;
            BlockedMovement.Add();

            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var spell = args.Slot;
            var senderValid = sender != null && sender.Owner != null && sender.Owner.IsMe;

            if (!senderValid || !Items.Contains(spell) || !getCheckBoxItem(spells, "Enabled" + spell))
            {
                return;
            }

            var min = getSliderItem(spells, "MinDelay" + spell);
            var max = getSliderItem(spells, "MaxDelay" + spell);
            var delay = min >= max ? min : WeightedRandom.Next(min, max);

            if (LastCast[spell].TimeSince() < delay)
            {
                BlockedSpellCount++;
                args.Process = false;
                return;
            }

            LastCast[spell] = Utils.TickCount;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            var senderValid = sender != null && sender.IsValid && sender.IsMe;

            if (!senderValid || args.Order != GameObjectOrder.MoveTo || !getCheckBoxItem(move, "MovementEnabled"))
            {
                return;
            }
            if (LastMovementPosition != Vector3.Zero && args.TargetPosition.LSDistance(LastMovementPosition) < 300)
            {
                if (NextMovementDelay == 0)
                {
                    var min = getSliderItem(move, "MinDelay");
                    var max = getSliderItem(move, "MaxDelay");
                    NextMovementDelay = min > max ? min : WeightedRandom.Next(min, max);
                }

                if (getCheckBoxItem(move, "MovementHumanizeRate") && LastMove.TimeSince() < NextMovementDelay)
                {
                    NextMovementDelay = 0;
                    BlockedMoveCount++;
                    args.Process = false;
                    return;
                }

                if (getCheckBoxItem(move, "MovementHumanizeDistance"))
                {
                    var wp = ObjectManager.Player.GetWaypoints();
                    if (args.TargetPosition.LSDistance(Player.ServerPosition) < 50)
                    {
                        BlockedMoveCount++;
                        args.Process = false;
                        return;
                    }
                }
            }

            LastMovementPosition = args.TargetPosition;
            LastMove = Utils.TickCount;
        }
    }
}