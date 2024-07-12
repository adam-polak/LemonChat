using Dapper;
using LemonChat.DataAccessLibrary.Models;
using Npgsql;

namespace LemonChat.DataAccessLibrary.Controllers;

public class ChatInfoController
{
    private string table_name;
    private string _connection_string;

    public ChatInfoController(string connectionString)
    {
        table_name = Environment.GetEnvironmentVariable("CHATINFO_TABLE") ?? "ChatInfo_Table";
        _connection_string = connectionString;
    }

    public List<ChatInfo> ChatsUserIsIn(string username)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<ChatInfo> chats = (List<ChatInfo>)connection.Query<ChatInfo>($"SELECT * FROM {table_name};");
            connection.Close();
            return chats;
        }
    }

    public List<string> UsersInChat(int id)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<ChatInfo> info = (List<ChatInfo>)connection.Query<ChatInfo>($"SELECT * FROM {table_name} WHERE chatid={id};");
            connection.Close();
            List<string> users = [];
            foreach(ChatInfo chat in info) users.Add(chat.Username);
            return users;
        }
    }
}