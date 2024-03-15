using Assimp;
using Genesis.Core;
using Genesis.Core.GameElements;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Animation3D
{

    public struct AssimpNodeData
    {
        public mat4 transformation;
        public string name;
        public int childrenCount;
        public List<AssimpNodeData> children;
    };

    public class Animation
    {
        public String Name { get; set; }
        public float Duration { get; set; }
        public float TicksPerSecond { get; set; }
        public List<Bone> Bones { get; set; }
        public AssimpNodeData RootNode { get; set; }
        public Dictionary<String, boneinfo> BoneInfoMap { get; set; }

        public Animation(Assimp.Scene scene, Model model, int index)
        {
            this.Bones = new List<Bone>();
            var animation = scene.Animations[index];
            this.Name = animation.Name;
            this.Duration = (float)animation.DurationInTicks;
            this.TicksPerSecond = (float)animation.TicksPerSecond;
            var rootNode = new AssimpNodeData();
            this.ReadHeirarchyData(ref rootNode, scene.RootNode);
            this.RootNode = rootNode;
            ReadMissingBones(animation, model);
        }

        void ReadHeirarchyData(ref AssimpNodeData dest, Assimp.Node src)
        {
            Debug.Assert(src != null);
            dest.name = src.Name;
            dest.transformation = Utils.ConvertToGlmMat4(src.Transform);
            dest.childrenCount = src.ChildCount;
            dest.children = new List<AssimpNodeData>();

            for (int i = 0; i < src.ChildCount; i++)
            {
                AssimpNodeData newData = new AssimpNodeData();
                ReadHeirarchyData(ref newData, src.Children[i]);
                dest.children.Add(newData);
            }
        }

        void ReadMissingBones(Assimp.Animation animation, Model model)
        {
            int size = animation.NodeAnimationChannelCount;


            //reading channels(bones engaged in an animation and their keyframes)
            for (int i = 0; i < size; i++)
            {
                var channel = animation.NodeAnimationChannels[i];
                string boneName = channel.NodeName;

                if (!model.BoneInfoMap.ContainsKey(boneName))
                {
                    boneinfo boneinfo = new boneinfo();
                    boneinfo.id = model.BoneCounter;

                    // why no offset????
                    model.BoneInfoMap.Add(boneName, boneinfo);
                    model.BoneCounter++;
                }
                Bones.Add(new Bone(boneName, model.BoneInfoMap[boneName].id, channel));
            }

            BoneInfoMap = model.BoneInfoMap;
        }

        public Bone FindBone(string name)
        {
            var bone = Bones.FirstOrDefault(b => b.Name == name);
            if (bone != null)
            {
                return bone;
            }
            return null;
        }
    }
}
