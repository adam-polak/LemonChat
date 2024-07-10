namespace LemonChat.DataAccessLibrary.Models;

public class Chat
{
    public int ChatID { get; set; }
    public int MessageID { get; set; }
    public required string Message { get; set; }
    public required string Sent_By { get; set; }
    public required string Date { get; set; }
}