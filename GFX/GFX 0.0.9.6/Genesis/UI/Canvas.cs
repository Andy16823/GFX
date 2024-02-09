using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    public class Canvas
    {
        public String Name { get; set; }
        public List<Entity> Entities { get; set; }
        public Vec3 Location { get; set; }
        public Vec3 Size { get; set; }
        public bool Enabled { get; set; } = true;
        private Game game;

        /// <summary>
        /// Creates a new canvas instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public Canvas(String name, Vec3 location, Vec3 size)
        {
            Name = name;
            Entities = new List<Entity>();
            Location = location;
            Size = size;
        }

        /// <summary>
        /// Adds a entity to the canvas
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        /// <summary>
        /// Initial the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        public void OnInit(Game game, Scene scene)
        {
            this.game = game;
            foreach (var item in Entities)
            {
                item.OnInit(game, scene, this);
            }
        }

        /// <summary>
        /// Update the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        public void OnUpdate(Game game, Scene scene)
        {
            if(this.Enabled)
            {
                foreach (var item in Entities)
                {
                    if(item.Enabled)
                    {
                        item.OnUpdate(game, scene, this);
                    }
                }
            }
        }

        /// <summary>
        /// Renders the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        /// <param name="scene"></param>
        public void OnRender(Game game, IRenderDevice renderDevice, Scene scene)
        {
            if(this.Enabled)
            {
                foreach (var item in Entities)
                {
                    if (item.Enabled)
                    {
                        item.OnRender(game, renderDevice, scene, this);
                    }
                }
            }
        }

        /// <summary>
        /// Dispose the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        public void OnDispose(Game game, Scene scene)
        {
            foreach (var item in Entities)
            {
                item.OnDispose(game, scene, this);
            }
        }

        /// <summary>
        /// Return a entitiy wich equals to the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Entity GetEntity(String name)
        {
            foreach (var entity in Entities)
            {
                if(entity.Name.Equals(name))
                {
                    return entity;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the bounds from the canvas
        /// </summary>
        /// <returns></returns>
        public Rect GetBounds()
        {
            return new Rect(Location.X, Location.Y, Size.X, Size.Y);
        }

        /// <summary>
        /// Returns the screen bounds from the canvas
        /// </summary>
        /// <returns></returns>
        public Rect GetScreenBounds()
        {
            Vec3 offset = game.GetSceneCord(Location);
            //Vec3 offset = 
            return new Rect(offset.X, offset.Y, Size.X, Size.Y);
        }
    }
}
