using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;
using SharpDX;

namespace ARAMDetFull.Champions
{
    class RekSai : Champion
    {
        private ActiveModes aModes = new ActiveModes();

        
        public static Spell QBurrowed
        {
            get { return QBurrowed; }
        }
        public RekSai()
        {
            Console.WriteLine("RekSai Opened");
            LXOrbwalker.AfterAttack += ExecuteAfterAttack;

        }


        public override void useQ(Obj_AI_Base target)
        {
            if (QBurrowed.IsReady() || !player.IsBurrowed())
                return;
            QBurrowed.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {

        }

        public override void useE(Obj_AI_Base target)
        {
            
        }

        public override void useR(Obj_AI_Base target)
        {
            
        }

        public override void setUpSpells()
        {
           /* SpellManager.Initialize();

            //Initialize our Spells
            _r = new Spell(SpellSlot.R);
            aModes = new ActiveModes();
            // Unburrowed
            _qNormal = new Spell(SpellSlot.Q, 300);
            _wNormal = new Spell(SpellSlot.W, 0);
            _eNormal = new Spell(SpellSlot.E, 250);

            // Burrowed
            _qBurrowed = new Spell(SpellSlot.Q, 1500);
            _wBurrowed = new Spell(SpellSlot.W, 0);
            _eBurrowed = new Spell(SpellSlot.E, 750);

            // Finetune spells
            _qBurrowed.SetSkillshot(0.125f, 60, 4000, true, SkillshotType.SkillshotLine);
            EBurrowed.SetSkillshot(0, 60, 1600, false, SkillshotType.SkillshotLine);*/
        }

        public override void useSpells()
        {
            //if (!player.IsBurrowed() && W.IsReady())
            //    _wNormal.Cast();


           // var tar = ARAMTargetSelector.getBestTarget(700);
            //if(tar != null)
           //     aModes.OnCombo();

           // tar = ARAMTargetSelector.getBestTarget(QBurrowed.Range);
           // if (tar != null) useQ(tar);
        }

        public override void farm()
        {
            //aModes.OnWaveClear();
        }

        public void ExecuteAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && target is Obj_AI_Base)
            {
                aModes.OnCombo(true, target as Obj_AI_Base);
            }

            if (unit.IsMe && target is Obj_AI_Minion)
            {
                aModes.OnWaveClear(true, target as Obj_AI_Minion);
            }
        }


        public float GetFullDamage(AIHeroClient target)
        {
            // AA damage
            float damage = (float)player.LSGetAutoAttackDamage(target);

            // Q
            if (SpellManager.Q.IsReady() || player.HasQActive())
                damage += GetRealDamage(SpellSlot.Q, target);

            // W
            if (SpellManager.W.IsReady())
                damage += GetRealDamage(SpellManager.W,target);

            // E
            if (SpellManager.E.IsReady())
                damage += GetRealDamage(SpellManager.E,target);

            return damage;
        }

        public enum BurrowState
        {
            BURROWED,
            UNBURROWED,
            AUTOMATIC
        }

        public static float GetRealDamage(Spell spell, Obj_AI_Base target)
        {
            return GetRealDamage(spell.Slot, target, spell.Instance.Name.Contains("Burrowed") ? BurrowState.BURROWED : BurrowState.UNBURROWED);
        }

