using Dapper;
using LemonChat.DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Npgsql;

namespace LemonChat.DataAccessLibrary.Controllers;

public class ChatGroupController
{

    private string _connection_string;

    public ChatGroupController(string connectionString)
    {
        _connection_string = connectionString;
    }

    public void SendChat(int chatId, string username, string message)
    {
        int messageId = LastMessageId(chatId) + 1;
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            string sqlCommand = $"INSERT INTO {chatId}_group (ChatId, MessageId, Message, Sent_By, Date)"
                                + $" VALUES ({chatId}, {messageId}, '{message}', '{username}', '{DateTime.Now.ToShortDateString()}');";
            try {
                NpgsqlCommand cmd = new NpgsqlCommand(sqlCommand, connection);
                cmd.ExecuteNonQuery();
            } catch(Exception e) {
                Console.WriteLine("Table doesn't exist");
            }
            connection.Close();
        }
    }

    private int LastMessageId(int chatId)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            List<ChatGroup> chats;
            try
            {
                chats = (List<ChatGroup>)connection.Query<ChatGroup>($"SELECT * FROM chat{chatId}_group;");
            } catch(Exception e) {
                chats = [];
                Console.WriteLine("Table doesn't exist");
            }
            int largest = 0;
            foreach(ChatGroup chat in chats)
            {
                largest = chat.ChatId > largest ? chat.ChatId : largest;
            }
            connection.Close();
            return largest;
        }
    }
    
}