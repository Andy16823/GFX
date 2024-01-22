using Genesis.Core.Prefabs;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.Prefabs
{
    public class Skybox : Element3D
    {
        public Skybox(string name, string path, Vec3 location, Vec3 rotation, Vec3 scale) : base(name, path, location, rotation, scale)
        {
        }
    }
}
