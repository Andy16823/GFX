using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.UI
{
    public delegate void UIEvent(Entity entity, Game game, Scene scene, Canvas canvas);

    public class Entity
    {
        public String Name { get; set; }
        public Vec3 Location { get; set; }
        public Vec3 Size { get; set; }
        public Entity Parent { get; set; }
        public List<Entity> Children { get; set; }
        public bool Enabled { get; set; } = true;

        public event UIEvent MouseEnter;
        public event UIEvent MouseLeave;
        public event UIEvent Click;

        private bool _isHover;
        private bool _isClick;

        /// <summary>
        /// Creates a new instance for a entitiy
        /// </summary>
        public Entity()
        {
            Children = new List<Entity>();
        }

        /// <summary>
        /// Adds a children to the entity. Also sets the perent of the child entity
        /// </summary>
        /// <param name="entity"></param>
        public void AddChildren(Entity entity) { 
            Children.Add(entity);
            entity.Parent = this;
        }

        /// <summary>
        /// Initial the entity
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        /// <param name="canvas"></param>
        public virtual void OnInit(Game game, Scene scene, Canvas canvas)
        {
            foreach (var item in Children)
            {
                item.OnInit(game, scene, canvas);
            }
        }

        /// <summary>
        /// Update the entity
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        /// <param name="canvas"></param>
        public virtual void OnUpdate(Game game, Scene scene, Canvas canvas)
        {
            if(IsHover(game, scene, canvas))
            {
                if(Input.IsKeyDown(Keys.LButton))
                {
                    if(Click != null) Click(this, game, scene, canvas);
                }
                if(!_isHover)
                {
                    if (MouseEnter != null)  MouseEnter(this, game, scene, canvas);
                    _isHover = true;
                }
            }
            else
            {
                if(_isHover)
                {
                    if (MouseLeave != null)  MouseLeave(this, game, scene, canvas);  
                    _isHover = false;
                }
            }
            foreach (var item in Children)
            {
                item.OnUpdate(game, scene, canvas);
            }
        }

        /// <summary>
        /// Renders the entity
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        /// <param name="scene"></param>
        /// <param name="canvas"></param>
        public virtual void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            foreach (var item in Children)
            {
                item.OnRender(game, renderDevice, scene, canvas);
            }
        }

        /// <summary>
        /// Dispose the entity
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        /// <param name="canvas"></param>
        public virtual void OnDispose(Game game, Scene scene, Canvas canvas)
        {
            foreach (var item in Children)
            {
                item.OnDispose(game, scene, canvas);
            }
        }

        /// <summary>
        /// Get the relative postion to the screen.
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public Vec3 GetRelativePos(Canvas canvas)
        {
            Rect cRect = canvas.GetScreenBounds();
            Vec3 loc = new Vec3(cRect.X, cRect.Y);
            if(Parent != null)
            {
                Vec3 pLoc = Parent.GetRelativePos(canvas);
                loc.Add(Parent.Location);
            }
            loc.X += Location.X;
            loc.Y += Location.Y;
            return loc;
        }

        /// <summary>
        /// Returns the children with the name name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Entity GetChildren(String name)
        {
            foreach (var entity in Children)
            {
                if(entity.Name.Equals(name))
                {
                    return entity;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the bounds relative to the canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public Rect GetRelativeBounds2D(Canvas canvas)
        {
            Vec3 relativeLocation = this.GetRelativePos(canvas);
            //return new Rect(relativeLocation.X, relativeLocation.Y, Size.X, Size.Y);
            return new Rect(relativeLocation.X - (Size.X / 2), relativeLocation.Y - (Size.Y / 2), Size.X, Size.Y);
        }

        /// <summary>
        /// Checks if the mouse hovers over the entity
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public bool IsHover(Game game, Scene scene, Canvas canvas)
        {
            Vec3 mouse = game.GetSceneCord(Input.GetRefMousePos(game));
            Rect rect = GetRelativeBounds2D(canvas);

            if (rect.Contains(mouse.X, mouse.Y))
            {
                Console.WriteLine(this.Name + " " + rect);
                return true;
            }
            return false;
        }

    }
}
