namespace SkyLv_Taric
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class FountainMoves
    {
        #region #GET

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }
        #endregion

        private static int FountainMove;

        static FountainMoves()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            FountainAutoMoves();
        }

        public static void FountainAutoMoves()
        {
            if (ObjectManager.Player.InFountain() && Utils.GameTimeTickCount - FountainMove >= 20000)
            {
                #region SummonersRift

                if (Game.MapId == GameMapId.SummonersRift && getCheckBoxItem(SkyLv_Taric.Misc, "Taric.AutoMoveFountainMovePosSummonersRift"))
                {
                    if (Player.Team == GameObjectTeam.Order)
                    {
                        switch (getBoxItem(SkyLv_Taric.Misc, "Taric.FountainMovePosSummonersRift"))
                        {
                            case 0:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(834.00f, 1300.00f, 105.60f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                            case 1:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(526.00f, 1352.00f, 103.02f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                            case 2:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(1370.00f, 538.00f, 99.85f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }

                        }
                    }

                    if (Player.Team == GameObjectTeam.Chaos)
                    {
                        switch (getBoxItem(SkyLv_Taric.Misc, "Taric.FountainMovePosSummonersRift"))
                        {
                            case 0:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(13886.00f, 13602.00f, 119.23f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                            case 1:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(13408.00f, 14294.00f, 126.02f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                            case 2:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(14172.00f, 13384.00f, 91.65f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                        }
                    }
                }

                #endregion

                #region TwistedTreeline

                if (Game.MapId == GameMapId.TwistedTreeline && getCheckBoxItem(SkyLv_Taric.Misc, "Taric.AutoMoveFountainMovePosTwistedTreeline"))
                {
                    if (Player.Team == GameObjectTeam.Order)
                    {
                        switch (getBoxItem(SkyLv_Taric.Misc, "Taric.FountainMovePosTwistedTreeline"))
                        {
                            case 0:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(2120.00f, 8943.00f, 17.44f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                            case 1:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(1018.00f, 6801.00f, 159.32f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                        }
                    }

                    if (Player.Team == GameObjectTeam.Chaos)
                    {
                        switch (getBoxItem(SkyLv_Taric.Misc, "Taric.FountainMovePosTwistedTreeline"))
                        {
                            case 0:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(13766.00f, 9147.00f, 14.05f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                            case 1:
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(14468.00f, 6823.00f, 158.93f));
                                    FountainMove = Utils.GameTimeTickCount;
                                    break;
                                }
                        }
                    }
                }

                #endregion

                #region HowlingAbyss

                if (Game.MapId == GameMapId.HowlingAbyss && getCheckBoxItem(SkyLv_Taric.Misc, "Taric.AutoMoveFountainMovePosHowlingAbyss"))
                {
                    if (Player.Team == GameObjectTeam.Order)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(1613.00f, 2423.00f, -177.89f));
                        FountainMove = Utils.GameTimeTickCount;
                        return;
                    }

                    if (Player.Team == GameObjectTeam.Chaos)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(10715.00f, 11129.00f, -177.89f));
                        FountainMove = Utils.GameTimeTickCount;
                        return;
                    }
                }

                #endregion
            }
        }
    }
}
