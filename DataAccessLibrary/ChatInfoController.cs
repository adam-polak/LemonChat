namespace LemonChat.DataAccessLibrary.Controllers;

public class ChatInfoController
{
    private string table_name;
    private string username;
    private UserController userController;

    public ChatInfoController(string user)
    {
        table_name = Environment.GetEnvironmentVariable("CHATINFO_TABLE") ?? "";
        username = user;
        userController = new UserController(username);
    }
}