namespace SCommon.MenuUtils
{
    public enum Language
    {
        English = 0,
        Turkish = 1,
        Korean = 2
    }

    public static class MenuLanguage
    {
        public static string GetDisplayName(string str)
        {
            return str;
        }
    }
}