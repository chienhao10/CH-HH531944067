using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Linq;

namespace KappaLeBlanc
{
    static class Extensions
    {
        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        public static void CreateStringMenu(this Menu menu, string[] list, string slidername, int defaultvalue = 0, int minvalue = 0)
        {
            var maxvalue = minvalue + list.Length - 1;
            if (defaultvalue < minvalue || defaultvalue > maxvalue)
                defaultvalue = minvalue;

            var a = menu.Add(slidername.ToString(), new Slider(list[defaultvalue - minvalue], defaultvalue, minvalue, maxvalue));
            a.OnValueChange += delegate
            (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs Args)
            {
                sender.DisplayName = list[Args.NewValue - minvalue];
            };
        }
        public static bool IsWall(this Vector2 pos)
        {
            if (!pos.IsValid())
                return false;

            var ipos = pos.ToNavMeshCell().CollFlags;
            if (ipos.HasFlag(CollisionFlags.Wall) || ipos.HasFlag(CollisionFlags.Building))
            {
                return true;
            }
            return false;
        }
        public static bool IsWall(this Vector3 pos)
        {
            if (!pos.IsValid())
                return false;

            var ipos = pos.ToNavMeshCell().CollFlags;
            if (ipos.HasFlag(CollisionFlags.Wall) || ipos.HasFlag(CollisionFlags.Building))
            {
                return true;
            }
            return false;
        }
        public static float GetCooldown(this Spell.SpellBase spell)
        {
            return spell.Handle.CooldownExpires - Game.Time;
        }
        public static float GetDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return spell.Slot.GetDamage(target);
        }
        public static float GetDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            var spellLevel = Player.Instance.Spellbook.GetSpell(slot).Level;
            float damage = 0;
            if (spellLevel == 0)
            {
                return 0;
            }
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:
                    damage = new float[] { 55, 80, 105, 130, 155 }[spellLevel] + 0.4f * Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.W:
                    damage = new float[] { 85, 125, 165, 205, 245 }[spellLevel] + 0.6f * Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.E:
                    damage = new float[] { 40, 65, 90, 115, 140 }[spellLevel] + 0.5f * Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.R:
                    switch(Lib.R.Name)
                    {
                        case "LeblancChaosOrbM":
                            damage = new float[] { 100, 200, 300 }[spellLevel] + 0.6f * Player.Instance.TotalMagicalDamage;
                            break;
                        case "LeblancSoulShackleM":
                            damage = new float[] { 100, 200, 300 }[spellLevel] + 0.6f * Player.Instance.TotalMagicalDamage;
                            break;
                        case "LeblancSlideM":
                            damage = new float[] { 150, 300, 450 }[spellLevel] + 0.9f * Player.Instance.TotalMagicalDamage;
                            break;
                    }
                    break;
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, damage) - 30;
        }
        public static float GetComboDamage(this AIHeroClient target)
        {
            float damage = 0;

            var WReady = !Program.RavenForm && Lib.W.IsReady();
            var RReady = Lib.R.Name.Equals("LeblancChaosOrbM") || Lib.R.Name.Equals("LeblancSoulShackleM") || Lib.R.Name.Equals("LeblancSlideM");

            damage += Player.Instance.GetAutoAttackDamage(target);

            if (Helper.CastCheckbox(LBMenu.ComboM, "Q") && Lib.Q.IsReady())
            {
                if (
                    (Helper.CastCheckbox(LBMenu.ComboM, "W") && WReady) ||
                    (Helper.CastCheckbox(LBMenu.ComboM, "E") && Lib.E.IsReady()) ||
                    (Helper.CastCheckbox(LBMenu.ComboM, "R") && RReady)
                    )
                    damage += Lib.Q.GetDamage(target);
                damage += Lib.Q.GetDamage(target);
            }
            if (Helper.CastCheckbox(LBMenu.ComboM, "W") && WReady)
            {
                damage += Lib.W.GetDamage(target);
            }
            if (Helper.CastCheckbox(LBMenu.ComboM, "E") && Lib.E.IsReady())
            {
                damage += Lib.E.GetDamage(target) * 2;
            }
            if (Helper.CastCheckbox(LBMenu.ComboM, "R") && RReady)
            {
                damage += Lib.R.GetDamage(target);
            }
            return damage;
        }
    }
}
