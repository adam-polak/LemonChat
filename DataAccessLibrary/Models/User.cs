namespace LemonChat.DataAccessLibrary.Models;

public class User
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public int Session_Key { get; set; }
}