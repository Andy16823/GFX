using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    internal class SceneShader : ShaderProgram
    {
        public SceneShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec2 aTexCoords;

                out vec2 TexCoords;

                void main()
                {
                    gl_Position = vec4(aPos, 1.0); 
                    TexCoords = aTexCoords;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core
                out vec4 FragColor;
  
                in vec2 TexCoords;

                uniform sampler2D screenTexture;
                uniform float gamma;

                void main()
                { 
                    vec4 texColor = texture(screenTexture, TexCoords);
                    texColor.rgb = pow(texColor.rgb, vec3(1.0 / gamma));
                    FragColor = texColor;
                }
            ");
        }
    }
}
