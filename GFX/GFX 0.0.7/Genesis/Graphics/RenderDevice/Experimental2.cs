﻿using Genesis.Core.Prefabs;
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

namespace Genesis.Graphics.RenderDevice
{
    public class Experimental2 : IRenderDevice
    {
        /// <summary>
        /// Struct for the viewport
        /// </summary>
        private float rot;
        private Camera camera;
        private mat4 p_mat;
        private mat4 v_mat;

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
            this.ShaderPrograms.Add("MVPRectShader", new Shaders.OpenGL.MVPRectShader());
            this.ShaderPrograms.Add("DiffuseShader", new Shaders.OpenGL.DiffuseShader());
            this.ShaderPrograms.Add("DiffuseNormalShader", new Shaders.OpenGL.DiffuseNormalShader());
            this.ShaderPrograms.Add("WireframeShader", new Shaders.OpenGL.WireframeShader());
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
            gl.Enable(OpenGL.DepthTest);
            gl.DepthFunc(OpenGL.Less);
            gl.Enable(OpenGL.Blend);
            gl.BlendFunc(OpenGL.SrcAlpha, OpenGL.OneMinusSrcAlpha);

            foreach (KeyValuePair<string, ShaderProgram> item in this.ShaderPrograms)
            {
                this.LoadShader(item.Key, item.Value);
            }

            this.BuildSpriteShape();
            this.BuildRectShape();
            this.BuildGlypheShape();
            this.CreateLineBuffer();
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

        public void CreateLineBuffer()
        {
            InstancedShape shape = new InstancedShape();
            shape.vbo = gl.GenBuffer(1);
            this.InstancedShapes.Add("LineShape", shape);
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
            Console.WriteLine(program.FragmentShader.Source);
        }

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
            mat4 mt_mat = mat4.Translate(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2), 0.0f);
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


            if(camera.Type == CameraType.Ortho)
            {
                p_mat = mat4.Ortho(x, right, y, top, 0.0f, 100.0f);
                v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));
            }
            else
            {
                //Vec3 cameraForward = Utils.CalculateCameraFront(camera);
                //vec3 cameraRotation = new vec3(camera.Rotation.X, camera.Rotation.Y, camera.Rotation.Z);

                //vec3 direction;
                //direction.x = (float)(System.Math.Cos(Utils.ToRadians(cameraRotation.y)) * System.Math.Cos(Utils.ToRadians(cameraRotation.x)));
                //direction.y = (float)System.Math.Sin(Utils.ToRadians(cameraRotation.x));
                //direction.z = (float)(System.Math.Sin(Utils.ToRadians(cameraRotation.y)) * System.Math.Cos(Utils.ToRadians(cameraRotation.x)));
                //vec3 cameraFront = glm.Normalized(direction);

                vec3 cameraPosition = camera.Location.ToGlmVec3();
                Vec3 cameraFront = Utils.CalculateCameraFront2(camera);

                p_mat = mat4.Perspective(Utils.ToRadians(45.0f), camera.Size.X / camera.Size.Y, camera.Near, camera.Far);
                v_mat =  mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));
            }

            if(this.camera == null)
            {
                this.camera = camera;
            }
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

            mat4 mt_mat = mat4.Translate(element.Location.X, element.Location.Y, element.Location.Z);
            mat4 mr_mat = mat4.RotateX(element.Rotation.X) * mat4.RotateY(element.Rotation.Y) * mat4.RotateZ(element.Rotation.Z);
            mat4 ms_mat = mat4.Scale(element.Scale.X, element.Scale.Y, element.Scale.Z);
            mat4 m_mat = mt_mat * mr_mat * ms_mat;
            mat4 mvp = p_mat * v_mat * m_mat;

            gl.Enable(OpenGL.Texture2D);
            foreach (var material in element.Model.Materials) 
            {
                gl.UseProgram(elementShaderID);
                gl.UniformMatrix4fv(gl.GetUniformLocation(elementShaderID, "mvp"), 1, false, mvp.ToArray());

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
    }
}