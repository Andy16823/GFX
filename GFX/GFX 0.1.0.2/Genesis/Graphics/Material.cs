using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Struktur zur Darstellung von Materialdaten, die in einem Material-Buffer gespeichert werden.
    /// </summary>
    public struct MaterialBuffer
    {
        /// <summary>
        /// Indicates whether the material buffer contains data.
        /// </summary>
        public bool HasData;

        /// <summary>
        /// Array for storing vertex data.
        /// </summary>
        public float[] Verticies;

        /// <summary>
        /// Array for storing normal data.
        /// </summary>
        public float[] Normals;

        /// <summary>
        /// Array for storing texture coordinates.
        /// </summary>
        public float[] Texcords;
    }

    /// <summary>
    /// Class representing a 3D material and managing its properties.
    /// </summary>
    public class Material
    {
        /// <summary>
        /// The diffuse color of the material.
        /// </summary>
        public Color DiffuseColor { get; set; }

        /// <summary>
        /// The path to the diffuse texture of the material.
        /// </summary>
        public String DiffuseTexture { get; set; }

        /// <summary>
        /// The path to the normal texture of the material.
        /// </summary>
        public String NormalTexture { get; set; }

        /// <summary>
        /// A collection of user-defined properties.
        /// </summary>
        public Dictionary<String, Object> Propeterys { get; set; }

        /// <summary>
        /// Constructor for the Material class. Initializes properties and the dictionary for user-defined properties.
        /// </summary>
        public Material()
        {
            this.Propeterys = new Dictionary<String, Object>();
        }
    }
}
