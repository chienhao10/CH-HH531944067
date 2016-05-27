using System.Reflection;
using AutoBuddy.Humanizers;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AutoBuddy.Utilities.AutoShop
{
    internal class BuildElement
    {
        public readonly ShopActionType action;
        private readonly BuildCreator bc;
        public readonly LoLItem item;
        private readonly PropertyInfo property;
        public int cost;
        private Label costSlots;
        public int freeSlots;
        private Label itemName;
        public int p;
        private CheckBox removeBox;
        private CheckBox upBox;

        public BuildElement(BuildCreator bc, Menu menu, LoLItem item, int index, ShopActionType action)
        {
            this.action = action;
            this.bc = bc;
            this.item = item;
            p = index;

            upBox = new CheckBox("up", false);
            removeBox = new CheckBox("remove", false);
            itemName = new Label(" ");
            costSlots = new Label(" ");

            PropertyInfo property2 = typeof (CheckBox).GetProperty("Size");

            property2.GetSetMethod(true).Invoke(itemName, new object[] {new Vector2(400, 0)});
            property2.GetSetMethod(true).Invoke(costSlots, new object[] {new Vector2(400, 0)});
            property2.GetSetMethod(true).Invoke(upBox, new object[] {new Vector2(40, 20)});
            property2.GetSetMethod(true).Invoke(removeBox, new object[] {new Vector2(80, 20)});


            menu.Add(position + "nam" + RandGen.r.Next(), itemName);
            menu.Add(position + "cs" + RandGen.r.Next(), costSlots);
            menu.Add(position + "up" + RandGen.r.Next(), upBox);
            menu.Add(position + "rem" + RandGen.r.Next(), removeBox);
            updateText();

            upBox.CurrentValue = false;
            removeBox.CurrentValue = false;
            upBox.OnValueChange += upBox_OnValueChange;
            removeBox.OnValueChange += removeBox_OnValueChange;
            property = typeof (CheckBox).GetProperty("Position");
        }

        public int position
        {
            get { return p; }
            set
            {
                p = value;
                updateText();
            }
        }


        public void updateText()
        {
            if (action == ShopActionType.Buy || action == ShopActionType.Sell)
            {
                itemName.CurrentValue = p.ToString().PadLeft(2, ' ') + ")" + action.ToString().PadLeft(6, ' ') + "    " +
                                        item.name;
                costSlots.CurrentValue = "Free slots: " + freeSlots + "      Cost: " + cost.ToString().PadLeft(4, ' ');
            }
            else
            {
                itemName.CurrentValue = p.ToString().PadLeft(2, ' ') + ")   " + action;
                costSlots.CurrentValue = "Free slots: " + freeSlots;
            }
        }

        private void removeBox_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.NewValue) return;
            if (!bc.Remove(p))
                Core.DelayAction(() => { removeBox.CurrentValue = false; }, 1);
        }


        private void upBox_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.NewValue) return;
            bc.MoveUp(p);
            Core.DelayAction(() => { upBox.CurrentValue = false; }, 1);
        }


        public void UpdatePos(Vector2 basePos)
        {
            property.GetSetMethod(true).Invoke(itemName, new object[] {basePos + new Vector2(0, p*20)});
            property.GetSetMethod(true).Invoke(costSlots, new object[] {basePos + new Vector2(250, p*20)});
            property.GetSetMethod(true).Invoke(upBox, new object[] {basePos + new Vector2(440, p*20 - 12)});
            property.GetSetMethod(true).Invoke(removeBox, new object[] {basePos + new Vector2(490, p*20 - 12)});
        }


        public void Remove(Menu menu)
        {
            menu.Remove(itemName);
            menu.Remove(costSlots);
            menu.Remove(removeBox);
            menu.Remove(upBox);
            costSlots = null;
            itemName = null;
            removeBox = null;
            upBox = null;
        }
    }
}