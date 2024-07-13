using Dapper;
using LemonChat.DataAccessLibrary.Models;
using Npgsql;

namespace LemonChat.DataAccessLibrary.Controllers;

public class ChatController
{

    private string _connection_string;
    private string table_name;

    public ChatController(string connectionString)
    {
        _connection_string = connectionString;
        table_name = Environment.GetEnvironmentVariable("CHAT_TABLE") ?? "CHAT_TABLE";
    }

    public void SendChat(int chatId, string username, string message)
    {
        int messageId = LastMessageId(chatId) + 1;
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            string sqlCommand = $"INSERT INTO {table_name} (ChatId, MessageId, Message, Sent_By, Date)"
                                + $" VALUES ({chatId}, {messageId}, '{message}', '{username}', '{DateTime.Now.ToShortDateString()}');";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlCommand, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    public List<Chat> GetChats(int chatId)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<Chat> chats = (List<Chat>)connection.Query<Chat>($"SELECT * FROM {table_name} WHERE chatid={chatId};");
            connection.Close();
            return chats;
        }
    }

    public void DeleteChat(int chatId, int messageId)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($"DELETE FROM {table_name} WHERE chatId={chatId} AND messageId={messageId};", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    private int LastMessageId(int chatId)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<Chat> chats = (List<Chat>)connection.Query<Chat>($"SELECT * FROM {table_name} WHERE chatId={chatId};");
            int largest = 0;
            foreach(Chat chat in chats)
            {
                largest = chat.MessageId > largest ? chat.MessageId : largest;
            }
            connection.Close();
            return largest;
        }
    }
    
}