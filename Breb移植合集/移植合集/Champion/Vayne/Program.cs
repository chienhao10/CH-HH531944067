using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Evade;
using SAutoCarry.Champions.Helpers;
using SharpDX;
using SOLOVayne;
using SOLOVayne.Utility.General;
using Color = System.Drawing.Color;
using LS = LeagueSharp.Common;
using SV = SoloVayne.Skills.Tumble;

namespace Vayne
{
    public static class Program
    {
        #region Init

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        #endregion

        #region ctor

        public static void OnLoad()
        {
            if (myHero.ChampionName != Champion.Vayne.ToString())
            {
                Chat.Print("[Berb] : You are not playing Vayne, this addon will not load.");
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 300, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Active(SpellSlot.R);
            E2 = new Spell.Skillshot(SpellSlot.E, 550, SkillShotType.Linear, (int) 0.42f, 1300, 50);

            var clean = Player.Spells.FirstOrDefault(o => o.SData.Name == "SummonerBoost");
            var h = Player.Spells.FirstOrDefault(o => o.SData.Name == "SummonerHeal");

            if (clean != null)
            {
                var cleanses = myHero.GetSpellSlotFromName("SummonerBoost");
                cleanse = new Spell.Active(cleanses);
            }

            if (h != null)
            {
                var he = myHero.GetSpellSlotFromName("SummonerHeal");
                heal = new Spell.Active(he);
            }

            InitMenu();
            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            CustomAntigapcloser.OnEnemyGapcloser += CustomAntigapcloser_OnEnemyGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        #endregion

        #region Instance Variables

        public static Spell.Skillshot Q, E2;
        public static Spell.Targeted E;
        public static Spell.Active W, R, cleanse, heal;

        #endregion

        #region Events

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (UseEAntiGapcloserBool && E.IsReady())
            {
                if (sender.IsEnemy && sender.Name == "Rengar_LeapSound.troy")
                {
                    var rengarEntity =
                        LS.HeroManager.Enemies.Find(h => h.ChampionName.Equals("Rengar") && h.IsValidTarget(E.Range));
                    if (rengarEntity != null)
                    {
                        myHero.Spellbook.CastSpell(SpellSlot.E, rengarEntity);
                    }
                }
            }
        }

        private static void CustomAntigapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (UseEAntiGapcloserBool && E.IsReady())
            {
                if (gapcloser.Sender.IsValidTarget(E.Range) &&
                    GPMenu[
                        string.Format("dz191.vhr.agplist.{0}.{1}", gapcloser.Sender.ChampionName.ToLowerInvariant(),
                            gapcloser.SpellName)].Cast<CheckBox>().CurrentValue)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.E, gapcloser.Sender);
                }
            }
        }

        private static void Clean()
        {
            if (Item.CanUseItem(ItemId.Quicksilver_Sash))
            {
                Core.DelayAction(delegate { Item.UseItem(ItemId.Quicksilver_Sash); }, CSSDelay);
            }
            if (Item.CanUseItem(ItemId.Mercurial_Scimitar))
            {
                Core.DelayAction(delegate { Item.UseItem(ItemId.Mercurial_Scimitar); }, CSSDelay);
            }
            if (Item.CanUseItem(ItemId.Dervish_Blade))
            {
                Core.DelayAction(delegate { Item.UseItem(ItemId.Dervish_Blade); }, CSSDelay);
            }
            if (cleanse != null)
            {
                if (cleanse.IsReady())
                {
                    Core.DelayAction(delegate { cleanse.Cast(); }, CSSDelay);
                }
            }
        }

        private static void Cleansers()
        {
            if (!Item.CanUseItem(ItemId.Quicksilver_Sash) && !Item.CanUseItem(ItemId.Mikaels_Crucible) &&
                !Item.CanUseItem(ItemId.Mercurial_Scimitar) && !Item.CanUseItem(ItemId.Dervish_Blade) &&
                (cleanse == null || !cleanse.IsReady()))
            {
                return;
            }

            if (myHero.HealthPercent >= cleanHP || !_Clean)
            {
                return;
            }

            if (CleanSpells)
            {
                if (myHero.HasBuff("zedrdeathmark") || myHero.HasBuff("FizzMarinerDoom") ||
                    myHero.HasBuff("MordekaiserChildrenOfTheGrave") || myHero.HasBuff("PoppyDiplomaticImmunity") ||
                    myHero.HasBuff("VladimirHemoplague") || myHero.HasBuff("zedulttargetmark") ||
                    myHero.HasBuff("AlZaharNetherGrasp"))
                {
                    Clean();
                }
            }


            if (Item.CanUseItem(ItemId.Mikaels_Crucible) && Item.HasItem(ItemId.Mikaels_Crucible))
            {
                foreach (
                    var ally in
                        EntityManager.Heroes.Allies.Where(
                            ally =>
                                ally.IsValid && !ally.IsDead &&
                                ItemMenu["MikaelsAlly" + ally.ChampionName].Cast<CheckBox>().CurrentValue &&
                                myHero.Distance(ally.Position) < 750 && ally.HealthPercent < (float) cleanHP))
                {
                    if (CleanSpells && ally.HasBuff("zedrdeathmark") || ally.HasBuff("FizzMarinerDoom") ||
                        ally.HasBuff("MordekaiserChildrenOfTheGrave") || ally.HasBuff("PoppyDiplomaticImmunity") ||
                        ally.HasBuff("VladimirHemoplague"))
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Stun) && Stun)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Snare) && Snare)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Charm) && Charm)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Fear) && Fear)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Stun) && Stun)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Taunt) && Taunt)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Suppression) && Suppression)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                    if (ally.HasBuffOfType(BuffType.Blind) && Blind)
                        Item.UseItem(ItemId.Mikaels_Crucible, ally);
                }
            }

            if (myHero.HasBuffOfType(BuffType.Stun) && Stun)
            {
                Clean();
            }
            if (myHero.HasBuffOfType(BuffType.Snare) && Snare)
            {
                Clean();
            }
            if (myHero.HasBuffOfType(BuffType.Charm) && Charm)
            {
                Clean();
            }
            if (myHero.HasBuffOfType(BuffType.Fear) && Fear)
            {
                Clean();
            }
            if ((myHero.HasBuffOfType(BuffType.Fear) ||
                 (myHero.HasBuffOfType(BuffType.Flee) && myHero.HasBuff("nocturefleeslow"))) && Fear)
            {
                Clean();
            }
            if (myHero.HasBuffOfType(BuffType.Stun) && Stun)
            {
                Clean();
            }
            if (myHero.HasBuffOfType(BuffType.Taunt) && Taunt)
            {
                Clean();
            }
            if (myHero.HasBuffOfType(BuffType.Suppression) && Suppression)
            {
                Clean();
            }
            if (myHero.HasBuffOfType(BuffType.Blind) && Blind)
            {
                Clean();
            }
        }

        public static void OnUpdate(EventArgs args)
        {
            if (_autoLevel)
            {
                AutoLevel._AutoSpell();
            }

            if (UseEAntiGapcloserBool && E.IsReady())
            {
                var getTrist =
                    EntityManager.Heroes.Enemies.Where(
                        x =>
                            x.ChampionName == Champion.Tristana.ToString() && x.HasBuff("TristanaW") &&
                            x.IsValidTarget(E.Range));
                if (getTrist.Any() &&
                    GPMenu[string.Format("dz191.vhr.agplist.{0}.{1}", "tristana", "rocketjump")].Cast<CheckBox>()
                        .CurrentValue)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.E, getTrist.FirstOrDefault());
                }
            }

            if (UseEAntiGapcloserBool && E.IsReady())
            {
                var getAli =
                    EntityManager.Heroes.Enemies.Where(
                        x =>
                            x.ChampionName == Champion.Alistar.ToString() && x.HasBuff("AlistarTrample") &&
                            x.IsValidTarget(E.Range));
                if (getAli.Any() &&
                    GPMenu[string.Format("dz191.vhr.agplist.{0}.{1}", "Alistar", "headbutt")].Cast<CheckBox>()
                        .CurrentValue)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.E, getAli.FirstOrDefault());
                }
            }

            if (lowlifepeel && E.IsReady() && !Q.IsReady() && (ObjectManager.Player.HealthPercent <= 25))
            {
                var meleeEnemies =
                    EntityManager.Heroes.Enemies.Where(m => m.Distance(ObjectManager.Player, true) <= 400 && m.IsMelee);

                if (meleeEnemies.Any())
                {
                    var mostDangerous =
                        meleeEnemies.OrderByDescending(m => m.GetAutoAttackDamage(ObjectManager.Player)).First();
                    myHero.Spellbook.CastSpell(SpellSlot.E, mostDangerous);
                }
            }

            if (useHeal && heal != null)
            {
                if (heal.IsReady())
                {
                    if (myHero.HealthPercent <= lowerHeal &&
                        (myHero.CountEnemiesInRange(325) >= 1 || myHero.HasBuff("summonerdot")))
                    {
                        heal.Cast();
                    }
                }
            }

            if (_Clean)
            {
                Cleansers();
            }

            if (Item.CanUseItem(ItemId.Blade_of_the_Ruined_King) && useBotrk)
            {
                var t = TargetSelector.GetTarget(550, DamageType.Physical);
                if (t.IsValidTarget() && t != null)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        Item.UseItem(ItemId.Blade_of_the_Ruined_King, t);
                    }
                }
            }

            if (Item.CanUseItem(ItemId.Bilgewater_Cutlass) && useCutlass)
            {
                var t = TargetSelector.GetTarget(550, DamageType.Magical);
                if (t.IsValidTarget() && t != null)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        Item.UseItem(ItemId.Bilgewater_Cutlass, t);
                    }
                }
            }

            if (Item.CanUseItem(ItemId.Youmuus_Ghostblade) && useGhostBlade)
            {
                var t = TargetSelector.GetTarget(750, DamageType.Magical);

                if (t.IsValidTarget() && t is AIHeroClient)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        Item.UseItem(ItemId.Youmuus_Ghostblade);
                    }
                }
            }

            if (myHero.CountEnemiesInRange(550 + 200) >= GetAutoR &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && UseRBool)
            {
                R.Cast();
            }

            if (UseEBool == 1)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(550)))
                {
                    if (IsCondemnable(enemy))
                    {
                        myHero.Spellbook.CastSpell(SpellSlot.E, enemy);
                    }
                }
            }

            if (UseEBool == 2 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(550)))
                {
                    if (IsCondemnable(enemy))
                    {
                        myHero.Spellbook.CastSpell(SpellSlot.E, enemy);
                    }
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (UseEInterruptBool)
            {
                if (e.DangerLevel == DangerLevel.High && sender.IsValidTarget(E.Range))
                {
                    myHero.Spellbook.CastSpell(SpellSlot.E, sender);
                }
            }
        }

        /// <summary>
        ///     Returns the spell slot with the name.
        /// </summary>
        public static SpellSlot GetSpellSlot(this AIHeroClient unit, string name)
        {
            foreach (
                var spell in
                    unit.Spellbook.Spells.Where(
                        spell => string.Equals(spell.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return spell.Slot;
            }

            return SpellSlot.Unknown;
        }

        public static void OnProcessSpellCast(GameObject sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient)
            {
                var s2 = (AIHeroClient) sender;
                if (s2.IsValidTarget() && s2.ChampionName == "Pantheon" &&
                    s2.GetSpellSlot(args.SData.Name) == SpellSlot.W)
                {
                    if (UseEAntiGapcloserBool && args.Target.IsMe && E.IsReady())
                    {
                        if (s2.IsValidTarget(E.Range))
                        {
                            myHero.Spellbook.CastSpell(SpellSlot.E, s2);
                        }
                    }
                }
            }

            if (QModeStringList == 4)
            {
                if (sender != null)
                {
                    if (args.Target != null)
                    {
                        if (args.Target.IsMe)
                        {
                            if (sender.Type == GameObjectType.AIHeroClient)
                            {
                                if (sender.IsEnemy)
                                {
                                    if (sender.Distance(myHero) < 190)
                                    {
                                        if (antiMelee)
                                        {
                                            if (Q.IsReady())
                                            {
                                                myHero.Spellbook.CastSpell(SpellSlot.Q,
                                                    (Vector3)
                                                        ObjectManager.Player.Position.Extend(sender.Position, -Q.Range));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (sender is AIHeroClient && sender.IsEnemy)
            {
                if (args.SData.Name == "summonerflash" && args.End.Distance(myHero.ServerPosition) < 350)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.E, (AIHeroClient) sender);
                }

                var sdata = EvadeSpellDatabase.GetByName(args.SData.Name);

                if (sdata != null)
                {
                    if (UseEAntiGapcloserBool &&
                        (myHero.Distance(args.Start.Extend(args.End, sdata.MaxRange)) < 350 || args.Target.IsMe) &&
                        sdata.IsBlink || sdata.IsDash)
                    {
                        if (E.IsReady())
                        {
                            myHero.Spellbook.CastSpell(SpellSlot.E, (AIHeroClient) sender);
                        }
                        if (Q.IsReady())
                        {
                            switch (UseQAntiGapcloserStringList)
                            {
                                case 3:
                                {
                                    if (args.End.Distance(myHero.ServerPosition) < 350)
                                    {
                                        var pos = myHero.ServerPosition.Extend(args.End, -300).To3D();
                                        if (!IsDangerousPosition(pos))
                                        {
                                            myHero.Spellbook.CastSpell(SpellSlot.Q, pos);
                                        }
                                    }
                                    if (sender.Distance(myHero) < 350)
                                    {
                                        var pos = myHero.ServerPosition.Extend(sender.Position, -300).To3D();
                                        if (!IsDangerousPosition(pos))
                                        {
                                            myHero.Spellbook.CastSpell(SpellSlot.Q, pos);
                                        }
                                    }
                                    break;
                                }
                                case 2:
                                {
                                    if (!E.IsReady())
                                    {
                                        if (args.End.Distance(myHero.ServerPosition) < 350)
                                        {
                                            var pos = myHero.ServerPosition.Extend(args.End, -300).To3D();
                                            if (!IsDangerousPosition(pos))
                                            {
                                                myHero.Spellbook.CastSpell(SpellSlot.Q, pos);
                                            }
                                        }
                                        if (sender.Distance(myHero) < 350)
                                        {
                                            var pos = myHero.ServerPosition.Extend(sender.Position, -300).To3D();
                                            if (!IsDangerousPosition(pos))
                                            {
                                                myHero.Spellbook.CastSpell(SpellSlot.Q, pos);
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var x = myHero.HPBarPosition.X;
            var y = myHero.HPBarPosition.Y + 200;

            if (menuKey)
            {
                Drawing.DrawText(x, y, UseRBool ? Color.DeepSkyBlue : Color.DarkGray, "Use R Auto? : " + UseRBool);
                Drawing.DrawText(x, y + 25, dontaa ? Color.DeepSkyBlue : Color.DarkGray, "Don't AA in R? : " + dontaa);
            }

            if (drawCurrentLogic)
            {
                if (QModeStringList == 1)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : Prada");
                }
                else if (QModeStringList == 2)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : Marksman");
                }
                else if (QModeStringList == 3)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : VHR/SOLO");
                }
                else if (QModeStringList == 4)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : Sharpshooter");
                }
                else if (QModeStringList == 5)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : Synx Auto Carry");
                }
                else if (QModeStringList == 6)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : To Cursor");
                }
                else if (QModeStringList == 7)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : Kite Melee");
                }
                else if (QModeStringList == 8)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : Kurisu");
                }
                else if (QModeStringList == 9)
                {
                    Drawing.DrawText(x, y + 50, Color.Red, "Current Q Logic : Safe Position");
                }

                if (EModeStringList == 1)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Prada Smart");
                }
                else if (EModeStringList == 2)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Prada Perfect");
                }
                else if (EModeStringList == 3)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Marksman");
                }
                else if (EModeStringList == 4)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Sharpshooter");
                }
                else if (EModeStringList == 5)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Gosu");
                }
                else if (EModeStringList == 6)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : VHR");
                }
                else if (EModeStringList == 7)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Prada Legacy");
                }
                else if (EModeStringList == 8)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Fastest");
                }
                else if (EModeStringList == 9)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Old Prada");
                }
                else if (EModeStringList == 10)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Synx Auto Carry");
                }
                else if (EModeStringList == 11)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : OKTW");
                }
                else if (EModeStringList == 12)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Shine - Hikicarry");
                }
                else if (EModeStringList == 13)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : Asuna - Hikicarry");
                }
                else if (EModeStringList == 14)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : 360 - Hikicarry");
                }
                else if (EModeStringList == 15)
                {
                    Drawing.DrawText(x, y + 75, Color.Red, "Current E Logic : SergixCondemn");
                }
            }

            if (DrawWStacksBool)
            {
                var target =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        enemy => enemy.HasBuff("vaynesilvereddebuff") && enemy.IsValidTarget(2000));
                if (target.IsValidTarget())
                {
                    var xa = target.HPBarPosition.X + 50;
                    var ya = target.HPBarPosition.Y - 20;

                    if (W.Level > 0)
                    {
                        var stacks = target.GetBuffCount("vaynesilvereddebuff");
                        if (stacks > -1)
                        {
                            for (var i = 0; i < 3; i++)
                            {
                                Drawing.DrawLine(xa + i*20, ya, xa + i*20 + 10, ya, 10,
                                    stacks <= i ? Color.DarkGray : Color.DeepSkyBlue);
                            }
                        }
                    }
                }
            }
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var possible2WTarget =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    h =>
                        h.ServerPosition.Distance(myHero.ServerPosition) < 500 &&
                        h.GetBuffCount("vaynesilvereddebuff") == 2);
            if (TryToFocus2WBool && possible2WTarget.IsValidTarget())
            {
                Orbwalker.ForcedTarget = possible2WTarget;
            }

            if (myHero.HasBuff("vaynetumblefade"))
            {
                if (dontaa)
                {
                    if (dontaaslider)
                    {
                        if (myHero.CountEnemiesInRange(750) >= dontaaenemy)
                        {
                            args.Process = false;
                        }
                    }
                    else
                    {
                        args.Process = false;
                    }
                }

                if (
                    EntityManager.Heroes.Enemies.Any(
                        e => e.ServerPosition.Distance(myHero.ServerPosition) < 350 && e.IsMelee) &&
                    DontAttackWhileInvisibleAndMeelesNearBool)
                {
                    args.Process = false;
                }
            }

            if (myHero.HasBuff("vaynetumblebonus") && args.Target is Obj_AI_Minion && UseQBonusOnEnemiesNotCS)
            {
                var possibleTarget = TargetSelector.GetTarget(-1f, DamageType.Physical);
                if (possibleTarget != null && possibleTarget.IsInAutoAttackRange(myHero))
                {
                    Orbwalker.ForcedTarget = possibleTarget;
                    args.Process = false;
                }
            }
            var possibleNearbyMeleeChampion =
                EntityManager.Heroes.Enemies.FirstOrDefault(e => e.ServerPosition.Distance(myHero.ServerPosition) < 350);

            if (possibleNearbyMeleeChampion.IsValidTarget())
            {
                if (Q.IsReady() && UseQBool)
                {
                    var pos = myHero.ServerPosition.Extend(possibleNearbyMeleeChampion.ServerPosition, -350).To3D();
                    if (!IsDangerousPosition(pos))
                    {
                        myHero.Spellbook.CastSpell(SpellSlot.Q, pos);
                    }
                    args.Process = false;
                }
            }
        }

        private static readonly string[] MobNames =
        {
            "SRU_Red", "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak",
            "SRU_Krug", "Sru_Crab", "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith"
        };

        public static bool IsPositionSafe(Vector2 position)
        {
            var myPos = ObjectManager.Player.Position.To2D();
            var newPos = position - myPos;
            newPos.Normalize();

            var checkPos = position + newPos*(Q.Range - Vector2.Distance(position, myPos));
            var enemy = EntityManager.Heroes.Enemies.Find(e => e.Distance(checkPos) < 350);
            return enemy == null;
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
            var possible2WTarget =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    h =>
                        h.ServerPosition.Distance(myHero.ServerPosition) < 500 &&
                        h.GetBuffCount("vaynesilvereddebuff") == 2);
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (possible2WTarget.IsValidTarget() && UseEAs3rdWProcBool &&
                    possible2WTarget.Path.LastOrDefault().Distance(myHero.ServerPosition) < 1000)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.E, possible2WTarget);
                }
            }
            if (target is AIHeroClient && UseQBool)
            {
                if (Q.IsReady())
                {
                    var tg = target as AIHeroClient;
                    if (tg != null)
                    {
                        var mode = QModeStringList;
                        var tumblePosition = Game.CursorPos;
                        switch (mode)
                        {
                            case 1: // Prada
                                tumblePosition = GetTumblePos(tg);
                                break;
                            case 2: // Marksman
                                if (tg.Distance(ObjectManager.Player.Position) > myHero.GetAutoAttackRange() &&
                                    IsPositionSafe(tg.Position.To2D()))
                                {
                                    tumblePosition = tg.Position;
                                }
                                else if (IsPositionSafe(Game.CursorPos.To2D()))
                                {
                                    tumblePosition = Game.CursorPos;
                                }
                                Orbwalker.ForcedTarget = tg;
                                break;
                            case 3: // VHR
                                if (smartq)
                                {
                                    var position = SV.TumbleLogicProvider.GetSOLOVayneQPosition();
                                    if (position != Vector3.Zero)
                                    {
                                        CastTumble(position, tg);
                                    }
                                }
                                else
                                {
                                    var position =
                                        ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f).To3D();
                                    if (position.IsSafe())
                                    {
                                        CastTumble(position, tg);
                                    }
                                }
                                break;
                            case 4: // sharpshooter
                                if (target.Type == GameObjectType.AIHeroClient)
                                {
                                    if (UseQBool)
                                    {
                                        if (Q.IsReady())
                                        {
                                            if (
                                                ObjectManager.Player.Position.Extend(Game.CursorPos, 700)
                                                    .CountEnemiesInRange(700) <= 1)
                                            {
                                                myHero.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                                            }
                                        }
                                    }
                                }
                                break;
                            case 5: // Synx Auto Carry
                                if (target is AIHeroClient)
                                {
                                    if (Q.IsReady())
                                    {
                                        var pos = Tumble.FindTumblePosition(target as AIHeroClient);

                                        if (pos.IsValid())
                                        {
                                            tumblePosition = pos;
                                        }
                                    }
                                }
                                break;
                            case 6: // Cursor
                                var smartQPosition = GetSmartQPosition();
                                var smartQCheck = smartQPosition != Vector3.Zero;
                                var QPosition = smartQCheck ? smartQPosition : Game.CursorPos;
                                var afterTumblePosition = ObjectManager.Player.ServerPosition.Extend(QPosition, 300f);
                                var distanceToTarget = afterTumblePosition.Distance(target.Position, true);
                                if ((distanceToTarget < Math.Pow(ObjectManager.Player.AttackRange + 65, 2) &&
                                     distanceToTarget > 110*110) || Cqspam)
                                {
                                    DefaultQCast(QPosition, tg);
                                }
                                break;
                            case 7: // Kite Melee
                                var _smartQPosition = GetSmartQPosition();
                                var _smartQCheck = _smartQPosition != Vector3.Zero;
                                var _QPosition = _smartQCheck ? _smartQPosition : Game.CursorPos;
                                var _afterTumblePosition = ObjectManager.Player.ServerPosition.Extend(_QPosition, 300f);
                                var _distanceToTarget = _afterTumblePosition.Distance(target.Position, true);
                                if ((_distanceToTarget < Math.Pow(ObjectManager.Player.AttackRange + 65, 2) &&
                                     _distanceToTarget > 110*110) || Cqspam)
                                {
                                    if (MeleeEnemiesTowardsMe.Any() &&
                                        !MeleeEnemiesTowardsMe.All(m => m.HealthPercent <= 15))
                                    {
                                        var Closest =
                                            MeleeEnemiesTowardsMe.OrderBy(m => m.Distance(ObjectManager.Player)).First();
                                        var whereToQ =
                                            Closest.ServerPosition.Extend(ObjectManager.Player.ServerPosition,
                                                Closest.Distance(ObjectManager.Player) + 300f).To3D();
                                        if (whereToQ.IsSafe())
                                        {
                                            CastQ(whereToQ);
                                        }
                                    }
                                    else
                                    {
                                        DefaultQCast(_QPosition, tg);
                                    }
                                }
                                break;
                            case 8: // Kurisu
                                var __smartQPosition = GetSmartQPosition();
                                var __smartQCheck = __smartQPosition != Vector3.Zero;
                                var __QPosition = __smartQCheck ? __smartQPosition : Game.CursorPos;
                                var __afterTumblePosition = ObjectManager.Player.ServerPosition.Extend(__QPosition, 300f);
                                var __distanceToTarget = __afterTumblePosition.Distance(target.Position, true);
                                if ((__distanceToTarget < Math.Pow(ObjectManager.Player.AttackRange + 65, 2) &&
                                     __distanceToTarget > 110*110) || Cqspam)
                                {
                                    var range = tg.GetAutoAttackRange();
                                    var path = CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                                        Prediction.Position.PredictUnitPosition(tg, (int) 0.25), 300f, range);

                                    if (path.Count() > 0)
                                    {
                                        var TumblePosition = path.MinOrDefault(x => x.Distance(Game.CursorPos)).To3D();
                                        if (!TumblePosition.IsSafe(true))
                                        {
                                            CastQ(TumblePosition);
                                        }
                                    }
                                    else
                                    {
                                        DefaultQCast(__QPosition, tg);
                                    }
                                }
                                break;
                            case 9:
                                SafePositionQ(tg);
                                break;
                            default:
                                tumblePosition = Game.CursorPos;
                                break;
                        }
                        if (tumblePosition.Distance(myHero.Position) > 2000 || IsDangerousPosition(tumblePosition))
                        {
                            if (mode != 3 || mode != 6 || mode != 7 || mode != 8 || mode != 9)
                            {
                                return;
                            }
                        }
                        if (mode != 3 || mode != 6 || mode != 7 || mode != 8 || mode != 4 || mode != 9)
                        {
                            myHero.Spellbook.CastSpell(SpellSlot.Q, tumblePosition);
                        }
                    }
                }
            }
            if (target is Obj_AI_Minion && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var tg = target as Obj_AI_Minion;
                if (E.IsReady())
                {
                    if (MobNames.Contains(tg.CharData.BaseSkinName) && tg.IsValidTarget() && UseEJungleFarm)
                    {
                        myHero.Spellbook.CastSpell(SpellSlot.E, tg);
                    }
                }
                if (Q.IsReady())
                {
                    if (tg.Name.Contains("SRU_") && !IsDangerousPosition(Game.CursorPos) && useQJG)
                    {
                        myHero.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
                    if (useQLane &&
                        EntityManager.MinionsAndMonsters.EnemyMinions.Count(
                            m =>
                                m.Position.Distance(myHero.Position) < 550 &&
                                m.Health < myHero.GetAutoAttackDamage(m) + myHero.GetSpellDamage(m, SpellSlot.Q)) > 1 &&
                        !IsDangerousPosition(Game.CursorPos))
                    {
                        myHero.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
                    if (useQLane && UnderAllyTurret(myHero.Position))
                    {
                        if (
                            EntityManager.MinionsAndMonsters.EnemyMinions.Count(
                                m =>
                                    m.Position.Distance(myHero.Position) < 550 &&
                                    m.Health < myHero.GetAutoAttackDamage(m) + myHero.GetSpellDamage(m, SpellSlot.Q)) >
                            0 && !IsDangerousPosition(Game.CursorPos))
                        {
                            myHero.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                        }
                    }
                }
            }
            if (UseQOnlyAt2WStacksBool && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                possible2WTarget.IsValidTarget())
            {
                myHero.Spellbook.CastSpell(SpellSlot.Q, GetTumblePos(possible2WTarget));
            }
        }

        public static void SafePositionQ(AIHeroClient enemy)
        {
            var range = LS.Orbwalking.GetRealAutoAttackRange(enemy);
            var path = LS.Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                LS.Prediction.GetPrediction(enemy, 0.25f).UnitPosition.To2D(), Q.Range, range);

            if (path.Count() > 0)
            {
                var epos = path.MinOrDefault(x => x.Distance(Game.CursorPos));
                if (epos.To3D().UnderTurret(true) || epos.To3D().IsWall())
                {
                    return;
                }

                if (epos.To3D().CountEnemiesInRange(Q.Range - 100) > 0)
                {
                    return;
                }
                myHero.Spellbook.CastSpell(SpellSlot.Q, epos.To3D());
            }
            if (path.Count() == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Q.Range).To3D();
                if (epos.UnderTurret(true) || epos.IsWall())
                {
                    return;
                }

                // no intersection or target to close
                myHero.Spellbook.CastSpell(SpellSlot.Q,
                    ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Q.Range).To3D());
            }
        }

        private static void CastTumble(Vector3 Position, Obj_AI_Base target)
        {
            var WallQPosition = SV.TumbleHelper.GetQBurstModePosition();
            if (WallQPosition != null && ObjectManager.Player.ServerPosition.IsSafeEx() &&
                !ObjectManager.Player.ServerPosition.UnderTurret(true))
            {
                var V3WallQ = (Vector3) WallQPosition;
                CastQ(V3WallQ);
                return;
            }

            var TumbleQEPosition = SV.TumbleLogicProvider.GetQEPosition();
            if (TumbleQEPosition != Vector3.Zero)
            {
                CastQ(TumbleQEPosition);
                return;
            }

            Orbwalker.ForcedTarget = target;

            if (ObjectManager.Player.CountEnemiesInRange(1500f) >= 3)
            {
            }

            CastQ(Position);
        }

        public static T MinOrDefault<T, TR>(this IEnumerable<T> container, Func<T, TR> valuingFoo)
            where TR : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var minElem = enumerator.Current;
            var minVal = valuingFoo(minElem);

            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);

                if (currVal.CompareTo(minVal) < 0)
                {
                    minVal = currVal;
                    minElem = enumerator.Current;
                }
            }

            return minElem;
        }

        public static Vector2[] CircleCircleIntersection(Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var D = center1.Distance(center2);
            //The Circles dont intersect:
            if (D > radius1 + radius2 || (D <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] {};
            }

            var A = (radius1*radius1 - radius2*radius2 + D*D)/(2*D);
            var H = (float) Math.Sqrt(radius1*radius1 - A*A);
            var Direction = (center2 - center1).Normalized();
            var PA = center1 + A*Direction;
            var S1 = PA + H*Direction.Perpendicular();
            var S2 = PA - H*Direction.Perpendicular();
            return new[] {S1, S2};
        }

        public static float GetRealAutoAttackRange(AIHeroClient attacker, AttackableUnit target)
        {
            var result = attacker.AttackRange + attacker.BoundingRadius;
            if (target.IsValidTarget())
            {
                return result + target.BoundingRadius;
            }
            return result;
        }

        private const float Range = 1200f;

        public static IEnumerable<AIHeroClient> MeleeEnemiesTowardsMe
        {
            get
            {
                return
                    EntityManager.Heroes.Enemies.FindAll(
                        m =>
                            m.IsMelee &&
                            m.Distance(ObjectManager.Player) <= GetRealAutoAttackRange(m, ObjectManager.Player) &&
                            (m.ServerPosition.To2D() + (m.BoundingRadius + 25f)*m.Direction.To2D().Perpendicular())
                                .Distance(ObjectManager.Player.ServerPosition.To2D()) <=
                            m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) &&
                            m.IsValidTarget(Range));
            }
        }

        public static Vector3 GetSmartQPosition()
        {
            if (!Csmartq || !E.IsReady())
            {
                return Vector3.Zero;
            }

            const int currentStep = 30;
            var direction = ObjectManager.Player.Direction.To2D().Perpendicular();
            for (var i = 0f; i < 360f; i += currentStep)
            {
                var angleRad = DegreeToRadian(i);
                var rotatedPosition = (ObjectManager.Player.Position.To2D() + 300f*direction.Rotated(angleRad)).To3D();
                if (GetTarget(rotatedPosition).IsValidTarget() && rotatedPosition.IsSafe())
                {
                    return rotatedPosition;
                }
            }

            return Vector3.Zero;
        }

        public static Vector3 GetAfterTumblePosition(Vector3 endPosition)
        {
            return ObjectManager.Player.ServerPosition.Extend(endPosition, 300f).To3D();
        }

        public static List<AIHeroClient> GetLhEnemiesNear(this Vector3 position, float range, float healthpercent)
        {
            return
                EntityManager.Heroes.Enemies.Where(
                    hero => hero.IsValidTarget(range, true, position) && hero.HealthPercent <= healthpercent).ToList();
        }

        public static bool UnderAllyTurret_Ex(this Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(t => t.IsAlly && !t.IsDead);
        }

        public static bool IsSafe(this Vector3 position, bool noQIntoEnemiesCheck = false)
        {
            if (position.UnderTurret(true) && !ObjectManager.Player.UnderTurret(true))
            {
                return false;
            }

            var allies = position.CountAlliesInRange(ObjectManager.Player.AttackRange);
            var enemies = position.CountEnemiesInRange(ObjectManager.Player.AttackRange);
            var lhEnemies = position.GetLhEnemiesNear(ObjectManager.Player.AttackRange, 15).Count();

            if (enemies <= 1) ////It's a 1v1, safe to assume I can Q
            {
                return true;
            }

            if (position.UnderAllyTurret_Ex())
            {
                var nearestAllyTurret =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(a => a.IsAlly)
                        .OrderBy(d => d.Distance(position, true))
                        .FirstOrDefault();

                if (nearestAllyTurret != null)
                {
                    allies += 2;
                }
            }

            var normalCheck = allies + 1 > enemies - lhEnemies;
            var QEnemiesCheck = true;

            if (Cnoqenemies && noQIntoEnemiesCheck)
            {
                if (!Cnoqenemiesold)
                {
                    var Vector2Position = position.To2D();
                    var enemyPoints = Cdynamicqsafety ? GetEnemyPoints() : GetEnemyPoints(false);
                    if (enemyPoints.Contains(Vector2Position) && !Cqspam)
                    {
                        QEnemiesCheck = false;
                    }

                    var closeEnemies =
                        EntityManager.Heroes.Enemies.FindAll(
                            en =>
                                en.IsValidTarget(1500f) &&
                                !(en.Distance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f))
                            .OrderBy(en => en.Distance(position));

                    if (
                        !closeEnemies.All(
                            enemy => position.CountEnemiesInRange(Cdynamicqsafety ? enemy.AttackRange : 405f) <= 1))
                    {
                        QEnemiesCheck = false;
                    }
                }
                else
                {
                    var closeEnemies =
                        EntityManager.Heroes.Enemies.FindAll(en => en.IsValidTarget(1500f))
                            .OrderBy(en => en.Distance(position));
                    if (closeEnemies.Any())
                    {
                        QEnemiesCheck =
                            !closeEnemies.All(
                                enemy => position.CountEnemiesInRange(Cdynamicqsafety ? enemy.AttackRange : 405f) <= 1);
                    }
                }
            }

            return normalCheck && QEnemiesCheck;
        }

        private static void DefaultQCast(Vector3 position, AIHeroClient Target)
        {
            var afterTumblePosition = GetAfterTumblePosition(Game.CursorPos);
            var EnemyPoints = GetEnemyPoints();
            if (afterTumblePosition.IsSafe(true) || !EnemyPoints.Contains(Game.CursorPos.To2D()) ||
                (EnemiesClose.Count() == 1))
            {
                if (afterTumblePosition.Distance(Target.ServerPosition) <= Target.GetAutoAttackRange())
                {
                    CastQ(position);
                }
            }
        }

        private static void CastQ(Vector3 Position)
        {
            myHero.Spellbook.CastSpell(SpellSlot.Q, Position);
        }

        internal static AIHeroClient GetTarget(Vector3 position = default(Vector3))
        {
            var HeroList =
                EntityManager.Heroes.Enemies.Where(
                    h =>
                        h.IsValidTarget(E.Range) && !h.HasBuffOfType(BuffType.SpellShield) &&
                        !h.HasBuffOfType(BuffType.SpellImmunity));

            var Accuracy = 38;
            var PushDistance = 425;

            if (ObjectManager.Player.ServerPosition.UnderTurret(true))
            {
                return null;
            }

            if (EntityManager.Heroes.Allies.Count(ally => !ally.IsMe && ally.IsValidTarget(1500f)) == 0 &&
                ObjectManager.Player.CountEnemiesInRange(1500f) == 1)
            {
                //It's a 1v1 situation. We push condemn to the limit and lower the accuracy by 5%.
                Accuracy = 33;
                PushDistance = 460;
            }

            var startPosition = position != default(Vector3) ? position : ObjectManager.Player.ServerPosition;

            foreach (var Hero in HeroList)
            {
                if (Hero.Health + 10 <= ObjectManager.Player.GetAutoAttackDamage(Hero)*2)
                {
                    continue;
                }
                var prediction = E2.GetPrediction(Hero);
                var targetPosition = prediction.UnitPosition;
                var finalPosition = targetPosition.Extend(startPosition, -PushDistance);
                var finalPosition_ex = Hero.ServerPosition.Extend(startPosition, -PushDistance);
                var finalPosition_3 = prediction.CastPosition.Extend(startPosition, -PushDistance);

                //Yasuo Wall Logic
                if (YasuoWall.CollidesWithWall(startPosition, (Vector3) Hero.ServerPosition.Extend(startPosition, -450f)))
                {
                    continue;
                }

                //Condemn to turret logic
                if (
                    EntityManager.Turrets.Allies.Any(
                        m => m.IsValidTarget(float.MaxValue) && m.Distance(finalPosition) <= 450f))
                {
                    var turret =
                        EntityManager.Turrets.Allies.FirstOrDefault(
                            m => m.IsValidTarget(float.MaxValue) && m.Distance(finalPosition) <= 450f);
                    if (turret != null)
                    {
                        var enemies =
                            EntityManager.Heroes.Enemies.Where(m => m.Distance(turret) < 775f && m.IsValidTarget());

                        if (!enemies.Any())
                        {
                            return Hero;
                        }
                    }
                }

                //Condemn To Wall Logic
                var condemnRectangle =
                    new SOLOPolygon(SOLOPolygon.Rectangle(targetPosition.To2D(), finalPosition, Hero.BoundingRadius));
                var condemnRectangle_ex =
                    new SOLOPolygon(SOLOPolygon.Rectangle(Hero.ServerPosition.To2D(), finalPosition_ex,
                        Hero.BoundingRadius));
                var condemnRectangle_3 =
                    new SOLOPolygon(SOLOPolygon.Rectangle(prediction.CastPosition.To2D(), finalPosition_3,
                        Hero.BoundingRadius));

                if (IsBothNearWall(Hero))
                {
                    return null;
                }

                if (
                    condemnRectangle.Points.Count(
                        point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >=
                    condemnRectangle.Points.Count()*(Accuracy/100f)
                    ||
                    condemnRectangle_ex.Points.Count(
                        point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >=
                    condemnRectangle_ex.Points.Count()*(Accuracy/100f)
                    ||
                    condemnRectangle_3.Points.Count(
                        point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >=
                    condemnRectangle_ex.Points.Count()*(Accuracy/100f))
                {
                    return Hero;
                }
            }
            return null;
        }

        public static bool IsNotIntoEnemies(this Vector3 position)
        {
            if (!smartq && !noqenemies)
            {
                return true;
            }

            var enemyPoints = GetEnemyPoints();
            if (enemyPoints.ToList().Contains(position.To2D()) &&
                !enemyPoints.Contains(ObjectManager.Player.ServerPosition.To2D()))
            {
                return false;
            }

            var closeEnemies =
                EntityManager.Heroes.Enemies.FindAll(
                    en =>
                        en.IsValidTarget(1500f) &&
                        !(en.Distance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f));
            if (
                closeEnemies.All(
                    enemy => position.CountEnemiesInRange(enemy.AttackRange > 350 ? enemy.AttackRange : 400) == 0))
            {
                return true;
            }

            return false;
        }

        public static bool IsSafeEx(this Vector3 Position)
        {
            if (Position.UnderTurret(true) && !ObjectManager.Player.UnderTurret())
            {
                return false;
            }
            var range = 1000f;
            var lowHealthAllies =
                EntityManager.Heroes.Allies.Where(a => a.IsValidTarget(range) && a.HealthPercent < 10 && !a.IsMe);
            var lowHealthEnemies = EntityManager.Heroes.Allies.Where(a => a.IsValidTarget(range) && a.HealthPercent < 10);
            var enemies = ObjectManager.Player.CountEnemiesInRange(range);
            var allies = ObjectManager.Player.CountAlliesInRange(range);
            var enemyTurrets = EntityManager.Turrets.Enemies.Where(m => m.IsValidTarget(975f));
            var allyTurrets = EntityManager.Turrets.Allies.Where(m => m.IsValidTarget(975f));

            return allies - lowHealthAllies.Count() + allyTurrets.Count()*2 + 1 >=
                   enemies - lowHealthEnemies.Count() +
                   (!ObjectManager.Player.UnderTurret(true) ? enemyTurrets.Count()*2 : 0);
        }

        public static float DegreeToRadian(double angle)
        {
            return (float) (Math.PI*angle/180.0);
        }

        private static Vector3[] GetWallQPositions(Obj_AI_Base player, float Range)
        {
            Vector3[] vList =
            {
                (player.ServerPosition.To2D() + Range*player.Direction.To2D()).To3D(),
                (player.ServerPosition.To2D() - Range*player.Direction.To2D()).To3D()
            };
            return vList;
        }

        private static bool IsBothNearWall(Obj_AI_Base target)
        {
            var positions =
                GetWallQPositions(target, 110).ToList().OrderBy(pos => pos.Distance(target.ServerPosition, true));
            var positions_ex =
                GetWallQPositions(ObjectManager.Player, 110)
                    .ToList()
                    .OrderBy(pos => pos.Distance(ObjectManager.Player.ServerPosition, true));

            if (positions.Any(p => p.IsWall()) && positions_ex.Any(p => p.IsWall()))
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    EntityManager.Heroes.Enemies.Where(
                        m =>
                            m.Distance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.IsValidTarget(1500) &&
                            m.CountEnemiesInRange(m.IsMelee ? m.AttackRange*1.5f : m.AttackRange + 20*1.5f) > 0);
            }
        }

        public static List<Vector2> GetEnemyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList =
                EnemiesClose.Select(
                    enemy =>
                        new SOLOGeometry.Circle(enemy.ServerPosition.To2D(),
                            (dynamic ? (enemy.IsMelee ? enemy.AttackRange*1.5f : enemy.AttackRange) : staticRange) +
                            enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = SOLOGeometry.ClipPolygons(polygonsList);
            var pointList =
                pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y))
                    .Where(currentPoint => !currentPoint.IsWall())
                    .ToList();
            return pointList;
        }

        public static bool IsSafe(this Vector3 position)
        {
            return position.IsSafeEx()
                   && position.IsNotIntoEnemies()
                   && EntityManager.Heroes.Enemies.All(m => m.Distance(position) > 350f)
                   &&
                   (!position.UnderTurret(true) ||
                    (ObjectManager.Player.UnderTurret(true) && position.UnderTurret(true) &&
                     ObjectManager.Player.HealthPercent > 10));
            //Either it is not under turret or both the player and the position are under turret already and the health percent is greater than 10.
        }

        public static bool UnderAllyTurret(Vector3 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(t => t.IsAlly && !t.IsDead && pos.Distance(t) <= 900);
        }

        #endregion

        #region Menu Items

        public static bool UseQBool
        {
            get { return ComboMenu["useq"].Cast<CheckBox>().CurrentValue; }
        }

        public static int QModeStringList
        {
            get { return QSettings["qmode"].Cast<Slider>().CurrentValue; }
        }

        public static int UseQAntiGapcloserStringList
        {
            get { return ComboMenu["qantigc"].Cast<Slider>().CurrentValue; }
        }

        public static bool TryToFocus2WBool
        {
            get { return ComboMenu["focus2w"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool DontAttackWhileInvisibleAndMeelesNearBool
        {
            get { return ComboMenu["dontattackwhileinvisible"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseRBool
        {
            get { return ComboMenu["user"].Cast<KeyBind>().CurrentValue; }
        }

        public static int UseEBool
        {
            get { return CondemnSettings["usee"].Cast<Slider>().CurrentValue; }
        }

        public static int EModeStringList
        {
            get { return CondemnSettings["emode"].Cast<Slider>().CurrentValue; }
        }

        public static bool UseEInterruptBool
        {
            get { return CondemnSettings["useeinterrupt"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEAntiGapcloserBool
        {
            get { return CondemnSettings["useeantigapcloser"].Cast<CheckBox>().CurrentValue; }
        }

        public static int EPushDistanceSlider
        {
            get { return CondemnSettings["epushdist"].Cast<Slider>().CurrentValue; }
        }

        public static int EHitchanceSlider
        {
            get { return CondemnSettings["ehitchance"].Cast<Slider>().CurrentValue; }
        }

        public static bool UseEAs3rdWProcBool
        {
            get { return ExtraMenu["usee3rdwproc"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseQBonusOnEnemiesNotCS
        {
            get { return ExtraMenu["useqonenemiesnotcs"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseQOnlyAt2WStacksBool
        {
            get { return ExtraMenu["useqonlyon2stackedenemies"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool useQLane
        {
            get { return FarmSettings["useQLane"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool useQJG
        {
            get { return FarmSettings["useQJG"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEJungleFarm
        {
            get { return FarmSettings["useejgfarm"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool DrawWStacksBool
        {
            get { return DrawingMenu["drawwstacks"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool menuKey
        {
            get { return DrawingMenu["menuKey"].Cast<CheckBox>().CurrentValue; }
        }

        public static int GetAutoR
        {
            get { return ComboMenu["GetAutoR"].Cast<Slider>().CurrentValue; }
        }

        public static bool noqenemies
        {
            get { return QSettings["noqenemies"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool antiMelee
        {
            get { return QSettings["antiMelee"].Cast<CheckBox>().CurrentValue; }
        }

        public static int Accuracy
        {
            get { return ESettings["Accuracy"].Cast<Slider>().CurrentValue; }
        }

        public static int TumbleCondemnCount
        {
            get { return ESettings["TumbleCondemnCount"].Cast<Slider>().CurrentValue; }
        }

        public static int sacMode
        {
            get { return QSettings["sacMode"].Cast<Slider>().CurrentValue; }
        }

        public static bool TumbleCondemn
        {
            get { return ESettings["TumbleCondemn"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool TumbleCondemnSafe
        {
            get { return ESettings["TumbleCondemnSafe"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool DontCondemnTurret
        {
            get { return ESettings["DontCondemnTurret"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool DontSafeCheck
        {
            get { return QSettings["DontSafeCheck"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool DontQIntoEnemies
        {
            get { return QSettings["DontQIntoEnemies"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Wall
        {
            get { return QSettings["Wall"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Only2W
        {
            get { return QSettings["Only2W"].Cast<CheckBox>().CurrentValue; }
        }

        public static int CSSDelay
        {
            get { return ItemMenu["CSSDelay"].Cast<Slider>().CurrentValue; }
        }

        public static int cleanHP
        {
            get { return ItemMenu["cleanHP"].Cast<Slider>().CurrentValue; }
        }

        public static bool CleanSpells
        {
            get { return ItemMenu["CleanSpells"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Stun
        {
            get { return ItemMenu["Stun"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Snare
        {
            get { return ItemMenu["Snare"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Charm
        {
            get { return ItemMenu["Charm"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Fear
        {
            get { return ItemMenu["Fear"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Suppression
        {
            get { return ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Taunt
        {
            get { return ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Blind
        {
            get { return ItemMenu["Blind"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool _Clean
        {
            get { return ItemMenu["Clean"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool useBotrk
        {
            get { return ItemMenu["useBotrk"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool useCutlass
        {
            get { return ItemMenu["useCutlass"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool useGhostBlade
        {
            get { return ItemMenu["useGhostBlade"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool smartq
        {
            get { return QSettings["smartq"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Cdynamicqsafety
        {
            get { return QSettings["Cdynamicqsafety"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Csmartq
        {
            get { return QSettings["Csmartq"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Cnoqenemies
        {
            get { return QSettings["Cnoqenemies"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Cnoqenemiesold
        {
            get { return QSettings["Cnoqenemiesold"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Cqspam
        {
            get { return QSettings["Cqspam"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool drawCurrentLogic
        {
            get { return DrawingMenu["drawCurrentLogic"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool dontaa
        {
            get { return ComboMenu["dontaa"].Cast<KeyBind>().CurrentValue; }
        }

        public static bool onlyCondemnTarget
        {
            get { return CondemnSettings["onlyCondemnTarget"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool lowlifepeel
        {
            get { return ComboMenu["lowlifepeel"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool dontaaslider
        {
            get { return ComboMenu["dontaaslider"].Cast<CheckBox>().CurrentValue; }
        }

        public static int dontaaenemy
        {
            get { return ComboMenu["dontaaenemy"].Cast<Slider>().CurrentValue; }
        }

        public static bool useHeal
        {
            get { return ItemMenu["useHeal"].Cast<CheckBox>().CurrentValue; }
        }

        public static int lowerHeal
        {
            get { return ItemMenu["lowerHeal"].Cast<Slider>().CurrentValue; }
        }

        public static bool _autoLevel
        {
            get { return ExtraMenu["autoLevel"].Cast<CheckBox>().CurrentValue; }
        }

        #endregion

        #region Menu

        private static Menu Menu,
            ComboMenu,
            QSettings,
            CondemnSettings,
            ESettings,
            FarmSettings,
            ExtraMenu,
            DrawingMenu,
            ItemMenu,
            GPMenu;

        public static void BuildMenu()
        {
            GPMenu = MainMenu.AddMenu("[VHR] Anti-GP List", "dz191.vhr.agplist");
            {
                var enemyHeroesNames =
                    ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy).Select(hero => hero.ChampionName).ToList();
                foreach (var gp in CustomAntigapcloser.Spells.Where(h => enemyHeroesNames.Contains(h.ChampionName)))
                {
                    GPMenu.Add(
                        string.Format("dz191.vhr.agplist.{0}.{1}", gp.ChampionName.ToLowerInvariant(), gp.SpellName),
                        new CheckBox(gp.ChampionName + " " + gp.Slot + " (" + gp.SpellName + ")"));
                }
            }
        }

        private static void InitMenu()
        {
            BuildMenu();

            Menu = MainMenu.AddMenu("Vayne", "Vayne");
            Menu.AddLabel("Base Ported from Challenger Series & features ported from many other assemblies on L# - Berb");
            Menu.AddSeparator();

            ComboMenu = Menu.AddSubMenu("Combo Settings", "combo");
            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.Add("useq", new CheckBox("Use Q")); // UseQBool
            ComboMenu.AddSeparator();
            ComboMenu.Add("focus2w", new CheckBox("Try To Focus 2W", false)); // TryToFocus2WBool
            ComboMenu.Add("lowlifepeel", new CheckBox("Peel w/ E when Low HP", false)); // lowlifepeel
            ComboMenu.Add("dontattackwhileinvisible", new CheckBox("Smart Invisible Attacking"));
                // DontAttackWhileInvisibleAndMeelesNearBool
            ComboMenu.AddSeparator();
            ComboMenu.Add("user", new KeyBind("Use R In Combo", false, KeyBind.BindTypes.PressToggle, 'A')); // UseRBool
            ComboMenu.Add("GetAutoR", new Slider("R if >= X enemies : ", 2, 1, 5)); // GetAutoR
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("1 : Never | 2 : E-Not-Ready | 3 : Always");
            ComboMenu.Add("qantigc", new Slider("Use Q Antigapcloser:", 3, 1, 3)); // UseQAntiGapcloserStringList
            ComboMenu.AddSeparator();
            ComboMenu.Add("dontaa", new KeyBind("Don't Auto Attack in R", false, KeyBind.BindTypes.PressToggle, 'T'));
            ComboMenu.Add("dontaaslider", new CheckBox("^ Only use if greater than X # of enemies?"));
            ComboMenu.Add("dontaaenemy", new Slider("^ # of enemies", 3, 1, 5));
            ComboMenu.AddSeparator();

            QSettings = Menu.AddSubMenu("Q Settings", "qsettings");
            QSettings.AddGroupLabel("Q Settings");
            QSettings.AddSeparator();
            QSettings.AddLabel("1 : Prada | 2 : Marksman | 3 : VHR/SOLO | 4 : Sharpshooter | 5 : SAC");
            QSettings.AddLabel("6 : Cursor | 7 : Kite Melee | 8 : Kurisu | 9 : Safe Position - HikiGaya");
            QSettings.Add("qmode", new Slider("Q Mode:", 5, 1, 9)); // QModeStringList
            QSettings.AddSeparator();
            QSettings.AddGroupLabel("VHR/SOLOVayne Q Settings");
            QSettings.AddLabel("YOU HAVE TO HAVE OPTION 3 SELECTED TO USE THIS");
            QSettings.Add("noqenemies", new CheckBox("Don't Q into enemies")); // noqenemies
            QSettings.Add("smartq", new CheckBox("Try to QE when possible")); // noqenemiesold
            QSettings.Add("2wstacks", new CheckBox("Only Q if 2W Stacks on Target", false)); // 2wstacks
            QSettings.AddSeparator();
            QSettings.AddGroupLabel("Sharpshooter Q Settings");
            QSettings.AddLabel("YOU HAVE TO HAVE OPTION 4 SELECTED TO USE THIS");
            QSettings.Add("antiMelee", new CheckBox("Use Anti-Melee (Q)")); // antiMelee
            QSettings.AddSeparator();
            QSettings.AddGroupLabel("Synx Auto Carry Q Settings");
            QSettings.AddLabel("YOU HAVE TO HAVE OPTION 5 SELECTED TO USE THIS");
            QSettings.AddLabel("1 : Auto Position | 2 : Mouse Cursor");
            QSettings.Add("sacMode", new Slider("Q Mode: ", 1, 1, 2)); // sacMode
            QSettings.Add("DontSafeCheck", new CheckBox("Dont check tumble position is safe")); // DontSafeCheck
            QSettings.Add("DontQIntoEnemies", new CheckBox("Dont Q Into Enemies")); // DontQIntoEnemies
            QSettings.Add("Wall", new CheckBox("Always Tumble to wall if possible")); // Wall
            QSettings.Add("Only2W", new CheckBox("Tumble only when enemy has 2 w stacks", false)); // Only2W
            QSettings.AddSeparator();
            QSettings.AddGroupLabel("Cursor/Kite Melee/Kurisu Q Settings");
            QSettings.AddLabel("YOU HAVE TO HAVE OPTION 6, 7 OR 8 SELECTED TO USE THIS");
            QSettings.Add("Cdynamicqsafety", new CheckBox("Use Dynamic Q Safety"));
            QSettings.Add("Csmartq", new CheckBox("Use Smart Q"));
            QSettings.Add("Cnoqenemies", new CheckBox("Don't Q into enemies"));
            QSettings.Add("Cnoqenemiesold", new CheckBox("Don't Q into enemies Old"));
            QSettings.Add("Cqspam", new CheckBox("Ignore Checks AKA Spam Q"));
            QSettings.AddSeparator();

            CondemnSettings = Menu.AddSubMenu("Condemn Settings", "condemnsettings");
            CondemnSettings.AddGroupLabel("Condemn Menu");
            CondemnSettings.AddSeparator();
            CondemnSettings.AddLabel("1 : Always/Auto | 2 : In Combo | 3 : Never");
            CondemnSettings.Add("usee", new Slider("Use E", 1, 1, 3)); // UseEBool
            CondemnSettings.AddSeparator();
            CondemnSettings.AddLabel("1 : Prada Smart | 2 : Prada Perfect | 3 : Marksman");
            CondemnSettings.AddLabel("4 : Sharpshooter | 5 : Gosu | 6 : VHR");
            CondemnSettings.AddLabel("7 : Prada Legacy | 8 : Fastest | 9 : Old Prada");
            CondemnSettings.AddLabel("10 : Synx Auto Carry | 11 : OKTW | 12 : Shine - HikiCarry");
            CondemnSettings.AddLabel("13 : Asuna - Hikicarry | 14 : 360 - Hikicarry | 15 : SergixCondemn");
            CondemnSettings.Add("emode", new Slider("E Mode: ", 2, 1, 15)); // EModeStringList
            CondemnSettings.AddSeparator();
            CondemnSettings.Add("onlyCondemnTarget", new CheckBox("Only Condemn Current Target", false));
                // UseEInterruptBool
            CondemnSettings.Add("useeinterrupt", new CheckBox("Use E To Interrupt")); // UseEInterruptBool
            CondemnSettings.Add("useeantigapcloser", new CheckBox("Use E AntiGapcloser")); // UseEAntiGapcloserBool
            CondemnSettings.AddSeparator();
            CondemnSettings.Add("epushdist", new Slider("E Push Distance: ", 425, 300, 475)); // EPushDistanceSlider
            CondemnSettings.AddSeparator();
            CondemnSettings.Add("ehitchance", new Slider("Condemn Hitchance", 75)); // EHitchanceSlider
            CondemnSettings.AddSeparator();

            ESettings = Menu.AddSubMenu("E Settings", "esettings");
            ESettings.AddGroupLabel("SAC Condemn Settings");
            ESettings.AddLabel("YOU HAVE TO HAVE OPTION 10 SELECTED TO USE THIS");
            ESettings.Add("Accuracy", new Slider("Accuracy", 12, 2, 12)); // Accuracy
            ESettings.Add("TumbleCondemnCount", new Slider("Q->E Position Check Count", 12, 2, 12));
                // TumbleCondemnCount
            ESettings.Add("TumbleCondemn", new CheckBox("Q->E when possible")); // TumbleCondemn
            ESettings.AddSeparator();
            ESettings.Add("TumbleCondemnSafe", new CheckBox("Only Q->E when tumble position is safe", false));
                // TumbleCondemnSafe
            ESettings.Add("DontCondemnTurret", new CheckBox("Don't Condemn under turret?")); // TumbleCondemnSafe
            ESettings.AddSeparator();
            ESettings.AddGroupLabel("OKTW Condemn Settings");
            ESettings.AddLabel("YOU HAVE TO HAVE OPTION 11 SELECTED TO USE THIS");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
                ESettings.Add("stun" + enemy.ChampionName, new CheckBox("Condemn : " + enemy.ChampionName + "?"));
            }
            ESettings.AddSeparator();

            FarmSettings = Menu.AddSubMenu("Farm Settings", "farm");
            FarmSettings.AddGroupLabel("Farm Menu");
            FarmSettings.AddSeparator();
            FarmSettings.Add("useQLane", new CheckBox("Use Q Lane Clear"));
            FarmSettings.Add("useQJG", new CheckBox("Use Q Jungle Clear"));
            FarmSettings.Add("useejgfarm", new CheckBox("Use E Jungle")); // UseEJungleFarm
            FarmSettings.AddSeparator();

            ExtraMenu = Menu.AddSubMenu("Extra Settings", "extra");
            ExtraMenu.AddGroupLabel("Extra Settings");
            ExtraMenu.AddSeparator();
            ExtraMenu.Add("autoLevel", new CheckBox("Auto Level", false));
            ExtraMenu.Add("usee3rdwproc", new CheckBox("Use E as 3rd W Proc", false)); // UseEAs3rdWProcBool
            ExtraMenu.Add("useqonenemiesnotcs", new CheckBox("Use Q Bonus On ENEMY not CS", false));
                // UseQBonusOnEnemiesNotCS
            ExtraMenu.Add("useqonlyon2stackedenemies", new CheckBox("Use Q If Enemy Have 2W Stacks", false));
                // UseQOnlyAt2WStacksBool
            ExtraMenu.AddSeparator();

            ItemMenu = Menu.AddSubMenu("Activator", "item");
            ItemMenu.Add("useHeal", new CheckBox("Use Heal?"));
            ItemMenu.Add("lowerHeal", new Slider("Lower than X HP to heal :", 15));
            ItemMenu.AddSeparator();
            ItemMenu.Add("useBotrk", new CheckBox("Use Blade of the Ruined King?"));
            ItemMenu.Add("useCutlass", new CheckBox("Use Bilgewater Cutlass?"));
            ItemMenu.Add("useGhostBlade", new CheckBox("Use GhostBlade?"));
            ItemMenu.AddSeparator();
            ItemMenu.Add("Clean", new CheckBox("Auto QSS/Mercurial/Dervish/Mikaels/Cleanse"));
            ItemMenu.AddSeparator();
            ItemMenu.Add("CSSDelay", new Slider("QSS Delay", 0, 0, 1000)); // CSSDelay
            ItemMenu.AddSeparator();
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(ally => ally.IsAlly && !ally.IsMe))
            {
                ItemMenu.Add("MikaelsAlly" + ally.ChampionName, new CheckBox("Mikael : " + ally.ChampionName + "?"));
            }
            ItemMenu.AddSeparator();
            ItemMenu.Add("cleanHP", new Slider("Use only under % HP (101 : Forever)", 95, 0, 101)); // cleanHP
            ItemMenu.AddSeparator();
            ItemMenu.Add("CleanSpells", new CheckBox("Cleanse Dangerous (Zed R etc.)"));
            ItemMenu.Add("Stun", new CheckBox("Stun"));
            ItemMenu.Add("Snare", new CheckBox("Snare"));
            ItemMenu.Add("Charm", new CheckBox("Charm"));
            ItemMenu.Add("Fear", new CheckBox("Fear"));
            ItemMenu.Add("Suppression", new CheckBox("Suppression"));
            ItemMenu.Add("Taunt", new CheckBox("Taunt"));
            ItemMenu.Add("Blind", new CheckBox("Blind"));
            ItemMenu.AddSeparator();

            DrawingMenu = Menu.AddSubMenu("Draw Settings", "draw");
            DrawingMenu.AddGroupLabel("Drawing Menu");
            DrawingMenu.AddSeparator();
            DrawingMenu.Add("drawwstacks", new CheckBox("Draw W Stacks")); // DrawWStacksBool
            DrawingMenu.Add("menukey", new CheckBox("Draw Menu Keybinds")); // DrawWStacksBool
            DrawingMenu.Add("drawCurrentLogic", new CheckBox("Draw Q/E Current Logic")); // DrawWStacksBool
            DrawingMenu.AddSeparator();
        }

        #endregion Menu

        #region ChampionLogic

        public static bool IsCondemnable(AIHeroClient hero)
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical);

            if (onlyCondemnTarget)
            {
                if (target.NetworkId != hero.NetworkId)
                {
                    return false;
                }
            }

            if (!hero.IsValidTarget(550f) || hero.HasBuffOfType(BuffType.SpellShield) ||
                hero.HasBuffOfType(BuffType.SpellImmunity) || hero.IsDashing())
            {
                return false;
            }

            var pP = myHero.ServerPosition;
            var p = hero.ServerPosition;
            var pD = EPushDistanceSlider;
            var mode = EModeStringList;

            if (mode == 1 &&
                (IsCollisionable((Vector3) p.Extend(pP, -pD)) || IsCollisionable((Vector3) p.Extend(pP, -pD/2f)) ||
                 IsCollisionable((Vector3) p.Extend(pP, -pD/3f))))
            {
                if (!hero.CanMove)
                {
                    return true;
                }

                var enemiesCount = myHero.CountEnemiesInRange(1200);
                if (enemiesCount > 1 && enemiesCount <= 3)
                {
                    var prediction = E2.GetPrediction(hero);
                    for (var i = 15; i < pD; i += 75)
                    {
                        if (i > pD)
                        {
                            var lastPosFlags =
                                NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -pD).To3D());
                            if (lastPosFlags.HasFlag(CollisionFlags.Wall) ||
                                lastPosFlags.HasFlag(CollisionFlags.Building))
                            {
                                return true;
                            }
                            return false;
                        }
                        var posFlags = NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -i));
                        if (posFlags.HasFlag(CollisionFlags.Wall) || posFlags.HasFlag(CollisionFlags.Building))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                var hitchance = EHitchanceSlider;
                var angle = 0.20*hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float) (p.X + travelDistance*Math.Cos(Math.PI/180*angle)),
                    (float) (p.X + travelDistance*Math.Sin(Math.PI/180*angle)));
                var beta = new Vector2((float) (p.X - travelDistance*Math.Cos(Math.PI/180*angle)),
                    (float) (p.X - travelDistance*Math.Sin(Math.PI/180*angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (i > pD) return false;
                    if (IsCollisionable(pP.To2D().Extend(alpha, i).To3D()) &&
                        IsCollisionable(pP.To2D().Extend(beta, i).To3D())) return true;
                }
                return false;
            }

            if (mode == 2 &&
                (IsCollisionable(p.Extend(pP, -pD).To3D()) || IsCollisionable(p.Extend(pP, -pD/2f).To3D()) ||
                 IsCollisionable(p.Extend(pP, -pD/3f).To3D())))
            {
                if (!hero.CanMove)
                {
                    return true;
                }

                var hitchance = EHitchanceSlider;
                var angle = 0.20*hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float) (p.X + travelDistance*Math.Cos(Math.PI/180*angle)),
                    (float) (p.X + travelDistance*Math.Sin(Math.PI/180*angle)));
                var beta = new Vector2((float) (p.X - travelDistance*Math.Cos(Math.PI/180*angle)),
                    (float) (p.X - travelDistance*Math.Sin(Math.PI/180*angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (i > pD)
                    {
                        return IsCollisionable(alpha.Extend(pP.To2D(), -pD).To3D()) &&
                               IsCollisionable(beta.Extend(pP.To2D(), -pD).To3D());
                    }
                    if (IsCollisionable(alpha.Extend(pP.To2D(), -i).To3D()) &&
                        IsCollisionable(beta.Extend(pP.To2D(), -i).To3D())) return true;
                }
                return false;
            }

            if (mode == 9)
            {
                if (!hero.CanMove)
                    return true;

                var hitchance = EHitchanceSlider;
                var angle = 0.20*hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float) (p.X + travelDistance*Math.Cos(Math.PI/180*angle)),
                    (float) (p.X + travelDistance*Math.Sin(Math.PI/180*angle)));
                var beta = new Vector2((float) (p.X - travelDistance*Math.Cos(Math.PI/180*angle)),
                    (float) (p.X - travelDistance*Math.Sin(Math.PI/180*angle)));

                for (var i = 15; i < pD; i += 100)
                {
                    if (IsCollisionable(pP.To2D().Extend(alpha, i).To3D()) ||
                        IsCollisionable(pP.To2D().Extend(beta, i).To3D())) return true;
                }
                return false;
            }

            if (mode == 3)
            {
                var prediction = E2.GetPrediction(hero);
                return
                    NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -pD).To3D())
                        .HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -pD/2f).To3D())
                        .HasFlag(CollisionFlags.Wall);
            }

            if (mode == 4)
            {
                var prediction = E2.GetPrediction(hero);
                for (var i = 15; i < pD; i += 100)
                {
                    if (i > pD) return false;
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -i).To3D());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 5)
            {
                var prediction = E2.GetPrediction(hero);
                for (var i = 15; i < pD; i += 75)
                {
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -i).To3D());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 6)
            {
                var prediction = E2.GetPrediction(hero);
                for (var i = 15; i < pD; i += (int) hero.BoundingRadius) //:frosty:
                {
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -i).To3D());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 7)
            {
                var prediction = E2.GetPrediction(hero);
                for (var i = 15; i < pD; i += 75)
                {
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.To2D().Extend(pP.To2D(), -i).To3D());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (mode == 8 &&
                (IsCollisionable((Vector3) p.Extend(pP, -pD)) || IsCollisionable((Vector3) p.Extend(pP, -pD/2f)) ||
                 IsCollisionable((Vector3) p.Extend(pP, -pD/3f))))
            {
                return true;
            }

            if (mode == 10)
            {
                if (IsValidTarget(hero))
                {
                    return true;
                }
            }

            if (mode == 11)
            {
                if (CondemnCheck(myHero.ServerPosition, hero) &&
                    ESettings["stun" + hero.ChampionName].Cast<CheckBox>().CurrentValue)
                {
                    return true;
                }
            }

            if (mode == 12) // Shine
            {
                foreach (var a in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget(E.Range)))
                {
                    if (onlyCondemnTarget)
                    {
                        if (a.NetworkId != target.NetworkId)
                        {
                            return false;
                        }
                    }
                    var pushDistance = EPushDistanceSlider;
                    var targetPosition = E2.GetPrediction(a).UnitPosition;
                    var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized();
                    var checkDistance = pushDistance/40f;
                    for (var i = 0; i < 40; i++)
                    {
                        var finalPosition = targetPosition + pushDirection*checkDistance*i;
                        var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                        if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building))
                            //not sure about building, I think its turrets, nexus etc
                        {
                            return true;
                        }
                    }
                }
            }

            if (mode == 13) // Asuna
            {
                foreach (
                    var En in
                        EntityManager.Heroes.Enemies.Where(
                            b =>
                                b.IsValidTarget(E.Range) && !b.HasBuffOfType(BuffType.SpellShield) &&
                                !b.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    if (onlyCondemnTarget)
                    {
                        if (En.NetworkId != target.NetworkId)
                        {
                            return false;
                        }
                    }
                    var EPred = E2.GetPrediction(En);
                    var pushDist = EPushDistanceSlider;
                    var FinalPosition =
                        EPred.UnitPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -pushDist).To3D();

                    for (var i = 1; i < pushDist; i += (int) En.BoundingRadius)
                    {
                        var loc3 =
                            EPred.UnitPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -i).To3D();

                        if (loc3.IsWall() || AsunasAllyFountain(FinalPosition))
                        {
                            return true;
                        }
                    }
                }
            }

            if (mode == 14) // 360
            {
                foreach (
                    var enemy in
                        EntityManager.Heroes.Enemies.Where(
                            x =>
                                x.IsValidTarget(E.Range) && !x.HasBuffOfType(BuffType.SpellShield) &&
                                !x.HasBuffOfType(BuffType.SpellImmunity) && threeSixty(x)))
                {
                    if (onlyCondemnTarget)
                    {
                        if (enemy.NetworkId != target.NetworkId)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            if (mode == 15) // SergixCondemn
            {
                var pointwaswall = false;
                var d = target.Position.Distance(Efinishpos(target));
                for (var i = 0; i < d; i += 10)
                {
                    var dist = i > d ? d : i;
                    var point = target.Position.Extend(Efinishpos(target), dist);
                    if (pointwaswall)
                    {
                        if (point.IsWall())
                        {
                            return true;
                        }
                    }
                    if (point.IsWall())
                    {
                        pointwaswall = true;
                    }
                }
            }

            return false;
        }

        public static Vector3 Efinishpos(Obj_AI_Base ts)
        {
            return myHero.Position.Extend(ts.Position, ObjectManager.Player.Distance(ts.Position) + 490).To3D();
        }

        public static float CondemnRange = 550f;
        public static float CondemnKnockback = 490f;

        public static List<Vector2> Points = new List<Vector2>();

        public static bool threeSixty(AIHeroClient unit, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) ||
                ObjectManager.Player.IsDashing()) return false;
            var prediction = E2.GetPrediction(unit);
            var predictionsList = pos.IsValid()
                ? new List<Vector3> {pos.To3D()}
                : new List<Vector3>
                {
                    unit.ServerPosition,
                    unit.Position,
                    prediction.CastPosition,
                    prediction.UnitPosition
                };

            var wallsFound = 0;
            Points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < EPushDistanceSlider; i += (int) unit.BoundingRadius) // 420 = push distance
                {
                    var cPos =
                        ObjectManager.Player.Position.Extend(position, ObjectManager.Player.Distance(position) + i)
                            .To3D();
                    Points.Add(cPos.To2D());
                    if (NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Wall) ||
                        NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }
            if (wallsFound/predictionsList.Count >= 33/100f)
            {
                return true;
            }

            return false;
        }

        public static bool AsunasAllyFountain(Vector3 position)
        {
            float fountainRange = 750;
            var map = Game.MapId;
            if (Game.MapId == GameMapId.SummonersRift)
            {
                fountainRange = 1050;
            }
            return
                ObjectManager.Get<GameObject>()
                    .Where(spawnPoint => spawnPoint is Obj_SpawnPoint && spawnPoint.IsAlly)
                    .Any(spawnPoint => Vector2.Distance(position.To2D(), spawnPoint.Position.To2D()) < fountainRange);
        }

        public static bool IsValidTarget(AIHeroClient target)
        {
            var targetPosition = LS.Geometry.PositionAfter(target.GetWaypoints(), 300, (int) target.MoveSpeed);

            if (target.Distance(ObjectManager.Player.ServerPosition) < 650f &&
                IsCondemnable(ObjectManager.Player.ServerPosition.To2D(), targetPosition, target.BoundingRadius))
            {
                if (target.Path.Length == 0)
                {
                    var outRadius = 0.3f*target.MoveSpeed/(float) Math.Cos(2*Math.PI/Accuracy);
                    var count = 0;
                    for (var i = 1; i <= Accuracy; i++)
                    {
                        if (count + (Accuracy - i) < Accuracy/3)
                            return false;

                        var angle = i*2*Math.PI/Accuracy;
                        var x = target.Position.X + outRadius*(float) Math.Cos(angle);
                        var y = target.Position.Y + outRadius*(float) Math.Sin(angle);
                        if (IsCondemnable(ObjectManager.Player.ServerPosition.To2D(), new Vector2(x, y),
                            target.BoundingRadius))
                            count++;
                    }
                    return count >= Accuracy/3;
                }
                return true;
            }
            if (TumbleCondemn && Q.IsReady())
            {
                var outRadius = 300/(float) Math.Cos(2*Math.PI/TumbleCondemnCount);

                for (var i = 1; i <= TumbleCondemnCount; i++)
                {
                    var angle = i*2*Math.PI/TumbleCondemnCount;
                    var x = ObjectManager.Player.Position.X + outRadius*(float) Math.Cos(angle);
                    var y = ObjectManager.Player.Position.Y + outRadius*(float) Math.Sin(angle);
                    targetPosition = target.GetWaypoints().PositionAfter(300, (int) target.MoveSpeed);
                    var vec = new Vector2(x, y);
                    if (targetPosition.Distance(vec) < 550f &&
                        IsCondemnable(vec, targetPosition, target.BoundingRadius, 300f))
                    {
                        if (!TumbleCondemnSafe || Tumble.IsSafe(target, vec.To3D(), false).IsValid())
                        {
                            myHero.Spellbook.CastSpell(SpellSlot.Q, (Vector3) vec);
                            break;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        internal static Vector2 Deviation(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI/180.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0)
            {
                X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/4,
                Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/4
            };
            result = Vector2.Add(result, point1);
            return result;
        }

        private static bool IsCondemnable(Vector2 from, Vector2 targetPosition, float boundingRadius,
            float pushRange = -1)
        {
            if (pushRange == -1)
                pushRange = EPushDistanceSlider - 20f;

            var pushDirection = (targetPosition - from).Normalized();
            for (var i = 0; i < pushRange; i += 20)
            {
                var lastPost = targetPosition + pushDirection*i;
                if (!lastPost.To3D().UnderTurret(true) || !DontCondemnTurret)
                {
                    var colFlags = NavMesh.GetCollisionFlags(lastPost.X, lastPost.Y);
                    if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                    {
                        var sideA = lastPost + pushDirection*20f + pushDirection.Perpendicular()*boundingRadius;
                        var sideB = lastPost + pushDirection*20f - pushDirection.Perpendicular()*boundingRadius;

                        var flagsA = NavMesh.GetCollisionFlags(sideA.X, sideA.Y);
                        var flagsB = NavMesh.GetCollisionFlags(sideB.X, sideB.Y);

                        if ((flagsA.HasFlag(CollisionFlags.Wall) || flagsA.HasFlag(CollisionFlags.Building)) &&
                            (flagsB.HasFlag(CollisionFlags.Wall) || flagsB.HasFlag(CollisionFlags.Building)))
                            return true;
                    }
                }
            }
            return false;
        }

        public static Vector3 GetAggressiveTumblePos(Obj_AI_Base target)
        {
            var cursorPos = Game.CursorPos;

            if (!IsDangerousPosition(cursorPos)) return cursorPos;
            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && myHero.CountEnemiesInRange(800) == 1) return cursorPos;

            var aRC = new Geometry.Circle(myHero.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();

            var targetPosition = target.ServerPosition;

            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).To3D();
                var dist = v3.Distance(targetPosition);
                if (dist > 325 && dist < 450)
                {
                    return v3;
                }
            }
            return Vector3.Zero;
        }

        internal static class WaypointTracker
        {
            #region Static Fields

            public static readonly Dictionary<int, List<Vector2>> StoredPaths = new Dictionary<int, List<Vector2>>();
            public static readonly Dictionary<int, int> StoredTick = new Dictionary<int, int>();

            #endregion
        }

        public static List<Vector2> GetWaypoints(this Obj_AI_Base unit)
        {
            var result = new List<Vector2>();

            if (unit.IsVisible)
            {
                result.Add(unit.ServerPosition.To2D());
                result.AddRange(unit.Path.Select(point => point.To2D()));
            }
            else
            {
                List<Vector2> value;
                if (WaypointTracker.StoredPaths.TryGetValue(unit.NetworkId, out value))
                {
                    var path = value;
                    var timePassed = (Environment.TickCount - WaypointTracker.StoredTick[unit.NetworkId])/1000f;
                    if (path.GetPathLength() >= unit.MoveSpeed*timePassed)
                    {
                        result = CutPath(path, (int) (unit.MoveSpeed*timePassed));
                    }
                }
            }

            return result;
        }

        private static bool CondemnCheck(Vector3 fromPosition, AIHeroClient target) // OKTW Vayne E Logic
        {
            var prepos = E2.GetPrediction(target);

            float pushDistance = 460;

            if (myHero.ServerPosition != fromPosition)
                pushDistance = 410;

            var radius = 220;
            var start2 = target.ServerPosition;
            var end2 = prepos.CastPosition.Extend(fromPosition, -pushDistance);

            var start = start2.To2D();
            var end = end2;
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightEndPos = end + pDir*radius;
            var leftEndPos = end - pDir*radius;


            var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
            var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);


            var step = start2.Distance(rEndPos)/10;
            for (var i = 0; i < 10; i++)
            {
                var pr = start2.Extend(rEndPos, step*i);
                var pl = start2.Extend(lEndPos, step*i);
                if (pr.IsWall() && pl.IsWall())
                    return true;
            }

            return false;
        }

        public static float GetPathLength(this List<Vector2> path)
        {
            var distance = 0f;

            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        public static List<Vector2> CutPath(this List<Vector2> path, float distance)
        {
            var result = new List<Vector2>();
            for (var i = 0; i < path.Count - 1; i++)
            {
                var dist = path[i].Distance(path[i + 1]);
                if (dist > distance)
                {
                    result.Add(path[i] + distance*(path[i + 1] - path[i]).Normalized());

                    for (var j = i + 1; j < path.Count; j++)
                    {
                        result.Add(path[j]);
                    }

                    break;
                }

                distance -= dist;
            }

            return result.Count > 0 ? result : new List<Vector2> {path.Last()};
        }

        public static Vector3 GetTumblePos(Obj_AI_Base target)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return GetAggressiveTumblePos(target);

            var cursorPos = Game.CursorPos;

            if (!target.IsMelee && myHero.CountEnemiesInRange(800) == 1) return cursorPos;

            var targetWaypoints = GetWaypoints(target);

            if (targetWaypoints[targetWaypoints.Count - 1].Distance(myHero.ServerPosition) > 550)
                return Vector3.Zero;

            var aRC = new Geometry.Circle(myHero.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();
            var targetPosition = target.ServerPosition;
            var pList = (from p in aRC
                select new Vector2(p.X, p.Y).To3D()
                into v3
                let dist = v3.Distance(targetPosition)
                where !IsDangerousPosition(v3) && dist < 500
                select v3).ToList();

            if (myHero.UnderTurret() || myHero.CountEnemiesInRange(800) == 1 || cursorPos.CountEnemiesInRange(450) <= 1)
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
            }
            return pList.Count > 1
                ? pList.OrderByDescending(el => el.Distance(cursorPos)).FirstOrDefault()
                : Vector3.Zero;
        }

        public static bool UnderTurret(this Obj_AI_Base unit)
        {
            return UnderTurret(unit.Position, true);
        }

        public static bool UnderTurret(this Obj_AI_Base unit, bool enemyTurretsOnly)
        {
            return UnderTurret(unit.Position, enemyTurretsOnly);
        }

        public static bool UnderTurret(this Vector3 position, bool enemyTurretsOnly)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.IsValidTarget(950));
        }

        public static bool IsDangerousPosition(Vector3 pos)
        {
            return
                EntityManager.Heroes.Enemies.Any(
                    e =>
                        e.IsValidTarget() && (e.Distance(pos) < 375) &&
                        (e.GetWaypoints().LastOrDefault().Distance(pos) > 550)) ||
                (pos.UnderTurret(true) && !myHero.UnderTurret(true));
        }

        public static bool IsCollisionable(Vector3 pos)
        {
            return NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) ||
                   NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building);
        }

        #endregion
    }

    #region VHRPolygon

    internal class VHRPolygon
    {
        public List<Vector2> Points;

        public VHRPolygon(List<Vector2> p)
        {
            Points = p;
        }

        public void Add(Vector2 vec)
        {
            Points.Add(vec);
        }

        public int Count()
        {
            return Points.Count;
        }

        public bool Contains(Vector2 point)
        {
            var result = false;
            var j = Count() - 1;
            for (var i = 0; i < Count(); i++)
            {
                if (Points[i].Y < point.Y && Points[j].Y >= point.Y || Points[j].Y < point.Y && Points[i].Y >= point.Y)
                {
                    if (Points[i].X +
                        (point.Y - Points[i].Y)/(Points[j].Y - Points[i].Y)*(Points[j].X - Points[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static List<Vector2> Rectangle(Vector2 startVector2, Vector2 endVector2, float radius)
        {
            var points = new List<Vector2>();

            var v1 = endVector2 - startVector2;
            var to1Side = Vector2.Normalize(v1).Perpendicular()*radius;

            points.Add(startVector2 + to1Side);
            points.Add(startVector2 - to1Side);
            points.Add(endVector2 - to1Side);
            points.Add(endVector2 + to1Side);
            return points;
        }
    }

    #endregion
}