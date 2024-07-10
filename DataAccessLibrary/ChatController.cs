namespace LemonChat.DataAccessLibrary.Controllers;

public class ChatController
{
    private string table_name;

    public ChatController(int id)
    {
        table_name = $"{id}_group";
    }
    
}