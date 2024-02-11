using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class TerrainShader : ShaderProgram
    {
        public TerrainShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec3 inColor;  // Hinzugefügt für die Farbdaten

                out vec4 fragColor;  // Änderung von vec3 zu vec4

                uniform mat4 mvp;

                void main()
                {
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    fragColor = vec4(inColor, 1.0);  // Verwendung von inColor für die Farbdaten
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec4 fragColor;  // Änderung von vec3 zu vec4
                out vec4 finalColor;  // Änderung von vec4 zu vec4

                void main()
                {
                    finalColor = fragColor;  // Verwendung von fragColor anstelle von color
                }
            ");
        }
    }
}
