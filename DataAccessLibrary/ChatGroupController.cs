namespace LemonChat.DataAccessLibrary.Controllers;

public class ChatGroupController
{
    private string table_name;

    public ChatGroupController(int id)
    {
        table_name = $"{id}_group";
    }
    
}