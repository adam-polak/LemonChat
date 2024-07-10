using LemonChat.DataAccessLibrary.Models;
using Npgsql;
using Dapper;

namespace LemonChat.DataAccessLibrary.Controllers;

public class UserController
{
    private string connectionString;
    private static string User_Table = Environment.GetEnvironmentVariable("USER_TABLE") ?? "";
    private string chat_table;
    private string? username;

    public UserController(IConfiguration configuration, string user)
    {
        connectionString = configuration.GetConnectionString("Default") ?? "";
        username = user;
        chat_table = $"{username}_chats";
        if(!ContainsUser(connectionString, username)) username = null;
    }

    public static bool ContainsUser(string connectionString, string username)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            User? user = connection.Query<User>($"SELECT * FROM {UserController.User_Table} WHERE username='{username}';").FirstOrDefault();
            connection.Close();
            return user != null;
        }
    }

    private static bool ContainsUserChatTable(string connectionString, string username)
    {
        using(NpgsqlConnection connection =  new NpgsqlConnection(connectionString))
        {
            connection.Open();
            TableName? table = connection.Query<TableName>($"SELECT table_name FROM {Environment.GetEnvironmentVariable("DATABASE_NAME") ?? ""}.INFORMATION_SCHEMA.TABLES WHERE table_type='BASE TABLE' AND table_name='{username}_chats';").FirstOrDefault();
            connection.Close();
            return table != null;
        }
    }

    private static void EnsureUserChatTable(string connectionString, string username)
    {
        bool deleteTable = ContainsUserChatTable(connectionString, username);
        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            NpgsqlCommand cmd;
            if(deleteTable)
            {
                cmd = new NpgsqlCommand($"DELETE FROM {username}_chats;", connection);
                cmd.ExecuteNonQuery();
            } else {
                cmd = new NpgsqlCommand($"CREATE TABLE {username}_chats (chatID INTEGER PRIMARY KEY);");
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public static bool CreateUser(string connectionString, string username, string password)
    {
        if(ContainsUser(connectionString, username)) return false;
        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            //create user in user table
            EnsureUserChatTable(connectionString, username);
            connection.Close();
            return true;
        }
    }
}