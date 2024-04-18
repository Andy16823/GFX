﻿using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents an empty game element.
    /// </summary>
    public class Empty : GameElement
    {
        /// <summary>
        /// Initializes a new instance of the Empty class.
        /// </summary>
        public Empty() { 
        }

        /// <summary>
        /// Initializes a new instance of the Empty class.
        /// </summary>
        /// <param name="name">The name of the game element</param>
        /// <param name="location">The location of the game element</param>
        public Empty(String name, Vec3 location)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1);
            this.Rotation = Vec3.Zero();
        }
    }
}
