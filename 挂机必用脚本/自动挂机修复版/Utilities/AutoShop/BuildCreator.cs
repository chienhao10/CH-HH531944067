using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using EloBuddy.Sandbox;

namespace AutoBuddy.Utilities.AutoShop
{
    internal class BuildCreator
    {
        private readonly string buildFile;
        private readonly CheckBox enabled;
        private readonly Label l;
        private readonly Menu menu;

        private readonly List<BuildElement> myBuild;
        private readonly PropertyInfo property;
        private readonly EasyShopV2 shop;
        private readonly string sugBuild;

        private readonly CheckBox toDefault;

        public BuildCreator(Menu parentMenu, string dir, string build="")
        {
            sugBuild = build;
            property = typeof(CheckBox).GetProperty("Position");


            string specialPath = null;
         
           specialPath = SandboxConfig.DataDirectory + "AutoBuddy\\Builds\\";
           

            // OLD buildFile = Path.Combine(specialPath + "\\" + AutoWalker.p.ChampionName + "-" + Game.MapId + ".txt");
            buildFile = Path.Combine(specialPath + AutoWalker.p.ChampionName + "-" + Game.MapId + ".txt");
            l = new Label("Shopping list for " + Game.MapId);
            enabled = new CheckBox("Auto buy enabled", true);
            myBuild = new List<BuildElement>();

            menu = parentMenu.AddSubMenu("AutoShop: " + AutoWalker.p.ChampionName, "AB_SHOP_" + AutoWalker.p.ChampionName);
            menu.Add("eeewgrververv", l);
            menu.Add(AutoWalker.p.ChampionName + "enabled", enabled);
            LoadBuild();
            shop = new EasyShopV2(myBuild, enabled);







            Menu info = parentMenu.AddSubMenu("Shop-instructions");
            toDefault=new CheckBox("Delete custom build and set default ADC build", false);

            PropertyInfo property2 = typeof(CheckBox).GetProperty("Size");

            property2.GetSetMethod(true).Invoke(toDefault, new object[] { new Vector2(400, 25) });
            info.Add("defbuild", toDefault);
            info.AddSeparator(150);
            info.AddLabel(
                @"



:) Updated by TheYasuoMain :)

Commands(type them in the chat):

/b itemName  :buy an item, you don't need to type exact name for the item, just few first
characters, for example for BT its enough: /b thebloodt

/s itemName : sell an item

/buyhp:keep buying 1 hp potion(if not in champ's inventory already)
/stophp : stop buying hp potion and sell if any is owned.

AUTOSHOP WILL STOP WORKING IF FINDS ANY ITEMS IN INVENTORY THAT AREN'T
IN THE SEQUENCE.

Don't add to the list items that you can't buy, for example jungle items without smite.

Autoshop will stop if you have items that are not listed, so it's recommended
to sell whole inventory after changing list.

Builds are saved in C:\Users\Username\AppData\Roaming\EloBuddy\AutoBuddy\Builds
you can copy/share them.

            ");






            toDefault.OnValueChange += toDefault_OnValueChange;
            Chat.OnInput += Chat_OnInput;
            Drawing.OnEndScene += Drawing_OnEndScene;

        }

