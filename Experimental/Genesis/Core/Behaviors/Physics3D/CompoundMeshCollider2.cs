using Assimp;
using BulletSharp;
using Genesis.Core.GameElements;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    public class CompoundMeshCollider2 : ColliderBehavior3D
    {
        public CompoundMeshCollider2(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        public void CreateCollider(String path, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = (Element3D)this.Parent;
            var compoundShape = new CompoundShape();

            var btTranslation = BulletSharp.Math.Matrix.Translation(element.Location.ToBulletVec3());
            var btScale = BulletSharp.Math.Matrix.Scaling(1f);
            var btRotation = BulletSharp.Math.Matrix.RotationX(element.Rotation.X) * BulletSharp.Math.Matrix.RotationY(element.Rotation.Y) * BulletSharp.Math.Matrix.RotationZ(element.Rotation.Z);
            var btStartTransform = btTranslation * btRotation * btScale;

            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            //importer.SetConfig(new Assimp.Configs.NormalSmoothingAngleConfig(66.0f));
            var model = importer.ImportFile(path , Assimp.PostProcessPreset.TargetRealTimeQuality | Assimp.PostProcessSteps.Triangulate);

            foreach (var mesh in model.Meshes)
            {
                int[] indicies = mesh.GetIndices();
                float[] verticies = mesh.Vertices.SelectMany(v => new float[] {v.X, v.Y, v.Z}).ToArray();
                var meshScale = BulletSharp.Math.Matrix.Scaling(element.Size.ToBulletVec3());
                var node = CompoundMeshCollider2.FindMeshNode(model.RootNode, mesh.Name);
                var transform = CompoundMeshCollider2.GetMatrix(node.Transform) * meshScale;

                TriangleIndexVertexArray triangle = new TriangleIndexVertexArray(indicies, verticies);
                BvhTriangleMeshShape shape = new BvhTriangleMeshShape(triangle, true);

                compoundShape.AddChildShape(transform, shape);
            }

            //var box = new BoxShape(new BulletSharp.Math.Vector3(0.5f, 0.5f, 0.5f));
            //var boxTransform = BulletSharp.Math.Matrix.Translation(0 , 0, 0);
            //var boxScale = BulletSharp.Math.Matrix.Scaling(1f);
            //compoundShape.AddChildShape(boxTransform * boxScale, box);

            Collider = new BulletSharp.CollisionObject();
            Collider.CollisionShape = compoundShape;
            Collider.UserObject = this.Parent;
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
    }
}
