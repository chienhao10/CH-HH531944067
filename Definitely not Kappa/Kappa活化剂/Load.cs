namespace KappaUtility
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Items;

    using Misc;

    using Summoners;

    using Trackers;

    internal class Load
    {
        public static Menu UtliMenu;

        private static readonly CheckBox Corruptingc;

        private static readonly CheckBox Healthc;

        private static readonly CheckBox Huntersc;

        private static readonly CheckBox Refillablec;

        private static readonly CheckBox Biscuitc;

        private static readonly CheckBox Seraphc;

        private static readonly CheckBox Solaric;

        private static readonly CheckBox FaceOfTheMountainc;

        private static readonly CheckBox Zhonyasc;

        private static readonly Slider Corruptingh;

        private static readonly Slider Healthh;

        private static readonly Slider Huntersh;

        private static readonly Slider Refillableh;

        private static readonly Slider Biscuith;

        private static readonly Slider Seraphh;

        private static readonly Slider Solarih;

        private static readonly Slider FaceOfTheMountainh;

        private static readonly Slider Zhonyash;

        public static int _Corruptingh => Potions.PotMenu["CPH"].Cast<Slider>().CurrentValue;

        public static int _Healthh => Potions.PotMenu["HPH"].Cast<Slider>().CurrentValue;

        public static int _Huntersh => Potions.PotMenu["HPSH"].Cast<Slider>().CurrentValue;

        public static int _Refillableh => Potions.PotMenu["RPH"].Cast<Slider>().CurrentValue;

        public static int _Biscuith => Potions.PotMenu["BPH"].Cast<Slider>().CurrentValue;

        public static int _Seraphh => Defensive.DefMenu["Seraphh"].Cast<Slider>().CurrentValue;

        public static int _Solarih => Defensive.DefMenu["Solarih"].Cast<Slider>().CurrentValue;

        public static int _FaceOfTheMountainh => Defensive.DefMenu["FaceOfTheMountainh"].Cast<Slider>().CurrentValue;

        public static int _Zhonyash => Defensive.DefMenu["Zhonyash"].Cast<Slider>().CurrentValue;

        public static bool _Corruptingc
            =>
                !Potions.PotMenu["CP"].Cast<CheckBox>().CurrentValue || !Potions.Corrupting.IsOwned()
                || !Potions.Corrupting.IsReady();

        public static bool _Healthc
            =>
                Potions.PotMenu["HP"].Cast<CheckBox>().CurrentValue && Potions.Health.IsOwned()
                && Potions.Health.IsReady();

        public static bool _Huntersc
            =>
                Potions.PotMenu["HPS"].Cast<CheckBox>().CurrentValue && Potions.Hunters.IsOwned()
                && Potions.Hunters.IsReady();

        public static bool _Refillablec
            =>
                Potions.PotMenu["RP"].Cast<CheckBox>().CurrentValue && Potions.Refillable.IsOwned()
                && Potions.Refillable.IsReady();

        public static bool _Biscuitc
            =>
                Potions.PotMenu["BP"].Cast<CheckBox>().CurrentValue && Potions.Biscuit.IsOwned()
                && Potions.Biscuit.IsReady();

        public static bool _Seraphc
            =>
                Defensive.DefMenu["Seraph"].Cast<CheckBox>().CurrentValue && Defensive.Seraph.IsOwned()
                && Defensive.Seraph.IsReady();

        public static bool _Solaric
            =>
                Defensive.DefMenu["Solari"].Cast<CheckBox>().CurrentValue && Defensive.Solari.IsOwned()
                && Defensive.Solari.IsReady();

        public static bool _FaceOfTheMountainc
            =>
                Defensive.DefMenu["FaceOfTheMountain"].Cast<CheckBox>().CurrentValue && Defensive.FOTM.IsOwned()
                && Defensive.FOTM.IsReady();

        public static bool _Zhonyasc
            =>
                Defensive.DefMenu["Zhonyas"].Cast<CheckBox>().CurrentValue && Defensive.Zhonyas.IsOwned()
                && Defensive.Zhonyas.IsReady();

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            UtliMenu = MainMenu.AddMenu("CH汉化活化剂", "KappaUtility");
            UtliMenu.AddLabel("CH汉化 Kappa活化剂，有同样功能的脚本最好不要一起开");
            UtliMenu.AddLabel("如有些英雄脚本有自动水银选一个就好，关闭其中一个水银");
            UtliMenu.AddLabel("部分功能不能一一测试，有问题回报谢谢");
            AutoLvlUp.OnLoad();
            AutoQSS.OnLoad();
            AutoTear.OnLoad();
            AutoReveal.OnLoad();
            GanksDetector.OnLoad();
            Tracker.OnLoad();
            Surrender.OnLoad();
            SkinHax.OnLoad();
            Spells.OnLoad();
            Potions.OnLoad();
            Offensive.OnLoad();
            Defensive.OnLoad();

            Game.OnTick += Game_OnUpdate;
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Spells.Drawings();
            GanksDetector.OnDraw();
        }

        private static void OnEndScene(EventArgs args)
        {
            AutoReveal.Drawings();
            Tracker.Traps();
            Tracker.HPtrack();
            Tracker.track();
            GanksDetector.OnEndScene();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Offensive.Items();
                Defensive.Items();
            }
            try
            {
                AutoReveal.Reveal();
                AutoLvlUp.Levelup();
                AutoTear.OnUpdate();
                GanksDetector.OnUpdate();
                Smite.Smiteopepi();
                Spells.Cast();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((!(caster is AIHeroClient) && !(caster is Obj_AI_Turret)) || caster == null
                || target == null && caster.IsEnemy)
            {
                return;
            }

            if (target.IsValidTarget(Defensive.FOTM.Range) && _FaceOfTheMountainc)
            {
                if (target != null && target.HealthPercent < _FaceOfTheMountainh)
                {
                    Defensive.FOTM.Cast(target);
                }

                if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                {
                    Defensive.FOTM.Cast(target);
                }
            }

            if (target.IsValidTarget(Defensive.Solari.Range) && _Solaric)
            {
                if (target != null && target.HealthPercent < _Solarih)
                {
                    Defensive.Solari.Cast();
                }

                if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                {
                    Defensive.Solari.Cast();
                }
            }

            if (target != null && target.IsMe)
            {
                if (_Refillablec)
                {
                    if (target.HealthPercent < _Refillableh)
                    {
                        Potions.Refillable.Cast();
                    }

                    if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                    {
                        Potions.Refillable.Cast();
                    }
                }

                if (_Healthc)
                {
                    if (target.HealthPercent < _Healthh)
                    {
                        Potions.Health.Cast();
                    }

                    if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                    {
                        Potions.Health.Cast();
                    }
                }

                if (_Huntersc)
                {
                    if (target.HealthPercent < _Huntersh)
                    {
                        Potions.Hunters.Cast();
                    }

                    if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                    {
                        Potions.Hunters.Cast();
                    }
                }

                if (_Biscuitc)
                {
                    if (target.HealthPercent < _Biscuith)
                    {
                        Potions.Biscuit.Cast();
                    }

                    if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                    {
                        Potions.Biscuit.Cast();
                    }
                }

                if (_Corruptingc)
                {
                    if (target.HealthPercent < _Corruptingh)
                    {
                        Potions.Corrupting.Cast();
                    }

                    if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                    {
                        Potions.Corrupting.Cast();
                    }
                }

                if (_Seraphc)
                {
                    if (target.HealthPercent < _Seraphh)
                    {
                        Defensive.Seraph.Cast();
                    }

                    if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                    {
                        Defensive.Seraph.Cast();
                    }
                }

                if (_Zhonyasc)
                {
                    if (target.HealthPercent < _Zhonyash)
                    {
                        Defensive.Zhonyas.Cast();
                    }

                    if (caster.GetAutoAttackDamage(target) > target.TotalShieldHealth())
                    {
                        Defensive.Zhonyas.Cast();
                    }
                }
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((!(caster is AIHeroClient) && !(caster is Obj_AI_Turret)) || caster == null
                || target == null && caster.IsEnemy)
            {
                return;
            }
            if (target.IsValidTarget(Defensive.FOTM.Range))
            {
                if (_FaceOfTheMountainc && target.HealthPercent < _FaceOfTheMountainh)
                {
                    Defensive.FOTM.Cast(target);
                }

                if (caster.BaseAttackDamage > target.TotalShieldHealth()
                    || caster.BaseAbilityDamage > target.TotalShieldHealth())
                {
                    Defensive.FOTM.Cast(target);
                }
            }

            if (target.IsValidTarget(Defensive.Solari.Range) && _Solaric)
            {
                if (target.HealthPercent < _Solarih)
                {
                    Defensive.Solari.Cast();
                }

                if (caster.BaseAttackDamage > target.TotalShieldHealth()
                    || caster.BaseAbilityDamage > target.TotalShieldHealth())
                {
                    Defensive.Solari.Cast();
                }
            }

            if (target.IsMe)
            {
                if (_Refillablec)
                {
                    if (target.HealthPercent < _Refillableh)
                    {
                        Potions.Refillable.Cast();
                    }

                    if (caster.BaseAttackDamage > target.TotalShieldHealth()
                        || caster.BaseAbilityDamage > target.TotalShieldHealth())
                    {
                        Potions.Refillable.Cast();
                    }
                }

                if (_Healthc)
                {
                    if (target.HealthPercent < _Healthh)
                    {
                        Potions.Health.Cast();
                    }

                    if (caster.BaseAttackDamage > target.TotalShieldHealth()
                        || caster.BaseAbilityDamage > target.TotalShieldHealth())
                    {
                        Potions.Health.Cast();
                    }
                }

                if (_Huntersc)
                {
                    if (target.HealthPercent < _Huntersh)
                    {
                        Potions.Hunters.Cast();
                    }

                    if (caster.BaseAttackDamage > target.TotalShieldHealth()
                        || caster.BaseAbilityDamage > target.TotalShieldHealth())
                    {
                        Potions.Hunters.Cast();
                    }
                }

                if (_Biscuitc)
                {
                    if (target.HealthPercent < _Biscuith)
                    {
                        Potions.Biscuit.Cast();
                    }

                    if (caster.BaseAttackDamage > target.TotalShieldHealth()
                        || caster.BaseAbilityDamage > target.TotalShieldHealth())
                    {
                        Potions.Biscuit.Cast();
                    }
                }

                if (_Corruptingc)
                {
                    if (target.HealthPercent < _Corruptingh)
                    {
                        Potions.Corrupting.Cast();
                    }

                    if (caster.BaseAttackDamage > target.TotalShieldHealth()
                        || caster.BaseAbilityDamage > target.TotalShieldHealth())
                    {
                        Potions.Corrupting.Cast();
                    }
                }

                if (_Seraphc)
                {
                    if (target.HealthPercent < _Seraphh)
                    {
                        Defensive.Seraph.Cast();
                    }

                    if (caster.BaseAttackDamage > target.TotalShieldHealth()
                        || caster.BaseAbilityDamage > target.TotalShieldHealth())
                    {
                        Defensive.Seraph.Cast();
                    }
                }

                if (_Zhonyasc)
                {
                    if (target.HealthPercent < _Zhonyash)
                    {
                        Defensive.Zhonyas.Cast();
                    }

                    if (caster.BaseAttackDamage > target.TotalShieldHealth()
                        || caster.BaseAbilityDamage > target.TotalShieldHealth())
                    {
                        Defensive.Zhonyas.Cast();
                    }
                }
            }
        }
    }
}