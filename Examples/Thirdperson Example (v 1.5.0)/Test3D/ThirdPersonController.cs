using Genesis.Core;
using Genesis.Core.Behaviors.Physics3D;
using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Math;
using Genesis.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test3D
{
    public class ThirdPersonController : IGameBehavior
    {
        public enum Animation
        {
            ANIMATION_IDLE,
            ANIMATION_WALK,
            ANIMATION_RUNNING,
            ANIMATION_JUMPING,
            ANIMATION_STRAFE_RIGHT,
            ANIMATION_STRAFE_LEFT,
        }

        public RigidBodyBehavior3D RigidBody { get; set; }

        public String IdleAnimation { get; set; }
        public String WalkAnimation { get; set; }
        public String RunningAnimation { get; set; }
        public String JumpingAnimation { get; set; }
        public String StrafeLeftAnimation { get; set; }
        public String StrafeRightAnimation { get; set; }

        public bool Airborn { get; set; }

        private long lastJump = 0;

        public ThirdPersonController(RigidBodyBehavior3D rigidBody)
        {
            this.RigidBody = rigidBody;
            rigidBody.OnCollide += RigidBody_OnCollide;
            rigidBody.PhysicHandler.BeforePhysicsUpdate += PhysicHandler_BeforePhysicsUpdate;
            rigidBody.RigidBody.AngularFactor = new BulletSharp.Math.Vector3(0f, 0f, 0f);
        }

        private void PhysicHandler_BeforePhysicsUpdate(Scene scene, Game game, object element)
        {
            this.Airborn = true;
        }

        private void RigidBody_OnCollide(Scene scene, Game game, GameElement collision)
        {
            Console.WriteLine($"Coliding with {collision.Name}");
            this.Airborn = false;
        }

        public override void OnDestroy(Game game, GameElement parent)
        {
            
        }

        public override void OnRender(Game game, GameElement parent)
        {
            
        }

        public override void OnUpdate(Game game, GameElement parent)
        {
            Console.WriteLine(game.FPS);
            var model = (Model)parent;
            var animation = Animation.ANIMATION_IDLE;
            Vec3 oldVeloicity = RigidBody.GetLinearVelocity();
            var linearVelocity = new Vec3(0, oldVeloicity.Y, 0);
            float angularVelocity = 0f;
            float speed = ((float)game.DeltaTime) * 0.15f;

            if(this.Airborn)
            {
                linearVelocity = oldVeloicity;
            }

            if (Input.IsKeyDown(Input.Keys.W) && !Airborn)
            {
                animation = Animation.ANIMATION_WALK;
                if (Input.IsKeyDown(Input.Keys.Shift))
                {
                    speed = ((float)game.DeltaTime) * 0.5f;
                    animation = Animation.ANIMATION_RUNNING;
                }
                var vec = RigidBody.CalculateForwardVector(-speed);
                linearVelocity.X = vec.X;
                linearVelocity.Z = vec.Z;
                
            }
            else if (Input.IsKeyDown(Input.Keys.S))
            {
                var vec = RigidBody.CalculateForwardVector(speed);
                linearVelocity.X = vec.X;
                linearVelocity.Z = vec.Z;
                animation = Animation.ANIMATION_WALK;
            }

            // Straving
            if(Input.IsKeyDown(Input.Keys.A))
            {
                var vec = RigidBody.CalculateRightVector(speed);
                linearVelocity.X = vec.X;
                linearVelocity.Z = vec.Z;
                animation = Animation.ANIMATION_STRAFE_LEFT;
            }
            else if(Input.IsKeyDown(Input.Keys.D))
            {
                var vec = RigidBody.CalculateRightVector(-speed);
                linearVelocity.X = vec.X;
                linearVelocity.Z = vec.Z;
                animation = Animation.ANIMATION_STRAFE_RIGHT;
            }

            // Jumping
            if (Input.IsKeyDown(Input.Keys.Space) && CanJump())
            {
                var jumpSpeed = (float)game.DeltaTime * 1.2f;
                linearVelocity.Y += jumpSpeed;
                animation = Animation.ANIMATION_JUMPING;
                lastJump = Utils.GetCurrentTimeMillis();
            }

            // Adjust the rotation according to the mouse position
            var windowLocation = Window.GetClientLocation(game.RenderDevice.GetHandle());
            var mouseX = (int) windowLocation.X + (int)(game.Viewport.Width / 2);
            var mouseY = (int) windowLocation.Y + (int)(game.Viewport.Height / 2);
            Vec3 currentCursoPos = Input.GetMousePos();
            Input.SetCursorPos(mouseX, mouseY);
            Vec3 diff = currentCursoPos - Input.GetMousePos();
            angularVelocity -= (diff.X / 10);

            // Setup the velocity for the behavior
            RigidBody.LinearVelocity(linearVelocity);
            RigidBody.AngularVelocity(0f, angularVelocity, 0f);

            // Animation
            switch (animation)
            {
                case Animation.ANIMATION_IDLE:
                    this.PlayAnimation(this.IdleAnimation, model);
                    break;
                case Animation.ANIMATION_WALK:
                    this.PlayAnimation(this.WalkAnimation, model);
                    break;
                case Animation.ANIMATION_RUNNING:
                    this.PlayAnimation(this.RunningAnimation, model);
                    break;
                case Animation.ANIMATION_JUMPING:
                    this.PlayAnimation(this.JumpingAnimation, model);
                    break;
                case Animation.ANIMATION_STRAFE_RIGHT:
                    this.PlayAnimation(this.StrafeRightAnimation, model);
                    break;
                case Animation.ANIMATION_STRAFE_LEFT:
                    this.PlayAnimation(this.StrafeLeftAnimation, model);
                    break;
                default:
                    this.PlayAnimation(this.IdleAnimation, model);
                    break;
            }

            // Setup the camera
            var camera = game.SelectedScene.Camera;
            var fwd = Utils.GetTransformedForwardVector(parent.Rotation, 1.5f);
            camera.Location = RigidBody.GetLocation() + fwd;
            camera.Location = camera.Location.AddY(0.5f);
            Utils.LookAtY(camera, RigidBody.GetLocation());
            camera.Rotation = camera.Rotation.SubX((diff.Y / 50));
        }

        private bool CanJump()
        {
            var now = Utils.GetCurrentTimeMillis();
            if (now > lastJump + 500 && this.Airborn == false)
            {
                return true;
            }
            return false;
        }

        private void PlayAnimation(String animation, Model model)
        {
            if (model.Animator.CurrentAnimation.Name != animation)
            {
                model.PlayAnimation(animation);
            }
        }

        public override void OnInit(Game game, GameElement parent)
        {
            Input.HideCursor();
            var camera = game.SelectedScene.Camera;
            var fwd = Utils.GetTransformedForwardVector(parent.Rotation, 1.5f);
            camera.Location = RigidBody.GetLocation() + fwd;
            camera.Location = camera.Location.AddY(0.5f);
            Utils.LookAt(camera, RigidBody.GetLocation());
        }
    }
}
