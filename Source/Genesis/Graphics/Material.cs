using Newtonsoft.Json;
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
        public String Name { get; set; }
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
        /// Gets or sets the opacity
        /// </summary>
        public float Opacity { get; set; }

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
            this.DiffuseColor = Color.White;
        }

        /// <summary>
        /// Constructor for the Material class. Initializes properties and the dictionary for user-defined properties.
        /// </summary>
        public Material(String name, Color DiffuseColor)
        {
            this.Propeterys = new Dictionary<String, Object>();
            this.Name = name;
            this.DiffuseColor = DiffuseColor;
        }

        /// <summary>
        /// Serializes the material object into a JSON string.
        /// </summary>
        /// <returns>Serialized JSON string of the material.</returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Saves the material object to a JSON file.
        /// </summary>
        /// <param name="filename">The path to save the material JSON file.</param>
        public void SaveMaterial(String filename)
        {
            File.WriteAllText(filename, this.Serialize());
        }

        /// <summary>
        /// Loads a material from a JSON file.
        /// </summary>
        /// <param name="filename">The path to the JSON file containing the material data.</param>
        /// <returns>The loaded material object.</returns>
        public static Material LoadMaterial(String filename)
        {
            var json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Material>(json);
        }
    }
}
