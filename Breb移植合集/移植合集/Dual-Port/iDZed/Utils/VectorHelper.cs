// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;

namespace iDZed.Utils
{
    internal static class VectorHelper
    {
        public static Vector3[] GetVertices(AIHeroClient target, bool forZhonyas = false) //TODO Zhonyas triangular ult
        {
            var ultShadow = ShadowManager.RShadow;

            if (!ultShadow.Exists)
            {
                return new[] {Vector3.Zero, Vector3.Zero};
            }

            if (forZhonyas)
            {
                var vertex1 = ObjectManager.Player.ServerPosition.To2D() +
                              Vector2.Normalize(
                                  ObjectManager.Player.ServerPosition.To2D() +
                                  Vector2.Normalize(target.ServerPosition.To2D() - ultShadow.Position.To2D())*
                                  Zed._spells[SpellSlot.W].Range - ObjectManager.Player.ServerPosition.To2D() +
                                  Vector2.Normalize(target.ServerPosition.To2D() - ultShadow.Position.To2D())
                                      .Perpendicular()*Zed._spells[SpellSlot.W].Range).Perpendicular()*
                              Zed._spells[SpellSlot.W].Range;
                //
                var vertex2 = ObjectManager.Player.ServerPosition.To2D() +
                              Vector2.Normalize(
                                  ObjectManager.Player.ServerPosition.To2D() +
                                  Vector2.Normalize(target.ServerPosition.To2D() - ultShadow.Position.To2D())
                                      .Perpendicular()*Zed._spells[SpellSlot.W].Range -
                                  ObjectManager.Player.ServerPosition.To2D() +
                                  Vector2.Normalize(target.ServerPosition.To2D() - ultShadow.Position.To2D())*
                                  Zed._spells[SpellSlot.W].Range).Perpendicular()*Zed._spells[SpellSlot.W].Range;

                return new[] {vertex1.To3D(), vertex2.To3D()};
            }

            var vertex3 = ObjectManager.Player.ServerPosition.To2D() +
                          Vector2.Normalize(
                              target.ServerPosition.To2D() - ultShadow.ShadowObject.ServerPosition.To2D())
                              .Perpendicular()*Zed._spells[SpellSlot.W].Range;
            var vertex4 = ObjectManager.Player.ServerPosition.To2D() +
                          Vector2.Normalize(
                              ultShadow.ShadowObject.ServerPosition.To2D() - target.ServerPosition.To2D())
                              .Perpendicular()*Zed._spells[SpellSlot.W].Range;
            return new[] {vertex3.To3D(), vertex4.To3D()};
        }

        public static Vector3 GetBestPosition(AIHeroClient target, Vector3 firstPosition, Vector3 secondPosition)
        {
            if (firstPosition.IsWall() && !secondPosition.IsWall() &&
                secondPosition.Distance(target.ServerPosition) < firstPosition.Distance(target.ServerPosition))
                // if firstposition is a wall and second position isn't
            {
                return secondPosition; //return second position
            }
            if (secondPosition.IsWall() && !firstPosition.IsWall() &&
                firstPosition.Distance(target.ServerPosition) < secondPosition.Distance(target.ServerPosition))
                // if secondPosition is a wall and first position isn't
            {
                return firstPosition; // return first position
            }

            return firstPosition;
        }
    }
}