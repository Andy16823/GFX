using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class DiffuseSolidShader : ShaderProgram
    {
        public DiffuseSolidShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec3 inColor;
                layout(location = 2) in vec3 inNormal;

                out vec3 fragPos;
                out vec3 color;
                out vec3 normal;

                uniform mat4 projection;
                uniform mat4 view;
                uniform mat4 model;

                void main()
                {
                    gl_Position = projection * view * model * vec4(inPosition, 1.0);
                    fragPos = vec3(model * vec4(inPosition, 1.0));
                    normal = mat3(transpose(inverse(model))) * inNormal;
                    color = inColor;
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec3 fragPos;
                in vec3 color;
                in vec3 normal;

                out vec4 fragColor;

                uniform vec3 lightPos;
                uniform float lightIntensity;
                uniform vec3 lightColor;

                void main()
                {
                    float ambientStrength = lightIntensity;
                    vec3 ambient = ambientStrength * lightColor;

                    vec3 norm = normalize(normal);
                    vec3 lightDir = normalize(lightPos - fragPos);  

                    float diff = max(dot(norm, lightDir), 0.0);
                    vec3 diffuse = diff * lightColor;

                    vec3 result = (ambient + diffuse) * color;
                    fragColor = vec4(result, 1.0);
                }
            ");

        }
    }
}
