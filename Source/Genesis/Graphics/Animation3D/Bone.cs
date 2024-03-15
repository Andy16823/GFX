using Assimp;
using Genesis.Core;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Animation3D
{
    public struct KeyPosition
    {
        public vec3 position;
        public float timeStamp;
    };

    public struct KeyRotation
    {
        public quat orientation;
        public float timeStamp;
    };

    public struct KeyScale
    {
        public vec3 scale;
        public float timeStamp;
    };

    public class Bone
    {
        public List<KeyPosition> Positions { get; set; }
        public int NumPositions { get; set; }
        public List<KeyRotation> Rotations { get; set; }
        public int NumRotations { get; set; }
        public List<KeyScale> Scales { get; set; }
        public int NumScalings { get; set; }
        public mat4 LocalTransform { get; set; }
        public String Name { get; set; }
        public int ID { get; set; }


        public Bone(String name, int id, Assimp.NodeAnimationChannel channel)
        {
            this.Name = name;
            this.ID = id;
            this.LocalTransform = mat4.Identity;

            this.Positions = new List<KeyPosition>();
            NumPositions = channel.PositionKeyCount;
            for (int positionIndex = 0; positionIndex < NumPositions; ++positionIndex)
            {
                Vector3D aiPosition = channel.PositionKeys[positionIndex].Value;
                float timeStamp = (float)channel.PositionKeys[positionIndex].Time;
                KeyPosition data;
                data.position = Utils.GetGLMVec(aiPosition);
                data.timeStamp = timeStamp;
                Positions.Add(data);
            }

            this.Rotations = new List<KeyRotation>();
            NumRotations = channel.RotationKeyCount;
            for (int rotationIndex = 0; rotationIndex < NumRotations; ++rotationIndex)
            {
                Quaternion aiOrientation = channel.RotationKeys[rotationIndex].Value;
                float timeStamp = (float)channel.RotationKeys[rotationIndex].Time;
                KeyRotation data;
                data.orientation = Utils.GetGLMQuat(aiOrientation);
                data.timeStamp = timeStamp;
                Rotations.Add(data);
            }

            this.Scales = new List<KeyScale>();
            NumScalings = channel.ScalingKeyCount;
            for (int keyIndex = 0; keyIndex < NumScalings; ++keyIndex)
            {
                Vector3D scale = channel.ScalingKeys[keyIndex].Value;
                float timeStamp = (float)channel.ScalingKeys[keyIndex].Time;
                KeyScale data;
                data.scale = Utils.GetGLMVec(scale);
                data.timeStamp = timeStamp;
                Scales.Add(data);
            }
        }

        private int GetPositionIndex(float animationTime)
        {
            for (int index = 0; index < NumPositions - 1; ++index)
            {
                if (animationTime < Positions[index + 1].timeStamp)
                    return index;
            }
            Debug.Assert(false);
            return -1;
        }

        private int GetRotationIndex(float animationTime)
        {
            for (int index = 0; index < NumRotations - 1; ++index)
            {
                if (animationTime < Rotations[index + 1].timeStamp)
                    return index;
            }
            Debug.Assert(false);
            return -1;
        }

        private int GetScaleIndex(float animationTime)
        {
            for (int index = 0; index < NumScalings - 1; ++index)
            {
                if (animationTime < Scales[index + 1].timeStamp)
                    return index;
            }
            Debug.Assert(false);
            return -1;
        }

        public void Update(float animationTime)
        {
            //mat4 translation = InterpolatePosition(animationTime);
            //glm::mat4 rotation = InterpolateRotation(animationTime);
            //glm::mat4 scale = InterpolateScaling(animationTime);
            //m_LocalTransform = translation * rotation * scale;

            mat4 translation = mat4.Translate(Positions[GetPositionIndex(animationTime)].position);
            mat4 rotation = Rotations[GetRotationIndex(animationTime)].orientation.ToMat4;
            mat4 scale = mat4.Scale(Scales[GetScaleIndex(animationTime)].scale);
            LocalTransform = translation * rotation * scale;
        }
    }
}
