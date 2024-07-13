using LemonChat.DataAccessLibrary.Models;
using Npgsql;
using Dapper;

namespace LemonChat.DataAccessLibrary.Controllers;

public class UserController
{
    
    private string table_name;
    private string _connection_string;

    public UserController(string connectionString)
    {
        table_name = Environment.GetEnvironmentVariable("USER_TABLE") ?? "user_table";
        _connection_string = connectionString;
    }

    public bool ContainsUser(string username)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            User? user = connection.Query<User>($"SELECT * FROM {table_name} WHERE username='{username}';").FirstOrDefault();
            connection.Close();
            return user != null;
        }
    }

    public bool CreateUser(string username, string password)
    {
        if(ContainsUser(username)) return false;
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            string sqlStatement = $"INSERT INTO {table_name} (username, password, session_key) VALUES ('{username}', '{password}', 0);";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlStatement, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }
    }

    public bool IsCorrectLogin(string username, string password)
    {
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            User? user = connection.Query<User>($"SELECT * FROM {table_name} WHERE username='{username}' AND password='{password}';").FirstOrDefault();
            connection.Close();
            return user != null;
        }
    }

    public bool IsCorrectLogin(string username, int session_key)
    {
        if(session_key < 1000) return false;
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            User? user = connection.Query<User>($"SELECT * FROM {table_name} WHERE username='{username}' AND session_key={session_key};").FirstOrDefault();
            connection.Close();
            return user != null;
        }
    }

    public int CreateSessionKey(string username, string password)
    {
        if(!IsCorrectLogin(username, password)) return -1;
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            int session_key = new Random().Next(1000, 10000);
            NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE {table_name} SET session_key={session_key} WHERE username='{username}';", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return session_key;
        }
    }

    public void KillSessionKey(string username)
    {
        if(!ContainsUser(username)) return;
        using(NpgsqlConnection connection = new NpgsqlConnection(_connection_string))
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE {table_name} SET session_key=0 WHERE username='{username}';", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

}