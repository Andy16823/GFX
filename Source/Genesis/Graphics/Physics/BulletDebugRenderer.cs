﻿using BulletSharp;
using BulletSharp.Math;
using Genesis.Graphics.RenderDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Physics
{
    public class BulletDebugRenderer : DebugDraw
    {
        private IRenderDevice Renderer;
        private DebugDrawModes _debugMode = DebugDrawModes.All;

        public BulletDebugRenderer(IRenderDevice RenderDevice)
        {
            this.Renderer = RenderDevice;
        }

        public override DebugDrawModes DebugMode
        {
            get => _debugMode; set => _debugMode = value;
        }

        public override void Draw3DText(ref Vector3 location, string textString)
        {

        }

        public override void DrawLine(ref Vector3 from, ref Vector3 to, ref Vector3 color)
        {
            Renderer.DrawLine(new Genesis.Math.Vec3(from.X, from.Y, from.Z), new Genesis.Math.Vec3(to.X, to.Y, to.Z), System.Drawing.Color.Red);
        }

        public override void ReportErrorWarning(string warningString)
        {

        }
    }
}
