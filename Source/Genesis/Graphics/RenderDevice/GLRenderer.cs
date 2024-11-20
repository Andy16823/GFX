using Genesis.Core.GameElements;
using Genesis.Core;
using Genesis.Math;
using NetGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using System.Security.Policy;
using Microsoft.Win32.SafeHandles;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Windows.Forms;
using Genesis.Graphics.Shaders.OpenGL;
using System.CodeDom;
using Genesis.UI;
using Genesis.Graphics.Shapes;
using System.Runtime.CompilerServices;
using Genesis.Graphics.Animation3D;
using System.Runtime.InteropServices;
using static System.Windows.Forms.AxHost;
using BulletSharp;
using BulletSharp.SoftBody;
using Newtonsoft.Json.Linq;

namespace Genesis.Graphics.RenderDevice
{
    public class GLRenderer : IRenderDevice
    {
        /// <summary>
        /// Struct for the viewport
        /// </summary>
        private float rot;
        private Camera camera;
        private mat4 p_mat;
        private mat4 v_mat;

        private Dictionary<String, ShaderProgram> ShaderPrograms;
        private Dictionary<String, Shapes.Shape> InstancedShapes;
        private NetGL.OpenGL gl;
        private IntPtr hwnd;
        private Light lightSource;

        private RenderSettings m_renderSettings;
        
        public Framebuffer sceneBuffer;
        private Framebuffer uiBuffer;

        private Viewport m_viewport;

        public GLRenderer(IntPtr hwnd, RenderSettings settings)
        {
            this.hwnd = hwnd;
            m_renderSettings = settings;
        }

        /// <summary>
        /// Initial the moderngl render device
        /// </summary>
        public void Init()
        {
            gl = new NetGL.OpenGL();
            gl.modernGL = true;
            gl.Initial(hwnd);
            gl.SwapIntervalEXT(0);
            gl.ClearColor(0.188f, 0.0f, 0.188f, 0.0f);
            gl.Enable(OpenGL.DepthTest);
            gl.DepthFunc(OpenGL.Less);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.SrcAlpha, OpenGL.OneMinusSrcAlpha);
            gl.Enable(OpenGL.AlphaTest);
            gl.AlphaFunc(OpenGL.Greater, 0.1f);

            ///Initial the prebuild shaders
            this.ShaderPrograms = new Dictionary<String, ShaderProgram>();
            this.ShaderPrograms.Add("BasicShader", new BasicShader());
            this.ShaderPrograms.Add("MVPShader", new MVPShader());
            this.ShaderPrograms.Add("MVPSolidShader", new MVPSolidShader());
            this.ShaderPrograms.Add("MVPRectShader", new MVPRectShader());
            this.ShaderPrograms.Add("DiffuseShader", new DiffuseShader());
            this.ShaderPrograms.Add("DiffuseNormalShader", new DiffuseNormalShader());
            this.ShaderPrograms.Add("WireframeShader", new WireframeShader());
            this.ShaderPrograms.Add("ScreenShader", new ScreenShader());
            this.ShaderPrograms.Add("SceneShader", new SceneShader());
            this.ShaderPrograms.Add("SpriteShader", new SpriteShader());
            this.ShaderPrograms.Add("TerrainShader", new TerrainShader());
            this.ShaderPrograms.Add("ParticleShader", new ParticleShader());
            this.ShaderPrograms.Add("Light2DShader", new Light2DShader());
            this.ShaderPrograms.Add("SolidShapeShader", new SolidShapeShader());
            this.ShaderPrograms.Add("BorderCircleShader", new BorderCircleShader());

            foreach (KeyValuePair<string, ShaderProgram> item in this.ShaderPrograms)
            {
                this.CreateShader(item.Key, item.Value);
            }

            ///Initial the pre build shapes
            this.InstancedShapes = new Dictionary<String, Shapes.Shape>();
            this.InstancedShapes.Add("SpriteShape", new Shapes.SpriteShape());
            this.InstancedShapes.Add("RectShape", new Shapes.RectShape());
            this.InstancedShapes.Add("GlypheShape", new Shapes.GlypheShape());
            this.InstancedShapes.Add("LineShape", new Shapes.LineShape());
            this.InstancedShapes.Add("FrameShape", new Shapes.FrameShape());
            this.InstancedShapes.Add("Light2DShape", new Light2DShape());
            this.InstancedShapes.Add("CircleShape", new CircleShape());
            foreach (KeyValuePair<string, Shapes.Shape> item in this.InstancedShapes)
            {
                Console.WriteLine("Building Shape " + item.Key.ToString());
                this.BuildShape(item.Value);
                Console.WriteLine("Builded Shape " + item.Key.ToString() + " with error " + gl.GetError());
            }

            this.sceneBuffer = this.BuildFramebuffer(100, 100);
            this.uiBuffer = this.BuildFramebuffer(100, 100);
        }

        /// <summary>
        /// Creates an buffer for the shape
        /// </summary>
        /// <param name="shape"></param>
        private void BuildShape(Shapes.Shape shape)
        {
            float[] verticies = shape.GetShape();
            shape.vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, shape.vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
        }

        public void EditBufferSubData(int bufferId, int offset, float[] data)
        {
            gl.BindBuffer(OpenGL.ArrayBuffer, bufferId);
            gl.BufferSubData(OpenGL.ArrayBuffer, offset, data.Length * sizeof(float), data);
            gl.BindBuffer(OpenGL.ArrayBuffer, 0);
        }

        /// <summary>
        /// Creates a dynamic vertex buffer in OpenGL and initializes it with the specified vertices.
        /// Dynamic buffers are suitable for frequently changing data, like dynamic vertex updates.
        /// </summary>
        /// <param name="verticies">The array of vertices to be stored in the buffer.</param>
        /// <returns>The OpenGL handle (ID) of the created dynamic vertex buffer.</returns>
        public int CreateDynamicVertexBuffer(float[] verticies)
        {
            int vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            return vbo;
        }

