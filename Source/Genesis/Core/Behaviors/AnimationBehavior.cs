using Genesis.Core.GameElements;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Genesis.Core.Behaviors
{
    /// <summary>
    /// Represents a behavior that enables sprite animations in the Genesis framework.
    /// </summary>
    public class AnimationBehavior : IGameBehavior
    {
        /// <summary>
        /// Gets or sets the texture containing the animation frames.
        /// </summary>
        public Texture AnimationSheet { get; set; }

        /// <summary>
        /// Gets or sets the time in milliseconds between animation frames.
        /// </summary>
        public long FrameTime { get; set; }

        /// <summary>
        /// Gets or sets the number of cells (frames) in a row in the animation sheet.
        /// </summary>
        public float Cells { get; set; }

        /// <summary>
        /// Gets or sets the number of rows in the animation sheet.
        /// </summary>
        public float Rows { get; set; }

        /// <summary>
        /// Gets or sets the list of animations available for this behavior.
        /// </summary>
        public List<Animation> Animations { get; set; }

        /// <summary>
        /// Gets or sets the currently selected animation.
        /// </summary>
        public Animation SelectedAnimation { get; set; }

        private bool run;
        private Sprite sprite;
        private long lastFrame;
        private int currentCell;

        /// <summary>
        /// Initializes a new instance of the AnimationBehavior class.
        /// </summary>
        public AnimationBehavior()
        {
            this.Animations = new List<Animation>();
        }

        /// <summary>
        /// Initializes a new instance of the AnimationBehavior class with specified parameters.
        /// </summary>
        /// <param name="cells">The number of cells (frames) in a row in the animation sheet.</param>
        /// <param name="rows">The number of rows in the animation sheet.</param>
        /// <param name="frameTime">The time in milliseconds between animation frames.</param>
        /// <param name="animationSheet">The texture containing the animation frames.</param>
        public AnimationBehavior(float cells, float rows, long frameTime, Texture animationSheet)
        {
            this.Animations = new List<Animation>();
            this.Cells = cells;
            this.Rows = rows;
            this.FrameTime = frameTime;
            this.AnimationSheet = animationSheet;
        }


        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the game element is initialized.
        /// </summary>
        public override void OnInit(Game game, GameElement parent)
        {
            //if(parent.GetType() != typeof(Sprite))
            //{
            //    throw new Exception("The Animation Behavior is only usable with Sprites");
            //}
            sprite = (Sprite) parent;           
        }

        /// <summary>
        /// Adds an animation to the list of available animations.
        /// </summary>
        public void AddAnimation(Animation animation)
        {
            this.Animations.Add(animation);
        }

        /// <summary>
        /// Loads the specified animation by name.
        /// </summary>
        /// <param name="name">The name of the animation to load.</param>
        public void LoadAnimation(String name)
        {
            foreach (var animation in Animations)
            {
                if(animation.Name.Equals(name))
                {
                    SelectedAnimation = animation;
                    currentCell = animation.Cell;
                    sprite.Texture = AnimationSheet;
                }
            }
        }

        /// <summary>
        /// Starts playing the animation.
        /// </summary>
        public void Play()
        {
            if(!this.run)
            {
                this.run = true;
            }
        }

        /// <summary>
        /// Stops playing the animation.
        /// </summary>
        public void Stop()
        {
            if(this.run)
            {
                this.run = false;
            }
        }

        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        /// <summary>
        /// Called when the game element is updated.
        /// </summary>
        public override void OnUpdate(Game game, GameElement parent)
        {
            if(sprite == null)
            {
                sprite = (Sprite)parent;
            }

            long now = Utils.GetCurrentTimeMillis();
            if(now > lastFrame + FrameTime && run)
            {
                if(SelectedAnimation != null)
                {
                    Sprite sprite = (Sprite)parent;

                    if (sprite.Texture != AnimationSheet)
                    {
                        sprite.Texture = AnimationSheet;
                    }

                    float rowVal = 1 / ((float)Rows);
                    float colVal = 1 / ((float)Cells);


                    sprite.TexCoords.TopLeft.X = (float)currentCell * colVal;
                    sprite.TexCoords.TopLeft.Y = (float)SelectedAnimation.Row * rowVal;
                    sprite.TexCoords.TopRight.X = (float)currentCell * colVal + colVal;
                    sprite.TexCoords.TopRight.Y = (float)SelectedAnimation.Row * rowVal;
                    sprite.TexCoords.BottomRight.X = (float)currentCell * colVal + colVal;
                    sprite.TexCoords.BottomRight.Y = (float)SelectedAnimation.Row * rowVal + rowVal;
                    sprite.TexCoords.BottomLeft.X = (float)currentCell * colVal;
                    sprite.TexCoords.BottomLeft.Y = (float)SelectedAnimation.Row * rowVal + rowVal;

                    currentCell++;
                    if (currentCell >= SelectedAnimation.Cell + SelectedAnimation.Frames)
                    {
                        currentCell = SelectedAnimation.Cell;
                    }
                    lastFrame = now;
                }
            }
        }
    }
}
