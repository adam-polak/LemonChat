namespace LemonChat.DataAccessLibrary.Controllers;

public class UserController
{
    private string user_table;
    private string chat_table;
    private string username;

    public UserController(string user)
    {
        user_table = Environment.GetEnvironmentVariable("USER_TABLE") ?? "";
        username = user;
        chat_table = $"{username}_chats";
    }
}