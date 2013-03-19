using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Gareth Williams
 * 
 * states for the overall game
 */

namespace WindowsGame1
{
    public class StateMachine
    {
        public enum GameState
        {
            title_screen,
            lobby,
            game_on,
            game_over,
        }

        StateEventArgs args = new StateEventArgs();
        public delegate void StateChangedHandler(object sender, StateEventArgs args);
        public event StateChangedHandler StateChanged;

        GameState m_state = GameState.title_screen;
        public GameState State
        {
            set
            {
                GameState prev = m_state;
                if (prev == value)
                {
                    return;
                }

                m_state = value;

                if (StateChanged != null)
                {
                    args.prev = prev;
                    args.next = m_state;
                    StateChanged(this, args);
                }
            }
            get
            {
                return m_state;
            }
        }
    }
}
