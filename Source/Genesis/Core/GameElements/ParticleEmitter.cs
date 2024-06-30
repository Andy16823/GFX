using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents the buffers for particle data, including vertices, texture coordinates, colors, positions, rotations, and scales.
    /// </summary>
    public struct ParticleBuffers
    {
        public float[] verticies;
        public float[] texCords;
        public float[] colors;
        public float[] positions;
        public float[] rotations;
        public float[] scales;
    }

    /// <summary>
    /// Represents the definition of a particle, including location, rotation, size, delay, last update time, speed, rotation speed, and particle color.
    /// </summary>
    public class ParticleDeffinition {
        /// <summary>
        /// Gets or sets the location of the particle.
        /// </summary>
        public Vec3 Location { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the particle.
        /// </summary>
        public Vec3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the size of the particle.
        /// </summary>
        public Vec3 Size { get; set; }

        /// <summary>
        /// Gets or sets the delay before the particle becomes active.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// Gets or sets the last update time of the particle.
        /// </summary>
        public long LastUpdate { get; set; } = 0;

        /// <summary>
        /// Gets or sets the speed of the particle.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the rotation speed of the particle.
        /// </summary>
        public float RotationSpeed { get; set; }

        /// <summary>
        /// Gets or sets the color of the particle.
        /// </summary>
        public float[] ParticleColor { get; set; }
    }

    /// <summary>
    /// Represents a particle emitter as a game element.
    /// </summary>
    public class ParticleEmitter : GameElement
    {
        /// <summary>
        /// Gets or sets the texture used for the particles.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets or sets the primary direction of particle movement.
        /// </summary>
        public Vec3 ParticleDirection { get; set; } = new Vec3(1, 1, 0);

        /// <summary>
        /// Gets or sets the secondary direction of particle movement.
        /// </summary>
        public Vec3 ParticleDirection2 { get; set; } = new Vec3(-1, 1, 0);

        /// <summary>
        /// Gets or sets the maximum distance a particle can travel before being reset.
        /// </summary>
        public float ParticleDistance { get; set; } = 50f;

        /// <summary>
        /// Gets or sets the mask for the particles
        /// </summary>
        public Texture ParticleMask { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }


        /// <summary>
        /// Gets or sets the list of particle definitions managed by the emitter.
        /// </summary>
        public List<ParticleDeffinition> ParticleDeffinitions { get; set; }


        /// <summary>
        /// Initializes a new instance of the ParticleEmitter class with specified parameters.
        /// </summary>
        /// <param name="name">The name of the particle emitter.</param>
        /// <param name="location">The initial location of the particle emitter.</param>
        /// <param name="rotation">The initial rotation of the particle emitter.</param>
        /// <param name="size">The initial size of the particle emitter.</param>
        public ParticleEmitter(String name, Vec3 location, Vec3 rotation, Vec3 size, Texture mask) : base()
        {
            this.ParticleDeffinitions = new List<ParticleDeffinition>();
            this.Name = name;
            this.Location = location;
            this.Rotation = rotation;
            this.Size = size;
            this.ParticleMask = mask;
        }

        /// <summary>
        /// Generates particles with default settings, using the provided particle size.
        /// </summary>
        /// <param name="numPartikel">The number of particles to create.</param>
        /// <param name="particleSize">The size of each particle.</param>
        public void CreateParticles(int numPartikel, Vec3 particleSize)
        {
            this.CreateParticles(numPartikel, particleSize, Color.FromArgb(0,0,0), Color.FromArgb(255, 255, 255));
        }

        /// <summary>
        /// Generates particles with default settings, using the provided particle size and specified colors.
        /// </summary>
        /// <param name="numPartikel">The number of particles to create.</param>
        /// <param name="particleSize">The size of each particle.</param>
        /// <param name="colorA">The minimum color of particles.</param>
        /// <param name="colorB">The maximum color of particles.</param>
        public void CreateParticles(int numPartikel, Vec3 particleSize, Color colorA, Color colorB)
        {
            this.CreateParticles(numPartikel, particleSize, particleSize, new Vec3(0, 0, 0), new Vec3(0, 0, 360), 10, 100, 1f, 5f, 1f, 5f, colorA, colorB);
        }

        /// <summary>
        /// Generates particles with custom settings, including particle size, rotation, delay, speed, and rotation speed.
        /// </summary>
        /// <param name="numPartikel">The number of particles to create.</param>
        /// <param name="particleSize">The size of each particle.</param>
        /// <param name="minRotation">The minimum rotation of particles.</param>
        /// <param name="maxRotation">The maximum rotation of particles.</param>
        /// <param name="minDelay">The minimum delay before particles become active.</param>
        /// <param name="maxDelay">The maximum delay before particles become active.</param>
        /// <param name="minSpeed">The minimum speed of particles.</param>
        /// <param name="maxSpeed">The maximum speed of particles.</param>
        /// <param name="minRotationSpeed">The minimum rotation speed of particles.</param>
        /// <param name="maxRotationSpeed">The maximum rotation speed of particles.</param>
        /// <param name="colorA">The minimum color of particles.</param>
        /// <param name="colorB">The maximum color of particles.</param>
        public void CreateParticles(int numPartikel, Vec3 particleSize, Vec3 minRotation, Vec3 maxRotation, int minDelay, int maxDelay, float minSpeed, float maxSpeed, float minRotationSpeed, float maxRotationSpeed, Color colorA, Color colorB)
        {
            this.CreateParticles(numPartikel, particleSize, particleSize, minRotation, maxRotation, minDelay, maxDelay, minSpeed, maxSpeed, minRotationSpeed, maxRotationSpeed, colorA, colorB);
        }

        /// <summary>
        /// Generates particles with custom settings, including particle size range, rotation range, delay range, speed range, and rotation speed range.
        /// </summary>
        /// <param name="numPartikel">The number of particles to create.</param>
        /// <param name="particleSizeMax">The maximum size of particles.</param>
        /// <param name="particleSizeMin">The minimum size of particles.</param>
        /// <param name="minRotation">The minimum rotation of particles.</param>
        /// <param name="maxRotation">The maximum rotation of particles.</param>
        /// <param name="minDelay">The minimum delay before particles become active.</param>
        /// <param="maxDelay">The maximum delay before particles become active.</param>
        /// <param name="minSpeed">The minimum speed of particles.</param>
        /// <param name="maxSpeed">The maximum speed of particles.</param>
        /// <param name="minRotationSpeed">The minimum rotation speed of particles.</param>
        /// <param name="maxRotationSpeed">The maximum rotation speed of particles.</param>
        /// <param name="colorA">The minimum color of particles.</param>
        /// <param name="colorB">The maximum color of particles.</param>
        public void CreateParticles(int numPartikel, Vec3 particleSizeMax, Vec3 particleSizeMin, Vec3 minRotation, Vec3 maxRotation, int minDelay, int maxDelay, float minSpeed, float maxSpeed, float minRotationSpeed, float maxRotationSpeed, Color colorA, Color colorB)
        {
            this.StartColor = colorA;
            this.EndColor = colorB;

            Random random = new Random();
            for (int i = 0; i < numPartikel; i++)
            {
                Vec3 size = Vec3.Random(particleSizeMin, particleSizeMax, i);
                Vec3 rotation = Vec3.Random(minRotation, maxRotation, i);
                var delay = random.Next(minDelay, maxDelay); 

                ParticleDeffinition particleDeffinition = new ParticleDeffinition();
                particleDeffinition.Location = this.Location + new Vec3(0, 0, 0);
                particleDeffinition.Size = size;
                particleDeffinition.Rotation = rotation;
                particleDeffinition.Delay = delay;
                particleDeffinition.LastUpdate = Utils.GetCurrentTimeMillis();
                particleDeffinition.Speed = (float)(random.NextDouble() * (maxSpeed - minSpeed) + maxSpeed);
                particleDeffinition.RotationSpeed = (float)(random.NextDouble() * (maxRotationSpeed - minRotationSpeed) + maxRotationSpeed);
                particleDeffinition.ParticleColor = Utils.ConvertColor(StartColor);
                //particleDeffinition.ParticleColor = Utils.ConvertColor(Utils.GetRandomColor(colorA, colorB, i));
                this.ParticleDeffinitions.Add(particleDeffinition);
            }
        }

        /// <summary>
        /// Processes the movement and rotation of a particle based on its current state.
        /// </summary>
        /// <param name="particle">The particle definition to be processed.</param>
        /// <param name="now">The current time in milliseconds.</param>
        public void ProcessParticle(ParticleDeffinition particle, long now)
        {
            if(now > particle.LastUpdate + particle.Delay)
            {
                particle.Location += Vec3.Random(this.ParticleDirection, this.ParticleDirection2) * particle.Speed;
                var distance = this.Location.Distance(particle.Location);
                float progress = distance / this.ParticleDistance;
                var color = Utils.LerpColor(StartColor, EndColor, progress);
                particle.ParticleColor = color;

                if (distance > this.ParticleDistance)
                {
                    particle.Location = this.Location + new Vec3(0, 0, 0);
                    particle.Rotation.Z += particle.RotationSpeed;
                    particle.ParticleColor = Utils.ConvertColor(StartColor);
                }
                particle.LastUpdate = now;
            }
        }

        /// <summary>
        /// Generates the buffer for the particles, including vertices, colors, texture coordinates, positions, rotations, and scales.
        /// </summary>
        /// <returns>Particle buffers containing vertices, colors, texture coordinates, positions, rotations, and scales.</returns>
        public ParticleBuffers GetParticleBuffers()
        {
            var verticiesList = new List<float>();
            var colorsList = new List<float>();
            var texCordList = new List<float>();
            var positionsList = new List<float>();
            var rotationsList = new List<float>();
            var scalesList = new List<float>();

            var now = Utils.GetCurrentTimeMillis();

            foreach (var item in ParticleDeffinitions)
            {
                ProcessParticle(item, now);
                float[] verticies =
                {
                    -0.5f, -0.5f, 0.0f,
                    -0.5f, 0.5f, 0.0f,
                    0.5f, 0.5f, 0.0f,

                    -0.5f, -0.5f, 0.0f,
                    0.5f, 0.5f, 0.0f,
                    0.5f, -0.5f, 0.0f
                };
                verticiesList.AddRange(verticies);

                float[] ptColor = item.ParticleColor;
                float[] colors =
                {
                    ptColor[0], ptColor[1], ptColor[2],
                    ptColor[0], ptColor[1], ptColor[2],
                    ptColor[0], ptColor[1], ptColor[2],

                    ptColor[0], ptColor[1], ptColor[2],
                    ptColor[0], ptColor[1], ptColor[2],
                    ptColor[0], ptColor[1], ptColor[2]
                };
                colorsList.AddRange(colors);

                float[] texCords =
                {
                    0.0f, 0.0f,
                    0.0f, 1.0f,
                    1.0f, 1.0f,

                    0.0f, 0.0f,
                    1.0f, 1.0f,
                    1.0f, 0.0f
                };
                texCordList.AddRange(texCords);

                float[] positions =
                {
                    item.Location.X, item.Location.Y, item.Location.Z,
                    item.Location.X, item.Location.Y, item.Location.Z,
                    item.Location.X, item.Location.Y, item.Location.Z,

                    item.Location.X, item.Location.Y, item.Location.Z,
                    item.Location.X, item.Location.Y, item.Location.Z,
                    item.Location.X, item.Location.Y, item.Location.Z
                };
                positionsList.AddRange(positions);

                float[] rotations =
                {
                    item.Rotation.X, item.Rotation.Y, item.Rotation.Z,
                    item.Rotation.X, item.Rotation.Y, item.Rotation.Z,
                    item.Rotation.X, item.Rotation.Y, item.Rotation.Z,

                    item.Rotation.X, item.Rotation.Y, item.Rotation.Z,
                    item.Rotation.X, item.Rotation.Y, item.Rotation.Z,
                    item.Rotation.X, item.Rotation.Y, item.Rotation.Z
                };
                rotationsList.AddRange(rotations);

                float[] scales =
                {
                    item.Size.X, item.Size.Y, item.Size.Z,
                    item.Size.X, item.Size.Y, item.Size.Z,
                    item.Size.X, item.Size.Y, item.Size.Z,

                    item.Size.X, item.Size.Y, item.Size.Z,
                    item.Size.X, item.Size.Y, item.Size.Z,
                    item.Size.X, item.Size.Y, item.Size.Z
                };
                scalesList.AddRange(scales);

            }

            ParticleBuffers particleBuffers = new ParticleBuffers();
            particleBuffers.verticies = verticiesList.ToArray();
            particleBuffers.colors = colorsList.ToArray();
            particleBuffers.texCords = texCordList.ToArray();
            particleBuffers.positions = positionsList.ToArray();
            particleBuffers.rotations = rotationsList.ToArray();
            particleBuffers.scales = scalesList.ToArray();
            return particleBuffers;
        }

        /// <summary>
        /// Called when the particle emitter is being destroyed in the game.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public override void OnDestroy(Game game)
        {
            base.OnDestroy(game);
            game.RenderDevice.DisposeElement(this);
        }

        /// <summary>
        /// Renders the particle emitter using the specified render device.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            renderDevice.DrawGameElement(this);
            base.OnRender(game, renderDevice);
        }
    }
}
