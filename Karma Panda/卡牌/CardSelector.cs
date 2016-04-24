using System;

namespace TwistedBuddy
{
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    public enum Cards
    {
        Red,
        Yellow,
        Blue,
        None,
    }

    public enum SelectStatus
    {
        Ready,
        Selecting,
        Selected,
        Cooldown,
    }

    internal class CardSelector
    {
        public static Cards SelectedCard;
        public static int LastW;
        public static SelectStatus Status { get; set; }

        public static int Delay => Essentials.MiscMenu["delay"].Cast<CheckBox>().CurrentValue ? 500 : new Random().Next(125, 250);

        static CardSelector()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        public static void StartSelecting(Cards card)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && Status == SelectStatus.Ready)
            {
                SelectedCard = card;
                if (Environment.TickCount - LastW > 170 + Game.Ping / 2)
                {
                    Program.W.Cast();
                    LastW = Environment.TickCount;
                }
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name == "PickACard")
            {
                Status = SelectStatus.Selecting;
            }

            if (args.SData.Name.ToLower() == "goldcardlock" || args.SData.Name.ToLower() == "bluecardlock" ||
                args.SData.Name.ToLower() == "redcardlock")
            {
                Status = SelectStatus.Selected;
                SelectedCard = Cards.None;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var wName = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name;
            var wState = ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.W);

            if ((wState == SpellState.Ready &&
                 wName == "PickACard" &&
                 (Status != SelectStatus.Selecting || Environment.TickCount - LastW > 500)) ||
                ObjectManager.Player.IsDead)
            {
                Status = SelectStatus.Ready;
            }
            else if (wState == SpellState.Cooldown &&
                     wName == "PickACard")
            {
                SelectedCard = Cards.None;
                Status = SelectStatus.Cooldown;
            }
            else if (wState == SpellState.Surpressed &&
                     !ObjectManager.Player.IsDead)
            {
                Status = SelectStatus.Selected;
            }

            if (SelectedCard == Cards.Blue && wName.ToLower() == "bluecardlock" && Environment.TickCount - Delay > LastW)
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
            }
            else if (SelectedCard == Cards.Yellow && wName.ToLower() == "goldcardlock" && Environment.TickCount - Delay > LastW)
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
            }
            else if (SelectedCard == Cards.Red && wName.ToLower() == "redcardlock" && Environment.TickCount - Delay > LastW)
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
            }
        }
    }
}