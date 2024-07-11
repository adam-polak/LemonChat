namespace LemonChat.DataAccessLibrary.Models;

public class Chat
{
    public int ChatId { get; set; }
    public int MessageId { get; set; }
    public required string Message { get; set; }
    public required string Sent_By { get; set; }
    public required string Date { get; set; }
}