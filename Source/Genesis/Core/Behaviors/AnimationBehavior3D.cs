//using Genesis.Core.GameElements;
//using Genesis.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Genesis.Core.Behaviors
//{
//    public class AnimationBehavior3D : IGameBehavior
//    {
//        public List<Animation3D> Animations { get; set; }
//        public int SelectedAnimation { get; set; } = -1;
//        public long FrameTime { get; set; } = 100;
//        public int CurrentFrame { get; set; }

//        private long lastFrame;
//        private bool play;

//        public AnimationBehavior3D()
//        {
//            Animations = new List<Animation3D>();
//        }

//        public override void OnDestroy(Game game, GameElement parent)
//        {
            
//        }

//        public override void OnInit(Game game, GameElement parent)
//        {
//            if (this.Parent.GetType() != typeof(Element3D))
//            {
//                throw new NotSupportedException();
//            }

//            var parentElement = (Element3D)this.Parent;
//            foreach (var animation in this.Animations)
//            {
//                animation.InitAnimation(game.RenderDevice);
//                animation.CopyTextures(parentElement.Model);
//            }
//        }

//        public void LoadAnimation(String name)
//        {
//            for (int i = 0; i < this.Animations.Count; i++)
//            {
//                if (this.Animations[i].Name == name)
//                {
//                    this.SelectedAnimation = i;
//                    break;
//                }
//            }
//        }

//        public void PlayAnimation()
//        {
//            if(this.SelectedAnimation != -1)
//            {
//                this.play = true;
//                this.CurrentFrame = 0;
//            }
//        }

//        public bool IsPlaying()
//        {
//            return this.play;
//        }

//        public override void OnRender(Game game, GameElement parent)
//        {
            
//        }

//        public override void OnUpdate(Game game, GameElement parent)
//        {
//            if (this.play)
//            {
//                var parentElement = (Element3D)this.Parent;
//                var now = Utils.GetCurrentTimeMillis();
//                if (now > lastFrame + FrameTime)
//                {
//                    CurrentFrame++;
//                    if (CurrentFrame >= this.Animations[SelectedAnimation].Frames.Count)
//                    {
//                        CurrentFrame = 0;
//                    }
//                    parentElement.Model = this.Animations[SelectedAnimation].Frames[CurrentFrame];
//                    lastFrame = now;
//                }
//            }
//        }

//        public void SetFrame(int frame)
//        {
//            var parentElement = (Element3D)this.Parent;
//            parentElement.Model = this.Animations[SelectedAnimation].Frames[frame];
//        }
//    }
//}
