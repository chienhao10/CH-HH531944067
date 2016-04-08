using System.Collections.Generic;

namespace AdEvade.Languages
{
    public abstract class LanguageAddon
    {
        public Dictionary<Config.ConfigValue, string> LangDictionary { get; set; }
    }
}