using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using WindowsGame1.model;

namespace WindowsGame1
{
    class GameController : IController
    {
        KeyboardState previousKeyboardState;

        Vector2 textPosition;
        string instruction = OnscreenText.ReadyText;
        SpriteFont font;

        List<PlayerModel> models;
        List<GunslingerViewComponent> views;

        Random rand = new Random();
        int timeUntilHighNoonInMS = 0;
        bool _isHighNoon = false;
        bool flagDeathAnimationCompleted = false;

        public GameController(SpriteFont font)
        {
            this.font = font;
            textPosition = new Vector2((GameContext.width / GameContext.scaleFactor / 2) - 26, 10);
            models = new List<PlayerModel>();
            views  = new List<GunslingerViewComponent>();
        }

        //returns an index to identify the model you can bind the view to this
        public int addModel(PlayerModel m)
        {
            models.Add(m);
            return models.Count - 1;
        }
        public void addViewComponent(GunslingerViewComponent vc)
        {
            views.Add(vc);
            vc.DeathAnimationCompleted += vc_DeathAnimationCompleted;
        }

        void vc_DeathAnimationCompleted(object sender, EventArgs e)
        {
            flagDeathAnimationCompleted = true;
        }

        public bool isHighNoon
        {
            get { return _isHighNoon; }
        }

        #region IController
        public void Initialise()
        {
            instruction = OnscreenText.ReadyText;
            timeUntilHighNoonInMS = rand.Next(6 * 1000); // verbose variable names
            _isHighNoon = false;
            flagDeathAnimationCompleted = false;
            foreach (PlayerModel m in models)
            {
                m.Initialise();
            }
            foreach (GunslingerViewComponent vc in views)
            {
                vc.doIdleAnimation();
            }
        }

        public void processKeyboardInput(KeyboardState currentKeyboardState)
        {
            if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (currentKeyboardState.IsKeyDown(Keys.LeftShift) != previousKeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    foreach (PlayerModel m in models) m.processPlayerInput(0);
                }
            }
            if (currentKeyboardState.IsKeyDown(Keys.RightShift))
            {
                if (currentKeyboardState.IsKeyDown(Keys.RightShift) != previousKeyboardState.IsKeyDown(Keys.RightShift))
                {
                    foreach (PlayerModel m in models) m.processPlayerInput(1);
                }
            }

            previousKeyboardState = currentKeyboardState;
        }

        public void update(GameTime withGameTime)
        {
            int elapsedTime = withGameTime.ElapsedGameTime.Milliseconds;
            timeUntilHighNoonInMS -= elapsedTime;

            //OMG it's highnooon drawdrawdraw be quick or be dead!
            if (timeUntilHighNoonInMS < 0 && false == _isHighNoon)
            {
                instruction = OnscreenText.DrawText;
                _isHighNoon = true;
            }
            //update model (not most efficient way but most clear way - Verbosity!
            PlayerModel winner = null;
            foreach (PlayerModel m in models)
            {
                if (m.isShooting && m.isAlive)
                {
                    if (false == _isHighNoon)
                    {
                        m.isGunJammed = true;
                    }
                    else
                    {
                        winner = m;
                    }
                }
            }

            //it's a draw...
            if (models[0].isGunJammed && models[1].isGunJammed)
            {
                GameContext.messageBus.DispatchMessage(this, "gameOver", null);
                GameContext.gameStateMachine.State = StateMachine.GameState.game_over;
            }

            //there's a winner
            foreach (PlayerModel m in models)
            {
                if (winner == null) break;

                if (m == winner)
                {
                    m.isAlive = true;
                }
                else
                {
                    m.isAlive = false;
                }

                if (flagDeathAnimationCompleted && 
                    GameContext.gameStateMachine.State != StateMachine.GameState.game_over) //avoid retrigger in player loop
                {
                    GameContext.messageBus.DispatchMessage(this, "gameOver", winner);
                    GameContext.gameStateMachine.State = StateMachine.GameState.game_over;
                }
            }

            //update view components from model
            foreach (GunslingerViewComponent vc in views)
            {
                PlayerModel m = models.ElementAt(vc.getBinding());

                if (m.isAlive)
                {
                    //these may not stay mutually exclusive - hence no state machine
                    if (m.isShooting)
                    {
                        vc.doShootingAnimation();
                    }
                    
                    if (m.isGunJammed)
                    {
                        vc.doGunjamAnimation();
                    }
                    
                    if(false == m.isShooting && false == m.isGunJammed)
                    {
                        vc.doIdleAnimation();
                    }
                }
                else
                {
                   vc.doDeathAnimation();
                }

                //update the animation
                vc.update(withGameTime);
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            foreach (GunslingerViewComponent vc in views)
            {
                vc.Draw(spritebatch);
            }

            spritebatch.DrawString(font, instruction, textPosition, Color.White);
        }

        #endregion
    }
}
