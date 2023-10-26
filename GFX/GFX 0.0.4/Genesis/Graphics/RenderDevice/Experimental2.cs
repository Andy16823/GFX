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

namespace Genesis.Graphics.RenderDevice
{
    public class Experimental2 : IRenderDevice
    {
        /// <summary>
        /// Struct for the viewport
        /// </summary>

        private Camera camera;
        private mat4 p_mat;
        private mat4 v_mat;
        //private Matrix4x4 v_mat;

        private Dictionary<String, ShaderProgram> ShaderPrograms;
        private Dictionary<String, InstancedShape> InstancedShapes;
        private NetGL.OpenGL gl;
        private IntPtr hwnd;

        public Experimental2(IntPtr hwnd)
        {
            this.hwnd = hwnd;
            this.ShaderPrograms = new Dictionary<String, ShaderProgram>();
            this.ShaderPrograms.Add("BasicShader", new Shaders.OpenGL.BasicShader());
            this.ShaderPrograms.Add("MVPShader", new Shaders.OpenGL.MVPShader());
            this.ShaderPrograms.Add("MVPSolidShader", new Shaders.OpenGL.MVPSolidShader());
            this.InstancedShapes = new Dictionary<String, InstancedShape>();
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
            gl.Enable(NetGL.OpenGL.Blend);
            //gl.BlendFunc(NetGL.OpenGL.SrcAlpha, NetGL.OpenGL.OneMinusSrcAlpha);

            foreach (KeyValuePair<string, ShaderProgram> item in this.ShaderPrograms)
            {
                this.LoadShader(item.Key, item.Value);
            }

            this.BuildSpriteShape();
            this.BuildRectShape();
            this.BuildGlypheShape();
        }

        /// <summary>
        /// Creates a sprite shape
        /// </summary>
        private void BuildSpriteShape()
        {
            var spriteShape = new InstancedShape();
            float[] verticies =
            {
                //Verticies
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,

                //Colors
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

                //Tex Coords
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,

                0.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };

            // Create Vertexbuffer and set the data
            gl.UseProgram(ShaderPrograms["MVPShader"].ProgramID);
            spriteShape.vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, spriteShape.vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            this.InstancedShapes.Add("SpriteShape", spriteShape);
        }

