using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace ARAMDetFull
{
    class MapControl
    {

        public static SpellSlot[] spellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        internal class ChampControl
        {
            public AIHeroClient hero = null;

            public float reach = 0;

            public float dangerReach = 0;

            public int activeDangers = 0;





            protected List<SpellData> champSkillShots = new List<SpellData>();
            protected List<TargetSpellData> champTargSpells = new List<TargetSpellData>();

            public ChampControl(AIHeroClient champ)
            {
                hero = champ;
                champSkillShots = SpellDatabase.getChampionSkillshots(champ.ChampionName);
                champTargSpells = TargetSpellDatabase.getChampionTargSpell(champ.ChampionName);



                getReach();
            }

            public float getReach()
            {
                dangerReach = 0;
                reach = (ARAMSimulator.player.Level < 7 && hero.IsEnemy) ? 750 : hero.AttackRange + 200;
                activeDangers = 0;

                foreach (var slot in spellSlots)
                {
                    var spell = hero.Spellbook.GetSpell(slot);
                    if ((spell.CooldownExpires - Game.Time) > 1.5f || hero.Spellbook.CanUseSpell(slot) == SpellState.NotLearned)
                        continue;
                    var range = (spell.SData.CastRange < 1000) ? spell.SData.CastRange : 1000;
                    if (spell.SData.CastRange > range)
                        reach = range;
                }

                /*foreach (var sShot in champSkillShots)
                {
                    if(!hero.Spellbook.GetSpell(sShot.Slot).IsReady())
                        continue;
                    float range = (sShot.Range < 1000) ? sShot.Range + sShot.Radius : 1000;
                    if (range > reach)
                        reach = range;

                    if (sShot.IsDangerous && dangerReach <= range)
                    {
                        activeDangers++;
                        dangerReach = range;
                    }
                }

                foreach (var tSpell in champTargSpells)
                {
                    if (!hero.Spellbook.GetSpell(tSpell.Spellslot).IsReady() || tSpell.Type == SpellType.Skillshot)
                        continue;
                    float range = (tSpell.Range < 1000) ? tSpell.Range+200 : 1000;
                    if (range > reach)
                        reach = range;

                    if (isDangerousTarg(tSpell) && dangerReach <= range)
                    {
                        activeDangers++;
                        dangerReach = range;
                    }
                }*/
                return reach;
            }

            public int getccCount()
            {
                int count = champSkillShots.Count(sShot => sShot.IsDangerous);
                count += champTargSpells.Count(tSpell => tSpell.Type != SpellType.Skillshot && (tSpell.CcType == CcType.Fear || tSpell.CcType == CcType.Stun || tSpell.CcType == CcType.Pull || tSpell.CcType == CcType.Snare));

                return count;
            }

            public bool isDangerousTarg(TargetSpellData tSpell)
            {
                if(tSpell.Type == SpellType.Skillshot)
                    return false;

                if (tSpell.CcType == CcType.Stun || tSpell.CcType == CcType.Snare || tSpell.CcType == CcType.Fear ||
                    tSpell.CcType == CcType.Pull || tSpell.CcType == CcType.Taunt)
                    return true;
                return false;
            }
        }

        internal class MyControl : ChampControl
        {

            private Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>();

            public static Spellbook sBook = ObjectManager.Player.Spellbook;
            public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
            public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
            public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
            public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);

            public MyControl(AIHeroClient champ) : base(champ)
            {
                try
                {
                    spells.Add(SpellSlot.Q, new Spell(SpellSlot.Unknown));
                    spells.Add(SpellSlot.W, new Spell(SpellSlot.Unknown));
                    spells.Add(SpellSlot.E, new Spell(SpellSlot.Unknown));
                    spells.Add(SpellSlot.R, new Spell(SpellSlot.Unknown));

                    hero = champ;
                    champSkillShots = SpellDatabase.getChampionSkillshots(champ.ChampionName);
                    champTargSpells = TargetSpellDatabase.getChampionTargSpell(champ.ChampionName);


                    getReach();

                    foreach (var tSpell in champTargSpells)
                    {
                        if (spells[tSpell.Spellslot] != null)
                            spells[tSpell.Spellslot] = new Spell(tSpell.Spellslot, tSpell.Range);
                    }

                    foreach (var sShot in champSkillShots)
                    {
                        if (spells[sShot.Slot] != null)
                            spells[sShot.Slot] = new Spell(sShot.Slot, sShot.Range);
                    }

                    foreach (var sShot in champSkillShots)
                    {
                        if (spells[sShot.Slot] != null)
                        {
                            bool coll = sShot.CollisionObjects.Contains(SpellDatabase.CollisionObjectTypes.Minion);
                            spells[sShot.Slot].SetSkillshot(sShot.Delay, sBook.GetSpell(sShot.Slot).SData.LineWidth, sShot.MissileSpeed, coll, getSSType(sShot));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            public SkillshotType getSSType(SpellData sData)
            {
                if(sData.Type == SpellData.SkillShotType.SkillshotCircle)
                    return SkillshotType.SkillshotCircle;

                if (sData.Type == SpellData.SkillShotType.SkillshotCone)
                    return SkillshotType.SkillshotCone;

                return SkillshotType.SkillshotLine;
            }

            public void useSkillshots()
            {
                foreach (var sShot in champSkillShots)
                {
                    AIHeroClient bTarg = ARAMTargetSelector.getBestTarget(sShot.Range + sShot.Radius/2);
                    if (bTarg == null || spells[sShot.Slot] == null)
                        continue;
                    if (spells[sShot.Slot].IsReady())
                        spells[sShot.Slot].Cast(bTarg);
                }
            }

            public void useNonSkillshots()
            {
                foreach (var tSkill in champTargSpells)
                {
                    float range = (tSkill.Range != 0) ? tSkill.Range : 500;
                    if (tSkill.Type == SpellType.Self)
                    {
                        var bTarg = ARAMTargetSelector.getBestTarget(range, true);
                        if (bTarg != null && spells[tSkill.Spellslot] != null)
                        {
                            if (spells[tSkill.Spellslot].IsReady())
                                spells[tSkill.Spellslot].Cast();
                        }
                    }
                    else if (tSkill.Type == SpellType.Targeted)
                    {
                        var bTarg = ARAMTargetSelector.getBestTarget(range);
                        if (bTarg != null && spells[tSkill.Spellslot] != null)
                        {
                            if (spells[tSkill.Spellslot].IsReady())
                                spells[tSkill.Spellslot].Cast(bTarg);
                        }
                    }
                }
            }

            public float canDoDmgTo(Obj_AI_Base target)
            {
                float dmgreal = 0;
                float mana = 0;
                foreach (var spell in spells.Values)
                {
                    try
                    {

                        if(spell == null || !spell.IsReady())
                            continue;

                        float dmg = 0;
                        var checkRange = spell.Range + 250;
                        if (hero.LSDistance(target, true) < checkRange*checkRange)
                            dmg = spell.GetDamage(target);
                        if (dmg != 0)
                            mana += hero.Spellbook.GetSpell(spell.Slot).SData.Mana;
                        if (hero.Mana > mana)
                            dmgreal += dmg;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                return dmgreal;
            }
        }

        public static List<ChampControl> enemy_champions = new List<ChampControl>();

        public static List<ChampControl> ally_champions = new List<ChampControl>();

        public static MyControl myControler;

        public static void setupMapControl()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if(hero.IsMe)
                    continue;

                if(hero.IsAlly)
                    ally_champions.Add(new ChampControl(hero));

                if (hero.IsEnemy)
                    enemy_champions.Add(new ChampControl(hero));
            }
            myControler = new MyControl(ObjectManager.Player);
        }


        public static bool inDanger()
        {
            int enesAround = enemy_champions.Count(ene => !ene.hero.IsDead && ene.hero.LSIsValidTarget(1300));
            int allyAround = ally_champions.Count(aly => !aly.hero.IsDead && aly.hero.LSIsValidTarget(700));
            return (enesAround - allyAround) > 1;
        }

        public static AIHeroClient fightIsOn()
        {
            foreach (var enem in enemy_champions.Where(ene => !ene.hero.IsDead && ene.hero.IsVisible).OrderBy(ene => ene.hero.LSDistance(ObjectManager.Player,true)))
            {
                if (myControler.canDoDmgTo(enem.hero) > enem.hero.Health+250)
                    return enem.hero;

                if (ally_champions.Where(ene => !ene.hero.IsDead && !ene.hero.IsMe).Any(ally => enem.hero.LSDistance(ally.hero, true) < 500*500))
                {
                    return enem.hero;
                }
            }

            return null;
        }

        public static bool fightIsClose()
        {
            foreach (var enem in enemy_champions.Where(ene => !ene.hero.IsDead && ene.hero.IsVisible).OrderBy(ene => ene.hero.LSDistance(ObjectManager.Player, true)))
            {

                if (ally_champions.Where(ene => !ene.hero.IsDead && !ene.hero.IsMe).Any(ally => enem.hero.LSDistance(ally.hero, true) < 400 * 400))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool fightIsOn(Obj_AI_Base target)
        {
            if (myControler.canDoDmgTo(target) > target.Health)
                    return true;

                if (ally_champions.Where(ene => !ene.hero.IsDead && !ene.hero.IsMe).Any(ally => target.LSDistance(ally.hero, true) < 300 * 300))
                {
                    return true;
                }

            return false;
        }


        public static int enemiesAroundPoint(Vector2 point, float range)
        {
            int count = 0;
            foreach (var ene in enemy_champions.Where(ene=>!ene.hero.IsDead))
            {
                if (ene.hero.LSDistance(point, true) < range*range)
                    count++;
            }
            return count;
        }

        public static int balanceAroundPoint(Vector2 point, float range)
        {
            int balance = 0;
            balance -= enemy_champions.Where(ene => !ene.hero.IsDead).Count(ene => ene.hero.LSDistance(point, true) < range * range);

            balance += ally_champions.Where(aly => !aly.hero.IsDead).Count(aly => aly.hero.LSDistance(point, true) < (range - 150) * (range - 150));
            return balance;
        }

        public static int balanceAroundPointAdvanced(Vector2 point, float range)
        {
            int balance = (point.To3D().UnderTurret(true)) ? -80 : (point.To3D().UnderTurret(false)) ? 80 : 0;
            foreach (var ene in enemy_champions)
            {
                if (!ene.hero.IsDead && ene.hero.LSDistance(point, true) < range*range && !unitIsUseless(ene.hero))
                    balance -= (int)((ene.hero.HealthPercent + 20 - ene.hero.Deaths * 4 + ene.hero.ChampionsKilled * 4) *
                               ((ARAMSimulator.player.Level < 7)
                        ? 1.3f
                        : 1f));
            }


            foreach (var aly in ally_champions)
            {
                if (!aly.hero.IsDead && aly.hero.LSDistance(point, true) < 2000 * 2000 && aly.hero.LSDistance(ARAMSimulator.toNex.Position) < (point.LSDistance(ARAMSimulator.toNex.Position) + 450 + (ARAMSimulator.tankBal * -5) + (ARAMSimulator.agrobalance * 3)))
                    balance += ((int)aly.hero.HealthPercent + 20 + 20 - aly.hero.Deaths * 4 + aly.hero.ChampionsKilled * 4);
            }
            var myBal = ((int)myControler.hero.HealthPercent + 20 + 20 - myControler.hero.Deaths * 10 +
                         myControler.hero.ChampionsKilled*10);
            balance += (myBal<0)?10:myBal;
            return balance;
        }

        public static double unitIsUselessFor(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare || buff.Type == BuffType.Fear))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        public static bool unitIsUseless(Obj_AI_Base unit)
        {
            return unitIsUselessFor(unit) > 0.7;
        }

        public static ChampControl getByObj(Obj_AI_Base champ)
        {
            return enemy_champions.FirstOrDefault(ene => ene.hero.NetworkId == champ.NetworkId);
        }

        public static List<int> usedRelics = new List<int>();

        public static Obj_AI_Base ClosestRelic()
        {
            var closesEnem = ClosestEnemyTobase();
            //var closesEnemTower = ClosestEnemyTobase();
            var hprelics = ObjectManager.Get<Obj_AI_Base>().Where(
                r => r.IsValid && !r.IsDead && (r.Name.Contains("HealthRelic") || r.Name.Contains("BardChime") || (r.Name.Contains("BardPickup") && ObjectManager.Player.ChampionName == "Bard")) 
                    && !usedRelics.Contains(r.NetworkId) && (closesEnem == null || r.LSDistance(ARAMSimulator.fromNex.Position, true) - 500 < closesEnem.LSDistance(ARAMSimulator.fromNex.Position, true))).ToList().OrderBy(r => ARAMSimulator.player.LSDistance(r, true));
            return hprelics.FirstOrDefault();
        }

        public static Obj_AI_Base ClosestEnemyTobase()
        {
            return
                HeroManager.Enemies
                    .Where(h => h.IsValid && !h.IsDead && h.IsVisible && h.IsEnemy)
                    .OrderBy(h => h.LSDistance(ARAMSimulator.fromNex.Position, true))
                    .FirstOrDefault();
        }

        public static Obj_AI_Base ClosestEnemyTower()
        {
            return
                ObjectManager.Get<Obj_AI_Turret>()
                    .Where(tur => !tur.IsDead)
                    .OrderBy(tur => tur.LSDistance(ObjectManager.Player.Position, true))
                    .FirstOrDefault();
        }

        /* LOGIC!!
         * 
         * Go to Kill minions
         * If no minions go for enemy tower
         * Cut path on enemies range
         * 
         * Orbwalk all the way
         * 
         * 
         * 
         */

    }
}
