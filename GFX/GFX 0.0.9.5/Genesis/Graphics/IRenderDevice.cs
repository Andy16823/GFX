using Genesis.Core;
using Genesis.Core.Prefabs;
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
    public interface IRenderDevice
    {        
        void Init();
        void InitGameElement(GameElement element);
        void InitSprite(Sprite sprite);
        void InitElement3D(Element3D element);
        void Begin();
        void End();
        Framebuffer BuildFramebuffer(int width, int height);
        Framebuffer BuildFramebuffer(int width, int height, int texture);
        Framebuffer BuildFramebuffer(int width, int height, Texture texture);
        void UpdateFramebufferSize(Framebuffer framebuffer, int width, int height);
        void SetFramebuffer(Framebuffer framebuffer);
        void SetFramebuffer(int framebuffer);
        void PrepareSceneRendering(Scene scene);
        void FinishSceneRendering(Scene scene);
        void PrepareCanvasRendering(Scene scene, Canvas canvas);
        void FinishCanvasRendering(Scene scene, Canvas canvas);
        void LoadTexture(Texture texture);
        void LoadFont(Font font);
        void SetCamera(Camera camera);
        void SetLightSource(Light light);
        void PushMatrix();
        void PopMatrix();
        void Viewport(float x, float y, float width, float height);
        void Rotate(float angle, Vec3 vector);
        void Translate(Vec3 vector);
        void Translate(float x, float y, float z);
        void DrawGameElement(GameElement element);
        void DrawSprite(Sprite sprite);
        void DrawSprite(Vec3 location, Vec3 size, Texture texture);
        void DrawSprite(Vec3 location, Vec3 size, Texture texture, TexCoords texCoords);
        void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture);
        void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture, TexCoords texCoords);
        void DrawBufferedSprite(BufferedSprite bufferedSprite);
        void DrawElement3D(Element3D element);
        void DrawTexture(Vec3 location, Vec3 size, float repeatX, float repeatY, Texture texture);
        void DrawRect(Rect rect, Color color);
        void DrawRect(Rect rect, Color color, float borderWidth);
        void FillRect(Rect rect, Color color);
        void DrawVectors(Vec3[] vecs, Color color);
        void DrawString(String text, Vec3 location, float fontsize, Font font, Color color);
        void DrawString(String text, Vec3 location, float fontsize, float spacing, Font font, Color color);
        void DrawMesh(Mesh mesh, Color color);
        void DrawLine(Vec3 from, Vec3 to, Color color);
        void DrawSkyBox(Skybox skybox);
        void DisposeTexture(Texture texture);
        void DisposeFont(Font font);
        void DisposeElement3D(Element3D element);
        void ModelViewMatrix();
        void ProjectionMatrix();
        void TextureRepeatT();
        void TextureRepeatS();
        void TextureClampT();
        void TextureClampS();
        void Dispose();
        int GetError();
        IntPtr GetHandle();
    }
}
