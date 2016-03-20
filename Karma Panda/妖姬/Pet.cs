using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace LelBlanc
{
    internal class Pet
    {
        /// <summary>
        /// The Pet Instance
        /// </summary>
        public static GameObject LeBlancPet { get; set; }

        /// <summary>
        /// The Path of the Player
        /// </summary>
        public static Vector3 NewPath { get; set; }

        /// <summary>
        /// Delay for Movement
        /// </summary>
        public static int HumanizedDelay { get; set; }

        /// <summary>
        /// Method to Move Pet
        /// </summary>
        public static void MovePet()
        {
            if (LeBlancPet == null || NewPath.IsZero)
            {
                return;
            }

            HumanizedDelay = new Random().Next(1000);

            Core.DelayAction(() =>
            {
                Player.IssueOrder(GameObjectOrder.MovePet, RotatePosition(NewPath));
            }, HumanizedDelay);
        }

        public static Vector3 RotatePosition(Vector3 path)
        {
            var rotateAroundPoint = path.To2D().RotateAroundPoint(Player.Instance.Position.To2D(), 180);
            var finalPath = new Vector3(rotateAroundPoint,
                NavMesh.GetHeightForPosition(rotateAroundPoint.X, rotateAroundPoint.Y));
            return finalPath;
        }
    }
}