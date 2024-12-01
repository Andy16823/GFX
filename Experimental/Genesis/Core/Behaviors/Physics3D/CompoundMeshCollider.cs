using BulletSharp.Math;
using BulletSharp;
using Genesis.Core.GameElements;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assimp.Configs;
using Genesis.Graphics.Shapes;

namespace Genesis.Core.Behaviors.Physics3D
{
    public class CompoundMeshCollider : ColliderBehavior3D
    {
        public CompoundMeshCollider(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        public override void CreateCollider(int collisionGroup = -1, int collisionMask = -1)
        {
            var element = (Element3D)this.Parent;
            var compoundShape = new CompoundShape();

            var btTranslation = BulletSharp.Math.Matrix.Translation(element.Location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(element.Rotation.X) * BulletSharp.Math.Matrix.RotationY(element.Rotation.Y) * BulletSharp.Math.Matrix.RotationZ(element.Rotation.Z);
            var btStartTransform = btTranslation * btRotation;

            foreach (var mesh in element.Meshes)
            {
                int[] indicies = mesh.Indicies.ToArray();
                float[] verticies = mesh.Vericies.ToArray();

                var meshTranslation = BulletSharp.Math.Matrix.Translation(0.0f, 0.0f, 0.0f);
                var meshScale = BulletSharp.Math.Matrix.Scaling(element.Size.ToBulletVec3());
                var meshRotation = BulletSharp.Math.Matrix.RotationX(element.Rotation.X) * BulletSharp.Math.Matrix.RotationY(element.Rotation.Y) * BulletSharp.Math.Matrix.RotationZ(element.Rotation.Z);
                var meshTransform = meshTranslation * meshRotation * meshScale;

                TriangleIndexVertexArray triangle = new TriangleIndexVertexArray(indicies, verticies);
                BvhTriangleMeshShape shape = new BvhTriangleMeshShape(triangle, false);
                compoundShape.AddChildShape(meshTransform, shape);
            }

            Collider = new BulletSharp.CollisionObject();
            Collider.CollisionShape = compoundShape;
            Collider.UserObject = this.Parent;
            Collider.WorldTransform = btStartTransform;
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}
