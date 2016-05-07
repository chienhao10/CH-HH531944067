namespace SkyLv_Taric
{

    using LeagueSharp;
    using LeagueSharp.Common;

    using System.Linq;
    using System.Collections.Generic;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    internal class SkyLv_Taric
    {

        public static Menu Menu;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);

        public static List<Spell> SpellList = new List<Spell>();

        public static List<AIHeroClient> Enemies = new List<AIHeroClient>(), Allies = new List<AIHeroClient>();

        public const string ChampionName = "Taric";

        public static Menu Combo, Harass, Misc, Draw, LaneClear, JungleClear;

        public static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public static readonly string[] Monsters =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", "TT_NGolem5",
            "TT_NGolem2", "TT_NWolf6", "TT_NWolf3","TT_NWraith1", "TT_Spider"
        };

        public SkyLv_Taric()
        {

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 300f);
            W = new Spell(SpellSlot.W, 1100f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R, 400f);

            Q.SetTargetted(0.5f, 300);
            W.SetTargetted(0.5f, 300);
            E.SetSkillshot(0.1f, 100, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetTargetted(2.5f, 300);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menu = MainMenu.AddMenu("SkyLv " + ChampionName + " By LuNi", "SkyLv " + ChampionName + " By LuNi");

            Combo = Menu.AddSubMenu("Combo", "Combo");
            //
            Combo.AddGroupLabel("Q Settings :");
            Combo.Add("Taric.UseQCombo", new CheckBox("Use Self Q In Combo"));
            Combo.Add("Taric.MinimumStackSelfQCombo", new Slider("Minimum Q Stack To Use Self Q In Combo", 2, 1, 3));
            Combo.Add("Taric.MinimumHpPercentSelfQCombo", new Slider("Minimum Hp Percent To Use Self Q In Combo", 100, 0, 100));
            Combo.Add("Taric.UseQAlly", new CheckBox("Use Q On Ally"));
            Combo.Add("Taric.UseQAllyMode", new ComboBox("Q On Ally Mode", 1, "On Combo Key", "Auto Cast"));

            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(a => a.Team == Player.Team && a.NetworkId != Player.NetworkId))
            {
                Combo.AddLabel(Ally.ChampionName);
                Combo.Add(Ally.NetworkId + "MinimumHpQAlly", new Slider("Q Ally If Health Percent Under", 60, 0, 100));
                Combo.Add(Ally.NetworkId + "MinimumStacksQAlly", new Slider("Minimum Q Stack To Use Q On Ally", 2, 1, 3));
                Combo.AddSeparator();
            }
            //

            //
            Combo.AddGroupLabel("W Settings :");
            Combo.Add("Taric.UseWCombo", new CheckBox("Use Self W In Combo"));
            Combo.Add("Taric.UseWIncomingDamageCombo", new CheckBox("Use Self W Only On Incoming Damages In Combo"));
            Combo.Add("Taric.UseWAlly", new CheckBox("Use W On Ally"));
            Combo.Add("Taric.UseWAllyMode", new ComboBox("W On Ally Mode", 1, "On Combo Key", "Auto Cast"));
            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(a => a.Team == Player.Team && a.NetworkId != Player.NetworkId))
            {
                Combo.AddLabel(Ally.ChampionName);
                Combo.Add(Ally.NetworkId + "MinimumHpWAlly", new Slider("W Ally If Health Percent Under", 100, 0, 100));
                Combo.Add(Ally.NetworkId + "IncomingDamageWAlly", new CheckBox("Only On Ally Incoming Damage", true));
                Combo.AddSeparator();
            }
            //

            //
            Combo.AddSeparator();
            Combo.AddGroupLabel("E Settings :");
            Combo.Add("Taric.UseECombo", new CheckBox("Use Self E In Combo"));
            Combo.Add("Taric.UseEFromAlly", new CheckBox("Use E From Ally In Combo"));
            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(a => a.Team == Player.Team && a.NetworkId != Player.NetworkId))
            {
                Combo.AddLabel(Ally.ChampionName);
                Combo.Add(Ally.NetworkId + "AlwaysComboFromAlly", new CheckBox("Always E From This Ally If Can't Cast MySelf", true));
                Combo.Add(Ally.NetworkId + "AllyCCEComboFromAlly", new CheckBox("On Ally CC'ed", true));
                Combo.Add(Ally.NetworkId + "TargetCCEComboFromAlly", new CheckBox("On Target CC'ed", true));
                Combo.Add(Ally.NetworkId + "TargetInterruptEComboFromAlly", new CheckBox("Auto E From Ally On Interruptable Target", true));
                Combo.Add(Ally.NetworkId + "TargetDashEPEComboFromAlly", new CheckBox("On Target Dash End Position", true));
                Combo.AddSeparator();
            }
            //
            Combo.Add("Taric.UseTaricAAPassiveCombo", new CheckBox("Use All Taric AA Passive In Combo"));

            Combo.AddGroupLabel("Auto Spell Usage");
            Combo.AddLabel("Auto Q Settings");
            Combo.Add("Taric.UseAutoQ", new CheckBox("Use Auto Q Safe Mode"));
            Combo.Add("Taric.MinimumHpSafeAutoQ", new Slider("Minimum Health Percent To Use Auto Q Safe Mode", 25, 0, 100));
            Combo.Add("Taric.MinimumEnemySafeAutoQ", new Slider("Minimum Enemy In Range To Use Auto Q Safe Mode", 1, 0, 5));
            Combo.Add("Taric.MinimumStackSafeAutoQ", new Slider("Minimum Q Stack To Use Auto Q Safe Mode", 1, 1, 3));
            Combo.AddLabel("Auto W Settings");
            Combo.Add("Taric.UseAutoW", new CheckBox("Use Auto W Safe Mode", false));
            Combo.Add("Taric.MinimumHpSafeAutoW", new Slider("Minimum Health Percent To Use Auto W Safe Mode", 25, 0, 100));
            Combo.Add("Taric.MinimumEnemySafeAutoW", new Slider("Minimum Enemy In Range To Use Auto W Safe Mode", 1, 0, 5));
            Combo.AddLabel("Auto E Settings");
            Combo.Add("Taric.UseAutoEGapCloser", new CheckBox("Use E Anti Gap Closer"));
            Combo.Add("Taric.MinimumHpEGapCloser", new Slider("E Anti Gap Closer If Health Percent Under Or Equal", 100, 0, 100));
            Combo.Add("Taric.MinimumEnemyEGapCloser", new Slider("Minimum Enemy In Range To E Anti Gap Closer", 0, 0, 5));
            Combo.AddLabel("Auto R Settings");
            Combo.Add("Taric.UseAutoR", new CheckBox("Use Auto R Safe Mode"));
            Combo.Add("Taric.MinimumHpSafeAutoR", new Slider("Minimum Health Percent To Use Auto R Safe Mode", 40, 0, 100));
            Combo.Add("Taric.MinimumEnemySafeAutoR", new Slider("Minimum Enemy In Range To Use Auto R Safe Mode", 1, 0, 5));
            Combo.AddSeparator();
            Combo.AddGroupLabel("KS Mode");
            Combo.Add("Taric.KS", new CheckBox("Kill Steal"));
            Combo.AddGroupLabel("KS Mode > Spell Settings");
            Combo.Add("Taric.UseAAKS", new CheckBox("KS With AA"));
            Combo.Add("Taric.UseIgniteKS", new CheckBox("KS With Ignite"));
            Combo.Add("Taric.UseEKS", new CheckBox("KS With E"));
            Combo.AddSeparator();
            Combo.AddGroupLabel("Combo Misc");
            Combo.Add("Taric.EOnDashendPosition", new CheckBox("Use E On Enemy Dash End Position"));

            Harass = Menu.AddSubMenu("Harass", "Harass");
            Harass.AddGroupLabel("Q :");
            Harass.Add("Taric.UseQHarass", new CheckBox("Use Q In Harass", false));
            Harass.Add("Taric.QMiniManaHarass", new Slider("Minimum Mana To Use Q In Harass", 50, 0, 100));
            Harass.AddGroupLabel("W :");
            Harass.Add("Taric.UseWHarass", new CheckBox("Use W In Harass", false));
            Harass.Add("Taric.WMiniManaHarass", new Slider("Minimum Mana To Use W In Harass", 50, 0, 100));
            Harass.AddGroupLabel("E :");
            Harass.Add("Taric.UseEHarass", new CheckBox("Use E In Harass", true));
            Harass.Add("Taric.EMiniManaHarass", new Slider("Minimum Mana To Use E In Harass", 0, 0, 100));
            Harass.AddGroupLabel("Settings :");
            Harass.Add("Taric.UseTaricAAPassiveHarass", new CheckBox("Use All Taric AA Passive In Harass"));
            Harass.Add("Taric.HarassActiveT", new KeyBind("Harass (toggle) !", false, KeyBind.BindTypes.PressToggle, 'J'));

            LaneClear = Menu.AddSubMenu("LaneClear", "LaneClear");
            LaneClear.AddGroupLabel("Q :");
            LaneClear.Add("Taric.UseQLaneClear", new CheckBox("Use Q in LaneClear", false));
            LaneClear.Add("Taric.QMiniManaLaneClear", new Slider("Minimum Mana Percent To Use Q In LaneClear"));
            LaneClear.Add("Taric.QMiniMinimionAroundLaneClear", new Slider("Minimum Minion Around To Use Q In LaneClear", 6, 1, 10));
            LaneClear.AddGroupLabel("W :");
            LaneClear.Add("Taric.UseWLaneClear", new CheckBox("Use W in LaneClear", false));
            LaneClear.Add("Taric.WMiniManaLaneClear", new Slider("Minimum Mana Percent To Use W In LaneClear"));
            LaneClear.Add("Taric.WMiniMinimionAroundLaneClear", new Slider("Minimum Minion Around To Use W In LaneClear", 6, 1, 10));
            LaneClear.AddGroupLabel("E :");
            LaneClear.Add("Taric.UseELaneClear", new CheckBox("Use E in LaneClear", false));
            LaneClear.Add("Taric.EMiniManaLaneClear", new Slider("Minimum Mana Percent To Use E In LaneClear"));
            LaneClear.Add("Taric.EMiniHitLaneClear", new Slider("Minimum Minion Hit To Use E In LaneClear", 3, 1, 6));
            LaneClear.AddGroupLabel("Settings :");
            LaneClear.Add("Taric.SafeLaneClear", new CheckBox("Dont Use Spell In Lane Clear If Enemy in Dangerous Range"));
            LaneClear.Add("Taric.UseTaricAAPassiveLaneClear", new CheckBox("Use All Taric AA Passive In LaneClear"));

            JungleClear = Menu.AddSubMenu("JungleClear", "JungleClear");
            JungleClear.AddGroupLabel("Q :");
            JungleClear.Add("Taric.UseQJungleClear", new CheckBox("Use Q In JungleClear"));
            JungleClear.Add("Taric.QMiniManaJungleClear", new Slider("Minimum Mana To Use Q In JungleClear"));
            JungleClear.AddGroupLabel("W :");
            JungleClear.Add("Taric.UseWJungleClear", new CheckBox("Use W In JungleClear"));
            JungleClear.Add("Taric.WMiniManaJungleClear", new Slider("Minimum Mana To Use W In JungleClear"));
            JungleClear.AddGroupLabel("E :");
            JungleClear.Add("Taric.UseEJungleClear", new CheckBox("Use E In JungleClear"));
            JungleClear.Add("Taric.EMiniManaJungleClear", new Slider("Minimum Mana To Use E In JungleClear"));
            JungleClear.AddGroupLabel("Settings :");
            JungleClear.Add("Taric.SafeJungleClear", new CheckBox("Dont Use Spell In Jungle Clear If Enemy in Dangerous Range"));
            JungleClear.Add("Taric.SpellOnlyBigMonster", new CheckBox("Use Spell Only On Big Monster"));
            JungleClear.Add("Taric.UseTaricAAPassiveJungleClear", new CheckBox("Use All Taric AA Passive In JungleClear"));
            JungleClear.AddGroupLabel("-----------------------------------------------");
            JungleClear.AddGroupLabel("Jungle KS Mode :");
            JungleClear.Add("Taric.JungleKS", new CheckBox("Jungle KS"));
            JungleClear.AddGroupLabel("Jungle KS Mode > Advanced Settings :");
            JungleClear.Add("Taric.UseAAJungleKS", new CheckBox("KS With AA"));
            JungleClear.Add("Taric.UseEJungleKS", new CheckBox("KS With E"));

            Misc = Menu.AddSubMenu("Misc", "Misc");
            Misc.Add("Taric.UsePacketCast", new CheckBox("Use PacketCast", false));
            Misc.Add("Taric.AutoEInterrupt", new CheckBox("Auto E On Interruptable"));
            Misc.Add("Taric.AutoPotion", new CheckBox("Use Auto Potion"));
            Misc.AddGroupLabel("Auto Move Best Pos In Fountain");
            if (Game.MapId == GameMapId.SummonersRift)
            {
                Misc.Add("Taric.AutoMoveFountainMovePosSummonersRift", new CheckBox("Auto Move Best Pos in Fountain", false));
                Misc.Add("Taric.FountainMovePosSummonersRift", new ComboBox("Player Lane", 1, "Mid", "Top", "Bot"));
            }

            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                Misc.Add("Taric.AutoMoveFountainMovePosTwistedTreeline", new CheckBox("Auto Move Best Pos in Fountain"));
                Misc.Add("Taric.FountainMovePosTwistedTreeline", new ComboBox("Player Lane", 0, "Top", "Bot"));
            }

            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                Misc.Add("Taric.AutoMoveFountainMovePosHowlingAbyss", new CheckBox("Auto Move Best Pos in Fountain"));
            }

            Draw = Menu.AddSubMenu("Drawings", "Drawings");
            Draw.Add("QRange", new CheckBox("Q range"));//.SetValue(new Circle(true, System.Drawing.Color.Orange)));
            Draw.Add("WRange", new CheckBox("W range"));//.SetValue(new Circle(true, System.Drawing.Color.Green)));
            Draw.Add("ERange", new CheckBox("E range"));//.SetValue(new Circle(true, System.Drawing.Color.Blue)));
            Draw.Add("RRange", new CheckBox("R range"));//.SetValue(new Circle(true, System.Drawing.Color.Gold)));
            Draw.Add("DrawOrbwalkTarget", new CheckBox("Draw Orbwalk target"));//.SetValue(new Circle(true, System.Drawing.Color.Pink)));
            Draw.Add("SpellDraw.Radius", new Slider("Spell Draw Radius", 10, 1, 20));
            Draw.Add("OrbwalkDraw.Radius", new Slider("Orbwalk Draw Radius", 10, 1, 20));


            new OnUpdateFeatures();
            new OnProcessSpellCast();
            new Interrupter();
            new CastOnDash();
            new AntiGapCLoser();
            new CastOnDash();
            new KillSteal();
            new JungleSteal();
            new Combo();
            new Harass();
            new JungleClear();
            new LaneClear();
            new PotionManager();
            new Draw();
            new FountainMoves();
        }
    }
}