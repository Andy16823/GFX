using Assimp;
using BulletSharp;
using Genesis.Core.GameElements;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    public class CompoundMeshCollider : ColliderBehavior3D
    {
        public CompoundMeshCollider(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        public void CreateCollider(String file, int collisionGroup = -1, int collisionMask = -1)
        {
            Element3D element = (Element3D)this.Parent;

            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            var model = importer.ImportFile(file, Assimp.PostProcessPreset.TargetRealTimeQuality | Assimp.PostProcessSteps.PreTransformVertices);
            var compoundShape = new CompoundShape();

            var scale = Utils.GetElementWorldScale(element);
            var btStartTransform = Utils.GetBtTransform(element);

            foreach (var mesh in model.Meshes)
            {
                int[] indicies = mesh.GetIndices();
                float[] verticies = mesh.Vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray();

                TriangleIndexVertexArray triangle = new TriangleIndexVertexArray(indicies, verticies);
                BvhTriangleMeshShape shape = new BvhTriangleMeshShape(triangle, true);

                compoundShape.AddChildShape(BulletSharp.Math.Matrix.Identity, shape);
            }

            compoundShape.LocalScaling = scale.ToBulletVec3();
            compoundShape.CalculateLocalInertia(0f);

            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = compoundShape;
            Collider.WorldTransform = btStartTransform;
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }

        public override void CreateCollider(int collisionGroup = -1, int collisionMask = -1)
        {
            throw new NotImplementedException();
        }

        private static Node FindMeshNode(Node root, String meshname)
        {
            return root.FindNode(meshname);
        }

        private static BulletSharp.Math.Matrix GetMatrix(Assimp.Matrix4x4 matrix)
        {
            return new BulletSharp.Math.Matrix(
                matrix.A1, matrix.B1, matrix.C1, matrix.D1, // Erste Spalte
                matrix.A2, matrix.B2, matrix.C2, matrix.D2, // Zweite Spalte
                matrix.A3, matrix.B3, matrix.C3, matrix.D3, // Dritte Spalte
                matrix.A4, matrix.B4, matrix.C4, matrix.D4  // Vierte Spalte
            );
        }

        private static BulletSharp.Math.Vector3 TransformVector(Vec3 vec)
        {
            return new BulletSharp.Math.Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}
