using Genesis.Core;
using Genesis.Core.GameElements;
using Genesis.Math;
using Genesis.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Interface for a rendering device that handles graphics rendering operations.
    /// </summary>
    public interface IRenderDevice
    {
        /// <summary>
        /// Initializes the rendering device.
        /// </summary>
        void Init();

        /// <summary>
        /// Initializes a game element for rendering.
        /// </summary>
        void InitGameElement(GameElement element);

        /// <summary>
        /// Initializes a sprite for rendering.
        /// </summary>
        void InitSprite(Sprite sprite);

        /// <summary>
        /// Initializes a 3D element for rendering.
        /// </summary>
        void InitElement3D(Element3D element);

        /// <summary>
        /// Begins the rendering process.
        /// </summary>
        void Begin();

        /// <summary>
        /// Ends the rendering process.
        /// </summary>
        void End();

        /// <summary>
        /// Creates a dynamic vertex buffer with the specified float array.
        /// </summary>
        /// <param name="floats">The float array for the dynamic vertex buffer.</param>
        /// <returns>The ID of the created dynamic vertex buffer.</returns>
        int CreateDynamicVertexBuffer(float[] floats);

        /// <summary>
        /// Creates a static vertex buffer with the specified vertices.
        /// </summary>
        /// <param name="verticies">The vertices for the static vertex buffer.</param>
        /// <returns>The ID of the created static vertex buffer.</returns>
        int CreateStaticVertexBuffer(float[] verticies);

        /// <summary>
        /// Builds a framebuffer with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the framebuffer.</param>
        /// <param name="height">The height of the framebuffer.</param>
        /// <returns>The built framebuffer.</returns>
        Framebuffer BuildFramebuffer(int width, int height);

        /// <summary>
        /// Builds a framebuffer with the specified width, height, and texture ID.
        /// </summary>
        /// <param name="width">The width of the framebuffer.</param>
        /// <param name="height">The height of the framebuffer.</param>
        /// <param name="texture">The ID of the texture associated with the framebuffer.</param>
        /// <returns>The built framebuffer.</returns>
        Framebuffer BuildFramebuffer(int width, int height, int texture);

        /// <summary>
        /// Builds a framebuffer with the specified width, height, and texture.
        /// </summary>
        /// <param name="width">The width of the framebuffer.</param>
        /// <param name="height">The height of the framebuffer.</param>
        /// <param name="texture">The texture associated with the framebuffer.</param>
        /// <returns>The built framebuffer.</returns>
        Framebuffer BuildFramebuffer(int width, int height, Texture texture);

        /// <summary>
        /// Updates the size of the specified framebuffer.
        /// </summary>
        /// <param name="framebuffer">The framebuffer to update.</param>
        /// <param name="width">The new width of the framebuffer.</param>
        /// <param name="height">The new height of the framebuffer.</param>
        void UpdateFramebufferSize(Framebuffer framebuffer, int width, int height);

        /// <summary>
        /// Sets the active framebuffer using its ID.
        /// </summary>
        /// <param name="framebuffer">The ID of the framebuffer to set as active.</param>
        void SetFramebuffer(Framebuffer framebuffer);

        /// <summary>
        /// Sets the active framebuffer using the specified framebuffer object.
        /// </summary>
        /// <param name="framebuffer">The framebuffer to set as active.</param>
        void SetFramebuffer(int framebuffer);

        /// <summary>
        /// Prepares the rendering for a 2D scene.
        /// </summary>
        /// <param name="scene">The 2D scene to prepare.</param>
        void PrepareSceneRendering(Scene scene);

        /// <summary>
        /// Finishes the rendering for a 2D scene.
        /// </summary>
        /// <param name="scene">The 2D scene to finish rendering.</param>
        void FinishSceneRendering(Scene scene);

        /// <summary>
        /// Prepares the render for the 2D lightmap
        /// </summary>
        /// <param name="scene"></param>
        void PrepareLightmap2D(Scene scene, Framebuffer framebuffer);

        /// <summary>
        /// Finish the lightmap 2D rendering
        /// </summary>
        /// <param name="scene"></param>
        void FinishLightmap2D(Scene scene, Framebuffer framebuffer);

        /// <summary>
        /// Prepares the rendering for a canvas within a 2D scene.
        /// </summary>
        /// <param name="scene">The 2D scene containing the canvas.</param>
        /// <param name="canvas">The canvas to prepare rendering for.</param>
        void PrepareCanvasRendering(Scene scene, Canvas canvas);

        /// <summary>
        /// Finishes the rendering for a canvas within a 2D scene.
        /// </summary>
        /// <param name="scene">The 2D scene containing the canvas.</param>
        /// <param name="canvas">The canvas to finish rendering.</param>
        void FinishCanvasRendering(Scene scene, Canvas canvas);

        /// <summary>
        /// Loads a texture into the rendering device.
        /// </summary>
        /// <param name="texture">The texture to load.</param>
        void LoadTexture(Texture texture);

        /// <summary>
        /// Loads a font into the rendering device.
        /// </summary>
        /// <param name="font">The font to load.</param>
        void LoadFont(Font font);

        /// <summary>
        /// Sets the camera for rendering.
        /// </summary>
        /// <param name="camera">The camera to set.</param>
        void SetCamera(Camera camera);

        /// <summary>
        /// Sets the light source for rendering.
        /// </summary>
        /// <param name="light">The light source to set.</param>
        void SetLightSource(Light light);

        /// <summary>
        /// Pushes the current matrix onto the matrix stack.
        /// </summary>
        void PushMatrix();

        /// <summary>
        /// Pops the matrix from the top of the matrix stack.
        /// </summary>
        void PopMatrix();

        /// <summary>
        /// Sets the viewport for rendering.
        /// </summary>
        /// <param name="x">The x-coordinate of the viewport.</param>
        /// <param name="y">The y-coordinate of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        void Viewport(float x, float y, float width, float height);

        /// <summary>
        /// Rotates the matrix by the specified angle around the given vector.
        /// </summary>
        /// <param name="angle">The rotation angle in degrees.</param>
        /// <param name="vector">The vector to rotate around.</param>
        void Rotate(float angle, Vec3 vector);

        /// <summary>
        /// Translates the matrix by the specified vector.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        void Translate(Vec3 vector);

        /// <summary>
        /// Translates the matrix by the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate translation.</param>
        /// <param name="y">The y-coordinate translation.</param>
        /// <param name="z">The z-coordinate translation.</param>
        void Translate(float x, float y, float z);

        /// <summary>
        /// Draws a game element in the current rendering context.
        /// </summary>
        /// <param name="element">The game element to draw.</param>
        void DrawGameElement(GameElement element);

        /// <summary>
        /// Draws a sprite in the current rendering context.
        /// </summary>
        /// <param name="sprite">The sprite to draw.</param>
        void DrawSprite(Sprite sprite);

        /// <summary>
        /// Draws a sprite with specified location, size, and texture in the current rendering context.
        /// </summary>
        /// <param name="location">The location of the sprite.</param>
        /// <param name="size">The size of the sprite.</param>
        /// <param name="texture">The texture of the sprite.</param>
        void DrawSprite(Vec3 location, Vec3 size, Texture texture);

        /// <summary>
        /// Draws a sprite with specified location, size, texture, and texture coordinates in the current rendering context.
        /// </summary>
        /// <param name="location">The location of the sprite.</param>
        /// <param name="size">The size of the sprite.</param>
        /// <param name="texture">The texture of the sprite.</param>
        /// <param name="texCoords">The texture coordinates of the sprite.</param>
        void DrawSprite(Vec3 location, Vec3 size, Texture texture, TexCoords texCoords);

        /// <summary>
        /// Draws a sprite with specified location, size, color, and texture in the current rendering context.
        /// </summary>
        /// <param name="location">The location of the sprite.</param>
        /// <param name="size">The size of the sprite.</param>
        /// <param name="color">The color of the sprite.</param>
        /// <param name="texture">The texture of the sprite.</param>
        void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture);

        /// <summary>
        /// Draws a sprite with specified location, size, color, texture, and texture coordinates in the current rendering context.
        /// </summary>
        /// <param name="location">The location of the sprite.</param>
        /// <param name="size">The size of the sprite.</param>
        /// <param name="color">The color of the sprite.</param>
        /// <param name="texture">The texture of the sprite.</param>
        /// <param name="texCoords">The texture coordinates of the sprite.</param>
        void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture, TexCoords texCoords);

        /// <summary>
        /// Draws a buffered sprite in the current rendering context.
        /// </summary>
        /// <param name="bufferedSprite">The buffered sprite to draw.</param>
        void DrawBufferedSprite(BufferedSprite bufferedSprite);

        /// <summary>
        /// Draws a 3D element in the current rendering context.
        /// </summary>
        /// <param name="element">The 3D element to draw.</param>
        void DrawElement3D(Element3D element);

        /// <summary>
        /// Draws a texture with specified location, size, and texture coordinates in the current rendering context.
        /// </summary>
        /// <param name="location">The location of the texture.</param>
        /// <param name="size">The size of the texture.</param>
        /// <param name="repeatX">The horizontal texture repeat factor.</param>
        /// <param name="repeatY">The vertical texture repeat factor.</param>
        /// <param name="texture">The texture to draw.</param>
        void DrawTexture(Vec3 location, Vec3 size, float repeatX, float repeatY, Texture texture);

        /// <summary>
        /// Draws a colored rectangle with specified position and size in the current rendering context.
        /// </summary>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        void DrawRect(Rect rect, Color color);

        /// <summary>
        /// Draws a colored rectangle with specified position, size, and border width in the current rendering context.
        /// </summary>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="borderWidth">The width of the rectangle border.</param>
        void DrawRect(Rect rect, Color color, float borderWidth);

        /// <summary>
        /// Fills a colored rectangle with specified position and size in the current rendering context.
        /// </summary>
        /// <param name="rect">The rectangle to fill.</param>
        /// <param name="color">The color to fill the rectangle with.</param>
        void FillRect(Rect rect, Color color);

        /// <summary>
        /// Draws vectors with specified positions in the current rendering context.
        /// </summary>
        /// <param name="vecs">The array of vectors to draw.</param>
        /// <param name="color">The color of the vectors.</param>
        void DrawVectors(Vec3[] vecs, Color color);

        /// <summary>
        /// Draws a string with specified text, location, fontsize, font, and color in the current rendering context.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="location">The location of the text.</param>
        /// <param name="fontsize">The fontsize of the text.</param>
        /// <param name="font">The font to use for drawing the text.</param>
        /// <param name="color">The color of the text.</param>
        void DrawString(String text, Vec3 location, float fontsize, Font font, Color color);

        /// <summary>
        /// Draws a string with specified text, location, fontsize, spacing, font, and color in the current rendering context.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="location">The location of the text.</param>
        /// <param name="fontsize">The fontsize of the text.</param>
        /// <param name="spacing">The spacing between characters.</param>
        /// <param name="font">The font to use for drawing the text.</param>
        /// <param name="color">The color of the text.</param>
        void DrawString(String text, Vec3 location, float fontsize, float spacing, Font font, Color color);

        /// <summary>
        /// Draws a mesh with specified mesh and color in the current rendering context.
        /// </summary>
        /// <param name="mesh">The mesh to draw.</param>
        /// <param name="color">The color of the mesh.</param>
        void DrawMesh(Mesh mesh, Color color);

        /// <summary>
        /// Draws a line with specified starting and ending points and color in the current rendering context.
        /// </summary>
        /// <param name="from">The starting point of the line.</param>
        /// <param name="to">The ending point of the line.</param>
        /// <param name="color">The color of the line.</param>
        void DrawLine(Vec3 from, Vec3 to, Color color);

        /// <summary>
        /// Draws a skybox in the current rendering context.
        /// </summary>
        /// <param name="skybox">The skybox to draw.</param>
        void DrawSkyBox(Skybox skybox);

        /// <summary>
        /// Disposes the specified texture, freeing up resources.
        /// </summary>
        /// <param name="texture">The texture to dispose.</param>
        void DisposeTexture(Texture texture);

        /// <summary>
        /// Disposes the specified font, freeing up resources.
        /// </summary>
        /// <param name="font">The font to dispose.</param>
        void DisposeFont(Font font);

        /// <summary>
        /// Disposes the game element
        /// </summary>
        /// <param name="element">The element to dispose</param>
        void DisposeElement(GameElement element);

        /// <summary>
        /// Disposes the specified 3D element, freeing up resources.
        /// </summary>
        /// <param name="element">The 3D element to dispose.</param>
        void DisposeElement3D(Element3D element);

        /// <summary>
        /// Sets the current rendering matrix to the model-view matrix.
        /// </summary>
        void ModelViewMatrix();

        /// <summary>
        /// Sets the current rendering matrix to the projection matrix.
        /// </summary>
        void ProjectionMatrix();

        /// <summary>
        /// Sets the texture wrapping mode to repeat in the T direction.
        /// </summary>
        void TextureRepeatT();

        /// <summary>
        /// Sets the texture wrapping mode to repeat in the S direction.
        /// </summary>
        void TextureRepeatS();

        /// <summary>
        /// Sets the texture wrapping mode to clamp in the T direction.
        /// </summary>
        void TextureClampT();

        /// <summary>
        /// Sets the texture wrapping mode to clamp in the S direction.
        /// </summary>
        void TextureClampS();

        /// <summary>
        /// Disposes of the rendering device and releases any resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Gets the last OpenGL error code.
        /// </summary>
        /// <returns>The error code.</returns>
        int GetError();

        /// <summary>
        /// Gets the handle (pointer) to the underlying rendering context.
        /// </summary>
        /// <returns>The handle to the rendering context.</returns>
        IntPtr GetHandle();
    }
}
