using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class StateEventArgs: EventArgs
    {
        public StateMachine.GameState prev;
        public StateMachine.GameState next;
    }
}
