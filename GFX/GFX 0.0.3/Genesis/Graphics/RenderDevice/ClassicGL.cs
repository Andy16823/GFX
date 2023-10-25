using Genesis.Core;
using Genesis.Core.Prefabs;
using Genesis.Math;
using NetGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.RenderDevice
{
    public class ClassicGL : IRenderDevice
    {
        private NetGL.OpenGL gl;
        private IntPtr hwnd;

        public ClassicGL(IntPtr hwnd)
        {
            this.hwnd = hwnd;
        }

        public void Begin()
        {
            gl.Clear(NetGL.OpenGL.ColorBufferBit | NetGL.OpenGL.DepthBufferBit);
        }

        public void DisposeTexture(Texture texture)
        {
            if(texture.RenderID != 0)
            {
                gl.DeleteTextures(1, texture.RenderID);
                texture.RenderID = 0;
                Console.WriteLine("Texture " + texture.Name + " disposed");
            }          
        }

        public void DisposeFont(Font font)
        {
            if(font.RenderID != 0)
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
            gl.Color3f(color.R, color.G, color.B);
            gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Line);
            gl.Begin(NetGL.OpenGL.Quads);
            gl.LineWidth(borderWidth);
            gl.Vertex3f(rect.X, rect.Y, 0f);
            gl.Vertex3f(rect.X, rect.Y + rect.Height, 0f);
            gl.Vertex3f(rect.X + rect.Width, rect.Y + rect.Height, 0f);
            gl.Vertex3f(rect.X + rect.Width, rect.Y, 0f);
            gl.LineWidth(1.0f);
            gl.End();
            gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Fill);
            gl.Color3f(1f, 1f, 1f);
        }

        public void DrawSprite(Sprite sprite)
        {
            float tX = sprite.Location.X + (sprite.Size.X / 2);
            float tY = sprite.Location.Y + (sprite.Size.Y / 2);

            this.ModelViewMatrix();
            this.PushMatrix();
            this.Translate(tX, tY, 0.0f);
            this.Rotate(sprite.Rotation, new Vec3(0f, 0f, 200f));
            this.Translate(-tX, -tY, 0.0f);
            this.DrawSprite(sprite.Location, sprite.Size, sprite.Color, sprite.Texture, sprite.TexCoords);
            this.PopMatrix();
        }

        public void DrawSprite(Vec3 location, Vec3 size, Texture texture)
        {
            //gl.Enable(NetGL.OpenGL.Texture2D);
            //gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            //gl.Begin(NetGL.OpenGL.Quads);
            //// 1 
            //gl.TexCoord2f(0, 0);
            //gl.Vertex3f(location.X, location.Y, 0f);
            //gl.TexCoord2f(0, 1);
            //gl.Vertex3f(location.X, location.Y + size.Y, 0f);
            //gl.TexCoord2f(1, 1);
            //gl.Vertex3f(location.X + size.X, location.Y + size.Y, 0f);
            //gl.TexCoord2f(1, 0);
            //gl.Vertex3f(location.X + size.X, location.Y, 0f);
            //gl.End();
            //gl.Disable(NetGL.OpenGL.Texture2D);
            DrawSprite(location, size, Color.White, texture);
        }

        public void DrawSprite(Vec3 location, Vec3 size, Texture texture, TexCoords texCoords)
        {
            //gl.Enable(NetGL.OpenGL.Texture2D);
            //gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            //gl.Begin(NetGL.OpenGL.Quads);
            //// 1 
            //gl.TexCoord2f(texCoords.TopLeft.X, texCoords.TopLeft.Y);
            //gl.Vertex3f(location.X, location.Y, 0f);
            //gl.TexCoord2f(texCoords.BottomLeft.X, texCoords.BottomLeft.Y);
            //gl.Vertex3f(location.X, location.Y + size.Y, 0f);
            //gl.TexCoord2f(texCoords.BottomRight.X, texCoords.BottomRight.Y);
            //gl.Vertex3f(location.X + size.X, location.Y + size.Y, 0f);
            //gl.TexCoord2f(texCoords.TopRight.X, texCoords.TopRight.Y);
            //gl.Vertex3f(location.X + size.X, location.Y, 0f);
            //gl.End();
            //gl.Disable(NetGL.OpenGL.Texture2D);
            DrawSprite(location, size, Color.White, texture, texCoords);
        }

        public void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture)
        {
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.Begin(NetGL.OpenGL.Quads);
            gl.Color3f(color.R / 255f, color.G / 255f, color.B / 255f);

            gl.TexCoord2f(0, 0);
            gl.Vertex3f(location.X, location.Y, 0f);

            gl.TexCoord2f(0, 1);
            gl.Vertex3f(location.X, location.Y + size.Y, 0f);

            gl.TexCoord2f(1, 1);
            gl.Vertex3f(location.X + size.X, location.Y + size.Y, 0f);

            gl.TexCoord2f(1, 0);
            gl.Vertex3f(location.X + size.X, location.Y, 0f);
            gl.End();
            gl.Color3f(1f, 1f, 1f);
            gl.Disable(NetGL.OpenGL.Texture2D);
        }

        public void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture, TexCoords texCoords)
        {
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Begin(NetGL.OpenGL.Quads);
            
            gl.Color3f(color.R / 255f, color.G / 255f, color.B / 255f);
            gl.TexCoord2f(texCoords.TopLeft.X, texCoords.TopLeft.Y);
            gl.Vertex3f(location.X, location.Y, 0f);

            gl.TexCoord2f(texCoords.BottomLeft.X, texCoords.BottomLeft.Y);
            gl.Vertex3f(location.X, location.Y + size.Y, 0f);
            
            gl.TexCoord2f(texCoords.BottomRight.X, texCoords.BottomRight.Y);
            gl.Vertex3f(location.X + size.X, location.Y + size.Y, 0f);

            gl.TexCoord2f(texCoords.TopRight.X, texCoords.TopRight.Y);
            gl.Vertex3f(location.X + size.X, location.Y, 0f);

            gl.End();
            gl.Color3f(1f, 1f, 1f);
            gl.Disable(NetGL.OpenGL.Texture2D);
        }

        public void DrawVectors(Vec3[] vecs, Color color)
        {
            gl.Color3f(color.R, color.G, color.B);
            gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Line);
            gl.Begin(NetGL.OpenGL.Polygon);
            foreach (var vec in vecs)
            {
                gl.Vertex3f(vec.X, vec.Y, vec.Z);
            }
            gl.End();
            gl.PolygonMode(NetGL.OpenGL.FrontAndBack, NetGL.OpenGL.Fill);
            gl.Color3f(1f, 1f, 1f);
        }

        public void End()
        {
            gl.SwapLayerBuffers(NetGL.OpenGL.SwapMainPlane);
            gl.Flush();
        }

        public void FillRect(Rect rect, Color color)
        {
            gl.Color3f(color.R / 255f, color.G / 255f , color.B / 255f);
            gl.Begin(NetGL.OpenGL.Quads);
            gl.Vertex3f(rect.X, rect.Y, 0f);
            gl.Vertex3f(rect.X, rect.Y + rect.Height, 0f);
            gl.Vertex3f(rect.X + rect.Width, rect.Y + rect.Height, 0f);
            gl.Vertex3f(rect.X + rect.Width, rect.Y, 0f);
            gl.End();
            gl.Color3f(1f, 1f, 1f);
        }

        public void Init()
        {
            gl = new NetGL.OpenGL();
            gl.Initial(hwnd);
            gl.ClearColor(0.188f, 0.0f, 0.188f, 0.0f);
            gl.Enable(NetGL.OpenGL.Blend);
            gl.BlendFunc(NetGL.OpenGL.SrcAlpha, NetGL.OpenGL.OneMinusSrcAlpha);
        }

        public void LoadTexture(Texture texture)
        {
            Console.WriteLine("Trying to load the texture");
            if(texture.RenderID == 0)
            {
                //texture.RenderID = NetGL.Toolkit.TextureImporter.BindTexture(gl, NetGL.OpenGL., texture.Bitnmap);
                texture.RenderID = GenerateTexture(gl, texture);
                int errorID = gl.GetError();
                if(errorID != 0)
                {
                    Console.WriteLine("Test Error while loading the texture. Error Code " + errorID);
                }
                else
                {
                    Console.WriteLine("Texture loaded");
                }
            }
        }

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
            gl.PopMatrix();
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
            gl.Rotate(angle, vector.X, vector.Y, vector.Z);
        }

        public void Viewport(float x, float y, float width, float height)
        {
            gl.Viewport(x, y, width, height);
        }

        public void SetCamera(Camera camera)
        {
            float left = camera.Location.X - (camera.Size.X / 2);
            float top = camera.Location.Y - (camera.Size.Y / 2);
            float bottom = camera.Location.Y + (camera.Size.Y / 2);
            float right = camera.Location.X + (camera.Size.X / 2);


            gl.MatrixMode(NetGL.OpenGL.Projection);
            gl.LoadIdentity();
            if(camera.Type == CameraType.Ortho)
            {
                gl.Ortho(left, right, bottom, top, camera.Near, camera.Far);
            }
            else
            {
                gl.Translate(camera.Location.X, camera.Location.Y, camera.Location.Z);
                gl.Perspective(45.0f, 16f/9f,camera.Near,camera.Far);
                //gl.Frustum(left, right, bottom, top, camera.Near, camera.Far);
            }
            
        }

        public void Translate(Vec3 vector)
        {
            gl.Translate(vector.X, vector.Y, vector.Z);
        }

        public void Translate(float x, float y, float z)
        {
            gl.Translate(x,y,z);
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
            gl.Enable(NetGL.OpenGL.Texture2D);
            gl.BindTexture(NetGL.OpenGL.Texture2D, texture.RenderID);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
            gl.Begin(NetGL.OpenGL.Quads);
            // 1 
            gl.TexCoord2f(0, 0);
            gl.Vertex3f(location.X, location.Y, 0f);
            gl.TexCoord2f(0, repeatY);
            gl.Vertex3f(location.X, location.Y + size.Y, 0f);
            gl.TexCoord2f(repeateX, repeatY);
            gl.Vertex3f(location.X + size.X, location.Y + size.Y, 0f);
            gl.TexCoord2f(repeateX, 0);
            gl.Vertex3f(location.X + size.X, location.Y, 0f);
            gl.End();
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Clamp);
            gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Clamp);
            gl.Disable(NetGL.OpenGL.Texture2D);
        }

        public void DrawString(String text, Vec3 location, float fontSize, Font font, Color color)
        {
            // Old render method
            //int i = 0;
            //gl.Enable(NetGL.OpenGL.Texture2D);
            //gl.Color3f(color.R, color.G, color.B);
            //gl.BindTexture(NetGL.OpenGL.Texture2D, font.RenderID);
            //foreach (Char c in text)
            //{
            //    float spacing = fontSize * font.Spacing;
            //    float xOffset = i * 20f - (i * spacing);
            //    RenderGlyphe(new Vec3(location.X + xOffset, location.Y), font, fontSize, c);
            //    i++;
            //}
            //gl.Color3f(1.0f, 1.0f, 1.0f);
            //gl.Disable(NetGL.OpenGL.Texture2D);
            this.DrawString(text,location,fontSize, font.Spacing,font,color);
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

            gl.Begin(NetGL.OpenGL.Quads);
            // 1 
            gl.TexCoord2f(xLeft, yTop);
            gl.Vertex3f(location.X, location.Y, 0f);
            gl.TexCoord2f(xLeft, yBottom);
            gl.Vertex3f(location.X, location.Y + size, 0f);
            gl.TexCoord2f(xRight, yBottom);
            gl.Vertex3f(location.X + size, location.Y + size, 0f);
            gl.TexCoord2f(xRight, yTop);
            gl.Vertex3f(location.X + size, location.Y, 0f);
            gl.End();
        }

        public void DrawMesh(Mesh mesh, Color color)
        {
            foreach (var face in mesh.Faces)
            {
                //Enable texture if the face got a texture
                if (face.Texture != null)
                {
                    gl.Enable(NetGL.OpenGL.Texture2D);
                    gl.BindTexture(NetGL.OpenGL.Texture2D, face.Texture.RenderID);
                    gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapS, NetGL.OpenGL.Repeate);
                    gl.TexParameteri(NetGL.OpenGL.Texture2D, NetGL.OpenGL.TextureWrapT, NetGL.OpenGL.Repeate);
                }
                //Render the Face
                gl.Begin(NetGL.OpenGL.Triangles);
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    //Set the tex cords if a texture is set
                    if (face.Texture != null)
                    {
                        var texCord = face.TexCords[i];
                        gl.TexCoord2f(texCord.X, texCord.Y);
                    }
                    //Render the vertex
                    var vertex = face.Vertices[i];
                    gl.Color3f(color.R / 255f, color.G / 255f, color.B / 255f);
                    gl.Vertex3f(vertex.X, vertex.Y, vertex.Z);
                    gl.Color3f(1f, 1f, 1f);
                }
                gl.End();
                if(face.Texture != null)
                {
                    gl.Disable(NetGL.OpenGL.Texture2D);
                }
            }
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

        public void InitGameElement(GameElement element)
        {
            
        }

        public void InitSprite(Sprite sprite)
        {

        }
    }
}
