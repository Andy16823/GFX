using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class SolidShapeShader : ShaderProgram
    {
        public SolidShapeShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;

                uniform mat4 mvp;

                void main()
                {
                    gl_Position = mvp * vec4(inPosition, 1.0);
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                out vec4 fragColor;
                uniform vec4 color;

                void main()
                {
                    fragColor = color;
                }
            ");
        }
    }
}
