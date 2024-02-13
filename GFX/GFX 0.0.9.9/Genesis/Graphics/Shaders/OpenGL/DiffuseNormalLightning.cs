using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class DiffuseNormalLightning : ShaderProgram
    {
        public DiffuseNormalLightning()
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
                uniform vec3 lightPos;
                uniform float lightIntensity;
                uniform vec3 lightColor;

                void main()
                {
                    vec3 ambient = lightIntensity * vec3(1.0, 1.0, 1.0);
                    vec2 flippedTexCoord = vec2(texCoord.x, 1.0 - texCoord.y);
                    
                    vec3 norm = texture(normalMap, fragNormal.xy).rgb;
                    // transform normal vector to range [-1,1]
                    norm = normalize(norm * 2.0 - 1.0);  

                    //vec3 norm = normalize(fragNormal);
                    vec3 lightDir = normalize(lightPos - fragPos);

                    float diff = max(dot(norm, lightDir), 0.0);
                    vec3 diffuse = diff * lightColor;

                    // Texturfarbe lesen
                    vec4 texColor = texture(textureSampler, flippedTexCoord);

                    vec3 result = (ambient + diffuse) * texColor.rgb;

                    // Setze den finalen Farbwert
                    fragColor = vec4(result, texColor.a);
                }
            ");
        }
    }
}
