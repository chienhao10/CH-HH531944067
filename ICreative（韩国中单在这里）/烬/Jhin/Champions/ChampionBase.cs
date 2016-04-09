using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using Jhin;
using Jhin.Managers;
using Jhin.Model;
using Jhin.Utilities;
using SharpDX;

namespace Jhin.Champions
{
    public class ChampionBase
    {
        public const float RefreshTime = 0.4f;

        public readonly Dictionary<int, BestDamageResult> PredictedDamage = new Dictionary<int, BestDamageResult>();
        public SpellBase E;

        public SpellBase Q;
        public SpellBase R;
        public SpellBase R2;

        public float Range = 1200f;

        public AIHeroClient Target;
        public SpellBase W;
        public SpellBase W2;
        public ChampionBase()
        {
            MenuManager.Initialize();
            Game.OnTick += delegate
            {
                if (MyHero.IsDead || !AIO.Initialized)
                {
                    return;
                }
                PermaActive();
                KillSteal();
                /*
                if (Orbwalker.GotAutoAttackReset && Orbwalker.GetTarget() != null)
                {
                    return;
                }
                */
                if (ModeManager.Combo)
                {
                    Combo();
                }
                else if (ModeManager.Harass)
                {
                    Harass();
                }
                else if (ModeManager.LaneClear || ModeManager.JungleClear)
                {
                    if (ModeManager.LaneClear)
                    {
                        LaneClear();
                    }
                    if (ModeManager.JungleClear)
                    {
                        JungleClear();
                    }
                }
                else if (ModeManager.LastHit)
                {
                    LastHit();
                }
                if (ModeManager.Flee)
                {
                    Flee();
                }
            };
        }

        public AIHeroClient MyHero
        {
            get { return AIO.MyHero; }
        }

        public Vector3 MousePos
        {
            get { return Game.CursorPos; }
        }

        protected Menu Menu
        {
            get { return MenuManager.Menu; }
        }

        public Menu ComboMenu
        {
            get { return MenuManager.GetSubMenu("Combo"); }
        }

        public Menu KillStealMenu
        {
            get { return MenuManager.GetSubMenu("KillSteal"); }
        }

        public Menu HarassMenu
        {
            get { return MenuManager.GetSubMenu("Harass"); }
        }

        public Menu ClearMenu
        {
            get { return MenuManager.GetSubMenu("Clear"); }
        }

        public Menu PredictionMenu
        {
            get { return MenuManager.GetSubMenu("Prediction"); }
        }

        public Menu DrawingsMenu
        {
            get { return MenuManager.GetSubMenu("Drawings"); }
        }

        public Menu FleeMenu
        {
            get { return MenuManager.GetSubMenu("Flee"); }
        }

        public Menu AutomaticMenu
        {
            get { return MenuManager.GetSubMenu("Automatic"); }
        }

        public Menu MiscMenu
        {
            get { return MenuManager.GetSubMenu("Misc"); }
        }

        public Menu KeysMenu
        {
            get { return MenuManager.GetSubMenu("Keys"); }
        }

        public Menu EvaderMenu
        {
            get { return MenuManager.GetSubMenu("Evader"); }
        }
        public Menu UltimateMenu
        {
            get { return MenuManager.GetSubMenu("Ultimate"); }
        }

        public SpellBase Ignite
        {
            get { return SpellManager.Ignite; }
        }

        public SpellBase Heal
        {
            get { return SpellManager.Heal; }
        }

        public SpellBase Smite
        {
            get { return SpellManager.Smite; }
        }

        public SpellBase Flash
        {
            get { return SpellManager.Flash; }
        }


        protected virtual void PermaActive()
        {
        }

        protected virtual void Combo()
        {
        }

        protected virtual void Harass()
        {
        }

        protected virtual void LaneClear()
        {
            LastHit();
        }

        protected virtual void JungleClear()
        {
        }

        protected virtual void LastHit()
        {
        }

