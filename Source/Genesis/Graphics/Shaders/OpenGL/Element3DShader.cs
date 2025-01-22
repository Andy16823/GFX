using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class Element3DShader : ShaderProgram
    {
        public Element3DShader()
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
                out vec4 FragPosLightSpace;

                uniform mat4 projection;
                uniform mat4 view;
                uniform mat4 model;

                uniform mat4 mvp;
                uniform mat4 lightSpaceMatrix;

                void main()
                {
                    //gl_Position = mvp * vec4(inPosition, 1.0);
                    fragPos = vec3(model * vec4(inPosition, 1.0));
                    fragNormal = transpose(inverse(mat3(model))) * inNormal;
                    color = inColor;
                    texCoord = inTexCoord;
                    FragPosLightSpace = lightSpaceMatrix * vec4(fragPos, 1.0);
                    gl_Position = projection * view * vec4(fragPos, 1.0);
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec3 fragPos;
                in vec3 fragNormal;
                in vec3 color;
                in vec2 texCoord;
                in vec4 FragPosLightSpace;

                out vec4 fragColor;
                uniform sampler2D textureSampler;
                uniform sampler2D normalMap;
                uniform sampler2D shadowMap;
                uniform vec3 lightPos;
                uniform float lightIntensity;
                uniform vec3 lightColor;
                uniform vec4 materialColor;

                float ShadowCalculation(vec4 fragPosLightSpace)
                {
                    // perform perspective divide
                    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
                    // transform to [0,1] range
                    projCoords = projCoords * 0.5 + 0.5;
                    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
                    float closestDepth = texture(shadowMap, projCoords.xy).r; 
                    // get depth of current fragment from light's perspective
                    float currentDepth = projCoords.z;
                    // check whether current frag pos is in shadow
                    float bias = max(0.05 * (1.0 - dot(fragNormal, lightPos)), 0.005);  
                    float shadow = 0.0;
                    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
                    for(int x = -1; x <= 1; ++x)
                    {
                        for(int y = -1; y <= 1; ++y)
                        {
                            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
                            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
                        }    
                    }
                    shadow /= 9.0;
    
                    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
                    if(projCoords.z > 1.0)
                        shadow = 0.0;
        
                    return shadow;
                }  
                
                void main()
                {
                    vec3 ambient = lightIntensity * vec3(1.0, 1.0, 1.0);
                    vec2 flippedTexCoord = vec2(texCoord.x, 1.0 - texCoord.y);
                    
                    vec3 norm = normalize(fragNormal);
                    vec3 lightDir = normalize(lightPos - fragPos);

                    float diff = max(dot(norm, lightDir), 0.0);
                    vec3 diffuse = diff * lightColor;

                    // Texturfarbe lesen
                    vec4 texColor = texture(textureSampler, flippedTexCoord);

                    float shadow = ShadowCalculation(FragPosLightSpace);   

                    vec3 result = (ambient + (1.0 - shadow) * diffuse) * texColor.rgb;

                    // Setze den finalen Farbwert
                    fragColor = materialColor * vec4(result, texColor.a);
                }
            ");
        }
    }
}
