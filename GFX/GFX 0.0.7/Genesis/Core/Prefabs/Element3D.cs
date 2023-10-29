using BulletSharp;
using BulletSharp.SoftBody;
using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.Math;
using System.Xml.Linq;

namespace Genesis.Core.Prefabs
{
    public class Element3D : GameElement
    {
        public Vec3 Location { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }
        public ShaderProgram Shader { get; set; }
        public OpenObjectLoader.Model Model { get; set; }
        public RigidBody RigidBody { get; set; }

        public Element3D(String name, String path, Vec3 location, Vec3 rotation, Vec3 scale)
        {
            this.Name = name;
            this.Location = location;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Model = new OpenObjectLoader.WavefrontLoader().LoadModel(path);
            this.Model.Propertys.Add("path", new System.IO.FileInfo(path).DirectoryName);
        }

        /// <summary>
        /// Creates a rigid body fot the element
        /// </summary>
        /// <param name="world"></param>
        /// <param name="mass"></param>
        public void CreateRigidBody(DiscreteDynamicsWorld world, float mass)
        {
            ConvexHullShape shape = new ConvexHullShape(this.Model.GetMesh());
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass));
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(this.Location.X, this.Location.Y, this.Location.Z);
            BulletSharp.Math.Matrix rotation = BulletSharp.Math.Matrix.RotationX(this.Rotation.X) * BulletSharp.Math.Matrix.RotationY(this.Rotation.Y) * BulletSharp.Math.Matrix.RotationZ(this.Rotation.Z);
            info.MotionState = new DefaultMotionState(transform);
            this.RigidBody = new BulletSharp.RigidBody(info);
            this.RigidBody.ApplyGravity();
            this.RigidBody.CollisionShape.LocalScaling = new Vector3(this.Scale.X, this.Scale.Y, this.Scale.Z);
            world.AddRigidBody(this.RigidBody);
        }

        public void UpdateRigidBody()
        {
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Translation(this.Location.X, this.Location.Y, this.Location.Z);
            this.RigidBody.MotionState = new DefaultMotionState(transform);
        }

        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            if(this.RigidBody != null)
            {
                Vector3 position = RigidBody.WorldTransform.Origin;
                this.Location.Y = position.Y;
            }
        }

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            renderDevice.InitElement3D(this);
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawElement3D(this);
        }

        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement3D(this);
        }

    }
}
