using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class BufferedSpriteShader : ShaderProgram
    {
        public BufferedSpriteShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core
                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec2 inTexCoord;
                layout(location = 2) in vec4 inInstanceVertexColor;
                layout(location = 3) in mat4 inInstanceMatrix;
                layout(location = 7) in vec4 inUvTransform;
                layout(location = 8) in vec4 inExtras;

                uniform mat4 p_mat;
                uniform mat4 v_mat;

                out vec3 fragPos;
                out vec2 texCoord;
                out vec4 vertexColor;
                out vec4 uvTransform;
                out vec4 extras;

                void main() {
                    mat4 mvp = p_mat * v_mat * inInstanceMatrix;
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    fragPos = inPosition;
                    texCoord = inTexCoord;
                    vertexColor = inInstanceVertexColor;
                    uvTransform = inUvTransform;
                    extras = inExtras;
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core
                in vec3 fragPos;
                in vec2 texCoord;
                in vec4 vertexColor;
                in vec4 uvTransform;
                in vec4 extras;

                out vec4 FragColor;
                
                uniform sampler2D textureSampler;

                void main() {
                    if(extras.x == 0.0) {
                        discard;
                    }
                    //vec2 transformedTexCoord = texCoord * uvTransform.xy + uvTransform.zw;
                    //vec4 texColor = texture(textureSampler, transformedTexCoord);
                    //FragColor = texColor * vertexColor;

                    vec2 flippedTexCoord = vec2(texCoord.x, 1.0 - texCoord.y);
                    vec2 transformedTexCoord = flippedTexCoord * uvTransform.xy + uvTransform.zw;
                    vec4 texColor = texture(textureSampler, transformedTexCoord);
                    FragColor = texColor * vertexColor;
                }
            ");
        }
    }
}
