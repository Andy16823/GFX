using BulletSharp;
using Genesis.Core;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    /// <summary>
    /// Represents a delegate for handling physics events.
    /// </summary>
    /// <param name="scene">The scene involved in the physics event</param>
    /// <param name="game">The game involved in the physics event</param>
    /// <param name="element">The element involved in the physics event</param>
    public delegate void PhysicHandlerEvent(Scene scene, Game game, object element);

    /// <summary>
    /// Represents the properties related to physics.
    /// </summary>
    public struct PhysicPropeterys
    {
        public float gravityX;
        public float gravityY;
        public float gravityZ;
    }

    /// <summary>
    /// Represents an abstract class for handling physics interactions.
    /// </summary>
    public abstract class PhysicHandler
    {
        /// <summary>
        /// Gets or sets the dictionary of physics callbacks.
        /// </summary>
        public Dictionary<object, PhysicHandlerEvent>  Callbacks { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicHandler"/> class.
        /// </summary>
        public PhysicHandler()
        {
            this.Callbacks = new Dictionary<object, PhysicHandlerEvent>();
        }

        /// <summary>
        /// Processes physics interactions for a given scene and game.
        /// </summary>
        /// <param name="scene">The scene to process physics for</param>
        /// <param name="game">The game to process physics for</param>
        public abstract void Process(Scene scene, Game game);

        /// <summary>
        /// Manages physics interactions for a specific physics behavior.
        /// </summary>
        /// <param name="physicsBehavior">The physics behavior to manage</param>
        public virtual void ManageElement(PhysicsBehavior physicsBehavior, int collisionGroup = -1, int collisionMask = -1)
        {
            this.Callbacks.Add(physicsBehavior.GetPhysicsObject(), (scene, game, cObj) =>
            {
                physicsBehavior.Collide(scene, game, (GameElement) cObj);
            });
        }

        /// <summary>
        /// Removes the specified physics behavior from the physics world.
        /// </summary>
        /// <param name="physicsBehavior">The physics behavior to be removed.</param>
        /// <remarks>
        /// This abstract method should be implemented to remove the collision object associated with the provided physics behavior from the physics world.
        /// </remarks>
        public abstract void RemoveElement(PhysicsBehavior physicsBehavior);
    }
}
