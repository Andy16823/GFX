using GlmSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Animation3D
{
    public class Animator
    {
        public List<mat4> FinalBoneMatrices { get; set; }
        public Animation CurrentAnimation { get; set; }
        public float CurrentTime { get; set; }
        public float DeltaTime { get; set; }
        public bool Play { get; set; } = true;


        public Animator(Animation animation)
        {
            this.CurrentTime = 0;
            this.CurrentAnimation = animation;

            FinalBoneMatrices = new List<mat4>();
            for (int i = 0; i < 100; i++)
            {
                FinalBoneMatrices.Add(mat4.Identity);
            }
        }

        public void UpdateAnimation(float dt)
        {
            if(this.Play)
            {
                this.DeltaTime = dt;
                if (CurrentAnimation != null)
                {
                    this.CurrentTime += CurrentAnimation.TicksPerSecond * dt;
                    CurrentTime = CurrentTime % CurrentAnimation.Duration;
                    CalculateBoneTransform(CurrentAnimation.RootNode, mat4.Identity);
                }
            }
        }

        public void LoadAnimation(Animation3D.Animation animation)
        {
            this.Play = false;
            this.CurrentAnimation = animation;
            this.CurrentTime = 0;
            this.Play = true;
        }

        public void CalculateBoneTransform(AssimpNodeData node, mat4 parentTransform)
        {
            string nodeName = node.name;
            mat4 nodeTransform = node.transformation;

            Bone Bone = CurrentAnimation.FindBone(nodeName);

            if (Bone != null)
            {
                Bone.Update(CurrentTime);
                nodeTransform = Bone.LocalTransform;
            }

            mat4 globalTransformation = parentTransform * nodeTransform;

            var boneInfoMap = CurrentAnimation.BoneInfoMap;
            if (boneInfoMap.ContainsKey(nodeName))
            {
                int index = boneInfoMap[nodeName].id;
                mat4 offset = boneInfoMap[nodeName].offset;
                Debug.Assert(!glm.IsNaN(offset.m00));
                FinalBoneMatrices[index] = globalTransformation * offset;
            }

            for (int i = 0; i < node.childrenCount; i++)
            {
                CalculateBoneTransform(node.children[i], globalTransformation);
            }
        }
    }
}
