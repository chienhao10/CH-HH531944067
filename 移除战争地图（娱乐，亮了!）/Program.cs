using System;

using EloBuddy;

namespace remove_fog_of_war
{
    internal static class Program
    {
        private static void Main()
        {
            Hacks.DisableTextures = true;

            ManagedTexture.OnLoad += args =>
            {
                if (args.Texture.Equals("FogOfWarOverlay.dds", StringComparison.OrdinalIgnoreCase))
                {
                    args.Process = false;
                }
            };
        }
    }
}
