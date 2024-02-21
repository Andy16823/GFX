using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class WireframeShader : ShaderProgram
    {
        public WireframeShader() {
            this.VertexShader = new Shader(@"

                #version 330 core

                layout(location = 0) in vec3 inPosition;

                uniform mat4 projection;
                uniform mat4 view;

                void main()
                {
                    gl_Position = projection * view * vec4(inPosition, 1.0);
                }"
            );

            this.FragmentShader = new Shader(@"
                #version 330 core

                out vec4 fragColor;

                void main()
                {
                    fragColor = vec4(0.0, 1.0, 0.0, 1.0); // Grüne Linie (RGBA)
                }"
            );
        }
    }
}
