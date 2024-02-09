using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class ShaderProgram
    {
        public int ProgramID { get; set; }
        public Shader VertexShader { get; set; }
        public Shader FragmentShader { get; set; }

        public ShaderProgram()
        {
            this.VertexShader = new Shader();
            this.FragmentShader = new Shader();
        }
    }
}
