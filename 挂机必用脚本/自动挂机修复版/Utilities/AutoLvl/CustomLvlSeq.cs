using System;
using System.IO;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace AutoBuddy.Utilities.AutoLvl
{
    internal enum SkillToLvl
    {
        NotSet = 0,
        Q = 1,
        W = 2,
        E = 3,
        R = 4
    }


    internal class CustomLvlSeq
    {
        private readonly bool[] locked;
        private readonly CheckBox updater;
        private readonly AIHeroClient champ;
        private readonly DefautSequences def;
        private string lvlFile;
        private readonly LvlSlider[] sliders;
        private readonly SkillToLvl[] skills;
        private readonly int maxLvl;
        private readonly CheckBox clear, defau, profile1, profile2;
        private int profile;
        private bool sa;
        private readonly string dir;
        private readonly string se;
        private readonly Slider humanMin, humanMax;
        private readonly SkillLevelUp lvlUp;

        public CustomLvlSeq(Menu m, AIHeroClient champ, string dir, string seq = "", int maxlvl = 18)
        {
            locked = new bool[] { true };
            this.dir = dir;
            se = seq;
            Menu menuSettings = m.AddSubMenu("Skill LvlUp", "AB_SL_SETTINGS");
            CheckBox enabled = new CheckBox("Enabled", true);
            menuSettings.AddGroupLabel("General");
            menuSettings.Add(champ + "enabled", enabled);
            menuSettings.AddSeparator(10);
            menuSettings.AddGroupLabel("Current profile");
            profile1 = new CheckBox("Profile 1", true);
            profile2 = new CheckBox("Profile 2", false);
            menuSettings.Add(champ.ChampionName + Game.MapId + "p1", profile1);
            menuSettings.Add(champ.ChampionName + Game.MapId + "p2", profile2);


            
            updater = new CheckBox("Update default sequences");
            clear = new CheckBox("Clear current profile", false);
            menuSettings.Add("clear", clear);
            defau = new CheckBox("Set current profile to default");
            menuSettings.Add("defaults", defau);
            menuSettings.AddSeparator(10);

            menuSettings.AddGroupLabel("Humanizer");
            humanMin = new Slider("Minimum time after level up to upgrade an ability(miliseconds)", 300, 0, 2000);
            humanMax = new Slider("Maximum time after level up to upgrade an ability(miliseconds)", 500, 0, 2000);
            menuSettings.Add("xhm", humanMin);
            menuSettings.Add("xhmx", humanMax);
            humanMin.OnValueChange += humanMin_OnValueChange;
            humanMax.OnValueChange += humanMax_OnValueChange;
            menuSettings.AddSeparator(10);
            menuSettings.AddGroupLabel("Updater");
            menuSettings.Add("updateSkills", updater);
            updater.CurrentValue = false;
            locked[0] = false;
            updater.OnValueChange += updater_OnValueChange;
            clear.CurrentValue = false;
            clear.OnValueChange += clear_OnValueChange;
            defau.CurrentValue = false;
            defau.OnValueChange += defau_OnValueChange;
            profile1.OnValueChange += profile1_OnValueChange;
            profile2.OnValueChange += profile2_OnValueChange;

            profile = profile1.CurrentValue ? 1 : 2;
            lvlFile = Path.Combine(dir + "\\" + "Skills-" + champ.ChampionName + "-" + Game.MapId +"-P"+profile+ ".txt");

            this.champ = champ;
            def = new DefautSequences(dir + "\\" + "Skills-DEFAULT.txt");
            maxLvl = maxlvl;
            Menu menu = m.AddSubMenu("Skill sequence: " + champ.ChampionName);




            
            sliders = new LvlSlider[maxlvl];
            skills = new SkillToLvl[maxlvl];
            for (int i = 0; i < maxLvl; i++)
            {
                sliders[i] = new LvlSlider(menu, i, this);
            }
            load(seq);
            lvlUp=new SkillLevelUp(skills, enabled)
            {
                maxTime = humanMax.CurrentValue,
                minTime = humanMin.CurrentValue
            };

        }

        void humanMax_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            if (args.NewValue < humanMin.CurrentValue)
            {
                humanMax.CurrentValue = humanMin.CurrentValue;
            }
            lvlUp.maxTime = humanMax.CurrentValue;
        }

        void humanMin_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            if (args.NewValue > humanMax.CurrentValue)
            {
                humanMin.CurrentValue=humanMax.CurrentValue;
            }
            lvlUp.minTime = humanMin.CurrentValue;
        }

        private void updater_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (locked[0])
            {
                Core.DelayAction(() => updater.CurrentValue = true, 1);

                return;
            }
            if (args.NewValue && !args.OldValue)
            {
                Core.DelayAction(() => updater.CurrentValue = true, 1);
                updater.DisplayName = "Updating...";
                def.updateSequences(locked);
                unlockButton();
            }
        }

        private void unlockButton()
        {
            if (!locked[0])
            {
                updater.CurrentValue = false;
                updater.DisplayName = "Update default sequences";
                return;
            }
            Core.DelayAction(unlockButton, 80);
        }



        private void profile1_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.NewValue)
            {
                Core.DelayAction(()=>profile1.CurrentValue=true, 1);
                return;
            }

            profile2.OnValueChange -= profile2_OnValueChange;
            profile2.CurrentValue = false;
            profile2.OnValueChange += profile2_OnValueChange;
            profile = 1;
            lvlFile = Path.Combine(dir + "\\" + "Skills-" + champ.ChampionName + "-" + Game.MapId + "-P" + profile + ".txt");
            load(se);
        }

        private void profile2_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.NewValue)
            {
                Core.DelayAction(() => profile2.CurrentValue = true, 1);
                return;
            }

            profile1.OnValueChange -= profile1_OnValueChange;
            profile1.CurrentValue = false;
            profile1.OnValueChange += profile1_OnValueChange;
            profile = 2;
            lvlFile = Path.Combine(dir + "\\" + "Skills-" + champ.ChampionName + "-" + Game.MapId + "-P" + profile + ".txt");
            load(se);
        }


        private void clear_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.NewValue) return;
            clearSeq();
            Core.DelayAction(() => { clear.CurrentValue = false; }, 200);

        }

        private void defau_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.NewValue) return;
            initSeq(def.GetDefaultSequence(champ.Hero));
            if (File.Exists(lvlFile))
            {
                File.Delete(lvlFile);
            }
            Core.DelayAction(() => { defau.CurrentValue = false; }, 200);

        }

        private void clearSeq()
        {
            sa = false;
            for (int i = 0; i < maxLvl; i++)
            {
                skills[i] = SkillToLvl.NotSet;
                sliders[i].Skill = SkillToLvl.NotSet;

            }
            sa = true;
            save();
        }

        private void initSeq(string seq)
        {

            for (int i = 0; i < maxLvl; i++)
            {
                skills[i] = SkillToLvl.NotSet;
            }
            sa = false;
            if (string.IsNullOrEmpty(seq) || seq.Split(';').Length != maxLvl)
            {
                for (int i = 0; i < maxLvl; i++)
                {
                    sliders[i].Skill = SkillToLvl.NotSet;
                }
            }
            else
            {
                try
                {

                    string[] s = seq.Split(';');

                    for (int i = 0; i < maxLvl; i++)
                    {
                        sliders[i].Skill = (SkillToLvl)Enum.Parse(typeof(SkillToLvl), s[i], true);
                    }
                    for (int i = 0; i < maxLvl; i++)
                    {
                        skills[i] = (SkillToLvl)Enum.Parse(typeof(SkillToLvl), s[i], true);
                    }
                }
                catch
                {
                    Chat.Print("Skill upgrader: couldn't load skill sequence, set it manually.");
                    for (int i = 0; i < maxLvl; i++)
                    {
                        sliders[i].Skill = SkillToLvl.NotSet;
                    }
                }

            }



            sa = true;

        }

        private void load(string seq)
        {
            initSeq(File.Exists(lvlFile) ? File.ReadAllText(lvlFile) : (string.IsNullOrEmpty(seq) ? def.GetDefaultSequence(champ.Hero) : seq));
        }
        private void save()
        {
            string s = string.Empty;
            for (int i = 0; i < maxLvl; i++)
            {
                s += ";" + skills[i];
            }
            File.WriteAllText(lvlFile, s.Substring(1));
        }

        private int CountSkillLvl(SkillToLvl s, int level)
        {
            int lvl = 0;
            for (int i = 0; i < level; i++)
            {
                if (skills[i] == s)
                    lvl++;
            }
            return lvl;
        }

        public bool canLvl(SkillToLvl s, int level)
        {
            int q = CountSkillLvl(s, 18);
            if (s == SkillToLvl.R)
            {
                if (q >= 3) return false;
                if (level < 5) return CountSkillLvl(s, 5) < 0;
                if (level < 10) return CountSkillLvl(s, 10) < 1;
                if (level < 15) return CountSkillLvl(s, 15) < 2;
                return q < 3;
            }

            if (q >= 5) return false;
            if (level < 2) return CountSkillLvl(s, 2) < 1;
            if (level < 4) return CountSkillLvl(s, 4) < 2;
            if (level < 6) return CountSkillLvl(s, 6) < 3;
            if (level < 8) return CountSkillLvl(s, 8) < 4;
            return q < 5;
        }

        public void SetSkill(int level, SkillToLvl skill)
        {
            skills[level] = skill;
            if (sa)
                save();
        }
    }
}