        protected virtual void KillSteal()
        {
            foreach (var enemy in UnitManager.ValidEnemyHeroesInRange)
            {
                if (KillStealMenu.CheckBox("Ignite") && Ignite.IsKillable(enemy))
                {
                    if (!MyHero.InAutoAttackRange(enemy) ||
                        MyHero.GetAttackDamage(enemy, true) <= enemy.TotalShieldHealth())
                    {
                        Ignite.Cast(enemy);
                    }
                }
                if (KillStealMenu.CheckBox("Smite") && Smite.IsKillable(enemy))
                {
                    Smite.Cast(enemy);
                }
            }
        }

        protected virtual void Flee()
        {
        }

        public virtual void OnDraw()
        {
        }

        public virtual void OnEndScene()
        {
        }


        protected virtual float GetComboDamage(Obj_AI_Base target, IEnumerable<SpellBase> list)
        {
            return 2f*MyHero.GetAttackDamage(target, true) + list.Sum(spell => spell.GetDamage(target));
        }

        public virtual BestDamageResult GetBestCombo(Obj_AI_Base target)
        {
            if (target != null)
            {
                var canBeCalculated = FpsBooster.CanBeExecuted(CalculationType.Damage);
                if (!PredictedDamage.ContainsKey(target.NetworkId))
                {
                    PredictedDamage[target.NetworkId] = new BestDamageResult {Target = target};
                    canBeCalculated = false;
                }
                var bestDamage = PredictedDamage[target.NetworkId];
                if (!canBeCalculated)
                {
                    return bestDamage;
                }
                bestDamage = new BestDamageResult {Target = target};
                foreach (var r1 in R.IsReady ? new[] {false, true} : new[] {false})
                {
                    var list = new List<SpellBase> {Smite, Ignite};
                    var manaWasted = 0f;
                    if (r1)
                    {
                        list.Add(R);
                        manaWasted = R.Mana;
                    }
                    foreach (var q1 in Q.IsReady ? new[] {false, true} : new[] {false})
                    {
                        if (q1)
                        {
                            list.Add(Q);
                            manaWasted += Q.Mana;
                        }
                        foreach (var w1 in W.IsReady ? new[] {false, true} : new[] {false})
                        {
                            if (w1)
                            {
                                list.Add(W);
                                manaWasted += W.Mana;
                            }
                            foreach (var e1 in E.IsReady ? new[] {false, true} : new[] {false})
                            {
                                if (e1)
                                {
                                    list.Add(E);
                                    manaWasted += E.Mana;
                                }
                                if (manaWasted <= MyHero.Mana)
                                {
                                    var result = GetComboDamage(target, list);
                                    if (bestDamage.Damage >= target.TotalShieldHealth())
                                    {
                                        if (result >= target.TotalShieldHealth() &&
                                            (result < bestDamage.Damage || manaWasted < bestDamage.Mana ||
                                             (bestDamage.R && !r1)))
                                        {
                                            bestDamage.Damage = result;
                                            bestDamage.Mana = manaWasted;
                                            bestDamage.List = list;
                                        }
                                    }
                                    else
                                    {
                                        if (result >= bestDamage.Damage)
                                        {
                                            bestDamage.Damage = result;
                                            bestDamage.Mana = manaWasted;
                                            bestDamage.List = list;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                PredictedDamage[target.NetworkId] = bestDamage;
                return PredictedDamage[target.NetworkId];
            }
            return new BestDamageResult {Damage = 0, Mana = 0};
        }

        public virtual float GetSpellDamage(SpellSlot slot, Obj_AI_Base target)
        {
            if (slot != SpellSlot.Unknown)
            {
                if (slot == SpellSlot.Summoner1 || slot == SpellSlot.Summoner2)
                {
                    if (Ignite.Slot == slot)
                    {
                        return MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                    }
                    if (Smite.Slot == slot)
                    {
                        return MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
                    }
                    return 0f;
                }
                return AIO.MyHero.GetSpellDamage(target, slot);
            }
            return 0f;
        }
    }
}