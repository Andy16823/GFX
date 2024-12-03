using BulletSharp;
using Genesis.Core.GameElements;
using Genesis.Math;
using Genesis.Physics;
using GlmSharp;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Behaviors.Physics3D
{
    public class MeshCollider : ColliderBehavior3D
    {
        public MeshCollider(PhysicHandler physicHandler) : base(physicHandler)
        {
        }

        public void CreateCollider(String file, int collisionGroup = -1, int collisionMask = -1)
        {
            Element3D element = (Element3D)this.Parent;

            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            //importer.SetConfig(new Assimp.Configs.NormalSmoothingAngleConfig(66.0f));
            var model = importer.ImportFile(file, Assimp.PostProcessPreset.TargetRealTimeQuality | Assimp.PostProcessSteps.PreTransformVertices);

            var mesh = model.Meshes[0];
            int[] indicies = mesh.GetIndices();
            float[] verticies = mesh.Vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray();

            // TriangleIndexVertexArray für Mesh-Kollisionsform
            TriangleIndexVertexArray triangle = new TriangleIndexVertexArray(indicies, verticies);
            BvhTriangleMeshShape shape = new BvhTriangleMeshShape(triangle, true);

            // Position, Rotation und Skalierung abrufen
            var location = Utils.GetElementWorldLocation(element);
            var rotation = Utils.GetElementWorldRotation(element);
            var scale = Utils.GetElementWorldScale(element);

            quat quat = new quat(new vec3(Utils.ToRadians(rotation.X), Utils.ToRadians(rotation.Y), Utils.ToRadians(rotation.Z)));
            mat4 rotMat = new mat4(quat);

            BulletSharp.Math.Matrix btTranslation = BulletSharp.Math.Matrix.Translation(location.X, location.Y, location.Z);
            BulletSharp.Math.Matrix btRotation = new BulletSharp.Math.Matrix(rotMat.ToArray());

            //var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            //var btRotation = BulletSharp.Math.Matrix.RotationX(Utils.ToRadians(rotation.X))
            //                * BulletSharp.Math.Matrix.RotationY(Utils.ToRadians(rotation.Y))
            //                * BulletSharp.Math.Matrix.RotationZ(Utils.ToRadians(rotation.Z));

            var btStartTransform = btRotation * btTranslation;

            shape.LocalScaling = scale.ToBulletVec3();
            shape.CalculateLocalInertia(0f);

            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = shape;
            Collider.WorldTransform = btStartTransform;
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }

        public override void CreateCollider(int collisionGroup = -1, int collisionMask = -1)
        {
            throw new NotImplementedException();
        }
    }
}
