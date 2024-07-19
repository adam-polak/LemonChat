using Dapper;
using LemonChat.DataAccessLibrary.Models;
using Npgsql;

namespace LemonChat.DataAccessLibrary.Controllers;

public class ChatInfoController
{
    private string table_name;
    private string user_inchat_table;
    private string _connection_string;

    public ChatInfoController(string connectionString)
    {
        table_name = Environment.GetEnvironmentVariable("CHATINFO_TABLE") ?? "ChatInfo_Table";
        user_inchat_table = Environment.GetEnvironmentVariable("USER_INCHAT_TABLE") ?? "USER_INCHAT_TABLE";
        _connection_string = connectionString;
    }

    public List<UserInChat> ChatsUserIsIn(string username)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<UserInChat> chats = (List<UserInChat>)connection.Query<UserInChat>($"SELECT * FROM {user_inchat_table} WHERE username='{username}';");
            connection.Close();
            return chats;
        }
    }

    public List<string> UsersInChat(int id)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<UserInChat> usersInChat = (List<UserInChat>)connection.Query<UserInChat>($"SELECT * FROM {user_inchat_table} WHERE chatid={id};");
            connection.Close();
            List<string> users = [];
            foreach(UserInChat user in usersInChat) users.Add(user.Username);
            return users;
        }
    }

    public void CreateChat(string username, string chat_name)
    {
        Random rnd = new Random();
        int id = rnd.Next(100, 100000);
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<ChatInfo> chats = (List<ChatInfo>)connection.Query<ChatInfo>($"SELECT * FROM {table_name} GROUP BY chatid;");
            HashSet<int> ids = new HashSet<int>();
            foreach(ChatInfo chat in chats)
            {
                ids.Add(chat.ChatId);
            }
            while(ids.Contains(id)) id = rnd.Next(100, 100000);
            NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO {table_name} (chatid, chat_name, username) VALUES ({id}, '{chat_name}', '{username}');", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        AddUserToChat(id, username);
    }

    public bool IsUserInChat(int id, string username)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            UserInChat? user = connection.Query<UserInChat>($"SELECT * FROM {user_inchat_table} WHERE chatid={id} AND username='{username}';").FirstOrDefault();
            connection.Close();
            return user != null;
        }
    }

    public void AddUserToChat(int id, string username)
    {
        if(IsUserInChat(id, username)) return;
        string chat_name = GetChatName(id);
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO {user_inchat_table} (chatid, chat_name, username) VALUES ({id}, '{chat_name}', '{username}');", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    private string GetChatName(int id)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            ChatInfo? info = connection.Query<ChatInfo>($"SELECT * FROM {table_name} WHERE chatid={id};").FirstOrDefault();
            connection.Close();
            return info != null ? info.Chat_Name : "";
        }
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