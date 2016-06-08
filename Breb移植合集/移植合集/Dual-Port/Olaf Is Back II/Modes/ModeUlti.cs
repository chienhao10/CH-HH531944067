using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using OlafxQx.Evade;
using SpellData = OlafxQx.Evade.SpellData;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace OlafxQx.Modes
{
    internal static class ModeUlti
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell R => Champion.PlayerSpells.R;

        private static readonly BuffType[] BuffList =
        {
            BuffType.Stun, BuffType.Blind, BuffType.Charm, BuffType.Fear,
            BuffType.Knockback, BuffType.Knockup, BuffType.Taunt, BuffType.Slow, BuffType.Silence, BuffType.Disarm,
            BuffType.Snare
        };

        private static readonly string[] BuffListCaption =
        {
            "Stun", "Blind", "Charm", "Fear", "Knockback", "Knockup",
            "Taunt", "Slow", "Silence", "Disarm", "Snare"
        };

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = ParentMenu.AddSubMenu("R:", "MenuR");

            MenuLocal.Add("MenuR.R.Enabled", new KeyBind("Enabled:", true, KeyBind.BindTypes.PressToggle, 'K'));
            MenuLocal.Add("MenuR.R.OnyChampionSpells", new KeyBind("Dodge Only Champion Spells:", false, KeyBind.BindTypes.HoldActive, 32));

            MenuLocal.AddGroupLabel("Buffs:");
            foreach (var displayName in BuffListCaption)
            {
                MenuLocal.Add("Buff." + displayName, new CheckBox(displayName));
            }

            MenuLocal.AddGroupLabel("Enemy Spells:");
            foreach (var c in HeroManager.Enemies.SelectMany(t => Evade.SpellDatabase.Spells.Where(s => s.Type == SpellData.SkillShotType.SkillshotTargeted).Where(c =>string.Equals(c.ChampionName, t.ChampionName,StringComparison.InvariantCultureIgnoreCase)).OrderBy(s => s.ChampionName)))
            {
                MenuLocal.Add("BuffT." + c.ChampionName + c.Slot, new CheckBox(c.ChampionName + " : " + c.Slot));
            }

            Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (!MenuLocal["MenuR.R.Enabled"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            if (MenuLocal["MenuR.R.OnyChampionSpells"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            if (!R.IsReady())
            {
                return;
            }

            ExecuteUltimateForBuffs();
        }

        private static void ExecuteUltimateForBuffs()
        {
            for (int i = 0; i < BuffListCaption.Length; i++)
            {
                if (MenuLocal["Buff." + BuffListCaption[i]].Cast<CheckBox>().CurrentValue &&
                    ObjectManager.Player.HasBuffOfType(BuffList[i]))
                {
                    R.Cast();
                }
            }
        }

        private static void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!R.IsReady() || !MenuLocal["MenuR.R.Enabled"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            if (!(sender is AIHeroClient) || !sender.LSIsValidTarget(1500) || sender.IsMe || sender.IsAlly)
            {
                return;
            }

            if (sender.IsDead)
            {

                return;
            }

            if (!sender.LSIsValidTarget(Champion.PlayerSpells.Q.Range))
            {
                return;
            }

            if (sender.Team == ObjectManager.Player.Team || !args.Target.IsMe)
            {
                return;
            }

            if (sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            foreach (
                var spell in
                    SpellDatabase.Spells.Where(s => s.Type == SpellData.SkillShotType.SkillshotTargeted)
                        .Where(
                            spell =>
                                args.Target.IsMe && spell.Slot == args.Slot &&
                                string.Equals(((AIHeroClient) sender).ChampionName, spell.ChampionName,
                                    StringComparison.InvariantCultureIgnoreCase) &&
                                MenuLocal["BuffT." + spell.ChampionName + spell.Slot] != null &&
                                MenuLocal["BuffT." + spell.ChampionName + spell.Slot].Cast<CheckBox>().CurrentValue))
            {
                R.Cast();
            }
        }
    }
}