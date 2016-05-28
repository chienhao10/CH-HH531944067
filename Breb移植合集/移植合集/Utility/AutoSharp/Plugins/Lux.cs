//lux get part of script from ChewyMoon

using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Plugins
{

    public class Lux : PluginBase
    {
        public static GameObject EGameObject;

        public Lux()
        {
          
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1300);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1075);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1100);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 3340);

            Q.SetSkillshot(0.25f, 70, 1200, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 150, 1200, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 275, 1300, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1, 190, float.MaxValue, false, SkillshotType.SkillshotLine);

        }


        public override void OnUpdate(EventArgs args)
        {

            KS();
            StealBlue();
            StealRed();

            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    CastQ(Target);
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    CastE(Target);
                }
            }

        }

        private void StealBlue()
        {
            if (!R.IsReady()) return;

            var blueBuffs = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name.ToUpper().Equals("SRU_BLUE"));
            foreach (
                var blueBuff in
                    blueBuffs.Where(
                        blueBuff => Player.LSGetSpellDamage(blueBuff, SpellSlot.R) > blueBuff.Health))
            {
                R.Cast(blueBuff);
            }
        }

        private void StealRed()
        {
            if (!R.IsReady()) return;

            var redBuffs = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name.ToUpper().Equals("SRU_RED"));
            foreach (
                var redBuff in
                    redBuffs.Where(
                        redBuff => Player.LSGetSpellDamage(redBuff, SpellSlot.R) > redBuff.Health))
            {
                R.Cast(redBuff);
            }
        }

        public  bool EActivated
        {
            get { return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 || EGameObject != null; }
        }




        private void CastE(AIHeroClient target)
        {
            if (EActivated)
            {
                if (
                    !ObjectManager.Get<AIHeroClient>()
                        .Where(x => x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.LSIsValidTarget())
                        .Any(enemy => enemy.LSDistance(EGameObject.Position) <= E.Width)) return;

                var isInAaRange = Player.LSDistance(target) <= Orbwalking.GetRealAutoAttackRange(Player);

                if (isInAaRange && !HasPassive(target))
                    E.Cast();

                // Pop E if the target is out of AA range
                if (!isInAaRange)
                    E.Cast();
            }
            else
            {
                E.Cast(target);
            }
        }

        private void CastQ(Obj_AI_Base target)
        {
            var input = Q.GetPrediction(target);
            var col = Q.GetCollision(Player.ServerPosition.LSTo2D(), new List<Vector2> { input.CastPosition.LSTo2D() });
            var minions = col.Where(x => !(x is AIHeroClient)).Count(x => x.IsMinion);

            if (minions <= 1)
                Q.Cast(input.CastPosition);
        }

        public  bool HasPassive(AIHeroClient hero)
        {
            return hero.HasBuff("luxilluminatingfraulein");
        }

        // ReSharper disable once InconsistentNaming
        public void KS()
        {
            if (!R.IsReady())
                return;
            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => x.LSIsValidTarget())
                        .Where(x => !x.IsZombie)
                        .Where(x => !x.IsDead)
                        .Where(enemy => Player.GetDamageSpell(enemy, SpellSlot.R).CalculatedDamage > enemy.Health))
            {
                R.Cast(enemy);
                return;
            }
        }



        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboRKS", "Use R KS", true);
        }
    }
}


