namespace KappAzir.Modes
{
    using EloBuddy;

    internal class Flee : ModeManager
    {
        public static void Execute()
        {
            Jumper.jump(Game.CursorPos, Game.CursorPos);
        }
    }
}