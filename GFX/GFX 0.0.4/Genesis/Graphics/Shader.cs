using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Shader
    {
        public int ShaderID { get; set; }
        public String Source { get; set; }

        public Shader()
        {

        }

        public Shader(String source)
        {
            this.Source = source;
        }

        public static Shader FromFile(String filename)
        {
            Shader shader = new Shader();
            shader.Source = System.IO.File.ReadAllText(filename);
            return shader;
        }
    }
}
