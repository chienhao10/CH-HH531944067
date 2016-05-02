namespace KappAIO
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK.Events;

    internal class KappAIO
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            KappaUtility.Load.Execute();

            switch (Player.Instance.Hero)
            {
                case Champion.AurelionSol:
                    {
                        AurelionSol.Program.Execute();
                    }
                    break;

                case Champion.Brand:
                    {
                        KappaBrand.Program.Execute();
                    }
                    break;

                case Champion.Darius:
                    {
                        Darius.KappaDarius.Execute();
                    }
                    break;

                case Champion.Kalista:
                    {
                        Kalista_FlyHack.Program.Execute();
                    }
                    break;

                case Champion.Ekko:
                    {
                        KappaEkko.Load.Execute();
                    }
                    break;

                case Champion.Kindred:
                    {
                        KappaKindred.Load.Execute();
                    }
                    break;

                case Champion.Lissandra:
                    {
                        KappaLissandra.Lissandra.Execute();
                    }
                    break;

                case Champion.Azir:
                    {
                        KappAzir.Program.Execute();
                    }
                    break;

                case Champion.Karthus:
                    {
                        Karthus.Program.Execute();
                    }
                    break;

                case Champion.Khazix:
                    {
                        Khappa_Zix.Load.Load.Execute();
                    }
                    break;

                case Champion.Lulu:
                    {
                        Lulu.Program.Execute();
                    }
                    break;

                case Champion.Malzahar:
                    {
                        Malzahar.Program.Execute();
                    }
                    break;

                case Champion.Olaf:
                    {
                        Olaf.Program.Execute();
                    }
                    break;

                case Champion.Vayne:
                    {
                        Vayne_Rot_Sec.Program.Execute();
                    }
                    break;

                case Champion.MonkeyKing:
                    {
                        JustWukong.Program.Execute();
                    }
                    break;
            }
        }
    }
}