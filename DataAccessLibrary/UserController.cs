using LemonChat.DataAccessLibrary.Models;
using Npgsql;
using Dapper;

namespace LemonChat.DataAccessLibrary.Controllers;

public class UserController
{
    private string connectionString;
    private static string User_Table = Environment.GetEnvironmentVariable("USER_TABLE") ?? "";
    private static string User_InChat_Table = Environment.GetEnvironmentVariable("USER_INCHAT_TABLE") ?? "";
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

    private static bool IsUserInChat(string connectionString, string username, int chatId)
    {
        using(NpgsqlConnection connection =  new NpgsqlConnection(connectionString))
        {
            connection.Open();
            TableName? table = connection.Query<TableName>($"SELECT * FROM {User_InChat_Table} WHERE chatID={chatId} AND user='{username}';").FirstOrDefault();
            connection.Close();
            return table != null;
        }
    }

    public static bool CreateUser(string connectionString, string username, string password)
    {
        if(ContainsUser(connectionString, username)) return false;
        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string sqlStatement = $"INSERT INTO {User_Table} (username, password, session_key) VALUES ('{username}', '{password}', 0);";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlStatement, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }
    }

    public static bool IsCorrectLogin(string connectionString, string username, string password)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            User? user = connection.Query<User>($"SELECT * FROM {User_Table} WHERE username='{username}' AND password='{password}';").FirstOrDefault();
            connection.Close();
            return user != null;
        }
    }

    public static bool IsCorrectLogin(string connectionString, string username, int session_key)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            User? user = connection.Query<User>($"SELECT * FROM {User_Table} WHERE username='{username}' AND session_key={session_key};").FirstOrDefault();
            if(user != null)
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE {User_Table} SET session_key=0 WHERE username='{username}';", connection);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            return user != null;
        }
    }

    public static int CreateSessionKey(string connectionString, string username, string password)
    {
        if(!IsCorrectLogin(connectionString, username, password)) return -1;
        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            int session_key = new Random().Next(100, 1000);
            NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE {User_Table} SET session_key={session_key} WHERE username='{username}';", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return session_key;
        }
    }
}