using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shaders.OpenGL
{
    public class ParticleShader : ShaderProgram
    {
        public ParticleShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec3 inColor;
                layout(location = 2) in vec2 inTexCoord;
                layout(location = 3) in vec3 inParticlePosition;
                layout(location = 4) in vec3 inParticleRotation;
                layout(location = 5) in vec3 inParticleSize;

                out vec3 color;
                out vec2 texCoord;

                uniform mat4 projection;
                uniform mat4 view;
                uniform mat4 model;

                mat4 translate(vec3 translation) {
                    return mat4(
                        vec4(1.0, 0.0, 0.0, 0.0),
                        vec4(0.0, 1.0, 0.0, 0.0),
                        vec4(0.0, 0.0, 1.0, 0.0),
                        vec4(translation, 1.0)
                    );
                }

                mat4 rotate(float angle) {
                    float cosA = cos(angle);
                    float sinA = sin(angle);
                    return mat4(
                        vec4(cosA, -sinA, 0.0, 0.0),
                        vec4(sinA, cosA, 0.0, 0.0),
                        vec4(0.0, 0.0, 1.0, 0.0),
                        vec4(0.0, 0.0, 0.0, 1.0)
                    );
                }

                mat4 scale(vec3 scaling) {
                    return mat4(
                        vec4(scaling.x, 0.0, 0.0, 0.0),
                        vec4(0.0, scaling.y, 0.0, 0.0),
                        vec4(0.0, 0.0, scaling.z, 0.0),
                        vec4(0.0, 0.0, 0.0, 1.0)
                    );
                }

                void main() {
                    mat4 model2 = translate(inParticlePosition) * rotate(inParticleRotation.z) * scale(inParticleSize);
                    mat4 mvp = projection * view * model2;

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
                    //vec2 flippedTexCoord = vec2(texCoord.x, -texCoord.y);
                    //fragColor = texture(textureSampler, flippedTexCoord) * vec4(color, 1.0);

                    float alpha = texture(textureSampler, texCoord).r;
                    fragColor = vec4(color.rgb, alpha);

                    //fragColor = vec4(color, 1.0);
                }
            ");
        }
    }
}
