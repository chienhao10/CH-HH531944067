using VayneHunter_Reborn.Modules.ModuleHelpers;

namespace VayneHunter_Reborn.Modules
{
    interface IModule
    {
        void OnLoad();

        bool ShouldGetExecuted();

        ModuleType GetModuleType();

        void OnExecute();
    }
}
