using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    //public class BasicShader : ShaderProgram
    //{
    //    public BasicShader()
    //    {
    //        this.VertexShader = new Graphics.Shader(@"
    //            #version 330 core

    //            layout(location = 0) in vec3 inPosition;
    //            layout(location = 1) in vec3 inColor;
    //            layout(location = 2) in vec2 inTexCoord;

    //            out vec3 color;
    //            out vec2 texCoord;

    //            void main()
    //            {
    //                gl_Position = vec4(inPosition, 1.0);
    //                color = inColor;
    //                texCoord = inTexCoord;
    //            }
    //        ");

    //        this.FragmentShader = new Graphics.Shader(@"
    //            #version 330 core

    //            in vec3 color;
    //            in vec2 texCoord;

    //            out vec4 fragColor;
    //            uniform sampler2D textureSampler;

    //            void main()
    //            {
    //                fragColor = texture(textureSampler, texCoord) * vec4(color, 1.0);
    //            }
    //        ");
    //    }
    //}
    public class MVPShader : ShaderProgram
    {
        public MVPShader()
        {
            this.VertexShader = new Graphics.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec3 inColor;
                layout(location = 2) in vec2 inTexCoord;

                out vec3 color;
                out vec2 texCoord;

                uniform mat4 mvp;

                void main()
                {
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    color = inColor;
                    texCoord = inTexCoord;
                }
            ");

            this.FragmentShader = new Graphics.Shader(@"
                #version 330 core

                in vec3 color;
                in vec2 texCoord;

                out vec4 fragColor;
                uniform sampler2D textureSampler;

                void main()
                {
                    fragColor = texture(textureSampler, texCoord) * vec4(color, 1.0);
                }
            ");
        }
    }
}
