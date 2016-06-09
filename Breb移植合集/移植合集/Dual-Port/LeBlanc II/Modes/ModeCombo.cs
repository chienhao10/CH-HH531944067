using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Champion;
using Color = SharpDX.Color;
using Leblanc.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace Leblanc.Modes
{

    enum ActiveComboMode
    {
        Mode2xQ,
        Mode2xW,
        ModeAuto
    }

    enum ComboMode
    {
        Mode2xQ = 0,
        Mode2xW = 1,
        ModeAuto = 2
    }



    enum ComboStatus
    {
        Completed,
        NotCompleted
    }

    enum CanMakeCombo
    {
        Yes,
        No
    }

    internal class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuHunt { get; private set; }
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;
        private static LeagueSharp.Common.Spell Q2 => Champion.PlayerSpells.Q2;
        private static LeagueSharp.Common.Spell W2 => Champion.PlayerSpells.W2;
        private static LeagueSharp.Common.Spell E2 => Champion.PlayerSpells.E2;

        private static int LastComboChangeKeyTick = 0;

        public static SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

        public static AIHeroClient Target => TargetSelector.GetTarget(Q.Range, DamageType.Magical);


        private static ActiveComboMode ActiveComboMode { get; set; }
        public static List<Vector2> ListWJumpPositions = new List<Vector2>();


        internal class ListJumpPositions
        {
            public Vector2 Position { get; set; }
            public string Name { get; set; }
        }

        private static readonly List<ListJumpPositions> ExistingJumpPositions = new List<ListJumpPositions>();


        public static ComboMode ComboMode
        {
            get
            {
                switch (MenuLocal["Combo.Mode"].Cast<ComboBox>().CurrentValue)
                {
                    case 0:
                        ActiveComboMode = ActiveComboMode.Mode2xQ;
                        return ComboMode.Mode2xQ;
                    case 1:
                        ActiveComboMode = ActiveComboMode.Mode2xW;
                        return ComboMode.Mode2xW;
                    case 2:
                        ActiveComboMode = ActiveComboMode.ModeAuto;
                        return ComboMode.ModeAuto;
                }

                return ComboMode.Mode2xQ;
            }
        }

        public static void Init()
        {

            MenuLocal = Modes.ModeConfig.MenuConfig.AddSubMenu("Combo", "Combo");
            {
                MenuLocal.Add("Combo.Mode", new ComboBox("Combo Mode:", 1, "Q:R", "W:R", "Auto"));
                MenuLocal.Add("Combo.UseW", new ComboBox("W:", 1, "Off", "On"));
                MenuLocal.Add("Combo.UseW.Far", new ComboBox("W: Jump for killable distant enemy", 1, "Off", "On"));
                MenuLocal.Add("Combo.UseE", new ComboBox("E:", 1, "Off", "On"));
                MenuLocal.Add("Combo.Ignite", new ComboBox("Ignite:", 1, "Off", "On"));
            }

            Game.OnUpdate += GameOnOnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += DrawingOnOnDraw;
            Drawing.OnDraw += DrawingHutMode;
        }

        private static int GetWHits(Obj_AI_Base target, List<Obj_AI_Base> targets = null)
        {
            if (targets != null && (ComboMode == ComboMode.Mode2xW || ComboMode == ComboMode.ModeAuto))
            {
                targets = targets.Where(t => t.LSIsValidTarget((W.Range + W.Width))).ToList();
                var pred = W.GetPrediction(target);
                if (pred.Hitchance >= HitChance.Medium)
                {
                    var circle = new LeagueSharp.Common.Geometry.Polygon.Circle(pred.UnitPosition, target.BoundingRadius + W.Width);
                    circle.Draw(System.Drawing.Color.Aqua, 5);

                    return 1 + (from t in targets.Where(x => x.NetworkId != target.NetworkId)
                                let pred2 = W.GetPrediction(t)
                                where pred2.Hitchance >= HitChance.Medium
                                select new LeagueSharp.Common.Geometry.Polygon.Circle(pred2.UnitPosition, t.BoundingRadius * 0.9f)).Count(
                            circle2 => circle2.Points.Any(p => circle.IsInside(p)));
                }
            }
            if (W.IsInRange(target))
            {
                return 1;
            }
            return 0;
        }

        private static void DrawingHutMode(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (!Modes.ModeDraw.MenuLocal["Draw.Enable"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }


            foreach (var e in HeroManager.Enemies.Where(e => !e.IsDead && e.IsVisible))
            {
                if (e.Health < GetComboDamage(e))
                {
                    Common.CommonGeometry.DrawText(CommonGeometry.Text, "Combo Kill", e.HPBarPosition.X + 8, e.HPBarPosition.Y + 36, SharpDX.Color.GreenYellow);
                }
            }
        }

        public static int GetPriority(string championName)
        {
            string[] low =
            {
                "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Soraka", "Tahm", "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

            string[] medium =
            {
                "Aatrox", "Akali", "Darius", "Diana", "Ekko", "Elise", "Evelynn", "Fiddlesticks", "Fiora", "Fizz",
                "Galio", "Gangplank", "Gragas", "Heimerdinger", "Irelia", "Jax", "Jayce", "Kassadin", "Kayle", "Kha'Zix",
                "Lee Sin", "Lissandra", "Maokai", "Mordekaiser", "Morgana", "Nocturne", "Nidalee", "Pantheon", "Poppy",
                "RekSai", "Rengar", "Riven", "Rumble", "Ryze", "Shaco", "Swain", "Trundle", "Tryndamere", "Udyr",
                "Urgot", "Vladimir", "Vi", "XinZhao", "Zilean"
            };

            string[] high =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven", "Ezreal",
                "Graves", "Jhin", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Yasuo", "Zed", "Ziggs"
            };

            if (medium.Contains(championName))
            {
                return 2;
            }

            if (high.Contains(championName))
            {
                return 3;
            }

            return 1;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (ModeConfig.MenuKeys["Key.ChangeCombo"].Cast<KeyBind>().CurrentValue && Environment.TickCount > LastComboChangeKeyTick + 250)
            {
                var newValue = MenuLocal["Combo.Mode"].Cast<ComboBox>().CurrentValue + 1;
                if (MenuLocal["Combo.Mode"].Cast<ComboBox>().CurrentValue == 2)
                {
                    newValue = 0;
                }

                MenuLocal["Combo.Mode"].Cast<ComboBox>().CurrentValue = newValue;

                LastComboChangeKeyTick = Environment.TickCount;
            }
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (Shop.IsOpen)
            {
                return;
            }

            if (Target.LSIsValidTarget(Q.Range * 2))
            {
                var wComboHits = GetWHits(Target, HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range + W.Width)).Cast<Obj_AI_Base>().ToList());
            }
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (!Modes.ModeDraw.MenuLocal["Draw.Enable"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }


            var nTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (nTarget.LSIsValidTarget() && nTarget.Health < ComboDamage2xW(nTarget))
            {

                Render.Circle.DrawCircle(nTarget.Position, 105f, System.Drawing.Color.AliceBlue);

            }

            var xComboString = "Combo Mode: ";
            System.Drawing.Color xComboColor = System.Drawing.Color.FromArgb(100, 255, 200, 37);

            string[] vComboString = new[]
            {
                "Q:R", "W:R", "Auto"
            };

            System.Drawing.Color[] vComboColor = new[]
            {
                System.Drawing.Color.FromArgb(255, 4, 0, 255),
                System.Drawing.Color.Red,
                System.Drawing.Color.FromArgb(255, 46, 47, 46),
            };

            var nComboMode = MenuLocal["Combo.Mode"].Cast<ComboBox>().CurrentValue;
            xComboString = xComboString + vComboString[MenuLocal["Combo.Mode"].Cast<ComboBox>().CurrentValue];
            xComboColor = vComboColor[MenuLocal["Combo.Mode"].Cast<ComboBox>().CurrentValue];

            Common.CommonGeometry.DrawBox(new Vector2(Drawing.Width * 0.45f, Drawing.Height * 0.80f), 125, 18, xComboColor, 1, System.Drawing.Color.Black);
            Common.CommonGeometry.DrawText(CommonGeometry.Text, xComboString, Drawing.Width * 0.455f, Drawing.Height * 0.803f, SharpDX.Color.Wheat);

            return;
            /*
            var t = TargetSelector.GetTarget(W.Range + Q.Range - 20, DamageType.Magical);
            if (!t.LSIsValidTarget(W.Range + Q.Range - 20))
            {
                return;
            }

            if (t.LSIsValidTarget(W.Range))
            {
                return;
            }

            bool canJump = false;

            if (ComboMode == ComboMode.Mode2xQ)
            {
                if ((t.Health < ComboDamage2xQ(t) && Q.IsReady() && R.IsReady()) ||
                    (t.Health < Q.GetDamage(t) && Q.IsReady()))
                {
                    canJump = true;
                }
            }

            if (canJump && W.IsReady() && !W.StillJumped())
            {
                var x = GetJumpPosition(t, W.Range);
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    W.Cast(x);
                }
                return;
            }
            */
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                //Game.PrintChat(Q.Cooldown.ToString());
                if (Target.LSIsValidTarget(Q.Range) && CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xQ && Target.Health < ComboDamage2xQ(Target))
                {
                    Q2.CastOnUnit(Target);
                }

                if (Target.LSIsValidTarget(W.Range) && CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xW && Target.Health < ComboDamage2xW(Target))
                {
                    W2.Cast(Target);
                }

                ExecuteFarCombo();

                if (MenuLocal["Combo.Ignite"].Cast<ComboBox>().CurrentValue == 1)
                {
                    if (IgniteSlot != SpellSlot.Unknown &&
                        ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                    {
                        if (Target.LSIsValidTarget(650) && !Target.HaveImmortalBuff() &&
                            ObjectManager.Player.GetSummonerSpellDamage(Target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) + 150 >=
                            Target.Health && (!Q.IsReady() || W.StillJumped()))
                        {
                            ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, Target);
                        }
                    }
                }

                switch (ComboMode)
                {
                    case ComboMode.Mode2xQ:
                        {
                            ActiveComboMode = ActiveComboMode.Mode2xQ;
                            ExecuteMode2xQ();
                            break;
                        }

                    case ComboMode.Mode2xW:
                        {
                            ActiveComboMode = ActiveComboMode.Mode2xW;
                            ExecuteMode2xW();
                            break;
                        }

                    case ComboMode.ModeAuto:
                        {
                            ActiveComboMode = ActiveComboMode.ModeAuto;
                            ExecuteModeAuto();
                            break;
                        }
                }
                ExecuteSpells();
            }
        }

        static void ExecuteFarCombo()
        {
            var rangeWxWxQ = W.Range * 2 + Q.Range - 50;
            var rangeWxQ = W.Range + Q.Range;
            var range2xW = W.Range * 2 - 50;

            if (Q.Level == 0 || Q.IsReady())
            {
                return;
            }

            int[] qPerLevel = new[] { 55, 80, 105, 130, 155 };

            var qDamage = ObjectManager.Player.TotalMagicalDamage / 100 * 40 + qPerLevel[Q.Level - 1];

            foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(rangeWxQ) && !e.LSIsValidTarget(Q.Range) && e.Health <= Q.GetDamage(e) * 2 + ComboDamage2xQ(e) + E.GetDamage(e) * 2))
            {
                if (Q.IsReady() && W.IsReady() && R.IsReady())
                {
                    W.Cast(e);
                }
            }
        }
        static void ExecuteSpells()
        {
            if (!R.IsReady())
            {
                if (ActiveComboMode == ActiveComboMode.Mode2xQ)
                {
                    if (MenuLocal["Combo.UseW"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        Champion.PlayerSpells.CastW(Target);
                    }

                    if (MenuLocal["Combo.UseE"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        Champion.PlayerSpells.CastE(Target);
                    }

                    Champion.PlayerSpells.CastQ(Target);
                }

                if (ActiveComboMode == ActiveComboMode.Mode2xW || ActiveComboMode == ActiveComboMode.ModeAuto)
                {
                    if (MenuLocal["Combo.UseE"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        Champion.PlayerSpells.CastE(Target);
                    }

                    Champion.PlayerSpells.CastQ(Target);

                    if (MenuLocal["Combo.UseW"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        Champion.PlayerSpells.CastW(Target);
                    }
                }
            }
            else
            {
                if (!Q.IsReady() && ActiveComboMode == ActiveComboMode.Mode2xQ && CommonHelper.SpellRStatus != CommonHelper.SpellRName.R2xQ && Q.Cooldown > 1)
                {
                    if (MenuLocal["Combo.UseW"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        Champion.PlayerSpells.CastW(Target);
                    }

                    if (MenuLocal["Combo.UseE"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        Champion.PlayerSpells.CastE(Target);
                    }
                }

                if (ActiveComboMode == ActiveComboMode.Mode2xW && CommonHelper.SpellRStatus != CommonHelper.SpellRName.R2xW && (W.StillJumped() || W.Cooldown > 1))
                {
                    Champion.PlayerSpells.CastQ(Target);

                    if (MenuLocal["Combo.UseE"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        Champion.PlayerSpells.CastE(Target);
                    }

                }
            }
        }

        static bool CanCastCombo(ComboMode comboMode)
        {
            if (!R.IsReady())
            {
                return false;
            }

            if (comboMode == ComboMode.Mode2xQ && Q.IsReady())
            {
                return true;
            }

            if (comboMode == ComboMode.Mode2xW && W.IsReady())
            {
                return true;
            }

            return false;
        }

        private static void ExecuteMode2xQ()
        {
            if (!Target.LSIsValidTarget(Q.Range))
            {
                return;
            }

            if (!Q.IsReady() && !R.IsReady())
            {
                return;
            }

            Champion.PlayerSpells.CastQ(Target);
            Champion.PlayerSpells.CastQ2(Target);
        }

        private static void ExecuteMode2xW()
        {
            if (!Target.LSIsValidTarget(W.Range))
            {
                return;
            }

            if (!W.IsReady() && !R.IsReady())
            {
                return;
            }

            Champion.PlayerSpells.CastW(Target);
            Champion.PlayerSpells.CastW2(Target);
        }

        private static void ExecuteModeAuto()
        {
            if (!R.IsReady())
            {
                return;
            }

            var find = HeroManager.Enemies.Find(e => e.NetworkId != Target.NetworkId && e.LSDistance(Target) <= W.Width);
            if (find == null)
                return;
            if (find != null && CanCastCombo(ComboMode.Mode2xW))
            {
                var wComboHits = GetWHits(Target, HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range + W.Width)).Cast<Obj_AI_Base>().ToList());

                if (wComboHits >= 2)
                {
                    ExecuteMode2xW();
                }
                return;
            }

            if (find == null && CanCastCombo(ComboMode.Mode2xQ))
            {
                ExecuteMode2xQ();
            }

            if (Q.IsReady() && !W.IsReady())
            {
                ExecuteMode2xQ();
                return;
            }

            if (!Q.IsReady() && W.IsReady())
            {
                ExecuteMode2xW();
                return;
            }

            if (Target.LSIsValidTarget(Q.Range) && CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xQ)
            {
                Q2.CastOnUnit(Target);
                return;
            }

            if (Target.LSIsValidTarget(W.Range) && CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xW)
            {
                W2.Cast(Target);
                return;
            }
        }

        private static List<List<Vector2>> GetCombinations(List<Vector2> allValues)
        {
            var collection = new List<List<Vector2>>();
            for (var counter = 0; counter < (1 << allValues.Count); ++counter)
            {
                var combination = allValues.Where((t, i) => (counter & (1 << i)) == 0).ToList();

                collection.Add(combination);
            }
            return collection;
        }

        public struct HitLocation
        {
            public int EnemyHit;
            public Vector2 Position;

            public HitLocation(Vector2 position, int enemyHit)
            {
                Position = position;
                EnemyHit = enemyHit;
            }
        }

        public static HitLocation GetBestCircularHitLocation(List<Vector2> enemyPositions, float width, float range,
            int useMECMax = 9)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = ObjectManager.Player.ServerPosition.LSTo2D();

            range = range * range;

            if (enemyPositions.Count == 0)
            {
                return new HitLocation(result, minionCount);
            }

            /* Use MEC to get the best positions only when there are less than 9 positions because it causes lag with more. */
            if (enemyPositions.Count <= useMECMax)
            {
                var subGroups = GetCombinations(enemyPositions);
                foreach (var subGroup in subGroups)
                {
                    if (subGroup.Count > 0)
                    {
                        var circle = MEC.GetMec(subGroup);

                        if (circle.Radius <= width && circle.Center.LSDistance(startPos, true) <= range)
                        {
                            minionCount = subGroup.Count;
                            return new HitLocation(circle.Center, minionCount);
                        }
                    }
                }
            }
            else
            {
                foreach (var pos in enemyPositions)
                {
                    if (pos.LSDistance(startPos, true) <= range)
                    {
                        var count = enemyPositions.Count(pos2 => pos.LSDistance(pos2, true) <= width * width);

                        if (count >= minionCount)
                        {
                            result = pos;
                            minionCount = count;
                        }
                    }
                }
            }

            return new HitLocation(result, minionCount);
        }


        public static HitLocation GetCircularHitLocation(List<Vector2> minionPositions, float overrideWidth = -1)
        {
            return GetBestCircularHitLocation(minionPositions, overrideWidth >= 0 ? overrideWidth : W.Width, W.Range);
        }

        private static void ExecuteModeCustom()
        {
            return;
            /*
            var nTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (nTarget.LSIsValidTarget() && nTarget.Health < ComboDamage2xW(nTarget))
            {
            }

            IEnumerable<Vector2> rangedMinionsW = HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range + W.Width + 30)).Select(a => a.Position.LSTo2D());

            //rangedMinionsW.AddRange(HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range + W.Width + 30)).Select(x => x.Position.To2D()));
            //var rangedMinionsW =
            //    HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range + W.Width + 30)).Select(x => x.Position.To2D());

            var locationW = W.GetCircularFarmLocation((List<Vector2>)rangedMinionsW, W.Width * 0.75f);

            if (locationW.MinionsHit > 1)
            {
                ExecuteMode2xW();
            }

            if (Target.LSIsValidTarget(Q.Range))
            {
                if (HeroManager.Enemies.Find(e => e.LSDistance(Target) < W.Range && e.NetworkId != Target.NetworkId) ==
                    null)
                {
                    ExecuteMode2xQ();
                }
            }
            else if (Target.LSIsValidTarget(Q.Range) && MenuHunt["Hunt." + Target.ChampionName].Cast<CheckBox>().CurrentValue)
            {
                ExecuteMode2xQ();
            }
            */
        }

        private static void ExecuteCastItems(AttackableUnit t)
        {
            foreach (var item in Common.CommonItems.ItemDb)
            {
                if (item.Value.ItemType == Common.CommonItems.EnumItemType.AoE
                    && item.Value.TargetingType == Common.CommonItems.EnumItemTargettingType.EnemyHero)
                {
                    if (t is AIHeroClient && t.LSIsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady())
                        item.Value.Item.Cast();
                }

                if (item.Value.ItemType == Common.CommonItems.EnumItemType.Targeted
                    && item.Value.TargetingType == Common.CommonItems.EnumItemTargettingType.EnemyHero)
                {
                    if (t.LSIsValidTarget(item.Value.Item.Range) && item.Value.Item.IsReady())
                        item.Value.Item.Cast((AIHeroClient)t);
                }
            }
        }

        public static float GetComboDamage(AttackableUnit t)
        {
            double fComboDamage = 0;

            fComboDamage += Q.IsReady() ? Q.GetDamage(t as AIHeroClient) + Q.GetDamage(t as AIHeroClient, 1) : 0;

            fComboDamage += W.IsReady() ? W.GetDamage(t as AIHeroClient) : 0;

            fComboDamage += E.IsReady() ? E.GetDamage(t as AIHeroClient) * 2 : 0;

            if (ComboMode == ComboMode.Mode2xQ)
            {
                fComboDamage += ComboDamage2xQ(t as AIHeroClient);
            }

            if (ComboMode == ComboMode.Mode2xW)
            {
                fComboDamage += ComboDamage2xW(t as AIHeroClient);
            }

            fComboDamage += Common.CommonSummoner.IgniteSlot != SpellSlot.Unknown &&
                            ObjectManager.Player.Spellbook.CanUseSpell(Common.CommonSummoner.IgniteSlot) ==
                            SpellState.Ready
                ? (float)ObjectManager.Player.GetSummonerSpellDamage(t as AIHeroClient, LeagueSharp.Common.Damage.SummonerSpell.Ignite)
                : 0f;

            //fComboDamage += Items.CanUseItem(3092)
            //    ? (float) ObjectManager.Player.GetItemDamage(t as AIHeroClient, Damage.DamageItems.FrostQueenClaim)
            //    : 0;

            return (float)fComboDamage;
        }

        public static float GetComboDamage(AIHeroClient t, bool q, bool w, bool e, bool rQ, bool rW, bool ignite)
        {
            double fComboDamage = 0;


            fComboDamage += q && Q.IsReady() ? Q.GetDamage(t as AIHeroClient) * 2 : 0;

            fComboDamage += w && W.IsReady() ? W.GetDamage(t as AIHeroClient) : 0;

            fComboDamage += e && E.IsReady() ? E.GetDamage(t as AIHeroClient) * 2 : 0;

            if (rQ && ComboMode == ComboMode.Mode2xQ)
            {
                fComboDamage += ComboDamage2xQ(t as AIHeroClient);
            }

            if (rW && ComboMode == ComboMode.Mode2xW)
            {
                fComboDamage += ComboDamage2xW(t as AIHeroClient);
            }

            fComboDamage += ignite && Common.CommonSummoner.IgniteSlot != SpellSlot.Unknown &&
                            ObjectManager.Player.Spellbook.CanUseSpell(Common.CommonSummoner.IgniteSlot) ==
                            SpellState.Ready
                ? (float)ObjectManager.Player.GetSummonerSpellDamage(t as AIHeroClient, LeagueSharp.Common.Damage.SummonerSpell.Ignite)
                : 0f;

            //fComboDamage += Items.CanUseItem(3092)
            //    ? (float)ObjectManager.Player.GetItemDamage(t as AIHeroClient, Damage.DamageItems.FrostQueenClaim)
            //    : 0;

            return (float)fComboDamage;
        }
        private static ComboMode GetBestComboMode(AttackableUnit t)
        {

            double[] damages = new double[3]; // 3
            ComboMode[] modes = new ComboMode[3]; // 3

            var max = Q.GetDamage(t as AIHeroClient) + ComboDamage2xQ(t as AIHeroClient);

            var result = 0;

            damages[0] = Q.GetDamage(t as AIHeroClient) + ComboDamage2xQ(t as AIHeroClient);
            modes[0] = ComboMode.Mode2xQ;

            damages[1] = W.GetDamage(t as AIHeroClient) + ComboDamage2xW(t as AIHeroClient);
            modes[0] = ComboMode.Mode2xW;

            for (int i = 0; i < 3; i++) //8
            {
                if (!(max < damages[i]))
                {
                    continue;
                }

                max = damages[i];
                result = i;
            }

            return modes[result];
        }


        public static double ComboDamage2xQ(Obj_AI_Base t)
        {
            if (R.Level == 0)
            {
                return 0;
            }

            var qDmg = ObjectManager.Player.CalcDamage(t, DamageType.Magical, new double[] { 100, 200, 300 }[R.Level - 1] + .6f * ObjectManager.Player.FlatMagicDamageMod);
            return qDmg * 2;
        }

        private static double ComboDamage2xW(Obj_AI_Base t)
        {
            if (R.Level == 0)
            {
                return 0;
            }

            var perDmg = new[] { 150f, 300f, 450 };
            var dmg = ObjectManager.Player.CalcDamage(t, DamageType.Magical,
                perDmg[R.Level - 1] + .9f * ObjectManager.Player.FlatMagicDamageMod);
            return dmg * 2 + W.GetDamage(t);
        }

        private static Vector2 GetJumpPosition(AIHeroClient t, float range, string name = "first")
        {
            List<Vector2> xList = new List<Vector2>();

            Vector2 location = ObjectManager.Player.Position.LSTo2D() +
                               Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D()) * W.Range;
            Vector2 wCastPosition = location;

            //Render.Circle.DrawCircle(wCastPosition.To3D(), 105f, System.Drawing.Color.Red);


            if (!wCastPosition.LSIsWall())
            {
                xList.Add(wCastPosition);
            }

            if (!wCastPosition.LSIsWall())
            {
                ExistingJumpPositions.Add(new ListJumpPositions
                {
                    Position = wCastPosition,
                    Name = name
                });

                ListWJumpPositions.Add(wCastPosition);
            }

            if (wCastPosition.LSIsWall())
            {
                for (int j = 20; j < 80; j += 20)
                {
                    Vector2 wcPositive = ObjectManager.Player.Position.LSTo2D() +
                                         Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D())
                                             .LSRotated(j * (float)Math.PI / 180) * W.Range;
                    if (!wcPositive.LSIsWall())
                    {
                        ListWJumpPositions.Add(wcPositive);
                    }

                    Vector2 wcNegative = ObjectManager.Player.Position.LSTo2D() +
                                         Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D())
                                             .LSRotated(-j * (float)Math.PI / 180) * W.Range;
                    if (!wcNegative.LSIsWall())
                    {
                        ListWJumpPositions.Add(wcNegative);
                    }
                }

                float xDiff = ObjectManager.Player.Position.X - t.Position.X;
                float yDiff = ObjectManager.Player.Position.Y - t.Position.Y;
                int angle = (int)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            }

            //foreach (var aa in ListWJumpPositions)
            //{
            //    Render.Circle.DrawCircle(aa.To3D2(), 105f, System.Drawing.Color.White);
            //}
            var al1 = xList.OrderBy(al => al.LSDistance(t.Position)).First();

            var color = System.Drawing.Color.DarkRed;
            var width = 4;

            var startpos = ObjectManager.Player.Position;
            var endpos = al1.To3D();
            if (startpos.LSDistance(endpos) > 100)
            {
                var endpos1 = al1.To3D() +
                              (startpos - endpos).LSTo2D().LSNormalized().LSRotated(25 * (float)Math.PI / 180).To3D() * 75;
                var endpos2 = al1.To3D() +
                              (startpos - endpos).LSTo2D().LSNormalized().LSRotated(-25 * (float)Math.PI / 180).To3D() * 75;

                //var x1 = new LeagueSharp.Common.Geometry.Polygon.Line(startpos, endpos);
                //x1.Draw(color, width - 2);
                new LeagueSharp.Common.Geometry.Polygon.Line(startpos, endpos).Draw(color, width - 2);


                var y1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos, endpos1);
                y1.Draw(color, width - 2);
                var z1 = new LeagueSharp.Common.Geometry.Polygon.Line(endpos, endpos2);
                z1.Draw(color, width - 2);
            }


            //foreach (var al in ListWJumpPositions.OrderBy(al => al.Distance(t.Position)))
            //{
            //    Render.Circle.DrawCircle(al.To3D(), 105f, System.Drawing.Color.White);
            //}
            //            Render.Circle.DrawCircle(al1.To3D(), 85, System.Drawing.Color.White);
            return al1;
        }

    }
}