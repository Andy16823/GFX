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
    /// <summary>
    /// Represents a keyframe position in an animation.
    /// </summary>
    public struct KeyPosition
    {
        public vec3 position;
        public float timeStamp;
    };

    /// <summary>
    /// Represents a keyframe rotation in an animation.
    /// </summary>
    public struct KeyRotation
    {
        public quat orientation;
        public float timeStamp;
    };

    /// <summary>
    /// Represents a keyframe scale in an animation.
    /// </summary>
    public struct KeyScale
    {
        public vec3 scale;
        public float timeStamp;
    };

    /// <summary>
    /// Represents a bone in a skeletal animation system.
    /// </summary>
    public class Bone
    {
        /// <summary>
        /// List of position keyframes for the bone.
        /// </summary>
        public List<KeyPosition> Positions { get; set; }

        /// <summary>
        /// Number of position keyframes.
        /// </summary>
        public int NumPositions { get; set; }

        /// <summary>
        /// List of rotation keyframes for the bone.
        /// </summary>
        public List<KeyRotation> Rotations { get; set; }

        /// <summary>
        /// Number of rotation keyframes.
        /// </summary>
        public int NumRotations { get; set; }

        /// <summary>
        /// List of scale keyframes for the bone.
        /// </summary>
        public List<KeyScale> Scales { get; set; }

        /// <summary>
        /// Number of scale keyframes.
        /// </summary>
        public int NumScalings { get; set; }

        /// <summary>
        /// Local transformation matrix of the bone.
        /// </summary>
        public mat4 LocalTransform { get; set; }

        /// <summary>
        /// Name of the bone.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// ID of the bone.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Initializes a new instance of the Bone class.
        /// </summary>
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

        /// <summary>
        /// Gets the index of the position keyframe at the specified animation time.
        /// </summary>
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

        /// <summary>
        /// Gets the index of the rotation keyframe at the specified animation time.
        /// </summary>
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

        /// <summary>
        /// Gets the index of the scale keyframe at the specified animation time.
        /// </summary>
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

        /// <summary>
        /// Calculates the interpolation factor between two keyframes.
        /// </summary>
        private float GetScaleFactor(float lastTimeStamp, float nextTimeStamp, float animationTime)
        {
            float scaleFactor = 0.0f;
            float midWayLength = animationTime - lastTimeStamp;
            float framesDiff = nextTimeStamp - lastTimeStamp;
            scaleFactor = midWayLength / framesDiff;
            return scaleFactor;
        }

        /// <summary>
        /// Interpolates position for the bone at the specified animation time.
        /// </summary>
        private mat4 InterpolatePosition(float animationTime)
        {
            if (1 == NumPositions)
                return mat4.Identity * mat4.Translate(Positions[0].position);

            int p0Index = GetPositionIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(Positions[p0Index].timeStamp, Positions[p1Index].timeStamp, animationTime);
            vec3 finalPosition = vec3.Mix(Positions[p0Index].position, Positions[p1Index].position, scaleFactor);
            return mat4.Identity * mat4.Translate(finalPosition);
        }

        /// <summary>
        /// Interpolates rotation for the bone at the specified animation time.
        /// </summary>
        private mat4 InterpolateRotation(float animationTime)
        {
            if (1 == NumRotations)
            {
                var rotation = Rotations[0].orientation.Normalized;
                return rotation.ToMat4;
            }

            int p0Index = GetRotationIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(Rotations[p0Index].timeStamp, Rotations[p1Index].timeStamp, animationTime);
            quat finalRotation = quat.SLerp(Rotations[p0Index].orientation, Rotations[p1Index].orientation, scaleFactor);
            finalRotation = finalRotation.Normalized;

            if (glm.IsNaN(finalRotation.Length))
            {
                var rotation = Rotations[p0Index].orientation.Normalized;
                return rotation.ToMat4;
            }

            return finalRotation.ToMat4;
        }

        /// <summary>
        /// Interpolates scale for the bone at the specified animation time.
        /// </summary>
        private mat4 InterpolateScaling(float animationTime)
        {
            if (1 == NumScalings)
                return mat4.Identity * mat4.Scale(Scales[0].scale);

            int p0Index = GetScaleIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(Scales[p0Index].timeStamp, Scales[p1Index].timeStamp, animationTime);
            vec3 finalScale = vec3.Mix(Scales[p0Index].scale, Scales[p1Index].scale, scaleFactor);
            return mat4.Identity * mat4.Scale(finalScale);
        }

        /// <summary>
        /// Updates the bone transformation based on the animation time.
        /// </summary>
        public void Update(float animationTime, bool interpolate)
        {
            if(interpolate)
            {
                mat4 translation = InterpolatePosition(animationTime);
                mat4 rotation = InterpolateRotation(animationTime);
                mat4 scale = InterpolateScaling(animationTime);
                LocalTransform = translation * rotation * scale;
            }
            else
            {
                mat4 translation = mat4.Translate(Positions[GetPositionIndex(animationTime)].position);
                mat4 rotation = Rotations[GetRotationIndex(animationTime)].orientation.ToMat4;
                mat4 scale = mat4.Scale(Scales[GetScaleIndex(animationTime)].scale);
                LocalTransform = translation * rotation * scale;
            }            
        }
    }
}
