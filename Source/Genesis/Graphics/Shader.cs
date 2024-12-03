using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a shader used in graphics rendering.
    /// </summary>
    public class Shader
    {
        /// <summary>
        /// Gets or sets the ID of the shader.
        /// </summary>
        public int ShaderID { get; set; }

        /// <summary>
        /// Gets or sets the source code of the shader.
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// Default constructor for the Shader class.
        /// </summary>
        public Shader()
        {

        }

        /// <summary>
        /// Constructor for the Shader class that initializes the source code.
        /// </summary>
        /// <param name="source">The source code of the shader.</param>
        public Shader(String source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Creates a Shader object by reading the source code from a file.
        /// </summary>
        /// <param name="filename">The path to the file containing the shader source code.</param>
        /// <returns>A Shader object with the source code read from the file.</returns>
        public static Shader FromFile(String filename)
        {
            Shader shader = new Shader();
            shader.Source = System.IO.File.ReadAllText(filename);
            return shader;
        }
    }
}
