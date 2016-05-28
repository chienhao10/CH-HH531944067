using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Core
{
    internal class MissileReturn
    {
        private static readonly Menu Config = Program.Config;
        public static Menu Sub;
        public MissileClient Missile;
        private Vector3 MissileEndPos;
        private readonly string MissileName;
        private readonly string MissileReturnName;
        private readonly Spell QWER;
        public AIHeroClient Target;

        public MissileReturn(string missile, string missileReturnName, Spell qwer)
        {
            Sub = Config.AddSubMenu("Missile Settings");
            Sub.Add("aim", new CheckBox("Auto aim returned missile (" + qwer.Slot + ")"));
            Sub.Add("drawHelper", new CheckBox("Show " + qwer.Slot + " helper"));

            MissileName = missile;
            MissileReturnName = missileReturnName;
            QWER = qwer;

            GameObject.OnCreate += SpellMissile_OnCreateOld;
            GameObject.OnDelete += Obj_SpellMissile_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool getCheckBoxItem(string item)
        {
            return Sub[item].Cast<CheckBox>().CurrentValue;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Missile != null && Missile.IsValid && getCheckBoxItem("drawHelper"))
                OktwCommon.DrawLineRectangle(Missile.Position, Player.Position, (int)QWER.Width, 1, Color.White);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (getCheckBoxItem("aim"))
            {
                var posPred = CalculateReturnPos();
                if (posPred != Vector3.Zero)
                {
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                    {
                        Orbwalker.OrbwalkTo(posPred);
                    }
                }
                else
                {
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                    {
                        Orbwalker.OrbwalkTo(Game.CursorPos);
                    }
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == QWER.Slot)
            {
                MissileEndPos = args.End;
            }
        }

        private void SpellMissile_OnCreateOld(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
                return;

            var missile = (MissileClient)sender;

            if (missile.SData.Name != null)
            {
                if (missile.SData.Name.ToLower() == MissileName.ToLower() ||
                    missile.SData.Name.ToLower() == MissileReturnName.ToLower())
                {
                    Missile = missile;
                }
            }
        }

        private void Obj_SpellMissile_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
                return;

            var missile = (MissileClient)sender;

            if (missile.SData.Name != null)
            {
                if (missile.SData.Name.ToLower() == MissileReturnName.ToLower())
                {
                    Missile = null;
                }
            }
        }

        public Vector3 CalculateReturnPos()
        {
            if (Target == null)
            {
                return Vector3.Zero;
            }
            if (Missile != null && Missile.IsValid && Target.IsValidTarget())
            {
                if (Missile != null)
                {
                    var finishPosition = Missile.Position;
                    if (Missile.SData.Name.ToLower() == MissileName.ToLower())
                    {
                        finishPosition = MissileEndPos;
                    }

                    var misToPlayer = Player.LSDistance(finishPosition);
                    var tarToPlayer = Player.LSDistance(Target);

                    if (misToPlayer > tarToPlayer)
                    {
                        var misToTarget = Target.LSDistance(finishPosition);

                        if (misToTarget < QWER.Range && misToTarget > 50)
                        {
                            var cursorToTarget = Target.LSDistance(Player.Position.LSExtend(Game.CursorPos, 100));
                            var ext = finishPosition.LSExtend(Target.ServerPosition, cursorToTarget + misToTarget);

                            if (ext.LSDistance(Player.Position) < 800 && ext.CountEnemiesInRange(400) < 2)
                            {
                                if (getCheckBoxItem("drawHelper"))
                                    Utility.DrawCircle(ext, 100, System.Drawing.Color.White, 1, 1);
                                return ext;
                            }
                        }
                    }
                }
            }
            return Vector3.Zero;
        }
    }
}