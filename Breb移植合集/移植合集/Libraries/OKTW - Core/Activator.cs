using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Activator
    {
        public static Menu Config = Program.Config;
        public static Menu Sub;

        public static Items.Item

            //Cleans
            Mikaels = new Items.Item(3222, 600f),
            Quicksilver = new Items.Item(3140),
            Mercurial = new Items.Item(3139),
            Dervish = new Items.Item(3137),
            //REGEN
            Potion = new Items.Item(2003),
            ManaPotion = new Items.Item(2004),
            Flask = new Items.Item(2041),
            Biscuit = new Items.Item(2010),
            Refillable = new Items.Item(2031),
            Hunter = new Items.Item(2032),
            Corrupting = new Items.Item(2033),
            //attack
            Botrk = new Items.Item(3153, 550f),
            Cutlass = new Items.Item(3144, 550f),
            Youmuus = new Items.Item(3142, 650f),
            Hydra = new Items.Item(3074, 440f),
            Hydra2 = new Items.Item(3077, 440f),
            HydraTitanic = new Items.Item(3748, 150f),
            Hextech = new Items.Item(3146, 700f),
            FrostQueen = new Items.Item(3092, 850f),

            //def
            FaceOfTheMountain = new Items.Item(3401, 600f),
            Zhonya = new Items.Item(3157),
            Seraph = new Items.Item(3040),
            Solari = new Items.Item(3190, 600f),
            Randuin = new Items.Item(3143, 400f);

        private SpellSlot heal, barrier, ignite, exhaust, flash, smite, teleport, cleanse;

        private AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool getCheckBoxItem(string item)
        {
            return Sub[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Sub[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Sub[item].Cast<KeyBind>().CurrentValue;
        }

        public void LoadOKTW()
        {
            Sub = Config.AddSubMenu("Activator OKTW©");

            teleport = Player.GetSpellSlot("SummonerTeleport");
            heal = Player.GetSpellSlot("summonerheal");
            barrier = Player.GetSpellSlot("summonerbarrier");
            ignite = Player.GetSpellSlot("summonerdot");
            exhaust = Player.GetSpellSlot("summonerexhaust");
            flash = Player.GetSpellSlot("summonerflash");
            cleanse = Player.GetSpellSlot("SummonerBoost");
            smite = Player.GetSpellSlot("summonersmite");

            if (smite == SpellSlot.Unknown)
            {
                smite = Player.GetSpellSlot("itemsmiteaoe");
            }
            if (smite == SpellSlot.Unknown)
            {
                smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
            }
            if (smite == SpellSlot.Unknown)
            {
                smite = Player.GetSpellSlot("s5_summonersmitequick");
            }
            if (smite == SpellSlot.Unknown)
            {
                smite = Player.GetSpellSlot("s5_summonersmiteduel");
            }

            if (smite != SpellSlot.Unknown)
            {
                Sub.AddGroupLabel("Summoner > Smite");
                Sub.Add("SmiteEnemy", new CheckBox("Auto Smite enemy under 50% hp"));
                Sub.Add("SmiteEnemyKS", new CheckBox("Auto Smite enemy KS"));
                Sub.Add("Smite", new KeyBind("Auto Smite mobs OKTW", false, KeyBind.BindTypes.PressToggle, 'N'));
                Sub.Add("Rdragon", new CheckBox("Dragon"));
                Sub.Add("Rbaron", new CheckBox("Baron"));
                Sub.Add("Rherald", new CheckBox("Herald"));
                Sub.Add("Rred", new CheckBox("Red"));
                Sub.Add("Rblue", new CheckBox("Blue"));
                Sub.AddSeparator();
                //Config.Item("Smite").Permashow(true);
            }

            if (exhaust != SpellSlot.Unknown)
            {
                Sub.AddGroupLabel("Summoner > Exhaust");
                Sub.Add("Exhaust", new CheckBox("Exhaust"));
                Sub.Add("Exhaust1", new CheckBox("Exhaust if Channeling Important Spell"));
                Sub.Add("Exhaust2", new CheckBox("Always in combo", false));
                Sub.AddSeparator();
            }

            if (heal != SpellSlot.Unknown)
            {
                Sub.AddGroupLabel("Summoner > Heal");
                Sub.Add("Heal", new CheckBox("Heal"));
                Sub.Add("AllyHeal", new CheckBox("AllyHeal"));
                Sub.AddSeparator();
            }
            if (barrier != SpellSlot.Unknown)
            {
                Sub.AddGroupLabel("Summoner > Barrier");
                Sub.Add("Barrier", new CheckBox("Barrier"));
                Sub.AddSeparator();
            }
            if (ignite != SpellSlot.Unknown)
            {
                Sub.AddGroupLabel("Summoner > Ignite");
                Sub.Add("Ignite", new CheckBox("Ignite"));
                Sub.AddSeparator();
            }

            if (cleanse != SpellSlot.Unknown)
            {
                Sub.AddGroupLabel("Summoner > Cleanse");
                Sub.Add("Cleanse", new CheckBox("Cleanse"));
                Sub.AddSeparator();
            }

            Sub.Add("pots", new CheckBox("Potion, ManaPotion, Flask, Biscuit"));
            Sub.AddSeparator();

            // OFFENSIVE
            Sub.AddGroupLabel("Item > BOTRK");
            Sub.Add("Botrk", new CheckBox("Botrk"));
            Sub.Add("BotrkKS", new CheckBox("Botrk KS"));
            Sub.Add("BotrkLS", new CheckBox("Botrk LifeSaver"));
            Sub.Add("BotrkCombo", new CheckBox("Botrk always in combo", false));
            Sub.AddSeparator();

            Sub.AddGroupLabel("Item > Cutlass");
            Sub.Add("Cutlass", new CheckBox("Cutlass"));
            Sub.Add("CutlassKS", new CheckBox("Cutlass KS"));
            Sub.Add("CutlassCombo", new CheckBox("Cutlass always in combo"));
            Sub.AddSeparator();

            Sub.AddGroupLabel("Item > Hextech");
            Sub.Add("Hextech", new CheckBox("Hextech"));
            Sub.Add("HextechKS", new CheckBox("Hextech KS"));
            Sub.Add("HextechCombo", new CheckBox("Hextech always in combo"));
            Sub.AddSeparator();

            Sub.AddGroupLabel("Item > Youmuus");
            Sub.Add("Youmuus", new CheckBox("Youmuus"));
            Sub.Add("YoumuusR", new CheckBox("TwitchR, AsheQ"));
            Sub.Add("YoumuusKS", new CheckBox("Youmuus KS"));
            Sub.Add("YoumuusCombo", new CheckBox("Youmuus always in combo", false));
            Sub.AddSeparator();

            Sub.AddGroupLabel("Item > Hydra");
            Sub.Add("Hydra", new CheckBox("Hydra"));
            Sub.Add("HydraTitanic", new CheckBox("Hydra Titanic"));
            Sub.AddSeparator();

            Sub.AddGroupLabel("Item > FrostQueen");
            Sub.Add("FrostQueen", new CheckBox("FrostQueen"));
            Sub.AddSeparator();

            // DEF
            Sub.AddGroupLabel("Item > Defensive");
            Sub.Add("Randuin", new CheckBox("Randuin"));
            Sub.Add("FaceOfTheMountain", new CheckBox("FaceOfTheMountain"));
            Sub.Add("Zhonya", new CheckBox("Zhonya"));

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                var spell = enemy.Spellbook.Spells[3];
                Sub.Add("spellZ" + spell.SData.Name,
                    new CheckBox(enemy.ChampionName + ": " + spell.Name,
                        spell.SData.TargettingType == SpellDataTargetType.Unit));
            }

            Sub.Add("Seraph", new CheckBox("Seraph"));
            Sub.Add("Solari", new CheckBox("Solari"));
            Sub.AddSeparator();

            // CLEANSERS

            Sub.Add("Clean", new CheckBox("Quicksilver, Mikaels, Mercurial, Dervish"));

            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(ally => ally.IsAlly))
            {
                Sub.Add("MikaelsAlly" + ally.ChampionName, new CheckBox("Mikaels :" + ally.ChampionName));
            }

            Sub.Add("CSSdelay", new Slider("Delay x ms", 0, 0, 1000));
            Sub.Add("cleanHP", new Slider("Use only under % HP", 80));
            Sub.Add("CleanSpells", new CheckBox("ZedR FizzR MordekaiserR PoppyR VladimirR"));
            Sub.Add("Stun", new CheckBox("Stun"));
            Sub.Add("Snare", new CheckBox("Snare"));
            Sub.Add("Charm", new CheckBox("Charm"));
            Sub.Add("Fear", new CheckBox("Fear"));
            Sub.Add("Suppression", new CheckBox("Suppression"));
            Sub.Add("Taunt", new CheckBox("Taunt"));
            Sub.Add("Blind", new CheckBox("Blind"));

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (getCheckBoxItem("HydraTitanic") && Program.Combo && HydraTitanic.IsReady() &&
                target.IsValid<AIHeroClient>())
            {
                HydraTitanic.Cast();
                Orbwalker.ResetAutoAttack();
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || sender.Type != GameObjectType.AIHeroClient)
                return;


            if (sender.Distance(Player.Position) > 1600)
                return;

            if (Zhonya.IsReady() && getCheckBoxItem("Zhonya"))
            {
                if (Sub["spellZ" + args.SData.Name] != null && getCheckBoxItem("spellZ" + args.SData.Name))
                {
                    if (args.Target != null && args.Target.NetworkId == Player.NetworkId)
                    {
                        Zhonya.Cast();
                    }
                    else
                    {
                        var castArea = Player.Distance(args.End)*(args.End - Player.ServerPosition).Normalized() +
                                       Player.ServerPosition;
                        if (castArea.Distance(Player.ServerPosition) < Player.BoundingRadius/2)
                            Zhonya.Cast();
                    }
                }
            }

            if (CanUse(exhaust) && getCheckBoxItem("Exhaust"))
            {
                foreach (
                    var ally in
                        Program.Allies.Where(
                            ally =>
                                ally.IsValid && !ally.IsDead && ally.HealthPercent < 51 &&
                                Player.Distance(ally.ServerPosition) < 700))
                {
                    double dmg = 0;
                    if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
                    {
                        dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
                    }
                    else
                    {
                        var castArea = ally.Distance(args.End)*(args.End - ally.ServerPosition).Normalized() +
                                       ally.ServerPosition;
                        if (castArea.Distance(ally.ServerPosition) < ally.BoundingRadius/2)
                            dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
                        else
                            continue;
                    }

                    if (ally.Health - dmg < ally.CountEnemiesInRange(700)*ally.Level*40)
                        Player.Spellbook.CastSpell(exhaust, sender);
                }
            }
        }

        private void Survival()
        {
            if (Player.HealthPercent < 60 && (Seraph.IsReady() || Zhonya.IsReady() || CanUse(barrier)))
            {
                var dmg = OktwCommon.GetIncomingDamage(Player, 1);
                var enemys = Player.CountEnemiesInRange(800);
                if (dmg > 0 || enemys > 0)
                {
                    if (CanUse(barrier) && getCheckBoxItem("Barrier"))
                    {
                        var value = 95 + Player.Level*20;
                        if (dmg > value && Player.HealthPercent < 50)
                            Player.Spellbook.CastSpell(barrier, Player);
                        else if (Player.Health - dmg < enemys*Player.Level*20)
                            Player.Spellbook.CastSpell(barrier, Player);
                        else if (Player.Health - dmg < Player.Level*10)
                            Seraph.Cast();
                    }

                    if (Seraph.IsReady() && getCheckBoxItem("Seraph"))
                    {
                        var value = Player.Mana*0.2 + 150;
                        if (dmg > value && Player.HealthPercent < 50)
                            Seraph.Cast();
                        else if (Player.Health - dmg < enemys*Player.Level*20)
                            Seraph.Cast();
                        else if (Player.Health - dmg < Player.Level*10)
                            Seraph.Cast();
                    }

                    if (Zhonya.IsReady() && getCheckBoxItem("Zhonya"))
                    {
                        if (dmg > Player.Level*35)
                        {
                            Zhonya.Cast();
                        }
                        else if (Player.Health - dmg < enemys*Player.Level*20)
                        {
                            Zhonya.Cast();
                        }
                        else if (Player.Health - dmg < Player.Level*10)
                        {
                            Zhonya.Cast();
                        }
                    }
                }
            }


            if (!Solari.IsReady() && !FaceOfTheMountain.IsReady() && !CanUse(heal))
                return;

            foreach (
                var ally in
                    Program.Allies.Where(
                        ally =>
                            ally.IsValid && !ally.IsDead && ally.HealthPercent < 50 &&
                            Player.Distance(ally.ServerPosition) < 700))
            {
                var dmg = OktwCommon.GetIncomingDamage(ally, 1);
                var enemys = ally.CountEnemiesInRange(700);
                if (dmg == 0 && enemys == 0)
                    continue;

                if (CanUse(heal) && getCheckBoxItem("Heal"))
                {
                    if (!getCheckBoxItem("AllyHeal") && !ally.IsMe)
                        return;

                    if (ally.Health - dmg < enemys*ally.Level*15)
                        Player.Spellbook.CastSpell(heal, ally);
                    else if (ally.Health - dmg < ally.Level*10)
                        Player.Spellbook.CastSpell(heal, ally);
                }

                if (getCheckBoxItem("Solari") && Solari.IsReady() && Player.Distance(ally.ServerPosition) < Solari.Range)
                {
                    var value = 75 + 15*Player.Level;
                    if (dmg > value && Player.HealthPercent < 50)
                        Solari.Cast();
                    else if (ally.Health - dmg < enemys*ally.Level*15)
                        Solari.Cast();
                    else if (ally.Health - dmg < ally.Level*10)
                        Solari.Cast();
                }

                if (getCheckBoxItem("FaceOfTheMountain") && FaceOfTheMountain.IsReady() &&
                    Player.Distance(ally.ServerPosition) < FaceOfTheMountain.Range)
                {
                    var value = 0.1*Player.MaxHealth;
                    if (dmg > value && Player.HealthPercent < 50)
                        FaceOfTheMountain.Cast(ally);
                    else if (ally.Health - dmg < enemys*ally.Level*15)
                        FaceOfTheMountain.Cast(ally);
                    else if (ally.Health - dmg < ally.Level*10)
                        FaceOfTheMountain.Cast(ally);
                }
            }
        }


        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!Youmuus.IsReady() || !getCheckBoxItem("YoumuusR"))
                return;
            if (args.Slot == SpellSlot.R && (Player.ChampionName == "Twitch"))
            {
                Youmuus.Cast();
            }
            if (args.Slot == SpellSlot.Q && (Player.ChampionName == "Ashe"))
            {
                Youmuus.Cast();
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.InFountain() || Player.IsRecalling() || Player.IsDead)
            {
                return;
            }

            Cleansers();
            Smite();
            Survival();

            if (!Program.LagFree(0))
                return;

            if (getCheckBoxItem("pots"))
                PotionManagement();

            Ignite();
            //Teleport();
            Exhaust();
            Offensive();
            Defensive();
            ZhonyaCast();
        }

        private void Teleport()
        {
            if (CanUse(teleport) && !Player.HasBuff("teleport"))
            {
                foreach (
                    var ally in
                        Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.CountEnemiesInRange(1000) > 0)
                    )
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValid && !enemy.IsDead))
                    {
                        var distanceEA = enemy.Distance(ally);
                        if (distanceEA < 1000)
                        {
                            foreach (
                                var obj in
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .Where(obj => obj.IsAlly && distanceEA < obj.Position.Distance(ally.Position)))
                            {
                                Player.Spellbook.CastSpell(teleport, obj);
                            }
                        }
                    }
                }
            }
        }

        private void Smite()
        {
            if (CanUse(smite))
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 520, MinionTeam.Neutral);
                if (mobs.Count == 0 &&
                    (Player.GetSpellSlot("s5_summonersmiteplayerganker") != SpellSlot.Unknown ||
                     Player.GetSpellSlot("s5_summonersmiteduel") != SpellSlot.Unknown))
                {
                    var enemy = TargetSelector.GetTarget(500, DamageType.True);
                    if (enemy.IsValidTarget())
                    {
                        if (enemy.HealthPercent < 50 && getCheckBoxItem("SmiteEnemy"))
                            Player.Spellbook.CastSpell(smite, enemy);

                        var smiteDmg = Player.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Smite);

                        if (getCheckBoxItem("SmiteEnemyKS") &&
                            enemy.Health - OktwCommon.GetIncomingDamage(enemy) < smiteDmg)
                            Player.Spellbook.CastSpell(smite, enemy);
                    }
                }
                if (mobs.Count > 0 && getKeyBindItem("Smite"))
                {
                    foreach (var mob in mobs)
                    {
                        if (((mob.BaseSkinName == "SRU_Dragon" && getCheckBoxItem("Rdragon"))
                             || (mob.BaseSkinName == "SRU_Baron" && getCheckBoxItem("Rbaron"))
                             || (mob.BaseSkinName == "SRU_RiftHerald" && getCheckBoxItem("Rherald"))
                             || (mob.BaseSkinName == "SRU_Red" && getCheckBoxItem("Rred"))
                             || (mob.BaseSkinName == "SRU_Blue" && getCheckBoxItem("Rblue")))
                            && mob.Health <= Player.GetSummonerSpellDamage(mob, DamageLibrary.SummonerSpells.Smite))
                        {
                            Player.Spellbook.CastSpell(smite, mob);
                        }
                    }
                }
            }
        }

        private void Exhaust()
        {
            if (CanUse(exhaust) && getCheckBoxItem("Exhaust"))
            {
                if (getCheckBoxItem("Exhaust1"))
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(650) && enemy.IsChannelingImportantSpell()))
                    {
                        Player.Spellbook.CastSpell(exhaust, enemy);
                    }
                }

                if (getCheckBoxItem("Exhaust2") && Program.Combo)
                {
                    var t = TargetSelector.GetTarget(650, DamageType.Physical);
                    if (t.IsValidTarget())
                    {
                        Player.Spellbook.CastSpell(exhaust, t);
                    }
                }
            }
        }

        private void Ignite()
        {
            if (CanUse(ignite) && getCheckBoxItem("Ignite"))
            {
                var enemy = TargetSelector.GetTarget(600, DamageType.True);
                if (enemy.IsValidTarget() && OktwCommon.ValidUlt(enemy))
                {
                    var pred = enemy.Health - OktwCommon.GetIncomingDamage(enemy);

                    var IgnDmg = Player.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite);

                    if (pred <= IgnDmg && Player.ServerPosition.Distance(enemy.ServerPosition) > 500 &&
                        enemy.CountAlliesInRange(500) < 2)
                        Player.Spellbook.CastSpell(ignite, enemy);

                    if (pred <= 2*IgnDmg)
                    {
                        if (enemy.PercentLifeStealMod > 10)
                            Player.Spellbook.CastSpell(ignite, enemy);

                        if (enemy.HasBuff("RegenerationPotion") || enemy.HasBuff("ItemMiniRegenPotion") ||
                            enemy.HasBuff("ItemCrystalFlask"))
                            Player.Spellbook.CastSpell(ignite, enemy);

                        if (enemy.Health > Player.Health)
                            Player.Spellbook.CastSpell(ignite, enemy);
                    }
                }
            }
        }

        private void ZhonyaCast()
        {
            if (getCheckBoxItem("Zhonya") && Zhonya.IsReady())
            {
                float time = 10;
                if (Player.HasBuff("zedrdeathmark"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "zedulttargetmark");
                }
                if (Player.HasBuff("FizzMarinerDoom"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "FizzMarinerDoom");
                }
                if (Player.HasBuff("MordekaiserChildrenOfTheGrave"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "MordekaiserChildrenOfTheGrave");
                }
                if (Player.HasBuff("VladimirHemoplague"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "VladimirHemoplague");
                }
                if (time < 1 && time > 0)
                    Zhonya.Cast();
            }
        }

        private void Cleansers()
        {
            if (!Quicksilver.IsReady() && !Mikaels.IsReady() && !Mercurial.IsReady() && !Dervish.IsReady() &&
                !cleanse.IsReady())
                return;

            if (Player.HealthPercent >= getSliderItem("cleanHP") || !getCheckBoxItem("Clean"))
                return;

            if (Player.HasBuff("zedrdeathmark") || Player.HasBuff("FizzMarinerDoom") ||
                Player.HasBuff("MordekaiserChildrenOfTheGrave") || Player.HasBuff("PoppyDiplomaticImmunity") ||
                Player.HasBuff("VladimirHemoplague"))
                Clean();

            if (Mikaels.IsReady())
            {
                foreach (var ally in Program.Allies.Where(
                    ally =>
                        ally.IsValid && !ally.IsDead && getCheckBoxItem("MikaelsAlly" + ally.ChampionName) &&
                        Player.Distance(ally.Position) < Mikaels.Range
                        && ally.HealthPercent < (float) getSliderItem("cleanHP")))
                {
                    if (ally.HasBuff("zedrdeathmark") || ally.HasBuff("FizzMarinerDoom") ||
                        ally.HasBuff("MordekaiserChildrenOfTheGrave") || ally.HasBuff("PoppyDiplomaticImmunity") ||
                        ally.HasBuff("VladimirHemoplague"))
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Stun) && getCheckBoxItem("Stun")) // getCheckBoxItem("Stun")
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Snare) && getCheckBoxItem("Snare")) // getCheckBoxItem("Snare")
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Charm) && getCheckBoxItem("Charm")) // getCheckBoxItem("Charm")
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Fear) && getCheckBoxItem("Fear")) // getCheckBoxItem("Fear")
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Stun) && getCheckBoxItem("Stun")) // getCheckBoxItem("Stun")
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Taunt) && getCheckBoxItem("Taunt")) // getCheckBoxItem("Taunt")
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Suppression) && getCheckBoxItem("Suppression"))
                        // getCheckBoxItem("Suppression")
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Blind) && getCheckBoxItem("Blind")) // getCheckBoxItem("Blind")
                        Mikaels.Cast(ally);
                }
            }

            if (Player.HasBuffOfType(BuffType.Stun) && getCheckBoxItem("Stun"))
                Clean();
            if (Player.HasBuffOfType(BuffType.Snare) && getCheckBoxItem("Snare"))
                Clean();
            if (Player.HasBuffOfType(BuffType.Charm) && getCheckBoxItem("Charm"))
                Clean();
            if (Player.HasBuffOfType(BuffType.Fear) && getCheckBoxItem("Fear"))
                Clean();
            if (Player.HasBuffOfType(BuffType.Stun) && getCheckBoxItem("Stun"))
                Clean();
            if (Player.HasBuffOfType(BuffType.Taunt) && getCheckBoxItem("Taunt"))
                Clean();
            if (Player.HasBuffOfType(BuffType.Suppression) && getCheckBoxItem("Suppression"))
                Clean();
            if (Player.HasBuffOfType(BuffType.Blind) && getCheckBoxItem("Blind"))
                Clean();
        }

        private void Clean()
        {
            if (Quicksilver.IsReady())
                Utility.DelayAction.Add(getSliderItem("CSSdelay"), () => Quicksilver.Cast());
            else if (Mercurial.IsReady())
                Utility.DelayAction.Add(getSliderItem("CSSdelay"), () => Mercurial.Cast());
            else if (Dervish.IsReady())
                Utility.DelayAction.Add(getSliderItem("CSSdelay"), () => Dervish.Cast());
            else if (cleanse != SpellSlot.Unknown && cleanse.IsReady() && getCheckBoxItem("Cleanse"))
                Utility.DelayAction.Add(getSliderItem("CSSdelay"), () => Player.Spellbook.CastSpell(cleanse, Player));
        }

        private void Defensive()
        {
            if (Randuin.IsReady() && getCheckBoxItem("Randuin") && Player.CountEnemiesInRange(Randuin.Range) > 0)
            {
                Randuin.Cast();
            }
        }

        private void Offensive()
        {
            if (Botrk.IsReady() && getCheckBoxItem("Botrk"))
            {
                var t = TargetSelector.GetTarget(Botrk.Range, DamageType.Physical);
                if (t.IsValidTarget())
                {
                    if (getCheckBoxItem("BotrkKS") &&
                        Player.CalcDamage(t, DamageType.Physical, t.MaxHealth*0.1) >
                        t.Health - OktwCommon.GetIncomingDamage(t))
                        Botrk.Cast(t);
                    if (getCheckBoxItem("BotrkLS") &&
                        Player.Health < Player.MaxHealth*0.5 - OktwCommon.GetIncomingDamage(Player))
                        Botrk.Cast(t);
                    if (getCheckBoxItem("BotrkCombo") && Program.Combo)
                        Botrk.Cast(t);
                }
            }

            if (Hextech.IsReady() && getCheckBoxItem("Hextech"))
            {
                var t = TargetSelector.GetTarget(Hextech.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (getCheckBoxItem("HextechKS") &&
                        Player.CalcDamage(t, DamageType.Magical, 150 + Player.FlatMagicDamageMod*0.4) >
                        t.Health - OktwCommon.GetIncomingDamage(t))
                        Hextech.Cast(t);
                    if (getCheckBoxItem("HextechCombo") && Program.Combo)
                        Hextech.Cast(t);
                }
            }

            if (Program.Combo && FrostQueen.IsReady() && getCheckBoxItem("FrostQueen") &&
                Player.CountEnemiesInRange(800) > 0)
            {
                FrostQueen.Cast();
            }

            if (Cutlass.IsReady() && getCheckBoxItem("Cutlass"))
            {
                var t = TargetSelector.GetTarget(Cutlass.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (getCheckBoxItem("CutlassKS") &&
                        Player.CalcDamage(t, DamageType.Magical, 100) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Cutlass.Cast(t);
                    if (getCheckBoxItem("CutlassCombo") && Program.Combo)
                        Cutlass.Cast(t);
                }
            }

            if (Youmuus.IsReady() && getCheckBoxItem("Youmuus"))
            {
                var t = Orbwalker.LastTarget;

                if (t.IsValidTarget() && t is AIHeroClient)
                {
                    if (getCheckBoxItem("YoumuusKS") && t.Health < Player.MaxHealth)
                        Youmuus.Cast();
                    if (getCheckBoxItem("YoumuusCombo") && Program.Combo)
                        Youmuus.Cast();
                }
            }

            if (getCheckBoxItem("Hydra"))
            {
                if (Hydra.IsReady() && Player.CountEnemiesInRange(Hydra.Range) > 0)
                    Hydra.Cast();
                else if (Hydra2.IsReady() && Player.CountEnemiesInRange(Hydra2.Range) > 0)
                    Hydra2.Cast();
            }
        }

        private void PotionManagement()
        {
            if (Player.Health + 250 > Player.MaxHealth)
                return;

            if (Player.HealthPercent > 50 && Player.CountEnemiesInRange(700) == 0)
                return;

            if (Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemMiniRegenPotion") ||
                Player.HasBuff("ItemCrystalFlaskJungle") || Player.HasBuff("ItemDarkCrystalFlask") ||
                Player.HasBuff("ItemCrystalFlask"))
                return;

            if (Potion.IsReady())
                Potion.Cast();
            else if (Biscuit.IsReady())
                Biscuit.Cast();
            else if (Hunter.IsReady())
                Hunter.Cast();
            else if (Corrupting.IsReady())
                Corrupting.Cast();
            else if (Refillable.IsReady())
                Refillable.Cast();
        }

        private bool CanUse(SpellSlot sum)
        {
            if (sum != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(sum) == SpellState.Ready)
                return true;
            return false;
        }
    }
}