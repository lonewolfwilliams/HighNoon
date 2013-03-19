using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindowsGame1.model;
using XNATweener;

namespace WindowsGame1
{
    class TitleController : IController
    {
        string title = OnscreenText.TitleText;
        SpriteFont font;
        
        KeyboardState previousKeyboardState;

        public TitleController(SpriteFont font)
        {
            this.font = font;
        }

        public void Initialise()
        {
            //stub...
        }

        public void processKeyboardInput(Microsoft.Xna.Framework.Input.KeyboardState currentKeyboardState)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (previousKeyboardState.IsKeyDown(Keys.Space) !=
                    currentKeyboardState.IsKeyDown(Keys.Space))
                {
                    GameContext.gameStateMachine.State = StateMachine.GameState.lobby;
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                if (previousKeyboardState.IsKeyDown(Keys.R) !=
                    currentKeyboardState.IsKeyDown(Keys.R))
                {
                    GameContext.scoreModel.ResetScore();
                }
            }

            previousKeyboardState = currentKeyboardState;
        }

        public void update(Microsoft.Xna.Framework.GameTime withGameTime)
        {
            //stub...
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch)
        {
            spritebatch.DrawString(font, title, new Vector2(10, 10), Color.White);
        }
    }
}
