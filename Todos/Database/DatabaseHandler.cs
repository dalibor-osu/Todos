using Microsoft.Data.Sqlite;
using Todos.Models;

namespace Todos.Database;

public class DatabaseHandler
{
    private const string ConnectionQuery = "Data Source=./Database/database.db";
    private SqliteConnection _connection;

    public DatabaseHandler()
    {
        _connection = new SqliteConnection(ConnectionQuery);
    }

    public DatabaseActionResult GetItems(ref List<TodoItem?> items, int filter = 0)
    {
        _connection.Open();

        string query = "SELECT * FROM todos" + (filter == 0 ? "" : $" WHERE state = {filter}");
        SqliteCommand command = CreateCommand(query);
        SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            TodoItem? item = GetItemFromReader(reader);
            items.Add(item);
        }

        _connection.Close();

        return DatabaseActionResult.Success;
    }

    public DatabaseActionResult GetSingleItem(Guid guid, ref TodoItem? item)
    {
        _connection.Open();
        
        if (!ItemExists(guid))
        {
            _connection.Close();
            return DatabaseActionResult.NotFound;
        }

        string query = $"SELECT * FROM todos WHERE id = \"{guid.ToString()}\"";
        SqliteCommand command = CreateCommand(query);
        SqliteDataReader reader = command.ExecuteReader();

        reader.Read();
        item = GetItemFromReader(reader);
        
        _connection.Close();
        
        return DatabaseActionResult.Success;
    }

    public DatabaseActionResult UpdateItem(TodoItem? item)
    {
        _connection.Open();
        
        if (item == null || !ItemExists(item.Id))
        {
            _connection.Close();
            return DatabaseActionResult.NotFound;
        }

        string query = $"UPDATE todos SET title = @title, state = @state, content = @content WHERE id = @id";
        SqliteCommand command = CreateCommand(query);
        PrepareStatement(item, ref command);
        command.ExecuteNonQuery();
        
        _connection.Close();

        return DatabaseActionResult.Success;
    }

    public DatabaseActionResult InsertNewItem(TodoItem item)
    {
        _connection.Open();

        if (ItemExists(item.Id))
        {
            _connection.Close();
            return DatabaseActionResult.AlreadyExists;
        }

        string query = $"INSERT INTO todos VALUES (@id, @title, @state, @content);";
        SqliteCommand command = CreateCommand(query);
        PrepareStatement(item, ref command);
        command.ExecuteNonQuery();
        
        _connection.Close();
        
        return DatabaseActionResult.Success;
    }

    public DatabaseActionResult DeleteItem(Guid id)
    {
        _connection.Open();
        
        if (!ItemExists(id))
        {
            _connection.Close();
            return DatabaseActionResult.NotFound;
        }

        string query = $"DELETE FROM todos WHERE id = \"{id}\";";
        SqliteCommand command = CreateCommand(query);
        command.ExecuteNonQuery();
        
        _connection.Close();
        
        return DatabaseActionResult.Success;
    }

    private SqliteCommand CreateCommand(string query)
    {
        SqliteCommand command = _connection.CreateCommand();
        command.CommandText = query;
        return command;
    }

    private TodoItem? GetItemFromReader(SqliteDataReader reader)
    {
        Guid id = reader.GetGuid(0);
        string title = reader.GetString(1);
        int state = reader.GetInt32(2);
        string content = reader.GetString(3);
        
        return new TodoItem(id, title, state, content);
    }
    private bool ItemExists(Guid id)
    {
        string query = $"SELECT COUNT(*) FROM todos WHERE id = \"{id}\";";
        SqliteCommand command = CreateCommand(query);
        int count = Convert.ToInt32(command.ExecuteScalar());

        return count > 0;
    }
    private void PrepareStatement(TodoItem? item, ref SqliteCommand command)
    {
        command.Parameters.AddWithValue("@id", item.Id.ToString());
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@state", item.State);
        command.Parameters.AddWithValue("@content", item.Content);
        command.Prepare();
    }
}