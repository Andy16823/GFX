using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class MVPRectShader : ShaderProgram
    {
        public MVPRectShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec3 inColor;

                out vec3 color;
                out vec3 position;

                uniform mat4 mvp;

                void main()
                {
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    color = inColor;
                    position = inPosition;
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec3 color;
                in vec3 position;

                out vec4 fragColor;
                uniform float aspect;
                uniform float border_width;

                void main()
                {
                    float bw = (border_width / 100) * aspect;
                    float maxX = 0.5 - bw / aspect;
                    float minX = -0.5 + bw / aspect;
                    float maxY = 0.5 - bw;
                    float minY = -0.5 + bw;

                   if (position.x < maxX && position.x > minX && position.y < maxY && position.y > minY) {
                        gl_FragColor = vec4(color, 0.0);
                   } else {
                        gl_FragColor = vec4(color, 1.0);
                   }          
                }
            ");
        }
    }
}
