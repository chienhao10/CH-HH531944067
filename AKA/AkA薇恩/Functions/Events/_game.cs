
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Aka_s_Vayne_reworked.Functions.Events
{
    class _game
    {
        public static void Move(float kite, float humanizer)
        {
            if (Game.Time * 1000 > Variables.lastaa + Variables._Player.AttackCastDelay * 1000 - Game.Ping / 2 + kite && Game.Time * 1000 > Variables.lastmove + humanizer)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                Variables.lastmove = Game.Time * 1000;
            }
        }

        public static AIHeroClient FocusWTarget
        {
            get
            {
                return ObjectManager.Get<AIHeroClient>()
                    .Where(
                        enemy =>
                            !enemy.IsDead &&
                            enemy.IsValidTarget((Program.Q.IsReady() ? Program.Q.Range : 0) + Variables._Player.AttackRange))
                    .FirstOrDefault(
                        enemy => enemy.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count > 0));
            }
        }

        public static bool CanUseBotrk
        {
            get
            {
                if (Game.Time * 1000 >=
                    Variables.lastaa + Variables._Player.AttackDelay * 1000 - Variables._Player.AttackDelay * 1000 / 1.5 &&
                    Game.Time * 1000 <
                    Variables.lastaa + Variables._Player.AttackDelay * 1000 - Variables._Player.AttackDelay * 1000 / 1.7 &&
                    Game.Time * 1000 > Variables.lastaa + Variables._Player.AttackCastDelay * 1000)
                {
                    return true;
                }
                return false;
            }
        }

        public static void Botrk2(Obj_AI_Base unit)
        {
            if (Modes.Combo.AfterAttack && (unit.Distance(ObjectManager.Player) > 500f || (ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) * 100 <= 95))
            {
                if (Item.HasItem(3144) && Item.CanUseItem(3144))
                {
                    Item.UseItem(3144, unit);
                }
                if (Item.HasItem(3153) && Item.CanUseItem(3153))
                {
                    Item.UseItem(3153, unit);
                }
            }
        }

        public static void Botrk(Obj_AI_Base unit)
        {
            if (Item.HasItem(3144) && Item.CanUseItem(3144) && CanUseBotrk)
                Item.UseItem(3144, unit);
            if (Item.HasItem(3153) && Item.CanUseItem(3153) && CanUseBotrk)
                Item.UseItem(3153, unit);
        }

        public static AIHeroClient Target
        {
            get
            {
                foreach (var unit in EntityManager.Heroes.Enemies.
                   OrderBy(x => x.Health + x.Health * (x.Armor / (x.Armor + 100)) - x.TotalAttackDamage * x.AttackSpeedMod - x.TotalMagicalDamage).Where(x =>
                                                              x.IsValidTarget(ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius + x.BoundingRadius)
                                                              && !x.IsZombie
                                                              && !x.HasBuff("JudicatorIntervention")           //kayle R
                                                              && !x.HasBuff("AlphaStrike")                     //Master Yi Q
                                                              && !x.HasBuff("zhonyasringshield")               //zhonya
                                                              && !x.HasBuff("VladimirSanguinePool")            //vladimir W
                                                              && !x.HasBuff("ChronoShift")                     //zilean R
                                                              && !x.HasBuff("yorickrazombie")                  //yorick R
                                                              && !x.HasBuff("mordekaisercotgself")             //mordekaiser R
                                                              && !x.HasBuff("UndyingRage")                     //tryndamere R
                                                              && !x.HasBuff("sionpassivezombie")               //sion Passive
                                                              && !x.HasBuff("elisespidere")                    //elise not visible
                                                              && !x.HasBuff("KarthusDeathDefiedBuff")          //karthus passive
                                                              && !x.HasBuff("kogmawicathiansurprise")          //kog'maw passive
                                                              && !x.HasBuff("zyrapqueenofthorns")              //zyra passive
                                                              && !x.HasBuff("monkeykingdecoystealth")          //wukong W not visible
                                                              && !x.HasBuff("JaxCounterStrike")                //Jax E
                                                              && !x.HasBuff("Deceive")                         //Shaco not visible
                                                              && !Variables._Player.HasBuff("BlindingDart") //Me Teemo Q
                                                              && !x.HasBuff("camouflagestealth")               //Teemo not visible
                                                              && !x.HasBuff("khazixrstealth")                  //Kha'Zix not visible
                                                              && !x.HasBuff("evelynnstealthmarker")            //Evelynn not visible
                                                              && !x.HasBuff("akaliwstealth")))                 //Akali not visible


                {
                    return unit;
                }
                return null;
            }
        }
    }
}
