using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace ScaryKalista
{
    public static class Kalista
    {
        public static AIHeroClient Soulbound
        {
            get { return EntityManager.Heroes.Allies.FirstOrDefault(x => x.HasBuff("kalistacoopstrikeally")); }
        }
        public static bool BalistaPossible
        {
            get { return Soulbound != null && Soulbound.ChampionName == "Blitzcrank"; }
        }
        private static bool _fleeActivated;

        private static readonly Vector2 Baron = new Vector2(5007.124f, 10471.45f);
        private static readonly Vector2 Dragon = new Vector2(9866.148f, 4414.014f);

        public static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Kalista") return;
            
            Spells.InitSpells();
            Items.InitItems();
            if (Game.MapId == GameMapId.SummonersRift) WallJump.InitSpots();
            Config.Initialize();
            InitEvents();
        }

        private static void InitEvents()
        {
            DamageIndicator.DamageToUnit = Damages.GetActualDamage;
            Game.OnTick += OnTick;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Orbwalker.OnUnkillableMinion += Modes.OnUnkillableMinion;
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base target, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!BalistaPossible) return;

            if (args.Buff.DisplayName == "RocketGrab" && target.IsEnemy && Spells.R.IsReady())
            {
                var hero = target as AIHeroClient;
                if (hero == null 
                    || !Config.BalistaMenu.IsChecked("balista." + hero.ChampionName)
                    || (Config.BalistaMenu.IsChecked("balista.comboOnly") && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
                {
                    return;
                }

                if (hero.IsValidTarget() 
                    && Player.Instance.Distance(Soulbound) >= Config.BalistaMenu.GetValue("balista.distance")
                    && Spells.R.IsInRange(Soulbound))
                {
                    Spells.R.Cast();
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            if (Player.Instance.IsDead) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Modes.LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Modes.JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Modes.Flee();
                _fleeActivated = true;
            }

            Modes.PermaActive();

            if (_fleeActivated && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                _fleeActivated = false;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
                Orbwalker.ForcedTarget = null;
            }

            if (Config.SentinelMenu.IsActive("sentinel.castBaron"))
            {
                Modes.CastW(Baron);
            }
            if (Config.SentinelMenu.IsActive("sentinel.castDragon"))
            {
                Modes.CastW(Dragon);
            }
        }
        private static void OnDraw(EventArgs args)
        {
            if (Config.DrawMenu.IsChecked("draw.Q"))
            {
                Circle.Draw(Color.DarkRed, Spells.Q.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.W"))
            {
                Circle.Draw(Color.DarkRed, Spells.W.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.E"))
            {
                Circle.Draw(Color.DarkRed, Spells.E.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.R"))
            {
                Circle.Draw(Color.DarkRed, Spells.R.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.killableMinions"))
            {
                foreach (var killableMinion in 
                    EntityManager.MinionsAndMonsters.GetLaneMinions(
                        EntityManager.UnitTeam.Enemy, Player.Instance.Position, Spells.E.Range)
                        .Where(x => x.IsRendKillable()))
                {
                    Circle.Draw(Color.GreenYellow, 25f, 2f, killableMinion.Position);
                }
            }

            if (Config.DrawMenu.IsChecked("draw.percentage"))
            {

                foreach (var enemy in
                    EntityManager.Heroes.Enemies
                    .Where(x => Player.Instance.Distance(x) <= 2000f && !x.IsDead && x.IsVisible && x.HasRendBuff()))
                {
                    var percent = Math.Floor((Damages.GetActualDamage(enemy) / enemy.GetTotalHealth()) * 100);

                    if (percent >= 100 && !enemy.IsRendKillable())
                    {
                        Drawing.DrawText(enemy.HPBarPosition.X + 140, enemy.HPBarPosition.Y + 5, System.Drawing.Color.Red, "Can't kill!", 20);
                    }
                    else
                    {
                        Drawing.DrawText(enemy.HPBarPosition.X + 140, enemy.HPBarPosition.Y + 5, System.Drawing.Color.White, 
                        enemy.IsRendKillable() ? "Killable!" : percent + "%", 20);
                    }
                }
            }

            if (Config.DrawMenu.IsChecked("draw.stacks"))
            {
                foreach (var enemy in
                    EntityManager.Heroes.Enemies
                    .Where(x => Player.Instance.Distance(x) <= 2000f && !x.IsDead && x.IsVisible && x.HasRendBuff()))
                {
                    var stacks = enemy.GetRendBuff().Count;
                    Drawing.DrawText(enemy.Position.X, enemy.Position.Y, System.Drawing.Color.White, "Stacks: " + stacks);
                }
            }

            if (Config.DrawMenu.IsChecked("draw.jumpSpots"))
            {
                foreach (var spot in WallJump.JumpSpots.Where(s => Player.Instance.Distance(s[0]) <= 2000))
                {
                    Circle.Draw(Color.DarkGray, 30f, spot[0]);
                }
            }

            if (BalistaPossible && Config.DrawMenu.IsChecked("draw.balista"))
            {
                Circle.Draw(Color.Aqua, Config.BalistaMenu.GetValue("balista.distance"), Player.Instance.Position);
            }
        }
    }
}
