using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    public delegate void FindPathAsynchResult(Waypoint result);

    /// <summary>
    /// Represents a cell in the navigation grid.
    /// </summary>
    public struct GridCell
    {
        /// <summary>
        /// The x-coordinate of the cell.
        /// </summary>
        public float x;

        /// <summary>
        /// The y-coordinate of the cell.
        /// </summary>
        public float y;

        /// <summary>
        /// The width of the cell.
        /// </summary>
        public float width;

        /// <summary>
        /// The height of the cell.
        /// </summary>
        public float height;

        /// <summary>
        /// Converts the grid cell to a rectangle.
        /// </summary>
        /// <returns>A rectangle representing the grid cell.</returns>
        public Rect ToRect()
        {
            Rect rect = new Rect(x, y, width, height);
            return rect;
        }

        /// <summary>
        /// Converts the grid cell coordinates to an Vec3
        /// </summary>
        /// <returns></returns>
        public Vec3 ToVec3()
        {
            return new Vec3(x, y);
        }
    }

    /// <summary>
    /// Represents an obstacle in the navigation grid.
    /// </summary>
    public struct Obstacle
    {
        /// <summary>
        /// The column index of the cell where the obstacle is located.
        /// </summary>
        public int cellX;

        /// <summary>
        /// The row index of the cell where the obstacle is located.
        /// </summary>
        public int cellY;
    }

    public class NavMesh : GameElement
    {
        /// <summary>
        /// The location of the navigation mesh in 3D space.
        /// </summary>
        public Vec3 Location { get; set; }

        /// <summary>
        /// The width of each cell in the grid.
        /// </summary>
        public float CellWidth { get; set; }

        /// <summary>
        /// The height of each cell in the grid.
        /// </summary>
        public float CellHeight { get; set; }

        /// <summary>
        /// The number of cells in the X direction.
        /// </summary>
        public int CellsX { get; set; }

        /// <summary>
        /// The number of cells in the Y direction.
        /// </summary>
        public int CellsY { get; set; }

        /// <summary>
        /// The cost of moving straight from one cell to another.
        /// </summary>
        public int StraightCost { get; set; } = 10;

        /// <summary>
        // Gets or sets the value if the path finding is use diagonal cells
        /// </summary>
        public bool UseDiagonal { get; set; } = false;

        /// <summary>
        /// Gets or sets the value if debug mode is used
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// List of grid cells in the navigation mesh.
        /// </summary>
        private List<GridCell> m_grid;

        /// <summary>
        /// List of obstacles in the navigation mesh.
        /// </summary>
        private List<Obstacle> m_obstacles;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavMesh"/> class.
        /// </summary>
        /// <param name="location">The location of the navigation mesh.</param>
        /// <param name="cellsX">The number of cells in the X direction.</param>
        /// <param name="cellsY">The number of cells in the Y direction.</param>
        /// <param name="cellSize">The size of each cell in the grid.</param>
        public NavMesh(String name, Vec3 location, int cellsX, int cellsY, float cellSize)
        {
            this.Name = name;
            this.Location = location;
            this.CellsX = cellsX;
            this.CellsY = cellsY;
            this.CellWidth = cellSize;
            this.CellHeight = cellSize;
            this.BuildGrid();
            m_obstacles = new List<Obstacle>();
        }

        /// <summary>
        /// Renders the navmesh
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            if(this.Debug)
            {
                this.DrawGrid(renderDevice);
            }
        }

        /// <summary>
        /// Adds an obstacle to the navigation mesh.
        /// </summary>
        /// <param name="column">The column index of the cell where the obstacle is located.</param>
        /// <param name="row">The row index of the cell where the obstacle is located.</param>
        public void AddObstacle(int column, int row)
        {
            Obstacle obstacle = new Obstacle();
            obstacle.cellX = column;
            obstacle.cellY = row;
            m_obstacles.Add(obstacle);
        }

        /// <summary>
        /// Checks if there is an obstacle at the specified cell coordinates.
        /// </summary>
        /// <param name="col">The column index of the cell to check.</param>
        /// <param name="row">The row index of the cell to check.</param>
        /// <returns>True if there is an obstacle at the specified cell, otherwise false.</returns>
        public bool IsObstacle(int col, int row)
        {
            foreach (var obstacle in m_obstacles)
            {
                if (obstacle.cellX == col && obstacle.cellY == row)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if there is an obstacle at the specified grid cell.
        /// </summary>
        /// <param name="cell">The grid cell to check for obstacles.</param>
        /// <returns>True if there is an obstacle at the specified grid cell, otherwise false.</returns>
        public bool HasObstacle(GridCell cell)
        {
            int cellColumn = (int)(cell.x / CellWidth);
            int cellRow = (int)(cell.y / CellHeight);

            foreach (var obstacle in m_obstacles)
            {
                if (obstacle.cellX == cellColumn && obstacle.cellY == cellRow)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Builds the grid of cells for the navigation mesh.
        /// </summary>
        public void BuildGrid()
        {
            var start = GetStartVector();
            m_grid = new List<GridCell>();

            for (int y = 0; y < CellsY; y++)
            {
                for (int x = 0; x < CellsX; x++)
                {
                    GridCell gridCell = new GridCell();
                    gridCell.x = start.X + (x * CellWidth);
                    gridCell.y = start.Y + (y * CellHeight);
                    gridCell.width = CellWidth;
                    gridCell.height = CellHeight;
                    m_grid.Add(gridCell);
                }
            }
        }

        /// <summary>
        /// Draws the grid cells of the navigation mesh.
        /// </summary>
        /// <param name="renderer">The renderer used to draw the grid cells.</param>
        public void DrawGrid(IRenderDevice renderer)
        {
            if (m_grid != null)
            {
                foreach (var gridCell in m_grid)
                {
                    if (this.HasObstacle(gridCell))
                    {
                        renderer.FillRect(gridCell.ToRect(), System.Drawing.Color.Red);
                        renderer.DrawRect(gridCell.ToRect(), System.Drawing.Color.Green, 2);
                    }
                    else
                    {
                        renderer.DrawRect(gridCell.ToRect(), System.Drawing.Color.Green, 2);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the path on the navigation mesh.
        /// </summary>
        /// <param name="renderer">The renderer used to draw the path.</param>
        /// <param name="end">The destination waypoint of the path.</param>
        public void DrawPath(IRenderDevice renderer, Waypoint end)
        {
            var waypoint = end;
            while (waypoint != null)
            {
                var cell = GetGridCell(waypoint.column, waypoint.row);
                renderer.FillRect(cell.ToRect(), System.Drawing.Color.Yellow);
                waypoint = waypoint.parent;
            }
        }

        /// <summary>
        /// Get the gird cell at the given location
        /// </summary>
        /// <param name="col">The column index</param>
        /// <param name="row">The row index</param>
        /// <returns></returns>
        public GridCell GetGridCell(int col, int row)
        {
            int index = row * CellsX + col;
            return m_grid[index];
        }

        /// <summary>
        /// Get the gridd cell from coordinats (workarround)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GridCell GetGridCellFromCoordinates(int x, int y)
        {
            foreach (var item in m_grid)
            {
                if (item.ToRect().Contains(x + (item.width / 2), y + (item.height / 2)))
                {
                    return item;
                }
            }
            GridCell gridCell = new GridCell();
            gridCell.x = -1;
            return gridCell;
        }

        public int[] GetIndicesFromGridCell(GridCell cell)
        {
            int column = (int)(cell.x / CellWidth);
            int row = (int)(cell.y / CellHeight);
            return new int[] { column, row };
        }

        /// <summary>
        /// Checks if at the given column and row index is on the grid
        /// </summary>
        /// <param name="col">The column index</param>
        /// <param name="row">The row index</param>
        /// <returns></returns>
        private bool HasGridCell(int col, int row)
        {
            if (col < 0 || row < 0)
            {
                return false;
            }

            int index = GetGridCellIndex(col, row);
            if (index >= 0 && index < m_grid.Count)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the given waypoint in on the grid
        /// </summary>
        /// <param name="waypoint">The waypoint to check</param>
        /// <returns></returns>
        private bool HasGridCell(Waypoint waypoint)
        {
            return this.HasGridCell(waypoint.column, waypoint.row);
        }

        /// <summary>
        /// Returns the cell index at the given indicies
        /// </summary>
        /// <param name="col">Column index</param>
        /// <param name="row">Row index</param>
        /// <returns></returns>
        private int GetGridCellIndex(int col, int row)
        {
            return row * CellsX + col;
        }

        /// <summary>
        /// Calculates the huristic costs for the destination
        /// </summary>
        /// <param name="col">destination column index</param>
        /// <param name="row">destination row index</param>
        /// <param name="eCol">target column index</param>
        /// <param name="eRow">target row index</param>
        /// <returns></returns>
        private int CalculateHCost(int col, int row, int eCol, int eRow)
        {
            var x = System.Math.Abs(col - eCol);
            var y = System.Math.Abs(row - eRow);
            var cost = this.StraightCost * (x + y);
            return cost;
        }

        /// <summary>
        /// Calculates the huristic cost for the waypoint
        /// </summary>
        /// <param name="waypoint">the destination waypoint</param>
        /// <param name="eCol">target column index</param>
        /// <param name="eRow">target row index</param>
        /// <returns></returns>
        private int CalculateHCost(Waypoint waypoint, int eCol, int eRow)
        {
            return CalculateHCost(waypoint.column, waypoint.row, eCol, eRow);
        }

        /// <summary>
        /// Checks if the end is within the list
        /// </summary>
        /// <param name="waypoints">list of the waypoints</param>
        /// <param name="eCol">target column index</param>
        /// <param name="rRow">target row index</param>
        /// <returns></returns>
        private bool ContainsEnd(List<Waypoint> waypoints, int eCol, int rRow)
        {
            foreach (var item in waypoints)
            {
                if (item.column == eCol && item.row == rRow)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the end from the given dictonary
        /// </summary>
        /// <param name="waypoints">the waypoints dictionary</param>
        /// <param name="eCol">target column</param>
        /// <param name="eRwo">target row</param>
        /// <returns></returns>
        private Waypoint GetEnd(Dictionary<int, Waypoint> waypoints, int eCol, int eRwo)
        {
            foreach (var item in waypoints.Values)
            {
                if (item.column == eCol && item.row == eRwo)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the end from the given list
        /// </summary>
        /// <param name="waypoints">List of waypoints</param>
        /// <param name="eCol">target column</param>
        /// <param name="eRwo">target row</param>
        /// <returns></returns>
        private Waypoint GetEnd(List<Waypoint> waypoints, int eCol, int eRwo)
        {
            foreach (var item in waypoints)
            {
                if (item.column == eCol && item.row == eRwo)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a path from the starting cell to the ending cell on the navigation mesh.
        /// </summary>
        /// <param name="sCol">The column index of the starting cell.</param>
        /// <param name="sRow">The row index of the starting cell.</param>
        /// <param name="eCol">The column index of the ending cell.</param>
        /// <param name="eRow">The row index of the ending cell.</param>
        /// <returns>The destination waypoint of the path if found, otherwise null.</returns>
        public void FindPathAsync(int sCol, int sRow, int eCol, int eRow, FindPathAsynchResult callback)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                var waypoint = FindPath(sCol, sRow, eCol, eRow);
                e.Result = waypoint;
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                var result = (Waypoint)e.Result;
                callback(result);
            };
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Finds a path from the starting cell to the ending cell on the navigation mesh.
        /// </summary>
        /// <param name="sCol">The column index of the starting cell.</param>
        /// <param name="sRow">The row index of the starting cell.</param>
        /// <param name="eCol">The column index of the ending cell.</param>
        /// <param name="eRow">The row index of the ending cell.</param>
        /// <returns>The destination waypoint of the path if found, otherwise null.</returns>
        public Waypoint FindPath(int sCol, int sRow, int eCol, int eRow)
        {
            var closedList = new Dictionary<int, Waypoint>();
            var openList = new Dictionary<int, Waypoint>();

            Waypoint start = new Waypoint();
            start.column = sCol;
            start.row = sRow;
            start.fCost = 0;
            start.hCost = 0;
            start.gCost = 0;
            start.parent = null;

            this.Navigate(start, eCol, eRow, ref closedList, ref openList);
            var end = GetEnd(closedList, eCol, eRow);
            return end;
        }

        /// <summary>
        /// Finds a path from the starting cell to the ending cell on the navigation mesh.
        /// </summary>
        /// <param name="waypoint">The current waypoint.</param>
        /// <param name="eCol">The column index of the ending cell.</param>
        /// <param name="eRow">The row index of the ending cell.</param>
        /// <param name="closedList">The list of closed waypoints.</param>
        /// <param name="openList">The list of open waypoints.</param>
        private void Navigate(Waypoint waypoint, int eCol, int eRow, ref Dictionary<int, Waypoint> closedList, ref Dictionary<int, Waypoint> openList)
        {
            var neighbors = new List<Waypoint>();
            int col = waypoint.column;
            int row = waypoint.row;
            int index = this.GetGridCellIndex(col, row);
            var parent = waypoint;

            // 1. Define the start for this iteration
            closedList.Add(index, waypoint);
            openList.Remove(index);

            // 2. Define the neighbors for this iteration
            var east = new Waypoint();
            east.column = col + 1;
            east.row = row;
            east.parent = parent;
            east.hCost = this.CalculateHCost(east, eCol, eRow);
            east.gCost = 10;
            east.fCost = east.hCost + east.gCost;
            neighbors.Add(east);

            var south = new Waypoint();
            south.column = col;
            south.row = row + 1;
            south.parent = parent;
            south.hCost = this.CalculateHCost(south, eCol, eRow);
            south.gCost = 10;
            south.fCost = south.hCost + south.gCost;
            neighbors.Add(south);

            var west = new Waypoint();
            west.column = col - 1;
            west.row = row;
            west.parent = parent;
            west.hCost = this.CalculateHCost(west, eCol, eRow);
            west.gCost = 10;
            west.fCost = west.hCost + west.gCost;
            neighbors.Add(west);

            var north = new Waypoint();
            north.column = col;
            north.row = row - 1;
            north.parent = parent;
            north.hCost = this.CalculateHCost(north, eCol, eRow);
            north.gCost = 10;
            north.fCost = north.hCost + north.gCost;
            neighbors.Add(north);

            if (UseDiagonal)
            {
                var southEast = new Waypoint();
                southEast.column = col + 1;
                southEast.row = row + 1;
                southEast.parent = parent;
                southEast.hCost = this.CalculateHCost(southEast, eCol, eRow);
                southEast.gCost = 14;
                southEast.fCost = southEast.hCost + southEast.gCost;
                neighbors.Add(southEast);

                var southWest = new Waypoint();
                southWest.column = col - 1;
                southWest.row = row + 1;
                southWest.parent = parent;
                southWest.hCost = this.CalculateHCost(southWest, eCol, eRow);
                southWest.gCost = 14;
                southWest.fCost = southWest.hCost + southWest.gCost;
                neighbors.Add(southWest);

                var northWest = new Waypoint();
                northWest.column = col - 1;
                northWest.row = row - 1;
                northWest.parent = parent;
                northWest.hCost = this.CalculateHCost(northWest, eCol, eRow);
                northWest.gCost = 14;
                northWest.fCost = northWest.hCost + northWest.gCost;
                neighbors.Add(northWest);

                var northEast = new Waypoint();
                northEast.column = col + 1;
                northEast.row = row - 1;
                northEast.parent = parent;
                northEast.hCost = this.CalculateHCost(northEast, eCol, eRow);
                northEast.gCost = 14;
                northEast.fCost = northEast.hCost + northEast.gCost;
                neighbors.Add(northEast);
            }

            // 3. Handle the neighbors for this iteration
            var filed = neighbors[0];
            foreach (var nb in neighbors)
            {
                var nbIndex = GetGridCellIndex(nb.column, nb.row);
                // Fehler mit -1 colum index---
                if (!closedList.ContainsKey(nbIndex) && !openList.ContainsKey(nbIndex) && HasGridCell(nb))
                {
                    if (!IsObstacle(nb.column, nb.row))
                    {
                        openList.Add(nbIndex, nb);
                    }
                }
                else if (openList.ContainsKey(nbIndex))
                {
                    openList[nbIndex].CopyValues(nb);
                }
            }

            //4. Find the next cell
            var containsEnd = ContainsEnd(neighbors, eCol, eRow);
            if (openList.Count > 0 && !containsEnd)
            {
                Waypoint next = null;
                foreach (var item in openList.Values)
                {
                    if (next == null)
                    {
                        next = item;
                    }
                    else if (item.fCost < next.fCost)
                    {
                        next = item;
                    }
                }
                this.Navigate(next, eCol, eRow, ref closedList, ref openList);
            }
            else if (containsEnd)
            {
                var end = GetEnd(neighbors, eCol, eRow);
                var endIndex = GetGridCellIndex(end.column, end.row);
                if (!closedList.ContainsKey(endIndex))
                {
                    closedList[endIndex] = end;
                }
                else
                {
                    closedList[endIndex].CopyValues(end);
                }
                return;
            }
        }

        /// <summary>
        /// Calculates the starting position of the navigation mesh.
        /// </summary>
        /// <returns>The starting position of the navigation mesh.</returns>
        private Vec3 GetStartVector()
        {
            float totalWidth = CellsX * CellWidth;
            float totalHeight = CellsY * CellHeight;
            float startX = Location.X - (float)totalWidth / 2;
            float startY = Location.Y - (float)totalHeight / 2;

            return Location;
        }

        public static List<Waypoint> ToPath(Waypoint waypoint)
        {
            var list = new List<Waypoint>();
            var wp = waypoint.parent;
            while (wp != null)
            {
                list.Add(wp);
                wp = wp.parent;
            }
            list.Reverse();

            return list;
        }
    }
}
