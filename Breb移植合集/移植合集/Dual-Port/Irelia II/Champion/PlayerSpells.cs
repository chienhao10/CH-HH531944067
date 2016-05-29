using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace Irelia.Champion
{
    public static class PlayerSpells
    {
        public static List<LeagueSharp.Common.Spell> SpellList = new List<LeagueSharp.Common.Spell>();

        public static LeagueSharp.Common.Spell Q, W, E, R;

        public static int LastAutoAttackTick;

        public static int LastQCastTick;

        public static int LastECastTick;

        public static int LastSpellCastTick;

        public static void Init()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 660f);
            Q.SetTargetted(0f, 2200);

            W = new LeagueSharp.Common.Spell(SpellSlot.W);

            E = new LeagueSharp.Common.Spell(SpellSlot.E, 325);
            E.SetSkillshot(0.15f, 75f, 1500f, false, SkillshotType.SkillshotCircle);

            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1000f);
            R.SetSkillshot(0.15f, 120f, 1600f, false, SkillshotType.SkillshotLine);

            SpellList.AddRange(new[] { Q, W, E, R });

            Game.OnUpdate += GameOnOnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;
        }

        public static void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!unit.IsMe)
            {
                return;
            }
            if (spell.SData.Name.Contains("summoner"))
            {
                return;
            }
            //Game.PrintChat(spell.SData.Name);
            if (spell.SData.Name.ToLower().Contains("attack"))
            {
                LastAutoAttackTick = Environment.TickCount;

            }

            //if (!spell.SData.Name.ToLower().Contains("attack"))
            //{
            //    LastSpellCastTick = Environment.TickCount;
            //    Orbwalking.ResetAutoAttackTimer();
            //}


            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    {
                        LastQCastTick = Environment.TickCount;
                        LastSpellCastTick = Environment.TickCount;
                        Orbwalker.ResetAutoAttack();
                        break;
                    }

                case SpellSlot.E:
                    {
                        LastECastTick = Environment.TickCount;
                        LastSpellCastTick = Environment.TickCount;
                        Orbwalker.ResetAutoAttack();
                        break;
                    }

                case SpellSlot.R:
                    {
                        LastQCastTick = Environment.TickCount;
                        LastSpellCastTick = Environment.TickCount;
                        Orbwalker.ResetAutoAttack();
                        break;
                    }
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (getBoxItem(Modes.ModeSettings.MenuSettingE, "Settings.E.Auto") == 1)
                //getBoxItem(Modes.ModeSettings.MenuSettingQ, "Settings.Q.CastDelay")
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if (t.IsValidTarget())
                    {
                        CastECombo(t);
                    }
                }
            }
        }

        public static void CastQObjects(Obj_AI_Base t)
        {
            if (!Q.CanCast(t))
            {
                return;
            }

            if (Environment.TickCount - LastQCastTick >= (getBoxItem(Modes.ModeSettings.MenuSettingQ, "Settings.Q.CastDelay") + 1) * 250)
            {
                Q.CastOnUnit(t);
            }
        }

        public static void CastQCombo(Obj_AI_Base t)
        {
            //if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            //{
            //    Game.PrintChat("Shen Active!");
            //    return;
            //}

            if (!Q.CanCast(t))
            {
                return;
            }

            if (getBoxItem(Modes.ModeCombo.MenuLocal, "Combo.Mode") == 1 && LastAutoAttackTick < LastSpellCastTick && t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            {
                return;
            }

            if (Environment.TickCount - LastQCastTick < (getBoxItem(Modes.ModeSettings.MenuSettingQ, "Settings.Q.CastDelay") + 1) * 250)
            {
                return;
            }

            foreach (var enemy in Common.AutoBushHelper.EnemyInfo.Where(
                x =>
                    Q.CanCast(x.Player)
                    && Environment.TickCount - x.LastSeenForE >= (getBoxItem(Modes.ModeSettings.MenuSettingQ, "Settings.Q.VisibleDelay") + 1) * 250
                    && x.Player.NetworkId == t.NetworkId).Select(x => x.Player).Where(enemy => enemy != null))
            {
                Q.CastOnUnit(t);
                LastQCastTick = Environment.TickCount;
            }
        }

        public static void CastECombo(Obj_AI_Base t)
        {
            //if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            //{
            //    Game.PrintChat("Shen Active!");
            //    return;
            //}

            if (!E.CanCast(t))
            {
                return;
            }

            foreach (var enemy in Common.AutoBushHelper.EnemyInfo.Where(
                x =>
                    E.CanCast(x.Player) &&
                    Environment.TickCount - x.LastSeenForE >= (getBoxItem(Modes.ModeSettings.MenuSettingE, "Settings.E.VisibleDelay") + 1) * 250 &&
                    x.Player.NetworkId == t.NetworkId).Select(x => x.Player))
            {
                if (enemy != null)
                {
                    E.CastOnUnit(t);
                    LastECastTick = Environment.TickCount;
                }
            }
        }
    }
}
