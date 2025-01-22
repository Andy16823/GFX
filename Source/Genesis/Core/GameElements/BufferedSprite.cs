using BulletSharp.Math;
using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Genesis.Core.WindowUtilities;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a buffered sprite instance used in graphics rendering.
    /// </summary>
    public class BufferedSpriteInstance : GameElement
    {
        private Vec3 m_location;
        private Vec3 m_rotation;
        private Vec3 m_size;
        private bool m_visible;
        private Color m_color;
        private Vec4 m_uvTransform;

        /// <summary>
        /// Gets or sets the location of the buffered sprite instance.
        /// </summary>
        public override Vec3 Location
        {
            get => m_location;
            set
            {
                m_location = value;
                if (Parent != null)
                {
                    (Parent as BufferedSprite).UpdateInstance(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the buffered sprite instance.
        /// </summary>
        public override Vec3 Rotation
        {
            get => m_rotation;
            set
            {
                m_rotation = value;
                if (Parent != null)
                {
                    (Parent as BufferedSprite).UpdateInstance(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the buffered sprite instance.
        /// </summary>
        public Vec3 Size
        {
            get => m_size;
            set
            {
                m_size = value;
                if (Parent != null)
                {
                    (Parent as BufferedSprite).UpdateInstance(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the buffered sprite instance is visible.
        /// </summary>
        public bool Visible
        {
            get => m_visible;
            set
            {
                m_visible = value;
                if (Parent != null)
                {
                    (Parent as BufferedSprite).UpdateInstance(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the buffered sprite instance.
        /// </summary>
        public Color Color
        {
            get => m_color;
            set
            {
                m_color = value;
                if (Parent != null)
                {
                    (Parent as BufferedSprite).UpdateInstance(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the UV transform of the buffered sprite instance.
        /// </summary>
        public Vec4 UVTransform
        {
            get => m_uvTransform;
            set
            {
                m_uvTransform = value;
                if (Parent != null)
                {
                    (Parent as BufferedSprite).UpdateInstance(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the instance ID of the buffered sprite instance.
        /// </summary>
        public int InstanceID { get; set; }

        /// <summary>
        /// Constructor for the BufferedSpriteInstance class that initializes the buffered sprite instance.
        /// </summary>
        public BufferedSpriteInstance()
        {
            this.Location = new Vec3(0.0f, 0.0f, 0.0f);
            this.Rotation = new Vec3(0.0f, 0.0f, 0.0f);
            this.Size = new Vec3(1.0f, 1.0f, 1.0f);
            this.Color = Color.White;
            this.UVTransform = DefaultUVTransform();
            this.Visible = false;
        }

        /// <summary>
        /// Gets the matrix array of the buffered sprite instance.
        /// </summary>
        /// <returns></returns>
        public float[] GetMatrixArray()
        {
            var t_mat = mat4.Translate(Location.ToGlmVec3());
            var r_mat = mat4.RotateX(Utils.ToRadians(Rotation.X)) * mat4.RotateY(Utils.ToRadians(Rotation.Y)) * mat4.RotateZ(Utils.ToRadians(Rotation.Z));
            var s_mat = mat4.Scale(Size.ToGlmVec3());

            var matrix = t_mat * r_mat * s_mat;
            return matrix.ToArray();
        }

        /// <summary>
        /// Gets the default UV transform of the buffered sprite instance.
        /// </summary>
        /// <returns></returns>
        public static Vec4 DefaultUVTransform()
        {
            return new Vec4(1.0f, 1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Gets the color array of the buffered sprite instance.
        /// </summary>
        /// <returns></returns>
        public float[] GetColorArray()
        {
            return Utils.ConvertColor(Color);
        }

        /// <summary>
        /// Gets the UV transform array of the buffered sprite instance.
        /// </summary>
        /// <returns></returns>
        public float[] GetUVTransformArray()
        {
            return new float[] { UVTransform.X, UVTransform.Y, UVTransform.Z, UVTransform.W };
        }

        /// <summary>
        /// Gets the extras array of the buffered sprite instance.
        /// </summary>
        /// <returns></returns>
        public float[] GetExtrasArray()
        {
            return new float[] { this.Visible ? 1.0f : 0.0f, 0.0f, 0.0f, 0.0f };
        }
    }

    /// <summary>
    /// Represents a buffered sprite used in graphics rendering.
    /// </summary>
    public class BufferedSprite : GameElement
    {

        /// <summary>
        /// Gets or sets the list of buffered sprite instances.
        /// </summary>
        public List<BufferedSpriteInstance> Instances { get; set; }


        /// <summary>
        /// Gets or sets the texture of the buffered sprite.
        /// </summary>
        public Texture Texture { get; set; }


        private IRenderDevice renderer;


        /// <summary>
        /// Constructor for the BufferedSprite class that initializes the buffered sprite with a texture.
        /// </summary>
        /// <param name="texture"></param>
        public BufferedSprite(Texture texture)
        {
            this.Instances = new List<BufferedSpriteInstance>();
            this.Texture = texture;
        }

        /// <summary>
        /// Constructor for the BufferedSprite class that initializes the buffered sprite with a sprite sheet.
        /// </summary>
        /// <param name="spriteSheet"></param>
        public BufferedSprite(SpriteSheet spriteSheet)
        {
            this.Instances = new List<BufferedSpriteInstance>();
            this.Texture = spriteSheet.Texture;
        }

        /// <summary>
        /// Bakes the instances for the buffered sprite.
        /// </summary>
        /// <param name="count"></param>
        public void BakeInstances(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.CreateInstance(Vec3.Zero(), Vec3.Zero(), Vec3.Zero(), Color.White, BufferedSpriteInstance.DefaultUVTransform(), false);
            }
        }

        /// <summary>
        /// Creates a new instance of the buffered sprite.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public BufferedSpriteInstance CreateInstance(Vec3 location, Vec3 rotation, Vec3 size, Color color, Vec4 uvTransform, bool visible = true)
        {
            var instance = new BufferedSpriteInstance
            {
                Location = location,
                Rotation = rotation,
                Size = size,
                Color = color,
                UVTransform = uvTransform,
                Visible = visible,
                Parent = this
            };
            this.Instances.Add(instance);
            instance.InstanceID = this.Instances.Count - 1;

            return instance;
        }

        /// <summary>
        /// Updates the instance of the buffered sprite.
        /// </summary>
        /// <param name="instance"></param>
        public void UpdateInstance(BufferedSpriteInstance instance)
        {
            if (this.renderer != null)
            {
                var matrixOffsetSize = 16 * sizeof(float);
                var mbo = (int)this.Propertys["mbo"];
                renderer.EditBufferSubData(mbo, instance.InstanceID * matrixOffsetSize, instance.GetMatrixArray());

                var colorOffsetSize = 4 * sizeof(float);
                var cbo = (int)this.Propertys["cbo"];
                renderer.EditBufferSubData(cbo, instance.InstanceID * colorOffsetSize, instance.GetColorArray());

                var uvTransformOffsetSize = 4 * sizeof(float);
                var uvto = (int)this.Propertys["uvto"];
                renderer.EditBufferSubData(uvto, instance.InstanceID * uvTransformOffsetSize, instance.GetUVTransformArray());

                var extrasOffsetSize = 4 * sizeof(float);
                var exbo = (int)this.Propertys["exbo"];
                renderer.EditBufferSubData(exbo, instance.InstanceID * extrasOffsetSize, instance.GetExtrasArray());
            }
        }

        /// <summary>
        /// Finds a hidden instance of the buffered sprite.
        /// </summary>
        /// <returns></returns>
        public BufferedSpriteInstance FindHiddenInstance()
        {
            foreach (var instance in Instances)
            {
                if (!instance.Visible)
                {
                    return instance;
                }
            }
            return null;
        }

        /// <summary>
        /// Initializes the game element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
            this.renderer = renderDevice;
        }

        /// <summary>
        /// Renders the game element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawBufferedSprite(this);
        }

        /// <summary>
        /// Updates the game element.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnUpdate(Game game, IRenderDevice renderDevice)
        {
            base.OnUpdate(game, renderDevice);
        }

        /// <summary>
        /// Clears the GPU memory
        /// </summary>
        /// <param name="game"></param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement(this);
        }

        /// <summary>
        /// Gets the vertex buffer for the buffered sprite.
        /// </summary>
        /// <returns></returns>
        public static float[] GetVertexBuffer()
        {
            return new float[]
            {
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f
            };
        }

        /// <summary>
        /// Gets the index buffer for the buffered sprite.
        /// </summary>
        /// <returns></returns>
        public static int[] GetIndexBuffer()
        {
            return new int[] 
            {
                0, 1, 3,
                3, 1, 2
            };
        }

        /// <summary>
        /// Gets the uv buffer for the buffered sprite.
        /// </summary>
        /// <returns></returns>
        public static float[] GetUVBuffer()
        {
            return new float[] 
            {
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };
        }
    }
}