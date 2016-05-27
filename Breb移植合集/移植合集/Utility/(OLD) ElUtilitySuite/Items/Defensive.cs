using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using ExorAIO.Utilities;

namespace ElUtilitySuite.Items
{
    internal static class DefensiveExtensions
    {
        #region Public Methods and Operators

        public static int CountHerosInRange(this AIHeroClient target, bool checkteam, float range = 1200f)
        {
            var objListTeam = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(range));
            return objListTeam.Count(hero => checkteam ? hero.Team != target.Team : hero.Team == target.Team);
        }

        public static bool IsValidState(this AIHeroClient target)
        {
            return !target.HasBuffOfType(BuffType.SpellShield) && !target.HasBuffOfType(BuffType.SpellImmunity) && !target.HasBuffOfType(BuffType.Invulnerability);
        }

        #endregion
    }

    internal class Defensive : IPlugin
    {
        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #region Public Methods and Operators

        private AIHeroClient Allies()
        {
            var target = Player;
            foreach (var unit in
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsAlly && x.IsValidTarget(900))
                    .OrderByDescending(xe => xe.Health / xe.MaxHealth * 100))
            {
                target = unit;
            }

            return target;
        }


        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = Entry.menu;
        public static Menu defenseMenu;

        public static bool getCheckBoxItem(string item)
        {
            return defenseMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public void CreateMenu(Menu rootMenu)
        {
            defenseMenu = rootMenu.AddSubMenu("Defensive", "DefensiveMenu");
            defenseMenu.Add("1", new CheckBox("Zeke's Harbinger"));
            defenseMenu.Add("2", new CheckBox("Banner of Command"));
            defenseMenu.Add("3", new CheckBox("Face of the Mountain"));
            defenseMenu.Add("4", new CheckBox("Locket of the Iron Solari"));
            defenseMenu.Add("5", new CheckBox("Wooglet's Witchcap"));
            defenseMenu.Add("6", new CheckBox("Seraph's Embrace"));
            defenseMenu.Add("7", new CheckBox("Guardian's Horn"));
            defenseMenu.Add("8", new CheckBox("Talisman of Ascension"));
            defenseMenu.Add("9", new CheckBox("Righteous Glory"));
            defenseMenu.Add("0", new CheckBox("Randuin's Omen"));
        }

        public void Load()
        {
            try
            {
                Game.OnUpdate += OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods
        

        private void DefensiveItemManager()
        {
            /// <summary>
            ///     The Zeke's Herald Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Zekes_Harbinger))
            {
                if (!getCheckBoxItem("1")) { return; }
                if (HeroManager.Allies.Any(a => a.HasBuff("itemstarksbindingbufferproc") || (!a.IsDead && a.HasBuff("rallyingbanneraurafriend"))))
                {
                    return;
                }

                if (HeroManager.Allies.OrderBy(t => t.FlatCritChanceMod).First().IsValidTarget(800f, false))
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Zekes_Harbinger, HeroManager.Allies.OrderBy(t => t.FlatCritChanceMod).First());
                }
            }

            /// <summary>
            ///     The Banner of Command Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Banner_of_Command))
            {
                if (!getCheckBoxItem("2")) { return; }
                if (GameObjects.AllyMinions.Any(m => m.CharData.BaseSkinName.Contains("MinionSuper")))
                {
                    foreach (Obj_AI_Minion super in GameObjects.AllyMinions.Where(m => m.IsValidTarget(1200f, false) && m.CharData.BaseSkinName.Contains("MinionSuper")))
                    {
                        EloBuddy.SDK.Item.UseItem(ItemId.Banner_of_Command, super);
                    }
                }
                else if (GameObjects.AllyMinions.Any(m => m.CharData.BaseSkinName.Contains("MinionSiege")))
                {
                    foreach (Obj_AI_Minion siege in
                        GameObjects.AllyMinions.Where(
                        m =>
                            m.IsValidTarget(1200f, false) &&
                            m.CharData.BaseSkinName.Contains("MinionSiege")))
                    {
                        EloBuddy.SDK.Item.UseItem(ItemId.Banner_of_Command, siege);
                    }
                }
            }

            /// <summary>
            ///     The Face of the Mountain Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Face_of_the_Mountain))
            {
                if (!getCheckBoxItem("3")) { return; }
                foreach (AIHeroClient ally in HeroManager.Allies.Where(
                    a =>
                        a.IsValidTarget(500f, false) &&
                        HealthPrediction.GetHealthPrediction(a, (int)(250 + Game.Ping / 2f)) <= a.MaxHealth / 4))
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Face_of_the_Mountain);
                    return;
                }
            }

            /// <summary>
            ///     The Locket of the Iron Solari Logic.
            /// </summary>
            if (!EloBuddy.SDK.Item.CanUseItem(ItemId.Face_of_the_Mountain) && EloBuddy.SDK.Item.CanUseItem(ItemId.Locket_of_the_Iron_Solari))
            {
                if (!getCheckBoxItem("4")) { return; }
                if (HeroManager.Allies.Count(
                    a =>
                        a.IsValidTarget(600f, false) &&
                        HealthPrediction.GetHealthPrediction(a, (int)(250 + Game.Ping / 2f)) <= a.MaxHealth / 1.5) >= 3)
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Locket_of_the_Iron_Solari);
                    return;
                }
            }

            /// <summary>
            ///     The Wooglet's Witchcap Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Wooglets_Witchcap))
            {
                if (!getCheckBoxItem("5")) { return; }
                if (HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int)(250 + Game.Ping / 2f)) <= ObjectManager.Player.MaxHealth / 4)
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Wooglets_Witchcap);
                    return;
                }
            }

            /// <summary>
            ///     The Seraph's Embrace Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Seraphs_Embrace))
            {
                if (!getCheckBoxItem("6")) { return; }
                if (HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int)(250 + Game.Ping / 2f)) <= ObjectManager.Player.MaxHealth / 4)
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Seraphs_Embrace);
                    return;
                }
            }

            /// <summary>
            ///     The Guardian's Horn Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Guardians_Horn))
            {
                if (!getCheckBoxItem("7")) { return; }
                if (HeroManager.Enemies.Count(t => t.IsValidTarget(1000f)) >= 3)
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Guardians_Horn);
                    return;
                }
            }

            /// <summary>
            ///     The Talisman of Ascension Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Talisman_of_Ascension))
            {
                if (!getCheckBoxItem("8")) { return; }
                if (HeroManager.Enemies.Count(
                    t =>
                        t.IsValidTarget(2000f) &&
                        t.CountEnemiesInRange(1500f) <=
                            ObjectManager.Player.CountAlliesInRange(1500f) + t.CountAlliesInRange(1500f) - 1) > 1)
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Talisman_of_Ascension);
                    return;
                }
            }

            /// <summary>
            ///     The Righteous Glory Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Righteous_Glory))
            {
                if (!getCheckBoxItem("9")) { return; }
                if (!ObjectManager.Player.HasBuff("ItemRighteousGlory"))
                {
                    if (HeroManager.Enemies.Count(t => t.IsValidTarget(2000f) && t.CountEnemiesInRange(1500f) <= ObjectManager.Player.CountAlliesInRange(1500f) + t.CountAlliesInRange(1500f) - 1) > 1)
                    {
                        EloBuddy.SDK.Item.UseItem(ItemId.Righteous_Glory);
                        return;
                    }
                }
                else
                {
                    if (ObjectManager.Player.CountEnemiesInRange(450f) >= 2)
                    {
                        EloBuddy.SDK.Item.UseItem(ItemId.Righteous_Glory);
                    }
                }
                return;
            }

            /// <summary>
            ///     The Randuin's Omen Logic.
            /// </summary>
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Randuins_Omen))
            {
                if (!getCheckBoxItem("0")) { return; }
                if (ObjectManager.Player.CountEnemiesInRange(500f) >= 2)
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Randuins_Omen);
                    return;
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Entry.Player.IsDead)
            {
                return;
            }

            try
            {
                DefensiveItemManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}