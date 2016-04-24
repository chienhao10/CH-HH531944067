using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Microsoft.Win32;
using SharpDX;
using System.Threading;
using System.Net;
using EloBuddy.SDK.Rendering;
using System.Diagnostics;
using Color = System.Drawing.Color;

namespace UnrealSkill
{
    //OGIRINAL GETHUB: https://github.com/Soresu/LeagueSharp
    //CONVERT BY UNREALSKILL99
    //FROM ELOBUDDY
    internal class Gangplank
    {
        #region Public
        public static Menu Menu;
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static string NomeHeroi = "GangPlank";
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static readonly Obj_AI_Base InimigoGlobal;
        public static bool justQ, justE;
        public Vector3 ePos;
        public const int BarrelExplosionRange = 350;
        public const int BarrelConnectionRange = 700;
        public static List<Barrel> savedBarrels = new List<Barrel>();
        public static Text Text = new EloBuddy.SDK.Rendering.Text("", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 10, System.Drawing.FontStyle.Bold));
        public static Text Text2 = new EloBuddy.SDK.Rendering.Text("", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 20, System.Drawing.FontStyle.Bold));
        #endregion Public
        public Gangplank()
        {
            InitGangPlank();
            Chat.Print("|| GangPlank Load || Credit <font color='#00CC00'><b>Soresu</b></font> <b><font color='#002eff'> & </font><font color='#FF0000'>UnrealSkill99</font></b> & </font><font color='#FF0000'>CH</font></b>|| Challenger Script To EB GP", Color.White);
            Chat.Print("|| GangPlank Load || Credit <font color='#00CC00'><b>Soresu</b></font> <b><font color='#002eff'> & </font><font color='#FF0000'>UnrealSkill99</font></b> & </font><font color='#FF0000'>CH</font></b>|| Challenger Script To EB GP", Color.White);
            Drawing.OnDraw += Game_OnDraw2;
            Loading.OnLoadingComplete += InitMenu;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }
        #region Etc...
        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (savedBarrels[i].barrel.NetworkId == sender.NetworkId || savedBarrels[i].barrel.IsDead)
                {
                    savedBarrels.RemoveAt(i);
                    return;
                }
            }
        }
        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion, System.Environment.TickCount));
            }
        }
        public static IEnumerable<Obj_AI_Minion> Barrils()
        {
            return savedBarrels.Select(b => b.barrel).Where(b => b.IsValid);
        }
        public static IEnumerable<Obj_AI_Base> BarrilsIE()
        {
            return savedBarrels.Select(b => b.barrel).Where(b => b.IsValid);
        }
        public static bool KillableBarrel(Obj_AI_Base targetB,bool melee = false,Obj_AI_Base sender = null, float missileTravelTime = -1)
        {
            if (targetB.Health < 2) return true;
            if (sender == null)  sender = player;
           // if (missileTravelTime == -1) missileTravelTime = GetQTime(targetB);
            var barrel = savedBarrels.FirstOrDefault(b => b.barrel.NetworkId == targetB.NetworkId);
           /* if (barrel != null)
            {
                var time = targetB.Health * getEActivationDelay() * 1000;
                if (System.Environment.TickCount - barrel.time + (melee ? sender.AttackDelay : missileTravelTime) * 1000 > time)
                {
                    return true;
                }
            }*/
            return false;
        }
        public static float GetQTime(Obj_AI_Base targetB)
        {
            return player.Distance(targetB) / 2800f + Q.CastDelay;
        }
        private void InitGangPlank()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 625);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Circular, 450, 2000, 390) { AllowedCollisionCount = int.MaxValue };
            R = new Spell.Skillshot(SpellSlot.R, int.MaxValue, SkillShotType.Circular, 250, int.MaxValue, 600) { AllowedCollisionCount = int.MaxValue };

        }
        public static List<Vector3> PointsAroundTheTarget(Vector3 pos, float dist, float prec = 15, float prec2 = 6)
        {
            if (!pos.IsValid())
            {
                return new List<Vector3>();
            }
            List<Vector3> list = new List<Vector3>();
            if (dist > 205)
            {
                prec = 30;
                prec2 = 8;
            }
            if (dist > 805)
            {
                dist = (float)(dist * 1.5);
                prec = 45;
                prec2 = 10;
            }
            var angle = 360 / prec * Math.PI / 180.0f;
            var step = dist * 2 / prec2;
            for (int i = 0; i < prec; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    list.Add(new Vector3(pos.X + (float)(Math.Cos(angle * i) * (j * step)), pos.Y + (float)(Math.Sin(angle * i) * (j * step)) - 90, pos.Z));
                }
            }

            return list;
        }
        #endregion Etc...
        //Sempre Ativo
        public static void SempreAtivado()
        {
            // Auto remover CC
            var AutoUseW = Menu["W"].Cast<CheckBox>().CurrentValue;
            if (AutoUseW)
            {
                if (Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Slow) || Player.HasBuffOfType(BuffType.Blind) || Player.HasBuffOfType(BuffType.Suppression)
                || Player.HasBuffOfType(BuffType.Snare) || Player.HasBuffOfType(BuffType.Taunt))
                {
                    Core.DelayAction(() => W.Cast(), 400);
                }
            }
            //Auto Roubar Jungle
            var AutoKillStealJG = Menu["AQ"].Cast<CheckBox>().CurrentValue;
            if (AutoKillStealJG)
            {
                var JungleClear = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).FirstOrDefault(e => e.IsValidTarget() && e.Health <= Player.Instance.GetSpellDamage(e, SpellSlot.Q));
                if (Q.IsReady() && JungleClear.IsValidTarget(Q.Range + 150))
                {
                    Q.Cast(JungleClear);
                }
            }
            //Auto KS
            if (true)
            {
                foreach (var hero in EntityManager.Heroes.Enemies.Where(hero => !hero.IsMe && !hero.IsDead && hero.IsHPBarRendered == true))
                {
                    var percent = (int)(hero.Health / hero.MaxHealth * 100);
                    if (percent <= 10)
                    {
                        var AutoRKS = Menu["R2"].Cast<CheckBox>().CurrentValue;
                        if (hero.Distance(Player.Instance) <= 625)
                        {
                            if (hero.Health <= Player.Instance.GetSpellDamage(hero, SpellSlot.Q))
                            {
                                //Chat.Print("Auto KillSteal [Q] " + hero.ChampionName, Color.White);
                                Q.Cast(hero);
                            }
                        }
                        else if (AutoRKS && R.IsReady())
                        {
                            var vec = hero.ServerPosition / ObjectManager.Player.Position;
                            var Logica = R.GetPrediction(hero).CastPosition + Vector3.Normalize(vec) * 100;
                            Player.Instance.Spellbook.CastSpell(SpellSlot.R, Logica);
                            //Chat.Print("Auto KillSteal [R] " + hero.ChampionName,Color.White);
                        }

                    }
                }

                // Usar R Em Team Figth
                var UsaR = Menu["R"].Cast<CheckBox>().CurrentValue;
                var Clear = EntityManager.Heroes.Enemies.Where(m => m.IsValidTarget(R.Width)).ToArray();
                if (UsaR)
                {
                    foreach (var Rtarget in EntityManager.Heroes.Allies.Where(i => !i.IsDead && i.CountEnemiesInRange(600) >= 2))
                    {
                        if (Rtarget != null)
                        {
                            Player.Instance.Spellbook.CastSpell(SpellSlot.R, Rtarget);
                        }
                    }
                }
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                // Usa Q em Barril
                var Explodir = ObjectManager.Get<Obj_AI_Base>().Where(o => o.BaseSkinName == "GangplankBarrel" && o.Distance(Player.Instance) <= Q.Range && o.Health == 1 && o.CountEnemiesInRange(350) >= 1).FirstOrDefault();
                if (Explodir != null)
                {
                    Q.Cast(Explodir);
                }
                else
                {
                    Combo();
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) Clear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))Lasthit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))Jungle();
            SempreAtivado();// Sempre Ativado
            LoadingSkin();// UsaSkinHack
        }
        public static void Lasthit()
        {
            // LastHit Farm
            var LastHit = Menu["QL"].Cast<CheckBox>().CurrentValue;
            if (Q.IsReady() && LastHit)
            {
                foreach(var Farm in EntityManager.MinionsAndMonsters.GetLaneMinions().Where(i => i.Health <= Player.Instance.GetSpellDamage(i, SpellSlot.Q)))
                {
                    Q.Cast(Farm);
                }
            }
        }
        private static void Clear()
        {
            var Clear = EntityManager.MinionsAndMonsters.GetLaneMinions().Where(m => m.IsValidTarget(E.Range)).ToArray();
            var pos = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(Clear, BarrelExplosionRange, 1000);
            var B = BarrilsIE().FirstOrDefault(b => b.Distance(pos.CastPosition) <= BarrelExplosionRange);
            if (B == null) Lasthit();
            // ColocaBarril
            var UsaBarrilFarm = Menu["EF"].Cast<CheckBox>().CurrentValue;
            if (UsaBarrilFarm)
            {
                //if (Clear.Length == 0) return;
                if (pos.HitNumber >= 3 && B == null) E.Cast(pos.CastPosition);
            }
            // Atira em Barrel
            var UsaQFarm = Menu["QF"].Cast<CheckBox>().CurrentValue;
            if (UsaQFarm)
            {
                foreach (var BarrelCircular in ObjectManager.Get<Obj_AI_Base>().Where(o => o.BaseSkinName == "GangplankBarrel" && o.Health == 1))
                {
                    if (BarrelCircular != null && Q.IsReady())
                    {
                        var Clear1 = EntityManager.MinionsAndMonsters.GetLaneMinions().Count(i => i.IsInRange(B, BarrelExplosionRange) && i.IsEnemy && !i.IsDead);
                        if (Clear1 > 0)
                        {
                            Core.DelayAction(() => Q.Cast(B), 100);
                        }
                    }
                }
            }
        }
        private static void Jungle()
        {
            // Usa Q em Jungle
            var UsaQJG = Menu["QJ"].Cast<CheckBox>().CurrentValue;
            var JungleClear = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).FirstOrDefault(e => e.IsValidTarget() && e.HealthPercent >= 20);
            if (Q.IsReady() && JungleClear.IsValidTarget(Q.Range) && UsaQJG)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, JungleClear);
            }
        }
        public static void Combo()
        {
            // Usa Q
            var UsaQ = Menu["Q"].Cast<CheckBox>().CurrentValue;
            // Usa W
            var UsaW = Menu["W2"].Cast<CheckBox>().CurrentValue;
            // Usa E
            var UsaE = Menu["E"].Cast<CheckBox>().CurrentValue;
            // Slide W
            var UsaW2 = Menu["WPerc"].Cast<Slider>().CurrentValue;
            // Usa Barril Extendido
            var EExtendido = Menu["E2"].Cast<CheckBox>().CurrentValue;
            // Usa AOE Extendido
            var AOEExtendido = Menu["AOE3"].Cast<Slider>().CurrentValue;

            // Usa Modo Barrel Extendido
            var ModoBExtended = Menu["SS"].Cast<ComboBox>().DisplayName;
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target == null) return;
            if (true)//Atk Barril se estiver com 3 de Vida
            {
                var AtkBarrel = Barrils().OrderBy(o => o.IsValid).FirstOrDefault(o => o.Health >= 2 && o.Health >= 1 && o.Distance(player) < Player.Instance.GetAutoAttackRange(o));
                if (AtkBarrel != null && AtkBarrel.Health >= 2)
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, AtkBarrel);//Auto estaka Barrel Atacando
                    //return;
                }
            }
            if (Player.Instance.HealthPercent <= UsaW2 && UsaW && Player.Instance.CountEnemiesInRange(800) >= 1) W.Cast();
            var NaoUsaQ = false;
            var barrels = Barrils().Where(o =>o.IsValid && !o.IsDead && o.Distance(Player.Instance) < 1600 && o.BaseSkinName == "GangplankBarrel").ToList();

            if (EExtendido && Q.IsReady() && E.IsReady())
            {
                var Prediction = E.GetPrediction(target);
                if (Prediction.HitChance >= HitChance.Low)
                {
                    var Qbarrels = Barrils().Where(o => o.Distance(Player.Instance) <= Q.Range + 100 && KillableBarrel(o));
                    foreach (var Qbarrel in Qbarrels)//.OrderByDescending(b => b.Distance(target) <= BarrelExplosionRange))
                    {
                        if (ModoBExtended == "☑ 1 职业")//Modo Profissional
                        {
                            if (Qbarrel.Distance(Prediction.UnitPosition) <= BarrelConnectionRange)
                            {
                                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.Distance(Player.Instance) <= 2000);
                                    var pos = GetBarrelPoints(Qbarrel.Position).Where(p => p.Distance(Qbarrel.Position) <= BarrelConnectionRange).OrderByDescending(p => p.Distance(p) <= BarrelExplosionRange).ThenBy(p => p.Distance(target.Position)).FirstOrDefault();           
                                    NaoUsaQ = true;
                                    E.Cast(pos);
                            }
                        }
                        if (ModoBExtended == "☑ 2 高级")//Modo Avançado
                        {
                        var point = GetBarrelPoints(Qbarrel.Position).Where(p =>p.IsValid() && !p.IsWall() && p.Distance(player.Position) < E.Range && target.Distance(p) < BarrelExplosionRange &&
                            p.Distance(Prediction.UnitPosition) <= BarrelExplosionRange && Qbarrel.Distance(p) <= BarrelConnectionRange && 
                            savedBarrels.Count(b => b.barrel.Position.Distance(p) <= BarrelExplosionRange) < 1)
                            .OrderBy(p => p.Distance(Prediction.UnitPosition)).FirstOrDefault();
                        if (point.IsValid())
                        {
                            NaoUsaQ = true;
                            E.Cast(point);
                        } 
                        }
                    }
                }
            }

            if (true)
            {
                if (barrels.Any() && Q.IsReady())
                {
                    var detoneateTargetBarrels = barrels.Where(b => b.Distance(player) <= Q.Range);
                    if (true)//Auto Atirar no Barril se detectado Inimigo Dentro
                    {
                        if (detoneateTargetBarrels.Any())
                        {
                            foreach (var detoneateTargetBarrel in detoneateTargetBarrels)
                            {
                                if (!KillableBarrel(detoneateTargetBarrel)) continue;
                                if (detoneateTargetBarrel.Distance(target) < BarrelExplosionRange && target.Distance(detoneateTargetBarrel.Position) < BarrelExplosionRange)
                                {
                                    NaoUsaQ = true;
                                    Q.Cast(detoneateTargetBarrel);
                                    return;
                                }
                                var detoneateTargetBarrelSeconds = barrels.Where(b => b.Distance(detoneateTargetBarrel) <= BarrelConnectionRange);
                                if (detoneateTargetBarrelSeconds.Any())
                                {
                                    foreach (var detoneateTargetBarrelSecond in detoneateTargetBarrelSeconds)
                                    {
                                        if (detoneateTargetBarrelSecond.Distance(target) <= BarrelExplosionRange && target.Distance(detoneateTargetBarrelSecond.Position) <= BarrelExplosionRange)
                                        {
                                            NaoUsaQ = true;
                                            Q.Cast(detoneateTargetBarrel);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        if (AOEExtendido > 0)// 1 a 5 - padrao 2 - Blow up enemies with E
                        {
                            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget() && e.Distance(Player.Instance) < 1050 && e.IsHPBarRendered == true); //normal 600
                            if (detoneateTargetBarrels.Any())
                            {
                                foreach (var detoneateTargetBarrel in detoneateTargetBarrels)
                                {
                                    if (!KillableBarrel(detoneateTargetBarrel))
                                    {
                                        continue;
                                    }
                                    var enemyCount = enemies.Count(e => e.Distance(detoneateTargetBarrel.Position) < BarrelExplosionRange);
                                    if (enemyCount >= AOEExtendido && detoneateTargetBarrel.CountEnemiesInRange(BarrelExplosionRange) >= AOEExtendido)
                                    {
                                        NaoUsaQ = true;
                                        Q.Cast(detoneateTargetBarrel);
                                        return;
                                    }
                                    var detoneateTargetBarrelSeconds = barrels.Where(b => b.Distance(detoneateTargetBarrel) < BarrelConnectionRange);
                                    if (detoneateTargetBarrelSeconds.Any())
                                    {
                                        foreach (var detoneateTargetBarrelSecond in detoneateTargetBarrelSeconds)
                                        {
                                            if (enemyCount + enemies.Count(e =>e.Distance(detoneateTargetBarrelSecond.Position) < BarrelExplosionRange) >= AOEExtendido && detoneateTargetBarrelSecond.CountEnemiesInRange(BarrelExplosionRange) >= AOEExtendido)
                                            {
                                                NaoUsaQ = true;
                                                Q.Cast(detoneateTargetBarrel);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
             /*   if (E.IsReady() && player.Distance(target) < E.Range &&//useeAlways menu
                    !justE && target.Health > Player.Instance.GetSpellDamage(target, SpellSlot.Q) + player.GetAutoAttackDamage(target) &&
                    Player.Instance.CanMove && 1 < Player.Instance.Spellbook.GetSpell(SpellSlot.E).Ammo) //eStacksC menu
                {
                    CastE(target, barrels);
                }*/

                var Barrils = BarrilsIE().FirstOrDefault(b => b.IsInRange(Player.Instance, Q.Range + 100));
                if (UsaE && Barrils == null && target.Distance(Player.Instance) <= E.Range)
                {
                    var vec = target.ServerPosition / ObjectManager.Player.Position;
                    var Logica = E.GetPrediction(Player.Instance).CastPosition + Vector3.Normalize(vec) * 350;
                    var Logica2 = E.GetPrediction(Player.Instance).CastPosition + Vector3.Normalize(vec) - 10;
            
                    if (Player.Instance.Distance(target.Position) >= 620)
                    {
                        E.Cast(Logica);
                        CastE(target, barrels);
                    }
                    if (Player.Instance.Distance(target.Position) <= 610)
                    {
                        E.Cast(Logica2);
                    }
                    
                }
                var Qbarrels = BarrilsIE().FirstOrDefault(o => o.Distance(player) < Q.Range + 100);
                if (Qbarrels != null && Player.Instance.Spellbook.GetSpell(SpellSlot.E).Ammo > 0 && Q.IsReady() && target.Health > Player.Instance.GetSpellDamage(target, SpellSlot.Q))
                {
                    NaoUsaQ = true;
                }
                if (!NaoUsaQ && UsaQ)
                {
                    CastQonHero(target, barrels);
                }
            }
        }
        private static void CastQonHero(Obj_AI_Base target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.FirstOrDefault(b => target.Distance(b.Position) <= BarrelExplosionRange) != null) return;// && target.Health > Player.Instance.GetSpellDamage(target, SpellSlot.Q)) return;
            Q.Cast(target);
        }
        private static void CastE(Obj_AI_Base target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.Count(b => b.CountEnemiesInRange(BarrelConnectionRange) >= 1) <= 1)
            {
                if (true)
                {
                    CastEtarget(target);
                }
                return;
            }
            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget() && e.Distance(player) < E.Range);
            List<Vector3> points = new List<Vector3>();
            foreach (var barrel in barrels.Where(b => b.Distance(player) < Q.Range && KillableBarrel(b)))
            {
                if (barrel != null)
                {
                    var newP = GetBarrelPoints(barrel.Position).Where(p => !p.IsWall());
                    if (newP.Any())
                    {
                        points.AddRange(newP.Where(p => p.Distance(player.Position) < E.Range));
                    }
                }
            }
            var bestPoint = points.Where(b => enemies.Count(e => e.Distance(b) < BarrelExplosionRange) > 0).OrderByDescending(b => enemies.Count(e => e.Distance(b) < BarrelExplosionRange)).FirstOrDefault();
            if (bestPoint.IsValid() && !savedBarrels.Any(b => b.barrel.Position.Distance(bestPoint) < BarrelConnectionRange))
            {
                E.Cast(bestPoint);
            }
        }
        private static void CastEtarget(Obj_AI_Base target)
        {
            var Prediction = E.GetPrediction(target); ;
            var pos = Prediction.CastPosition;
            if (pos.Distance(pos) >= 900)//400
            {
                E.Cast(pos);
            }
        }
        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "GangplankQWrapper")
                {
                    if (!justQ)
                    {
                        justQ = true;
                        Core.DelayAction(() => justQ = false, 200);
                    }
                }
                if (args.SData.Name == "GangplankE")
                {
                    ePos = args.End;
                    if (!justE)
                    {
                        justE = true;
                        Core.DelayAction(() => justE = false, 200);
                    }
                }
            }
            if (sender.IsEnemy && sender is Obj_AI_Base && sender.Distance(player) < E.Range)
            {
                var targetBarrels =savedBarrels.Where(b => b.barrel.NetworkId == args.Target.NetworkId && KillableBarrel(b.barrel, sender.IsMelee, (Obj_AI_Base) sender, sender.Distance(b.barrel) / args.SData.MissileSpeed));
                foreach (var barrelData in targetBarrels)
                {
                    if (barrelData.barrel.Distance(player) < Player.Instance.GetAutoAttackRange(barrelData.barrel) && Player.Instance.CanAttack && !justQ)
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, barrelData.barrel);
                    }
                    else
                    {
                        savedBarrels.Remove(barrelData);
                        Console.WriteLine("Barrel removed");
                    }
                }
            }
        }
        private static IEnumerable<Vector3> GetBarrelPoints(Vector3 point)
        {
            return PointsAroundTheTarget(point, BarrelConnectionRange, 20f).Where(p => p.Distance(point) >= BarrelExplosionRange);
        }
        public static float getEActivationDelay()
        {
            if (player.Level >= 13)
            {
                return 0.5f;
            }
            if (player.Level >= 7)
            {
                return 1f;
            }
            return 2f;
        }
        private static void JungleVida()
        {
            var ShowTextHPJungle = Menu["Draw-JGHP"].Cast<CheckBox>().CurrentValue;
            if (ShowTextHPJungle)
            {
                foreach (var JungleClearHP in EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range + 150).Where(e => e.IsValidTarget()))
                {
                    if (JungleClearHP.Health > Player.Instance.GetSpellDamage(JungleClearHP, SpellSlot.Q))
                    {
                        var Vida = (int)(JungleClearHP.Health / JungleClearHP.MaxHealth * 100);
                        Drawing.DrawText(Drawing.WorldToScreen(JungleClearHP.Position) - new Vector2(30, -30), Color.White, "HP " + Vida.ToString(), 2);
                    }
                    else if (JungleClearHP.Health <= Player.Instance.GetSpellDamage(JungleClearHP, SpellSlot.Q))
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(JungleClearHP.Position) - new Vector2(30, -30), Color.Red, "Kill", 2);
                    }
                }
            }
        }
        public static void LoadingSkin()
        {
            foreach (var Objeto in Barrils())
            {
                var SkinHackSelect = Menu["SkinHack"].DisplayName;
                switch (SkinHackSelect)
                {
                    case "Classic Gangplank": Objeto.SetSkinId(0);  break;
                    case "Spooky Gangplank":Objeto.SetSkinId(1);  break;
                    case "Minuteman Gangplank": Objeto.SetSkinId(2);   break;
                    case "Sailor Gangplank":  Objeto.SetSkinId(3);   break;
                    case "Toy Soldier Gangplank":Objeto.SetSkinId(4);  break;
                    case "Special Forces Gangplank": Objeto.SetSkinId(5); break;
                    case "Sultan Gangplank": Objeto.SetSkinId(6);break;
                    case "Captain Gangplank": Objeto.SetSkinId(7); break;
                }
            }
            if (Menu["UseSkinHack"].Cast<CheckBox>().CurrentValue)
            {
                if (Menu.Get<KeyBind>("SkinLoad").CurrentValue)
                {
                    var SkinHackSelect1 = Menu["SkinHack"].DisplayName;
                    switch (SkinHackSelect1)
                    {
                        case "Classic Gangplank": Player.Instance.SetSkinId(0);  break;
                        case "Spooky Gangplank": Player.Instance.SetSkinId(1); break;
                        case "Minuteman Gangplank": Player.Instance.SetSkinId(2); break;
                        case "Sailor Gangplank": Player.Instance.SetSkinId(3);  break;
                        case "Toy Soldier Gangplank": Player.Instance.SetSkinId(4); break;
                        case "Special Forces Gangplank":  Player.Instance.SetSkinId(5); break;
                        case "Sultan Gangplank":Player.Instance.SetSkinId(6); break;
                        case "Captain Gangplank": Player.Instance.SetSkinId(7); break;
                    }
                }
            }
        }
        private static void DrawHealths()
        {//Crédit Soresu From LeagueSharp
            var DrawTxt = Menu["Draw-Text"].Cast<CheckBox>().CurrentValue;
            if (DrawTxt)
            {
                float i = 0;
                foreach (var hero in EntityManager.Heroes.Enemies.Where(hero => !hero.IsMe && !hero.IsDead))
                {
                    var playername = hero.Name;
                    if (playername.Length > 13)
                    {
                        playername = playername.Remove(9) + "...";
                    }
                    var champion = hero.ChampionName;
                    if (champion.Length > 12)
                    {
                        champion = champion.Remove(7) + "...";
                    }
                    var percent = (int)(hero.Health / hero.MaxHealth * 100);
                    var color = Color.Red;
                    if (percent > 25)
                    {
                        color = Color.Orange;
                    }
                    if (percent > 50)
                    {
                        color = Color.Yellow;
                    }
                    if (percent > 75)
                    {
                        color = Color.LimeGreen;
                    }
                    Drawing.DrawText(Drawing.Width * 0.8f, Drawing.Height * 0.15f + i, color, "[ " + champion + " ]");
                    Drawing.DrawText(Drawing.Width * 0.9f, Drawing.Height * 0.15f + i, color, ((int)hero.Health).ToString() + " [" + percent.ToString() + "%]");
                    i += 20f;
                }
            }
        }
        private static void Game_OnDraw2(EventArgs args)
        {
            DrawHealths();// Hp Bar Enemy
            JungleVida();//Vida JG
            var SkinHackSelect = Menu["SkinHack"].DisplayName;
            Color color;
            switch (SkinHackSelect)
            {
                default: color = Color.Transparent;  break;
                case "Classic Gangplank": color = Color.MediumVioletRed; break;
                case "Spooky Gangplank": color = Color.DeepSkyBlue; break;
                case "Minuteman Gangplank": color = Color.MediumBlue; break;
                case "Sailor Gangplank":color = Color.WhiteSmoke; break;
                case "Toy Soldier Gangplank": color = Color.DarkRed; break;
                case "Special Forces Gangplank": color = Color.MediumSeaGreen; break;
                case "Sultan Gangplank": color = Color.BlueViolet;  break;
                case "Captain Gangplank": color = Color.Maroon; break;
            }
            var DrawQ = Menu["Draw-Q"].Cast<CheckBox>().CurrentValue;
            var DrawE = Menu["Draw-E"].Cast<CheckBox>().CurrentValue;
            var DrawBarrel = Menu["Draw-Barrel"].Cast<CheckBox>().CurrentValue;
            var DrawBarrelExtend = Menu["Draw-BarrelExtend"].Cast<CheckBox>().CurrentValue;

            if (DrawQ)
            {
                Drawing.DrawCircle(Player.Instance.Position, Q.Range, color);
                Drawing.DrawCircle(Player.Instance.Position, Player.Instance.GetAutoAttackRange(), color);
            }
            if (DrawE) Drawing.DrawCircle(Player.Instance.Position, E.Range, color);
            if (DrawBarrel)
            {
                foreach (var barrel in Barrils())
                { Drawing.DrawCircle(barrel.Position, 350f, color); }
            }
            if (DrawBarrelExtend)
            {
                foreach (var RangeEextend in Barrils())
                { Drawing.DrawCircle(RangeEextend.Position, 700f, color); }
            }
            if (true )
            {
                //Drawing.DrawCircle(Game.CursorPos, BarrelExplosionRange, color);
            }
            /////////////////////////////////////////////////////////////

            var DrawETxt = Menu["Draw-Eexplosion"].Cast<CheckBox>().CurrentValue;
            if (DrawETxt)
            {
                foreach (var Objeto in Barrils())
                {
                    Text2.Position = Drawing.WorldToScreen(Objeto.Position) - new Vector2(20 - 20);
                    Text2.Color = Color.White;
                    Text2.TextValue = Objeto.Health.ToString();
                    Text2.Draw();

                    var InimigoNoRangeDoE = EntityManager.Heroes.Enemies.Where(o => o.IsValid && o.CountEnemiesInRange(700) >= 1);
                    var ContarInimigos = InimigoNoRangeDoE;
                    if (Objeto.IsValid && Objeto.CountEnemiesInRange(700) >= 1)//CountEnemiesInRange(390) >= 1))
                    {
                        Text.Position = Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(43, 100);
                        Text.Color = Color.White;
                        Text.TextValue = " [✖] 延长距离炸桶";
                        Text.Draw();
                        if (Objeto.CountEnemiesInRange(350) >= 1)
                        {
                            Text.Position = Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(43, 80);
                            Text.Color = Color.White;
                            Text.TextValue = " [✖] 敌人距离";
                            Text.Draw();
                        }
                    }
                }
            }
            if (false)
            {
                foreach (var barrelData in savedBarrels)
                {
                    float time =
                        Math.Min(
                            System.Environment.TickCount - barrelData.time -
                            barrelData.barrel.Health * getEActivationDelay() * 1000f, 0) / 1000f;
                    if (time < 0)
                    {
                        Drawing.DrawText(
                            barrelData.barrel.HPBarPosition.X - time.ToString().Length * 5 + 40,
                            barrelData.barrel.HPBarPosition.Y - 20, Color.DarkOrange,
                            string.Format("{0:0.00}", time).Replace("-", ""));
                    }
                }
            }
        }
        public static void InitMenu(EventArgs args)
        {
            Menu = MainMenu.AddMenu(NomeHeroi, NomeHeroi);
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.AddLabel("〈〈〈〈 连招 〉〉〉〉");
            Menu.Add("Q", new CheckBox("❐   " + NomeHeroi + " [Q] - 敌人", true));
            Menu.Add("W", new CheckBox("❐   " + NomeHeroi + " [W] - 自动解控", true));
            Menu.Add("E", new CheckBox("❐   " + NomeHeroi + " [E] - 逻辑", true));
            Menu.Add("R", new CheckBox("❐   " + NomeHeroi + " [R] - 团战", true));
            Menu.Add("R2", new CheckBox("❐   " + NomeHeroi +" [R] [Q] - 抢头", true));
            Menu.AddGroupLabel("推荐关闭 [E] - 逻辑 & [R] - 团战");
            Menu.AddGroupLabel("手动放下炸桶然后使用连招来自动完成");
            Menu.AddGroupLabel("下一个要放的炸桶位置");
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.Add("W2", new CheckBox("❐   " + NomeHeroi + " [W] - 设置", true));
            var WHP = Menu.Add("WPerc", new Slider("% 血量使用W", 60, 10, 80));
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.AddLabel("〈〈〈〈 延长距离炸桶 〉〉〉〉");
            Menu.Add("E2", new CheckBox("❐   " + NomeHeroi + " [E] - 延长距离炸桶", true));
            var Modes = new[] { "☑ 1 职业", "☑ 2 高级" };
            var BarrelMode = Menu.Add("SS", new ComboBox("选择模式 [延长距离炸桶] 推荐 [职业]", Modes));
            BarrelMode.DisplayName = Modes[BarrelMode.CurrentValue];
            BarrelMode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs) { sender.DisplayName = Modes[changeArgs.NewValue]; }; ;
            Menu.AddGroupLabel("推荐炸桶模式 [ ☑ 2 高级 ]");
            var AOE1 = Menu.Add("AOE3", new Slider("AOE 炸桶延迟数量 [推荐 2]", 2, 1, 5));
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.AddLabel("〈〈〈〈 农兵 〉〉〉〉");
            Menu.Add("AQ", new CheckBox("❐   " + NomeHeroi + " [Q] 自动抢野", true));
            Menu.Add("QJ", new CheckBox("❐   " + NomeHeroi + " [Q] 打野", true));
            Menu.Add("QL", new CheckBox("❐   " + NomeHeroi + " [Q] 尾兵", true));
            Menu.Add("EF", new CheckBox("❐   " + NomeHeroi + " [E] 农兵", true));
            Menu.Add("QF", new CheckBox("❐   " + NomeHeroi + " [Q] 炸桶", true));
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.AddLabel("〈〈〈〈 线圈 1 〉〉〉〉");
            Menu.Add("Draw-Q", new CheckBox("❐   " + NomeHeroi + " [Q] - 范围", true));
            Menu.Add("Draw-E", new CheckBox("❐   " + NomeHeroi + " [E] - 范围", true));
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.AddLabel("〈〈〈〈 线圈 2 〉〉〉〉");
            Menu.Add("Draw-Text", new CheckBox("❐   " + NomeHeroi + " [HP] - 显示敌人", true));
            Menu.Add("Draw-JGHP", new CheckBox("❐   " + NomeHeroi + " [HP] - 显示野怪", true));
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.AddLabel("〈〈〈〈 炸桶线圈 〉〉〉〉");
            Menu.Add("Draw-Barrel", new CheckBox("❐   " + NomeHeroi +       " [炸桶] - 爆炸", true));
            Menu.Add("Draw-BarrelExtend", new CheckBox("❐   " + NomeHeroi + " [炸桶] - 延长", true));
            Menu.Add("Draw-Eexplosion", new CheckBox("❐   " + NomeHeroi +   " [炸桶] - 文字", true));
            Menu.AddLabel("______________________________________________________________________________________");
            Menu.AddLabel("〈〈〈〈 换肤 + 炸桶皮肤 〉〉〉〉");
            Menu.Add("UseSkinHack", new CheckBox("❐ " + NomeHeroi + "  开启换肤", true));
            Menu.Add("SkinLoad", new KeyBind("应用皮肤 / 炸桶皮肤", false, KeyBind.BindTypes.HoldActive, 'N'));
            var ID = new[] { "Classic Gangplank", "Spooky Gangplank", "Minuteman Gangplank", "Sailor Gangplank", "Toy Soldier Gangplank", "Special Forces Gangplank", "Sultan Gangplank", "Captain Gangplank" };
            var Skin = Menu.Add("SkinHack", new ComboBox(NomeHeroi, 6, ID));
            Skin.DisplayName = ID[Skin.CurrentValue];
            Skin.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs) { sender.DisplayName = ID[changeArgs.NewValue]; }; ;

            Menu.AddSeparator(2);
            Menu.AddGroupLabel("GangPlank 更新记录" + System.Environment.NewLine);
            Menu.AddSeparator(2);
            Menu.AddLabel("✖ Script: BanSharp To EloBuddy" + System.Environment.NewLine);
            Menu.AddLabel("✖ Crédit: Soresu /  UnrealSkill99  / CH");

            Menu.AddLabel("✔ Add: Auto KillSteal Jungle Using [Q]" + System.Environment.NewLine);
            Menu.AddLabel("✔ Add: Auto KS Using [Q] & [R]" + System.Environment.NewLine);
            Menu.AddLabel("✔ Add: [R] Team Figth 2 Ally VS 2 Enemy or More." + System.Environment.NewLine);
            Menu.AddLabel("✔ Add: HP Text Jungles" + System.Environment.NewLine);
            Menu.AddLabel("✔ Add: SkinHack + Object" + System.Environment.NewLine);
            Menu.AddLabel("✔ Add: Barrel Range Extended & Text Info" + System.Environment.NewLine);
            Menu.AddLabel("✔ Add: Auto Remove CC Using [W]" + System.Environment.NewLine);
            Menu.AddSeparator(2);
        }
    }
        internal class Barrel
    {
        public Obj_AI_Minion barrel;
        public float time;

        public Barrel(Obj_AI_Minion objAiBase, int tickCount)
        {
            barrel = objAiBase;
            time = tickCount;
        }
    }
}