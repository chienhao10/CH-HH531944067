using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace ScaryKalista
{
    class WallJump
    {
        public static List<Vector3[]> JumpSpots = new List<Vector3[]>();
        private static float _lastMoveClick;
        private static float _lastDistance;
        private static float _currentDistance;
        private static bool _jumped;

        public static Vector3[] GetJumpSpot()
        {
            return JumpSpots
                .Where(x => Player.Instance.Distance(x[0]) <= 150)
                .OrderBy(x => Player.Instance.Distance(x[0]))
                .FirstOrDefault();
        }

        public static void JumpWall()
        {
            var spot = GetJumpSpot();
            if (Spells.Q.IsReady()
                && spot != null
                && Environment.TickCount - _lastMoveClick > 100)
            {
                if (Player.Instance.Distance(spot[0]) <= 4)
                {
                    Spells.Q.Cast(spot[1]);
                    Player.IssueOrder(GameObjectOrder.MoveTo, spot[1]);
                    _lastMoveClick = Environment.TickCount;
                    _jumped = true;
                }
                else if (Player.Instance.Distance(spot[0]) > 4
                    && Player.Instance.Distance(spot[0]) < 60)
                {
                    _lastDistance = _currentDistance;
                    _currentDistance = Player.Instance.Distance(spot[0]);
                    if (_lastDistance == _currentDistance)
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, spot[1]);
                        _lastMoveClick = Environment.TickCount;
                    }
                }
                else
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, spot[0]);
                    _lastMoveClick = Environment.TickCount;
                }
            }
        }

        public static void InitSpots()
        {
            //blue side wolves - left wall (top)
            JumpSpots.Add(new[] { new Vector3(2848, 6942, 53), new Vector3(3058, 6960, 52) });
            JumpSpots.Add(new[] { new Vector3(3064, 6962, 52), new Vector3(2809, 6936, 53) });

            //blue side wolves - left wall (middle)
            JumpSpots.Add(new[] { new Vector3(2774, 6558, 57), new Vector3(3072, 6607, 51) });
            JumpSpots.Add(new[] { new Vector3(3074, 6608, 51), new Vector3(2755, 6523, 57) });

            //blue side wolves - left wall (bottom)
            JumpSpots.Add(new[] { new Vector3(3024, 6108, 57), new Vector3(3195, 6307, 52) });
            JumpSpots.Add(new[] { new Vector3(3200, 6243, 52), new Vector3(3022, 6111, 57) });

            //red side wolves - right wall (top)
            JumpSpots.Add(new[] { new Vector3(11772, 8856, 50), new Vector3(11513, 8762, 65) });
            JumpSpots.Add(new[] { new Vector3(11572, 8706, 64), new Vector3(11817, 8903, 50) });

            //red side wolves - right wall (middle)
            JumpSpots.Add(new[] { new Vector3(11772, 8206, 55), new Vector3(12095, 8281, 52) });
            JumpSpots.Add(new[] { new Vector3(12072, 8256, 52), new Vector3(11755, 8206, 55) });

            //red side wolves - right wall (bottom)
            JumpSpots.Add(new[] { new Vector3(11772, 7906, 52), new Vector3(12110, 7980, 53) });
            JumpSpots.Add(new[] { new Vector3(12072, 7906, 53), new Vector3(11767, 7900, 52) });

            //bottom bush wall (top)
            JumpSpots.Add(new[] { new Vector3(11410, 5526, 23), new Vector3(11647, 5452, 54) });
            JumpSpots.Add(new[] { new Vector3(11646, 5452, 54), new Vector3(11354, 5511, 8) });

            //bottom bush wall (middle)
            JumpSpots.Add(new[] { new Vector3(11722, 5058, 52), new Vector3(11345, 4813, -71) });
            JumpSpots.Add(new[] { new Vector3(11428, 4984, -71), new Vector3(11725, 5120, 52) });

            //bot bush wall (bottom)
            JumpSpots.Add(new[] { new Vector3(11772, 4608, -71), new Vector3(11960, 4802, 51) });
            JumpSpots.Add(new[] { new Vector3(11922, 4758, 51), new Vector3(11697, 4614, -71) });

            //top bush wall (top)
            JumpSpots.Add(new[] { new Vector3(3074, 10056, 54), new Vector3(3437, 10186, -66) });
            JumpSpots.Add(new[] { new Vector3(3324, 10206, -65), new Vector3(2964, 10012, 54) });

            //top bush wall (middle)
            JumpSpots.Add(new[] { new Vector3(3474, 9856, -65), new Vector3(3104, 9701, 52) });
            JumpSpots.Add(new[] { new Vector3(3226, 9752, 52), new Vector3(3519, 9833, -65) });

            //top bush wall (bottom)
            JumpSpots.Add(new[] { new Vector3(3488, 9414, 13), new Vector3(3224, 9440, 51) });
            JumpSpots.Add(new[] { new Vector3(3226, 9438, 51), new Vector3(3478, 9422, 16) });

            //mid wall - top side (top)
            JumpSpots.Add(new[] { new Vector3(6524, 8856, -71), new Vector3(6685, 9116, 49) });
            JumpSpots.Add(new[] { new Vector3(6664, 9002, 43), new Vector3(6484, 8804, -71) });

            //mid wall - top side (middle)
            JumpSpots.Add(new[] { new Vector3(6874, 8606, -69), new Vector3(7095, 8727, 52) });
            JumpSpots.Add(new[] { new Vector3(7074, 8706, 52), new Vector3(6857, 8517, -71) });

            //mid wall - top side (bottom)
            JumpSpots.Add(new[] { new Vector3(7174, 8256, -33), new Vector3(7456, 8539, 53) });
            JumpSpots.Add(new[] { new Vector3(7422, 8406, 53), new Vector3(7100, 8159, -24) });

            //mid wall - bot side (top)
            JumpSpots.Add(new[] { new Vector3(7658, 6512, 5), new Vector3(7378, 6298, 52) });
            JumpSpots.Add(new[] { new Vector3(7470, 6260, 52), new Vector3(7714, 6544, -1) });

            //mid wall - bot side (middle)
            JumpSpots.Add(new[] { new Vector3(8034, 6198, -71), new Vector3(7813, 5938, 52) });
            JumpSpots.Add(new[] { new Vector3(7898, 6004, 51), new Vector3(8139, 6210, -71) });

            //mid wall - bot side (bottom)
            JumpSpots.Add(new[] { new Vector3(8222, 5808, 32), new Vector3(8412, 6081, -71) });
            JumpSpots.Add(new[] { new Vector3(8344, 6022, -71), new Vector3(8194, 5742, 42) });

            //baron wall
            JumpSpots.Add(new[] { new Vector3(5774, 10656, 55), new Vector3(5355, 10657, -71) });
            JumpSpots.Add(new[] { new Vector3(5474, 10656, -71), new Vector3(5812, 10832, 55) });

            //baron entrance wall (left)
            JumpSpots.Add(new[] { new Vector3(4474, 10406, -71), new Vector3(4292, 10199, -71) });
            JumpSpots.Add(new[] { new Vector3(4292, 10270, -71), new Vector3(4480, 10437, -71) });

            //baron entrance wall (right)
            JumpSpots.Add(new[] { new Vector3(5074, 10006, -71), new Vector3(4993, 9706, -70) });
            JumpSpots.Add(new[] { new Vector3(5000, 9754, -71), new Vector3(5083, 9998, -71) });

            //dragon wall
            JumpSpots.Add(new[] { new Vector3(9322, 4358, -71), new Vector3(8971, 4284, 52) });
            JumpSpots.Add(new[] { new Vector3(9072, 4208, 53), new Vector3(9378, 4431, -71) });

            //dragon entrance wall (left)
            JumpSpots.Add(new[] { new Vector3(9812, 4918, -71), new Vector3(9803, 5249, -68) });
            JumpSpots.Add(new[] { new Vector3(9822, 5158, -71), new Vector3(9751, 4884, -71) });

            //dragon entrance wall (right)
            JumpSpots.Add(new[] { new Vector3(10422, 4458, -71), new Vector3(10643, 4641, -68) });
            JumpSpots.Add(new[] { new Vector3(10622, 4558, -71), new Vector3(10375, 4441, -71) });

            //top golllems wall
            JumpSpots.Add(new[] { new Vector3(6524, 12006, 56), new Vector3(6553, 11666, 53) });
            JumpSpots.Add(new[] { new Vector3(6574, 11706, 53), new Vector3(6543, 12054, 56) });

            //bot gollems wall
            JumpSpots.Add(new[] { new Vector3(8250, 2894, 51), new Vector3(8213, 3326, 51) });
            JumpSpots.Add(new[] { new Vector3(8222, 3158, 51), new Vector3(8282, 2741, 51) });

            //blue side bot tribush wall (left)
            JumpSpots.Add(new[] { new Vector3(9482, 2786, 49), new Vector3(9535, 3203, 55) });
            JumpSpots.Add(new[] { new Vector3(9530, 3126, 59), new Vector3(9505, 2756, 49) });

            //blue side bot tribush wall (middle)
            JumpSpots.Add(new[] { new Vector3(9772, 2758, 49), new Vector3(9862, 3111, 58) });
            JumpSpots.Add(new[] { new Vector3(9872, 3066, 58), new Vector3(9815, 2673, 49) });

            //blue side bot tribush wall (right)
            JumpSpots.Add(new[] { new Vector3(10206, 2888, 49), new Vector3(10046, 2675, 49) });
            JumpSpots.Add(new[] { new Vector3(10022, 2658, 49), new Vector3(10259, 2925, 49) });

            //red side toplane tribush wall (right)
            JumpSpots.Add(new[] { new Vector3(5274, 11806, 57), new Vector3(5363, 12185, 56) });
            JumpSpots.Add(new[] { new Vector3(5324, 12106, 56), new Vector3(5269, 11725, 57) });

            //red side toplane tribush wall (middle)
            JumpSpots.Add(new[] { new Vector3(5000, 11874, 57), new Vector3(5110, 12210, 56) });
            JumpSpots.Add(new[] { new Vector3(5072, 12146, 56), new Vector3(4993, 11836, 57) });

            //red side toplane tribush wall (left)
            JumpSpots.Add(new[] { new Vector3(4624, 12006, 57), new Vector3(4825, 12307, 56) });
            JumpSpots.Add(new[] { new Vector3(4776, 12224, 56), new Vector3(4605, 11970, 57) });

            //blue side razorbeak wall
            JumpSpots.Add(new[] { new Vector3(7372, 5858, 52), new Vector3(7115, 5524, 55) });
            JumpSpots.Add(new[] { new Vector3(7174, 5608, 58), new Vector3(7424, 5905, 52) });

            //blue side blue buff wall (right)
            JumpSpots.Add(new[] { new Vector3(3774, 7706, 52), new Vector3(3856, 7412, 51) });
            JumpSpots.Add(new[] { new Vector3(3828, 7428, 51), new Vector3(3802, 7743, 52) });

            //blue side blue buff wall (left)
            JumpSpots.Add(new[] { new Vector3(3424, 7408, 52), new Vector3(3422, 7759, 53) });
            JumpSpots.Add(new[] { new Vector3(3434, 7722, 52), new Vector3(3437, 7398, 52) });

            //blue side blue buff - right wall
            JumpSpots.Add(new[] { new Vector3(4144, 8030, 50), new Vector3(4382, 8149, 48) });
            JumpSpots.Add(new[] { new Vector3(4374, 8156, 48), new Vector3(4124, 8022, 50) });

            //blue side rock between blue buff/baron (left)
            JumpSpots.Add(new[] { new Vector3(4664, 8652, -10), new Vector3(4624, 9010, -68) });
            JumpSpots.Add(new[] { new Vector3(4662, 8896, -69), new Vector3(4672, 8519, 26) });

            //blue side rock between blue buff/baron (right)
            JumpSpots.Add(new[] { new Vector3(3774, 9206, -14), new Vector3(4074, 9322, -67) });
            JumpSpots.Add(new[] { new Vector3(4024, 9306, -68), new Vector3(3737, 9233, -8) });

            //red side blue buff wall (left)
            JumpSpots.Add(new[] { new Vector3(11022, 7208, 51), new Vector3(10904, 7521, 52) });
            JumpSpots.Add(new[] { new Vector3(11022, 7506, 52), new Vector3(11040, 7179, 51) });

            //red side blue buff wall (right)
            JumpSpots.Add(new[] { new Vector3(11440, 7208, 52), new Vector3(11449, 7517, 52) });
            JumpSpots.Add(new[] { new Vector3(11470, 7486, 52), new Vector3(11458, 7155, 52) });

            //red side rock between blue buff/dragon (left)
            JumpSpots.Add(new[] { new Vector3(10172, 6208, 16), new Vector3(10189, 5922, -71) });
            JumpSpots.Add(new[] { new Vector3(10172, 5958, -71), new Vector3(10185, 6286, 29) });

            //red side rock between blue buff/dragon (right)
            JumpSpots.Add(new[] { new Vector3(10722, 5658, -66), new Vector3(11049, 5660, -22) });
            JumpSpots.Add(new[] { new Vector3(11022, 5658, -30), new Vector3(10665, 5662, -68) });

            //blue side top tribush wall (top)
            JumpSpots.Add(new[] { new Vector3(2574, 9656, 54), new Vector3(2800, 9596, 52) });
            JumpSpots.Add(new[] { new Vector3(2774, 9656, 53), new Vector3(2537, 9674, 54) });

            //blue side top tribush wall (bottom)
            JumpSpots.Add(new[] { new Vector3(2874, 9306, 51), new Vector3(2500, 9262, 52) });
            JumpSpots.Add(new[] { new Vector3(2598, 9272, 52), new Vector3(2884, 9291, 51) });

            //blue side wolves - right wall (bottom)
            JumpSpots.Add(new[] { new Vector3(4624, 5858, 51), new Vector3(4772, 5636, 50) });
            JumpSpots.Add(new[] { new Vector3(4774, 5658, 50), new Vector3(4644, 5876, 51) });

            //blue side wolves - right wall (top)
            JumpSpots.Add(new[] { new Vector3(4924, 6158, 52), new Vector3(4869, 6452, 51) });
            JumpSpots.Add(new[] { new Vector3(4874, 6408, 51), new Vector3(4938, 6062, 51) });

            //blue razorbeak - left wall
            JumpSpots.Add(new[] { new Vector3(6174, 5308, 49), new Vector3(5998, 5536, 52) });
            JumpSpots.Add(new[] { new Vector3(6024, 5508, 52), new Vector3(6199, 5286, 49) });

            //red side bottom tribush wall (bottom)
            JumpSpots.Add(new[] { new Vector3(12260, 5220, 52), new Vector3(12027, 5265, 54) });
            JumpSpots.Add(new[] { new Vector3(12122, 5208, 54), new Vector3(12327, 5243, 52) });

            //red side bottom tribush wall (top)
            JumpSpots.Add(new[] { new Vector3(11972, 5558, 54), new Vector3(12343, 5498, 53) });
            JumpSpots.Add(new[] { new Vector3(12272, 5558, 53), new Vector3(11969, 5480, 55) });

            //red side razorbeak - rightdown wall
            JumpSpots.Add(new[] { new Vector3(8672, 9606, 50), new Vector3(8831, 9384, 52) });
            JumpSpots.Add(new[] { new Vector3(8830, 9382, 52), new Vector3(8646, 9635, 50) });

            //red side wolves - left wall (top)
            JumpSpots.Add(new[] { new Vector3(10222, 9056, 50), new Vector3(10061, 9282, 52) });
            JumpSpots.Add(new[] { new Vector3(10072, 9306, 52), new Vector3(10193, 9052, 50) });

            //red side wolves - left wall (bottom)
            JumpSpots.Add(new[] { new Vector3(9972, 8506, 68), new Vector3(9856, 8831, 50) });
            JumpSpots.Add(new[] { new Vector3(9872, 8756, 50), new Vector3(9967, 8429, 65) });

            //red size razorbeak - right wall
            JumpSpots.Add(new[] { new Vector3(8072, 9806, 51), new Vector3(8369, 9807, 50) });
            JumpSpots.Add(new[] { new Vector3(8372, 9806, 50), new Vector3(8066, 9796, 51) });

            //blue side base wall (right)
            JumpSpots.Add(new[] { new Vector3(4524, 3258, 96), new Vector3(4780, 3460, 51) });
            JumpSpots.Add(new[] { new Vector3(4774, 3408, 51), new Vector3(4463, 3260, 96) });

            //blue side base wall (left)
            JumpSpots.Add(new[] { new Vector3(3074, 4558, 96), new Vector3(3182, 4917, 54) });
            JumpSpots.Add(new[] { new Vector3(3174, 4858, 54), new Vector3(3085, 4539, 96) });

            //red side base wall (right)
            JumpSpots.Add(new[] { new Vector3(11712, 10390, 91), new Vector3(11621, 10092, 52) });
            JumpSpots.Add(new[] { new Vector3(11622, 10106, 52), new Vector3(11735, 10430, 91) });

            //red base wall (left)
            JumpSpots.Add(new[] { new Vector3(10308, 11682, 91), new Vector3(9999, 11554, 52) });
            JumpSpots.Add(new[] { new Vector3(10022, 11556, 52), new Vector3(10321, 11664, 91) });
        }
    }
}
