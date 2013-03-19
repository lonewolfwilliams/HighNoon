// Animation.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Animation
    {
        public event EventHandler AnimationCompleted;

        Texture2D spriteStrip;
        float scale;

        // The time since we last updated the frame
        int elapsedTime;
        // The time we display a frame until the next one
        int frameTime;

        //the colour of the frame
        Color color;
        
        //blitting
        Rectangle sourceRect = new Rectangle();
        Rectangle destinationRect = new Rectangle();

        // Width of a given frame
        public int FrameWidth;
        public int FrameHeight;
        public bool Active;
        public bool Looping;

        public int currentFrame;
        public int frameCount;
        public int startFrame;
        
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position,
            int frameWidth, int frameHeight, int frameStart, int frameCount,
            int frametime, Color color, float scale, bool looping)
        {
            // Keep a local copy of the values passed in
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.startFrame = frameStart;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Set the Animation to active by default
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (Active == false)
                return;

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
                (int)Position.Y - (int)(FrameHeight * scale) / 2,
                (int)(FrameWidth * scale),
                (int)(FrameHeight * scale));

            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                currentFrame++;
                if ((currentFrame - startFrame) >= frameCount)
                {
                    currentFrame = startFrame;

                    // If we are not looping deactivate the animation
                    if (Looping == false)
                    {
                        Active = false;
                    }

                    if (AnimationCompleted != null)
                    {
                        AnimationCompleted(this, EventArgs.Empty);
                    }
                }

                elapsedTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool flip)
        {
            if (flip) spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color, 0f, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 1f);
            else spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
        }

        public void reset()
        {
            Active = true;
            currentFrame = startFrame;
        }
    }
}
