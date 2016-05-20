using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace Auto_Carry_Vayne.Features.Module.Items
{
    class Qss : IModule
    {
        public void OnLoad()
        {
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.Buff.Type == BuffType.Taunt && Manager.MenuManager.QssTaunt)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Stun && Manager.MenuManager.QssStun)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Snare && Manager.MenuManager.QssSnare)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Polymorph && Manager.MenuManager.QssPolymorph)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Blind && Manager.MenuManager.QssBlind)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Flee && Manager.MenuManager.QssFear)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Charm && Manager.MenuManager.QssCharm)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Suppression && Manager.MenuManager.QssSupression)
            {
                DoQSS();
            }
            if (args.Buff.Type == BuffType.Silence && Manager.MenuManager.QssSilence)
            {
                DoQSS();
            }
            if (args.Buff.Name == "zedulttargetmark")
            {
                UltQSS();
            }
            if (args.Buff.Name == "VladimirHemoplague")
            {
                UltQSS();
            }
            if (args.Buff.Name == "FizzMarinerDoom")
            {
                UltQSS();
            }
            if (args.Buff.Name == "MordekaiserChildrenOfTheGrave")
            {
                UltQSS();
            }
            if (args.Buff.Name == "PoppyDiplomaticImmunity")
            {
                UltQSS();
            }
        }

        private static void DoQSS()
        {
            if (Manager.SpellManager.Qss.IsOwned() && Manager.SpellManager.Qss.IsReady() && Variables._Player.CountEnemiesInRange(1800) > 0 && Manager.MenuManager.Qss)
            {
                Core.DelayAction(() => Manager.SpellManager.Qss.Cast(), Manager.MenuManager.QssDelay);
            }

            if (Manager.SpellManager.Mercurial.IsOwned() && Manager.SpellManager.Mercurial.IsReady() && Variables._Player.CountEnemiesInRange(1800) > 0 && Manager.MenuManager.Qss)
            {
                Core.DelayAction(() => Manager.SpellManager.Mercurial.Cast(), Manager.MenuManager.QssDelay);
            }
        }

        private static void UltQSS()
        {
            if (Manager.SpellManager.Qss.IsOwned() && Manager.SpellManager.Qss.IsReady() && Manager.MenuManager.Qss)
            {
                Core.DelayAction(() => Manager.SpellManager.Qss.Cast(), Manager.MenuManager.QssDelay);
            }

            if (Manager.SpellManager.Mercurial.IsOwned() && Manager.SpellManager.Mercurial.IsReady() && Manager.MenuManager.Qss)
            {
                Core.DelayAction(() => Manager.SpellManager.Mercurial.Cast(), Manager.MenuManager.QssDelay);
            }
        }

        public bool ShouldGetExecuted()
        {
            return false;
        }

        public void OnExecute() { }
    }
}
