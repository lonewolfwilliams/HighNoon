using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1.model
{
    public class MessageBus
    {
        MessageArgs m_message = new MessageArgs();
        public delegate void MessageHandler(MessageArgs args);
        public event MessageHandler OnGenericMessage;
        public void DispatchMessage(object sender, string token, object payload)
        {
            if (OnGenericMessage != null)
            {
                m_message.sender = sender;
                m_message.token = token;
                m_message.message = payload;
                OnGenericMessage(m_message);
            }
        }
    }
}
