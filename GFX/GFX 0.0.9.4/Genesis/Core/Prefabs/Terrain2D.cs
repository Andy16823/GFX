using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    public class Terrain2D : GameElement
    {
        public Texture Texture { get; set; }
        public float CellsX { get; set; }
        public float CellsY { get; set; }
        public float CellSize { get; set; }

        private float texRepeatX;
        private float texRepeatY;

        public Terrain2D(String name, Vec3 location, float cellsX, float cellsY, float cellSize, Texture texture)
        {
            this.Name = name;
            this.Location = location;
            this.CellsX = cellsX;
            this.CellsY = cellsY;
            this.CellSize= cellSize;
            this.Texture = texture;
            this.Size = new Vec3(cellsX * cellSize, cellsY * cellSize);
            texRepeatX = Size.X / cellSize;
            texRepeatY = Size.Y / cellSize;  
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            if (Texture.RenderID == 0)
            {
                renderDevice.LoadTexture(Texture);
            }
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);

            float tX = Location.X + (Size.X / 2);
            float tY = Location.Y + (Size.Y / 2);

            renderDevice.ModelViewMatrix();
            renderDevice.PushMatrix();
            renderDevice.DrawTexture(Location, Size, texRepeatX, texRepeatY, Texture);
            renderDevice.PopMatrix();
        }

        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeTexture(Texture);
        }

        public Rect GetBounds()
        {
            return new Rect(Location.X, Location.Y, Size.X, Size.Y);
        }

    }
}
