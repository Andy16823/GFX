using Genesis.Core.Prefabs;
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
    public class AnimationBehavior : IGameBehavior
    {
        public Texture AnimationSheet { get; set; }
        public long FrameTime { get; set; }
        public float Cells { get; set; }
        public float Rows { get; set; }
        public List<Animation> Animations { get; set; }
        public Animation SelectedAnimation { get; set; }

        private bool run;
        private Sprite sprite;
        private long lastFrame;
        private int currentCell;

        public AnimationBehavior()
        {
            this.Animations = new List<Animation>();
        }

        public AnimationBehavior(float cells, float rows, long frameTime, Texture animationSheet)
        {
            this.Animations = new List<Animation>();
            this.Cells = cells;
            this.Rows = rows;
            this.FrameTime = frameTime;
            this.AnimationSheet = animationSheet;
        }

        public void OnDestroy(Game game, GameElement parent)
        {
            
        }

        public void OnInit(Game game, GameElement parent)
        {
            //if(parent.GetType() != typeof(Sprite))
            //{
            //    throw new Exception("The Animation Behavior is only usable with Sprites");
            //}
            sprite = (Sprite) parent;           
        }

        public void AddAnimation(Animation animation)
        {
            this.Animations.Add(animation);
        }

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

        public void Play()
        {
            if(!this.run)
            {
                this.run = true;
            }
        }

        public void Stop()
        {
            if(this.run)
            {
                this.run = false;
            }
        }

        public void OnRender(Game game, GameElement parent)
        {
            
        }

        public void OnUpdate(Game game, GameElement parent)
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
