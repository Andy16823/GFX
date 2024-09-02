using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class LightmapShader : ShaderProgram
    {
        public LightmapShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core
                layout (location = 0) in vec3 aPos;

                uniform mat4 lightSpaceMatrix;
                uniform mat4 model;

                void main()
                {
                    gl_Position = lightSpaceMatrix * model * vec4(aPos, 1.0);
                }   
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                out vec4 FragColor;

                void main()
                {             
                    // Visualisiere den Tiefenwert als Graustufenfarbe
                    //float depthValue = gl_FragCoord.z;
                    //FragColor = vec4(vec3(depthValue), 1.0);
                } 
            ");
        }
    }
}
