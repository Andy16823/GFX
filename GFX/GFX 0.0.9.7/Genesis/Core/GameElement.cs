using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core
{
    public abstract class GameElement
    {
        public String Name { get; set; }
        public String Tag { get; set; }
        public Vec3 Location { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Size { get; set; }
        public List<IGameBehavior> Behaviors { get; set; }
        public Scene Scene { get; set; }
        public bool Enabled { get; set; } = true;
        public Dictionary<String, Object>  Propertys { get; set; }
        public List<GameElement> Children { get; set; }
        public GameElement Parent { get; set; }
        public String UUID { get; set; }

        /// <summary>
        /// Creates a new game element
        /// </summary>
        public GameElement()
        {
            this.Propertys = new Dictionary<string, object>();
            this.Behaviors= new List<IGameBehavior>();
            this.Children = new List<GameElement>();
            this.Location = Vec3.Zero();
            this.Rotation = Vec3.Zero();
            this.Size = Vec3.Zero();
            this.UUID = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Called when the game get initalized
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public virtual void Init(Game game, IRenderDevice renderDevice)
        {
            renderDevice.InitGameElement(this);
            foreach (var item in this.Behaviors) 
            {
                item.OnInit(game, this);
            }
            foreach (var element in this.Children)
            {
                element.Init(game, renderDevice);
            }
        }

        /// <summary>
        /// Called when the game get renders
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public virtual void OnRender(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in this.Behaviors)
            {
                item.OnRender(game, this);
            }
            foreach (var element in this.Children)
            {
                element.OnRender(game, renderDevice);
            }
        }

        /// <summary>
        /// Called when the game gets updated
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public virtual void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in this.Behaviors)
            {
                item.OnUpdate(game, this);
            }
            foreach (var element in this.Children)
            {
                element.OnUpdate(game, renderDevice);
            }
        }

        /// <summary>
        /// Called when the game gets disposed
        /// </summary>
        /// <param name="game"></param>
        public virtual void OnDestroy(Game game)
        {
            foreach (var item in this.Behaviors)
            {
                item.OnDestroy(game, this);
            }
            foreach (var element in this.Children)
            {
                element.OnDestroy(game);
            }
        }

        /// <summary>
        /// Adds behavior of type t and returns them
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public T AddBehavior<T>(T behavior) where T : IGameBehavior
        {
            this.Behaviors.Add(behavior);
            behavior.Parent = this;
            return behavior;
        }

        /// <summary>
        /// Adds a behavior
        /// </summary>
        /// <param name="behavior"></param>
        public void AddBehavior(IGameBehavior behavior)
        {
            behavior.Parent = this;
            this.Behaviors.Add(behavior);
        }

        /// <summary>
        /// Gets the first game behavior of type t
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IGameBehavior GetBehavior<T>()
        {
            foreach (var item in this.Behaviors)
            {
                if(item.GetType() == typeof(T))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns all behaviors of the type t
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<IGameBehavior> GetBehaviors<T>()
        {
            List<IGameBehavior> beh = new List<IGameBehavior>();
            foreach (var item in this.Behaviors)
            {
                if(item.GetType() == typeof(T))
                {
                    beh.Add(item);
                }
            }
            return beh;
        }

        /// <summary>
        /// Adds an children to the game element
        /// </summary>
        /// <param name="element"></param>
        public void AddChild(GameElement element)
        {
            this.Children.Add(element);
            element.Parent = this;
        }

        /// <summary>
        /// Gets the propeterys from the element
        /// </summary>
        /// <param name="element"></param>
        public virtual void GetInstance(GameElement element)
        {
            element.Propertys = this.Propertys.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }
}
