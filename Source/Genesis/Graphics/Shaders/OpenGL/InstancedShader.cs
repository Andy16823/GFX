using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class InstancedShader : ShaderProgram
    {
        public InstancedShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout(location = 0) in vec3 inPosition;  // Vertex-Position
                layout(location = 1) in vec3 inVertexColor;
                layout(location = 2) in vec2 inTexCoord;
                layout(location = 3) in vec3 inNormal;
                layout(location = 4) in mat4 inInstanceMatrix;  // Transformationsmatrix pro Instanz

                uniform mat4 projection;
                uniform mat4 view;

                out vec3 fragPos;
                out vec3 fragNormal;
                out vec3 fragColor;
                out vec2 texCoord;

                void main() {
                    // Berechnung der Modell-View-Projection Matrix
                    mat4 mvp = projection * view * inInstanceMatrix;

                    // Setze die Vertex-Position
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    fragPos = inPosition;
                    fragNormal = inNormal;
                    fragColor = inVertexColor;
                    texCoord = inTexCoord;
                }      
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec3 fragPos;
                in vec3 fragNormal;
                in vec3 fragColor;
                in vec2 texCoord;

                out vec4 outColor;
                
                uniform sampler2D textureSampler;
                uniform sampler2D normalMap;
                uniform vec3 lightPos;
                uniform float lightIntensity;
                uniform vec3 lightColor;
                uniform vec4 materialColor;

                uniform vec3 viewPos;


                void main() {
                    vec3 color = texture(textureSampler, texCoord).rgb;
                    float alpha = texture(textureSampler, texCoord).a;
                    vec3 normal = normalize(fragNormal);

                    vec3 ambient = lightIntensity * lightColor;
                    vec3 lightDir = normalize(lightPos - fragPos);

                    float diff = max(dot(lightDir, normal), 0.0);
                    vec3 diffuse = diff * lightColor;

                    vec3 viewDir = normalize(viewPos - fragPos);
                    vec3 reflectDir = reflect(-lightDir, normal);

                    float spec = 0.0;
                    vec3 halfwayDir = normalize(lightDir + viewDir); 
                    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
                    vec3 specular = spec * lightColor;

                    vec3 lighting = (ambient + diffuse + specular) * color;    

                    outColor = materialColor * vec4(lighting, alpha);
                }
            ");
        }
    }
}
