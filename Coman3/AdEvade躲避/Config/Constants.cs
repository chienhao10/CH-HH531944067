using System;
using System.IO;

namespace AdEvade.Config
{
    public static class Constants
    {
        public const string AllChampions = "AllChampions";
        public static readonly string ConfigSaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy", "AdEvade/Configs");
        public const string ConfigSaveDataFileName = "Data.xml";
        public const short ValueSeperator = (short.MaxValue - 1) / 2;
        public const float DrawChangeLength = 5; //Seconds
    }
}