using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Carry_Vayne.Features.Module
{
    interface IModule
    {
        void OnLoad();

        bool ShouldGetExecuted();

        ModuleType GetModuleType();

        void OnExecute();
    }

    enum ModuleType
    {
        OnUpdate, OnAfterAA, Other
    }
}
