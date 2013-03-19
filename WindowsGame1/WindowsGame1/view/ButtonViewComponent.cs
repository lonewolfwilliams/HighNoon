using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1.view
{
    class ButtonViewComponent
    {
        Animation buttonAnimation;
        
        public bool ShowPressAnimation = false;

        Vector2 position;
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

        public void Initialise(AnimationFactory factory, bool isLeftButton)
        {
            if (isLeftButton)
            {
                this.buttonAnimation = factory.create(AnimationTypes.leftbutton);
            }
            else
            {
                this.buttonAnimation = factory.create(AnimationTypes.rightbutton);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (false == ShowPressAnimation)
            {
                buttonAnimation.currentFrame = 1;
            }
            buttonAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            buttonAnimation.Position = position;
            buttonAnimation.Draw(spritebatch, false);
        }
    }
}
