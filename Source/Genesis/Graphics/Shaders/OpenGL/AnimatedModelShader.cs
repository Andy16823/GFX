using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class AnimatedModelShader : ShaderProgram
    {
        public AnimatedModelShader()
        {
            this.VertexShader = new Shader(@"
                #version 430 core

                layout(location = 0) in vec3 pos;
                layout(location = 1) in vec3 norm;
                layout(location = 2) in vec2 tex;
                layout(location = 3) in ivec4 boneIds; 
                layout(location = 4) in vec4 weights;
	
                uniform mat4 projection;
                uniform mat4 view;
                uniform mat4 model;
	
                const int MAX_BONES = 100;
                const int MAX_BONE_INFLUENCE = 4;
                uniform mat4 finalBonesMatrices[MAX_BONES];
	
                out vec2 texCoord;
	
                void main()
                {
                    vec4 totalPosition = vec4(0.0f);
                    for(int i = 0 ; i < MAX_BONE_INFLUENCE ; i++)
                    {
                        if(boneIds[i] == -1) 
                            continue;
                        if(boneIds[i] >=MAX_BONES) 
                        {
                            totalPosition = vec4(pos,1.0f);
                            break;
                        }
                        vec4 localPosition = finalBonesMatrices[boneIds[i]] * vec4(pos,1.0f);
                        totalPosition += localPosition * weights[i];
                        vec3 localNormal = mat3(finalBonesMatrices[boneIds[i]]) * norm;
                    }
		
                    mat4 viewModel = view * model;
                    gl_Position =  projection * viewModel * totalPosition;
                    texCoord = tex;
                }
            ");

            //Creating the fragment shader
            this.FragmentShader = new Shader(@"
                #version 430 core

                in vec3 fragPos;
                in vec3 color;
                in vec2 texCoord;

                out vec4 fragColor;
                uniform sampler2D textureSampler;
                uniform sampler2D normalMap;

                void main()
                {
                    vec2 flippedTexCoord = vec2(texCoord.x, 1.0 - texCoord.y);
                    vec4 texColor = texture(textureSampler, flippedTexCoord);

                    fragColor = texColor * vec4(1.0, 1.0, 1.0, 1.0);
                }
            ");
        }
    }
}
