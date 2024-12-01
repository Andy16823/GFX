using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class BorderCircleShader : ShaderProgram
    {
        public BorderCircleShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;

                out vec3 position;

                uniform mat4 mvp;
                uniform mat4 m_mat;

                void main()
                {
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    vec4 test = m_mat * vec4(inPosition, 1.0);
                    position = test.xyz;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core

                in vec3 position;

                out vec4 fragColor;

                uniform vec3 color;
                uniform float radius;
                uniform float border_width;

                void main()
                {
                    float distanceToCenter = length(position.xy - vec2(0.5, 0.5)); 
                    float borderDistance = radius - border_width;
                    
                    if (abs(distanceToCenter - radius) < border_width && distanceToCenter > borderDistance) {
                        fragColor = vec4(color, 1.0); 
                    } else {
                        discard;
                    }
                }
            ");
        }
    }
}
