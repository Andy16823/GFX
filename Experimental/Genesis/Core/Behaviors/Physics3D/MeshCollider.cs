using BulletSharp;
using Genesis.Core.GameElements;
using Genesis.Math;
using Genesis.Physics;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
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

        public override void CreateCollider(int collisionGroup = -1, int collisionMask = -1)
        {
            Element3D element = (Element3D)this.Parent;

            int[] indicies = element.Meshes[0].Indicies.ToArray();
            float[] verticies = element.Meshes[0].Vericies.ToArray();

            // TriangleIndexVertexArray für Mesh-Kollisionsform
            TriangleIndexVertexArray triangle = new TriangleIndexVertexArray(indicies, verticies);
            BvhTriangleMeshShape shape = new BvhTriangleMeshShape(triangle, true);

            // Position, Rotation und Skalierung abrufen
            Vec3 location = Utils.GetElementWorldLocation(element);
            Vec3 rotation = Utils.GetElementWorldRotation(element);
            Vec3 scale = Utils.GetElementWorldScale(element);

            // Matrixen für Translation, Rotation und Skalierung erstellen
            var btTranslation = BulletSharp.Math.Matrix.Translation(location.ToBulletVec3());
            var btRotation = BulletSharp.Math.Matrix.RotationX(rotation.X)
                            * BulletSharp.Math.Matrix.RotationY(rotation.Y)
                            * BulletSharp.Math.Matrix.RotationZ(rotation.Z);
            var btScale = BulletSharp.Math.Matrix.Scaling(scale.X, scale.Y, scale.Z);

            // Gesamttransformation
            var btStartTransform = btScale * (btTranslation * btRotation);

            // Optional: Berechnung des Inertia, falls das Objekt dynamisch ist
            shape.CalculateLocalInertia(0f); // Für statische Objekte

            // Collider erstellen und in der Physik-Engine registrieren
            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = shape;
            Collider.WorldTransform = btStartTransform;
            PhysicHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}
