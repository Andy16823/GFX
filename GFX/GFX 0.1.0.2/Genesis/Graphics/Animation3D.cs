using BulletSharp.SoftBody;
using NetGL;
using OpenObjectLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    public class Animation3D
    {
        public String Name { get; set; }
        public List<Model> Frames { get; set; }

        public Animation3D(String name)
        {
            Name = name;
            Frames = new List<Model>();
        }

        public Animation3D(String name, String directory)
        {
            Name = name;
            Frames = new List<Model>();
            this.LoadFramesFromDirectory(directory);
        }

        public void LoadFramesFromDirectory(String directory)
        {
            OpenObjectLoader.WavefrontLoader wavefrontLoader = new OpenObjectLoader.WavefrontLoader();
            var files = System.IO.Directory.GetFiles(directory);
            foreach (var file in files) { 
                FileInfo fileInfo = new FileInfo(file);
                if(fileInfo.Extension == ".obj")
                {
                    this.Frames.Add(wavefrontLoader.LoadModel(file));
                }
            }
        }

        public void CopyTextures(Model model)
        {
            foreach (var item in this.Frames)
            {
                foreach (var material in item.Materials)
                {
                    var refMaterial = model.GetMaterial(material.Name);
                    material.Propeterys["tex_id"] = refMaterial.Propeterys["tex_id"];
                    material.Propeterys["normal_id"] = refMaterial.Propeterys["normal_id"];
                }
            }
        }

        public void InitAnimation(IRenderDevice renderer)
        {
            foreach (var frame in this.Frames)
            {
                foreach (var material in frame.Materials)
                {
                    int vbo = renderer.CreateDynamicVertexBuffer(material.IndexVerticies());
                    material.Propeterys.Add("vbo", vbo);

                    int tbo = renderer.CreateDynamicVertexBuffer(material.IndexTexCoords());
                    material.Propeterys.Add("tbo", tbo);

                    int nbo = renderer.CreateDynamicVertexBuffer(material.IndexNormals());
                    material.Propeterys.Add("nbo", nbo);

                    material.Propeterys.Add("tris", material.IndexVerticies().Length / 3);
                }
            }
        }

    }
}
