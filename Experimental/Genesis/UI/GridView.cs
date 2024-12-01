using Genesis.Core;
using Genesis.Math;
using Genesis.UI.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    public class GridView : Widget
    {
        public List<IItem> Items { get; set; }
        public Font Font { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public float CellSize { get; set; }
        public float Spacing { get; set; }
        public Color BackgroundColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color BorderColor { get; set; } = Color.FromArgb(0, 0, 0);
        public float Padding { get; set; } = 5.0f;

        public GridView(String name, Vec3 location, int columns, int rows, float cellSize, float spacing, WidgetAnchor anchor)
        {
            this.Name = name;
            this.Location = location;
            this.Columns = columns;
            this.Rows = rows;
            this.CellSize = cellSize;
            this.Spacing = spacing;
            this.Anchor = anchor;
            this.Size = CalculateSize(columns, rows, cellSize, spacing);
            this.Items = new List<IItem>();
        }

        public static Vec3 CalculateSize(int columns, int rows, float cellSize, float spacing) 
        {
            float spacingX = spacing * (columns - 1);
            float spacingY = spacing * (rows - 1);
            float cellsX = cellSize * columns;
            float cellsY = cellSize * rows;

            float width = cellsX + spacingX;
            float height = cellsY + spacingY;

            return new Vec3(width, height);
        }



        public override void OnRender(Game game, Graphics.IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            var bounds = TransformBounds(new Rect(GetRelativePos(canvas), this.Size), this.Anchor);

            var itemStartX = bounds.X - (bounds.Width / 2) + (CellSize / 2);
            var itemStartY = bounds.Y - (bounds.Height  / 2) + (CellSize /2);

            //renderDevice.FillRect(bounds, Color.Red);

            for (int y = 0; y < this.Rows; y++)
            {
                for (int x = 0; x < this.Columns; x++)
                {
                    float cellX = itemStartX + (x * CellSize);
                    if(x > 0)
                    {
                        cellX += (x * Spacing);
                    }
                    float cellY = itemStartY + (y * CellSize);
                    if(y > 0)
                    {
                        cellY += (y * Spacing);
                    }
                    var cellBounds = new Rect(cellX, cellY, CellSize, CellSize);

                    
                    renderDevice.FillRect(cellBounds, this.BackgroundColor);

                    var itemIndex = (y * Rows) + x;
                    if(Items.Count > itemIndex)
                    {
                        var item = Items[itemIndex];
                        item.OnRender(renderDevice, this, cellBounds);
                    }

                    renderDevice.DrawRect(cellBounds, this.BorderColor, 2f);
                }
            }

        }

    }
}
