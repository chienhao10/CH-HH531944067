using System;
using System.Reflection;
using AutoBuddy.Utilities.AutoLvl;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AutoBuddy.Utilities
{
    internal class LvlSlider
    {
        private readonly CustomLvlSeq cus;
        private readonly string[] skills;
        private readonly Slider s;
        private readonly int level;
        public SkillToLvl Skill {
            get { return (SkillToLvl) (s.CurrentValue==5?0:s.CurrentValue); }
            set
            {
                
                s.CurrentValue = (int)value;
            }
        }
        public LvlSlider(Menu menu, int level, CustomLvlSeq cus)
        {
            this.level = level;
            s = new Slider(" ", 0, 0, 5);
            this.cus = cus;
            level += 1;
            skills =new string[]
            {
                "Level "+level+": Not set", "Level "+level+": Q", "Level "+level+": W", "Level "+level+": E", "Level "+level+": R", "Level "+level+": Not set"
            };
            s.DisplayName = skills[s.CurrentValue];
            s.OnValueChange += s_OnValueChange;

            menu.Add(level + AutoWalker.p.ChampionName+"d", s);
        }

        private void s_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {

            if (args.NewValue!=5&&args.NewValue!=0&&!cus.canLvl((SkillToLvl)args.NewValue, level))
            {

                s.CurrentValue = 0;
                

            }
            else
            {
                cus.SetSkill(level, (SkillToLvl)args.NewValue);
                sender.DisplayName = skills[args.NewValue];
            }

                

            

        }
    }
}
