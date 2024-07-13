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
            List<ChatInfo> chats = (List<ChatInfo>)connection.Query<ChatInfo>($"SELECT * FROM {table_name} WHERE username='{username}';");
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

    public void CreateChat(string username, string chat_name)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<ChatInfo> chats = (List<ChatInfo>)connection.Query<ChatInfo>($"SELECT * FROM {table_name} GROUP BY chatid;");
            HashSet<int> ids = new HashSet<int>();
            foreach(ChatInfo chat in chats)
            {
                ids.Add(chat.ChatId);
            }
            Random rnd = new Random();
            int id = rnd.Next(100, 100000);
            while(ids.Contains(id)) id = rnd.Next(100, 100000);
            NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO {table_name} (chatid, chat_name, username) VALUES ({id}, '{chat_name}', '{username}');", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void AddUserToChat(int id, string username, string chat_name)
    {

    }

    public void LeaveChat(int chatId, string username)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<ChatInfo> info = (List<ChatInfo>)connection.Query<ChatInfo>($"SELECT * FROM {table_name} WHERE ChatId={chatId};");
            NpgsqlCommand cmd = new NpgsqlCommand($"DELETE FROM {table_name} WHERE ChatId={chatId} AND Username='{username}';", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

}