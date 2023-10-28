using Genesis.Core;
using Genesis.Core.Prefabs;
using Genesis.Math;
using NetGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Genesis.Graphics.RenderDevice
{
    public struct InstancedShape
    {
        public int vbo;
        public int cbo;
        public int tbo;
    }

    public class ModernGL : IRenderDevice
    {
        private Matrix4x4 p_mat;
        private Matrix4x4 v_mat;

        private Dictionary<String, ShaderProgram> ShaderPrograms;
        private Dictionary<String, InstancedShape> InstancedShapes;
        private NetGL.OpenGL gl;
        private IntPtr hwnd;

        public ModernGL(IntPtr hwnd)
        {
            this.hwnd = hwnd;
            this.ShaderPrograms = new Dictionary<String, ShaderProgram>(); 
            this.ShaderPrograms.Add("BasicShader", new Shaders.OpenGL.BasicShader());
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
            gl.ClearColor(0.188f, 0.0f, 0.188f, 0.0f);
            gl.Enable(NetGL.OpenGL.Blend);
            //gl.BlendFunc(NetGL.OpenGL.SrcAlpha, NetGL.OpenGL.OneMinusSrcAlpha);

            foreach (KeyValuePair<string, ShaderProgram> item in this.ShaderPrograms)
            {
                this.LoadShader(item.Key, item.Value);
            }

            this.BuildSpriteShape();
        }

        private void BuildSpriteShape()
        {
            var spriteShape = new InstancedShape();
            //Calculate new buffer data
            float[] verticies =
            {
                // Erstes Dreieck
                0.0f, 0.0f, 0.0f,  // Unten links
                0.0f, 1.0f, 0.0f,  // Oben links
                1.0f, 1.0f, 0.0f,  // Oben rechts

                // Zweites Dreieck
                0.0f, 0.0f, 0.0f,  // Unten links
                1.0f, 1.0f, 0.0f,  // Oben rechts
                1.0f, 0.0f, 0.0f   // Unten rechts
            };
            spriteShape.vbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, spriteShape.vbo);
            gl.BufferData(OpenGL.ArrayBuffer, verticies.Length * sizeof(float), verticies, OpenGL.DynamicDraw);

            float[] colors =
            {
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f
            };
            spriteShape.cbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, spriteShape.cbo);
            gl.BufferData(OpenGL.ArrayBuffer, colors.Length * sizeof(float), colors, OpenGL.DynamicDraw);
            
            //Create texcords
            float[] textCoordsf =
            {
                0.0f, 0.0f, 
                0.0f, 1.0f,
                1.0f, 1.0f,

                0.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };
            spriteShape.tbo = gl.GenBuffer(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, spriteShape.tbo);
            gl.BufferData(OpenGL.ArrayBuffer, textCoordsf.Length * sizeof(float), textCoordsf, OpenGL.DynamicDraw);
            this.InstancedShapes.Add("SpriteShape", spriteShape);
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

        public void InitElement3D(Element3D element)
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
            //DrawRect(rect, color, 1.0f);
        }

        public void DrawRect(Rect rect, Color color, float borderWidth)
        {
            //gl.Color3f(color.R, color.G, color.B);
            //gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Line);
            //gl.Begin(NetGL.OpenGL.Quads);
            //gl.LineWidth(borderWidth);
            //gl.Vertex3f(rect.X, rect.Y, 0f);
            //gl.Vertex3f(rect.X, rect.Y + rect.Height, 0f);
            //gl.Vertex3f(rect.X + rect.Width, rect.Y + rect.Height, 0f);
            //gl.Vertex3f(rect.X + rect.Width, rect.Y, 0f);
            //gl.LineWidth(1.0f);
            //gl.End();
            //gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Fill);
            //gl.Color3f(1f, 1f, 1f);
        }

        public void DrawSprite(Vec3 location, Vec3 size, Texture texture)
        {
            DrawSprite(location, size, Color.White, texture);
        }

        public void DrawSprite(Vec3 location, Vec3 size, Texture texture, TexCoords texCoords)
        {
            //DrawSprite(location, size, Color.White, texture, texCoords);
        }

        public void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture)
        {
            gl.MatrixMode(OpenGL.ModelView);
            gl.LoadIdentity();
            gl.Scale(size.X, size.Y, 0f);
            gl.Translate(location.X / size.X, location.Y / size.Y, 0.0f);

            gl.UseProgram(0);

            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.Begin(NetGL.OpenGL.Quads);
            gl.Color3f(color.R / 255f, color.G / 255f, color.B / 255f);

            gl.TexCoord2f(0, 0);
            gl.Vertex3f(0f, 0f, 0f);

            gl.TexCoord2f(0, 1);
            gl.Vertex3f(0f, 1f, 0f);

            gl.TexCoord2f(1, 1);
            gl.Vertex3f(1f, 1f, 0f);

            gl.TexCoord2f(1, 0);
            gl.Vertex3f(1f, 0f, 0f);
            gl.End();
            gl.Color3f(1f, 1f, 1f);
            gl.Disable(NetGL.OpenGL.Texture2D);
        }

        /// <summary>
        /// Renders a sprite with modern gl
        /// </summary>
        /// <param name="sprite"></param>
        public void DrawSprite(Sprite sprite)
        {
            float tX = sprite.Location.X / sprite.Size.X;
            float tY = sprite.Location.Y / sprite.Size.Y;

            float rX = tX + sprite.Size.X / 2;
            float rY = tY + sprite.Size.Y / 2;

            gl.MatrixMode(OpenGL.ModelView);
            gl.LoadIdentity();

            //gl.Translate(rX, rY, 0.0f);
            //gl.Rotate(sprite.Rotation, 0f, 0f, 1f);
            //gl.Translate(-rX, -rY, 0.0f);
            gl.Scale(sprite.Size.X, sprite.Size.Y, 0f);
            gl.Translate(tX, tY, 0.0f);
            

            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.UseProgram(ShaderPrograms["BasicShader"].ProgramID);
            gl.SetProjectionMatrix(gl.GetUniformLocation(ShaderPrograms["BasicShader"].ProgramID, "projMatrix"), 1, false);
            gl.SetModelviewMatrix(gl.GetUniformLocation(ShaderPrograms["BasicShader"].ProgramID, "modelMatrix"), 1, false);

            //Load the texture
            gl.BindTexture(NetGL.OpenGL.Texture2D, sprite.Texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Uniform1I(gl.GetUniformLocation(ShaderPrograms["BasicShader"].ProgramID, "textureSampler"), 0);

            //Load the vertex buffer
            int vertexBuffer = this.InstancedShapes["SpriteShape"].vbo;
            gl.EnableVertexAttribArray(0);
            gl.BindBuffer(OpenGL.ArrayBuffer, vertexBuffer);
            gl.VertexAttribPointer(0, 3, OpenGL.Float, false, 0, 0);


            //Load the collor buffer
            int colorBuffer = this.InstancedShapes["SpriteShape"].cbo;
            gl.EnableVertexAttribArray(1);
            gl.BindBuffer(OpenGL.ArrayBuffer, colorBuffer);
            gl.VertexAttribPointer(1, 3, OpenGL.Float, false, 0, 0);

            //Load the texture coords
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
            int texCordBuffer = this.InstancedShapes["SpriteShape"].tbo;
            gl.EnableVertexAttribArray(2);
            gl.BindBuffer(OpenGL.ArrayBuffer, texCordBuffer);
            gl.BufferData(OpenGL.ArrayBuffer, textCoordsf.Length * sizeof(float), textCoordsf, OpenGL.DynamicDraw);
            gl.VertexAttribPointer(2, 2, OpenGL.Float, false, 0, 0);

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
            gl.MatrixMode(OpenGL.ModelView);
            gl.LoadIdentity();
            gl.Scale(rect.Width, rect.Height, 0f);
            gl.Translate(rect.X / rect.Width, rect.Y / rect.Height, 0.0f);

            gl.UseProgram(0);
            gl.Color3f(color.R / 255f, color.G / 255f, color.B / 255f);
            gl.Begin(NetGL.OpenGL.Quads);
            gl.Vertex3f(0f, 0f, 0f);
            gl.Vertex3f(0f, 1f, 0f);
            gl.Vertex3f(1f, 1f, 0f);
            gl.Vertex3f(1f, 0f, 0f);
            gl.End();
            gl.Color3f(1f, 1f, 1f);
        }

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

        public void Viewport(float x, float y, float width, float height)
        {
            gl.Viewport(x, y, width, height);
        }

        public void SetCamera(Camera camera)
        {
            float x = camera.Location.X - (camera.Size.X / 2);
            float y = camera.Location.Y - (camera.Size.Y / 2);
            float bottom = camera.Location.Y + (camera.Size.Y / 2);
            float right = camera.Location.X + (camera.Size.X / 2);

            gl.MatrixMode(NetGL.OpenGL.Projection);
            gl.LoadIdentity();
            gl.Ortho(0f, camera.Size.X, camera.Size.Y, 0f, camera.Near, camera.Far);
            gl.Translate(-x, -y, 0);
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

        public void DrawString(String text, Vec3 location, float fontSize, Font font, Color color)
        {
            this.DrawString(text, location, fontSize, font.Spacing, font, color);
        }

        public void DrawString(String text, Vec3 location, float fontSize, float spacing, Font font, Color color)
        {
            String[] strings = text.Split('\n');
            int y = 0;

            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.Color3f(color.R, color.G, color.B);
            gl.BindTexture(NetGL.OpenGL.Texture2D, font.RenderID);
            foreach (var str in strings)
            {
                int i = 0;
                foreach (Char c in str)
                {
                    float _spacing = fontSize * spacing;
                    float xOffset = i * fontSize - (i * _spacing);
                    float yOffset = y * fontSize + (y * 0);
                    RenderGlyphe(new Vec3(location.X + xOffset, location.Y + yOffset), font, fontSize, c);
                    i++;
                }
                y++;
            }
            gl.Color3f(1.0f, 1.0f, 1.0f);
            gl.Disable(NetGL.OpenGL.Texture2D);
        }

        private void RenderGlyphe(Vec3 location, Font font, float size, Char character)
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

            gl.MatrixMode(OpenGL.ModelView);
            gl.LoadIdentity();
            gl.Scale(size, size, 0f);
            gl.Translate(location.X / size, location.Y / size, 0.0f);

            gl.Begin(NetGL.OpenGL.Quads);
            // 1 
            gl.TexCoord2f(xLeft, yTop);
            gl.Vertex3f(0f, 0f, 0f);
            gl.TexCoord2f(xLeft, yBottom);
            gl.Vertex3f(0f, 1f, 0f);
            gl.TexCoord2f(xRight, yBottom);
            gl.Vertex3f(1f, 1f, 0f);
            gl.TexCoord2f(xRight, yTop);
            gl.Vertex3f(1f, 0f, 0f);
            gl.End();
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

        public void Dispose()
        {

        }

        public void DrawElement3D(Element3D element)
        {
            throw new NotImplementedException();
        }
    }
}