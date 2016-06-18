using System.Linq;
using System.Collections.Generic;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using WuAIO.Bases;

namespace WuAIO.Managers
{
    class DamageManager
    {
        private readonly AIHeroClient Player = EloBuddy.Player.Instance;

        private bool _enabled { get { return MenuManager.Menus[HeroBase.HERO_MENU]["Drawings"].Keys.Last().IsActive("damageindicator"); } }
        
        private Item _shiv, _trinity, _sheen, _echo, _iceborn;

        public readonly List<Bases.Damage> _damages;

        public enum ScalingTypes
        {
            AD = 0, ADBonus = 1, AP = 2, APBonus = 3, Armor = 4, Speed = 5,
            TargetLostLife = 6, TargetAP = 7, TargetAPBonus = 8
            
        };
        
        public DamageManager(List<Bases.Damage> damages)
        {
            _damages = damages;

            //Initializing itens
            _shiv = new Item(ItemId.Statikk_Shiv);
            _trinity = new Item(ItemId.Trinity_Force);
            _sheen = new Item(ItemId.Sheen);
            _echo = new Item(ItemId.Ludens_Echo);
            _iceborn = new Item(ItemId.Iceborn_Gauntlet);

            //Damage Indicator
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        public float SpellDamage(Obj_AI_Base target, SpellSlot slot, float addedDamage = -20)
        {
            if (target == null || !target.IsValidTarget() || !_damages.Any(it => it.spell.Slot == slot)) return 0;

            var damage = new float();

            foreach (var dmg in _damages.Where(it => it.spell.Slot == slot))
            {
                var BonusDamage = new float();

                if (dmg.scales != null)
                {
                    foreach (var scale in dmg.scales)
                    {
                        BonusDamage += scale.scaling[dmg.spell.Level] * GetProperty(scale.scalingType, target);
                    }
                }

                /*if (_echo.IsOwned() && _echo.ItemInfo.Stacks == 100 && dmg.dealtDamageType == DamageType.Magical)
                {
                    BonusDamage += 100 + (0.1f * GetProperty(ScalingTypes.AP));
                }*/

                if (dmg.applyOnHitEffects)
                {
                    if (_trinity.IsOwned() && (_trinity.IsReady() || Player.HasBuff("sheen"))) BonusDamage += 2 * Player.BaseAttackDamage;
                    else if (_sheen.IsOwned() && (_sheen.IsReady() || Player.HasBuff("sheen"))) BonusDamage += Player.BaseAttackDamage;

                    if (_iceborn.IsOwned() && ((_iceborn.IsReady() && !Player.HasBuff("itemfrozenfist")) || Player.HasBuff("itemfrozenfist"))) BonusDamage += 1.25f * Player.BaseAttackDamage;

                    /*if (_shiv.IsOwned() && _shiv.Stacks == 100)
                    {
                        if (target.IsMinion)
                            BonusDamage += (154 / 17) * Player.Level + (66 - (154 / 17));
                            //f(x) = (154/17)x + (66 - (154/17))
                        else
                            BonusDamage += (70 / 17) * Player.Level + (30 - (70 / 17));
                            //f(x) = (70/17)x + (30 - (70/17))
                    }*/
                }

                damage += Player.CalculateDamageOnUnit(target, dmg.dealtDamageType, dmg.baseDamage[dmg.spell.Level] + BonusDamage, dmg.isAbility, dmg.isAAorTargeted);
                damage += addedDamage;
            }

            return damage;
        }

        public float GetComboDamage(Obj_AI_Base target)
        {
            var damage = new float();

            damage += SpellDamage(target, SpellSlot.Q);
            damage += SpellDamage(target, SpellSlot.W);
            damage += SpellDamage(target, SpellSlot.E);
            damage += SpellDamage(target, SpellSlot.R);

            return damage;
        }

        public float GetSpellBonusDamage(SpellSlot slot)
        {
            if (!_damages.Any(it => it.spell.Slot == slot)) return 0;

            var scaleDamage = new float();

            foreach (var dmg in _damages.Where(it => it.spell.Slot == slot))
            {
                foreach (var scale in dmg.scales)
                {
                    scaleDamage += scale.scaling[dmg.spell.Level] * GetProperty(scale.scalingType);
                }
            }

            return scaleDamage;
        }

        public float GetSpellBaseDamage(SpellSlot slot)
        {
            if (!_damages.Any(it => it.spell.Slot == slot)) return 0;

            var baseDamage = new float();

            foreach (var dmg in _damages.Where(it => it.spell.Slot == slot))
            {
                baseDamage += dmg.baseDamage[dmg.spell.Level];
            }

            return baseDamage;
        }

        private float GetProperty(ScalingTypes type, Obj_AI_Base target = null)
        {
            switch (type)
            {
                case ScalingTypes.AD:
                    return Player.TotalAttackDamage;

                case ScalingTypes.ADBonus:
                    return Player.FlatPhysicalDamageMod;

                case ScalingTypes.AP:
                    return Player.TotalMagicalDamage;

                case ScalingTypes.APBonus:
                    return Player.FlatMagicDamageMod;

                case ScalingTypes.Armor:
                    return Player.Armor;

                case ScalingTypes.Speed:
                    return Player.MoveSpeed;

                case ScalingTypes.TargetLostLife:
                    return target.MaxHealth - target.Health;

                case ScalingTypes.TargetAP:
                    return target.TotalMagicalDamage;

                case ScalingTypes.TargetAPBonus:
                    return target.FlatMagicDamageMod;

                default:
                    return 0;

            }
        }

        private void Drawing_OnEndScene(System.EventArgs args)
        {
            if (!_enabled) return;

            foreach (var enemy in EntityManager.Heroes.Enemies.Where(it => it.IsValidTarget() && !it.IsDead && it.IsHPBarRendered))
            {
                float FutureDamage = GetComboDamage(enemy) > enemy.Health ? -1 : GetComboDamage(enemy) / enemy.MaxHealth;

                if (FutureDamage == -1)
                {
                    Drawing.DrawText(enemy.Position.WorldToScreen().X - 30, enemy.Position.WorldToScreen().Y - 150, Color.Yellow, "Killable");
                    continue;
                }
                
                Line.DrawLine
                (
                    Color.LightSkyBlue, 9f,
                    new Vector2(enemy.HPBarPosition.X + 1, enemy.HPBarPosition.Y + 9),
                    new Vector2(enemy.HPBarPosition.X + 1 + FutureDamage * 104, enemy.HPBarPosition.Y + 9)
                );
            }

            return;
        }
    }
}
