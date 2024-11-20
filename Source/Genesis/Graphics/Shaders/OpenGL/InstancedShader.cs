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
                layout(location = 2) in mat4 inInstanceMatrix;  // Transformationsmatrix pro Instanz

                uniform mat4 projection;
                uniform mat4 view;

                out vec3 fragColor;

                void main() {
                    // Berechnung der Modell-View-Projection Matrix
                    mat4 mvp = projection * view * inInstanceMatrix;

                    // Setze die Vertex-Position
                    gl_Position = mvp * vec4(inPosition, 1.0);

                    // Setze eine Beispiel-Farbe
                    fragColor = vec3(1.0, 0.0, 0.0);  // Rot
                }      
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core
                in vec3 fragColor;
                out vec4 outColor;

                void main() {
                    outColor = vec4(fragColor, 1.0);  // Ausgabe der Farbe
                }
            ");
        }
    }
}
