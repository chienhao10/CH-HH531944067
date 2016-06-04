using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace ARAMDetFull
{
    abstract class SumSpell
    {
        public Spell spell;
        public string spellName;
        public abstract void useSpell();

        public SumSpell(Spell spel)
        {
            spell = spel;
        }

    }
    


    class SummonerSpells
    {
        Spell sum1 = new Spell(SpellSlot.Summoner1);
        Spell sum2 = new Spell(SpellSlot.Summoner2);
        SumSpell sSpell1 = null;
        SumSpell sSpell2 = null;

        private static AIHeroClient player = ObjectManager.Player;

        public SummonerSpells()
        {
            Chat.Print(sum1.Instance.Name);
            switch (sum1.Instance.Name.ToLower())
            {
                case "summonerflash":
                    sSpell1 = new Flash(sum1);
                    break;
                case "summonerdot"://Ignite
                    sSpell1 = new Ignite(sum1);
                    break;
                case "summonerheal"://Heal
                    sSpell1 = new Heal(sum1);
                    break;
                case "summonerhaste"://Ghost
                    sSpell1 = new Ghost(sum1);
                    break;
                case "summonerexhaust"://Exhoust
                    sSpell1 = new Exhoust(sum1);
                    break;
                case "summonerbarrier"://Barrier
                    sSpell1 = new Barrier(sum1);
                    break;
                case "summonermana"://Clarity
                    sSpell1 = new Clarity(sum1);
                    break;
                case "summonersnowball"://Clarity
                    sSpell1 = new SnowBall(sum1);
                    break;
            }

            Chat.Print(sum2.Instance.Name);
            switch (sum2.Instance.Name.ToLower())
            {
                case "summonerflash":
                    sSpell2 = new Flash(sum2);
                    break;
                case "summonerdot"://Ignite
                    sSpell2 = new Ignite(sum2);
                    break;
                case "summonerheal"://Heal
                    sSpell2 = new Heal(sum2);
                    break;
                case "summonerhaste"://Ghost
                    sSpell2 = new Ghost(sum2);
                    break;
                case "summonerexhaust"://Exhoust
                    sSpell2 = new Exhoust(sum2);
                    break;
                case "summonerbarrier"://Barrier
                    sSpell2 = new Barrier(sum2);
                    break;
                case "summonermana"://Clarity
                    sSpell2 = new Clarity(sum2);
                    break;
                case "summonersnowball"://Clarity
                    sSpell1 = new SnowBall(sum1);
                    break;
            }
        }

        public void useSumoners()
        {
            if(sSpell1 != null)
                sSpell1.useSpell();
            if (sSpell2 != null)
                sSpell2.useSpell();
        }

        class SnowBall : SumSpell
        {
            AIHeroClient snowed;

            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;

                if (spell.Instance.Name.Equals("snowballfollowupcast"))
                {
                    if (snowed != null)
                    {
                        if (player.HealthPercent > 30 && snowed.HealthPercent < 65 && MapControl.balanceAroundPointAdvanced(snowed.Position.LSTo2D(), 850) > -20)
                            spell.Cast();
                    }
                }
                else
                {
                    var tar = ARAMTargetSelector.getBestTarget(spell.Range);
                    if (tar != null)
                    {
                        spell.Cast(tar);
                        snowed = tar;
                    }
                }

                
            }

            public SnowBall(Spell spel)
                : base(spel)
            {
                spell.Range = 1200f;
                spell.SetSkillshot(.33f, 50f, 1600, true, SkillshotType.SkillshotLine);
                spell.MinHitChance = HitChance.High;
                Console.WriteLine("SummonerSpell Set");
            }
        }

        //SummonerFlash
        class Flash : SumSpell
        {
            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;
                if (player.LSCountEnemiesInRange(600) > 1 && player.HealthPercent < 40)
                {
                        spell.Cast(player.Position.LSExtend(ARAMSimulator.fromNex.Position, 450));
                }
            }

            public Flash(Spell spel) : base(spel)
            {
                Console.WriteLine("SummonerSpell Set");
            }
        }

        class Ignite : SumSpell
        {
            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;
                var tar = ARAMTargetSelector.getBestTarget(450);
                if(tar != null)
                    if (tar.HealthPercent > 20 && tar.HealthPercent < 50)
                    {
                        spell.Cast(tar);
                    }
            }

            public Ignite(Spell spel)
                : base(spel)
            {
                Console.WriteLine("SummonerSpell Set");
            }
        }

        class Heal : SumSpell
        {
            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;
                if (player.LSCountEnemiesInRange(600) > 0 && player.HealthPercent < 30)
                {
                    spell.Cast(player);
                }
            }

            public Heal(Spell spel)
                : base(spel)
            {
                Console.WriteLine("SummonerSpell Set");
            }
        }

        class Ghost : SumSpell
        {
            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;
                if (player.InShop())
                {
                    spell.Cast();
                }
            }

            public Ghost(Spell spel)
                : base(spel)
            {
                Console.WriteLine("SummonerSpell Set");
            }
        }

        class Exhoust : SumSpell
        {
            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;
                var tar = ARAMTargetSelector.getBestTarget(300);
                if (tar != null)
                    if (tar.HealthPercent > 20)
                    {
                        spell.Cast(tar);
                    }
            }

            public Exhoust(Spell spel)
                : base(spel)
            {
                Console.WriteLine("SummonerSpell Set");
            }
        }

        class Barrier : SumSpell
        {
            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;
                if (player.LSCountEnemiesInRange(600) > 0 && player.HealthPercent < 20)
                {
                    spell.Cast();
                }
            }

            public Barrier(Spell spel)
                : base(spel)
            {
                Console.WriteLine("SummonerSpell Set");
            }
        }

        class Clarity : SumSpell
        {
            public override void useSpell()
            {
                if (!spell.IsReady())
                    return;
                if (player.ManaPercent < 25)
                {
                    spell.Cast();
                }
            }

            public Clarity(Spell spel)
                : base(spel)
            {
                Console.WriteLine("SummonerSpell Set");
            }
        }

    }
}