        public static float GetRealDamage(SpellSlot slot, Obj_AI_Base target, BurrowState state = BurrowState.AUTOMATIC)
        {
            // Damage holders
            float damage = 0;
            float extraDamage = 0;
            var damageType = DamageType.Physical;

            // Validate spell level
            var spellLevel = player.Spellbook.GetSpell(slot).Level;
            if (spellLevel == 0)
                return 0;
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:

                    if (state == BurrowState.UNBURROWED || state == BurrowState.AUTOMATIC && !player.IsBurrowed())
                    {
                        // Rek'Sai's next 3 basic attacks within 5 seconds deal 15/35/55/75/95 (+0.4) bonus Physical Damage to nearby enemies.
                        damage = new float[] { 15, 35, 55, 75, 95 }[spellLevel] + 0.4f * player.TotalAttackDamage();
                        extraDamage = (float)player.LSGetAutoAttackDamage(target);
                    }
                    else
                    {
                        // Rek'Sai launches a burst of void-charged earth that explodes on first unit hit, dealing 60/90/120/150/180 (+1) Magic Damage
                        // and revealing non-stealthed enemies hit for 2.5 seconds.
                        damageType = DamageType.Magical;
                        damage = new float[] { 60, 90, 120, 150, 180 }[spellLevel] + player.TotalMagicalDamage();
                    }

                    break;

                case SpellSlot.W:

                    if (state == BurrowState.BURROWED || state == BurrowState.AUTOMATIC && player.IsBurrowed())
                    {
                        // Un-burrow, dealing 60/110/160/210/260 (+0.5) Physical Damage and knocking up nearby enemies for up to 1 second based on their proximity to Rek'Sai.
                        // A unit cannot be hit by Un-burrow more than once every 10 seconds.
                        if (!target.HasBurrowBuff())
                        {
                            damage = new float[] { 60, 110, 160, 210, 260 }[spellLevel] + 0.5f * player.TotalAttackDamage();
                        }
                    }

                    break;

                case SpellSlot.E:

                    if (state == BurrowState.UNBURROWED || state == BurrowState.AUTOMATIC && !player.IsBurrowed())
                    {
                        // Rek'Sai bites a target dealing undefined Physical Damage, increasing by up to 100% at maximum Fury. If Rek'Sai has 100 Fury, Furious Bite deals True Damage.
                        // Maximum Damage: undefined
                        damage = new float[] { 0.8f, 0.9f, 1, 1.1f, 1.2f }[spellLevel];
                        damage *= player.TotalAttackDamage();
                        damage *= (1 + player.ManaPercent / 100);
                        // True damage on full
                        if (player.HasMaxFury())
                            damageType = DamageType.True;
                    }

                    break;
            }

            // Return 0 on no damage
            if (damage == 0 && extraDamage == 0)
                return 0;

