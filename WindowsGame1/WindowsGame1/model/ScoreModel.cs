using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1.model
{
    public class ScoreModel
    {
        public int leftPlayerScore = 0;
        public int rightPlayerScore = 0;

        public ScoreModel()
        {
            GameContext.messageBus.OnGenericMessage += messageBus_OnGenericMessage;
        }

        void messageBus_OnGenericMessage(MessageArgs args)
        {
            if(args.token != "gameOver")
            {
                return;
            }

            PlayerModel winner = args.message as PlayerModel;

            if (winner == null) //draw
            {
                return;
            }
            if (winner.playerNumber == 0)
            {
                leftPlayerScore++;
            }
            else
            {
                rightPlayerScore++;
            }
        }

        public void ResetScore()
        {
            leftPlayerScore = 0;
            rightPlayerScore = 0;
        }
    }
}
