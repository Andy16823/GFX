using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    /// <summary>
    /// Defines different collision groups for physics interactions.
    /// </summary>
    [Flags]
    public enum CollisionGroup
    {
        /// <summary>
        /// Represents no collision group.
        /// </summary>
        None = 0,

        /// <summary>
        /// Group for static objects that do not move.
        /// </summary>
        StaticObject = CollisionFilterGroups.StaticFilter,

        /// <summary>
        /// Group for player-controlled characters.
        /// </summary>
        Player = CollisionFilterGroups.CharacterFilter,

        /// <summary>
        /// Group for non-player characters (NPCs).
        /// </summary>
        NPC = 1 << 6,

        /// <summary>
        /// Group for vehicles.
        /// </summary>
        Vehicle = 1 << 7,

        /// <summary>
        /// Group for projectiles or other similar objects.
        /// </summary>
        Projectile = 1 << 8,

        /// <summary>
        /// Group for trigger volumes or sensors.
        /// </summary>
        Trigger = CollisionFilterGroups.SensorTrigger,

        /// <summary>
        /// Group for interactive objects.
        /// </summary>
        InteractiveObject = 1 << 9,

        /// <summary>
        /// Group for environmental objects.
        /// </summary>
        EnvironmentObject = 1 << 10,

        /// <summary>
        /// Group for particle systems.
        /// </summary>
        Particles = 1 << 11,

        /// <summary>
        /// Group for collision detection components.
        /// </summary>
        Collider = 1 << 12,

        /// <summary>
        /// Group for editor entities such as gizmos and debug objects.
        /// </summary>
        EditorEntity = 1 << 13,

        /// <summary>
        /// Group representing all collision groups.
        /// </summary>
        All = CollisionFilterGroups.AllFilter
    }

    /// <summary>
    /// Provides extension methods for the <see cref="CollisionGroup"/> enum.
    /// </summary>
    public static class CollisionGroupExtensions
    {
        /// <summary>
        /// Gets the integer value associated with a collision group.
        /// </summary>
        /// <param name="group">The collision group.</param>
        /// <returns>The integer value of the collision group.</returns>
        public static int GetGroup(this CollisionGroup group)
        {
            return (int)group;
        }

        /// <summary>
        /// Gets the collision mask for the specified collision group, defining which other groups it collides with.
        /// </summary>
        /// <param name="group">The collision group.</param>
        /// <returns>The collision mask for the collision group.</returns>
        public static int GetCollisionMask(this CollisionGroup group)
        {
            switch (group)
            {
                case CollisionGroup.None:
                    return (int) CollisionGroup.None;
                case CollisionGroup.StaticObject:
                    return (int)(CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.Vehicle | CollisionGroup.InteractiveObject | CollisionGroup.EnvironmentObject);

                case CollisionGroup.Player:
                    return (int)(CollisionGroup.StaticObject | CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.Vehicle | CollisionGroup.Trigger | CollisionGroup.InteractiveObject | CollisionGroup.Collider);

                case CollisionGroup.NPC:
                    return (int)(CollisionGroup.StaticObject | CollisionGroup.Player | CollisionGroup.Vehicle | CollisionGroup.Trigger | CollisionGroup.InteractiveObject);

                case CollisionGroup.Vehicle:
                    return (int)(CollisionGroup.StaticObject | CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.InteractiveObject | CollisionGroup.EnvironmentObject | CollisionGroup.Vehicle | CollisionGroup.Trigger);

                case CollisionGroup.Projectile:
                    return (int)(CollisionGroup.StaticObject | CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.Vehicle);

                case CollisionGroup.Trigger:
                    return (int)(CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.Vehicle);

                case CollisionGroup.InteractiveObject:
                    return (int)(CollisionGroup.StaticObject | CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.Vehicle);

                case CollisionGroup.EnvironmentObject:
                    return (int)(CollisionGroup.StaticObject | CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.Vehicle);

                case CollisionGroup.Particles:
                    return (int)CollisionGroup.None;

                case CollisionGroup.Collider:
                    return (int)(CollisionGroup.Player | CollisionGroup.NPC | CollisionGroup.Vehicle);

                case CollisionGroup.EditorEntity:
                    return (int)CollisionGroup.EditorEntity;

                case CollisionGroup.All:
                    return (int)CollisionGroup.All;

                default:
                    return (int)CollisionGroup.All;
            }
        }
    }
}