            // Calculate damage on target and return (-20 so it's actually accurate lol)
            return (float)player.CalcDamage(target, damageType, damage) + extraDamage - 20;
        }




    }

    public class ActiveModes
    {
        private static AIHeroClient player = ObjectManager.Player;

        private static Spell Q { get { return SpellManager.Q; } }
        private static Spell W { get { return SpellManager.W; } }
        private static Spell E { get { return SpellManager.E; } }
        private static Spell R { get { return SpellManager.R; } }

        public static void OnPermaActive()
        {
            // Re-enable auto attacks that might have been disabled
            LXOrbwalker.SetAttack(true);
        }


        

        public void OnCombo(bool afterAttack = false, Obj_AI_Base afterAttackTarget = null)
        {


            // TODO: Smite usage

            // Unburrowed
            if (!player.IsBurrowed())
            {
                // Config values
                var useQ = true;
                var useW = true;
                var useE = true;
                var useBurrowQ = true;



                // Validate spells we wanna use
                if ((useQ ? !Q.IsReady() : true) && (useW ? !W.IsReady() : true) && (useE ? !E.IsReady() : true))
                    return;

                // Get a low range target, since we don't have much range with our spells
                var target = ARAMTargetSelector.getBestTarget(useQ && Q.IsReady() ? Q.Range : E.Range);

                if (target != null)
                {

                    // General Q usage, we can safely spam that I guess
                    if (afterAttack && useQ && Q.IsReady())
                        Q.Cast(true);

                    // E usage, only cast on secure kill, full fury or our health is low
                    if (useE && E.IsReady() && (target.Health < E.GetDamage(target) || player.HasMaxFury() || player.IsLowHealth()))
                        E.Cast(target);
                }

                // Burrow usage
                if (target != null && useW && W.IsReady() && !player.HasQActive())
                {
                    if (target.CanBeKnockedUp())
                    {
                        W.Cast();
                    }
                    else if ((!useQ || !Q.IsReady()) && useBurrowQ && SpellManager.QBurrowed.IsReallyReady())
                    {
                        // Check if the player could make more attack attack damage than the Q damage, else cast W
                        if (Math.Floor(player.AttackSpeed()) * player.LSGetAutoAttackDamage(target) < SpellManager.QBurrowed.GetRealDamage(target))
                            W.Cast();
                    }
                }
            }
            // Burrowed
            else
            {
                // Disable auto attacks
               //LXOrbwalker.SetAttack(false);

                // Config values
                var useQ = true;
                var useW = true;
                var useE = true;
                var useNormalQ = true;
                var useNormalE = true;

                // General Q usage
                if (useQ && Q.IsReady())
                {
                    // Get a target at Q range
                    var target = ARAMTargetSelector.getBestTarget(Q.Range);

                    if (target != null)
                        Q.Cast(target);
                }

                // Gapclose with E, only for (almost) secured kills
                if (useE && E.IsReady())
                {
                    // Get targets that could be valid for our combo
                    var validRangeTargets = ObjectManager.Get<AIHeroClient>().Where(h => h.LSDistance(player, true) < Math.Pow(Q.Range + 150, 2) && h.LSDistance(player, true) > Math.Pow(Q.Range - 150, 2));

                    // Get a target that could die with our combo
                    var target = validRangeTargets.FirstOrDefault(t =>
                        t.Health <
                        W.GetRealDamage(t) +
                            // Let's say 2 AAs without Q and 4 AAs with Q
                        (SpellManager.QNormal.IsReallyReady(1000) ? SpellManager.QNormal.GetRealDamage(t) * 3 + player.LSGetAutoAttackDamage(t) : player.LSGetAutoAttackDamage(t) * 2) +
                        (SpellManager.ENormal.IsReallyReady(1000) ? SpellManager.ENormal.GetRealDamage(t) : 0)
                    );

                    if (target != null)
                    {
                        // Digg tunnel to target Kappa
                        E.Cast(target);
                    }
                    else
                    {
                        // Snipe Q targets, experimental, dunno if I'll leave this in here
                        if (useQ && Q.IsReallyReady(1000))
                        {
                            var snipeTarget = ARAMTargetSelector.getBestTarget(E.Range + Q.Range);
                            if (snipeTarget != null && snipeTarget.Health < Q.GetRealDamage(snipeTarget))
                            {
                                // Digg tunnel to the target direction
                                var prediction = E.GetPrediction(snipeTarget, false, float.MaxValue);
                                E.Cast(prediction.CastPosition);
                            }
                        }
                    }
                }

                // Check if we need to unburrow
                if (useW && ((useNormalQ ? SpellManager.QNormal.IsReallyReady(250) : true) || (useNormalE ? SpellManager.ENormal.IsReallyReady(250) : true)))
                {
                    // Get a target above the player that is within our spell range
                    var target = ARAMTargetSelector.getBestTarget(useNormalQ && SpellManager.QNormal.IsReallyReady(250) ? SpellManager.QNormal.Range : useNormalE ? SpellManager.ENormal.Range : Orbwalking.GetRealAutoAttackRange(player));
                    if (target != null)
                    {
                        // Unburrow
                        W.Cast();
                    }
                }
            }
        }

        

        public void OnWaveClear(bool afterAttack = false, Obj_AI_Base afterAttackTarget = null)
        {
            // TODO: Smite usage
            // Config values
            var useQ = false;
            var useE = true;

            // Unburrowed
            if (!player.IsBurrowed())
            {
                if (afterAttack && afterAttackTarget.Team != GameObjectTeam.Neutral)
                {
                    Console.WriteLine("FarmLine");

                    // Validate spells we wanna use
                    if ((useQ ? !Q.IsReady() : true) && (useE ? !E.IsReady() : true))
                        return;

                    // Get surrounding minions
                    var minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                    if (minions.Count > 0)
                    {
                        // Q usage
                        if (useQ && Q.IsReady())
                        {
                            // Check the number of Minions we would hit with Q,
                            // Bounce radius is 450 according to RitoDecode (thanks Husky Kappa)
                            if (minions.Where(m => m.LSDistance(player, true) < 450 * 450).Count() >= 2)
                                Q.Cast();
                        }

                        // E usage
                        if (useE && E.IsReady())
                        {
                            var target = minions.FirstOrDefault(m => player.HasMaxFury() || m.Health < E.GetRealDamage(m));
                            if (target != null)
                                E.Cast(target);
                        }
                    }
                }
            }
            // Burrowed
            else
            {
                // Disable auto attacks
               // LXOrbwalker.SetAttack(true);

                if (Q.IsReady())
                {
                    // Get the best position to shoot the Q
                    var location = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(Q.Range).Select(m => m.ServerPosition.LSTo2D()).ToList(), Q.Width, Q.Range);
                    if (location.MinionsHit > 0)
                        Q.Cast(location.Position);
                }
                else
                {
                    // Get minions above us
                    var minions = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(player), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                    if (minions.Count > 0)
                    {
                        // Unburrow
                        W.Cast();
                    }
                }
            }
        }

       
    }

    public static class Extensions
    {
        private const string TARGET_BURROW_NAME = "RekSaiKnockupImmune";
        private const string Q_ACTIVE_NAME = "RekSaiQ";

        private static AIHeroClient player = ObjectManager.Player;

        public enum BurrowState
        {
            BURROWED,
            UNBURROWED,
            AUTOMATIC
        }

        public static float GetRealDamage(this Spell spell, Obj_AI_Base target)
        {
            return GetRealDamage(spell.Slot, target, spell.Instance.Name.Contains("Burrowed") ? BurrowState.BURROWED : BurrowState.UNBURROWED);
        }

        public static float GetRealDamage(SpellSlot slot, Obj_AI_Base target, BurrowState state = BurrowState.AUTOMATIC)
        {
            // Damage holders
            float damage = 0;
            float extraDamage = 0;
            var damageType = DamageType.Physical;

            // Validate spell level
            var spellLevel = player.Spellbook.GetSpell(slot).Level;
            if (spellLevel == 0)
                return 0;
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:

                    if (state == BurrowState.UNBURROWED || state == BurrowState.AUTOMATIC && !player.IsBurrowed())
                    {
                        // Rek'Sai's next 3 basic attacks within 5 seconds deal 15/35/55/75/95 (+0.4) bonus Physical Damage to nearby enemies.
                        damage = new float[] { 15, 35, 55, 75, 95 }[spellLevel] + 0.4f * player.TotalAttackDamage();
                        extraDamage = (float)player.LSGetAutoAttackDamage(target);
                    }
                    else
                    {
                        // Rek'Sai launches a burst of void-charged earth that explodes on first unit hit, dealing 60/90/120/150/180 (+1) Magic Damage
                        // and revealing non-stealthed enemies hit for 2.5 seconds.
                        damageType = DamageType.Magical;
                        damage = new float[] { 60, 90, 120, 150, 180 }[spellLevel] + player.TotalMagicalDamage();
                    }

                    break;

                case SpellSlot.W:

                    if (state == BurrowState.BURROWED || state == BurrowState.AUTOMATIC && player.IsBurrowed())
                    {
                        // Un-burrow, dealing 60/110/160/210/260 (+0.5) Physical Damage and knocking up nearby enemies for up to 1 second based on their proximity to Rek'Sai.
                        // A unit cannot be hit by Un-burrow more than once every 10 seconds.
                        if (!target.HasBurrowBuff())
                        {
                            damage = new float[] { 60, 110, 160, 210, 260 }[spellLevel] + 0.5f * player.TotalAttackDamage();
                        }
                    }

                    break;

                case SpellSlot.E:

                    if (state == BurrowState.UNBURROWED || state == BurrowState.AUTOMATIC && !player.IsBurrowed())
                    {
                        // Rek'Sai bites a target dealing undefined Physical Damage, increasing by up to 100% at maximum Fury. If Rek'Sai has 100 Fury, Furious Bite deals True Damage.
                        // Maximum Damage: undefined
                        damage = new float[] { 0.8f, 0.9f, 1, 1.1f, 1.2f }[spellLevel];
                        damage *= player.TotalAttackDamage();
                        damage *= (1 + player.ManaPercent / 100);
                        // True damage on full
                        if (player.HasMaxFury())
                            damageType = DamageType.True;
                    }

                    break;
            }

            // Return 0 on no damage
            if (damage == 0 && extraDamage == 0)
                return 0;

            // Calculate damage on target and return (-20 so it's actually accurate lol)
            return (float)player.CalcDamage(target, damageType, damage) + extraDamage - 20;
        }


        public static bool HasMaxFury(this AIHeroClient target)
        {
            return target.Mana == target.MaxMana;
        }

        public static bool IsBurrowed(this AIHeroClient target)
        {
            return ObjectManager.Player.HasBuff("RekSaiW");
        }

        public static float TotalAttackDamage(this Obj_AI_Base target)
        {
            return target.BaseAttackDamage + target.FlatPhysicalDamageMod;
        }

        public static float TotalMagicalDamage(this Obj_AI_Base target)
        {
            return target.BaseAbilityDamage + target.FlatMagicDamageMod;
        }

        public static float AttackSpeed(this Obj_AI_Base target)
        {
            return 1 / target.AttackDelay;
        }


        public static float GetStunDuration(this Obj_AI_Base target)
        {
            return target.Buffs.Where(b => b.IsActive && Game.Time < b.EndTime &&
                (b.Type == BuffType.Charm ||
                b.Type == BuffType.Knockback ||
                b.Type == BuffType.Stun ||
                b.Type == BuffType.Suppression ||
                b.Type == BuffType.Snare)).Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) -
                Game.Time;
        }

        public static bool IsLowHealth(this Obj_AI_Base target)
        {
            return target.HealthPercent < 10;
        }

        public static bool HasQActive(this AIHeroClient target)
        {
            if (!target.IsMe)
                return false;

            return target.HasBuff(Q_ACTIVE_NAME);
        }

        public static bool HasBurrowBuff(this Obj_AI_Base target)
        {
            return target.HasBuff(TARGET_BURROW_NAME);
        }

        public static BuffInstance GetBurrowBuff(this Obj_AI_Base target)
        {
            return target.Buffs.FirstOrDefault(b => b.DisplayName == TARGET_BURROW_NAME);
        }

        public static int GetAliveEnemiesInRange(this Vector3 pos, float range)
        {
            return ObjectManager.Get<AIHeroClient>()
                    .Count(ene => ene.IsEnemy && ene.IsValid && !ene.IsDead && ene.LSDistance(pos, true) < range * range); ;
        }

        public static float GetBurrowBuffDuration(this Obj_AI_Base target)
        {
            var buff = target.GetBurrowBuff();
            if (buff != null)
                return buff.EndTime - Game.Time;

            return 0f;
        }
        public static bool IsUnder(this AIHeroClient player, Obj_AI_Base target)
        {
            return player.IsBurrowed()
                && target.LSDistance(player.Position) < 260;
        }
        public static bool QActive(this AIHeroClient Hero)
        {
            return Hero.Buffs.Any(buff => buff.IsValidBuff()
                && buff.DisplayName.ToLowerInvariant() == "reksaiq"
                && Hero.NetworkId == buff.Caster.NetworkId);
        }

        public static bool CanBeKnockedUp(this Obj_AI_Base target)
        {
            return target != null && ObjectManager.Player.IsBurrowed() ? target.GetBurrowBuffDuration() == 0 : target.GetBurrowBuffDuration() < 1;
        }
    }

    public static class SpellManager
    {
        private static AIHeroClient player = ObjectManager.Player;

        private static Spell _r;
        private static Spell _qNormal, _wNormal, _eNormal;
        private static Spell _qBurrowed, _wBurrowed, _eBurrowed;

        public static Spell QNormal
        {
            get { return _qNormal; }
        }
        public static Spell WNormal
        {
            get { return _wNormal; }
        }
        public static Spell ENormal
        {
            get { return _eNormal; }
        }

        public static Spell QBurrowed
        {
            get { return QBurrowed; }
        }
        public static Spell WBurrowed
        {
            get { return _wBurrowed; }
        }
        public static Spell EBurrowed
        {
            get { return _eBurrowed; }
        }

        public static Spell Q
        {
            get { return player.IsBurrowed() ? QBurrowed : QNormal; }
        }
        public static Spell W
        {
            get { return player.IsBurrowed() ? WBurrowed : WNormal; }
        }
        public static Spell E
        {
            get { return player.IsBurrowed() ? EBurrowed : ENormal; }
        }
        public static Spell R
        {
            get { return _r; }
        }

        private static Dictionary<string, float[]> cooldowns;
        private static readonly Dictionary<Spell, float> cooldownExpires = new Dictionary<Spell, float>();

        private static bool smiteSearched = false;
        private static bool hasSmite = false;

        public static void Initialize()
        {
            // General
            _r = new Spell(SpellSlot.R);

            // Unburrowed
            _qNormal = new Spell(SpellSlot.Q, 300);
            _wNormal = new Spell(SpellSlot.W, 0);
            _eNormal = new Spell(SpellSlot.E, 250);

            // Burrowed
            _qBurrowed = new Spell(SpellSlot.Q, 1500);
            _wBurrowed = new Spell(SpellSlot.W, 0);
            _eBurrowed = new Spell(SpellSlot.E, 750);

            // Finetune spells
            QBurrowed.SetSkillshot(0.125f, 60, 4000, true, SkillshotType.SkillshotLine);
            EBurrowed.SetSkillshot(0, 60, 1600, false, SkillshotType.SkillshotLine);

            // Initialize cooldowns
            cooldowns = new Dictionary<string, float[]>()
            {
                { "RekSaiQ", new float[] { 4, 4, 4, 4, 4 } },
                { "RekSaiE", new float[] { 12, 12, 12, 12, 12 } },
                { "RekSaiR", new float[] { 150, 110, 80 } },
                { "reksaiqburrowed", new float[] { 11, 10, 9, 8, 7 } },
                { "reksaieburrowed", new float[] { 20, 19.5f, 19, 18.5f, 18 } },
            };

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }


        private static Spell GetSpellFromSlot(SpellSlot slot)
        {
            return slot == SpellSlot.Q ? Q : slot == SpellSlot.W ? W : slot == SpellSlot.E ? E : slot == SpellSlot.R ? R : null;
        }


        public static float Cooldown(this Spell spell)
        {
            if (cooldownExpires.ContainsKey(spell))
                return cooldownExpires[spell] - Game.Time;

            return 0;
        }

        public static bool IsReallyReady(this Spell spell, int timeInMillis = 0)
        {
            if (cooldownExpires.ContainsKey(spell))
                return spell.Cooldown() - (float)timeInMillis / 1000f <= 0;

            return true;
        }

        public static SpellDataInst GetSmiteSpell(this AIHeroClient target)
        {
            return target.Spellbook.Spells.FirstOrDefault(s => s.Name.ToLower().Contains("smite"));
        }

        public static bool HasSmite()
        {
            if (!smiteSearched)
            {
                smiteSearched = true;
                hasSmite = player.GetSmiteSpell() != null;
            }
            return hasSmite;
        }


        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                var spell = GetSpellFromSlot(args.Slot);

                switch (args.Slot)
                {
                    // Special cases for W, it has a fixed cooldown of 1 and 4
                    case SpellSlot.W:

                        if (player.IsBurrowed())
                            cooldownExpires[WNormal] = Game.Time + 4;
                        else
                            cooldownExpires[WBurrowed] = Game.Time + 1;
                        break;

                    case SpellSlot.Q:
                    case SpellSlot.E:
                    case SpellSlot.R:

                        cooldownExpires[spell] = Game.Time + cooldowns[spell.Instance.Name][spell.Level - 1] * (1 + player.PercentCooldownMod);
                        break;
                }
            }
        }
    }
}
