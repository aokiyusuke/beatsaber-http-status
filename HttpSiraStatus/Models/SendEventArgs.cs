﻿using HttpSiraStatus.Util;

namespace HttpSiraStatus
{
    public class SendEventArgs
    {
        public JSONNode JsonNode { get; private set; }
        public SendEventArgs(JSONNode node)
        {
            this.JsonNode = node;
        }
    }

    public delegate void SendEventHandler(object sender, SendEventArgs e);
}
