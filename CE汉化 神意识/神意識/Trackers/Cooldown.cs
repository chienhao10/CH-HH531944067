namespace CowAwareness.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using CowAwareness.Features;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    using Color = System.Drawing.Color;

    public class Cooldown : Feature
    {
        #region Static Fields

        private static readonly SpellSlot[] SpellsSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        private static readonly Dictionary<string, Sprite> SummonerSpells = new Dictionary<string, Sprite>();

        private static readonly SpellSlot[] SummonersSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };

        #endregion

        #region Fields

        private Text text;

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "Cooldowns";
            }
        }

        #endregion

        #region Properties

        private bool TrackAllies
        {
            get
            {
                return this["allies"].Cast<CheckBox>().CurrentValue;
            }
        }

        private bool TrackEnemies
        {
            get
            {
                return this["enemies"].Cast<CheckBox>().CurrentValue;
            }
        }

        private bool TrackSelf
        {
            get
            {
                return this["self"].Cast<CheckBox>().CurrentValue;
            }
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Menu.AddLabel("偵測技能和召喚師技能CD");

            this.Menu.Add("allies", new CheckBox("偵測隊友"));
            this.Menu.Add("enemies", new CheckBox("偵測敵人"));
            this.Menu.Add("self", new CheckBox("偵測自己"));

            this.text = new Text(string.Empty, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold));

            SummonerSpells.Add("summonerheal", new Sprite(TextureLoader.BitmapToTexture(Resources.summonerheal)));
            SummonerSpells.Add("summonerhaste", new Sprite(TextureLoader.BitmapToTexture(Resources.summonerhaste)));
            SummonerSpells.Add("summonerflash", new Sprite(TextureLoader.BitmapToTexture(Resources.summonerflash)));
            SummonerSpells.Add(
                "summonerteleport",
                new Sprite(TextureLoader.BitmapToTexture(Resources.summonerteleport)));
            SummonerSpells.Add("summonerexhaust", new Sprite(TextureLoader.BitmapToTexture(Resources.summonerexhaust)));
            SummonerSpells.Add("summonerdot", new Sprite(TextureLoader.BitmapToTexture(Resources.summonerdot)));
            SummonerSpells.Add("summonerboost", new Sprite(TextureLoader.BitmapToTexture(Resources.summonerboost)));
            SummonerSpells.Add("summonerbarrier", new Sprite(TextureLoader.BitmapToTexture(Resources.summonerbarrier)));
            SummonerSpells.Add("summonersmite", new Sprite(TextureLoader.BitmapToTexture(Resources.summonersmite)));
            SummonerSpells.Add(
                "s5_summonersmiteduel",
                new Sprite(TextureLoader.BitmapToTexture(Resources.s5_summonersmiteduel)));
            SummonerSpells.Add(
                "s5_summonersmiteplayerganker",
                new Sprite(TextureLoader.BitmapToTexture(Resources.s5_summonersmiteplayerganker)));

            this.Menu.AddSeparator(30);
            this.Menu.AddLabel("已知問題:");
            this.Menu.AddLabel("不能偵測一些可以主動減技能CD");
            this.Menu.AddLabel("例如菲歐拉Q,刀妹Q");

            Drawing.OnEndScene += this.Drawing_OnEndScene;
        }

        private float AdditionalXOffset(AIHeroClient hero)
        {
            var champName = hero.ChampionName.ToLower();

            switch (champName)
            {
                case "darius":
                    return -4;
            }

            return 0;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var hero in EntityManager.Heroes.AllHeroes.Where(o => o.IsHPBarRendered))
            {
                if (hero.IsMe && !this.TrackSelf)
                {
                    continue;
                }

                if (hero.IsEnemy && !this.TrackEnemies)
                {
                    continue;
                }

                if (hero.IsAlly && !hero.IsMe && !this.TrackAllies)
                {
                    continue;
                }

                foreach (var slot in SpellsSlots)
                {
                    this.DrawSpell(hero, slot);
                }

                foreach (var slot in SummonersSlots)
                {
                    this.DrawSummoner(hero, slot);
                }
            }
        }

        private void DrawSpell(AIHeroClient hero, SpellSlot slot)
        {
            var spell = hero.Spellbook.GetSpell(slot);
            var color = Color.Cyan;
            var cooldown = spell.CooldownExpires - Game.Time;
            var location = this.GetSpellLocation(hero, slot);

            var str = slot.ToString();

            if (!spell.IsLearned)
            {
                str = "?";
                color = Color.White;
            }
            else if (hero.Mana < spell.SData.Mana)
            {
                str = "M";
                color = Color.Red;
            }
            else if (cooldown > 0)
            {
                str = ((int)Math.Ceiling(cooldown)).ToString();
                color = Color.Orange;
            }

            this.text.Draw(string.Format("{0,3}", str), color, (int)location.X, (int)location.Y);
        }

        private void DrawSummoner(AIHeroClient hero, SpellSlot slot)
        {
            var spell = hero.Spellbook.GetSpell(slot);
            var cooldown = spell.CooldownExpires - Game.Time;
            var location = this.GetSummonerLocation(hero, spell.Slot);
            var color = cooldown > 0 ? Color.Red : Color.Green;

            Sprite sprite;

            if (SummonerSpells.TryGetValue(spell.Name.ToLower(), out sprite))
            {
                sprite.Draw(location);
            }

            var lineX = location.X + (hero.IsMe ? 17 : -3);
            Drawing.DrawLine(new Vector2(lineX, location.Y), new Vector2(lineX, location.Y - 16), 3f, color);

            if (cooldown > 0)
            {
                var offset = hero.IsMe ? 22 : -27;
                this.text.Draw(
                    string.Format("{0,3}", (int)Math.Ceiling(cooldown)),
                    Color.White,
                    (int)location.X + offset,
                    (int)location.Y);
            }
        }

        private Vector2 GetSpellLocation(AIHeroClient hero, SpellSlot slot)
        {
            var gap = 27;
            var x = hero.HPBarPosition.X + (hero.IsMe ? 34 : 0) + this.AdditionalXOffset(hero);
            var y = hero.HPBarPosition.Y + 23;

            switch (slot)
            {
                case SpellSlot.Q:
                    return new Vector2(x, y);
                case SpellSlot.W:
                    return new Vector2(x + gap, y);
                case SpellSlot.E:
                    return new Vector2(x + 2 * gap, y);
                case SpellSlot.R:
                    return new Vector2(x + 3 * gap, y);
            }

            return Vector2.Zero;
        }

        private Vector2 GetSummonerLocation(AIHeroClient hero, SpellSlot slot)
        {
            var x = hero.HPBarPosition.X + (hero.IsMe ? 133 : -18) + this.AdditionalXOffset(hero);
            var y = hero.HPBarPosition.Y + (hero.IsMe ? -9 : -4);

            var location = new Vector2(x, y);

            if (slot == SpellSlot.Summoner2)
            {
                location.Y += 16;
            }

            return location;
        }

        #endregion
    }
}