using Genesis.Graphics;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a base class for game elements in the Genesis framework.
    /// </summary>
    public abstract class GameElement
    {
        private Vec3 location;
        private Vec3 rotation;
        private Vec3 scale;

        /// <summary>
        /// Gets or sets the name of the game element.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the tag associated with the game element.
        /// </summary>
        public String Tag { get; set; }

        /// <summary>
        /// Gets or sets the 3D coordinates of the game element.
        /// </summary>
        public virtual Vec3 Location { 
            get => location;
            set => location = value;
        }

        /// <summary>
        /// Gets or sets the 3D rotation of the game element.
        /// </summary>
        public virtual Vec3 Rotation {
            get => rotation; 
            set => rotation = value; 
        }

        /// <summary>
        /// Gets or sets the 3D size of the game element.
        /// </summary>
        public virtual Vec3 Size { get; set; }

        /// <summary>
        /// Gets or sets the list of behaviors associated with the game element.
        /// </summary>
        public List<IGameBehavior> Behaviors { get; set; }

        /// <summary>
        /// Gets or sets the scene to which the game element belongs.
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game element is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the properties associated with the game element.
        /// </summary>
        public Dictionary<String, Object>  Propertys { get; set; }

        /// <summary>
        /// Gets or sets the list of child game elements.
        /// </summary>
        public List<GameElement> Children { get; set; }

        /// <summary>
        /// Gets or sets the parent game element.
        /// </summary>
        public GameElement Parent { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the game element.
        /// </summary>
        public String UUID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the 3D element casts shadows. 
        /// Shadows are currently only supported for 3D elements.
        /// </summary>
        public bool CastShadows { get; set; } = true;

        /// <summary>
        /// Creates a new instance of the GameElement class.
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
        /// Called when the game is initialized.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The rendering device.</param>
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
        /// Called when the game is rendered.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The rendering device.</param>
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
        /// Called when the game is updated.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The rendering device.</param>
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
        /// Called when the game element is disposed.
        /// </summary>
        /// <param name="game">The game instance.</param>
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
        /// Called when the game element collides with another object.
        /// </summary>
        /// <param name="collision">The collision data.</param>
        public virtual void OnCollide(Collision collision)
        {
            foreach (var behavior in this.Behaviors)
            {
                behavior.OnCollide(collision, this);
            }

            foreach (var child in this.Children)
            {
                child.OnCollide(collision);
            }
        }

        /// <summary>
        /// Adds a behavior of type T to the game element and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the behavior.</typeparam>
        /// <param name="behavior">The behavior to be added.</param>
        /// <returns>The added behavior of type T.</returns>
        public T AddBehavior<T>(T behavior) where T : IGameBehavior
        {
            this.Behaviors.Add(behavior);
            behavior.Parent = this;
            return behavior;
        }

        /// <summary>
        /// Adds a behavior to the game element.
        /// </summary>
        /// <param name="behavior">The behavior to be added.</param>
        public void AddBehavior(IGameBehavior behavior)
        {
            behavior.Parent = this;
            this.Behaviors.Add(behavior);
        }

        /// <summary>
        /// Gets the first game behavior of type T.
        /// </summary>
        /// <typeparam name="T">The type of the behavior.</typeparam>
        /// <returns>The first game behavior of type T, or null if not found.</returns>
        public T GetBehavior<T>() where T : IGameBehavior
        {
            foreach (var item in this.Behaviors)
            {
                if (typeof(T).IsAssignableFrom(item.GetType()))
                {
                    return item as T;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns all behaviors of type T.
        /// </summary>
        /// <typeparam name="T">The type of the behavior.</typeparam>
        /// <returns>A list of all behaviors of type T.</returns>
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
        /// Adds a child game element to the current game element.
        /// </summary>
        /// <param name="element">The child game element to be added.</param>
        public void AddChild(GameElement element)
        {
            this.Children.Add(element);
            element.Parent = this;
        }

        /// <summary>
        /// Copies the properties from another game element instance to the current one.
        /// </summary>
        /// <param name="element">The game element from which to copy properties.</param>
        public virtual void GetInstance(GameElement element)
        {
            element.Propertys = this.Propertys.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        /// <summary>
        /// Determines whether the current object has a tag that matches the specified value.
        /// </summary>
        /// <param name="value">The tag value to compare with the current object's tag.</param>
        /// <returns>
        /// <c>true</c> if the current object's tag matches the specified value (case-insensitive); otherwise, <c>false</c>.
        /// </returns>
        public bool HasTag(String value)
        {
            if (this.Tag != null)
            {
                if (this.Tag.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
