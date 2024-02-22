using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class DiffuseNormalShader : ShaderProgram
    {
        public DiffuseNormalShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec3 inColor;
                layout(location = 2) in vec2 inTexCoord;
                layout(location = 3) in vec3 inNormal;

                out vec3 fragPos;
                out vec3 fragNormal;
                out vec3 color;
                out vec2 texCoord;

                uniform mat4 mvp;

                void main()
                {
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    fragPos = inPosition;
                    fragNormal = inNormal;
                    color = inColor;
                    texCoord = inTexCoord;
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec3 fragPos;
                in vec3 fragNormal;
                in vec3 color;
                in vec2 texCoord;

                out vec4 fragColor;
                uniform sampler2D textureSampler;
                uniform sampler2D normalMap;

                void main()
                {
                    vec2 flippedTexCoord = vec2(texCoord.x, 1.0 - texCoord.y);
                    vec3 norm = texture(normalMap, flippedTexCoord).xyz * 2.0 - 1.0;
                    norm = normalize(norm);

                    vec3 lightDir = normalize(vec3(0.0, 1.0, 1.0));
                    float diff = max(dot(norm, lightDir), 0.0);

                    vec4 texColor = texture(textureSampler, flippedTexCoord);

                    vec3 diffuse = diff * texture(textureSampler, flippedTexCoord).rgb * vec3(1.0, 1.0, 1.0);
                    fragColor = vec4(diffuse, texColor.a);
                }
            ");
        }
    }
}
