public class EventChatRoom
{
    public string RoomName { get; }

    public event Action<string>? OnMessage;

    public EventChatRoom(string roomName)
    {
        RoomName = roomName;
    }

    public void SendMessage(string sender, string message)
    {
        string fullMessage = $"[{RoomName}] {sender}: {message}";
        if (OnMessage != null)
        {
            OnMessage.Invoke(fullMessage);
        }
        else
        {
            Console.WriteLine($"Няма абонати – съобщението се губи!");
        }
    }
}
