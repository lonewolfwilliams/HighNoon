using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindowsGame1.model;

namespace WindowsGame1
{
    class GameOverController : IController
    {
        string gameOverText = string.Empty;
        SpriteFont font;

        KeyboardState previousKeyboardState;

        public GameOverController(SpriteFont font)
        {
            this.font = font;
            GameContext.messageBus.OnGenericMessage += messageBus_OnGenericMessage;
        }

        void messageBus_OnGenericMessage(model.MessageArgs msg)
        {
            if (msg.token != "gameOver")
            {
                return;
            }

            //no winner
            if (msg.message == null)
            {
                gameOverText = "Players draw\nspace to continue";
                return;
            }

            //winner
            PlayerModel model = (msg.message as PlayerModel);

            /*
            if (model.playerNumber == 0)
            {
                gameOverText = "Left Player wins\npress any key";
            }
            else
            {
                gameOverText = "Right Player wins\npress any key";
            }
            */

            var score = GameContext.scoreModel;
            gameOverText = string.Format(OnscreenText.GameOverText, score.leftPlayerScore, score.rightPlayerScore);

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
                    GameContext.gameStateMachine.State = StateMachine.GameState.title_screen;
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
            spritebatch.DrawString(font, gameOverText, new Vector2(10, 10), Color.White);
        }
    }
}
