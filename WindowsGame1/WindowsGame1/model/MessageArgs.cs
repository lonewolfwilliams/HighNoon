using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1.model
{
    public class MessageArgs : EventArgs
    {
        public string token = string.Empty;
        public object sender;
        public object message;
    }
}
