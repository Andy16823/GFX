using Genesis.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a callback for a specific frame in an animation.
    /// </summary>
    public class AnimationCallback
    {
        /// <summary>
        /// Delegate for the animation event callback.
        /// </summary>
        /// <param name="game">The game instance triggering the event.</param>
        /// <param name="element">The game element associated with the event.</param>
        public delegate void AnimationEvent(Game game, GameElement element);

        /// <summary>
        /// Gets or sets the name of the animation.
        /// </summary>
        public String AnimationName { get; set; }

        /// <summary>
        /// Gets or sets the frame number at which the callback is triggered.
        /// </summary>
        public int Frame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an animation callback has been triggered.
        /// </summary>
        /// <value>
        /// <c>true</c> if a callback has been triggered; otherwise, <c>false</c>.
        /// </value>
        public bool CallbackRised { get; set; } = false;

        /// <summary>
        /// Gets or sets the callback method to be invoked.
        /// </summary>
        public AnimationEvent Callback { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationCallback"/> class.
        /// </summary>
        public AnimationCallback()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationCallback"/> class with specified animation name, frame, and callback.
        /// </summary>
        /// <param name="animationName">The name of the animation.</param>
        /// <param name="frame">The frame number at which the callback is triggered.</param>
        /// <param name="callback">The callback method to be invoked.</param>
        public AnimationCallback(String animationName, int frame, AnimationEvent callback)
        {
            this.AnimationName = animationName;
            this.Frame = frame;
            this.Callback = callback;
        }
    }
}