        /// <summary>
        /// Creates a static vertex buffer in OpenGL and initializes it with the specified vertices.
        /// Static buffers are suitable for infrequently changing data, like static geometry.
        /// </summary>
        /// <param name="verticies">The array of vertices to be stored in the buffer.</param>
        /// <returns>The OpenGL handle (ID) of the created static vertex buffer.</returns>
        public int CreateStaticVertexBuffer(float[] verticies)
        {
            int vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.StaticDraw);
            return vbo;
        }

        /// <summary>
        /// Creates a new framebuffer
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Framebuffer BuildFramebuffer(int width, int height)
        {
            int framebuffer = gl.GenFramebuffers(1);
            gl.BindFramebuffer(OpenGL.FrameBuffer, framebuffer);

            int texture = gl.GenTextures(1);
            gl.BindTexture(OpenGL.Texture2D, texture);
            gl.TexImage2D(OpenGL.Texture2D, 0, OpenGL.RGBA, width, height, 0, OpenGL.RGBA, OpenGL.UnsignedByte);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMinFilter, NetGL.OpenGL.Linear);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMagFilter, NetGL.OpenGL.Linear);
            gl.FrameBufferTexture2D(OpenGL.FrameBuffer, OpenGL.ColorAttachment0, OpenGL.Texture2D, texture, 0);

            int rbo = gl.GenRenderbuffers(1);
            gl.BindRenderbuffer(OpenGL.RenderBuffer, rbo);
            gl.RenderbufferStorage(OpenGL.RenderBuffer, OpenGL.DepthComponent24, width, height);
            gl.BindRenderbuffer(OpenGL.RenderBuffer, 0);
            gl.FramebufferRenderbuffer(OpenGL.FrameBuffer, OpenGL.DepthAttachment, OpenGL.RenderBuffer, rbo);

            if (gl.CheckFramebufferStatus(OpenGL.FrameBuffer) == OpenGL.FrameBufferComplete)
            {
                Console.WriteLine("Framebuffer complete");
            }
            else
            {
                Console.WriteLine("Framebuffer Incomplete");
            }
            gl.BindFramebuffer(OpenGL.FrameBuffer, 0);

            Framebuffer buffer = new Framebuffer();
            buffer.Texture = texture;
            buffer.RenderBuffer = rbo;
            buffer.FramebufferID = framebuffer;

            return buffer;
        }

        /// <summary>
        /// Creates a new framebuffer
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Framebuffer BuildFramebuffer(int width, int height, Texture texture)
        {
            return this.BuildFramebuffer(width, height, texture.RenderID);
        }

        /// <summary>
        /// Creates a new framebuffer
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Framebuffer BuildFramebuffer(int width, int height, int texture)
        {
            int framebuffer = gl.GenFramebuffers(1);
            gl.BindFramebuffer(OpenGL.FrameBuffer, framebuffer);

            gl.BindTexture(OpenGL.Texture2D, texture);
            gl.TexImage2D(OpenGL.Texture2D, 0, OpenGL.RGBA, width, height, 0, OpenGL.RGBA, OpenGL.UnsignedByte);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMinFilter, NetGL.OpenGL.Linear);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMagFilter, NetGL.OpenGL.Linear);
            gl.FrameBufferTexture2D(OpenGL.FrameBuffer, OpenGL.ColorAttachment0, OpenGL.Texture2D, texture, 0);

            int rbo = gl.GenRenderbuffers(1);
            gl.BindRenderbuffer(OpenGL.RenderBuffer, rbo);
            gl.RenderbufferStorage(OpenGL.RenderBuffer, OpenGL.DepthComponent24, width, height);
            gl.BindRenderbuffer(OpenGL.RenderBuffer, 0);
            gl.FramebufferRenderbuffer(OpenGL.FrameBuffer, OpenGL.DepthAttachment, OpenGL.RenderBuffer, rbo);

            if (gl.CheckFramebufferStatus(OpenGL.FrameBuffer) == OpenGL.FrameBufferComplete)
            {
                Console.WriteLine("Framebuffer complete");
            }
            else
            {
                Console.WriteLine("Framebuffer Incomplete");
            }
            gl.BindFramebuffer(OpenGL.FrameBuffer, 0);

            Framebuffer buffer = new Framebuffer();
            buffer.Texture = texture;
            buffer.RenderBuffer = rbo;
            buffer.FramebufferID = framebuffer;

            return buffer;
        }

        /// <summary>
        /// Update the framebuffer size
        /// </summary>
        /// <param name="framebuffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void UpdateFramebufferSize(Framebuffer framebuffer, int width, int height)
        {
            gl.BindTexture(OpenGL.Texture2D, framebuffer.Texture);
            gl.TexImage2D(OpenGL.Texture2D, 0, OpenGL.RGBA, width, height, 0, OpenGL.RGBA, OpenGL.UnsignedByte);

            gl.BindRenderbuffer(OpenGL.RenderBuffer, framebuffer.RenderBuffer);
            gl.RenderbufferStorage(OpenGL.RenderBuffer, OpenGL.DepthComponent24, width, height);
        }

        /// <summary>
        /// Loads a shader program
        /// </summary>
        /// <param name="name"></param>
        /// <param name="program"></param>
        public void CreateShader(String name, ShaderProgram program)
        {
            Console.WriteLine("Loading " + name + " Shader Program");
            //Creating the vertex shader from the program
            Console.WriteLine("Creating Vertexshader");
            program.VertexShader.ShaderID = gl.CreateShader(OpenGL.VertexShader);
            gl.SetShaderSource(program.VertexShader.ShaderID, 1, program.VertexShader.Source);
            gl.CompileShader(program.VertexShader.ShaderID);
            Console.WriteLine("Created Vertexshader with error " + gl.GetError());

            //Creating the fragment shader from the program
            Console.WriteLine("Creating Fragmentshader");
            program.FragmentShader.ShaderID = gl.CreateShader(OpenGL.FragmentShader);
            gl.SetShaderSource(program.FragmentShader.ShaderID, 1, program.FragmentShader.Source);
            gl.CompileShader(program.FragmentShader.ShaderID);
            Console.WriteLine("Created Fragmentshader with error " + gl.GetError());

            Console.WriteLine("Creating Program");
            program.ProgramID = gl.CreateProgram();
            gl.AttachShader(program.ProgramID, program.VertexShader.ShaderID);
            gl.AttachShader(program.ProgramID, program.FragmentShader.ShaderID);
            gl.LinkProgram(program.ProgramID);
            Console.WriteLine("Created Program with error " + gl.GetError());

            gl.DeleteShader(program.VertexShader.ShaderID);
            gl.DeleteShader(program.FragmentShader.ShaderID);
        }

        /// <summary>
        /// Initializes a shader program, either by retrieving a pre-built instance if available or creating a new one.
        /// </summary>
        /// <param name="shader">The shader program to initialize.</param>
        /// <returns>The ID of the initialized shader program.</returns>
        private int InitShader(ShaderProgram shader)
        {
            var preBuildShader = this.GetShaderProgram(shader);
            if (preBuildShader != null)
            {
                Console.WriteLine("Shader " + shader.GetType().Name + " found");
                return preBuildShader.ProgramID;
            }
            else
            {
                Console.WriteLine("Shader " + shader.GetType().Name + " created");
                this.CreateShader(shader.GetType().Name, shader);
                this.ShaderPrograms.Add(shader.GetType().Name, shader);
                return shader.ProgramID;
            }
        }

        /// <summary>
        /// Initial the sprite
        /// </summary>
        /// <param name="sprite"></param>
        public void InitSprite(Sprite sprite)
        {

        }

        /// <summary>
        /// Inits the game element
        /// </summary>
        /// <param name="element"></param>
        public void InitGameElement(GameElement element)
        {
            if(element.GetType() == typeof(BufferedSprite))
            {
                InitBufferedSprite((BufferedSprite)element);                
            }
            else if(element.GetType() == typeof(Qube))
            {
                InitCube((Qube) element);
            }
            else if (element.GetType() == typeof(Terrain3D))
            {
                this.InitTerrain3D((Terrain3D)element);
            }
            else if(element.GetType() == typeof(ParticleEmitter))
            {
                this.InitParticleEmitter((ParticleEmitter)element);
            }
            else if(element.GetType() == typeof(Genesis.Core.GameElements.Model))
            {
                this.InitModel((Core.GameElements.Model)element);
            }
            else if(element.GetType() == typeof(Genesis.Core.Light2D))
            {
                //this.InitLight2D((Light2D)element); Using the instanced shape
            }
        }

        /// <summary>
        /// Init the buffered sprite
        /// </summary>
        /// <param name="bufferedSprite"></param>
        private void InitBufferedSprite(BufferedSprite bufferedSprite)
        {
            float[] verticies = bufferedSprite.Verticies.ToArray();
            int vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            bufferedSprite.Propertys.Add("vbo", vbo);

            float[] color = bufferedSprite.Colors.ToArray();
            int cbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.BufferData(OpenGL.ArrayBuffer, color.Length * sizeof(float), color, OpenGL.DynamicDraw);
            bufferedSprite.Propertys.Add("cbo", cbo);

            float[] texCoords = bufferedSprite.TexCoords.ToArray();
            int tex = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, tex);
            gl.BufferData(OpenGL.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, OpenGL.DynamicDraw);
            bufferedSprite.Propertys.Add("tbo", tex);

            bufferedSprite.Propertys.Add("tris", bufferedSprite.Verticies.Count / 3);
        }

        /// <summary>
        /// Initializes a cube object by setting up its associated shader program and loading vertex, color, and normal data into the GPU buffers.
        /// </summary>
        /// <param name="cube">The cube object to initialize.</param>
        private void InitCube(Qube cube)
        {
            // 1. Check if the shader already exist. 
            cube.Propertys.Add("ShaderID", this.InitShader(cube.Shader));

            // 2. Init the texures
            cube.Material.Propeterys.Add("tex_id", this.InitTexture(cube.Material.DiffuseTexture));
            cube.Material.Propeterys.Add("normal_id", this.InitNormalMap(cube.Material.NormalTexture));

            int VAO = gl.GenVertexArrays(1);
            gl.BindVertexArray(VAO);

            // 2. Load verticies into the gpu
            float[] verticies = cube.Shape.GetShape();
            int vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
            cube.Propertys.Add("vbo", vbo);

            // 3. Load colors into the gpu
            float[] color = Qube.GetColors(cube.Material.DiffuseColor);
            int cbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.BufferData(OpenGL.ArrayBuffer, color.Length * sizeof(float), color, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 0);
            cube.Propertys.Add("cbo", cbo);

            // 4. Load the texcoords
            float[] texCords = cube.Shape.GetTextureCoordinates();
            int tbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, tbo);
            gl.BufferData(OpenGL.ArrayBuffer, texCords.Length * sizeof(float), texCords, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);
            cube.Propertys.Add("tbo", tbo);

            float[] normals = cube.Shape.GetNormals();
            int nbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, nbo);
            gl.BufferData(OpenGL.ArrayBuffer, normals.Length * sizeof(float), normals, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(3);
            gl.VertexAttribPointer(3, 3, OpenGL.Float, false, 0, 0);
            cube.Propertys.Add("nbo", nbo);

            cube.Propertys.Add("vao", VAO);
            cube.Propertys.Add("tris", (verticies.Length / 3));
            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Inital an 3D element
        /// </summary>
        /// <param name="element"></param>
        public void InitElement3D(Element3D element)
        {
            //Checks if a shader is set

            if(element.Shader == null)
            {
                element.Propertys.Add("ShaderID", ShaderPrograms["DiffuseShader"].ProgramID);
            }
            else
            {
                element.Propertys.Add("ShaderID", this.InitShader(element.Shader));
            }

            //Inital the Materials
            for (int i = 0; i < element.Materials.Count; i++)
            {
                var material = element.Materials[i];
                var buffers = element.GetMaterialBuffers(material);
                if(buffers.HasData)
                {
                    material.Propeterys.Add("tex_id", this.InitTexture(element.Propertys["path"] + "\\" + material.DiffuseTexture));
                    material.Propeterys.Add("normal_id", this.InitNormalMap(element.Propertys["path"] + "\\" + material.NormalTexture));

                    int vao = gl.GenVertexArrays(1);
                    gl.BindVertexArray(vao);

                    float[] verticies = buffers.Verticies;
                    int vbo = gl.GenBuffer(1);
                    gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
                    gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
                    gl.EnableVertexAttribArray(0);
                    gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
                    material.Propeterys.Add("vbo", vbo);                    

                    gl.DisableVertexAttribArray(1);
  
                    float[] texCoords = buffers.Texcords;
                    int tbo = gl.GenBuffer(1);
                    gl.BindBuffer(OpenGL.ArrayBuffer, tbo);
                    gl.BufferData(OpenGL.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, OpenGL.DynamicDraw);
                    gl.EnableVertexAttribArray(2);
                    gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);
                    material.Propeterys.Add("tbo", tbo);

                    float[] normals = buffers.Normals;
                    int nbo = gl.GenBuffer(1);
                    gl.BindBuffer(OpenGL.ArrayBuffer, nbo);
                    gl.BufferData(OpenGL.ArrayBuffer, normals.Length * sizeof(float), normals, OpenGL.DynamicDraw);
                    gl.EnableVertexAttribArray(3);
                    gl.VertexAttribPointer(3, 3, OpenGL.Float, false, 0, 0);
                    material.Propeterys.Add("nbo", nbo);

                    material.Propeterys.Add("vao", vao);
                    material.Propeterys.Add("tris", verticies.Length / 3);
                    gl.BindVertexArray(0);
                }
            }
        }

        /// <summary>
        /// Initial the diffuse texture for the 3D model
        /// If the texture file isnt existing an empty 1x1 texture get created
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int InitTexture(String path)
        {
            Bitmap bitmap;
            if(File.Exists(path))
            {
                bitmap = (Bitmap)Bitmap.FromFile(path);
            }
            else
            {
                bitmap = Utils.CreateEmptyTexture(1, 1);
            }
            int texid = gl.GenTextures(1);
            gl.BindTexture(NetGL.OpenGL.Texture2D, texid);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMinFilter, NetGL.OpenGL.LinearMipMapLinear);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMagFilter, NetGL.OpenGL.Linear);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.TexImage2D(NetGL.OpenGL.Texture2D, 0, NetGL.OpenGL.RGBA, bitmap.Width, bitmap.Height, 0, NetGL.OpenGL.BGRAExt, NetGL.OpenGL.UnsignedByte, bitmap);
            gl.GenerateMipMap(OpenGL.Texture2D);
            int errorID = gl.GetError();
            if (errorID != 0)
            {
                Console.WriteLine("2 Error while loading the texture. Error Code " + errorID);
            }
            return texid;
        }

        /// <summary>
        /// Inital the normal map for an 3D element
        /// if the normal map file isnt existing an empty 1x1 normal map get created
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int InitNormalMap(String path)
        {
            Bitmap bitmap;
            if (File.Exists(path))
            {
                bitmap = (Bitmap)Bitmap.FromFile(path);
            }
            else
            {
                bitmap = Utils.CreateEmptyNormalMap(1, 1);
            }
            int texid = gl.GenTextures(1);
            gl.BindTexture(NetGL.OpenGL.Texture2D, texid);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMinFilter, NetGL.OpenGL.LinearMipMapLinear);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureMagFilter, NetGL.OpenGL.Linear);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.TexImage2D(NetGL.OpenGL.Texture2D, 0, NetGL.OpenGL.RGBA, bitmap.Width, bitmap.Height, 0, NetGL.OpenGL.BGRAExt, NetGL.OpenGL.UnsignedByte, bitmap);
            gl.GenerateMipMap(OpenGL.Texture2D);
            int errorID = gl.GetError();
            if (errorID != 0)
            {
                Console.WriteLine("2 Error while loading the texture. Error Code " + errorID);
            }
            return texid;
        }



        /// <summary>
        /// Beginn to draw
        /// </summary>
        public void Begin()
        {
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        /// <summary>
        /// Draws a rect with no fill
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public void DrawRect(Rect rect, Color color)
        {
            DrawRect(rect, color, 1.0f);
        }

        /// <summary>
        /// Draws a rect with no fill
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="borderWidth"></param>
        public void DrawRect(Rect rect, Color color, float borderWidth)
        {
            var shader = ShaderPrograms["MVPRectShader"].ProgramID;
            var fcolor = Utils.ConvertColor(color, true);
            var shape = InstancedShapes["RectShape"];

            gl.Disable(OpenGL.DepthTest);
            //Creates the modelview matrix
            mat4 mt_mat = mat4.Translate(rect.X, rect.Y, 0.0f);
            mat4 mr_mat = mat4.RotateZ(0.0f);
            mat4 ms_mat = mat4.Scale(rect.Width, rect.Height, 0.0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.UseProgram(shader);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shader, "mvp"), 1, false, mvp.ToArray());
            gl.Uniform1f(gl.GetUniformLocation(shader, "aspect"), rect.Width / rect.Height);
            gl.Uniform1f(gl.GetUniformLocation(shader, "border_width"), borderWidth);
            gl.Uniform4f(gl.GetUniformLocation(shader, "color"), fcolor[0], fcolor[1], fcolor[2], fcolor[3]);

            //Load the vertex buffer and binds them
            int vertexBuffer = shape.vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Enable(OpenGL.DepthTest);
            //Console.WriteLine(gl.GetError());
        }

        /// <summary>
        /// Renders an filled circle
        /// </summary>
        /// <param name="center">Center of the circle</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="color">Color of the circle</param>
        public void FillCircle(Vec3 center, float radius, Color color)
        {
            var shader = ShaderPrograms["SolidShapeShader"].ProgramID;
            var fcolor = Utils.ConvertColor(color, true);
            var shape = (CircleShape) InstancedShapes["CircleShape"];

            gl.Disable(OpenGL.DepthTest);

            //Creates the modelview matrix
            mat4 mt_mat = mat4.Translate(center.X, center.Y, 0.0f);
            mat4 mr_mat = mat4.RotateZ(0);
            mat4 ms_mat = mat4.Scale(radius * 2, radius * 2, 0.0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            mat4 mvp = p_mat * v_mat * m_mat;

            //Send data to the shader progra,,
            gl.UseProgram(shader);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shader, "mvp"), 1, false, mvp.ToArray());
            gl.Uniform4f(gl.GetUniformLocation(shader, "color"), fcolor[0], fcolor[1], fcolor[2], fcolor[3]);

            //Load the vertex buffer and binds them
            gl.BindBuffer(OpenGL.ArrayBuffer, shape.vbo);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
            gl.DrawArrays(OpenGL.TriangleFan, 0, shape.Segments + 2);

            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Draws an circle
        /// </summary>
        /// <param name="center">Location for the circle</param>
        /// <param name="radius">Radius for the circle</param>
        /// <param name="color">Color for the circle</param>
        /// <param name="borderWidth">Border width for the circle</param>
        public void DrawCircle(Vec3 center, float radius, Color color, float borderWidth)
        {
            var shader = ShaderPrograms["BorderCircleShader"].ProgramID;
            var fcolor = Utils.ConvertColor(color);
            var shape = (CircleShape)InstancedShapes["CircleShape"];

            gl.Disable(OpenGL.DepthTest);

            //Creates the modelview matrix
            mat4 mt_mat = mat4.Translate(center.X, center.Y, 0.0f);
            mat4 mr_mat = mat4.RotateZ(0);
            mat4 ms_mat = mat4.Scale(radius * 2, radius * 2, 0.0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            mat4 mvp = p_mat * v_mat * m_mat;

            //Send data to the shader progra,,
            gl.UseProgram(shader);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shader, "mvp"), 1, false, mvp.ToArray());
            gl.UniformMatrix4fv(gl.GetUniformLocation(shader, "m_mat"), 1, false, m_mat.ToArray());
            gl.Uniform3f(gl.GetUniformLocation(shader, "color"), fcolor[0], fcolor[1], fcolor[2]);
            gl.Uniform1f(gl.GetUniformLocation(shader, "radius"), radius);
            gl.Uniform1f(gl.GetUniformLocation(shader, "border_width"), borderWidth);

            //Load the vertex buffer and binds them
            gl.BindBuffer(OpenGL.ArrayBuffer, shape.vbo);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
            gl.DrawArrays(OpenGL.TriangleFan, 0, shape.Segments + 2);

            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        public void DrawLine(Vec3 from, Vec3 to, Color color)
        {
            int shaderID = this.ShaderPrograms["WireframeShader"].ProgramID;
            int vbo = this.InstancedShapes["LineShape"].vbo;
            gl.UseProgram(shaderID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "projection"), 1, false, p_mat.ToArray());
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "view"), 1, false, v_mat.ToArray());

            float[] bufferData =
            {
                from.X, from.Y, from.Z,
                to.X, to.Y, to.Z
            };
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, bufferData.Length * sizeof(float), bufferData, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
            gl.DrawArrays(OpenGL.Lines, 0, 2);
        }

        /// <summary>
        /// Renders the GameElement
        /// </summary>
        /// <param name="element"></param>
        public void DrawGameElement(GameElement element)
        {
            if(element.GetType() == typeof(Qube))
            {
                this.DrawCube((Qube)element);
            }
            else if(element.GetType() == typeof(Terrain3D))
            {
                this.RenderTerrain3D((Terrain3D)element);
            }
            else if(element.GetType() == typeof(ParticleEmitter))
            {
                this.DrawParticleEmitter((ParticleEmitter)element);
            }
            else if (element.GetType() == typeof(Genesis.Core.GameElements.Model))
            {
                this.DrawModel((Genesis.Core.GameElements.Model)element);
            }
            else if (element.GetType() == typeof(Genesis.Core.Light2D))
            {
                this.RenderLight2D((Light2D)element);
            }
        }

        /// <summary>
        /// Draws a sprite
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="texture"></param>
        public void DrawSprite(Vec3 location, Vec3 size, Texture texture)
        {
            DrawSprite(location, size, Color.White, texture);
        }

        /// <summary>
        /// Draws a sprite
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="texture"></param>
        /// <param name="texCoords"></param>
        public void DrawSprite(Vec3 location, Vec3 size, Texture texture, TexCoords texCoords)
        {
            DrawSprite(location, size, Color.White, texture, texCoords);
        }

        /// <summary>
        /// Renders a sprite
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="texture"></param>
        public void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture)
        {
            gl.Disable(OpenGL.DepthTest);
            //Create the modelview matrix
            mat4 mt_mat = mat4.Translate(location.X, location.Y, location.Z);
            mat4 mr_mat = mat4.RotateZ(0f);
            mat4 ms_mat = mat4.Scale(size.X, size.Y, size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["SpriteShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            //Load the texture and send it to the shader
            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "textureSampler"), 0);

            //Load the vertex buffer and set the new tex coords
            int vertexBuffer = this.InstancedShapes["SpriteShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            //Send the color data to the shader
            float[] colors = GetColorArray(color);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), colors.Length * sizeof(float), colors);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            //Create the new text coords and send them to the shader
            float[] textCoordsf = this.GetSpriteBaseTexCoords();
            gl.BufferSubData(OpenGL.ArrayBuffer, 36 * sizeof(float), textCoordsf.Length * sizeof(float), textCoordsf);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            //Draw the Sprite
            gl.DrawArrays(OpenGL.Triangles, 0, 6);

            gl.Disable(NetGL.OpenGL.Texture2D);
            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Renders a sprite with modern gl
        /// </summary>
        /// <param name="sprite"></param>
        public void DrawSprite(Sprite sprite)
        {
            gl.Disable(OpenGL.DepthTest);
            //Create the modelview matrix
            mat4 mt_mat = mat4.Translate(sprite.Location.X, sprite.Location.Y, sprite.Location.Z);
            mat4 mr_mat = mat4.RotateZ(Utils.ToRadians(sprite.Rotation.Z));
            mat4 ms_mat = mat4.Scale(sprite.Size.X, sprite.Size.Y, sprite.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["SpriteShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            //Load the texture and send it to the shader
            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(NetGL.OpenGL.Texture2D, sprite.Texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "textureSampler"), 0);

            //Load the vertex buffer and set the new tex coords
            int vertexBuffer = this.InstancedShapes["SpriteShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);
            
            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            //Send the color data to the shader
            float[] color = this.GetSpriteColor(sprite);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), color.Length * sizeof(float), color);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            //Create the tex coords and send them to the buffer
            float[] textCoordsf = this.GetSpriteTexCords(sprite);
            gl.BufferSubData(OpenGL.ArrayBuffer, 36 * sizeof(float), textCoordsf.Length * sizeof(float), textCoordsf);            
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            //Draw the sprite
            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Disable(NetGL.OpenGL.Texture2D);
            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Draws a Sprite
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="texture"></param>
        /// <param name="texCoords"></param>
        public void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture, TexCoords texCoords)
        {
            gl.Disable(OpenGL.DepthTest);
            //Create the modelview matrix
            mat4 mt_mat = mat4.Translate(location.X, location.Y, location.Z);
            mat4 mr_mat = mat4.RotateZ(0f);
            mat4 ms_mat = mat4.Scale(size.X, size.Y, size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["SpriteShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            //Load the texture and send it to the shader
            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "textureSampler"), 0);

            //Load the vertex buffer and set the new tex coords
            int vertexBuffer = this.InstancedShapes["SpriteShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            //Send the color data to the shader
            float[] colorf = this.GetColorArray(color);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), colorf.Length * sizeof(float), colorf);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            //Create the tex coords and send them to the buffer
            float[] textCoordsf = this.GetTexCords(texCoords);
            gl.BufferSubData(OpenGL.ArrayBuffer, 36 * sizeof(float), textCoordsf.Length * sizeof(float), textCoordsf);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            //Draw the sprite
            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Disable(NetGL.OpenGL.Texture2D);
            gl.Enable(OpenGL.DepthTest);
        }

        public void DrawBufferedSprite(BufferedSprite bufferedSprite)
        {
            gl.Disable(OpenGL.DepthTest);
            //Create the modelview matrix
            mat4 mt_mat = mat4.Translate(bufferedSprite.Location.X, bufferedSprite.Location.Y, bufferedSprite.Location.Z);
            mat4 mr_mat = mat4.RotateZ(0f);
            mat4 ms_mat = mat4.Scale(1f, 1f, 1f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["SpriteShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            //Load the texture and send it to the shader
            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(NetGL.OpenGL.Texture2D, bufferedSprite.Texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "textureSampler"), 0);

            //Load the vertex buffer and set the new tex coords
            
            //Send the vertex data to the shader
            
            int vertexBuffer = (int)bufferedSprite.Propertys["vbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            //Send the color data to the shader
            int cbo = (int)bufferedSprite.Propertys["cbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 0);

            //Create the tex coords and send them to the buffer
            int tbo = (int)bufferedSprite.Propertys["tbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, tbo);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);

            //Draw the sprite
            gl.DrawArrays(OpenGL.Triangles, 0, (int)bufferedSprite.Propertys["tris"]);
            gl.Disable(NetGL.OpenGL.Texture2D);
            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Draws the vector array
        /// </summary>
        /// <param name="vecs"></param>
        /// <param name="color"></param>
        public void DrawVectors(Vec3[] vecs, Color color)
        {

        }

        /// <summary>
        /// Ends the rendering
        /// </summary>
        public void End()
        {
            gl.Flush();
            gl.SwapLayerBuffers(NetGL.OpenGL.SwapMainPlane);
        }

        /// <summary>
        /// Fills a rectangle with the given color. 
        /// LegacyGL
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public void FillRect(Rect rect, Color color)
        {
            var shader = ShaderPrograms["SolidShapeShader"].ProgramID;
            var fcolor = Utils.ConvertColor(color, true);
            var shape = InstancedShapes["RectShape"];

            gl.Disable(OpenGL.DepthTest);
            //Creates the modelview matrix
            mat4 mt_mat = mat4.Translate(rect.X, rect.Y, 0.0f);
            mat4 mr_mat = mat4.RotateZ(0.0f);
            mat4 ms_mat = mat4.Scale(rect.Width, rect.Height, 0.0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.UseProgram(shader);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shader, "mvp"), 1, false, mvp.ToArray());
            gl.Uniform4f(gl.GetUniformLocation(shader, "color"), fcolor[0], fcolor[1], fcolor[2], fcolor[3]);

            //Load the vertex buffer and binds them
            int vertexBuffer = shape.vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Enable(OpenGL.DepthTest);
            //Console.WriteLine(gl.GetError());
        }

        /// <summary>
        /// Loads a texture into the vram
        /// </summary>
        /// <param name="texture"></param>
        public void LoadTexture(Texture texture)
        {
            Console.WriteLine("Trying to load the texture " + texture.Name);
            if (texture.RenderID == 0)
            {
                //texture.RenderID = NetGL.Toolkit.TextureImporter.BindTexture(gl, NetGL.OpenGL., texture.Bitnmap);
                texture.RenderID = GenerateTexture(gl, texture);
                int errorID = gl.GetError();
                if (errorID != 0)
                {
                    Console.WriteLine("Test Error while loading the texture. Error Code " + errorID);
                }
                else
                {
                    Console.WriteLine("Texture " + texture.Name + " loaded");
                }
            }
        }

        /// <summary>
        /// Loads the the font
        /// </summary>
        /// <param name="font"></param>
        public void LoadFont(Font font)
        {
            Console.WriteLine("Trying to load the font");
            if (font.RenderID == 0)
            {
                //font.RenderID = NetGL.Toolkit.TextureImporter.BindTexture(gl, NetGL.OpenGL.BGRAExt, font.FontAtlas);
                font.RenderID = GenerateTexture(gl, font.FontAtlas);
                int errorID = gl.GetError();
                if (errorID != 0)
                {
                    Console.WriteLine("Error while loading the font. Error Code " + errorID);
                }
                else
                {
                    Console.WriteLine("font loaded");
                }
            }
        }

        /// <summary>
        /// This function will not be used within this render device
        /// </summary>
        public void ModelViewMatrix()
        {
            //gl.MatrixMode(NetGL.OpenGL.ModelView);
            //gl.LoadIdentity();
        }

        /// <summary>
        /// This function will not be used within this render device
        /// </summary>
        public void PopMatrix()
        {
            //gl.PopMatrix();
        }

        /// <summary>
        /// This function will not be used within this render device
        /// </summary>
        public void ProjectionMatrix()
        {
            //gl.MatrixMode(NetGL.OpenGL.Projection);
        }

        /// <summary>
        /// This function will not be used within this render device
        /// </summary>
        public void PushMatrix()
        {
            //gl.PushMatrix();
        }

        /// <summary>
        /// This function will not be used within this render device
        /// </summary>
        public void Rotate(float angle, Vec3 vector)
        {
            //gl.Rotate(angle, vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Sets the viewport for the rendering
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetViewport(Viewport viewport)
        {
            gl.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            UpdateFramebufferSize(this.sceneBuffer, (int)viewport.Width, (int)viewport.Height);
            UpdateFramebufferSize(this.uiBuffer, (int)viewport.Width, (int)viewport.Height);

            if(m_viewport == null || m_viewport != viewport)
            {
                m_viewport = viewport;
            }
        }

        /// <summary>
        /// Set the Projection and view matrices
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(Viewport viewport, Camera camera)
        {
            float correction = camera.CalculateScreenCorrection(viewport);

            if (camera.Type == CameraType.Ortho)
            {
                float halfWidth = (viewport.Width / 2) / correction;
                float halfHeight = (viewport.Height / 2) / correction;

                float left = camera.Location.X - halfWidth;
                float right = camera.Location.X + halfWidth;
                float bottom = camera.Location.Y - halfHeight;
                float top = camera.Location.Y + halfHeight;

                p_mat = mat4.Ortho(left, right, bottom, top, 0.1f, 100.0f);
                v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));
            }
            else
            {
                float aspectRatio = (viewport.Width * correction) / (viewport.Height * correction);
                vec3 cameraPosition = camera.Location.ToGlmVec3();
                Vec3 cameraFront = Utils.CalculateCameraFront2(camera);

                p_mat = mat4.Perspective(Utils.ToRadians(camera.FOV), aspectRatio, camera.Near, camera.Far);
                v_mat =  mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));
            }

            if(this.camera == null || this.camera != camera)
            {
                this.camera = camera;
            }
        }

        /// <summary>
        /// Sets the mvp matrix for ui rendering
        /// </summary>
        private void SetUIMatrices()
        {
            float x = 0;
            float y = 0;
            float top = y + m_viewport.Height;
            float right = x + m_viewport.Width;

            p_mat = mat4.Ortho(x, right, y, top, 0.0f, 100.0f);
            v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));
        }

        /// <summary>
        /// This function will not be used within this render device
        /// </summary>
        public void Translate(Vec3 vector)
        {
            //gl.Translate(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// This function will not be used within this render device
        /// </summary>
        public void Translate(float x, float y, float z)
        {
            //gl.Translate(x, y, z);
        }

        /// <summary>
        /// Sets the texture repeatT
        /// </summary>
        public void TextureRepeatT()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
        }

        /// <summary>
        /// Sets the texture repeatS
        /// </summary>
        public void TextureRepeatS()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
        }

        /// <summary>
        /// Sets the texture clampT
        /// </summary>
        public void TextureClampT()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Clamp);
        }

        /// <summary>
        /// Sets the texture clampS
        /// </summary>
        public void TextureClampS()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Clamp);
        }

        /// <summary>
        /// Draws a texture
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="repeateX"></param>
        /// <param name="repeatY"></param>
        /// <param name="texture"></param>
        public void DrawTexture(Vec3 location, Vec3 size, float repeateX, float repeatY, Texture texture)
        {
            gl.Disable(OpenGL.DepthTest);

            mat4 mt_mat = mat4.Translate(location.X, location.Y, location.Z);
            mat4 mr_mat = mat4.RotateZ(0f);
            mat4 ms_mat = mat4.Scale(size.X, size.Y, size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            mat4 mvp = p_mat * v_mat * m_mat;

            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["SpriteShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["SpriteShader"].ProgramID, "textureSampler"), 0);

            int vertexBuffer = this.InstancedShapes["SpriteShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            float[] color = this.GetColorArray(Color.White);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), color.Length * sizeof(float), color);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            float[] textCoordsf =
            {
                0.0f, 0.0f,
                0.0f, 1.0f * repeatY,
                1.0f * repeateX, 1.0f * repeatY,

                0.0f, 0.0f,
                1.0f * repeateX, 1.0f * repeatY,
                1.0f * repeateX, 0.0f
            };
            gl.BufferSubData(OpenGL.ArrayBuffer, 36 * sizeof(float), textCoordsf.Length * sizeof(float), textCoordsf);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Disable(NetGL.OpenGL.Texture2D);
            gl.Enable(OpenGL.DepthTest);

        }

        /// <summary>
        /// Renders a String
        /// </summary>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="fontSize"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        public void DrawString(String text, Vec3 location, float fontSize, Font font, Color color)
        {
            this.DrawString(text, location, fontSize, font.Spacing, font, color);
        }

        /// <summary>
        /// Renders a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="fontSize"></param>
        /// <param name="spacing"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        public void DrawString(String text, Vec3 location, float fontSize, float spacing, Font font, Color color)
        {
            gl.Disable(OpenGL.DepthTest);
            String[] strings = text.Split('\n');
            int y = 0;

            //Console.WriteLine(location.ToString());
            float stringWith = Utils.GetStringWidth(text, fontSize, spacing);
            float stringHeight = Utils.GetStringHeight(text, fontSize, 0f);

            gl.Enable(OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["MVPShader"].ProgramID);
            gl.BindTexture(NetGL.OpenGL.Texture2D, font.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["MVPShader"].ProgramID, "textureSampler"), 0);

            foreach (var str in strings)
            {
                int i = 0;
                foreach (Char c in str)
                {
                    float _spacing = fontSize * spacing;
                    float xOffset = i * fontSize - (i * _spacing);
                    float yOffset = y * fontSize + (y * 0);
                    /// Font height /4 ist ein test
                    RenderGlyphe(new Vec3(location.X - (stringWith / 2) + (fontSize / 2) + xOffset, location.Y - (stringHeight / 2) + (fontSize / 4) + yOffset, 0.0f), font, fontSize, c, color);
                    i++;
                }
                y++;
            }
            gl.Disable(NetGL.OpenGL.Texture2D);
            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Renders a glyphe from a string
        /// </summary>
        /// <param name="location"></param>
        /// <param name="font"></param>
        /// <param name="size"></param>
        /// <param name="character"></param>
        private void RenderGlyphe(Vec3 location, Font font, float size, Char character, Color color)
        {
            Glyphe glyphe = font.GetGlyphe(character);
            if (glyphe == null) return;
            float row = glyphe.Row;
            float col = glyphe.Column;
            float rowVal = 1 / ((float)font.Rows + 1);
            float colVal = 1 / ((float)font.Column + 1);
            float xLeft = col * colVal;
            float xRight = xLeft + colVal;
            float yTop = row * rowVal;
            float yBottom = yTop + rowVal;

            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;

            mat4 mt_mat = mat4.Translate(location.X, location.Y, location.Z);
            mat4 mr_mat = mat4.RotateZ(0f);
            mat4 ms_mat = mat4.Scale(size, size, 0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            mat4 mvp = p_mat * v_mat * m_mat;

            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["MVPShader"].ProgramID, "mvp"), 1, false, mvp.ToArray<float>());

            int vertexBuffer = InstancedShapes["GlypheShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            float[] newColor = this.GetColorArray(color);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), newColor.Length * sizeof(float), newColor);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            float[] texCords = {
                xLeft, yBottom,
                xLeft, yTop,
                xRight, yTop,

                xLeft, yBottom,
                xRight, yTop,
                xRight, yBottom
            };
            gl.BufferSubData(OpenGL.ArrayBuffer, 36 * sizeof(float), texCords.Length * sizeof(float), texCords);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            gl.DrawArrays(OpenGL.Triangles, 0, 6);
        }

        /// <summary>
        /// Draws a mesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="color"></param>
        public void DrawMesh(Mesh mesh, Color color)
        {

        }

        /// <summary>
        /// Returns an error code from the render device
        /// </summary>
        /// <returns></returns>
        public int GetError()
        {
            return gl.GetError();
        }

        /// <summary>
        /// Returns the handle of the render target
        /// </summary>
        /// <returns></returns>
        public IntPtr GetHandle()
        {
            return this.hwnd;
        }

        /// <summary>
        /// Generates a texture
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public int GenerateTexture(OpenGL gl, Texture texture)
        {
            int textureint = this.GenerateTexture(gl, texture.Bitnmap);
            return textureint;
        }

        /// <summary>
        /// Generates a texture
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public int GenerateTexture(OpenGL gl, Bitmap texture)
        {
            int textureint = gl.GenTextures(1);
            gl.BindTexture(OpenGL.Texture2D, textureint);
            int errorID = gl.GetError();
            if (errorID != 0)
            {
                Console.WriteLine("Error while binding the texture. Error Code " + errorID);
            }
            gl.TexParameteri(OpenGL.Texture2D, OpenGL.TextureMagFilter, OpenGL.Linear);
            gl.TexParameteri(OpenGL.Texture2D, OpenGL.TextureMinFilter, OpenGL.Linear);
            gl.TexParameteri(OpenGL.Texture2D, OpenGL.TextureWrapS, OpenGL.Clamp);
            gl.TexParameteri(OpenGL.Texture2D, OpenGL.TextureWrapT, OpenGL.Clamp);
            gl.TexImage2D(OpenGL.Texture2D, 0, OpenGL.RGBA, texture.Width, texture.Height, 0, OpenGL.BGRA, OpenGL.UnsignedByte, texture);
            errorID = gl.GetError();
            if (errorID != 0)
            {
                Console.WriteLine("2 Error while loading the texture. Error Code " + errorID);
            }
            return textureint;
        }

        /// <summary>
        /// Returns an color array
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private float[] GetColorArray(Color color)
        {
            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;

            float[] newColor =
            {
                r, g, b,
                r, g, b,
                r, g, b,

                r, g, b,
                r, g, b,
                r, g, b
            };
            return newColor;
        }

        /// <summary>
        /// Returns an array with the color data for a sprite
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        private float[] GetSpriteColor(Sprite sprite)
        {
            return this.GetColorArray(sprite.Color);
        }

        /// <summary>
        /// Returns the texture coords from the given sprite
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        private float[] GetSpriteTexCords(Sprite sprite)
        {
            float[] textCoordsf =
{
                // Erstes Dreieck
                sprite.TexCoords.BottomLeft.X, sprite.TexCoords.BottomLeft.Y,
                sprite.TexCoords.TopLeft.X, sprite.TexCoords.TopLeft.Y,
                sprite.TexCoords.TopRight.X, sprite.TexCoords.TopRight.Y,

                sprite.TexCoords.BottomLeft.X, sprite.TexCoords.BottomLeft.Y,
                sprite.TexCoords.TopRight.X, sprite.TexCoords.TopRight.Y,
                sprite.TexCoords.BottomRight.X, sprite.TexCoords.BottomRight.Y,
            };
            return textCoordsf;
        }

        /// <summary>
        /// Gets an array with the texture coords
        /// </summary>
        /// <param name="cords"></param>
        /// <returns></returns>
        private float[] GetTexCords(TexCoords cords)
        {
            float[] textCoordsf =
{
                // Erstes Dreieck
                cords.BottomLeft.X, cords.BottomLeft.Y,
                cords.TopLeft.X, cords.TopLeft.Y,
                cords.TopRight.X, cords.TopRight.Y,

                cords.BottomLeft.X, cords.BottomLeft.Y,
                cords.TopRight.X, cords.TopRight.Y,
                cords.BottomRight.X, cords.BottomRight.Y,
            };
            return textCoordsf;
        }

        /// <summary>
        /// Returns the basic shape tex coords for a sprite
        /// </summary>
        /// <returns></returns>
        private float[] GetSpriteBaseTexCoords()
        {
            float[] textCoordsf =
            {
                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f,

                0.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 1.0f
            };
            return textCoordsf;
        }

        /// <summary>
        /// Draws an 3D element within the scene
        /// </summary>
        /// <param name="element"></param>
        public void DrawElement3D(Element3D element)
        {
            int elementShaderID = (int)element.Propertys["ShaderID"];

            mat4 mt_mat = Utils.GetModelTransformation(element);
            mat4 mr_mat = Utils.GetModelRotation(element);
            mat4 ms_mat = Utils.GetModelScale(element);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            mat4 mvp = p_mat * v_mat * m_mat;

            gl.Enable(OpenGL.Texture2D);
            foreach (var material in element.Materials) 
            {
                if (material.Propeterys.ContainsKey("vbo"))
                {
                    var materialColor = Utils.ConvertColor(material.DiffuseColor, true);
                    gl.UseProgram(elementShaderID);
                    gl.UniformMatrix4fv(gl.GetUniformLocation(elementShaderID, "mvp"), 1, false, mvp.ToArray());
                    gl.Uniform4f(gl.GetUniformLocation(elementShaderID, "materialColor"), materialColor[0], materialColor[1], materialColor[2], materialColor[3]);

                    if (this.lightSource != null)
                    {
                        Vec3 ligtDirection = lightSource.GetLightDirection(camera);
                        Vec3 lightColor = lightSource.GetLightColor();
                        gl.Uniform3f(gl.GetUniformLocation(elementShaderID, "lightPos"), lightSource.Location.X, lightSource.Location.Y, lightSource.Location.Z);
                        gl.Uniform1f(gl.GetUniformLocation(elementShaderID, "lightIntensity"), lightSource.Intensity);
                        gl.Uniform3f(gl.GetUniformLocation(elementShaderID, "lightColor"), lightColor.X, lightColor.Y, lightColor.Z);
                    }

                    gl.ActiveTexture(OpenGL.Texture0);
                    gl.BindTexture(OpenGL.Texture2D, (int)material.Propeterys["tex_id"]);
                    gl.Uniform1I(gl.GetUniformLocation(elementShaderID, "textureSampler"), 0);

                    gl.ActiveTexture(OpenGL.Texture1);
                    gl.BindTexture(OpenGL.Texture2D, (int)material.Propeterys["normal_id"]);
                    gl.Uniform1I(gl.GetUniformLocation(elementShaderID, "normalMap"), 1);

                    gl.BindVertexArray((int)material.Propeterys["vao"]);
                    gl.DrawArrays(OpenGL.Triangles, 0, (int)material.Propeterys["tris"]);
                    gl.BindVertexArray(0);
                }
            }
            gl.ActiveTexture(OpenGL.Texture0);
            gl.Disable(OpenGL.Texture2D);
            //Console.WriteLine(gl.GetError());
        }

        /// <summary>
        /// Returns the shader programm with the typeof the refProgram. 
        /// Returns null is no program found
        /// </summary>
        /// <param name="refProgram"></param>
        /// <returns></returns>
        public ShaderProgram GetShaderProgram(ShaderProgram refProgram)
        {
            foreach(var program in ShaderPrograms)
            {
                if(program.Value.GetType() == refProgram.GetType())
                {
                    return program.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Dispose the 3D element
        /// </summary>
        /// <param name="element"></param>
        public void DisposeElement3D(Element3D element)
        {
            Console.WriteLine("Disposing Element3D: " + element.Name);
            //Dispose the textures
            foreach(var material in element.Materials)
            {
                if(material.Propeterys.ContainsKey("vbo"))
                {
                    gl.DeleteTextures(1, (int)material.Propeterys["tex_id"]);
                    gl.DeleteTextures(1, (int)material.Propeterys["normal_id"]);
                    gl.DeleteBuffers(1, (int)material.Propeterys["vbo"]);
                    gl.DeleteBuffers(1, (int)material.Propeterys["tbo"]);
                    gl.DeleteBuffers(1, (int)material.Propeterys["nbo"]);
                }
            }
            Console.WriteLine("Element3D: " + element.Name + " Disposed");
        }

        /// <summary>
        /// Returns the native renderer
        /// </summary>
        /// <returns></returns>
        public OpenGL GetRenderer()
        {
            return this.gl;
        }

        /// <summary>
        /// Sets an Lightsource
        /// </summary>
        /// <param name="light"></param>
        public void SetLightSource(Light light)
        {
            this.lightSource = light;
        }

        /// <summary>
        /// Draws an skybox
        /// </summary>
        /// <param name="skybox"></param>
        public void DrawSkyBox(Skybox skybox)
        {
            int elementShaderID = (int)skybox.Propertys["ShaderID"];

            mat4 mt_mat = mat4.Translate(skybox.Location.X, skybox.Location.Y, skybox.Location.Z);
            mat4 mr_mat = Utils.GetModelRotation(skybox);
            mat4 ms_mat = mat4.Scale(skybox.Size.X, skybox.Size.Y, skybox.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            mat4 mvp = p_mat * v_mat * m_mat;

            gl.Enable(OpenGL.Texture2D);
            foreach (var material in skybox.Materials)
            {
                if (material.Propeterys.ContainsKey("vbo"))
                {
                    gl.UseProgram(elementShaderID);
                    gl.UniformMatrix4fv(gl.GetUniformLocation(elementShaderID, "mvp"), 1, false, mvp.ToArray());

                    if (this.lightSource != null)
                    {
                        Vec3 ligtDirection = lightSource.GetLightDirection(camera);
                        Vec3 lightColor = lightSource.GetLightColor();
                        gl.Uniform3f(gl.GetUniformLocation(elementShaderID, "lightPos"), lightSource.Location.X, lightSource.Location.Y, lightSource.Location.Z);
                        gl.Uniform1f(gl.GetUniformLocation(elementShaderID, "lightIntensity"), lightSource.Intensity);
                        gl.Uniform3f(gl.GetUniformLocation(elementShaderID, "lightColor"), lightColor.X, lightColor.Y, lightColor.Z);
                    }

                    gl.ActiveTexture(OpenGL.Texture0);
                    gl.BindTexture(OpenGL.Texture2D, (int)material.Propeterys["tex_id"]);
                    gl.Uniform1I(gl.GetUniformLocation(elementShaderID, "textureSampler"), 0);

                    gl.ActiveTexture(OpenGL.Texture1);
                    gl.BindTexture(OpenGL.Texture2D, (int)material.Propeterys["normal_id"]);
                    gl.Uniform1I(gl.GetUniformLocation(elementShaderID, "normalMap"), 1);

                    gl.EnableVertexAttribArray(0);
                    gl.BindBuffer(OpenGL.ArrayBuffer, (int)material.Propeterys["vbo"]);
                    gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

                    gl.EnableVertexAttribArray(2);
                    gl.BindBuffer(OpenGL.ArrayBuffer, (int)material.Propeterys["tbo"]);
                    gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);

                    gl.EnableVertexAttribArray(3);
                    gl.BindBuffer(OpenGL.ArrayBuffer, (int)material.Propeterys["nbo"]);
                    gl.VertexAttribPointer(3, 3, OpenGL.Float, false, 0, 0);

                    gl.DrawArrays(OpenGL.Triangles, 0, (int)material.Propeterys["tris"]);
                }
            }

            gl.Disable(OpenGL.Texture2D);
        }

        /// <summary>
        /// Prepares the renderer for scene rendering
        /// </summary>
        /// <param name="scene">The scene to render</param>
        public void PrepareSceneRendering(Scene scene)
        {
            gl.BindFramebuffer(OpenGL.FrameBuffer, sceneBuffer.FramebufferID);
            gl.Enable(OpenGL.DepthTest);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.One, OpenGL.OneMinusSrcAlpha);
            gl.ClearColor(0f, 0f, 0f, 0f);
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        /// <summary>
        /// Finish the scene rendering
        /// </summary>
        /// <param name="scene">The scene which getted rendered</param>
        public void FinishSceneRendering(Scene scene)
        {
            gl.BindFramebuffer(OpenGL.FrameBuffer, 0);
            gl.Disable(OpenGL.DepthTest);

            gl.Enable(OpenGL.FramebufferSRGB);
            if(scene.BackgroundTexture != null)
            {
                DrawFramebuffer(scene.BackgroundTexture.RenderID, ShaderPrograms["SceneShader"].ProgramID, m_renderSettings.gamma);
            }
            DrawFramebuffer(sceneBuffer.Texture, ShaderPrograms["SceneShader"].ProgramID, m_renderSettings.gamma);
            gl.Disable(OpenGL.FramebufferSRGB);

            gl.Enable(OpenGL.DepthTest);
        }

        public void PrepareLightmap2D(Scene scene, Framebuffer framebuffer)
        {
            Scene2D scene2D = (Scene2D)scene;
            this.UpdateFramebufferSize(framebuffer, (int)m_viewport.Width, (int)m_viewport.Height);
            gl.BindFramebuffer(OpenGL.FrameBuffer, framebuffer.FramebufferID);
            gl.Enable(OpenGL.DepthTest);
            gl.Enable(OpenGL.Blend);
            gl.ClearColor(0, 0, 0, scene2D.LightmapIntensity);
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        public void FinishLightmap2D(Scene scene, Framebuffer framebuffer)
        {
            gl.BindFramebuffer(OpenGL.FrameBuffer, 0);
            gl.Disable(OpenGL.DepthTest);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.DstColor, OpenGL.OneMinusSrcAlpha); // Hier evtl fehler 
            DrawFramebuffer(framebuffer.Texture);

            //Test
            gl.BlendFunc(OpenGL.One, OpenGL.OneMinusSrcAlpha);

            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Prepares the renderer for the canvas rendering
        /// </summary>
        /// <param name="scene">The scene from the canvas</param>
        /// <param name="canvas">The canvas to render</param>
        public void PrepareCanvasRendering(Scene scene, Canvas canvas)
        {
            SetUIMatrices();
            gl.BindFramebuffer(OpenGL.FrameBuffer, uiBuffer.FramebufferID);
            gl.Enable(OpenGL.DepthTest);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.One, OpenGL.OneMinusSrcAlpha);
            gl.ClearColor(0f, 0f, 0f, 0f);
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        /// <summary>
        /// Finish the canvas rendering
        /// </summary>
        /// <param name="scene">The scene from the canvas</param>
        /// <param name="canvas">The canvas to render</param>
        public void FinishCanvasRendering(Scene scene, Canvas canvas)
        {
            SetCamera(m_viewport, this.camera);
            gl.BindFramebuffer(OpenGL.FrameBuffer, 0);
            gl.Disable(OpenGL.DepthTest);

            // use the gamma correction also on the canvas.
            gl.Enable(OpenGL.FramebufferSRGB);
            DrawFramebuffer(uiBuffer.Texture, ShaderPrograms["SceneShader"].ProgramID, m_renderSettings.gamma);
            gl.Disable(OpenGL.FramebufferSRGB);

            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Draws an framebuffer
        /// </summary>
        /// <param name="textureID">The framebuffer texture id</param>
        private void DrawFramebuffer(int textureID)
        {
            // load shader
            gl.UseProgram(ShaderPrograms["ScreenShader"].ProgramID);

            // send texture to the shader
            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(OpenGL.Texture2D, textureID);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["ScreenShader"].ProgramID, "screenTexture"), 0);

            // bind vertex buffer
            gl.BindBuffer(OpenGL.ArrayBuffer, InstancedShapes["FrameShape"].vbo);

            // define verticies
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            // define texture coords
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, OpenGL.Float, false, 0, 18 * sizeof(float));

            // render vertex buffer
            gl.DrawArrays(OpenGL.Triangles, 0, 6);
        }

        /// <summary>
        /// Draw the framebuffer with another shader programm and gamma
        /// </summary>
        /// <param name="textureID"></param>
        /// <param name="shaderProgramm"></param>
        /// <param name="gamma"></param>
        private void DrawFramebuffer(int textureID, int shaderProgramm, float gamma = 0.0f)
        {
            // load shader
            gl.UseProgram(shaderProgramm);

            // send texture to the shader
            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(OpenGL.Texture2D, textureID);
            gl.Uniform1I(gl.GetUniformLocation(shaderProgramm, "screenTexture"), 0);
            gl.Uniform1f(gl.GetUniformLocation(shaderProgramm, "gamma"), gamma);

            // bind vertex buffer
            gl.BindBuffer(OpenGL.ArrayBuffer, InstancedShapes["FrameShape"].vbo);

            // define verticies
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            // define texture coords
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, OpenGL.Float, false, 0, 18 * sizeof(float));

            // render vertex buffer
            gl.DrawArrays(OpenGL.Triangles, 0, 6);
        }

        /// <summary>
        /// Sets an framebuffer as active render target
        /// </summary>
        /// <param name="framebuffer">The framebuffer for the rendering</param>
        public void SetFramebuffer(Framebuffer framebuffer)
        {
            this.SetFramebuffer(framebuffer.FramebufferID);
        }

        /// <summary>
        /// Sets an framebuffer as active render target
        /// </summary>
        /// <param name="framebuffer">The framebuffer id for the rendering</param>
        public void SetFramebuffer(int framebuffer)
        {
            gl.BindFramebuffer(OpenGL.FrameBuffer, framebuffer);
            gl.Enable(OpenGL.DepthTest);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.One, OpenGL.OneMinusSrcAlpha);
            gl.ClearColor(0f, 0f, 0f, 0f);
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        /// <summary>
        /// Renders an qube
        /// </summary>
        /// <param name="qube">The qube to render</param>
        private void DrawCube(Qube cube)
        {
            // Build the matrices
            mat4 mt_mat = mat4.Translate(cube.Location.ToGlmVec3());
            mat4 mr_mat = mat4.RotateX(cube.Rotation.X) * mat4.RotateY(cube.Rotation.Y) * mat4.RotateZ(cube.Rotation.Z);
            mat4 ms_mat = mat4.Scale(cube.Size.ToGlmVec3());
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            // Assign the matrices to the shader
            int shaderID = (int)cube.Propertys["ShaderID"];
            gl.UseProgram(shaderID);

            if(gl.GetUniformLocation(shaderID, "mvp") != -1)
            {
                mat4 mvp = p_mat * v_mat * m_mat;
                gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "mvp"), 1, false, mvp.ToArray());
            }
            else
            {
                gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "projection"), 1, false, p_mat.ToArray());
                gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "view"), 1, false, v_mat.ToArray());
                gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "model"), 1, false, m_mat.ToArray());
            }

            if (this.lightSource != null)
            {
                Vec3 ligtDirection = lightSource.GetLightDirection(camera);
                Vec3 lightColor = lightSource.GetLightColor();
                gl.Uniform3f(gl.GetUniformLocation(shaderID, "lightPos"), lightSource.Location.X, lightSource.Location.Y, lightSource.Location.Z);
                gl.Uniform1f(gl.GetUniformLocation(shaderID, "lightIntensity"), lightSource.Intensity);
                gl.Uniform3f(gl.GetUniformLocation(shaderID, "lightColor"), lightColor.X, lightColor.Y, lightColor.Z);
            }

            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(OpenGL.Texture2D, (int)cube.Material.Propeterys["tex_id"]);
            gl.Uniform1I(gl.GetUniformLocation(shaderID, "textureSampler"), 0);

            gl.ActiveTexture(OpenGL.Texture1);
            gl.BindTexture(OpenGL.Texture2D, (int)cube.Material.Propeterys["normal_id"]);
            gl.Uniform1I(gl.GetUniformLocation(shaderID, "normalMap"), 1);

            gl.BindVertexArray((int)cube.Propertys["vao"]);
            gl.DrawArrays(OpenGL.Triangles, 0, (int)cube.Propertys["tris"]);
            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Init an terrain3d
        /// </summary>
        /// <param name="terrain">The terrain to initalize</param>
        private void InitTerrain3D(Terrain3D terrain)
        {
            // 2. Load verticies into the gpu
            float[] verticies = terrain.TerrainData.verticies;
            int vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            terrain.Propertys.Add("vbo", vbo);

            float[] colors = terrain.TerrainData.colors;
            int cbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.BufferData(OpenGL.ArrayBuffer, colors.Length * sizeof(float), colors, OpenGL.DynamicDraw);
            terrain.Propertys.Add("cbo", cbo);

            terrain.Propertys.Add("ShaderID", ShaderPrograms["TerrainShader"].ProgramID);
        }

        /// <summary>
        /// Renders an terrain3D
        /// </summary>
        /// <param name="terrain">The terrain to render</param>
        private void RenderTerrain3D(Terrain3D terrain)
        {
            mat4 mt_mat = mat4.Translate(terrain.Location.ToGlmVec3());
            mat4 mr_mat = mat4.RotateX(terrain.Rotation.X) * mat4.RotateY(terrain.Rotation.Y) * mat4.RotateZ(terrain.Rotation.Z);
            mat4 ms_mat = mat4.Scale(terrain.Size.ToGlmVec3());
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            mat4 mvp = p_mat * v_mat * m_mat;

            int shaderID = (int)terrain.Propertys["ShaderID"];
            gl.UseProgram(shaderID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "mvp"), 1, false, mvp.ToArray());
            Console.WriteLine(gl.GetError());

            int vbo = (int)terrain.Propertys["vbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            int cbo = (int)terrain.Propertys["cbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 0);

            gl.DrawArrays(OpenGL.Triangles, 0, terrain.TerrainData.tris);
            
            Console.WriteLine("redered terrain3d");
        }

        /// <summary>
        /// Initial the particle emitter
        /// </summary>
        /// <param name="emitter">The emitter to initalize</param>
        private void InitParticleEmitter(ParticleEmitter emitter)
        {
            ParticleBuffers buffers = emitter.GetParticleBuffers();

            int vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.verticies.Length * sizeof(float), buffers.verticies, OpenGL.DynamicDraw);
            emitter.Propertys.Add("vbo", vbo);

            int cbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.colors.Length * sizeof(float), buffers.colors, OpenGL.DynamicDraw);
            emitter.Propertys.Add("cbo", cbo);

            int tbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, tbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.texCords.Length * sizeof(float), buffers.texCords, OpenGL.DynamicDraw);
            emitter.Propertys.Add("tbo", tbo);

            int pbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, pbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.positions.Length * sizeof(float), buffers.positions, OpenGL.DynamicDraw);
            emitter.Propertys.Add("pbo", pbo);

            int rbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, rbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.rotations.Length * sizeof(float), buffers.rotations, OpenGL.DynamicDraw);
            emitter.Propertys.Add("rbo", rbo);

            int sbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, sbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.scales.Length * sizeof(float), buffers.scales, OpenGL.DynamicDraw);
            emitter.Propertys.Add("sbo", sbo);

            emitter.Propertys.Add("ShaderID", this.ShaderPrograms["ParticleShader"].ProgramID);
        }

        /// <summary>
        /// Draws the particle emitter
        /// </summary>
        public void DrawParticleEmitter(ParticleEmitter emitter)
        {
            ParticleBuffers buffers = emitter.GetParticleBuffers();

            mat4 mt_mat = mat4.Translate(emitter.Location.ToGlmVec3());
            mat4 mr_mat = mat4.RotateX(emitter.Rotation.X) * mat4.RotateY(emitter.Rotation.Y) * mat4.RotateZ(emitter.Rotation.Z);
            mat4 ms_mat = mat4.Scale(emitter.Size.ToGlmVec3());
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            int shaderID = (int)emitter.Propertys["ShaderID"];
            gl.UseProgram(shaderID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "projection"), 1, false, p_mat.ToArray());
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "view"), 1, false, v_mat.ToArray());
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "model"), 1, false, m_mat.ToArray());
            Console.WriteLine(gl.GetError());

            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(OpenGL.Texture2D, emitter.ParticleMask.RenderID);
            gl.Uniform1I(gl.GetUniformLocation(shaderID, "textureSampler"), 0);

            int vbo = (int)emitter.Propertys["vbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            int cbo = (int)emitter.Propertys["cbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.colors.Length * sizeof(float), buffers.colors, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 0);

            int tbo = (int)emitter.Propertys["tbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, tbo);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);

            int pbo = (int)emitter.Propertys["pbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, pbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.positions.Length * sizeof(float), buffers.positions, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(3);
            gl.VertexAttribPointer(3, 3, OpenGL.Float, false, 0, 0);

            int rbo = (int)emitter.Propertys["rbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, rbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.rotations.Length * sizeof(float), buffers.rotations, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(4);
            gl.VertexAttribPointer(4, 3, OpenGL.Float, false, 0, 0);

            int sbo = (int)emitter.Propertys["sbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, sbo);
            gl.BufferData(OpenGL.ArrayBuffer, buffers.scales.Length * sizeof(float), buffers.scales, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(5);
            gl.VertexAttribPointer(5, 3, OpenGL.Float, false, 0, 0);

            gl.DrawArrays(OpenGL.Triangles, 0, emitter.ParticleDeffinitions.Count * 6);
        }

        private void InitModel(Core.GameElements.Model model)
        {
            var preBuildShader = this.GetShaderProgram(model.Shader);
            if (preBuildShader != null)
            {
                model.Propertys.Add("ShaderID", preBuildShader.ProgramID);
                Console.WriteLine("Shader found");
            }
            else
            {
                this.CreateShader(model.Shader.GetType().Name, model.Shader);
                this.ShaderPrograms.Add(model.Shader.GetType().Name, model.Shader);
                model.Propertys.Add("ShaderID", model.Shader.ProgramID);
                Console.WriteLine("Shader was not found, added it into the shaders list");
            }

            foreach (var mesh in model.Meshes)
            {
                mesh.Material.Propeterys["DiffuseTextureID"] = InitTexture(mesh.Material.DiffuseTexture);

                var verices = mesh.Vertices.ToArray();
                var vertexSize = Marshal.SizeOf<vertex>();
                var indices = mesh.Indices.ToArray();

                int VAO = gl.GenVertexArrays(1);
                int VBO = gl.GenBuffer(1);
                int EBO = gl.GenBuffer(1);

                gl.BindVertexArray(VAO);

                gl.BindBuffer(OpenGL.ArrayBuffer, VBO);
                gl.BufferData(OpenGL.ArrayBuffer, verices.Length * vertexSize, verices, OpenGL.StaticDraw);

                gl.BindBuffer(OpenGL.ElementArrayBuffer, EBO);
                gl.BufferData(OpenGL.ElementArrayBuffer, indices.Length * sizeof(int), indices, OpenGL.StaticDraw);

                gl.EnableVertexAttribArray(0);
                gl.VertexAttribPointer(0, 3, OpenGL.Float, false, vertexSize, IntPtr.Zero);

                gl.EnableVertexAttribArray(2);
                gl.VertexAttribPointer(2, 2, OpenGL.Float, false, vertexSize, Marshal.OffsetOf<vertex>("textcoords"));

                gl.EnableVertexAttribArray(3);
                gl.VertexAtrribIPointer(3, 4, OpenGL.Int, vertexSize, Marshal.OffsetOf<vertex>("BoneIDs"));

                // weights
                gl.EnableVertexAttribArray(4);
                gl.VertexAttribPointer(4, 4, OpenGL.Float, false, vertexSize, Marshal.OffsetOf<vertex>("BoneWeights"));

                gl.BindVertexArray(0);

                mesh.Propertys.Add("vao", VAO);
                mesh.Propertys.Add("vbo", VBO);
                mesh.Propertys.Add("ebo", EBO);

            }
        }

        private void DrawModel(Core.GameElements.Model model)
        {
            mat4 mt_mat = mat4.Translate(model.Location.ToGlmVec3());
            mat4 mr_mat = mat4.RotateX(model.Rotation.X) * mat4.RotateY(model.Rotation.Y) * mat4.RotateZ(model.Rotation.Z);
            mat4 ms_mat = mat4.Scale(model.Size.ToGlmVec3());
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            foreach (var mesh in model.Meshes)
            {
                int vao = (int)mesh.Propertys["vao"];
                int ebo = (int)mesh.Propertys["ebo"];

                var program = (int)model.Propertys["ShaderID"];
                gl.UseProgram(program);
                gl.UniformMatrix4fv(gl.GetUniformLocation(program, "projection"), 1, false, p_mat.ToArray());
                gl.UniformMatrix4fv(gl.GetUniformLocation(program, "view"), 1, false, v_mat.ToArray());
                gl.UniformMatrix4fv(gl.GetUniformLocation(program, "model"), 1, false, m_mat.ToArray());

                for (int i = 0; i < 100; i++)
                {
                    gl.UniformMatrix4fv(gl.GetUniformLocation(program, "finalBonesMatrices[" + i.ToString() + "]"), 1, false, model.Animator.FinalBoneMatrices[i].ToArray());
                }

                gl.ActiveTexture(OpenGL.Texture0);
                gl.BindTexture(OpenGL.Texture2D, (int) mesh.Material.Propeterys["DiffuseTextureID"]);
                gl.Uniform1I(gl.GetUniformLocation(program, "textureSampler"), 0);

                gl.BindVertexArray(vao);
                gl.BindBuffer(OpenGL.ElementArrayBuffer, ebo);
                gl.DrawElements(OpenGL.Triangles, mesh.Indices.Count, OpenGL.UnsignedInt);
                gl.BindVertexArray(0);
            }
        }

        /// <summary>
        /// Initial an 2D light
        /// Unused because of the instance shape
        /// </summary>
        /// <param name="light"></param>
        private void InitLight2D(Light2D light)
        {
            var vao = gl.GenVertexArrays(1);
            gl.BindVertexArray(vao);

            var verticies = Light2D.GetVericies();
            var texCords = Light2D.GetTexCoords();

            var vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);


            var tbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, tbo);
            gl.BufferData(OpenGL.ArrayBuffer, texCords.Length * sizeof(float), texCords, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);

            light.Propertys.Add("vao", vao);
            light.Propertys.Add("vbo", vbo);
            light.Propertys.Add("tbo", tbo);
            light.Propertys.Add("tris", 6);

            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Renders an 2D light
        /// </summary>
        /// <param name="light"></param>
        private void RenderLight2D(Light2D light)
        {
            var lightColor = Utils.ConvertColor(light.LightColor);

            gl.Disable(OpenGL.DepthTest);
            //Create the modelview matrix
            mat4 mt_mat = mat4.Translate(light.Location.X, light.Location.Y, light.Location.Z);
            mat4 mr_mat = mat4.Identity;
            mat4 ms_mat = mat4.Scale(light.Size.X, light.Size.Y, light.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.Enable(NetGL.OpenGL.Texture2D);

            var shader = ShaderPrograms["Light2DShader"].ProgramID;
            gl.UseProgram(ShaderPrograms["Light2DShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shader, "mvp"), 1, false, mvp.ToArray());
            gl.Uniform3f(gl.GetUniformLocation(shader, "color"), lightColor[0], lightColor[1], lightColor[2]);

            //Load the texture and send it to the shader
            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(NetGL.OpenGL.Texture2D, light.LightShape.RenderID);
            gl.Uniform1I(gl.GetUniformLocation(shader, "textureSampler"), 0);

            int vbo = this.InstancedShapes["Light2DShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 18 * sizeof(float));

            //Draw the sprite
            gl.DrawArrays(OpenGL.Triangles, 0, 6);

            gl.Disable(NetGL.OpenGL.Texture2D);
            gl.Enable(OpenGL.DepthTest);
        }

        /// <summary>
        /// Disposes the render device
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, ShaderProgram> item in ShaderPrograms)
            {
                Console.WriteLine("Dispose " + item.Key);
                this.DisposeShader(item.Value);
            }

            foreach (var item in InstancedShapes)
            {
                Console.WriteLine("Dispose " + item.Key);
                gl.DeleteBuffers(1, item.Value.vbo);
                Console.WriteLine("Disposed " + item.Key + " with error " + gl.GetError());
            }
        }

        /// <summary>
        /// Deletes the shader program
        /// </summary>
        /// <param name="program"></param>
        public void DisposeShader(ShaderProgram program)
        {
            gl.DeleteProgram(program.ProgramID);
        }

        /// <summary>
        /// Disposes the texture
        /// </summary>
        /// <param name="texture"></param>
        public void DisposeTexture(Texture texture)
        {
            if (texture.RenderID != 0)
            {
                gl.DeleteTextures(1, texture.RenderID);
                texture.RenderID = 0;
                Console.WriteLine("Texture " + texture.Name + " disposed");
            }
        }

        /// <summary>
        /// Disposes the font
        /// </summary>
        /// <param name="font"></param>
        public void DisposeFont(Font font)
        {
            if (font.RenderID != 0)
            {
                gl.DeleteTextures(1, font.RenderID);
                font.RenderID = 0;
            }
        }

        /// <summary>
        /// Disposes the element data from the gpu
        /// </summary>
        /// <param name="element">The element to dispose</param>
        public void DisposeElement(GameElement element)
        {
            if (element.GetType() == typeof(Qube))
            {
                this.DisposeCube((Qube)element);
            }
            else if(element.GetType() == typeof(BufferedSprite))
            {
                this.DisposeBufferedSprite((BufferedSprite)element);
            }
            else if (element.GetType() == typeof(ParticleEmitter))
            {
                this.DisposeParticleEmitter((ParticleEmitter)element);
            }
            else if(element.GetType() == typeof(Genesis.Core.GameElements.Model))
            {
                this.DisposeModel((Genesis.Core.GameElements.Model) element);
            }
            else if(element.GetType() == typeof(Light2D))
            {
                //this.DisposeLight2D((Light2D)element); Using instance shape
            }
        }

        private void DisposeBufferedSprite(BufferedSprite sprite)
        {
            Console.WriteLine("Disposing buffered sprite " + sprite.UUID);
            gl.DeleteBuffers(1, (int)sprite.Propertys["vbo"]);
            gl.DeleteBuffers(1, (int)sprite.Propertys["cbo"]);
            gl.DeleteBuffers(1, (int)sprite.Propertys["tbo"]);
            Console.WriteLine("Disposed buffered sprite with error " + gl.GetError());
        }

        private void DisposeCube(Qube cube)
        {
            Console.WriteLine("Disposing " + cube.Name);
            gl.DeleteVertexArrays(1, (int)cube.Propertys["vao"]);
            gl.DeleteBuffers(1, (int)cube.Propertys["vbo"]);
            gl.DeleteBuffers(1, (int)cube.Propertys["cbo"]);
            gl.DeleteBuffers(1, (int)cube.Propertys["tbo"]);
            gl.DeleteBuffers(1, (int)cube.Propertys["nbo"]);
            Console.WriteLine(cube.Name + " Disposed!");
        } 

        /// <summary>
        /// Disposes the particle emitter
        /// </summary>
        /// <param name="emitter"></param>
        private void DisposeParticleEmitter(ParticleEmitter emitter)
        {
            if(emitter.Propertys.ContainsKey("vbo")) 
            {
                Console.WriteLine("Disposing " + emitter.Name);
                gl.DeleteBuffers(1, (int) emitter.Propertys["vbo"]);
                gl.DeleteBuffers(1, (int)emitter.Propertys["cbo"]);
                gl.DeleteBuffers(1, (int)emitter.Propertys["tbo"]);
                gl.DeleteBuffers(1, (int)emitter.Propertys["pbo"]);
                gl.DeleteBuffers(1, (int)emitter.Propertys["rbo"]);
                gl.DeleteBuffers(1, (int)emitter.Propertys["sbo"]);
                Console.WriteLine(emitter.Name + " Disposed!");
            }
        }

        private void DisposeModel(Genesis.Core.GameElements.Model model)
        {
            foreach (var mesh in model.Meshes)
            {
                int vao = (int)mesh.Propertys["vao"];
                int ebo = (int)mesh.Propertys["ebo"];
                int vbo = (int)mesh.Propertys["vbo"];

                Console.WriteLine("Disposing Mesh " + mesh.Name);
                gl.DeleteBuffers(1, vbo);
                gl.DeleteBuffers(1, ebo);
                gl.DeleteVertexArrays(1, vao);
                gl.DeleteTextures(1, (int)mesh.Material.Propeterys["DiffuseTextureID"]);
                Console.WriteLine("Disposed Mesh with error " + gl.GetError());
            }
        }

        private void DisposeLight2D(Light2D light2D)
        {
            int vao = (int)light2D.Propertys["vao"];
            int vbo = (int)light2D.Propertys["vbo"];
            int tbo = (int)light2D.Propertys["tbo"];
            Console.WriteLine("Dispose Light2D " + light2D.UUID);
            gl.DeleteVertexArrays(1, vao);
            gl.DeleteBuffers(1, vbo);
            gl.DeleteBuffers(1, tbo);
            Console.WriteLine("Disposed Light2D " + light2D.UUID + " with error " + gl.GetError());
        }

        public Framebuffer BuildShadowMap(int width, int height)
        {
            return null;
        }

        public mat4 GenerateLightspaceMatrix(Camera camera, Viewport viewport, Light lightSource)
        {
            return mat4.Identity;
        }

        public void PrepareShadowPass(Framebuffer shadowmap, mat4 lightspaceMatrix)
        {
            
        }

        public void RenderShadowmap(Framebuffer shadowmap, mat4 lightspaceMatrix, Scene3D scene)
        {
            
        }

        public void FinishShadowPass(Viewport viewport)
        {
            
        }

        public void SetProjectionMatrix(mat4 projectionMatrix)
        {
            p_mat = projectionMatrix;
        }

        public void SetViewMatrix(mat4 viewMatrix)
        {
            v_mat = viewMatrix;
        }

        public void InitMaterial(Material material)
        {
            
        }

        public void InitInstance(RenderInstanceContainer element)
        {
            throw new NotImplementedException();
        }

        public void DrawInstance(RenderInstanceContainer element)
        {
            throw new NotImplementedException();
        }
    }
}
