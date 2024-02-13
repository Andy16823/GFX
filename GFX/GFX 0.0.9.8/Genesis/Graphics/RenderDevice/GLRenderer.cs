using Genesis.Core.Prefabs;
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
using OpenObjectLoader;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Windows.Forms;
using Genesis.Graphics.Shaders.OpenGL;
using System.CodeDom;
using Genesis.UI;
using Genesis.Graphics.Shapes;

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
        
        public Framebuffer sceneBuffer;
        private Framebuffer uiBuffer;

        public GLRenderer(IntPtr hwnd)
        {
            this.hwnd = hwnd;
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

            ///Initial the prebuild shaders
            this.ShaderPrograms = new Dictionary<String, ShaderProgram>();
            this.ShaderPrograms.Add("BasicShader", new Shaders.OpenGL.BasicShader());
            this.ShaderPrograms.Add("MVPShader", new Shaders.OpenGL.MVPShader());
            this.ShaderPrograms.Add("MVPSolidShader", new Shaders.OpenGL.MVPSolidShader());
            this.ShaderPrograms.Add("MVPRectShader", new Shaders.OpenGL.MVPRectShader());
            this.ShaderPrograms.Add("DiffuseShader", new Shaders.OpenGL.DiffuseShader());
            this.ShaderPrograms.Add("DiffuseNormalShader", new Shaders.OpenGL.DiffuseNormalShader());
            this.ShaderPrograms.Add("WireframeShader", new Shaders.OpenGL.WireframeShader());
            this.ShaderPrograms.Add("ScreenShader", new Shaders.OpenGL.ScreenShader());
            this.ShaderPrograms.Add("SpriteShader", new Shaders.OpenGL.SpriteShader());
            this.ShaderPrograms.Add("TerrainShader", new Shaders.OpenGL.TerrainShader());

            foreach (KeyValuePair<string, ShaderProgram> item in this.ShaderPrograms)
            {
                this.LoadShader(item.Key, item.Value);
            }

            ///Initial the pre build shapes
            this.InstancedShapes = new Dictionary<String, Shapes.Shape>();
            this.InstancedShapes.Add("SpriteShape", new Shapes.SpriteShape());
            this.InstancedShapes.Add("RectShape", new Shapes.RectShape());
            this.InstancedShapes.Add("GlypheShape", new Shapes.GlypheShape());
            this.InstancedShapes.Add("LineShape", new Shapes.LineShape());
            this.InstancedShapes.Add("FrameShape", new Shapes.FrameShape());
            foreach (KeyValuePair<string, Shapes.Shape> item in this.InstancedShapes)
            {
                this.BuildShape(item.Value);
            }

            this.sceneBuffer = this.BuildFramebuffer(100, 100);
            this.uiBuffer = this.BuildFramebuffer(100, 100);
        }

        /// <summary>
        /// Creates an buffer for the shape
        /// </summary>
        /// <param name="shape"></param>
        public void BuildShape(Shapes.Shape shape)
        {
            float[] verticies = shape.GetShape();
            shape.vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, shape.vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
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

        public Framebuffer BuildFramebuffer(int width, int height, Texture texture)
        {
            return this.BuildFramebuffer(width, height, texture.RenderID);
        }

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

        public void UpdateFramebufferSize(Framebuffer framebuffer, int width, int height)
        {
            gl.BindTexture(OpenGL.Texture2D, framebuffer.Texture);
            gl.TexImage2D(OpenGL.Texture2D, 0, OpenGL.RGBA, width, height, 0, OpenGL.RGBA, OpenGL.UnsignedByte);

            gl.BindRenderbuffer(OpenGL.RenderBuffer, framebuffer.RenderBuffer);
            gl.RenderbufferStorage(OpenGL.RenderBuffer, OpenGL.DepthComponent24, width, height);
        }

        /// <summary>
        /// Disposes the render device
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, ShaderProgram> item in ShaderPrograms)
            {
                Console.WriteLine("Dispose " +  item.Key);
                this.DisoseShader(item.Value);
            }

            foreach(var item in InstancedShapes) {
                Console.WriteLine("Dispose " + item.Key + " vbo");
                gl.DeleteBuffers(1, item.Value.vbo);
            }
        }

        /// <summary>
        /// Loads a shader program
        /// </summary>
        /// <param name="name"></param>
        /// <param name="program"></param>
        public void LoadShader(String name, ShaderProgram program)
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
        /// Deletes the shader program
        /// </summary>
        /// <param name="program"></param>
        public void DisoseShader(ShaderProgram program)
        {
            gl.DeleteProgram(program.ProgramID);
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
                BufferedSprite bufferedSprite = (BufferedSprite)element;
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
            else if(element.GetType() == typeof(Qube))
            {
                Qube qube = (Qube)element;

                // 1. Check if the shader already exist. 
                var preBuildShader = this.GetShaderProgram(qube.Shader);
                if (preBuildShader != null)
                {
                    qube.Propertys.Add("ShaderID", preBuildShader.ProgramID);
                    Console.WriteLine("Qube Shader Found");
                }
                else
                {
                    this.LoadShader(qube.Shader.GetType().Name, qube.Shader);
                    this.ShaderPrograms.Add(qube.Shader.GetType().Name, qube.Shader);
                    element.Propertys.Add("ShaderID", qube.Shader.ProgramID);
                    Console.WriteLine("Qube Shader not Found");
                }
                
                // 2. Load verticies into the gpu
                float[] verticies = qube.Shape.GetShape();
                int vbo = gl.GenBuffer(1);
                gl.BindBuffer(OpenGL.ArrayBuffer, vbo); 
                gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
                qube.Propertys.Add("vbo", vbo);

                // 3. Load colors into the gpu
                float[] color = Qube.GetColors(qube.Color);
                int cbo = gl.GenBuffer(1);
                gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
                gl.BufferData(OpenGL.ArrayBuffer, color.Length * sizeof(float), color, OpenGL.DynamicDraw);
                qube.Propertys.Add("cbo", cbo);

                float[] normals = qube.Shape.GetNormals();
                int nbo = gl.GenBuffer(1);
                gl.BindBuffer(OpenGL.ArrayBuffer, nbo);
                gl.BufferData(OpenGL.ArrayBuffer, normals.Length * sizeof(float), normals, OpenGL.DynamicDraw);
                qube.Propertys.Add("nbo", nbo);

                qube.Propertys.Add("tris", (verticies.Length / 3));
            }
            else if (element.GetType() == typeof(Terrain3D))
            {
                this.InitTerrain3D((Terrain3D)element);
            }
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
                //Checks if a prebuild shader allready exist
                var preBuildShader = this.GetShaderProgram(element.Shader);
                if(preBuildShader != null)
                {
                    element.Propertys.Add("ShaderID", preBuildShader.ProgramID);
                    Console.WriteLine("Shader found");
                }
                else
                {
                    this.LoadShader(element.Shader.GetType().Name, element.Shader);
                    this.ShaderPrograms.Add(element.Shader.GetType().Name, element.Shader);
                    element.Propertys.Add("ShaderID", element.Shader.ProgramID);
                    Console.WriteLine("Shader was not found, added it into the shaders list");
                }
            }

            //Inital the Materials
            foreach(Material material in element.Model.Materials)
            {
                material.Propeterys.Add("tex_id", this.InitElement3DTexture(element.Model.Propertys["path"] + "\\" + material.TexturePath));
                material.Propeterys.Add("normal_id", this.InitElement3DNormalMap(element.Model.Propertys["path"] + "\\" + material.NormalPath));

                float[] verticies = material.IndexVerticies();
                int vbo = gl.GenBuffer(1);
                gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
                gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
                material.Propeterys.Add("vbo", vbo);

                float[] texCoords = material.IndexTexCoords();
                int tbo = gl.GenBuffer(1);
                gl.BindBuffer(OpenGL.ArrayBuffer, tbo);
                gl.BufferData(OpenGL.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, OpenGL.DynamicDraw);
                material.Propeterys.Add("tbo", tbo);

                float[] normals = material.IndexNormals();
                int nbo = gl.GenBuffer(1);
                gl.BindBuffer(OpenGL.ArrayBuffer, nbo);
                gl.BufferData(OpenGL.ArrayBuffer, normals.Length * sizeof(float), normals, OpenGL.DynamicDraw);
                material.Propeterys.Add("nbo", nbo);

                material.Propeterys.Add("tris", verticies.Length / 3);
            }
        }

        /// <summary>
        /// Initial the diffuse texture for the 3D model
        /// If the texture file isnt existing an empty 1x1 texture get created
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int InitElement3DTexture(String path)
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
        public int InitElement3DNormalMap(String path)
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
            gl.Disable(OpenGL.DepthTest);
            //Creates the modelview matrix
            mat4 mt_mat = mat4.Translate(rect.X, rect.Y, 0.0f);
            mat4 mr_mat = mat4.RotateZ(0.0f);
            mat4 ms_mat = mat4.Scale(rect.Width, rect.Height, 0.0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.UseProgram(ShaderPrograms["MVPRectShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["MVPRectShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());
            gl.Uniform1f(gl.GetUniformLocation(ShaderPrograms["MVPRectShader"].ProgramID, "aspect"), rect.Width / rect.Height);
            gl.Uniform1f(gl.GetUniformLocation(ShaderPrograms["MVPRectShader"].ProgramID, "border_width"), borderWidth);

            //Load the vertex buffer and binds them
            int vertexBuffer = this.InstancedShapes["RectShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            //Create the new colors and send them to the shader
            float[] newColor = this.GetColorArray(color);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), newColor.Length * sizeof(float), newColor);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Enable(OpenGL.DepthTest);
            //Console.WriteLine(gl.GetError());
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
                this.RenderQube((Qube)element);
            }
            else if(element.GetType() == typeof(Terrain3D))
            {
                this.RenderTerrain3D((Terrain3D)element);
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
            mat4 mr_mat = mat4.RotateZ(sprite.Rotation.Z);
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
            gl.Disable(OpenGL.DepthTest);
            //Creates the modelview matrix
            mat4 mt_mat = mat4.Translate(rect.X, rect.Y, 0.0f);
            mat4 mr_mat = mat4.RotateZ(0.0f);
            mat4 ms_mat = mat4.Scale(rect.Width, rect.Height, 0.0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.UseProgram(ShaderPrograms["MVPSolidShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["MVPSolidShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            //Load the vertex buffer and binds them
            int vertexBuffer = this.InstancedShapes["RectShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            //Create the new colors and send them to the shader
            float[] newColor = this.GetColorArray(color);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), newColor.Length * sizeof(float), newColor);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

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
        public void Viewport(float x, float y, float width, float height)
        {
            gl.Viewport(x, y, width, height);
            UpdateFramebufferSize(this.sceneBuffer, (int)width, (int)height);
            UpdateFramebufferSize(this.uiBuffer, (int)width, (int)height);
        }

        /// <summary>
        /// Set the Projection and view matrices
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(Camera camera)
        {
            if(camera.Type == CameraType.Ortho)
            {
                float x = camera.Location.X - (camera.Size.X / 2);
                float y = camera.Location.Y - (camera.Size.Y / 2);
                float top = y + camera.Size.Y;
                float right = x + camera.Size.X;

                p_mat = mat4.Ortho(x, right, y, top, 0.0f, 100.0f);
                v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));
            }
            else
            {
                vec3 cameraPosition = camera.Location.ToGlmVec3();
                Vec3 cameraFront = Utils.CalculateCameraFront2(camera);

                p_mat = mat4.Perspective(Utils.ToRadians(45.0f), camera.Size.X / camera.Size.Y, camera.Near, camera.Far);
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
            float top = y + camera.Size.Y;
            float right = x + camera.Size.X;

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
            gl.Enable(OpenGL.AlphaTest);
            gl.AlphaFunc(OpenGL.Greater, 0.1f);
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
            foreach (var material in element.Model.Materials) 
            {
                gl.UseProgram(elementShaderID);
                gl.UniformMatrix4fv(gl.GetUniformLocation(elementShaderID, "mvp"), 1, false, mvp.ToArray());

                if(this.lightSource != null)
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
                gl.BindBuffer(OpenGL.ArrayBuffer ,(int)material.Propeterys["vbo"]);
                gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
                
                gl.EnableVertexAttribArray(2);
                gl.BindBuffer(OpenGL.ArrayBuffer, (int)material.Propeterys["tbo"]);
                gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);

                gl.EnableVertexAttribArray(3);
                gl.BindBuffer(OpenGL.ArrayBuffer, (int)material.Propeterys["nbo"]);
                gl.VertexAttribPointer(3, 3, OpenGL.Float, false, 0, 0);

                gl.DrawArrays(OpenGL.Triangles, 0, (int)material.Propeterys["tris"]);
            }

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
            foreach(var material in element.Model.Materials)
            {
                Console.WriteLine("Disposing Material: " + material.Name);
                gl.DeleteTextures(1, (int)material.Propeterys["tex_id"]);
                gl.DeleteTextures(1, (int)material.Propeterys["normal_id"]);
                gl.DeleteBuffers(1, (int)material.Propeterys["vbo"]);
                gl.DeleteBuffers(1, (int)material.Propeterys["tbo"]);
                gl.DeleteBuffers(1, (int)material.Propeterys["nbo"]);
            }
            Console.WriteLine("Element3D: " + element.Name + " Disposed");
        }

        public OpenGL GetRenderer()
        {
            return this.gl;
        }

        public void SetLightSource(Light light)
        {
            this.lightSource = light;
        }

        public void DrawSkyBox(Skybox skybox)
        {
            int elementShaderID = (int)skybox.Propertys["ShaderID"];

            mat4 mt_mat = mat4.Translate(skybox.Location.X, skybox.Location.Y, skybox.Location.Z);
            mat4 mr_mat = mat4.RotateX(skybox.Rotation.X) * mat4.RotateY(skybox.Rotation.Y) * mat4.RotateZ(skybox.Rotation.Z);
            mat4 ms_mat = mat4.Scale(skybox.Size.X, skybox.Size.Y, skybox.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            mat4 mvp = p_mat * v_mat * m_mat;

            gl.Enable(OpenGL.Texture2D);
            foreach (var material in skybox.Model.Materials)
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

            gl.Disable(OpenGL.Texture2D);
        }

        public void PrepareSceneRendering(Scene scene)
        {
            gl.BindFramebuffer(OpenGL.FrameBuffer, sceneBuffer.FramebufferID);
            gl.Enable(OpenGL.DepthTest);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.One, OpenGL.OneMinusSrcAlpha);
            gl.ClearColor(0f, 0f, 0f, 0f);
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        public void FinishSceneRendering(Scene scene)
        {
            gl.BindFramebuffer(OpenGL.FrameBuffer, 0);
            gl.Disable(OpenGL.DepthTest);
            gl.UseProgram(ShaderPrograms["ScreenShader"].ProgramID);

            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(OpenGL.Texture2D, sceneBuffer.Texture);
            gl.BindBuffer(OpenGL.ArrayBuffer, InstancedShapes["FrameShape"].vbo);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["ScreenShader"].ProgramID, "screenTexture"), 0);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, OpenGL.Float, false, 0, 18 * sizeof(float));
            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Enable(OpenGL.DepthTest);
        }

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

        public void FinishCanvasRendering(Scene scene, Canvas canvas)
        {
            SetCamera(this.camera);
            gl.BindFramebuffer(OpenGL.FrameBuffer, 0);
            gl.Disable(OpenGL.DepthTest);
            gl.UseProgram(ShaderPrograms["ScreenShader"].ProgramID);

            gl.ActiveTexture(OpenGL.Texture0);
            gl.BindTexture(OpenGL.Texture2D, uiBuffer.Texture);
            gl.BindBuffer(OpenGL.ArrayBuffer, InstancedShapes["FrameShape"].vbo);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["ScreenShader"].ProgramID, "screenTexture"), 0);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, OpenGL.Float, false, 0, 18 * sizeof(float));
            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            gl.Enable(OpenGL.DepthTest);
        }

        public void SetFramebuffer(Framebuffer framebuffer)
        {
            this.SetFramebuffer(framebuffer.FramebufferID);
        }

        public void SetFramebuffer(int framebuffer)
        {
            gl.BindFramebuffer(OpenGL.FrameBuffer, framebuffer);
            gl.Enable(OpenGL.DepthTest);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.One, OpenGL.OneMinusSrcAlpha);
            gl.ClearColor(0f, 0f, 0f, 0f);
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        private void RenderQube(Qube qube)
        {
            mat4 mt_mat = mat4.Translate(qube.Location.ToGlmVec3());
            mat4 mr_mat = mat4.RotateX(qube.Rotation.X) * mat4.RotateY(qube.Rotation.Y) * mat4.RotateZ(qube.Rotation.Z);
            mat4 ms_mat = mat4.Scale(qube.Size.ToGlmVec3());
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            int shaderID = (int)qube.Propertys["ShaderID"];
            gl.UseProgram(shaderID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "projection"), 1, false, p_mat.ToArray());
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "view"), 1, false, v_mat.ToArray());
            gl.UniformMatrix4fv(gl.GetUniformLocation(shaderID, "model"), 1, false, m_mat.ToArray());

            if (this.lightSource != null)
            {
                Vec3 ligtDirection = lightSource.GetLightDirection(camera);
                Vec3 lightColor = lightSource.GetLightColor();
                gl.Uniform3f(gl.GetUniformLocation(shaderID, "lightPos"), lightSource.Location.X, lightSource.Location.Y, lightSource.Location.Z);
                gl.Uniform1f(gl.GetUniformLocation(shaderID, "lightIntensity"), lightSource.Intensity);
                gl.Uniform3f(gl.GetUniformLocation(shaderID, "lightColor"), lightColor.X, lightColor.Y, lightColor.Z);
            }

            int vbo = (int)qube.Propertys["vbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, vbo);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            int cbo = (int)qube.Propertys["cbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, cbo);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 0);

            int nbo = (int)qube.Propertys["nbo"];
            gl.BindBuffer(OpenGL.ArrayBuffer, nbo);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 3, OpenGL.Float, false, 0, 0);

            gl.DrawArrays(OpenGL.Triangles, 0, (int)qube.Propertys["tris"]);
        }

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
    }
}
