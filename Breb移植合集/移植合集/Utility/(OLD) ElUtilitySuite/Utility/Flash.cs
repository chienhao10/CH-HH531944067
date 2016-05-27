namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.Common;
    using SharpDX;    /// <summary>
                      /// Automatically levels R.
                      /// </summary>
    internal class Flash : IPlugin
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// 
        public static bool getCheckBoxItem(string item)
        {
            return flashM[item].Cast<CheckBox>().CurrentValue;
        }

        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu flashM;
        public void CreateMenu(Menu rootMenu)
        {
            flashM = rootMenu.AddSubMenu("Flash Check", "Flash");
            flashM.Add("WallCheck", new CheckBox("Wall Check", false));
            flashM.Add("IgniteCheck", new CheckBox("Ignite Check", false));
            flashM.Add("Extend", new CheckBox("Extend", false));
            flashM.Add("Enabled", new CheckBox("Enabled", false));
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Spellbook.OnCastSpell += Spellbook_OnCastSpell; ;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender == null || !sender.Owner.IsMe || args.Slot == SpellSlot.Unknown || !ObjectManager.Player.Spellbook.GetSpell(args.Slot).Name.Equals("SummonerFlash", StringComparison.OrdinalIgnoreCase) || !getCheckBoxItem("Enabled"))
            {
                return;
            }
            var endPos = args.StartPosition;
            var distance = ObjectManager.Player.ServerPosition.LSDistance(endPos);
            if (getCheckBoxItem("WallCheck") && endPos.LSIsWall())
            {
                var wallStart = Vector3.Zero;
                var wallEnd = Vector3.Zero;
                for (var i = 0; 900 > i; i++)
                {
                    var pos = ObjectManager.Player.Position.LSExtend(endPos, i);
                    if (wallStart.Equals(Vector3.Zero) && pos.LSIsWall())
                    {
                        wallStart = ObjectManager.Player.Position.LSExtend(endPos, i - 1);
                    }
                    if (!wallStart.Equals(Vector3.Zero) && !pos.LSIsWall())
                    {
                        wallEnd = pos;
                        break;
                    }
                }
                if (!wallStart.Equals(Vector3.Zero))
                {
                    if (wallEnd.Equals(Vector3.Zero) || wallEnd.LSDistance(endPos) > wallStart.LSDistance(endPos))
                    {
                        args.Process = false;
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, wallStart);
                    }
                }
            }
            if (getCheckBoxItem("IgniteCheck"))
            {
                var buff =
                    ObjectManager.Player.Buffs.Where(
                        b => b.Name.Equals("SummonerDot", StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(b => b.EndTime)
                        .FirstOrDefault();
                if (buff != null && buff.EndTime - Game.Time > 0f)
                {
                    var hero = buff.Caster as AIHeroClient;
                    if (hero != null)
                    {
                        var ticks = (int)(buff.EndTime - Game.Time);
                        var damage =
                            hero.GetSummonerSpellDamage(ObjectManager.Player, Damage.SummonerSpell.Ignite) / 5f *
                            (ticks > 0 ? ticks : 1);
                        if (damage >= ObjectManager.Player.Health)
                        {
                            args.Process = false;
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem("Extend"))
            {
                if (distance < 390f)
                {
                    args.Process = false;
                    ObjectManager.Player.Spellbook.CastSpell(
                        args.Slot, ObjectManager.Player.ServerPosition.LSExtend(endPos, 450f));
                }
            }
        }
    }
    #endregion
}