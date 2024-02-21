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
    public class StaticMeshBehavior : Physics.PhysicsBehavior
    {
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Create the rigidbody for this behavior
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="mass"></param>
        /// <exception cref="InvalidOperationException"></exception>
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
        /// Gets called when the element collides with another
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="game"></param>
        /// <param name="collisionObject"></param>
        public override void Collide(Scene scene, Game game, BulletSharp.RigidBody collisionObject)
        {
            base.Collide(scene, game, collisionObject);

        }

        /// <summary>
        /// Sets the scaling for the RigidBody
        /// </summary>
        public void UpdateRigidBody()
        {
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(Parent.Location.X, Parent.Location.Y, Parent.Location.Z);
            this.RigidBody.CollisionShape.LocalScaling = new Vector3(Parent.Size.X, Parent.Size.Y, Parent.Size.Z);
            this.RigidBody.MotionState = new DefaultMotionState(transform);
        }

        /// <summary>
        /// Destroys the behavior
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnDestroy(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Check on init if the parent type is a Element3D
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        /// <exception cref="InvalidOperationException"></exception>
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
        /// Update callback
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public override void OnUpdate(Game game, GameElement parent)
        {

        }

        /// <summary>
        /// Returns the physic object
        /// </summary>
        /// <returns></returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }

        public override T GetPhysicsObject<T>()
        {
            return (T)(object)RigidBody;
        }
    }
}
