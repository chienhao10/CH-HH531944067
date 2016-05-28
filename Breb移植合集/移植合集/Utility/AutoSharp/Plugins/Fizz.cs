using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Fizz : PluginBase
    {
        public static bool UseEAgain;

        public Fizz()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 560);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 0);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 370);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1275);

            E.SetSkillshot(0.5f, 120, 1300, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, 250f, 1200f, false, SkillshotType.SkillshotLine);
            UseEAgain = true;
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                Combo(Target);
            }
        }

        // combo from sigma series
        private void Combo(AIHeroClient target)
        {
            if (target != null)
            {
                if (target.LSIsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }
                //castItems(target);
                if (target.LSIsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(target, true);
                }
                if (target.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)) && W.IsReady())
                {
                    W.Cast(Player);
                    return;
                }
                if (target.LSIsValidTarget(800) && E.IsReady() && UseEAgain)
                {
                    if (target.LSIsValidTarget(370 + 250) && ESpell.Name == "FizzJump")
                    {
                        E.Cast(target, true);
                        UseEAgain = false;
                        LeagueSharp.Common.Utility.DelayAction.Add(250, () => UseEAgain = true);
                    }
                    if (target.LSIsValidTarget(370 + 150) && target.LSIsValidTarget(330) == false &&
                        ESpell.Name == "fizzjumptwo")
                    {
                        E.Cast(target, true);
                    }
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public static Spellbook SpellBook = ObjectManager.Player.Spellbook;
        public static SpellDataInst ESpell = SpellBook.GetSpell(SpellSlot.E);
    }
}