        private void toDefault_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.NewValue) return;

            
            Core.DelayAction(() => { toDefault.CurrentValue = false; }, 200);
            Reset();
            LoadBuild();
        }

        private void Reset()
        {
            if(File.Exists(buildFile))
                File.Delete(buildFile);
            foreach (BuildElement buildElement in myBuild)
            {
                buildElement.Remove(menu);
            }
            myBuild.Clear();
        }

        private void AddElement(LoLItem it, ShopActionType ty)
        {
            if (ty != ShopActionType.Buy || ty != ShopActionType.Sell)
            {
                int hp = myBuild.Count(e => e.action == ShopActionType.StartHpPot) -
                         myBuild.Count(e => e.action == ShopActionType.StopHpPot);
                if (ty == ShopActionType.StartHpPot && hp != 0) return;
                if (ty == ShopActionType.StopHpPot && hp == 0) return;
            }

            BuildElement b = new BuildElement(this, menu, it, myBuild.Any() ? myBuild.Max(a => a.position) + 1 : 1, ty);

            List<LoLItem> c = new List<LoLItem>();
            BrutalItemInfo.InventorySimulator(myBuild, c);
            b.cost = BrutalItemInfo.InventorySimulator(new List<BuildElement> { b }, c);
            b.freeSlots = 7 - c.Count;
            b.updateText();
            if (b.freeSlots == -1)
            {
                Chat.Print("Couldn't add " + it + ", inventory is full.");
                b.Remove(menu);
            }
            else
                myBuild.Add(b);
        }

        private void LoadBuild()
        {
           if (!File.Exists(buildFile))
              
            {
                Chat.Print("Custom build doesn't exist: " + buildFile);
                if (!sugBuild.Equals(string.Empty))
                {
                    LoadInternalBuild();
                }
                return;
            }
            try
            {
                string s = File.ReadAllText(buildFile);
                if (s.Equals(string.Empty))
                {
                    Chat.Print("AutoBuddy: the build is empty.");
                    LoadInternalBuild();
                    return;
                }
                foreach (ItemAction ac in DeserializeBuild(s))
                {
                    AddElement(BrutalItemInfo.GetItemByID(ac.item), ac.t);
                    Console.Write("Custom Build Loading ");
                }
                Chat.Print("Loaded build from: " + buildFile);
            }
            catch (Exception e)
            {
                Chat.Print("AutoBuddy: couldn't load the build.");
              
                LoadInternalBuild();
                Console.WriteLine(e.Message);
            }

        }

        private void LoadInternalBuild()
        {
            try
            {
                if (sugBuild.Equals(string.Empty))
                {
                    Chat.Print("AutoBuddy: internal build is empty.");
                    return;
                }
                foreach (ItemAction ac in DeserializeBuild(sugBuild))
                {
                    AddElement(BrutalItemInfo.GetItemByID(ac.item), ac.t);
                }
            }
            catch (Exception e)
            {
                Chat.Print("AutoBuddy: internal build load failed.");
                Console.WriteLine(e.Message);
            }
            Chat.Print("AutoBuddy: Internal build loaded.");
        }

        private void SaveBuild()
        {
            File.WriteAllText(buildFile, SerializeBuild());
        }

        private string SerializeBuild()
        {
            string s = string.Empty;
            foreach (BuildElement el in myBuild.OrderBy(el => el.position))
            {
                s += el.item.id + ":" + el.action + ",";
            }
            return s.Equals(string.Empty) ? s : s.Substring(0, s.Length - 1);
        }

        private IEnumerable<ItemAction> DeserializeBuild(string serialized)
        {
            List<ItemAction> b = new List<ItemAction>();
            foreach (string s in serialized.Split(','))
            {
                ItemAction ac = new ItemAction { item = -1 };
                foreach (string s2 in s.Split(':'))
                {
                    if (ac.item == -1)
                        ac.item = int.Parse(s2);
                    else
                        ac.t = (ShopActionType)Enum.Parse(typeof(ShopActionType), s2, true);
                }
                b.Add(ac);
            }
            return b;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!MainMenu.IsVisible) return;
            property.GetSetMethod(true).Invoke(enabled, new object[] { l.Position + new Vector2(433, 0) });
            foreach (BuildElement ele in myBuild)
            {
                ele.UpdatePos(new Vector2(l.Position.X, l.Position.Y + 10));
            }
        }

        public void MoveUp(int index)
        {
            if (index <= 2) return;
            BuildElement th = myBuild.First(ele => ele.position == index);
            BuildElement up = myBuild.First(ele => ele.position == index - 1);
            th.position--;
            up.position++;

            foreach (BuildElement el in myBuild.OrderBy(b => b.position))
            {
                List<LoLItem> c = new List<LoLItem>();
                BrutalItemInfo.InventorySimulator(myBuild, c, el.position - 1);
                el.cost = BrutalItemInfo.InventorySimulator(new List<BuildElement> { el }, c);
                el.freeSlots = 7 - c.Count;
                el.updateText();
            }
            SaveBuild();
        }

        public void MoveDown(int index)
        {
            if (index == myBuild.Count || index == 2) return;
            BuildElement th = myBuild.First(ele => ele.position == index);
            BuildElement dn = myBuild.First(ele => ele.position == index + 1);
            th.position++;
            dn.position--;

            SaveBuild();
        }

        public bool Remove(int index)
        {
            if (myBuild.Count > 1 && index == 1) return false;
            BuildElement th = myBuild.First(ele => ele.position == index);
            myBuild.Remove(th);
            th.Remove(menu);
            foreach (BuildElement el in myBuild.OrderBy(b => b.position).Where(b => b.position > index))
            {
                el.position--;


                List<LoLItem> c = new List<LoLItem>();
                BrutalItemInfo.InventorySimulator(myBuild, c, el.position - 1);
                el.cost = BrutalItemInfo.InventorySimulator(new List<BuildElement> { el }, c);
                el.freeSlots = 7 - c.Count;
                el.updateText();
            }


            SaveBuild();
            return true;
        }

        private void Chat_OnInput(ChatInputEventArgs args)
        {
            if (args.Input.ToLower().StartsWith("/b "))
            {
                args.Process = false;
                string itemName = args.Input.Substring(2);
                LoLItem i = BrutalItemInfo.FindBestItem(itemName);
                Chat.Print("Buy " + i.name);

                if (myBuild.Count == 0 && !i.groups.Equals("RelicBase"))
                {
                    AddElement(BrutalItemInfo.GetItemByID(3340), ShopActionType.Buy);
                    Chat.Print("Added also warding totem.");
                }
                AddElement(i, ShopActionType.Buy);
                SaveBuild();
            }
            else if (args.Input.ToLower().StartsWith("/s "))
            {
                args.Process = false;
                string itemName = args.Input.Substring(2);
                LoLItem i = BrutalItemInfo.FindBestItemAll(itemName);
                Chat.Print("Sell " + i.name);

                AddElement(i, ShopActionType.Sell);
                SaveBuild();
            }
            else if (args.Input.ToLower().Equals("/buyhp"))
            {
                if (myBuild.Count == 0)
                {
                    AddElement(BrutalItemInfo.GetItemByID(3340), ShopActionType.Buy);
                    Chat.Print("Added also warding totem.");
                }
                AddElement(BrutalItemInfo.GetItemByID(2003), ShopActionType.StartHpPot);
                SaveBuild();
                args.Process = false;
            }
            else if (args.Input.ToLower().Equals("/stophp"))
            {
                if (myBuild.Count == 0)
                {
                    AddElement(BrutalItemInfo.GetItemByID(3340), ShopActionType.Buy);
                    Chat.Print("Added also warding totem.");
                }
                AddElement(BrutalItemInfo.GetItemByID(2003), ShopActionType.StopHpPot);
                SaveBuild();
                args.Process = false;
            }
        }

        private struct ItemAction
        {
            public ShopActionType t;
            public int item;
        }
    }
}
