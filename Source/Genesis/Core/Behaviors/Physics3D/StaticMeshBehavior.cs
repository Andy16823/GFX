using BulletSharp;
using BulletSharp.Math;
using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ToDo Rewort for assimp
namespace Genesis.Core.Behaviors.Physics3D
{
    /// <summary>
    /// Represents a Physics behavior for a static mesh in 3D.
    /// </summary>
    public class StaticMeshBehavior : Physics.PhysicsBehavior
    {
        /// <summary>
        /// Gets or sets the RigidBody associated with this behavior.
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Creates the RigidBody for the StaticMeshBehavior.
        /// </summary>
        /// <param name="handler">Physics handler responsible for managing elements.</param>
        /// <param name="mass">Mass of the rigid body.</param>
        /// <param name="mesh">Mesh data for creating the collision shape.</param>
        /// <exception cref="InvalidOperationException">Thrown when the parent element is not of type Element3D.</exception>
        public void CreateRigidBody(Physics.PhysicHandler handler, float mass, Mesh mesh)
        {
            if (this.Parent.GetType() == typeof(Element3D))
            {
                Element3D element = (Element3D)this.Parent;

                int[] indicies = mesh.Indicies.ToArray();
                float[] verticies = mesh.Vericies.ToArray();

                TriangleIndexVertexArray triangle = new TriangleIndexVertexArray(indicies, verticies);
                BvhTriangleMeshShape shape = new BvhTriangleMeshShape(triangle, true);
                RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass));
                BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(element.Location.X, element.Location.Y, element.Location.Z);
                BulletSharp.Math.Matrix rotation = BulletSharp.Math.Matrix.RotationX(element.Rotation.X) * BulletSharp.Math.Matrix.RotationY(element.Rotation.Y) * BulletSharp.Math.Matrix.RotationZ(element.Rotation.Z);
                info.MotionState = new DefaultMotionState(rotation * transform);
                this.RigidBody = new BulletSharp.RigidBody(info);
                this.RigidBody.ApplyGravity();
                this.RigidBody.CollisionShape.LocalScaling = new Vector3(element.Size.X, element.Size.Y, element.Size.Z);

                handler.ManageElement(this);
            }
            else
            {
                throw new InvalidOperationException("Invalid element for this Behavior");
            }
        }

        /// <summary>
        /// Updates the RigidBody's position and scaling based on the associated parent element.
        /// </summary>
        public void UpdateRigidBody()
        {
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.X, Parent.Location.Y, Parent.Location.Z);
            this.RigidBody.CollisionShape.LocalScaling = new Vector3(Parent.Size.X, Parent.Size.Y, Parent.Size.Z);
            this.RigidBody.MotionState = new DefaultMotionState(transform);
        }

        /// <summary>
        /// Destroys the behavior.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnDestroy(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Checks on init if the parent type is an Element3D.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        /// <exception cref="InvalidOperationException">Thrown when the parent element is not of type Element3D.</exception>
        public override void OnInit(Game game, GameElement parent)
        {
            if (parent.GetType() != typeof(Element3D))
            {
                throw new InvalidOperationException("The Physics3D Behavior can only be attached to an Element3D.");
            }
        }

        /// <summary>
        /// Renderer callback
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnRender(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Update callback.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnUpdate(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Returns the physics object.
        /// </summary>
        /// <returns></returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        /// <summary>
        /// Returns the physics object with the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }
    }
}
