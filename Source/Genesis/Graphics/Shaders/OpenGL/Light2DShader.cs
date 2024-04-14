using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class Light2DShader : ShaderProgram
    {
        public Light2DShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec3 inColor;
                layout(location = 2) in vec2 inTexCoord;

                out vec2 texCoord;

                uniform mat4 mvp;

                void main()
                {
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    texCoord = inTexCoord;
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec2 texCoord;

                out vec4 fragColor;
                
                uniform sampler2D textureSampler;
                uniform vec3 color;

                void main()
                {
                    fragColor = texture(textureSampler, texCoord) * vec4(color, 1.0);
                }
            ");
        }
    }
}
