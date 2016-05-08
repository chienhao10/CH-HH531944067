using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace EvadeA.Pathfinding
{
    public class Node
    {
        public Vector2 Point;
        public List<Node> Neightbours;

        public Node(Vector2 point)
        {
            Point = point;
            Neightbours = new List<Node>();
        }
    }
}
