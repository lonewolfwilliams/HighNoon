using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindowsGame1.view;
using XNATweener;

namespace WindowsGame1
{
    class LobbyController : IController
    {
        KeyboardState previousKeyboardState;

        bool leftPlayerJoined = false;
        bool rightPlayerJoined = false;
        bool leftPlayerTweenComplete = false;
        bool rightPlayerTweenComplete = false;
        bool buttonTweenComplete = false;
        
        GunslingerViewComponent leftPlayer;
        GunslingerViewComponent rightPlayer;
        ButtonViewComponent leftButton;
        ButtonViewComponent rightButton;

        Tweener leftPlayerDropOntoScreen;
        Tweener rightPlayerDropOntoScreen;
        Tweener buttonsDropOffScreen;

        public LobbyController( GunslingerViewComponent leftPlayer, GunslingerViewComponent rightPlayer, 
                                ButtonViewComponent leftButton, ButtonViewComponent rightButton)
        {
            this.leftPlayer = leftPlayer;
            this.rightPlayer = rightPlayer;
            this.leftButton = leftButton;
            this.rightButton = rightButton;
        }

        #region IController

        public void Initialise()
        {
            leftPlayerJoined = false;
            rightPlayerJoined = false;
            leftPlayerTweenComplete = false;
            rightPlayerTweenComplete = false;
            buttonTweenComplete = false;

            float vCenter = (GameContext.height / GameContext.scaleFactor / 2) + 16;
            leftPlayer.Y = -16;
            leftPlayerDropOntoScreen = new Tweener(-16, vCenter, TimeSpan.FromSeconds(1), Bounce.EaseOut);
            leftPlayerDropOntoScreen.Stop();
            leftPlayerDropOntoScreen.Ended += leftPlayerDropOntoScreen.Stop;

            rightPlayer.Y = -16;
            rightPlayerDropOntoScreen = new Tweener(-16, vCenter, TimeSpan.FromSeconds(1), Bounce.EaseOut);
            rightPlayerDropOntoScreen.Stop();
            rightPlayerDropOntoScreen.Ended += rightPlayerDropOntoScreen.Stop;

            buttonsDropOffScreen = new Tweener((GameContext.height / GameContext.scaleFactor / 2) + 44,
                                                GameContext.height / GameContext.scaleFactor + 16,
                                                TimeSpan.FromSeconds(0.25), Quadratic.EaseOut);
            buttonsDropOffScreen.Stop();
            buttonsDropOffScreen.Ended += buttonsDropOffScreen_Ended;

            leftPlayer.doIdleAnimation();
            rightPlayer.doIdleAnimation();
        }

        public void processKeyboardInput(Microsoft.Xna.Framework.Input.KeyboardState currentKeyboardState)
        {
            if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (currentKeyboardState.IsKeyDown(Keys.LeftShift) != previousKeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    leftPlayerJoined = true;  
                }
            }
            if (currentKeyboardState.IsKeyDown(Keys.RightShift))
            {
                if (currentKeyboardState.IsKeyDown(Keys.RightShift) != previousKeyboardState.IsKeyDown(Keys.RightShift))
                {
                    rightPlayerJoined = true;
                }
            }

            previousKeyboardState = currentKeyboardState;
        }

        public void update(Microsoft.Xna.Framework.GameTime withGameTime)
        {
            leftPlayer.update(withGameTime);
            rightPlayer.update(withGameTime);
            leftButton.Update(withGameTime);
            rightButton.Update(withGameTime);

            leftButton.ShowPressAnimation = !leftPlayerJoined;
            rightButton.ShowPressAnimation = !rightPlayerJoined;

            //animate player avatars onto screen
            if (leftPlayerJoined && false == leftPlayerDropOntoScreen.Running)
            {
                leftPlayerDropOntoScreen.Ended += leftPlayerDropOntoScreen_Ended;
                leftPlayerDropOntoScreen.Start();
            }
            
            leftPlayerDropOntoScreen.Update(withGameTime);
            leftPlayer.Y = leftPlayerDropOntoScreen.Position;

            if (rightPlayerJoined && false == rightPlayerDropOntoScreen.Running)
            {
                rightPlayerDropOntoScreen.Ended += rightPlayerDropOntoScreen_Ended;
                rightPlayerDropOntoScreen.Start();
            }
            
            rightPlayerDropOntoScreen.Update(withGameTime);
            rightPlayer.Y = rightPlayerDropOntoScreen.Position;

            if (leftPlayerTweenComplete && 
                rightPlayerTweenComplete &&
                false == buttonsDropOffScreen.Running)
            {
                buttonsDropOffScreen.Ended += buttonsDropOffScreen_Ended;
                buttonsDropOffScreen.Start();
            }

            buttonsDropOffScreen.Update(withGameTime);
            leftButton.Y = buttonsDropOffScreen.Position;
            rightButton.Y = buttonsDropOffScreen.Position;

            if (buttonTweenComplete)
            {
                buttonsDropOffScreen.Ended -= buttonsDropOffScreen_Ended;
                GameContext.gameStateMachine.State = StateMachine.GameState.game_on;
            }
        }

        void rightPlayerDropOntoScreen_Ended()
        {
            rightPlayerDropOntoScreen.Ended -= rightPlayerDropOntoScreen_Ended;
            leftPlayerTweenComplete = true;
        }

        void leftPlayerDropOntoScreen_Ended()
        {
            leftPlayerDropOntoScreen.Ended -= leftPlayerDropOntoScreen_Ended;
            rightPlayerTweenComplete = true;
        }

        void buttonsDropOffScreen_Ended()
        {
            buttonTweenComplete = true;
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch)
        {
            if (GameContext.gameStateMachine.State != StateMachine.GameState.lobby)
            {
                return;
            }

            leftPlayer.Draw(spritebatch);
            rightPlayer.Draw(spritebatch);
            leftButton.Draw(spritebatch);
            rightButton.Draw(spritebatch);
        }

        #endregion
    }
}
