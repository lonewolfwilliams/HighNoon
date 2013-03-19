using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public enum AnimationTypes { idle, shooting, gunjam, death, leftbutton, rightbutton };

    public class AnimationFactory
    {
        private Vector2 offscreen;

        private int _cellWidth;
        private int _cellHeight;
        
        public int cellWidth
        {
            get { return _cellWidth; }
        }
        public int cellHeight
        {
            get { return _cellHeight; }
        }

        public ContentManager withContext;
        public AnimationFactory(ContentManager withContext) 
        {
            this.withContext = withContext;

            offscreen = new Vector2(0, 0);
            _cellHeight = 32;
            _cellWidth = 32;
        }
        
        public Animation create(AnimationTypes from)
        {
            Animation animationCreated = new Animation();
            
            int startFrame;
            int numberOfFrames;
            int frameDuration;

            switch (from)
            {
                case AnimationTypes.shooting:
                    {
                        startFrame = 0;
                        numberOfFrames = 2;
                        frameDuration = 480;
                        Texture2D playerTexture = withContext.Load<Texture2D>("gunslinger");
                        animationCreated.Initialize(playerTexture, offscreen,
                            _cellWidth, _cellHeight, startFrame, numberOfFrames, frameDuration, Color.White, 1.0f, false);
                        break;
                    }
                case AnimationTypes.gunjam:
                    {
                        startFrame = 2;
                        numberOfFrames = 1;
                        frameDuration = 480;
                        Texture2D playerTexture = withContext.Load<Texture2D>("gunslinger");
                        animationCreated.Initialize(playerTexture, offscreen,
                            _cellWidth, _cellHeight, startFrame, numberOfFrames, frameDuration, Color.White, 1.0f, false);
                        break;
                    }
                default:
                case AnimationTypes.idle:
                    {
                        startFrame = 3;
                        numberOfFrames = 2;
                        frameDuration = 480;
                        Texture2D playerTexture = withContext.Load<Texture2D>("gunslinger");
                        animationCreated.Initialize(playerTexture, offscreen,
                            _cellWidth, _cellHeight, startFrame, numberOfFrames, frameDuration, Color.White, 1.0f, true);
                        break;
                    }
                case AnimationTypes.death:
                    {
                        startFrame = 5;
                        numberOfFrames = 8;
                        frameDuration = 240;
                        Texture2D playerTexture = withContext.Load<Texture2D>("gunslinger");
                        animationCreated.Initialize(playerTexture, offscreen,
                            _cellWidth, _cellHeight, startFrame, numberOfFrames, frameDuration, Color.White, 1.0f, false);
                        break;
                    }
                case AnimationTypes.leftbutton:
                    {
                        startFrame = 0;
                        numberOfFrames = 2;
                        frameDuration = 240;
                        Texture2D buttonTexture = withContext.Load<Texture2D>("leftbuttonAnim");
                        animationCreated.Initialize(buttonTexture, offscreen,
                            _cellWidth, _cellHeight, startFrame, numberOfFrames, frameDuration, Color.White, 1.0f, true);
                        break;
                    }
                case AnimationTypes.rightbutton:
                    {
                        startFrame = 0;
                        numberOfFrames = 2;
                        frameDuration = 240;
                        Texture2D buttonTexture = withContext.Load<Texture2D>("rightbuttonAnim");
                        animationCreated.Initialize(buttonTexture, offscreen,
                            _cellWidth, _cellHeight, startFrame, numberOfFrames, frameDuration, Color.White, 1.0f, true);
                        break;
                    }
            }

            return animationCreated;

        }
    }
}
