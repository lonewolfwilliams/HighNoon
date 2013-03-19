using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class GunslingerViewComponent
    {
        public event EventHandler DeathAnimationCompleted;

        //state machine
        private Animation IdleAnimation;
        private Animation ShootingAnimation;
        private Animation DeathAnimation;
        private Animation GunjamAnimation;
        private Animation currentAnimation;

        public Vector2 position;
        public bool isLeftPlayer;
        public int playerBinding;

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public GunslingerViewComponent()
        {
            playerBinding = -1;
        }

        public int getBinding()
        {
            return playerBinding;
        }

        //dependency injection
        public void Initialize(AnimationFactory factory, bool isLeftPlayer)
        {
            IdleAnimation = factory.create(AnimationTypes.idle);
            ShootingAnimation = factory.create(AnimationTypes.shooting);
            DeathAnimation = factory.create(AnimationTypes.death);
            DeathAnimation.AnimationCompleted += DeathAnimation_AnimationCompleted;
            GunjamAnimation = factory.create(AnimationTypes.gunjam);

            position = new Vector2(0, 0);

            this.isLeftPlayer = isLeftPlayer;

            currentAnimation = null;
        }

        void DeathAnimation_AnimationCompleted(object sender, EventArgs e)
        {
            if (DeathAnimationCompleted != null)
            {
                DeathAnimationCompleted(this, EventArgs.Empty);
            }
        }

        public void update(GameTime withGameTime)
        {
            currentAnimation.Update(withGameTime);
        }
        
        public void Draw(SpriteBatch withContext)
        {
            currentAnimation.Position = this.position;
            bool flip = !isLeftPlayer;
            currentAnimation.Draw(withContext, flip);
        }

        public void doIdleAnimation()
        {
            if (currentAnimation == IdleAnimation) return;
            currentAnimation = IdleAnimation;
            resetCurrentAnimation();
        }
        public void doShootingAnimation()
        {
            if (currentAnimation == ShootingAnimation) return;
            currentAnimation = ShootingAnimation;
            resetCurrentAnimation();
        }
        public void doDeathAnimation()
        {
            if (currentAnimation == DeathAnimation) return;
            currentAnimation = DeathAnimation;
            resetCurrentAnimation();
        }
        public void doGunjamAnimation()
        {
            if (currentAnimation == GunjamAnimation) return;
            currentAnimation = GunjamAnimation;
            resetCurrentAnimation();
        }

        private void resetCurrentAnimation()
        {
            currentAnimation.Position = this.position;
            currentAnimation.reset();
        }
    }
}
