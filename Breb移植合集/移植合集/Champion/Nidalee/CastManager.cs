using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using KL = KurisuNidalee.KurisuLib;
using KN = KurisuNidalee.KurisuNidalee;
using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using SkillshotType = SebbyLib.Prediction.SkillshotType;
using Spell = LeagueSharp.Common.Spell;

namespace KurisuNidalee
{
    internal class CastManager
    {
        // Human Q Logic

        public static Menu qHMenu = KN.qHMenu,
            wHMenu = KN.wHMenu,
            eHMenu = KN.eHMenu,
            rHMenu = KN.rHMenu,
            qCMenu = KN.qCMenu,
            wCMenu = KN.wCMenu,
            eCMenu = KN.eCMenu,
            rCMenu = KN.rCMenu,
            drawMenu = KN.drawMenu,
            jungleMenu = KN.jungleMenu,
            autoMenu = KN.autoMenu;

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

        internal static void CastJavelin(Obj_AI_Base target, string mode)
        {
            // if not harass mode ignore mana check
            if (!KL.CatForm() && KL.CanUse(KL.Spells["Javelin"], true, mode))
            {
                if (mode != "ha" || KL.Player.ManaPercent > 65)
                {
                    if (target.IsValidTarget(KL.Spells["Javelin"].Range))
                    {
                        if (target.IsChampion())
                        {
                            var qoutput = KL.Spells["Javelin"].GetPrediction(target);

                            if (getCheckBoxItem(qHMenu, "ndhqcheck"))
                            {
                                switch (getBoxItem(KN.Root, "ppred"))
                                {
                                    case 1:
                                        var pi = new PredictionInput
                                        {
                                            Aoe = false,
                                            Collision = true,
                                            Speed = 1300f,
                                            Delay = 0.25f,
                                            Range = 1500f,
                                            From = KN.Player.ServerPosition,
                                            Radius = 40f,
                                            Unit = target,
                                            Type = SkillshotType.SkillshotLine
                                        };

                                        var po = Prediction.GetPrediction(pi);
                                        if (po.Hitchance == (SebbyLib.Prediction.HitChance) (getBoxItem(KN.Root, "ndhqch") + 3))
                                        {
                                            KL.Spells["Javelin"].Cast(po.CastPosition);
                                        }

                                        break;

                                    case 2:
                                        var so = KL.Spells["Javelin"].GetPrediction((AIHeroClient) target);
                                        if (so.Hitchance == (HitChance) (getBoxItem(KN.Root, "ndhqch") + 3))
                                        {
                                            KL.Spells["Javelin"].Cast(so.CastPosition);
                                        }
                                        break;

                                    case 0:
                                        var co = KL.Spells["Javelin"].GetPrediction(target);
                                        if (co.Hitchance == (HitChance) (getBoxItem(KN.Root, "ndhqch") + 3))
                                        {
                                            KL.Spells["Javelin"].Cast(co.CastPosition);
                                        }
                                        break;
                                }
                            }

                            if (qoutput.Hitchance == HitChance.Collision && KL.Smite.IsReady())
                            {
                                if (getCheckBoxItem(KN.qHMenu, "qsmcol") && target.Health <= KL.CatDamage(target) * 3)
                                {
                                    if (qoutput.CollisionObjects.All(i => i.NetworkId != KL.Player.NetworkId))
                                    {
                                        if (qoutput.CollisionObjects.Cast<AIHeroClient>().Any())
                                        {
                                            return;
                                        }
                                        var obj = qoutput.CollisionObjects.Cast<Obj_AI_Minion>().ToList();
                                        if (obj.Count == 1)
                                        {
                                            if (obj.Any(i => i.Health <= KL.Player.GetSummonerSpellDamage(i, Damage.SummonerSpell.Smite) && KL.Player.Distance(i) < 500 && KL.Player.Spellbook.CastSpell(KL.Smite, obj.First())))
                                            {
                                                KL.Spells["Javelin"].Cast(qoutput.CastPosition);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }

                            if (!getCheckBoxItem(qHMenu, "ndhqcheck"))
                                KL.Spells["Javelin"].Cast(target);
                        }
                        else
                        {
                            KL.Spells["Javelin"].Cast(target);
                        }
                    }
                }
            }
        }

        // Human W Logic
        internal static void CastBushwhack(Obj_AI_Base target, string mode)
        {
            if (target == null)
            {
                return;
            }
            // if not harass mode ignore mana check
            if (!KL.CatForm() && KL.CanUse(KL.Spells["Bushwhack"], true, mode))
            {
                if (KL.Player.ManaPercent <= 65 && target.IsHunted() && target.CanMove)
                {
                    return;
                }

                if (mode != "ha" || KL.Player.ManaPercent > 65)
                {
                    if (target.IsValidTarget(KL.Spells["Bushwhack"].Range))
                    {
                        // try bushwhack prediction
                        if (getBoxItem(wHMenu, "ndhwforce") == 0)
                        {
                            if (target.IsChampion())
                                KL.Spells["Bushwhack"].CastIfHitchanceEquals(target, HitChance.VeryHigh);
                            else
                                KL.Spells["Bushwhack"].Cast(target.ServerPosition);
                        }

                        // try bushwhack behind target
                        if (getBoxItem(wHMenu, "ndhwforce") == 1)
                        {
                            var unitpos = KL.Spells["Bushwhack"].GetPrediction(target).UnitPosition;
                            KL.Spells["Bushwhack"].Cast(unitpos.Extend(KL.Player.ServerPosition, -75f));
                        }
                    }
                }
            }
        }


        // Cougar Q Logic
        internal static void CastTakedown(Obj_AI_Base target, string mode)
        {
            if (KL.CatForm() && KL.CanUse(KL.Spells["Takedown"], false, mode))
            {
                if (target.IsValidTarget(KL.Player.AttackRange + KL.Spells["Takedown"].Range))
                {
                    KL.Spells["Takedown"].CastOnUnit(target);
                }
            }
        }

        // Cougar W Logic
        internal static void CastPounce(Obj_AI_Base target, string mode)
        {
            // check the actual spell timer and if we have it enabled in our menu
            if (!KL.CatForm() || !KL.CanUse(KL.Spells["Pounce"], false, mode))
                return;

            // check if target is hunted in 750 range
            if (!target.IsValidTarget(KL.Spells["ExPounce"].Range))
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (KL.Player.HealthPercent <= getSliderItem(wCMenu, "ndcwcHPChecl"))
                {
                    return;
                }

                if (KL.Player.CountEnemiesInRange(750) >= getSliderItem(wCMenu, "ndcwcEnemy") && target.IsHunted())
                {
                    return;
                }
            }

            if (target.IsHunted())
            {
                // get hitbox
                var radius = KL.Player.AttackRange + KL.Player.LSDistance(KL.Player.BBox.Minimum) + 1;

                // force pounce if menu item enabled
                if (target.IsHunted() && getCheckBoxItem(wCMenu, "ndcwhunt") ||

                    // or of target is greater than my attack range
                    target.LSDistance(KL.Player.ServerPosition) > radius ||

                    // or is jungling or waveclearing (without farm distance check)
                    mode == "jg" || mode == "wc" && !getCheckBoxItem(wCMenu, "ndcwdistwc") ||

                    // or combo mode and ignoring distance check
                    !target.IsHunted() && mode == "co" && !getCheckBoxItem(wCMenu, "ndcwdistco"))
                {
                    if (getCheckBoxItem(jungleMenu, "kitejg") && mode == "jg" &&
                        target.LSDistance(Game.CursorPos) > 600 && target.LSDistance(KL.Player.ServerPosition) <= 300)
                    {
                        KL.Spells["Pounce"].Cast(Game.CursorPos);
                        return;
                    }

                    KL.Spells["Pounce"].Cast(target.ServerPosition);
                }
            }

            // if target is not hunted
            else
            {
                // check if in the original pounce range
                if (target.LSDistance(KL.Player.ServerPosition) > KL.Spells["Pounce"].Range)
                    return;

                // get hitbox
                var radius = KL.Player.AttackRange + KL.Player.LSDistance(KL.Player.BBox.Minimum) + 1;

                // check minimum distance before pouncing
                if (target.LSDistance(KL.Player.ServerPosition) > radius ||

                    // or is jungling or waveclearing (without distance checking)
                    mode == "jg" || mode == "wc" && !getCheckBoxItem(wCMenu, "ndcwdistwc") ||

                    // or combo mode with no distance checking
                    mode == "co" && !getCheckBoxItem(wCMenu, "ndcwdistco"))
                {
                    if (target.IsChampion())
                    {
                        if (getCheckBoxItem(wCMenu, "ndcwcheck"))
                        {
                            var voutout = KL.Spells["Pounce"].GetPrediction(target);
                            if (voutout.Hitchance >= (HitChance) getBoxItem(wCMenu, "ndcwch") + 3)
                            {
                                KL.Spells["Pounce"].Cast(voutout.CastPosition);
                            }
                        }
                        else
                            KL.Spells["Pounce"].Cast(target.ServerPosition);
                    }
                    else
                    {
                        // check pouncing near enemies
                        if (mode == "wc" && getCheckBoxItem(wCMenu, "ndcwene") &&
                            target.ServerPosition.CountEnemiesInRange(550) > 0)
                            return;

                        // check pouncing under turret
                        if (mode == "wc" && getCheckBoxItem(wCMenu, "ndcwtow") &&
                            target.ServerPosition.UnderTurret(true))
                            return;

                        KL.Spells["Pounce"].Cast(target.ServerPosition);
                    }
                }
            }
        }


        // Cougar E Logic
        internal static void CastSwipe(Obj_AI_Base target, string mode)
        {
            if (KL.CatForm() && KL.CanUse(KL.Spells["Swipe"], false, mode))
            {
                if (target.IsValidTarget(KL.Spells["Swipe"].Range))
                {
                    if (target.IsChampion())
                    {
                        if (getCheckBoxItem(eCMenu, "ndcecheck"))
                        {
                            var voutout = KL.Spells["Swipe"].GetPrediction(target);
                            if (voutout.Hitchance >= (HitChance) getBoxItem(eCMenu, "ndcech") + 3)
                            {
                                KL.Spells["Swipe"].Cast(voutout.CastPosition);
                            }
                        }
                        else
                            KL.Spells["Swipe"].Cast(target.ServerPosition);
                    }
                    else
                    {
                        // try aoe swipe if menu item > 1
                        var minhit = getSliderItem(eCMenu, "ndcenum");
                        if (minhit > 1 && mode == "wc")
                            KL.CastSmartSwipe();

                        // or cast normal
                        else
                            KL.Spells["Swipe"].Cast(target.ServerPosition);
                    }
                }
            }

            // check valid target in range
        }


        internal static void SwitchForm(Obj_AI_Base target, string mode)
        {
            // catform -> human
            if (target == null)
            {
                return;
            }

            if (KL.CatForm() && KL.CanUse(KL.Spells["Aspect"], false, mode))
            {
                if (!target.IsValidTarget(KL.Spells["Javelin"].Range))
                    return;

                // get hitbox
                var radius = KL.Player.AttackRange + KL.Player.LSDistance(KL.Player.BBox.Minimum) + 1;

                // dont switch if have Q buff and near target
                if (KL.CanUse(KL.Spells["Takedown"], true, mode) && KL.Player.HasBuff("Takedown") &&
                    target.LSDistance(KL.Player.ServerPosition) <= KL.Spells["Takedown"].Range + 65f)
                {
                    return;
                }

                // change form if Q is ready and meets hitchance
                if (target.IsChampion())
                {
                    if (KL.SpellTimer["Javelin"].IsReady())
                    {
                        var poutput = KL.Spells["Javelin"].GetPrediction(target);
                        if (poutput.Hitchance >= HitChance.High)
                        {
                            KL.Spells["Aspect"].Cast();
                        }
                    }
                }
                else
                {
                    // change to human if out of pounce range and can die
                    if (!KL.SpellTimer["Pounce"].IsReady(3) && target.LSDistance(KL.Player.ServerPosition) <= 525)
                    {
                        if (target.LSDistance(KL.Player.ServerPosition) > radius)
                        {
                            if (KL.Player.GetAutoAttackDamage(target, true)*3 >= target.Health)
                                KL.Spells["Aspect"].Cast();
                        }
                    }
                }

                // is jungling
                if (mode == "jg")
                {
                    if (KL.CanUse(KL.Spells["Bushwhack"], true, mode) ||
                        KL.CanUse(KL.Spells["Javelin"], true, mode))
                    {
                        if ((!KL.SpellTimer["Pounce"].IsReady(2) || !KL.CanUse(KL.Spells["Pounce"], false, mode)) &&
                            (!KL.SpellTimer["Swipe"].IsReady() || !KL.CanUse(KL.Spells["Swipe"], false, mode)) &&
                            (!KL.SpellTimer["Takedown"].IsReady() || !KL.CanUse(KL.Spells["Takedown"], false, mode)) ||
                            !(KL.Player.LSDistance(target.ServerPosition) <= 355) ||
                            !getKeyBindItem(jungleMenu, "jgaacount"))
                        {
                            if (KL.Spells["Javelin"].Cast(target) != Spell.CastStates.Collision &&
                                KL.SpellTimer["Javelin"].IsReady())
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                            else if (!KL.CanUse(KL.Spells["Javelin"], true, mode))
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                        }
                    }
                }
            }

            // human -> catform
            if (!KL.CatForm() && KL.CanUse(KL.Spells["Aspect"], true, mode))
            {
                switch (mode)
                {
                    case "jg":
                        if (KL.Counter < getSliderItem(jungleMenu, "aareq") &&
                            getKeyBindItem(jungleMenu, "jgaacount"))
                        {
                            return;
                        }
                        break;
                    case "gap":
                        if (target.IsValidTarget(375))
                        {
                            KL.Spells["Aspect"].Cast();
                            return;
                        }
                        break;
                    case "wc":
                        if (target.IsValidTarget(375) && target.IsMinion)
                        {
                            KL.Spells["Aspect"].Cast();
                            return;
                        }
                        break;
                }

                if (target.IsHunted())
                {
                    // force switch no swipe/takedown req
                    if (!getCheckBoxItem(rHMenu, "ndhrcreq") && mode == "co" ||
                        !getCheckBoxItem(rHMenu, "ndhrjreq") && mode == "jg")
                    {
                        KL.Spells["Aspect"].Cast();
                        return;
                    }

                    if (target.LSDistance(KL.Player) > KL.Spells["Takedown"].Range + 50 &&
                        !KL.CanUse(KL.Spells["Pounce"], false, mode))
                        return;

                    // or check if pounce timer is ready before switch
                    if (KL.Spells["Aspect"].IsReady() && target.IsValidTarget(KL.Spells["ExPounce"].Range))
                    {
                        // dont change form if swipe or takedown isn't ready
                        if ((KL.SpellTimer["Takedown"].IsReady() || KL.SpellTimer["Swipe"].IsReady()) &&
                            KL.SpellTimer["Pounce"].IsReady(1))
                            KL.Spells["Aspect"].Cast();
                    }
                }
                else
                {
                    // check if in pounce range
                    if (target.IsValidTarget(KL.Spells["Pounce"].Range + 55))
                    {
                        if (mode != "jg")
                        {
                            // switch to cougar if can kill target
                            if (KL.CatDamage(target)*3 >= target.Health)
                            {
                                if (mode == "co" && target.IsValidTarget(KL.Spells["Pounce"].Range + 200))
                                {
                                    if (!KL.CanUse(KL.Spells["Javelin"], true, "co") ||
                                        KL.Spells["Javelin"].Cast(target) == Spell.CastStates.Collision)
                                    {
                                        KL.Spells["Aspect"].Cast();
                                    }
                                }
                            }

                            // switch if Q disabled in menu
                            if (!KL.CanUse(KL.Spells["Javelin"], true, mode) ||

                                // delay the cast .5 seconds
                                Utils.GameTimeTickCount - (int) (KL.TimeStamp["Javelin"]*1000) +
                                (6 + 6*KL.Player.PercentCooldownMod)*1000 >= 500 &&

                                // if Q is not ready in 2 seconds
                                !KL.SpellTimer["Javelin"].IsReady(2))
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                        }
                        else
                        {
                            if (KL.Spells["Javelin"].Cast(target) == Spell.CastStates.Collision &&
                                getCheckBoxItem(jungleMenu, "spcol"))
                            {
                                if (KL.Spells["Aspect"].IsReady())
                                    KL.Spells["Aspect"].Cast();
                            }

                            if ((!KL.SpellTimer["Bushwhack"].IsReady() || !KL.CanUse(KL.Spells["Bushwhack"], true, mode)) &&
                                (!KL.SpellTimer["Javelin"].IsReady(3) || !KL.CanUse(KL.Spells["Javelin"], true, mode)))
                            {
                                if (KL.Spells["Aspect"].IsReady())
                                    KL.Spells["Aspect"].Cast();
                            }
                        }
                    }


                    if (KN.Target.IsValidTarget(KL.Spells["Javelin"].Range) && target.IsChampion())
                    {
                        if (KL.SpellTimer["Javelin"].IsReady())
                        {
                            // check if in pounce range.
                            if (target.LSDistance(KL.Player.ServerPosition) <= KL.Spells["Pounce"].Range + 100f)
                            {
                                // if we dont meet hitchance on Q target pounce nearest target
                                var poutput = KL.Spells["Javelin"].GetPrediction(KN.Target);
                                if (poutput.Hitchance < (HitChance) (getBoxItem(KN.Root, "ndhqch") + 3))
                                {
                                    if (KL.Spells["Aspect"].IsReady())
                                        KL.Spells["Aspect"].Cast();
                                }
                            }
                        }

                        if (KN.Target.IsHunted() &&
                            KN.Target.LSDistance(KL.Player.ServerPosition) > KL.Spells["ExPounce"].Range + 100)
                        {
                            if (target.LSDistance(KL.Player.ServerPosition) <= KL.Spells["Pounce"].Range + 25)
                            {
                                if (KL.Spells["Aspect"].IsReady())
                                    KL.Spells["Aspect"].Cast();
                            }
                        }

                        if (!KL.SpellTimer["Javelin"].IsReady())
                        {
                            if (target.LSDistance(KL.Player.ServerPosition) <= KL.Spells["Pounce"].Range + 125)
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                        }
                    }
                }
            }
        }
    }
}