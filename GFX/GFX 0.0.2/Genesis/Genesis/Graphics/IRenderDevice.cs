using Genesis.Math;
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
        void Begin();
        void End();
        void LoadTexture(Texture texture);
        void LoadFont(Font font);
        void SetCamera(Camera camera);
        void PushMatrix();
        void PopMatrix();
        void Viewport(float x, float y, float width, float height);
        void Rotate(float angle, Vec3 vector);
        void Translate(Vec3 vector);
        void Translate(float x, float y, float z);
        void DrawSprite(Vec3 location, Vec3 size, Texture texture);
        void DrawSprite(Vec3 location, Vec3 size, Texture texture, TexCoords texCoords);
        void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture);
        void DrawSprite(Vec3 location, Vec3 size, Color color, Texture texture, TexCoords texCoords);
        void DrawTexture(Vec3 location, Vec3 size, float repeatX, float repeatY, Texture texture);
        void DrawRect(Rect rect, Color color);
        void DrawRect(Rect rect, Color color, float borderWidth);
        void FillRect(Rect rect, Color color);
        void DrawVectors(Vec3[] vecs, Color color);
        void DrawString(String text, Vec3 location, float fontsize, Font font, Color color);
        void DrawString(String text, Vec3 location, float fontsize, float spacing, Font font, Color color);
        void DrawMesh(Mesh mesh, Color color);
        void DisposeTexture(Texture texture);
        void DisposeFont(Font font);
        void ModelViewMatrix();
        void ProjectionMatrix();
        void TextureRepeatT();
        void TextureRepeatS();
        void TextureClampT();
        void TextureClampS();
        int GetError();
        IntPtr GetHandle();
    }
}