        /// <summary>
        /// Builds an rect shape
        /// </summary>
        private void BuildRectShape()
        {
            var shape = new InstancedShape();
            float[] verticies =
            {
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
            };
            gl.UseProgram(this.ShaderPrograms["MVPSolidShader"].ProgramID);
            shape.vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, shape.vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));
            this.InstancedShapes.Add("RectShape", shape);
        }

        /// <summary>
        /// Builds a glyphe shape
        /// </summary>
        public void BuildGlypheShape()
        {
            InstancedShape shape = new InstancedShape();
            float[] verticies =
{
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,

                -0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,

                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f,

                0.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 1.0f
            };
            shape.vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, shape.vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);
            this.InstancedShapes.Add("GlypheShape", shape);
        }

        /// <summary>
        /// Loads a shader program
        /// </summary>
        /// <param name="name"></param>
        /// <param name="program"></param>
        private void LoadShader(String name, ShaderProgram program)
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

            Console.WriteLine(program.FragmentShader.Source);
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

        }

        public void Begin()
        {
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        public void DisposeTexture(Texture texture)
        {
            if (texture.RenderID != 0)
            {
                gl.DeleteTextures(1, texture.RenderID);
                texture.RenderID = 0;
                Console.WriteLine("Texture " + texture.Name + " disposed");
            }
        }

        public void DisposeFont(Font font)
        {
            if (font.RenderID != 0)
            {
                gl.DeleteTextures(1, font.RenderID);
                font.RenderID = 0;
            }
        }

        public void DrawRect(Rect rect, Color color)
        {
            DrawRect(rect, color, 1.0f);
        }

        public void DrawRect(Rect rect, Color color, float borderWidth)
        {
            //Build the mvp matrix
            mat4 mt_mat = mat4.Translate(rect.X, rect.Y, 0.0f);
            mat4 mr_mat = mat4.RotateZ(0.0f);
            mat4 ms_mat = mat4.Scale(rect.Width, rect.Height, 0.0f);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader and set the mvp matrix
            gl.UseProgram(this.ShaderPrograms["MVPSolidShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(this.ShaderPrograms["MVPSolidShader"].ProgramID,"mvp"), 1, false, mvp.ToArray<float>());

            float r = color.R;
            float g = color.G;
            float b = color.B;

            float[] newColor =
            {
                r, g, b,
                r, g, b,
                r, g, b,

                r, g, b,
                r, g, b,
                r, g, b
            };
            gl.BindBuffer(OpenGL.ArrayBuffer, this.InstancedShapes["RectShape"].vbo);
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), newColor.Length * sizeof(float), newColor);
            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            //Console.WriteLine("Oma aher");
        }

        public void DrawSprite(Vec3 location, Vec3 size, Texture texture)
        {
            DrawSprite(location, size, Color.White, texture);
        }

        public void DrawSprite(Vec3 location, Vec3 size, Texture texture, TexCoords texCoords)
        {
            //DrawSprite(location, size, Color.White, texture, texCoords);
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
            //Create the modelview matrix
            mat4 mt_mat = mat4.Translate(location.X, location.Y, location.Z);
            mat4 mr_mat = mat4.RotateZ(0f);
            mat4 ms_mat = mat4.Scale(size.X, size.Y, size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["MVPShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["MVPShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            //Load the texture and send it to the shader
            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["MVPShader"].ProgramID, "textureSampler"), 0);

            //Load the vertex buffer and set the new tex coords
            int vertexBuffer = this.InstancedShapes["SpriteShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);

            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);

            //Send the color data to the shader
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            //Create the new text coords and send them to the shader
            float[] textCoordsf =
            {
                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f,

                0.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 1.0f
            };
            gl.BufferSubData(OpenGL.ArrayBuffer, 36 * sizeof(float), textCoordsf.Length * sizeof(float), textCoordsf);
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            //Draw the Sprite
            gl.DrawArrays(OpenGL.Triangles, 0, 6);

            gl.Disable(NetGL.OpenGL.Texture2D);
            //Console.WriteLine(gl.GetError());
        }

        /// <summary>
        /// Renders a sprite with modern gl
        /// </summary>
        /// <param name="sprite"></param>
        public void DrawSprite(Sprite sprite)
        {
            //Create the modelview matrix
            mat4 mt_mat = mat4.Translate(sprite.Location.X, sprite.Location.Y, sprite.Location.Z);
            mat4 mr_mat = mat4.RotateZ(sprite.Rotation);
            mat4 ms_mat = mat4.Scale(sprite.Size.X, sprite.Size.Y, sprite.Size.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;

            //Create the mvp matrix
            mat4 mvp = p_mat * v_mat * m_mat;

            //Load the shader program and set the mvp matrix
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["MVPShader"].ProgramID);
            gl.UniformMatrix4fv(gl.GetUniformLocation(ShaderPrograms["MVPShader"].ProgramID, "mvp"), 1, false, mvp.ToArray());

            //Load the texture and send it to the shader
            gl.BindTexture(NetGL.OpenGL.Texture2D, sprite.Texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["MVPShader"].ProgramID, "textureSampler"), 0);

            //Load the vertex buffer and set the new tex coords
            int vertexBuffer = this.InstancedShapes["SpriteShape"].vbo;
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);
            
            //Send the vertex data to the shader
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);
            
            //Send the color data to the shader
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            //Create the tex coords and send them to the buffer
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
            gl.BufferSubData(OpenGL.ArrayBuffer, 36 * sizeof(float), textCoordsf.Length * sizeof(float), textCoordsf);            
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 36 * sizeof(float));

            //Draw the Sprite
            gl.DrawArrays(OpenGL.Triangles, 0, 6);

            gl.Disable(NetGL.OpenGL.Texture2D);
            //Console.WriteLine(gl.GetError());
        }


        public void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture, TexCoords texCoords)
        {

        }

        public void DrawVectors(Vec3[] vecs, Color color)
        {
            //gl.Color3f(color.R, color.G, color.B);
            //gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Line);
            //gl.Begin(NetGL.OpenGL.Polygon);
            //foreach (var vec in vecs)
            //{
            //    gl.Vertex3f(vec.X, vec.Y, vec.Z);
            //}
            //gl.End();
            //gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Fill);
            //gl.Color3f(1f, 1f, 1f);
        }

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
            //Creates the modelview matrix
            mat4 mt_mat = mat4.Translate(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2), 0.0f);
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
            float r = (float) color.R / 255;
            float g = (float) color.G / 255;
            float b = (float) color.B / 255;
            float[] newColor =
            {
                r, g, b,
                r, g, b,
                r, g, b,

                r, g, b,
                r, g, b,
                r, g, b
            };
            gl.BufferSubData(OpenGL.ArrayBuffer, 18 * sizeof(float), newColor.Length * sizeof(float), newColor);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 18 * sizeof(float));

            gl.DrawArrays(OpenGL.Triangles, 0, 6);
            //Console.WriteLine(gl.GetError());
        }

        /// <summary>
        /// Loads a texture into the vram
        /// </summary>
        /// <param name="texture"></param>
        public void LoadTexture(Texture texture)
        {
            Console.WriteLine("Trying to load the texture");
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
                    Console.WriteLine("Texture loaded");
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

        public void ModelViewMatrix()
        {
            gl.MatrixMode(NetGL.OpenGL.ModelView);
            gl.LoadIdentity();
        }

        public void PopMatrix()
        {
            //gl.PopMatrix();
        }

        public void ProjectionMatrix()
        {
            gl.MatrixMode(NetGL.OpenGL.Projection);
        }

        public void PushMatrix()
        {
            gl.PushMatrix();
        }

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
        }

        /// <summary>
        /// Set the Projection and view matrices
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(Camera camera)
        {
            float x = camera.Location.X - (camera.Size.X / 2);
            float y = camera.Location.Y - (camera.Size.Y / 2);
            float top = y + camera.Size.Y;
            float right = x + camera.Size.X;

            //Console.WriteLine(camera.Size.Y + " " + y + " " + top);

            p_mat = mat4.Ortho(x, right, y, top, 0.0f, 100.0f);
            v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));

            if(this.camera == null)
            {
                this.camera = camera;
            }
        }

        public void Translate(Vec3 vector)
        {
            //gl.Translate(vector.X, vector.Y, vector.Z);
        }

        public void Translate(float x, float y, float z)
        {
            //gl.Translate(x, y, z);
        }

        public void TextureRepeatT()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
        }

        public void TextureRepeatS()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
        }

        public void TextureClampT()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Clamp);
        }

        public void TextureClampS()
        {
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Clamp);
        }

        public void DrawTexture(Vec3 location, Vec3 size, float repeateX, float repeatY, Texture texture)
        {
            //gl.Enable(NetGL.OpenGL.Texture2D);
            //gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            //gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            //gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            //gl.Begin(NetGL.OpenGL.Quads);
            //// 1 
            //gl.TexCoord2f(0, 0);
            //gl.Vertex3f(location.X, location.Y, 0f);
            //gl.TexCoord2f(0, repeatY);
            //gl.Vertex3f(location.X, location.Y + size.Y, 0f);
            //gl.TexCoord2f(repeateX, repeatY);
            //gl.Vertex3f(location.X + size.X, location.Y + size.Y, 0f);
            //gl.TexCoord2f(repeateX, 0);
            //gl.Vertex3f(location.X + size.X, location.Y, 0f);
            //gl.End();
            //gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Clamp);
            //gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Clamp);
            //gl.Disable(NetGL.OpenGL.Texture2D);
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
            String[] strings = text.Split('\n');
            int y = 0;

            //Console.WriteLine(location.ToString());

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
                    RenderGlyphe(new Vec3(location.X + xOffset, location.Y + yOffset, 0.0f), font, fontSize, c, color);
                    i++;
                }
                y++;
            }
            gl.Disable(NetGL.OpenGL.Texture2D);
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

            float[] newColor =
            {
                r, g, b,
                r, g, b,
                r, g, b,

                r, g, b,
                r, g, b,
                r, g, b
            };
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

        public void DrawMesh(Mesh mesh, Color color)
        {
            //foreach (var face in mesh.Faces)
            //{
            //    //Enable texture if the face got a texture
            //    if (face.Texture != null)
            //    {
            //        gl.Enable(NetGL.OpenGL.Texture2D);
            //        gl.BindTexture(NetGL.OpenGL.Texture2D, face.Texture.RenderID);
            //        gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            //        gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            //    }
            //    //Render the Face
            //    gl.Begin(NetGL.OpenGL.Triangles);
            //    for (int i = 0; i < face.Vertices.Count; i++)
            //    {
            //        //Set the tex cords if a texture is set
            //        if (face.Texture != null)
            //        {
            //            var texCord = face.TexCords[i];
            //            gl.TexCoord2f(texCord.X, texCord.Y);
            //        }
            //        //Render the vertex
            //        var vertex = face.Vertices[i];
            //        gl.Color3f(color.R / 255f, color.G / 255f, color.B / 255f);
            //        gl.Vertex3f(vertex.X, vertex.Y, vertex.Z);
            //        gl.Color3f(1f, 1f, 1f);
            //    }
            //    gl.End();
            //    if (face.Texture != null)
            //    {
            //        gl.Disable(NetGL.OpenGL.Texture2D);
            //    }
            //}
        }

        public int GetError()
        {
            return gl.GetError();
        }

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
    }
}
