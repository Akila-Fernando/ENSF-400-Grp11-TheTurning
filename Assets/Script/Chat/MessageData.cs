using System;
using System.Collections.Generic;

[Serializable]
public class Message
{
    public string sender;
    public string text;
}

[Serializable]
public class MessageList
{
    public List<Message> messages;
}