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
    /// <summary>
    /// Represents data associated with a node in the Assimp scene hierarchy.
    /// </summary>
    public struct AssimpNodeData
    {
        public mat4 transformation;
        public string name;
        public int childrenCount;
        public List<AssimpNodeData> children;
    };

    /// <summary>
    /// Represents an animation associated with a 3D model.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Name of the animation.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Duration of the animation in ticks.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Number of ticks per second for the animation.
        /// </summary>
        public float TicksPerSecond { get; set; }

        /// <summary>
        /// List of bones affected by the animation.
        /// </summary>
        public List<Bone> Bones { get; set; }

        /// <summary>
        /// Root node of the animation's scene hierarchy.
        /// </summary>
        public AssimpNodeData RootNode { get; set; }

        /// <summary>
        /// Mapping of bone names to bone information.
        /// </summary>
        public Dictionary<String, boneinfo> BoneInfoMap { get; set; }

        /// <summary>
        /// Initializes a new instance of the Animation class.
        /// </summary>
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

        /// <summary>
        /// Reads hierarchy data from the Assimp scene node.
        /// </summary>
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

        /// <summary>
        /// Reads bones engaged in the animation and their keyframes.
        /// </summary>
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

        /// <summary>
        /// Finds a bone with the specified name.
        /// </summary>
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
