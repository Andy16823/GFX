using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    public class Waypoint
    {
        /// <summary>
        /// The column position of the waypoint.
        /// </summary>
        public int column;

        /// <summary>
        /// The row position of the waypoint.
        /// </summary>
        public int row;

        /// <summary>
        /// The parent waypoint in the navigation path.
        /// </summary>
        public Waypoint parent;

        /// <summary>
        /// The heuristic cost from this waypoint to the goal.
        /// </summary>
        public int hCost;

        /// <summary>
        /// The actual cost from the starting point to this waypoint.
        /// </summary>
        public int gCost;

        /// <summary>
        /// The total cost of the waypoint (fCost = gCost + hCost).
        /// </summary>
        public int fCost;

        /// <summary>
        /// Copies the values of another waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint to copy values from.</param>
        public void CopyValues(Waypoint waypoint)
        {
            //this.parent = waypoint.parent;
            this.hCost = waypoint.hCost;
            this.gCost = waypoint.gCost;
            this.fCost = waypoint.fCost;
        }
    }
}
