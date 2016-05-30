using AutoBuddy.Humanizers;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace AutoBuddy.Utilities.AutoLvl
{
    internal class SkillLevelUp
    {
        private readonly CheckBox enabled;
        private readonly SkillToLvl[] skills;
        private readonly SpellDataInst e = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E);
        private readonly SpellDataInst q = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q);
        private readonly SpellDataInst r = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R);
        private readonly SpellDataInst w = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W);
        public int minTime=0;
        public int maxTime=0;

        private int oldLvl = 1;

        public SkillLevelUp(SkillToLvl[] skills, CheckBox enabled)
        {
            this.enabled = enabled;
            enabled.OnValueChange += enabled_OnValueChange;
            this.skills = skills;
            Core.DelayAction(() => OnLvLUp(ObjectManager.Player.Level), RandGen.r.Next(minTime, maxTime));
            //Obj_AI_Base.OnLevelUp += Player_OnLevelUp;TODO waiting for devs to fix onlvlup...
            Game.OnTick += Game_OnTick;
        }

        private void Game_OnTick(System.EventArgs args)
        {
            if (AutoWalker.p.Level <= oldLvl) return;
            oldLvl = AutoWalker.p.Level;
            OnLvLUp(oldLvl);
        }

        private void enabled_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if(args.NewValue)
                OnLvLUp(ObjectManager.Player.Level, true);
        }

        private void Player_OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (sender != ObjectManager.Player) return;
            Chat.Print("lvlup");
            Core.DelayAction(() => OnLvLUp(args.Level), RandGen.r.Next(minTime, maxTime));
        }

        private void OnLvLUp(int level, bool overrid=false)
        {
            if(!enabled.CurrentValue&&!overrid)return;
            for (int z = 0; z < level; z++)
            {
                int qDesired = 0, wDesired = 0, eDesired = 0, rDesired = 0;
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    switch (skills[i])
                    {
                        case SkillToLvl.Q:
                            qDesired++;
                            break;
                        case SkillToLvl.W:
                            wDesired++;
                            break;
                        case SkillToLvl.E:
                            eDesired++;
                            break;
                        case SkillToLvl.R:
                            rDesired++;
                            break;
                    }
                }
                if (r.Level < rDesired)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                if (q.Level < qDesired)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (w.Level < wDesired)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (e.Level < eDesired)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
        }
    }
}