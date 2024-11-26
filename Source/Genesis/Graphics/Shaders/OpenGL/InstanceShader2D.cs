using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class InstanceShader2D : ShaderProgram
    {
        public InstanceShader2D()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout(location = 0) in vec3 inPosition;
                layout(location = 4) in mat4 inInstanceMatrix;

                uniform mat4 projection;
                uniform mat4 view;

                out vec3 fragPos;
                out vec2 texCoord;

                void main() {
                    mat4 mvp = projection * view * inInstanceMatrix;
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    fragPos = inPosition;
                    texCoord = inTexCoord;
                }      
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec3 fragPos;
                in vec2 texCoord;

                out vec4 outColor;
                
                uniform sampler2D textureSampler;
                uniform vec4 materialColor;

                void main() {
                    outColor = texture(textureSampler, materialColor) * vec4(fragColor, 1.0);
                }
            ");
        }
    }
}
